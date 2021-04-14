using System;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class OtherUPNSuffixFiller : AbstractDataTableFiller
	{
		private static string GetColumnValue(DataRow row, string column)
		{
			string result = null;
			if (row != null && row.Table.Columns.Contains(column) && !DBNull.Value.Equals(row[column]))
			{
				result = (string)row[column];
			}
			return result;
		}

		public OtherUPNSuffixFiller(string inputSuffixColumn, string fillColumn)
		{
			this.inputSuffixColumn = inputSuffixColumn;
			this.fillColumn = fillColumn;
		}

		public override ICommandBuilder CommandBuilder
		{
			get
			{
				return null;
			}
		}

		public override void BuildCommand(string searchText, object[] pipeline, DataRow row)
		{
			this.otherUPNSuffix = OtherUPNSuffixFiller.GetColumnValue(row, this.inputSuffixColumn);
		}

		public override void BuildCommandWithScope(string searchText, object[] pipeline, DataRow row, object scope)
		{
			this.otherUPNSuffix = OtherUPNSuffixFiller.GetColumnValue(row, this.inputSuffixColumn);
		}

		protected override void OnFill(DataTable table)
		{
			if (!string.IsNullOrEmpty(this.otherUPNSuffix) && !table.Rows.Contains(this.otherUPNSuffix))
			{
				DataRow dataRow = table.NewRow();
				Type dataType = table.Columns[this.fillColumn].DataType;
				if (dataType.IsAssignableFrom(typeof(SmtpDomainWithSubdomains)))
				{
					dataRow[this.fillColumn] = new SmtpDomainWithSubdomains(this.otherUPNSuffix);
				}
				else
				{
					dataRow[this.fillColumn] = this.otherUPNSuffix;
				}
				table.BeginLoadData();
				table.Rows.Add(dataRow);
				table.EndLoadData();
			}
		}

		public override object Clone()
		{
			return new OtherUPNSuffixFiller(this.inputSuffixColumn, this.fillColumn);
		}

		private string inputSuffixColumn;

		private string fillColumn;

		private string otherUPNSuffix;
	}
}
