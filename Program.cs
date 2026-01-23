using MinimalApi.Infraestrutura.Db;
using MinimalApi.DTOs;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Servicos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Entidades;
using MinhaMinimalApi.Dominio.Enuns;


#region builder
var builder = WebApplication.CreateBuilder(args);

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
app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.Login(loginDTO);
    if (administrador != null)
        return Results.Ok("Login realizado com sucesso!");
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
}).WithTags("Administradores");


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
}).WithTags("Administradores");

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
}).WithTags("Administradores");

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
}).WithTags("Veículos");


app.MapGet("/veiculos", ([FromQuery]int? pagina, VeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina);
    return Results.Ok(veiculos);
}).WithTags("Veículos");




app.MapGet("/veiculos/{id}", ([FromRoute]int id, IVeiculosServico veiculoServico) =>
{
    var veiculos = veiculoServico.BuscarPorId(id);

    if (veiculos == null)
        return Results.NotFound();
    return Results.Ok(veiculos);
}).WithTags("Veículos");



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
}).WithTags("Veículos");





app.MapDelete("/veiculos/{id}", ([FromRoute]int id,  IVeiculosServico veiculoServico) =>
{
    var veiculos = veiculoServico.BuscarPorId(id);

    if (veiculos == null)
        return Results.NotFound();


    veiculoServico.Deletar(veiculos);
    return Results.NoContent();
}).WithTags("Veículos");

#endregion


#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion