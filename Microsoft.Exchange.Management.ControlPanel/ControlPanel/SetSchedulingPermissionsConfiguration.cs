using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetSchedulingPermissionsConfiguration : SetResourceConfigurationBase
	{
		[DataMember]
		public bool AllBookInPolicy
		{
			get
			{
				return (bool)(base["AllBookInPolicy"] ?? false);
			}
			set
			{
				base["AllBookInPolicy"] = value;
			}
		}

		[DataMember]
		public bool AllRequestInPolicy
		{
			get
			{
				return (bool)(base["AllRequestInPolicy"] ?? false);
			}
			set
			{
				base["AllRequestInPolicy"] = value;
			}
		}

		[DataMember]
		public bool AllRequestOutOfPolicy
		{
			get
			{
				return (bool)(base["AllRequestOutOfPolicy"] ?? false);
			}
			set
			{
				base["AllRequestOutOfPolicy"] = value;
			}
		}

		[DataMember]
		public PeopleIdentity[] BookInPolicy
		{
			get
			{
				return this.bookInPolicy;
			}
			set
			{
				this.bookInPolicy = value;
				base["BookInPolicy"] = value.ToSMTPAddressArray();
			}
		}

		[DataMember]
		public PeopleIdentity[] RequestInPolicy
		{
			get
			{
				return this.requestInPolicy;
			}
			set
			{
				this.requestInPolicy = value;
				base["RequestInPolicy"] = value.ToSMTPAddressArray();
			}
		}

		[DataMember]
		public PeopleIdentity[] RequestOutOfPolicy
		{
			get
			{
				return this.requestOutOfPolicy;
			}
			set
			{
				this.requestOutOfPolicy = value;
				base["RequestOutOfPolicy"] = value.ToSMTPAddressArray();
			}
		}

		private PeopleIdentity[] bookInPolicy;

		private PeopleIdentity[] requestInPolicy;

		private PeopleIdentity[] requestOutOfPolicy;
	}
}
