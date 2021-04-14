using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class ReminderItemActionType
	{
		public ReminderActionType ActionType
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

		public ItemIdType ItemId
		{
			get
			{
				return this.itemIdField;
			}
			set
			{
				this.itemIdField = value;
			}
		}

		public string NewReminderTime
		{
			get
			{
				return this.newReminderTimeField;
			}
			set
			{
				this.newReminderTimeField = value;
			}
		}

		private ReminderActionType actionTypeField;

		private ItemIdType itemIdField;

		private string newReminderTimeField;
	}
}
