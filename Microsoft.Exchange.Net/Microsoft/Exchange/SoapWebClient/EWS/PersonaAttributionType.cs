using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class PersonaAttributionType
	{
		public string Id;

		public ItemIdType SourceId;

		public string DisplayName;

		public bool IsWritable;

		[XmlIgnore]
		public bool IsWritableSpecified;

		public bool IsQuickContact;

		[XmlIgnore]
		public bool IsQuickContactSpecified;

		public bool IsHidden;

		[XmlIgnore]
		public bool IsHiddenSpecified;

		public FolderIdType FolderId;
	}
}
