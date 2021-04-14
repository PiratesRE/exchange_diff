using System;
using System.Data;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public class AddressList
	{
		public static void GetObjectPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			EmailAddressPolicy.GetObjectPostAction(inputRow, dataTable, store);
		}

		public static void NewObjectPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store, PowerShellResults[] results)
		{
			DDIUtil.InsertWarningIfSucceded(results, Strings.NewAddressListWarning);
		}

		public static void SetObjectPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store, PowerShellResults[] results)
		{
			DDIUtil.InsertWarningIfSucceded(results, Strings.EditAddressListWarning);
		}
	}
}
