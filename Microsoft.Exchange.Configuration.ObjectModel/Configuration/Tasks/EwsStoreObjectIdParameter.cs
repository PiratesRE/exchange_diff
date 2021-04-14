using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class EwsStoreObjectIdParameter : IIdentityParameter
	{
		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session)
		{
			LocalizedString? localizedString;
			return ((IIdentityParameter)this).GetObjects<T>(rootId, session, null, out localizedString);
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (!(session is EwsStoreDataProvider))
			{
				throw new NotSupportedException("session: " + session.GetType().FullName);
			}
			if (optionalData != null && optionalData.AdditionalFilter != null)
			{
				throw new NotSupportedException("Supplying Additional Filters is not currently supported by this IdParameter.");
			}
			T t = (this.ewsStoreObjectId != null) ? ((T)((object)session.Read<T>(this.ewsStoreObjectId))) : ((EwsStoreDataProvider)session).FindByAlternativeId<T>(this.rawIdentity);
			if (t == null)
			{
				notFoundReason = new LocalizedString?(Strings.ErrorManagementObjectNotFound(this.ToString()));
				return new T[0];
			}
			notFoundReason = null;
			return new T[]
			{
				t
			};
		}

		void IIdentityParameter.Initialize(ObjectId objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			EwsStoreObjectId ewsStoreObjectId = objectId as EwsStoreObjectId;
			if (ewsStoreObjectId == null)
			{
				throw new NotSupportedException("objectId: " + objectId.GetType().FullName);
			}
			this.ewsStoreObjectId = ewsStoreObjectId;
		}

		string IIdentityParameter.RawIdentity
		{
			get
			{
				return this.rawIdentity;
			}
		}

		public EwsStoreObjectIdParameter()
		{
		}

		public EwsStoreObjectIdParameter(EwsStoreObject ewsObject) : this(ewsObject.Identity)
		{
		}

		public EwsStoreObjectIdParameter(EwsStoreObjectId storeObjectId)
		{
			if (storeObjectId == null)
			{
				throw new ArgumentNullException("storeObjectId");
			}
			this.rawIdentity = storeObjectId.ToString();
			((IIdentityParameter)this).Initialize(storeObjectId);
		}

		public EwsStoreObjectIdParameter(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentNullException("id");
			}
			this.rawIdentity = id;
			EwsStoreObjectId objectId;
			if (EwsStoreObjectId.TryParse(id, out objectId))
			{
				((IIdentityParameter)this).Initialize(objectId);
			}
		}

		public EwsStoreObjectIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
			this.displayName = namedIdentity.DisplayName;
		}

		public override string ToString()
		{
			return this.displayName ?? this.rawIdentity;
		}

		private readonly string displayName;

		private readonly string rawIdentity;

		[NonSerialized]
		private EwsStoreObjectId ewsStoreObjectId;
	}
}
