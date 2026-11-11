using TrabajoHuamanga.Dominio.Entidades;
using Xunit;

namespace TrabajoHuamanga.Tests.Dominio;

public class ProfesionalTests
{
    [Fact]
    public void Crear_ConDatosValidos_RetornaProfesional()
    {
        var rubroId = Guid.NewGuid();
        var zonaId = Guid.NewGuid();

        var prof = Profesional.Crear("Juan Pérez", "966123456", "Electricista experimentado",
            rubroId, zonaId, null);

        Assert.NotEqual(Guid.Empty, prof.Id);
        Assert.Equal("Juan Pérez", prof.NombreCompleto);
        Assert.Equal("966123456", prof.Telefono);
        Assert.Equal(rubroId, prof.RubroId);
        Assert.Equal(zonaId, prof.ZonaId);
        Assert.True(prof.Activo);
    }

    [Fact]
    public void Crear_SinNombre_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            Profesional.Crear("", "966123456", "Desc", Guid.NewGuid(), Guid.NewGuid()));
    }

    [Fact]
    public void Crear_SinTelefono_LanzaExcepcion()
    {
        Assert.Throws<ArgumentException>(() =>
            Profesional.Crear("Juan", "", "Desc", Guid.NewGuid(), Guid.NewGuid()));
    }

    [Fact]
    public void Desactivar_CambiaEstado()
    {
        var prof = Profesional.Crear("Juan", "966123456", "Desc", Guid.NewGuid(), Guid.NewGuid());
        prof.Desactivar();
        Assert.False(prof.Activo);
    }

    [Fact]
    public void Activar_DespuesDeDesactivar_CambiaEstado()
    {
        var prof = Profesional.Crear("Juan", "966123456", "Desc", Guid.NewGuid(), Guid.NewGuid());
        prof.Desactivar();
        prof.Activar();
        Assert.True(prof.Activo);
    }

    [Fact]
    public void CalcularPromedioReputacion_SinCalificaciones_RetornaCero()
    {
        var prof = Profesional.Crear("Juan", "966123456", "Desc", Guid.NewGuid(), Guid.NewGuid());
        Assert.Equal(0, prof.CalcularPromedioReputacion());
    }
}

public class CalificacionTests
{
    [Fact]
    public void Crear_ConDatosValidos_RetornaCalificacion()
    {
        var profId = Guid.NewGuid();
        var cal = Calificacion.Crear(profId, "María", 5, "Excelente trabajo");

        Assert.NotEqual(Guid.Empty, cal.Id);
        Assert.Equal(profId, cal.ProfesionalId);
        Assert.Equal("María", cal.NombreUsuario);
        Assert.Equal(5, cal.Puntaje);
        Assert.True(cal.Visible);
        Assert.False(cal.Reportada);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Crear_ConPuntajeInvalido_LanzaExcepcion(int puntaje)
    {
        Assert.Throws<ArgumentException>(() =>
            Calificacion.Crear(Guid.NewGuid(), "María", puntaje, "Comentario"));
    }
}