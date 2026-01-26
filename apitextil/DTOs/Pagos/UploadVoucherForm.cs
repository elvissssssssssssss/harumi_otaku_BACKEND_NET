using Microsoft.AspNetCore.Http;

namespace Apitextil.DTOs.Pagos;

public class UploadVoucherForm
{
    public IFormFile Imagen { get; set; } = default!;
    public string? NroOperacion { get; set; }
}
