using Crm.Commands.API.Services;
using Crm.Commands.Supervisors;
using Crm.Commands.Managers;
using Crm.Commands.Clients;
using Crm.Messages.Bus;
using Crm.Commands.Data;
using Crm.Messages.Bus.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("Default");

builder.Services.AddGrpc();
builder.Services.LoadDataModule(connectionString);
builder.Services.LoadClientModule();
builder.Services.LoadManagerModule();
builder.Services.LoadSupervisorModule();
builder.Services.LoadMessageBusModule(
    new MessageConfiguration(),
    typeof(ClientModule).Assembly,
    typeof(SupervisorModule).Assembly,
    typeof(ManagerModule).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCleanDatabase();
app.MapGrpcService<ManagerService>();
app.MapGrpcService<SupervisorService>();

app.Run();
