# 10. Configuración de Dominio/Subdominio DNS y CDN

## 10.1 Configuración DNS

### Dominio propuesto: `trabajohuamanga.com`

```
; Archivo de zona DNS
; trabajohuamanga.com

; Registros A — apuntan al servidor
@           A       203.0.113.10          ; IP del servidor principal
www         CNAME   trabajohuamanga.com.  ; www redirige al dominio principal
api         A       203.0.113.10          ; Subdominio para la API REST

; Registros para email (opcional)
@           MX  10  mail.trabajohuamanga.com.
```

### Configuración NGINX con dominios:

```nginx
# Frontend Web
server {
    listen 80;
    server_name trabajohuamanga.com www.trabajohuamanga.com;

    location / {
        proxy_pass http://web:80;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }

    # Redirigir www a dominio sin www
    if ($host = www.trabajohuamanga.com) {
        return 301 http://trabajohuamanga.com$request_uri;
    }
}

# API REST en subdominio
server {
    listen 80;
    server_name api.trabajohuamanga.com;

    location / {
        proxy_pass http://api:80;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
    }
}
```

### SSL/TLS con Certbot (Let's Encrypt):

```bash
# Instalar Certbot
apt install certbot python3-certbot-nginx

# Obtener certificado SSL gratuito
certbot --nginx -d trabajohuamanga.com -d www.trabajohuamanga.com -d api.trabajohuamanga.com

# Renovación automática
certbot renew --dry-run
```

## 10.2 Configuración CDN

### Opción: Cloudflare (plan gratuito)

1. **Registrar dominio** en Cloudflare
2. **Cambiar nameservers** del registrador a los de Cloudflare
3. **Configurar reglas de caché**:

```
Reglas de caché en Cloudflare:
├── *.css, *.js, *.png → Cache Everything (TTL: 1 semana)
├── /api/* → Bypass Cache (siempre fresco)
└── / → Standard (respeta Cache-Control del servidor)
```

### Diagrama con CDN:

```
Usuario (Huamanga)
        │
        ▼
┌───────────────┐
│  Cloudflare   │  ← CDN + DNS + SSL + DDoS Protection
│  (Edge Lima)  │
└───────┬───────┘
        │ Solo si MISS
        ▼
┌───────────────┐
│    NGINX      │  ← Reverse Proxy (origen)
│  (Servidor)   │
└───────┬───────┘
        │
   ┌────┴────┐
   ▼         ▼
 Web MVC   API REST
```

### Beneficios del CDN:
- **Caché de assets estáticos** (CSS, JS, imágenes) en edge servers cercanos
- **SSL gratuito** con Cloudflare Universal SSL
- **Protección DDoS** incluida en plan gratuito
- **Compresión Brotli** automática para respuestas HTML/CSS/JS
- **Latencia reducida**: Edge server en Lima (~30ms vs ~100ms directo)

### Headers de caché en la aplicación:

```csharp
// En Program.cs — Configurar caché de archivos estáticos
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append(
            "Cache-Control", "public, max-age=604800"); // 7 días
    }
});
```

## 10.3 Resumen de URLs

| Servicio | URL | Descripción |
|----------|-----|-------------|
| Frontend | https://trabajohuamanga.com | Aplicación web principal |
| API | https://api.trabajohuamanga.com | REST API + Swagger |
| Swagger | https://api.trabajohuamanga.com/swagger | Documentación interactiva |
