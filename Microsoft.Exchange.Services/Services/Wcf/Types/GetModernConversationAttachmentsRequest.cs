using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetModernConversationAttachmentsRequest : BaseRequest
	{
		[DataMember(Name = "FoldersToIgnore", IsRequired = false)]
		public BaseFolderId[] FoldersToIgnore { get; set; }

		[DataMember(IsRequired = false)]
		public bool ClientSupportsIrm
		{
			get
			{
				return this.clientSupportsIrm;
			}
			set
			{
				this.clientSupportsIrm = value;
			}
		}

		[DataMember(Name = "Conversations", IsRequired = true)]
		public ConversationRequestType[] Conversations { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetModernConversationAttachments(callContext, this);
		}

		internal override void Validate()
		{
			base.Validate();
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return BaseRequest.GetServerInfoForItemId(callContext, this.Conversations[0].ConversationId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			if (this.Conversations == null || taskStep > this.Conversations.Length)
			{
				return null;
			}
			return base.GetResourceKeysForItemId(false, callContext, this.Conversations[taskStep].ConversationId);
		}

		private bool clientSupportsIrm;
	}
}
