using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class LockName<T> : ILockName, IEquatable<ILockName>, IComparable<ILockName> where T : IComparable<T>, IEquatable<T>
	{
		public LockName(T nameValue, LockManager.LockLevel lockLevel)
		{
			this.nameValue = nameValue;
			this.lockLevel = lockLevel;
			this.hashCode = (int)(lockLevel ^ (LockManager.LockLevel)nameValue.GetHashCode());
		}

		public LockManager.LockLevel LockLevel
		{
			get
			{
				return this.lockLevel;
			}
		}

		public LockManager.NamedLockObject CachedLockObject
		{
			get
			{
				return this.cachedLockObject;
			}
			set
			{
				this.cachedLockObject = value;
			}
		}

		public T NameValue
		{
			get
			{
				return this.nameValue;
			}
		}

		public ILockName GetLockNameToCache()
		{
			return this;
		}

		public bool Equals(ILockName other)
		{
			if (this.lockLevel != other.LockLevel)
			{
				return false;
			}
			LockName<T> lockName = other as LockName<T>;
			T t = this.nameValue;
			return t.Equals(lockName.NameValue);
		}

		public int CompareTo(ILockName other)
		{
			int num = ((int)this.lockLevel).CompareTo((int)other.LockLevel);
			if (num == 0)
			{
				LockName<T> lockName = other as LockName<T>;
				T t = this.nameValue;
				num = t.CompareTo(lockName.NameValue);
			}
			return num;
		}

		public override int GetHashCode()
		{
			return this.hashCode;
		}

		public override string ToString()
		{
			string str = this.lockLevel.ToString();
			string str2 = " ";
			T t = this.nameValue;
			return str + str2 + t.ToString();
		}

		private readonly int hashCode;

		private readonly LockManager.LockLevel lockLevel;

		private readonly T nameValue;

		private LockManager.NamedLockObject cachedLockObject;
	}
}
