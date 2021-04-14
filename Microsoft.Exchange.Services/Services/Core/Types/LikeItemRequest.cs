using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(ItemId))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class LikeItemRequest : BaseRequest
	{
		public LikeItemRequest(ItemId itemId, bool isUnlike = false)
		{
			this.ItemId = itemId;
			this.IsUnlike = isUnlike;
		}

		[DataMember(Name = "ItemId", IsRequired = true)]
		public ItemId ItemId { get; set; }

		[DataMember(Name = "IsUnlike")]
		public bool IsUnlike { get; set; }

		internal override void Validate()
		{
			base.Validate();
			if (this.ItemId == null)
			{
				throw FaultExceptionUtilities.CreateFault(new ServiceInvalidOperationException(ResponseCodeType.ErrorInvalidIdEmpty), FaultParty.Sender);
			}
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysForItemId(true, callContext, this.ItemId);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return BaseRequest.GetServerInfoForItemIdList(callContext, new ItemId[]
			{
				this.ItemId
			});
		}
	}
}
