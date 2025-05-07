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
            // Normalize a returnUrl para a raiz se for equivalente a "/"
            if (string.IsNullOrEmpty(returnUrl) || returnUrl == "/" || returnUrl == "/Account/Login?returnUrl=%2F")
            {
                returnUrl = Url.Content("~/");
            }

            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/"); // URL padrão caso returnUrl seja nulo

            // Verifique se o modelo está válido
            if (!ModelState.IsValid)
            {
                return View(model); // Retorne a view com os erros de validação
            }

            // Procure o usuário pelo número de telefone
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.Phone);

            if (user == null)
            {
                _notyf.Error("Usuário não cadastrado.");
                return View(model); // Retorna a view com a mensagem de erro
            }

            // Tente fazer o login com o usuário encontrado
            var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);

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
            return View(model); // Retorna a view com a mensagem de erro
        }


        [AllowAnonymous]
        public IActionResult Register()
        {

            var registerViewModel = new RegisterViewModel();
            return View(registerViewModel);

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Criar o novo usuário com os dados do modelo
                var user = new ApplicationUser
                {
                    UserName = model.Telefone,
                    PhoneNumber = model.Telefone,
                    Nome = model.Nome,
                    Endereco = model.Endereco,
                    Bairro = model.Bairro,
                    Ativo = true
                };

                // Criar o usuário no banco de dados
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Adicionar o usuário à role "Member"
                    var roleResult = await _userManager.AddToRoleAsync(user, "Member");

                    if (!roleResult.Succeeded)
                    {
                        _notyf.Error("Erro ao cadastrar usuário.");
                        return View(model); // Retorna a view com os erros, se houverem
                    }

                    // Fazer login do usuário
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }

                _notyf.Error("Erro ao cadastrar usuário.");

            }

            _notyf.Warning("Preencha os campos corretamente.");

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.User = null;
            await _signInManager.SignOutAsync();
            // Redireciona para a página de Login após o logout
            return RedirectToAction("Login", "Account");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        // Lista todos os usuários
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }

        // GET: Editar usuário
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Nome = user.Nome,
                Telefone = user.PhoneNumber,
                Endereco = user.Endereco,
                Bairro = user.Bairro,
                Role = roles.FirstOrDefault(),
                Ativo = user.Ativo
            };

            return View(model);
        }

        // POST: Editar usuário
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.Nome = model.Nome;
            user.PhoneNumber = model.Telefone;
            user.UserName = model.Telefone;
            user.Endereco = model.Endereco;
            user.Bairro = model.Bairro;
            user.Ativo = model.Ativo;

            var userRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles);

            if (!string.IsNullOrEmpty(model.Role))
            {
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                _notyf.Success("Usuário atualizado com sucesso.");
                return RedirectToAction(nameof(Index));
            }

            _notyf.Error("Erro ao atualizar usuário.");
            return View(model);
        }

        // GET: Confirmar exclusão
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            return View(user);
        }

        // POST: Confirmar exclusão
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                _notyf.Success("Usuário excluído com sucesso.");
                return RedirectToAction(nameof(Index));
            }

            _notyf.Error("Erro ao excluir usuário.");
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new ResetPasswordViewModel
            {
                UserId = user.Id
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound();

            // Remove a senha atual, se houver
            var removePassword = await _userManager.RemovePasswordAsync(user);
            if (!removePassword.Succeeded)
            {
                _notyf.Error("Erro ao remover senha atual.");
                return View(model);
            }

            // Adiciona a nova senha
            var addPassword = await _userManager.AddPasswordAsync(user, model.NewPassword);
            if (addPassword.Succeeded)
            {
                _notyf.Success("Senha redefinida com sucesso.");
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in addPassword.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            _notyf.Error("Erro ao redefinir senha.");
            return View(model);
        }


    }
}
