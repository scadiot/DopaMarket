﻿using DopaMarket.Dto;
using DopaMarket.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DopaMarket.Controllers.Api
{
    public class SearchController : ApiController
    {
        const int ItemPerPage = 3;

        ApplicationDbContext _context;

        public SearchController()
        {
            _context = new ApplicationDbContext();
        }

        public SearchResult Get([FromUri]SearchRequest dtoSearchRequest)
        {
            if(dtoSearchRequest == null)
            {
                HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
                throw new HttpResponseException(httpResponseMessage);
            }
            var searchRequest = new SearchTool.SearchRequest();

            if(dtoSearchRequest.Brands != null)
            {
                searchRequest.Brands = dtoSearchRequest.Brands.Split(',').Select(s => s.Trim()).ToArray();
            }

            if (dtoSearchRequest.Keywords != null)
            {
                searchRequest.Keywords = dtoSearchRequest.Keywords.Split(',').Select(s => s.Trim()).ToArray();
            }

            if (dtoSearchRequest.Category != null)
            {
                searchRequest.Category = _context.Categories.SingleOrDefault(c => c.LinkName == dtoSearchRequest.Category);
            }           

            searchRequest.Sort = dtoSearchRequest.Sort != null ? dtoSearchRequest.Sort : "a-z";
            searchRequest.Page = dtoSearchRequest.Page;
            searchRequest.FilterPriceMin = dtoSearchRequest.FilterPriceMin;
            searchRequest.FilterPriceMax = dtoSearchRequest.FilterPriceMax;
            searchRequest.ItemPerPage = ItemPerPage;

            var searchTool = new SearchTool(_context);
            var searchResult = searchTool.Execute(searchRequest);

            var dtoSearchResult = new Dto.SearchResult();
            dtoSearchResult.Items = searchResult.Items;
            dtoSearchResult.Brands = searchResult.Brands.Select(b => new SearchBrand() { Brand = b, Selected = searchRequest.Brands != null ? searchRequest.Brands.Contains(b.LinkName) : false });
            dtoSearchResult.Categories = searchResult.Categories;
            dtoSearchResult.Page = searchRequest.Page;
            dtoSearchResult.PageCount = searchResult.PageCount;
            dtoSearchResult.ItemsCount = searchResult.ItemsCount;
            dtoSearchResult.ItemsCountAfterFilter = searchResult.ItemsCountAfterFilter;
            dtoSearchResult.ItemsPerPage = ItemPerPage;
            dtoSearchResult.PriceMin = searchResult.PriceMin;
            dtoSearchResult.PriceMax = searchResult.PriceMax;

            return dtoSearchResult;
        }
    }
}
