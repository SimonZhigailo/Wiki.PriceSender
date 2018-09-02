using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using CarParts.Common.Log;
using Wiki.PriceSender.Service.Models;

namespace Wiki.PriceSender.Service
{
    public class SendPriceEvent : LoggerEvent
    {

        //8999 - отправка прайс листа
        public SendPriceEvent(int profileId, DateTime? nextSend, string to, int clientId, int groupId) : base(profileId + "", 8999, null)
        {
            this._message = string.Format("Отправка прайс-листа. Профиль:{0}", profileId);
            this["sendTime"] = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            this["nextTime"] = nextSend.HasValue?nextSend.ToString() : "";
            this["emailTo"] = to;
            this["profileId"] = profileId.ToString();
            this["clientId"] = clientId.ToString();
            this["groupId"] = groupId.ToString();

        }
    }
    //8998 - ошибка отправки прайс листа

        public class PriceSendErrorEvent : LoggerEvent { 


             public PriceSendErrorEvent(int profileId, string errorMessage, Exception e, string to, int clientId, int groupId) : base("PriceSendErrorEvent", 8998, null)
             {
                 this._message = string.Format("Прайс не отравлен: {0}", errorMessage);
                 this["erorMessage"] = errorMessage;
                 this["profileId"] = profileId.ToString();
                 this["emailTo"] = to;
                 this["clientId"] = clientId.ToString();
                 this["groupId"] = groupId.ToString();

             }
    }
    //9003 - обновление категориий в прайсе
    //(int группа, string откуда(прайс-дерево-категория), значение) отлавливать в репозитории
    public class PriceChangeEvent : LoggerEvent
    {
        public PriceChangeEvent(int groupId, string managerName, KeyValuePair<string, string> val, string message, string type )
            : base("PriceChangeEvent : " + groupId, 9003, "Изменение в категориях прайс группы")
        {

            this["groupId"] = groupId.ToString();
            this["managerName"] = managerName;
            this[val.Key] = val.Value;
            this["message"] = message;
            this["type"] = type;
        }
    }

    public class PriceChangeClientEvent : LoggerEvent
    {
        //9004 - обновление информации о клиенте
        //(в репозитории сделать выборку и переслать параметры в виде пары)
        public PriceChangeClientEvent(int groupId, List<KeyValuePair<string, string>> vals, string managerName, string message, string type) : base("PriceChangeClientEvent : "+groupId, 9004, "Обновление информации о клиенте")
        {
            this["groupId"] = groupId.ToString();
            this["managerName"] = managerName;
            this["message"] = message;
            this["type"] = type;

            foreach (KeyValuePair<string, string> pair in vals)
            {
                this[pair.Key] = pair.Value;
            }
        }
    }

    public class PriceCreateClientEvent : LoggerEvent
    {
        //9005
        public PriceCreateClientEvent(int groupId, List<KeyValuePair<string, string>> vals, string managerName,
            string message, string type)
            : base("PriceCreateClientEvent : " + groupId, 9005, "Добавление клиента в прайс-группу")
        {
            this["groupId"] = groupId.ToString();
            this["managerName"] = managerName;
            this["message"] = message;
            this["type"] = type;

            foreach (KeyValuePair<string, string> pair in vals)
            {
                this[pair.Key] = pair.Value;
            }
        }
    }

    public class PriceDeleteClientEvent : LoggerEvent
    {
        //9006
        public PriceDeleteClientEvent(int groupId, List<KeyValuePair<string, string>> vals, string managerName,
            string message, string type)
            : base("PriceDeleteClientEvent : " + groupId, 9006, "Удаление клиента из прайс-группы")
        {
            this["groupId"] = groupId.ToString();
            this["managerName"] = managerName;
            this["message"] = message;
            this["type"] = type;

            foreach (KeyValuePair<string, string> pair in vals)
            {
                this[pair.Key] = pair.Value;
            }
        }
    }


}