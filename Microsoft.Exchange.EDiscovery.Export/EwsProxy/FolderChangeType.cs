using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class FolderChangeType
	{
		[XmlElement("DistinguishedFolderId", typeof(DistinguishedFolderIdType))]
		[XmlElement("FolderId", typeof(FolderIdType))]
		public BaseFolderIdType Item
		{
			get
			{
				return this.itemField;
			}
			set
			{
				this.itemField = value;
			}
		}

		[XmlArrayItem("AppendToFolderField", typeof(AppendToFolderFieldType), IsNullable = false)]
		[XmlArrayItem("SetFolderField", typeof(SetFolderFieldType), IsNullable = false)]
		[XmlArrayItem("DeleteFolderField", typeof(DeleteFolderFieldType), IsNullable = false)]
		public FolderChangeDescriptionType[] Updates
		{
			get
			{
				return this.updatesField;
			}
			set
			{
				this.updatesField = value;
			}
		}

		private BaseFolderIdType itemField;

		private FolderChangeDescriptionType[] updatesField;
	}
}
