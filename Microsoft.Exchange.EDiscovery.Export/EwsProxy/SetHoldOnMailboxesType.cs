using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DebuggerStepThrough]
	[Serializable]
	public class SetHoldOnMailboxesType : BaseRequestType
	{
		public HoldActionType ActionType
		{
			get
			{
				return this.actionTypeField;
			}
			set
			{
				this.actionTypeField = value;
			}
		}

		public string HoldId
		{
			get
			{
				return this.holdIdField;
			}
			set
			{
				this.holdIdField = value;
			}
		}

		public string Query
		{
			get
			{
				return this.queryField;
			}
			set
			{
				this.queryField = value;
			}
		}

		[XmlArrayItem("String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] Mailboxes
		{
			get
			{
				return this.mailboxesField;
			}
			set
			{
				this.mailboxesField = value;
			}
		}

		public string Language
		{
			get
			{
				return this.languageField;
			}
			set
			{
				this.languageField = value;
			}
		}

		public bool IncludeNonIndexableItems
		{
			get
			{
				return this.includeNonIndexableItemsField;
			}
			set
			{
				this.includeNonIndexableItemsField = value;
			}
		}

		[XmlIgnore]
		public bool IncludeNonIndexableItemsSpecified
		{
			get
			{
				return this.includeNonIndexableItemsFieldSpecified;
			}
			set
			{
				this.includeNonIndexableItemsFieldSpecified = value;
			}
		}

		public bool Deduplication
		{
			get
			{
				return this.deduplicationField;
			}
			set
			{
				this.deduplicationField = value;
			}
		}

		[XmlIgnore]
		public bool DeduplicationSpecified
		{
			get
			{
				return this.deduplicationFieldSpecified;
			}
			set
			{
				this.deduplicationFieldSpecified = value;
			}
		}

		public string InPlaceHoldIdentity
		{
			get
			{
				return this.inPlaceHoldIdentityField;
			}
			set
			{
				this.inPlaceHoldIdentityField = value;
			}
		}

		public string ItemHoldPeriod
		{
			get
			{
				return this.itemHoldPeriodField;
			}
			set
			{
				this.itemHoldPeriodField = value;
			}
		}

		private HoldActionType actionTypeField;

		private string holdIdField;

		private string queryField;

		private string[] mailboxesField;

		private string languageField;

		private bool includeNonIndexableItemsField;

		private bool includeNonIndexableItemsFieldSpecified;

		private bool deduplicationField;

		private bool deduplicationFieldSpecified;

		private string inPlaceHoldIdentityField;

		private string itemHoldPeriodField;
	}
}
