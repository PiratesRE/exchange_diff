using System;

namespace Microsoft.Exchange.Data.QueueViewer
{
	[Serializable]
	internal class MessageInfoPropertyBag : PropertyBag
	{
		public MessageInfoPropertyBag() : this(false, 16)
		{
		}

		public MessageInfoPropertyBag(bool readOnly, int initialSize) : base(readOnly, initialSize)
		{
			base.SetField(this.ObjectVersionPropertyDefinition, ExchangeObjectVersion.Exchange2010);
		}

		internal override ProviderPropertyDefinition ObjectVersionPropertyDefinition
		{
			get
			{
				return ExtensibleMessageInfoSchema.ExchangeVersion;
			}
		}

		internal override ProviderPropertyDefinition ObjectStatePropertyDefinition
		{
			get
			{
				return ExtensibleMessageInfoSchema.ObjectState;
			}
		}

		internal override ProviderPropertyDefinition ObjectIdentityPropertyDefinition
		{
			get
			{
				return ExtensibleMessageInfoSchema.Identity;
			}
		}
	}
}
