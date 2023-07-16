using System.ComponentModel.DataAnnotations;
using WebAPIAutores.Vadlidations;

namespace WebAPIAutores.Test.PruebasUnitarias
{
    [TestClass]
    public class CapitalizeAttributeTests
    {
        [TestMethod]
        public void LetraMinuscula_Error()
        {
            var capitalize = new CapitalizeAttribute();
            var valor = "juan";
            var valorContext = new ValidationContext(new { Nombre = valor });

            var resultado = capitalize.GetValidationResult(valor, valorContext);

            Assert.AreEqual("La primera letra debe ser mayúscula..", resultado.ErrorMessage);
        }

        [TestMethod]
        public void ValorNulo_NoError()
        {
            var capitalize = new CapitalizeAttribute();
            string? valor = null;
            var valorContext = new ValidationContext(new { Nombre = valor });

            var resultado = capitalize.GetValidationResult(valor, valorContext);

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void LetraMayuscula_NoError()
        {
            var capitalize = new CapitalizeAttribute();
            string valor = "Juan";
            var valorContext = new ValidationContext(new { Nombre = valor });

            var resultado = capitalize.GetValidationResult(valor, valorContext);

            Assert.IsNull(resultado);
        }
    }
}