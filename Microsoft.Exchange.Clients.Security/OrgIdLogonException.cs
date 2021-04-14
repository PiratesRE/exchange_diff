using System;
using System.Globalization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Clients.Security
{
	public abstract class OrgIdLogonException : LocalizedException
	{
		protected abstract string ErrorMessageFormatString { get; }

		public abstract Strings.IDs ErrorMessageStringId { get; }

		protected OrgIdLogonException(string userName, string logoutLink) : base(new LocalizedString(null), null)
		{
			if (string.IsNullOrWhiteSpace(userName))
			{
				throw new ArgumentNullException("userName");
			}
			this.userName = userName;
			this.logoutLink = logoutLink;
		}

		public override string Message
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, this.ErrorMessageFormatString, new object[]
				{
					this.userName,
					this.logoutLink
				});
			}
		}

		public string UserName
		{
			get
			{
				return this.userName;
			}
		}

		public string LogoutLink
		{
			get
			{
				return this.logoutLink;
			}
		}

		private readonly string userName;

		private readonly string logoutLink;
	}
}
