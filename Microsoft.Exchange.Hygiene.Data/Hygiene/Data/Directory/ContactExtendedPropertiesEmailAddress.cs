using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class ContactExtendedPropertiesEmailAddress : ConfigurablePropertyBag
	{
		public string ContactEmailAddress
		{
			get
			{
				return (string)this[ContactExtendedPropertiesEmailAddress.ContactEmailAddressProp];
			}
			set
			{
				this[ContactExtendedPropertiesEmailAddress.ContactEmailAddressProp] = value.ToLower();
			}
		}

		public Guid TenantId
		{
			get
			{
				return (Guid)this[ContactExtendedPropertiesEmailAddress.TenantIdProp];
			}
			set
			{
				this[ContactExtendedPropertiesEmailAddress.TenantIdProp] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.ToString());
			}
		}

		public override ObjectState ObjectState
		{
			get
			{
				return (ObjectState)this[DalHelper.ObjectStateProp];
			}
		}

		public override string ToString()
		{
			return this.ContactEmailAddress.ToString();
		}

		public static readonly HygienePropertyDefinition ContactEmailAddressProp = new HygienePropertyDefinition("EmailAddress", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition TenantIdProp = new HygienePropertyDefinition("id_TenantId", typeof(Guid));

		public static readonly HygienePropertyDefinition ContactIdProp = new HygienePropertyDefinition("id_ContactId", typeof(Guid));
	}
}
