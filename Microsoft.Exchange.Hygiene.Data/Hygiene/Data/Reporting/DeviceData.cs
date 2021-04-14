using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.Reporting
{
	internal class DeviceData : ConfigurablePropertyBag
	{
		public string AccessSetBy
		{
			get
			{
				return (string)this[DeviceCommonSchema.AccessSetByProperty];
			}
			set
			{
				this[DeviceCommonSchema.AccessSetByProperty] = value;
			}
		}

		public int? AccessState
		{
			get
			{
				return (int?)this[DeviceCommonSchema.AccessStateProperty];
			}
			set
			{
				this[DeviceCommonSchema.AccessStateProperty] = value;
			}
		}

		public int? AccessStateReason
		{
			get
			{
				return (int?)this[DeviceCommonSchema.AccessStateReasonProperty];
			}
			set
			{
				this[DeviceCommonSchema.AccessStateReasonProperty] = value;
			}
		}

		public Guid ActivityId
		{
			get
			{
				return (Guid)this[DeviceCommonSchema.ActivityIdProperty];
			}
			set
			{
				this[DeviceCommonSchema.ActivityIdProperty] = value;
			}
		}

		public int DateKey
		{
			get
			{
				return (int)this[DeviceCommonSchema.DateKeyProperty];
			}
			set
			{
				this[DeviceCommonSchema.DateKeyProperty] = value;
			}
		}

		public DateTime? DeletedTime
		{
			get
			{
				return (DateTime?)this[DeviceCommonSchema.DeletedTimeProperty];
			}
			set
			{
				this[DeviceCommonSchema.DeletedTimeProperty] = value;
			}
		}

		public Guid DeviceId
		{
			get
			{
				return (Guid)this[DeviceCommonSchema.DeviceIdProperty];
			}
			set
			{
				this[DeviceCommonSchema.DeviceIdProperty] = value;
			}
		}

		public string DeviceLanguage
		{
			get
			{
				return (string)this[DeviceCommonSchema.DeviceLanguageProperty];
			}
			set
			{
				this[DeviceCommonSchema.DeviceLanguageProperty] = value;
			}
		}

		public string DeviceModel
		{
			get
			{
				return (string)this[DeviceCommonSchema.DeviceModelProperty];
			}
			set
			{
				this[DeviceCommonSchema.DeviceModelProperty] = value;
			}
		}

		public string DeviceName
		{
			get
			{
				return (string)this[DeviceCommonSchema.DeviceNameProperty];
			}
			set
			{
				this[DeviceCommonSchema.DeviceNameProperty] = value;
			}
		}

		public string DeviceType
		{
			get
			{
				return (string)this[DeviceCommonSchema.DeviceTypeProperty];
			}
			set
			{
				this[DeviceCommonSchema.DeviceTypeProperty] = value;
			}
		}

		public string EASId
		{
			get
			{
				return (string)this[DeviceCommonSchema.EASIdProperty];
			}
			set
			{
				this[DeviceCommonSchema.EASIdProperty] = value;
			}
		}

		public string EASVersion
		{
			get
			{
				return (string)this[DeviceCommonSchema.EASVersionProperty];
			}
			set
			{
				this[DeviceCommonSchema.EASVersionProperty] = value;
			}
		}

		public int HashBucket
		{
			get
			{
				return (int)this[DeviceCommonSchema.HashBucketProperty];
			}
			set
			{
				this[DeviceCommonSchema.HashBucketProperty] = value;
			}
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(Guid.NewGuid().ToString());
			}
		}

		public string IMEI
		{
			get
			{
				return (string)this[DeviceCommonSchema.IMEIProperty];
			}
			set
			{
				this[DeviceCommonSchema.IMEIProperty] = value;
			}
		}

		public Guid? IntuneId
		{
			get
			{
				return (Guid?)this[DeviceCommonSchema.IntuneIdProperty];
			}
			set
			{
				this[DeviceCommonSchema.IntuneIdProperty] = value;
			}
		}

		public string MobileNetwork
		{
			get
			{
				return (string)this[DeviceCommonSchema.MobileNetworkProperty];
			}
			set
			{
				this[DeviceCommonSchema.MobileNetworkProperty] = value;
			}
		}

		public Guid OrganizationalUnitRoot
		{
			get
			{
				return (Guid)this[DeviceCommonSchema.OrganizationalUnitRootProperty];
			}
			set
			{
				this[DeviceCommonSchema.OrganizationalUnitRootProperty] = value;
			}
		}

		public string PhoneNumber
		{
			get
			{
				return (string)this[DeviceCommonSchema.PhoneNumberProperty];
			}
			set
			{
				this[DeviceCommonSchema.PhoneNumberProperty] = value;
			}
		}

		public string Platform
		{
			get
			{
				return (string)this[DeviceCommonSchema.PlatformProperty];
			}
			set
			{
				this[DeviceCommonSchema.PlatformProperty] = value;
			}
		}

		public string PolicyApplied
		{
			get
			{
				return (string)this[DeviceCommonSchema.PolicyAppliedProperty];
			}
			set
			{
				this[DeviceCommonSchema.PolicyAppliedProperty] = value;
			}
		}

		public DateTime TimeStamp
		{
			get
			{
				return (DateTime)this[DeviceCommonSchema.TimeStampProperty];
			}
			set
			{
				this[DeviceCommonSchema.TimeStampProperty] = value;
			}
		}

		public string User
		{
			get
			{
				return (string)this[DeviceCommonSchema.UserProperty];
			}
			set
			{
				this[DeviceCommonSchema.UserProperty] = value;
			}
		}

		public string UserAgent
		{
			get
			{
				return (string)this[DeviceCommonSchema.UserAgentProperty];
			}
			set
			{
				this[DeviceCommonSchema.UserAgentProperty] = value;
			}
		}

		public override IEnumerable<PropertyDefinition> GetPropertyDefinitions(bool isChangedOnly)
		{
			if (isChangedOnly)
			{
				return base.GetPropertyDefinitions(isChangedOnly);
			}
			return DeviceData.propertydefinitions;
		}

		internal static readonly HygienePropertyDefinition[] propertydefinitions = new HygienePropertyDefinition[]
		{
			DeviceCommonSchema.OrganizationalUnitRootProperty,
			DeviceCommonSchema.HashBucketProperty,
			DeviceCommonSchema.DeviceIdProperty,
			DeviceCommonSchema.EASIdProperty,
			DeviceCommonSchema.IntuneIdProperty,
			DeviceCommonSchema.UserProperty,
			DeviceCommonSchema.DeviceNameProperty,
			DeviceCommonSchema.DeviceModelProperty,
			DeviceCommonSchema.DeviceTypeProperty,
			DeviceCommonSchema.IMEIProperty,
			DeviceCommonSchema.PhoneNumberProperty,
			DeviceCommonSchema.MobileNetworkProperty,
			DeviceCommonSchema.EASVersionProperty,
			DeviceCommonSchema.UserAgentProperty,
			DeviceCommonSchema.DeviceLanguageProperty,
			DeviceCommonSchema.DeletedTimeProperty,
			DeviceCommonSchema.ActivityIdProperty,
			DeviceCommonSchema.TimeStampProperty,
			DeviceCommonSchema.DateKeyProperty,
			DeviceCommonSchema.PlatformProperty,
			DeviceCommonSchema.AccessStateProperty,
			DeviceCommonSchema.AccessStateReasonProperty,
			DeviceCommonSchema.AccessSetByProperty,
			DeviceCommonSchema.PolicyAppliedProperty
		};
	}
}
