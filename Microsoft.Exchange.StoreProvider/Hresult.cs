using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class Hresult
	{
		public static int GetScode(int hr)
		{
			return hr & 65535;
		}

		public static Hresult.Facility GetFacility(int hr)
		{
			return (Hresult.Facility)((hr & 268369920) >> 16);
		}

		internal enum Facility
		{
			NULL,
			RPC,
			DISPATCH,
			STORAGE,
			ITF,
			WIN32 = 7,
			WINDOWS,
			SECURITY,
			CONTROL,
			CERT,
			INTERNET,
			MEDIASERVER,
			MSMQ,
			SETUPAPI,
			SCARD,
			COMPLUS,
			AAF,
			URT,
			ACS,
			DPLAY,
			UMI,
			SXS,
			WINDOWS_CE,
			HTTP,
			BACKGROUNDCOPY = 32,
			CONFIGURATION,
			STATE_MANAGEMENT,
			METADIRECTORY,
			WINDOWSUPDATE,
			DIRECTORYSERVICE
		}
	}
}
