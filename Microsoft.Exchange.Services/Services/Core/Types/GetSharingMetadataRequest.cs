using System;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "GetSharingMetadataType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetSharingMetadataRequest : BaseRequest
	{
		[XmlAnyElement(Name = "IdOfFolderToShare")]
		public XmlElement IdOfFolderToShare
		{
			get
			{
				return this.idOfFolderToShare;
			}
			set
			{
				this.idOfFolderToShare = value;
			}
		}

		[XmlElement("SenderSmtpAddress", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string SenderSmtpAddress
		{
			get
			{
				return this.senderSmtpAddress;
			}
			set
			{
				this.senderSmtpAddress = value;
			}
		}

		[XmlArrayItem("SmtpAddress", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		[XmlArray("Recipients", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public string[] Recipients
		{
			get
			{
				return this.recipients;
			}
			set
			{
				this.recipients = value;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetSharingMetadata(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return BaseRequest.GetServerInfoForSingleId(callContext, this.IdOfFolderToShare);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(false, callContext);
		}

		private XmlElement idOfFolderToShare;

		private string senderSmtpAddress;

		private string[] recipients;
	}
}
