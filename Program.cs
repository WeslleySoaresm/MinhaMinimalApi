using MinimalApi.Infraestrutura.Db;
using MinimalApi.DTOs;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Servicos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Entidades;
using MinhaMinimalApi.Dominio.Enuns;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;


#region builder
var builder = WebApplication.CreateBuilder(args);

// 1. Extração segura da chave
var jwtKey = builder.Configuration["Jwt:Key"]; // Ajuste conforme seu appsettings.json
if (string.IsNullOrEmpty(jwtKey)) jwtKey = "101520";


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => // Removi a string "JwtBearerDefaults", use o padrão
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false, // Se definir como true, precisa configurar ValidIssuer
        ValidateAudience = false, // Se definir como true, precisa configurar ValidAudience
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero // Remova o delay de 5min para testes precisos
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculosServico, VeiculoServico>();
builder.Services.AddScoped<VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<DbContexto>(Options => 
    Options.UseMySql(
        builder.Configuration.GetConnectionString("Mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Mysql"))
    )
    );

builder.Services.AddOpenApi();

var app = builder.Build();
#endregion


#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion


#region Administrador


String GerarTokenJwt(Administrador administrador, string chaveSecreta)
{
    if(String.IsNullOrEmpty(jwtKey)) return string.Empty;
    var tokenHandler = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chaveSecreta));
    var credentials = new SigningCredentials(tokenHandler, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        claims: new[]
        {
            new System.Security.Claims.Claim("id", administrador.Id.ToString()),
            new System.Security.Claims.Claim("email", administrador.Email),
            new System.Security.Claims.Claim("perfil", administrador.Perfil)
        },
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: credentials
    );
    Console.WriteLine($"Tamanho da chave lida: {chaveSecreta.Length} caracteres");
    return new JwtSecurityTokenHandler().WriteToken(token);
}


app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.Login(loginDTO);

    
    if (administrador != null){
        string token = GerarTokenJwt(administrador, jwtKey);
        
        return Results.Ok(new AdministradorLogado
        {
            Email = administrador.Email,
            Perfil = administrador.Perfil,
            Token = token
        });
    }
    else
        return Results.Unauthorized();
}).WithTags("Administradores");

app.MapPost("/Administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
{
    // 1. Validação (Deveria estar em um Validator, mas manteremos aqui para simplificar seu aprendizado)
    var validacao = new ErrosDeValidacao { Mensagens = new List<string>() };
    if (string.IsNullOrEmpty(administradorDTO.Email)) validacao.Mensagens.Add("O email é obrigatório.");
    if (string.IsNullOrEmpty(administradorDTO.Senha)) validacao.Mensagens.Add("A senha é obrigatória.");
    
    if (validacao.Mensagens.Count > 0) return Results.BadRequest(validacao);

    // 2. Mapeamento para Entidade
    var novoAdmin = new Administrador
    {
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
    };

    // 3. Persistência
    administradorServico.Incluir(novoAdmin); // Aqui o 'novoAdmin' ganha um ID do banco

    // 4. Retorno correto usando a instância 'novoAdmin'
    return Results.Created($"/administradores/{novoAdmin.Id}", novoAdmin);
}).RequireAuthorization().WithTags("Administradores");


app.MapGet("/Administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
{
    var adms = new List<AdministradorModelView>();
    var administradores = administradorServico.Todos(pagina);
    foreach (var administrador in administradores)
    {
        adms.Add(new AdministradorModelView
        {   
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });
    }
    return Results.Ok(adms);
}).RequireAuthorization().WithTags("Administradores");

app.MapGet("/Administradores/{id}", ([FromRoute]int id, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.BuscarPorId(id);

    if (administrador == null)
        return Results.NotFound();
    return Results.Ok(new AdministradorModelView
        {   
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });
}).RequireAuthorization().WithTags("Administradores");

#endregion


#region veiculos

ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosDeValidacao
    {
        Mensagens = new List<string>()
    };
   
    if (string.IsNullOrEmpty(veiculoDTO.Nome))
        validacao.Mensagens.Add("O nome do veículo é obrigatório.");
        
    if (string.IsNullOrEmpty(veiculoDTO.Marca))
        validacao.Mensagens.Add("A marca do veículo é obrigatória.");

    if (veiculoDTO.Ano <= 0)
        validacao.Mensagens.Add("O ano do veículo deve ser um número positivo."); 
    
    return validacao;
};

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculosServico veiculoServico) =>
{
    var validacao = validaDTO(veiculoDTO);
    if (validacao.Mensagens.Count > 0) return Results.BadRequest(validacao);

    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };

    veiculoServico.Incluir(veiculo);

    // Use 'veiculo.Id' (instância criada), não 'veiculoDTO.Id' (objeto de entrada)
    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
}).RequireAuthorization().WithTags("Veículos");


app.MapGet("/veiculos", ([FromQuery]int? pagina, VeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina);
    return Results.Ok(veiculos);
}).RequireAuthorization().WithTags("Veículos");




app.MapGet("/veiculos/{id}", ([FromRoute]int id, IVeiculosServico veiculoServico) =>
{
    var veiculos = veiculoServico.BuscarPorId(id);

    if (veiculos == null)
        return Results.NotFound();
    return Results.Ok(veiculos);
}).RequireAuthorization().WithTags("Veículos");



app.MapPut("/veiculos/{id}", ([FromRoute]int id, VeiculoDTO veiculoDTO, IVeiculosServico veiculoServico) =>
{
    var veiculos = veiculoServico.BuscarPorId(id);

    if (veiculos == null)
        return Results.NotFound();

    //VALIDAÇÃO
    var validacao = validaDTO(veiculoDTO);
    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);  

    

    veiculos.Nome = veiculoDTO.Nome;
    veiculos.Marca = veiculoDTO.Marca;
    veiculos.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculos);
    return Results.Ok(veiculos);
}).RequireAuthorization().WithTags("Veículos");





app.MapDelete("/veiculos/{id}", ([FromRoute]int id,  IVeiculosServico veiculoServico) =>
{
    var veiculos = veiculoServico.BuscarPorId(id);

    if (veiculos == null)
        return Results.NotFound();


    veiculoServico.Deletar(veiculos);
    return Results.NoContent();
}).RequireAuthorization().WithTags("Veículos");

#endregion


#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();    
app.UseAuthorization();

app.Run();
#endregion