using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class StreamingSubscriptionRequestType
	{
		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderIdType), IsNullable = false)]
		[XmlArrayItem("FolderId", typeof(FolderIdType), IsNullable = false)]
		public BaseFolderIdType[] FolderIds;

		[XmlArrayItem("EventType", IsNullable = false)]
		public NotificationEventTypeType[] EventTypes;

		[XmlAttribute]
		public bool SubscribeToAllFolders;

		[XmlIgnore]
		public bool SubscribeToAllFoldersSpecified;
	}
}
