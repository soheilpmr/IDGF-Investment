using BackEndInfrastructure.DynamicLinqCore.Helper;
using BackEndInfrastructure.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace BackEndInfrastructure.DynamicLinqCore
{
    public static class QueryableExtensions
    {

        /// <summary>
        /// Applies paging, sorting and filtering over IQueryable using Dynamic Linq.
        /// </summary>
        /// <typeparam name="T">The type of the IQueryable.</typeparam>
        /// <param name="queryable">The IQueryable which paging, sorting and filtering would be applied to.</param>
        /// <param name="take">Page size.</param>
        /// <param name="skip">Pages to skip.</param>
        /// <param name="sort">Requested sort order.</param>
        /// <param name="filter">Requested filters.</param>

        /// <returns>A LinqDataResult object populated from the processed IQueryable.</returns>
        public static async Task<LinqDataResult<T>> ToLinqDataResultAsync<T>(this IQueryable<T> queryable, int take, int skip, IEnumerable<Sort> sort, Filter filter, DataBase dataBase = DataBase.SQLServer)
        {
            var total = await queryable.CountAsync();
            var filteredCount = total;
            if (filter != null && filter.Logic != null)
            {
                // Filter the data first
                queryable = Filter<T>(queryable, filter);

                // Calculate the total number of records (needed for paging)
                filteredCount = await queryable.CountAsync();
            }

            // Sort the data
            queryable = Sort(queryable, sort);

            // Finally page the data
            if (take > 0)
            {
                if (dataBase == DataBase.SQLServer)
                    queryable = PageSQL(queryable, take, skip);
                if (dataBase == DataBase.Oracle)
                    queryable = PageOracle(queryable, take, skip);
            }

            var rtn = new LinqDataResult<T>
            {
                Data = queryable.AsEnumerable(),
                RecordsTotal = total,
                RecordsFiltered = filteredCount,
            };

            return rtn;
        }

        /// <summary>
        /// Applies paging, sorting and filtering over IQueryable using Dynamic Linq.
        /// </summary>
        /// <typeparam name="T">The type of the IQueryable.</typeparam>
        /// <param name="queryable">The IQueryable which paging, sorting and filtering would be applied to.</param>
        /// <param name="take">Page size.</param>
        /// <param name="skip">Pages to skip.</param>
        /// <param name="sort">Requested sort order.</param>
        /// <param name="filter">Requested filters.</param>

        /// <returns>A LinqDataResult object populated from the processed IQueryable.</returns>
        public static async Task<LinqDataResult<P>> ToLinqDataResultAsync<T, P>(this IQueryable<T> queryable, int take, int skip, IEnumerable<Sort> sort, Filter filter, DataBase dataBase)
            where T : P
        {
            var total = await queryable.CountAsync();
            var filteredCount = total;
            if (filter != null && filter.Logic != null)
            {
                // Filter the data first
                queryable = Filter(queryable, filter);

                // Calculate the total number of records (needed for paging)
                filteredCount = await queryable.CountAsync();
            }

            // Sort the data
            queryable = Sort(queryable, sort);

            // Finally page the data
            if (take > 0)
            {
                if (dataBase == DataBase.SQLServer)
                    queryable = PageSQL(queryable, take, skip);
                if (dataBase == DataBase.Oracle)
                    queryable = PageOracle(queryable, take, skip);
            }

            return new LinqDataResult<P>
            {
                Data = queryable.AsEnumerable().Cast<P>(),
                RecordsTotal = total,
                RecordsFiltered = filteredCount,
            };
        }


        //private static IQueryable<T> Filter<T>(IQueryable<T> queryable, Filter filter)
        //{
        //    if (filter != null && filter.Logic != null)
        //    {
        //        // Collect a flat list of all filters
        //        var filters = filter.GetFlat();

        //        // Get all filter values as array (needed by the Where method of Dynamic Linq)
        //        var values = filters.Select(f => f.Value).ToArray();

        //        // generate expression e.g. Field1 = @0 And Field2 > @1
        //        string predicate = filter.ToExpression(filters);

        //        // Use the Where method of Dynamic Linq to filter the data
        //        queryable = queryable.Where(predicate, values);
        //    }

        //    return queryable;
        //}

        /// <summary>
        /// this method added for fixing error in datagrid in react
        /// import React from "react";
        /// import axios from "axios";
        /// import { DataGrid
        /// from "@mui/x-data-grid";
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private static IQueryable<T> Filter<T>(IQueryable<T> queryable, Filter filter)
        {
            if (filter != null && filter.Logic != null)
            {
                var filters = filter.GetFlat();

                var values = new List<object>();
                var expressions = new List<string>();

                foreach (var f in filters)
                {
                    if(f.Field == "id")
                    {
                        f.Field = "ID";
                    }
                    var fieldName = char.ToUpper(f.Field[0]) + f.Field.Substring(1);
                    var propertyType = typeof(T).GetProperty(fieldName)?.PropertyType;

                    //var propertyType = typeof(T).GetProperty(f.Field)?.PropertyType;
                    object parsedValue = f.Value;

                    if (propertyType != null)
                    {
                        var nonNullableType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

                        parsedValue = Convert.ChangeType(f.Value, nonNullableType);

                        if (nonNullableType == typeof(string))
                        {
                            expressions.Add($"{f.Field}.Contains(@{values.Count})");
                        }
                        else
                        {
                            expressions.Add($"{f.Field} == @{values.Count}");
                        }

                        values.Add(parsedValue);
                    }
                }

                // Combine expressions with AND/OR logic
                string predicate = string.Join($" {filter.Logic} ", expressions);

                queryable = queryable.Where(predicate, values.ToArray());
            }

            return queryable;
        }



        private static IQueryable<T> Sort<T>(IQueryable<T> queryable, IEnumerable<Sort> sort)
        {
            if (sort != null && sort.Any())
            {
                // order by expression: e.g. Field1 asc, Field2 desc
                var ordering = String.Join(",", sort.Select(s => s.ToExpression()));

                // Use the OrderBy method of Dynamic Linq to sort the data
                return queryable.OrderBy(ordering);
            }

            return queryable;
        }

        private static IQueryable<T> PageSQL<T>(IQueryable<T> queryable, int take, int skip)
        {
            var rtn = queryable.Skip(skip).Take(take);
            return rtn;
        }

        private static IQueryable<T> PageOracle<T>(IQueryable<T> queryable, int take, int skip)
        {
            var rtn = queryable.AsEnumerable().Skip(skip).Take(take).AsQueryable();
            return rtn;
        }
    }
}
