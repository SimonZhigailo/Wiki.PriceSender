using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wiki.PriceSender.Service.Models.PriceList;

namespace Wiki.PriceSender.Service.Helpers
{
    public static class Helper
    {
        //1 - ClientId, 2 - Date, 3 - Email, 4 - FileName, 5 - ManagerName
        public static string GetFilterPriceSendVal(int num)
        {
            string filter = "ManagerName";
            switch (num)
            {
                case 1:
                    filter = "ClientId";
                    break;

                case 2:
                    filter = "Date";
                    break;

                case 3:
                    filter = "Email";
                    break;
                case 4:
                    filter = "FileName";
                    break;
            }
            return filter;
        }

        public static PriceChangeClientEvent CalculateClientUpdates(PriceSendModel model, PriceGroupToClient changedModel, string userName)
        {
            //можно было сделать через Object.GetProperties, но пока для теста так
            var list = new List<KeyValuePair<string, string>>();
            var days = new string(changedModel.DaysSend);
            string times = changedModel.TimeSend.Aggregate("", (current, s) => current + (s + "; "));


            if (!days.Equals(model.DaysSend))
            {
                list.Add(new KeyValuePair<string, string>("DaysSend", days  + " => " + model.DaysSend));
            }
            if (!times.Equals(model.TimesSend))
            {
                list.Add(new KeyValuePair<string, string>("TimeSend", times + " => " + model.TimesSend));
            }
            if (!model.Email.Equals(changedModel.ToEmail))
            {
                list.Add(new KeyValuePair<string, string>("ToEmail", changedModel.ToEmail + " => " +  model.Email));
            }
            if (!model.FileName.Equals(changedModel.FileName))
            {
                list.Add(new KeyValuePair<string, string>("FileName", changedModel.FileName + " => " + model.FileName));
            }
            if (!model.Subject.Equals(changedModel.MailSubject))
            {
                list.Add(new KeyValuePair<string, string>("MailSubject", changedModel.MailSubject + " => " + model.Subject));
            }
            if (!model.Body.Equals(changedModel.MailBody))
            {
                var bodyres = (changedModel.MailBody + " => " + model.Body).Substring(0, 490);
                list.Add(new KeyValuePair<string, string>("MailBody", bodyres));
            }
            if (!model.FileConfig.Equals(changedModel.FileConfig))
            {
                list.Add(new KeyValuePair<string, string>("FileConfig",  model.FileConfig));
            }

            return ServiceFactory.GetPriceChangeClientEvent(model.GroupId, list, userName, "Изменены данные клиента " + changedModel.ClientCode, "update");

        }

        public static PriceCreateClientEvent CalculateClientCreate(PriceGroupToClient changedModel, string userName)
        {
            var days = new string(changedModel.DaysSend);
            string times = changedModel.TimeSend.Aggregate("", (current, s) => current + (s + "; "));

            var list = new List<KeyValuePair<string, string>>();

            list.Add(new KeyValuePair<string, string>("DaysSend", days));
            list.Add(new KeyValuePair<string, string>("TimeSend", times));
            list.Add(new KeyValuePair<string, string>("ToEmail", changedModel.ToEmail));
            list.Add(new KeyValuePair<string, string>("FileName", changedModel.FileName));
            list.Add(new KeyValuePair<string, string>("MailSubject", changedModel.MailSubject));
            list.Add(new KeyValuePair<string, string>("MailBody", changedModel.MailBody));
            list.Add(new KeyValuePair<string, string>("FileConfig", changedModel.FileConfig));

            return ServiceFactory.GetPriceCreateClientEvent(changedModel.GroupId, list, userName, "Добавленны данные клиента " + changedModel.ClientCode, "create");

        }





    }
}
