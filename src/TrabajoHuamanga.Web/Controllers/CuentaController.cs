using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TrabajoHuamanga.Dominio.Entidades;
using TrabajoHuamanga.Dominio.Interfaces;

namespace TrabajoHuamanga.Web.Controllers;

public class CuentaController : Controller
{
    private readonly IUsuarioRepository _usuarios;
    public CuentaController(IUsuarioRepository usuarios) { _usuarios = usuarios; }

    public IActionResult Login(string? returnUrl = null) { ViewBag.ReturnUrl = returnUrl; return View(); }

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) { ViewBag.Error = "Ingrese email y contrasena."; return View(); }
        var usuario = await _usuarios.ObtenerPorEmailAsync(email);
        if (usuario == null || !VerificarPassword(password, usuario.PasswordHash)) { ViewBag.Error = "Email o contrasena incorrectos."; return View(); }
        if (!usuario.Activo) { ViewBag.Error = "Su cuenta esta desactivada."; return View(); }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.NombreCompleto),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString())
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) });
        TempData["Mensaje"] = "Bienvenido, " + usuario.NombreCompleto + "!";
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Registrar() { return View(); }

    [HttpPost]
    public async Task<IActionResult> Registrar(string nombreCompleto, string email, string password, string confirmarPassword, string rol)
    {
        if (string.IsNullOrWhiteSpace(nombreCompleto) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) { ViewBag.Error = "Todos los campos son obligatorios."; return View(); }
        if (password != confirmarPassword) { ViewBag.Error = "Las contrasenas no coinciden."; return View(); }
        if (password.Length < 6) { ViewBag.Error = "La contrasena debe tener al menos 6 caracteres."; return View(); }
        if (await _usuarios.ExisteEmailAsync(email)) { ViewBag.Error = "Ya existe una cuenta con ese email."; return View(); }

        var rolUsuario = rol == "Profesional" ? RolUsuario.Profesional : RolUsuario.Vecino;
        var passwordHash = HashPassword(password);
        var usuario = Usuario.Crear(nombreCompleto, email, passwordHash, rolUsuario);
        await _usuarios.AgregarAsync(usuario);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Name, usuario.NombreCompleto),
            new Claim(ClaimTypes.Email, usuario.Email),
            new Claim(ClaimTypes.Role, usuario.Rol.ToString())
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7) });
        TempData["Mensaje"] = "Cuenta creada exitosamente! Bienvenido, " + usuario.NombreCompleto + ".";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Mensaje"] = "Has cerrado sesion.";
        return RedirectToAction("Index", "Home");
    }

    private static string HashPassword(string password)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(password + "TrabajoHuamanga2024");
        var hash = System.Security.Cryptography.SHA256.Create().ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private static bool VerificarPassword(string password, string hash) { return HashPassword(password) == hash; }
}
