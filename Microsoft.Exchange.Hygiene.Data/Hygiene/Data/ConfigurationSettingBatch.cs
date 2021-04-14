using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class ConfigurationSettingBatch<T> : ConfigurablePropertyBag where T : IConfigurable, IPropertyBag
	{
		public ConfigurationSettingBatch(ADObjectId organizationalUnitRoot)
		{
			if (organizationalUnitRoot == null)
			{
				throw new ArgumentNullException("organizationalUnitRoot");
			}
			this.OrganizationalUnitRoot = organizationalUnitRoot;
			this.Batch = new MultiValuedProperty<T>();
		}

		public override ObjectId Identity
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		private ADObjectId OrganizationalUnitRoot
		{
			get
			{
				return (ADObjectId)this[ConfigurationSettingBatchSchema.OrganizationalUnitRootProp];
			}
			set
			{
				this[ConfigurationSettingBatchSchema.OrganizationalUnitRootProp] = value;
			}
		}

		private MultiValuedProperty<T> Batch
		{
			get
			{
				return (MultiValuedProperty<T>)this[ConfigurationSettingBatchSchema.BatchProp];
			}
			set
			{
				this[ConfigurationSettingBatchSchema.BatchProp] = value;
			}
		}

		public void Add(T configurableObject)
		{
			if (configurableObject == null)
			{
				throw new ArgumentNullException("configurableObject");
			}
			object obj = configurableObject[ConfigurationSettingBatchSchema.OrganizationalUnitRootProp];
			if (obj != null && ((obj is Guid && (Guid)obj != this.OrganizationalUnitRoot.ObjectGuid) || (obj is ADObjectId && ((ADObjectId)obj).ObjectGuid != this.OrganizationalUnitRoot.ObjectGuid)))
			{
				throw new InvalidOperationException("Attempt to add object with mismatching tenant id to an existing batch.");
			}
			this.Batch.Add(configurableObject);
		}
	}
}
