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
	public class UpdateFolderType : BaseRequestType
	{
		[XmlArrayItem("FolderChange", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public FolderChangeType[] FolderChanges
		{
			get
			{
				return this.folderChangesField;
			}
			set
			{
				this.folderChangesField = value;
			}
		}

		private FolderChangeType[] folderChangesField;
	}
}
