using System;
using System.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class PreFillADObjectIdFiller : FixedDataFiller
	{
		public PreFillADObjectIdFiller(DataTable table)
		{
			base.DataTable = table;
		}

		public override object Clone()
		{
			return new PreFillADObjectIdFiller(base.DataTable);
		}
	}
}
