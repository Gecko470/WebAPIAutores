using Microsoft.EntityFrameworkCore;

namespace WebAPIAutores.Utilities
{
    public static class HttpContextExtensions
    {
        public async static Task ParametrosPaginacionEnCabecera<T>(this HttpContext httpContext, IQueryable<T> queryable)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            double cantidad = await queryable.CountAsync();
            httpContext.Response.Headers.Add("cantidadRegistros", cantidad.ToString());
        }
    }
}
