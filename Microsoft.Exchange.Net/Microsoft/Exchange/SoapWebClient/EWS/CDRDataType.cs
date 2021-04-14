using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[Serializable]
	public class CDRDataType
	{
		public DateTime CallStartTime;

		public string CallType;

		public string CallIdentity;

		public string ParentCallIdentity;

		public string UMServerName;

		public string DialPlanGuid;

		public string DialPlanName;

		public int CallDuration;

		public string IPGatewayAddress;

		public string IPGatewayName;

		public string GatewayGuid;

		public string CalledPhoneNumber;

		public string CallerPhoneNumber;

		public string OfferResult;

		public string DropCallReason;

		public string ReasonForCall;

		public string TransferredNumber;

		public string DialedString;

		public string CallerMailboxAlias;

		public string CalleeMailboxAlias;

		public string CallerLegacyExchangeDN;

		public string CalleeLegacyExchangeDN;

		public string AutoAttendantName;

		public AudioQualityType AudioQualityMetrics;

		public DateTime CreationTime;
	}
}
