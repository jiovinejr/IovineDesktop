using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Collections.Generic;
using System.IO;
using System;

namespace ShipApp.MVVM.Models
{
    public class ReadDataFile
    {
        private readonly MemoryStream _stream;
        private readonly IWorkbook _workbook;
        private readonly ISheet _sheet;
        private readonly string _shipName;

        public ReadDataFile(MemoryStream stream, string fileName)
        {
            _stream = stream;
            _workbook = CreateWorkbookFromStream(_stream, fileName);
            _sheet = GetRelevantSheet(_workbook);
            _shipName = GetShipNameFromSheet(_sheet);
        }

        public string ShipName => _shipName;
        public ISheet Sheet => _sheet;

        public List<ExcelRecord> CreateExcelOrderList()
        {
            var list = new List<ExcelRecord>();
            foreach (IRow row in _sheet)
            {
                if (row == null || row.GetCell(0) == null) continue;

                if (row.GetCell(0).CellType == CellType.Numeric)
                {
                    var record = MapRowToExcelRecord(row);
                    list.Add(record);
                }
            }
            return list;
        }

        private ExcelRecord MapRowToExcelRecord(IRow row)
        {
            var formatter = new DataFormatter();
            var quantityStr = formatter.FormatCellValue(row.GetCell(0));
            var quantity = decimal.Parse(quantityStr);
            var measurement = formatter.FormatCellValue(row.GetCell(1));
            var item = formatter.FormatCellValue(row.GetCell(2));

            return new ExcelRecord
            {
                RowNumber = row.RowNum,
                Quantity = quantity,
                Measurement = measurement,
                Item  = item,
                ShipName = _shipName
            };
        }

        private IWorkbook CreateWorkbookFromStream(Stream stream, string fileName)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return Path.GetExtension(fileName) switch
            {
                ".xls" => new HSSFWorkbook(stream),
                ".xlsx" => new XSSFWorkbook(stream),
                _ => throw new InvalidDataException("Unsupported file extension")
            };
        }

        private ISheet GetRelevantSheet(IWorkbook workbook)
        {
            return workbook.NumberOfSheets > 2 ? workbook.GetSheetAt(2) : workbook.GetSheetAt(0);
        }

        private string GetShipNameFromSheet(ISheet sheet)
        {
            return sheet.GetRow(0).GetCell(2)?.StringCellValue ?? "Unknown";
        }
    }
}
