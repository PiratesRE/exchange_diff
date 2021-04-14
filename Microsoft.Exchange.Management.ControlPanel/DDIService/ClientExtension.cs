using System;
using System.Data;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class ClientExtension
	{
		public static void GetPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			ExtensionUtility.ExtensionGetPostAction(false, inputRow, dataTable, store);
		}
	}
}
