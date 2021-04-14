using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class SyncFolderHierarchyType : BaseRequestType
	{
		public FolderResponseShapeType FolderShape
		{
			get
			{
				return this.folderShapeField;
			}
			set
			{
				this.folderShapeField = value;
			}
		}

		public TargetFolderIdType SyncFolderId
		{
			get
			{
				return this.syncFolderIdField;
			}
			set
			{
				this.syncFolderIdField = value;
			}
		}

		public string SyncState
		{
			get
			{
				return this.syncStateField;
			}
			set
			{
				this.syncStateField = value;
			}
		}

		private FolderResponseShapeType folderShapeField;

		private TargetFolderIdType syncFolderIdField;

		private string syncStateField;
	}
}
