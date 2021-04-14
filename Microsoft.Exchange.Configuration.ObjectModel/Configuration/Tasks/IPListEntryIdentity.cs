using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class IPListEntryIdentity : ObjectId, IIdentityParameter
	{
		public IPListEntryIdentity(int index)
		{
			this.index = index;
		}

		public IPListEntryIdentity(string index)
		{
			this.index = int.Parse(index);
		}

		public IPListEntryIdentity(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
		}

		internal int Index
		{
			get
			{
				return this.index;
			}
		}

		public override string ToString()
		{
			return this.index.ToString();
		}

		public override byte[] GetBytes()
		{
			return BitConverter.GetBytes(this.index);
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			return this.GetObjects<T>(rootId, session, optionalData, out notFoundReason);
		}

		internal IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session) where T : IConfigurable, new()
		{
			LocalizedString? localizedString;
			return this.GetObjects<T>(rootId, session, null, out localizedString);
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session)
		{
			return this.GetObjects<T>(rootId, session);
		}

		internal IEnumerable<T> GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			if (optionalData != null && optionalData.AdditionalFilter != null)
			{
				throw new NotSupportedException("Supplying Additional Filters is not currently supported by this IdParameter.");
			}
			notFoundReason = null;
			T t = (T)((object)session.Read<T>(this));
			T[] result;
			if (t != null)
			{
				result = new T[]
				{
					t
				};
			}
			else
			{
				result = new T[0];
			}
			return result;
		}

		void IIdentityParameter.Initialize(ObjectId objectId)
		{
			this.Initialize(objectId);
		}

		internal void Initialize(ObjectId objectId)
		{
			this.index = ((IPListEntryIdentity)objectId).index;
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

		private int index;
	}
}
