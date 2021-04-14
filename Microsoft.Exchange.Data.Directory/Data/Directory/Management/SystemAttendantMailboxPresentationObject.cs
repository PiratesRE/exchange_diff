using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class SystemAttendantMailboxPresentationObject : MailEnabledRecipient
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return SystemAttendantMailboxPresentationObject.schema;
			}
		}

		public SystemAttendantMailboxPresentationObject()
		{
		}

		public SystemAttendantMailboxPresentationObject(ADSystemAttendantMailbox dataObject) : base(dataObject)
		{
		}

		private static SystemAttendantMailboxPresentationObjectSchema schema = ObjectSchema.GetInstance<SystemAttendantMailboxPresentationObjectSchema>();
	}
}
