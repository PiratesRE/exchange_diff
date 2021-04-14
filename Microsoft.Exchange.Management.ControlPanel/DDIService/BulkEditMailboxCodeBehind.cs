using System;
using System.Data;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class BulkEditMailboxCodeBehind
	{
		public static void SetADPermissionPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			MailboxPropertiesHelper.SetADPermissionPreAction(inputRow, dataTable, store);
		}

		public static int CalculateExpectedEvents(DataRow inputRow, DataRow dataTable)
		{
			int num = ((object[])inputRow["Identity"]).Length;
			int num2 = BulkEditMailboxCodeBehind.RetrieveCount(dataTable, "GrantSendOnBehalfToAdded");
			int num3 = BulkEditMailboxCodeBehind.RetrieveCount(dataTable, "GrantSendOnBehalfToRemoved");
			int num4 = BulkEditMailboxCodeBehind.RetrieveCount(dataTable, "SendAsPermissionsAdded");
			int num5 = BulkEditMailboxCodeBehind.RetrieveCount(dataTable, "SendAsPermissionsRemoved");
			int num6 = BulkEditMailboxCodeBehind.RetrieveCount(dataTable, "FullAccessPermissionsAdded");
			int num7 = BulkEditMailboxCodeBehind.RetrieveCount(dataTable, "FullAccessPermissionsRemoved");
			int num8 = 0;
			num8 += (num6 + num7) * 4;
			num8 += num4 + num5;
			if (num2 > 0)
			{
				num8++;
			}
			if (num3 > 0)
			{
				num8++;
			}
			return num8 * num;
		}

		private static int RetrieveCount(DataRow row, string columnName)
		{
			if (row[columnName] != DBNull.Value)
			{
				return ((object[])row[columnName]).Length;
			}
			return 0;
		}
	}
}
