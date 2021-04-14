using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class SystemMailboxPresentationObject : MailEnabledRecipient
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return SystemMailboxPresentationObject.schema;
			}
		}

		public SystemMailboxPresentationObject()
		{
		}

		public SystemMailboxPresentationObject(ADSystemMailbox dataObject) : base(dataObject)
		{
		}

		private static SystemMailboxPresentationObjectSchema schema = ObjectSchema.GetInstance<SystemMailboxPresentationObjectSchema>();
	}
}
