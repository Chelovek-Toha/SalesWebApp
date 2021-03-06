﻿using SalesUpdater.Interfaces.Core.DataTransferObject;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Helpers;
using X.PagedList;

namespace SalesUpdater.DAL.ReaderWriter
{
    public interface IGenericDbReaderWriter<TCoreModel> where TCoreModel : CoreModel
    {
        Task<TCoreModel> AddAsync(TCoreModel model);

        Task<TCoreModel> UpdateAsync(TCoreModel model);

        Task DeleteAsync(int id);

        Task<IPagedList<TCoreModel>> GetPagedListAsync(int pageNumber, int pageSize,
            Expression<Func<TCoreModel, bool>> predicate = null, SortDirection sortDirection = SortDirection.Ascending);

        Task<TCoreModel> GetAsync(int id);

        Task<IEnumerable<TCoreModel>> FindAsync(Expression<Func<TCoreModel, bool>> predicate);
    }
}
