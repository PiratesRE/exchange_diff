using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.TenantMonitoring
{
	[Serializable]
	public sealed class NotificationIdParameter : IIdentityParameter
	{
		public NotificationIdParameter(string identity)
		{
			this.rawIdentity = identity;
		}

		public NotificationIdParameter(NotificationIdentity identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			this.rawIdentity = identity.ToString();
		}

		public NotificationIdParameter(Notification notification) : this((NotificationIdentity)notification.Identity)
		{
		}

		public NotificationIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
			this.rawIdentity = namedIdentity.DisplayName;
		}

		public NotificationIdParameter() : this(string.Empty)
		{
		}

		public static NotificationIdParameter Parse(string identity)
		{
			return new NotificationIdParameter(identity);
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			LocalizedString? localizedString;
			return this.GetObjects<T>(rootId, session, null, out localizedString);
		}

		public IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			notFoundReason = null;
			QueryFilter queryFilter = null;
			if (optionalData != null && optionalData.AdditionalFilter != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					optionalData.AdditionalFilter
				});
			}
			return session.FindPaged<T>(queryFilter, rootId, false, null, 0);
		}

		public void Initialize(ObjectId objectId)
		{
			NotificationIdentity notificationIdentity = objectId as NotificationIdentity;
			if (notificationIdentity == null)
			{
				throw new ArgumentException("objectId is of the wrong type", "objectId");
			}
			this.rawIdentity = notificationIdentity.ToString();
		}

		public string RawIdentity
		{
			get
			{
				return this.rawIdentity;
			}
		}

		private string rawIdentity;
	}
}
