using System.Globalization;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using petmypet.Context;
using ReflectionIT.Mvc.Paging;


var builder = WebApplication.CreateBuilder(args);

// ============================================
// Configuração padrão: Adiciona os serviços essenciais
// ============================================
// Essa linha adiciona os serviços de controle da aplicação, como MVC e outros que a aplicação precisa
builder.Services.AddControllersWithViews();

// ============================================
// Configuração customizada: Conexão com banco de dados MySQL
// ============================================
// Configura o Entity Framework para usar MySQL, com a versão específica do servidor MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 25))));

// ============================================
// Configuração customizada: Identity
// ============================================
// Adiciona os serviços de autenticação e autorização usando Identity com suporte a roles e tokens
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// ============================================
// Configuração customizada: Política de senha no Identity
// ============================================
// Configurações personalizadas para simplificar a criação de senhas para usuários
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false; // Remove necessidade de número
    options.Password.RequireLowercase = false; // Remove necessidade de letra minúscula
    options.Password.RequireUppercase = false; // Remove necessidade de letra maiúscula
    options.Password.RequireNonAlphanumeric = false; // Remove necessidade de caracteres especiais
    options.Password.RequiredLength = 1;  // Define o tamanho mínimo da senha como 1
    options.Password.RequiredUniqueChars = 1; // Define apenas 1 caractere único como necessário
});

// ============================================
// Configuração customizada: Repositórios
// ============================================
// Registra os repositórios que serão injetados nos controladores, usando o padrão de injeção de dependência

//builder.Services.AddTransient<IProdutoRepository, ProdutoRepository>();
//builder.Services.AddTransient<ICategoriaRepository, CategoriaRepository>();
//builder.Services.AddTransient<IClienteRepository, ClienteRepository>();
//builder.Services.AddTransient<ICaixaRepository, CaixaRepository>();
//builder.Services.AddTransient<IFormaPagamentoRepository, FormaPagamentoRepository>();
//builder.Services.AddTransient<IVendaRepository, VendaRepository>();
//builder.Services.AddScoped<ICarrinhoCompraRepository, CarrinhoCompraRepository>();
//builder.Services.AddScoped<ICrediarioRepository, CrediarioRepository>();
//builder.Services.AddScoped<ISeedUserRoleInitial, SeedUserRoleInitial>(); // Serviço para criar perfis de usuário e roles iniciais

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 3;
    config.IsDismissable = true;
    config.Position = NotyfPosition.BottomRight;
});

// ============================================
// Configuração customizada: Políticas de autorização
// ============================================
// Cria uma política de autorização onde apenas usuários com o perfil "Admin" podem acessar certas áreas
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", politica => politica.RequireRole("Admin"));
});

// ============================================
// Configuração customizada: Acesso ao carrinho de compras
// ============================================
// O método GetCarrinho retorna o carrinho de compras da sessão do usuário
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//builder.Services.AddScoped(sp => CarrinhoCompra.GetCarrinho(sp));

// ============================================
// Configuração padrão: Habilitação de paginação
// ============================================
// Adiciona suporte para paginação, com estilo de paginação Bootstrap 4 e configura o nome do parâmetro da página
builder.Services.AddPaging(options =>
{
    options.ViewName = "bootstrap4"; // Define o estilo de paginação
    options.PageParameterName = "pageindex"; // Define o parâmetro da URL que indicará a página
});

// ============================================
// Configuração customizada: Cache e Sessão
// ============================================
// Habilita o uso de cache na memória para melhorar o desempenho
builder.Services.AddMemoryCache();
// Habilita o uso de sessões para manter dados temporários entre requisições
builder.Services.AddSession();

var app = builder.Build();

app.UseNotyf();

// ============================================
// Configuração de localização
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
// Configuração customizada: Seed de usuários e roles
// ============================================
// Inicializa as roles de usuário (Admin, etc.) e cria usuários iniciais
//CriarPerfisUsuarios(app);

// ============================================
// Configuração customizada: Sessão
// ============================================
// Ativa o uso de sessões na aplicação
app.UseSession();

// ============================================
// Configuração padrão: Autenticação e autorização
// ============================================
// Habilita o uso de autenticação (verificação de identidade) e autorização (controle de acesso)
app.UseAuthentication();
app.UseAuthorization();

// ============================================
// Configuração customizada: Rotas
// ============================================
// Define as rotas personalizadas
//app.MapControllerRoute(
//    name: "areas",
//    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

//void CriarPerfisUsuarios(WebApplication app)
//{
//    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();
//    using (var scope = scopedFactory.CreateScope())
//    {
//        var service = scope.ServiceProvider.GetService<ISeedUserRoleInitial>();
//        service.SeedRoles();
//        service.SeedUsers();

//    }
//}

