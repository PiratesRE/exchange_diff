using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class RecipientFilterConfig : MessageHygieneAgentConfig
	{
		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return RecipientFilterConfig.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchMessageHygieneRecipientFilterConfig";
			}
		}

		[ValidateCount(0, 800)]
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<SmtpAddress> BlockedRecipients
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[RecipientFilterConfigSchema.BlockedRecipients];
			}
			set
			{
				this[RecipientFilterConfigSchema.BlockedRecipients] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RecipientValidationEnabled
		{
			get
			{
				return (bool)this[RecipientFilterConfigSchema.RecipientValidationEnabled];
			}
			set
			{
				this[RecipientFilterConfigSchema.RecipientValidationEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool BlockListEnabled
		{
			get
			{
				return (bool)this[RecipientFilterConfigSchema.BlockListEnabled];
			}
			set
			{
				this[RecipientFilterConfigSchema.BlockListEnabled] = value;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			base.ValidateMaximumCollectionCount(errors, this.BlockedRecipients, 800, RecipientFilterConfigSchema.BlockedRecipients);
		}

		public const string CanonicalName = "RecipientFilterConfig";

		private const string MostDerivedClass = "msExchMessageHygieneRecipientFilterConfig";

		private static readonly RecipientFilterConfigSchema schema = ObjectSchema.GetInstance<RecipientFilterConfigSchema>();

		private struct Validation
		{
			public const int MaxBlockedRecipientsLength = 800;
		}
	}
}
