using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	internal class ExchangeConfigurationUnitHandler : ICustomObjectHandler
	{
		public static ExchangeConfigurationUnitHandler Instance
		{
			get
			{
				return ExchangeConfigurationUnitHandler.instance.Value;
			}
		}

		public bool HandleObject(TenantRelocationSyncObject obj, ModifyRequest modRequest, UpdateData mData, TenantRelocationSyncData syncData, ITopologyConfigurationSession targetPartitionSession)
		{
			bool flag = false;
			return flag | this.HandleSupportedSharedConfigurationsProperty(obj, modRequest, mData, syncData, targetPartitionSession);
		}

		private bool HandleSupportedSharedConfigurationsProperty(TenantRelocationSyncObject obj, ModifyRequest modRequest, UpdateData mData, TenantRelocationSyncData syncData, ITopologyConfigurationSession targetPartitionSession)
		{
			if (!TenantRelocationConfigImpl.GetConfig<bool>("TranslateSupportedSharedConfigurations"))
			{
				return false;
			}
			DirectoryAttributeModification directoryAttributeModification = null;
			MultiValuedProperty<ADObjectId> multiValuedProperty = (MultiValuedProperty<ADObjectId>)obj[OrganizationSchema.SupportedSharedConfigurations];
			MultiValuedProperty<ADObjectId> multiValuedProperty2;
			if (multiValuedProperty != null && multiValuedProperty.Count > 0)
			{
				OrganizationId organizationId = syncData.Source.OrganizationId;
				Exception ex;
				OrganizationId organizationId2 = SharedConfiguration.FindMostRecentSharedConfigurationInPartition(organizationId, syncData.Target.PartitionId, out ex);
				if (ex != null)
				{
					throw ex;
				}
				directoryAttributeModification = this.GetDirectoryAttributeModification(DirectoryAttributeOperation.Add);
				directoryAttributeModification.Add(organizationId2.ConfigurationUnit.DistinguishedName);
				modRequest.Modifications.Add(directoryAttributeModification);
			}
			else if (this.TryGetSupportedSharedConfigurations(targetPartitionSession, modRequest.DistinguishedName, syncData, out multiValuedProperty2) && multiValuedProperty2 != null && multiValuedProperty2.Count > 0)
			{
				directoryAttributeModification = this.GetDirectoryAttributeModification(DirectoryAttributeOperation.Delete);
				foreach (ADObjectId adobjectId in multiValuedProperty2)
				{
					directoryAttributeModification.Add(adobjectId.DistinguishedName);
				}
				modRequest.Modifications.Add(directoryAttributeModification);
			}
			if (directoryAttributeModification != null)
			{
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<string, DirectoryAttributeOperation>((long)this.GetHashCode(), "GenerateModifyRequestLinkMetaDataHandler: add item: attribute {0}, op:{1}", directoryAttributeModification.Name, directoryAttributeModification.Operation);
				return true;
			}
			return false;
		}

		private DirectoryAttributeModification GetDirectoryAttributeModification(DirectoryAttributeOperation operation)
		{
			return new DirectoryAttributeModification
			{
				Name = ExchangeConfigurationUnitHandler.supportedSharedConfigurationsAttributeName,
				Operation = operation
			};
		}

		private bool TryGetSupportedSharedConfigurations(IConfigurationSession session, string cuObjectDN, TenantRelocationSyncData syncData, out MultiValuedProperty<ADObjectId> links)
		{
			ADObjectId adobjectId = new ADObjectId(cuObjectDN);
			bool useConfigNC = session.UseConfigNC;
			session.UseConfigNC = adobjectId.IsDescendantOf(syncData.Target.PartitionConfigNcRoot);
			bool result;
			try
			{
				ADRawEntry adrawEntry = session.ReadADRawEntry(adobjectId, ExchangeConfigurationUnitHandler.sharedConfigurationsPropertyList);
				if (adrawEntry == null)
				{
					links = null;
					result = false;
				}
				else
				{
					links = (MultiValuedProperty<ADObjectId>)adrawEntry[OrganizationSchema.SupportedSharedConfigurations];
					result = true;
				}
			}
			finally
			{
				session.UseConfigNC = useConfigNC;
			}
			return result;
		}

		private static readonly Lazy<ExchangeConfigurationUnitHandler> instance = new Lazy<ExchangeConfigurationUnitHandler>(() => new ExchangeConfigurationUnitHandler());

		private static readonly IEnumerable<PropertyDefinition> sharedConfigurationsPropertyList = new PropertyDefinition[]
		{
			OrganizationSchema.SupportedSharedConfigurations
		};

		private static readonly string supportedSharedConfigurationsAttributeName = OrganizationSchema.SupportedSharedConfigurations.LdapDisplayName;
	}
}
