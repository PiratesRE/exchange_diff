using System;
using System.Linq.Expressions;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Entities.DataModel;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class DataEntityPropertyProvider<T> : GenericPropertyProvider<T>, IExpressionQueryBuilder where T : IEntity
	{
		public DataEntityPropertyProvider(string propertyName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("propertyName", propertyName);
			this.PropertyName = propertyName;
		}

		public string PropertyName { get; private set; }

		public Func<object, ConstantExpression> QueryConstantBuilder { get; set; }

		public Func<MemberExpression> PropertyExpressionBuilder { get; set; }

		public virtual MemberExpression GetQueryPropertyExpression()
		{
			if (this.PropertyExpressionBuilder != null)
			{
				return this.PropertyExpressionBuilder();
			}
			return Expression.Property(DataEntityPropertyProvider<T>.LamdaParameter, typeof(T), this.PropertyName);
		}

		public virtual ConstantExpression GetQueryConstant(object value)
		{
			if (this.QueryConstantBuilder != null)
			{
				return this.QueryConstantBuilder(value);
			}
			return Expression.Constant(value);
		}

		private static readonly ParameterExpression LamdaParameter = Expression.Parameter(typeof(T), "x");
	}
}
