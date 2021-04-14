using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "SetHoldOnMailboxesType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Name = "SetHoldOnMailboxesRequest", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class SetHoldOnMailboxesRequest : BaseRequest
	{
		[XmlElement("ActionType")]
		[IgnoreDataMember]
		public HoldAction ActionType
		{
			get
			{
				return this.actionType;
			}
			set
			{
				this.actionType = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "ActionType", IsRequired = true)]
		public string ActionTypeString
		{
			get
			{
				return EnumUtilities.ToString<HoldAction>(this.actionType);
			}
			set
			{
				this.actionType = EnumUtilities.Parse<HoldAction>(value);
			}
		}

		[XmlElement("HoldId")]
		[DataMember(Name = "HoldId", IsRequired = true)]
		public string HoldId
		{
			get
			{
				return this.holdId;
			}
			set
			{
				this.holdId = value;
			}
		}

		[XmlElement("Query")]
		[DataMember(Name = "Query", IsRequired = true)]
		public string Query
		{
			get
			{
				return this.query;
			}
			set
			{
				this.query = value;
			}
		}

		[XmlArray(ElementName = "Mailboxes", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[XmlArrayItem(ElementName = "String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(string))]
		[DataMember(Name = "Mailboxes", IsRequired = true)]
		public string[] Mailboxes
		{
			get
			{
				return this.mailboxes;
			}
			set
			{
				this.mailboxes = value;
			}
		}

		[XmlElement("Language")]
		[DataMember(Name = "Language", IsRequired = false)]
		public string Language
		{
			get
			{
				return this.language;
			}
			set
			{
				this.language = value;
			}
		}

		[DataMember(Name = "IncludeNonIndexableItems", IsRequired = false)]
		[XmlElement("IncludeNonIndexableItems")]
		public bool IncludeNonIndexableItems
		{
			get
			{
				return this.includeNonIndexableItems;
			}
			set
			{
				this.includeNonIndexableItems = value;
			}
		}

		[DataMember(Name = "Deduplication", IsRequired = false)]
		[XmlElement("Deduplication")]
		public bool Deduplication
		{
			get
			{
				return this.deduplication;
			}
			set
			{
				this.deduplication = value;
			}
		}

		[DataMember(Name = "InPlaceHoldIdentity", IsRequired = false)]
		[XmlElement("InPlaceHoldIdentity")]
		public string InPlaceHoldIdentity
		{
			get
			{
				return this.inPlaceHoldIdentity;
			}
			set
			{
				this.inPlaceHoldIdentity = value;
			}
		}

		[DataMember(Name = "ItemHoldPeriod", IsRequired = false)]
		[XmlElement("ItemHoldPeriod")]
		public string ItemHoldPeriod
		{
			get
			{
				return this.itemHoldPeriod;
			}
			set
			{
				this.itemHoldPeriod = value;
			}
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new SetHoldOnMailboxes(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int currentStep)
		{
			return null;
		}

		private HoldAction actionType;

		private string holdId;

		private string query;

		private string[] mailboxes;

		private string language;

		private bool includeNonIndexableItems;

		private bool deduplication;

		private string inPlaceHoldIdentity;

		private string itemHoldPeriod;
	}
}
