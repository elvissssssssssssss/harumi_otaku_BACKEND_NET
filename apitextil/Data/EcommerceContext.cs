
using Microsoft.EntityFrameworkCore;
using Apitextil.Models.Entities;

namespace Apitextil.Data;

public class EcommerceContext : DbContext
{
    public EcommerceContext(DbContextOptions<EcommerceContext> options) : base(options) { }

    public DbSet<tblUsuario> tblusuarios => Set<tblUsuario>();
    public DbSet<tblCategoria> tblcategorias => Set<tblCategoria>();
    public DbSet<tblProducto> tblproductos => Set<tblProducto>();

    public DbSet<tblCarrito> tblcarritos => Set<tblCarrito>();
    public DbSet<tblCarritoItem> tblcarrito_items => Set<tblCarritoItem>();

    public DbSet<tblEstadoOrden> tblestados_orden => Set<tblEstadoOrden>();
    public DbSet<tblOrden> tblordenes => Set<tblOrden>();
    public DbSet<tblOrdenItem> tblorden_items => Set<tblOrdenItem>();
    public DbSet<tblOrdenEstadoHistorial> tblorden_estado_historial => Set<tblOrdenEstadoHistorial>();

    public DbSet<tblPago> tblpagos => Set<tblPago>();
    public DbSet<tblPagoYapeVoucher> tblpago_yape_voucher => Set<tblPagoYapeVoucher>();
    public DbSet<tblPagoValidacion> tblpago_validacion => Set<tblPagoValidacion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ====== Tablas (mapea a tus nombres tbl...) ======
        modelBuilder.Entity<tblUsuario>().ToTable("tblusuarios");
        modelBuilder.Entity<tblCategoria>().ToTable("tblcategorias");
        modelBuilder.Entity<tblProducto>().ToTable("tblproductos");
        modelBuilder.Entity<tblCarrito>().ToTable("tblcarritos");
        modelBuilder.Entity<tblCarritoItem>().ToTable("tblcarrito_items");
        modelBuilder.Entity<tblEstadoOrden>().ToTable("tblestados_orden");
        modelBuilder.Entity<tblOrden>().ToTable("tblordenes");
        modelBuilder.Entity<tblOrdenItem>().ToTable("tblorden_items");
        modelBuilder.Entity<tblOrdenEstadoHistorial>().ToTable("tblorden_estado_historial");
        modelBuilder.Entity<tblPago>().ToTable("tblpagos");
        modelBuilder.Entity<tblPagoYapeVoucher>().ToTable("tblpago_yape_voucher");
        modelBuilder.Entity<tblPagoValidacion>().ToTable("tblpago_validacion");
        modelBuilder.Entity<tblUsuario>(e =>
        {
            e.ToTable("tblusuarios");
            e.Property(x => x.PasswordHash).HasColumnName("password_hash");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
            e.Property(x => x.Nombre).HasColumnName("nombre");
            e.Property(x => x.Apellido).HasColumnName("apellido");
            e.Property(x => x.Rol).HasColumnName("rol");
            e.Property(x => x.Email).HasColumnName("email");
        });
        // ===== tblusuarios =====
        modelBuilder.Entity<tblUsuario>(e =>
        {
            e.ToTable("tblusuarios");
            e.Property(x => x.PasswordHash).HasColumnName("password_hash");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        });
        modelBuilder.Entity<tblPagoYapeVoucher>(e =>
        {
            e.ToTable("tblpago_yape_voucher");

            e.HasKey(x => x.PagoId);                         // PK [web:364]
            e.Property(x => x.PagoId).HasColumnName("pago_id");

            e.Property(x => x.YapeQrPayload).HasColumnName("yape_qr_payload");
            e.Property(x => x.VoucherImagenUrl).HasColumnName("voucher_imagen_url");
            e.Property(x => x.NroOperacion).HasColumnName("nro_operacion");
            e.Property(x => x.PaidAt).HasColumnName("paid_at");

            // relación 1-1 (opcional pero recomendado)
            e.HasOne(x => x.Pago)
             .WithOne() // o .WithOne(p => p.PagoYapeVoucher) si tienes navegación en tblPago
             .HasForeignKey<tblPagoYapeVoucher>(x => x.PagoId);
        });

        // ===== tblcategorias =====
        modelBuilder.Entity<tblCategoria>(e =>
        {
            e.ToTable("tblcategorias");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        });

        // ===== tblproductos =====
        modelBuilder.Entity<tblProducto>(e =>
        {
            e.ToTable("tblproductos");
            e.Property(x => x.CategoriaId).HasColumnName("categoria_id");
            e.Property(x => x.FotoUrl).HasColumnName("foto_url");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        });

        // ===== tblcarritos =====
        modelBuilder.Entity<tblCarrito>(e =>
        {
            e.ToTable("tblcarritos");
            e.Property(x => x.UsuarioId).HasColumnName("usuario_id");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        });

        // ===== tblcarrito_items =====
        modelBuilder.Entity<tblCarritoItem>(e =>
        {
            e.ToTable("tblcarrito_items");
            e.Property(x => x.CarritoId).HasColumnName("carrito_id");
            e.Property(x => x.ProductoId).HasColumnName("producto_id");
            e.Property(x => x.PrecioUnitario).HasColumnName("precio_unitario");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        });

        // ===== tblordenes =====
        modelBuilder.Entity<tblOrden>(e =>
        {
            e.ToTable("tblordenes");
            e.Property(x => x.UsuarioId).HasColumnName("usuario_id");
            e.Property(x => x.PickupAt).HasColumnName("pickup_at");
            e.Property(x => x.TotalAmount).HasColumnName("total_amount");
            e.Property(x => x.EstadoActualId).HasColumnName("estado_actual_id");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        });

        // ===== tblorden_items =====
        modelBuilder.Entity<tblOrdenItem>(e =>
        {
            e.ToTable("tblorden_items");
            e.Property(x => x.OrdenId).HasColumnName("orden_id");
            e.Property(x => x.ProductoId).HasColumnName("producto_id");
            e.Property(x => x.PrecioUnitario).HasColumnName("precio_unitario");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
        });

        // ===== tblorden_estado_historial =====
        modelBuilder.Entity<tblOrdenEstadoHistorial>(e =>
        {
            e.ToTable("tblorden_estado_historial");
            e.Property(x => x.OrdenId).HasColumnName("orden_id");
            e.Property(x => x.EstadoId).HasColumnName("estado_id");
            e.Property(x => x.CambiadoPorUsuarioId).HasColumnName("cambiado_por_usuario_id");
            e.Property(x => x.FechaCambio).HasColumnName("fecha_cambio");
        });

        // ===== tblpagos =====
        modelBuilder.Entity<tblPago>(e =>
        {
            e.ToTable("tblpagos");
            e.Property(x => x.OrdenId).HasColumnName("orden_id");
            e.Property(x => x.CreatedAt).HasColumnName("created_at");
            e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        });

        // ===== tblpago_yape_voucher =====
        modelBuilder.Entity<tblPagoYapeVoucher>(e =>
        {
            e.ToTable("tblpago_yape_voucher");
            e.HasKey(x => x.PagoId);
            e.Property(x => x.PagoId).HasColumnName("pago_id");
            e.Property(x => x.YapeQrPayload).HasColumnName("yape_qr_payload");
            e.Property(x => x.VoucherImagenUrl).HasColumnName("voucher_imagen_url");
            e.Property(x => x.NroOperacion).HasColumnName("nro_operacion");
            e.Property(x => x.PaidAt).HasColumnName("paid_at");
        });

        // ===== tblpago_validacion =====
        modelBuilder.Entity<tblPagoValidacion>(e =>
        {
            e.ToTable("tblpago_validacion");
            e.Property(x => x.PagoId).HasColumnName("pago_id");
            e.Property(x => x.AdminUsuarioId).HasColumnName("admin_usuario_id");
            e.Property(x => x.ValidatedAt).HasColumnName("validated_at");
        });

        // ===== tblusuarios =====
        modelBuilder.Entity<tblUsuario>(e =>
{
    e.ToTable("tblusuarios");
    e.Property(x => x.PasswordHash).HasColumnName("password_hash");
    e.Property(x => x.CreatedAt).HasColumnName("created_at");
    e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
});

// ===== tblcategorias =====
modelBuilder.Entity<tblCategoria>(e =>
{
    e.ToTable("tblcategorias");
    e.Property(x => x.CreatedAt).HasColumnName("created_at");
    e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
});

// ===== tblproductos =====
modelBuilder.Entity<tblProducto>(e =>
{
    e.ToTable("tblproductos");
    e.Property(x => x.CategoriaId).HasColumnName("categoria_id");
    e.Property(x => x.FotoUrl).HasColumnName("foto_url");
    e.Property(x => x.CreatedAt).HasColumnName("created_at");
    e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
});

// ===== tblcarritos =====
modelBuilder.Entity<tblCarrito>(e =>
{
    e.ToTable("tblcarritos");
    e.Property(x => x.UsuarioId).HasColumnName("usuario_id");
    e.Property(x => x.CreatedAt).HasColumnName("created_at");
    e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
});

// ===== tblcarrito_items =====
modelBuilder.Entity<tblCarritoItem>(e =>
{
    e.ToTable("tblcarrito_items");
    e.Property(x => x.CarritoId).HasColumnName("carrito_id");
    e.Property(x => x.ProductoId).HasColumnName("producto_id");
    e.Property(x => x.PrecioUnitario).HasColumnName("precio_unitario");
    e.Property(x => x.CreatedAt).HasColumnName("created_at");
    e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
});

// ===== tblordenes =====
modelBuilder.Entity<tblOrden>(e =>
{
    e.ToTable("tblordenes");
    e.Property(x => x.UsuarioId).HasColumnName("usuario_id");
    e.Property(x => x.PickupAt).HasColumnName("pickup_at");
    e.Property(x => x.TotalAmount).HasColumnName("total_amount");
    e.Property(x => x.EstadoActualId).HasColumnName("estado_actual_id");
    e.Property(x => x.CreatedAt).HasColumnName("created_at");
    e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
});

// ===== tblorden_items =====
modelBuilder.Entity<tblOrdenItem>(e =>
{
    e.ToTable("tblorden_items");
    e.Property(x => x.OrdenId).HasColumnName("orden_id");
    e.Property(x => x.ProductoId).HasColumnName("producto_id");
    e.Property(x => x.PrecioUnitario).HasColumnName("precio_unitario");
    e.Property(x => x.CreatedAt).HasColumnName("created_at");
});

// ===== tblorden_estado_historial =====
modelBuilder.Entity<tblOrdenEstadoHistorial>(e =>
{
    e.ToTable("tblorden_estado_historial");
    e.Property(x => x.OrdenId).HasColumnName("orden_id");
    e.Property(x => x.EstadoId).HasColumnName("estado_id");
    e.Property(x => x.CambiadoPorUsuarioId).HasColumnName("cambiado_por_usuario_id");
    e.Property(x => x.FechaCambio).HasColumnName("fecha_cambio");
});

// ===== tblpagos =====
modelBuilder.Entity<tblPago>(e =>
{
    e.ToTable("tblpagos");
    e.Property(x => x.OrdenId).HasColumnName("orden_id");
    e.Property(x => x.CreatedAt).HasColumnName("created_at");
    e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
});

// ===== tblpago_yape_voucher =====
modelBuilder.Entity<tblPagoYapeVoucher>(e =>
{
    e.ToTable("tblpago_yape_voucher");
    e.HasKey(x => x.PagoId);
    e.Property(x => x.PagoId).HasColumnName("pago_id");
    e.Property(x => x.YapeQrPayload).HasColumnName("yape_qr_payload");
    e.Property(x => x.VoucherImagenUrl).HasColumnName("voucher_imagen_url");
    e.Property(x => x.NroOperacion).HasColumnName("nro_operacion");
    e.Property(x => x.PaidAt).HasColumnName("paid_at");
});

// ===== tblpago_validacion =====
modelBuilder.Entity<tblPagoValidacion>(e =>
{
    e.ToTable("tblpago_validacion");
    e.Property(x => x.PagoId).HasColumnName("pago_id");
    e.Property(x => x.AdminUsuarioId).HasColumnName("admin_usuario_id");
    e.Property(x => x.ValidatedAt).HasColumnName("validated_at");
});

        // ====== Keys especiales ======
        modelBuilder.Entity<tblPagoYapeVoucher>()
            .HasKey(x => x.PagoId);

        // ====== Índices útiles ======
        modelBuilder.Entity<tblUsuario>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<tblCarritoItem>()
            .HasIndex(x => new { x.CarritoId, x.ProductoId })
            .IsUnique();

        modelBuilder.Entity<tblPago>()
            .HasIndex(x => x.OrdenId)
            .IsUnique(); // 1 pago por orden

        modelBuilder.Entity<tblPagoValidacion>()
            .HasIndex(x => x.PagoId)
            .IsUnique(); // 1 validación por pago

        // ====== Relaciones (EF Core convención + refuerzo) ======
        modelBuilder.Entity<tblProducto>()
            .HasOne(p => p.Categoria)
            .WithMany(c => c.Productos)
            .HasForeignKey(p => p.CategoriaId);

        modelBuilder.Entity<tblCarrito>()
            .HasOne(c => c.Usuario)
            .WithMany(u => u.Carritos)
            .HasForeignKey(c => c.UsuarioId);

        modelBuilder.Entity<tblCarritoItem>()
            .HasOne(i => i.Carrito)
            .WithMany(c => c.Items)
            .HasForeignKey(i => i.CarritoId);

        modelBuilder.Entity<tblCarritoItem>()
            .HasOne(i => i.Producto)
            .WithMany()
            .HasForeignKey(i => i.ProductoId);

        modelBuilder.Entity<tblOrden>()
            .HasOne(o => o.Usuario)
            .WithMany(u => u.Ordenes)
            .HasForeignKey(o => o.UsuarioId);

        modelBuilder.Entity<tblOrden>()
            .HasOne(o => o.EstadoActual)
            .WithMany()
            .HasForeignKey(o => o.EstadoActualId);

        modelBuilder.Entity<tblOrdenItem>()
            .HasOne(i => i.Orden)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.OrdenId);

        modelBuilder.Entity<tblOrdenItem>()
            .HasOne(i => i.Producto)
            .WithMany()
            .HasForeignKey(i => i.ProductoId);

        modelBuilder.Entity<tblOrdenEstadoHistorial>()
            .HasOne(h => h.Orden)
            .WithMany(o => o.HistorialEstados)
            .HasForeignKey(h => h.OrdenId);

        modelBuilder.Entity<tblOrdenEstadoHistorial>()
            .HasOne(h => h.Estado)
            .WithMany()
            .HasForeignKey(h => h.EstadoId);

        modelBuilder.Entity<tblOrdenEstadoHistorial>()
            .HasOne(h => h.CambiadoPorUsuario)
            .WithMany()
            .HasForeignKey(h => h.CambiadoPorUsuarioId);

        // Pago 1-1 Orden
        modelBuilder.Entity<tblPago>()
            .HasOne(p => p.Orden)
            .WithOne(o => o.Pago)
            .HasForeignKey<tblPago>(p => p.OrdenId);

        // YapeVoucher 1-1 Pago
        modelBuilder.Entity<tblPago>()
    .HasOne(p => p.PagoYapeVoucher)
    .WithOne(v => v.Pago)
    .HasForeignKey<tblPagoYapeVoucher>(v => v.PagoId);


        // Validación 1-1 Pago
        modelBuilder.Entity<tblPago>()
            .HasOne(p => p.Validacion)
            .WithOne(v => v.Pago)
            .HasForeignKey<tblPagoValidacion>(v => v.PagoId);

        modelBuilder.Entity<tblPagoValidacion>()
            .HasOne(v => v.AdminUsuario)
            .WithMany(u => u.ValidacionesRealizadas)
            .HasForeignKey(v => v.AdminUsuarioId);
    }

}
