using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.Directory
{
	internal class GroupExtendedPropertiesEmailAddress : ConfigurablePropertyBag
	{
		public string GroupEmailAddress
		{
			get
			{
				return (string)this[GroupExtendedPropertiesEmailAddress.GroupEmailAddressProp];
			}
			set
			{
				this[GroupExtendedPropertiesEmailAddress.GroupEmailAddressProp] = value.ToLower();
			}
		}

		public Guid TenantId
		{
			get
			{
				return (Guid)this[GroupExtendedPropertiesEmailAddress.TenantIdProp];
			}
			set
			{
				this[GroupExtendedPropertiesEmailAddress.TenantIdProp] = value;
			}
		}

		public Guid GroupId
		{
			get
			{
				return (Guid)this[GroupExtendedPropertiesEmailAddress.GroupIdProp];
			}
			set
			{
				this[GroupExtendedPropertiesEmailAddress.GroupIdProp] = value;
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
			return this.GroupEmailAddress.ToString();
		}

		public static readonly HygienePropertyDefinition GroupEmailAddressProp = new HygienePropertyDefinition("EmailAddress", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);

		public static readonly HygienePropertyDefinition TenantIdProp = new HygienePropertyDefinition("id_TenantId", typeof(Guid));

		public static readonly HygienePropertyDefinition GroupIdProp = new HygienePropertyDefinition("id_GroupId", typeof(Guid));
	}
}
