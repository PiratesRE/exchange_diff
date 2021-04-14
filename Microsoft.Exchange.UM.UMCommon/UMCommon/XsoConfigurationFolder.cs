using System;
using System.Collections;
using System.IO;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal sealed class XsoConfigurationFolder : IConfigurationFolder, IPromptCounter
	{
		internal XsoConfigurationFolder(UMMailboxRecipient mailbox)
		{
			this.mailbox = mailbox;
		}

		public bool IsOof
		{
			get
			{
				return this.GetConfigValue("OofStatus", false);
			}
			set
			{
				this.Dictionary["OofStatus"] = value;
			}
		}

		public MailboxGreetingEnum CurrentMailboxGreetingType
		{
			get
			{
				if (!this.IsOof)
				{
					return MailboxGreetingEnum.Voicemail;
				}
				return MailboxGreetingEnum.Away;
			}
			set
			{
				try
				{
					this.IsOof = (value == MailboxGreetingEnum.Away);
					this.Save();
				}
				catch (StorageTransientException innerException)
				{
					throw new UserConfigurationException(Strings.TransientlyUnableToAccessUserConfiguration(this.mailbox.MailAddress), innerException);
				}
				catch (StoragePermanentException innerException2)
				{
					throw new UserConfigurationException(Strings.PermanentlyUnableToAccessUserConfiguration(this.mailbox.MailAddress), innerException2);
				}
			}
		}

		public string PlayOnPhoneDialString
		{
			get
			{
				object obj = this.Dictionary["PlayOnPhoneDialString"];
				if (obj == null)
				{
					obj = this.mailbox.ADRecipient.UMExtension;
				}
				else if (!(obj is string))
				{
					this.DeleteCorruptedConfiguration();
					obj = this.mailbox.ADRecipient.UMExtension;
				}
				else if (string.IsNullOrEmpty((string)obj))
				{
					obj = this.mailbox.ADRecipient.UMExtension;
				}
				return (string)obj;
			}
			set
			{
				this.Dictionary["PlayOnPhoneDialString"] = value;
			}
		}

		public string TelephoneAccessFolderEmail
		{
			get
			{
				string result;
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.mailbox.CreateSessionLock())
				{
					object obj = this.Dictionary["TelephoneAccessFolderEmail"];
					bool flag = false;
					if (obj == null)
					{
						flag = true;
					}
					else if (!(obj is string))
					{
						this.DeleteCorruptedConfiguration();
						flag = true;
					}
					else if (string.IsNullOrEmpty((string)obj))
					{
						flag = true;
					}
					else
					{
						byte[] entryId = Convert.FromBase64String((string)obj);
						StoreObjectId storeObjectId = StoreObjectId.FromProviderSpecificId(entryId);
						try
						{
							using (Folder.Bind(mailboxSessionLock.Session, storeObjectId))
							{
							}
						}
						catch (ObjectNotFoundException ex)
						{
							CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, this, "TelephoneAccessFolderEmail is invalid (id = '{0}'). This could be because it was deleted by the user. Exception: {1}", new object[]
							{
								storeObjectId,
								ex
							});
							flag = true;
						}
					}
					if (flag)
					{
						StoreObjectId defaultFolderId = mailboxSessionLock.Session.GetDefaultFolderId(DefaultFolderType.Inbox);
						obj = Convert.ToBase64String(defaultFolderId.ProviderLevelItemId);
					}
					result = (string)obj;
				}
				return result;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.mailbox.CreateSessionLock())
					{
						byte[] entryId = Convert.FromBase64String(value);
						StoreObjectId folderId = StoreObjectId.FromProviderSpecificId(entryId);
						using (Folder.Bind(mailboxSessionLock.Session, folderId, null))
						{
							this.Dictionary["TelephoneAccessFolderEmail"] = value;
						}
						return;
					}
				}
				this.Dictionary["TelephoneAccessFolderEmail"] = null;
			}
		}

		public VoiceNotificationStatus VoiceNotificationStatus
		{
			get
			{
				object obj = this.Dictionary["VoiceNotificationStatus"];
				if (obj == null)
				{
					obj = VoiceNotificationStatus.EnabledByDefault;
				}
				return (VoiceNotificationStatus)obj;
			}
			set
			{
				this.Dictionary["VoiceNotificationStatus"] = (int)value;
			}
		}

		public bool UseAsr
		{
			get
			{
				return this.GetConfigValue("UseAsr", true);
			}
			set
			{
				this.Dictionary["UseAsr"] = value;
			}
		}

		public bool ReceivedVoiceMailPreviewEnabled
		{
			get
			{
				return this.GetConfigValue("ReceivedVoiceMailPreviewEnabled", true);
			}
			set
			{
				this.Dictionary["ReceivedVoiceMailPreviewEnabled"] = value;
			}
		}

		public bool SentVoiceMailPreviewEnabled
		{
			get
			{
				return this.GetConfigValue("SentVoiceMailPreviewEnabled", true);
			}
			set
			{
				this.Dictionary["SentVoiceMailPreviewEnabled"] = value;
			}
		}

		public bool ReadUnreadVoicemailInFIFOOrder
		{
			get
			{
				return this.GetConfigValue("ReadUnreadVoicemailInFIFOOrder", false);
			}
			set
			{
				this.Dictionary["ReadUnreadVoicemailInFIFOOrder"] = value;
			}
		}

		public MultiValuedProperty<string> BlockedNumbers
		{
			get
			{
				object obj = this.Dictionary["BlockedNumbers"];
				string[] value;
				if (obj == null)
				{
					value = new string[0];
				}
				else if (!(obj is string[]))
				{
					value = new string[0];
					this.DeleteCorruptedConfiguration();
				}
				else
				{
					value = (string[])obj;
				}
				return new MultiValuedProperty<string>(value);
			}
			set
			{
				this.Dictionary["BlockedNumbers"] = ((value != null) ? value.ToArray() : null);
			}
		}

		public bool IsFirstTimeUser
		{
			get
			{
				return this.GetConfigValue("FirstTimeUser", true);
			}
			set
			{
				this.Dictionary["FirstTimeUser"] = value;
			}
		}

		private IDictionary Dictionary
		{
			get
			{
				if (this.dictionary == null)
				{
					try
					{
						this.dictionary = this.CopyFromUserConfig();
					}
					catch (CorruptDataException)
					{
						this.DeleteCorruptedConfiguration();
						this.dictionary = this.CopyFromUserConfig();
					}
					catch (InvalidOperationException)
					{
						this.DeleteCorruptedConfiguration();
						this.dictionary = this.CopyFromUserConfig();
					}
				}
				return this.dictionary;
			}
		}

		public GreetingBase OpenNameGreeting()
		{
			if (this.adRecipient == null)
			{
				IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromADRecipient(this.mailbox.ADRecipient, false);
				this.adRecipient = iadrecipientLookup.LookupByObjectId(this.mailbox.ADRecipient.Id);
			}
			object[] args = new object[]
			{
				this.adRecipient,
				"RecordedName"
			};
			return (GreetingBase)Activator.CreateInstance(XsoConfigurationFolder.CoreTypeLoader.AdGreetingType, BindingFlags.Instance | BindingFlags.NonPublic, null, args, null);
		}

		public GreetingBase OpenCustomMailboxGreeting(MailboxGreetingEnum gt)
		{
			object[] args = new object[]
			{
				this.mailbox,
				this.GetCustomMailboxGreetingConfigName(gt)
			};
			return (GreetingBase)Activator.CreateInstance(XsoConfigurationFolder.CoreTypeLoader.XsoGreetingType, BindingFlags.Instance | BindingFlags.NonPublic, null, args, null);
		}

		public IPassword OpenPassword()
		{
			return new XsoPasswordImpl(this.mailbox);
		}

		public bool HasCustomMailboxGreeting(MailboxGreetingEnum g)
		{
			bool result;
			try
			{
				result = this.HasNonEmptyStreamConfig("Um.CustomGreetings." + this.GetCustomMailboxGreetingConfigName(g));
			}
			catch (StorageTransientException innerException)
			{
				throw new UserConfigurationException(Strings.TransientlyUnableToAccessUserConfiguration(this.mailbox.MailAddress), innerException);
			}
			catch (StoragePermanentException innerException2)
			{
				throw new UserConfigurationException(Strings.PermanentlyUnableToAccessUserConfiguration(this.mailbox.MailAddress), innerException2);
			}
			return result;
		}

		public void RemoveCustomMailboxGreeting(MailboxGreetingEnum g)
		{
			if (!this.DeleteConfiguration("Um.CustomGreetings." + this.GetCustomMailboxGreetingConfigName(g)))
			{
				throw new UserConfigurationException(Strings.UnableToRemoveCustomGreeting(this.mailbox.MailAddress));
			}
		}

		public void Save()
		{
			this.CopyToUserConfig(this.Dictionary);
			this.promptsDirty = false;
		}

		public void SetPromptCount(string promptId, int newCount)
		{
			try
			{
				this.Dictionary["PromptCount_" + promptId] = newCount;
				this.promptsDirty = true;
			}
			catch (LocalizedException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, this, "SetPromptCount ignoring LocalizedException le={0}.", new object[]
				{
					ex.Message
				});
			}
		}

		public int GetPromptCount(string promptId)
		{
			object obj = this.Dictionary["PromptCount_" + promptId];
			if (obj == null)
			{
				return 0;
			}
			if (!(obj is int))
			{
				this.DeleteCorruptedConfiguration();
				return 0;
			}
			return (int)obj;
		}

		public void SavePromptCount()
		{
			try
			{
				if (this.promptsDirty)
				{
					this.Save();
				}
			}
			catch (StorageTransientException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, this, "SavePromptCount Ignoring storage transient exception: {0}.", new object[]
				{
					ex.Message
				});
			}
			catch (QuotaExceededException ex2)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, this, "SavePromptCount Ignoring QuotaExceededException: {0}.", new object[]
				{
					ex2.Message
				});
			}
		}

		private string GetCustomMailboxGreetingConfigName(MailboxGreetingEnum gt)
		{
			switch (gt)
			{
			case MailboxGreetingEnum.Voicemail:
				return "External";
			case MailboxGreetingEnum.Away:
				return "Oof";
			default:
				ExAssert.RetailAssert(false, "Invalid Greeting type {0}", new object[]
				{
					gt.GetType().Name
				});
				return string.Empty;
			}
		}

		private bool DeleteConfiguration(string configName)
		{
			bool result;
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.mailbox.CreateSessionLock())
			{
				OperationResult operationResult = mailboxSessionLock.Session.UserConfigurationManager.DeleteMailboxConfigurations(new string[]
				{
					configName
				});
				result = (operationResult == OperationResult.Succeeded);
			}
			return result;
		}

		private void DeleteCorruptedConfiguration()
		{
			PIIMessage data = PIIMessage.Create(PIIType._SmtpAddress, this.mailbox.MailAddress);
			CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, this, data, "Found a corrupted configuration file for user=_SmtpAddress! Deleting!", new object[0]);
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CorruptedConfiguration, null, new object[]
			{
				this.mailbox
			});
			this.dictionary = null;
			if (!this.DeleteConfiguration("Um.General"))
			{
				throw new UserConfigurationException(Strings.CorruptedConfigurationCouldNotBeDeleted(this.mailbox.MailAddress));
			}
		}

		private UserConfiguration RebuildConfiguration(MailboxSession s)
		{
			this.DeleteCorruptedConfiguration();
			return s.UserConfigurationManager.CreateMailboxConfiguration("Um.General", UserConfigurationTypes.Dictionary);
		}

		private bool HasNonEmptyStreamConfig(string configName)
		{
			bool result;
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.mailbox.CreateSessionLock())
			{
				try
				{
					using (UserConfiguration mailboxConfiguration = mailboxSessionLock.Session.UserConfigurationManager.GetMailboxConfiguration(configName, UserConfigurationTypes.Stream))
					{
						using (Stream stream = mailboxConfiguration.GetStream())
						{
							result = (stream.Length != 0L);
						}
					}
				}
				catch (ObjectNotFoundException)
				{
					result = false;
				}
				catch (CorruptDataException)
				{
					result = false;
				}
			}
			return result;
		}

		private UserConfiguration GetConfig(MailboxSession s)
		{
			UserConfiguration result = null;
			try
			{
				result = s.UserConfigurationManager.GetMailboxConfiguration("Um.General", UserConfigurationTypes.Dictionary);
			}
			catch (ObjectNotFoundException)
			{
				PIIMessage data = PIIMessage.Create(PIIType._SmtpAddress, this.mailbox.MailAddress);
				CallIdTracer.TraceDebug(ExTraceGlobals.XsoTracer, this, data, "Creating UM General configuration folder for user: _SmtpAddress.", new object[]
				{
					this.mailbox.MailAddress
				});
				result = s.UserConfigurationManager.CreateMailboxConfiguration("Um.General", UserConfigurationTypes.Dictionary);
			}
			catch (CorruptDataException)
			{
				result = this.RebuildConfiguration(s);
			}
			catch (InvalidOperationException)
			{
				result = this.RebuildConfiguration(s);
			}
			return result;
		}

		private IDictionary CopyFromUserConfig()
		{
			Hashtable hashtable = new Hashtable();
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.mailbox.CreateSessionLock())
			{
				using (UserConfiguration config = this.GetConfig(mailboxSessionLock.Session))
				{
					IDictionary dictionary = config.GetDictionary();
					foreach (object key in dictionary.Keys)
					{
						hashtable[key] = dictionary[key];
					}
				}
			}
			return hashtable;
		}

		private void CopyToUserConfig(IDictionary srcDictionary)
		{
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.mailbox.CreateSessionLock())
			{
				using (UserConfiguration config = this.GetConfig(mailboxSessionLock.Session))
				{
					IDictionary dictionary = config.GetDictionary();
					foreach (object key in srcDictionary.Keys)
					{
						dictionary[key] = srcDictionary[key];
					}
					config.Save();
				}
			}
		}

		private bool GetConfigValue(string key, bool defaultValue)
		{
			object obj = this.Dictionary[key];
			if (obj == null)
			{
				return defaultValue;
			}
			if (!(obj is bool))
			{
				this.DeleteCorruptedConfiguration();
				return defaultValue;
			}
			return (bool)obj;
		}

		private UMMailboxRecipient mailbox;

		private IDictionary dictionary;

		private ADRecipient adRecipient;

		private bool promptsDirty;

		internal static class CoreTypeLoader
		{
			private static Assembly GetGreetingAssembly()
			{
				return Assembly.LoadFrom(Path.Combine(Utils.GetExchangeBinPath(), "Microsoft.Exchange.UM.UMCore.dll"));
			}

			private const string CoreAssemblyName = "Microsoft.Exchange.UM.UMCore.dll";

			private const string XsoGreetingName = "Microsoft.Exchange.UM.UMCore.XsoGreeting";

			private const string AdGreetingName = "Microsoft.Exchange.UM.UMCore.ADGreeting";

			private const string GlobCfgName = "Microsoft.Exchange.UM.UMCore.GlobCfg";

			internal static readonly Assembly GreetingAssembly = XsoConfigurationFolder.CoreTypeLoader.GetGreetingAssembly();

			internal static readonly Type XsoGreetingType = XsoConfigurationFolder.CoreTypeLoader.GreetingAssembly.GetType("Microsoft.Exchange.UM.UMCore.XsoGreeting", true);

			internal static readonly Type AdGreetingType = XsoConfigurationFolder.CoreTypeLoader.GreetingAssembly.GetType("Microsoft.Exchange.UM.UMCore.ADGreeting", true);

			internal static readonly Type GlobCfgType = XsoConfigurationFolder.CoreTypeLoader.GreetingAssembly.GetType("Microsoft.Exchange.UM.UMCore.GlobCfg", true);
		}
	}
}
