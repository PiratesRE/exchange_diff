using System;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("CreateManagedFolderRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class CreateManagedFolderRequest : BaseRequest
	{
		[XmlArray(ElementName = "FolderNames", IsNullable = false, Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[XmlArrayItem(ElementName = "FolderName", IsNullable = false, Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public string[] FolderNames
		{
			get
			{
				return this.folderNames;
			}
			set
			{
				this.folderNames = value;
			}
		}

		[XmlAnyElement(Name = "Mailbox", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public XmlElement Mailbox
		{
			get
			{
				return this.mailbox;
			}
			set
			{
				this.mailbox = value;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new CreateManagedFolder(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return BaseRequest.GetServerInfoFromMailboxElement(callContext, this.Mailbox);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return base.GetResourceKeysFromProxyInfo(true, callContext);
		}

		internal const string FolderNameElementName = "FolderName";

		internal const string FolderNamesElementName = "FolderNames";

		internal const string MailboxElementName = "Mailbox";

		private string[] folderNames;

		private XmlElement mailbox;
	}
}
