using EventoWeb.Comum.Aplicacao.Eventos;
using EventoWeb.Comum.Aplicacao.FormasPagamento;
using EventoWeb.Comum.Aplicacao.Inscricoes;
using EventoWeb.Comum.Aplicacao.Pedidos;
using EventoWeb.Comum.Aplicacao.Precos;
using EventoWeb.Comum.Negocio.Entidades;
using EventoWeb.Comum.Negocio.Entidades.Financeiro;
using EventoWeb.Comum.Negocio.Entidades.IntegracaoFinanceira;
using EventoWeb.Comum.Negocio.Repositorios;
using EventoWeb.Comum.Negocio.Servicos;
using EventoWeb.Comum.Negocio.Servicos.Notificacoes.Inscricoes;
using EventoWeb.Comum.Persistencia.Integracoes.Asaas;
using EventoWeb.Comum.Persistencia.Mapeamentos;
using EventoWeb.Comum.Persistencia.MigracoesBD;
using EventoWeb.Comum.Persistencia.Repositorios;
using EventoWeb.Secretaria.Aplicacao.Contas;
using EventoWeb.Secretaria.Aplicacao.ContasBancarias;
using EventoWeb.Secretaria.Aplicacao.Inscricoes;
using EventoWeb.Secretaria.Aplicacao.Pedidos;
using EventoWeb.Secretaria.Aplicacao.RegistrosIntegracao;
using EventoWeb.Secretaria.Aplicacao.Seguranca;
using EventoWeb.Secretaria.Negocio.Repositorios;
using EventoWeb.Secretaria.Negocio.Servicos.Contas;
using EventoWeb.Secretaria.Negocio.Servicos.Notificacoes.Pagamentos;
using EventoWeb.Secretaria.Negocio.Servicos.RegistroIntegracao;
using EventoWeb.Secretaria.Persistencia.Mapeamentos;
using EventoWeb.Secretaria.Persistencia.MigracoesBD;
using EventoWeb.Secretaria.Persistencia.Repositorios;
using EventoWeb.Secretaria.Negocio.Entidades.Atividades;
using EventoWeb.Secretaria.Relatorios.Aplicacao.Interfaces;
using EventoWeb.Secretaria.Relatorios.Aplicacao.Servicos;
using EventoWeb.Secretaria.Relatorios.Relatorios.Implementacoes;
using eventoweb_secretaria_back;
using eventoweb_secretaria_back.Logging;
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Mapping.ByCode;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ContentRootPath = AppContext.BaseDirectory,
    Args = args,
    ApplicationName = System.Diagnostics.Process.GetCurrentProcess().ProcessName
});

var configuracao = new ConfiguracaoAutorizacao();
builder.Services.AddSingleton(configuracao);

builder.Services.AddControllers();

var databaseSection = builder.Configuration.GetSection("Database");
var connectionString = databaseSection.GetValue<string>("ConnectionString")
                        ?? throw new InvalidOperationException("Database:ConnectionString nao configurado.");
var nhDialect = databaseSection.GetValue<string>("Dialect")
               ?? "NHibernate.Dialect.MySQL5Dialect";
var nhDriver = databaseSection.GetValue<string>("Driver")
              ?? "NHibernate.Driver.MySqlDataDriver";

var logFilePath = builder.Configuration["Logging:File:Path"];
if (!string.IsNullOrWhiteSpace(logFilePath))
{
    var contentRoot = builder.Environment.ContentRootPath;
    var fullPath = Path.IsPathRooted(logFilePath)
        ? logFilePath
        : Path.Combine(contentRoot, logFilePath);
    builder.Logging.AddProvider(new FileLoggerProvider(fullPath));
}

builder.Services
    .AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddMySql5()
        .WithGlobalConnectionString(connectionString)
        .ScanIn([
            typeof(Migracao01).Assembly,
            typeof(UserMigration).Assembly
        ]).For.Migrations())
    .AddLogging(lb => lb.AddFluentMigratorConsole());

builder.Services.AddSingleton<ISessionFactory>(_ =>
{
    NHibernate.Cfg.Environment.UseReflectionOptimizer = false;
    var configuration = new Configuration();
    configuration.SetProperty(NHibernate.Cfg.Environment.ConnectionProvider,
        "NHibernate.Connection.DriverConnectionProvider");
    configuration.SetProperty(NHibernate.Cfg.Environment.ConnectionString, connectionString);
    configuration.SetProperty(NHibernate.Cfg.Environment.Dialect, nhDialect);
    configuration.SetProperty(NHibernate.Cfg.Environment.ConnectionDriver, nhDriver);
    var mapper = new ModelMapper();
    var mappingAssemblyComum = typeof(InscricaoMapping).Assembly;
    var mappingAssemblySecretaria = typeof(UsuarioMapping).Assembly;
    mapper.AddMappings(mappingAssemblyComum.GetExportedTypes());
    mapper.AddMappings(mappingAssemblySecretaria.GetExportedTypes());
    configuration.AddMapping(mapper.CompileMappingForAllExplicitlyAddedEntities());

    return configuration.BuildSessionFactory();
});

builder.Services.AddSingleton<IDictionary<EnumIntegracaoExterna, IIntegracaoExterna>, Dictionary<EnumIntegracaoExterna, IIntegracaoExterna>>(provider =>
{
    var dict = new Dictionary<EnumIntegracaoExterna, IIntegracaoExterna>()
    {
        { EnumIntegracaoExterna.Asaas, new IntegracaoFinanceiraAsaas() }
    };

    return dict;
});

builder.Services.AddScoped((provider) => {
    var factory = provider.GetService<ISessionFactory>() ??
                  throw new ArgumentNullException(nameof(ISessionFactory));

    return new ContextoSecretariaNH(factory.OpenSession());
});

builder.Services.AddScoped<IContexto>(p => p.GetRequiredService<ContextoSecretariaNH>());
builder.Services.AddScoped(p => p.GetRequiredService<ContextoSecretariaNH>().Inscricoes);
builder.Services.AddScoped(p => p.GetRequiredService<ContextoSecretariaNH>().Pessoas);
builder.Services.AddScoped(p => p.GetRequiredService<ContextoSecretariaNH>().Eventos);
builder.Services.AddScoped(p => p.GetRequiredService<ContextoSecretariaNH>().PrecosInscricao);
builder.Services.AddScoped(p => p.GetRequiredService<ContextoSecretariaNH>().Pedidos);
builder.Services.AddScoped(p => p.GetRequiredService<ContextoSecretariaNH>().FormasPagamento);
builder.Services.AddScoped(p => p.GetRequiredService<ContextoSecretariaNH>().IntegracoesFinanceirasPorFormasPagamento);
builder.Services.AddScoped(p => p.GetRequiredService<ContextoSecretariaNH>().RegistrosIntegracoesFinanceiras);
builder.Services.AddScoped(p => p.GetRequiredService<ContextoSecretariaNH>().ModelosMensagemNotificacao);
builder.Services.AddScoped(p => p.GetRequiredService<ContextoSecretariaNH>().MensagensNotificacao);
builder.Services.AddScoped(p => p.GetRequiredService<ContextoSecretariaNH>().Usuarios);
builder.Services.AddScoped(p => p.GetRequiredService<ContextoSecretariaNH>().Contas);
builder.Services.AddScoped(p => p.GetRequiredService<ContextoSecretariaNH>().ContasBancarias);
builder.Services.AddScoped(p => p.GetRequiredService<ContextoSecretariaNH>().Atividades);

builder.Services.AddScoped<IEnumerable<IValidacao<Inscricao>>>(provider =>
{
    var validacoes = new List<IValidacao<Inscricao>>()
    {
        new ValidacaoInscricaoDadosPessoa(),
        new ValidacaoInscricaoPessoaJaInscrita(provider.GetRequiredService<IInscricoes>())
    };

    return validacoes;
});


builder.Services.AddScoped<SrvLiquidacaoConta>();
builder.Services.AddScoped<SrvCriacaoRegistroIntegracao>();
builder.Services.AddScoped<SrvConsultaRegistroIntegracao>();
builder.Services.AddScoped<SrvNotificacaoNovoPagamento>();
builder.Services.AddScoped<SrvNotificacaoInscricao>();

builder.Services.AddScoped<AppEventoListagem>();
builder.Services.AddScoped<AppEventoCalcularIdade>();
builder.Services.AddScoped<AppEventoObtencao>();
builder.Services.AddScoped<AppContaBancariaListagem>();
builder.Services.AddScoped<AppInscricaoInclusao>();
builder.Services.AddScoped<AppInscricaoAtualizacao>();
builder.Services.AddScoped<AppInscricaoAtualizacaoSituacao>();
builder.Services.AddScoped<AppInscricaoObtencao>();
builder.Services.AddScoped<AppInscricaoPesquisaPessoa>();
builder.Services.AddScoped<AppInscricaoListagem>();
builder.Services.AddScoped<AppInscricaoInclusao>();
builder.Services.AddScoped<AppInscricaoAtualizacao>();
builder.Services.AddScoped<AppInscricaoObtencao>();
builder.Services.AddScoped<AppInscricaoPesquisaPessoa>();
builder.Services.AddScoped<AppPrecoInscricaoObtencaoIdade>();
builder.Services.AddScoped<AppPedidoInclusao>();
builder.Services.AddScoped<AppPedidoObtencao>();
builder.Services.AddScoped<AppPedidoInclusao>();
builder.Services.AddScoped<AppFormasPagamentoListagem>();
builder.Services.AddScoped<AppRegistroIntegracaoObtencao>();
builder.Services.AddScoped<AppRegistroIntegracaoInclusao>();
builder.Services.AddScoped<AppRegistroIntegracaoConsulta>();
builder.Services.AddScoped<AppContaLiquidacao>();
builder.Services.AddScoped<AppUsuarioAutenticacao>();

// Relatórios
builder.Services.AddScoped<IRelatorioGerador<Atividade>, DivisaoAtividadeRelatorio>();
builder.Services.AddScoped<AppDivisaoAtividadeRelatorio>();

builder.Services.AddAuthentication(authOptions =>
{
    authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(bearerOptions =>
{
    var paramsValidation = bearerOptions.TokenValidationParameters;
    paramsValidation.IssuerSigningKey = configuracao.ChaveEmissor;
    paramsValidation.ValidAudience = configuracao.Publico;
    paramsValidation.ValidIssuer = configuracao.Emissor;

    // Valida a assinatura de um token recebido
    paramsValidation.ValidateIssuerSigningKey = true;

    // Verifica se um token recebido ainda é válido
    paramsValidation.ValidateLifetime = true;

    // Tempo de tolerância para a expiração de um token (utilizado
    // caso haja problemas de sincronismo de horário entre diferentes
    // computadores envolvidos no processo de comunicação)
    paramsValidation.ClockSkew = TimeSpan.Zero;
});

// Ativa o uso do token como forma de autorizar o acesso
// a recursos deste projeto
builder.Services.AddAuthorization(auth =>
{
    auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
        .RequireAuthenticatedUser().Build());
});

builder.Services.AddCors();

builder.Services.AddSpaStaticFiles(configuration =>
{
    configuration.RootPath = "ClientApp";
});


var app = builder.Build();

app.UseStaticFiles();
app.UseSpaStaticFiles();

using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();
}

app.Services.GetService<ISessionFactory>();

app.UseCors(builder => builder
    .AllowAnyHeader()
    .AllowAnyMethod()
    .SetIsOriginAllowed(_ => true)
    .AllowCredentials()
);

//app.UseHttpsRedirection();
app.MapControllers();
app.ConfigureExceptionHandler();

app.UseSpa(spa =>
{
    spa.Options.SourcePath = "/ClientApp";
});

app.Run();
