using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class DeletedOccurrenceStateDefinitionType : BaseCalendarItemStateDefinitionType
	{
		public DateTime OccurrenceDate
		{
			get
			{
				return this.occurrenceDateField;
			}
			set
			{
				this.occurrenceDateField = value;
			}
		}

		public bool IsOccurrencePresent
		{
			get
			{
				return this.isOccurrencePresentField;
			}
			set
			{
				this.isOccurrencePresentField = value;
			}
		}

		[XmlIgnore]
		public bool IsOccurrencePresentSpecified
		{
			get
			{
				return this.isOccurrencePresentFieldSpecified;
			}
			set
			{
				this.isOccurrencePresentFieldSpecified = value;
			}
		}

		private DateTime occurrenceDateField;

		private bool isOccurrencePresentField;

		private bool isOccurrencePresentFieldSpecified;
	}
}
