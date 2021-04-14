using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetRemindersType : BaseRequestType
	{
		public DateTime BeginTime;

		[XmlIgnore]
		public bool BeginTimeSpecified;

		public DateTime EndTime;

		[XmlIgnore]
		public bool EndTimeSpecified;

		public int MaxItems;

		[XmlIgnore]
		public bool MaxItemsSpecified;

		public GetRemindersTypeReminderType ReminderType;

		[XmlIgnore]
		public bool ReminderTypeSpecified;
	}
}
