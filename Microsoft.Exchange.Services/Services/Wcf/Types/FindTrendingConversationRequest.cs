using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class FindTrendingConversationRequest : BaseRequest
	{
		[XmlIgnore]
		[DataMember(Name = "ParentFolderId", IsRequired = true)]
		public TargetFolderId ParentFolderId { get; set; }

		[XmlIgnore]
		[DataMember(Name = "PageSize", IsRequired = true)]
		public int PageSize { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new FindTrendingConversationCommand(callContext, this);
		}

		internal override void Validate()
		{
			base.Validate();
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
