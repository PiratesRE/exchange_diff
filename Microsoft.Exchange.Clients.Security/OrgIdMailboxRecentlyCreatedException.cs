using System;
using System.Globalization;
using Microsoft.Exchange.Clients.Common;

namespace Microsoft.Exchange.Clients.Security
{
	public class OrgIdMailboxRecentlyCreatedException : OrgIdMailboxNotFoundException
	{
		protected override string ErrorMessageFormatString
		{
			get
			{
				int hoursBetweenAccountCreationAndNow = this.HoursBetweenAccountCreationAndNow;
				if (hoursBetweenAccountCreationAndNow != 1)
				{
					return Strings.MailboxRecentlyCreatedErrorMessageMoreThanOneHour;
				}
				return Strings.MailboxRecentlyCreatedErrorMessageOneHour;
			}
		}

		public override ErrorMode? ErrorMode
		{
			get
			{
				return new ErrorMode?(Microsoft.Exchange.Clients.Common.ErrorMode.MailboxNotReady);
			}
		}

		public override Strings.IDs ErrorMessageStringId
		{
			get
			{
				int hoursBetweenAccountCreationAndNow = this.HoursBetweenAccountCreationAndNow;
				if (hoursBetweenAccountCreationAndNow != 1)
				{
					return -870357301;
				}
				return -1420330575;
			}
		}

		public override string Message
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, this.ErrorMessageFormatString, new object[]
				{
					base.UserName,
					base.LogoutLink,
					this.HoursBetweenAccountCreationAndNow.ToString()
				});
			}
		}

		public int HoursBetweenAccountCreationAndNow
		{
			get
			{
				if (this.creationTimeSpanToNow.TotalHours < 1.0)
				{
					return 1;
				}
				if (this.creationTimeSpanToNow.TotalHours > 23.0)
				{
					return 23;
				}
				return (int)Math.Round(this.creationTimeSpanToNow.TotalHours);
			}
		}

		public OrgIdMailboxRecentlyCreatedException(string userName, string logoutUrl, TimeSpan creationTimeSpanToNow) : base(userName, logoutUrl)
		{
			this.creationTimeSpanToNow = creationTimeSpanToNow;
		}

		private readonly TimeSpan creationTimeSpanToNow;
	}
}
