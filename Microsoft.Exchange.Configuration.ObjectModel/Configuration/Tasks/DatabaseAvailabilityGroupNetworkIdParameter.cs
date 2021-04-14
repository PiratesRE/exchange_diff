using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class DatabaseAvailabilityGroupNetworkIdParameter : IIdentityParameter
	{
		public DatabaseAvailabilityGroupNetworkIdParameter()
		{
		}

		public DatabaseAvailabilityGroupNetworkIdParameter(DatabaseAvailabilityGroupNetwork dagNet)
		{
			this.m_objectId = (DagNetworkObjectId)dagNet.Identity;
			this.m_rawName = this.m_objectId.FullName;
		}

		public DatabaseAvailabilityGroupNetworkIdParameter(ObjectId objId)
		{
			if (objId is DagNetworkObjectId)
			{
				this.m_objectId = (DagNetworkObjectId)objId;
				this.m_rawName = this.m_objectId.FullName;
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

		internal string DagName
		{
			get
			{
				return this.m_objectId.DagName;
			}
		}

		internal string NetName
		{
			get
			{
				return this.m_objectId.NetName;
			}
		}

		public static DatabaseAvailabilityGroupNetworkIdParameter Parse(string identity)
		{
			DatabaseAvailabilityGroupNetworkIdParameter databaseAvailabilityGroupNetworkIdParameter = new DatabaseAvailabilityGroupNetworkIdParameter();
			databaseAvailabilityGroupNetworkIdParameter.m_rawName = identity;
			databaseAvailabilityGroupNetworkIdParameter.m_objectId = new DagNetworkObjectId(identity);
			if (databaseAvailabilityGroupNetworkIdParameter.m_objectId.DagName == string.Empty || databaseAvailabilityGroupNetworkIdParameter.m_objectId.NetName == string.Empty)
			{
				throw new ArgumentException(Strings.ErrorInvalidIdentity(identity), "Identity");
			}
			return databaseAvailabilityGroupNetworkIdParameter;
		}

		public void Initialize(ObjectId objectId)
		{
			this.m_objectId = (DagNetworkObjectId)objectId;
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
			QueryFilter queryFilter = new DagNetworkQueryFilter(this.m_objectId);
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

		private DagNetworkObjectId m_objectId;

		private string m_rawName;
	}
}
