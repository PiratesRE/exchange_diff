using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.EntitySets.Linq.ExpressionVisitors;

namespace Microsoft.Exchange.Entities.EntitySets.Linq.ExtensionMethods
{
	internal static class OrderByClauseExtensions
	{
		public static IOrderedQueryable ApplyTo(this OrderByClause clause, IQueryable query, bool alreadyOrdered)
		{
			LambdaExpression lambdaExpression = (LambdaExpression)clause.Expression.RemoveQuote();
			MethodInfo methodInfo;
			if (alreadyOrdered)
			{
				methodInfo = ((clause.Direction == ListSortDirection.Ascending) ? QueryableMethods.ThenBy : QueryableMethods.ThenByDescending);
			}
			else
			{
				methodInfo = ((clause.Direction == ListSortDirection.Ascending) ? QueryableMethods.OrderBy : QueryableMethods.OrderByDescending);
			}
			Type[] typeArguments = new Type[]
			{
				query.ElementType,
				lambdaExpression.Body.Type
			};
			MethodInfo methodInfo2 = methodInfo.MakeGenericMethod(typeArguments);
			object[] parameters = new object[]
			{
				query,
				lambdaExpression
			};
			return methodInfo2.Invoke(null, parameters) as IOrderedQueryable;
		}
	}
}
