using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WebAPIAutores.Controllers.v1;
using WebAPIAutores.DTOs;
using WebAPIAutores.Test.Mocks;

namespace WebAPIAutores.Test.PruebasUnitarias
{
    [TestClass]
    public class RootControllerTests
    {
        [TestMethod]
        public async Task UsuarioEsAmin_Obtenemos4Links()
        {
            var authorizationServiceMock = new IAuthorizationServiceMock();
            authorizationServiceMock.resultado = AuthorizationResult.Success();

            var rootController = new RootController(authorizationServiceMock);
            rootController.Url = new UrlHelperMock();

            var resultado = await rootController.Get();

            Assert.AreEqual(4, resultado.Value.Count());
        }

        [TestMethod]
        public async Task UsuarioNoEsAmin_Obtenemos2Links()
        {
            var authorizationServiceMock = new IAuthorizationServiceMock();
            authorizationServiceMock.resultado = AuthorizationResult.Failed();

            var rootController = new RootController(authorizationServiceMock);
            rootController.Url = new UrlHelperMock();

            var resultado = await rootController.Get();

            Assert.AreEqual(2, resultado.Value.Count());
        }

        [TestMethod]
        public async Task UsuarioNoEsAmin_Obtenemos2Links_UsandoMoq()
        {
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<IEnumerable<IAuthorizationRequirement>>())).Returns(Task.FromResult(AuthorizationResult.Failed()));
            mockAuthorizationService.Setup(x => x.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>())).Returns(Task.FromResult(AuthorizationResult.Failed()));

            var mockIUrlHelper = new Mock<IUrlHelper>();
            mockIUrlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(string.Empty);

            var rootController = new RootController(mockAuthorizationService.Object);
            rootController.Url = mockIUrlHelper.Object;

            var resultado = await rootController.Get();

            Assert.AreEqual(2, resultado.Value.Count());
        }
    }
}
