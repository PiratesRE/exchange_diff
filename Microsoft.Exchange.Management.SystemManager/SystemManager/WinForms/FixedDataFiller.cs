using System;
using System.Data;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class FixedDataFiller : AbstractDataTableFiller
	{
		protected override void OnFill(DataTable table)
		{
			table.Merge(this.DataTable);
		}

		public override object Clone()
		{
			return new FixedDataFiller
			{
				DataTable = this.DataTable
			};
		}

		public DataTable DataTable { get; set; }

		public override ICommandBuilder CommandBuilder
		{
			get
			{
				return NullableCommandBuilder.Value;
			}
		}

		public override void BuildCommand(string searchText, object[] pipeline, DataRow row)
		{
		}

		public override void BuildCommandWithScope(string searchText, object[] pipeline, DataRow row, object scope)
		{
		}
	}
}
