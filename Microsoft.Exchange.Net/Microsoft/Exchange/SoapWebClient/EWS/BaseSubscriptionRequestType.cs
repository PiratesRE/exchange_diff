using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlInclude(typeof(PushSubscriptionRequestType))]
	[XmlInclude(typeof(PullSubscriptionRequestType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public abstract class BaseSubscriptionRequestType
	{
		[XmlArrayItem("FolderId", typeof(FolderIdType), IsNullable = false)]
		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderIdType), IsNullable = false)]
		public BaseFolderIdType[] FolderIds;

		[XmlArrayItem("EventType", IsNullable = false)]
		public NotificationEventTypeType[] EventTypes;

		public string Watermark;

		[XmlAttribute]
		public bool SubscribeToAllFolders;

		[XmlIgnore]
		public bool SubscribeToAllFoldersSpecified;
	}
}
