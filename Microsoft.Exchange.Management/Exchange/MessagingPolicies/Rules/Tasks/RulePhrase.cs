using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public abstract class RulePhrase : ConfigurableObject
	{
		public RulePhrase() : base(new SimpleProviderPropertyBag())
		{
			this.Reset();
		}

		public string Name
		{
			get
			{
				return (string)this.propertyBag[RulePhraseSchema.Name];
			}
			private set
			{
				this.propertyBag[RulePhraseSchema.Name] = value;
			}
		}

		public int Rank
		{
			get
			{
				return (int)this.propertyBag[RulePhraseSchema.Rank];
			}
			private set
			{
				this.propertyBag[RulePhraseSchema.Rank] = value;
			}
		}

		public string LinkedDisplayText
		{
			get
			{
				return (string)this.propertyBag[RulePhraseSchema.LinkedDisplayText];
			}
			private set
			{
				this.propertyBag[RulePhraseSchema.LinkedDisplayText] = value;
			}
		}

		internal int MaxDescriptionListLength { get; set; }

		internal virtual void Reset()
		{
			this.MaxDescriptionListLength = 200;
		}

		internal abstract string Description { get; }

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return RulePhrase.schema;
			}
		}

		internal void SetReadOnlyProperties(string name, int rank, string linkedDisplayText)
		{
			this.Name = name;
			this.Rank = rank;
			this.LinkedDisplayText = linkedDisplayText;
			base.ResetChangeTracking();
		}

		private static ObjectSchema schema = ObjectSchema.GetInstance<RulePhraseSchema>();

		internal class RulePhraseValidationError : ValidationError
		{
			public RulePhraseValidationError(LocalizedString description, string name) : base(description)
			{
				this.name = name;
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			private readonly string name;
		}
	}
}
