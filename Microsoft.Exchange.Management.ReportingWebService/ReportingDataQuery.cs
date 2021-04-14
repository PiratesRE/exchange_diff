using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	internal class ReportingDataQuery<T> : IOrderedQueryable<T>, IQueryable<T>, IEnumerable<T>, IOrderedQueryable, IQueryable, IEnumerable, IQueryProvider
	{
		public ReportingDataQuery(IReportingDataSource dataSource, IEntity entity) : this(dataSource, entity, null)
		{
			this.expression = Expression.Constant(this);
		}

		public ReportingDataQuery(IReportingDataSource dataSource, IEntity entity, Expression expression)
		{
			if (!typeof(T).IsAssignableFrom(entity.ClrType))
			{
				throw new ArgumentException("The underline clr type of the resource entity doesn't match the generic type T");
			}
			this.dataSource = dataSource;
			this.entity = entity;
			this.expression = expression;
		}

		public ReportingDataQuery(IReportingDataSource dataSource, IEntity entity, Expression expression, Expression expressionWithouSelect)
		{
			if (expressionWithouSelect == null)
			{
				throw new ArgumentNullException("expressionWithouSelect");
			}
			this.dataSource = dataSource;
			this.entity = entity;
			this.expression = expression;
			this.expressionWithouSelect = expressionWithouSelect;
		}

		public Type ElementType
		{
			get
			{
				return typeof(T);
			}
		}

		public Expression Expression
		{
			get
			{
				return this.expression;
			}
		}

		public IQueryProvider Provider
		{
			get
			{
				return this;
			}
		}

		internal Expression ExpressionWithouSelect
		{
			get
			{
				return this.expressionWithouSelect;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			if (!typeof(T).IsAssignableFrom(this.entity.ClrType))
			{
				throw new NotSupportedException("The underline clr type of the resouce entity doesn't match the generic type T");
			}
			IEnumerator<T> enumerator = null;
			ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.CmdletResponseTime, delegate
			{
				enumerator = this.dataSource.GetData<T>(this.entity, this.expression).GetEnumerator();
			});
			return enumerator;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			Expression queryExpression = this.expressionWithouSelect ?? this.expression;
			IEnumerator enumerator = null;
			ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.CmdletResponseTime, delegate
			{
				enumerator = this.dataSource.GetData(this.entity, queryExpression).GetEnumerator();
			});
			return enumerator;
		}

		public IQueryable CreateQuery(Expression expression)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			Type elementType = ReportingDataQuery<T>.TypeSystem.GetElementType(expression.Type);
			Type type = typeof(IQueryable<>).MakeGenericType(new Type[]
			{
				elementType
			});
			if (!type.IsAssignableFrom(expression.Type))
			{
				throw new ArgumentException("The underline clr type of the expression doesn't match the generic types");
			}
			Expression expression2;
			object[] args;
			if (this.IsNewQueryableForSelect(this.Expression, expression, out expression2))
			{
				args = new object[]
				{
					this.dataSource,
					this.entity,
					expression,
					expression2
				};
			}
			else
			{
				args = new object[]
				{
					this.dataSource,
					this.entity,
					expression
				};
			}
			return (IQueryable)Activator.CreateInstance(typeof(ReportingDataQuery<>).MakeGenericType(new Type[]
			{
				elementType
			}), args);
		}

		public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			if (!typeof(IQueryable<TElement>).IsAssignableFrom(expression.Type))
			{
				throw new ArgumentException("The underline clr type of the expression doesn't match the generic type TElement");
			}
			Expression expression2;
			if (this.IsNewQueryableForSelect(this.Expression, expression, out expression2))
			{
				return new ReportingDataQuery<TElement>(this.dataSource, this.entity, expression, expression2);
			}
			return new ReportingDataQuery<TElement>(this.dataSource, this.entity, expression);
		}

		public object Execute(Expression expression)
		{
			return this.Execute<object>(expression);
		}

		public TElement Execute<TElement>(Expression expression)
		{
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			IQueryable result = null;
			ElapsedTimeWatcher.Watch(RequestStatistics.RequestStatItem.CmdletResponseTime, delegate
			{
				result = this.dataSource.GetData(this.entity, this.expression).AsQueryable();
			});
			Expression expression2 = ReportingDataQuery<T>.DecorateExpression(result, (MethodCallExpression)expression);
			return result.Provider.Execute<TElement>(expression2);
		}

		private static Expression DecorateExpression(IQueryable query, MethodCallExpression oldExpression)
		{
			Stack<MethodCallExpression> stack = new Stack<MethodCallExpression>();
			while (oldExpression != null && oldExpression.Arguments != null && oldExpression.Arguments.Count >= 1 && !(oldExpression.Method == null) && oldExpression.Arguments[0] != null)
			{
				stack.Push(oldExpression);
				oldExpression = (oldExpression.Arguments[0] as MethodCallExpression);
			}
			Expression expression = query.Expression;
			foreach (MethodCallExpression methodCallExpression in stack)
			{
				MethodInfo method = methodCallExpression.Method;
				List<Expression> list = new List<Expression>(methodCallExpression.Arguments.Count);
				list.Add(expression);
				list.AddRange(methodCallExpression.Arguments.Skip(1));
				expression = Expression.Call(method, list);
			}
			return expression;
		}

		private bool IsNewQueryableForSelect(Expression currentQuery, Expression newQuery, out Expression newQueryWithoutSelect)
		{
			ReportingDataQuery<T>.SelectQueryFilter selectQueryFilter = new ReportingDataQuery<T>.SelectQueryFilter();
			ReportingDataQuery<T>.SelectQueryFilter selectQueryFilter2 = new ReportingDataQuery<T>.SelectQueryFilter();
			selectQueryFilter.Visit(currentQuery);
			newQueryWithoutSelect = selectQueryFilter2.Visit(newQuery);
			return !selectQueryFilter.HasSelectQuery && selectQueryFilter2.HasSelectQuery;
		}

		private readonly IReportingDataSource dataSource;

		private readonly IEntity entity;

		private readonly Expression expression;

		private readonly Expression expressionWithouSelect;

		private static class TypeSystem
		{
			internal static Type GetElementType(Type seqType)
			{
				Type type = ReportingDataQuery<T>.TypeSystem.FindIEnumerable(seqType);
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
						Type type3 = ReportingDataQuery<T>.TypeSystem.FindIEnumerable(seqType2);
						if (type3 != null)
						{
							return type3;
						}
					}
				}
				if (seqType.BaseType != null && seqType.BaseType != typeof(object))
				{
					return ReportingDataQuery<T>.TypeSystem.FindIEnumerable(seqType.BaseType);
				}
				return null;
			}
		}

		private class SelectQueryFilter : ExpressionVisitor
		{
			internal bool HasSelectQuery { get; private set; }

			protected override Expression VisitMethodCall(MethodCallExpression node)
			{
				if (node.Method.DeclaringType == typeof(Queryable) && node.Method.Name.Equals("select", StringComparison.InvariantCultureIgnoreCase))
				{
					this.HasSelectQuery = true;
					return this.Visit(node.Arguments[0]);
				}
				return base.VisitMethodCall(node);
			}
		}
	}
}
