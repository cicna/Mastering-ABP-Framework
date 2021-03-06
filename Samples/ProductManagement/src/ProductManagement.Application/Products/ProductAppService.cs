﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using ProductManagement.Categories;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

namespace ProductManagement.Products
{
    public class ProductAppService :
        ProductManagementAppService, IProductAppService
    {
        private readonly IRepository<Product, Guid> _productRepository;
        private readonly IRepository<Category, Guid> _categoryRepository;

        public ProductAppService(
            IRepository<Product, Guid> productRepository,
            IRepository<Category, Guid> categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<PagedResultDto<ProductDto>> GetListAsync(
            PagedAndSortedResultRequestDto input)
        {
            var queryable = await _productRepository
                .WithDetailsAsync(x => x.Category);

            queryable = queryable
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
                .OrderBy(input.Sorting ?? nameof(Product.Name));

            var count = await AsyncExecuter.CountAsync(queryable);
            var products = await AsyncExecuter.ToListAsync(queryable);

            return new PagedResultDto<ProductDto>(
                count,
                ObjectMapper.Map<List<Product>, List<ProductDto>>(products)
            );
        }

        public async Task CreateAsync(CreateProductDto input)
        {
            await _productRepository.InsertAsync(
                ObjectMapper.Map<CreateProductDto, Product>(input)
            );
        }

        public async Task<ListResultDto<CategoryLookupDto>> GetCategoriesAsync()
        {
            var categories = await _categoryRepository.GetListAsync();
            return new ListResultDto<CategoryLookupDto>(
                ObjectMapper.Map<List<Category>, List<CategoryLookupDto>>(categories)
            );
        }
    }
}
