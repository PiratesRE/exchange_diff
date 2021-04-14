using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class CDRDataType
	{
		public DateTime CallStartTime
		{
			get
			{
				return this.callStartTimeField;
			}
			set
			{
				this.callStartTimeField = value;
			}
		}

		public string CallType
		{
			get
			{
				return this.callTypeField;
			}
			set
			{
				this.callTypeField = value;
			}
		}

		public string CallIdentity
		{
			get
			{
				return this.callIdentityField;
			}
			set
			{
				this.callIdentityField = value;
			}
		}

		public string ParentCallIdentity
		{
			get
			{
				return this.parentCallIdentityField;
			}
			set
			{
				this.parentCallIdentityField = value;
			}
		}

		public string UMServerName
		{
			get
			{
				return this.uMServerNameField;
			}
			set
			{
				this.uMServerNameField = value;
			}
		}

		public string DialPlanGuid
		{
			get
			{
				return this.dialPlanGuidField;
			}
			set
			{
				this.dialPlanGuidField = value;
			}
		}

		public string DialPlanName
		{
			get
			{
				return this.dialPlanNameField;
			}
			set
			{
				this.dialPlanNameField = value;
			}
		}

		public int CallDuration
		{
			get
			{
				return this.callDurationField;
			}
			set
			{
				this.callDurationField = value;
			}
		}

		public string IPGatewayAddress
		{
			get
			{
				return this.iPGatewayAddressField;
			}
			set
			{
				this.iPGatewayAddressField = value;
			}
		}

		public string IPGatewayName
		{
			get
			{
				return this.iPGatewayNameField;
			}
			set
			{
				this.iPGatewayNameField = value;
			}
		}

		public string GatewayGuid
		{
			get
			{
				return this.gatewayGuidField;
			}
			set
			{
				this.gatewayGuidField = value;
			}
		}

		public string CalledPhoneNumber
		{
			get
			{
				return this.calledPhoneNumberField;
			}
			set
			{
				this.calledPhoneNumberField = value;
			}
		}

		public string CallerPhoneNumber
		{
			get
			{
				return this.callerPhoneNumberField;
			}
			set
			{
				this.callerPhoneNumberField = value;
			}
		}

		public string OfferResult
		{
			get
			{
				return this.offerResultField;
			}
			set
			{
				this.offerResultField = value;
			}
		}

		public string DropCallReason
		{
			get
			{
				return this.dropCallReasonField;
			}
			set
			{
				this.dropCallReasonField = value;
			}
		}

		public string ReasonForCall
		{
			get
			{
				return this.reasonForCallField;
			}
			set
			{
				this.reasonForCallField = value;
			}
		}

		public string TransferredNumber
		{
			get
			{
				return this.transferredNumberField;
			}
			set
			{
				this.transferredNumberField = value;
			}
		}

		public string DialedString
		{
			get
			{
				return this.dialedStringField;
			}
			set
			{
				this.dialedStringField = value;
			}
		}

		public string CallerMailboxAlias
		{
			get
			{
				return this.callerMailboxAliasField;
			}
			set
			{
				this.callerMailboxAliasField = value;
			}
		}

		public string CalleeMailboxAlias
		{
			get
			{
				return this.calleeMailboxAliasField;
			}
			set
			{
				this.calleeMailboxAliasField = value;
			}
		}

		public string CallerLegacyExchangeDN
		{
			get
			{
				return this.callerLegacyExchangeDNField;
			}
			set
			{
				this.callerLegacyExchangeDNField = value;
			}
		}

		public string CalleeLegacyExchangeDN
		{
			get
			{
				return this.calleeLegacyExchangeDNField;
			}
			set
			{
				this.calleeLegacyExchangeDNField = value;
			}
		}

		public string AutoAttendantName
		{
			get
			{
				return this.autoAttendantNameField;
			}
			set
			{
				this.autoAttendantNameField = value;
			}
		}

		public AudioQualityType AudioQualityMetrics
		{
			get
			{
				return this.audioQualityMetricsField;
			}
			set
			{
				this.audioQualityMetricsField = value;
			}
		}

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

		private DateTime callStartTimeField;

		private string callTypeField;

		private string callIdentityField;

		private string parentCallIdentityField;

		private string uMServerNameField;

		private string dialPlanGuidField;

		private string dialPlanNameField;

		private int callDurationField;

		private string iPGatewayAddressField;

		private string iPGatewayNameField;

		private string gatewayGuidField;

		private string calledPhoneNumberField;

		private string callerPhoneNumberField;

		private string offerResultField;

		private string dropCallReasonField;

		private string reasonForCallField;

		private string transferredNumberField;

		private string dialedStringField;

		private string callerMailboxAliasField;

		private string calleeMailboxAliasField;

		private string callerLegacyExchangeDNField;

		private string calleeLegacyExchangeDNField;

		private string autoAttendantNameField;

		private AudioQualityType audioQualityMetricsField;

		private DateTime creationTimeField;
	}
}
