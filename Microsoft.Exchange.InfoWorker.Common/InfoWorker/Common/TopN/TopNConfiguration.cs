using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.InfoWorker.Common.TopN
{
	internal class TopNConfiguration
	{
		public override string ToString()
		{
			if (this.toString == null)
			{
				this.toString = "TopNConfiguration for user " + this.mailboxSession.MailboxOwner + ". ";
			}
			return this.toString;
		}

		internal TopNConfiguration(MailboxSession mailboxSession)
		{
			this.mailboxSession = mailboxSession;
			this.lastScanTime = ExDateTime.MinValue;
		}

		internal MailboxSession MailboxSession
		{
			get
			{
				return this.mailboxSession;
			}
		}

		internal int Version
		{
			get
			{
				return this.version;
			}
			set
			{
				this.version = value;
			}
		}

		internal ExDateTime LastScanTime
		{
			get
			{
				return this.lastScanTime;
			}
			set
			{
				this.lastScanTime = value;
			}
		}

		internal bool ScanRequested
		{
			get
			{
				return this.scanRequested;
			}
			set
			{
				this.scanRequested = value;
			}
		}

		internal KeyValuePair<string, int>[] WordFrequency
		{
			get
			{
				return this.wordFrequency;
			}
			set
			{
				this.wordFrequency = value;
			}
		}

		internal bool ReadWordFrequencyMap()
		{
			bool result;
			using (UserConfiguration userConfiguration = this.OpenMessage(true))
			{
				if (userConfiguration == null)
				{
					TopNConfiguration.Tracer.TraceError<TopNConfiguration>((long)this.GetHashCode(), "{0}: FAI could not be opened or created.", this);
					result = false;
				}
				else
				{
					using (Stream stream = userConfiguration.GetStream())
					{
						Exception ex = null;
						try
						{
							Type[] allowList = new Type[]
							{
								typeof(KeyValuePair<string, int>)
							};
							this.wordFrequency = (KeyValuePair<string, int>[])SafeSerialization.SafeBinaryFormatterDeserializeWithAllowList(stream, allowList, null);
						}
						catch (SafeSerialization.BlockedTypeException ex2)
						{
							ex = ex2;
						}
						catch (ArgumentNullException ex3)
						{
							ex = ex3;
						}
						catch (SerializationException ex4)
						{
							ex = ex4;
						}
						catch (Exception ex5)
						{
							ex = ex5;
						}
						if (ex != null)
						{
							TopNConfiguration.Tracer.TraceError<TopNConfiguration, Exception>((long)this.GetHashCode(), "{0}: FAI message is corrupt. Exception: {1}", this, ex);
							result = false;
						}
						else
						{
							result = true;
						}
					}
				}
			}
			return result;
		}

		internal bool ReadMetaData()
		{
			bool result;
			using (UserConfiguration userConfiguration = this.OpenMessage(true))
			{
				if (userConfiguration == null)
				{
					TopNConfiguration.Tracer.TraceError<TopNConfiguration>((long)this.GetHashCode(), "{0}: FAI could not be opened or created.", this);
					result = false;
				}
				else
				{
					IDictionary dictionary = userConfiguration.GetDictionary();
					if (dictionary == null)
					{
						TopNConfiguration.Tracer.TraceError<TopNConfiguration>((long)this.GetHashCode(), "{0}: No meta data in FAI item. Item may be corrupt.", this);
						result = false;
					}
					else
					{
						Exception ex = null;
						try
						{
							if (dictionary["Version"] is string)
							{
								this.version = (int)dictionary["Version"];
							}
							if (dictionary["LastScanTime"] is ExDateTime)
							{
								this.lastScanTime = (ExDateTime)dictionary["LastScanTime"];
							}
							if (!this.IsMailboxExtendedPropertySupported())
							{
								if (dictionary["ScanRequested"] is bool)
								{
									this.scanRequested = (bool)dictionary["ScanRequested"];
								}
							}
							else
							{
								object mailboxExtendedProperty = this.GetMailboxExtendedProperty();
								if (mailboxExtendedProperty is PropertyError)
								{
									TopNConfiguration.Tracer.TraceError<TopNConfiguration, object>((long)this.GetHashCode(), "{0}: Extended Mailbox property returned property error {1}", this, mailboxExtendedProperty);
									return false;
								}
								this.ScanRequested = (bool)mailboxExtendedProperty;
							}
						}
						catch (CorruptDataException ex2)
						{
							ex = ex2;
						}
						if (ex != null)
						{
							TopNConfiguration.Tracer.TraceError<TopNConfiguration, Exception>((long)this.GetHashCode(), "{0}: FAI message is corrupt. Exception: {1}", this, ex);
							result = false;
						}
						else
						{
							result = true;
						}
					}
				}
			}
			return result;
		}

		internal bool Save(bool onlyMetaData)
		{
			bool result;
			using (UserConfiguration userConfiguration = this.OpenMessage(true))
			{
				if (userConfiguration == null)
				{
					TopNConfiguration.Tracer.TraceError<TopNConfiguration>((long)this.GetHashCode(), "{0}: Save() failed because FAI could not be opened or created.", this);
					result = false;
				}
				else
				{
					IDictionary dictionary = userConfiguration.GetDictionary();
					dictionary["Version"] = this.version;
					dictionary["LastScanTime"] = this.lastScanTime;
					if (!this.IsMailboxExtendedPropertySupported())
					{
						dictionary["ScanRequested"] = this.scanRequested;
					}
					if (onlyMetaData)
					{
						result = this.SaveMessage(userConfiguration);
					}
					else
					{
						using (Stream stream = userConfiguration.GetStream())
						{
							IFormatter formatter = ExchangeBinaryFormatterFactory.CreateBinaryFormatter(null);
							Exception ex = null;
							try
							{
								formatter.Serialize(stream, this.wordFrequency);
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
								TopNConfiguration.Tracer.TraceError<TopNConfiguration, Exception>((long)this.GetHashCode(), "{0}: Failed to serialize word frequency data. Exception: {1}", this, ex);
								return false;
							}
						}
						result = this.SaveMessage(userConfiguration);
					}
				}
			}
			return result;
		}

		internal void Delete()
		{
			StoreId defaultFolderId = this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
			this.mailboxSession.UserConfigurationManager.DeleteFolderConfigurations(defaultFolderId, new string[]
			{
				"TopNWords.Data"
			});
		}

		private UserConfiguration OpenMessage(bool createIfMissingOrCorrupt)
		{
			UserConfiguration userConfiguration = null;
			StoreId defaultFolderId = this.mailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
			Exception ex = null;
			try
			{
				userConfiguration = this.mailboxSession.UserConfigurationManager.GetFolderConfiguration("TopNWords.Data", UserConfigurationTypes.Stream | UserConfigurationTypes.Dictionary, defaultFolderId);
			}
			catch (ObjectNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (CorruptDataException ex3)
			{
				ex = ex3;
			}
			if (userConfiguration == null)
			{
				TopNConfiguration.Tracer.TraceDebug<string, Exception>(0L, "FAI message '{0}' is missing or corrupt. Exception: {1}", "TopNWords.Data", ex);
				if (createIfMissingOrCorrupt)
				{
					if (ex is CorruptDataException)
					{
						this.mailboxSession.UserConfigurationManager.DeleteFolderConfigurations(defaultFolderId, new string[]
						{
							"TopNWords.Data"
						});
					}
					userConfiguration = this.mailboxSession.UserConfigurationManager.CreateFolderConfiguration("TopNWords.Data", UserConfigurationTypes.Stream | UserConfigurationTypes.Dictionary, defaultFolderId);
				}
			}
			return userConfiguration;
		}

		private bool SaveMessage(UserConfiguration config)
		{
			Exception ex = null;
			try
			{
				config.Save();
				if (this.IsMailboxExtendedPropertySupported())
				{
					this.SetMailboxExtendedProperty();
				}
			}
			catch (ObjectExistedException ex2)
			{
				ex = ex2;
			}
			catch (SaveConflictException ex3)
			{
				ex = ex3;
			}
			catch (QuotaExceededException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				TopNConfiguration.Tracer.TraceError<TopNConfiguration, Exception>((long)this.GetHashCode(), "{0}: could not be saved. Exception: {1}", this, ex);
				return false;
			}
			return true;
		}

		private void SetMailboxExtendedProperty()
		{
			object mailboxExtendedProperty = this.GetMailboxExtendedProperty();
			if (mailboxExtendedProperty is PropertyError || (bool)mailboxExtendedProperty != this.ScanRequested)
			{
				this.MailboxSession.Mailbox[MailboxSchema.IsTopNEnabled] = this.ScanRequested;
				this.MailboxSession.Mailbox.Save();
				this.MailboxSession.Mailbox.Load();
			}
		}

		private object GetMailboxExtendedProperty()
		{
			this.MailboxSession.Mailbox.Load(new PropertyDefinition[]
			{
				MailboxSchema.IsTopNEnabled
			});
			return this.MailboxSession.Mailbox.TryGetProperty(MailboxSchema.IsTopNEnabled);
		}

		private bool IsMailboxExtendedPropertySupported()
		{
			ServerVersion serverVersion = new ServerVersion(this.mailboxSession.MailboxOwner.MailboxInfo.Location.ServerVersion);
			return serverVersion.Major > 14 || (serverVersion.Major == 14 && serverVersion.Minor > 1) || (serverVersion.Major == 14 && serverVersion.Minor == 1 && serverVersion.Build > 116);
		}

		internal const string MessageClass = "TopNWords.Data";

		internal const UserConfigurationTypes Types = UserConfigurationTypes.Stream | UserConfigurationTypes.Dictionary;

		internal const string VersionKey = "Version";

		internal const string LastScanTimeKey = "LastScanTime";

		internal const string ScanRequestedKey = "ScanRequested";

		private const int MailboxMajorVersion = 14;

		private const int MailboxMinorVersion = 1;

		private const int MailboxBuildNumber = 116;

		internal static readonly TimeSpan UpdateInterval = TimeSpan.FromDays(30.0);

		private MailboxSession mailboxSession;

		private int version;

		private ExDateTime lastScanTime;

		private bool scanRequested;

		private KeyValuePair<string, int>[] wordFrequency;

		private string toString;

		protected static readonly Trace Tracer = ExTraceGlobals.TopNTracer;
	}
}
