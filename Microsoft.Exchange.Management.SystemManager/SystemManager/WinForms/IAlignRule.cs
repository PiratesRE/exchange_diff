using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface IAlignRule
	{
		void Apply(AlignUnitsCollection units);
	}
}
