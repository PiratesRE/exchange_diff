using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	internal sealed class Tracker
	{
		public bool IsPending(Guid mailboxGuid, StoreId folderId)
		{
			string item = this.ConvertToKey(mailboxGuid, folderId);
			bool result;
			lock (this.locker)
			{
				result = this.pending.Contains(item);
			}
			return result;
		}

		public bool Start(Guid mailboxGuid, StoreId folderId)
		{
			string item = this.ConvertToKey(mailboxGuid, folderId);
			lock (this.locker)
			{
				if (this.pending.Contains(item))
				{
					return false;
				}
				this.pending.Add(item);
			}
			return true;
		}

		public void End(Guid mailboxGuid, StoreId folderId)
		{
			string item = this.ConvertToKey(mailboxGuid, folderId);
			lock (this.locker)
			{
				this.pending.Remove(item);
			}
		}

		private string ConvertToKey(Guid mailboxGuid, StoreId folderId)
		{
			return mailboxGuid.ToString() + "/" + folderId.ToBase64String();
		}

		private HashSet<string> pending = new HashSet<string>();

		private object locker = new object();
	}
}
