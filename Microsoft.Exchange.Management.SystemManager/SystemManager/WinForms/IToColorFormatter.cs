using System;
using System.Drawing;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface IToColorFormatter
	{
		Color Format(object colorKey);
	}
}
