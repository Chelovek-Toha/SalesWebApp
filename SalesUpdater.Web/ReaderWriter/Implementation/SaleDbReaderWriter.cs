﻿using SalesUpdater.DAL.Repositories;
using SalesUpdater.Entity;
using SalesUpdater.Interfaces.Core.DataTransferObject;
using SalesUpdater.Interfaces.DAL.Repositories;
using SalesUpdater.Web.ReaderWriter.Implementation;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;
using X.PagedList;

namespace SalesUpdater.Web.ReaderWriter.Implementation
{
    public class SaleDbReaderWriter : ISaleDbReaderWriter
    {
        private SalesContext Context { get; }
        private ReaderWriterLockSlim Locker { get; }

        private IClientRepository Clients { get; }
        private IManagerRepository Managers { get; }
        private IProductRepository Products { get; }
        private ISaleRepository Sales { get; }

        public SaleDbReaderWriter(SalesContext context, ReaderWriterLockSlim locker)
        {
            Context = context;
            Locker = locker;

            var mapper = AutoMapper.CreateConfiguration().CreateMapper();
            Clients = new ClientRepository(Context, mapper);
            Managers = new ManagerRepository(Context, mapper);
            Products = new ProductRepository(Context, mapper);
            Sales = new SaleRepository(Context, mapper);
        }

        public async Task<IPagedList<SaleDTO>> GetUsingPagedListAsync(int number, int size,
            Expression<Func<SaleDTO, bool>> predicate = null, SortDirection sortDirection = SortDirection.Ascending)
        {
            return await Sales.GetUsingPagedListAsync(number, size, predicate).ConfigureAwait(false);
        }

        public async Task<SaleDTO> GetAsync(int id)
        {
            return await Sales.GetAsync(id).ConfigureAwait(false);
        }

        public async Task<SaleDTO> AddAsync(SaleDTO sale)
        {
            Locker.EnterWriteLock();
            try
            {
                await FindOutIds(sale).ConfigureAwait(false);

                var result = Sales.Add(sale);
                await Sales.SaveAsync().ConfigureAwait(false);

                return result;
            }
            finally
            {
                if (Locker.IsWriteLockHeld)
                {
                    Locker.ExitWriteLock();
                }
            }
        }

        public async Task<SaleDTO> UpdateAsync(SaleDTO sale)
        {
            Locker.EnterWriteLock();
            try
            {
                await FindOutIds(sale).ConfigureAwait(false);

                var result = Sales.Update(sale);
                await Sales.SaveAsync().ConfigureAwait(false);

                return result;
            }
            finally
            {
                if (Locker.IsWriteLockHeld)
                {
                    Locker.ExitWriteLock();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            Locker.EnterReadLock();
            try
            {
                await Sales.DeleteAsync(id).ConfigureAwait(false);
                await Sales.SaveAsync().ConfigureAwait(false);
            }
            finally
            {
                if (Locker.IsReadLockHeld)
                {
                    Locker.ExitReadLock();
                }
            }
        }

        public async Task<IEnumerable<SaleDTO>> FindAsync(Expression<Func<SaleDTO, bool>> predicate)
        {
            return await Sales.FindAsync(predicate).ConfigureAwait(false);
        }

        private async Task FindOutIds(SaleDTO sale)
        {
            if (await Clients.DoesClientExistAsync(sale.Clients).ConfigureAwait(false))
            {
                sale.Clients.ID = await Clients.GetIdAsync(sale.Clients.Name, sale.Clients.Surname)
                    .ConfigureAwait(false);
            }
            else
            {
                ThrowArgumentException("There is no such Client. Register it!");
            }

            if (await Managers.DoesManagerExistAsync(sale.Managers))
            {
                sale.Managers.ID = await Managers.GetIdAsync(sale.Managers.Surname).ConfigureAwait(false);
            }
            else
            {
                ThrowArgumentException("There is no such Manager. Register it!");
            }

            if (await Products.DoesProductExistAsync(sale.Products))
            {
                sale.Products.ID = await Products.GetIdAsync(sale.Products.Name).ConfigureAwait(false);
            }
            else
            {
                ThrowArgumentException("There is no such Product. Register it!");
            }
        }

        private static void ThrowArgumentException(string message)
        {
            throw new ArgumentException(message);
        }
    }
}