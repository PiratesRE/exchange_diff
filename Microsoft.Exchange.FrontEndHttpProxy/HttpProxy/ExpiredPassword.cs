using System;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.HttpProxy
{
	public class ExpiredPassword : OwaPage
	{
		protected ExpiredPassword.ExpiredPasswordReason Reason
		{
			get
			{
				return this.reason;
			}
		}

		protected string Destination
		{
			get
			{
				string text = base.Request.Form["url"];
				if (string.IsNullOrEmpty(text))
				{
					return "../";
				}
				return text;
			}
		}

		protected string UserNameLabel
		{
			get
			{
				switch (OwaVdirConfiguration.Instance.LogonFormat)
				{
				case LogonFormats.PrincipalName:
					return LocalizedStrings.GetHtmlEncoded(1677919363);
				case LogonFormats.UserName:
					return LocalizedStrings.GetHtmlEncoded(537815319);
				}
				return LocalizedStrings.GetHtmlEncoded(78658498);
			}
		}

		protected bool PasswordChanged
		{
			get
			{
				return this.passwordChanged;
			}
		}

		protected bool ShouldClearAuthenticationCache
		{
			get
			{
				return true;
			}
		}

		protected override bool UseStrictMode
		{
			get
			{
				return false;
			}
		}

		[DllImport("netapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern uint NetUserChangePassword(string domainname, string username, IntPtr oldpassword, IntPtr newpassword);

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.reason = ExpiredPassword.ExpiredPasswordReason.None;
			this.passwordChanged = false;
			this.ChangePassword();
			if (this.passwordChanged)
			{
				Utility.DeleteFbaAuthCookies(base.Request, base.Response);
			}
		}

		private static ExpiredPassword.ChangePasswordResult ChangePasswordNUCP(string logonName, SecureString oldPassword, SecureString newPassword)
		{
			if (logonName == null || oldPassword == null || newPassword == null)
			{
				throw new ArgumentNullException();
			}
			string text = string.Empty;
			string text2 = string.Empty;
			switch (OwaVdirConfiguration.Instance.LogonFormat)
			{
			case LogonFormats.FullDomain:
				ExpiredPassword.GetDomainUser(logonName, ref text, ref text2);
				break;
			case LogonFormats.PrincipalName:
				text = NativeHelpers.GetDomainName();
				text2 = logonName;
				break;
			case LogonFormats.UserName:
				if (logonName.IndexOf("\\") == -1)
				{
					text2 = logonName;
					text = NativeHelpers.GetDomainName();
				}
				else
				{
					ExpiredPassword.GetDomainUser(logonName, ref text, ref text2);
				}
				break;
			}
			if (text == string.Empty || text2 == string.Empty)
			{
				return ExpiredPassword.ChangePasswordResult.OtherError;
			}
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			try
			{
				intPtr = Marshal.SecureStringToGlobalAllocUnicode(oldPassword);
				intPtr2 = Marshal.SecureStringToGlobalAllocUnicode(newPassword);
				uint num = ExpiredPassword.NetUserChangePassword(text, text2, intPtr, intPtr2);
				if (num != 0U)
				{
					uint num2 = num;
					if (num2 == 5U)
					{
						return ExpiredPassword.ChangePasswordResult.LockedOut;
					}
					if (num2 == 86U)
					{
						return ExpiredPassword.ChangePasswordResult.InvalidCredentials;
					}
					if (num2 != 2245U)
					{
						return ExpiredPassword.ChangePasswordResult.OtherError;
					}
					return ExpiredPassword.ChangePasswordResult.BadNewPassword;
				}
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.ZeroFreeGlobalAllocUnicode(intPtr);
				}
				if (intPtr2 != IntPtr.Zero)
				{
					Marshal.ZeroFreeGlobalAllocUnicode(intPtr2);
				}
			}
			return ExpiredPassword.ChangePasswordResult.Success;
		}

		private static void GetDomainUser(string logonName, ref string domain, ref string user)
		{
			string[] array = logonName.Split(new char[]
			{
				'\\'
			});
			if (array.Length == 2)
			{
				domain = array[0];
				user = array[1];
			}
		}

		private static bool SecureStringEquals(SecureString secureStringA, SecureString secureStringB)
		{
			if (secureStringA == null || secureStringB == null || secureStringA.Length != secureStringB.Length)
			{
				return false;
			}
			using (SecureArray<char> secureArray = secureStringA.ConvertToSecureCharArray())
			{
				using (SecureArray<char> secureArray2 = secureStringB.ConvertToSecureCharArray())
				{
					for (int i = 0; i < secureStringA.Length; i++)
					{
						if (secureArray.ArrayValue[i] != secureArray2.ArrayValue[i])
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		private void ChangePassword()
		{
			SecureHtmlFormReader secureHtmlFormReader = new SecureHtmlFormReader(base.Request);
			secureHtmlFormReader.AddSensitiveInputName("oldPwd");
			secureHtmlFormReader.AddSensitiveInputName("newPwd1");
			secureHtmlFormReader.AddSensitiveInputName("newPwd2");
			SecureNameValueCollection secureNameValueCollection = null;
			try
			{
				if (secureHtmlFormReader.TryReadSecureFormData(out secureNameValueCollection))
				{
					string text = null;
					SecureString secureString = null;
					SecureString secureString2 = null;
					SecureString secureString3 = null;
					try
					{
						secureNameValueCollection.TryGetUnsecureValue("username", out text);
						secureNameValueCollection.TryGetSecureValue("oldPwd", out secureString);
						secureNameValueCollection.TryGetSecureValue("newPwd1", out secureString2);
						secureNameValueCollection.TryGetSecureValue("newPwd2", out secureString3);
						if (text != null && secureString != null && secureString2 != null && secureString3 != null)
						{
							if (!ExpiredPassword.SecureStringEquals(secureString2, secureString3))
							{
								this.reason = ExpiredPassword.ExpiredPasswordReason.PasswordConflict;
							}
							else
							{
								switch (ExpiredPassword.ChangePasswordNUCP(text, secureString, secureString2))
								{
								case ExpiredPassword.ChangePasswordResult.Success:
									this.reason = ExpiredPassword.ExpiredPasswordReason.None;
									this.passwordChanged = true;
									break;
								case ExpiredPassword.ChangePasswordResult.InvalidCredentials:
									this.reason = ExpiredPassword.ExpiredPasswordReason.InvalidCredentials;
									break;
								case ExpiredPassword.ChangePasswordResult.LockedOut:
									this.reason = ExpiredPassword.ExpiredPasswordReason.LockedOut;
									break;
								case ExpiredPassword.ChangePasswordResult.BadNewPassword:
									this.reason = ExpiredPassword.ExpiredPasswordReason.InvalidNewPassword;
									break;
								case ExpiredPassword.ChangePasswordResult.OtherError:
									this.reason = ExpiredPassword.ExpiredPasswordReason.InvalidCredentials;
									break;
								}
							}
						}
					}
					finally
					{
						secureString.Dispose();
						secureString2.Dispose();
						secureString3.Dispose();
					}
				}
			}
			finally
			{
				if (secureNameValueCollection != null)
				{
					secureNameValueCollection.Dispose();
				}
			}
		}

		private const string DestinationParameter = "url";

		private const string DefaultDestination = "../";

		private const string UsernameParameter = "username";

		private const string OldPasswordParameter = "oldPwd";

		private const string NewPassword1Parameter = "newPwd1";

		private const string NewPassword2Parameter = "newPwd2";

		private const int NetUserChangePasswordSuccess = 0;

		private const int NetUserChangePasswordAccessDenied = 5;

		private const int NetUserChangePasswordInvalidOldPassword = 86;

		private const int NetUserChangePasswordDoesNotMeetPolicyRequirement = 2245;

		private ExpiredPassword.ExpiredPasswordReason reason;

		private bool passwordChanged;

		protected enum ChangePasswordResult
		{
			Success,
			InvalidCredentials,
			LockedOut,
			BadNewPassword,
			OtherError
		}

		protected enum ExpiredPasswordReason
		{
			None,
			InvalidCredentials,
			InvalidNewPassword,
			PasswordConflict,
			LockedOut
		}
	}
}
