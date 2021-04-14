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
	[XmlInclude(typeof(ModifiedEventType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlInclude(typeof(MovedCopiedEventType))]
	[Serializable]
	public class BaseObjectChangedEventType : BaseNotificationEventType
	{
		public DateTime TimeStamp
		{
			get
			{
				return this.timeStampField;
			}
			set
			{
				this.timeStampField = value;
			}
		}

		[XmlElement("ItemId", typeof(ItemIdType))]
		[XmlElement("FolderId", typeof(FolderIdType))]
		public object Item
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

		public FolderIdType ParentFolderId
		{
			get
			{
				return this.parentFolderIdField;
			}
			set
			{
				this.parentFolderIdField = value;
			}
		}

		private DateTime timeStampField;

		private object itemField;

		private FolderIdType parentFolderIdField;
	}
}
