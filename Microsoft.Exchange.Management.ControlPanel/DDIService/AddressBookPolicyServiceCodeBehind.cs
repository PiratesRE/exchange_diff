using System;
using System.Data;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public class AddressBookPolicyServiceCodeBehind
	{
		public static void SortAndAddNoPolicyRow(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow["ABPName"] = Strings.AddressBookPolicyNoPolicy;
			dataRow["ABPIdentity"] = string.Empty;
			dataRow["IsNoPolicy"] = true.ToString();
			dataTable.Rows.InsertAt(dataRow, 0);
			dataTable.DefaultView.Sort = "IsNoPolicy DESC, ABPName ASC";
		}
	}
}
