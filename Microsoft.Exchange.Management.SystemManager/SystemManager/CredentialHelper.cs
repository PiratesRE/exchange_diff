using System;
using System.Globalization;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager
{
	public static class CredentialHelper
	{
		public static PSCredential PromptForCredentials(IntPtr parentWindow, string displayName, Uri targetUri, string errorMessage, ref bool pfSave)
		{
			string text = errorMessage;
			NativeMethods.CREDUI_INFO credUiInfo;
			credUiInfo..ctor(parentWindow);
			credUiInfo.pszCaptionText = Strings.CredentialInputDialogTitle(displayName);
			NativeMethods.CredUIReturnCodes credUIReturnCodes;
			for (;;)
			{
				credUiInfo.pszMessageText = (text ?? string.Empty);
				string userName;
				SecureString password;
				credUIReturnCodes = (WinformsHelper.IsWin7OrLater() ? CredentialHelper.SspiPromptForCredentials(credUiInfo, targetUri, ref pfSave, out userName, out password) : CredentialHelper.CredUIPromptForCredentials(credUiInfo, ref pfSave, out userName, out password));
				if (credUIReturnCodes == null)
				{
					try
					{
						return new PSCredential(userName, password);
					}
					catch (PSArgumentException)
					{
						text = Strings.InvalidUserNameOrPassword;
						continue;
					}
				}
				if (credUIReturnCodes == 1223)
				{
					break;
				}
				if (credUIReturnCodes != 1315)
				{
					goto IL_8F;
				}
				text = Strings.InvalidUserNameOrPassword;
			}
			return null;
			IL_8F:
			throw new InvalidOperationException(string.Format("PromptForCredentials failed with error {0}.", credUIReturnCodes.ToString()));
		}

		public static PSCredential ReadCredential(string credentialKey)
		{
			NativeMethods.Credential credential;
			NativeMethods.CredRead(credentialKey, 1, 0U, ref credential);
			if (credential == null)
			{
				return null;
			}
			return new PSCredential(credential.UserName, credential.Password);
		}

		public static bool SaveCredential(string credentialKey, PSCredential credential)
		{
			NativeMethods.Credential credential2 = new NativeMethods.Credential(credentialKey, credential.UserName, credential.Password);
			return NativeMethods.CredWrite(credential2, 0U);
		}

		public static void RemoveCredential(string credentialKey)
		{
			NativeMethods.CredDelete(credentialKey, 1, 0);
		}

		private static NativeMethods.CredUIReturnCodes SspiPromptForCredentials(NativeMethods.CREDUI_INFO credUiInfo, Uri targetUri, ref bool pfSave, out string userName, out SecureString password)
		{
			IntPtr zero = IntPtr.Zero;
			NativeMethods.CredUIReturnCodes credUIReturnCodes = NativeMethods.SspiPromptForCredentials(targetUri.Host, ref credUiInfo, 0U, "Negotiate", IntPtr.Zero, ref zero, ref pfSave, 1);
			if (credUIReturnCodes == null)
			{
				try
				{
					CredentialHelper.AuthIdentityToCredential(zero, out userName, out password);
					return credUIReturnCodes;
				}
				finally
				{
					NativeMethods.SspiFreeAuthIdentity(zero);
				}
			}
			userName = null;
			password = null;
			return credUIReturnCodes;
		}

		private static NativeMethods.CredUIReturnCodes CredUIPromptForCredentials(NativeMethods.CREDUI_INFO credUiInfo, ref bool pfSave, out string userName, out SecureString password)
		{
			StringBuilder stringBuilder = new StringBuilder(CredentialHelper.MaxUserNameLength);
			StringBuilder stringBuilder2 = new StringBuilder(CredentialHelper.MaxPasswordLength);
			NativeMethods.CredUIReturnCodes credUIReturnCodes = NativeMethods.CredUIPromptForCredentials(ref credUiInfo, null, IntPtr.Zero, 0U, stringBuilder, stringBuilder.Capacity, stringBuilder2, stringBuilder2.Capacity, ref pfSave, 262338);
			userName = ((credUIReturnCodes == null) ? stringBuilder.ToString() : null);
			password = ((credUIReturnCodes == null) ? stringBuilder2.ToString().ConvertToSecureString() : null);
			return credUIReturnCodes;
		}

		private static void AuthIdentityToCredential(IntPtr authIdentity, out string userName, out SecureString password)
		{
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			StringBuilder stringBuilder3 = new StringBuilder();
			NativeMethods.SspiEncodeReturnCode sspiEncodeReturnCode = NativeMethods.SspiEncodeAuthIdentityAsStrings(authIdentity, ref stringBuilder, ref stringBuilder2, ref stringBuilder3);
			if (sspiEncodeReturnCode != null)
			{
				throw new InvalidOperationException(string.Format("AuthIdentityToCredential failed with error {0}", sspiEncodeReturnCode.ToString()));
			}
			userName = null;
			if (!string.IsNullOrEmpty(stringBuilder2.ToString()))
			{
				userName = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", new object[]
				{
					stringBuilder2.ToString(),
					stringBuilder.ToString()
				});
			}
			else
			{
				userName = stringBuilder.ToString();
			}
			password = stringBuilder3.ToString().ConvertToSecureString();
		}

		public static bool ForceConnection(Uri uri)
		{
			CredentialHelper.IWinHttpRequest winHttpRequest = (CredentialHelper.IWinHttpRequest)new CredentialHelper.WinHttpRequestClass();
			bool result;
			try
			{
				winHttpRequest.Open("GET", uri.ToString(), null);
				winHttpRequest.SetAutoLogonPolicy(CredentialHelper.WinHttpRequestAutoLogonPolicy.AutoLogonPolicy_Never);
				winHttpRequest.Send(null);
				result = true;
			}
			catch (COMException)
			{
				result = false;
			}
			catch (InvalidComObjectException)
			{
				result = false;
			}
			catch (TargetException)
			{
				result = false;
			}
			return result;
		}

		private const string NEGOSSP_NAME_W = "Negotiate";

		private static readonly int MaxUserNameLength = 1024;

		private static readonly int MaxPasswordLength = 127;

		[Guid("2087C2F4-2CEF-4953-A8AB-66779B670495")]
		[ComImport]
		private class WinHttpRequestClass
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			public extern WinHttpRequestClass();
		}

		[Guid("9D8A6DF8-13DE-4B1F-A330-67C719D62514")]
		public enum WinHttpRequestAutoLogonPolicy
		{
			AutoLogonPolicy_Always,
			AutoLogonPolicy_OnlyIfBypassProxy,
			AutoLogonPolicy_Never
		}

		[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
		[Guid("016FE2EC-B2C8-45F8-B23B-39E53A75396B")]
		[ComImport]
		public interface IWinHttpRequest
		{
			[DispId(1)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			void Open([MarshalAs(UnmanagedType.BStr)] [In] string Method, [MarshalAs(UnmanagedType.BStr)] [In] string Url, [MarshalAs(UnmanagedType.Struct)] [In] [Optional] object Async);

			[DispId(5)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			void Send([MarshalAs(UnmanagedType.Struct)] [In] [Optional] object Body);

			[DispId(18)]
			[MethodImpl(MethodImplOptions.InternalCall)]
			void SetAutoLogonPolicy([In] CredentialHelper.WinHttpRequestAutoLogonPolicy AutoLogonPolicy);
		}
	}
}
