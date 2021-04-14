using System;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class RecipientAdaptor : IRecipient
	{
		internal RecipientAdaptor(CoreRecipient coreRecipient, ICoreObject propertyMappingReference, Encoding string8Encoding, bool wantUnicode)
		{
			this.coreRecipient = coreRecipient;
			this.propertyMappingReference = propertyMappingReference;
			this.string8Encoding = string8Encoding;
			this.wantUnicode = wantUnicode;
		}

		public IPropertyBag PropertyBag
		{
			get
			{
				if (this.recipientPropertyBag == null)
				{
					this.recipientPropertyBag = new RecipientPropertyBagAdaptor(this.coreRecipient.PropertyBag, this.propertyMappingReference, this.string8Encoding, this.wantUnicode);
				}
				return this.recipientPropertyBag;
			}
		}

		public void Save()
		{
			if (!this.coreRecipient.TryValidateRecipient())
			{
				throw new CorruptRecipientException("Failed to save recipient. The recipient does not have the required properties.", (ErrorCode)2147746075U);
			}
		}

		private readonly CoreRecipient coreRecipient;

		private readonly ICoreObject propertyMappingReference;

		private readonly Encoding string8Encoding;

		private readonly bool wantUnicode;

		private RecipientPropertyBagAdaptor recipientPropertyBag;
	}
}
