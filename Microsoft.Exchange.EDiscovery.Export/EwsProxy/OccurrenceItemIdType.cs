using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class OccurrenceItemIdType : BaseItemIdType
	{
		[XmlAttribute]
		public string RecurringMasterId
		{
			get
			{
				return this.recurringMasterIdField;
			}
			set
			{
				this.recurringMasterIdField = value;
			}
		}

		[XmlAttribute]
		public string ChangeKey
		{
			get
			{
				return this.changeKeyField;
			}
			set
			{
				this.changeKeyField = value;
			}
		}

		[XmlAttribute]
		public int InstanceIndex
		{
			get
			{
				return this.instanceIndexField;
			}
			set
			{
				this.instanceIndexField = value;
			}
		}

		private string recurringMasterIdField;

		private string changeKeyField;

		private int instanceIndexField;
	}
}
