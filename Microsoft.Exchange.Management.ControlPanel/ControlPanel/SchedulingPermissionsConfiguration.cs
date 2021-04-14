using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SchedulingPermissionsConfiguration : ResourceConfigurationBase
	{
		public SchedulingPermissionsConfiguration(CalendarConfiguration calendarConfiguration) : base(calendarConfiguration)
		{
		}

		[DataMember]
		public string AllBookInPolicy
		{
			get
			{
				return base.CalendarConfiguration.AllBookInPolicy.ToJsonString(null);
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AllRequestInPolicy
		{
			get
			{
				return base.CalendarConfiguration.AllRequestInPolicy.ToJsonString(null);
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AllRequestOutOfPolicy
		{
			get
			{
				return base.CalendarConfiguration.AllRequestOutOfPolicy.ToString().ToLowerInvariant();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public PeopleIdentity[] BookInPolicy
		{
			get
			{
				if (base.CalendarConfiguration.BookInPolicy == null)
				{
					return null;
				}
				return RecipientObjectResolver.Instance.ResolveLegacyDNs(base.CalendarConfiguration.BookInPolicy).ToPeopleIdentityArray();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public PeopleIdentity[] RequestInPolicy
		{
			get
			{
				if (base.CalendarConfiguration.RequestInPolicy == null)
				{
					return null;
				}
				return RecipientObjectResolver.Instance.ResolveLegacyDNs(base.CalendarConfiguration.RequestInPolicy).ToPeopleIdentityArray();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public PeopleIdentity[] RequestOutOfPolicy
		{
			get
			{
				if (base.CalendarConfiguration.RequestOutOfPolicy == null)
				{
					return null;
				}
				return RecipientObjectResolver.Instance.ResolveLegacyDNs(base.CalendarConfiguration.RequestOutOfPolicy).ToPeopleIdentityArray();
			}
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}
