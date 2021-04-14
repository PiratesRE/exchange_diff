using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class SetHoldOnMailboxesType : BaseRequestType
	{
		public HoldActionType ActionType;

		public string HoldId;

		public string Query;

		[XmlArrayItem("String", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public string[] Mailboxes;

		public string Language;

		public bool IncludeNonIndexableItems;

		[XmlIgnore]
		public bool IncludeNonIndexableItemsSpecified;

		public bool Deduplication;

		[XmlIgnore]
		public bool DeduplicationSpecified;

		public string InPlaceHoldIdentity;

		public string ItemHoldPeriod;
	}
}
