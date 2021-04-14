using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[Serializable]
	public class ApprovalRequestDataType
	{
		public bool IsUndecidedApprovalRequest
		{
			get
			{
				return this.isUndecidedApprovalRequestField;
			}
			set
			{
				this.isUndecidedApprovalRequestField = value;
			}
		}

		[XmlIgnore]
		public bool IsUndecidedApprovalRequestSpecified
		{
			get
			{
				return this.isUndecidedApprovalRequestFieldSpecified;
			}
			set
			{
				this.isUndecidedApprovalRequestFieldSpecified = value;
			}
		}

		public int ApprovalDecision
		{
			get
			{
				return this.approvalDecisionField;
			}
			set
			{
				this.approvalDecisionField = value;
			}
		}

		[XmlIgnore]
		public bool ApprovalDecisionSpecified
		{
			get
			{
				return this.approvalDecisionFieldSpecified;
			}
			set
			{
				this.approvalDecisionFieldSpecified = value;
			}
		}

		public string ApprovalDecisionMaker
		{
			get
			{
				return this.approvalDecisionMakerField;
			}
			set
			{
				this.approvalDecisionMakerField = value;
			}
		}

		public DateTime ApprovalDecisionTime
		{
			get
			{
				return this.approvalDecisionTimeField;
			}
			set
			{
				this.approvalDecisionTimeField = value;
			}
		}

		[XmlIgnore]
		public bool ApprovalDecisionTimeSpecified
		{
			get
			{
				return this.approvalDecisionTimeFieldSpecified;
			}
			set
			{
				this.approvalDecisionTimeFieldSpecified = value;
			}
		}

		private bool isUndecidedApprovalRequestField;

		private bool isUndecidedApprovalRequestFieldSpecified;

		private int approvalDecisionField;

		private bool approvalDecisionFieldSpecified;

		private string approvalDecisionMakerField;

		private DateTime approvalDecisionTimeField;

		private bool approvalDecisionTimeFieldSpecified;
	}
}
