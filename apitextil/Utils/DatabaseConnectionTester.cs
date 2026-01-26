using Apitextil.Data;
using Microsoft.EntityFrameworkCore;

namespace apitextil;

public class DatabaseConnectionTester
{
    private readonly EcommerceContext _db;

    public DatabaseConnectionTester(EcommerceContext db)
    {
        _db = db;
    }

    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            var canConnect = await _db.Database.CanConnectAsync();
            Console.WriteLine(canConnect
                ? "Conexión EF Core exitosa a la base de datos."
                : "No se pudo conectar (CanConnectAsync=false).");
            return canConnect;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al conectarse a la base de datos (EF Core): {ex.Message}");
            return false;
        }
    }
}
