using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Flags]
	public enum GlsDomainFlags
	{
		None = 0,
		Nego2Enabled = 1,
		OAuth2ClientProfileEnabled = 2,
		Both = 3
	}
}
