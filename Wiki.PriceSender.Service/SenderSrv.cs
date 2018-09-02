using System;
using System.Security.Policy;
using CarParts.Common.Log;

namespace Wiki.PriceSender.Service
{
    internal class SenderSrv
    {

        public static string Url { get; set; }


        public static readonly FileLogger Logger=new FileLogger("PriceSender");




    }
}