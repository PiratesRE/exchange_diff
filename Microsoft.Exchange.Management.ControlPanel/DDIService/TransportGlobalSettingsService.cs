using System;
using System.Data;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.DDIService
{
	public static class TransportGlobalSettingsService
	{
		public static void OnPostGlobalSettings(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			dataRow["MaxRecipientEnvelopeLimit"] = DDIUtil.ConvertUnlimitedToString<int>(dataRow["MaxRecipientEnvelopeLimit"], (int t) => t.ToString());
			dataRow["MaxReceiveSize"] = DDIUtil.ConvertUnlimitedToString<ByteQuantifiedSize>(dataRow["MaxReceiveSize"], (ByteQuantifiedSize s) => s.ToMB(3));
			dataRow["MaxSendSize"] = DDIUtil.ConvertUnlimitedToString<ByteQuantifiedSize>(dataRow["MaxSendSize"], (ByteQuantifiedSize s) => s.ToMB(3));
			if (!DDIHelper.IsEmptyValue(dataRow["SafetyNetHoldTime"]))
			{
				dataRow["SafetyNetHoldTime"] = ((EnhancedTimeSpan)dataRow["SafetyNetHoldTime"]).ToString(TimeUnit.Day, 2);
			}
			if (!DDIHelper.IsEmptyValue(dataRow["MaxDumpsterTime"]))
			{
				dataRow["MaxDumpsterTime"] = ((EnhancedTimeSpan)dataRow["MaxDumpsterTime"]).ToString(TimeUnit.Day, 2);
			}
		}

		public static void OnPreGlobalSettings(DataRow inputRow, DataTable dataTable, DataObjectStore store)
		{
			DataRow dataRow = dataTable.Rows[0];
			if (!DBNull.Value.Equals(dataRow["MaxReceiveSize"]))
			{
				dataRow["MaxReceiveSize"] = DDIUtil.ConvertStringToUnlimited((string)dataRow["MaxReceiveSize"], (string t) => t.FromMB());
			}
			if (!DBNull.Value.Equals(dataRow["MaxSendSize"]))
			{
				dataRow["MaxSendSize"] = DDIUtil.ConvertStringToUnlimited((string)dataRow["MaxSendSize"], (string t) => t.FromMB());
			}
			if (!DBNull.Value.Equals(dataRow["SafetyNetHoldTime"]))
			{
				dataRow["SafetyNetHoldTime"] = ((string)dataRow["SafetyNetHoldTime"]).FromTimeSpan(TimeUnit.Day).ToString();
			}
			if (!DBNull.Value.Equals(dataRow["MaxDumpsterTime"]))
			{
				dataRow["MaxDumpsterTime"] = ((string)dataRow["MaxDumpsterTime"]).FromTimeSpan(TimeUnit.Day).ToString();
			}
		}

		private const string SafetyNetHoldTimeName = "SafetyNetHoldTime";

		private const string MaxDumpsterTimeName = "MaxDumpsterTime";

		private const string MaxReceiveSizeName = "MaxReceiveSize";

		private const string MaxRecipientEnvelopeLimitName = "MaxRecipientEnvelopeLimit";

		private const string MaxSendSizeName = "MaxSendSize";

		private const string ExternalPostmasterAddressName = "ExternalPostmasterAddress";
	}
}
