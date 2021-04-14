using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public static class ConfigurableHelper
	{
		public static DataColumn[] GetCommonDataColumns()
		{
			DataColumn dataColumn = new DataColumn("Identity", typeof(object));
			dataColumn.ExtendedProperties["LambdaExpression"] = "DistinguishedName,Guid=>WinformsHelper.GenerateADObjectId(Guid(@0[\"Guid\"]),@0[DistinguishedName].ToString())";
			return new DataColumn[]
			{
				dataColumn,
				new DataColumn("Name", typeof(string)),
				new DataColumn("ObjectClass", typeof(string)),
				new DataColumn("Guid", typeof(Guid)),
				new DataColumn("DistinguishedName", typeof(string))
			};
		}

		public static DataTable GetCommonDataTable()
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.AddRange(ConfigurableHelper.GetCommonDataColumns());
			dataTable.PrimaryKey = new DataColumn[]
			{
				dataTable.Columns["Guid"]
			};
			return dataTable;
		}

		public static DataColumn GetDataColumnWithExpectedType(string columnName, Type expectedType)
		{
			DataColumn dataColumn = new DataColumn(columnName, typeof(object));
			dataColumn.ExtendedProperties["ExpectedType"] = expectedType;
			return dataColumn;
		}

		public static void AddColumnWithExpectedType(this DataColumnCollection columns, string columnName, Type expectedType)
		{
			columns.Add(ConfigurableHelper.GetDataColumnWithExpectedType(columnName, expectedType));
		}

		public static void AddColumnWithLambdaExpression(this DataColumnCollection columns, string columnName, Type columnType, string lambdaExpression)
		{
			DataColumn dataColumn = new DataColumn(columnName, columnType);
			dataColumn.ExtendedProperties["LambdaExpression"] = lambdaExpression;
			columns.Add(dataColumn);
		}
	}
}
