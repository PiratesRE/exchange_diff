﻿using System;
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
	public class ReminderMessageDataType
	{
		public string ReminderText;

		public string Location;

		public DateTime StartTime;

		[XmlIgnore]
		public bool StartTimeSpecified;

		public DateTime EndTime;

		[XmlIgnore]
		public bool EndTimeSpecified;

		public ItemIdType AssociatedCalendarItemId;
	}
}
