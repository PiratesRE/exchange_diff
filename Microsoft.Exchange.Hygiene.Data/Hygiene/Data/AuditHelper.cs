using System;
using System.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal static class AuditHelper
	{
		public static void ApplyAuditProperties(IPropertyBag propertyBag, Guid auditId = default(Guid), string userId = null)
		{
			if (propertyBag == null)
			{
				throw new ArgumentNullException("propertyBag");
			}
			if (auditId == Guid.Empty)
			{
				if (propertyBag[AuditHelper.AuditIdProp] != null)
				{
					auditId = (Guid)propertyBag[AuditHelper.AuditIdProp];
				}
				if (auditId == Guid.Empty)
				{
					auditId = Guid.NewGuid();
				}
			}
			if (string.IsNullOrEmpty(userId))
			{
				userId = (string)propertyBag[AuditHelper.UserIdProp];
				if (string.IsNullOrEmpty(userId))
				{
					using (Process currentProcess = Process.GetCurrentProcess())
					{
						userId = string.Format("Unknown-{0}-{1}", currentProcess.ProcessName, Environment.MachineName);
					}
				}
			}
			propertyBag[AuditHelper.AuditIdProp] = auditId;
			propertyBag[AuditHelper.UserIdProp] = userId;
		}

		public static readonly HygienePropertyDefinition UserIdProp = new HygienePropertyDefinition("AuditUserId", typeof(string), string.Empty, ExchangeObjectVersion.Exchange2003, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition AuditIdProp = new HygienePropertyDefinition("AuditId", typeof(Guid), Guid.Empty, ExchangeObjectVersion.Exchange2003, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly string UnknownUserId = "Unknown";

		public static readonly string ForwardSyncUserId = "ForwardSync";

		public static readonly string Migrate1415SyncUserId = "Migrate1415";

		public static readonly string SecurityUserId = "Security";
	}
}
