using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[Serializable]
	public class OccurrencesRangeType
	{
		[XmlAttribute]
		public DateTime Start
		{
			get
			{
				return this.startField;
			}
			set
			{
				this.startField = value;
			}
		}

		[XmlIgnore]
		public bool StartSpecified
		{
			get
			{
				return this.startFieldSpecified;
			}
			set
			{
				this.startFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public DateTime End
		{
			get
			{
				return this.endField;
			}
			set
			{
				this.endField = value;
			}
		}

		[XmlIgnore]
		public bool EndSpecified
		{
			get
			{
				return this.endFieldSpecified;
			}
			set
			{
				this.endFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public int Count
		{
			get
			{
				return this.countField;
			}
			set
			{
				this.countField = value;
			}
		}

		[XmlIgnore]
		public bool CountSpecified
		{
			get
			{
				return this.countFieldSpecified;
			}
			set
			{
				this.countFieldSpecified = value;
			}
		}

		[XmlAttribute]
		public bool CompareOriginalStartTime
		{
			get
			{
				return this.compareOriginalStartTimeField;
			}
			set
			{
				this.compareOriginalStartTimeField = value;
			}
		}

		[XmlIgnore]
		public bool CompareOriginalStartTimeSpecified
		{
			get
			{
				return this.compareOriginalStartTimeFieldSpecified;
			}
			set
			{
				this.compareOriginalStartTimeFieldSpecified = value;
			}
		}

		private DateTime startField;

		private bool startFieldSpecified;

		private DateTime endField;

		private bool endFieldSpecified;

		private int countField;

		private bool countFieldSpecified;

		private bool compareOriginalStartTimeField;

		private bool compareOriginalStartTimeFieldSpecified;
	}
}
