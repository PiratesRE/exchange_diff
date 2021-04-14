using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(GetDelegateRequest))]
	[XmlType("BaseDelegateType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[XmlInclude(typeof(RemoveDelegateRequest))]
	[XmlInclude(typeof(AddDelegateRequest))]
	[XmlInclude(typeof(UpdateDelegateRequest))]
	public abstract class BaseDelegateRequest : BaseRequest
	{
		protected BaseDelegateRequest(bool isWriteOperation = true)
		{
			this.isWriteOperation = isWriteOperation;
		}

		[XmlElement("Mailbox")]
		public EmailAddressWrapper Mailbox
		{
			get
			{
				return this.emailAddress;
			}
			set
			{
				this.emailAddress = value;
			}
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			string text = this.Mailbox.EmailAddress;
			MailboxIdServerInfo result = null;
			if (Util.IsValidSmtpAddress(text))
			{
				result = MailboxIdServerInfo.Create(text);
			}
			return result;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int currentStep)
		{
			if (this.resourceKeys == null)
			{
				this.resourceKeys = base.GetResourceKeysFromProxyInfo(this.isWriteOperation, callContext);
			}
			return this.resourceKeys;
		}

		private readonly bool isWriteOperation;

		private EmailAddressWrapper emailAddress;

		private ResourceKey[] resourceKeys;
	}
}
