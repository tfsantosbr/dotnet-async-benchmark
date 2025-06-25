namespace AsyncTest.Api.Application;

public static class GetProductOptimizedAsync
{
    public class DatabaseContext
    {
        public async Task<string> GetProductAsync()
        {
            await Task.Delay(1);
            return "Product Details";
        }
    }

    public class Repository(DatabaseContext dbContext)
    {
        public Task<string> GetProductAsync()
        {
            return dbContext.GetProductAsync();
        }
    }

    public class Handler(Repository productRepository)
    {
        public Task<string> GetProductAsync()
        {
            return productRepository.GetProductAsync();
        }
    }

    public static WebApplication MapOptimizedGetProductEndpoint(this WebApplication app)
    {
        app.MapGet("/products/optimized", async (Handler handler) =>
        {
            var product = await handler.GetProductAsync();
            return product;
        });

        return app;
    }
}

