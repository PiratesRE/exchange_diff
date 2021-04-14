using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[Serializable]
	public class FlagType
	{
		public FlagStatusType FlagStatus
		{
			get
			{
				return this.flagStatusField;
			}
			set
			{
				this.flagStatusField = value;
			}
		}

		public DateTime StartDate
		{
			get
			{
				return this.startDateField;
			}
			set
			{
				this.startDateField = value;
			}
		}

		[XmlIgnore]
		public bool StartDateSpecified
		{
			get
			{
				return this.startDateFieldSpecified;
			}
			set
			{
				this.startDateFieldSpecified = value;
			}
		}

		public DateTime DueDate
		{
			get
			{
				return this.dueDateField;
			}
			set
			{
				this.dueDateField = value;
			}
		}

		[XmlIgnore]
		public bool DueDateSpecified
		{
			get
			{
				return this.dueDateFieldSpecified;
			}
			set
			{
				this.dueDateFieldSpecified = value;
			}
		}

		public DateTime CompleteDate
		{
			get
			{
				return this.completeDateField;
			}
			set
			{
				this.completeDateField = value;
			}
		}

		[XmlIgnore]
		public bool CompleteDateSpecified
		{
			get
			{
				return this.completeDateFieldSpecified;
			}
			set
			{
				this.completeDateFieldSpecified = value;
			}
		}

		private FlagStatusType flagStatusField;

		private DateTime startDateField;

		private bool startDateFieldSpecified;

		private DateTime dueDateField;

		private bool dueDateFieldSpecified;

		private DateTime completeDateField;

		private bool completeDateFieldSpecified;
	}
}
