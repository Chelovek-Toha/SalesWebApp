﻿using AutoMapper;
using SalesUpdater.Entity;
using SalesUpdater.Interfaces.Core.DataTransferObject;
using System;
using System.Collections.Generic;
using X.PagedList;

namespace SalesUpdater.Web.ReaderWriter.Implementation
{
    internal static class AutoMapper
    {
        internal static MapperConfiguration CreateConfiguration()
        {
            try
            {
                return new MapperConfiguration(config =>
                {
                    try
                    {
                        config.CreateMap<Clients, ClientDTO>();
                        config.CreateMap<ClientDTO, Clients>();
                    }
                    catch(Exception e)
                    {
                        throw;
                    }

                    config.CreateMap<Products, ProductDTO>();
                    config.CreateMap<ProductDTO, Products>();

                    config.CreateMap<Managers, ManagerDTO>();
                    config.CreateMap<ManagerDTO, Managers>();

                    config.CreateMap<Sales, SaleDTO>();
                    config.CreateMap<SaleDTO, Sales>()
                        .ForMember(x => x.Clients, opt => opt.Ignore())
                        .ForMember(x => x.Managers, opt => opt.Ignore())
                        .ForMember(x => x.Products, opt => opt.Ignore());
                    try
                    {
                        config
                            .CreateMap<IPagedList<Clients>, IPagedList<ClientDTO>>()
                            .ConvertUsing<PagedListConverter<Clients, ClientDTO>>();
                        config
                            .CreateMap<IPagedList<ClientDTO>, IPagedList<Clients>>()
                            .ConvertUsing<PagedListConverter<ClientDTO, Clients>>();

                        config
                            .CreateMap<IPagedList<Products>, IPagedList<ProductDTO>>()
                            .ConvertUsing<PagedListConverter<Products, ProductDTO>>();
                        config
                            .CreateMap<IPagedList<ProductDTO>, IPagedList<Products>>()
                            .ConvertUsing<PagedListConverter<ProductDTO, Products>>();

                        config
                            .CreateMap<IPagedList<Managers>, IPagedList<ManagerDTO>>()
                            .ConvertUsing<PagedListConverter<Managers, ManagerDTO>>();
                        config
                            .CreateMap<IPagedList<ManagerDTO>, IPagedList<Managers>>()
                            .ConvertUsing<PagedListConverter<ManagerDTO, Managers>>();

                        config
                            .CreateMap<IPagedList<Sales>, IPagedList<SaleDTO>>()
                            .ConvertUsing<PagedListConverter<Sales, SaleDTO>>();
                        config
                            .CreateMap<IPagedList<SaleDTO>, IPagedList<Sales>>()
                            .ConvertUsing<PagedListConverter<SaleDTO, Sales>>();
                    }
                    catch (Exception u)
                    {
                        throw;
                    }
                });
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }

    public class PagedListConverter<TSource, TDestination>
        : ITypeConverter<IPagedList<TSource>, IPagedList<TDestination>>
        where TSource : class
        where TDestination : class
    {
        public IPagedList<TDestination> Convert(IPagedList<TSource> source, IPagedList<TDestination> destination,
            ResolutionContext context)
        {
            var sourceList = context.Mapper.Map<IEnumerable<TSource>, IEnumerable<TDestination>>(source);
            IPagedList<TDestination> pagedResult = new StaticPagedList<TDestination>(sourceList, source.GetMetaData());
            return pagedResult;
        }
    }
}