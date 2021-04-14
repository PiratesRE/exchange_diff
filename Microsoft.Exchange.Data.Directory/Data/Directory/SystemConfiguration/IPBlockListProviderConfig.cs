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
	public sealed class IPBlockListProviderConfig : MessageHygieneAgentConfig
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
				return IPBlockListProviderConfig.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return IPBlockListProviderConfig.mostDerivedClass;
			}
		}

		[ValidateCount(0, 800)]
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<SmtpAddress> BypassedRecipients
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[IPBlockListProviderConfigSchema.BypassedRecipients];
			}
			set
			{
				this[IPBlockListProviderConfigSchema.BypassedRecipients] = value;
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			base.ValidateMaximumCollectionCount(errors, this.BypassedRecipients, 800, IPBlockListProviderConfigSchema.BypassedRecipients);
		}

		public const string CanonicalName = "IPBlockListProviderConfig";

		private static IPBlockListProviderConfigSchema schema = ObjectSchema.GetInstance<IPBlockListProviderConfigSchema>();

		private static string mostDerivedClass = "msExchMessageHygieneIPBlockListProviderConfig";

		private struct Validation
		{
			public const int MaxBypassedRecipientsLength = 800;
		}
	}
}
