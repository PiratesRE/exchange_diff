using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public sealed class EasConstants
	{
		internal static int MaxWorkerThreadsPerProc
		{
			get
			{
				return 100;
			}
		}

		internal const string UserAgentString = "MRS-EASConnection-UserAgent";

		internal const EasProtocolVersion DefaultEasProtocolVersion = EasProtocolVersion.Version140;
	}
}
