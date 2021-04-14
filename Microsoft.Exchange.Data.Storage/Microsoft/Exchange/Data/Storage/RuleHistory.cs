using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RuleHistory : ICollection<long>, IEnumerable<long>, IEnumerable
	{
		internal RuleHistory(Item item, byte[] ruleHistoryData, StoreSession session)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (ruleHistoryData == null)
			{
				throw new ArgumentNullException("ruleHistoryData");
			}
			this.item = item;
			this.gids = new Collection<byte[]>();
			this.session = session;
			this.Initialize(ruleHistoryData);
		}

		private void Initialize(byte[] ruleHistoryData)
		{
			int num = ruleHistoryData.Length / 22;
			int i = 0;
			int num2 = 0;
			while (i < num)
			{
				byte[] destinationArray = new byte[22];
				Array.Copy(ruleHistoryData, num2, destinationArray, 0, 22);
				this.gids.Add(destinationArray);
				i++;
				num2 += 22;
			}
		}

		public void Save()
		{
			byte[] value = this.Serialize();
			this.item[ItemSchema.RuleTriggerHistory] = value;
		}

		private byte[] Serialize()
		{
			byte[] array = new byte[22 * this.gids.Count];
			int num = 0;
			foreach (byte[] sourceArray in this.gids)
			{
				Array.Copy(sourceArray, 0, array, num, 22);
				num += 22;
			}
			return array;
		}

		public void Add(long item)
		{
			StoreSession storeSession = this.session;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				this.gids.Add(this.session.Mailbox.MapiStore.GlobalIdFromId(item));
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.RuleHistoryError, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("RuleHistory.Add. item = {0}.", item),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.RuleHistoryError, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("RuleHistory.Add. item = {0}.", item),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
		}

		public void Clear()
		{
			this.gids.Clear();
		}

		public bool Contains(long item)
		{
			StoreSession storeSession = this.session;
			bool flag = false;
			byte[] x;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				x = this.session.Mailbox.MapiStore.GlobalIdFromId(item);
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.RuleHistoryError, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("RuleHistory.Contains. item = {0}.", item),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.RuleHistoryError, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("RuleHistory.Contains. item = {0}.", item),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			ArrayComparer<byte> comparer = ArrayComparer<byte>.Comparer;
			foreach (byte[] y in this.gids)
			{
				if (comparer.Equals(x, y))
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(long[] array, int arrayIndex)
		{
			throw new NotSupportedException();
		}

		public int Count
		{
			get
			{
				return this.gids.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool Remove(long item)
		{
			StoreSession storeSession = this.session;
			bool flag = false;
			bool result;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				result = this.gids.Remove(this.session.Mailbox.MapiStore.GlobalIdFromId(item));
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.RuleHistoryError, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("RuleHistory.Remove. item = {0}.", item),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.RuleHistoryError, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("RuleHistory.Remove. item = {0}.", item),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			return result;
		}

		public IEnumerator<long> GetEnumerator()
		{
			throw new NotSupportedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		internal const int GidSize = 22;

		private readonly Item item;

		private readonly StoreSession session;

		private readonly Collection<byte[]> gids;
	}
}
