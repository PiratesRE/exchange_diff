using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[Serializable]
	public class ContextMoveStatusValue
	{
		[XmlAttribute]
		public ContextMoveStage Stage
		{
			get
			{
				return this.stageField;
			}
			set
			{
				this.stageField = value;
			}
		}

		[XmlAttribute(DataType = "nonNegativeInteger")]
		public string OtherEpoch
		{
			get
			{
				return this.otherEpochField;
			}
			set
			{
				this.otherEpochField = value;
			}
		}

		[XmlAttribute(DataType = "positiveInteger")]
		public string OtherPartitionId
		{
			get
			{
				return this.otherPartitionIdField;
			}
			set
			{
				this.otherPartitionIdField = value;
			}
		}

		[XmlAttribute]
		public DateTime StartTimestamp
		{
			get
			{
				return this.startTimestampField;
			}
			set
			{
				this.startTimestampField = value;
			}
		}

		[XmlAttribute]
		public DateTime EndTimestamp
		{
			get
			{
				return this.endTimestampField;
			}
			set
			{
				this.endTimestampField = value;
			}
		}

		[XmlIgnore]
		public bool EndTimestampSpecified
		{
			get
			{
				return this.endTimestampFieldSpecified;
			}
			set
			{
				this.endTimestampFieldSpecified = value;
			}
		}

		private ContextMoveStage stageField;

		private string otherEpochField;

		private string otherPartitionIdField;

		private DateTime startTimestampField;

		private DateTime endTimestampField;

		private bool endTimestampFieldSpecified;
	}
}
