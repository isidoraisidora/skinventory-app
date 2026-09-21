using ClosedXML.Excel;
using Service.Interface;

namespace Service.Implementation;

public class ExcelExportService : IExcelExportService
{
    private readonly IProductService _productService;

    public ExcelExportService(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<byte[]> ExportProductsAsync()
    {
        var products = await _productService.GetAllAsync(null, null);
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Products");

        var headers = new[]
        {
            "Product Id", "Name", "Barcode", "Brand"
        };

        for (int i = 0; i < headers.Length; i++)
        {
            ws.Cell(1, i + 1).Value = headers[i];
        }

        int row = 2;
        foreach (var p in products)
        {
            ws.Cell(row, 1).Value = p.Id.ToString();
            ws.Cell(row, 2).Value = p.Name;
            ws.Cell(row, 3).Value = p.Barcode;
            ws.Cell(row, 4).Value = p.Brand;
            row++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}