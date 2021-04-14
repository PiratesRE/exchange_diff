using System;
using System.Linq.Expressions;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal interface IExpressionQueryBuilder
	{
		MemberExpression GetQueryPropertyExpression();

		ConstantExpression GetQueryConstant(object value);
	}
}
