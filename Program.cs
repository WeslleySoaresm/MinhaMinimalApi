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

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, VeiculoServico veiculoServico) =>
{
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


app.MapGet("/veiculos/{id}", ([FromRoute]int id, VeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.BuscarPorId(id);

    if (veiculos == null)
        return Results.NotFound();
    return Results.Ok(veiculos);
}).WithTags("Veículos");


#endregion


#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion