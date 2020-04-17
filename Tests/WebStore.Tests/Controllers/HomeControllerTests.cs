using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebStore.Controllers;

using Assert = Xunit.Assert;

namespace WebStore.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTests
    {
        [TestMethod]
        public void Index_Returns_View()
        {
            var controller = new HomeController();

            var result = controller.Index();

            Assert.IsType<ViewResult>(result);
        }

        [TestMethod]
        public void SomeAction_Returns_View()
        {
            var controller = new HomeController();

            var result = controller.SomeAction();

            Assert.IsType<ViewResult>(result);
        }

        [TestMethod]
        public void Blog_Returns_View()
        {
            var controller = new HomeController();

            var result = controller.Blog();

            Assert.IsType<ViewResult>(result);
        }

        [TestMethod]
        public void BlogSingle_Returns_View()
        {
            var controller = new HomeController();

            var result = controller.BlogSingle();

            Assert.IsType<ViewResult>(result);
        }

        [TestMethod]
        public void ContactUs_Returns_View()
        {
            var controller = new HomeController();

            var result = controller.ContactUs();

            Assert.IsType<ViewResult>(result);
        }

        [TestMethod, ExpectedException(typeof(ApplicationException))]
        public void Throw_Thrown_ApplicationException()
        {
            var controller = new HomeController();

            const string expected_exception_text = "123";

            var result = controller.Throw(expected_exception_text);

            Assert.IsType<ViewResult>(result);
        }

        [TestMethod]
        public void Throw_Thrown_ApplicationException2()
        {
            var controller = new HomeController();

            const string expected_exception_text = "123";

            var exception = Assert.Throws<ApplicationException>(() => controller.Throw(expected_exception_text));

            Assert.Equal(expected_exception_text, exception.Message);
        }

        [TestMethod]
        public void Error404_Returns_View()
        {
            var controller = new HomeController();

            var result = controller.Error404();

            Assert.IsType<ViewResult>(result);
        }

        [TestMethod]
        public void ErrorStatus_404_RedirectTo_Error404()
        {
            var controller = new HomeController();

            const string status_code = "404";

            var result = controller.ErrorStatus(status_code);

            var redirect_to_action = Assert.IsType<RedirectToActionResult>(result); //(RedirectToActionResult) result;
            Assert.Null(redirect_to_action.ControllerName);
            Assert.Equal(nameof(HomeController.Error404), redirect_to_action.ActionName);
        }
    }
}
