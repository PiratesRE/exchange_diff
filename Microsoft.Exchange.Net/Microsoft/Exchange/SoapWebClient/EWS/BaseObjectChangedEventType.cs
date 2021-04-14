using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlInclude(typeof(MovedCopiedEventType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(ModifiedEventType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class BaseObjectChangedEventType : BaseNotificationEventType
	{
		public DateTime TimeStamp;

		[XmlElement("ItemId", typeof(ItemIdType))]
		[XmlElement("FolderId", typeof(FolderIdType))]
		public object Item;

		public FolderIdType ParentFolderId;
	}
}
