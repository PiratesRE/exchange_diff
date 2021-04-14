using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class ReminderType
	{
		public string Subject;

		public string Location;

		public DateTime ReminderTime;

		public DateTime StartDate;

		public DateTime EndDate;

		public ItemIdType ItemId;

		public ItemIdType RecurringMasterItemId;

		public ReminderGroupType ReminderGroup;

		[XmlIgnore]
		public bool ReminderGroupSpecified;

		public string UID;
	}
}
