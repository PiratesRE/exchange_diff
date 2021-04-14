using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Exchange.Win32;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Security
{
	internal class SafeLsaUntrustedHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public SafeLsaUntrustedHandle() : base(true)
		{
		}

		public SafeLsaUntrustedHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		public static SafeLsaUntrustedHandle Create()
		{
			SafeLsaUntrustedHandle result;
			int num = LsaNativeMethods.LsaConnectUntrusted(out result);
			if (num == 0)
			{
				return result;
			}
			throw new Win32Exception(LsaNativeMethods.LsaNtStatusToWinError(num));
		}

		public int LookupPackage(string package)
		{
			int result;
			using (LsaNativeMethods.SafeLsaAnsiString safeLsaAnsiString = new LsaNativeMethods.SafeLsaAnsiString(package))
			{
				int num = LsaNativeMethods.LsaLookupAuthenticationPackage(this, safeLsaAnsiString, out result);
				if (num != 0)
				{
					throw new Win32Exception(LsaNativeMethods.LsaNtStatusToWinError(num), "LsaLookupAuthenticationPackage failed for package " + package);
				}
			}
			return result;
		}

		public void PurgeTicketCache(int packageId)
		{
			LsaNativeMethods.KerberosPurgeTicketCacheRequest kerberosPurgeTicketCacheRequest = new LsaNativeMethods.KerberosPurgeTicketCacheRequest(0);
			int num = 0;
			int ntstatus2;
			int ntstatus = LsaNativeMethods.LsaCallAuthenticationPackage(this, packageId, ref kerberosPurgeTicketCacheRequest, Marshal.SizeOf(kerberosPurgeTicketCacheRequest), IntPtr.Zero, ref num, out ntstatus2);
			int num2 = LsaNativeMethods.LsaNtStatusToWinError(ntstatus);
			if (num2 != 0)
			{
				throw new Win32Exception(num2);
			}
			num2 = LsaNativeMethods.LsaNtStatusToWinError(ntstatus2);
			if (num2 != 0)
			{
				throw new Win32Exception(num2);
			}
		}

		public void AddExtraCredentials(int packageId, string username, string domain, SecureString password, LsaNativeMethods.KerbRequestCredentialFlags flags, LsaNativeMethods.LUID luid)
		{
			using (SafeSecureHGlobalHandle safeSecureHGlobalHandle = LsaNativeMethods.KerberosAddCredentialsRequest.MarshalToNative(username, domain, password, flags, luid))
			{
				int num = 0;
				int ntstatus2;
				int ntstatus = LsaNativeMethods.LsaCallAuthenticationPackage(this, packageId, safeSecureHGlobalHandle.DangerousGetHandle(), safeSecureHGlobalHandle.Length, IntPtr.Zero, ref num, out ntstatus2);
				int num2 = LsaNativeMethods.LsaNtStatusToWinError(ntstatus);
				if (num2 != 0)
				{
					throw new Win32Exception(num2);
				}
				num2 = LsaNativeMethods.LsaNtStatusToWinError(ntstatus2);
				if (num2 != 0)
				{
					throw new Win32Exception(num2);
				}
			}
		}

		public void LiveQueryUserInfo(int packageId, out ulong cid, out string userName, out string ticket, out string siteName)
		{
			cid = 0UL;
			userName = string.Empty;
			ticket = string.Empty;
			siteName = string.Empty;
			int num = 0;
			int num2 = 0;
			IntPtr zero = IntPtr.Zero;
			LsaNativeMethods.LiveQueryUserInfoRequest liveQueryUserInfoRequest = new LsaNativeMethods.LiveQueryUserInfoRequest(0);
			try
			{
				int num3 = LsaNativeMethods.LsaCallAuthenticationPackage(this, packageId, ref liveQueryUserInfoRequest, Marshal.SizeOf(liveQueryUserInfoRequest), out zero, out num2, out num);
				if (num3 == 0)
				{
					num3 = num;
				}
				int num4 = LsaNativeMethods.LsaNtStatusToWinError(num3);
				if (num4 != 0)
				{
					throw new Win32Exception(num4);
				}
				Type typeFromHandle = typeof(LsaNativeMethods.LiveQueryUserInfoResponse);
				if (Marshal.SizeOf(typeFromHandle) > num2)
				{
					throw new Win32Exception("Response buffer is too small for a LiveQUeryUserInfoResponse");
				}
				LsaNativeMethods.LiveQueryUserInfoResponse liveQueryUserInfoResponse = (LsaNativeMethods.LiveQueryUserInfoResponse)Marshal.PtrToStructure(zero, typeFromHandle);
				cid = liveQueryUserInfoResponse.Cid;
				userName = liveQueryUserInfoResponse.GetEmailAddress(zero, num2);
				ticket = liveQueryUserInfoResponse.GetTicket(zero, num2);
				siteName = liveQueryUserInfoResponse.GetSiteName(zero, num2);
			}
			finally
			{
				if (zero != IntPtr.Zero)
				{
					LsaNativeMethods.LsaFreeReturnBuffer(zero);
				}
			}
		}

		protected override bool ReleaseHandle()
		{
			return LsaNativeMethods.LsaDeregisterLogonProcess(this.handle) == 0;
		}
	}
}
