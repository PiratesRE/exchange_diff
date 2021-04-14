using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal class OrganizationSettingsData : IOrganizationSettingsData
	{
		public OrganizationSettingsData(ActiveSyncOrganizationSettings organizationSettings, IConfigurationSession scopedSession)
		{
			if (scopedSession == null)
			{
				throw new ArgumentNullException("scopedSession");
			}
			if (organizationSettings == null)
			{
				throw new ArgumentNullException("organizationSettings");
			}
			this.Identity = organizationSettings.OriginalId;
			this.defaultAccessLevel = organizationSettings.DefaultAccessLevel;
			this.userMailInsert = organizationSettings.UserMailInsert;
			this.allowAccessForUnSupportedPlatform = organizationSettings.AllowAccessForUnSupportedPlatform;
			this.adminMailRecipients = organizationSettings.AdminMailRecipients;
			this.otaNotificationMailInsert = organizationSettings.OtaNotificationMailInsert;
			this.deviceFiltering = null;
			this.IsIntuneManaged = organizationSettings.IsIntuneManaged;
			if (organizationSettings.DeviceFiltering != null && organizationSettings.DeviceFiltering.DeviceFilters != null)
			{
				this.deviceFiltering = organizationSettings.DeviceFiltering.DeviceFilters.ToDictionary((ActiveSyncDeviceFilter deviceFilter) => deviceFilter.Name);
			}
			ADPagedReader<ActiveSyncDeviceAccessRule> adpagedReader = scopedSession.FindPaged<ActiveSyncDeviceAccessRule>(organizationSettings.Id, QueryScope.OneLevel, null, null, 0);
			foreach (ActiveSyncDeviceAccessRule deviceAccessRule in adpagedReader)
			{
				((IOrganizationSettingsData)this).AddOrUpdateDeviceAccessRule(deviceAccessRule);
			}
			this.organizationId = organizationSettings.OrganizationId;
		}

		public ADObjectId Identity { get; private set; }

		DeviceAccessLevel IOrganizationSettingsData.DefaultAccessLevel
		{
			get
			{
				return this.defaultAccessLevel;
			}
		}

		string IOrganizationSettingsData.UserMailInsert
		{
			get
			{
				return this.userMailInsert;
			}
		}

		bool IOrganizationSettingsData.AllowAccessForUnSupportedPlatform
		{
			get
			{
				return this.allowAccessForUnSupportedPlatform;
			}
		}

		public bool IsIntuneManaged { get; private set; }

		IList<SmtpAddress> IOrganizationSettingsData.AdminMailRecipients
		{
			get
			{
				return this.adminMailRecipients;
			}
		}

		string IOrganizationSettingsData.OtaNotificationMailInsert
		{
			get
			{
				return this.otaNotificationMailInsert;
			}
		}

		Dictionary<string, ActiveSyncDeviceFilter> IOrganizationSettingsData.DeviceFiltering
		{
			get
			{
				return this.deviceFiltering;
			}
		}

		bool IOrganizationSettingsData.IsRulesListEmpty
		{
			get
			{
				foreach (List<DeviceAccessRuleData> list in this.deviceAccessRules)
				{
					if (list != null && list.Count > 0)
					{
						return false;
					}
				}
				return true;
			}
		}

		MicrosoftExchangeRecipient IOrganizationSettingsData.GetExchangeRecipient()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.organizationId), 220, "GetExchangeRecipient", "f:\\15.00.1497\\sources\\dev\\AirSync\\src\\AirSync\\OrganizationSettingsData.cs");
			return tenantOrTopologyConfigurationSession.FindMicrosoftExchangeRecipient();
		}

		DeviceAccessRuleData IOrganizationSettingsData.EvaluateDevice(DeviceAccessCharacteristic characteristic, string queryString)
		{
			List<DeviceAccessRuleData> list = this.deviceAccessRules[(int)characteristic];
			if (list == null)
			{
				return null;
			}
			foreach (DeviceAccessRuleData deviceAccessRuleData in list)
			{
				if (string.Equals(deviceAccessRuleData.QueryString, queryString, StringComparison.OrdinalIgnoreCase))
				{
					return deviceAccessRuleData;
				}
			}
			return null;
		}

		void IOrganizationSettingsData.AddOrUpdateDeviceAccessRule(ActiveSyncDeviceAccessRule deviceAccessRule)
		{
			if (this.deviceAccessRules[(int)deviceAccessRule.Characteristic] == null)
			{
				this.deviceAccessRules[(int)deviceAccessRule.Characteristic] = new List<DeviceAccessRuleData>(10);
			}
			List<DeviceAccessRuleData> list = this.deviceAccessRules[(int)deviceAccessRule.Characteristic];
			DeviceAccessRuleData deviceAccessRuleData = new DeviceAccessRuleData(deviceAccessRule);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].Identity.Equals(deviceAccessRuleData.Identity))
				{
					AirSyncDiagnostics.TraceInfo<DeviceAccessRuleData, ADObjectId>(ExTraceGlobals.RequestsTracer, this, "Updating rule {0} for Org {1} cache.", deviceAccessRuleData, this.Identity);
					list[i] = deviceAccessRuleData;
					return;
				}
			}
			AirSyncDiagnostics.TraceInfo<DeviceAccessRuleData, ADObjectId>(ExTraceGlobals.RequestsTracer, this, "Adding new rule {0} for Org {1} cache.", deviceAccessRuleData, this.Identity);
			list.Add(deviceAccessRuleData);
		}

		void IOrganizationSettingsData.RemoveDeviceAccessRule(string distinguishedName)
		{
			for (int i = 0; i < this.deviceAccessRules.Length; i++)
			{
				List<DeviceAccessRuleData> list = this.deviceAccessRules[i];
				if (list != null && list.Count > 0)
				{
					for (int j = 0; j < list.Count; j++)
					{
						if (list[j].Identity.Equals(distinguishedName))
						{
							AirSyncDiagnostics.TraceInfo<ADObjectId, ADObjectId>(ExTraceGlobals.RequestsTracer, this, "Removing rule {0} from Org {1} cache.", list[j].Identity, this.Identity);
							list.RemoveAt(j);
							return;
						}
					}
				}
			}
			AirSyncDiagnostics.TraceError<ADObjectId, string>(ExTraceGlobals.RequestsTracer, this, "Trying to remove a rule not in this Organization. OrganizationId: {0}, rule: {1}", this.Identity, distinguishedName);
		}

		private static readonly int numberOfDeviceAccessCharacteristics = Enum.GetValues(typeof(DeviceAccessCharacteristic)).Length;

		private List<DeviceAccessRuleData>[] deviceAccessRules = new List<DeviceAccessRuleData>[OrganizationSettingsData.numberOfDeviceAccessCharacteristics];

		private OrganizationId organizationId;

		private readonly DeviceAccessLevel defaultAccessLevel;

		private readonly string userMailInsert;

		private readonly bool allowAccessForUnSupportedPlatform;

		private readonly IList<SmtpAddress> adminMailRecipients;

		private readonly string otaNotificationMailInsert;

		private readonly Dictionary<string, ActiveSyncDeviceFilter> deviceFiltering;
	}
}
