using System;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public class ChangePassword : OwaPage, IRegistryOnlyForm
	{
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			try
			{
				if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.IsLogonFormatEmail.Enabled)
				{
					this.userName = Utilities.HtmlEncode(base.OwaContext.LogonIdentity.PrimarySmtpAddress.ToString());
				}
				else
				{
					this.userName = Utilities.HtmlEncode(base.OwaContext.LogonIdentity.GetLogonName());
				}
			}
			catch (OwaIdentityException innerException)
			{
				throw new OwaChangePasswordTransientException(Strings.ChangePasswordFailedGetName, innerException);
			}
		}

		protected string UserName
		{
			get
			{
				return this.userName;
			}
		}

		protected string UserNameLabel
		{
			get
			{
				if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).OwaDeployment.IsLogonFormatEmail.Enabled)
				{
					return LocalizedStrings.GetHtmlEncoded(-1568335488);
				}
				return LocalizedStrings.GetHtmlEncoded(50262124);
			}
		}

		private string userName;
	}
}
