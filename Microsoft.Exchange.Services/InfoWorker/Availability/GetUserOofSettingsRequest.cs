using System;
using System.Xml.Serialization;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.InfoWorker.Availability
{
	[XmlType("GetUserOofSettingsRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetUserOofSettingsRequest : BaseAvailabilityRequest
	{
		[XmlElement(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public EmailAddress Mailbox
		{
			get
			{
				return this.mailbox;
			}
			set
			{
				this.mailbox = value;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetUserOofSettings(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			MailboxIdServerInfo result = null;
			if (this.Mailbox != null)
			{
				string address = this.Mailbox.Address;
				if (Util.IsValidSmtpAddress(address))
				{
					result = MailboxIdServerInfo.Create(address);
				}
			}
			return result;
		}

		private EmailAddress mailbox;
	}
}
