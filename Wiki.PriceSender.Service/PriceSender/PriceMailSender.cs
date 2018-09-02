using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using CarParts.Common;
using CarParts.Common.Log;
using Wiki.PriceSender.Dto;
using Wiki.PriceSender.Service.Models;
using Wiki.PriceSender.Service.Models.PriceList;
using Wiki.Service.Configuration;

namespace Wiki.PriceSender.Service.PriceSender
{
    public class PriceMailSender
    {
        private const string ArhiveDirKey = "arhiveDir";
        private readonly int _configId;
        private SchedulerItem _config;
        private static TimeSpan _offsetLastSend = new TimeSpan(0, 4, 0);


        private static TaskQueue _queue = new TaskQueue(10);

        private FileLogger _logger = new FileLogger("PriceMailSender");


        private SendPriceEvent _sendEvent;
        private SchedulerRepository _schedulerRepository;

        public PriceMailSender(int configId, SchedulerRepository schedulerRepository)
        {
            this._config = this._schedulerRepository.GetScheduler(configId);
            this._configId = configId;
            this._schedulerRepository = schedulerRepository;
        }

        private string Key { get { return "PriceMailSender_" + this._configId; } }

        public PriceMailSender(SchedulerItem config, SchedulerRepository schedulerRepository)
        {
            this._config = config;
            this._configId = config.Id;
            this._schedulerRepository = schedulerRepository;
        }

        public bool Send()
        {

            var task = _queue.AddTask(this.Key, SendInternall);

            if (task != null)
                this._logger.WriteEvent(
                    string.Format("Add task SendPrice. key:{0}, total:{1} wait:{2} active:{3}", this.Key, _queue.TotalTaskCount, _queue.WaitTasksCount, _queue.ActiveTaskCount));

            return task != null;
        }


        private void SendInternall()
        {
            try
            {
                this._sendEvent = new SendPriceEvent(_config.Id, _config.GetNextTime(_config.NextSend + _offsetLastSend), _config.Email, _config.ClientId, _config.GroupId);

                this._sendEvent["fileName"] = _config.FileName;

                //создание прайса в виде массива байт
                var pr = CreatePrice();

                if (string.IsNullOrWhiteSpace(this.Config.Email) || this.Config.Email.Trim().Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Where(t => !string.IsNullOrWhiteSpace(t)).ToArray().Length == 0)
                {
                    SenderSrv.Logger.WriteEvent(string.Format("SendInternall not sended. To is empty. Id:{0}, FileName:{1}", this.Config.Id, this.Config.FileName));
                    return;
                }

                var mailSetting =
                    this._schedulerRepository
                        .GetEmailSettingsForMailingPricesService()
                        .FirstOrDefault(x => x.Id == this.Config.ProfileId);

                this._sendEvent["emailFrom"] = mailSetting.Email;

                //new MailSender().SendEmail(
                //    this.Config.Email,
                //    this.Config.Subject ?? "price",
                //    this.Config.Body ?? "", false, pr, this.Config.FileName, mailSetting);

                ServiceFactory.GetPriceSchedulerRepository().UpdateLastSend(this.Config.ClientId);
                SaveFile(pr);

                this._logger.WriteEvent(
                    string.Format("End task SendPrice. key:{0}, total:{1} wait:{2} active:{3}",
                        this.Key, _queue.TotalTaskCount, _queue.WaitTasksCount, _queue.ActiveTaskCount));


                this._schedulerRepository.SaveSendEvent(this._sendEvent);
            }
            catch (Exception er)
            {
                this._logger.WriteError("Error send price. Id:" + this.Config.Id, er);
                var evn=new PriceSendErrorEvent(_config.Id, er.Message, er, _config.Email, _config.ClientId, _config.GroupId);

                this._schedulerRepository.SaveSendError(evn);
            }

        }

        public string SaveFile(byte[] pr)
        {
            var arhiveFile = Path.Combine(ArhiveDir, FileName + ".zip");
            this._sendEvent["filePath"] = arhiveFile;
            using (FileStream zipToOpen = new FileStream(arhiveFile, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry readmeEntry = archive.CreateEntry(this.Config.FileName);
                    var stream = readmeEntry.Open();
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write(pr);
                    }
                }
            }

            return arhiveFile;
        }

        public byte[] CreatePrice()
        {

            var sw = Stopwatch.StartNew();
            //создание прайса из SchedulerItem
            var creator = new PriceCreator(this.Config);
            //прайс лист для отправки
            var items = this._schedulerRepository.GetPriceItems(this.Config.ClientId,this.Config.GroupId);

            var readDate = sw.ElapsedMilliseconds;
            sw.Restart();

            //прайс в виде byte[]
            var pr = creator.CreatePrice(items);
            this._logger.WriteEvent("Generate price. ClientId:{0}. ReadDate/generate file {1}/{2}ms.", this._configId, readDate, sw.ElapsedMilliseconds);

            return pr;
        }

        public SchedulerItem Config
        {
            get
            {
                return this._config ??
                       (this._config = this._schedulerRepository.GetScheduler(this._configId));
            }
        }

        private string ArhiveDir
        {
            get
            {
                var clientId = this._configId;
                var dir = GetSchedulerConfigDir(clientId);
                return dir;
            }
        }

        private static string GetBaseArchiveDir
        {
            get
            {
                var baseDir = ConfigurationContainer.Configuration[ArhiveDirKey];
                if (string.IsNullOrEmpty(baseDir))
                {
                    baseDir = Path.Combine(Environment.CurrentDirectory, "Arhives");
                    ConfigurationContainer.Configuration[ArhiveDirKey] = baseDir;
                    ConfigurationContainer.Configuration.Save();
                }
                return baseDir;
            }
        }

        public static string GetSchedulerConfigDir(int configId)
        {

            var dir = Path.Combine(GetBaseArchiveDir,"SendedPrice", configId.ToString());
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
        }

        public static ArhiveFile[] GetArhives(int configId)
        {
            var dir = GetSchedulerConfigDir(configId);
            var files = Directory.GetFiles(dir);
            var arhives = files.Select(x => new ArhiveFile(x)).ToArray();
            return arhives;

        }

        public static IEnumerable<ArhiveFile> GetAllArhives()
        {
            foreach (var configIdDir in Directory.GetDirectories(GetBaseArchiveDir).Select(o => o.Split(new[] { '\\' }).Last()))
            {
                int configId = 0;
                try
                {
                    configId = int.Parse(configIdDir);
                }
                catch
                {
                    continue;
                }
                var dir = GetSchedulerConfigDir(configId);
                var files = Directory.GetFiles(dir);
                var arhives = files.Select(x => new ArhiveFile(x)).ToArray();
                foreach (var arhive in arhives)
                {
                    arhive.ConfigId = configId;
                    yield return arhive;
                }
            }
        }

        public string FileName
        {
            get
            {
                var file = new ArhiveFile
                {
                    Date = DateTime.Now,
                    FileType = this.Config.FileType,
                    FileName = this.Config.FileName,
                    Email = this.Config.Email
                };

                return file.ToString();
            }
        }

    }
}