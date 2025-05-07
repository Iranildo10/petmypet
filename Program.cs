using System.Globalization;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using petmypet.Context;
using petmypet.Models;
using petmypet.Services;


var builder = WebApplication.CreateBuilder(args);

// ============================================
// Configura��o padr�o: Adiciona os servi�os essenciais
// ============================================
// Essa linha adiciona os servi�os de controle da aplica��o, como MVC e outros que a aplica��o precisa
builder.Services.AddControllersWithViews();

// ============================================
// Configura��o customizada: Conex�o com banco de dados MySQL
// ============================================
// Configura o Entity Framework para usar MySQL, com a vers�o espec�fica do servidor MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 25))));

// ============================================
// Configura��o customizada: Identity
// ============================================
// Adiciona os servi�os de autentica��o e autoriza��o usando Identity com suporte a roles e tokens
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ============================================
// Configura��o customizada: Pol�tica de senha no Identity
// ============================================
// Configura��es personalizadas para simplificar a cria��o de senhas para usu�rios
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false; // Remove necessidade de n�mero
    options.Password.RequireLowercase = false; // Remove necessidade de letra min�scula
    options.Password.RequireUppercase = false; // Remove necessidade de letra mai�scula
    options.Password.RequireNonAlphanumeric = false; // Remove necessidade de caracteres especiais
    options.Password.RequiredLength = 1;  // Define o tamanho m�nimo da senha como 1
    options.Password.RequiredUniqueChars = 0; // Define apenas 1 caractere �nico como necess�rio
});

// ============================================
// Configura��o customizada: Reposit�rios
// ============================================
// Registra os reposit�rios que ser�o injetados nos controladores, usando o padr�o de inje��o de depend�ncia

//builder.Services.AddTransient<IProdutoRepository, ProdutoRepository>();
//builder.Services.AddTransient<ICategoriaRepository, CategoriaRepository>();
//builder.Services.AddTransient<IClienteRepository, ClienteRepository>();
//builder.Services.AddTransient<ICaixaRepository, CaixaRepository>();
//builder.Services.AddTransient<IFormaPagamentoRepository, FormaPagamentoRepository>();
//builder.Services.AddTransient<IVendaRepository, VendaRepository>();
//builder.Services.AddScoped<ICarrinhoCompraRepository, CarrinhoCompraRepository>();
//builder.Services.AddScoped<ICrediarioRepository, CrediarioRepository>();
builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>(); // Servi�o para criar perfis de usu�rio e roles iniciais

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 3;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight; // Define a posi��o no topo � direita
});

// ============================================
// Configura��o customizada: Pol�ticas de autoriza��o
// ============================================
// Cria uma pol�tica de autoriza��o onde apenas usu�rios com o perfil "Admin" podem acessar certas �reas
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", politica => politica.RequireRole("Admin"));
});

// ============================================
// Configura��o customizada: Acesso ao carrinho de compras
// ============================================
// O m�todo GetCarrinho retorna o carrinho de compras da sess�o do usu�rio
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//builder.Services.AddScoped(sp => CarrinhoCompra.GetCarrinho(sp));

// ============================================
// Configura��o customizada: Cache e Sess�o
// ============================================
// Habilita o uso de cache na mem�ria para melhorar o desempenho
builder.Services.AddMemoryCache();
// Habilita o uso de sess�es para manter dados tempor�rios entre requisi��es
builder.Services.AddSession();

builder.Configuration.AddEnvironmentVariables();

var app = builder.Build();

app.UseNotyf();

// ============================================
// Configura��o de localiza��o
// ============================================
var supportedCultures = new[]
{
    new CultureInfo("pt-BR"),
    new CultureInfo("en-US")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("pt-BR"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ============================================
// Configura��o customizada: Seed de usu�rios e roles
// ============================================
// Inicializa as roles de usu�rio (Admin, etc.) e cria usu�rios iniciais
CriarPerfisUsuarios(app);

// ============================================
// Configura��o customizada: Sess�o
// ============================================
// Ativa o uso de sess�es na aplica��o
app.UseSession();

// ============================================
// Configura��o padr�o: Autentica��o e autoriza��o
// ============================================
// Habilita o uso de autentica��o (verifica��o de identidade) e autoriza��o (controle de acesso)
app.UseAuthentication();
app.UseAuthorization();

// ============================================
// Configura��o customizada: Rotas
// ============================================
// Define as rotas personalizadas
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Agendamentos}/{action=Index}/{id?}");

app.Run();

void CriarPerfisUsuarios(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<ISeedUserRoleInitial>();
        service.SeedRoles();
        service.SeedUsers();

    }
}

