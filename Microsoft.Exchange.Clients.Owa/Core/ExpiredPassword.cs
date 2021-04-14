using System;
using System.Security;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Core
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
				string text = Utilities.GetQueryStringParameter(base.Request, "url", false) ?? Utilities.GetFormParameter(base.Request, "url", false);
				if (string.IsNullOrEmpty(text))
				{
					return "../owa14.aspx";
				}
				return text;
			}
		}

		protected string UserNameLabel
		{
			get
			{
				switch (OwaConfigurationManager.Configuration.LogonFormat)
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
				return OwaConfigurationManager.Configuration.ClientAuthCleanupLevel == ClientAuthCleanupLevels.High;
			}
		}

		protected override bool UseStrictMode
		{
			get
			{
				return false;
			}
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			this.reason = ExpiredPassword.ExpiredPasswordReason.None;
			this.passwordChanged = false;
			if (Globals.ChangeExpiredPasswordEnabled)
			{
				this.ChangePassword();
				if (this.passwordChanged)
				{
					Utilities.DeleteFBASessionCookies(base.Response);
				}
			}
		}

		private void ChangePassword()
		{
			string text = Utilities.GetFormParameter(base.Request, "username", false);
			using (SecureString secureFormParameter = Utilities.GetSecureFormParameter(base.Request, "oldPwd", false))
			{
				using (SecureString secureFormParameter2 = Utilities.GetSecureFormParameter(base.Request, "newPwd1", false))
				{
					using (SecureString secureFormParameter3 = Utilities.GetSecureFormParameter(base.Request, "newPwd2", false))
					{
						if (text != null && secureFormParameter != null && secureFormParameter2 != null && secureFormParameter3 != null)
						{
							if (!Utilities.SecureStringEquals(secureFormParameter2, secureFormParameter3))
							{
								this.reason = ExpiredPassword.ExpiredPasswordReason.PasswordConflict;
							}
							else
							{
								if (OwaConfigurationManager.Configuration.LogonFormat == LogonFormats.UserName && text.IndexOf("\\") == -1)
								{
									text = string.Format("{0}\\{1}", OwaConfigurationManager.Configuration.DefaultDomain, text);
								}
								switch (Utilities.ChangePassword(text, secureFormParameter, secureFormParameter2))
								{
								case Utilities.ChangePasswordResult.Success:
									this.reason = ExpiredPassword.ExpiredPasswordReason.None;
									this.passwordChanged = true;
									break;
								case Utilities.ChangePasswordResult.InvalidCredentials:
									this.reason = ExpiredPassword.ExpiredPasswordReason.InvalidCredentials;
									break;
								case Utilities.ChangePasswordResult.LockedOut:
									this.reason = ExpiredPassword.ExpiredPasswordReason.LockedOut;
									break;
								case Utilities.ChangePasswordResult.BadNewPassword:
									this.reason = ExpiredPassword.ExpiredPasswordReason.InvalidNewPassword;
									break;
								case Utilities.ChangePasswordResult.OtherError:
									this.reason = ExpiredPassword.ExpiredPasswordReason.InvalidCredentials;
									break;
								}
							}
						}
					}
				}
			}
		}

		private const string DestinationParameter = "url";

		private const string DefaultDestination = "../owa14.aspx";

		private const string UsernameParameter = "username";

		private const string OldPasswordParameter = "oldPwd";

		private const string NewPassword1Parameter = "newPwd1";

		private const string NewPassword2Parameter = "newPwd2";

		private ExpiredPassword.ExpiredPasswordReason reason;

		private bool passwordChanged;

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
