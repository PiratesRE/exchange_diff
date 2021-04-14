using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class GetUMCallDataRecordsType : BaseRequestType
	{
		public DateTime StartDateTime
		{
			get
			{
				return this.startDateTimeField;
			}
			set
			{
				this.startDateTimeField = value;
			}
		}

		[XmlIgnore]
		public bool StartDateTimeSpecified
		{
			get
			{
				return this.startDateTimeFieldSpecified;
			}
			set
			{
				this.startDateTimeFieldSpecified = value;
			}
		}

		public DateTime EndDateTime
		{
			get
			{
				return this.endDateTimeField;
			}
			set
			{
				this.endDateTimeField = value;
			}
		}

		[XmlIgnore]
		public bool EndDateTimeSpecified
		{
			get
			{
				return this.endDateTimeFieldSpecified;
			}
			set
			{
				this.endDateTimeFieldSpecified = value;
			}
		}

		public int Offset
		{
			get
			{
				return this.offsetField;
			}
			set
			{
				this.offsetField = value;
			}
		}

		[XmlIgnore]
		public bool OffsetSpecified
		{
			get
			{
				return this.offsetFieldSpecified;
			}
			set
			{
				this.offsetFieldSpecified = value;
			}
		}

		public int NumberOfRecords
		{
			get
			{
				return this.numberOfRecordsField;
			}
			set
			{
				this.numberOfRecordsField = value;
			}
		}

		[XmlIgnore]
		public bool NumberOfRecordsSpecified
		{
			get
			{
				return this.numberOfRecordsFieldSpecified;
			}
			set
			{
				this.numberOfRecordsFieldSpecified = value;
			}
		}

		public string UserLegacyExchangeDN
		{
			get
			{
				return this.userLegacyExchangeDNField;
			}
			set
			{
				this.userLegacyExchangeDNField = value;
			}
		}

		public UMCDRFilterByType FilterBy
		{
			get
			{
				return this.filterByField;
			}
			set
			{
				this.filterByField = value;
			}
		}

		private DateTime startDateTimeField;

		private bool startDateTimeFieldSpecified;

		private DateTime endDateTimeField;

		private bool endDateTimeFieldSpecified;

		private int offsetField;

		private bool offsetFieldSpecified;

		private int numberOfRecordsField;

		private bool numberOfRecordsFieldSpecified;

		private string userLegacyExchangeDNField;

		private UMCDRFilterByType filterByField;
	}
}
