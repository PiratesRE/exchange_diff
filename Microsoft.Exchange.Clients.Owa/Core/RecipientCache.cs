using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal abstract class RecipientCache
	{
		protected RecipientCache(UserContext userContext, short cacheSize, UserConfiguration configuration)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			this.userContext = userContext;
			this.cacheSize = cacheSize;
			this.cacheEntries = new List<RecipientInfoCacheEntry>((int)cacheSize);
			if (userContext.CanActAsOwner)
			{
				this.Load(configuration);
			}
		}

		public static bool RunGetCacheOperationUnderDefaultExceptionHandler(RecipientCache.GetCacheOperation operation, int hashCode)
		{
			return RecipientCache.RunGetCacheOperationUnderExceptionHandler(operation, new RecipientCache.ExceptionHandler(RecipientCache.HandleGetCacheException), hashCode);
		}

		public static bool RunGetCacheOperationUnderExceptionHandler(RecipientCache.GetCacheOperation operation, RecipientCache.ExceptionHandler exceptionHandler, int hashCode)
		{
			try
			{
				operation();
				return true;
			}
			catch (DataSourceTransientException e)
			{
				exceptionHandler(e, hashCode);
			}
			catch (DataSourceOperationException e2)
			{
				exceptionHandler(e2, hashCode);
			}
			catch (StorageTransientException e3)
			{
				exceptionHandler(e3, hashCode);
			}
			catch (StoragePermanentException e4)
			{
				exceptionHandler(e4, hashCode);
			}
			catch (MapiRetryableException e5)
			{
				exceptionHandler(e5, hashCode);
			}
			catch (MapiPermanentException e6)
			{
				exceptionHandler(e6, hashCode);
			}
			return false;
		}

		protected UserContext UserContext
		{
			get
			{
				return this.userContext;
			}
		}

		public int CacheLength
		{
			get
			{
				return this.cacheEntries.Count;
			}
		}

		public bool IsDirty
		{
			get
			{
				return this.isDirty;
			}
		}

		public List<RecipientInfoCacheEntry> CacheEntries
		{
			get
			{
				return this.cacheEntries;
			}
		}

		public void Sort()
		{
			this.cacheEntries.Sort(new RecipientCache.UsageSort());
		}

		public void SortByDisplayName()
		{
			this.cacheEntries.Sort(new RecipientCache.DisplayNameSort());
		}

		public void AddEntry(string displayName, string smtpAddress, string routingAddress, string alias, string routingType, AddressOrigin addressOrigin, int recipientFlags, string itemId, EmailAddressIndex emailAddressIndex)
		{
			this.AddEntry(displayName, smtpAddress, routingAddress, alias, routingType, addressOrigin, recipientFlags, itemId, emailAddressIndex, null, null);
		}

		public void AddEntry(string displayName, string smtpAddress, string routingAddress, string alias, string routingType, AddressOrigin addressOrigin, int recipientFlags, string itemId, EmailAddressIndex emailAddressIndex, string sipUri, string mobilePhoneNumber)
		{
			if (!Utilities.IsMapiPDL(routingType) && string.IsNullOrEmpty(routingAddress))
			{
				return;
			}
			this.AddEntry(new RecipientInfoCacheEntry(displayName, smtpAddress, routingAddress, alias, routingType, addressOrigin, recipientFlags, itemId, emailAddressIndex, sipUri, mobilePhoneNumber));
		}

		public virtual void AddEntry(RecipientInfoCacheEntry newEntry)
		{
			if (newEntry == null)
			{
				throw new ArgumentNullException("newEntry");
			}
			int num;
			if (Utilities.IsMapiPDL(newEntry.RoutingType))
			{
				num = this.GetEntryIndexByItemId(newEntry.ItemId);
			}
			else
			{
				num = this.GetEntryIndexByEmail(newEntry.RoutingAddress);
			}
			int num2;
			if (num == -1)
			{
				if (this.cacheEntries.Count >= (int)this.cacheSize)
				{
					num2 = this.GetLeastPriorityEntryIndex();
					this.cacheEntries[num2] = newEntry;
				}
				else
				{
					num2 = this.cacheEntries.Count;
					this.cacheEntries.Add(newEntry);
				}
				newEntry.SetSessionFlag();
			}
			else
			{
				num2 = num;
				this.UpdateEntry(newEntry, num2);
			}
			this.cacheEntries[num2].UpdateTimeStamp();
			this.isDirty = true;
		}

		private static void HandleGetCacheException(Exception e, int hashCode)
		{
			ExTraceGlobals.CoreCallTracer.TraceError<string>((long)hashCode, "Failed to get cache from server. Exception {0}", e.Message);
		}

		private void UpdateEntry(RecipientInfoCacheEntry newEntry, int oldEntryIndex)
		{
			RecipientInfoCacheEntry recipientInfoCacheEntry = this.cacheEntries[oldEntryIndex];
			recipientInfoCacheEntry.IncrementUsage();
			recipientInfoCacheEntry.SetSessionFlag();
			if (!string.Equals(newEntry.DisplayName, newEntry.RoutingAddress))
			{
				recipientInfoCacheEntry.DisplayName = newEntry.DisplayName;
			}
			if (newEntry.Alias.Length > 0)
			{
				recipientInfoCacheEntry.Alias = newEntry.Alias;
			}
			recipientInfoCacheEntry.RoutingType = newEntry.RoutingType;
			recipientInfoCacheEntry.ItemId = newEntry.ItemId;
			recipientInfoCacheEntry.EmailAddressIndex = newEntry.EmailAddressIndex;
			recipientInfoCacheEntry.AddressOrigin = newEntry.AddressOrigin;
			recipientInfoCacheEntry.SipUri = newEntry.SipUri;
			recipientInfoCacheEntry.MobilePhoneNumber = newEntry.MobilePhoneNumber;
		}

		private int GetEntryIndexByEmail(string routingAddress)
		{
			if (string.IsNullOrEmpty(routingAddress))
			{
				return -1;
			}
			for (int i = 0; i < this.cacheEntries.Count; i++)
			{
				if (string.Equals(this.cacheEntries[i].RoutingAddress, routingAddress, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
			return -1;
		}

		private int GetEntryIndexByItemId(string itemId)
		{
			if (string.IsNullOrEmpty(itemId))
			{
				return -1;
			}
			for (int i = 0; i < this.cacheEntries.Count; i++)
			{
				if (string.CompareOrdinal(this.cacheEntries[i].ItemId, itemId) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		private int GetLeastPriorityEntryIndex()
		{
			IComparer<RecipientInfoCacheEntry> comparer = new RecipientCache.UsageSort();
			int num = 0;
			for (int i = 1; i < this.cacheEntries.Count; i++)
			{
				if (comparer.Compare(this.cacheEntries[i], this.cacheEntries[num]) > 0)
				{
					num = i;
				}
			}
			return num;
		}

		private void ShiftBackEntries(int index)
		{
			this.cacheEntries.RemoveAt(index);
		}

		internal void RemoveEntry(string email, string id)
		{
			if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(id))
			{
				throw new OwaInvalidRequestException("email and id can not be both empty");
			}
			int num;
			if (!string.IsNullOrEmpty(email))
			{
				num = this.GetEntryIndexByEmail(email);
			}
			else
			{
				num = this.GetEntryIndexByItemId(id);
			}
			if (num == -1)
			{
				return;
			}
			this.ShiftBackEntries(num);
			this.isDirty = true;
		}

		protected void FinishSession(RecipientCache backEndCache, UserConfiguration configuration)
		{
			this.Merge(backEndCache);
			foreach (RecipientInfoCacheEntry recipientInfoCacheEntry in this.cacheEntries)
			{
				recipientInfoCacheEntry.Decay();
			}
			this.Commit(configuration);
		}

		internal void StartCacheSession()
		{
			foreach (RecipientInfoCacheEntry recipientInfoCacheEntry in this.cacheEntries)
			{
				recipientInfoCacheEntry.ClearSessionFlag();
			}
		}

		public void ClearCache()
		{
			this.cacheEntries.Clear();
			this.isDirty = true;
		}

		protected void Merge(RecipientCache externalRecipientCache)
		{
			if (externalRecipientCache == null)
			{
				return;
			}
			if (!(externalRecipientCache.syncTime > this.syncTime))
			{
				return;
			}
			int cacheLength = externalRecipientCache.CacheLength;
			int count = this.cacheEntries.Count;
			if (cacheLength <= 0)
			{
				return;
			}
			int num = cacheLength + count;
			List<RecipientInfoCacheEntry> list = new List<RecipientInfoCacheEntry>((num > (int)this.cacheSize) ? num : ((int)this.cacheSize));
			IComparer<RecipientInfoCacheEntry> comparer = new RecipientCache.PrimaryKeySort();
			this.cacheEntries.Sort(comparer);
			externalRecipientCache.cacheEntries.Sort(comparer);
			int num2 = 0;
			int num3 = 0;
			while (num2 < count || num3 < cacheLength)
			{
				RecipientInfoCacheEntry recipientInfoCacheEntry;
				if (num2 < count)
				{
					recipientInfoCacheEntry = this.CacheEntries[num2];
				}
				else
				{
					recipientInfoCacheEntry = null;
				}
				RecipientInfoCacheEntry recipientInfoCacheEntry2;
				if (num3 < cacheLength)
				{
					recipientInfoCacheEntry2 = externalRecipientCache.CacheEntries[num3];
				}
				else
				{
					recipientInfoCacheEntry2 = null;
				}
				int num4 = comparer.Compare(recipientInfoCacheEntry, recipientInfoCacheEntry2);
				int num5 = 0;
				if (recipientInfoCacheEntry != null && recipientInfoCacheEntry2 != null)
				{
					num5 = (int)(Convert.ToByte(Utilities.IsMapiPDL(recipientInfoCacheEntry.RoutingType)) | Convert.ToByte(Utilities.IsMapiPDL(recipientInfoCacheEntry2.RoutingType)));
				}
				if (num4 < 0 || num5 == 1)
				{
					if (recipientInfoCacheEntry.IsSessionFlagSet())
					{
						list.Add(recipientInfoCacheEntry);
					}
					num2++;
				}
				else if (num4 == 0)
				{
					short sessionCount = Math.Max(recipientInfoCacheEntry.SessionCount, recipientInfoCacheEntry2.SessionCount);
					if (recipientInfoCacheEntry2.DateTimeTicks > recipientInfoCacheEntry.DateTimeTicks)
					{
						this.UpdateEntry(recipientInfoCacheEntry2, num2);
						list.Add(this.cacheEntries[num2]);
					}
					else
					{
						list.Add(recipientInfoCacheEntry);
					}
					list[list.Count - 1].SessionCount = sessionCount;
					num2++;
					num3++;
				}
				else
				{
					list.Add(recipientInfoCacheEntry2);
					num3++;
				}
			}
			list.Sort(new RecipientCache.UsageSort());
			if (num > (int)this.cacheSize && list.Count > (int)this.cacheSize)
			{
				this.cacheEntries = list.GetRange(0, (int)this.cacheSize);
				return;
			}
			this.cacheEntries = list;
		}

		public abstract void Commit(bool mergeBeforeCommit);

		internal virtual void Commit(RecipientCache backEndRecipientCache, UserConfiguration configuration)
		{
			this.Merge(backEndRecipientCache);
			this.Commit(configuration);
		}

		protected virtual void Commit(UserConfiguration configuration)
		{
			this.Commit(configuration, false);
		}

		protected virtual void Commit(UserConfiguration configuration, bool forceSave)
		{
			if (this.userContext.CanActAsOwner)
			{
				if (this.cacheEntries.Count == 0 && !this.isDirty && !forceSave)
				{
					return;
				}
				using (RecipientInfoCache recipientInfoCache = RecipientInfoCache.Create(configuration))
				{
					try
					{
						recipientInfoCache.Save(this.cacheEntries, "AutoCompleteCache", (int)this.cacheSize);
					}
					finally
					{
						recipientInfoCache.DetachUserConfiguration();
					}
				}
			}
			this.isDirty = false;
		}

		private void Load(UserConfiguration configuration)
		{
			try
			{
				using (RecipientInfoCache recipientInfoCache = RecipientInfoCache.Create(configuration))
				{
					try
					{
						this.cacheEntries = recipientInfoCache.Load("AutoCompleteCache");
						this.syncTime = recipientInfoCache.LastModifiedTime;
					}
					finally
					{
						recipientInfoCache.DetachUserConfiguration();
					}
				}
			}
			catch (XmlException ex)
			{
				ExTraceGlobals.CoreTracer.TraceError<string>(0L, "Parser threw an XML exception: {0}'", ex.Message);
				this.ClearCache();
				this.Commit(configuration, true);
			}
			catch (CorruptDataException ex2)
			{
				ExTraceGlobals.CoreTracer.TraceError<string>(0L, "Parser threw a CorruptDataException exception: {0}'", ex2.Message);
				this.ClearCache();
				this.Commit(configuration, true);
			}
		}

		private const string AutoCompleteParamName = "AutoCompleteCache";

		private const string AutoCompleteEntryName = "entry";

		private const string AutoCompleteCacheVersionName = "version";

		private short cacheSize = 100;

		private bool isDirty;

		private UserContext userContext;

		private List<RecipientInfoCacheEntry> cacheEntries;

		private ExDateTime syncTime;

		public delegate void GetCacheOperation();

		public delegate void ExceptionHandler(Exception e, int hashCode);

		private class DisplayNameSort : IComparer<RecipientInfoCacheEntry>
		{
			public int Compare(RecipientInfoCacheEntry x, RecipientInfoCacheEntry y)
			{
				if (x == null && y == null)
				{
					return 0;
				}
				if (x == null)
				{
					return 1;
				}
				if (y == null)
				{
					return -1;
				}
				if (x.DisplayName != null && y.DisplayName != null)
				{
					return x.DisplayName.CompareTo(y.DisplayName);
				}
				if (x.DisplayName != null && y.DisplayName == null)
				{
					return 1;
				}
				if (x.DisplayName == null && y.DisplayName != null)
				{
					return -1;
				}
				return 0;
			}
		}

		private class PrimaryKeySort : IComparer<RecipientInfoCacheEntry>
		{
			public int Compare(RecipientInfoCacheEntry x, RecipientInfoCacheEntry y)
			{
				if (x == null && y == null)
				{
					return 0;
				}
				if (x == null)
				{
					return 1;
				}
				if (y == null)
				{
					return -1;
				}
				string text = x.RoutingAddress;
				string text2 = y.RoutingAddress;
				if (Utilities.IsMapiPDL(x.RoutingType))
				{
					text = x.ItemId;
				}
				if (Utilities.IsMapiPDL(y.RoutingType))
				{
					text2 = y.ItemId;
				}
				if (text != null && text2 != null)
				{
					return string.CompareOrdinal(text, text2);
				}
				if (text != null && text2 == null)
				{
					return -1;
				}
				if (text == null && text2 != null)
				{
					return 1;
				}
				return 0;
			}
		}

		private class UsageSort : IComparer<RecipientInfoCacheEntry>
		{
			public int Compare(RecipientInfoCacheEntry x, RecipientInfoCacheEntry y)
			{
				if (x == null && y == null)
				{
					return 0;
				}
				if (x == null)
				{
					return 1;
				}
				if (y == null)
				{
					return -1;
				}
				if (y.Usage != x.Usage)
				{
					return y.Usage.CompareTo(x.Usage);
				}
				if (y.DateTimeTicks != x.DateTimeTicks)
				{
					return y.DateTimeTicks.CompareTo(x.DateTimeTicks);
				}
				return x.DisplayName.CompareTo(y.DisplayName);
			}
		}
	}
}
