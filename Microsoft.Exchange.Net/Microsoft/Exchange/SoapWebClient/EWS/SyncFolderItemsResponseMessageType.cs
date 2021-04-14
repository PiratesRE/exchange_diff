using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class SyncFolderItemsResponseMessageType : ResponseMessageType
	{
		public string SyncState;

		public bool IncludesLastItemInRange;

		[XmlIgnore]
		public bool IncludesLastItemInRangeSpecified;

		public SyncFolderItemsChangesType Changes;
	}
}
