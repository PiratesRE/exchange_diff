using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Reporting;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.Reporting
{
	internal class TenantThrottleInfoBatch : ConfigurablePropertyBag
	{
		public TenantThrottleInfoBatch()
		{
			this.TenantThrottleInfoList = new MultiValuedProperty<TenantThrottleInfo>();
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.identity.ToString());
			}
		}

		public int PartitionId
		{
			get
			{
				return (int)this[TenantThrottleInfoBatchSchema.PhysicalInstanceKeyProp];
			}
			set
			{
				this[TenantThrottleInfoBatchSchema.PhysicalInstanceKeyProp] = value;
			}
		}

		public int FssCopyId
		{
			get
			{
				return (int)this[TenantThrottleInfoBatchSchema.FssCopyIdProp];
			}
			set
			{
				this[TenantThrottleInfoBatchSchema.FssCopyIdProp] = value;
			}
		}

		public MultiValuedProperty<TenantThrottleInfo> TenantThrottleInfoList
		{
			get
			{
				return (MultiValuedProperty<TenantThrottleInfo>)this[TenantThrottleInfoBatchSchema.TenantThrottleInfoListProperty];
			}
			set
			{
				this[TenantThrottleInfoBatchSchema.TenantThrottleInfoListProperty] = value;
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(TenantThrottleInfoBatchSchema);
		}

		private readonly Guid identity = CombGuidGenerator.NewGuid();
	}
}
