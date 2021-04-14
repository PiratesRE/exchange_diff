using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class FindItemResponseMessageType : ResponseMessageType
	{
		public FindItemParentType RootFolder
		{
			get
			{
				return this.rootFolderField;
			}
			set
			{
				this.rootFolderField = value;
			}
		}

		[XmlArrayItem("Term", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public HighlightTermType[] HighlightTerms
		{
			get
			{
				return this.highlightTermsField;
			}
			set
			{
				this.highlightTermsField = value;
			}
		}

		private FindItemParentType rootFolderField;

		private HighlightTermType[] highlightTermsField;
	}
}
