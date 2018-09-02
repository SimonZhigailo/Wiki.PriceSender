using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using CarParts.Data.Componets;
using Wiki.PriceSender.Dto;

namespace Wiki.PriceSender.Service.PriceSender
{
    public class SchedulerRepository
    {
        private const string AddEventItem = "INSERT INTO [EventItems]([id],[EventId],[Description],[Start],[Stop]) VALUES (NEWID(),@eId,@descr,@start,@stop)";
        private readonly string _connectionString;

        public SchedulerRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        private LafSqlCommand GetCommand(string cmd)
        {
            return new LafSqlCommand(cmd, new SqlConnection(this._connectionString));

        }


        public SchedulerItem GetScheduler(int id)
        {
            return GetSchedulers(id).FirstOrDefault(x => x.Id == id);
        }

        public List<SchedulerItem> GetSchedulersByGroupId(int groupId)
        {
            var query = "select * from PriceGroupToClient where PriceGroupId = " + groupId;
            var result = new List<SchedulerItem>();

            using (var cmd = GetCommand(query))
            {
                cmd.ExecuteReader(r =>
                {
                    var model = CreatePriceSendModel(r);
                    //model.EmailSetting = emails.First(o => o.Id == model.ProfileId);
                    result.Add(model);
                });

            }
            return result;
        }

        //вернуть scheduler или лист всех, если id = 0
        public List<SchedulerItem> GetSchedulers(int id = 0)
        {
            //var emails = GetEmailSettingsForMailingPricesService();

            using (var cmd = GetCommand("PriceSender_GetSchedulers"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("id", id);
                var result = new List<SchedulerItem>();
                cmd.ExecuteReader(r =>
                {
                    var model = CreatePriceSendModel(r);
                    //model.EmailSetting = emails.First(o => o.Id == model.ProfileId);
                    result.Add(model);
                });
                return result;
            }

        }

        private static SchedulerItem CreatePriceSendModel(DbDataReader r)
        {
            var model = new SchedulerItem
            {
                Id = r.GetValue<int>("Id"),
                GroupId = r.GetValue<int>("PriceGroupId"),
                ClientId = r.GetValue<int>("ClientId"),
                Email = r.GetValue<string>("Email"),
                DaysSend = r.GetValue<string>("DaysSend"),
                TimesSend = r.GetValue<string>("TimeSend"),
                FileName = r.GetValue<string>("FileName"),
                FileConfig = r.GetValue<string>("FileConfig"),
                FileType = r.GetValue<string>("FileType"),
                IsEnabled = r.GetValue<bool>("IsEnabled"),
                Subject = r.GetValue<string>("MailSubject"),
                Body = r.GetValue<string>("MailBody"),
                ProfileId = r.GetValue<int>("ProfileId"),
                LastSend = r.GetValue<DateTime>("LastSend")
            };
            return model;
        }

        public void SaveSendEvent(SendPriceEvent sendEvent)
        {
            new EventDbLogger(this._connectionString).SaveLog(sendEvent);
        }

        public void SaveSendError(PriceSendErrorEvent sendEvent)
        {
            new EventDbLogger(this._connectionString).SaveLog(sendEvent);
        }

        public DateTime GetLastSendDate(int shedulerId)
        {
            using (var cmd = GetCommand("select top 1 Start from [Events] e where e.Code=9002 and EventLabel=@id order by Start desc"))
            {
                cmd.AddParameter("id", shedulerId);
                return (cmd.ExecuteValue() as DateTime?) ?? DateTime.MinValue;
            }

            //using (var cmd=GetCommand("select top 1 Start from [Events] e where e.Code=9002 and EventLabel=@id order by Start desc"))
            //{
            //    cmd.AddParameter("id", shedulerId);
            //    return (cmd.ExecuteValue() as DateTime?)??DateTime.MinValue;
            //}
        }

        public Dictionary<int, DateTime> GetLastSendDates()
        {
            using (var cmd = GetCommand("select EventLabel,  max(Start) as start from [Events] e where e.Code=9002 group by EventLabel"))
            {
                var res = new Dictionary<int, DateTime>();
                cmd.ExecuteReader(r =>
                {
                    var label = r.GetValue<string>("EventLabel") ?? "";
                    int id;
                    if (int.TryParse(label, out id))
                    {
                        res[id] = r.GetValue<DateTime>("start");
                    }
                });

                return res;
            }

        }

        public void SaveScheduler(SchedulerItem schedulerItem)
        {
            if (schedulerItem.ProfileId == 0) throw new Exception("bad Email setting - is null");
            using (var cmd = GetCommand("PriceSender_SaveScheduler"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("id", schedulerItem.Id);
                cmd.AddParameter("groupId", schedulerItem.GroupId);
                cmd.AddParameter("clientId", schedulerItem.ClientId);
                cmd.AddParameter("email", schedulerItem.Email);
                cmd.AddParameter("daysSend", schedulerItem.DaysSend);
                cmd.AddParameter("timeSend", schedulerItem.TimesSend);
                cmd.AddParameter("fileName", schedulerItem.FileName);
                cmd.AddParameter("fileConfig", schedulerItem.FileConfig);
                cmd.AddParameter("fileType", schedulerItem.FileType);
                cmd.AddParameter("isEnabled", schedulerItem.IsEnabled);
                cmd.AddParameter("mailSubject", schedulerItem.Subject);
                cmd.AddParameter("mailBody", schedulerItem.Body);
                cmd.AddParameter("profileId", schedulerItem.ProfileId);
                var newId = (cmd.ExecuteValue() as int?) ?? 0;
                if (newId != 0)
                    schedulerItem.Id = newId;
            }
        }

        public void DeleteScheduler(int id)
        {
            using (var cmd = GetCommand("PriceSender_DeleteScheduler"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("id", id);
                cmd.Execute();
            }
        }

        public void UpdateLastSend(int clientId)
        {
            using (var cmd = GetCommand("update PriceGroupToClient set LastSend=GETDATE() where clientId=@id"))
            {
                cmd.CommandType = CommandType.Text;
                cmd.AddParameter("id", clientId);
                cmd.Execute();
            }
        }

        public List<EmailSetting> GetEmailSettingsForMailingPricesService() 
        {
            using (var cmd = GetCommand("GetEmailSettingsForMailingPricesService"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                var result = new List<EmailSetting>();
                cmd.ExecuteReader(r =>
                {
                    var item = new EmailSetting
                    {
                        Id = r.GetValue<int>("Id"),
                        Email = r.GetValue<string>("PriceEmail"),
                        Login = r.GetValue<string>("PriceEmailLogin"),
                        Password = r.GetValue<string>("PriceEmailPassword"),
                        SmtpPort = r.GetValue<int>("PriceEmailSmtpPort"),
                        SmtpServer = r.GetValue<string>("PriceEmailServer"),
                        UseSsl = r.GetValue<bool>("PriceEmailUseSSL")
                    };
                    result.Add(item);
                });
                return result;
            }
        }


        public List<PriceItem> GetPriceItems(int clientId, int groupId)
        {
            using (var cmd = this.GetCommand("PriceSender_GetPriceList"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("ClientId", clientId);
                cmd.AddParameter("GroupId", groupId);
                cmd.CommandTimeout = 0;
                var result = new List<PriceItem>();
                cmd.ExecuteReader(r =>
                {
                    var item = new PriceItem
                    {
                        Catalog = r.GetValue<string>("CatalogName"),
                        Number = r.GetValue<string>("SellerNumber"),
                        Analog = r.GetValue<string>("Analog"),
                        Name = r.GetValue<string>("Name"),
                        Quantity = r.GetValue<int?>("Qty"),
                        MinOrder = r.GetValue<int?>("MinOrder"),
                        Price = r.GetValue<decimal>("Price"),
                        DeliveryDays = r.GetValue<int>("DeliveryDays"),
                        BarCode = r.GetValue<string>("BarCode")
                    };
                    result.Add(item);
                });

                return result;
            }
        }

        public PriceGroup GetPriceGroup(int id)
        {
            return GetPriceGroups(id).FirstOrDefault(x => x.Id == id);
        }

        public List<PriceGroup> GetPriceGroups(int id = 0)
        {
            using (var cmd = GetCommand("PriceSender_GetPriceGroups"))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.AddParameter("id", id);
                var res = new List<PriceGroup>();

                cmd.ExecuteReader((r, i) =>
                {
                    if (i == 1)
                    {
                        var item = new PriceGroup
                        {
                            Id = r.GetValue<int>("id"),
                            Name = r.GetValue<string>("name"),
                            PriceListIds = new List<int>()

                        };
                        res.Add(item);
                    }
                    if (i == 2)
                    {
                        var idItem = r.GetValue<int>("PriceGroupId");
                        var item = res.FirstOrDefault(x => x.Id == idItem);
                        if (item != null)
                            item.PriceListIds.Add(r.GetValue<int>("PriceListId"));
                    }
                });
                return res;
            }
        }




    }
}