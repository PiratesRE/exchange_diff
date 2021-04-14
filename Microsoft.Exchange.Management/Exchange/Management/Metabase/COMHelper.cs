using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Management.Metabase
{
	internal class COMHelper
	{
		[DllImport("ole32.dll", CharSet = CharSet.Auto)]
		private static extern uint CoCreateInstanceEx([MarshalAs(UnmanagedType.LPStruct)] [In] Guid rclsid, [MarshalAs(UnmanagedType.IUnknown)] [In] object punkOuter, [In] uint dwClsContext, [In] [Out] ref COSERVERINFO pServerInfo, [In] uint cmq, [In] [Out] MULTI_QI[] pResults);

		[DllImport("ole32.dll", CharSet = CharSet.Auto)]
		private static extern uint CoSetProxyBlanket([In] IntPtr pProxy, [In] int dwAuthnSvc, [In] int dwAuthzSvc, [MarshalAs(UnmanagedType.LPWStr)] [In] string pServerPrincName, [In] int dwAuthnLevel, [In] int dwImpLevel, [In] IntPtr pAuthInfo, [In] int dwCapabilities);

		internal static uint Create(Guid guid, Guid iid, string serverName, ref object COMObj)
		{
			uint num = 0U;
			MULTI_QI[] array = new MULTI_QI[1];
			array[0].SetIID(iid);
			try
			{
				COAUTHINFO authInfo = new COAUTHINFO(Authn.Winnt, Authz.None, AuthnLevel.Default, ImpLevel.Impersonate);
				COSERVERINFO coserverinfo = new COSERVERINFO(serverName);
				coserverinfo.SetAuthInfo(authInfo);
				try
				{
					ClsCtx dwClsContext = ClsCtx.InprocServer | ClsCtx.LocalServer | ClsCtx.RemoteServer;
					num = COMHelper.CoCreateInstanceEx(guid, null, (uint)dwClsContext, ref coserverinfo, (uint)array.Length, array);
					if (num != 0U)
					{
						return num;
					}
					try
					{
						num = COMHelper.CoSetProxyBlanket(array[0].pItf, (int)authInfo.dwAuthnSvc, (int)authInfo.dwAuthzSvc, authInfo.pwszServerPrincName, (int)authInfo.dwAuthnLevel, (int)authInfo.dwImpersonationLevel, authInfo.pAuthIdentityData, authInfo.dwCapabilities);
						if (num != 0U)
						{
							return num;
						}
						COMObj = Marshal.GetObjectForIUnknown(array[0].pItf);
					}
					finally
					{
						Marshal.Release(array[0].pItf);
					}
				}
				finally
				{
					coserverinfo.Dispose();
				}
			}
			finally
			{
				array[0].Dispose();
			}
			return num;
		}

		private const uint S_OK = 0U;
	}
}
