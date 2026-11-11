using TrabajoHuamanga.Dominio.Interfaces;

namespace TrabajoHuamanga.Infraestructura.Adaptadores;

/// <summary>
/// Adaptador de WhatsApp: genera enlace wa.me (gratis, sin API token).
/// </summary>
public class GeneradorEnlaceWhatsApp : IGeneradorEnlaceContacto
{
    public string GenerarEnlace(string telefono, string nombreProfesional, string rubroNombre)
    {
        // Limpiar teléfono: solo dígitos
        var tel = new string(telefono.Where(char.IsDigit).ToArray());

        // Si no tiene código de país, agregar Perú (+51)
        if (!tel.StartsWith("51") && tel.Length <= 9)
            tel = "51" + tel;

        var mensaje = Uri.EscapeDataString(
            $"Hola {nombreProfesional}, te encontré en Trabajo Huamanga como {rubroNombre}. ¿Podemos coordinar?");

        return $"https://wa.me/{tel}?text={mensaje}";
    }
}
