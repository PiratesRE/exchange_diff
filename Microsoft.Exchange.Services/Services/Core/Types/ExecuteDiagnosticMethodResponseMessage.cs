using System;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("ExecuteDiagnosticMethodResponseMessage", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ExecuteDiagnosticMethodResponseMessage : ResponseMessage
	{
		public ExecuteDiagnosticMethodResponseMessage()
		{
		}

		internal ExecuteDiagnosticMethodResponseMessage(ServiceResultCode code, ServiceError error, XmlNode returnValue) : base(code, error)
		{
			this.ReturnValue = returnValue;
		}

		[XmlElement]
		public XmlNode ReturnValue { get; set; }
	}
}
