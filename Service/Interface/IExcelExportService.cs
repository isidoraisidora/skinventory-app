namespace Service.Interface;

public interface IExcelExportService
{
    Task<byte[]> ExportProductsAsync();
}