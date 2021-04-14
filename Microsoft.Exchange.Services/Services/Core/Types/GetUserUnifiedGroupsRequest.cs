using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Core.Search;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "GetUserUnifiedGroupsRequest", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetUserUnifiedGroupsRequest : BaseRequest
	{
		[DataMember(Name = "RequestedGroupsSets", IsRequired = false)]
		[XmlArrayItem("RequestedUnifiedGroupsSet", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArray("RequestedGroupsSets")]
		public RequestedUnifiedGroupsSet[] RequestedGroupsSets { get; set; }

		internal RequestedUnifiedGroupsSet[] ValidateParams()
		{
			if (this.RequestedGroupsSets == null || !this.RequestedGroupsSets.Any<RequestedUnifiedGroupsSet>())
			{
				return new RequestedUnifiedGroupsSet[]
				{
					new RequestedUnifiedGroupsSet
					{
						FilterType = UnifiedGroupsFilterType.All,
						SortType = UnifiedGroupsSortType.DisplayName,
						SortDirection = SortDirection.Ascending
					}
				};
			}
			if (this.RequestedGroupsSets.Length > 3)
			{
				throw new ServiceArgumentException((CoreResources.IDs)3784063568U, CoreResources.ErrorMaxRequestedUnifiedGroupsSetsExceeded);
			}
			return this.RequestedGroupsSets;
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetUserUnifiedGroups(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}

		private const int MaxRequestedGroupsSets = 3;
	}
}
