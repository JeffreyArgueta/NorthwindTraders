using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.Application;
using NorthwindTraders.Application.Services;
using NorthwindTraders.Infrastructure;
using NorthwindTraders.Domain.Contracts;
using FluentValidation;
using FluentValidation.Results;
using System.Text.Json;
using NorthwindTraders.Api;
using NorthwindTraders.Application.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the DI container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register FluentValidation validators from the Application assembly
// Register validators explicitly (FluentValidation DI helper not referenced to keep dependencies minimal)
builder.Services.AddScoped<FluentValidation.IValidator<CreateSupplierDto>, NorthwindTraders.Application.Validation.CreateSupplierDtoValidator>();
builder.Services.AddScoped<FluentValidation.IValidator<UpdateSupplierDto>, NorthwindTraders.Application.Validation.UpdateSupplierDtoValidator>();
builder.Services.AddScoped<FluentValidation.IValidator<CreateCustomerDto>, NorthwindTraders.Application.Validation.CreateCustomerDtoValidator>();
builder.Services.AddScoped<FluentValidation.IValidator<UpdateCustomerDto>, NorthwindTraders.Application.Validation.UpdateCustomerDtoValidator>();
builder.Services.AddScoped<FluentValidation.IValidator<CreateEmployeeDto>, NorthwindTraders.Application.Validation.CreateEmployeeDtoValidator>();
builder.Services.AddScoped<FluentValidation.IValidator<UpdateEmployeeDto>, NorthwindTraders.Application.Validation.UpdateEmployeeDtoValidator>();
builder.Services.AddScoped<FluentValidation.IValidator<CreateShipperDto>, NorthwindTraders.Application.Validation.CreateShipperDtoValidator>();
builder.Services.AddScoped<FluentValidation.IValidator<UpdateShipperDto>, NorthwindTraders.Application.Validation.UpdateShipperDtoValidator>();
builder.Services.AddScoped<FluentValidation.IValidator<CreateOrderDto>, NorthwindTraders.Application.Validation.CreateOrderDtoValidator>();
builder.Services.AddScoped<FluentValidation.IValidator<UpdateOrderDto>, NorthwindTraders.Application.Validation.UpdateOrderDtoValidator>();
builder.Services.AddScoped<FluentValidation.IValidator<CreateOrderDetailDto>, NorthwindTraders.Application.Validation.CreateOrderDetailDtoValidator>();
builder.Services.AddScoped<FluentValidation.IValidator<UpdateOrderDetailDto>, NorthwindTraders.Application.Validation.UpdateOrderDetailDtoValidator>();
builder.Services.AddScoped<FluentValidation.IValidator<CategoryDto>, NorthwindTraders.Application.Validation.CreateCategoryDtoValidator>();
builder.Services.AddScoped<FluentValidation.IValidator<EmployeeTerritoryDto>, NorthwindTraders.Application.Validation.CreateEmployeeTerritoryDtoValidator>();
// duplicate registrations for product/update already covered above

// Register Application and Infrastructure layers
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")!;
builder.Services.AddApplication();
builder.Services.AddNorthwindInfrastructure(connectionString);

// CORS: configure allowed origins from ALLOWED_ORIGINS env var (comma-separated)
var allowedOriginsEnv = builder.Configuration["ALLOWED_ORIGINS"] ?? Environment.GetEnvironmentVariable("ALLOWED_ORIGINS");
var allowedOrigins = Array.Empty<string>();
if (!string.IsNullOrWhiteSpace(allowedOriginsEnv))
{
    allowedOrigins = allowedOriginsEnv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
    {
        if (allowedOrigins.Length == 0)
        {
            // deny all origins by default
            policy.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(_ => false);
        }
        else
        {
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Default");

// Global exception handling - convert errors to RFC7807-like ProblemDetails without sensitive info
app.UseExceptionHandler(errApp =>
{
    errApp.Run(async context =>
    {
        context.Response.ContentType = "application/problem+json";
        var ex = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        var detail = app.Environment.IsDevelopment() ? ex?.ToString() : "An unexpected error occurred.";
        var problem = new
        {
            type = "https://example.com/probs/internal",
            title = "Internal Server Error",
            status = 500,
            detail
        };
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(problem);
    });
});

// Orders endpoints
app.MapGet("/api/orders", async (
    [FromServices] IOrderService service,
    CancellationToken ct,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 20) =>
{
    if (pageSize > 100) pageSize = 100;
    var paged = await service.GetAllAsync(page, pageSize, ct);
    return Results.Ok(paged);
})
.WithName("GetAllOrders")
.WithOpenApi()
.WithTags("Orders");

app.MapGet("/api/orders/{id}", async ([FromRoute] int id, [FromServices] IOrderService service, CancellationToken ct) =>
{
    var o = await service.GetByIdAsync(id, ct);
    return o == null ? Results.NotFound() : Results.Ok(o);
})
.WithName("GetOrderById")
.WithOpenApi()
.WithTags("Orders");

    app.MapPost("/api/orders", async ([FromBody] CreateOrderDto dto, [FromServices] IOrderService service, CancellationToken ct) =>
    {
        var created = await service.CreateAsync(dto, ct);
        return Results.Created($"/api/orders/{created.OrderID}", created);
    })
    .WithName("CreateOrder")
    .WithOpenApi()
    .WithTags("Orders")
    .AddEndpointFilter(async (context, next) => await new ValidatorEndpointFilter<CreateOrderDto>(context.HttpContext.RequestServices.GetService<FluentValidation.IValidator<CreateOrderDto>>()).InvokeAsync(context, next));

    app.MapPut("/api/orders/{id}", async ([FromRoute] int id, [FromBody] UpdateOrderDto dto, [FromServices] IOrderService service, CancellationToken ct) =>
    {
        var updated = await service.UpdateAsync(id, dto, ct);
        return Results.Ok(updated);
    })
    .WithName("UpdateOrder")
    .WithOpenApi()
    .WithTags("Orders")
    .AddEndpointFilter(async (context, next) => await new ValidatorEndpointFilter<UpdateOrderDto>(context.HttpContext.RequestServices.GetService<FluentValidation.IValidator<UpdateOrderDto>>()).InvokeAsync(context, next));

app.MapDelete("/api/orders/{id}", async ([FromRoute] int id, [FromServices] IOrderService service, CancellationToken ct) =>
{
    await service.DeleteAsync(id, ct);
    return Results.NoContent();
})
.WithName("DeleteOrder")
.WithOpenApi()
.WithTags("Orders");

// Order details
app.MapGet("/api/orders/{orderId}/details", async ([FromRoute] int orderId, [FromServices] IOrderDetailService service, CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 50) =>
{
    var paged = await service.GetByOrderIdAsync(orderId, page, pageSize, ct);
    return Results.Ok(paged);
})
.WithTags("OrderDetails")
.WithOpenApi();

app.MapGet("/api/orders/{orderId}/details/{productId}", async ([FromRoute] int orderId, [FromRoute] int productId, [FromServices] IOrderDetailService service, CancellationToken ct) =>
{
    var od = await service.GetByIdAsync(orderId, productId, ct);
    return od == null ? Results.NotFound() : Results.Ok(od);
})
.WithTags("OrderDetails")
.WithOpenApi();

    app.MapPost("/api/orders/{orderId}/details", async ([FromRoute] int orderId, [FromBody] CreateOrderDetailDto dto, [FromServices] IOrderDetailService service, CancellationToken ct) =>
    {
        var created = await service.CreateAsync(orderId, dto, ct);
        return Results.Created($"/api/orders/{orderId}/details/{created.ProductID}", created);
    })
    .WithTags("OrderDetails")
    .WithOpenApi()
    .AddEndpointFilter(async (context, next) => await new ValidatorEndpointFilter<CreateOrderDetailDto>(context.HttpContext.RequestServices.GetService<FluentValidation.IValidator<CreateOrderDetailDto>>()).InvokeAsync(context, next));

    app.MapPut("/api/orders/{orderId}/details/{productId}", async ([FromRoute] int orderId, [FromRoute] int productId, [FromBody] UpdateOrderDetailDto dto, [FromServices] IOrderDetailService service, CancellationToken ct) =>
    {
        var updated = await service.UpdateAsync(orderId, productId, dto, ct);
        return Results.Ok(updated);
    })
    .WithTags("OrderDetails")
    .WithOpenApi()
    .AddEndpointFilter(async (context, next) => await new ValidatorEndpointFilter<UpdateOrderDetailDto>(context.HttpContext.RequestServices.GetService<FluentValidation.IValidator<UpdateOrderDetailDto>>()).InvokeAsync(context, next));

app.MapDelete("/api/orders/{orderId}/details/{productId}", async ([FromRoute] int orderId, [FromRoute] int productId, [FromServices] IOrderDetailService service, CancellationToken ct) =>
{
    await service.DeleteAsync(orderId, productId, ct);
    return Results.NoContent();
})
.WithTags("OrderDetails")
.WithOpenApi();

// Customers
app.MapGet("/api/customers", async ([FromServices] ICustomerService service, CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 20) =>
    {
        if (pageSize > 100) pageSize = 100;
        return Results.Ok(await service.GetAllAsync(page, pageSize, ct));
    })
.WithOpenApi()
.WithTags("Customers");

app.MapGet("/api/customers/{id}", async ([FromRoute] string id, [FromServices] ICustomerService service, CancellationToken ct) =>
{
    var c = await service.GetByIdAsync(id, ct);
    return c == null ? Results.NotFound() : Results.Ok(c);
})
.WithOpenApi()
.WithTags("Customers");

    app.MapPost("/api/customers", async ([FromBody] CreateCustomerDto dto, [FromServices] ICustomerService service, CancellationToken ct) =>
    {
        var created = await service.CreateAsync(dto, ct);
        return Results.Created($"/api/customers/{created.CustomerID}", created);
    })
    .WithOpenApi()
    .WithTags("Customers")
    .AddEndpointFilter(async (context, next) => await new ValidatorEndpointFilter<CreateCustomerDto>(context.HttpContext.RequestServices.GetService<FluentValidation.IValidator<CreateCustomerDto>>()).InvokeAsync(context, next));

    app.MapPut("/api/customers/{id}", async ([FromRoute] string id, [FromBody] UpdateCustomerDto dto, [FromServices] ICustomerService service, CancellationToken ct) =>
    {
        var updated = await service.UpdateAsync(id, dto, ct);
        return Results.Ok(updated);
    })
    .WithOpenApi()
    .WithTags("Customers")
    .AddEndpointFilter(async (context, next) => await new ValidatorEndpointFilter<UpdateCustomerDto>(context.HttpContext.RequestServices.GetService<FluentValidation.IValidator<UpdateCustomerDto>>()).InvokeAsync(context, next));

app.MapDelete("/api/customers/{id}", async ([FromRoute] string id, [FromServices] ICustomerService service, CancellationToken ct) =>
{
    await service.DeleteAsync(id, ct);
    return Results.NoContent();
})
.WithOpenApi()
.WithTags("Customers");

// Employees
app.MapGet("/api/employees", async ([FromServices] IEmployeeService service, CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 20) =>
    {
        if (pageSize > 100) pageSize = 100;
        return Results.Ok(await service.GetAllAsync(page, pageSize, ct));
    })
.WithOpenApi()
.WithTags("Employees");

app.MapGet("/api/employees/{id}", async ([FromRoute] int id, [FromServices] IEmployeeService service, CancellationToken ct) =>
{
    var e = await service.GetByIdAsync(id, ct);
    return e == null ? Results.NotFound() : Results.Ok(e);
})
.WithOpenApi()
.WithTags("Employees");

    app.MapPost("/api/employees", async ([FromBody] CreateEmployeeDto dto, [FromServices] IEmployeeService service, CancellationToken ct) =>
    {
        var created = await service.CreateAsync(dto, ct);
        return Results.Created($"/api/employees/{created.EmployeeID}", created);
    })
    .WithOpenApi()
    .WithTags("Employees")
    .AddEndpointFilter(async (context, next) => await new ValidatorEndpointFilter<CreateEmployeeDto>(context.HttpContext.RequestServices.GetService<FluentValidation.IValidator<CreateEmployeeDto>>()).InvokeAsync(context, next));

    app.MapPut("/api/employees/{id}", async ([FromRoute] int id, [FromBody] UpdateEmployeeDto dto, [FromServices] IEmployeeService service, CancellationToken ct) =>
    {
        var updated = await service.UpdateAsync(id, dto, ct);
        return Results.Ok(updated);
    })
    .WithOpenApi()
    .WithTags("Employees")
    .AddEndpointFilter(async (context, next) => await new ValidatorEndpointFilter<UpdateEmployeeDto>(context.HttpContext.RequestServices.GetService<FluentValidation.IValidator<UpdateEmployeeDto>>()).InvokeAsync(context, next));

app.MapDelete("/api/employees/{id}", async ([FromRoute] int id, [FromServices] IEmployeeService service, CancellationToken ct) =>
{
    await service.DeleteAsync(id, ct);
    return Results.NoContent();
})
.WithOpenApi()
.WithTags("Employees");

// Shippers
app.MapGet("/api/shippers", async ([FromServices] IShipperService service, CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 20) =>
    {
        if (pageSize > 100) pageSize = 100;
        return Results.Ok(await service.GetAllAsync(page, pageSize, ct));
    })
.WithOpenApi()
.WithTags("Shippers");

app.MapGet("/api/shippers/{id}", async ([FromRoute] int id, [FromServices] IShipperService service, CancellationToken ct) =>
{
    var s = await service.GetByIdAsync(id, ct);
    return s == null ? Results.NotFound() : Results.Ok(s);
})
.WithOpenApi()
.WithTags("Shippers");

    app.MapPost("/api/shippers", async ([FromBody] CreateShipperDto dto, [FromServices] IShipperService service, CancellationToken ct) =>
    {
        var created = await service.CreateAsync(dto, ct);
        return Results.Created($"/api/shippers/{created.ShipperID}", created);
    })
    .WithOpenApi()
    .WithTags("Shippers")
    .AddEndpointFilter(async (context, next) => await new ValidatorEndpointFilter<CreateShipperDto>(context.HttpContext.RequestServices.GetService<FluentValidation.IValidator<CreateShipperDto>>()).InvokeAsync(context, next));

    app.MapPut("/api/shippers/{id}", async ([FromRoute] int id, [FromBody] UpdateShipperDto dto, [FromServices] IShipperService service, CancellationToken ct) =>
    {
        var updated = await service.UpdateAsync(id, dto, ct);
        return Results.Ok(updated);
    })
    .WithOpenApi()
    .WithTags("Shippers")
    .AddEndpointFilter(async (context, next) => await new ValidatorEndpointFilter<UpdateShipperDto>(context.HttpContext.RequestServices.GetService<FluentValidation.IValidator<UpdateShipperDto>>()).InvokeAsync(context, next));

app.MapDelete("/api/shippers/{id}", async ([FromRoute] int id, [FromServices] IShipperService service, CancellationToken ct) =>
{
    await service.DeleteAsync(id, ct);
    return Results.NoContent();
})
.WithOpenApi()
.WithTags("Shippers");

// Products
app.MapGet("/api/products", async ([FromServices] IProductService service, CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 20) =>
    {
        if (pageSize > 100) pageSize = 100;
        return Results.Ok(await service.GetAllAsync(page, pageSize, ct));
    })
.WithOpenApi()
.WithTags("Products");

app.MapGet("/api/products/{id}", async ([FromRoute] int id, [FromServices] IProductService service, CancellationToken ct) =>
{
    var p = await service.GetByIdAsync(id, ct);
    return p == null ? Results.NotFound() : Results.Ok(p);
})
.WithOpenApi()
.WithTags("Products");

    app.MapPost("/api/products", async ([FromBody] CreateProductDto dto, [FromServices] IProductService service, CancellationToken ct) =>
    {
        var created = await service.CreateAsync(dto, ct);
        return Results.Created($"/api/products/{created.ProductID}", created);
    })
    .WithOpenApi()
    .WithTags("Products")
    .AddEndpointFilter(async (context, next) => await new ValidatorEndpointFilter<CreateProductDto>(context.HttpContext.RequestServices.GetService<FluentValidation.IValidator<CreateProductDto>>()).InvokeAsync(context, next));

    app.MapPut("/api/products/{id}", async ([FromRoute] int id, [FromBody] UpdateProductDto dto, [FromServices] IProductService service, CancellationToken ct) =>
    {
        var updated = await service.UpdateAsync(id, dto, ct);
        return Results.Ok(updated);
    })
    .WithOpenApi()
    .WithTags("Products")
    .AddEndpointFilter(async (context, next) => await new ValidatorEndpointFilter<UpdateProductDto>(context.HttpContext.RequestServices.GetService<FluentValidation.IValidator<UpdateProductDto>>()).InvokeAsync(context, next));

app.MapDelete("/api/products/{id}", async ([FromRoute] int id, [FromServices] IProductService service, CancellationToken ct) =>
{
    await service.DeleteAsync(id, ct);
    return Results.NoContent();
})
.WithOpenApi()
.WithTags("Products");

// Categories (read-only)
app.MapGet("/api/categories", async ([FromServices] ICategoryService service, CancellationToken ct) =>
    {
        return Results.Ok(await service.GetAllAsync());
    })
.WithOpenApi()
.WithTags("Categories");

app.MapGet("/api/categories/{id}", async ([FromRoute] int id, [FromServices] ICategoryService service, CancellationToken ct) =>
{
    var category = await service.GetByIdAsync(id);
    return category == null ? Results.NotFound() : Results.Ok(category);
})
.WithOpenApi()
.WithTags("Categories");

// Regions (read-only)
app.MapGet("/api/regions", async ([FromServices] IRegionService service, CancellationToken ct) =>
    {
        return Results.Ok(await service.GetAllAsync());
    })
.WithOpenApi()
.WithTags("Regions");

app.MapGet("/api/regions/{id}", async ([FromRoute] int id, [FromServices] IRegionService service, CancellationToken ct) =>
{
    var region = await service.GetByIdAsync(id);
    return region == null ? Results.NotFound() : Results.Ok(region);
})
.WithOpenApi()
.WithTags("Regions");

// Suppliers
app.MapGet("/api/suppliers", async ([FromServices] ISupplierService service, CancellationToken ct, [FromQuery] int page = 1, [FromQuery] int pageSize = 20) =>
{
    if (pageSize > 100) pageSize = 100;
    return Results.Ok(await service.GetAllAsync(page, pageSize, ct));
})
.WithOpenApi()
.WithTags("Suppliers");

app.MapGet("/api/suppliers/{id}", async ([FromRoute] int id, [FromServices] ISupplierService service, CancellationToken ct) =>
{
    var s = await service.GetByIdAsync(id, ct);
    return s == null ? Results.NotFound() : Results.Ok(s);
})
.WithOpenApi()
.WithTags("Suppliers");

    app.MapPost("/api/suppliers", async ([FromBody] CreateSupplierDto dto, [FromServices] ISupplierService service, CancellationToken ct) =>
    {
        var created = await service.CreateAsync(dto, ct);
        return Results.Created($"/api/suppliers/{created.SupplierID}", created);
    })
    .WithOpenApi()
    .WithTags("Suppliers")
    .AddEndpointFilter(async (context, next) => await new ValidatorEndpointFilter<CreateSupplierDto>(context.HttpContext.RequestServices.GetService<FluentValidation.IValidator<CreateSupplierDto>>()).InvokeAsync(context, next));

    app.MapPut("/api/suppliers/{id}", async ([FromRoute] int id, [FromBody] UpdateSupplierDto dto, [FromServices] ISupplierService service, CancellationToken ct) =>
    {
        var updated = await service.UpdateAsync(id, dto, ct);
        return Results.Ok(updated);
    })
    .WithOpenApi()
    .WithTags("Suppliers")
    .AddEndpointFilter(async (context, next) => await new ValidatorEndpointFilter<UpdateSupplierDto>(context.HttpContext.RequestServices.GetService<FluentValidation.IValidator<UpdateSupplierDto>>()).InvokeAsync(context, next));

app.MapDelete("/api/suppliers/{id}", async ([FromRoute] int id, [FromServices] ISupplierService service, CancellationToken ct) =>
{
    await service.DeleteAsync(id, ct);
    return Results.NoContent();
})
.WithOpenApi()
.WithTags("Suppliers");

app.Run();
