﻿using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DesignerCategory("code")]
	[Serializable]
	public class GetRemindersResponseMessageType : ResponseMessageType
	{
		[XmlArrayItem("Reminder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		public ReminderType[] Reminders
		{
			get
			{
				return this.remindersField;
			}
			set
			{
				this.remindersField = value;
			}
		}

		private ReminderType[] remindersField;
	}
}