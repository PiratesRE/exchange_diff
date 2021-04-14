using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Exchange.Entities.EntitySets.Linq.ExpressionVisitors
{
	internal static class QueryableMethods
	{
		private static MethodInfo GetMethod<TReturn>(Expression<Func<object, TReturn>> expression)
		{
			return expression.GetGenericMethodDefinition();
		}

		public static readonly MethodInfo Count = QueryableMethods.GetMethod<int>((object _) => QueryableMethods.Queryable.Count<int>());

		public static readonly MethodInfo IndexedSelect = QueryableMethods.GetMethod<IQueryable<int>>((object _) => QueryableMethods.Queryable.Select(QueryableMethods.IndexedSelectExpression));

		public static readonly MethodInfo LongCount = QueryableMethods.GetMethod<long>((object _) => QueryableMethods.Queryable.LongCount<int>());

		public static readonly MethodInfo OrderBy = QueryableMethods.GetMethod<IOrderedQueryable<int>>((object _) => QueryableMethods.Queryable.OrderBy(QueryableMethods.OrderingExpression));

		public static readonly MethodInfo OrderByDescending = QueryableMethods.GetMethod<IOrderedQueryable<int>>((object _) => QueryableMethods.Queryable.OrderByDescending(QueryableMethods.OrderingExpression));

		public static readonly MethodInfo Select = QueryableMethods.GetMethod<IQueryable<int>>((object _) => QueryableMethods.Queryable.Select(QueryableMethods.SelectExpression));

		public static readonly MethodInfo Skip = QueryableMethods.GetMethod<IQueryable<int>>((object _) => QueryableMethods.Queryable.Skip(0));

		public static readonly MethodInfo Take = QueryableMethods.GetMethod<IQueryable<int>>((object _) => QueryableMethods.Queryable.Take(0));

		public static readonly MethodInfo ThenBy = QueryableMethods.GetMethod<IOrderedQueryable<int>>((object _) => QueryableMethods.Queryable.ThenBy(QueryableMethods.OrderingExpression));

		public static readonly MethodInfo ThenByDescending = QueryableMethods.GetMethod<IOrderedQueryable<int>>((object _) => QueryableMethods.Queryable.ThenByDescending(QueryableMethods.OrderingExpression));

		public static readonly MethodInfo Where = QueryableMethods.GetMethod<IQueryable<int>>((object _) => QueryableMethods.Queryable.Where(QueryableMethods.FilteringExpression));

		private static readonly Expression<Func<int, bool>> FilteringExpression = null;

		private static readonly Expression<Func<int, int, int>> IndexedSelectExpression = null;

		private static readonly Expression<Func<int, int>> OrderingExpression = null;

		private static readonly IOrderedQueryable<int> Queryable = null;

		private static readonly Expression<Func<int, int>> SelectExpression = null;
	}
}
