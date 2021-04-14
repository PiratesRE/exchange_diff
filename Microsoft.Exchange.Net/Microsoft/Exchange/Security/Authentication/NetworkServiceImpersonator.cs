using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Security.Authentication
{
	internal static class NetworkServiceImpersonator
	{
		public static void Initialize()
		{
			if (!NetworkServiceImpersonator.initialized)
			{
				lock (NetworkServiceImpersonator.locker)
				{
					if (!NetworkServiceImpersonator.initialized)
					{
						ExWatson.SendReportOnUnhandledException(new ExWatson.MethodDelegate(NetworkServiceImpersonator.InitializeInternal));
					}
				}
			}
		}

		private static void InitializeInternal()
		{
			WindowsIdentity current = WindowsIdentity.GetCurrent();
			if (current.User.IsWellKnown(WellKnownSidType.NetworkServiceSid))
			{
				ExTraceGlobals.AuthenticationTracer.Information(0L, "Already running as NT AUTHORITY\\NetworkService");
				NetworkServiceImpersonator.windowsIdentity = current;
				NetworkServiceImpersonator.initialized = true;
				return;
			}
			current.Dispose();
			try
			{
				NetworkServiceImpersonator.windowsIdentity = NetworkServiceImpersonator.LogonAsNetworkService();
			}
			catch (LocalizedException ex)
			{
				NetworkServiceImpersonator.exception = ex;
			}
			finally
			{
				if (NetworkServiceImpersonator.windowsIdentity != null)
				{
					NetworkServiceImpersonator.initialized = true;
				}
				else
				{
					if (NetworkServiceImpersonator.exception == null)
					{
						NetworkServiceImpersonator.exception = new LogonAsNetworkServiceException("Unknown exception");
					}
					NetworkServiceImpersonator.failedInitializationAttempts++;
					if (NetworkServiceImpersonator.failedInitializationAttempts >= 20)
					{
						NetworkServiceImpersonator.initialized = true;
					}
				}
			}
		}

		public static LocalizedException Exception
		{
			get
			{
				return NetworkServiceImpersonator.exception;
			}
		}

		public static WindowsImpersonationContext Impersonate()
		{
			if (NetworkServiceImpersonator.exception != null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceError<LocalizedException>(0L, "Impersonation failed because of exception: {0}", NetworkServiceImpersonator.exception);
				throw NetworkServiceImpersonator.exception;
			}
			ExTraceGlobals.AuthenticationTracer.Information(0L, "Impersonating network service");
			return NetworkServiceImpersonator.windowsIdentity.Impersonate();
		}

		private static WindowsIdentity LogonAsNetworkService()
		{
			SafeTokenHandle safeTokenHandle = new SafeTokenHandle();
			try
			{
				ExTraceGlobals.AuthenticationTracer.Information(0L, "Calling LogonUser for NT AUTHORITY\\NetworkService");
				if (!SspiNativeMethods.LogonUser("NetworkService", "NT AUTHORITY", IntPtr.Zero, LogonType.Service, LogonProvider.Default, ref safeTokenHandle))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					ExTraceGlobals.AuthenticationTracer.TraceError<int>(0L, "LogonUser failed: {0}", lastWin32Error);
					throw new LogonAsNetworkServiceException(lastWin32Error.ToString());
				}
				ExTraceGlobals.AuthenticationTracer.Information(0L, "LogonUser succeeded");
				Exception ex2;
				try
				{
					return new WindowsIdentity(safeTokenHandle.DangerousGetHandle(), "Logon", WindowsAccountType.Normal, true);
				}
				catch (ArgumentException ex)
				{
					ex2 = ex;
				}
				catch (SecurityException ex3)
				{
					ex2 = ex3;
				}
				ExTraceGlobals.AuthenticationTracer.TraceError<Exception>(0L, "WindowsIdentity failed: {0}", ex2);
				throw new LogonAsNetworkServiceException(ex2.Message, ex2);
			}
			finally
			{
				safeTokenHandle.Close();
			}
			WindowsIdentity result;
			return result;
		}

		private const int maximumFailedInitializationAttempts = 20;

		private static bool initialized;

		private static WindowsIdentity windowsIdentity;

		private static object locker = new object();

		private static LocalizedException exception;

		private static int failedInitializationAttempts;
	}
}
