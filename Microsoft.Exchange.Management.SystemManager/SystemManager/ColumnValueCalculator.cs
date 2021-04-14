using System;
using System.ComponentModel;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class ColumnValueCalculator
	{
		public ColumnValueCalculator(string dependentColumn, string resultColumn, TypeConverter typeConverter)
		{
			this.DependentColumn = dependentColumn;
			this.ResultColumn = resultColumn;
			this.TypeConverter = typeConverter;
		}

		public string DependentColumn { get; private set; }

		public string ResultColumn { get; private set; }

		public TypeConverter TypeConverter { get; private set; }

		public void Calculate(DataRow dataRow)
		{
			object obj = dataRow[this.DependentColumn];
			Type dataType = dataRow.Table.Columns[this.DependentColumn].DataType;
			if (!DBNull.Value.Equals(obj) && dataType.IsEnum)
			{
				obj = Enum.ToObject(dataType, obj);
			}
			dataRow[this.ResultColumn] = this.TypeConverter.ConvertFrom(obj);
		}

		public void Calcuate(DataTable table)
		{
			foreach (object obj in table.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				this.Calculate(dataRow);
			}
		}

		public static void CalculateAll(DataRow dataRow)
		{
			foreach (object obj in dataRow.Table.Columns)
			{
				DataColumn dataColumn = (DataColumn)obj;
				if (dataColumn.ExtendedProperties["ColumnValueCalculator"] != null)
				{
					(dataColumn.ExtendedProperties["ColumnValueCalculator"] as ColumnValueCalculator).Calculate(dataRow);
				}
			}
		}

		public static void CalculateAll(DataTable table)
		{
			foreach (object obj in table.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				ColumnValueCalculator.CalculateAll(dataRow);
			}
		}

		public const string ColumnValueCalculatorProperty = "ColumnValueCalculator";
	}
}
