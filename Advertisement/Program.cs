using System.Text.Json;
using Models;
using Database;

using Advertisement.AdPrediction;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseStaticFiles();
app.MapGet("/", () =>
{
    return Results.Redirect("/U1");
});

app.MapGet("/internal/logo", () =>
{
    var imagePath = "logo.png";

    // Check if the file exists
    if (System.IO.File.Exists(imagePath))
    {
        // Return the image file using PhysicalFile result
        // return new PhysicalFileResult(imagePath, "image/png"); // Adjust the content type as needed
        return Results.File(imagePath, "image/png");
    }
    else
    {
        // If the file doesn't exist, return a 404 Not Found response or handle it accordingly
        return Results.NotFound();
    }
});

app.MapGet("/internal/favicon", () =>
{
    var imagePath = "favicon.ico";

    // Check if the file exists
    if (System.IO.File.Exists(imagePath))
    {
        // Return the image file using PhysicalFile result
        // return new PhysicalFileResult(imagePath, "image/png"); // Adjust the content type as needed
        return Results.File(imagePath, "image/ico");
    }
    else
    {
        // If the file doesn't exist, return a 404 Not Found response or handle it accordingly
        return Results.NotFound();
    }
});

app.MapGet("/{id}", (IWebHostEnvironment env, string id) =>
{
    // Path to your HTML file
    var filePath = Path.Combine(env.ContentRootPath, "wwwroot", "index.html");

    // Read the content of the HTML file
    var htmlContent = File.ReadAllText(filePath);

    var ConversionClickDataset = JsonSerializer.Serialize(DatabaseLib.GetMonthlyStats());

    // Generate JavaScript code with the initialized variable
    var javascriptCode = $"<script>var ConversionClickDataset = '{ConversionClickDataset}';</script>";

    // Insert the JavaScript code into the HTML content
    htmlContent = htmlContent.Replace("</head>", $"{javascriptCode}</head>");

    // Replace the placeholder in the HTML content with the actual id
    htmlContent = htmlContent.Replace("{id}", id);

    // Replace the placeholder in the HTML content with the actual id
    htmlContent = htmlContent.Replace("{profileID}", id);

    // Return the HTML file
    return Results.Content(htmlContent, "text/html");

    //return Results.File(filePath, "text/html");
});

app.MapGet("/api/ad/{width}/{height}", (int width, int height) =>
{
    var imageUrl = DatabaseLib.SearchProductByDimensions(height, width);
    if (imageUrl != "")
    {
        //Advertisement.AdPrediction.UserPurchases test = new Advertisement.AdPrediction.UserPurchases(id);
        imageUrl = DatabaseLib.GetProductImageById(imageUrl);
        return Results.Redirect(imageUrl);
    }
    else
    {
        imageUrl = "https://picsum.photos/" + width + "/" + height + ".jpg";
        return Results.Redirect(imageUrl);
    }

    // Create algorithm class with given paramaters
    //var image = UserPurchases(width, height, user);

    // Prepare the HTML response with an image (and id temporarily for testing purposes)
    //var htmlResponse = $"<a href=\"{imageURL}\"><img src=\"{imageURL}\" alt=\"Ad Image\" width=\"{width}\" height=\"{height}\"></a>";

    // Return the HTML response
    //return Results.Content(htmlResponse, "text/html");

    // Return the HTML response
});

app.MapGet("/ad/clicked", (string id) =>
{
    //var productName = DatabaseLib.GetProductById(id);
    //var pageURL = "/api/" + productName;

    //call function to increment clicks
    DatabaseLib.IncrementClicks();

    //Return the Product Page response
    return Results.Ok();
});

app.MapPost("/ad/converted", () =>
{
    //call function to increment Conversions
    DatabaseLib.IncrementConversions();

    return Results.Ok();
});

app.MapPost("/api/parsejson", async (HttpContext context) =>
{
    using (StreamReader reader = new StreamReader(context.Request.Body))
    {
        var json = await reader.ReadToEndAsync();

        //Parse the JSON to JsonDocument
        using (JsonDocument doc = JsonDocument.Parse(json))
        {
            // Detect the root element type
            JsonElement root = doc.RootElement;

            // Dynamically determine the model based on the root element's properties
            if (root.TryGetProperty("image", out _))
            {
                // Deserialize to Product model
                var product = JsonSerializer.Deserialize<Product>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Process the product data as needed

                return Results.Text($"Received product: {product.Name}");
            }
            else if (root.TryGetProperty("name", out _))
            {
                // Deserialize to another model
                var user = JsonSerializer.Deserialize<User>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Process the other model data as needed

                return Results.Text($"Received data: {user.Name}");
            }
            else if (root.TryGetProperty("stats", out _))
            {
                // Deserialize to another model
                var category = JsonSerializer.Deserialize<Category>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Process the other model data as needed

                return Results.Text($"Received data: {category.Name}");
            }
            else
            {
                // Handle unrecognized JSON structure
                return Results.BadRequest("Unrecognized JSON structure");
            }
        }
    }
}); string strExeFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;

app.Run();
string strWorkPath = System.IO.Path.GetDirectoryName(strExeFilePath);