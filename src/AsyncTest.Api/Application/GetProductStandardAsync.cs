namespace AsyncTest.Api.Application;

public static class GetProductStandardAsync
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
        public async Task<string> GetProductAsync()
        {
            return await dbContext.GetProductAsync();
        }
    }

    public class Handler(Repository productRepository)
    {
        public async Task<string> GetProductAsync()
        {
            return await productRepository.GetProductAsync();
        }
    }

    public static WebApplication MapStandardGetProductEndpoint(this WebApplication app)
    {
        app.MapGet("/products/standard", async (Handler handler) =>
        {
            var product = await handler.GetProductAsync();
            return product;
        });

        return app;
    }
}

