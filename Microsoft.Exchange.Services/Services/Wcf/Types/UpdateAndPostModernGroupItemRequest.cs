using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UpdateAndPostModernGroupItemRequest : UpdateItemRequest
	{
		[XmlIgnore]
		[DataMember(Name = "ModernGroupEmailAddress", IsRequired = true)]
		public EmailAddressWrapper ModernGroupEmailAddress { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new UpdateAndPostModernGroupItem(callContext, this);
		}

		internal override void Validate()
		{
			base.Validate();
			if (this.ModernGroupEmailAddress == null || base.ItemChanges == null || base.ItemChanges.Length == 0)
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
