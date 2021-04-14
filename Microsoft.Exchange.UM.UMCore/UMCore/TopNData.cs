using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Microsoft.Exchange.Compliance.Serialization.Formatters;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class TopNData
	{
		protected TopNData(TopNData.CachedData cached, TopNData.ITopNWords topNWords, OffensiveWordsFilter offensiveWordsFilter)
		{
			this.cached = cached;
			this.offensiveWordsFilter = offensiveWordsFilter;
			if (this.cached.ContainsWords && this.cached.ScanTime == topNWords.LastScanTime)
			{
				this.isFiltered = true;
				this.isCached = true;
				return;
			}
			this.isFiltered = false;
			this.isCached = false;
			this.rawList = topNWords.WordList;
			this.rawScanTime = new ExDateTime?(topNWords.LastScanTime);
		}

		public static TopNData Create(UMSubscriber subscriber, OffensiveWordsFilter offensiveWordsFilter)
		{
			TopNData result;
			using (subscriber.CreateSessionLock())
			{
				TopNData.CachedData cachedData = new TopNData.MailboxCachedData(subscriber);
				TopNData.ITopNWords topNWords = new TopNData.EmptyTopNWords();
				result = new TopNData(cachedData, topNWords, offensiveWordsFilter);
			}
			return result;
		}

		public List<KeyValuePair<string, int>> GetFilteredList(BaseUMOfflineTranscriber transcriber)
		{
			List<KeyValuePair<string, int>> list = null;
			if (this.isFiltered)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, this.GetHashCode(), "TopNData::Filter has no work to do because the list is already filtered", new object[0]);
				list = this.cached.WordList;
			}
			else if (this.rawList == null || this.rawList.Count == 0 || this.rawScanTime == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, this.GetHashCode(), "TopNData::Filter has no work to do because the raw topn list is empty", new object[0]);
			}
			else
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, this.GetHashCode(), "TopNData::Filter running on a list of count '{0}'", new object[]
				{
					this.rawList.Count
				});
				Exception ex = null;
				try
				{
					list = transcriber.FilterWordsInLexion(this.rawList, 2500);
					list = this.offensiveWordsFilter.Filter(list);
					this.cached.WordList = list;
					this.cached.ScanTime = this.rawScanTime.Value;
					this.isFiltered = true;
					this.isCached = false;
				}
				catch (InvalidOperationException ex2)
				{
					ex = ex2;
				}
				catch (COMException ex3)
				{
					ex = ex3;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, this.GetHashCode(), "Filter ending with error='{0}'", new object[]
				{
					ex
				});
			}
			if (list == null)
			{
				list = new List<KeyValuePair<string, int>>(0);
			}
			if (list.Count > 2500)
			{
				list = list.GetRange(0, 2500);
			}
			return list;
		}

		public bool TryCache()
		{
			Exception ex = null;
			try
			{
				if (!this.isCached && this.isFiltered)
				{
					this.cached.Save();
					this.isCached = true;
				}
			}
			catch (StorageTransientException ex2)
			{
				ex = ex2;
			}
			catch (StoragePermanentException ex3)
			{
				ex = ex3;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, this.GetHashCode(), "TopNData::TryCache completed with error = '{0}'", new object[]
			{
				ex
			});
			return this.isCached;
		}

		public const int MaximumFilteredWords = 2500;

		private readonly TopNData.CachedData cached;

		private readonly OffensiveWordsFilter offensiveWordsFilter;

		private List<KeyValuePair<string, int>> rawList;

		private ExDateTime? rawScanTime;

		private bool isFiltered;

		private bool isCached;

		protected abstract class CachedData
		{
			public ExDateTime ScanTime { get; set; }

			public List<KeyValuePair<string, int>> WordList { get; set; }

			public bool ContainsWords
			{
				get
				{
					return this.WordList != null && this.WordList.Count > 0;
				}
			}

			public abstract void Save();
		}

		private class MailboxCachedData : TopNData.CachedData
		{
			public MailboxCachedData(UMSubscriber subscriber)
			{
				this.subscriber = subscriber;
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.subscriber.CreateSessionLock())
				{
					using (UserConfiguration config = this.GetConfig(mailboxSessionLock.Session))
					{
						this.ReadDictionary(config);
						this.ReadStream(config);
					}
				}
			}

			public override void Save()
			{
				if (base.ContainsWords)
				{
					using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.subscriber.CreateSessionLock())
					{
						using (UserConfiguration config = this.GetConfig(mailboxSessionLock.Session))
						{
							this.SaveDictionary(config);
							this.SaveStream(config);
							config.Save();
						}
					}
				}
			}

			private void ReadDictionary(UserConfiguration config)
			{
				IDictionary dictionary = config.GetDictionary();
				object obj = dictionary["ScanTime"];
				if (obj == null)
				{
					obj = ExDateTime.MinValue;
				}
				else if (!(obj is ExDateTime))
				{
					using (this.RebuildConfiguration())
					{
					}
					obj = ExDateTime.MinValue;
				}
				base.ScanTime = (ExDateTime)obj;
			}

			private void SaveDictionary(UserConfiguration config)
			{
				IDictionary dictionary = config.GetDictionary();
				dictionary["ScanTime"] = base.ScanTime;
			}

			private void ReadStream(UserConfiguration config)
			{
				using (Stream stream = config.GetStream())
				{
					Exception ex = null;
					List<KeyValuePair<string, int>> list = null;
					try
					{
						list = (List<KeyValuePair<string, int>>)TypedBinaryFormatter.DeserializeObject(stream, new Type[]
						{
							Type.GetType("System.Collections.Generic.List`1[[System.Collections.Generic.KeyValuePair`2[[System.String, mscorlib],[System.Int32, mscorlib]], mscorlib]]")
						}, null, true);
					}
					catch (ArgumentNullException ex2)
					{
						ex = ex2;
					}
					catch (SerializationException ex3)
					{
						ex = ex3;
					}
					finally
					{
						base.WordList = (list ?? new List<KeyValuePair<string, int>>());
						if (ex != null)
						{
							CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, this.GetHashCode(), "Error reading cached word list '{0}'", new object[]
							{
								ex
							});
						}
					}
				}
			}

			private void SaveStream(UserConfiguration config)
			{
				using (Stream stream = config.GetStream())
				{
					IFormatter formatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
					Exception ex = null;
					try
					{
						formatter.Serialize(stream, base.WordList);
					}
					catch (ArgumentNullException ex2)
					{
						ex = ex2;
					}
					catch (SerializationException ex3)
					{
						ex = ex3;
					}
					if (ex != null)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, this.GetHashCode(), "Error saving cached word list '{0}'", new object[]
						{
							ex
						});
					}
				}
			}

			private UserConfiguration GetConfig(MailboxSession s)
			{
				UserConfiguration result = null;
				StoreId defaultFolderId = s.GetDefaultFolderId(DefaultFolderType.Inbox);
				try
				{
					result = s.UserConfigurationManager.GetFolderConfiguration("TopNWords.Data.OutOfGrammar", UserConfigurationTypes.Stream | UserConfigurationTypes.Dictionary, defaultFolderId);
				}
				catch (ObjectNotFoundException)
				{
					result = s.UserConfigurationManager.CreateFolderConfiguration("TopNWords.Data.OutOfGrammar", UserConfigurationTypes.Stream | UserConfigurationTypes.Dictionary, defaultFolderId);
				}
				catch (CorruptDataException)
				{
					result = this.RebuildConfiguration();
				}
				catch (InvalidOperationException)
				{
					result = this.RebuildConfiguration();
				}
				return result;
			}

			private UserConfiguration RebuildConfiguration()
			{
				UserConfiguration result;
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.subscriber.CreateSessionLock())
				{
					StoreId defaultFolderId = mailboxSessionLock.Session.GetDefaultFolderId(DefaultFolderType.Inbox);
					mailboxSessionLock.Session.UserConfigurationManager.DeleteMailboxConfigurations(new string[]
					{
						"TopNWords.Data.OutOfGrammar"
					});
					result = mailboxSessionLock.Session.UserConfigurationManager.CreateFolderConfiguration("TopNWords.Data.OutOfGrammar", UserConfigurationTypes.Stream | UserConfigurationTypes.Dictionary, defaultFolderId);
				}
				return result;
			}

			private const string MessageClass = "TopNWords.Data.OutOfGrammar";

			private const string ScanTimeName = "ScanTime";

			private UMSubscriber subscriber;
		}

		protected interface ITopNWords
		{
			ExDateTime LastScanTime { get; }

			List<KeyValuePair<string, int>> WordList { get; }
		}

		protected class EmptyTopNWords : TopNData.ITopNWords
		{
			public EmptyTopNWords()
			{
				this.WordList = new List<KeyValuePair<string, int>>();
				this.LastScanTime = ExDateTime.Now.AddMinutes(-1.0);
			}

			public ExDateTime LastScanTime { get; private set; }

			public List<KeyValuePair<string, int>> WordList { get; private set; }
		}
	}
}
