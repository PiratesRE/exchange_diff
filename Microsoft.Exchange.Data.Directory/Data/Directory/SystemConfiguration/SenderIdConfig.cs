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
	public sealed class SenderIdConfig : MessageHygieneAgentConfig
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return SenderIdConfig.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchMessageHygieneSenderIDConfig";
			}
		}

		[Parameter(Mandatory = false)]
		public SenderIdAction SpoofedDomainAction
		{
			get
			{
				return (SenderIdAction)this[SenderIdConfigSchema.SpoofedDomainAction];
			}
			set
			{
				this[SenderIdConfigSchema.SpoofedDomainAction] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SenderIdAction TempErrorAction
		{
			get
			{
				return (SenderIdAction)this[SenderIdConfigSchema.TempErrorAction];
			}
			set
			{
				this[SenderIdConfigSchema.TempErrorAction] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateCount(0, 800)]
		public MultiValuedProperty<SmtpAddress> BypassedRecipients
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[SenderIdConfigSchema.BypassedRecipients];
			}
			set
			{
				this[SenderIdConfigSchema.BypassedRecipients] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateCount(0, 800)]
		public MultiValuedProperty<SmtpDomain> BypassedSenderDomains
		{
			get
			{
				return (MultiValuedProperty<SmtpDomain>)this[SenderIdConfigSchema.BypassedSenderDomains];
			}
			set
			{
				this[SenderIdConfigSchema.BypassedSenderDomains] = value;
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.TempErrorAction == SenderIdAction.Delete)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.InvalidTempErrorSetting, SenderIdConfigSchema.TempErrorAction, this));
			}
			base.ValidateMaximumCollectionCount(errors, this.BypassedRecipients, 800, SenderIdConfigSchema.BypassedRecipients);
			base.ValidateMaximumCollectionCount(errors, this.BypassedSenderDomains, 800, SenderIdConfigSchema.BypassedSenderDomains);
		}

		public const string CanonicalName = "SenderIdConfig";

		private const string MostDerivedClass = "msExchMessageHygieneSenderIDConfig";

		private static SenderIdConfigSchema schema = ObjectSchema.GetInstance<SenderIdConfigSchema>();

		private struct Validation
		{
			public const int MaxBypassedRecipients = 800;

			public const int MaxBypassedSenderDomains = 800;
		}
	}
}
