using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.Management.BackSync.Processors
{
	internal class IncludedBackIntoBacksyncDetector : PipelineProcessor
	{
		public IncludedBackIntoBacksyncDetector(IDataProcessor next, ServiceInstanceId serviceInstanceId) : base(next)
		{
			this.serviceInstanceId = serviceInstanceId;
		}

		protected override bool ProcessInternal(PropertyBag propertyBag)
		{
			if (propertyBag.Contains(SyncUserSchema.ServiceOriginatedResource))
			{
				MultiValuedProperty<Capability> multiValuedProperty = (MultiValuedProperty<Capability>)propertyBag[SyncUserSchema.ServiceOriginatedResource];
				if (!multiValuedProperty.Contains(Capability.ExcludedFromBackSync) && propertyBag.Count == 6)
				{
					propertyBag.SetField(SyncObjectSchema.FaultInServiceInstance, this.serviceInstanceId);
				}
			}
			return true;
		}

		private readonly ServiceInstanceId serviceInstanceId;
	}
}
