using System;

namespace Microsoft.Office.CompliancePolicy.ComplianceData
{
	[Flags]
	public enum ComplianceItemStatusFlag : uint
	{
		None = 0U,
		Preserved = 1U,
		Archived = 2U,
		Recycled = 4U
	}
}
