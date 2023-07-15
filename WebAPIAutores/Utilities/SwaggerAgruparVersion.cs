using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebAPIAutores.Utilities
{
    public class SwaggerAgruparVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var nameSpaceControlador = controller.ControllerType.Namespace;
            var version = nameSpaceControlador.Split('.').Last();

            controller.ApiExplorer.GroupName = version;
        }
    }
}
