using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DebuggerStepThrough]
	[Serializable]
	public class ClientIntentType
	{
		public ItemIdType ItemId;

		public int Intent;

		public int ItemVersion;

		public bool WouldRepair;

		public ClientIntentMeetingInquiryActionType PredictedAction;

		[XmlIgnore]
		public bool PredictedActionSpecified;
	}
}
