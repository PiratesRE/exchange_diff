using System;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("ExecuteDiagnosticMethodRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class ExecuteDiagnosticMethodRequest : BaseRequest
	{
		[XmlElement("Verb")]
		public string Verb { get; set; }

		[XmlElement]
		public XmlNode Parameter { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new ExecuteDiagnosticMethod(callContext, this);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			return null;
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return null;
		}
	}
}
