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
	[DebuggerStepThrough]
	[Serializable]
	public class ConversationActionType
	{
		public ConversationActionTypeType Action
		{
			get
			{
				return this.actionField;
			}
			set
			{
				this.actionField = value;
			}
		}

		public ItemIdType ConversationId
		{
			get
			{
				return this.conversationIdField;
			}
			set
			{
				this.conversationIdField = value;
			}
		}

		public TargetFolderIdType ContextFolderId
		{
			get
			{
				return this.contextFolderIdField;
			}
			set
			{
				this.contextFolderIdField = value;
			}
		}

		public DateTime ConversationLastSyncTime
		{
			get
			{
				return this.conversationLastSyncTimeField;
			}
			set
			{
				this.conversationLastSyncTimeField = value;
			}
		}

		[XmlIgnore]
		public bool ConversationLastSyncTimeSpecified
		{
			get
			{
				return this.conversationLastSyncTimeFieldSpecified;
			}
			set
			{
				this.conversationLastSyncTimeFieldSpecified = value;
			}
		}

		public bool ProcessRightAway
		{
			get
			{
				return this.processRightAwayField;
			}
			set
			{
				this.processRightAwayField = value;
			}
		}

		[XmlIgnore]
		public bool ProcessRightAwaySpecified
		{
			get
			{
				return this.processRightAwayFieldSpecified;
			}
			set
			{
				this.processRightAwayFieldSpecified = value;
			}
		}

		public TargetFolderIdType DestinationFolderId
		{
			get
			{
				return this.destinationFolderIdField;
			}
			set
			{
				this.destinationFolderIdField = value;
			}
		}

		[XmlArrayItem("String", IsNullable = false)]
		public string[] Categories
		{
			get
			{
				return this.categoriesField;
			}
			set
			{
				this.categoriesField = value;
			}
		}

		public bool EnableAlwaysDelete
		{
			get
			{
				return this.enableAlwaysDeleteField;
			}
			set
			{
				this.enableAlwaysDeleteField = value;
			}
		}

		[XmlIgnore]
		public bool EnableAlwaysDeleteSpecified
		{
			get
			{
				return this.enableAlwaysDeleteFieldSpecified;
			}
			set
			{
				this.enableAlwaysDeleteFieldSpecified = value;
			}
		}

		public bool IsRead
		{
			get
			{
				return this.isReadField;
			}
			set
			{
				this.isReadField = value;
			}
		}

		[XmlIgnore]
		public bool IsReadSpecified
		{
			get
			{
				return this.isReadFieldSpecified;
			}
			set
			{
				this.isReadFieldSpecified = value;
			}
		}

		public DisposalType DeleteType
		{
			get
			{
				return this.deleteTypeField;
			}
			set
			{
				this.deleteTypeField = value;
			}
		}

		[XmlIgnore]
		public bool DeleteTypeSpecified
		{
			get
			{
				return this.deleteTypeFieldSpecified;
			}
			set
			{
				this.deleteTypeFieldSpecified = value;
			}
		}

		public RetentionType RetentionPolicyType
		{
			get
			{
				return this.retentionPolicyTypeField;
			}
			set
			{
				this.retentionPolicyTypeField = value;
			}
		}

		[XmlIgnore]
		public bool RetentionPolicyTypeSpecified
		{
			get
			{
				return this.retentionPolicyTypeFieldSpecified;
			}
			set
			{
				this.retentionPolicyTypeFieldSpecified = value;
			}
		}

		public string RetentionPolicyTagId
		{
			get
			{
				return this.retentionPolicyTagIdField;
			}
			set
			{
				this.retentionPolicyTagIdField = value;
			}
		}

		public FlagType Flag
		{
			get
			{
				return this.flagField;
			}
			set
			{
				this.flagField = value;
			}
		}

		public bool SuppressReadReceipts
		{
			get
			{
				return this.suppressReadReceiptsField;
			}
			set
			{
				this.suppressReadReceiptsField = value;
			}
		}

		[XmlIgnore]
		public bool SuppressReadReceiptsSpecified
		{
			get
			{
				return this.suppressReadReceiptsFieldSpecified;
			}
			set
			{
				this.suppressReadReceiptsFieldSpecified = value;
			}
		}

		private ConversationActionTypeType actionField;

		private ItemIdType conversationIdField;

		private TargetFolderIdType contextFolderIdField;

		private DateTime conversationLastSyncTimeField;

		private bool conversationLastSyncTimeFieldSpecified;

		private bool processRightAwayField;

		private bool processRightAwayFieldSpecified;

		private TargetFolderIdType destinationFolderIdField;

		private string[] categoriesField;

		private bool enableAlwaysDeleteField;

		private bool enableAlwaysDeleteFieldSpecified;

		private bool isReadField;

		private bool isReadFieldSpecified;

		private DisposalType deleteTypeField;

		private bool deleteTypeFieldSpecified;

		private RetentionType retentionPolicyTypeField;

		private bool retentionPolicyTypeFieldSpecified;

		private string retentionPolicyTagIdField;

		private FlagType flagField;

		private bool suppressReadReceiptsField;

		private bool suppressReadReceiptsFieldSpecified;
	}
}
