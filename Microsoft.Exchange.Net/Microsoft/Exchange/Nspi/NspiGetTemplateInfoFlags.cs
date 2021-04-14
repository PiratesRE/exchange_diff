using System;

namespace Microsoft.Exchange.Nspi
{
	[Flags]
	public enum NspiGetTemplateInfoFlags
	{
		None = 0,
		Template = 1,
		Script = 4,
		EmailType = 16,
		HelpFileName = 32,
		HelpFile = 64
	}
}
