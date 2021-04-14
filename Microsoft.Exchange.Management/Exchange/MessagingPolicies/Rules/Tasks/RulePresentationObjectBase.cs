using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public abstract class RulePresentationObjectBase : IConfigurable, IVersionable
	{
		public RulePresentationObjectBase()
		{
			this.transportRule = new TransportRule();
		}

		public RulePresentationObjectBase(TransportRule transportRule)
		{
			if (transportRule != null)
			{
				this.transportRule = transportRule;
				return;
			}
			this.transportRule = new TransportRule();
		}

		public ObjectId Identity
		{
			get
			{
				return this.transportRule.Identity;
			}
		}

		public string DistinguishedName
		{
			get
			{
				return this.transportRule.DistinguishedName;
			}
		}

		public Guid Guid
		{
			get
			{
				return this.transportRule.Guid;
			}
		}

		public Guid ImmutableId
		{
			get
			{
				return this.transportRule.ImmutableId;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.transportRule.OrganizationId;
			}
		}

		public string Name
		{
			get
			{
				return this.transportRule.Name;
			}
			set
			{
				this.transportRule.Name = value;
			}
		}

		public bool IsValid
		{
			get
			{
				return this.isValid;
			}
		}

		public DateTime? WhenChanged
		{
			get
			{
				return this.transportRule.WhenChanged;
			}
		}

		ObjectSchema IVersionable.ObjectSchema
		{
			get
			{
				return ((IVersionable)this.transportRule).ObjectSchema;
			}
		}

		bool IVersionable.ExchangeVersionUpgradeSupported
		{
			get
			{
				return this.transportRule.ExchangeVersionUpgradeSupported;
			}
		}

		bool IVersionable.IsPropertyAccessible(PropertyDefinition propertyDefinition)
		{
			return ((IVersionable)this.transportRule).IsPropertyAccessible(propertyDefinition);
		}

		public ExchangeObjectVersion ExchangeVersion
		{
			get
			{
				return this.transportRule.ExchangeVersion;
			}
		}

		internal string TransportRuleXml
		{
			get
			{
				return this.transportRule.Xml;
			}
		}

		internal ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return this.transportRule.MaximumSupportedExchangeObjectVersion;
			}
		}

		ExchangeObjectVersion IVersionable.MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return this.MaximumSupportedExchangeObjectVersion;
			}
		}

		internal bool IsReadOnly
		{
			get
			{
				return this.transportRule.IsReadOnly;
			}
		}

		bool IVersionable.IsReadOnly
		{
			get
			{
				return this.IsReadOnly;
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				return ObjectState.Unchanged;
			}
		}

		public override string ToString()
		{
			if (this.Identity != null)
			{
				return this.Identity.ToString();
			}
			if (!string.IsNullOrEmpty(this.Name))
			{
				return this.Name;
			}
			return base.ToString();
		}

		void IConfigurable.CopyChangesFrom(IConfigurable source)
		{
		}

		void IConfigurable.ResetChangeTracking()
		{
		}

		public abstract ValidationError[] Validate();

		internal void SetTransportRule(TransportRule transportRule)
		{
			this.transportRule = transportRule;
		}

		internal virtual void SuppressPiiData(PiiMap piiMap)
		{
			this.transportRule.DistinguishedName = (SuppressingPiiProperty.TryRedact(RulePresentationObjectBaseSchema.DistinguishedName, this.transportRule.DistinguishedName, piiMap) as string);
			this.transportRule.Name = (SuppressingPiiProperty.TryRedact(RulePresentationObjectBaseSchema.Name, this.transportRule.Name, piiMap) as string);
		}

		private TransportRule transportRule;

		protected bool isValid = true;
	}
}
