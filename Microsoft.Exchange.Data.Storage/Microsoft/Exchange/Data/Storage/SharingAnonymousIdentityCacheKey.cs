using System;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal abstract class SharingAnonymousIdentityCacheKey
	{
		protected SharingAnonymousIdentityCacheKey(int cachedHashCode)
		{
			this.cachedHashCode = cachedHashCode;
		}

		public static bool operator ==(SharingAnonymousIdentityCacheKey key1, SharingAnonymousIdentityCacheKey key2)
		{
			return object.Equals(key1, key2);
		}

		public static bool operator !=(SharingAnonymousIdentityCacheKey key1, SharingAnonymousIdentityCacheKey key2)
		{
			return !object.Equals(key1, key2);
		}

		public abstract string Lookup(out SecurityIdentifier sid);

		public override int GetHashCode()
		{
			return this.cachedHashCode;
		}

		public override bool Equals(object obj)
		{
			return this.InternalEquals(obj);
		}

		protected abstract bool InternalEquals(object obj);

		private readonly int cachedHashCode;
	}
}
