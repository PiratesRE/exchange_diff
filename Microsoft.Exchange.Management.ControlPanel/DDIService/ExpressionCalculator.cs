using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Management.ControlPanel.DDI;
using Microsoft.Exchange.Management.SystemManager;

namespace Microsoft.Exchange.Management.DDIService
{
	public sealed class ExpressionCalculator
	{
		public static ExpressionCalculator Parse(DataTable table)
		{
			return ExpressionCalculator.Parse(table, "LambdaExpression");
		}

		public static ExpressionCalculator Parse(DataTable table, string lambdaExpressionKey)
		{
			ExpressionCalculator expressionCalculator = new ExpressionCalculator();
			string text = string.Empty;
			try
			{
				IEnumerable<DataColumn> enumerable = from DataColumn o in table.Columns
				where o.ExtendedProperties.Contains(lambdaExpressionKey)
				select o;
				foreach (DataColumn dataColumn in enumerable)
				{
					text = dataColumn.ExtendedProperties[lambdaExpressionKey].ToString();
					ColumnExpression columnExpression = ExpressionCalculator.BuildColumnExpression(text);
					columnExpression.ResultColumn = dataColumn.ColumnName;
					expressionCalculator.query.Add(columnExpression);
				}
			}
			catch (ParseException innerException)
			{
				throw new LambdaExpressionException(text, innerException);
			}
			catch (TargetInvocationException innerException2)
			{
				throw new LambdaExpressionException(text, innerException2);
			}
			return expressionCalculator;
		}

		public static ColumnExpression BuildColumnExpression(string lambdaExpression)
		{
			ColumnExpression result;
			try
			{
				result = ExpressionCalculator.BuildColumnExpression(lambdaExpression);
			}
			catch (ParseException innerException)
			{
				throw new LambdaExpressionException(lambdaExpression, innerException);
			}
			catch (TargetInvocationException innerException2)
			{
				throw new LambdaExpressionException(lambdaExpression, innerException2);
			}
			return result;
		}

		public static object CalculateLambdaExpression(ColumnExpression expression, Type dataType, DataRow dataRow, DataRow inputRow)
		{
			object result;
			try
			{
				Type[] servicePredefinedTypes = null;
				if (inputRow != null)
				{
					DataObjectStore dataObjectStore = inputRow.Table.ExtendedProperties["DataSourceStore"] as DataObjectStore;
					if (dataObjectStore != null)
					{
						servicePredefinedTypes = dataObjectStore.ServicePredefinedTypes;
					}
				}
				result = ExpressionCalculator.CalculateLambdaExpression(expression, dataType, servicePredefinedTypes, dataRow, inputRow);
			}
			catch (ParseException innerException)
			{
				throw new LambdaExpressionException(expression.Expression, innerException);
			}
			catch (TargetInvocationException innerException2)
			{
				throw new LambdaExpressionException(expression.Expression, innerException2);
			}
			return result;
		}

		internal static Delegate CompileLambdaExpression(ColumnExpression expression, Type dataType, Type[] servicePredefinedTypes, DataRow dataRow, DataRow inputRow)
		{
			Delegate result;
			try
			{
				result = ExpressionCalculator.CompileLambdaExpression(expression, dataType, servicePredefinedTypes, dataRow, inputRow);
			}
			catch (ParseException innerException)
			{
				throw new LambdaExpressionException(expression.Expression, innerException);
			}
			catch (TargetInvocationException innerException2)
			{
				throw new LambdaExpressionException(expression.Expression, innerException2);
			}
			return result;
		}

		public int Count
		{
			get
			{
				return this.query.Count;
			}
		}

		public IList<KeyValuePair<string, object>> CalculateAll(DataRow dataRow, DataRow inputRow)
		{
			return this.CalculateCore(dataRow, inputRow, this.query);
		}

		public IList<KeyValuePair<string, object>> CalculateSpecifiedColumn(string column, DataRow dataRow)
		{
			return this.CalculateSpecifiedColumn(column, dataRow, null);
		}

		public IList<KeyValuePair<string, object>> CalculateAffectedColumns(string changedColumn, DataRow dataRow, DataRow inputRow)
		{
			IEnumerable<ColumnExpression> source = from c in this.query
			where c.DependentColumns.Contains(changedColumn)
			select c;
			return this.CalculateCore(dataRow, inputRow, source.ToArray<ColumnExpression>());
		}

		public IList<KeyValuePair<string, object>> CalculateSpecifiedColumn(string column, DataRow dataRow, DataRow inputRow)
		{
			IEnumerable<ColumnExpression> source = from c in this.query
			where c.ResultColumn == column
			select c;
			return this.CalculateCore(dataRow, inputRow, source.ToArray<ColumnExpression>());
		}

		private Type GetColumnRealDataType(DataColumn dataColumn)
		{
			return ((Type)dataColumn.ExtendedProperties["RealDataType"]) ?? dataColumn.DataType;
		}

		private IList<KeyValuePair<string, object>> CalculateCore(DataRow dataRow, DataRow inputRow, IList<ColumnExpression> query)
		{
			List<KeyValuePair<string, object>> list = new List<KeyValuePair<string, object>>();
			foreach (ColumnExpression columnExpression in query)
			{
				list.Add(new KeyValuePair<string, object>(columnExpression.ResultColumn, ExpressionCalculator.CalculateLambdaExpression(columnExpression, this.GetColumnRealDataType(dataRow.Table.Columns[columnExpression.ResultColumn]), dataRow, inputRow)));
			}
			return list;
		}

		private IList<ColumnExpression> query = new List<ColumnExpression>();
	}
}
