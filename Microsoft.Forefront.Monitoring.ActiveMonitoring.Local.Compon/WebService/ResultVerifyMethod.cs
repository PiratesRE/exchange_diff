using System;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.WebService
{
	public enum ResultVerifyMethod
	{
		ReturnType,
		ReturnValue,
		ReturnValueRegex,
		ReturnValueContains,
		ReturnValueXml,
		ReturnValueIsNull,
		ReturnValueIsNotNull,
		ReturnValueUseValidator,
		PropertyValue,
		PropertyValueRegex,
		PropertyValueContains,
		PropertyValueXml
	}
}
