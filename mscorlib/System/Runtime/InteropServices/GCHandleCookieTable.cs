using System;
using System.Collections.Generic;
using System.Threading;

namespace System.Runtime.InteropServices
{
	internal class GCHandleCookieTable
	{
		internal GCHandleCookieTable()
		{
			this.m_HandleList = new IntPtr[10];
			this.m_CycleCounts = new byte[10];
			this.m_HandleToCookieMap = new Dictionary<IntPtr, IntPtr>(10);
			this.m_syncObject = new object();
			for (int i = 0; i < 10; i++)
			{
				this.m_HandleList[i] = IntPtr.Zero;
				this.m_CycleCounts[i] = 0;
			}
		}

		internal IntPtr FindOrAddHandle(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				return IntPtr.Zero;
			}
			IntPtr intPtr = IntPtr.Zero;
			object syncObject = this.m_syncObject;
			lock (syncObject)
			{
				if (this.m_HandleToCookieMap.ContainsKey(handle))
				{
					return this.m_HandleToCookieMap[handle];
				}
				if (this.m_FreeIndex < this.m_HandleList.Length && Volatile.Read(ref this.m_HandleList[this.m_FreeIndex]) == IntPtr.Zero)
				{
					Volatile.Write(ref this.m_HandleList[this.m_FreeIndex], handle);
					intPtr = this.GetCookieFromData((uint)this.m_FreeIndex, this.m_CycleCounts[this.m_FreeIndex]);
					this.m_FreeIndex++;
				}
				else
				{
					this.m_FreeIndex = 0;
					while (this.m_FreeIndex < 16777215)
					{
						if (this.m_HandleList[this.m_FreeIndex] == IntPtr.Zero)
						{
							Volatile.Write(ref this.m_HandleList[this.m_FreeIndex], handle);
							intPtr = this.GetCookieFromData((uint)this.m_FreeIndex, this.m_CycleCounts[this.m_FreeIndex]);
							this.m_FreeIndex++;
							break;
						}
						if (this.m_FreeIndex + 1 == this.m_HandleList.Length)
						{
							this.GrowArrays();
						}
						this.m_FreeIndex++;
					}
				}
				if (intPtr == IntPtr.Zero)
				{
					throw new OutOfMemoryException(Environment.GetResourceString("OutOfMemory_GCHandleMDA"));
				}
				this.m_HandleToCookieMap.Add(handle, intPtr);
			}
			return intPtr;
		}

		internal IntPtr GetHandle(IntPtr cookie)
		{
			IntPtr zero = IntPtr.Zero;
			if (!this.ValidateCookie(cookie))
			{
				return IntPtr.Zero;
			}
			return Volatile.Read(ref this.m_HandleList[this.GetIndexFromCookie(cookie)]);
		}

		internal void RemoveHandleIfPresent(IntPtr handle)
		{
			if (handle == IntPtr.Zero)
			{
				return;
			}
			object syncObject = this.m_syncObject;
			lock (syncObject)
			{
				if (this.m_HandleToCookieMap.ContainsKey(handle))
				{
					IntPtr cookie = this.m_HandleToCookieMap[handle];
					if (this.ValidateCookie(cookie))
					{
						int indexFromCookie = this.GetIndexFromCookie(cookie);
						byte[] cycleCounts = this.m_CycleCounts;
						int num = indexFromCookie;
						cycleCounts[num] += 1;
						Volatile.Write(ref this.m_HandleList[indexFromCookie], IntPtr.Zero);
						this.m_HandleToCookieMap.Remove(handle);
						this.m_FreeIndex = indexFromCookie;
					}
				}
			}
		}

		private bool ValidateCookie(IntPtr cookie)
		{
			int num;
			byte b;
			this.GetDataFromCookie(cookie, out num, out b);
			if (num >= 16777215)
			{
				return false;
			}
			if (num >= this.m_HandleList.Length)
			{
				return false;
			}
			if (Volatile.Read(ref this.m_HandleList[num]) == IntPtr.Zero)
			{
				return false;
			}
			byte b2 = (byte)(AppDomain.CurrentDomain.Id % 255);
			byte b3 = Volatile.Read(ref this.m_CycleCounts[num]) ^ b2;
			return b == b3;
		}

		private void GrowArrays()
		{
			int num = this.m_HandleList.Length;
			IntPtr[] array = new IntPtr[num * 2];
			byte[] array2 = new byte[num * 2];
			Array.Copy(this.m_HandleList, array, num);
			Array.Copy(this.m_CycleCounts, array2, num);
			this.m_HandleList = array;
			this.m_CycleCounts = array2;
		}

		private IntPtr GetCookieFromData(uint index, byte cycleCount)
		{
			byte b = (byte)(AppDomain.CurrentDomain.Id % 255);
			return (IntPtr)((long)((long)(cycleCount ^ b) << 24) + (long)((ulong)index) + 1L);
		}

		private void GetDataFromCookie(IntPtr cookie, out int index, out byte xorData)
		{
			uint num = (uint)((int)cookie);
			index = (int)((num & 16777215U) - 1U);
			xorData = (byte)((num & 4278190080U) >> 24);
		}

		private int GetIndexFromCookie(IntPtr cookie)
		{
			uint num = (uint)((int)cookie);
			return (int)((num & 16777215U) - 1U);
		}

		private const int InitialHandleCount = 10;

		private const int MaxListSize = 16777215;

		private const uint CookieMaskIndex = 16777215U;

		private const uint CookieMaskSentinal = 4278190080U;

		private Dictionary<IntPtr, IntPtr> m_HandleToCookieMap;

		private volatile IntPtr[] m_HandleList;

		private volatile byte[] m_CycleCounts;

		private int m_FreeIndex;

		private readonly object m_syncObject;
	}
}
