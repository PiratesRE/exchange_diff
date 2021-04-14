using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MailboxRegionalConfiguration : UserConfigurationObject
	{
		internal override UserConfigurationObjectSchema Schema
		{
			get
			{
				return MailboxRegionalConfiguration.schema;
			}
		}

		[Parameter(Mandatory = false)]
		public string DateFormat
		{
			get
			{
				return (string)this[MailboxRegionalConfigurationSchema.DateFormat];
			}
			set
			{
				this[MailboxRegionalConfigurationSchema.DateFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNull]
		public CultureInfo Language
		{
			get
			{
				return (CultureInfo)this[MailboxRegionalConfigurationSchema.Language];
			}
			set
			{
				this[MailboxRegionalConfigurationSchema.Language] = value;
			}
		}

		public bool DefaultFolderNameMatchingUserLanguage
		{
			get
			{
				return (bool)this[MailboxRegionalConfigurationSchema.DefaultFolderNameMatchingUserLanguage];
			}
			internal set
			{
				this[MailboxRegionalConfigurationSchema.DefaultFolderNameMatchingUserLanguage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string TimeFormat
		{
			get
			{
				return (string)this[MailboxRegionalConfigurationSchema.TimeFormat];
			}
			set
			{
				this[MailboxRegionalConfigurationSchema.TimeFormat] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ExTimeZoneValue TimeZone
		{
			get
			{
				return (ExTimeZoneValue)this[MailboxRegionalConfigurationSchema.TimeZone];
			}
			set
			{
				this[MailboxRegionalConfigurationSchema.TimeZone] = value;
			}
		}

		public static string GetDefaultDateFormat(CultureInfo language)
		{
			return MailboxRegionalConfiguration.GetDateTimeInfo(language).ShortDatePattern;
		}

		public static string GetDefaultTimeFormat(CultureInfo language)
		{
			return MailboxRegionalConfiguration.GetDateTimeInfo(language).ShortTimePattern;
		}

		public static bool ValidateDateFormat(CultureInfo language, object dateFormat, out string defaultFormat)
		{
			DateTimeFormatInfo dateTimeInfo = MailboxRegionalConfiguration.GetDateTimeInfo(language);
			defaultFormat = dateTimeInfo.ShortDatePattern;
			string[] array;
			return dateFormat != null && dateFormat is string && MailboxRegionalConfiguration.ValidateFormat(dateTimeInfo, (string)dateFormat, 'd', out array);
		}

		public static bool ValidateTimeFormat(CultureInfo language, object timeFormat, out string defaultFormat)
		{
			DateTimeFormatInfo dateTimeInfo = MailboxRegionalConfiguration.GetDateTimeInfo(language);
			defaultFormat = dateTimeInfo.ShortTimePattern;
			string[] array;
			return timeFormat != null && timeFormat is string && MailboxRegionalConfiguration.ValidateFormat(dateTimeInfo, (string)timeFormat, 't', out array);
		}

		internal static DateTimeFormatInfo GetDateTimeInfo(CultureInfo language)
		{
			return new CultureInfo(language.LCID)
			{
				DateTimeFormat = 
				{
					Calendar = new GregorianCalendar()
				}
			}.DateTimeFormat;
		}

		internal static bool ValidateFormat(DateTimeFormatInfo dateTimeFormatInfo, string format, char dateTimePatternSelector, out string[] validFormats)
		{
			validFormats = dateTimeFormatInfo.GetAllDateTimePatterns(dateTimePatternSelector);
			return Array.Exists<string>(validFormats, (string value) => value == format.Trim());
		}

		internal static object DateFormatGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[MailboxRegionalConfigurationSchema.RawDateFormat];
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			CultureInfo cultureInfo = (CultureInfo)propertyBag[MailboxRegionalConfigurationSchema.Language];
			if (cultureInfo != null)
			{
				return cultureInfo.DateTimeFormat.ShortDatePattern;
			}
			return null;
		}

		internal static void DateFormatSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[MailboxRegionalConfigurationSchema.RawDateFormat] = value;
		}

		internal static object TimeFormatGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[MailboxRegionalConfigurationSchema.RawTimeFormat];
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			CultureInfo cultureInfo = (CultureInfo)propertyBag[MailboxRegionalConfigurationSchema.Language];
			if (cultureInfo != null)
			{
				return cultureInfo.DateTimeFormat.ShortTimePattern;
			}
			return null;
		}

		internal static void TimeFormatSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[MailboxRegionalConfigurationSchema.RawTimeFormat] = value;
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (base.IsModified(MailboxRegionalConfigurationSchema.Language) && this.Language == null)
			{
				errors.Add(new ObjectValidationError(ServerStrings.ErrorLanguageIsNull, this.Identity, string.Empty));
			}
			if (base.IsModified(MailboxRegionalConfigurationSchema.Language) || base.IsModified(MailboxRegionalConfigurationSchema.DateFormat) || base.IsModified(MailboxRegionalConfigurationSchema.TimeFormat))
			{
				if (this.Language != null)
				{
					this.Language.DateTimeFormat.Calendar = new GregorianCalendar();
					string[] value;
					if (!MailboxRegionalConfiguration.ValidateFormat(this.Language.DateTimeFormat, this.DateFormat, 'd', out value))
					{
						errors.Add(new ObjectValidationError(ServerStrings.ErrorInvalidDateFormat(this.DateFormat, this.Language.ToString(), string.Join(", ", value)), this.Identity, string.Empty));
					}
					string[] value2;
					if (!MailboxRegionalConfiguration.ValidateFormat(this.Language.DateTimeFormat, this.TimeFormat, 't', out value2))
					{
						errors.Add(new ObjectValidationError(ServerStrings.ErrorInvalidTimeFormat(this.TimeFormat, this.Language.ToString(), string.Join(", ", value2)), this.Identity, string.Empty));
						return;
					}
				}
				else if (!string.IsNullOrEmpty(this.DateFormat) || !string.IsNullOrEmpty(this.TimeFormat))
				{
					errors.Add(new ObjectValidationError(ServerStrings.ErrorSetDateTimeFormatWithoutLanguage, this.Identity, string.Empty));
				}
			}
		}

		public override IConfigurable Read(MailboxStoreTypeProvider session, ObjectId identity)
		{
			base.Principal = ExchangePrincipal.FromADUser(session.ADUser, null);
			using (UserConfigurationDictionaryAdapter<MailboxRegionalConfiguration> userConfigurationDictionaryAdapter = new UserConfigurationDictionaryAdapter<MailboxRegionalConfiguration>(session.MailboxSession, "OWA.UserOptions", new GetUserConfigurationDelegate(UserConfigurationHelper.GetMailboxConfiguration), MailboxRegionalConfiguration.mailboxProperties))
			{
				userConfigurationDictionaryAdapter.Fill(this);
			}
			if (base.Principal.PreferredCultures.Any<CultureInfo>())
			{
				this.Language = base.Principal.PreferredCultures.First<CultureInfo>();
			}
			return this;
		}

		public override void Save(MailboxStoreTypeProvider session)
		{
			using (UserConfigurationDictionaryAdapter<MailboxRegionalConfiguration> userConfigurationDictionaryAdapter = new UserConfigurationDictionaryAdapter<MailboxRegionalConfiguration>(session.MailboxSession, "OWA.UserOptions", new GetUserConfigurationDelegate(UserConfigurationHelper.GetMailboxConfiguration), MailboxRegionalConfiguration.mailboxProperties))
			{
				userConfigurationDictionaryAdapter.Save(this);
			}
			if (base.IsModified(MailboxRegionalConfigurationSchema.Language) && this.Language != null)
			{
				this.SaveCultures(session.MailboxSession.GetADRecipientSession(false, ConsistencyMode.FullyConsistent));
			}
			base.ResetChangeTracking();
		}

		private void SaveCultures(IRecipientSession adRecipientSession)
		{
			PreferredCultures preferredCultures = new PreferredCultures(base.Principal.PreferredCultures);
			preferredCultures.AddSupportedCulture(this.Language, (CultureInfo culture) => true);
			ADUser aduser = adRecipientSession.Read(base.Principal.ObjectId) as ADUser;
			if (aduser != null)
			{
				aduser.Languages.Clear();
				Util.AddRange<CultureInfo, CultureInfo>(aduser.Languages, preferredCultures);
				try
				{
					adRecipientSession.Save(aduser);
				}
				catch (DataValidationException innerException)
				{
					throw new CorruptDataException(ServerStrings.ExCannotSaveInvalidObject(aduser), innerException);
				}
				catch (DataSourceOperationException ex)
				{
					throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex, null, "MailboxRegionalConfiguration::SaveCultures. Failed due to directory exception {0}.", new object[]
					{
						ex
					});
				}
				catch (DataSourceTransientException ex2)
				{
					throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex2, null, "MailboxRegionalConfiguration::SaveCultures. Failed due to directory exception {0}.", new object[]
					{
						ex2
					});
				}
			}
		}

		private static MailboxRegionalConfigurationSchema schema = ObjectSchema.GetInstance<MailboxRegionalConfigurationSchema>();

		private static readonly SimplePropertyDefinition[] mailboxProperties = new SimplePropertyDefinition[]
		{
			MailboxRegionalConfigurationSchema.DateFormat,
			MailboxRegionalConfigurationSchema.TimeFormat,
			MailboxRegionalConfigurationSchema.TimeZone
		};
	}
}
