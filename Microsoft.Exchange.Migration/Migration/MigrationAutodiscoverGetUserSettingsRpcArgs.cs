using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MigrationAutodiscoverGetUserSettingsRpcArgs : MigrationProxyRpcArgs
	{
		public MigrationAutodiscoverGetUserSettingsRpcArgs(string userName, string encryptedPassword, string userDomain, string autodiscoverDomain, Uri autodiscoverUrl, ExchangeVersion? exchangeVersion, string userSmtpAddress) : base(userName, encryptedPassword, userDomain, MigrationProxyRpcType.GetUserSettings)
		{
			this.AutodiscoverDomain = autodiscoverDomain;
			this.AutodiscoverUrl = autodiscoverUrl;
			this.ExchangeVersion = exchangeVersion;
			this.UserSmtpAddress = userSmtpAddress;
		}

		public MigrationAutodiscoverGetUserSettingsRpcArgs(byte[] requestBlob) : base(requestBlob, MigrationProxyRpcType.GetUserSettings)
		{
		}

		public string AutodiscoverDomain
		{
			get
			{
				return base.GetProperty<string>(2416574495U);
			}
			set
			{
				base.SetPropertyAsString(2416574495U, value);
			}
		}

		public Uri AutodiscoverUrl
		{
			get
			{
				string property = base.GetProperty<string>(2416640031U);
				Uri result;
				if (Uri.TryCreate(property, UriKind.Absolute, out result))
				{
					return result;
				}
				return null;
			}
			set
			{
				string value2 = null;
				if (value != null)
				{
					value2 = value.AbsoluteUri;
				}
				base.SetProperty(2416640031U, value2);
			}
		}

		public ExchangeVersion? ExchangeVersion
		{
			get
			{
				object obj;
				if (this.PropertyCollection.TryGetValue(2416705539U, out obj) && obj is int)
				{
					return new ExchangeVersion?((ExchangeVersion)obj);
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.PropertyCollection[2416705539U] = value.Value;
					return;
				}
				this.PropertyCollection.Remove(2416705539U);
			}
		}

		public string UserSmtpAddress
		{
			get
			{
				return base.GetProperty<string>(2416508959U);
			}
			set
			{
				base.SetPropertyAsString(2416508959U, value);
			}
		}

		public override bool Validate(out string errorMsg)
		{
			if (!base.Validate(out errorMsg))
			{
				return false;
			}
			if (string.IsNullOrEmpty(this.AutodiscoverDomain))
			{
				errorMsg = "Autodiscover Domain cannot be null or empty.";
				return false;
			}
			if (string.IsNullOrEmpty(this.UserSmtpAddress))
			{
				errorMsg = "Autodiscover User Smtp Address cannot be null or empty.";
				return false;
			}
			errorMsg = null;
			return true;
		}
	}
}
