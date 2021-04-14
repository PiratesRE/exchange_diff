using System;

namespace Microsoft.Exchange.TextProcessing.Boomerang
{
	internal class Constants
	{
		internal const int BoomerangRandomDataLength = 5;

		internal const int BoomerangMacSize = 5;

		internal const int BoomerangRecipientHintSize = 1;

		internal const int BoomerangDateHintSize = 1;

		internal const int BoomerangVersionLength = 1;

		internal const string BoomerangVersion = "0";

		internal const int BoomerangDefaultValidIntervalsConfig = 30;

		internal const string XMSExchangeOrganizationValidBoomerang = "X-MS-Exchange-Organization-Valid-Boomerang";

		internal static readonly byte[] BoomerangCodeKey = new byte[]
		{
			93,
			34,
			225,
			50,
			124,
			245,
			72,
			28,
			170,
			204,
			9,
			245,
			217,
			28,
			0,
			45
		};
	}
}
