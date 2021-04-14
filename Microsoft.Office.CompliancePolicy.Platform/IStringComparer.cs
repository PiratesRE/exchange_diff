using System;

namespace Microsoft.Office.CompliancePolicy
{
	internal interface IStringComparer
	{
		bool Equals(string x, string y);
	}
}
