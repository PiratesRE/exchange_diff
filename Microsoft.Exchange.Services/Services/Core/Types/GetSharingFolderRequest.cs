using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "GetSharingFolderType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetSharingFolderRequest : BaseRequest
	{
		[XmlElement("SmtpAddress", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string SmtpAddress { get; set; }

		[XmlElement("DataType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string DataType { get; set; }

		[XmlElement("SharedFolderId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string SharedFolderId { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetSharingFolder(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return callContext.GetServerInfoForEffectiveCaller();
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}
	}
}
