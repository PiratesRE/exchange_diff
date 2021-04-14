using System;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "RefreshSharingFolderType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class RefreshSharingFolderRequest : BaseRequest
	{
		[XmlAnyElement(Name = "SharingFolderId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public XmlElement SharingFolderId
		{
			get
			{
				return this.sharingFolderId;
			}
			set
			{
				this.sharingFolderId = value;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new RefreshSharingFolder(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return BaseRequest.GetServerInfoForSingleId(callContext, this.SharingFolderId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}

		private XmlElement sharingFolderId;
	}
}
