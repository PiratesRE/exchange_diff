using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class RefreshSharingFolderType : BaseRequestType
	{
		public FolderIdType SharingFolderId
		{
			get
			{
				return this.sharingFolderIdField;
			}
			set
			{
				this.sharingFolderIdField = value;
			}
		}

		private FolderIdType sharingFolderIdField;
	}
}
