using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.Exchange.Management.SystemManager
{
	public sealed class ExpressionCalculator
	{
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

		public static ExpressionCalculator Parse(DataTable table)
		{
			return ExpressionCalculator.Parse(table, "LambdaExpression");
		}

		public static ExpressionCalculator Parse(DataTable table, string lambdaExpressionKey)
		{
			IEnumerable<DataColumn> enumerable = from DataColumn o in table.Columns
			where o.ExtendedProperties.Contains(lambdaExpressionKey)
			select o;
			ExpressionCalculator expressionCalculator = new ExpressionCalculator();
			foreach (DataColumn dataColumn in enumerable)
			{
				string lambdaExpression = dataColumn.ExtendedProperties[lambdaExpressionKey].ToString();
				ColumnExpression columnExpression = ExpressionCalculator.BuildColumnExpression(lambdaExpression);
				columnExpression.ResultColumn = dataColumn.ColumnName;
				expressionCalculator.query.Add(columnExpression);
			}
			return expressionCalculator;
		}

		public static ColumnExpression BuildColumnExpression(string lambdaExpression)
		{
			ColumnExpression columnExpression = new ColumnExpression();
			string[] array = lambdaExpression.Split(new string[]
			{
				"=>"
			}, StringSplitOptions.None);
			foreach (string text in array[0].Split(new char[]
			{
				','
			}, StringSplitOptions.None))
			{
				columnExpression.DependentColumns.Add(text.Trim());
			}
			columnExpression.Expression = array[1];
			return columnExpression;
		}

		public static object CalculateLambdaExpression(ColumnExpression expression, Type dataType, DataRow dataRow, DataRow inputRow)
		{
			return ExpressionCalculator.CalculateLambdaExpression(expression, dataType, null, dataRow, inputRow);
		}

		public static object CalculateLambdaExpression(ColumnExpression expression, Type dataType, Type[] servicePredefinedTypes, DataRow dataRow, DataRow inputRow)
		{
			if (!ExpressionCalculator.IsCacheValid(expression, dataRow, inputRow))
			{
				expression.CachedDelegate = new CachedDelegate
				{
					TemplateDataRow = ((dataRow != null) ? dataRow.Table.Clone().NewRow() : null),
					TemplateInputRow = ((inputRow != null) ? inputRow.Table.Clone().NewRow() : null)
				};
				expression.CachedDelegate.CompiledDelegate = ExpressionCalculator.CompileLambdaExpression(expression, dataType, servicePredefinedTypes, expression.CachedDelegate.TemplateDataRow, expression.CachedDelegate.TemplateInputRow);
			}
			if (expression.CachedDelegate.TemplateDataRow != null)
			{
				expression.CachedDelegate.TemplateDataRow.ItemArray = dataRow.ItemArray;
			}
			if (expression.CachedDelegate.TemplateInputRow != null)
			{
				expression.CachedDelegate.TemplateInputRow.ItemArray = inputRow.ItemArray;
			}
			return expression.CachedDelegate.CompiledDelegate.DynamicInvoke(expression.DependentColumns.ToArray());
		}

		private static bool IsCacheValid(ColumnExpression expression, DataRow dataRow, DataRow inputRow)
		{
			if (expression.CachedDelegate == null)
			{
				return false;
			}
			bool flag = (dataRow == null && expression.CachedDelegate.TemplateDataRow == null) || (dataRow != null && expression.CachedDelegate.TemplateDataRow != null);
			bool flag2 = (inputRow == null && expression.CachedDelegate.TemplateInputRow == null) || (inputRow != null && expression.CachedDelegate.TemplateInputRow != null);
			return flag && flag2;
		}

		internal static Delegate CompileLambdaExpression(ColumnExpression expression, Type dataType, DataRow dataRow, DataRow inputRow)
		{
			return ExpressionCalculator.CompileLambdaExpression(expression, dataType, null, dataRow, inputRow);
		}

		internal static Delegate CompileLambdaExpression(ColumnExpression expression, Type dataType, Type[] servicePredefinedTypes, DataRow dataRow, DataRow inputRow)
		{
			ParameterExpression[] array = new ParameterExpression[expression.DependentColumns.Count];
			for (int i = 0; i < expression.DependentColumns.Count; i++)
			{
				array[i] = Expression.Parameter(typeof(string), expression.DependentColumns[i]);
			}
			LambdaExpression lambdaExpression = DynamicExpression.ParseLambda(array, dataType, expression.Expression, servicePredefinedTypes, new object[]
			{
				dataRow,
				inputRow
			});
			return lambdaExpression.Compile();
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
