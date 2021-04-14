using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[XmlInclude(typeof(PullSubscriptionRequestType))]
	[XmlInclude(typeof(PushSubscriptionRequestType))]
	[DebuggerStepThrough]
	[Serializable]
	public abstract class BaseSubscriptionRequestType
	{
		[XmlArrayItem("DistinguishedFolderId", typeof(DistinguishedFolderIdType), IsNullable = false)]
		[XmlArrayItem("FolderId", typeof(FolderIdType), IsNullable = false)]
		public BaseFolderIdType[] FolderIds
		{
			get
			{
				return this.folderIdsField;
			}
			set
			{
				this.folderIdsField = value;
			}
		}

		[XmlArrayItem("EventType", IsNullable = false)]
		public NotificationEventTypeType[] EventTypes
		{
			get
			{
				return this.eventTypesField;
			}
			set
			{
				this.eventTypesField = value;
			}
		}

		public string Watermark
		{
			get
			{
				return this.watermarkField;
			}
			set
			{
				this.watermarkField = value;
			}
		}

		[XmlAttribute]
		public bool SubscribeToAllFolders
		{
			get
			{
				return this.subscribeToAllFoldersField;
			}
			set
			{
				this.subscribeToAllFoldersField = value;
			}
		}

		[XmlIgnore]
		public bool SubscribeToAllFoldersSpecified
		{
			get
			{
				return this.subscribeToAllFoldersFieldSpecified;
			}
			set
			{
				this.subscribeToAllFoldersFieldSpecified = value;
			}
		}

		private BaseFolderIdType[] folderIdsField;

		private NotificationEventTypeType[] eventTypesField;

		private string watermarkField;

		private bool subscribeToAllFoldersField;

		private bool subscribeToAllFoldersFieldSpecified;
	}
}
