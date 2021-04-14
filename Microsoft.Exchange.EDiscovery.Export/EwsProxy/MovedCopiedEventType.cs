using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class MovedCopiedEventType : BaseObjectChangedEventType
	{
		[XmlElement("OldFolderId", typeof(FolderIdType))]
		[XmlElement("OldItemId", typeof(ItemIdType))]
		public object Item1
		{
			get
			{
				return this.item1Field;
			}
			set
			{
				this.item1Field = value;
			}
		}

		public FolderIdType OldParentFolderId
		{
			get
			{
				return this.oldParentFolderIdField;
			}
			set
			{
				this.oldParentFolderIdField = value;
			}
		}

		private object item1Field;

		private FolderIdType oldParentFolderIdField;
	}
}
