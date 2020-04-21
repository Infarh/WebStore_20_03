using System;
using Microsoft.AspNetCore.Mvc;
using WebStore.Controllers;
using WebStore.Domain.ViewModels;
using WebStore.Interfaces.Services;

namespace WebStore.Components
{
    public class BreadCrumbsViewComponent : ViewComponent
    {
        private readonly IProductData _ProductData;

        public BreadCrumbsViewComponent(IProductData ProductData) => _ProductData = ProductData;

        private void GetParameters(out BreadCrumbType Type, out int id, out BreadCrumbType FromType)
        {
            Type = Request.Query.ContainsKey("SectionId")
                ? BreadCrumbType.Section
                : Request.Query.ContainsKey("BrandId")
                    ? BreadCrumbType.Brand
                    : BreadCrumbType.None;

            if ((string) ViewContext.RouteData.Values["action"] == nameof(CatalogController.Details))
                Type = BreadCrumbType.Product;

            id = 0;

            FromType = BreadCrumbType.Section;

            switch (Type)
            {
                default: throw new ArgumentOutOfRangeException(nameof(Type), Type, null);
                case BreadCrumbType.None: break;


                case BreadCrumbType.Section:
                    id = int.Parse(Request.Query["SectionId"].ToString());
                    break;

                case BreadCrumbType.Brand: 
                    id = int.Parse(Request.Query["BrandId"].ToString());
                    break;

                case BreadCrumbType.Product:
                    id = int.Parse(ViewContext.RouteData.Values["id"].ToString() ?? string.Empty);
                    if (Request.Query.ContainsKey("FromBrand"))
                        FromType = BreadCrumbType.Brand;
                    break;
            }
        }

        public IViewComponentResult Invoke()
        {
            GetParameters(out var type, out var id, out var from_type);

            switch (type)
            {
                default: return View(Array.Empty<BreadCrumbsViewModel>());

                case BreadCrumbType.Section: 
                    return View(new []
                    {
                        new BreadCrumbsViewModel
                        {
                            BreadCrumbType = BreadCrumbType.Section,
                            Id = id,
                            Name = _ProductData.GetSectionById(id).Name
                        }, 
                    });

                case BreadCrumbType.Brand:
                    return View(new[]
                    {
                        new BreadCrumbsViewModel
                        {
                            BreadCrumbType = BreadCrumbType.Brand,
                            Id = id,
                            Name = _ProductData.GetBrandById(id).Name
                        },
                    });

                case BreadCrumbType.Product:
                    var product = _ProductData.GetProductById(id);
                    return View(new[]
                    {
                        new BreadCrumbsViewModel
                        {
                            BreadCrumbType = from_type,
                            Id = from_type == BreadCrumbType.Section
                                ? product.Section.Id
                                : product.Brand.Id,
                            Name = from_type == BreadCrumbType.Section
                                ? product.Section.Name
                                : product.Brand.Name
                        },
                        new BreadCrumbsViewModel
                        {
                            BreadCrumbType = BreadCrumbType.Product,
                            Id = product.Id,
                            Name = product.Name
                        },
                    });
            }
        }
    }
}
