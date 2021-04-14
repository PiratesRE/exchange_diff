using System;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class BinaryFileDataObject : ConfigurableObject
	{
		public byte[] FileData
		{
			get
			{
				return (byte[])this.propertyBag[BinaryFileDataObjectSchema.FileData];
			}
			set
			{
				this.propertyBag[BinaryFileDataObjectSchema.FileData] = value;
			}
		}

		public BinaryFileDataObject() : base(new SimpleProviderPropertyBag())
		{
		}

		internal void SetIdentity(ObjectId id)
		{
			this[this.propertyBag.ObjectIdentityPropertyDefinition] = id;
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return BinaryFileDataObject.s_schema;
			}
		}

		private static ObjectSchema s_schema = ObjectSchema.GetInstance<BinaryFileDataObjectSchema>();
	}
}
