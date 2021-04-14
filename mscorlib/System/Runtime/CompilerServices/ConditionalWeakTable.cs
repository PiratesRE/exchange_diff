using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.CompilerServices
{
	[ComVisible(false)]
	[__DynamicallyInvokable]
	public sealed class ConditionalWeakTable<TKey, TValue> where TKey : class where TValue : class
	{
		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public ConditionalWeakTable()
		{
			this._buckets = new int[0];
			this._entries = new ConditionalWeakTable<TKey, TValue>.Entry[0];
			this._freeList = -1;
			this._lock = new object();
			this.Resize();
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public bool TryGetValue(TKey key, out TValue value)
		{
			if (key == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
			}
			object @lock = this._lock;
			bool result;
			lock (@lock)
			{
				this.VerifyIntegrity();
				result = this.TryGetValueWorker(key, out value);
			}
			return result;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public void Add(TKey key, TValue value)
		{
			if (key == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
			}
			object @lock = this._lock;
			lock (@lock)
			{
				this.VerifyIntegrity();
				this._invalid = true;
				int num = this.FindEntry(key);
				if (num != -1)
				{
					this._invalid = false;
					ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_AddingDuplicate);
				}
				this.CreateEntry(key, value);
				this._invalid = false;
			}
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public bool Remove(TKey key)
		{
			if (key == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.key);
			}
			object @lock = this._lock;
			bool result;
			lock (@lock)
			{
				this.VerifyIntegrity();
				this._invalid = true;
				int num = RuntimeHelpers.GetHashCode(key) & int.MaxValue;
				int num2 = num % this._buckets.Length;
				int num3 = -1;
				for (int num4 = this._buckets[num2]; num4 != -1; num4 = this._entries[num4].next)
				{
					if (this._entries[num4].hashCode == num && this._entries[num4].depHnd.GetPrimary() == key)
					{
						if (num3 == -1)
						{
							this._buckets[num2] = this._entries[num4].next;
						}
						else
						{
							this._entries[num3].next = this._entries[num4].next;
						}
						this._entries[num4].depHnd.Free();
						this._entries[num4].next = this._freeList;
						this._freeList = num4;
						this._invalid = false;
						return true;
					}
					num3 = num4;
				}
				this._invalid = false;
				result = false;
			}
			return result;
		}

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public TValue GetValue(TKey key, ConditionalWeakTable<TKey, TValue>.CreateValueCallback createValueCallback)
		{
			if (createValueCallback == null)
			{
				throw new ArgumentNullException("createValueCallback");
			}
			TValue tvalue;
			if (this.TryGetValue(key, out tvalue))
			{
				return tvalue;
			}
			TValue tvalue2 = createValueCallback(key);
			object @lock = this._lock;
			TValue result;
			lock (@lock)
			{
				this.VerifyIntegrity();
				this._invalid = true;
				if (this.TryGetValueWorker(key, out tvalue))
				{
					this._invalid = false;
					result = tvalue;
				}
				else
				{
					this.CreateEntry(key, tvalue2);
					this._invalid = false;
					result = tvalue2;
				}
			}
			return result;
		}

		[__DynamicallyInvokable]
		public TValue GetOrCreateValue(TKey key)
		{
			return this.GetValue(key, (TKey k) => Activator.CreateInstance<TValue>());
		}

		[SecuritySafeCritical]
		[FriendAccessAllowed]
		internal TKey FindEquivalentKeyUnsafe(TKey key, out TValue value)
		{
			object @lock = this._lock;
			lock (@lock)
			{
				for (int i = 0; i < this._buckets.Length; i++)
				{
					for (int num = this._buckets[i]; num != -1; num = this._entries[num].next)
					{
						object obj;
						object obj2;
						this._entries[num].depHnd.GetPrimaryAndSecondary(out obj, out obj2);
						if (object.Equals(obj, key))
						{
							value = (TValue)((object)obj2);
							return (TKey)((object)obj);
						}
					}
				}
			}
			value = default(TValue);
			return default(TKey);
		}

		internal ICollection<TKey> Keys
		{
			[SecuritySafeCritical]
			get
			{
				List<TKey> list = new List<TKey>();
				object @lock = this._lock;
				lock (@lock)
				{
					for (int i = 0; i < this._buckets.Length; i++)
					{
						for (int num = this._buckets[i]; num != -1; num = this._entries[num].next)
						{
							TKey tkey = (TKey)((object)this._entries[num].depHnd.GetPrimary());
							if (tkey != null)
							{
								list.Add(tkey);
							}
						}
					}
				}
				return list;
			}
		}

		internal ICollection<TValue> Values
		{
			[SecuritySafeCritical]
			get
			{
				List<TValue> list = new List<TValue>();
				object @lock = this._lock;
				lock (@lock)
				{
					for (int i = 0; i < this._buckets.Length; i++)
					{
						for (int num = this._buckets[i]; num != -1; num = this._entries[num].next)
						{
							object obj = null;
							object obj2 = null;
							this._entries[num].depHnd.GetPrimaryAndSecondary(out obj, out obj2);
							if (obj != null)
							{
								list.Add((TValue)((object)obj2));
							}
						}
					}
				}
				return list;
			}
		}

		[SecuritySafeCritical]
		internal void Clear()
		{
			object @lock = this._lock;
			lock (@lock)
			{
				for (int i = 0; i < this._buckets.Length; i++)
				{
					this._buckets[i] = -1;
				}
				int j;
				for (j = 0; j < this._entries.Length; j++)
				{
					if (this._entries[j].depHnd.IsAllocated)
					{
						this._entries[j].depHnd.Free();
					}
					this._entries[j].next = j - 1;
				}
				this._freeList = j - 1;
			}
		}

		[SecurityCritical]
		private bool TryGetValueWorker(TKey key, out TValue value)
		{
			int num = this.FindEntry(key);
			if (num != -1)
			{
				object obj = null;
				object obj2 = null;
				this._entries[num].depHnd.GetPrimaryAndSecondary(out obj, out obj2);
				if (obj != null)
				{
					value = (TValue)((object)obj2);
					return true;
				}
			}
			value = default(TValue);
			return false;
		}

		[SecurityCritical]
		private void CreateEntry(TKey key, TValue value)
		{
			if (this._freeList == -1)
			{
				this.Resize();
			}
			int num = RuntimeHelpers.GetHashCode(key) & int.MaxValue;
			int num2 = num % this._buckets.Length;
			int freeList = this._freeList;
			this._freeList = this._entries[freeList].next;
			this._entries[freeList].hashCode = num;
			this._entries[freeList].depHnd = new DependentHandle(key, value);
			this._entries[freeList].next = this._buckets[num2];
			this._buckets[num2] = freeList;
		}

		[SecurityCritical]
		private void Resize()
		{
			int num = this._buckets.Length;
			bool flag = false;
			int i;
			for (i = 0; i < this._entries.Length; i++)
			{
				if (this._entries[i].depHnd.IsAllocated && this._entries[i].depHnd.GetPrimary() == null)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				num = HashHelpers.GetPrime((this._buckets.Length == 0) ? 6 : (this._buckets.Length * 2));
			}
			int num2 = -1;
			int[] array = new int[num];
			for (int j = 0; j < num; j++)
			{
				array[j] = -1;
			}
			ConditionalWeakTable<TKey, TValue>.Entry[] array2 = new ConditionalWeakTable<TKey, TValue>.Entry[num];
			for (i = 0; i < this._entries.Length; i++)
			{
				DependentHandle depHnd = this._entries[i].depHnd;
				if (depHnd.IsAllocated && depHnd.GetPrimary() != null)
				{
					int num3 = this._entries[i].hashCode % num;
					array2[i].depHnd = depHnd;
					array2[i].hashCode = this._entries[i].hashCode;
					array2[i].next = array[num3];
					array[num3] = i;
				}
				else
				{
					this._entries[i].depHnd.Free();
					array2[i].depHnd = default(DependentHandle);
					array2[i].next = num2;
					num2 = i;
				}
			}
			while (i != array2.Length)
			{
				array2[i].depHnd = default(DependentHandle);
				array2[i].next = num2;
				num2 = i;
				i++;
			}
			this._buckets = array;
			this._entries = array2;
			this._freeList = num2;
		}

		[SecurityCritical]
		private int FindEntry(TKey key)
		{
			int num = RuntimeHelpers.GetHashCode(key) & int.MaxValue;
			for (int num2 = this._buckets[num % this._buckets.Length]; num2 != -1; num2 = this._entries[num2].next)
			{
				if (this._entries[num2].hashCode == num && this._entries[num2].depHnd.GetPrimary() == key)
				{
					return num2;
				}
			}
			return -1;
		}

		private void VerifyIntegrity()
		{
			if (this._invalid)
			{
				throw new InvalidOperationException(Environment.GetResourceString("CollectionCorrupted"));
			}
		}

		[SecuritySafeCritical]
		protected override void Finalize()
		{
			try
			{
				if (!Environment.HasShutdownStarted)
				{
					if (this._lock != null)
					{
						object @lock = this._lock;
						lock (@lock)
						{
							if (!this._invalid)
							{
								ConditionalWeakTable<TKey, TValue>.Entry[] entries = this._entries;
								this._invalid = true;
								this._entries = null;
								this._buckets = null;
								for (int i = 0; i < entries.Length; i++)
								{
									entries[i].depHnd.Free();
								}
							}
						}
					}
				}
			}
			finally
			{
				base.Finalize();
			}
		}

		private int[] _buckets;

		private ConditionalWeakTable<TKey, TValue>.Entry[] _entries;

		private int _freeList;

		private const int _initialCapacity = 5;

		private readonly object _lock;

		private bool _invalid;

		[__DynamicallyInvokable]
		public delegate TValue CreateValueCallback(TKey key);

		private struct Entry
		{
			public DependentHandle depHnd;

			public int hashCode;

			public int next;
		}
	}
}
