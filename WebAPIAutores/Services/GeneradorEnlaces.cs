using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using WebAPIAutores.DTOs;

namespace WebAPIAutores.Services
{
    public class GeneradorEnlaces
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IActionContextAccessor actionContextAccessor;

        public GeneradorEnlaces(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor, IActionContextAccessor actionContextAccessor)
        {
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.actionContextAccessor = actionContextAccessor;
        }

        private IUrlHelper ConstruirUrlHelper()
        {
            var factoria = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factoria.GetUrlHelper(actionContextAccessor.ActionContext);
        }

        private async Task<bool> EsAmin()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var esAdmin = await authorizationService.AuthorizeAsync(httpContext.User, "esAdmin");
            return esAdmin.Succeeded;
        }
        public async Task GenerarEnlaces(AutorDTO autorDTO)
        {
            var esAdmin = await EsAmin();
            var Url = ConstruirUrlHelper();

            autorDTO.Enlaces.Add(new HateoasDTO(enlace: Url.Link("AutorById", new { id = autorDTO.Id }), descripcion: "self", metodo: "GET"));

            if (esAdmin)
            {
                autorDTO.Enlaces.Add(new HateoasDTO(enlace: Url.Link("putAutor", new { id = autorDTO.Id }), descripcion: "autor-update", metodo: "PUT"));
                autorDTO.Enlaces.Add(new HateoasDTO(enlace: Url.Link("deleteAutor", new { id = autorDTO.Id }), descripcion: "autor-delete", metodo: "DELETE"));
            }

        }
    }
}
