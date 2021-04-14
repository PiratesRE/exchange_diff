using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("GetInboxRulesRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class GetInboxRulesRequest : InboxRulesRequest
	{
		public GetInboxRulesRequest() : base(ExTraceGlobals.GetInboxRulesCallTracer, false)
		{
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new GetInboxRules(callContext, this);
		}
	}
}
