using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class UserExtendedPropertiesEmailAddress : ConfigurablePropertyBag
	{
		public string UserEmailAddress
		{
			get
			{
				return (string)this[UserExtendedPropertiesEmailAddress.UserEmailAddressProp];
			}
			set
			{
				this[UserExtendedPropertiesEmailAddress.UserEmailAddressProp] = value.ToLower();
			}
		}

		public Guid TenantId
		{
			get
			{
				return (Guid)this[UserExtendedPropertiesEmailAddress.TenantIdProp];
			}
			set
			{
				this[UserExtendedPropertiesEmailAddress.TenantIdProp] = value;
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
			return this.UserEmailAddress.ToString();
		}

		public static readonly HygienePropertyDefinition UserEmailAddressProp = new HygienePropertyDefinition("EmailAddress", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition TenantIdProp = new HygienePropertyDefinition("id_TenantId", typeof(Guid));
	}
}
