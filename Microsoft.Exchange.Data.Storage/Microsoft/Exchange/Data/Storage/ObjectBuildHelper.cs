using System;
using System.Linq.Expressions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ObjectBuildHelper<T> where T : new()
	{
		public static T Build()
		{
			return ObjectBuildHelper<T>.func();
		}

		private static Expression<Func<T>> expression = Expression.Lambda<Func<T>>(Expression.New(typeof(T)), new ParameterExpression[0]);

		private static Func<T> func = ObjectBuildHelper<T>.expression.Compile();
	}
}
