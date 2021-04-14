using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class FolderMruCache : IComparer
	{
		private FolderMruCache(UserContext userContext)
		{
			this.userContext = userContext;
			this.cacheEntries = new FolderMruCacheEntry[20];
			this.Load();
		}

		public FolderMruCacheEntry[] CacheEntries
		{
			get
			{
				return this.cacheEntries;
			}
		}

		public int CacheLength
		{
			get
			{
				return this.cacheLength;
			}
		}

		public static FolderMruCache GetCacheInstance(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			FolderMruCache folderMruCache = null;
			if (!userContext.IsMruSessionStarted)
			{
				userContext.IsMruSessionStarted = true;
				folderMruCache = new FolderMruCache(userContext);
				folderMruCache.StartCacheSession();
				folderMruCache.Commit();
			}
			if (folderMruCache == null)
			{
				return new FolderMruCache(userContext);
			}
			return folderMruCache;
		}

		public static void DeleteFromCache(StoreObjectId folderId, UserContext userContext)
		{
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			FolderMruCache cacheInstance = FolderMruCache.GetCacheInstance(userContext);
			int entryIndexByFolderId = cacheInstance.GetEntryIndexByFolderId(folderId);
			if (entryIndexByFolderId == -1)
			{
				return;
			}
			cacheInstance.ShiftBackEntries(entryIndexByFolderId + 1);
			cacheInstance.Commit();
		}

		public static void FinishCacheSession(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			FolderMruCache cacheInstance = FolderMruCache.GetCacheInstance(userContext);
			cacheInstance.InternalFinishCacheSession();
			cacheInstance.Commit();
		}

		int IComparer.Compare(object x, object y)
		{
			FolderMruCacheEntry folderMruCacheEntry = x as FolderMruCacheEntry;
			FolderMruCacheEntry folderMruCacheEntry2 = y as FolderMruCacheEntry;
			if (folderMruCacheEntry == null && folderMruCacheEntry2 == null)
			{
				return 0;
			}
			if (folderMruCacheEntry == null)
			{
				return 1;
			}
			if (folderMruCacheEntry2 == null)
			{
				return -1;
			}
			if (folderMruCacheEntry2.UsageCount != folderMruCacheEntry.UsageCount)
			{
				return folderMruCacheEntry2.UsageCount.CompareTo(folderMruCacheEntry.UsageCount);
			}
			return folderMruCacheEntry2.LastAccessedDateTimeTicks.CompareTo(folderMruCacheEntry.LastAccessedDateTimeTicks);
		}

		public void Sort()
		{
			Array.Sort(this.cacheEntries, this);
		}

		private static int ComputeDecay(double percent, int setDecay, int usageCount)
		{
			int num = (int)Math.Round(percent * (double)usageCount);
			int result;
			if (num > setDecay)
			{
				result = num;
			}
			else
			{
				result = setDecay;
			}
			return result;
		}

		public void AddEntry(FolderMruCacheEntry newEntry)
		{
			if (newEntry == null)
			{
				throw new ArgumentNullException("newEntry");
			}
			int entryIndexByFolderId = this.GetEntryIndexByFolderId(newEntry.FolderId);
			int num;
			if (entryIndexByFolderId == -1)
			{
				if (this.firstEmptyIndex == -1)
				{
					num = this.GetLeastUsageEntryIndex();
				}
				else
				{
					num = this.firstEmptyIndex;
				}
				newEntry.SetInitialUsage();
				this.cacheEntries[num] = newEntry;
				if (this.firstEmptyIndex != -1 && this.firstEmptyIndex < 19)
				{
					this.firstEmptyIndex++;
					this.cacheLength = this.firstEmptyIndex;
				}
				else
				{
					this.firstEmptyIndex = -1;
					this.cacheLength = 20;
				}
			}
			else
			{
				num = entryIndexByFolderId;
				this.UpdateEntry(newEntry, num);
			}
			this.cacheEntries[num].UpdateTimeStamp();
		}

		private void UpdateEntry(FolderMruCacheEntry newEntry, int oldEntryIndex)
		{
			FolderMruCacheEntry folderMruCacheEntry = this.cacheEntries[oldEntryIndex];
			folderMruCacheEntry.UsageCount++;
			folderMruCacheEntry.UsedInCurrentSession = true;
		}

		private void ShiftBackEntries(int index)
		{
			int i = index;
			if (index < this.cacheEntries.Length)
			{
				while (i < this.cacheLength)
				{
					this.cacheEntries[i - 1] = this.cacheEntries[i];
					i++;
				}
			}
			this.cacheEntries[i - 1] = null;
			if (this.firstEmptyIndex > 0)
			{
				this.firstEmptyIndex--;
			}
			else if (this.firstEmptyIndex == -1)
			{
				this.firstEmptyIndex = 19;
			}
			this.cacheLength = this.firstEmptyIndex;
		}

		public int GetEntryIndexByFolderId(StoreObjectId folderId)
		{
			if (folderId == null)
			{
				return -1;
			}
			for (int i = 0; i < this.cacheLength; i++)
			{
				if (folderId.Equals(this.cacheEntries[i].FolderId))
				{
					return i;
				}
			}
			return -1;
		}

		private int GetLeastUsageEntryIndex()
		{
			int num = 0;
			for (int i = 1; i < this.cacheLength; i++)
			{
				if (((IComparer)this).Compare(this.cacheEntries[i], this.cacheEntries[num]) > 0)
				{
					num = i;
				}
			}
			return num;
		}

		private void InternalFinishCacheSession()
		{
			int num = -1;
			int i = 0;
			while (i < this.cacheLength)
			{
				FolderMruCacheEntry folderMruCacheEntry = this.cacheEntries[i];
				if (!folderMruCacheEntry.UsedInCurrentSession)
				{
					folderMruCacheEntry.NumberOfSessionsSinceLastUse++;
				}
				else
				{
					folderMruCacheEntry.NumberOfSessionsSinceLastUse = 0;
				}
				int numberOfSessionsSinceLastUse = folderMruCacheEntry.NumberOfSessionsSinceLastUse;
				if (numberOfSessionsSinceLastUse <= 12)
				{
					if (numberOfSessionsSinceLastUse <= 6)
					{
						if (numberOfSessionsSinceLastUse != 3 && numberOfSessionsSinceLastUse != 6)
						{
							goto IL_E9;
						}
					}
					else if (numberOfSessionsSinceLastUse != 9)
					{
						if (numberOfSessionsSinceLastUse != 12)
						{
							goto IL_E9;
						}
						num = FolderMruCache.ComputeDecay(0.25, 2, folderMruCacheEntry.UsageCount);
						goto IL_FA;
					}
					num = 1;
				}
				else if (numberOfSessionsSinceLastUse <= 23)
				{
					if (numberOfSessionsSinceLastUse != 17)
					{
						if (numberOfSessionsSinceLastUse != 23)
						{
							goto IL_E9;
						}
						num = FolderMruCache.ComputeDecay(0.5, 4, folderMruCacheEntry.UsageCount);
					}
					else
					{
						num = FolderMruCache.ComputeDecay(0.25, 3, folderMruCacheEntry.UsageCount);
					}
				}
				else if (numberOfSessionsSinceLastUse != 26)
				{
					if (numberOfSessionsSinceLastUse != 31)
					{
						goto IL_E9;
					}
					num = 0;
					folderMruCacheEntry.UsageCount = 0;
				}
				else
				{
					num = FolderMruCache.ComputeDecay(0.75, 5, folderMruCacheEntry.UsageCount);
				}
				IL_FA:
				if (num > 0)
				{
					folderMruCacheEntry.DecayUsage(num);
				}
				i++;
				continue;
				IL_E9:
				if (folderMruCacheEntry.NumberOfSessionsSinceLastUse > 31)
				{
					folderMruCacheEntry.UsageCount = 0;
					goto IL_FA;
				}
				goto IL_FA;
			}
		}

		internal void StartCacheSession()
		{
			for (int i = 0; i < this.cacheLength; i++)
			{
				this.cacheEntries[i].UsedInCurrentSession = false;
			}
		}

		public void ClearCache()
		{
			for (int i = 0; i < this.cacheLength; i++)
			{
				this.cacheEntries[i] = null;
			}
			this.firstEmptyIndex = 0;
			this.cacheLength = 0;
		}

		public void Commit()
		{
			using (UserConfiguration userConfiguration = this.GetUserConfiguration())
			{
				using (Stream xmlStream = userConfiguration.GetXmlStream())
				{
					xmlStream.SetLength(0L);
					using (StreamWriter streamWriter = Utilities.CreateStreamWriter(xmlStream))
					{
						this.RenderXml(streamWriter);
					}
				}
				try
				{
					userConfiguration.Save();
				}
				catch (ObjectNotFoundException ex)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "FolderMruCache.Commit: Failed. Exception: {0}", ex.Message);
				}
				catch (QuotaExceededException ex2)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "FolderMruCache.Commit: Failed. Exception: {0}", ex2.Message);
				}
				catch (SaveConflictException ex3)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "FolderMruCache.Commit: Failed. Exception: {0}", ex3.Message);
				}
			}
		}

		public void Load()
		{
			using (UserConfiguration userConfiguration = this.GetUserConfiguration())
			{
				using (Stream xmlStream = userConfiguration.GetXmlStream())
				{
					if (xmlStream != null && xmlStream.Length > 0L)
					{
						using (XmlTextReader xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(xmlStream))
						{
							this.Parse(xmlTextReader);
							goto IL_3B;
						}
					}
					this.ClearCache();
					IL_3B:;
				}
			}
		}

		private UserConfiguration GetUserConfiguration()
		{
			UserConfiguration userConfiguration = null;
			try
			{
				userConfiguration = this.userContext.MailboxSession.UserConfigurationManager.GetMailboxConfiguration("OWA.FolderMruCache", UserConfigurationTypes.XML);
			}
			catch (ObjectNotFoundException)
			{
				userConfiguration = this.userContext.MailboxSession.UserConfigurationManager.CreateMailboxConfiguration("OWA.FolderMruCache", UserConfigurationTypes.XML);
				try
				{
					userConfiguration.Save();
				}
				catch (QuotaExceededException ex)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "FolderMruCache.GetUserConfiguration: Failed. Exception: {0}", ex.Message);
				}
				catch (SaveConflictException ex2)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "FolderMruCache.GetUserConfiguration: Failed. Exception: {0}", ex2.Message);
				}
			}
			return userConfiguration;
		}

		public void RenderXml(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			using (XmlTextWriter xmlTextWriter = new XmlTextWriter(writer))
			{
				xmlTextWriter.WriteStartElement("FolderMruCache");
				for (int i = 0; i < this.cacheLength; i++)
				{
					this.cacheEntries[i].RenderEntryXml(xmlTextWriter, "entry");
				}
				xmlTextWriter.WriteFullEndElement();
			}
		}

		private void Parse(XmlTextReader reader)
		{
			try
			{
				reader.WhitespaceHandling = WhitespaceHandling.All;
				this.state = FolderMruCache.XmlParseState.Start;
				while (this.state != FolderMruCache.XmlParseState.Finished && reader.Read())
				{
					switch (this.state)
					{
					case FolderMruCache.XmlParseState.Start:
						this.ParseStart(reader);
						break;
					case FolderMruCache.XmlParseState.Root:
						this.ParseRoot(reader);
						break;
					case FolderMruCache.XmlParseState.Child:
						this.ParseChild(reader);
						break;
					}
				}
			}
			catch (XmlException ex)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Parser threw an XML exception: {0}'", ex.Message);
				this.ClearCache();
				this.Commit();
			}
			catch (OwaFolderMruParserException ex2)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Mru parser threw an exception: {0}'", ex2.Message);
				this.ClearCache();
				this.Commit();
			}
		}

		private void ParseStart(XmlTextReader reader)
		{
			if (XmlNodeType.Element != reader.NodeType || string.CompareOrdinal("FolderMruCache", reader.Name) != 0)
			{
				this.ThrowParserException(reader);
				return;
			}
			if (reader.IsEmptyElement)
			{
				this.state = FolderMruCache.XmlParseState.Finished;
				return;
			}
			this.state = FolderMruCache.XmlParseState.Root;
		}

		private void ParseRoot(XmlTextReader reader)
		{
			if (reader.NodeType == XmlNodeType.Element)
			{
				if (reader.IsEmptyElement)
				{
					this.ThrowParserException(reader);
					return;
				}
				if (string.CompareOrdinal("entry", reader.Name) != 0)
				{
					this.ThrowParserException(reader);
					return;
				}
				if (this.firstEmptyIndex != -1)
				{
					FolderMruCacheEntry folderMruCacheEntry = new FolderMruCacheEntry();
					folderMruCacheEntry.ParseEntry(reader, this.userContext);
					this.cacheEntries[this.firstEmptyIndex] = folderMruCacheEntry;
					this.state = FolderMruCache.XmlParseState.Child;
					if (this.firstEmptyIndex < 19 && this.firstEmptyIndex != -1)
					{
						this.firstEmptyIndex++;
						this.cacheLength = this.firstEmptyIndex;
						return;
					}
					this.firstEmptyIndex = -1;
					this.cacheLength = 20;
					return;
				}
			}
			else
			{
				if (reader.NodeType == XmlNodeType.EndElement && string.CompareOrdinal("FolderMruCache", reader.Name) == 0)
				{
					this.state = FolderMruCache.XmlParseState.Finished;
					return;
				}
				this.ThrowParserException(reader);
			}
		}

		private void ParseChild(XmlTextReader reader)
		{
			if (reader.NodeType == XmlNodeType.EndElement && string.CompareOrdinal(reader.Name, "entry") == 0)
			{
				this.state = FolderMruCache.XmlParseState.Root;
				return;
			}
			this.ThrowParserException(reader);
		}

		private void ThrowParserException(XmlTextReader reader)
		{
			string text = null;
			string message = string.Format(CultureInfo.InvariantCulture, "Mru Parser. Invalid request. Line number: {0} Position: {1}.{2}", new object[]
			{
				reader.LineNumber.ToString(CultureInfo.InvariantCulture),
				reader.LinePosition.ToString(CultureInfo.InvariantCulture),
				(text != null) ? (" " + text) : string.Empty
			});
			throw new OwaFolderMruParserException(message, null, this);
		}

		public const short CacheSize = 20;

		private const string ConfigurationName = "OWA.FolderMruCache";

		private const string MruParamName = "FolderMruCache";

		private const string MruEntryName = "entry";

		private UserContext userContext;

		private FolderMruCacheEntry[] cacheEntries;

		private int firstEmptyIndex;

		private int cacheLength;

		private FolderMruCache.XmlParseState state;

		private enum XmlParseState
		{
			Start,
			Root,
			Child,
			Finished
		}
	}
}
