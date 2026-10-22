using LearnHub.Api.Data;

namespace LearnHub.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        app.MapGet("/api/orders", (string? studentName, InMemoryStore store) =>
            store.GetOrders(studentName));

        app.MapPost("/api/orders/{id:int}/refund", (int id, InMemoryStore store) =>
        {
            var (order, error) = store.RefundOrder(id);
            if (order is null)
                return error switch
                {
                    "Order not found." => Results.NotFound(),
                    "This order was already refunded." => Results.Conflict(new { error }),
                    _ => Results.BadRequest(new { error })
                };

            return Results.Ok(order);
        });
    }
}
