using MagicVilla_API.Modelos;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Datos
{
	public class ApplicationDbContext :DbContext
	{
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
        {
            
        }
        public DbSet<Villa> Villas { get; set; }
		public DbSet<NumeroVilla> NumeroVillas { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Villa>().HasData(
				new Villa()
				{
					Id = 1,
					Nombre = "Villa Real",
					Detalle = "Detalle de la villa ...",
					ImageUrl = "",
					Ocupantes = 5,
					MetrosCuadrados = 50,
					Tarifa = 200,
					Amenidad = "",
					FechaActualizacion=DateTime.Now,
					FechaCreacion=DateTime.Now
				},
				new Villa()
				{
					Id = 2,
					Nombre = "Premium Vista a la piscina",
					Detalle = "Detalle de la villa ...",
					ImageUrl = "",
					Ocupantes = 4,
					MetrosCuadrados = 40,
					Tarifa = 150,
					Amenidad = "",
					FechaActualizacion = DateTime.Now,
					FechaCreacion = DateTime.Now
				}
				);
			;

		}
	}
}
