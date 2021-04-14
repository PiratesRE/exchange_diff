using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[Serializable]
	public class PropagationTaskValue
	{
		public XmlElement Parameter
		{
			get
			{
				return this.parameterField;
			}
			set
			{
				this.parameterField = value;
			}
		}

		[XmlAttribute]
		public string TaskId
		{
			get
			{
				return this.taskIdField;
			}
			set
			{
				this.taskIdField = value;
			}
		}

		[XmlAttribute]
		public string TaskType
		{
			get
			{
				return this.taskTypeField;
			}
			set
			{
				this.taskTypeField = value;
			}
		}

		[XmlAttribute]
		public int Priority
		{
			get
			{
				return this.priorityField;
			}
			set
			{
				this.priorityField = value;
			}
		}

		[XmlAttribute]
		public DateTime CreationTime
		{
			get
			{
				return this.creationTimeField;
			}
			set
			{
				this.creationTimeField = value;
			}
		}

		[XmlAttribute]
		public PropagationTaskStatus Status
		{
			get
			{
				return this.statusField;
			}
			set
			{
				this.statusField = value;
			}
		}

		[XmlAttribute]
		public int RetryCount
		{
			get
			{
				return this.retryCountField;
			}
			set
			{
				this.retryCountField = value;
			}
		}

		[XmlAttribute]
		public DateTime EarliestStartTime
		{
			get
			{
				return this.earliestStartTimeField;
			}
			set
			{
				this.earliestStartTimeField = value;
			}
		}

		[XmlAttribute(DataType = "duration")]
		public string AccumulatedExecutionTime
		{
			get
			{
				return this.accumulatedExecutionTimeField;
			}
			set
			{
				this.accumulatedExecutionTimeField = value;
			}
		}

		private XmlElement parameterField;

		private string taskIdField;

		private string taskTypeField;

		private int priorityField;

		private DateTime creationTimeField;

		private PropagationTaskStatus statusField;

		private int retryCountField;

		private DateTime earliestStartTimeField;

		private string accumulatedExecutionTimeField;
	}
}
