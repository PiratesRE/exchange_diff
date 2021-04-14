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
	public class SyncFolderItemsType : BaseRequestType
	{
		public ItemResponseShapeType ItemShape
		{
			get
			{
				return this.itemShapeField;
			}
			set
			{
				this.itemShapeField = value;
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

		[XmlArrayItem("ItemId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public ItemIdType[] Ignore
		{
			get
			{
				return this.ignoreField;
			}
			set
			{
				this.ignoreField = value;
			}
		}

		public int MaxChangesReturned
		{
			get
			{
				return this.maxChangesReturnedField;
			}
			set
			{
				this.maxChangesReturnedField = value;
			}
		}

		public SyncFolderItemsScopeType SyncScope
		{
			get
			{
				return this.syncScopeField;
			}
			set
			{
				this.syncScopeField = value;
			}
		}

		[XmlIgnore]
		public bool SyncScopeSpecified
		{
			get
			{
				return this.syncScopeFieldSpecified;
			}
			set
			{
				this.syncScopeFieldSpecified = value;
			}
		}

		private ItemResponseShapeType itemShapeField;

		private TargetFolderIdType syncFolderIdField;

		private string syncStateField;

		private ItemIdType[] ignoreField;

		private int maxChangesReturnedField;

		private SyncFolderItemsScopeType syncScopeField;

		private bool syncScopeFieldSpecified;
	}
}
