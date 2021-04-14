using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.Exchange.Management.SystemManager
{
	public static class DynamicExpression
	{
		public static Expression Parse(Type resultType, string expression, params object[] values)
		{
			ExpressionParser expressionParser = new ExpressionParser(null, expression, values);
			return expressionParser.Parse(resultType);
		}

		public static LambdaExpression ParseLambda(Type itType, Type resultType, string expression, params object[] values)
		{
			return DynamicExpression.ParseLambda(new ParameterExpression[]
			{
				Expression.Parameter(itType, "")
			}, resultType, expression, null, values);
		}

		public static LambdaExpression ParseLambda(ParameterExpression[] parameters, Type resultType, string expression, Type[] servicePredefinedTypes, params object[] values)
		{
			ExpressionParser expressionParser = new ExpressionParser(parameters, expression, servicePredefinedTypes, values);
			if (parameters.Length < 5)
			{
				return Expression.Lambda(expressionParser.Parse(resultType), parameters);
			}
			List<Type> list = (from param in parameters
			select param.Type).ToList<Type>();
			list.Add(resultType);
			Type delegateType = DynamicExpression.funcTypes[parameters.Length - 5].MakeGenericType(list.ToArray());
			return Expression.Lambda(delegateType, expressionParser.Parse(resultType), parameters);
		}

		public static Expression<Func<T, S>> ParseLambda<T, S>(string expression, params object[] values)
		{
			return (Expression<Func<T, S>>)DynamicExpression.ParseLambda(typeof(T), typeof(S), expression, values);
		}

		public static Type CreateClass(params DynamicProperty[] properties)
		{
			return ClassFactory.Instance.GetDynamicClass(properties);
		}

		public static Type CreateClass(IEnumerable<DynamicProperty> properties)
		{
			return ClassFactory.Instance.GetDynamicClass(properties);
		}

		private static readonly Type[] funcTypes = new Type[]
		{
			typeof(Func<, , , , , >),
			typeof(Func<, , , , , , >)
		};
	}
}
