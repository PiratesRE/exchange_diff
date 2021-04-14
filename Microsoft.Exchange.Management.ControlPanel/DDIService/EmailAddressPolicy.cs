using System;
using System.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	public class EmailAddressPolicy
	{
		public static void GetObjectPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			string value = string.Empty;
			dataRow["IsPrecannedRecipientFilterType"] = (dataRow["IsCustomRecipientFilterType"] = (dataRow["IsOtherRecipientFilterType"] = false));
			if (dataRow["RecipientFilterType"] != DBNull.Value)
			{
				switch ((WellKnownRecipientFilterType)dataRow["RecipientFilterType"])
				{
				case WellKnownRecipientFilterType.Unknown:
				case WellKnownRecipientFilterType.Legacy:
					dataRow["IsOtherRecipientFilterType"] = true;
					value = Strings.CustomFilterDescription((string)dataRow["LdapRecipientFilter"]);
					break;
				case WellKnownRecipientFilterType.Custom:
					dataRow["IsCustomRecipientFilterType"] = true;
					value = Strings.CustomFilterDescription((string)dataRow["RecipientFilter"]);
					break;
				case WellKnownRecipientFilterType.Precanned:
					dataRow["IsPrecannedRecipientFilterType"] = true;
					value = LocalizedDescriptionAttribute.FromEnum(typeof(WellKnownRecipientType), dataRow["IncludedRecipients"]);
					break;
				default:
					throw new NotSupportedException("Unkown WellKnownRecipientFilterType: " + dataRow["RecipientFilterType"].ToStringWithNull());
				}
				dataRow["IncludeRecipientDescription"] = value;
			}
		}

		public static void NewObjectPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store, PowerShellResults[] results)
		{
			DDIUtil.InsertWarningIfSucceded(results, Strings.NewEAPWarning);
		}

		public static void SetObjectPostAction(DataRow inputRow, DataTable dataTable, DataObjectStore store, PowerShellResults[] results)
		{
			DDIUtil.InsertWarningIfSucceded(results, Strings.EditEAPWarning);
		}
	}
}
