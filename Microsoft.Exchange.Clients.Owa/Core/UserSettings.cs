using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Flags]
	public enum UserSettings : uint
	{
		Mail = 1U,
		Spelling = 2U,
		Calendar = 4U,
		General = 8U,
		Regional = 16U,
		Language = 32U
	}
}
