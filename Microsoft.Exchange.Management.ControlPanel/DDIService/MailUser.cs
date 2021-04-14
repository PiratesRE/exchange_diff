using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.Management.ControlPanel.WebControls;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class MailUser
	{
		public static void GetObjectPostAction(DataRow inputRow, DataTable table, DataObjectStore store)
		{
			DataRow dataRow = table.Rows[0];
			if (dataRow["SimpleEmailAddresses"] != DBNull.Value)
			{
				dataRow["EmailAddresses"] = EmailAddressList.FromProxyAddressCollection((ProxyAddressCollection)dataRow["SimpleEmailAddresses"]);
			}
			if (dataRow["ObjectCountryOrRegion"] != DBNull.Value)
			{
				dataRow["CountryOrRegion"] = ((CountryInfo)dataRow["ObjectCountryOrRegion"]).Name;
			}
			MailboxPropertiesHelper.GetMaxSendReceiveSize(inputRow, table, store);
			MailboxPropertiesHelper.GetAcceptRejectSendersOrMembers(inputRow, table, store);
		}

		public static void SetRecipientFilterPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			if (DatacenterRegistry.IsForefrontForOffice())
			{
				inputRow["RecipientTypeFilter"] = inputRow["FFORecipientTypeFilter"].ToString();
			}
		}

		public static void GetSuggestionPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			MailboxPropertiesHelper.FilterNoSmtpEmailAddresses(inputRow, dataTable, store);
		}

		public static void SetObjectPreAction(DataRow inputRow, DataTable table, DataObjectStore store)
		{
			MailboxPropertiesHelper.SetMaxSendReceiveSize(inputRow, table, store);
		}

		public static void NewObjectPreAction(DataRow inputRow, DataTable table, DataObjectStore store)
		{
			DataRow dataRow = table.Rows[0];
			bool flag = !(dataRow["IsNotSmtpAddress"] is DBNull) && (bool)dataRow["IsNotSmtpAddress"];
			string text = (dataRow["ExternalEmailAddress"] is DBNull) ? null : ((string)dataRow["ExternalEmailAddress"]);
			List<string> list = new List<string>();
			list.Add("ExternalEmailAddress");
			if (string.IsNullOrEmpty(text))
			{
				text = (string)dataRow["MicrosoftOnlineServicesID"];
			}
			if (flag)
			{
				dataRow["ExternalEmailAddress"] = (string)dataRow["AddressPrefix"] + ":" + text;
			}
			else
			{
				dataRow["ExternalEmailAddress"] = "SMTP:" + text;
			}
			store.SetModifiedColumns(list);
		}

		public static string GetFilterByNameString(object name)
		{
			return string.Format("Name -eq '{0}'", ((string)name).Replace("'", "''"));
		}

		public static void GenerateName(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			ReducedRecipient reducedRecipient = store.GetDataObject("ReducedRecipient") as ReducedRecipient;
			string text = (string)inputRow["MailUserDisplayName"];
			if (reducedRecipient != null)
			{
				string text2 = " " + Guid.NewGuid().ToString("B").ToUpperInvariant();
				if (text.Length > 64)
				{
					text = text.SurrogateSubstring(0, text.Length - text2.Length);
				}
				inputRow["MailUserName"] = text + text2;
			}
			else
			{
				if (text.Length > 64)
				{
					text = text.SurrogateSubstring(0, 64);
				}
				inputRow["MailUserName"] = text;
			}
			store.SetModifiedColumns(new List<string>
			{
				"MailUserName"
			});
		}

		public static readonly string NewObjectWorkflowOutput = Util.IsDataCenter ? "Identity,RecipientTypeDetails,DisplayName,LocRecipientTypeDetails,PrimarySmtpAddress,Alias,City,Company,CountryOrRegion,Department,EmailAddressesTxt,FirstName,LastName,Office,Phone,PostalCode,RecipientType,StateOrProvince,Title,WhenChanged" : "Identity,RecipientTypeDetails,DisplayName,LocRecipientTypeDetails,PrimarySmtpAddress,Alias,City,Company,CountryOrRegion,Department,EmailAddressesTxt,FirstName,LastName,Office,Phone,PostalCode,RecipientType,StateOrProvince,Title,WhenChanged,EmailAddressPolicyEnabled,HiddenFromAddressListsEnabled,Name,OrganizationalUnit,CustomAttribute1,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15";
	}
}
