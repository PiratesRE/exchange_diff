using System;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Directory
{
	internal class SystemAddressListMemberCountCacheValue
	{
		internal SystemAddressListMemberCountCacheValue(AddressBookBase systemAddressList)
		{
			this.cachedCountLock = new ReaderWriterLockSlim();
			if (systemAddressList == null)
			{
				this.systemAddressListExists = false;
				this.memberCount = 0;
				this.lifetime = TimeSpan.MaxValue;
				return;
			}
			this.systemAddressListGuid = systemAddressList.Guid;
		}

		internal int InitializeMemberCount(IConfigurationSession session, ExDateTime now, int quota)
		{
			try
			{
				if (!this.cachedCountLock.TryEnterReadLock(SystemAddressListMemberCountCacheValue.readerLockTimeout))
				{
					throw new TransientException(DirectoryStrings.ErrorTimeoutReadingSystemAddressListMemberCount);
				}
				if (!this.systemAddressListExists)
				{
					return 0;
				}
			}
			finally
			{
				try
				{
					this.cachedCountLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			SystemAddressListMemberCountCacheValue.UpdateInfo o = new SystemAddressListMemberCountCacheValue.UpdateInfo(session, now, quota);
			Interlocked.Increment(ref this.asyncUpdatesInProgress);
			this.UpdateMemberCount(o);
			this.ThrowAnySavedExceptionsFromAsyncThreads();
			return this.memberCount;
		}

		internal int GetMemberCount(IConfigurationSession session, ExDateTime now, int quota)
		{
			int result = 0;
			this.ThrowAnySavedExceptionsFromAsyncThreads();
			try
			{
				if (!this.cachedCountLock.TryEnterReadLock(SystemAddressListMemberCountCacheValue.readerLockTimeout))
				{
					throw new TransientException(DirectoryStrings.ErrorTimeoutReadingSystemAddressListMemberCount);
				}
				if (this.systemAddressListExists && this.lastQueriedTime + this.lifetime < now)
				{
					SystemAddressListMemberCountCacheValue.UpdateInfo updateInfo = new SystemAddressListMemberCountCacheValue.UpdateInfo(session, now, quota);
					this.StartAsyncUpdate(updateInfo);
				}
				result = this.memberCount;
			}
			finally
			{
				try
				{
					this.cachedCountLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			return result;
		}

		internal int GetMemberCountImmediate(IConfigurationSession session)
		{
			int num = 0;
			try
			{
				if (!this.cachedCountLock.TryEnterReadLock(SystemAddressListMemberCountCacheValue.readerLockTimeout))
				{
					throw new TransientException(DirectoryStrings.ErrorTimeoutReadingSystemAddressListMemberCount);
				}
				if (!this.systemAddressListExists)
				{
					throw new ADNoSuchObjectException(DirectoryStrings.SystemAddressListDoesNotExist);
				}
				num = AddressBookBase.GetAddressListSize(session, this.systemAddressListGuid);
				if (num == -1)
				{
					throw new ADNoSuchObjectException(DirectoryStrings.SystemAddressListDoesNotExist);
				}
			}
			finally
			{
				try
				{
					this.cachedCountLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			return num;
		}

		private void StartAsyncUpdate(SystemAddressListMemberCountCacheValue.UpdateInfo updateInfo)
		{
			if (this.asyncUpdatesInProgress == 0)
			{
				Interlocked.Increment(ref this.asyncUpdatesInProgress);
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.UpdateMemberCount), updateInfo);
			}
		}

		private void UpdateMemberCount(object o)
		{
			try
			{
				SystemAddressListMemberCountCacheValue.UpdateInfo updateInfo = (SystemAddressListMemberCountCacheValue.UpdateInfo)o;
				Exception ex = null;
				int num = 0;
				try
				{
					num = AddressBookBase.GetAddressListSize(updateInfo.Session, this.systemAddressListGuid);
				}
				catch (Exception ex2)
				{
					ex = ex2;
				}
				try
				{
					this.cachedCountLock.EnterWriteLock();
					if (ex != null)
					{
						this.asyncException = ex;
					}
					else if (num == -1)
					{
						this.systemAddressListExists = false;
						this.memberCount = 0;
						this.lastQueriedTime = updateInfo.Now;
						this.lifetime = TimeSpan.MaxValue;
					}
					else
					{
						this.memberCount = num;
						this.lastQueriedTime = updateInfo.Now;
						this.lifetime = this.CalculateValidLifetime(updateInfo.Quota);
					}
				}
				finally
				{
					try
					{
						this.cachedCountLock.ExitWriteLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
			}
			finally
			{
				Interlocked.Decrement(ref this.asyncUpdatesInProgress);
			}
		}

		private void ThrowAnySavedExceptionsFromAsyncThreads()
		{
			try
			{
				if (!this.cachedCountLock.TryEnterUpgradeableReadLock(SystemAddressListMemberCountCacheValue.readerLockTimeout))
				{
					throw new TransientException(DirectoryStrings.ErrorTimeoutReadingSystemAddressListMemberCount);
				}
				if (this.asyncException != null)
				{
					try
					{
						if (!this.cachedCountLock.TryEnterWriteLock(SystemAddressListMemberCountCacheValue.writerLockTimeout))
						{
							throw new TransientException(DirectoryStrings.ErrorTimeoutWritingSystemAddressListMemberCount);
						}
						Exception ex = this.asyncException;
						this.asyncException = null;
						throw ex;
					}
					finally
					{
						try
						{
							this.cachedCountLock.ExitWriteLock();
						}
						catch (SynchronizationLockException)
						{
						}
					}
				}
			}
			finally
			{
				try
				{
					this.cachedCountLock.ExitUpgradeableReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
		}

		private TimeSpan CalculateValidLifetime(int quota)
		{
			double num = (T)ThrottlingPolicyDefaults.ExchangeMaxCmdlets / (T)ThrottlingPolicyDefaults.PowerShellMaxCmdletsTimePeriod;
			double num2 = (double)(quota - this.memberCount);
			if (num2 < 0.0)
			{
				num2 = 0.0;
			}
			int num3 = Convert.ToInt32(num2 / num);
			if (num3 < 30)
			{
				return TimeSpan.FromSeconds(0.0);
			}
			if (num3 > 300)
			{
				return TimeSpan.FromSeconds(300.0);
			}
			return TimeSpan.FromSeconds((double)num3);
		}

		private static readonly TimeSpan readerLockTimeout = TimeSpan.FromSeconds(120.0);

		private static readonly TimeSpan writerLockTimeout = TimeSpan.FromSeconds(300.0);

		private Guid systemAddressListGuid;

		private int memberCount = int.MinValue;

		private ExDateTime lastQueriedTime = (ExDateTime)DateTime.MinValue;

		private TimeSpan lifetime;

		private bool systemAddressListExists = true;

		private ReaderWriterLockSlim cachedCountLock;

		private int asyncUpdatesInProgress;

		private Exception asyncException;

		private class UpdateInfo
		{
			internal UpdateInfo(IConfigurationSession session, ExDateTime now, int quota)
			{
				this.Session = session;
				this.Now = now;
				this.Quota = quota;
			}

			internal IConfigurationSession Session;

			internal ExDateTime Now;

			internal int Quota;
		}
	}
}
