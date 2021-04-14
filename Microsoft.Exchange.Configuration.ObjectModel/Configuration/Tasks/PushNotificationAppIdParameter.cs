using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class PushNotificationAppIdParameter : ADIdParameter
	{
		public PushNotificationAppIdParameter()
		{
		}

		public PushNotificationAppIdParameter(string identity) : base(identity)
		{
		}

		public PushNotificationAppIdParameter(ADObjectId objectId) : base(objectId)
		{
		}

		public PushNotificationAppIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		protected override QueryFilter AdditionalQueryFilter
		{
			get
			{
				return null;
			}
		}

		public static PushNotificationAppIdParameter Parse(string identity)
		{
			return new PushNotificationAppIdParameter(identity);
		}
	}
}
