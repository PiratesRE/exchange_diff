using System;
using System.Collections.Generic;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.MapiDisp
{
	internal interface IMapiRpc : IDisposable
	{
		Dictionary<int, MapiSession> SessionsHash { get; }

		int Initialize();

		int DoConnect(IExecutionDiagnostics executionDiagnostics, out IntPtr contextHandle, string userDn, ClientSecurityContext callerSecurityContext, byte[] sessionSecurityContext, int flags, int connectionMode, int codePageId, int localeIdString, int localeIdSort, out TimeSpan pollsMax, out int retryCount, out TimeSpan retryDelay, out string distinguishedNamePrefix, out string displayName, short[] clientVersion, ArraySegment<byte> auxIn, ref byte[] auxOut, out int sizeAuxOut, Action<int> notificationPendingCallback);

		int DoDisconnect(IExecutionDiagnostics executionDiagnostics, ref IntPtr contextHandle);

		int DoRpc(IExecutionDiagnostics executionDiagnostics, ref IntPtr contextHandle, IList<ArraySegment<byte>> ropInArray, ArraySegment<byte> ropOut, out int sizeRopOut, bool internalAccessPrivileges, ArraySegment<byte> auxIn, ArraySegment<byte> auxOut, out int sizeAuxOut, bool fakeRequest, out byte[] fakeOut);

		MapiSession SessionFromSessionId(int sessionId);

		void DeregisterSession(MapiContext context, MapiSession session);

		IEnumerable<MapiSession> GetSessionListSnapshot();
	}
}
