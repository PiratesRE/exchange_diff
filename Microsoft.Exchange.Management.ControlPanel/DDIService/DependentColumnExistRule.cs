using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Exchange.Management.DDIService
{
	internal class DependentColumnExistRule : LambdaExpressionRule
	{
		protected override List<string> OnValidate(string lambdaExpression, Service profile)
		{
			List<string> list = new List<string>();
			string[] array = lambdaExpression.Split(new string[]
			{
				"=>"
			}, StringSplitOptions.None);
			string[] array2 = array[0].Split(new char[]
			{
				',',
				' '
			}, StringSplitOptions.RemoveEmptyEntries);
			string[] array3 = array2;
			for (int i = 0; i < array3.Length; i++)
			{
				string columnName = array3[i];
				if (profile.Variables.All((Variable columnProfile) => !columnName.Equals(columnProfile.Name)))
				{
					if (profile.ExtendedColumns.All((DataColumn dataColumn) => !dataColumn.ColumnName.Equals(columnName)))
					{
						list.Add(string.Format("{0} in input columns is not a valid column name in lambda expression", columnName));
					}
				}
			}
			string funcExpression = array[1];
			foreach (string text in array2)
			{
				if (!this.IsColumnUsed(funcExpression, text, "@0") && !this.IsColumnUsed(funcExpression, text, "@1"))
				{
					list.Add(string.Format("Column {0} is not used in lambda expression {1}", text, lambdaExpression));
				}
			}
			return list;
		}

		private bool IsColumnUsed(string funcExpression, string columnName, string prefix)
		{
			string value = string.Format("{0}[{1}]", prefix, columnName);
			string value2 = string.Format("{0}[\"{1}\"]", prefix, columnName);
			return funcExpression.Contains(value) || funcExpression.Contains(value2);
		}
	}
}
