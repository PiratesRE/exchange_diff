using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Collections;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class ContentFilterConfig : MessageHygieneAgentConfig
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ContentFilterConfig.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchMessageHygieneContentFilterConfig";
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		[Parameter(Mandatory = false)]
		public AsciiString RejectionResponse
		{
			get
			{
				return (AsciiString)this[ContentFilterConfigSchema.RejectionResponse];
			}
			set
			{
				this[ContentFilterConfigSchema.RejectionResponse] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool OutlookEmailPostmarkValidationEnabled
		{
			get
			{
				return (bool)this[ContentFilterConfigSchema.OutlookEmailPostmarkValidationEnabled];
			}
			set
			{
				this[ContentFilterConfigSchema.OutlookEmailPostmarkValidationEnabled] = value;
			}
		}

		[ValidateCount(0, 800)]
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<SmtpAddress> BypassedRecipients
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[ContentFilterConfigSchema.BypassedRecipients];
			}
			set
			{
				this[ContentFilterConfigSchema.BypassedRecipients] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress? QuarantineMailbox
		{
			get
			{
				return (SmtpAddress?)this[ContentFilterConfigSchema.QuarantineMailbox];
			}
			set
			{
				this[ContentFilterConfigSchema.QuarantineMailbox] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int SCLRejectThreshold
		{
			get
			{
				return (int)this[ContentFilterConfigSchema.SCLRejectThreshold];
			}
			set
			{
				this[ContentFilterConfigSchema.SCLRejectThreshold] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SCLRejectEnabled
		{
			get
			{
				return (bool)this[ContentFilterConfigSchema.SCLRejectEnabled];
			}
			set
			{
				this[ContentFilterConfigSchema.SCLRejectEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int SCLDeleteThreshold
		{
			get
			{
				return (int)this[ContentFilterConfigSchema.SCLDeleteThreshold];
			}
			set
			{
				this[ContentFilterConfigSchema.SCLDeleteThreshold] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SCLDeleteEnabled
		{
			get
			{
				return (bool)this[ContentFilterConfigSchema.SCLDeleteEnabled];
			}
			set
			{
				this[ContentFilterConfigSchema.SCLDeleteEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int SCLQuarantineThreshold
		{
			get
			{
				return (int)this[ContentFilterConfigSchema.SCLQuarantineThreshold];
			}
			set
			{
				this[ContentFilterConfigSchema.SCLQuarantineThreshold] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SCLQuarantineEnabled
		{
			get
			{
				return (bool)this[ContentFilterConfigSchema.SCLQuarantineEnabled];
			}
			set
			{
				this[ContentFilterConfigSchema.SCLQuarantineEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateCount(0, 800)]
		public MultiValuedProperty<SmtpAddress> BypassedSenders
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this[ContentFilterConfigSchema.BypassedSenders];
			}
			set
			{
				this[ContentFilterConfigSchema.BypassedSenders] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateCount(0, 800)]
		public MultiValuedProperty<SmtpDomainWithSubdomains> BypassedSenderDomains
		{
			get
			{
				return (MultiValuedProperty<SmtpDomainWithSubdomains>)this[ContentFilterConfigSchema.BypassedSenderDomains];
			}
			set
			{
				this[ContentFilterConfigSchema.BypassedSenderDomains] = value;
			}
		}

		internal ReadOnlyCollection<ContentFilterPhrase> GetPhrases()
		{
			List<ContentFilterPhrase> list = new List<ContentFilterPhrase>();
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)this[ContentFilterConfigSchema.EncodedPhrases];
			if (multiValuedProperty != null)
			{
				foreach (string encoded in multiValuedProperty)
				{
					try
					{
						list.Add(ContentFilterPhrase.Decode(encoded));
					}
					catch (FormatException)
					{
					}
				}
			}
			return new ReadOnlyCollection<ContentFilterPhrase>(list);
		}

		internal void AddPhrase(ContentFilterPhrase phrase)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)this[ContentFilterConfigSchema.EncodedPhrases];
			if (multiValuedProperty == null)
			{
				multiValuedProperty = new MultiValuedProperty<string>();
			}
			multiValuedProperty.Add(phrase.Encode());
			this[ContentFilterConfigSchema.EncodedPhrases] = multiValuedProperty;
		}

		internal void RemovePhrase(ContentFilterPhrase phrase)
		{
			MultiValuedProperty<string> multiValuedProperty = (MultiValuedProperty<string>)this[ContentFilterConfigSchema.EncodedPhrases];
			if (multiValuedProperty == null)
			{
				multiValuedProperty = new MultiValuedProperty<string>();
			}
			multiValuedProperty.Remove(phrase.Encode());
			this[ContentFilterConfigSchema.EncodedPhrases] = multiValuedProperty;
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (this.SCLDeleteEnabled)
			{
				if (this.SCLRejectEnabled && this.SCLDeleteThreshold <= this.SCLRejectThreshold)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.DeleteAndRejectThreshold, ContentFilterConfigSchema.SCLDeleteThreshold, this));
				}
				if (this.SCLQuarantineEnabled && this.SCLDeleteThreshold <= this.SCLQuarantineThreshold)
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.DeleteAndQuarantineThreshold, ContentFilterConfigSchema.SCLDeleteThreshold, this));
				}
			}
			if (this.SCLRejectEnabled && this.SCLQuarantineEnabled && this.SCLRejectThreshold <= this.SCLQuarantineThreshold)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.RejectAndQuarantineThreshold, ContentFilterConfigSchema.SCLRejectThreshold, this));
			}
			bool flag = this.QuarantineMailbox != null && this.QuarantineMailbox != SmtpAddress.Empty;
			bool flag2 = flag && this.QuarantineMailbox != SmtpAddress.NullReversePath && this.QuarantineMailbox.Value.IsValidAddress;
			if ((flag || this.SCLQuarantineEnabled) && !flag2)
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.QuarantineMailboxIsInvalid, ContentFilterConfigSchema.QuarantineMailbox, this));
			}
			base.ValidateMaximumCollectionCount(errors, this.BypassedRecipients, 800, ContentFilterConfigSchema.BypassedRecipients);
			base.ValidateMaximumCollectionCount(errors, this.BypassedSenderDomains, 800, ContentFilterConfigSchema.BypassedSenderDomains);
			base.ValidateMaximumCollectionCount(errors, this.BypassedSenders, 800, ContentFilterConfigSchema.BypassedSenders);
		}

		public const string CanonicalName = "ContentFilterConfig";

		private const string MostDerivedClass = "msExchMessageHygieneContentFilterConfig";

		private static readonly ContentFilterConfigSchema schema = ObjectSchema.GetInstance<ContentFilterConfigSchema>();

		private static class Validation
		{
			public const int MaxBypassedRecipients = 800;

			public const int MaxBypassedSenders = 800;

			public const int MaxBypassedSenderDomains = 800;
		}
	}
}
