using System;
using System.Globalization;
using System.IO;

namespace Wiki.PriceSender.Dto
{
    public class ArhiveFile
    {


        public int ConfigId { get; set; }
        public int ManagerId { get; set; } 

        private DateTime _date;
        public ArhiveFile() { }

        public ArhiveFile(string str)
        {
            if(string.IsNullOrWhiteSpace(str))
                return;

            var fileName = Path.GetFileNameWithoutExtension(str);
            this.OrigiginalName = fileName;
            var fiels = fileName.Split('&');
            DateTime.TryParseExact(fiels[0], "yyyy.MM.dd_HH.mm.ss", CultureInfo.CurrentCulture, DateTimeStyles.None,
                out this._date);
            if(fiels.Length<2)
                return;
            this.FileType = fiels[1];
            if(fiels.Length<3)
                return;
            this.FileName = fiels[2];
            if(fiels.Length<4)
                return;
            this.Email = fiels[3];

        }

        public DateTime Date
        {
            get { return this._date; }
            set { this._date = value; }
        }

        public string Email { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string OrigiginalName { get; set; }

        public override string ToString()
        {
            var fn = string.Format("{0:yyyy.MM.dd_HH.mm.ss}&{1}&{2}&{3}"
                , this.Date, this.FileType, this.FileName, this.Email);

            return fn;
        }
    }
}