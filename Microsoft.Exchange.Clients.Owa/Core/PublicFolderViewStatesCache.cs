using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class PublicFolderViewStatesCache
	{
		private PublicFolderViewStatesCache(UserContext userContext)
		{
			this.cacheEntries = new Dictionary<string, PublicFolderViewStatesEntry>();
			this.userContext = userContext;
			this.Load();
		}

		internal static PublicFolderViewStatesCache GetInstance(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (userContext.PublicFolderViewStatesCache == null && userContext.PublicFolderViewStatesCache == null)
			{
				userContext.PublicFolderViewStatesCache = new PublicFolderViewStatesCache(userContext);
			}
			return userContext.PublicFolderViewStatesCache;
		}

		internal PublicFolderViewStatesEntry this[string folderId]
		{
			get
			{
				if (this.CacheEntryExists(folderId))
				{
					return this.cacheEntries[folderId];
				}
				return null;
			}
		}

		private void RenderXml(TextWriter writer)
		{
			using (XmlTextWriter xmlTextWriter = new XmlTextWriter(writer))
			{
				xmlTextWriter.WriteStartElement("PublicFolderViewStates");
				PublicFolderViewStatesEntry[] array = new PublicFolderViewStatesEntry[this.cacheEntries.Count];
				this.cacheEntries.Values.CopyTo(array, 0);
				Array.Sort<PublicFolderViewStatesEntry>(array, new PublicFolderViewStatesEntry.LastAccessDateTimeTicksComparer());
				int num = 0;
				while (num < array.Length && num < 50)
				{
					array[num].RenderEntryXml(xmlTextWriter, "Entry");
					num++;
				}
				xmlTextWriter.WriteFullEndElement();
			}
		}

		internal void ClearCache()
		{
			this.cacheEntries.Clear();
		}

		internal void AddEntry(string folderId, PublicFolderViewStatesEntry newEntry)
		{
			while (this.cacheEntries.Count >= 50 && this.RemoveTheOldestEntry())
			{
			}
			this.cacheEntries.Add(folderId, newEntry);
		}

		private bool RemoveTheOldestEntry()
		{
			PublicFolderViewStatesEntry publicFolderViewStatesEntry = null;
			foreach (PublicFolderViewStatesEntry publicFolderViewStatesEntry2 in this.cacheEntries.Values)
			{
				if (publicFolderViewStatesEntry == null || publicFolderViewStatesEntry.LastAccessedDateTimeTicks > publicFolderViewStatesEntry2.LastAccessedDateTimeTicks)
				{
					publicFolderViewStatesEntry = publicFolderViewStatesEntry2;
				}
			}
			if (publicFolderViewStatesEntry != null)
			{
				this.cacheEntries.Remove(publicFolderViewStatesEntry.FolderId);
				return true;
			}
			return false;
		}

		private UserConfiguration GetUserConfiguration()
		{
			UserConfiguration userConfiguration = null;
			try
			{
				userConfiguration = this.userContext.MailboxSession.UserConfigurationManager.GetMailboxConfiguration("OWA.PublicFolderViewStates", UserConfigurationTypes.XML);
			}
			catch (ObjectNotFoundException)
			{
				userConfiguration = this.userContext.MailboxSession.UserConfigurationManager.CreateMailboxConfiguration("OWA.PublicFolderViewStates", UserConfigurationTypes.XML);
				try
				{
					userConfiguration.Save();
				}
				catch (QuotaExceededException ex)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "PublicFolderViewStatesCache.GetUserConfiguration: Failed. Exception: {0}", ex.Message);
				}
				catch (SaveConflictException ex2)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "PublicFolderViewStatesCache.GetUserConfiguration: Failed. Exception: {0}", ex2.Message);
				}
			}
			return userConfiguration;
		}

		internal void Commit()
		{
			this.LoadAndMerge();
			this.InternalCommit();
		}

		private void InternalCommit()
		{
			using (UserConfiguration userConfiguration = this.GetUserConfiguration())
			{
				using (Stream xmlStream = userConfiguration.GetXmlStream())
				{
					xmlStream.SetLength(0L);
					using (StreamWriter streamWriter = new StreamWriter(xmlStream))
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
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "PublicFolderViewStatesCache.Commit: Failed. Exception: {0}", ex.Message);
				}
				catch (QuotaExceededException ex2)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "PublicFolderViewStatesCache.Commit: Failed. Exception: {0}", ex2.Message);
				}
				catch (SaveConflictException ex3)
				{
					ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "PublicFolderViewStatesCache.Commit: Failed. Exception: {0}", ex3.Message);
				}
			}
		}

		internal void Load()
		{
			this.ClearCache();
			this.LoadAndMerge();
		}

		private void LoadAndMerge()
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

		internal bool CacheEntryExists(string folderId)
		{
			return this.cacheEntries.ContainsKey(folderId);
		}

		private void Parse(XmlTextReader reader)
		{
			try
			{
				reader.WhitespaceHandling = WhitespaceHandling.All;
				this.state = PublicFolderViewStatesCache.XmlParseState.Start;
				while (this.state != PublicFolderViewStatesCache.XmlParseState.Finished && reader.Read())
				{
					switch (this.state)
					{
					case PublicFolderViewStatesCache.XmlParseState.Start:
						this.ParseStart(reader);
						break;
					case PublicFolderViewStatesCache.XmlParseState.Root:
						this.ParseRoot(reader);
						break;
					case PublicFolderViewStatesCache.XmlParseState.Child:
						this.ParseChild(reader);
						break;
					}
				}
			}
			catch (XmlException)
			{
				this.ClearCache();
				this.InternalCommit();
			}
			catch (OwaPublicFolderViewStatesParseException)
			{
				this.ClearCache();
				this.InternalCommit();
			}
		}

		private void ParseStart(XmlTextReader reader)
		{
			if (XmlNodeType.Element != reader.NodeType || !string.Equals("PublicFolderViewStates", reader.Name, StringComparison.OrdinalIgnoreCase))
			{
				this.ThrowParserException(reader);
				return;
			}
			if (reader.IsEmptyElement)
			{
				this.state = PublicFolderViewStatesCache.XmlParseState.Finished;
				return;
			}
			this.state = PublicFolderViewStatesCache.XmlParseState.Root;
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
				if (!string.Equals("Entry", reader.Name, StringComparison.OrdinalIgnoreCase))
				{
					this.ThrowParserException(reader);
					return;
				}
				if (this.cacheEntries.Count < 100)
				{
					PublicFolderViewStatesEntry publicFolderViewStatesEntry = new PublicFolderViewStatesEntry();
					publicFolderViewStatesEntry.ParseEntry(reader);
					PublicFolderViewStatesEntry publicFolderViewStatesEntry2 = this[publicFolderViewStatesEntry.FolderId];
					if (publicFolderViewStatesEntry2 != null)
					{
						if (!publicFolderViewStatesEntry2.Dirty)
						{
							long lastAccessedDateTimeTicks = publicFolderViewStatesEntry2.LastAccessedDateTimeTicks;
							if (publicFolderViewStatesEntry.LastAccessedDateTimeTicks < lastAccessedDateTimeTicks)
							{
								publicFolderViewStatesEntry.LastAccessedDateTimeTicks = lastAccessedDateTimeTicks;
							}
							this.cacheEntries[publicFolderViewStatesEntry.FolderId] = publicFolderViewStatesEntry;
						}
					}
					else
					{
						this.cacheEntries.Add(publicFolderViewStatesEntry.FolderId, publicFolderViewStatesEntry);
					}
					this.state = PublicFolderViewStatesCache.XmlParseState.Child;
					return;
				}
			}
			else
			{
				if (reader.NodeType == XmlNodeType.EndElement && string.Equals("PublicFolderViewStates", reader.Name, StringComparison.OrdinalIgnoreCase))
				{
					this.state = PublicFolderViewStatesCache.XmlParseState.Finished;
					return;
				}
				this.ThrowParserException(reader);
			}
		}

		private void ParseChild(XmlTextReader reader)
		{
			if (reader.NodeType == XmlNodeType.EndElement && string.Equals(reader.Name, "Entry", StringComparison.OrdinalIgnoreCase))
			{
				this.state = PublicFolderViewStatesCache.XmlParseState.Root;
				return;
			}
			this.ThrowParserException(reader);
		}

		private void ThrowParserException(XmlTextReader reader)
		{
			throw new OwaPublicFolderViewStatesParseException(string.Format(CultureInfo.InvariantCulture, "Invalid request. Line number: {0} Position: {1}.{2}", new object[]
			{
				reader.LineNumber.ToString(CultureInfo.InvariantCulture),
				reader.LinePosition.ToString(CultureInfo.InvariantCulture),
				"Parsing PublicFolderViewStates error."
			}), null, this);
		}

		private const string PublicFolderViewStatesParamName = "PublicFolderViewStates";

		private const string PublicFolderViewStatesEntryParamName = "Entry";

		private const int MaxSavedEntryNumber = 50;

		private const int MaxMergedEntryNumber = 100;

		private const string ConfigurationName = "OWA.PublicFolderViewStates";

		private UserContext userContext;

		private Dictionary<string, PublicFolderViewStatesEntry> cacheEntries;

		private PublicFolderViewStatesCache.XmlParseState state;

		private enum XmlParseState
		{
			Start,
			Root,
			Child,
			Finished
		}
	}
}
