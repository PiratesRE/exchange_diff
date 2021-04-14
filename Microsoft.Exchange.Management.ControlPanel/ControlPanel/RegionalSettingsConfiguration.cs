using System;
using System.Globalization;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RegionalSettingsConfiguration : BaseRow
	{
		public RegionalSettingsConfiguration(MailboxRegionalConfiguration mailboxRegionalConfiguration) : base(mailboxRegionalConfiguration)
		{
			this.MailboxRegionalConfiguration = mailboxRegionalConfiguration;
		}

		public MailboxRegionalConfiguration MailboxRegionalConfiguration { get; private set; }

		public MailboxCalendarConfiguration MailboxCalendarConfiguration { get; set; }

		private bool IsUserLanguageSupported
		{
			get
			{
				return this.MailboxRegionalConfiguration.Language != null && Culture.IsSupportedCulture(this.MailboxRegionalConfiguration.Language);
			}
		}

		[DataMember]
		public string DateFormat
		{
			get
			{
				if (this.IsUserLanguageSupported && this.MailboxRegionalConfiguration.DateFormat != null)
				{
					return this.MailboxRegionalConfiguration.DateFormat;
				}
				if (this.UserCulture == null)
				{
					return null;
				}
				return this.UserCulture.DateTimeFormat.ShortDatePattern;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		internal CultureInfo UserCulture
		{
			get
			{
				if (this.userCulture == null)
				{
					this.userCulture = (this.IsUserLanguageSupported ? Culture.GetCultureInfoInstance(this.MailboxRegionalConfiguration.Language.LCID) : Culture.GetPreferredCulture(LocalSession.Current.RbacConfiguration.ExecutingUserLanguages));
				}
				return this.userCulture;
			}
		}

		[DataMember]
		public int Language
		{
			get
			{
				return this.UserCulture.LCID;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool DefaultFolderNameMatchingUserLanguage
		{
			get
			{
				return this.MailboxRegionalConfiguration.DefaultFolderNameMatchingUserLanguage;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string TimeFormat
		{
			get
			{
				if (this.IsUserLanguageSupported && this.MailboxRegionalConfiguration.TimeFormat != null)
				{
					return this.MailboxRegionalConfiguration.TimeFormat;
				}
				if (this.UserCulture == null)
				{
					return null;
				}
				return this.UserCulture.DateTimeFormat.ShortTimePattern;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string TimeZone
		{
			get
			{
				if (this.MailboxRegionalConfiguration.TimeZone != null)
				{
					return this.MailboxRegionalConfiguration.TimeZone.ToString();
				}
				return null;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string WorkingHoursTimeZone
		{
			get
			{
				if (this.MailboxCalendarConfiguration != null && this.MailboxCalendarConfiguration.WorkingHoursTimeZone != null)
				{
					return this.MailboxCalendarConfiguration.WorkingHoursTimeZone.ToString();
				}
				return null;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private CultureInfo userCulture;
	}
}
