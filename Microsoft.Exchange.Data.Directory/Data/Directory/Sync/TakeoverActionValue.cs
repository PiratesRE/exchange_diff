using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[GeneratedCode("svcutil", "4.0.30319.17627")]
	[Serializable]
	public class TakeoverActionValue
	{
		[XmlAttribute]
		public TakeoverActionType ActionType
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

		[XmlAttribute]
		public DateTime ActionCreationTimestamp
		{
			get
			{
				return this.actionCreationTimestampField;
			}
			set
			{
				this.actionCreationTimestampField = value;
			}
		}

		[XmlAttribute]
		public string VerifiedDomain
		{
			get
			{
				return this.verifiedDomainField;
			}
			set
			{
				this.verifiedDomainField = value;
			}
		}

		[XmlAttribute]
		public string SourceContextId
		{
			get
			{
				return this.sourceContextIdField;
			}
			set
			{
				this.sourceContextIdField = value;
			}
		}

		[XmlAttribute]
		public string TargetContextId
		{
			get
			{
				return this.targetContextIdField;
			}
			set
			{
				this.targetContextIdField = value;
			}
		}

		[XmlAttribute(DataType = "nonNegativeInteger")]
		public string EncodingVersion
		{
			get
			{
				return this.encodingVersionField;
			}
			set
			{
				this.encodingVersionField = value;
			}
		}

		[XmlAttribute(DataType = "nonNegativeInteger")]
		public string GroupingId
		{
			get
			{
				return this.groupingIdField;
			}
			set
			{
				this.groupingIdField = value;
			}
		}

		[XmlAttribute(DataType = "nonNegativeInteger")]
		public string UserCount
		{
			get
			{
				return this.userCountField;
			}
			set
			{
				this.userCountField = value;
			}
		}

		[XmlAttribute]
		public TakeoverStatus Status
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

		[XmlIgnore]
		public bool StatusSpecified
		{
			get
			{
				return this.statusFieldSpecified;
			}
			set
			{
				this.statusFieldSpecified = value;
			}
		}

		private TakeoverActionType actionTypeField;

		private DateTime actionCreationTimestampField;

		private string verifiedDomainField;

		private string sourceContextIdField;

		private string targetContextIdField;

		private string encodingVersionField;

		private string groupingIdField;

		private string userCountField;

		private TakeoverStatus statusField;

		private bool statusFieldSpecified;
	}
}
