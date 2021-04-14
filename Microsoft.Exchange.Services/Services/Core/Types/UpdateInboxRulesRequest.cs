using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("UpdateInboxRulesRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class UpdateInboxRulesRequest : InboxRulesRequest
	{
		[XmlElement(Order = 1)]
		public bool RemoveOutlookRuleBlob { get; set; }

		[XmlIgnore]
		public bool RemoveOutlookRuleBlobSpecified { get; set; }

		[XmlArray(Order = 2)]
		[XmlArrayItem("SetRuleOperation", Type = typeof(SetRuleOperation), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArrayItem("DeleteRuleOperation", Type = typeof(DeleteRuleOperation), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArrayItem("CreateRuleOperation", Type = typeof(CreateRuleOperation), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public RuleOperation[] Operations { get; set; }

		public UpdateInboxRulesRequest() : base(ExTraceGlobals.UpdateInboxRulesCallTracer, true)
		{
		}

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new UpdateInboxRules(callContext, this);
		}
	}
}
