using System;
using System.Collections.Generic;
using System.Data;
using System.Security;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class MasterAccountPicker
	{
		public static void GetListPreAction(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			if (input["UserName"] != DBNull.Value && input["Password"] != DBNull.Value)
			{
				List<string> list = new List<string>();
				string text = (string)input["UserName"];
				if (!string.IsNullOrEmpty(text))
				{
					input["Credential"] = text.ToPSCredential((SecureString)input["Password"]);
					list.Add("Credential");
					store.SetModifiedColumns(list);
				}
			}
		}
	}
}
