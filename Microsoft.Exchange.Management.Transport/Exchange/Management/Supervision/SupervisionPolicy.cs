using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.Supervision
{
	[Serializable]
	public sealed class SupervisionPolicy : ConfigurableObject
	{
		public SupervisionPolicy(string orgname) : base(new SupervisionPolicyPropertyBag())
		{
			this.Identity = new SupervisionPolicyId(orgname);
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return SupervisionPolicy.schema;
			}
		}

		private new bool IsValid
		{
			get
			{
				return true;
			}
		}

		public new ObjectId Identity
		{
			get
			{
				return (ObjectId)this.propertyBag[SupervisionPolicySchema.Identity];
			}
			internal set
			{
				this.propertyBag[SupervisionPolicySchema.Identity] = value;
			}
		}

		public bool ClosedCampusInboundPolicyEnabled
		{
			get
			{
				return (bool)this.propertyBag[SupervisionPolicySchema.ClosedCampusInboundPolicyEnabled];
			}
			internal set
			{
				this.propertyBag[SupervisionPolicySchema.ClosedCampusInboundPolicyEnabled] = value;
			}
		}

		public MultiValuedProperty<SmtpDomain> ClosedCampusInboundPolicyDomainExceptions
		{
			get
			{
				return (MultiValuedProperty<SmtpDomain>)this.propertyBag[SupervisionPolicySchema.ClosedCampusInboundDomainExceptions];
			}
			internal set
			{
				this.propertyBag[SupervisionPolicySchema.ClosedCampusInboundDomainExceptions] = value;
			}
		}

		public MultiValuedProperty<SmtpAddress> ClosedCampusInboundPolicyGroupExceptions
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this.propertyBag[SupervisionPolicySchema.ClosedCampusInboundGroupExceptions];
			}
			internal set
			{
				this.propertyBag[SupervisionPolicySchema.ClosedCampusInboundGroupExceptions] = value;
			}
		}

		public bool ClosedCampusOutboundPolicyEnabled
		{
			get
			{
				return (bool)this.propertyBag[SupervisionPolicySchema.ClosedCampusOutboundPolicyEnabled];
			}
			internal set
			{
				this.propertyBag[SupervisionPolicySchema.ClosedCampusOutboundPolicyEnabled] = value;
			}
		}

		public MultiValuedProperty<SmtpDomain> ClosedCampusOutboundPolicyDomainExceptions
		{
			get
			{
				return (MultiValuedProperty<SmtpDomain>)this.propertyBag[SupervisionPolicySchema.ClosedCampusOutboundDomainExceptions];
			}
			internal set
			{
				this.propertyBag[SupervisionPolicySchema.ClosedCampusOutboundDomainExceptions] = value;
			}
		}

		public MultiValuedProperty<SmtpAddress> ClosedCampusOutboundPolicyGroupExceptions
		{
			get
			{
				return (MultiValuedProperty<SmtpAddress>)this.propertyBag[SupervisionPolicySchema.ClosedCampusOutboundGroupExceptions];
			}
			internal set
			{
				this.propertyBag[SupervisionPolicySchema.ClosedCampusOutboundGroupExceptions] = value;
			}
		}

		public bool BadWordsPolicyEnabled
		{
			get
			{
				return (bool)this.propertyBag[SupervisionPolicySchema.BadWordsPolicyEnabled];
			}
			internal set
			{
				this.propertyBag[SupervisionPolicySchema.BadWordsPolicyEnabled] = value;
			}
		}

		public string BadWordsList
		{
			get
			{
				return (string)this.propertyBag[SupervisionPolicySchema.BadWordsList];
			}
			internal set
			{
				this.propertyBag[SupervisionPolicySchema.BadWordsList] = value;
			}
		}

		public bool AntiBullyingPolicyEnabled
		{
			get
			{
				return (bool)this.propertyBag[SupervisionPolicySchema.AntiBullyingPolicyEnabled];
			}
			internal set
			{
				this.propertyBag[SupervisionPolicySchema.AntiBullyingPolicyEnabled] = value;
			}
		}

		internal static readonly string BadWordsSeparator = ",";

		private static SupervisionPolicySchema schema = new SupervisionPolicySchema();
	}
}
