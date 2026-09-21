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
        /*var products = await _productService.GetAllAsync(null, null);
        using var workbook = new XLWorkbook();*/
        return null;
    }
}