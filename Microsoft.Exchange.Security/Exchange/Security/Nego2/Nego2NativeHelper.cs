using System;
using System.Globalization;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Exchange.Security.Nego2
{
	internal static class Nego2NativeHelper
	{
		internal static NetworkCredential GetCredential(IntPtr ppAuthIdentity)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			int num = Nego2NativeHelper.SspiEncodeAuthIdentityAsStrings(ppAuthIdentity, ref stringBuilder, ref stringBuilder2, ref stringBuilder3);
			if (num != 0)
			{
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "SspiEncodeAuthIdentityAsStrings failed with error {0}", new object[]
				{
					num
				}));
			}
			return new NetworkCredential(stringBuilder.ToString(), stringBuilder3.ToString(), stringBuilder2.ToString());
		}

		[DllImport("sspicli.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern int SspiEncodeAuthIdentityAsStrings(IntPtr pAuthIdentity, ref StringBuilder pszUserName, ref StringBuilder pszDomainName, ref StringBuilder pszPackedCredentialsString);

		[DllImport("Nego2NativeInterface.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern int CreateLiveClientAuthBufferWithPlainPassword([MarshalAs(UnmanagedType.LPWStr)] string wszUserName, [MarshalAs(UnmanagedType.LPWStr)] string wszPassword, uint dwFlags, bool bIsBusinessInstance, out IntPtr ppAuthBuffer, out IntPtr pwdAuthBufferLen);

		[DllImport("Nego2NativeInterface.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern int FreeAuthBuffer(IntPtr ppAuthBuffer);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct CREDUI_INFO
		{
			public int cbSize;

			public IntPtr hwndParent;

			public string pszMessageText;

			public string pszCaptionText;

			public IntPtr hbmBanner;
		}
	}
}
