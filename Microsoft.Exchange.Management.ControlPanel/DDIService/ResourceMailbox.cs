using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class ResourceMailbox
	{
		public static void GetListPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			foreach (object obj in dataTable.Rows)
			{
				DataRow dataRow = (DataRow)obj;
				RecipientTypeDetails recipientTypeDetails = (RecipientTypeDetails)dataRow["RecipientTypeDetails"];
				dataRow["IsRoom"] = (recipientTypeDetails == RecipientTypeDetails.RoomMailbox);
				dataRow["MailboxType"] = MailboxPropertiesHelper.TranslateMailboxTypeForListview(recipientTypeDetails, false, Guid.Empty);
			}
		}

		public static void GetObjectPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			MailboxPropertiesHelper.GetCountryOrRegion(inputRow, dataTable, store);
			MailboxPropertiesHelper.GetEmailAddresses(inputRow, dataTable, store);
			DataRow dataRow = dataTable.Rows[0];
			CalendarConfiguration calendarConfiguration = store.GetDataObject("CalendarConfiguration") as CalendarConfiguration;
			if (calendarConfiguration != null)
			{
				bool flag = calendarConfiguration.AutomateProcessing == CalendarProcessingFlags.AutoAccept;
				bool flag2 = flag && !calendarConfiguration.AllBookInPolicy && calendarConfiguration.AllRequestInPolicy;
				bool flag3 = flag && calendarConfiguration.AllBookInPolicy && !calendarConfiguration.AllRequestInPolicy;
				dataRow["AutomateProcessingManual"] = flag2;
				dataRow["AutomateProcessingAuto"] = flag3;
				dataRow["AutomateProcessingCustomized"] = (!flag2 && !flag3);
				dataRow["AdditionalResponse"] = (calendarConfiguration.AddAdditionalResponse ? calendarConfiguration.AdditionalResponse : string.Empty);
			}
			MailboxPropertiesHelper.UpdateCanSetABP(dataRow, store);
		}

		public static void SetObjectPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			List<string> list = new List<string>();
			bool flag = false;
			bool flag2 = false;
			if (!DBNull.Value.Equals(dataRow["AutomateProcessingAuto"]))
			{
				flag = (bool)dataRow["AutomateProcessingAuto"];
			}
			if (!DBNull.Value.Equals(dataRow["AutomateProcessingManual"]))
			{
				flag2 = (bool)dataRow["AutomateProcessingManual"];
			}
			if (flag && flag2)
			{
				throw new ArgumentException("automateProcessingAuto and automateProcessingManual cannot be true at the same time.");
			}
			if (flag || flag2)
			{
				inputRow["AutomateProcessing"] = (dataRow["AutomateProcessing"] = CalendarProcessingFlags.AutoAccept);
				inputRow["AllBookInPolicy"] = (dataRow["AllBookInPolicy"] = flag);
				inputRow["AllRequestInPolicy"] = (dataRow["AllRequestInPolicy"] = !flag);
				list.Add("AutomateProcessing");
				list.Add("AllBookInPolicy");
				list.Add("AllRequestInPolicy");
			}
			if (!DBNull.Value.Equals(dataRow["AdditionalResponse"]))
			{
				bool flag3 = !string.IsNullOrEmpty((string)dataRow["AdditionalResponse"]);
				inputRow["AddAdditionalResponse"] = (dataRow["AddAdditionalResponse"] = flag3);
				list.Add("AddAdditionalResponse");
			}
			if (list.Count > 0)
			{
				store.SetModifiedColumns(list);
			}
		}

		public static void SetObjectPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store, PowerShellResults[] results)
		{
			if (results[0].ErrorRecords != null && results[0].ErrorRecords.Length > 0)
			{
				string[] array = new string[]
				{
					Strings.NewRoomCreationWarningText
				};
				array = array.Concat(from x in results[0].ErrorRecords
				select x.Message).ToArray<string>();
				if (results[0].Warnings != null)
				{
					array = array.Concat(results[0].Warnings).ToArray<string>();
				}
				results[0].Warnings = array;
				results[0].ErrorRecords = Array<ErrorRecord>.Empty;
			}
		}

		public static void GetSDOPostAction(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			CalendarConfiguration calendarConfiguration = store.GetDataObject("CalendarConfiguration") as CalendarConfiguration;
			if (calendarConfiguration != null)
			{
				dataRow["ResourceDelegates"] = calendarConfiguration.ResourceDelegates.ResolveRecipientsForSDO(3, (RecipientObjectResolverRow x) => Strings.DisplayedResourceDelegates(x.DisplayName, x.PrimarySmtpAddress));
				dataRow["AutomateProcessingAuto"] = (calendarConfiguration.AutomateProcessing == CalendarProcessingFlags.AutoAccept && calendarConfiguration.AllBookInPolicy && !calendarConfiguration.AllRequestInPolicy);
			}
			RecipientTypeDetails recipientTypeDetails = (RecipientTypeDetails)dataRow["RecipientTypeDetails"];
			dataRow["IsRoom"] = (recipientTypeDetails == RecipientTypeDetails.RoomMailbox);
		}

		public static void TargetAllMDBsPostAction(DataRow input, DataTable dataTable, DataObjectStore store)
		{
			store.SetModifiedColumns(new List<string>
			{
				"TargetAllMDBs"
			});
		}

		public static void GetSuggestionPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			MailboxPropertiesHelper.FilterNoSmtpEmailAddresses(inputRow, dataTable, store);
		}

		public static void FilterCloudSendAsPermission(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			MailboxPropertiesHelper.FilterCloudSendAsPermission(inputRow, dataTable, store);
		}

		public static void FilterEntSendAsPermission(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			MailboxPropertiesHelper.FilterEntSendAsPermission(inputRow, dataTable, store);
		}

		public static void FilterFullAccessPermission(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			MailboxPropertiesHelper.FilterFullAccessPermission(inputRow, dataTable, store);
		}

		public static void SetMailboxPermissionPreAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			MailboxPropertiesHelper.SetMailboxPermissionPreAction(inputRow, dataTable, store);
		}

		private const string RecipientTypeDetailsColumnName = "RecipientTypeDetails";

		private const string IsRoomColumnName = "IsRoom";

		private const string AutomateProcessingCustomizedColumnName = "AutomateProcessingCustomized";

		private const string AutomateProcessingManualColumnName = "AutomateProcessingManual";

		private const string AutomateProcessingAutoColumnName = "AutomateProcessingAuto";

		private const string AdditionalResponseColumnName = "AdditionalResponse";

		private const string AutomateProcessingColumnName = "AutomateProcessing";

		private const string AllBookInPolicyColumnName = "AllBookInPolicy";

		private const string AllRequestInPolicyColumnName = "AllRequestInPolicy";

		private const string AddAdditionalResponseColumnName = "AddAdditionalResponse";

		private const string ResourceDelegatesColumnName = "ResourceDelegates";

		private const string CalendarConfigurationObjectName = "CalendarConfiguration";

		private const int DisplayedRecipientsNumberForSDO = 3;

		public static readonly string NewObjectWorkflowOutput = "Identity,RecipientTypeDetails,IsRoom,DisplayNameForList,MailboxType,PrimarySmtpAddress,Alias,City,Company,CountryOrRegion,Database,Department,EmailAddressesTxt,EmailAddressPolicyEnabled,ExchangeVersion,HiddenFromAddressListsEnabled,Name,Office,OrganizationalUnit,Phone,PostalCode,RecipientType,StateOrProvince,WhenChanged,CustomAttribute1,CustomAttribute2,CustomAttribute3,CustomAttribute4,CustomAttribute5,CustomAttribute6,CustomAttribute7,CustomAttribute8,CustomAttribute9,CustomAttribute10,CustomAttribute11,CustomAttribute12,CustomAttribute13,CustomAttribute14,CustomAttribute15";
	}
}
