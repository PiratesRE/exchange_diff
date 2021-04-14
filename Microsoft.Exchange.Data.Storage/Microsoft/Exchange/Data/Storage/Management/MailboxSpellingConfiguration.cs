using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class MailboxSpellingConfiguration : UserConfigurationObject
	{
		internal override UserConfigurationObjectSchema Schema
		{
			get
			{
				return MailboxSpellingConfiguration.schema;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CheckBeforeSend
		{
			get
			{
				return (bool)this[MailboxSpellingConfigurationSchema.CheckBeforeSend];
			}
			set
			{
				this[MailboxSpellingConfigurationSchema.CheckBeforeSend] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SpellcheckerSupportedLanguage DictionaryLanguage
		{
			get
			{
				return (SpellcheckerSupportedLanguage)this[MailboxSpellingConfigurationSchema.DictionaryLanguage];
			}
			set
			{
				this[MailboxSpellingConfigurationSchema.DictionaryLanguage] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IgnoreUppercase
		{
			get
			{
				return (bool)this[MailboxSpellingConfigurationSchema.IgnoreUppercase];
			}
			set
			{
				this[MailboxSpellingConfigurationSchema.IgnoreUppercase] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IgnoreMixedDigits
		{
			get
			{
				return (bool)this[MailboxSpellingConfigurationSchema.IgnoreMixedDigits];
			}
			set
			{
				this[MailboxSpellingConfigurationSchema.IgnoreMixedDigits] = value;
			}
		}

		public override IConfigurable Read(MailboxStoreTypeProvider session, ObjectId identity)
		{
			base.Principal = ExchangePrincipal.FromADUser(session.ADUser, null);
			IConfigurable result;
			using (UserConfigurationDictionaryAdapter<MailboxSpellingConfiguration> userConfigurationDictionaryAdapter = new UserConfigurationDictionaryAdapter<MailboxSpellingConfiguration>(session.MailboxSession, "OWA.UserOptions", new GetUserConfigurationDelegate(UserConfigurationHelper.GetMailboxConfiguration), MailboxSpellingConfiguration.mailboxProperties))
			{
				result = userConfigurationDictionaryAdapter.Read(base.Principal);
			}
			return result;
		}

		public override void Save(MailboxStoreTypeProvider session)
		{
			using (UserConfigurationDictionaryAdapter<MailboxSpellingConfiguration> userConfigurationDictionaryAdapter = new UserConfigurationDictionaryAdapter<MailboxSpellingConfiguration>(session.MailboxSession, "OWA.UserOptions", new GetUserConfigurationDelegate(UserConfigurationHelper.GetMailboxConfiguration), MailboxSpellingConfiguration.mailboxProperties))
			{
				userConfigurationDictionaryAdapter.Save(this);
			}
			base.ResetChangeTracking();
		}

		private static MailboxSpellingConfigurationSchema schema = ObjectSchema.GetInstance<MailboxSpellingConfigurationSchema>();

		private static SimplePropertyDefinition[] mailboxProperties = new SimplePropertyDefinition[]
		{
			MailboxSpellingConfigurationSchema.CheckBeforeSend,
			MailboxSpellingConfigurationSchema.DictionaryLanguage,
			MailboxSpellingConfigurationSchema.IgnoreMixedDigits,
			MailboxSpellingConfigurationSchema.IgnoreUppercase
		};
	}
}
