using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.UM.UMCommon
{
	[Serializable]
	public class UMMailboxConfiguration : ConfigurableObject
	{
		public UMMailboxConfiguration(ObjectId identity) : base(new SimpleProviderPropertyBag())
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			this.propertyBag.SetField(SimpleProviderObjectSchema.Identity, identity);
		}

		public MailboxGreetingEnum Greeting
		{
			get
			{
				return (MailboxGreetingEnum)this[UMMailboxConfigurationSchema.Greeting];
			}
			internal set
			{
				this[UMMailboxConfigurationSchema.Greeting] = value;
			}
		}

		public MailboxFolder FolderToReadEmailsFrom
		{
			get
			{
				return (MailboxFolder)this[UMMailboxConfigurationSchema.FolderToReadEmailsFrom];
			}
			internal set
			{
				this[UMMailboxConfigurationSchema.FolderToReadEmailsFrom] = value;
			}
		}

		public bool ReadOldestUnreadVoiceMessagesFirst
		{
			get
			{
				return (bool)this[UMMailboxConfigurationSchema.ReadOldestUnreadVoiceMessagesFirst];
			}
			internal set
			{
				this[UMMailboxConfigurationSchema.ReadOldestUnreadVoiceMessagesFirst] = value;
			}
		}

		public string DefaultPlayOnPhoneNumber
		{
			get
			{
				return (string)this[UMMailboxConfigurationSchema.DefaultPlayOnPhoneNumber];
			}
			internal set
			{
				this[UMMailboxConfigurationSchema.DefaultPlayOnPhoneNumber] = value;
			}
		}

		public bool ReceivedVoiceMailPreviewEnabled
		{
			get
			{
				return (bool)this[UMMailboxConfigurationSchema.ReceivedVoiceMailPreviewEnabled];
			}
			internal set
			{
				this[UMMailboxConfigurationSchema.ReceivedVoiceMailPreviewEnabled] = value;
			}
		}

		public bool SentVoiceMailPreviewEnabled
		{
			get
			{
				return (bool)this[UMMailboxConfigurationSchema.SentVoiceMailPreviewEnabled];
			}
			internal set
			{
				this[UMMailboxConfigurationSchema.SentVoiceMailPreviewEnabled] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return UMMailboxConfiguration.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public const string GreetingParameterName = "Greeting";

		public const string FolderToReadEmailsFromParameterName = "FolderToReadEmailsFrom";

		public const string ReadOldestUnreadVoiceMessageFirstParameterName = "ReadOldestUnreadVoiceMessageFirst";

		public const string DefaultPlayOnPhoneNumberParameterName = "DefaultPlayOnPhoneNumber";

		public const string ReceivedVoiceMailPreviewEnabledParameterName = "ReceivedVoiceMailPreviewEnabled";

		public const string SentVoiceMailPreviewEnabledParameterName = "SentVoiceMailPreviewEnabled";

		private static UMMailboxConfigurationSchema schema = ObjectSchema.GetInstance<UMMailboxConfigurationSchema>();
	}
}
