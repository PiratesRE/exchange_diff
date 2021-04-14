using System;
using System.Xml.Serialization;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.OOF;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.InfoWorker.Availability
{
	[XmlType("SetUserOofSettingsRequest", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SetUserOofSettingsRequest : BaseAvailabilityRequest
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

		[XmlElement(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public UserOofSettings UserOofSettings
		{
			get
			{
				return this.userOofSettings;
			}
			set
			{
				this.userOofSettings = value;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new SetUserOofSettings(callContext, this);
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

		internal const string ElementName = "SetUserOofSettingsRequest";

		internal const string MailboxElementName = "Mailbox";

		private EmailAddress mailbox;

		private UserOofSettings userOofSettings;
	}
}
