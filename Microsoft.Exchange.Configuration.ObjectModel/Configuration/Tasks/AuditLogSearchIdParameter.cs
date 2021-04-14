using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class AuditLogSearchIdParameter : IIdentityParameter
	{
		public AuditLogSearchIdParameter()
		{
		}

		public AuditLogSearchId GetId()
		{
			return this.Identity;
		}

		public AuditLogSearchIdParameter(string identity)
		{
			if (string.IsNullOrEmpty(identity))
			{
				throw new ArgumentNullException("Identity");
			}
			Guid requestId;
			if (!Guid.TryParse(identity, out requestId))
			{
				throw new ArgumentException("Identity should be a valid Guid", "Identity");
			}
			this.Identity = new AuditLogSearchId(requestId);
		}

		public AuditLogSearchIdParameter(ObjectId id)
		{
			this.Initialize(id);
		}

		string IIdentityParameter.RawIdentity
		{
			get
			{
				return this.RawIdentity;
			}
		}

		internal string RawIdentity
		{
			get
			{
				return this.ToString();
			}
		}

		void IIdentityParameter.Initialize(ObjectId objectId)
		{
			this.Initialize(objectId);
		}

		public static AuditLogSearchIdParameter Parse(string identity)
		{
			return new AuditLogSearchIdParameter(identity);
		}

		public override string ToString()
		{
			if (this.Identity != null)
			{
				return this.Identity.ToString();
			}
			return null;
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session)
		{
			return this.GetObjects<T>(rootId, session);
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			return this.GetObjects<T>(rootId, session, optionalData, out notFoundReason);
		}

		internal virtual void Initialize(ObjectId id)
		{
			this.Identity = (id as AuditLogSearchId);
		}

		internal IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			notFoundReason = new LocalizedString?(Strings.ErrorManagementObjectNotFound(this.ToString()));
			return session.FindPaged<T>((optionalData == null) ? null : optionalData.AdditionalFilter, rootId, false, null, 0);
		}

		internal IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			LocalizedString? localizedString;
			return this.GetObjects<T>(rootId, session, null, out localizedString);
		}

		private AuditLogSearchId Identity;
	}
}
