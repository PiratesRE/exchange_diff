using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Model;
using Microsoft.Exchange.Flighting;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Win32;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch.WebService.Infrastructure
{
	internal class SearchPolicy : ISearchPolicy
	{
		public SearchPolicy(IRecipientSession recipientSession, CallerInfo callerInfo, ExchangeRunspaceConfiguration runspaceConfiguration, IBudget budget = null)
		{
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			if (callerInfo == null)
			{
				throw new ArgumentNullException("callerInfo");
			}
			this.CallerInfo = callerInfo;
			this.RecipientSession = recipientSession;
			this.RunspaceConfiguration = runspaceConfiguration;
			this.ThrottlingSettings = new SearchPolicy.ThrottlingPolicySettings(this);
			this.ExecutionSettings = new SearchPolicy.ExecutionPolicySettings(this);
			this.Recorder = new Recorder(this);
			this.Budget = budget;
		}

		public CallerInfo CallerInfo { get; private set; }

		public IRecipientSession RecipientSession { get; private set; }

		public ExchangeRunspaceConfiguration RunspaceConfiguration { get; private set; }

		public IThrottlingSettings ThrottlingSettings { get; private set; }

		public IExecutionSettings ExecutionSettings { get; private set; }

		public IBudget Budget { get; private set; }

		public Recorder Recorder { get; private set; }

		public ActivityScope GetActivityScope()
		{
			return ActivityContext.Start(null);
		}

		public static class ServiceName
		{
			public const string GetSearchableMailboxes = "GetSearchableMailboxes";

			public const string SearchMailboxes = "SearchMailboxes";
		}

		internal class ThrottlingPolicySettings : IThrottlingSettings
		{
			public ThrottlingPolicySettings(ISearchPolicy policy)
			{
				if (policy == null)
				{
					throw new ArgumentNullException("policy");
				}
				this.throttlingPolicy = SearchFactory.Current.GetThrottlingPolicy(policy);
			}

			public uint DiscoveryMaxConcurrency
			{
				get
				{
					if (!this.throttlingPolicy.DiscoveryMaxConcurrency.IsUnlimited)
					{
						return this.throttlingPolicy.DiscoveryMaxConcurrency.Value;
					}
					Recorder.Trace(2L, TraceType.WarningTrace, "ThrottlingPolicySettings.DiscoveryMaxConcurrency Fallback");
					return ThrottlingPolicyDefaults.DiscoveryMaxConcurrency.Value;
				}
			}

			public uint DiscoveryMaxKeywords
			{
				get
				{
					if (!this.throttlingPolicy.DiscoveryMaxKeywords.IsUnlimited)
					{
						return this.throttlingPolicy.DiscoveryMaxKeywords.Value;
					}
					Recorder.Trace(2L, TraceType.WarningTrace, "ThrottlingPolicySettings.DiscoveryMaxKeywords Fallback");
					return ThrottlingPolicyDefaults.DiscoveryMaxKeywords.Value;
				}
			}

			public uint DiscoveryMaxKeywordsPerPage
			{
				get
				{
					if (!this.throttlingPolicy.DiscoveryMaxKeywordsPerPage.IsUnlimited)
					{
						return this.throttlingPolicy.DiscoveryMaxKeywordsPerPage.Value;
					}
					Recorder.Trace(2L, TraceType.WarningTrace, "ThrottlingPolicySettings.DiscoveryMaxKeywordsPerPage Fallback");
					return ThrottlingPolicyDefaults.DiscoveryMaxKeywordsPerPage.Value;
				}
			}

			public uint DiscoveryMaxMailboxes
			{
				get
				{
					if (!this.throttlingPolicy.DiscoveryMaxMailboxes.IsUnlimited)
					{
						return this.throttlingPolicy.DiscoveryMaxMailboxes.Value;
					}
					Recorder.Trace(2L, TraceType.WarningTrace, "ThrottlingPolicySettings.DiscoveryMaxMailboxes Fallback");
					return ThrottlingPolicyDefaults.DiscoveryMaxMailboxes.Value;
				}
			}

			public uint DiscoveryMaxPreviewSearchMailboxes
			{
				get
				{
					if (!this.throttlingPolicy.DiscoveryMaxPreviewSearchMailboxes.IsUnlimited)
					{
						return this.throttlingPolicy.DiscoveryMaxPreviewSearchMailboxes.Value;
					}
					Recorder.Trace(2L, TraceType.WarningTrace, "ThrottlingPolicySettings.DiscoveryMaxPreviewSearchMailboxes Fallback");
					return ThrottlingPolicyDefaults.DiscoveryMaxPreviewSearchMailboxes.Value;
				}
			}

			public uint DiscoveryMaxRefinerResults
			{
				get
				{
					if (!this.throttlingPolicy.DiscoveryMaxRefinerResults.IsUnlimited)
					{
						return this.throttlingPolicy.DiscoveryMaxRefinerResults.Value;
					}
					Recorder.Trace(2L, TraceType.WarningTrace, "ThrottlingPolicySettings.DiscoveryMaxRefinerResults Fallback");
					return ThrottlingPolicyDefaults.DiscoveryMaxRefinerResults.Value;
				}
			}

			public uint DiscoveryMaxSearchQueueDepth
			{
				get
				{
					if (!this.throttlingPolicy.DiscoveryMaxSearchQueueDepth.IsUnlimited)
					{
						return this.throttlingPolicy.DiscoveryMaxSearchQueueDepth.Value;
					}
					Recorder.Trace(2L, TraceType.WarningTrace, "ThrottlingPolicySettings.DiscoveryMaxSearchQueueDepth Fallback");
					return ThrottlingPolicyDefaults.DiscoveryMaxSearchQueueDepth.Value;
				}
			}

			public uint DiscoveryMaxStatsSearchMailboxes
			{
				get
				{
					if (!this.throttlingPolicy.DiscoveryMaxStatsSearchMailboxes.IsUnlimited)
					{
						return this.throttlingPolicy.DiscoveryMaxStatsSearchMailboxes.Value;
					}
					Recorder.Trace(2L, TraceType.WarningTrace, "ThrottlingPolicySettings.DiscoveryMaxStatsSearchMailboxes Fallback");
					return ThrottlingPolicyDefaults.DiscoveryMaxStatsSearchMailboxes.Value;
				}
			}

			public uint DiscoveryPreviewSearchResultsPageSize
			{
				get
				{
					if (!this.throttlingPolicy.DiscoveryPreviewSearchResultsPageSize.IsUnlimited)
					{
						return this.throttlingPolicy.DiscoveryPreviewSearchResultsPageSize.Value;
					}
					Recorder.Trace(2L, TraceType.WarningTrace, "ThrottlingPolicySettings.DiscoveryPreviewSearchResultsPageSize Fallback");
					return ThrottlingPolicyDefaults.DiscoveryPreviewSearchResultsPageSize.Value;
				}
			}

			public uint DiscoverySearchTimeoutPeriod
			{
				get
				{
					if (!this.throttlingPolicy.DiscoverySearchTimeoutPeriod.IsUnlimited)
					{
						return this.throttlingPolicy.DiscoverySearchTimeoutPeriod.Value;
					}
					Recorder.Trace(2L, TraceType.WarningTrace, "ThrottlingPolicySettings.DiscoverySearchTimeoutPeriod Fallback");
					return ThrottlingPolicyDefaults.DiscoverySearchTimeoutPeriod.Value;
				}
			}

			private IThrottlingPolicy throttlingPolicy;
		}

		internal class ExecutionPolicySettings : IExecutionSettings
		{
			public ExecutionPolicySettings(ISearchPolicy policy)
			{
				if (policy == null)
				{
					throw new ArgumentNullException("policy");
				}
				this.policy = policy;
				this.Snapshot = SearchFactory.Current.GetVariantConfigurationSnapshot(policy);
				this.settingsMap = this.Snapshot.Discovery.GetObjectsOfType<ISettingsValue>();
				this.useRegDiscoveryUseFastSearch = this.LookupRegBool("DiscoveryUseFastSearch", out this.regDiscoveryUseFastSearch);
			}

			public VariantConfigurationSnapshot Snapshot { get; private set; }

			public bool DiscoveryUseFastSearch
			{
				get
				{
					if (this.useRegDiscoveryUseFastSearch)
					{
						return this.regDiscoveryUseFastSearch;
					}
					return this.GetBool("DiscoveryUseFastSearch", false);
				}
			}

			public bool DiscoveryAggregateLogs
			{
				get
				{
					return this.GetBool("DiscoveryAggregateLogs", false);
				}
			}

			public bool DiscoveryExecutesInParallel
			{
				get
				{
					return this.GetBool("DiscoveryExecutesInParallel", true);
				}
			}

			public bool DiscoveryLocalSearchIsParallel
			{
				get
				{
					return this.GetBool("DiscoveryLocalSearchIsParallel", false);
				}
			}

			public int DiscoveryMaxMailboxes
			{
				get
				{
					return (int)this.GetNumber("DiscoveryMaxMailboxes", this.policy.ThrottlingSettings.DiscoveryMaxMailboxes);
				}
			}

			public int DiscoveryKeywordsBatchSize
			{
				get
				{
					return (int)this.GetNumber("DiscoveryKeywordsBatchSize", 6.0);
				}
			}

			public int DiscoveryDefaultPageSize
			{
				get
				{
					return (int)this.GetNumber("DiscoveryDefaultPageSize", 100.0);
				}
			}

			public uint DiscoveryMaxAllowedExecutorItems
			{
				get
				{
					return (uint)this.GetNumber("DiscoveryMaxAllowedExecutorItems", 30000.0);
				}
			}

			public int DiscoveryMaxAllowedMailboxQueriesPerRequest
			{
				get
				{
					return (int)this.GetNumber("DiscoveryMaxAllowedMailboxQueriesPerRequest", 5.0);
				}
			}

			public int DiscoveryMaxAllowedResultsPageSize
			{
				get
				{
					return (int)this.GetNumber("DiscoveryMaxAllowedResultsPageSize", 500.0);
				}
			}

			public uint DiscoveryADPageSize
			{
				get
				{
					return (uint)this.GetNumber("DiscoveryADPageSize", 50.0);
				}
			}

			public uint DiscoveryADLookupConcurrency
			{
				get
				{
					return (uint)this.GetNumber("DiscoveryADLookupConcurrency", 4.0);
				}
			}

			public uint DiscoveryServerLookupConcurrency
			{
				get
				{
					return (uint)this.GetNumber("DiscoveryServerLookupConcurrency", 4.0);
				}
			}

			public uint DiscoveryServerLookupBatch
			{
				get
				{
					return (uint)this.GetNumber("DiscoveryServerLookupBatch", 30.0);
				}
			}

			public uint DiscoveryFanoutConcurrency
			{
				get
				{
					return (uint)this.GetNumber("DiscoveryFanoutConcurrency", 100.0);
				}
			}

			public uint DiscoveryFanoutBatch
			{
				get
				{
					return (uint)this.GetNumber("DiscoveryFanoutBatch", 50.0);
				}
			}

			public uint DiscoveryLocalSearchConcurrency
			{
				get
				{
					return (uint)this.GetNumber("DiscoveryLocalSearchConcurrency", 50.0);
				}
			}

			public uint DiscoveryLocalSearchBatch
			{
				get
				{
					return (uint)this.GetNumber("DiscoveryLocalSearchBatchSize", 4294967295.0);
				}
			}

			public uint DiscoverySynchronousConcurrency
			{
				get
				{
					return 0U;
				}
			}

			public uint DiscoveryDisplaySearchBatchSize
			{
				get
				{
					return (uint)this.GetNumber("DiscoveryDisplaySearchBatchSize", 50.0);
				}
			}

			public uint DiscoveryDisplaySearchPageSize
			{
				get
				{
					return (uint)this.GetNumber("DiscoveryDisplaySearchPageSize", 1500.0);
				}
			}

			public TimeSpan SearchTimeout
			{
				get
				{
					return TimeSpan.FromMinutes(this.GetNumber("SearchTimeout", 4.5));
				}
			}

			public TimeSpan ServiceTopologyTimeout
			{
				get
				{
					return TimeSpan.FromSeconds(this.GetNumber("ServiceTopologyTimeout", 10.0));
				}
			}

			public TimeSpan MailboxServerLocatorTimeout
			{
				get
				{
					return TimeSpan.FromSeconds(this.GetNumber("MailboxServerLocatorTimeout", 30.0));
				}
			}

			public List<DefaultFolderType> ExcludedFolders
			{
				get
				{
					if (this.excludedFolders == null)
					{
						if (this.GetBool("DiscoveryExcludedFoldersEnabled", false))
						{
							this.excludedFolders = this.GetEnumList<DefaultFolderType>("DiscoveryExcludedFolders");
						}
						else
						{
							this.excludedFolders = new List<DefaultFolderType>();
						}
					}
					return this.excludedFolders;
				}
			}

			private double GetNumber(string key, double fallback)
			{
				Recorder.Trace(2L, TraceType.InfoTrace, new object[]
				{
					"ExecutionPolicySettings.GetNumber Key:",
					key,
					"Fallback:",
					fallback
				});
				if (this.settingsMap.ContainsKey(key))
				{
					Recorder.Trace(2L, TraceType.InfoTrace, "ExecutionPolicySettings.GetNumber Found Setting Key:", key);
					double num;
					if (double.TryParse(this.settingsMap[key].Value, out num))
					{
						if (num == -1.0)
						{
							num = 4294967295.0;
						}
						Recorder.Trace(2L, TraceType.InfoTrace, new object[]
						{
							"ExecutionPolicySettings.GetNumber Found Setting Key:",
							key,
							"Value:",
							num
						});
						return num;
					}
				}
				return fallback;
			}

			private bool GetBool(string key, bool fallback)
			{
				Recorder.Trace(2L, TraceType.InfoTrace, new object[]
				{
					"ExecutionPolicySettings.GetBool Key:",
					key,
					"Fallback:",
					fallback
				});
				if (this.settingsMap.ContainsKey(key))
				{
					Recorder.Trace(2L, TraceType.InfoTrace, "ExecutionPolicySettings.GetBool Found Setting Key:", key);
					bool flag;
					if (bool.TryParse(this.settingsMap[key].Value, out flag))
					{
						Recorder.Trace(2L, TraceType.InfoTrace, new object[]
						{
							"ExecutionPolicySettings.GetBool Found Setting Key:",
							key,
							"Value:",
							flag
						});
						return flag;
					}
				}
				return fallback;
			}

			private List<T> GetEnumList<T>(string key) where T : struct, IConvertible
			{
				Recorder.Trace(2L, TraceType.InfoTrace, "ExecutionPolicySettings.GetList Key:", key);
				if (!typeof(T).IsEnum)
				{
					throw new ArgumentException("Specified type is not an enum");
				}
				List<T> list = new List<T>();
				if (this.settingsMap.ContainsKey(key))
				{
					string value = this.settingsMap[key].Value;
					Recorder.Trace(2L, TraceType.InfoTrace, new object[]
					{
						"ExecutionPolicySettings.GetList Setting Key:",
						key,
						"Value:",
						(value == null) ? string.Empty : value
					});
					if (!string.IsNullOrWhiteSpace(value))
					{
						string[] array = value.Split(new char[]
						{
							','
						});
						foreach (string text in array)
						{
							T item;
							if (Enum.TryParse<T>(text, true, out item))
							{
								list.Add(item);
							}
							else
							{
								Recorder.Trace(2L, TraceType.InfoTrace, "ExecutionPolicySettings.GetList invalid enum value:", text);
							}
						}
					}
					else
					{
						Recorder.Trace(2L, TraceType.InfoTrace, "ExecutionPolicySettings.GetList Setting key not found:", key);
					}
				}
				return list;
			}

			private bool LookupRegBool(string name, out bool boolValue)
			{
				boolValue = false;
				bool result;
				try
				{
					using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32))
					{
						using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\eDiscovery"))
						{
							if (registryKey2 != null)
							{
								string text = registryKey2.GetValue(name) as string;
								bool flag;
								if (text == null || !bool.TryParse(text, out flag))
								{
									result = false;
								}
								else
								{
									boolValue = flag;
									result = true;
								}
							}
							else
							{
								Recorder.Trace(2L, TraceType.InfoTrace, "SearchPolicySettings.LookupRegBool: Registry not found for constants");
								result = false;
							}
						}
					}
				}
				catch (Exception arg)
				{
					Recorder.Trace(2L, TraceType.InfoTrace, string.Format("SearchPolicySettings.LookupRegBool: Failed to load registry data. Details: {0}", arg));
					result = false;
				}
				return result;
			}

			private readonly bool useRegDiscoveryUseFastSearch;

			private IDictionary<string, ISettingsValue> settingsMap = new Dictionary<string, ISettingsValue>();

			private List<DefaultFolderType> excludedFolders;

			private ISearchPolicy policy;

			private bool regDiscoveryUseFastSearch;
		}
	}
}
