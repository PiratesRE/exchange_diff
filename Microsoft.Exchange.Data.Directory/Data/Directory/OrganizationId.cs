using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	[ImmutableObject(true)]
	[Serializable]
	public sealed class OrganizationId : IEquatable<OrganizationId>, IOrganizationIdForEventLog
	{
		private OrganizationId()
		{
			this.orgUnit = null;
			this.configUnit = null;
			this.partitionId = null;
		}

		internal OrganizationId(ADObjectId orgUnit, ADObjectId configUnit)
		{
			this.Initialize(orgUnit, configUnit);
		}

		public ADObjectId OrganizationalUnit
		{
			get
			{
				return this.orgUnit;
			}
		}

		public ADObjectId ConfigurationUnit
		{
			get
			{
				return this.configUnit;
			}
		}

		internal PartitionId PartitionId
		{
			get
			{
				if (this.partitionId == null)
				{
					this.partitionId = PartitionId.LocalForest;
				}
				return this.partitionId;
			}
		}

		string IOrganizationIdForEventLog.IdForEventLog
		{
			get
			{
				if (this.Equals(OrganizationId.ForestWideOrgId))
				{
					return string.Empty;
				}
				if (this.ConfigurationUnit != null)
				{
					return this.ConfigurationUnit.DistinguishedName ?? this.ConfigurationUnit.ObjectGuid.ToString();
				}
				if (this.OrganizationalUnit != null)
				{
					return this.OrganizationalUnit.DistinguishedName ?? this.OrganizationalUnit.ObjectGuid.ToString();
				}
				return string.Empty;
			}
		}

		public static bool operator ==(OrganizationId a, OrganizationId b)
		{
			return object.ReferenceEquals(a, b) || (a != null && b != null && a.Equals(b));
		}

		public static bool operator !=(OrganizationId a, OrganizationId b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as OrganizationId);
		}

		public bool Equals(OrganizationId other)
		{
			return !(other == null) && ((this.OrganizationalUnit == null && other.OrganizationalUnit == null) || (this.OrganizationalUnit != null && other.OrganizationalUnit != null && this.OrganizationalUnit.DistinguishedName.Equals(other.OrganizationalUnit.DistinguishedName, StringComparison.OrdinalIgnoreCase)));
		}

		public override int GetHashCode()
		{
			if (this.OrganizationalUnit == null)
			{
				return 0;
			}
			return this.OrganizationalUnit.DistinguishedName.ToLower().GetHashCode();
		}

		public override string ToString()
		{
			if (this.orgUnit != null && this.configUnit != null)
			{
				return string.Format("{0} - {1}", this.orgUnit.ToCanonicalName(), this.configUnit.ToCanonicalName());
			}
			return string.Empty;
		}

		internal static object Getter(IPropertyBag propertyBag)
		{
			OrganizationId organizationId = OrganizationId.ForestWideOrgId;
			ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.OrganizationalUnitRoot];
			ADObjectId adobjectId2 = (ADObjectId)propertyBag[ADObjectSchema.ConfigurationUnit];
			if (adobjectId != null && adobjectId2 != null)
			{
				organizationId = new OrganizationId();
				organizationId.Initialize(adobjectId, adobjectId2);
			}
			else if (adobjectId != null || adobjectId2 != null)
			{
				ADObjectId adobjectId3 = (ADObjectId)propertyBag[ADObjectSchema.Id];
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.ErrorInvalidOrganizationId((adobjectId3 != null) ? adobjectId3.ToDNString() : "<null>", (adobjectId != null) ? adobjectId.ToDNString() : "<null>", (adobjectId2 != null) ? adobjectId2.ToDNString() : "<null>"), ADObjectSchema.OrganizationId, null), null);
			}
			return organizationId;
		}

		internal static void Setter(object value, IPropertyBag propertyBag)
		{
			OrganizationId organizationId = value as OrganizationId;
			propertyBag[ADObjectSchema.OrganizationalUnitRoot] = organizationId.OrganizationalUnit;
			propertyBag[ADObjectSchema.ConfigurationUnit] = organizationId.ConfigurationUnit;
		}

		internal static bool TryCreateFromBytes(byte[] bytes, Encoding encoding, out OrganizationId orgId)
		{
			orgId = null;
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			if (bytes.Length < 6)
			{
				return false;
			}
			byte b = bytes[0];
			if (b <= 0)
			{
				return false;
			}
			int num = 1;
			int num2 = (int)(1 + 5 * b);
			ADObjectId adobjectId = null;
			ADObjectId adobjectId2 = null;
			for (int i = 0; i < (int)b; i++)
			{
				if (bytes.Length < num + 4 + 1)
				{
					return false;
				}
				OrganizationId.ByteRepresentationTag byteRepresentationTag = (OrganizationId.ByteRepresentationTag)bytes[num++];
				int num3 = BitConverter.ToInt32(bytes, num);
				num += 4;
				if (num3 < 0)
				{
					return false;
				}
				if (num3 != 0 && bytes.Length < num2 + num3)
				{
					return false;
				}
				switch (byteRepresentationTag)
				{
				case OrganizationId.ByteRepresentationTag.ForestWideOrgIdTag:
					if (num3 != 0)
					{
						return false;
					}
					orgId = OrganizationId.ForestWideOrgId;
					return true;
				case OrganizationId.ByteRepresentationTag.OrgUnitTag:
					if (adobjectId != null || !ADObjectId.TryCreateFromBytes(bytes, num2, num3, encoding, out adobjectId))
					{
						return false;
					}
					break;
				case OrganizationId.ByteRepresentationTag.ConfigUnitTag:
					if (adobjectId2 != null || !ADObjectId.TryCreateFromBytes(bytes, num2, num3, encoding, out adobjectId2))
					{
						return false;
					}
					if (!ADSession.IsTenantConfigObjectInCorrectNC(adobjectId2))
					{
						return false;
					}
					break;
				}
				num2 += num3;
			}
			if (adobjectId == null || adobjectId2 == null)
			{
				return false;
			}
			orgId = new OrganizationId();
			orgId.Initialize(adobjectId, adobjectId2);
			return true;
		}

		internal byte[] GetBytes(Encoding encoding)
		{
			byte[] array;
			if (this.orgUnit == null)
			{
				array = new byte[6];
				array[0] = 1;
				array[1] = 0;
				ExBitConverter.Write(0, array, 2);
			}
			else
			{
				if (this.configUnit.ObjectGuid == Guid.Empty || this.orgUnit.ObjectGuid == Guid.Empty)
				{
					throw new InvalidOperationException("OrganizationId is not fully populated and cannot be serialized");
				}
				int byteCount = this.orgUnit.GetByteCount(encoding);
				int byteCount2 = this.configUnit.GetByteCount(encoding);
				array = new byte[byteCount + byteCount2 + 8 + 3];
				int num = 0;
				array[num++] = 2;
				array[num++] = 1;
				num += ExBitConverter.Write(byteCount, array, num);
				array[num++] = 2;
				num += ExBitConverter.Write(byteCount2, array, num);
				this.orgUnit.GetBytes(encoding, array, num);
				num += byteCount;
				this.configUnit.GetBytes(encoding, array, num);
			}
			return array;
		}

		internal static OrganizationId FromExternalDirectoryOrganizationId(Guid externalDirectoryOrganizationId)
		{
			string tenantContainerCN;
			PartitionId partitionIdByExternalDirectoryOrganizationId = ADAccountPartitionLocator.GetPartitionIdByExternalDirectoryOrganizationId(externalDirectoryOrganizationId, out tenantContainerCN);
			return OrganizationId.FromPartition<Guid>(externalDirectoryOrganizationId, externalDirectoryOrganizationId, tenantContainerCN, partitionIdByExternalDirectoryOrganizationId, (ITenantConfigurationSession session) => session.GetOrganizationIdFromExternalDirectoryOrgId(externalDirectoryOrganizationId), () => new CannotResolveExternalDirectoryOrganizationIdException(DirectoryStrings.CannotFindTenantCUByExternalDirectoryId(externalDirectoryOrganizationId.ToString())));
		}

		internal static OrganizationId FromTenantForestAndCN(string exoAccountForest, string exoTenantContainer)
		{
			PartitionId partitionId = new PartitionId(exoAccountForest);
			ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
			ITenantConfigurationSession session = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 505, "FromTenantForestAndCN", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\OrganizationId.cs");
			return OrganizationId.FromTenantContainerCN(exoTenantContainer, session);
		}

		internal static OrganizationId FromAcceptedDomain(string acceptedDomain)
		{
			string tenantContainerCN;
			Guid guid;
			PartitionId partitionIdByAcceptedDomainName = ADAccountPartitionLocator.GetPartitionIdByAcceptedDomainName(acceptedDomain, out tenantContainerCN, out guid);
			return OrganizationId.FromPartition<string>(acceptedDomain, guid, tenantContainerCN, partitionIdByAcceptedDomainName, (ITenantConfigurationSession session) => session.GetOrganizationIdFromOrgNameOrAcceptedDomain(acceptedDomain), () => new CannotResolveTenantNameException(DirectoryStrings.CannotFindTenantCUByAcceptedDomain(acceptedDomain)));
		}

		internal static OrganizationId FromMSAUserNetID(string msaUserNetID)
		{
			Guid externalDirectoryOrganizationId;
			string tenantContainerCN;
			PartitionId partitionIdByMSAUserNetID = ADAccountPartitionLocator.GetPartitionIdByMSAUserNetID(msaUserNetID, out tenantContainerCN, out externalDirectoryOrganizationId);
			return OrganizationId.FromPartition<Guid>(externalDirectoryOrganizationId, externalDirectoryOrganizationId, tenantContainerCN, partitionIdByMSAUserNetID, (ITenantConfigurationSession session) => session.GetOrganizationIdFromExternalDirectoryOrgId(externalDirectoryOrganizationId), () => new CannotResolveExternalDirectoryOrganizationIdException(DirectoryStrings.CannotFindTenantCUByExternalDirectoryId(externalDirectoryOrganizationId.ToString())));
		}

		private static OrganizationId FromPartition<T>(T lookupKey, Guid externalDirectoryOrganizationId, string tenantContainerCN, PartitionId partitionId, Func<ITenantConfigurationSession, OrganizationId> GetOrgFromAD, Func<Exception> CreateNotFoundException)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 561, "FromPartition", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\OrganizationId.cs");
			if (!string.IsNullOrEmpty(tenantContainerCN))
			{
				OrganizationId organizationId = OrganizationId.FromTenantContainerCN(tenantContainerCN, tenantConfigurationSession);
				organizationId.externalDirectoryOrganizationId = new Guid?(externalDirectoryOrganizationId);
				return organizationId;
			}
			OrganizationId organizationId2 = GetOrgFromAD(tenantConfigurationSession);
			if (organizationId2 != null)
			{
				ExTraceGlobals.GLSTracer.TraceDebug<OrganizationId, T>(0L, "[OrganizationIdConvertor.FromExternalDirectoryOrganizationId]: Found OrganizationId {0} for {1}", organizationId2, lookupKey);
				Guid value = Guid.Empty;
				if (externalDirectoryOrganizationId == Guid.Empty)
				{
					ADRawEntry adrawEntry = tenantConfigurationSession.ReadADRawEntry(organizationId2.ConfigurationUnit, new PropertyDefinition[]
					{
						ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationId
					});
					if (adrawEntry != null)
					{
						Guid.TryParse(adrawEntry[ExchangeConfigurationUnitSchema.ExternalDirectoryOrganizationId].ToString(), out value);
					}
				}
				else
				{
					value = externalDirectoryOrganizationId;
				}
				organizationId2.externalDirectoryOrganizationId = new Guid?(value);
				return organizationId2;
			}
			throw CreateNotFoundException();
		}

		private static OrganizationId FromTenantContainerCN(string tenantContainerCN, ITenantConfigurationSession session)
		{
			ADObjectId exchangeConfigurationUnitIdByName = session.GetExchangeConfigurationUnitIdByName(tenantContainerCN);
			ADObjectId childId = session.GetHostedOrganizationsRoot().GetChildId("OU", tenantContainerCN);
			OrganizationId organizationId = new OrganizationId();
			organizationId.Initialize(childId, exchangeConfigurationUnitIdByName);
			return organizationId;
		}

		internal void EnsureFullyPopulated()
		{
			if (this.configUnit.ObjectGuid == Guid.Empty || this.orgUnit.ObjectGuid == Guid.Empty)
			{
				this.PopulateGuidsAndExternalDirectoryOrganizationId();
			}
		}

		private void PopulateGuidsAndExternalDirectoryOrganizationId()
		{
			ExchangeConfigurationUnit exchangeConfigurationUnit = this.ReadCU();
			this.configUnit = exchangeConfigurationUnit.OrganizationId.configUnit;
			this.orgUnit = exchangeConfigurationUnit.OrganizationId.OrganizationalUnit;
			Guid value;
			if (Guid.TryParse(exchangeConfigurationUnit.ExternalDirectoryOrganizationId, out value))
			{
				this.externalDirectoryOrganizationId = new Guid?(value);
			}
		}

		internal string ToExternalDirectoryOrganizationId()
		{
			if (!Datacenter.IsMultiTenancyEnabled() || this == OrganizationId.ForestWideOrgId)
			{
				ExTraceGlobals.GLSTracer.TraceDebug<string>(0L, "[OrganizationIdConvertor.ToExternalDirectoryOrganizationId]: Returning string.Empty because {0}.", Datacenter.IsMultiTenancyEnabled() ? "orgId is ForestWideOrgId" : "multitenancy is not enabled");
				return string.Empty;
			}
			if (this.externalDirectoryOrganizationId == null)
			{
				this.PopulateGuidsAndExternalDirectoryOrganizationId();
			}
			return this.externalDirectoryOrganizationId.ToString();
		}

		private ExchangeConfigurationUnit ReadCU()
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 671, "ReadCU", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\OrganizationId.cs");
			ExchangeConfigurationUnit exchangeConfigurationUnit = tenantOrTopologyConfigurationSession.Read<ExchangeConfigurationUnit>(this.ConfigurationUnit);
			if (exchangeConfigurationUnit == null)
			{
				throw new CannotResolveTenantNameException(DirectoryStrings.TenantOrgContainerNotFoundException(this.configUnit.DistinguishedName));
			}
			return exchangeConfigurationUnit;
		}

		[Conditional("DEBUG")]
		private static void Dbg_CheckCallStack()
		{
			string stackTrace = Environment.StackTrace;
			if (!stackTrace.Contains("OrganizationIdGetter") && !stackTrace.Contains("FromTenantContainerCN"))
			{
				throw new NotSupportedException("OrganizationId's constructor can only be called from OrganizationIdGetter");
			}
		}

		private void Initialize(ADObjectId orgUnit, ADObjectId configUnit)
		{
			if (orgUnit == null)
			{
				throw new ArgumentNullException("orgUnit");
			}
			if (configUnit == null)
			{
				throw new ArgumentNullException("configUnit");
			}
			this.orgUnit = orgUnit;
			this.configUnit = configUnit;
			this.partitionId = ((orgUnit.DomainId != null && !PartitionId.IsLocalForestPartition(orgUnit.PartitionFQDN)) ? orgUnit.GetPartitionId() : PartitionId.LocalForest);
		}

		private ADObjectId orgUnit;

		private ADObjectId configUnit;

		private PartitionId partitionId;

		private Guid? externalDirectoryOrganizationId;

		internal static OrganizationId ForestWideOrgId = new OrganizationId();

		private enum ByteRepresentationTag : byte
		{
			ForestWideOrgIdTag,
			OrgUnitTag,
			ConfigUnitTag
		}
	}
}
