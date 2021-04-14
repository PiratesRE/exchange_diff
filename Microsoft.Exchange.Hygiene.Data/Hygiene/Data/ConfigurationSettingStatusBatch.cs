using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class ConfigurationSettingStatusBatch : ConfigurablePropertyBag
	{
		public ConfigurationSettingStatusBatch(Guid tenantId)
		{
			this.TenantId = tenantId;
			this.Batch = new MultiValuedProperty<IConfigurable>();
		}

		public override ObjectId Identity
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		private Guid TenantId
		{
			get
			{
				return (Guid)this[ConfigurationSettingStatusBatchSchema.TenantIdProp];
			}
			set
			{
				this[ConfigurationSettingStatusBatchSchema.TenantIdProp] = value;
			}
		}

		private MultiValuedProperty<IConfigurable> Batch
		{
			get
			{
				return (MultiValuedProperty<IConfigurable>)this[ConfigurationSettingBatchSchema.BatchProp];
			}
			set
			{
				this[ConfigurationSettingBatchSchema.BatchProp] = value;
			}
		}

		public void Add(IConfigurable configurableObject)
		{
			if (configurableObject == null)
			{
				throw new ArgumentNullException("configurableObject");
			}
			this.Batch.Add(configurableObject);
		}
	}
}
