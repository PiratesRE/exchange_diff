using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery
{
	internal class ThrottleSessionMap
	{
		private protected ReaderWriterLockSlim RwLock { protected get; private set; }

		internal ThrottleSessionMap()
		{
			this.session = new Dictionary<long, ThrottleSession>();
			this.RwLock = new ReaderWriterLockSlim();
		}

		internal ThrottleSession TryGetSession(long sessionId)
		{
			this.RwLock.EnterReadLock();
			ThrottleSession result;
			try
			{
				ThrottleSession throttleSession;
				if (this.session.TryGetValue(sessionId, out throttleSession))
				{
					result = throttleSession;
				}
				else
				{
					result = null;
				}
			}
			finally
			{
				this.RwLock.ExitReadLock();
			}
			return result;
		}

		internal void AddSession(long sessionId)
		{
			this.RwLock.EnterUpgradeableReadLock();
			try
			{
				if (this.session.ContainsKey(sessionId))
				{
					throw new ArgumentException(string.Format("Failed to add session. Session Id={0} already exist.", sessionId));
				}
				this.RwLock.EnterWriteLock();
				try
				{
					this.session.Add(sessionId, new ThrottleSession(sessionId));
				}
				finally
				{
					this.RwLock.ExitWriteLock();
				}
			}
			finally
			{
				this.RwLock.ExitUpgradeableReadLock();
			}
		}

		internal void RemoveSession(long sessionId)
		{
			this.RwLock.EnterWriteLock();
			try
			{
				this.session.Remove(sessionId);
			}
			finally
			{
				this.RwLock.ExitWriteLock();
			}
		}

		internal void SetMdb(long sessionId, Guid mdbGuid)
		{
			this.RwLock.EnterUpgradeableReadLock();
			try
			{
				if (!this.session.ContainsKey(sessionId))
				{
					throw new ArgumentException(string.Format("Failed to set Mdb. Session Id={0} not found.", sessionId));
				}
				if (this.session[sessionId].Mdb != null)
				{
					throw new ArgumentException(string.Format("Failed to set Mdb for SessionId={0}: Mdb is already set for current session.", sessionId));
				}
				this.RwLock.EnterWriteLock();
				try
				{
					this.session[sessionId].Mdb = new Guid?(mdbGuid);
				}
				finally
				{
					this.RwLock.ExitWriteLock();
				}
			}
			finally
			{
				this.RwLock.ExitUpgradeableReadLock();
			}
		}

		internal void AddRecipient(long sessionId, RoutingAddress address)
		{
			this.RwLock.EnterUpgradeableReadLock();
			try
			{
				if (!this.session.ContainsKey(sessionId))
				{
					throw new ArgumentException(string.Format("Failed to add recipient. Session Id={0} not found.", sessionId));
				}
				this.RwLock.EnterWriteLock();
				try
				{
					int num = 0;
					if (this.session[sessionId].Recipients.ContainsKey(address))
					{
						num = this.session[sessionId].Recipients[address];
					}
					this.session[sessionId].Recipients[address] = num + 1;
				}
				finally
				{
					this.RwLock.ExitWriteLock();
				}
			}
			finally
			{
				this.RwLock.ExitUpgradeableReadLock();
			}
		}

		internal void RemoveRecipient(long sessionId, RoutingAddress address)
		{
			this.RwLock.EnterUpgradeableReadLock();
			try
			{
				if (this.session.Count != 0)
				{
					if (!this.session.ContainsKey(sessionId))
					{
						throw new ArgumentException(string.Format("Failed to remove recipient. Session Id={0} not found.", sessionId));
					}
					this.RwLock.EnterWriteLock();
					try
					{
						int num = this.session[sessionId].Recipients[address];
						if (num == 1)
						{
							this.session[sessionId].Recipients.Remove(address);
						}
						else
						{
							this.session[sessionId].Recipients[address] = num - 1;
						}
					}
					finally
					{
						this.RwLock.ExitWriteLock();
					}
				}
			}
			finally
			{
				this.RwLock.ExitUpgradeableReadLock();
			}
		}

		private Dictionary<long, ThrottleSession> session;
	}
}
