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
	public class ChangeHighlightsType
	{
		public bool HasLocationChanged
		{
			get
			{
				return this.hasLocationChangedField;
			}
			set
			{
				this.hasLocationChangedField = value;
			}
		}

		[XmlIgnore]
		public bool HasLocationChangedSpecified
		{
			get
			{
				return this.hasLocationChangedFieldSpecified;
			}
			set
			{
				this.hasLocationChangedFieldSpecified = value;
			}
		}

		public string Location
		{
			get
			{
				return this.locationField;
			}
			set
			{
				this.locationField = value;
			}
		}

		public bool HasStartTimeChanged
		{
			get
			{
				return this.hasStartTimeChangedField;
			}
			set
			{
				this.hasStartTimeChangedField = value;
			}
		}

		[XmlIgnore]
		public bool HasStartTimeChangedSpecified
		{
			get
			{
				return this.hasStartTimeChangedFieldSpecified;
			}
			set
			{
				this.hasStartTimeChangedFieldSpecified = value;
			}
		}

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

		public bool HasEndTimeChanged
		{
			get
			{
				return this.hasEndTimeChangedField;
			}
			set
			{
				this.hasEndTimeChangedField = value;
			}
		}

		[XmlIgnore]
		public bool HasEndTimeChangedSpecified
		{
			get
			{
				return this.hasEndTimeChangedFieldSpecified;
			}
			set
			{
				this.hasEndTimeChangedFieldSpecified = value;
			}
		}

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

		private bool hasLocationChangedField;

		private bool hasLocationChangedFieldSpecified;

		private string locationField;

		private bool hasStartTimeChangedField;

		private bool hasStartTimeChangedFieldSpecified;

		private DateTime startField;

		private bool startFieldSpecified;

		private bool hasEndTimeChangedField;

		private bool hasEndTimeChangedFieldSpecified;

		private DateTime endField;

		private bool endFieldSpecified;
	}
}
