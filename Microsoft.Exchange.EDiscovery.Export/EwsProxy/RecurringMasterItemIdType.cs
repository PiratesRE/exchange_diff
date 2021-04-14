using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class RecurringMasterItemIdType : BaseItemIdType
	{
		[XmlAttribute]
		public string OccurrenceId
		{
			get
			{
				return this.occurrenceIdField;
			}
			set
			{
				this.occurrenceIdField = value;
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

		private string occurrenceIdField;

		private string changeKeyField;
	}
}
