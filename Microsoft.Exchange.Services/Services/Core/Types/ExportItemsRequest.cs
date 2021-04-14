using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("ExportItemsType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class ExportItemsRequest : BaseRequest, IRemoteArchiveRequest
	{
		[XmlElement("ItemIds", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public XmlNodeArray Ids
		{
			get
			{
				return this.ids;
			}
			set
			{
				this.ids = value;
				if (this.ids != null)
				{
					this.ids.Normalize();
				}
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			if (this != null && ((IRemoteArchiveRequest)this).IsRemoteArchiveRequest(callContext))
			{
				return ((IRemoteArchiveRequest)this).GetRemoteArchiveServiceCommand(callContext);
			}
			return new ExportItems(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.ids == null)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForIdList(callContext, this.ids.Nodes);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			if (this.ids == null || this.ids.Nodes.Count < taskStep)
			{
				return null;
			}
			if (this != null && ((IRemoteArchiveRequest)this).IsRemoteArchiveRequest(callContext))
			{
				return null;
			}
			return base.GetResourceKeysForXmlNode(false, callContext, this.ids.Nodes[taskStep]);
		}

		ExchangeServiceBinding IRemoteArchiveRequest.ArchiveService { get; set; }

		bool IRemoteArchiveRequest.IsRemoteArchiveRequest(CallContext callContext)
		{
			return ComplianceUtil.TryCreateArchiveService(callContext, this, this.ids != null, delegate
			{
				((IRemoteArchiveRequest)this).ArchiveService = ComplianceUtil.GetArchiveServiceForItemIdList(callContext, this.ids.Nodes);
			});
		}

		ServiceCommandBase IRemoteArchiveRequest.GetRemoteArchiveServiceCommand(CallContext callContext)
		{
			return new ExportRemoteArchiveItems(callContext, this);
		}

		internal const string ElementName = "ExportItems";

		private const string ItemIdsElementName = "ItemIds";

		private XmlNodeArray ids;
	}
}
