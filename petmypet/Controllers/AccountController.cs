using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using petmypet.Models;
using petmypet.ViewModels;

namespace petmypet.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly INotyfService _notyf;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, INotyfService notyf)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _notyf = notyf;
        }

        public IActionResult Login(string returnUrl = null)
        {
            // Defina o URL de retorno ou uma URL padrão (ex: página inicial)
            returnUrl ??= Url.Content("~/");

            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string Phone, string Password, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/"); // URL padrão caso returnUrl seja nulo

            // Procure o usuário pelo número de telefone
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == Phone);

            if (user == null)
            {
                _notyf.Error("Usuário não cadastrado.");
                return View(new LoginViewModel { ReturnUrl = returnUrl });
            }

            // Tente fazer o login com o usuário encontrado
            var result = await _signInManager.PasswordSignInAsync(user.UserName, Password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                // Redirecione para a returnUrl, se válida, ou para a página inicial
                if (Url.IsLocalUrl(returnUrl))
                {
                    _notyf.Success("Seja bem-vindo!");
                    return Redirect(returnUrl);
                }
                else
                {
                    _notyf.Success("Seja bem-vindo!");
                    return RedirectToAction("Index", "Home");
                }
            }

            // Se falhar, mostre o erro
            _notyf.Error("Número de telefone ou senha inválidos.");
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }


        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {

            var loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(loginViewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(LoginViewModel registroVM)
        {
            //if (ModelState.IsValid)
            //{
            //    var user = new IdentityUser { UserName = registroVM.UserName };
            //    var result = await _userManager.CreateAsync(user, registroVM.Password);

            //    if (result.Succeeded)
            //    {
            //        await _userManager.AddToRoleAsync(user, "Member");
            //        return RedirectToAction("Login", "Account");
            //    }
            //    else
            //    {
            //        this.ModelState.AddModelError("Registro", "Falha ao registrar o usuário");
            //        //registroVM.ErrorMessage = "Falha ao registrar usuário!";
            //    }

            //}

            return View(registroVM);
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.User = null;
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
