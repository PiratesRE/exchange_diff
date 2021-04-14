using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Entities.EntitySets.Linq.ExpressionVisitors
{
	internal static class StringMethods
	{
		public static MatchOptions? GetTextFilterMatchOptions(this MethodInfo method)
		{
			if (method == StringMethods.Contains)
			{
				return new MatchOptions?(MatchOptions.SubString);
			}
			if (method == StringMethods.EndsWith)
			{
				return new MatchOptions?(MatchOptions.Suffix);
			}
			if (method == StringMethods.StartsWith)
			{
				return new MatchOptions?(MatchOptions.Prefix);
			}
			return null;
		}

		private static MethodInfo GetMethod<TReturn>(Expression<Func<string, TReturn>> expression)
		{
			return ((MethodCallExpression)expression.Body).Method;
		}

		public static readonly MethodInfo Compare = StringMethods.GetMethod<int>((string x) => string.Compare(null, null));

		public static readonly MethodInfo Contains = StringMethods.GetMethod<bool>((string s) => s.Contains(null));

		public static readonly MethodInfo EndsWith = StringMethods.GetMethod<bool>((string s) => s.EndsWith(null));

		public static readonly MethodInfo StartsWith = StringMethods.GetMethod<bool>((string s) => s.StartsWith(null));
	}
}
