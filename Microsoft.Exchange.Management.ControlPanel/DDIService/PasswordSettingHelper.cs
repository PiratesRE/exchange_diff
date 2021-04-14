using System;
using System.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class PasswordSettingHelper
	{
		public static void GetObjectPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			RecipientTypeDetails recipientTypeDetails = (RecipientTypeDetails)dataRow["RecipientTypeDetails"];
			if (recipientTypeDetails == RecipientTypeDetails.LinkedMailbox)
			{
				dataRow["DomainUserName"] = (string)dataRow["LinkedMasterAccount"];
				return;
			}
			RbacPrincipal rbacPrincipal = RbacPrincipal.Current;
			if (rbacPrincipal.RbacConfiguration.SecurityAccessToken != null && rbacPrincipal.RbacConfiguration.SecurityAccessToken.LogonName != null)
			{
				dataRow["DomainUserName"] = rbacPrincipal.RbacConfiguration.SecurityAccessToken.LogonName;
				return;
			}
			dataRow["DomainUserName"] = string.Format("{0}\\{1}", ((ADObjectId)dataRow["Identity"]).DomainId.Name, (string)dataRow["SamAccountName"]);
		}
	}
}
