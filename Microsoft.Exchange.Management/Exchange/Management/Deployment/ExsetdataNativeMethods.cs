using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ExsetdataNativeMethods
	{
		[DllImport("exsetdata.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode, EntryPoint = "ScSetupAtom")]
		internal static extern uint SetupAtom(uint atomID, uint setupMode, string InstallDir, string SourceDir, string DCName, string Org, string LegacyOrg, string AdminGroup, string LegacyAdminGroup, string AdminGroupContainingRoutingGroup, string RoutingGroup, [MarshalAs(UnmanagedType.FunctionPtr)] ManagedLoggerDelegate logger);

		[DllImport("exsetdata.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal static extern uint ScGetFormattedError(uint sc, int langId, StringBuilder errorMsg, ref int maxBufferSize);
	}
}
