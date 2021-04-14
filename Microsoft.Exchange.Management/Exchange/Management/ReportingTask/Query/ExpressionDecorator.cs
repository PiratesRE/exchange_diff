using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Tasks;
using Microsoft.Exchange.Management.ReportingTask.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ReportingTask.Query
{
	internal class ExpressionDecorator<TReportObject> : QueryDecorator<TReportObject> where TReportObject : ReportObject
	{
		public ExpressionDecorator(ITaskContext taskContext) : base(taskContext)
		{
			base.IsEnforced = true;
		}

		public Expression Expression { get; set; }

		public override QueryOrder QueryOrder
		{
			get
			{
				return QueryOrder.Expression;
			}
		}

		public override IQueryable<TReportObject> GetQuery(IQueryable<TReportObject> query)
		{
			if (this.methodCallExpressionStack == null || this.methodCallExpressionStack.Count < 1)
			{
				return query;
			}
			query = this.CreateNewQuery(query);
			return query;
		}

		public override void Validate()
		{
			base.Validate();
			if (ExTraceGlobals.LogTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				ExTraceGlobals.LogTracer.Information<Expression>(0L, "Expression: {0}", this.Expression);
			}
			this.ValidateType();
			this.ExtractMethodCallExpressionStack();
		}

		private void ExtractMethodCallExpressionStack()
		{
			if (this.Expression == null)
			{
				return;
			}
			this.methodCallExpressionStack = new Stack<MethodCallExpression>(100);
			for (MethodCallExpression methodCallExpression = this.Expression as MethodCallExpression; methodCallExpression != null; methodCallExpression = (methodCallExpression.Arguments[0] as MethodCallExpression))
			{
				if (this.methodCallExpressionStack.Count >= 100)
				{
					throw new InvalidExpressionException(Strings.TooManyEmbeddedExpressions(100));
				}
				if (methodCallExpression.Arguments == null || methodCallExpression.Arguments.Count < 2 || methodCallExpression.Method == null || methodCallExpression.Arguments[0] == null || methodCallExpression.Arguments[1] == null)
				{
					throw new InvalidExpressionException(Strings.InvalidExpression(this.Expression.ToString()));
				}
				this.methodCallExpressionStack.Push(methodCallExpression);
			}
		}

		private void ValidateType()
		{
			if (this.Expression != null)
			{
				Type elementType = ExpressionDecorator<TReportObject>.TypeSystem.GetElementType(this.Expression.Type);
				if (elementType != null && elementType != typeof(TReportObject))
				{
					throw new InvalidExpressionException(Strings.InvalidTypeOfExpression(typeof(TReportObject).ToString(), elementType.ToString()));
				}
			}
		}

		private IQueryable<TReportObject> CreateNewQuery(IQueryable<TReportObject> query)
		{
			IQueryable<TReportObject> result;
			try
			{
				Expression expression = query.Expression;
				foreach (MethodCallExpression methodCallExpression in this.methodCallExpressionStack)
				{
					MethodInfo method = methodCallExpression.Method;
					List<Expression> list = new List<Expression>(methodCallExpression.Arguments.Count);
					list.Add(expression);
					list.AddRange(methodCallExpression.Arguments.Skip(1));
					expression = Expression.Call(method, list);
				}
				IQueryable<TReportObject> queryable = query.Provider.CreateQuery<TReportObject>(expression);
				result = queryable;
			}
			catch (ArgumentException innerException)
			{
				throw new InvalidExpressionException(Strings.InvalidExpression(this.Expression.ToString()), innerException);
			}
			return result;
		}

		public const int MaxEmbeddedExpressionCount = 100;

		private Stack<MethodCallExpression> methodCallExpressionStack;

		private static class TypeSystem
		{
			internal static Type GetElementType(Type seqType)
			{
				Type type = ExpressionDecorator<TReportObject>.TypeSystem.FindIEnumerable(seqType);
				if (type == null)
				{
					return seqType;
				}
				return type.GetGenericArguments()[0];
			}

			private static Type FindIEnumerable(Type seqType)
			{
				if (seqType == null || seqType == typeof(string))
				{
					return null;
				}
				if (seqType.IsArray)
				{
					return typeof(IEnumerable<>).MakeGenericType(new Type[]
					{
						seqType.GetElementType()
					});
				}
				if (seqType.IsGenericType)
				{
					foreach (Type type in seqType.GetGenericArguments())
					{
						Type type2 = typeof(IEnumerable<>).MakeGenericType(new Type[]
						{
							type
						});
						if (type2.IsAssignableFrom(seqType))
						{
							return type2;
						}
					}
				}
				Type[] interfaces = seqType.GetInterfaces();
				if (interfaces != null && interfaces.Length > 0)
				{
					foreach (Type seqType2 in interfaces)
					{
						Type type3 = ExpressionDecorator<TReportObject>.TypeSystem.FindIEnumerable(seqType2);
						if (type3 != null)
						{
							return type3;
						}
					}
				}
				if (seqType.BaseType != null && seqType.BaseType != typeof(object))
				{
					return ExpressionDecorator<TReportObject>.TypeSystem.FindIEnumerable(seqType.BaseType);
				}
				return null;
			}
		}
	}
}
