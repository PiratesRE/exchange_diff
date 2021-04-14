using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.BackSync;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal class MissingPropertyResolver : PipelineProcessor, IMissingPropertyResolver
	{
		public ADRawEntry LastProcessedEntry { get; private set; }

		public MissingPropertyResolver(IDataProcessor next, IPropertyLookup objectPropertyLookup) : base(next)
		{
			this.objectPropertyLookup = objectPropertyLookup;
		}

		protected override bool ProcessInternal(PropertyBag propertyBag)
		{
			ADObjectId id = (ADObjectId)propertyBag[ADObjectSchema.Id];
			ADRawEntry properties = this.objectPropertyLookup.GetProperties(id);
			if (properties == null)
			{
				ExTraceGlobals.BackSyncTracer.TraceDebug((long)SyncConfiguration.TraceId, "MissingPropertyResolver:: - Skipping object {0}. Cannot read missing properties. Object was removed. Next incremantal sync will pick up deletion.", new object[]
				{
					propertyBag[ADObjectSchema.Id]
				});
				return false;
			}
			foreach (object obj in properties.propertyBag.Keys)
			{
				ProviderPropertyDefinition providerPropertyDefinition = (ProviderPropertyDefinition)obj;
				if (!propertyBag.Contains(providerPropertyDefinition))
				{
					propertyBag.SetField(providerPropertyDefinition, properties[providerPropertyDefinition]);
				}
			}
			if (ProcessorHelper.IsObjectOrganizationUnit(propertyBag) && !propertyBag.Contains(SyncCompanySchema.DirSyncStatusAck) && propertyBag.Contains(ExtendedOrganizationalUnitSchema.DirSyncStatusAck) && propertyBag[ExtendedOrganizationalUnitSchema.DirSyncStatusAck] != null && ((MultiValuedProperty<string>)propertyBag[ExtendedOrganizationalUnitSchema.DirSyncStatusAck]).Count > 0)
			{
				propertyBag.SetField(SyncCompanySchema.DirSyncStatusAck, propertyBag[ExtendedOrganizationalUnitSchema.DirSyncStatusAck]);
			}
			this.LastProcessedEntry = properties;
			return true;
		}

		private readonly IPropertyLookup objectPropertyLookup;
	}
}
