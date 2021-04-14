using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.UM.UMCommon
{
	[Serializable]
	public class UMPrompt : ConfigurableObject
	{
		public UMPrompt(ObjectId identity) : base(new SimpleProviderPropertyBag())
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			this.propertyBag.SetField(SimpleProviderObjectSchema.Identity, identity);
		}

		public byte[] AudioData
		{
			get
			{
				return (byte[])this[UMPromptSchema.AudioData];
			}
			set
			{
				this[UMPromptSchema.AudioData] = value;
			}
		}

		public string Name
		{
			get
			{
				return (string)this[UMPromptSchema.Name];
			}
			set
			{
				this[UMPromptSchema.Name] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return UMPrompt.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static UMPromptSchema schema = ObjectSchema.GetInstance<UMPromptSchema>();
	}
}
