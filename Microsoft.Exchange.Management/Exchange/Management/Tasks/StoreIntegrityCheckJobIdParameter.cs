using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[Serializable]
	public class StoreIntegrityCheckJobIdParameter : IIdentityParameter
	{
		public StoreIntegrityCheckJobIdParameter()
		{
		}

		public StoreIntegrityCheckJobIdParameter(string identity)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (identity.Length == 0)
			{
				throw new ArgumentException("identity");
			}
			this.rawIdentity = identity;
		}

		public StoreIntegrityCheckJobIdParameter(StoreIntegrityCheckJobIdentity objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			this.rawIdentity = objectId.ToString();
		}

		public StoreIntegrityCheckJobIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
			this.rawIdentity = namedIdentity.Identity;
		}

		public string RawIdentity
		{
			get
			{
				return this.rawIdentity;
			}
			private set
			{
				this.rawIdentity = value;
			}
		}

		string IIdentityParameter.RawIdentity
		{
			get
			{
				return this.RawIdentity;
			}
		}

		public override string ToString()
		{
			return this.RawIdentity;
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session)
		{
			LocalizedString? localizedString;
			return ((IIdentityParameter)this).GetObjects<T>(rootId, session, null, out localizedString);
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			notFoundReason = null;
			return session.FindPaged<T>(null, rootId, false, null, 0);
		}

		void IIdentityParameter.Initialize(ObjectId objectId)
		{
			StoreIntegrityCheckJobIdentity storeIntegrityCheckJobIdentity = objectId as StoreIntegrityCheckJobIdentity;
			if (storeIntegrityCheckJobIdentity == null)
			{
				throw new ArgumentException("objectId");
			}
			this.rawIdentity = storeIntegrityCheckJobIdentity.ToString();
		}

		private string rawIdentity;
	}
}
