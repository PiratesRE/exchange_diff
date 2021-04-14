using System;
using System.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.DDIService
{
	public class MailboxDatabasePicker : DDICodeBehind
	{
		public static void GetMailboxDatabasePostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DatabasePropertiesHelper.FilterRecoveryDatabase(dataTable);
			int currentExchangeMajorVersion = Server.CurrentExchangeMajorVersion;
			string text = inputRow["Version"] as string;
			if (text == "*")
			{
				return;
			}
			if (!string.IsNullOrEmpty(text))
			{
				int.TryParse(text, out currentExchangeMajorVersion);
			}
			DatabasePropertiesHelper.FilterRowsByAdminDisplayVersion(inputRow, dataTable, store, currentExchangeMajorVersion, null);
		}
	}
}
