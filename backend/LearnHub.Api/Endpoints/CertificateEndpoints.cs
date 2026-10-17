using LearnHub.Api.Data;

namespace LearnHub.Api.Endpoints;

public static class CertificateEndpoints
{
    public static void MapCertificateEndpoints(this WebApplication app)
    {
        app.MapGet("/api/certificates", (string? studentName, InMemoryStore store) =>
            store.GetCertificates(studentName).Select(c => new
            {
                c.CourseTitle,
                c.StudentName,
                c.IssuedOn,
                c.CertificateId
            }));
    }
}
