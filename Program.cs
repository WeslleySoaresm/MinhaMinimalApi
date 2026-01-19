using MinimalApi.Infraestrutura.Db;
using MinimalApi.DTOs;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Servicos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Entidades;


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

// Criar um novo veículo 
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, VeiculoServico veiculoServico) =>
{
    //VALIDAÇÃO
    var validacao = validaDTO(veiculoDTO);
    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);  
    veiculoServico.Incluir(new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    });
    return Results.Created($"/veiculos/{veiculoDTO.Id}", veiculoDTO);
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