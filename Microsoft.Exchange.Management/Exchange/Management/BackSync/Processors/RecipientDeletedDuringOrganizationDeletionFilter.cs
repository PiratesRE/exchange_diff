using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal class RecipientDeletedDuringOrganizationDeletionFilter : PipelineProcessor
	{
		public RecipientDeletedDuringOrganizationDeletionFilter(IDataProcessor next, IPropertyLookup organizationLookup, ExcludedObjectReporter reporter) : base(next)
		{
			this.organizationLookup = organizationLookup;
			this.reporter = reporter;
		}

		protected override bool ProcessInternal(PropertyBag propertyBag)
		{
			if (!ProcessorHelper.IsDeletedObject(propertyBag))
			{
				return true;
			}
			ADObjectId tenantOU = ProcessorHelper.GetTenantOU(propertyBag);
			ADRawEntry properties = this.organizationLookup.GetProperties(tenantOU);
			if (!RecipientDeletedDuringOrganizationDeletionFilter.WasOrganizationForThisObjectDeleted(properties))
			{
				return true;
			}
			if (RecipientDeletedDuringOrganizationDeletionFilter.WasObjectDeletedBeforeOrganization(propertyBag, properties))
			{
				return true;
			}
			this.reporter.ReportExcludedObject(propertyBag, DirectoryObjectErrorCode.ObjectOutOfScope, ProcessingStage.RecipientDeletedDuringOrganizationDeletionFilter);
			return false;
		}

		private static bool WasObjectDeletedBeforeOrganization(PropertyBag propertyBag, ADRawEntry org)
		{
			DateTime? dateTime = (DateTime?)org[ExchangeConfigurationUnitSchema.WhenOrganizationStatusSet];
			if (dateTime != null)
			{
				MultiValuedProperty<AttributeMetadata> multiValuedProperty = (MultiValuedProperty<AttributeMetadata>)propertyBag[ADRecipientSchema.AttributeMetadata];
				foreach (AttributeMetadata attributeMetadata in multiValuedProperty)
				{
					if (attributeMetadata.AttributeName.Equals(SyncObjectSchema.Deleted.LdapDisplayName, StringComparison.OrdinalIgnoreCase))
					{
						return attributeMetadata.LastWriteTime.ToUniversalTime() < dateTime.Value.ToUniversalTime();
					}
				}
				return false;
			}
			return false;
		}

		private static bool WasOrganizationForThisObjectDeleted(ADRawEntry org)
		{
			return ProcessorHelper.IsDeletedObject(org.propertyBag) || ExchangeConfigurationUnit.IsBeingDeleted((OrganizationStatus)org[ExchangeConfigurationUnitSchema.OrganizationStatus]);
		}

		private readonly IPropertyLookup organizationLookup;

		private readonly ExcludedObjectReporter reporter;
	}
}
