using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class TenantPartitionHint
	{
		private bool IsRootOrganization
		{
			get
			{
				return this.externalDirectoryOrganizationId == TenantPartitionHint.ExternalDirectoryOrganizationIdForRootOrg;
			}
		}

		private TenantPartitionHint(PartitionId partitionId, string tenantName)
		{
			if (partitionId == null)
			{
				throw new ArgumentNullException("partitionId");
			}
			if (tenantName == null)
			{
				throw new ArgumentNullException("tenantName");
			}
			if (tenantName.Length > 64)
			{
				throw new TenantNameTooLongException(DirectoryStrings.TenantNameTooLong(tenantName));
			}
			this.partitionId = partitionId;
			this.tenantName = tenantName;
			if (tenantName == TenantPartitionHint.TenantNameForRootOrg)
			{
				this.externalDirectoryOrganizationId = new Guid?(TenantPartitionHint.ExternalDirectoryOrganizationIdForRootOrg);
			}
		}

		private TenantPartitionHint(Guid externalDirectoryOrganizationId)
		{
			this.externalDirectoryOrganizationId = new Guid?(externalDirectoryOrganizationId);
			if (externalDirectoryOrganizationId == TenantPartitionHint.ExternalDirectoryOrganizationIdForRootOrg)
			{
				this.tenantName = TenantPartitionHint.TenantNameForRootOrg;
			}
		}

		public static TenantPartitionHint FromOrganizationId(OrganizationId orgId)
		{
			if (orgId == null)
			{
				throw new ArgumentNullException("orgId");
			}
			if (orgId.Equals(OrganizationId.ForestWideOrgId))
			{
				return new TenantPartitionHint(orgId.PartitionId, TenantPartitionHint.TenantNameForRootOrg);
			}
			if (orgId.PartitionId == null || orgId.OrganizationalUnit == null)
			{
				throw new ArgumentNullException("PartitionId");
			}
			return new TenantPartitionHint(orgId.PartitionId, orgId.OrganizationalUnit.Name);
		}

		public static TenantPartitionHint FromPersistablePartitionHint(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (buffer.Length < 16)
			{
				throw new ArgumentException("buffer");
			}
			if (buffer.Length > 16)
			{
				byte[] array = new byte[16];
				Array.Copy(buffer, array, 16);
				buffer = array;
			}
			Guid guid = new Guid(buffer);
			return new TenantPartitionHint(guid);
		}

		public byte[] GetPersistablePartitionHint()
		{
			byte[] result;
			if (this.IsRootOrganization)
			{
				result = TenantPartitionHint.PersistableHintForRootOrg;
			}
			else
			{
				result = this.GetExternalDirectoryOrganizationId().ToByteArray();
			}
			return result;
		}

		public Guid GetExternalDirectoryOrganizationId()
		{
			Guid result;
			if (this.externalDirectoryOrganizationId != null)
			{
				result = this.externalDirectoryOrganizationId.Value;
			}
			else
			{
				result = ADAccountPartitionLocator.GetExternalDirectoryOrganizationIdByTenantName(this.tenantName, this.partitionId);
			}
			return result;
		}

		public OrganizationId GetOrganizationId()
		{
			if (this.IsRootOrganization)
			{
				return OrganizationId.ForestWideOrgId;
			}
			return OrganizationId.FromExternalDirectoryOrganizationId(this.GetExternalDirectoryOrganizationId());
		}

		public bool IsConsumer()
		{
			return (this.tenantName != null && TemplateTenantConfiguration.IsTemplateTenantName(this.tenantName)) || (this.externalDirectoryOrganizationId != null && this.GetExternalDirectoryOrganizationId() == TemplateTenantConfiguration.TemplateTenantExternalDirectoryOrganizationIdGuid);
		}

		internal static byte[] Serialize(TenantPartitionHint hint)
		{
			if (hint == null)
			{
				throw new ArgumentNullException("hint");
			}
			if (hint.tenantName != null && hint.tenantName.Length > 64)
			{
				throw new ArgumentException("tenantName is too long: " + hint.tenantName);
			}
			if (hint.partitionId != null && hint.partitionId.ForestFQDN != null && hint.partitionId.ForestFQDN.Length > 255)
			{
				throw new ArgumentException("partitionId.ForestFQDN is too long: " + hint.partitionId.ForestFQDN);
			}
			int num = TenantPartitionHint.Serialize(null, hint);
			byte[] array = new byte[num];
			TenantPartitionHint.Serialize(array, hint);
			return array;
		}

		private static int Serialize(byte[] buffer, TenantPartitionHint hint)
		{
			int num = 0;
			num += SerializedValue.SerializeInt32(1, buffer, num);
			TenantPartitionHint.SerializedTenantHintFlags serializedTenantHintFlags = TenantPartitionHint.SerializedTenantHintFlags.None;
			if (hint.externalDirectoryOrganizationId != null)
			{
				serializedTenantHintFlags |= TenantPartitionHint.SerializedTenantHintFlags.ExternalDirectoryOrganizationIdPresent;
			}
			num += SerializedValue.SerializeInt32((int)serializedTenantHintFlags, buffer, num);
			if (hint.externalDirectoryOrganizationId != null)
			{
				num += SerializedValue.SerializeGuid(hint.externalDirectoryOrganizationId.Value, buffer, num);
			}
			num += SerializedValue.SerializeString(hint.tenantName, buffer, num);
			string value = null;
			if (hint.partitionId != null && hint.partitionId.ForestFQDN != null)
			{
				value = hint.partitionId.ForestFQDN;
			}
			return num + SerializedValue.SerializeString(value, buffer, num);
		}

		internal static TenantPartitionHint Deserialize(byte[] buffer)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			int num = 0;
			int num2 = SerializedValue.ParseInt32(buffer, ref num);
			if (num2 < 1)
			{
				throw new InvalidSerializedFormatException("Invalid version for the serialized blob: " + num2.ToString());
			}
			TenantPartitionHint.SerializedTenantHintFlags serializedTenantHintFlags = (TenantPartitionHint.SerializedTenantHintFlags)SerializedValue.ParseInt32(buffer, ref num);
			Guid? guid = null;
			if ((serializedTenantHintFlags & TenantPartitionHint.SerializedTenantHintFlags.ExternalDirectoryOrganizationIdPresent) == TenantPartitionHint.SerializedTenantHintFlags.ExternalDirectoryOrganizationIdPresent)
			{
				guid = new Guid?(SerializedValue.ParseGuid(buffer, ref num));
			}
			string text = SerializedValue.ParseString(buffer, ref num);
			string text2 = SerializedValue.ParseString(buffer, ref num);
			if (text != null && text2 != null)
			{
				PartitionId partitionId = new PartitionId(text2);
				return new TenantPartitionHint(partitionId, text);
			}
			if (guid != null)
			{
				return new TenantPartitionHint(guid.Value);
			}
			return new TenantPartitionHint(TenantPartitionHint.ExternalDirectoryOrganizationIdForRootOrg);
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}:{2}", (this.partitionId != null) ? this.partitionId.ToString() : "<null>", (this.tenantName == TenantPartitionHint.TenantNameForRootOrg) ? "<RootOrg>" : this.tenantName, (this.externalDirectoryOrganizationId != null) ? this.externalDirectoryOrganizationId.Value.ToString() : "<null>");
		}

		internal ADSessionSettings GetTenantScopedADSessionSettingsServiceOnly()
		{
			if (this.externalDirectoryOrganizationId != null)
			{
				ExTraceGlobals.GetConnectionTracer.TraceDebug<string>(0L, "FromTenantPartitionHint(): Partition hint '{0}' has externalDirectoryOrganizationId initialized, no need to lookup", this.ToString());
				if (this.externalDirectoryOrganizationId == TenantPartitionHint.ExternalDirectoryOrganizationIdForRootOrg)
				{
					return ADSessionSettings.FromRootOrgScopeSet();
				}
				return ADSessionSettings.FromExternalDirectoryOrganizationId(this.externalDirectoryOrganizationId.Value);
			}
			else
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(this.partitionId);
				ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 442, "GetTenantScopedADSessionSettingsServiceOnly", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\TenantPartitionHint.cs");
				ExTraceGlobals.GetConnectionTracer.TraceDebug<TenantPartitionHint>(0L, "FromTenantPartitionHint(): Partition hint '{0}' has externalDirectoryOrganizationId not initialized, need to read this property from AD, looking up", this);
				ExchangeConfigurationUnit exchangeConfigurationUnitByName = tenantConfigurationSession.GetExchangeConfigurationUnitByName(this.tenantName);
				if (exchangeConfigurationUnitByName != null)
				{
					ExTraceGlobals.GetConnectionTracer.TraceDebug<TenantPartitionHint>(0L, "FromTenantPartitionHint(): Partition hint '{0}' has PartitionId and tenantName initialized, reading this OrganizationId property from CU", this);
					return ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(exchangeConfigurationUnitByName.OrganizationId);
				}
				ExTraceGlobals.GetConnectionTracer.TraceDebug<TenantPartitionHint>(0L, "FromTenantPartitionHint(): Partition hint '{0}' has partitionId and tenantName initialized, but failed to find this tenant's CU", this);
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_CannotResolveExternalDirectoryOrganizationId, this.tenantName, new object[]
				{
					DirectoryStrings.CannotResolveTenantName(this.tenantName),
					Environment.StackTrace
				});
				throw new CannotResolveExternalDirectoryOrganizationIdException(DirectoryStrings.CannotResolveTenantName(this.tenantName));
			}
		}

		private const int GuidSize = 16;

		private const int ExternalDirectoryOrganizationSize = 16;

		private const int MinSupportedSerializationBlobVersion = 1;

		internal static readonly string TenantNameForRootOrg = string.Empty;

		internal static readonly Guid ExternalDirectoryOrganizationIdForRootOrg = Guid.Empty;

		internal static readonly byte[] PersistableHintForRootOrg = TenantPartitionHint.ExternalDirectoryOrganizationIdForRootOrg.ToByteArray();

		private readonly PartitionId partitionId;

		private readonly Guid? externalDirectoryOrganizationId;

		private readonly string tenantName;

		[Flags]
		private enum SerializedTenantHintFlags
		{
			None = 0,
			ExternalDirectoryOrganizationIdPresent = 1
		}
	}
}
