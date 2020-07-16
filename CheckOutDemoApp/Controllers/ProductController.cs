using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CheckOutDemoApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace CheckOutDemoApp.Controllers
{
    public class ProductController : Controller
    {      
        [HttpGet]
        public IActionResult CreateProduct()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateProduct(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {                
                string fileName = System.IO.Path.GetTempPath() + model.ProductImage.FileName ;
                FileStream stream = new FileStream(fileName, FileMode.Create);
                model.ProductImage.CopyTo(stream);
                stream.Close();
                using (FileStream stream1 = System.IO.File.Open(fileName, FileMode.Open))
                {
                    var fileCreateOptions = new FileCreateOptions
                    {
                        File = stream1,
                        Purpose = FilePurpose.DisputeEvidence
                    };
                    var fileService = new FileService();
                    var file = fileService.Create(fileCreateOptions);

                    var fileLinkCreateOptions = new FileLinkCreateOptions
                    {
                        File = file.Id,
                    };
                    var fileLinkService = new FileLinkService();
                    var fileLink=fileLinkService.Create(fileLinkCreateOptions);

                    var productCreateOptions = new ProductCreateOptions
                    {
                        Name = model.Product.Name,
                        Active = model.Product.Active,
                        Description = model.Product.Description,
                        Images = new List<string> { fileLink.Url}
                    };
                    var productService = new ProductService();
                    var product = productService.Create(productCreateOptions);
                    return RedirectToAction("CreatePrice", "Price", new { ProductId = product.Id });
                }
            }            
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ListAllProducts()
        {
            var productService = new ProductService();
            StripeList<Product> products= await productService.ListAsync();
            return View(products);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RetrieveProduct(string ProductId)
        {
            var productService = new ProductService();
            Product product=productService.Get(ProductId);
            return View(product);
        }
        [HttpGet]
        public IActionResult DeleteProduct(string ProductId)
        {
            var productService = new ProductService();
            var product=productService.Delete(ProductId);
            if (product == null)
            {
                ViewBag.ErrorMessage = "Cannot Delete The Product";
                return View("Error");
            }
            return View(product);
        }
        [HttpGet]
        public IActionResult UpdateProduct(string ProductId)
        {
            ProductViewModel model = new ProductViewModel();
            var productService = new ProductService();
            model.Product  = productService.Get(ProductId);
            return View(model);
        }
        [HttpPost]
        public IActionResult UpdateProduct(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                string fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString()+"_"+ model.ProductImage.FileName;
                FileStream stream = new FileStream(fileName, FileMode.Create);
                model.ProductImage.CopyTo(stream);
                stream.Close();
                using (FileStream stream1 = System.IO.File.Open(fileName, FileMode.Open))
                {
                    var fileCreateOptions = new FileCreateOptions
                    {
                        File = stream1,
                        Purpose = FilePurpose.DisputeEvidence
                    };
                    var fileService = new FileService();
                    var file = fileService.Create(fileCreateOptions);

                    var fileLinkCreateOptions = new FileLinkCreateOptions
                    {
                        File = file.Id,
                    };
                    var fileLinkService = new FileLinkService();
                    var fileLink = fileLinkService.Create(fileLinkCreateOptions);
                    var productService = new ProductService();
                    var productUpdateOptions = new ProductUpdateOptions
                    {
                        Name = model.Product.Name,
                        Description = model.Product.Description,
                        Active = model.Product.Active,
                        Images=new List<string> { fileLink.Url}
                    };
                    productService.Update(model.Product.Id, productUpdateOptions);
                }
                return RedirectToAction("RetrieveProduct", new { ProductId = model.Product.Id });
            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ProductsWithPrice()
        {
            //List<ProductsWithPricesViewModel> model = new List<ProductsWithPricesViewModel>();
            //var productService = new ProductService();
            //var priceService = new PriceService();
            //StripeList<Product> products = productService.List();
            //foreach (var product in products)
            //{
            //    var priceListOptions = new PriceListOptions
            //    {
            //        Product = product.Id
            //    };
            //    StripeList<Price> prices=priceService.List(priceListOptions);
            //    ProductsWithPricesViewModel val = new ProductsWithPricesViewModel
            //    {

            //    };
            //}
            //var priceService = new PriceService();
            List<Price> ProductsWithPricesModel = new List<Price>();
            var priceService = new PriceService();
            var prices=priceService.List();
            foreach (var price in prices)
            {
                var priceGetOptions = new PriceGetOptions();
                priceGetOptions.AddExpand("product");
                var productwithprice=priceService.Get(price.Id, priceGetOptions);
                ProductsWithPricesModel.Add(productwithprice);                
            }

            return View(ProductsWithPricesModel);
        }
    }
}
