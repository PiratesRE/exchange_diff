using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class SyncFolderItemsType : BaseRequestType
	{
		public ItemResponseShapeType ItemShape;

		public TargetFolderIdType SyncFolderId;

		public string SyncState;

		[XmlArrayItem("ItemId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public ItemIdType[] Ignore;

		public int MaxChangesReturned;

		public SyncFolderItemsScopeType SyncScope;

		[XmlIgnore]
		public bool SyncScopeSpecified;
	}
}
