using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Audio;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.Prompts.Provisioning;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class UMConfigCache
	{
		internal string GetPrompt<T>(T config, string fileName) where T : ADConfigurationObject
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "UMConfigCache.GetPrompt {0}, {1}", new object[]
			{
				config,
				fileName
			});
			string text = null;
			if (!string.IsNullOrEmpty(fileName))
			{
				UMConfigCache.CacheEntry entry = this.GetEntry<T>(config.Id, config.OrganizationId);
				if (!entry.PromptsDownloaded)
				{
					entry.InitializeSessionCache();
				}
				text = entry.GetPrompt(fileName).FullName;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "UMConfigCache.GetPrompt returns {0}", new object[]
			{
				text
			});
			return text;
		}

		internal string CheckIfFileExists<T>(T config, string fileName) where T : ADConfigurationObject
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "UMConfigCache.CheckIfFileExists {0}, {1}", new object[]
			{
				config,
				fileName
			});
			if (string.IsNullOrEmpty(fileName))
			{
				return null;
			}
			string text = null;
			try
			{
				text = this.GetPrompt<T>(config, fileName);
			}
			catch (FileNotFoundException)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "UMConfigCache.CheckIfFileExists - File presently not in PCMCache", new object[0]);
			}
			if (text == null)
			{
				UMConfigCache.CacheEntry entry = this.GetEntry<T>(config.Id, config.OrganizationId);
				text = entry.GetPossiblyUnReferencedPromptIntoCache(fileName).FullName;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "UMConfigCache.CheckIfFileExists returns {0}", new object[]
			{
				text
			});
			return text;
		}

		internal void SetPrompt<T>(T config, string sourcePath, string selectedFileName) where T : ADConfigurationObject
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "UMConfigCache.SetPrompt {0}, {1}, {2}", new object[]
			{
				config,
				sourcePath,
				selectedFileName
			});
			UMConfigCache.CacheEntry entry = this.GetEntry<T>(config.Id, config.OrganizationId);
			entry.SetPrompt(sourcePath, selectedFileName);
		}

		internal T Find<T>(ADObjectId objectId, OrganizationId orgId) where T : ADConfigurationObject
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "UMConfigCache.Find '{0}' '{1}'", new object[]
			{
				objectId,
				orgId
			});
			T result = default(T);
			UMConfigCache.CacheEntry entry = this.GetEntry<T>(objectId, orgId);
			if (entry != null)
			{
				result = (T)((object)entry.Config);
			}
			return result;
		}

		private UMConfigCache.CacheEntry GetEntry<T>(ADObjectId key, OrganizationId orgId) where T : ADConfigurationObject
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "UMConfigCache.GetEntry '{0}' '{1}'", new object[]
			{
				key,
				orgId
			});
			UMConfigCache.CacheEntry cacheEntry;
			if (this.hashTable.ContainsKey(key))
			{
				cacheEntry = this.hashTable[key];
			}
			else if (typeof(T) == typeof(UMDialPlan))
			{
				cacheEntry = new UMConfigCache.UMDialPlanCacheEntry(key, orgId);
			}
			else
			{
				if (!(typeof(T) == typeof(UMAutoAttendant)))
				{
					throw new ArgumentException("key");
				}
				cacheEntry = new UMConfigCache.UMAutoAttendantCacheEntry(key, orgId);
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "Cache entry for '{0}' Found? = '{1}'", new object[]
			{
				key,
				cacheEntry.Config != null
			});
			UMConfigCache.CacheEntry result = null;
			if (cacheEntry.Config != null)
			{
				this.hashTable[key] = cacheEntry;
				result = cacheEntry;
			}
			return result;
		}

		private Dictionary<ADObjectId, UMConfigCache.CacheEntry> hashTable = new Dictionary<ADObjectId, UMConfigCache.CacheEntry>();

		private abstract class CacheEntry
		{
			internal CacheEntry(ADObjectId key, OrganizationId orgId)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "Constructing CacheEntry key='{0}', '{1}'", new object[]
				{
					key,
					orgId
				});
				this.config = this.ReadConfiguration(key, orgId);
			}

			internal void InitializeSessionCache()
			{
				if (this.config != null)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "Constructing CacheEntry config != null", new object[0]);
					bool flag = true;
					Exception ex = null;
					this.missingPromptList = new List<string>();
					FileStream fileStream = null;
					try
					{
						this.pcmCache = TempFileFactory.CreateTempDir();
						fileStream = this.EnsurePersistedCache();
						bool flag2 = null != fileStream;
						CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "CacheEntry , usePersistedCache ={0}", new object[]
						{
							flag2
						});
						if (!flag2)
						{
							this.EnsureSessionCache();
						}
						if (!this.ApplyConfiguration(false))
						{
							string missingPromptsString = this.GetMissingPromptsString();
							if (!string.IsNullOrEmpty(missingPromptsString))
							{
								this.LogMissingPromptsEvent(missingPromptsString);
								flag = false;
								ex = null;
							}
						}
						if (!flag2)
						{
							this.PersistSessionCache();
						}
					}
					catch (PublishingPointException ex2)
					{
						flag = false;
						ex = ex2;
					}
					catch (IOException ex3)
					{
						flag = false;
						ex = ex3;
					}
					finally
					{
						if (fileStream != null)
						{
							fileStream.Dispose();
						}
					}
					if (!flag)
					{
						this.LogInvalidConfiguration(this.config.Id, ex);
						throw CallRejectedException.Create(Strings.ObjectPromptsNotConsistent(this.Config.Name), ex, CallEndingReason.ObjectPromptsNotConsistent, "Object: {0}.", new object[]
						{
							this.Config.Name
						});
					}
				}
			}

			internal virtual ADConfigurationObject Config
			{
				get
				{
					return this.config;
				}
			}

			internal bool PromptsDownloaded
			{
				get
				{
					return this.sessionCache != null;
				}
			}

			protected string CacheRootPath
			{
				get
				{
					string path = Path.Combine(GlobCfg.ExchangeDirectory, "UnifiedMessaging\\Prompts\\Cache");
					return Path.Combine(path, this.Config.Guid.ToString());
				}
			}

			protected string PersistedCachePath
			{
				get
				{
					return Path.Combine(this.CacheRootPath, this.ChangeKey);
				}
			}

			protected abstract string ChangeKey { get; }

			protected ITempFile PcmCache
			{
				get
				{
					return this.pcmCache;
				}
			}

			internal FileInfo GetPrompt(string fileName)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "CacheEntry.GetPrompt filename='{0}'", new object[]
				{
					fileName
				});
				string text = Path.Combine(this.PcmCache.FilePath, fileName + ".wav");
				FileInfo fileInfo = new FileInfo(text);
				if (!fileInfo.Exists)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "Did not find file '{0}' at path '{1}'", new object[]
					{
						fileName,
						text
					});
					throw CallRejectedException.Create(Strings.ObjectPromptsNotConsistent(this.Config.Name), null, CallEndingReason.ObjectPromptsNotConsistent, "Object: {0}.", new object[]
					{
						this.Config.Name
					});
				}
				return fileInfo;
			}

			internal FileInfo GetPossiblyUnReferencedPromptIntoCache(string fileName)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "CacheEntry.GetPossiblyUnReferencedPromptIntoCache filename='{0}'", new object[]
				{
					fileName
				});
				this.ProcessPrompt(fileName, false);
				string text = Path.Combine(this.PcmCache.FilePath, fileName + ".wav");
				FileInfo fileInfo = new FileInfo(text);
				if (!fileInfo.Exists)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "Did not find file '{0}' at path '{1}'", new object[]
					{
						fileName,
						text
					});
					throw CallRejectedException.Create(Strings.ObjectPromptsNotConsistent(this.Config.Name), null, CallEndingReason.ObjectPromptsNotConsistent, "Object: {0}.", new object[]
					{
						this.Config.Name
					});
				}
				return fileInfo;
			}

			internal void SetPrompt(string sourcePath, string selectedFileName)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "CacheEntry.SetPrompt sourcePath='{0}', selectedFileName='{1}'", new object[]
				{
					sourcePath,
					selectedFileName
				});
				string text = Path.Combine(this.PcmCache.FilePath, selectedFileName + ".wav");
				File.Copy(sourcePath, text, true);
				TempFileFactory.AddNetworkServiceReadAccess(text, false);
			}

			protected abstract void LogMissingPromptsEvent(string missing);

			protected abstract ADConfigurationObject ReadConfiguration(ADObjectId objectId, OrganizationId orgId);

			protected abstract bool ApplyConfiguration(bool whatif);

			protected abstract void LogInvalidConfiguration(ADObjectId id, Exception e);

			protected abstract void LogCacheUpdateEvent();

			protected bool ProcessPrompt(string fileName)
			{
				return this.ProcessPrompt(fileName, true);
			}

			protected string GetMissingPromptsString()
			{
				string result = null;
				if (this.missingPromptList.Count > 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendLine();
					for (int i = 0; i < this.missingPromptList.Count; i++)
					{
						stringBuilder.AppendLine();
						stringBuilder.Append(this.missingPromptList[i]);
					}
					stringBuilder.AppendLine();
					stringBuilder.AppendLine();
					result = stringBuilder.ToString();
				}
				return result;
			}

			private bool ProcessPrompt(string fileName, bool logMissingPrompts)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "CacheEntry.ProcessPrompt({0}).", new object[]
				{
					fileName
				});
				bool flag = false;
				Exception ex = null;
				try
				{
					if (string.IsNullOrEmpty(fileName))
					{
						flag = true;
					}
					else
					{
						string path = fileName + ".wma";
						string text = Path.Combine(this.sessionCache.FullName, path);
						if (File.Exists(text))
						{
							CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "ProcessPrompt cacheFilePath({0}) exists.", new object[]
							{
								text
							});
							string text2 = Path.Combine(this.PcmCache.FilePath, fileName + ".wav");
							if (File.Exists(text2))
							{
								flag = true;
							}
							else
							{
								CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "Extracting source='{0}' to destination='{1}'", new object[]
								{
									text,
									text2
								});
								using (PcmWriter pcmWriter = new PcmWriter(text2, WaveFormat.Pcm8WaveFormat))
								{
									using (WmaReader wmaReader = new WmaReader(text))
									{
										byte[] array = new byte[wmaReader.SampleSize * 2];
										int count;
										while ((count = wmaReader.Read(array, array.Length)) > 0)
										{
											pcmWriter.Write(array, count);
										}
									}
								}
								TempFileFactory.AddNetworkServiceReadAccess(text2, false);
								flag = true;
							}
						}
						else
						{
							CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "ProcessPrompt cacheFilePath({0}) doesnt exists.", new object[]
							{
								text
							});
						}
					}
				}
				catch (InvalidWmaFormatException ex2)
				{
					ex = ex2;
				}
				catch (COMException ex3)
				{
					ex = ex3;
				}
				catch (IOException ex4)
				{
					ex = ex4;
				}
				if (!flag && logMissingPrompts)
				{
					this.missingPromptList.Add(fileName);
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "ProcessPrompt ret='{0}', error='{1}'.", new object[]
				{
					flag,
					ex
				});
				return flag;
			}

			private FileStream EnsurePersistedCache()
			{
				FileStream fileStream = this.LockPersistedCache();
				if (fileStream != null)
				{
					this.sessionCache = new DirectoryInfo(this.PersistedCachePath);
					if (!this.ApplyConfiguration(true))
					{
						fileStream.Dispose();
						fileStream = null;
						this.sessionCache.Delete(true);
						this.sessionCache = null;
					}
					else
					{
						CacheCleaner.TouchUpDirectory(this.sessionCache);
					}
				}
				return fileStream;
			}

			private void EnsureSessionCache()
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "CacheEntry.EnsureSessionCache", new object[0]);
				this.sessionCache = new DirectoryInfo(Path.Combine(this.CacheRootPath, Guid.NewGuid().ToString()));
				Directory.CreateDirectory(this.sessionCache.FullName);
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "Downloading prompts to {0}", new object[]
				{
					this.sessionCache
				});
				if (!this.ApplyConfiguration(true))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "executing DownloadAllAsWma for {0}", new object[]
					{
						this.Config.Name
					});
					using (IPublishingSession publishingSession = PublishingPoint.GetPublishingSession("UM", this.config))
					{
						publishingSession.DownloadAllAsWma(this.sessionCache);
					}
					CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "completed DownloadAllAsWma for {0}", new object[]
					{
						this.Config.Name
					});
				}
			}

			private FileStream LockPersistedCache()
			{
				FileStream result = null;
				if (Directory.Exists(this.PersistedCachePath))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "LockPersistedCache , directory {0} exists", new object[]
					{
						this.PersistedCachePath
					});
					try
					{
						result = new FileStream(Path.Combine(this.PersistedCachePath, Guid.NewGuid().ToString()), FileMode.Create, FileAccess.ReadWrite, FileShare.None, 8, FileOptions.DeleteOnClose);
						CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "LockPersistedCache , was able to open filestream", new object[0]);
					}
					catch (DirectoryNotFoundException ex)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "CreateDirectoryFileLock exception='{0}'", new object[]
						{
							ex
						});
					}
				}
				return result;
			}

			private void PersistSessionCache()
			{
				try
				{
					Directory.Move(this.sessionCache.FullName, this.PersistedCachePath);
					this.sessionCache = new DirectoryInfo(this.PersistedCachePath);
				}
				catch (IOException ex)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "Unable to persist session cache e='{0}'", new object[]
					{
						ex
					});
				}
			}

			private ADConfigurationObject config;

			private ITempFile pcmCache;

			private List<string> missingPromptList;

			private DirectoryInfo sessionCache;
		}

		private class UMAutoAttendantCacheEntry : UMConfigCache.CacheEntry
		{
			internal UMAutoAttendantCacheEntry(ADObjectId key, OrganizationId orgId) : base(key, orgId)
			{
			}

			protected override string ChangeKey
			{
				get
				{
					return this.AutoAttendantConfig.PromptChangeKey.ToString("N");
				}
			}

			private UMAutoAttendant AutoAttendantConfig
			{
				get
				{
					return (UMAutoAttendant)this.Config;
				}
			}

			protected override void LogMissingPromptsEvent(string missing)
			{
				if (!string.IsNullOrEmpty(missing))
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_AACustomPromptFileMissing, null, new object[]
					{
						this.Config.Id,
						CommonUtil.ToEventLogString(missing)
					});
				}
			}

			protected override ADConfigurationObject ReadConfiguration(ADObjectId objectId, OrganizationId orgId)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "AACacheEntry.ReadConfiguration('{0}', '{1}').", new object[]
				{
					objectId,
					orgId
				});
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(orgId, false);
				return iadsystemConfigurationLookup.GetAutoAttendantFromId(objectId);
			}

			protected override bool ApplyConfiguration(bool whatif)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "AACacheEntry.ApplyConfiguration().  Whatif={0}", new object[]
				{
					whatif
				});
				bool flag = true;
				flag &= base.ProcessPrompt(this.AutoAttendantConfig.AfterHoursMainMenuCustomPromptFilename);
				flag &= base.ProcessPrompt(this.AutoAttendantConfig.AfterHoursWelcomeGreetingFilename);
				flag &= base.ProcessPrompt(this.AutoAttendantConfig.BusinessHoursMainMenuCustomPromptFilename);
				flag &= base.ProcessPrompt(this.AutoAttendantConfig.BusinessHoursWelcomeGreetingFilename);
				flag &= base.ProcessPrompt(this.AutoAttendantConfig.InfoAnnouncementFilename);
				flag &= this.ProcessHolidaySchedules(this.AutoAttendantConfig);
				flag &= this.ProcessAfterHourKeyMappings(this.AutoAttendantConfig);
				flag &= this.ProcessBusinessHoursKeyMappings(this.AutoAttendantConfig);
				if (!whatif)
				{
					Util.IncrementCounter(AvailabilityCounters.PercentageCustomPromptDownloadFailures_Base, 1L);
					if (!flag)
					{
						Util.IncrementCounter(AvailabilityCounters.PercentageCustomPromptDownloadFailures, 1L);
					}
				}
				return flag;
			}

			protected override void LogInvalidConfiguration(ADObjectId id, Exception e)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "AACacheEntry.LogInvalidConfiguration().", new object[0]);
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_AACustomPromptInvalid, null, new object[]
				{
					id,
					CommonUtil.ToEventLogString(e)
				});
			}

			protected override void LogCacheUpdateEvent()
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_AutoAttendantCustomPromptCacheUpdate, null, new object[]
				{
					this.Config.Id
				});
			}

			private bool ProcessHolidaySchedules(UMAutoAttendant newConfig)
			{
				bool result = true;
				if (newConfig.HolidaySchedule != null)
				{
					foreach (HolidaySchedule holidaySchedule in newConfig.HolidaySchedule)
					{
						if (!base.ProcessPrompt(holidaySchedule.Greeting))
						{
							result = false;
						}
					}
				}
				return result;
			}

			private bool ProcessAfterHourKeyMappings(UMAutoAttendant newConfig)
			{
				bool result = true;
				if (newConfig.AfterHoursKeyMapping != null)
				{
					foreach (CustomMenuKeyMapping customMenuKeyMapping in newConfig.AfterHoursKeyMapping)
					{
						if (!base.ProcessPrompt(customMenuKeyMapping.PromptFileName))
						{
							result = false;
						}
					}
				}
				return result;
			}

			private bool ProcessBusinessHoursKeyMappings(UMAutoAttendant newConfig)
			{
				bool result = true;
				if (newConfig.BusinessHoursKeyMapping != null)
				{
					foreach (CustomMenuKeyMapping customMenuKeyMapping in newConfig.BusinessHoursKeyMapping)
					{
						if (!base.ProcessPrompt(customMenuKeyMapping.PromptFileName))
						{
							result = false;
						}
					}
				}
				return result;
			}

			private static UMAutoAttendant defaultConfig = new UMAutoAttendant();
		}

		private class UMDialPlanCacheEntry : UMConfigCache.CacheEntry
		{
			internal UMDialPlanCacheEntry(ADObjectId key, OrganizationId orgId) : base(key, orgId)
			{
			}

			protected override string ChangeKey
			{
				get
				{
					if (!string.IsNullOrEmpty(this.DialPlanConfig.PromptChangeKey))
					{
						return this.DialPlanConfig.PromptChangeKey;
					}
					return Guid.Empty.ToString("N");
				}
			}

			private UMDialPlan DialPlanConfig
			{
				get
				{
					return (UMDialPlan)this.Config;
				}
			}

			protected override void LogMissingPromptsEvent(string missing)
			{
				if (!string.IsNullOrEmpty(missing))
				{
					UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_DialPlanCustomPromptFileMissing, null, new object[]
					{
						this.Config.Id,
						CommonUtil.ToEventLogString(missing)
					});
				}
			}

			protected override ADConfigurationObject ReadConfiguration(ADObjectId objectId, OrganizationId orgId)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "DPCacheEntry.ReadConfiguration('{0}', '{1}').", new object[]
				{
					objectId,
					orgId
				});
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(orgId, false);
				return iadsystemConfigurationLookup.GetDialPlanFromId(objectId);
			}

			protected override bool ApplyConfiguration(bool whatif)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "DPCacheEntry.ApplyConfiguration().  Whatif={0}", new object[]
				{
					whatif
				});
				bool flag = true;
				flag &= base.ProcessPrompt(this.DialPlanConfig.WelcomeGreetingFilename);
				flag &= base.ProcessPrompt(this.DialPlanConfig.InfoAnnouncementFilename);
				if (!whatif)
				{
					Util.IncrementCounter(AvailabilityCounters.PercentageCustomPromptDownloadFailures_Base, 1L);
					if (!flag)
					{
						Util.IncrementCounter(AvailabilityCounters.PercentageCustomPromptDownloadFailures, 1L);
					}
				}
				return flag;
			}

			protected override void LogCacheUpdateEvent()
			{
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_DialPlanCustomPromptCacheUpdated, null, new object[]
				{
					this.Config.Id
				});
			}

			protected override void LogInvalidConfiguration(ADObjectId id, Exception e)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PromptProvisioningTracer, this, "DPCacheEntry.LogInvalidConfiguration().", new object[0]);
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_DialPlanCustomPromptInvalid, null, new object[]
				{
					id,
					CommonUtil.ToEventLogString(e)
				});
			}

			private static UMDialPlan defaultConfig = new UMDialPlan();
		}
	}
}
