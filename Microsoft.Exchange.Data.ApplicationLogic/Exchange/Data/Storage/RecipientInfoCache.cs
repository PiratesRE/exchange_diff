using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RecipientInfoCache : IDisposeTrackable, IDisposable
	{
		private RecipientInfoCache(MailboxSession mailboxSession, string configurationCacheName)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession is null!");
			}
			if (string.IsNullOrEmpty(configurationCacheName))
			{
				throw new ArgumentException("configurationCacheName is null or empty!");
			}
			this.configurationCacheName = configurationCacheName;
			this.GetOrOpenConfiguration(mailboxSession);
			this.disposeTracker = this.GetDisposeTracker();
		}

		private RecipientInfoCache(UserConfiguration userConfiguration)
		{
			if (userConfiguration == null)
			{
				throw new ArgumentNullException("userConfiguration is null!");
			}
			this.backendUserConfiguration = userConfiguration;
			this.disposeTracker = this.GetDisposeTracker();
		}

		public StoreObjectId ItemId
		{
			get
			{
				this.CheckDisposed("get_ItemId");
				return this.backendUserConfiguration.Id;
			}
		}

		public ExDateTime LastModifiedTime
		{
			get
			{
				this.CheckDisposed("get_LastModifiedTime");
				return this.lastModifiedTime;
			}
		}

		public static RecipientInfoCache Create(MailboxSession mailboxSession, string configurationCacheName)
		{
			return new RecipientInfoCache(mailboxSession, configurationCacheName);
		}

		public static RecipientInfoCache Create(UserConfiguration userConfiguration)
		{
			return new RecipientInfoCache(userConfiguration);
		}

		public List<RecipientInfoCacheEntry> Load(string parentXmlNodeName)
		{
			this.CheckDisposed("Load");
			if (string.IsNullOrEmpty(parentXmlNodeName))
			{
				throw new ArgumentException("parentXmlNodeName is null or Empty");
			}
			List<RecipientInfoCacheEntry> result;
			using (Stream xmlStream = this.backendUserConfiguration.GetXmlStream())
			{
				this.lastModifiedTime = this.backendUserConfiguration.LastModifiedTime;
				result = RecipientInfoCache.Deserialize(xmlStream, parentXmlNodeName, out this.backendCacheVersion);
			}
			return result;
		}

		public void Save(List<RecipientInfoCacheEntry> entries, string parentNodeName, int cacheSize)
		{
			this.CheckDisposed("Save");
			if (entries == null)
			{
				throw new ArgumentNullException("entries");
			}
			if (string.IsNullOrEmpty(parentNodeName))
			{
				throw new ArgumentException("parentNodeName is null or Empty");
			}
			if (cacheSize <= 0)
			{
				throw new ArgumentException("cacheSize must be greater than zero");
			}
			using (Stream xmlStream = this.backendUserConfiguration.GetXmlStream())
			{
				RecipientInfoCache.Serialize(xmlStream, entries, parentNodeName, cacheSize);
			}
			try
			{
				this.backendUserConfiguration.Save();
			}
			catch (ObjectNotFoundException ex)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "RecipientInfoCache.Commit: Failed. Exception: {0}", ex.Message);
			}
			catch (QuotaExceededException ex2)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "RecipientInfoCache.Commit: Failed. Exception: {0}", ex2.Message);
			}
			catch (SaveConflictException ex3)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "RecipientInfoCache.Commit: Failed. Exception: {0}", ex3.Message);
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public UserConfiguration DetachUserConfiguration()
		{
			this.CheckDisposed("DetachUserConfiguration");
			UserConfiguration result = this.backendUserConfiguration;
			this.backendUserConfiguration = null;
			return result;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RecipientInfoCache>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private static List<RecipientInfoCacheEntry> Deserialize(Stream stream, string parentNodeName, out int backendCacheVersion)
		{
			backendCacheVersion = 0;
			List<RecipientInfoCacheEntry> result;
			using (XmlReader xmlReader = XmlReader.Create(stream, new XmlReaderSettings
			{
				CloseInput = false,
				CheckCharacters = false,
				IgnoreWhitespace = false
			}))
			{
				List<RecipientInfoCacheEntry> list = new List<RecipientInfoCacheEntry>(100);
				try
				{
					if (xmlReader.Read())
					{
						if (!string.Equals(xmlReader.Name, parentNodeName, StringComparison.OrdinalIgnoreCase))
						{
							throw new CorruptDataException(ServerStrings.InvalidTagName(parentNodeName, xmlReader.Name));
						}
						if (xmlReader.HasAttributes)
						{
							string s = xmlReader["version"];
							if (!int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out backendCacheVersion))
							{
								throw new CorruptDataException(ServerStrings.VersionNotInteger);
							}
							if (backendCacheVersion < 3)
							{
								return new List<RecipientInfoCacheEntry>(0);
							}
						}
						while (xmlReader.Read() && (xmlReader.NodeType != XmlNodeType.EndElement || !string.Equals(parentNodeName, xmlReader.Name, StringComparison.OrdinalIgnoreCase)))
						{
							list.Add(RecipientInfoCacheEntry.ParseEntry(xmlReader));
						}
						if (xmlReader.Read())
						{
							throw new CorruptDataException(ServerStrings.UnexpectedTag(xmlReader.Name));
						}
					}
				}
				catch (XmlException ex)
				{
					ExTraceGlobals.StorageTracer.TraceDebug<string>(0L, "RecipientInfoCache.Deserialize: Failed. Exception: {0}", ex.Message);
					throw new CorruptDataException(ServerStrings.InvalidXml, ex);
				}
				result = list;
			}
			return result;
		}

		private static void Serialize(Stream xmlStream, List<RecipientInfoCacheEntry> entries, string parentNodeName, int cacheSize)
		{
			if (entries.Count > cacheSize)
			{
				entries.Sort();
				entries = entries.GetRange(entries.Count - cacheSize, cacheSize);
			}
			xmlStream.SetLength(0L);
			xmlStream.Position = 0L;
			using (XmlWriter xmlWriter = XmlWriter.Create(xmlStream, new XmlWriterSettings
			{
				CloseOutput = false,
				OmitXmlDeclaration = true,
				CheckCharacters = false
			}))
			{
				xmlWriter.WriteStartElement(parentNodeName);
				xmlWriter.WriteAttributeString("version", 3.ToString(CultureInfo.InvariantCulture));
				foreach (RecipientInfoCacheEntry recipientInfoCacheEntry in entries)
				{
					recipientInfoCacheEntry.Serialize(xmlWriter);
				}
				xmlWriter.WriteFullEndElement();
			}
			xmlStream.Flush();
		}

		private void GetOrOpenConfiguration(MailboxSession mailboxSession)
		{
			UserConfigurationManager userConfigurationManager = mailboxSession.UserConfigurationManager;
			if (userConfigurationManager == null)
			{
				userConfigurationManager = new UserConfigurationManager(mailboxSession);
			}
			try
			{
				this.backendUserConfiguration = userConfigurationManager.GetMailboxConfiguration(this.configurationCacheName, UserConfigurationTypes.XML);
			}
			catch (ObjectNotFoundException)
			{
				this.backendUserConfiguration = userConfigurationManager.CreateMailboxConfiguration(this.configurationCacheName, UserConfigurationTypes.XML);
			}
		}

		private void CheckDisposed(string methodName)
		{
			if (this.disposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.disposed, disposing);
			if (!this.disposed)
			{
				this.disposed = true;
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
				if (this.backendUserConfiguration != null)
				{
					this.backendUserConfiguration.Dispose();
					this.backendUserConfiguration = null;
				}
			}
		}

		public const string AutoCompleteCacheConfigurationName = "OWA.AutocompleteCache";

		public const short AutoCompleteCacheCacheSize = 100;

		public const string AutoCompleteNodeName = "AutoCompleteCache";

		private const string AutoCompleteCacheVersionName = "version";

		private const int CacheVersion = 3;

		private int backendCacheVersion;

		private UserConfiguration backendUserConfiguration;

		private string configurationCacheName;

		private bool disposed;

		private DisposeTracker disposeTracker;

		private ExDateTime lastModifiedTime = ExDateTime.MinValue;
	}
}
