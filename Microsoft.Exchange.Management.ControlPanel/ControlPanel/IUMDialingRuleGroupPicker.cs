using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[ServiceContract(Namespace = "ECP", Name = "UMDialingRuleGroupPicker")]
	public interface IUMDialingRuleGroupPicker : IGetListService<UMDialPlanFilterWithIdentity, UMDialingRuleGroupRow>
	{
	}
}
