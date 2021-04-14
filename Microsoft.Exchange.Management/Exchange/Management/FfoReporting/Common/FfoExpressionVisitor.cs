using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Exchange.Management.ReportingTask;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	internal class FfoExpressionVisitor<TReportObject> : ExpressionVisitor where TReportObject : new()
	{
		private FfoExpressionVisitor(object task)
		{
			this.task = task;
			foreach (Tuple<PropertyInfo, ODataInput> tuple in Schema.Utilities.GetProperties<ODataInput>(typeof(TReportObject)))
			{
				this.odataFields.Add(tuple.Item1.Name, tuple.Item2);
			}
		}

		public static void Parse(Expression node, object task)
		{
			FfoExpressionVisitor<TReportObject> ffoExpressionVisitor = new FfoExpressionVisitor<TReportObject>(task);
			ffoExpressionVisitor.Visit(node);
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			if (node.NodeType == ExpressionType.Equal || node.NodeType == ExpressionType.GreaterThan)
			{
				MemberExpression memberExpression = node.Left as MemberExpression;
				if (memberExpression != null)
				{
					PropertyInfo propertyInfo = memberExpression.Member as PropertyInfo;
					ConstantExpression constantExpression = node.Right as ConstantExpression;
					if (propertyInfo != null && constantExpression != null)
					{
						if (node.NodeType == ExpressionType.Equal)
						{
							ODataInput odataInput;
							if (this.odataFields.TryGetValue(propertyInfo.Name, out odataInput))
							{
								odataInput.SetCmdletProperty(this.task, Convert.ChangeType(constantExpression.Value, propertyInfo.PropertyType));
								return node;
							}
						}
						else if (propertyInfo.Name == "Index")
						{
							this.SetSkipToken((int)constantExpression.Value + 1);
							return node;
						}
					}
				}
			}
			else if (node.NodeType == ExpressionType.AndAlso || node.NodeType == ExpressionType.OrElse)
			{
				return base.VisitBinary(node);
			}
			throw new InvalidExpressionException(Strings.InvalidQueryParameters);
		}

		protected override Expression VisitMethodCall(MethodCallExpression node)
		{
			string name;
			if ((name = node.Method.Name) != null && name == "Take")
			{
				IPageableTask pageableTask = this.task as IPageableTask;
				if (pageableTask != null)
				{
					pageableTask.PageSize = (int)((ConstantExpression)node.Arguments[1]).Value;
					this.UpdateCmdletPageParameter();
				}
			}
			return base.VisitMethodCall(node);
		}

		private void SetSkipToken(int skipValue)
		{
			if (!(this.task is IPageableTask))
			{
				throw new NotSupportedException("Paging is not supported.");
			}
			this.skiptoken = new int?(skipValue);
			this.UpdateCmdletPageParameter();
		}

		private void UpdateCmdletPageParameter()
		{
			if (this.skiptoken != null)
			{
				IPageableTask pageableTask = (IPageableTask)this.task;
				pageableTask.Page = this.skiptoken.Value / pageableTask.PageSize + 1;
			}
		}

		private const string SkipToken = "Index";

		private const string Top = "Take";

		private object task;

		private Dictionary<string, ODataInput> odataFields = new Dictionary<string, ODataInput>();

		private int? skiptoken;
	}
}
