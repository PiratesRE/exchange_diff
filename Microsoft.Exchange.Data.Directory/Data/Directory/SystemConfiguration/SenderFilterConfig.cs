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
	public sealed class SenderFilterConfig : MessageHygieneAgentConfig
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
				return SenderFilterConfig.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchMessageHygieneSenderFilterConfig";
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateCount(0, 800)]
		public MultiValuedProperty<SmtpAddress> BlockedSenders
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[SenderFilterConfigSchema.BlockedSenders];
			}
			set
			{
				this[SenderFilterConfigSchema.BlockedSenders] = value;
			}
		}

		[ValidateCount(0, 800)]
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<SmtpDomain> BlockedDomains
		{
			get
			{
				return (MultiValuedProperty<SmtpDomain>)this[SenderFilterConfigSchema.BlockedDomains];
			}
			set
			{
				this[SenderFilterConfigSchema.BlockedDomains] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateCount(0, 800)]
		public MultiValuedProperty<SmtpDomain> BlockedDomainsAndSubdomains
		{
			get
			{
				return (MultiValuedProperty<SmtpDomain>)this[SenderFilterConfigSchema.BlockedDomainAndSubdomains];
			}
			set
			{
				this[SenderFilterConfigSchema.BlockedDomainAndSubdomains] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public BlockedSenderAction Action
		{
			get
			{
				return (BlockedSenderAction)this[SenderFilterConfigSchema.BlockedSenderAction];
			}
			set
			{
				this[SenderFilterConfigSchema.BlockedSenderAction] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool BlankSenderBlockingEnabled
		{
			get
			{
				return (bool)this[SenderFilterConfigSchema.BlockBlankSenders];
			}
			set
			{
				this[SenderFilterConfigSchema.BlockBlankSenders] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientBlockedSenderAction RecipientBlockedSenderAction
		{
			get
			{
				return (RecipientBlockedSenderAction)this[SenderFilterConfigSchema.RecipientBlockedSenderAction];
			}
			set
			{
				this[SenderFilterConfigSchema.RecipientBlockedSenderAction] = value;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			base.ValidateMaximumCollectionCount(errors, this.BlockedSenders, 800, SenderFilterConfigSchema.BlockedSenders);
			base.ValidateMaximumCollectionCount(errors, this.BlockedDomains, 800, SenderFilterConfigSchema.BlockedDomains);
			base.ValidateMaximumCollectionCount(errors, this.BlockedDomainsAndSubdomains, 800, SenderFilterConfigSchema.BlockedDomainAndSubdomains);
		}

		public const string CanonicalName = "SenderFilterConfig";

		private const string MostDerivedClass = "msExchMessageHygieneSenderFilterConfig";

		private static readonly SenderFilterConfigSchema schema = ObjectSchema.GetInstance<SenderFilterConfigSchema>();

		private static class Validation
		{
			public const int MaxBlockedSendersLength = 800;

			public const int MaxBlockedDomainsLength = 800;

			public const int MaxBlockedDomainsAndSubdomainsLength = 800;
		}
	}
}
