using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal class UserOpsTracker
	{
		public UserOpsTracker()
		{
			this.userOperations = new Dictionary<string, UserOp>(100);
			this.readerWriterLock = new FastReaderWriterLock();
		}

		public UserOp GetOperation(string user)
		{
			UserOp userOp = null;
			bool flag = false;
			this.readerWriterLock.AcquireReaderLock(-1);
			try
			{
				this.userOperations.TryGetValue(user, out userOp);
				if (userOp == null)
				{
					this.readerWriterLock.ReleaseReaderLock();
					this.readerWriterLock.AcquireWriterLock(-1);
					flag = true;
					this.userOperations.TryGetValue(user, out userOp);
					if (userOp == null)
					{
						userOp = new UserOp();
						userOp.User = user;
						this.userOperations.Add(user, userOp);
					}
				}
			}
			finally
			{
				Interlocked.Increment(ref userOp.refCount);
				if (flag)
				{
					this.readerWriterLock.ReleaseWriterLock();
				}
				else
				{
					this.readerWriterLock.ReleaseReaderLock();
				}
			}
			return userOp;
		}

		public void ReleaseOperation(UserOp clientOp)
		{
			if (Interlocked.Decrement(ref clientOp.refCount) == 0)
			{
				this.readerWriterLock.AcquireWriterLock(-1);
				try
				{
					if (clientOp.refCount == 0)
					{
						this.userOperations.Remove(clientOp.User);
						if (clientOp.HrdEvent != null)
						{
							clientOp.HrdEvent.Close();
						}
						if (clientOp.StsEvent != null)
						{
							clientOp.StsEvent.Close();
						}
					}
				}
				finally
				{
					this.readerWriterLock.ReleaseWriterLock();
				}
			}
		}

		private FastReaderWriterLock readerWriterLock;

		private Dictionary<string, UserOp> userOperations;
	}
}
