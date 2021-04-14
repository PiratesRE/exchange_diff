using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class LockName<T1, T2> : ILockName, IEquatable<ILockName>, IComparable<ILockName> where T1 : IComparable<T1>, IEquatable<T1> where T2 : IComparable<T2>, IEquatable<T2>
	{
		public LockName(T1 nameValue1, T2 nameValue2, LockManager.LockLevel lockLevel)
		{
			this.nameValue1 = nameValue1;
			this.nameValue2 = nameValue2;
			this.lockLevel = lockLevel;
			this.hashCode = (int)(lockLevel ^ (LockManager.LockLevel)nameValue1.GetHashCode() ^ (LockManager.LockLevel)nameValue2.GetHashCode());
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

		public T1 NameValue1
		{
			get
			{
				return this.nameValue1;
			}
		}

		public T2 NameValue2
		{
			get
			{
				return this.nameValue2;
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
			LockName<T1, T2> lockName = other as LockName<T1, T2>;
			T1 t = this.nameValue1;
			if (t.Equals(lockName.NameValue1))
			{
				T2 t2 = this.nameValue2;
				return t2.Equals(lockName.NameValue2);
			}
			return false;
		}

		public int CompareTo(ILockName other)
		{
			int num = ((int)this.lockLevel).CompareTo((int)other.LockLevel);
			if (num == 0)
			{
				LockName<T1, T2> lockName = other as LockName<T1, T2>;
				T1 t = this.nameValue1;
				num = t.CompareTo(lockName.NameValue1);
				if (num == 0)
				{
					T2 t2 = this.nameValue2;
					num = t2.CompareTo(lockName.NameValue2);
				}
			}
			return num;
		}

		public override int GetHashCode()
		{
			return this.hashCode;
		}

		public override string ToString()
		{
			string[] array = new string[6];
			array[0] = this.lockLevel.ToString();
			array[1] = ":[";
			string[] array2 = array;
			int num = 2;
			T1 t = this.nameValue1;
			array2[num] = t.ToString();
			array[3] = ",";
			string[] array3 = array;
			int num2 = 4;
			T2 t2 = this.nameValue2;
			array3[num2] = t2.ToString();
			array[5] = "]";
			return string.Concat(array);
		}

		private readonly int hashCode;

		private readonly LockManager.LockLevel lockLevel;

		private readonly T1 nameValue1;

		private readonly T2 nameValue2;

		private LockManager.NamedLockObject cachedLockObject;
	}
}
