using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace WebAPIAutores.Utilities
{
    public class CabeceraAttribute : Attribute, IActionConstraint
    {
        private readonly string cabecera;
        private readonly string valor;

        public int Order => 0;

        public CabeceraAttribute(string cabecera, string valor)
        {
            this.cabecera = cabecera;
            this.valor = valor;
        }

        public bool Accept(ActionConstraintContext context)
        {
            var cabeceras = context.RouteContext.HttpContext.Request.Headers;

            if (!cabeceras.ContainsKey(cabecera))
            {
                return false;
            }

            return string.Equals(cabeceras[cabecera], valor, StringComparison.OrdinalIgnoreCase);
        }
    }
}
