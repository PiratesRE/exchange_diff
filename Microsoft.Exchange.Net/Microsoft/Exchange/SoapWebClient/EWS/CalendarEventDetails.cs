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
	public class CalendarEventDetails
	{
		public string ID;

		public string Subject;

		public string Location;

		public bool IsMeeting;

		public bool IsRecurring;

		public bool IsException;

		public bool IsReminderSet;

		public bool IsPrivate;
	}
}
