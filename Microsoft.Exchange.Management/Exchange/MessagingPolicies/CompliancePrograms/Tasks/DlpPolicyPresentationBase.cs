using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks
{
	[Serializable]
	public abstract class DlpPolicyPresentationBase : IConfigurable, IVersionable
	{
		protected LocalizedString ErrorText { get; set; }

		public ObjectId Identity
		{
			get
			{
				return this.adDlpPolicy.Identity;
			}
		}

		public string DistinguishedName
		{
			get
			{
				return this.adDlpPolicy.DistinguishedName;
			}
		}

		public Guid Guid
		{
			get
			{
				return this.adDlpPolicy.Guid;
			}
		}

		public OrganizationId OrganizationId
		{
			get
			{
				return this.adDlpPolicy.OrganizationId;
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
				return this.adDlpPolicy.WhenChanged;
			}
		}

		public ExchangeObjectVersion ExchangeVersion
		{
			get
			{
				return this.adDlpPolicy.ExchangeVersion;
			}
		}

		public ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return this.adDlpPolicy.MaximumSupportedExchangeObjectVersion;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return this.adDlpPolicy.IsReadOnly;
			}
		}

		ObjectSchema IVersionable.ObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<DlpPolicyTemplateSchema>();
			}
		}

		protected ADComplianceProgram AdDlpPolicy
		{
			get
			{
				return this.adDlpPolicy;
			}
			set
			{
				this.adDlpPolicy = value;
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				return ObjectState.Unchanged;
			}
		}

		bool IVersionable.IsPropertyAccessible(PropertyDefinition propertyDefinition)
		{
			return this.AdDlpPolicy.IsPropertyAccessible(propertyDefinition);
		}

		bool IVersionable.ExchangeVersionUpgradeSupported
		{
			get
			{
				return this.AdDlpPolicy.ExchangeVersionUpgradeSupported;
			}
		}

		protected DlpPolicyPresentationBase(ADComplianceProgram adDlpPolicy)
		{
			this.adDlpPolicy = adDlpPolicy;
		}

		void IConfigurable.CopyChangesFrom(IConfigurable source)
		{
		}

		void IConfigurable.ResetChangeTracking()
		{
		}

		public virtual ValidationError[] Validate()
		{
			if (!this.isValid)
			{
				return new ValidationError[]
				{
					new ObjectValidationError(this.ErrorText, this.Identity, null)
				};
			}
			return ValidationError.None;
		}

		internal virtual void SuppressPiiData(PiiMap piiMap)
		{
			this.adDlpPolicy.DistinguishedName = (SuppressingPiiProperty.TryRedact(DlpPolicySchemaBase.DistinguishedName, this.adDlpPolicy.DistinguishedName, piiMap) as string);
		}

		protected bool isValid = true;

		private ADComplianceProgram adDlpPolicy;
	}
}
