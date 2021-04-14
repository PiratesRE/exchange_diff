using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Mapi;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public abstract class MapiIdParameter : IIdentityParameter
	{
		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			return this.GetObjects<T>(rootId, session, optionalData, out notFoundReason);
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session)
		{
			return this.GetObjects<T>(rootId, session);
		}

		internal IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			LocalizedString? localizedString;
			return this.GetObjects<T>(rootId, session, null, out localizedString);
		}

		internal virtual IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			if (!typeof(MapiObject).IsAssignableFrom(typeof(T)))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			if (!(session is MapiSession))
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterType("session", typeof(MapiSession).Name), "session");
			}
			notFoundReason = null;
			if (optionalData != null && optionalData.AdditionalFilter != null)
			{
				throw new NotSupportedException("Supplying Additional Filters is not currently supported by this IdParameter.");
			}
			if (rootId == null)
			{
				return new List<T>
				{
					(T)((object)session.Read<T>(this.mapiObjectId))
				};
			}
			return session.FindPaged<T>(null, rootId, true, null, 0);
		}

		void IIdentityParameter.Initialize(ObjectId objectId)
		{
			this.Initialize(objectId);
		}

		internal virtual void Initialize(ObjectId objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			if (null != this.mapiObjectId)
			{
				throw new InvalidOperationException(Strings.ErrorChangeImmutableType);
			}
			this.MapiObjectId = (objectId as MapiObjectId);
		}

		string IIdentityParameter.RawIdentity
		{
			get
			{
				return this.RawIdentity;
			}
		}

		internal virtual string RawIdentity
		{
			get
			{
				if (!(this.mapiObjectId != null))
				{
					return null;
				}
				return this.MapiObjectId.ToString();
			}
		}

		public MapiIdParameter()
		{
		}

		public MapiIdParameter(MapiObjectId mapiObjectId)
		{
			this.Initialize(mapiObjectId);
		}

		protected MapiObjectId MapiObjectId
		{
			get
			{
				return this.mapiObjectId;
			}
			set
			{
				if (null == value)
				{
					throw new ArgumentNullException("MapiObjectId");
				}
				this.mapiObjectId = value;
			}
		}

		private MapiObjectId mapiObjectId;
	}
}
