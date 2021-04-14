using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;

namespace Microsoft.Exchange.RpcHttpModules
{
	public class CachedDirectory : IDirectory
	{
		public CachedDirectory(IDirectory wrappedDirectory)
		{
			if (wrappedDirectory == null)
			{
				throw new ArgumentNullException("wrappedDirectory");
			}
			this.wrappedDirectory = wrappedDirectory;
		}

		internal static CachedDirectory DefaultCachedDirectory
		{
			get
			{
				if (CachedDirectory.defaultCachedDirectory == null)
				{
					lock (CachedDirectory.defaultObjectLock)
					{
						if (CachedDirectory.defaultCachedDirectory == null)
						{
							CachedDirectory.defaultCachedDirectory = new CachedDirectory(new Directory());
						}
					}
				}
				return CachedDirectory.defaultCachedDirectory;
			}
		}

		public SecurityIdentifier GetExchangeServersUsgSid()
		{
			if (this.exchangeServersUsgSid == null)
			{
				this.exchangeServersUsgSid = this.wrappedDirectory.GetExchangeServersUsgSid();
			}
			return this.exchangeServersUsgSid;
		}

		public bool AllowsTokenSerializationBy(WindowsIdentity windowsIdentity)
		{
			if (windowsIdentity == null)
			{
				throw new ArgumentNullException("windowsIdentity");
			}
			SecurityIdentifier user = windowsIdentity.User;
			try
			{
				this.syncLock.EnterReadLock();
				if (this.verifiedCallers.Contains(user))
				{
					return true;
				}
			}
			finally
			{
				try
				{
					this.syncLock.ExitReadLock();
				}
				catch (SynchronizationLockException)
				{
				}
			}
			bool flag = this.wrappedDirectory.AllowsTokenSerializationBy(windowsIdentity);
			if (flag)
			{
				try
				{
					this.syncLock.EnterWriteLock();
					this.verifiedCallers.Add(user);
				}
				finally
				{
					try
					{
						this.syncLock.ExitWriteLock();
					}
					catch (SynchronizationLockException)
					{
					}
				}
			}
			return flag;
		}

		private static CachedDirectory defaultCachedDirectory = null;

		private static object defaultObjectLock = new object();

		private IDirectory wrappedDirectory;

		private SecurityIdentifier exchangeServersUsgSid;

		private HashSet<SecurityIdentifier> verifiedCallers = new HashSet<SecurityIdentifier>();

		private ReaderWriterLockSlim syncLock = new ReaderWriterLockSlim();
	}
}
