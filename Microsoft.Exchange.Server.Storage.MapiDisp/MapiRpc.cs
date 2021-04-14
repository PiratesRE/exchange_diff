using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.MapiDisp;
using Microsoft.Exchange.Protocols.MAPI;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess;
using Microsoft.Exchange.RpcClientAccess.Monitoring;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;
using Microsoft.Exchange.Server.Storage.DirectoryServices;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.MapiDisp
{
	internal sealed class MapiRpc : DisposableBase, IMapiRpc, IDisposable
	{
		internal static IMapiRpc Instance
		{
			get
			{
				return MapiRpc.instance;
			}
		}

		internal static byte[] DefaultConnectAuxiliaryBuffer
		{
			get
			{
				if (MapiRpc.defaultConnectAuxiliaryBuffer == null)
				{
					MapiRpc.defaultConnectAuxiliaryBuffer = MapiRpc.ComputeStaticConnectAuxiliaryOutBuffer();
				}
				return MapiRpc.defaultConnectAuxiliaryBuffer;
			}
		}

		public Dictionary<int, MapiSession> SessionsHash
		{
			get
			{
				return this.sessionsHash;
			}
		}

		internal static void Initialize(IMapiRpc instance)
		{
			MapiRpc.instance = instance;
			RopLogon.AuthenticationContextCompression = new MapiRpc.Decompressor();
		}

		internal static void Terminate()
		{
			if (MapiRpc.instance != null)
			{
				MapiRpc.instance.Dispose();
				MapiRpc.instance = null;
			}
			RopLogon.AuthenticationContextCompression = null;
		}

		public MapiSession SessionFromSessionId(int sessionId)
		{
			MapiSession result;
			using (LockManager.Lock(this.sessionsHash))
			{
				MapiSession mapiSession;
				if (this.sessionsHash.TryGetValue(sessionId, out mapiSession) && mapiSession.IsDisposed)
				{
					mapiSession = null;
				}
				result = mapiSession;
			}
			return result;
		}

		public IEnumerable<MapiSession> GetSessionListSnapshot()
		{
			IEnumerable<MapiSession> result;
			using (LockManager.Lock(this.sessionsHash))
			{
				MapiSession[] array = new MapiSession[this.sessionsHash.Count];
				this.sessionsHash.Values.CopyTo(array, 0);
				result = array;
			}
			return result;
		}

		public int Initialize()
		{
			int num = Interlocked.Increment(ref MapiRpc.instanceCount);
			if (num != 1)
			{
				Interlocked.Decrement(ref MapiRpc.instanceCount);
				throw new NotSupportedException("Only one MapiRpc instance is supported per process!");
			}
			this.isInitialized = true;
			return (int)ErrorCode.NoError;
		}

		public int DoConnect(IExecutionDiagnostics executionDiagnostics, out IntPtr contextHandle, string userDn, ClientSecurityContext callerSecurityContext, byte[] sessionSecurityContext, int flags, int connectionMode, int codePageId, int localeIdString, int localeIdSort, out TimeSpan pollsMax, out int retryCount, out TimeSpan retryDelay, out string distinguishedNamePrefix, out string displayName, short[] clientVersion, ArraySegment<byte> auxIn, ref byte[] auxOut, out int sizeAuxOut, Action<int> notificationPendingCallback)
		{
			contextHandle = IntPtr.Zero;
			distinguishedNamePrefix = null;
			displayName = null;
			pollsMax = MapiRpc.PollsMaxDefault;
			retryCount = 60;
			retryDelay = MapiRpc.RetryDelayDefault;
			IntPtr localContextHandle = contextHandle;
			string localDistinguishedNamePrefix = distinguishedNamePrefix;
			string localDisplayName = displayName;
			ErrorCode errorCode = MapiRpc.Execute(executionDiagnostics, null, "DoConnect", true, ref localContextHandle, false, userDn, null, 10, auxIn, new ArraySegment<byte>(auxOut), out sizeAuxOut, delegate(MapiContext operationContext, ref MapiSession session, ref bool deregisterSession, AuxiliaryData auxiliaryData)
			{
				bool flag = false;
				ClientSecurityContext clientSecurityContext = null;
				try
				{
					bool flag2 = (flags & 256) != 0;
					bool flag3 = (flags & 1024) != 0;
					bool flag4 = (flags & 1) != 0;
					if (flag3 && flag4)
					{
						throw new ExExceptionInvalidParameter((LID)53968U, "May not mix transport and admin privs");
					}
					if ((flag2 || flag3 || flag4) && callerSecurityContext.UserSid.IsWellKnown(WellKnownSidType.NetworkServiceSid))
					{
						clientSecurityContext = Microsoft.Exchange.Server.Storage.StoreCommonServices.Globals.ProcessSecurityContext.Clone();
					}
					else if (sessionSecurityContext != null && callerSecurityContext.UserSid.IsWellKnown(WellKnownSidType.LocalSystemSid))
					{
						clientSecurityContext = ClientSecurityContextFactory.Create(operationContext, sessionSecurityContext);
						if (clientSecurityContext == null)
						{
							throw new ExExceptionAccessDenied((LID)51128U, "MapiRpc:DoConnect: failed to build client security context from session security context.");
						}
					}
					else
					{
						clientSecurityContext = callerSecurityContext.Clone();
					}
					if (!CultureHelper.IsValidLcid(operationContext, localeIdString))
					{
						DiagnosticContext.TraceDword((LID)44112U, (uint)localeIdString);
						throw new ExExceptionInvalidParameter((LID)56400U, "MapiRpc:DoConnect: invalid localeIdString.");
					}
					if (!CultureHelper.IsValidLcid(operationContext, localeIdSort))
					{
						DiagnosticContext.TraceDword((LID)60496U, (uint)localeIdSort);
						throw new ExExceptionInvalidParameter((LID)41040U, "MapiRpc:DoConnect: invalid localeIdSort.");
					}
					operationContext.Configure(clientSecurityContext, ClientType.User, localeIdString);
					if (!callerSecurityContext.IsAuthenticated)
					{
						throw new ExExceptionAccessDenied((LID)50744U, "MapiRpc:DoConnect: unauthenticated sessions are not supported.");
					}
					if (string.IsNullOrEmpty(userDn))
					{
						throw new ExExceptionAccessDenied((LID)47672U, "MapiRpc:DoConnect: invalid ConnectAs string.");
					}
					if (clientVersion == null || clientVersion.Length != 4)
					{
						throw new ExExceptionInvalidParameter((LID)44600U, "MapiRpc:DoConnect: The length of the passed in version array(s) is invalid.");
					}
					MapiVersion clientVersion2;
					try
					{
						clientVersion2 = new MapiVersion((ushort)clientVersion[0], (ushort)clientVersion[1], (ushort)clientVersion[2], (ushort)clientVersion[3]);
					}
					catch (ArgumentOutOfRangeException ex)
					{
						operationContext.OnExceptionCatch(ex);
						throw new ExExceptionInvalidParameter((LID)39480U, "MapiRpc:DoConnect: client version values are invalid.", ex);
					}
					if (!flag2 && !flag3 && !flag4)
					{
						throw new ExExceptionAccessDenied((LID)64592U, "Access denied while creating a new session.");
					}
					if ((flags & -1282) != 0)
					{
						throw new ExExceptionLogonFailed((LID)60984U, "The client requesting the connection passed in unsupported flags.");
					}
					if (!CultureHelper.IsValidCodePage(operationContext, codePageId))
					{
						DiagnosticContext.TraceDword((LID)57424U, (uint)codePageId);
						throw new ExExceptionInvalidParameter((LID)40016U, "MapiRpc:DoConnect: invalid code page ID.");
					}
					string text = null;
					string text2 = null;
					ClientMode clientMode = ClientMode.Unknown;
					for (int i = 0; i < auxiliaryData.Input.Count; i++)
					{
						AuxiliaryBlock auxiliaryBlock = auxiliaryData.Input[i];
						PerfClientInfoAuxiliaryBlock perfClientInfoAuxiliaryBlock = auxiliaryBlock as PerfClientInfoAuxiliaryBlock;
						if (perfClientInfoAuxiliaryBlock != null)
						{
							text = perfClientInfoAuxiliaryBlock.MachineName;
							clientMode = perfClientInfoAuxiliaryBlock.ClientMode;
						}
						else
						{
							PerfProcessInfoAuxiliaryBlock perfProcessInfoAuxiliaryBlock = auxiliaryBlock as PerfProcessInfoAuxiliaryBlock;
							if (perfProcessInfoAuxiliaryBlock != null)
							{
								text2 = perfProcessInfoAuxiliaryBlock.ProcessName;
							}
						}
					}
					if (ExTraceGlobals.RpcDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder = new StringBuilder(100);
						stringBuilder.Append("INPUT  DoConnect: userDn:[");
						stringBuilder.Append(userDn);
						stringBuilder.Append("] flags:[");
						stringBuilder.Append(flags);
						stringBuilder.Append("] codePageId:[");
						stringBuilder.Append(codePageId);
						stringBuilder.Append("] LocaleIdString:[");
						stringBuilder.Append(localeIdString);
						stringBuilder.Append("] LocaleIdSort:[");
						stringBuilder.Append(localeIdSort);
						stringBuilder.Append("] clientVersion:[");
						stringBuilder.Append(clientVersion2.ToString());
						stringBuilder.Append("] clientMachineName:[");
						stringBuilder.Append(text);
						stringBuilder.Append("] clientMode:[");
						stringBuilder.Append(clientMode.ToString());
						stringBuilder.Append("] clientProcessName:[");
						stringBuilder.Append(text2);
						stringBuilder.Append("]");
						ExTraceGlobals.RpcDetailTracer.TraceDebug(0L, stringBuilder.ToString());
					}
					ServerInfo serverInfo = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetServerInfo(operationContext);
					localDistinguishedNamePrefix = string.Empty;
					localDisplayName = string.Format("Microsoft Exchange Information Store Worker, Process Id:{0}", DiagnosticsNativeMethods.GetCurrentProcessId().ToString());
					string userDn2;
					if (string.Compare(serverInfo.ExchangeLegacyDN, userDn, StringComparison.OrdinalIgnoreCase) == 0)
					{
						if (ExTraceGlobals.RpcDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							ExTraceGlobals.RpcDetailTracer.TraceDebug(0L, "Connecting to the current Server legDN");
						}
						userDn2 = string.Empty;
					}
					else
					{
						userDn2 = userDn;
					}
					SecurityDescriptor ntsecurityDescriptor = Microsoft.Exchange.Server.Storage.DirectoryServices.Globals.Directory.GetServerInfo(operationContext).NTSecurityDescriptor;
					if (!MapiSession.CheckCreateSessionRightsOnConnect(ntsecurityDescriptor, clientSecurityContext, flag2, flag3, flag4))
					{
						throw new ExExceptionLogonFailed((LID)46648U, "Access denied while creating a new session.");
					}
					deregisterSession = true;
					session = new MapiSession();
					session.LockSession(true);
					session.ConfigureMapiSession(userDn2, ref clientSecurityContext, (CodePage)codePageId, localeIdString, localeIdSort, true, new Microsoft.Exchange.Protocols.MAPI.Version(clientVersion2.Value), text, clientMode, text2, flag2, flag3, flag4, MapiRpc.connectionHandlerFactory, MapiRpc.ropDriverFactory, notificationPendingCallback);
					localContextHandle = new IntPtr(this.RegisterSession(session));
					MapiRpc.ConfigureRopExecutionCallbacks(session);
					session.MapiExMonLogger = new MapiExMonLogger(false, session.RpcContext, session.UserDN, string.Empty, clientVersion2, string.Empty);
					auxiliaryData.AppendOutput(MapiRpc.DefaultConnectAuxiliaryBlocks);
					deregisterSession = false;
					flag = true;
				}
				catch (AuthzException ex2)
				{
					operationContext.OnExceptionCatch(ex2);
					throw new ExExceptionAccessDenied((LID)38984U, "Failed to create security context from authz context. See inner exception for details.", ex2);
				}
				finally
				{
					if (clientSecurityContext != null)
					{
						clientSecurityContext.Dispose();
					}
					if (!flag)
					{
						auxiliaryData.AppendOutput(new MapiEndpointAuxiliaryBlock(MapiEndpointProcessType.ManagedStore, null));
					}
					if (ExTraceGlobals.RpcDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder2 = new StringBuilder(100);
						stringBuilder2.Append("OUTPUT DoConnect: ");
						stringBuilder2.Append(flag ? "success" : "failure");
						stringBuilder2.Append(" sessionId:[");
						stringBuilder2.Append(localContextHandle.ToInt32());
						stringBuilder2.Append("] distinguishedNamePrefix:[");
						stringBuilder2.Append(localDistinguishedNamePrefix);
						stringBuilder2.Append("] displayName:[");
						stringBuilder2.Append(localDisplayName);
						stringBuilder2.Append("]");
						ExTraceGlobals.RpcDetailTracer.TraceDebug(0L, stringBuilder2.ToString());
					}
				}
				return ErrorCode.NoError;
			});
			if (errorCode == ErrorCode.NoError)
			{
				contextHandle = localContextHandle;
				distinguishedNamePrefix = localDistinguishedNamePrefix;
				displayName = localDisplayName;
			}
			return (int)errorCode;
		}

		public int DoDisconnect(IExecutionDiagnostics executionDiagnostics, ref IntPtr contextHandle)
		{
			IntPtr localContextHandle = contextHandle;
			contextHandle = IntPtr.Zero;
			int num;
			return (int)MapiRpc.Execute(executionDiagnostics, null, "DoDisconnect", true, ref localContextHandle, false, null, null, 0, new ArraySegment<byte>(Array<byte>.Empty), new ArraySegment<byte>(Array<byte>.Empty), out num, delegate(MapiContext operationContext, ref MapiSession session, ref bool deregisterSession, AuxiliaryData auxiliaryData)
			{
				if (ExTraceGlobals.RpcDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
				{
					StringBuilder stringBuilder = new StringBuilder(100);
					stringBuilder.Append("INPUT  DoDisonnect: sessionId:[");
					stringBuilder.Append(localContextHandle);
					stringBuilder.Append("]");
					ExTraceGlobals.RpcDetailTracer.TraceDebug(0L, stringBuilder.ToString());
				}
				deregisterSession = true;
				return ErrorCode.NoError;
			});
		}

		public int DoRpc(IExecutionDiagnostics executionDiagnostics, ref IntPtr contextHandle, IList<ArraySegment<byte>> ropInArraySegments, ArraySegment<byte> ropOut, out int sizeRopOut, bool internalAccessPrivileges, ArraySegment<byte> auxIn, ArraySegment<byte> auxOut, out int sizeAuxOut, bool fakeRequest, out byte[] fakeOut)
		{
			sizeRopOut = 0;
			fakeOut = null;
			IntPtr localContextHandle = contextHandle;
			int localSizeRopOut = sizeRopOut;
			byte[] localFakeOut = fakeOut;
			MapiExecutionDiagnostics mapiExecutionDiagnostics = executionDiagnostics as MapiExecutionDiagnostics;
			mapiExecutionDiagnostics.EnablePerClientTypePerfCounterUpdate();
			int result;
			try
			{
				for (int i = 0; i < ropInArraySegments.Count; i++)
				{
					mapiExecutionDiagnostics.UpdateRpcBytesReceived(ropInArraySegments[i].Count);
				}
				ErrorCode error = MapiRpc.Execute(executionDiagnostics, null, "DoRpc", true, ref localContextHandle, false, null, ropInArraySegments, 10, auxIn, auxOut, out sizeAuxOut, delegate(MapiContext operationContext, ref MapiSession session, ref bool deregisterSession, AuxiliaryData auxiliaryData)
				{
					session.LastUsedLogonIndex = byte.MaxValue;
					mapiExecutionDiagnostics.ExtractClientActivityFromAuxiliaryData(auxiliaryData);
					session.LastAccessTime = DateTime.UtcNow;
					session.LastClientActivityId = mapiExecutionDiagnostics.ClientActivityId;
					session.LastClientProtocol = mapiExecutionDiagnostics.ClientProtocolName;
					session.LastClientComponent = mapiExecutionDiagnostics.ClientComponentName;
					session.LastClientAction = mapiExecutionDiagnostics.ClientActionString;
					if (ExTraceGlobals.RpcBufferTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						int num = 100;
						for (int j = 0; j < ropInArraySegments.Count; j++)
						{
							ArraySegment<byte> arraySegment = ropInArraySegments[j];
							num += arraySegment.Count * 2 + arraySegment.Count / 4 + 2;
						}
						StringBuilder stringBuilder = new StringBuilder(num);
						stringBuilder.Append("MapiRpc:DoRpc(): BEGIN PROCESSING RPC BUFFER. Input:[");
						bool flag = true;
						for (int k = 0; k < ropInArraySegments.Count; k++)
						{
							ArraySegment<byte> arraySegment2 = ropInArraySegments[k];
							if (!flag)
							{
								stringBuilder.Append(", ");
							}
							stringBuilder.AppendAsString(arraySegment2.Array, arraySegment2.Offset, arraySegment2.Count);
							flag = false;
						}
						stringBuilder.Append("] sessionId:[");
						stringBuilder.Append((uint)localContextHandle.ToInt32());
						stringBuilder.Append("]");
						if (mapiExecutionDiagnostics.ClientActivityId != Guid.Empty)
						{
							stringBuilder.Append(" clientActivityId:[");
							stringBuilder.Append(mapiExecutionDiagnostics.ClientActionString.ToString());
							stringBuilder.Append("]");
						}
						if (!string.IsNullOrEmpty(mapiExecutionDiagnostics.ClientProtocolName))
						{
							stringBuilder.Append(" clientProtocol:[");
							stringBuilder.Append(mapiExecutionDiagnostics.ClientProtocolName);
							stringBuilder.Append("]");
						}
						if (!string.IsNullOrEmpty(mapiExecutionDiagnostics.ClientComponentName))
						{
							stringBuilder.Append(" clientComponent:[");
							stringBuilder.Append(mapiExecutionDiagnostics.ClientComponentName);
							stringBuilder.Append("]");
						}
						if (!string.IsNullOrEmpty(mapiExecutionDiagnostics.ClientActionString))
						{
							stringBuilder.Append(" clientAction:[");
							stringBuilder.Append(mapiExecutionDiagnostics.ClientActionString);
							stringBuilder.Append("]");
						}
						ExTraceGlobals.RpcBufferTracer.TraceDebug(0L, stringBuilder.ToString());
					}
					try
					{
						if (internalAccessPrivileges)
						{
							operationContext.GrantInternalAccessPrivileges();
						}
						session.RopDriver.Execute(ropInArraySegments, ropOut, out localSizeRopOut, auxiliaryData, fakeRequest, out localFakeOut);
						mapiExecutionDiagnostics.UpdateRpcBytesSent(localSizeRopOut);
						error = ErrorCode.NoError;
					}
					finally
					{
						if (internalAccessPrivileges)
						{
							operationContext.RevokeInternalAccessPrivileges();
						}
						if (ExTraceGlobals.RpcBufferTracer.IsTraceEnabled(TraceType.DebugTrace))
						{
							StringBuilder stringBuilder2 = new StringBuilder(localSizeRopOut * 2 + localSizeRopOut / 4 + 100);
							stringBuilder2.Append("MapiRpc:DoRpc(): END PROCESSING RPC BUFFER. Output:[");
							stringBuilder2.AppendAsString(ropOut.Array, ropOut.Offset, localSizeRopOut);
							stringBuilder2.Append("] sessionId:[");
							stringBuilder2.Append((uint)localContextHandle.ToInt32());
							stringBuilder2.Append("]");
							ExTraceGlobals.RpcBufferTracer.TraceDebug(0L, stringBuilder2.ToString());
						}
					}
					return error;
				});
				contextHandle = localContextHandle;
				sizeRopOut = localSizeRopOut;
				fakeOut = localFakeOut;
				result = (int)error;
			}
			finally
			{
				mapiExecutionDiagnostics.DisablePerClientTypePerfCounterUpdate();
			}
			return result;
		}

		internal static MapiBase ObjectFromHsot(MapiSession mapiSession, uint hsot)
		{
			ServerObjectHandle handle = new ServerObjectHandle(hsot);
			IServerObject serverObject;
			ErrorCode errorCode;
			mapiSession.RopDriver.TryGetServerObject(handle.LogonIndex, handle, out serverObject, out errorCode);
			return serverObject as MapiBase;
		}

		internal static void PumpNotificationsTask(object taskContext, Func<bool> shouldCallbackContinue, uint notification, uint completionKey, int data)
		{
			MapiRpc.PumpNotificationsDaemonTask(taskContext, shouldCallbackContinue);
		}

		internal static void PumpNotificationsDaemonTask(object taskContext, Func<bool> shouldCallbackContinue)
		{
			MapiExecutionDiagnostics executionDiagnostics = new MapiExecutionDiagnostics();
			using (MapiContext mapiContext = MapiContext.Create(executionDiagnostics))
			{
				MapiRpc.pumpRoundCookie++;
				if (MapiRpc.pumpRoundCookie == 0)
				{
					MapiRpc.pumpRoundCookie++;
				}
				while (shouldCallbackContinue())
				{
					NotificationContext nextUnvisitedPendingContext = NotificationContext.GetNextUnvisitedPendingContext(MapiRpc.pumpRoundCookie);
					if (nextUnvisitedPendingContext == null)
					{
						break;
					}
					INotificationSession session2 = nextUnvisitedPendingContext.Session;
					if (session2 != null)
					{
						int rpcContext = session2.RpcContext;
						if (rpcContext != 0)
						{
							IntPtr intPtr = new IntPtr(rpcContext);
							int num;
							MapiRpc.Execute(executionDiagnostics, mapiContext, "PumpNotifications", false, ref intPtr, true, null, null, 10, new ArraySegment<byte>(Array<byte>.Empty), new ArraySegment<byte>(Array<byte>.Empty), out num, delegate(MapiContext operationContext, ref MapiSession session, ref bool deregisterSession, AuxiliaryData auxiliaryData)
							{
								MapiRpc.PumpNotificationsForSession(operationContext, session, shouldCallbackContinue);
								return ErrorCode.NoError;
							});
						}
						mapiContext.ResetFailureHistory();
					}
				}
			}
		}

		internal static void PumpNotificationsForSession(MapiContext context, MapiSession session, Func<bool> shouldCallbackContinue)
		{
			List<int> list = null;
			bool flag = ExTraceGlobals.RpcBufferTracer.IsTraceEnabled(TraceType.DebugTrace);
			if (flag)
			{
				list = new List<int>(10);
			}
			NotificationEvent notificationEvent;
			while ((shouldCallbackContinue == null || shouldCallbackContinue()) && (notificationEvent = session.NotificationContext.PeekEvent()) != null)
			{
				if (flag)
				{
					list.Add(notificationEvent.MailboxNumber);
				}
				context.Initialize(notificationEvent.MdbGuid, notificationEvent.MailboxNumber, true, true);
				using (context.CreateUserIdentityFrame(session.AddressInfoUser.ObjectId))
				{
					ErrorCode errorCode = context.StartMailboxOperation();
					if (errorCode != ErrorCode.NoError)
					{
						throw new StoreException((LID)43600U, errorCode);
					}
					if (context.IsMailboxOperationStarted)
					{
						context.EndMailboxOperation(true);
					}
				}
			}
			if (flag)
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("MapiRpc:PumpNotificationsForSession(): events pumped for mailbox numbers:[");
				stringBuilder.Append(string.Join<int>(",", list.ToArray()));
				stringBuilder.Append("]");
				ExTraceGlobals.RpcBufferTracer.TraceDebug(0L, stringBuilder.ToString());
			}
		}

		public void DeregisterSession(MapiContext context, MapiSession session)
		{
			using (LockManager.Lock(this.sessionsHash, context.Diagnostics))
			{
				this.sessionsHash.Remove(session.RpcContext);
				session.RpcContext = 0;
			}
			if (!session.IsDisposed)
			{
				session.Dispose();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiRpc>(this);
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			try
			{
				if (this.isInitialized && calledFromDispose)
				{
					foreach (MapiSession mapiSession in this.sessionsHash.Values)
					{
						mapiSession.LockSession(false);
						try
						{
							mapiSession.Dispose();
						}
						finally
						{
							mapiSession.UnlockSession();
						}
					}
					Interlocked.Decrement(ref MapiRpc.instanceCount);
				}
			}
			finally
			{
				this.isInitialized = false;
			}
		}

		internal static ErrorCode Execute(IExecutionDiagnostics executionDiagnostics, MapiContext outerContext, string functionName, bool isRpc, ref IntPtr contextHandle, bool tryLockSession, string userDn, IList<ArraySegment<byte>> dataIn, int sizeInMegabytes, ArraySegment<byte> auxIn, ArraySegment<byte> auxOut, out int sizeAuxOut, MapiRpc.ExecuteDelegate executeDelegate)
		{
			ErrorCode errorCode = ErrorCode.NoError;
			bool flag = false;
			MapiSession mapiSession = null;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			AuxiliaryData auxiliaryData = null;
			sizeAuxOut = 0;
			LockManager.AssertNoLocksHeld();
			DiagnosticContext.Reset();
			if (Thread.CurrentThread.Name == null)
			{
				Thread.CurrentThread.Name = "RPC Server Thread";
			}
			MapiContext mapiContext;
			if (outerContext != null)
			{
				mapiContext = null;
			}
			else
			{
				MapiExecutionDiagnostics executionDiagnostics2 = executionDiagnostics as MapiExecutionDiagnostics;
				if (executionDiagnostics == null)
				{
					executionDiagnostics2 = new MapiExecutionDiagnostics();
				}
				mapiContext = MapiContext.Create(executionDiagnostics2, ClientType.MaxValue);
			}
			using (mapiContext)
			{
				StorePerDatabasePerformanceCountersInstance storePerDatabasePerformanceCountersInstance = null;
				MapiContext mapiContext3 = (mapiContext != null) ? mapiContext : outerContext;
				MapiExecutionDiagnostics mapiExecutionDiagnostics = (MapiExecutionDiagnostics)mapiContext3.Diagnostics;
				bool assertCondition = false;
				try
				{
					if (contextHandle != IntPtr.Zero)
					{
						mapiSession = MapiRpc.Instance.SessionFromSessionId(contextHandle.ToInt32());
						if (mapiSession == null)
						{
							contextHandle = IntPtr.Zero;
							errorCode = ErrorCode.CreateRpcServerUnavailable((LID)55864U);
						}
					}
					if (ErrorCode.NoError == errorCode)
					{
						if (isRpc)
						{
							mapiExecutionDiagnostics.OnRpcBegin();
							storePerDatabasePerformanceCountersInstance = PerformanceCounterFactory.GetDatabaseInstance((mapiSession != null) ? mapiSession.LastUsedDatabase : null);
							if (storePerDatabasePerformanceCountersInstance != null)
							{
								storePerDatabasePerformanceCountersInstance.RPCsInProgress.Increment();
								storePerDatabasePerformanceCountersInstance.PercentRPCsInProgress.Increment();
								storePerDatabasePerformanceCountersInstance.RateOfRPCs.Increment();
							}
						}
						if (mapiSession != null)
						{
							if (tryLockSession)
							{
								if (!mapiSession.TryLockSession(isRpc))
								{
									errorCode = ErrorCode.CreateSessionLocked((LID)58520U);
									mapiSession = null;
								}
							}
							else
							{
								mapiSession.LockSession(isRpc);
							}
							if (mapiSession != null && !mapiSession.IsValid)
							{
								errorCode = ErrorCode.CreateMdbNotInitialized((LID)46344U);
							}
							if (ErrorCode.NoError == errorCode)
							{
								mapiExecutionDiagnostics.UpdateClientType(mapiSession.InternalClientType);
								MapiRpc.SetContextForRopHandler(mapiSession, mapiContext3);
								flag4 = true;
								flag2 = MapiRpc.TurnOnPerUserTracing(mapiSession);
							}
						}
						else if (userDn != null)
						{
							flag2 = MapiRpc.EnablePerUserTracing(userDn);
						}
						flag3 = ExTraceGlobals.RpcOperationTracer.IsTraceEnabled(TraceType.FunctionTrace);
						if (flag3)
						{
							MapiRpc.TraceStartRpcMarker(functionName, contextHandle, mapiSession, errorCode);
						}
					}
					assertCondition = true;
				}
				finally
				{
					Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(assertCondition, "Exception in a critical entry block");
				}
				try
				{
					if (ErrorCode.NoError == errorCode)
					{
						if (mapiSession != null)
						{
							if (Microsoft.Exchange.Server.Storage.MapiDisp.Globals.IsTokenSingleInstancingEnabled)
							{
								mapiSession.HydrateSessionSecurityContext();
								mapiSession.HydrateDelegatedSecurityContext();
							}
							if (mapiSession.NeedToClose)
							{
								if (ExTraceGlobals.RpcOperationTracer.IsTraceEnabled(TraceType.ErrorTrace))
								{
									ExTraceGlobals.RpcOperationTracer.TraceError<string>(0L, "Session {0} will be deregistered", mapiSession.SessionId.ToString());
								}
								flag = true;
								errorCode = ErrorCode.CreateMdbNotInitialized((LID)49904U);
							}
							else
							{
								mapiContext3.Configure(mapiSession);
								((MapiExecutionDiagnostics)mapiContext3.Diagnostics).MapiExMonLogger = mapiSession.MapiExMonLogger;
							}
						}
						if (ErrorCode.NoError == errorCode)
						{
							if (isRpc)
							{
								auxiliaryData = AuxiliaryData.Parse(auxIn);
							}
							IDisposable disposable = null;
							using (disposable)
							{
								bool flag5 = mapiSession != null;
								errorCode = executeDelegate(mapiContext3, ref mapiSession, ref flag, auxiliaryData);
							}
						}
					}
				}
				catch (InsufficientMemoryException ex)
				{
					mapiContext3.OnExceptionCatch(ex);
					errorCode = MapiRpc.RecordException(mapiSession, ex, ErrorCode.CreateNotEnoughMemory((LID)33944U));
				}
				catch (StoreException ex2)
				{
					mapiContext3.OnExceptionCatch(ex2);
					errorCode = MapiRpc.RecordException(mapiSession, ex2, ErrorCode.CreateWithLid((LID)35992U, ex2.Error));
				}
				catch (BufferOutOfMemoryException ex3)
				{
					mapiContext3.OnExceptionCatch(ex3);
					errorCode = MapiRpc.RecordException(mapiSession, ex3, ErrorCode.CreateServerOutOfMemory((LID)53008U));
				}
				catch (BufferTooSmallException ex4)
				{
					mapiContext3.OnExceptionCatch(ex4);
					errorCode = MapiRpc.RecordException(mapiSession, ex4, ErrorCode.CreateBufferTooSmall((LID)60568U));
				}
				catch (RopExecutionException ex5)
				{
					mapiContext3.OnExceptionCatch(ex5);
					errorCode = MapiRpc.RecordException(mapiSession, ex5, ErrorCode.CreateWithLid((LID)44184U, (ErrorCodeValue)ex5.ErrorCode));
				}
				catch (BufferParseException ex6)
				{
					object diagnosticData = null;
					if (dataIn != null)
					{
						StringBuilder stringBuilder = new StringBuilder(1000);
						stringBuilder.Append("DataIn: ");
						for (int i = 0; i < dataIn.Count; i++)
						{
							ArraySegment<byte> arraySegment = dataIn[i];
							if (i > 0)
							{
								stringBuilder.Append(", ");
							}
							stringBuilder.AppendAsString(arraySegment.Array, arraySegment.Offset, arraySegment.Count);
						}
						diagnosticData = stringBuilder.ToString();
					}
					mapiContext3.OnExceptionCatch(ex6, diagnosticData);
					errorCode = MapiRpc.RecordException(mapiSession, ex6, ErrorCode.CreateRpcFormat((LID)40088U));
				}
				catch (NonFatalDatabaseException ex7)
				{
					mapiContext3.OnExceptionCatch(ex7);
					errorCode = MapiRpc.RecordException(mapiSession, ex7, ErrorCode.CreateDatabaseError((LID)64456U));
				}
				catch (FatalDatabaseException ex8)
				{
					flag = (mapiSession != null);
					mapiContext3.OnExceptionCatch(ex8);
					errorCode = MapiRpc.RecordException(mapiSession, ex8, ErrorCode.CreateDatabaseError((LID)39880U));
				}
				finally
				{
					bool flag6 = false;
					Exception ex9 = null;
					try
					{
						if (mapiSession != null)
						{
							Exception lastRememberedException = mapiSession.LastRememberedException;
							if (mapiContext3.IsAnyCriticalBlockFailed && mapiContext3.HighestFailedCriticalBlockScope >= CriticalBlockScope.MailboxSession)
							{
								if (ExTraceGlobals.RpcOperationTracer.IsTraceEnabled(TraceType.ErrorTrace))
								{
									ExTraceGlobals.RpcOperationTracer.TraceError<string, string, string>(0L, "Session {0} will be deregistered as critical error {1} reported while executing, last remembered exception on the session={2}", mapiSession.SessionId.ToString(), errorCode.ToString(), (lastRememberedException == null) ? string.Empty : lastRememberedException.ToString());
								}
								DiagnosticContext.TraceLocation((LID)50784U);
								flag = true;
							}
							if (Microsoft.Exchange.Server.Storage.MapiDisp.Globals.IsTokenSingleInstancingEnabled && !flag)
							{
								mapiSession.DehydrateDelegatedSecurityContext();
								mapiSession.DehydrateSessionSecurityContext();
							}
							if (flag)
							{
								contextHandle = IntPtr.Zero;
								MapiRpc.Instance.DeregisterSession(mapiContext3, mapiSession);
							}
						}
						try
						{
							mapiContext3.Disconnect();
						}
						catch (NonFatalDatabaseException ex10)
						{
							mapiContext3.OnExceptionCatch(ex10);
							errorCode = MapiRpc.RecordException(mapiSession, ex10, ErrorCode.CreateDatabaseError((LID)53688U));
						}
						catch (FatalDatabaseException ex11)
						{
							mapiContext3.OnExceptionCatch(ex11);
							errorCode = MapiRpc.RecordException(mapiSession, ex11, ErrorCode.CreateDatabaseError((LID)50376U));
						}
						if (flag4)
						{
							MapiRpc.SetContextForRopHandler(mapiSession, null);
						}
						if (mapiSession != null)
						{
							mapiSession.UnlockSession();
						}
						mapiContext3.DismountOnCriticalFailure();
						if (flag3)
						{
							MapiRpc.TraceEndRpcMarker(functionName, contextHandle, errorCode);
						}
						if (flag2)
						{
							MapiRpc.TurnOffPerUserTracing();
						}
						if (isRpc)
						{
							mapiExecutionDiagnostics.OnRpcEnd();
							long incrementValue = (long)mapiExecutionDiagnostics.RpcLatency.TotalMilliseconds;
							if (storePerDatabasePerformanceCountersInstance != null)
							{
								storePerDatabasePerformanceCountersInstance.RPCsInProgress.Decrement();
								storePerDatabasePerformanceCountersInstance.PercentRPCsInProgress.Decrement();
								storePerDatabasePerformanceCountersInstance.AverageRPCLatency.IncrementBy(incrementValue);
								storePerDatabasePerformanceCountersInstance.AverageRPCLatencyBase.Increment();
							}
							if (auxiliaryData != null)
							{
								if (ErrorCode.NoError != errorCode && DiagnosticContext.HasData)
								{
									auxiliaryData.AppendOutput(new DiagCtxCtxDataAuxiliaryBlock(DiagnosticContext.PackInfo()));
								}
								auxiliaryData.AppendOutput(mapiExecutionDiagnostics.CreateRpcStatisticsAuxiliaryBlock(storePerDatabasePerformanceCountersInstance));
								auxiliaryData.Serialize(auxOut, out sizeAuxOut);
							}
						}
						flag6 = true;
					}
					catch (Exception ex12)
					{
						ex9 = ex12;
						mapiContext3.OnExceptionCatch(ex12);
						throw;
					}
					finally
					{
						if (!flag6)
						{
							Microsoft.Exchange.Server.Storage.Common.Globals.AssertRetail(false, string.Format("Exception in a critical finally block: {0} {1}", (ex9 != null) ? ex9.Message : string.Empty, (ex9 != null) ? ex9.StackTrace : string.Empty));
						}
					}
					LockManager.AssertNoLocksHeld();
				}
			}
			return errorCode;
		}

		private static ErrorCode RecordException(MapiSession session, Exception e, ErrorCode error)
		{
			if (session != null)
			{
				session.RememberLastException(e);
			}
			ExTraceGlobals.RpcDetailTracer.TraceDebug<Exception, ErrorCode>(0L, "MapiRpc:Execute(): Mapped Exception=[{0}] to error [{1}]", e, error);
			return error;
		}

		internal int RegisterSession(MapiSession session)
		{
			int rpcContext;
			using (LockManager.Lock(this.sessionsHash))
			{
				session.RpcContext = this.NextSessionId();
				this.sessionsHash.Add(session.RpcContext, session);
				rpcContext = session.RpcContext;
			}
			return rpcContext;
		}

		private static void SetContextForRopHandler(MapiSession session, MapiContext context)
		{
			if (session.ConnectionHandler != null)
			{
				RopHandler ropHandler = (RopHandler)session.ConnectionHandler.RopHandler;
				ropHandler.MapiContext = context;
			}
		}

		private static void TraceStartRpcMarker(string rpcName, IntPtr contextHandle, MapiSession session, ErrorCode error)
		{
			StringBuilder stringBuilder = new StringBuilder(60);
			stringBuilder.Append("MARK CALL [MAPI][");
			stringBuilder.Append(rpcName);
			stringBuilder.Append("] session:[");
			if (contextHandle != IntPtr.Zero)
			{
				stringBuilder.Append(contextHandle.ToInt32());
			}
			else
			{
				stringBuilder.Append("new");
			}
			stringBuilder.Append("]");
			if (session != null && session.IsValid)
			{
				stringBuilder.Append(" sessionGuid:[");
				stringBuilder.Append(session.SessionId);
				stringBuilder.Append("] client:[");
				stringBuilder.Append(session.InternalClientType);
				if (session.ApplicationId != null)
				{
					stringBuilder.Append("] applicationId:[");
					stringBuilder.Append(session.ApplicationId);
				}
				stringBuilder.Append("]");
			}
			if (error != ErrorCode.NoError)
			{
				stringBuilder.Append(" error:[");
				stringBuilder.Append(error);
				stringBuilder.Append("]");
			}
			ExTraceGlobals.RpcOperationTracer.TraceFunction(0L, stringBuilder.ToString());
		}

		private static void TraceEndRpcMarker(string rpcName, IntPtr contextHandle, ErrorCode error)
		{
			StringBuilder stringBuilder = new StringBuilder(60);
			stringBuilder.Append("MARK CALL END [MAPI][");
			stringBuilder.Append(rpcName);
			stringBuilder.Append("] session:[");
			if (contextHandle != IntPtr.Zero)
			{
				stringBuilder.Append(contextHandle.ToInt32());
			}
			else
			{
				stringBuilder.Append("end");
			}
			if (error != ErrorCode.NoError)
			{
				stringBuilder.Append("] error:[");
				stringBuilder.Append(error);
			}
			stringBuilder.Append("]");
			ExTraceGlobals.RpcOperationTracer.TraceFunction(0L, stringBuilder.ToString());
		}

		private static void ConfigureRopExecutionCallbacks(MapiSession session)
		{
			RopDriver ropDriver = session.RopDriver as RopDriver;
			ropDriver.OnBeforeRopExecuted = delegate(Rop rop, ServerObjectHandleTable handleTable)
			{
				MapiRpc.OnBeforeRopExecuted(session, handleTable, rop);
			};
			ropDriver.OnAfterRopExecuted = delegate(Rop rop, ServerObjectHandleTable handleTable)
			{
				MapiRpc.OnAfterRopExecuted(session, handleTable, rop);
			};
		}

		private static void OnBeforeRopExecuted(MapiSession session, ServerObjectHandleTable serverObjectHandleTable, Rop rop)
		{
			if (rop.RopId != RopId.Logon && session.LastUsedLogonIndex != rop.LogonIndex)
			{
				if (ExTraceGlobals.RpcOperationTracer.IsTraceEnabled(TraceType.FunctionTrace))
				{
					MapiLogon logon = session.GetLogon((int)rop.LogonIndex);
					if (logon != null && logon.IsValid)
					{
						MapiRpc.TraceLogonDetails(logon);
					}
				}
				session.LastUsedLogonIndex = rop.LogonIndex;
			}
			if (ExTraceGlobals.RpcOperationTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				MapiRpc.TraceStartRopMarker(session, serverObjectHandleTable, rop);
			}
			if (ExTraceGlobals.RpcDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				string value;
				string value2;
				string value3;
				string value4;
				session.GetCurrentActivityDetails(out value, out value2, out value3, out value4);
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("INPUT  Rop.");
				stringBuilder.Append(rop.RopId);
				stringBuilder.Append(": session:[");
				stringBuilder.Append(session.SessionId);
				stringBuilder.Append("] logon:[");
				stringBuilder.Append(rop.LogonIndex);
				DualInputRop dualInputRop = rop as DualInputRop;
				if (dualInputRop != null)
				{
					stringBuilder.Append("] srcHsot:[");
					stringBuilder.Append(serverObjectHandleTable[(int)dualInputRop.SourceHandleTableIndex].HandleValue);
					stringBuilder.Append("] srcHsotId:[");
					stringBuilder.Append(dualInputRop.SourceHandleTableIndex);
					stringBuilder.Append("] dstHsot:[");
					stringBuilder.Append(serverObjectHandleTable[(int)dualInputRop.DestinationHandleTableIndex].HandleValue);
					stringBuilder.Append("] dstHsotId:[");
					stringBuilder.Append(dualInputRop.DestinationHandleTableIndex);
					stringBuilder.Append("]");
				}
				else
				{
					stringBuilder.Append("] hsot:[");
					stringBuilder.Append(serverObjectHandleTable[(int)rop.HandleTableIndex].HandleValue);
					stringBuilder.Append("] hsotId:[");
					stringBuilder.Append(rop.HandleTableIndex);
					stringBuilder.Append("]");
				}
				if (rop is InputOutputRop)
				{
					stringBuilder.Append(" newHsotId:[");
					stringBuilder.Append(((InputOutputRop)rop).ReturnHandleTableIndex);
					stringBuilder.Append("]");
				}
				stringBuilder.Append(" details:[");
				rop.AppendToString(stringBuilder);
				stringBuilder.Append("] activityId:[");
				stringBuilder.Append(value);
				stringBuilder.Append("] protocol:[");
				stringBuilder.Append(value2);
				stringBuilder.Append("] component:[");
				stringBuilder.Append(value3);
				stringBuilder.Append("] action:[");
				stringBuilder.Append(value4);
				stringBuilder.Append("]");
				ExTraceGlobals.RpcDetailTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			if (rop.RopId != RopId.Logon && !session.CanAcceptROPs)
			{
				throw new ExExceptionAccessDenied((LID)43576U, "Session is not authenticated");
			}
		}

		private static void OnAfterRopExecuted(MapiSession session, ServerObjectHandleTable serverObjectHandleTable, Rop rop)
		{
			StorePerDatabasePerformanceCountersInstance databaseInstance = PerformanceCounterFactory.GetDatabaseInstance((session != null) ? session.LastUsedDatabase : null);
			if (databaseInstance != null)
			{
				databaseInstance.RateOfROPs.Increment();
			}
			ErrorCodeValue errorCodeValue = ErrorCodeValue.NoError;
			if (rop.Result != null)
			{
				errorCodeValue = (ErrorCodeValue)rop.Result.ErrorCode;
			}
			if (ExTraceGlobals.RpcDetailTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				string value;
				string value2;
				string value3;
				string value4;
				session.GetCurrentActivityDetails(out value, out value2, out value3, out value4);
				StringBuilder stringBuilder = new StringBuilder(100);
				stringBuilder.Append("OUTPUT Rop.");
				stringBuilder.Append(rop.RopId);
				stringBuilder.Append((errorCodeValue == ErrorCodeValue.NoError) ? " success:[" : " error:[");
				stringBuilder.Append(errorCodeValue);
				if (errorCodeValue == ErrorCodeValue.NoError && rop is InputOutputRop)
				{
					stringBuilder.Append("] newHsot:[");
					stringBuilder.Append(serverObjectHandleTable[(int)((InputOutputRop)rop).ReturnHandleTableIndex].HandleValue);
				}
				stringBuilder.Append("]");
				if (rop.Result != null)
				{
					stringBuilder.Append(" details:[");
					rop.Result.AppendToString(stringBuilder);
					stringBuilder.Append("]");
				}
				stringBuilder.Append(" activityId:[");
				stringBuilder.Append(value);
				stringBuilder.Append("] protocol:[");
				stringBuilder.Append(value2);
				stringBuilder.Append("] component:[");
				stringBuilder.Append(value3);
				stringBuilder.Append("] action:[");
				stringBuilder.Append(value4);
				stringBuilder.Append("]");
				ExTraceGlobals.RpcDetailTracer.TraceDebug(0L, stringBuilder.ToString());
			}
			if (errorCodeValue != ErrorCodeValue.NoError && !ErrorHelper.ShouldSkipBreadcrumb(0, (byte)rop.RopId, errorCodeValue, 0U))
			{
				int mailboxNumber = 0;
				MapiLogon logon = session.GetLogon((int)rop.LogonIndex);
				Guid guid = (session.LastUsedDatabase != null) ? session.LastUsedDatabase.MdbGuid : Guid.Empty;
				if (logon != null && logon.IsValid && logon.MapiMailbox != null)
				{
					mailboxNumber = logon.MapiMailbox.MailboxNumber;
				}
				ErrorHelper.AddBreadcrumb(BreadcrumbKind.RopError, 0, (byte)rop.RopId, (byte)session.InternalClientType, guid.GetHashCode(), mailboxNumber, (int)errorCodeValue, null);
			}
			if (ExTraceGlobals.RpcOperationTracer.IsTraceEnabled(TraceType.FunctionTrace))
			{
				MapiRpc.TraceEndRopMarker(session, serverObjectHandleTable, rop);
			}
		}

		private static void TraceStartRopMarker(MapiSession session, ServerObjectHandleTable serverObjectHandleTable, Rop rop)
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("MARK OP [Mapi][");
			stringBuilder.Append(rop.RopId);
			stringBuilder.Append("]");
			stringBuilder.Append(" logon:[");
			stringBuilder.Append(rop.LogonIndex);
			stringBuilder.Append("]");
			if (rop.RopId != RopId.Logon)
			{
				if (rop is InputRop)
				{
					uint handleValue = serverObjectHandleTable[(int)((InputRop)rop).InputHandleTableIndex].HandleValue;
					stringBuilder.Append(" hsot:[");
					stringBuilder.Append(handleValue);
					stringBuilder.Append("]");
					MapiBase mapiBase = MapiRpc.ObjectFromHsot(session, handleValue);
					if (mapiBase != null)
					{
						stringBuilder.Append(" object:[");
						stringBuilder.Append(mapiBase.MapiObjectType);
						stringBuilder.Append("]");
					}
				}
				else
				{
					uint handleValue = serverObjectHandleTable[(int)rop.HandleTableIndex].HandleValue;
					stringBuilder.Append(" hsot:[");
					stringBuilder.Append(handleValue);
					stringBuilder.Append("]");
					MapiBase mapiBase = MapiRpc.ObjectFromHsot(session, handleValue);
					if (mapiBase != null)
					{
						stringBuilder.Append(" object:[");
						stringBuilder.Append(mapiBase.MapiObjectType);
						stringBuilder.Append("]");
					}
					if (rop is DualInputRop)
					{
						handleValue = serverObjectHandleTable[(int)((DualInputRop)rop).DestinationHandleTableIndex].HandleValue;
						if (rop.RopId == RopId.CopyToExtended)
						{
							stringBuilder.Append(" logonDest:[");
							stringBuilder.Append("]");
						}
						stringBuilder.Append(" hsotDest:[");
						stringBuilder.Append(handleValue);
						stringBuilder.Append("]");
						mapiBase = MapiRpc.ObjectFromHsot(session, handleValue);
						if (mapiBase != null)
						{
							stringBuilder.Append(" objectDest:[");
							stringBuilder.Append(mapiBase.MapiObjectType);
							stringBuilder.Append("]");
						}
					}
				}
			}
			ExTraceGlobals.RpcOperationTracer.TraceFunction(0L, stringBuilder.ToString());
		}

		private static void TraceEndRopMarker(MapiSession session, ServerObjectHandleTable serverObjectHandleTable, Rop rop)
		{
			if (rop.RopId == RopId.Logon)
			{
				MapiLogon logon = session.GetLogon((int)rop.LogonIndex);
				if (logon != null && logon.IsValid)
				{
					MapiRpc.TraceLogonDetails(logon);
				}
			}
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("MARK OP END [Mapi][");
			stringBuilder.Append(rop.RopId);
			stringBuilder.Append("]");
			if (rop.Result != null)
			{
				if (rop.Result.ErrorCode == ErrorCode.None)
				{
					if (rop is InputOutputRop)
					{
						uint handleValue = serverObjectHandleTable[(int)((InputOutputRop)rop).ReturnHandleTableIndex].HandleValue;
						stringBuilder.Append(" hsotOut:[");
						stringBuilder.Append(handleValue);
						stringBuilder.Append("]");
						MapiBase mapiBase = MapiRpc.ObjectFromHsot(session, handleValue);
						if (mapiBase != null)
						{
							stringBuilder.Append(" objectOut:[");
							stringBuilder.Append(mapiBase.MapiObjectType);
							stringBuilder.Append("]");
						}
					}
					else if (rop.RopId == RopId.Logon)
					{
						uint handleValue = serverObjectHandleTable[(int)rop.HandleTableIndex].HandleValue;
						stringBuilder.Append(" hsotOut:[");
						stringBuilder.Append(handleValue);
						stringBuilder.Append("]");
						MapiBase mapiBase = MapiRpc.ObjectFromHsot(session, handleValue);
						if (mapiBase != null)
						{
							stringBuilder.Append(" objectOut:[");
							stringBuilder.Append(mapiBase.MapiObjectType);
							stringBuilder.Append("]");
						}
					}
				}
				else
				{
					stringBuilder.Append(" error:[");
					stringBuilder.Append((ErrorCodeValue)rop.Result.ErrorCode);
					stringBuilder.Append("]");
				}
			}
			ExTraceGlobals.RpcOperationTracer.TraceFunction(0L, stringBuilder.ToString());
		}

		private static void TraceLogonDetails(MapiLogon logon)
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("MARK LOGON");
			stringBuilder.Append(" logon:[");
			stringBuilder.Append(logon.Index);
			stringBuilder.Append("] mdbGuid:[");
			stringBuilder.Append(logon.MapiMailbox.MdbGuid);
			stringBuilder.Append("] mailboxGuid:[");
			stringBuilder.Append(logon.MapiMailbox.MailboxGuid);
			stringBuilder.Append("] mbxN:[");
			stringBuilder.Append(logon.StoreMailbox.MailboxNumber);
			stringBuilder.Append("] raw owner:[");
			stringBuilder.Append(logon.MailboxOwnerAddressInfo.DisplayName);
			stringBuilder.Append("] effective owner:[");
			stringBuilder.Append(logon.EffectiveOwnerAddressInfo.DisplayName);
			stringBuilder.Append("] loggedOn user:[");
			stringBuilder.Append(logon.LoggedOnUserAddressInfo.DisplayName);
			stringBuilder.Append("]");
			ExTraceGlobals.RpcOperationTracer.TraceFunction(0L, stringBuilder.ToString());
		}

		private static void TurnOffPerUserTracing()
		{
			PerUserTracing.TurnOff();
		}

		private static bool TurnOnPerUserTracing(MapiSession session)
		{
			bool result = false;
			if (session.IsValid && PerUserTracing.IsConfigured)
			{
				bool flag = false;
				if (PerUserTracing.IsEnabledForUser(session.UserDN))
				{
					flag = true;
				}
				else
				{
					foreach (MapiLogon mapiLogon in session.Logons)
					{
						if (PerUserTracing.IsEnabledForUser(mapiLogon.LoggedOnUserAddressInfo.LegacyExchangeDN) || PerUserTracing.IsEnabledForUser(mapiLogon.MailboxOwnerAddressInfo.LegacyExchangeDN))
						{
							flag = true;
							break;
						}
					}
				}
				if (flag)
				{
					PerUserTracing.TurnOn();
					result = true;
				}
			}
			return result;
		}

		private static bool EnablePerUserTracing(string legacyDN)
		{
			bool result = false;
			if (legacyDN != null && PerUserTracing.IsConfigured && PerUserTracing.IsEnabledForUser(legacyDN))
			{
				PerUserTracing.TurnOn();
				result = true;
			}
			return result;
		}

		private static byte[] ComputeStaticConnectAuxiliaryOutBuffer()
		{
			byte[] array = new byte[100];
			int num;
			AuxiliaryData.SerializeAuxiliaryBlocks(MapiRpc.DefaultConnectAuxiliaryBlocks, new ArraySegment<byte>(array), out num);
			byte[] array2 = new byte[num];
			Buffer.BlockCopy(array, 0, array2, 0, num);
			return array2;
		}

		private int NextSessionId()
		{
			int num;
			do
			{
				num = this.nextSessionId;
				this.nextSessionId++;
				if (this.nextSessionId == 1073741824)
				{
					this.nextSessionId = 1;
				}
			}
			while (this.sessionsHash.ContainsKey(num));
			return num;
		}

		public const int RetryCountDefault = 60;

		public const string RpcServerThreadName = "RPC Server Thread";

		private const string SessionListLockName = "Session List";

		private const int MaxSessionId = 1073741824;

		private const string SystemAttendantName = "/cn=Microsoft System Attendant";

		public static readonly TimeSpan PollsMaxDefault = TimeSpan.FromMinutes(1.0);

		public static readonly TimeSpan RetryDelayDefault = TimeSpan.FromSeconds(1.0);

		private static readonly AuxiliaryBlock[] DefaultConnectAuxiliaryBlocks = new AuxiliaryBlock[]
		{
			new MapiEndpointAuxiliaryBlock(MapiEndpointProcessType.ManagedStore, null),
			new ServerCapabilitiesAuxiliaryBlock(ServerCapabilityFlag.PackedFastTransferUploadBuffers | ServerCapabilityFlag.PackedWriteStreamExtendedUploadBuffers)
		};

		private static int instanceCount = 0;

		private static IMapiRpc instance;

		private static int pumpRoundCookie = 0;

		private static DriverFactory ropDriverFactory = new DriverFactory();

		private static Func<MapiSession, IConnectionHandler> connectionHandlerFactory = new Func<MapiSession, IConnectionHandler>(ConnectionHandler.Create);

		private static byte[] defaultConnectAuxiliaryBuffer;

		private int nextSessionId = 1;

		private bool isInitialized;

		private Dictionary<int, MapiSession> sessionsHash = new Dictionary<int, MapiSession>(500);

		internal delegate ErrorCode ExecuteDelegate(MapiContext operationContext, ref MapiSession session, ref bool deregisterSession, AuxiliaryData auxiliaryData);

		private class Decompressor : IAuthenticationContextCompression
		{
			public bool TryDecompress(ArraySegment<byte> source, ArraySegment<byte> destination)
			{
				return CompressAndObfuscate.Instance.TryExpand(source, destination);
			}
		}
	}
}
