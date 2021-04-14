using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Name = "ModernGroupMembershipRequestMessageDetailsRequest", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ModernGroupMembershipRequestMessageDetailsRequest : BaseRequest
	{
		[DataMember(Name = "MessageId")]
		public ItemId MessageId { get; set; }

		internal StoreObjectId MessageStoreId { get; private set; }

		internal override void Validate()
		{
			if (this.MessageId == null || string.IsNullOrEmpty(this.MessageId.Id))
			{
				throw FaultExceptionUtilities.CreateFault(new InvalidRequestException(), FaultParty.Sender);
			}
			this.MessageStoreId = ServiceIdConverter.ConvertFromConcatenatedId(this.MessageId.Id, BasicTypes.Item, null).ToStoreObjectId();
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}
	}
}
