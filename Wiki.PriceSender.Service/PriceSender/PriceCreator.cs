using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using CarParts.Common.Log;
using EasyXLS;
using Newtonsoft.Json.Linq;
using Wiki.PriceSender.Dto;

namespace Wiki.PriceSender.Service.PriceSender
{
    public class PriceCreator : IDisposable
    {
        private readonly string _fileType;
        private Guid idDir = Guid.NewGuid();
        private string _fileName;
        private Dictionary<string, ColumConfig> _colums = new Dictionary<string, ColumConfig>();

        public PriceCreator(SchedulerItem config) : this(config.FileType, config.FileConfig)
        {
        }


        public PriceCreator(string fileType, string fileConfig)
        {
            this._fileType = fileType;
            var obj = JObject.Parse(fileConfig);
            var colums = obj.GetValue("colums").ToObject<ColumConfig[]>();
            colums.Where(x => x.Index > 0).ToList().ForEach(x =>
            {
                this._colums[x.Field.ToLower()] = x;
            });

        }

        public byte[] CreatePrice<TItem>(List<TItem> items)
        {
            EasyXLS.ExcelDocument doc = CreateDocument(items);
            using (var st = new MemoryStream())
            {
                switch (this._fileType)
                {
                    case "Xlsx":
                        {
                            doc.easy_WriteXLSXFile(st);
                            break;
                        }
                    case "Xls":
                        {
                            doc.easy_WriteXLSFile(st);
                            break;
                        }
                    case "Csv":
                        {
                            SaveToCsv(st, doc);
                            break;
                        }
                }
                return st.ToArray();
            }

        }

        private void SaveToCsv(MemoryStream st, ExcelDocument doc)
        {
            var ws = (ExcelWorksheet)doc.easy_getSheetAt(0);
            var tbl = ws.easy_getExcelTable();
            var rowCount = tbl.RowCount();
            using (var writer = new StreamWriter(st,Encoding.Default))
            {
                for (var i = 0; i < rowCount; i++)
                {
                    var row = tbl.easy_getRowAt(i);
                    var cellCount = row.Count();
                    for (var j = 0; j < cellCount; j++)
                    {
                        var cell = row.easy_getCellAt(j);
                        var value = cell.getValue();
                        writer.Write(value);
                        writer.Write(";");
                    }
                    writer.WriteLine();
                }
            }
        }


        public ExcelDocument CreateDocument<TItem>(List<TItem> items)
        {
            var doc = new ExcelDocument();
            doc.easy_addWorksheet("Прайс");
            var ws = (ExcelWorksheet)doc.easy_getSheetAt(0);
            var tbl = ws.easy_getExcelTable();
            CreateHeadRow(tbl);

            var config = CreateConfig<TItem>();

            foreach (var item in items)
            {
                AddItemRow(tbl, item, config);
            }
            return doc;
        }



        private List<PropertyAccessor> CreateConfig<TItem>()
        {
            var type = typeof(TItem);
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty)
                .ToDictionary(x => x.Name.ToLower());
            var properties = new List<PropertyAccessor>();
            foreach (var columConfig in this._colums)
            {
                if (props.ContainsKey(columConfig.Key))
                {
                    var accesor = new PropertyAccessor(columConfig.Value.Index, props[columConfig.Key], typeof(TItem));
                    properties.Add(accesor);
                }
            }

            return properties;
        }

        private void AddItemRow(ExcelTable tbl, object item, List<PropertyAccessor> config)
        {
            var row = new ExcelRow();
            foreach (var propertyAccessor in config)
            {
                propertyAccessor.FullCell(row, item);
            }
            //row.InitSell(0, false, item.Name);
            //row.InitSell(1, true, item.Catalog);
            //row.InitSell(2, true, item.Number);
            //row.InitSell(3, true, item.Analog);
            //row.InitSell(4, true, item.Quantity.ToString());
            //row.InitSell(5, true, item.Price.ToString());
            //row.InitSell(6, true, item.MinOrder.ToString());
            tbl.easy_addRow(row);

        }

        private void CreateHeadRow(ExcelTable tbl)
        {
            var row = new ExcelRow();

            foreach (var col in this._colums)
            {

                row.InitSell(col.Value.Index - 1, true, col.Value.Title);
            }

            tbl.easy_addRow(row);
        }

        public void Dispose()
        {
            if (this._fileName != null)
            {
                try
                {
                    File.Delete(this._fileName);
                }
                catch (Exception e)
                {
                    new FileLogger().WriteError("Error delete tempPrice file.", e);
                }
            }
        }
    }

    internal class PropertyAccessor
    {
        private readonly int _index;
        private Delegate _accessor;

        public PropertyAccessor(int index, PropertyInfo propertyInfo, Type type)
        {
            this._index = index - 1;
            var ex = Expression.Parameter(type, "x");
            var pr = Expression.Property(ex, propertyInfo);
            this._accessor = Expression.Lambda(pr, ex).Compile();
        }

        public void FullCell(ExcelRow row, object item)
        {
            row.InitSell(this._index, false, this._accessor.DynamicInvoke(item) + "");
        }
    }

    public class ColumConfig
    {
        public string Field { get; set; }
        public string Title { get; set; }
        public int Index { get; set; }
    }

    public static class ExcelRowExtension
    {
        public static void InitSell(this ExcelRow row, int index, bool isBold, string value)
        {
            var easyGetCell = row.easy_getCell(index);
            easyGetCell.setBold(isBold);
            easyGetCell.setDataType("string");
            easyGetCell.setValue(value);
        }

    }

    public enum FileFormat
    {
        Xlsx = 0,
        Csv
    }
}