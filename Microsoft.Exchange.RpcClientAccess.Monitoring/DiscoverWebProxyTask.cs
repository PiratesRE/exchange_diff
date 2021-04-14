using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DiscoverWebProxyTask : BaseTask
	{
		public DiscoverWebProxyTask(IContext context) : base(context, Strings.DiscoverWebProxyTaskTitle, Strings.DiscoverWebProxyTaskDescription, TaskType.Knowledge, new ContextProperty[0])
		{
		}

		protected override IEnumerator<ITask> Process()
		{
			string destinationUrl = string.Format("{0}://{1}", RpcHelper.DetectShouldUseSsl(base.Get<RpcProxyPort>(DiscoverWebProxyTask.DestinationPort)) ? Uri.UriSchemeHttps : Uri.UriSchemeHttp, base.Get<string>(DiscoverWebProxyTask.DestinationServer));
			try
			{
				base.Set<string>(DiscoverWebProxyTask.WebProxy, (DiscoverWebProxyTask.GetProxies(destinationUrl) ?? "<none>").Split(new char[]
				{
					';'
				})[0]);
				base.Result = TaskResult.Success;
				yield break;
			}
			catch (COMException ex)
			{
				base.Set<COMException>(BaseTask.Exception, ex);
				base.Set<string>(BaseTask.ErrorDetails, ((DiscoverWebProxyTask.WinHttp.ErrorCodes)ex.ErrorCode).ToString());
				base.Result = TaskResult.Failed;
				yield break;
			}
			yield break;
		}

		private static string GetProxies(string destinationUrl)
		{
			DiscoverWebProxyTask.WinHttp.WINHTTP_CURRENT_USER_IE_PROXY_CONFIG winhttp_CURRENT_USER_IE_PROXY_CONFIG = default(DiscoverWebProxyTask.WinHttp.WINHTTP_CURRENT_USER_IE_PROXY_CONFIG);
			DiscoverWebProxyTask.WinHttp.WINHTTP_AUTOPROXY_OPTIONS winhttp_AUTOPROXY_OPTIONS = new DiscoverWebProxyTask.WinHttp.WINHTTP_AUTOPROXY_OPTIONS
			{
				AutoLogonIfChallenged = false
			};
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				if (DiscoverWebProxyTask.WinHttp.WinHttpGetIEProxyConfigForCurrentUser(ref winhttp_CURRENT_USER_IE_PROXY_CONFIG))
				{
					if (winhttp_CURRENT_USER_IE_PROXY_CONFIG.AutoDetect)
					{
						winhttp_AUTOPROXY_OPTIONS.Flags = DiscoverWebProxyTask.WinHttp.AutoProxyFlags.AutoDetect;
						winhttp_AUTOPROXY_OPTIONS.AutoConfigUrl = null;
						winhttp_AUTOPROXY_OPTIONS.AutoDetectFlags = (DiscoverWebProxyTask.WinHttp.AutoDetectType.Dhcp | DiscoverWebProxyTask.WinHttp.AutoDetectType.DnsA);
					}
					else if (winhttp_CURRENT_USER_IE_PROXY_CONFIG.AutoConfigUrl != IntPtr.Zero)
					{
						winhttp_AUTOPROXY_OPTIONS.Flags = DiscoverWebProxyTask.WinHttp.AutoProxyFlags.AutoProxyConfigUrl;
						winhttp_AUTOPROXY_OPTIONS.AutoConfigUrl = Marshal.PtrToStringUni(winhttp_CURRENT_USER_IE_PROXY_CONFIG.AutoConfigUrl);
						winhttp_AUTOPROXY_OPTIONS.AutoDetectFlags = DiscoverWebProxyTask.WinHttp.AutoDetectType.None;
					}
				}
				else
				{
					Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
				}
			}
			finally
			{
				Marshal.FreeHGlobal(winhttp_CURRENT_USER_IE_PROXY_CONFIG.AutoConfigUrl);
				Marshal.FreeHGlobal(winhttp_CURRENT_USER_IE_PROXY_CONFIG.Proxy);
				Marshal.FreeHGlobal(winhttp_CURRENT_USER_IE_PROXY_CONFIG.ProxyBypass);
			}
			string result;
			using (DiscoverWebProxyTask.SafeInternetHandle safeInternetHandle = DiscoverWebProxyTask.WinHttp.WinHttpOpen("MSRPC", DiscoverWebProxyTask.WinHttp.AccessType.NoProxy, null, null, DiscoverWebProxyTask.WinHttp.SessionOpenFlags.Async))
			{
				string text;
				DiscoverWebProxyTask.WinHttpGetProxyForUrl(safeInternetHandle, destinationUrl, ref winhttp_AUTOPROXY_OPTIONS, out text);
				result = text;
			}
			return result;
		}

		private static void WinHttpGetProxyForUrl(DiscoverWebProxyTask.SafeInternetHandle session, string destination, ref DiscoverWebProxyTask.WinHttp.WINHTTP_AUTOPROXY_OPTIONS autoProxyOptions, out string proxyListString)
		{
			DiscoverWebProxyTask.WinHttp.WINHTTP_PROXY_INFO winhttp_PROXY_INFO = default(DiscoverWebProxyTask.WinHttp.WINHTTP_PROXY_INFO);
			proxyListString = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				if (DiscoverWebProxyTask.WinHttp.WinHttpGetProxyForUrl(session, destination, ref autoProxyOptions, out winhttp_PROXY_INFO))
				{
					proxyListString = ((winhttp_PROXY_INFO.AccessType == DiscoverWebProxyTask.WinHttp.AccessType.NamedProxy) ? Marshal.PtrToStringUni(winhttp_PROXY_INFO.Proxy) : null);
				}
				else
				{
					Marshal.ThrowExceptionForHR(Marshal.GetLastWin32Error());
				}
			}
			finally
			{
				Marshal.FreeHGlobal(winhttp_PROXY_INFO.Proxy);
				Marshal.FreeHGlobal(winhttp_PROXY_INFO.ProxyBypass);
			}
		}

		public static readonly ContextProperty<string> DestinationServer = ContextPropertySchema.RpcProxyServer.GetOnly();

		public static readonly ContextProperty<RpcProxyPort> DestinationPort = ContextPropertySchema.RpcProxyPort.GetOnly();

		public static readonly ContextProperty<string> WebProxy = ContextPropertySchema.WebProxyServer.SetOnly();

		[SuppressUnmanagedCodeSecurity]
		private static class WinHttp
		{
			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			[DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern bool WinHttpCloseHandle(IntPtr httpSession);

			[DllImport("winhttp.dll", SetLastError = true)]
			internal static extern bool WinHttpGetIEProxyConfigForCurrentUser(ref DiscoverWebProxyTask.WinHttp.WINHTTP_CURRENT_USER_IE_PROXY_CONFIG proxyConfig);

			[DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern bool WinHttpGetProxyForUrl(DiscoverWebProxyTask.SafeInternetHandle session, string url, [In] ref DiscoverWebProxyTask.WinHttp.WINHTTP_AUTOPROXY_OPTIONS autoProxyOptions, out DiscoverWebProxyTask.WinHttp.WINHTTP_PROXY_INFO proxyInfo);

			[DllImport("winhttp.dll", CharSet = CharSet.Unicode, SetLastError = true)]
			internal static extern DiscoverWebProxyTask.SafeInternetHandle WinHttpOpen(string userAgent, DiscoverWebProxyTask.WinHttp.AccessType accessType, string proxyName, string proxyBypass, DiscoverWebProxyTask.WinHttp.SessionOpenFlags flags);

			public enum AccessType
			{
				DefaultProxy,
				NoProxy,
				NamedProxy = 3
			}

			[Flags]
			public enum AutoDetectType
			{
				None = 0,
				Dhcp = 1,
				DnsA = 2
			}

			[Flags]
			public enum AutoProxyFlags
			{
				AutoDetect = 1,
				AutoProxyConfigUrl = 2,
				RunInProcess = 65536,
				RunOutProcessOnly = 131072
			}

			[Flags]
			public enum SessionOpenFlags
			{
				Async = 268435456
			}

			public enum ErrorCodes
			{
				AudodetectionFailed = 12180,
				AuthCertNeeded = 12044,
				AutoProxyServiceError = 12178,
				BadAutoProxyScript = 12166,
				CannotCallAfterOpen = 12103,
				CannotCallAfterSend = 12102,
				CannotCallBeforeOpen = 12100,
				CannotCallBeforeSend,
				CannotConnect = 12029,
				ChunkedEncodingHeaderSizeOverflow = 12183,
				ClientCertNoAccessPrivateKey = 12186,
				ClientCertNoPrivateKey = 12185,
				ConnectionError = 12030,
				HeaderAlreadyExists = 12155,
				HeaderCountExceeded = 12181,
				HeaderNotFound = 12150,
				HeaderSizeOverflow = 12182,
				IncorrectHandleState = 12019,
				IncorrectHandleType = 12018,
				InternalError = 12004,
				InvalidHeader = 12153,
				InvalidOption = 12009,
				InvalidQueryRequest = 12154,
				InvalidServerResponse = 12152,
				InvalidUrl = 12005,
				LoginFailure = 12015,
				NameNotResolved = 12007,
				NotInitialized = 12172,
				OperationCancelled = 12017,
				OptionNotSettable = 12011,
				OutOfHandles = 12001,
				RedirectFailed = 12156,
				ResendRequest = 12032,
				ResponseDrainOverflow = 12184,
				SecureCertCNInvalid = 12038,
				SecureCertDateInvalid = 12037,
				SecureCertRevFailed = 12057,
				SecureCertRevoked = 12170,
				SecureCertWrongUsage = 12179,
				SecureChannelError = 12157,
				SecureFailure = 12175,
				SecureInvalidCA = 12045,
				SecureInvalidCert = 12169,
				Shutdown = 12012,
				Success = 0,
				Timeout = 12002,
				UnableToDownloadScript = 12167,
				UnrecognizedScheme = 12006
			}

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			public struct WINHTTP_AUTOPROXY_OPTIONS
			{
				public DiscoverWebProxyTask.WinHttp.AutoProxyFlags Flags;

				public DiscoverWebProxyTask.WinHttp.AutoDetectType AutoDetectFlags;

				[MarshalAs(UnmanagedType.LPWStr)]
				public string AutoConfigUrl;

				public IntPtr Reserved1;

				public int Reserved2;

				public bool AutoLogonIfChallenged;
			}

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			public struct WINHTTP_CURRENT_USER_IE_PROXY_CONFIG
			{
				public bool AutoDetect;

				public IntPtr AutoConfigUrl;

				public IntPtr Proxy;

				public IntPtr ProxyBypass;
			}

			[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
			public struct WINHTTP_PROXY_INFO
			{
				public DiscoverWebProxyTask.WinHttp.AccessType AccessType;

				public IntPtr Proxy;

				public IntPtr ProxyBypass;
			}
		}

		[SuppressUnmanagedCodeSecurity]
		private sealed class SafeInternetHandle : SafeHandleZeroOrMinusOneIsInvalid
		{
			public SafeInternetHandle() : base(true)
			{
			}

			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			protected override bool ReleaseHandle()
			{
				return DiscoverWebProxyTask.WinHttp.WinHttpCloseHandle(this.handle);
			}
		}
	}
}
