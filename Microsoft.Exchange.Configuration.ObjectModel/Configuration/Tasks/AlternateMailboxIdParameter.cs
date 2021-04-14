using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Providers;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class AlternateMailboxIdParameter : IIdentityParameter
	{
		public AlternateMailboxIdParameter()
		{
		}

		public AlternateMailboxIdParameter(AlternateMailboxObject am)
		{
			this.m_objectId = (AlternateMailboxObjectId)am.Identity;
			this.m_rawName = this.m_objectId.FullName;
		}

		public AlternateMailboxIdParameter(ObjectId objId)
		{
			if (objId is AlternateMailboxObjectId)
			{
				this.m_objectId = (AlternateMailboxObjectId)objId;
				this.m_rawName = this.m_objectId.FullName;
				return;
			}
			if (objId is ADObjectId)
			{
				ADObjectId adobjectId = (ADObjectId)objId;
				this.m_objectId = new AlternateMailboxObjectId(string.Empty);
				this.m_objectId.UserId = new Guid?(adobjectId.ObjectGuid);
				this.m_objectId.UserName = (string.IsNullOrEmpty(adobjectId.Name) ? adobjectId.ObjectGuid.ToString() : adobjectId.Name);
			}
		}

		public ObjectId ObjectId
		{
			get
			{
				return this.m_objectId;
			}
		}

		public string RawIdentity
		{
			get
			{
				return this.m_rawName;
			}
		}

		internal string UserName
		{
			get
			{
				return this.m_objectId.UserName;
			}
		}

		internal string AmName
		{
			get
			{
				return this.m_objectId.AmName;
			}
		}

		public static AlternateMailboxIdParameter Parse(string identity)
		{
			return new AlternateMailboxIdParameter
			{
				m_rawName = identity,
				m_objectId = new AlternateMailboxObjectId(identity)
			};
		}

		public void Initialize(ObjectId objectId)
		{
			this.m_objectId = (AlternateMailboxObjectId)objectId;
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			return this.GetObjects<T>(rootId, session, optionalData, out notFoundReason);
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session)
		{
			return this.GetObjects<T>(rootId, session);
		}

		public override string ToString()
		{
			if (this.m_objectId == null)
			{
				return null;
			}
			return this.m_objectId.FullName;
		}

		internal IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			LocalizedString? localizedString;
			return this.GetObjects<T>(rootId, session, null, out localizedString);
		}

		internal IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			notFoundReason = null;
			QueryFilter queryFilter = new AlternateMailboxQueryFilter(this.m_objectId);
			if (optionalData != null && optionalData.AdditionalFilter != null)
			{
				queryFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					optionalData.AdditionalFilter
				});
			}
			return session.FindPaged<T>(queryFilter, rootId, true, null, 0);
		}

		private AlternateMailboxObjectId m_objectId;

		private string m_rawName;
	}
}
