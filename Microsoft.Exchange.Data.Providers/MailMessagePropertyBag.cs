using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Providers
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class MailMessagePropertyBag : PropertyBag
	{
		public MailMessagePropertyBag(bool readOnly, int initialSize) : base(readOnly, initialSize)
		{
		}

		public MailMessagePropertyBag()
		{
		}

		internal override ProviderPropertyDefinition ObjectIdentityPropertyDefinition
		{
			get
			{
				return MailMessageSchema.Identity;
			}
		}

		internal override ProviderPropertyDefinition ObjectVersionPropertyDefinition
		{
			get
			{
				return SimpleProviderObjectSchema.ExchangeVersion;
			}
		}

		internal override ProviderPropertyDefinition ObjectStatePropertyDefinition
		{
			get
			{
				return SimpleProviderObjectSchema.ObjectState;
			}
		}
	}
}
