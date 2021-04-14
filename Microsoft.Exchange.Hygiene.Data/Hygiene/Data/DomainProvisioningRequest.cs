using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class DomainProvisioningRequest : ConfigurablePropertyBag
	{
		public DomainProvisioningRequest()
		{
			this.RequestType = ObjectState.Changed;
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(string.Format("{0}\\{1}", this[DomainProvisioningRequestSchema.OrganizationalUnitRoot], this.DomainName));
			}
		}

		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[DomainProvisioningRequestSchema.OrganizationalUnitRoot];
			}
			set
			{
				this[DomainProvisioningRequestSchema.OrganizationalUnitRoot] = value;
			}
		}

		public string DomainName
		{
			get
			{
				return this[DomainProvisioningRequestSchema.DomainName] as string;
			}
			set
			{
				this[DomainProvisioningRequestSchema.DomainName] = value;
			}
		}

		public ObjectState RequestType
		{
			get
			{
				return (ObjectState)Enum.Parse(typeof(ObjectState), (string)this[DomainProvisioningRequestSchema.RequestType]);
			}
			set
			{
				if (value != ObjectState.Changed && value != ObjectState.Deleted)
				{
					throw new ArgumentException("Only Changed and Deleted are valid RequestType values");
				}
				this[DomainProvisioningRequestSchema.RequestType] = value.ToString();
			}
		}

		public DomainProvisioningRequestFlags RequestFlags
		{
			get
			{
				return (DomainProvisioningRequestFlags)this[DomainProvisioningRequestSchema.RequestFlags];
			}
			set
			{
				this[DomainProvisioningRequestSchema.RequestFlags] = value;
			}
		}

		public override Type GetSchemaType()
		{
			return typeof(DomainProvisioningRequestSchema);
		}
	}
}
