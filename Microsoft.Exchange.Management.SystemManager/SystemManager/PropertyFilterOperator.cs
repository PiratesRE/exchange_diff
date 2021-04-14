using System;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager
{
	public enum PropertyFilterOperator
	{
		[LocDescription(Strings.IDs.Equal)]
		Equal,
		[LocDescription(Strings.IDs.NotEqual)]
		NotEqual,
		[LocDescription(Strings.IDs.LessThan)]
		LessThan,
		[LocDescription(Strings.IDs.LessThanOrEqual)]
		LessThanOrEqual,
		[LocDescription(Strings.IDs.GreaterThan)]
		GreaterThan,
		[LocDescription(Strings.IDs.GreaterThanOrEqual)]
		GreaterThanOrEqual,
		[LocDescription(Strings.IDs.StartsWith)]
		StartsWith,
		[LocDescription(Strings.IDs.EndsWith)]
		EndsWith,
		[LocDescription(Strings.IDs.Contains)]
		Contains,
		[LocDescription(Strings.IDs.NotContains)]
		NotContains,
		[LocDescription(Strings.IDs.Present)]
		Present,
		[LocDescription(Strings.IDs.NotPresent)]
		NotPresent
	}
}
