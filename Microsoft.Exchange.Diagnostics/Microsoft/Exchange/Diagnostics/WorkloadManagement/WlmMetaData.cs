using System;

namespace Microsoft.Exchange.Diagnostics.WorkloadManagement
{
	internal enum WlmMetaData
	{
		[DisplayName("WLM.Cl")]
		WorkloadClassification,
		[DisplayName("WLM.Type")]
		WorkloadType,
		[DisplayName("WLM.Int")]
		IsInteractive,
		[DisplayName("WLM.SvcA")]
		IsServiceAccount,
		[DisplayName("WLM.Bal")]
		BudgetBalance,
		[DisplayName("WLM.TS")]
		TimeOnServer,
		[DisplayName("WLM.OBPolPart")]
		OverBudgetPolicyPart,
		[DisplayName("WLM.OBPolVal")]
		OverBudgetPolicyValue,
		[DisplayName("WLM.BT")]
		BudgetType
	}
}
