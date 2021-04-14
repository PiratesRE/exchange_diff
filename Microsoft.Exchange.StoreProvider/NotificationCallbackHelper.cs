using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class NotificationCallbackHelper
	{
		private NotificationCallbackHelper()
		{
			this.callbackDictionaryArray = new NotificationCallbackHelper.CallbackDictionary[NotificationCallbackHelper.CallbackDictionaryArraySize];
			for (int i = 0; i < this.callbackDictionaryArray.Length; i++)
			{
				this.callbackDictionaryArray[i] = new NotificationCallbackHelper.CallbackDictionary();
			}
			this.onNotifyDelegate = new NotificationCallbackHelper.OnNotifyDelegate(this.OnNotify);
			this.intPtrOnNotifyDelegate = Marshal.GetFunctionPointerForDelegate(this.onNotifyDelegate);
		}

		internal unsafe int OnNotify(ulong notificationCallbackId, int cNotifications, IntPtr iNotifications)
		{
			try
			{
				if (ComponentTrace<MapiNetTags>.CheckEnabled(31))
				{
					ComponentTrace<MapiNetTags>.Trace<ulong, int>(0, 31, (long)this.GetHashCode(), "NotificationCallbackHelper.OnNotify params: notificationCallbackId={0}, cNotifications={1}", notificationCallbackId, cNotifications);
				}
				if (iNotifications != IntPtr.Zero && cNotifications > 0)
				{
					NotificationHelper notificationHelper = this.GetNotificationHelper(notificationCallbackId);
					if (notificationHelper != null)
					{
						NOTIFICATION* ptr = (NOTIFICATION*)((void*)iNotifications);
						for (int i = 0; i < cNotifications; i++)
						{
							MapiNotification notification = MapiNotification.Create(ptr + i);
							notificationHelper.OnNotify(notification);
						}
					}
				}
			}
			catch (Exception e)
			{
				return Marshal.GetHRForException(e);
			}
			return 0;
		}

		public IntPtr IntPtrOnNotifyDelegate
		{
			get
			{
				return this.intPtrOnNotifyDelegate;
			}
		}

		public ulong RegisterNotificationHelper(NotificationHelper notificationHelper)
		{
			ulong num = (ulong)Interlocked.Increment(ref this.nextNotificationCallbackId);
			this.GetCallbackDictionary(num).Add(num, notificationHelper);
			return num;
		}

		public void UnregisterNotificationHelper(ulong notificationCallbackId)
		{
			this.GetCallbackDictionary(notificationCallbackId).Remove(notificationCallbackId);
		}

		private NotificationCallbackHelper.CallbackDictionary GetCallbackDictionary(ulong notificationCallbackId)
		{
			return this.callbackDictionaryArray[(int)(checked((IntPtr)(notificationCallbackId % unchecked((ulong)NotificationCallbackHelper.CallbackDictionaryArraySize))))];
		}

		private NotificationHelper GetNotificationHelper(ulong notificationCallbackId)
		{
			return this.GetCallbackDictionary(notificationCallbackId).GetNotificationHelper(notificationCallbackId);
		}

		private static uint CallbackDictionaryArraySize = (uint)(4 * Environment.ProcessorCount);

		private readonly NotificationCallbackHelper.CallbackDictionary[] callbackDictionaryArray;

		private long nextNotificationCallbackId;

		private readonly NotificationCallbackHelper.OnNotifyDelegate onNotifyDelegate;

		private readonly IntPtr intPtrOnNotifyDelegate;

		public static readonly NotificationCallbackHelper Instance = new NotificationCallbackHelper();

		private class CallbackDictionary
		{
			public void Add(ulong notificationCallbackId, NotificationHelper notificationHelper)
			{
				try
				{
					this.callbackLock.EnterWriteLock();
					this.callbacks.Add(notificationCallbackId, notificationHelper);
				}
				finally
				{
					try
					{
						this.callbackLock.ExitWriteLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
			}

			public void Remove(ulong notificationCallbackId)
			{
				NotificationHelper notificationHelper = null;
				try
				{
					this.callbackLock.EnterWriteLock();
					if (this.callbacks.TryGetValue(notificationCallbackId, out notificationHelper))
					{
						this.callbacks.Remove(notificationCallbackId);
					}
				}
				finally
				{
					try
					{
						this.callbackLock.ExitWriteLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
				if (notificationHelper != null)
				{
					notificationHelper.Dispose();
				}
			}

			public NotificationHelper GetNotificationHelper(ulong notificationCallbackId)
			{
				try
				{
					this.callbackLock.EnterReadLock();
					NotificationHelper result;
					if (this.callbacks.TryGetValue(notificationCallbackId, out result))
					{
						return result;
					}
				}
				finally
				{
					try
					{
						this.callbackLock.ExitReadLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
				return null;
			}

			private readonly Dictionary<ulong, NotificationHelper> callbacks = new Dictionary<ulong, NotificationHelper>(1);

			private readonly ReaderWriterLockSlim callbackLock = new ReaderWriterLockSlim();
		}

		internal delegate int OnNotifyDelegate(ulong notificationCallbackId, int cNotifications, IntPtr iNotifications);
	}
}
