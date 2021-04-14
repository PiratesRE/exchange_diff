using System;
using System.Data;

namespace Microsoft.Exchange.Management.DDIService
{
	public class UMHuntGroupService : DDICodeBehind
	{
		public static void OnPostGetObjectForNew(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			dataRow["UMDialPlan"] = (string)store.GetValue("UMDialPlan", "Name");
		}

		private const string UMHuntGroupName = "UMHuntGroup";

		private const string UMDialPlanName = "UMDialPlan";

		private const string UMIPGatewayName = "UMIPGateway";
	}
}
