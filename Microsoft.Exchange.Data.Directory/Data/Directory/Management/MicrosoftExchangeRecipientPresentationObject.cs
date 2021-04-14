using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class MicrosoftExchangeRecipientPresentationObject : MailEnabledRecipient
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return MicrosoftExchangeRecipientPresentationObject.schema;
			}
		}

		public MicrosoftExchangeRecipientPresentationObject()
		{
		}

		public MicrosoftExchangeRecipientPresentationObject(ADMicrosoftExchangeRecipient dataObject) : base(dataObject)
		{
		}

		private static MicrosoftExchangeRecipientPresentationObjectSchema schema = ObjectSchema.GetInstance<MicrosoftExchangeRecipientPresentationObjectSchema>();
	}
}
