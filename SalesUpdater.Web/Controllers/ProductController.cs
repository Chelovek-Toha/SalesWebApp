﻿using AutoMapper;
using SalesUpdater.Interfaces.Core.DataTransferObject;
using SalesUpdater.Web.Data.Contracts.Services;
using SalesUpdater.Web.Data.Models;
using SalesUpdater.Web.Data.Models.Filters;
using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using X.PagedList;

namespace SalesUpdater.Web.Data.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        private readonly IMapper _mapper;

        private readonly int _pageSize;

        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;

            _mapper = mapper;

            _pageSize = int.Parse(ConfigurationManager.AppSettings["itemsPerPage"]);
        }

        [HttpGet]
        public async Task<ActionResult> Index(int? page)
        {
            try
            {
                ViewBag.ProductFilter = new ProductViewFilterModel();

                var productsCoreModels = await _productService.GetPagedListAsync(page ?? 1, _pageSize);

                var productsViewModels =
                        _mapper.Map<IPagedList<ProductViewModel>>(productsCoreModels);

                return View(productsViewModels);
            }
            catch (Exception exception)
            {
                ViewBag.Error = exception.Message;

                return View();
            }
        }

        [HttpGet]
        public async Task<ActionResult> Find(ProductViewFilterModel productViewFilterModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var coreModels = await _productService
                        .GetPagedListAsync(productViewFilterModel.Page ?? 1, _pageSize)
                        .ConfigureAwait(false);

                    var viewModels = _mapper.Map<IPagedList<ProductViewModel>>(coreModels);

                    return PartialView("Partial/_ProductTable", viewModels);
                }

                var productsCoreModels = await _productService.Filter(
                    _mapper.Map<ProductCoreFilterModel>(productViewFilterModel), _pageSize);

                var productsViewModels = _mapper.Map<IPagedList<ProductViewModel>>(productsCoreModels);

                ViewBag.ProductFilterNameValue = productViewFilterModel.Name;

                return PartialView("Partial/_ProductTable", productsViewModels);
            }
            catch (Exception exception)
            {
                ViewBag.Error = exception.Message;

                return PartialView("Partial/_ProductTable");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(ProductViewModel product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(product);
                }

                await _productService.AddAsync(_mapper.Map<ProductDTO>(product)).ConfigureAwait(false);

                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                ViewBag.Error = exception.Message;

                return View();
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var productCoreModel = await _productService.GetAsync(id).ConfigureAwait(false);

                var productViewModel = _mapper.Map<ProductViewModel>(productCoreModel);

                return View(productViewModel);
            }
            catch (Exception exception)
            {
                ViewBag.Error = exception.Message;

                return View();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(ProductViewModel product)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(product);
                }

                await _productService.UpdateAsync(_mapper.Map<ProductDTO>(product)).ConfigureAwait(false);

                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                ViewBag.Error = exception.Message;

                return View();
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _productService.DeleteAsync(id).ConfigureAwait(false);

                return RedirectToAction("Index");
            }
            catch (Exception exception)
            {
                ViewBag.Error = exception.Message;

                return RedirectToAction("Index");
            }
        }
    }
}
