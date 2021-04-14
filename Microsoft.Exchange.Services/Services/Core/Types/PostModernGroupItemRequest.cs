using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("PostModernGroupItemType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class PostModernGroupItemRequest : CreateItemRequest
	{
		[XmlElement("ModernGroupEmailAddress")]
		[DataMember(Name = "ModernGroupEmailAddress", IsRequired = true)]
		public EmailAddressWrapper ModernGroupEmailAddress { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new PostModernGroupItem(callContext, this);
		}

		internal override void Validate()
		{
			base.Validate();
			if (this.ModernGroupEmailAddress == null || base.Items.Items == null || base.Items.Items.Length != 1 || !(base.Items.Items[0] is MessageType))
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException((CoreResources.IDs)3784063568U), FaultParty.Sender);
			}
			if (this.ModernGroupEmailAddress.MailboxType != MailboxHelper.MailboxTypeType.GroupMailbox.ToString())
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException((CoreResources.IDs)3784063568U), FaultParty.Sender);
			}
		}
	}
}
