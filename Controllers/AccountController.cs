using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ArreglaMiCiudad.Data;
using ArreglaMiCiudad.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace ArreglaMiCiudad.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AccountController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ====== Helpers ======
        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }

        // ====== REGISTER ======
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // check email
            bool emailExists = await _context.Users
                .AnyAsync(u => u.Email == model.Email);

            if (emailExists)
            {
                ModelState.AddModelError("", "Email is already registered.");
                return View(model);
            }

            // get Client role
            var clientRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == "Client");

            if (clientRole == null)
            {
                ModelState.AddModelError("", "Client role does not exist.");
                return View(model);
            }

            string? profilePath = null;
            string? idCardPath = null;

            // Carpeta base: wwwroot/uploads
            var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsRoot);

            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(model.ProfileImage.FileName);
                var filePath = Path.Combine(uploadsRoot, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(stream);
                }

                profilePath = Path.Combine("uploads", fileName); // ruta relativa para la web
            }

            if (model.IdCardImage != null && model.IdCardImage.Length > 0)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(model.IdCardImage.FileName);
                var filePath = Path.Combine(uploadsRoot, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.IdCardImage.CopyToAsync(stream);
                }

                idCardPath = Path.Combine("uploads", fileName);
            }

            var user = new User
            {
                Email = model.Email,
                Phone = model.Phone,
                PasswordHash = HashPassword(model.Password),
                CreatedAt = DateTime.Now,
                IsActive = true,
                RoleId = clientRole.RoleId
            };

            var client = new Client
            {
                User = user,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                ProfileImageUrl = profilePath ?? "",   // o null si la columna lo permite
                IdCardImageUrl = idCardPath ?? ""
            };

            _context.Users.Add(user);
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login", "Account");
        }

        // ====== LOGIN ======
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
            {
                TempData["LoginError"] = "Este correo no está registrado. ¿Quieres crear una cuenta?";
                return View(model);
            }

            if (!VerifyPassword(model.Password, user.PasswordHash))
            {
                TempData["LoginError"] = "La contraseña es incorrecta. Vuelve a intentarlo.";
                return View(model);
            }

            if (!user.IsActive)
            {
                TempData["LoginError"] = "Tu usuario está inactivo. Contacta al administrador.";
                return View(model);
            }

            // aquí ya es correcto: creamos los claims y hacemos SignIn
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim(ClaimTypes.Name, user.Email)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }


        // ====== LOGOUT ======
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
