using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Core.Search;

namespace Microsoft.Exchange.Services.Core.Types
{
	public class SubscribeToMessageChangesRequest : BaseRequest
	{
		public string SubscriptionId { get; set; }

		public TargetFolderId ParentFolderId { get; set; }

		public ItemResponseShape MessageShape { get; set; }

		public SortResults[] SortOrder { get; set; }

		internal Guid[] MailboxGuids { get; set; }

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			throw new NotImplementedException();
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			throw new NotImplementedException();
		}
	}
}
