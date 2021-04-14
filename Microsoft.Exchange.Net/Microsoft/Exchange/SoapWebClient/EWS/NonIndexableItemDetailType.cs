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
	public class NonIndexableItemDetailType
	{
		public ItemIdType ItemId;

		public ItemIndexErrorType ErrorCode;

		public string ErrorDescription;

		public bool IsPartiallyIndexed;

		public bool IsPermanentFailure;

		public string SortValue;

		public int AttemptCount;

		public DateTime LastAttemptTime;

		[XmlIgnore]
		public bool LastAttemptTimeSpecified;

		public string AdditionalInfo;
	}
}
