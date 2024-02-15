namespace TestServiceGRPC.Utils.Services;

public class RefGuidService
{
    /// <summary>
    /// This class is used to Register the Guid as Scoped in the DI container.
    /// Scoped means that a new instance is created for every HTTP request.
    /// The Guid is retrieved from the JWT token and stored in this wrapper by the SessionMiddleware.
    /// </summary>
    public Guid? Value { get; set; }
}
