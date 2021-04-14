using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Search.KqlParser;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[Serializable]
	public sealed class MailboxDiscoverySearch : DiscoverySearchBase
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MailboxDiscoverySearch.schema;
			}
		}

		internal override string ItemClass
		{
			get
			{
				return "IPM.Configuration.MailboxDiscoverySearch";
			}
		}

		public string Target
		{
			get
			{
				return (string)this[MailboxDiscoverySearchSchema.Target];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.Target] = value;
			}
		}

		public SearchState Status
		{
			get
			{
				return (SearchState)this[MailboxDiscoverySearchSchema.Status];
			}
			private set
			{
				this[MailboxDiscoverySearchSchema.Status] = value;
			}
		}

		public bool StatisticsOnly
		{
			get
			{
				return (bool)this[MailboxDiscoverySearchSchema.StatisticsOnly];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.StatisticsOnly] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IncludeUnsearchableItems
		{
			get
			{
				return (bool)this[MailboxDiscoverySearchSchema.IncludeUnsearchableItems];
			}
			set
			{
				this[MailboxDiscoverySearchSchema.IncludeUnsearchableItems] = value;
			}
		}

		public bool Resume
		{
			get
			{
				return (bool)this[MailboxDiscoverySearchSchema.Resume];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.Resume] = value;
			}
		}

		public DateTime LastStartTime
		{
			get
			{
				return (DateTime)this[MailboxDiscoverySearchSchema.LastStartTime];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.LastStartTime] = value;
			}
		}

		public DateTime LastEndTime
		{
			get
			{
				return (DateTime)this[MailboxDiscoverySearchSchema.LastEndTime];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.LastEndTime] = value;
			}
		}

		public int NumberOfMailboxes
		{
			get
			{
				return (int)this[MailboxDiscoverySearchSchema.NumberOfMailboxes];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.NumberOfMailboxes] = value;
			}
		}

		public int PercentComplete
		{
			get
			{
				return (int)this[MailboxDiscoverySearchSchema.PercentComplete];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.PercentComplete] = value;
			}
		}

		public long ResultItemCountCopied
		{
			get
			{
				return (long)this[MailboxDiscoverySearchSchema.ResultItemCountCopied];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.ResultItemCountCopied] = value;
			}
		}

		public long ResultItemCountEstimate
		{
			get
			{
				return (long)this[MailboxDiscoverySearchSchema.ResultItemCountEstimate];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.ResultItemCountEstimate] = value;
			}
		}

		public long ResultUnsearchableItemCountEstimate
		{
			get
			{
				return (long)this[MailboxDiscoverySearchSchema.ResultUnsearchableItemCountEstimate];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.ResultUnsearchableItemCountEstimate] = value;
			}
		}

		public long ResultSizeCopied
		{
			get
			{
				return (long)this[MailboxDiscoverySearchSchema.ResultSizeCopied];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.ResultSizeCopied] = value;
			}
		}

		public long ResultSizeEstimate
		{
			get
			{
				return (long)this[MailboxDiscoverySearchSchema.ResultSizeEstimate];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.ResultSizeEstimate] = value;
			}
		}

		public string ResultsPath
		{
			get
			{
				return (string)this[MailboxDiscoverySearchSchema.ResultsPath];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.ResultsPath] = value;
			}
		}

		public string ResultsLink
		{
			get
			{
				return (string)this[MailboxDiscoverySearchSchema.ResultsLink];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.ResultsLink] = value;
			}
		}

		public string PreviewResultsLink
		{
			get
			{
				return (string)this[MailboxDiscoverySearchSchema.PreviewResultsLink];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.PreviewResultsLink] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LoggingLevel LogLevel
		{
			get
			{
				return (LoggingLevel)this[MailboxDiscoverySearchSchema.LogLevel];
			}
			set
			{
				this[MailboxDiscoverySearchSchema.LogLevel] = value;
			}
		}

		public MultiValuedProperty<string> CompletedMailboxes
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxDiscoverySearchSchema.CompletedMailboxes];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.CompletedMailboxes] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> StatusMailRecipients
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailboxDiscoverySearchSchema.StatusMailRecipients];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.StatusMailRecipients] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> ManagedBy
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[MailboxDiscoverySearchSchema.ManagedBy];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.ManagedBy] = value;
			}
		}

		public string Query
		{
			get
			{
				return (string)this[MailboxDiscoverySearchSchema.Query];
			}
			internal set
			{
				string a = (string)this[MailboxDiscoverySearchSchema.Query];
				this[MailboxDiscoverySearchSchema.Query] = value;
				if (a != value)
				{
					this.internalQueryFilter = null;
					this.CalculatedQuery = null;
				}
			}
		}

		public MultiValuedProperty<string> Senders
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxDiscoverySearchSchema.Senders];
			}
			internal set
			{
				MultiValuedProperty<string> col = (MultiValuedProperty<string>)this[MailboxDiscoverySearchSchema.Senders];
				if (!MailboxDiscoverySearch.AreCollectionsEqual<string>(col, value))
				{
					this.internalQueryFilter = null;
					this.CalculatedQuery = null;
				}
				this[MailboxDiscoverySearchSchema.Senders] = value;
			}
		}

		public MultiValuedProperty<string> Recipients
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxDiscoverySearchSchema.Recipients];
			}
			internal set
			{
				MultiValuedProperty<string> col = (MultiValuedProperty<string>)this[MailboxDiscoverySearchSchema.Recipients];
				if (!MailboxDiscoverySearch.AreCollectionsEqual<string>(col, value))
				{
					this.internalQueryFilter = null;
					this.CalculatedQuery = null;
				}
				this[MailboxDiscoverySearchSchema.Recipients] = value;
			}
		}

		public ExDateTime? StartDate
		{
			get
			{
				return (ExDateTime?)this[MailboxDiscoverySearchSchema.StartDate];
			}
			set
			{
				if ((ExDateTime?)this[MailboxDiscoverySearchSchema.StartDate] != value)
				{
					this.internalQueryFilter = null;
					this.CalculatedQuery = null;
				}
				this[MailboxDiscoverySearchSchema.StartDate] = value;
			}
		}

		public ExDateTime? EndDate
		{
			get
			{
				return SearchObject.RoundEndDate((ExDateTime?)this[MailboxDiscoverySearchSchema.EndDate]);
			}
			set
			{
				if ((ExDateTime?)this[MailboxDiscoverySearchSchema.EndDate] != value)
				{
					this.internalQueryFilter = null;
					this.CalculatedQuery = null;
				}
				this[MailboxDiscoverySearchSchema.EndDate] = value;
			}
		}

		public MultiValuedProperty<KindKeyword> MessageTypes
		{
			get
			{
				if (this[MailboxDiscoverySearchSchema.MessageTypes] != null)
				{
					MultiValuedProperty<KindKeyword> multiValuedProperty = new MultiValuedProperty<KindKeyword>();
					MultiValuedProperty<string> multiValuedProperty2 = (MultiValuedProperty<string>)this[MailboxDiscoverySearchSchema.MessageTypes];
					foreach (string value in multiValuedProperty2)
					{
						multiValuedProperty.Add((KindKeyword)Enum.Parse(typeof(KindKeyword), value, true));
					}
					return multiValuedProperty;
				}
				return null;
			}
			internal set
			{
				MultiValuedProperty<string> multiValuedProperty = null;
				if (value != null)
				{
					multiValuedProperty = new MultiValuedProperty<string>();
					foreach (KindKeyword kindKeyword in value)
					{
						multiValuedProperty.Add(kindKeyword.ToString());
					}
				}
				MultiValuedProperty<string> col = this[MailboxDiscoverySearchSchema.MessageTypes] as MultiValuedProperty<string>;
				if (!MailboxDiscoverySearch.AreCollectionsEqual<string>(col, multiValuedProperty))
				{
					this.internalQueryFilter = null;
					this.CalculatedQuery = null;
				}
				this[MailboxDiscoverySearchSchema.MessageTypes] = multiValuedProperty;
			}
		}

		public string CalculatedQuery
		{
			get
			{
				return (string)this[MailboxDiscoverySearchSchema.CalculatedQuery];
			}
			private set
			{
				this[MailboxDiscoverySearchSchema.CalculatedQuery] = value;
			}
		}

		public MultiValuedProperty<string> UserKeywords
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxDiscoverySearchSchema.UserKeywords];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.UserKeywords] = value;
			}
		}

		public string KeywordsQuery
		{
			get
			{
				return (string)this[MailboxDiscoverySearchSchema.KeywordsQuery];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.KeywordsQuery] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Language
		{
			get
			{
				return (string)this[MailboxDiscoverySearchSchema.Language];
			}
			set
			{
				this[MailboxDiscoverySearchSchema.Language] = value;
				this.queryCulture = null;
			}
		}

		public MultiValuedProperty<string> Sources
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxDiscoverySearchSchema.Sources];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.Sources] = value;
			}
		}

		public bool AllSourceMailboxes
		{
			get
			{
				return (bool)this[MailboxDiscoverySearchSchema.AllSourceMailboxes];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.AllSourceMailboxes] = value;
			}
		}

		public MultiValuedProperty<string> PublicFolderSources
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxDiscoverySearchSchema.PublicFolderSources];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.PublicFolderSources] = value;
			}
		}

		public bool AllPublicFolderSources
		{
			get
			{
				return (bool)this[MailboxDiscoverySearchSchema.AllPublicFolderSources];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.AllPublicFolderSources] = value;
			}
		}

		public MultiValuedProperty<DiscoverySearchStats> SearchStatistics
		{
			get
			{
				return (MultiValuedProperty<DiscoverySearchStats>)this[MailboxDiscoverySearchSchema.SearchStatistics];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.SearchStatistics] = value;
			}
		}

		public SearchObjectVersion Version
		{
			get
			{
				return (SearchObjectVersion)this[MailboxDiscoverySearchSchema.Version];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.Version] = value;
			}
		}

		public DateTime CreatedTime
		{
			get
			{
				return (DateTime)this[MailboxDiscoverySearchSchema.CreatedTime];
			}
		}

		public string CreatedBy
		{
			get
			{
				return (string)this[MailboxDiscoverySearchSchema.CreatedBy];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.CreatedBy] = value;
			}
		}

		public DateTime LastModifiedTime
		{
			get
			{
				return (DateTime)this[MailboxDiscoverySearchSchema.LastModifiedTime];
			}
		}

		public string LastModifiedBy
		{
			get
			{
				return (string)this[MailboxDiscoverySearchSchema.LastModifiedBy];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.LastModifiedBy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ExcludeDuplicateMessages
		{
			get
			{
				return (bool)this[MailboxDiscoverySearchSchema.ExcludeDuplicateMessages];
			}
			set
			{
				this[MailboxDiscoverySearchSchema.ExcludeDuplicateMessages] = value;
			}
		}

		public MultiValuedProperty<string> Errors
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxDiscoverySearchSchema.Errors];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.Errors] = value;
			}
		}

		public MultiValuedProperty<KeywordHit> KeywordHits
		{
			get
			{
				return (MultiValuedProperty<KeywordHit>)this[MailboxDiscoverySearchSchema.KeywordHits];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.KeywordHits] = value;
			}
		}

		public MultiValuedProperty<string> Information
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxDiscoverySearchSchema.Information];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.Information] = value;
			}
		}

		public bool KeywordStatisticsDisabled
		{
			get
			{
				return (bool)this[MailboxDiscoverySearchSchema.KeywordStatisticsDisabled];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.KeywordStatisticsDisabled] = value;
			}
		}

		public bool PreviewDisabled
		{
			get
			{
				return (bool)this[MailboxDiscoverySearchSchema.PreviewDisabled];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.PreviewDisabled] = value;
			}
		}

		public string ScenarioId
		{
			get
			{
				return this[MailboxDiscoverySearchSchema.ScenarioId] as string;
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.ScenarioId] = value;
			}
		}

		[Parameter]
		public bool InPlaceHoldEnabled
		{
			get
			{
				return (bool)this[MailboxDiscoverySearchSchema.InPlaceHoldEnabled];
			}
			set
			{
				this[MailboxDiscoverySearchSchema.InPlaceHoldEnabled] = value;
			}
		}

		[Parameter]
		public Unlimited<EnhancedTimeSpan> ItemHoldPeriod
		{
			get
			{
				return (Unlimited<EnhancedTimeSpan>)this[MailboxDiscoverySearchSchema.ItemHoldPeriod];
			}
			set
			{
				this[MailboxDiscoverySearchSchema.ItemHoldPeriod] = value;
			}
		}

		[Parameter]
		public string Description
		{
			get
			{
				return (string)this[MailboxDiscoverySearchSchema.Description];
			}
			set
			{
				this[MailboxDiscoverySearchSchema.Description] = value;
			}
		}

		public string InPlaceHoldIdentity
		{
			get
			{
				return (string)this[MailboxDiscoverySearchSchema.InPlaceHoldIdentity];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.InPlaceHoldIdentity] = value;
			}
		}

		public string LegacySearchObjectIdentity
		{
			get
			{
				return (string)this[MailboxDiscoverySearchSchema.LegacySearchObjectIdentity];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.LegacySearchObjectIdentity] = value;
			}
		}

		public MultiValuedProperty<string> FailedToHoldMailboxes
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxDiscoverySearchSchema.FailedToHoldMailboxes];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.FailedToHoldMailboxes] = value;
			}
		}

		public MultiValuedProperty<string> InPlaceHoldErrors
		{
			get
			{
				return (MultiValuedProperty<string>)this[MailboxDiscoverySearchSchema.InPlaceHoldErrors];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.InPlaceHoldErrors] = value;
			}
		}

		public string ManagedByOrganization
		{
			get
			{
				return (string)this[MailboxDiscoverySearchSchema.ManagedByOrganization];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.ManagedByOrganization] = value;
			}
		}

		internal string FlightedFeatures
		{
			get
			{
				if (this.flightedFeatures == null)
				{
					return string.Empty;
				}
				return string.Join(",", this.flightedFeatures);
			}
		}

		private Dictionary<SearchState, Dictionary<SearchStateTransition, SearchState>> StateMachine
		{
			get
			{
				if (this.stateMachine == null)
				{
					Dictionary<SearchState, Dictionary<SearchStateTransition, SearchState>> value = new Dictionary<SearchState, Dictionary<SearchStateTransition, SearchState>>
					{
						{
							SearchState.InProgress,
							new Dictionary<SearchStateTransition, SearchState>
							{
								{
									SearchStateTransition.StopSearch,
									SearchState.Stopped
								},
								{
									SearchStateTransition.MoveToNextState,
									SearchState.Succeeded
								},
								{
									SearchStateTransition.MoveToNextPartialSuccessState,
									SearchState.PartiallySucceeded
								},
								{
									SearchStateTransition.Fail,
									SearchState.Failed
								}
							}
						},
						{
							SearchState.Failed,
							new Dictionary<SearchStateTransition, SearchState>
							{
								{
									SearchStateTransition.StartSearch,
									SearchState.Queued
								},
								{
									SearchStateTransition.ResetSearch,
									SearchState.NotStarted
								},
								{
									SearchStateTransition.DeleteSearch,
									SearchState.DeletionInProgress
								}
							}
						},
						{
							SearchState.Stopped,
							new Dictionary<SearchStateTransition, SearchState>
							{
								{
									SearchStateTransition.StartSearch,
									SearchState.Queued
								},
								{
									SearchStateTransition.ResetSearch,
									SearchState.NotStarted
								},
								{
									SearchStateTransition.DeleteSearch,
									SearchState.DeletionInProgress
								},
								{
									SearchStateTransition.StopSearch,
									SearchState.Stopped
								}
							}
						},
						{
							SearchState.Succeeded,
							new Dictionary<SearchStateTransition, SearchState>
							{
								{
									SearchStateTransition.StartSearch,
									SearchState.Queued
								},
								{
									SearchStateTransition.ResetSearch,
									SearchState.NotStarted
								},
								{
									SearchStateTransition.DeleteSearch,
									SearchState.DeletionInProgress
								}
							}
						},
						{
							SearchState.PartiallySucceeded,
							new Dictionary<SearchStateTransition, SearchState>
							{
								{
									SearchStateTransition.StartSearch,
									SearchState.Queued
								},
								{
									SearchStateTransition.ResetSearch,
									SearchState.NotStarted
								},
								{
									SearchStateTransition.DeleteSearch,
									SearchState.DeletionInProgress
								}
							}
						},
						{
							SearchState.EstimateInProgress,
							new Dictionary<SearchStateTransition, SearchState>
							{
								{
									SearchStateTransition.DeleteSearch,
									SearchState.DeletionInProgress
								},
								{
									SearchStateTransition.StopSearch,
									SearchState.EstimateStopped
								},
								{
									SearchStateTransition.MoveToNextState,
									SearchState.EstimateSucceeded
								},
								{
									SearchStateTransition.MoveToNextPartialSuccessState,
									SearchState.EstimatePartiallySucceeded
								},
								{
									SearchStateTransition.Fail,
									SearchState.EstimateFailed
								}
							}
						},
						{
							SearchState.EstimateSucceeded,
							new Dictionary<SearchStateTransition, SearchState>
							{
								{
									SearchStateTransition.StartSearch,
									SearchState.Queued
								},
								{
									SearchStateTransition.ResetSearch,
									SearchState.NotStarted
								},
								{
									SearchStateTransition.DeleteSearch,
									SearchState.DeletionInProgress
								}
							}
						},
						{
							SearchState.EstimateFailed,
							new Dictionary<SearchStateTransition, SearchState>
							{
								{
									SearchStateTransition.StartSearch,
									SearchState.Queued
								},
								{
									SearchStateTransition.ResetSearch,
									SearchState.NotStarted
								},
								{
									SearchStateTransition.DeleteSearch,
									SearchState.DeletionInProgress
								}
							}
						},
						{
							SearchState.EstimateStopped,
							new Dictionary<SearchStateTransition, SearchState>
							{
								{
									SearchStateTransition.StartSearch,
									SearchState.Queued
								},
								{
									SearchStateTransition.ResetSearch,
									SearchState.NotStarted
								},
								{
									SearchStateTransition.DeleteSearch,
									SearchState.DeletionInProgress
								},
								{
									SearchStateTransition.StopSearch,
									SearchState.Stopped
								}
							}
						},
						{
							SearchState.EstimatePartiallySucceeded,
							new Dictionary<SearchStateTransition, SearchState>
							{
								{
									SearchStateTransition.StartSearch,
									SearchState.Queued
								},
								{
									SearchStateTransition.ResetSearch,
									SearchState.NotStarted
								},
								{
									SearchStateTransition.DeleteSearch,
									SearchState.DeletionInProgress
								}
							}
						},
						{
							SearchState.DeletionInProgress,
							new Dictionary<SearchStateTransition, SearchState>()
						},
						{
							SearchState.NotStarted,
							new Dictionary<SearchStateTransition, SearchState>
							{
								{
									SearchStateTransition.StartSearch,
									SearchState.Queued
								},
								{
									SearchStateTransition.ResetSearch,
									SearchState.NotStarted
								},
								{
									SearchStateTransition.DeleteSearch,
									SearchState.DeletionInProgress
								}
							}
						},
						{
							SearchState.Queued,
							new Dictionary<SearchStateTransition, SearchState>
							{
								{
									SearchStateTransition.DeleteSearch,
									SearchState.DeletionInProgress
								},
								{
									SearchStateTransition.StopSearch,
									this.StatisticsOnly ? SearchState.EstimateStopped : SearchState.Stopped
								},
								{
									SearchStateTransition.MoveToNextState,
									this.StatisticsOnly ? SearchState.EstimateInProgress : SearchState.InProgress
								},
								{
									SearchStateTransition.Fail,
									SearchState.Failed
								}
							}
						}
					};
					Interlocked.CompareExchange<Dictionary<SearchState, Dictionary<SearchStateTransition, SearchState>>>(ref this.stateMachine, value, null);
				}
				return this.stateMachine;
			}
		}

		public bool IncludeKeywordStatistics
		{
			get
			{
				return (bool)this[MailboxDiscoverySearchSchema.IncludeKeywordStatistics];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.IncludeKeywordStatistics] = value;
			}
		}

		public int StatisticsStartIndex
		{
			get
			{
				return (int)this[MailboxDiscoverySearchSchema.StatisticsStartIndex];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.StatisticsStartIndex] = value;
			}
		}

		public int TotalKeywords
		{
			get
			{
				return (int)this[MailboxDiscoverySearchSchema.TotalKeywords];
			}
			internal set
			{
				this[MailboxDiscoverySearchSchema.TotalKeywords] = value;
			}
		}

		internal QueryFilter InternalQueryFilter
		{
			get
			{
				if (string.IsNullOrEmpty(this.CalculatedQuery))
				{
					this.UpdateCalculatedQuery();
				}
				bool flag = this.CalculatedQuery == MailboxDiscoverySearch.EmptyQueryReplacement;
				if (this.internalQueryFilter == null && !flag)
				{
					this.internalQueryFilter = MailboxDiscoverySearch.CalculateQueryFilter(this.CalculatedQuery, this.QueryCulture, delegate(Exception ex)
					{
						ExTraceGlobals.StorageTracer.TraceError<string, Exception>(0L, "Failed to parse the query string in the MailboxDiscoverySearch.InternalQueryFilter getter. The query is: '{0}'. Exception: {1}", this.CalculatedQuery, ex);
					});
				}
				return this.internalQueryFilter;
			}
			set
			{
				this.internalQueryFilter = value;
			}
		}

		internal CultureInfo QueryCulture
		{
			get
			{
				if (this.queryCulture == null)
				{
					this.queryCulture = CultureInfo.InvariantCulture;
					this.languageIsInvalid = false;
					if (!string.IsNullOrEmpty(this.Language))
					{
						try
						{
							this.queryCulture = new CultureInfo(this.Language);
						}
						catch (CultureNotFoundException)
						{
							this.languageIsInvalid = true;
							ExTraceGlobals.StorageTracer.TraceError<string>((long)this.GetHashCode(), "Culture info: \"{0}\" returns CultureNotFoundException", this.Language);
						}
					}
				}
				return this.queryCulture;
			}
		}

		internal bool IsFeatureFlighted(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("MailboxDiscoverySearch.IsFeatureFlighted - invalid feature name");
			}
			if (this.flightedFeatures == null)
			{
				return false;
			}
			foreach (string value in this.flightedFeatures)
			{
				if (name.Equals(value))
				{
					return true;
				}
			}
			return false;
		}

		internal void AddFlightedFeature(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("MailboxDiscoverySearch.AddFlightedFeature - invalid feature name");
			}
			if (this.flightedFeatures == null)
			{
				this.flightedFeatures = new List<string>();
			}
			if (!this.IsFeatureFlighted(name))
			{
				this.flightedFeatures.Add(name);
			}
		}

		internal static bool IsInProgressState(SearchState searchState)
		{
			return searchState == SearchState.Queued || searchState == SearchState.InProgress || searchState == SearchState.EstimateInProgress;
		}

		internal static bool IsInDeletionState(SearchState searchState)
		{
			return searchState == SearchState.DeletionInProgress;
		}

		internal static QueryFilter CalculateQueryFilter(string query, CultureInfo queryCulture, Action<Exception> exceptionHandler)
		{
			QueryFilter result;
			try
			{
				result = KqlParser.ParseAndBuildQuery(query, KqlParser.ParseOption.DisablePrefixMatch | KqlParser.ParseOption.AllowShortWildcards | KqlParser.ParseOption.EDiscoveryMode, queryCulture, RescopedAll.Default, null, null);
			}
			catch (ParserException obj)
			{
				exceptionHandler(obj);
				result = null;
			}
			return result;
		}

		internal static IDisposable SetAllowSettingStatus(bool? setStatus)
		{
			return MailboxDiscoverySearch.AllowSettingStatus.SetTestHook(setStatus);
		}

		internal static QueryFilter GetRetentionPeriodFilter(EnhancedTimeSpan period)
		{
			return new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ItemSchema.ReceivedTime, ExDateTime.UtcNow - period);
		}

		internal static string GetKeywordPhrase(QueryFilter filter, string userQuery, ref int position)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(?i)");
			IEnumerable<string> enumerable = filter.Keywords();
			stringBuilder.Append("\\(*");
			int num = 0;
			foreach (string text in enumerable)
			{
				stringBuilder.Append("[\\\"]?");
				stringBuilder.Append(text.TrimEnd(new char[]
				{
					'?'
				}).Replace("*", "\\*"));
				stringBuilder.Append("\\*?\\??[\\\"]?.*?");
				if (++num < enumerable.Count<string>() - 1)
				{
					stringBuilder.Append("\\b");
				}
			}
			stringBuilder.Append("\\)*");
			Regex regex = new Regex(stringBuilder.ToString());
			Match match = regex.Match(userQuery, position);
			string text2;
			if (match.Success)
			{
				position = match.Index + match.Length;
				int num2 = match.Value.Count((char c) => c == '(');
				int num3 = match.Value.Count((char c) => c == ')');
				if (num2 < num3)
				{
					text2 = new string('(', num3 - num2) + match.Value;
				}
				else if (num3 < num2)
				{
					text2 = match.Value + new string(')', num2 - num3);
				}
				else
				{
					text2 = match.Value;
				}
			}
			else
			{
				text2 = filter.Keywords().Aggregate((string i, string j) => i + " " + j);
			}
			if (filter is NotFilter)
			{
				text2 = ServerStrings.NotOperator + " " + filter.PropertyName + text2;
			}
			else
			{
				text2 = filter.PropertyName + text2;
			}
			return text2;
		}

		internal static void AddInPlaceHold(ADRecipient recipient, string holdId, IRecipientSession updateSession)
		{
			if (recipient == null)
			{
				throw new ArgumentException("recipient");
			}
			if (updateSession == null)
			{
				throw new ArgumentException("updateSession");
			}
			if (recipient.InPlaceHolds == null)
			{
				recipient.InPlaceHolds = new MultiValuedProperty<string>();
			}
			if (!recipient.InPlaceHolds.Contains(holdId))
			{
				recipient.InPlaceHolds.Add(holdId);
				if (recipient is ADUser && recipient.InPlaceHolds.Count == 1)
				{
					RecoverableItemsQuotaHelper.IncreaseRecoverableItemsQuotaIfNeeded((ADUser)recipient);
				}
				if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled && recipient.IsSoftDeleted && !recipient.IsInactiveMailbox && recipient.InPlaceHolds.Count == 1)
				{
					((ADUser)recipient).UpdateSoftDeletedStatusForHold(true);
				}
				updateSession.Save(recipient, true);
			}
		}

		internal static void RemoveInPlaceHold(ADRecipient recipient, string holdId, IRecipientSession updateSession)
		{
			if (updateSession == null)
			{
				throw new ArgumentException("updateSession");
			}
			if (recipient != null && recipient.InPlaceHolds != null && recipient.InPlaceHolds.Contains(holdId))
			{
				recipient.InPlaceHolds.Remove(holdId);
				if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled && recipient.IsSoftDeleted && recipient.IsInactiveMailbox && !((ADUser)recipient).IsInLitigationHoldOrInplaceHold)
				{
					((ADUser)recipient).UpdateSoftDeletedStatusForHold(false);
				}
				updateSession.Save(recipient, true);
			}
		}

		private static bool AreCollectionsEqual<T>(ICollection<T> col1, ICollection<T> col2)
		{
			if (col1 == null && col2 == null)
			{
				return true;
			}
			if (col1 != null != (col2 != null))
			{
				return false;
			}
			if (col1.Count != col2.Count)
			{
				return false;
			}
			foreach (T item in col1)
			{
				if (!col2.Contains(item))
				{
					return false;
				}
			}
			return true;
		}

		private static void ReportJobsProgress(int totalPossibleJobs, ref int totalProcessedJobs, int jobsDone, Action<int> reportProgress)
		{
			if (reportProgress != null)
			{
				int num = totalProcessedJobs * 100 / totalPossibleJobs;
				totalProcessedJobs += jobsDone;
				int num2 = totalProcessedJobs * 100 / totalPossibleJobs;
				if (num != num2)
				{
					reportProgress(num2);
				}
			}
		}

		private static bool CanSetStatus()
		{
			bool result = false;
			if (MailboxDiscoverySearch.AllowSettingStatus.Value != null && MailboxDiscoverySearch.AllowSettingStatus.Value == true)
			{
				result = true;
			}
			return result;
		}

		internal void UpdateState(SearchStateTransition action)
		{
			ExAssert.RetailAssert(this.StateMachine.ContainsKey(this.Status), "The state machine does not contain the current state transitions");
			Dictionary<SearchStateTransition, SearchState> dictionary = this.StateMachine[this.Status];
			string formatString = string.Format("The action {0} is not recognized by the state machine for the state {1}", action.ToString(), this.Status.ToString());
			ExAssert.RetailAssert(dictionary.ContainsKey(action), formatString);
			if (dictionary.ContainsKey(action))
			{
				this.Status = dictionary[action];
			}
		}

		internal void SetStatus(SearchState status)
		{
			if (!MailboxDiscoverySearch.CanSetStatus())
			{
				throw new Exception("Can set status only in tests. Please use UpdateState");
			}
			this.Status = status;
		}

		internal void SynchronizeHoldSettings(DiscoverySearchDataProvider dataProvider, IRecipientSession recipientSession, bool holdEnabled)
		{
			this.SynchronizeHoldSettings(dataProvider, recipientSession, holdEnabled, null);
		}

		internal LocalizedString SynchronizeHoldSettings(DiscoverySearchDataProvider dataProvider, IRecipientSession recipientSession, bool holdEnabled, Action<int> reportProgress)
		{
			if (!"b5d6efcd-1aee-42b9-b168-6fef285fe613".Equals(this.ManagedByOrganization ?? string.Empty, StringComparison.OrdinalIgnoreCase))
			{
				bool includeSoftDeletedObjects = recipientSession.SessionSettings.IncludeSoftDeletedObjects;
				bool includeInactiveMailbox = recipientSession.SessionSettings.IncludeInactiveMailbox;
				try
				{
					if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
					{
						recipientSession.SessionSettings.IncludeSoftDeletedObjects = true;
					}
					else
					{
						recipientSession.SessionSettings.IncludeSoftDeletedObjects = false;
						recipientSession.SessionSettings.IncludeInactiveMailbox = false;
					}
					Dictionary<string, ADObjectId> dictionary = this.FetchCurrentOnHoldMailboxes(dataProvider, recipientSession);
					int totalPossibleJobs = dictionary.Count + this.Sources.Count + 2;
					int num = 0;
					MailboxDiscoverySearch.ReportJobsProgress(totalPossibleJobs, ref num, 1, reportProgress);
					this.InPlaceHoldErrors = new MultiValuedProperty<string>();
					this.FailedToHoldMailboxes = new MultiValuedProperty<string>();
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.PartiallyConsistent, recipientSession.SessionSettings, 1455, "SynchronizeHoldSettings", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Search\\DiscoverySearch\\MailboxDiscoverySearch.cs");
					if (holdEnabled)
					{
						List<string> list = this.Sources.Intersect(dictionary.Keys).ToList<string>();
						string[] array = this.Sources.Except(dictionary.Keys).ToArray<string>();
						foreach (string key in list)
						{
							dictionary.Remove(key);
							MailboxDiscoverySearch.ReportJobsProgress(totalPossibleJobs, ref num, 2, reportProgress);
						}
						if (array.Length > 0)
						{
							Result<ADRawEntry>[] array2 = recipientSession.FindByLegacyExchangeDNs(array, new PropertyDefinition[]
							{
								ADRecipientSchema.LegacyExchangeDN,
								ADObjectSchema.Id
							});
							if (array2 != null && array2.Length > 0)
							{
								foreach (Result<ADRawEntry> result in array2)
								{
									string text = (string)result.Data[ADRecipientSchema.LegacyExchangeDN];
									ADObjectId entryId = (ADObjectId)result.Data[ADObjectSchema.Id];
									try
									{
										ADRecipient adrecipient = tenantOrRootOrgRecipientSession.Read(entryId);
										if (adrecipient == null)
										{
											throw new LocalizedException(ServerStrings.ErrorADUserFoundByReadOnlyButNotWrite(text));
										}
										MailboxDiscoverySearch.AddInPlaceHold(adrecipient, this.InPlaceHoldIdentity, tenantOrRootOrgRecipientSession);
									}
									catch (LocalizedException ex)
									{
										this.InPlaceHoldErrors.Add(ex.Message);
										this.FailedToHoldMailboxes.Add(text);
										ExTraceGlobals.StorageTracer.TraceError<string, LocalizedException>(0L, "Failed to place hold on mailbox '{0}'. Exception: {1}", text, ex);
									}
									MailboxDiscoverySearch.ReportJobsProgress(totalPossibleJobs, ref num, 1, reportProgress);
								}
							}
							else
							{
								foreach (string text2 in array)
								{
									this.InPlaceHoldErrors.Add(ServerStrings.ADUserNotFoundId(text2));
									this.FailedToHoldMailboxes.Add(text2);
									ExTraceGlobals.StorageTracer.TraceError(0L, string.Format("Failed to find mailbox '{0}' to be placed on hold.", text2));
									MailboxDiscoverySearch.ReportJobsProgress(totalPossibleJobs, ref num, 1, reportProgress);
								}
							}
						}
					}
					foreach (KeyValuePair<string, ADObjectId> keyValuePair in dictionary)
					{
						string key2 = keyValuePair.Key;
						try
						{
							ADRecipient recipient = tenantOrRootOrgRecipientSession.Read(keyValuePair.Value);
							MailboxDiscoverySearch.RemoveInPlaceHold(recipient, this.InPlaceHoldIdentity, tenantOrRootOrgRecipientSession);
						}
						catch (LocalizedException ex2)
						{
							this.InPlaceHoldErrors.Add(ex2.Message);
							this.FailedToHoldMailboxes.Add(key2);
							ExTraceGlobals.StorageTracer.TraceError<string, LocalizedException>(0L, "Failed to remove hold from mailbox '{0}'. Exception: {1}", key2, ex2);
						}
						MailboxDiscoverySearch.ReportJobsProgress(totalPossibleJobs, ref num, 1, reportProgress);
					}
					dataProvider.Save(this);
					MailboxDiscoverySearch.ReportJobsProgress(totalPossibleJobs, ref num, 1, reportProgress);
					SearchEventLogger.Instance.LogInPlaceHoldSettingsSynchronizedEvent(this);
				}
				finally
				{
					recipientSession.SessionSettings.IncludeSoftDeletedObjects = includeSoftDeletedObjects;
					recipientSession.SessionSettings.IncludeInactiveMailbox = includeInactiveMailbox;
				}
				return LocalizedString.Empty;
			}
			return ServerStrings.ErrorCannotSyncHoldObjectManagedByOtherOrg(base.Name, dataProvider.OrganizationId.ToString(), this.ManagedByOrganization);
		}

		internal bool ShouldWarnForInactiveOnHold(DiscoverySearchDataProvider dataProvider, IRecipientSession recipientSession, bool holdEnabled)
		{
			if (!"b5d6efcd-1aee-42b9-b168-6fef285fe613".Equals(this.ManagedByOrganization ?? string.Empty, StringComparison.OrdinalIgnoreCase))
			{
				try
				{
					if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
					{
						recipientSession.SessionSettings.IncludeSoftDeletedObjects = true;
					}
					Dictionary<string, ADObjectId> dictionary = this.FetchCurrentOnHoldMailboxes(dataProvider, recipientSession);
					if (holdEnabled)
					{
						List<string> list = this.Sources.Intersect(dictionary.Keys).ToList<string>();
						foreach (string key in list)
						{
							dictionary.Remove(key);
						}
					}
					foreach (KeyValuePair<string, ADObjectId> keyValuePair in dictionary)
					{
						string key2 = keyValuePair.Key;
						try
						{
							ADRecipient adrecipient = recipientSession.Read(keyValuePair.Value);
							if (adrecipient != null && adrecipient.InPlaceHolds != null && adrecipient.InPlaceHolds.Contains(this.InPlaceHoldIdentity) && adrecipient.IsInactiveMailbox)
							{
								return true;
							}
						}
						catch (LocalizedException ex)
						{
							this.InPlaceHoldErrors.Add(ex.Message);
							this.FailedToHoldMailboxes.Add(key2);
							ExTraceGlobals.StorageTracer.TraceError<string, LocalizedException>(0L, "Failed to read hold information for '{0}'. Exception: {1}", key2, ex);
						}
					}
					return false;
				}
				catch (LocalizedException ex2)
				{
					this.InPlaceHoldErrors.Add(ex2.Message);
					ExTraceGlobals.StorageTracer.TraceError<EwsStoreObjectId, LocalizedException>(0L, "Failed to see if we should warn for this discovery search:'{0}'. Exception: {1}", base.Identity, ex2);
				}
				return false;
			}
			return false;
		}

		internal Dictionary<string, ADObjectId> FetchCurrentOnHoldMailboxes(DiscoverySearchDataProvider dataProvider, IRecipientSession recipientSession)
		{
			ComparisonFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.InPlaceHolds, this.InPlaceHoldIdentity);
			IEnumerable<ADRawEntry> enumerable = recipientSession.FindPagedADRawEntry(null, QueryScope.SubTree, filter, null, 0, new PropertyDefinition[]
			{
				ADRecipientSchema.LegacyExchangeDN,
				ADObjectSchema.Id
			});
			Dictionary<string, ADObjectId> dictionary = new Dictionary<string, ADObjectId>();
			if (enumerable != null)
			{
				foreach (ADRawEntry adrawEntry in enumerable)
				{
					string key = (string)adrawEntry[ADRecipientSchema.LegacyExchangeDN];
					if (!dictionary.ContainsKey(key))
					{
						dictionary.Add(key, (ADObjectId)adrawEntry[ADObjectSchema.Id]);
					}
				}
			}
			return dictionary;
		}

		internal void UpdateCalculatedQuery()
		{
			string query = this.Query;
			string additionalFilter = this.GetAdditionalFilter();
			if (string.IsNullOrEmpty(additionalFilter))
			{
				this.CalculatedQuery = (string.IsNullOrEmpty(query) ? MailboxDiscoverySearch.EmptyQueryReplacement : query);
				return;
			}
			string str = string.Empty;
			if (!string.IsNullOrEmpty(query))
			{
				if (query.IndexOf(" OR ", StringComparison.OrdinalIgnoreCase) != -1)
				{
					str = string.Format("({0}) ", query);
				}
				else
				{
					str = string.Format("{0} ", query);
				}
			}
			this.CalculatedQuery = str + additionalFilter;
		}

		internal void UpdateKeywordsQuery(List<ValidationError> errors)
		{
			this.TotalKeywords = 0;
			this.UserKeywords = new MultiValuedProperty<string>();
			this.KeywordsQuery = string.Empty;
			if (!string.IsNullOrEmpty(this.Query) && this.StatisticsStartIndex > 0)
			{
				try
				{
					QueryFilter queryFilter = KqlParser.ParseAndBuildQuery(this.Query, KqlParser.ParseOption.ImplicitOr | KqlParser.ParseOption.UseCiKeywordOnly | KqlParser.ParseOption.DisablePrefixMatch | KqlParser.ParseOption.QueryPreserving | KqlParser.ParseOption.AllowShortWildcards, this.QueryCulture, RescopedAll.Default, null, null);
					ICollection<QueryFilter> collection;
					if (queryFilter.GetType() == typeof(OrFilter))
					{
						collection = AqsParser.FlattenQueryFilter(queryFilter);
					}
					else
					{
						collection = new List<QueryFilter>(1);
						collection.Add(queryFilter);
					}
					if (collection != null && collection.Count > 0)
					{
						this.TotalKeywords = collection.Count;
						if (this.TotalKeywords < this.StatisticsStartIndex)
						{
							errors.Add(new PropertyValidationError(ServerStrings.ErrorStatisticsStartIndexIsOutOfBound(this.StatisticsStartIndex, this.TotalKeywords), MailboxDiscoverySearchSchema.StatisticsStartIndex, this.StatisticsStartIndex));
						}
						else
						{
							string additionalFilter = this.GetAdditionalFilter();
							if (this.TotalKeywords > 1)
							{
								StringBuilder stringBuilder = new StringBuilder();
								int num = 0;
								int num2 = this.StatisticsStartIndex - 1;
								int num3 = 0;
								while (num2 < this.TotalKeywords && num3 < 25)
								{
									QueryFilter filter = collection.ElementAt(num2);
									string text = MailboxDiscoverySearch.GetKeywordPhrase(filter, this.Query, ref num);
									if (!this.UserKeywords.Contains(text))
									{
										this.UserKeywords.Add(text);
									}
									if (!string.IsNullOrEmpty(additionalFilter))
									{
										text = string.Format("({0} AND {1})", text, additionalFilter);
									}
									stringBuilder.Append(text);
									stringBuilder.Append(" OR ");
									num2++;
									num3++;
								}
								this.KeywordsQuery = stringBuilder.ToString().Substring(0, stringBuilder.ToString().Length - 4);
							}
							else if (this.TotalKeywords == 1)
							{
								this.UserKeywords.Add(this.Query);
								string text2 = this.Query;
								if (!string.IsNullOrEmpty(additionalFilter))
								{
									text2 = string.Format("({0} AND {1})", text2, additionalFilter);
								}
								this.KeywordsQuery = text2;
							}
						}
					}
				}
				catch (ParserException ex)
				{
					errors.Add(new PropertyValidationError(ServerStrings.ErrorInvalidQuery(base.Name, ex.Message), MailboxDiscoverySearchSchema.Query, this.Query));
				}
			}
		}

		internal void UpdateSearchStats(DiscoverySearchStats stats)
		{
			stats.EstimatedItems = this.ResultItemCountEstimate - this.ResultUnsearchableItemCountEstimate;
			stats.TotalItemsCopied = this.ResultItemCountCopied;
			this.SearchStatistics = new MultiValuedProperty<DiscoverySearchStats>();
			this.SearchStatistics.Add(stats);
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (!this.ItemHoldPeriod.IsUnlimited && this.ItemHoldPeriod.Value <= EnhancedTimeSpan.Zero)
			{
				errors.Add(new PropertyValidationError(ServerStrings.ErrorInvalidItemHoldPeriod(base.Name), MailboxDiscoverySearchSchema.ItemHoldPeriod, this.ItemHoldPeriod));
			}
			if (string.IsNullOrEmpty(this.CalculatedQuery))
			{
				this.UpdateCalculatedQuery();
			}
			if (this.CalculatedQuery.Length > 10240)
			{
				errors.Add(new PropertyValidationError(ServerStrings.ErrorInvalidQueryTooLong(base.Name), MailboxDiscoverySearchSchema.CalculatedQuery, this.CalculatedQuery));
			}
			bool flag = this.CalculatedQuery == MailboxDiscoverySearch.EmptyQueryReplacement;
			if (this.internalQueryFilter == null && !flag)
			{
				this.internalQueryFilter = MailboxDiscoverySearch.CalculateQueryFilter(this.CalculatedQuery, this.QueryCulture, delegate(Exception ex)
				{
					errors.Add(new PropertyValidationError(ServerStrings.ErrorInvalidQuery(this.Name, ex.Message), MailboxDiscoverySearchSchema.CalculatedQuery, this.CalculatedQuery));
				});
			}
			this.UpdateKeywordsQuery(errors);
			if (base.ObjectState == ObjectState.New)
			{
				if (string.IsNullOrEmpty(this.InPlaceHoldIdentity))
				{
					this.InPlaceHoldIdentity = Guid.NewGuid().ToString("N");
				}
			}
			else if (base.IsChanged(MailboxDiscoverySearchSchema.InPlaceHoldIdentity))
			{
				errors.Add(new PropertyValidationError(ServerStrings.ErrorInPlaceHoldIdentityChanged(base.Name), MailboxDiscoverySearchSchema.InPlaceHoldIdentity, this.InPlaceHoldIdentity));
			}
			else if (string.IsNullOrEmpty(this.InPlaceHoldIdentity))
			{
				errors.Add(new PropertyValidationError(ServerStrings.ErrorInvalidInPlaceHoldIdentity(base.Name), MailboxDiscoverySearchSchema.InPlaceHoldIdentity, this.InPlaceHoldIdentity));
			}
			if (this.StartDate != null && this.EndDate != null && this.StartDate > this.EndDate)
			{
				errors.Add(new PropertyValidationError(ServerStrings.InvalidateDateRange, MailboxDiscoverySearchSchema.StartDate, this.StartDate));
			}
			KindKeyword[] second = new KindKeyword[]
			{
				KindKeyword.faxes,
				KindKeyword.voicemail,
				KindKeyword.rssfeeds,
				KindKeyword.posts
			};
			if (this.MessageTypes.Intersect(second).Count<KindKeyword>() > 0)
			{
				errors.Add(new PropertyValidationError(ServerStrings.UnsupportedKindKeywords, MailboxDiscoverySearchSchema.MessageTypes, this.MessageTypes));
			}
			if (this.StatisticsStartIndex <= 0)
			{
				errors.Add(new PropertyValidationError(ServerStrings.ErrorInvalidStatisticsStartIndex(base.Name), MailboxDiscoverySearchSchema.StatisticsStartIndex, this.StatisticsStartIndex));
			}
			if (this.languageIsInvalid)
			{
				errors.Add(new PropertyValidationError(ServerStrings.ErrorInvalidQueryLanguage(base.Name, this.Language), MailboxDiscoverySearchSchema.Language, this.Language));
			}
		}

		private string BuildAddressesQuery(MultiValuedProperty<string> addresses, string[] addressTypes)
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			if (addresses != null)
			{
				int num = 0;
				foreach (string value in addressTypes)
				{
					foreach (string text in addresses)
					{
						stringBuilder.Append(value);
						stringBuilder.Append("\"");
						stringBuilder.Append(text.Replace("\\", "\\\\"));
						stringBuilder.Append("*\"");
						num = stringBuilder.Length;
						stringBuilder.Append(" OR ");
					}
				}
				if (num > 0)
				{
					stringBuilder.Length = num;
				}
			}
			string text2 = stringBuilder.ToString();
			if (text2.Contains(" OR "))
			{
				text2 = string.Format("({0})", text2);
			}
			return text2;
		}

		private string GetAdditionalFilter()
		{
			MultiValuedProperty<string> senders = this.Senders;
			MultiValuedProperty<string> recipients = this.Recipients;
			MultiValuedProperty<KindKeyword> messageTypes = this.MessageTypes;
			ExDateTime? startDate = this.StartDate;
			ExDateTime? endDate = this.EndDate;
			if ((senders == null || senders.Count == 0) && (recipients == null || recipients.Count == 0) && (messageTypes == null || messageTypes.Count == 0) && (startDate == null || startDate == null) && (endDate == null || endDate == null))
			{
				return null;
			}
			int num = 0;
			string text = string.Empty;
			if (startDate != null && startDate != null)
			{
				text = string.Format("received>=\"{0}\"", startDate.Value.ToUtc().ToString(this.QueryCulture));
			}
			if (endDate != null && endDate != null)
			{
				string text2 = string.Format("received<=\"{0}\"", SearchObject.RoundEndDate(endDate).Value.ToUtc().ToString(this.QueryCulture));
				if (!string.IsNullOrEmpty(text))
				{
					text = string.Format("({0} AND {1})", text, text2);
				}
				else
				{
					text = text2;
				}
			}
			StringBuilder stringBuilder = new StringBuilder(64);
			if (messageTypes != null)
			{
				num = 0;
				foreach (KindKeyword kindKeyword in messageTypes)
				{
					stringBuilder.Append(string.Format("kind:{0}", kindKeyword.ToString()));
					num = stringBuilder.Length;
					stringBuilder.Append(" OR ");
				}
				if (num > 0)
				{
					stringBuilder.Length = num;
				}
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			string text3 = this.BuildAddressesQuery(senders, new string[]
			{
				"from:"
			});
			string text4 = this.BuildAddressesQuery(recipients, new string[]
			{
				"to:",
				"cc:",
				"bcc:"
			});
			string text5 = stringBuilder.ToString();
			if (text5.Contains(" OR "))
			{
				text5 = string.Format("({0})", text5);
			}
			if (!string.IsNullOrEmpty(text5))
			{
				stringBuilder2.Append(text5);
			}
			string value = string.Empty;
			if (!string.IsNullOrEmpty(text3) && !string.IsNullOrEmpty(text4))
			{
				value = string.Format("({0} OR {1}) ", text3, text4);
			}
			else if (!string.IsNullOrEmpty(text3))
			{
				value = text3;
			}
			else if (!string.IsNullOrEmpty(text4))
			{
				value = text4;
			}
			if (stringBuilder2.Length > 0 && !string.IsNullOrEmpty(value))
			{
				stringBuilder2.Append(" AND ");
			}
			if (!string.IsNullOrEmpty(value))
			{
				stringBuilder2.Append(value);
			}
			if (stringBuilder2.Length > 0 && !string.IsNullOrEmpty(text))
			{
				stringBuilder2.Append(" AND ");
			}
			if (!string.IsNullOrEmpty(text))
			{
				stringBuilder2.Append(text);
			}
			return stringBuilder2.ToString();
		}

		public const string ManagedByRemoteExchangeOrganization = "b5d6efcd-1aee-42b9-b168-6fef285fe613";

		internal const string FlightFeatureSearchStats = "SearchStatsFlighted";

		internal const string FlightFeatureSearchScale = "SearchScaleFlighted";

		internal const string FlightFeaturePublicFolderSearch = "PublicFolderSearchFlighted";

		internal const string FlightFeatureDocIdHint = "DocIdHintFlighted";

		internal const string FlightFeatureUrlRebind = "UrlRebindFlighted";

		internal const string FlightFeatureExcludeFolders = "ExcludeFoldersFlighted";

		private const string OrSeparator = " OR ";

		private const int MaxQueryLength = 10240;

		private const int MaxKeywordCountPerStatisticsSearch = 25;

		internal static string EmptyQueryReplacement = "size>=0";

		private static readonly Hookable<bool?> AllowSettingStatus = Hookable<bool?>.Create(true, null);

		private static ObjectSchema schema = new MailboxDiscoverySearchSchema();

		private QueryFilter internalQueryFilter;

		private Dictionary<SearchState, Dictionary<SearchStateTransition, SearchState>> stateMachine;

		private CultureInfo queryCulture;

		private bool languageIsInvalid;

		private List<string> flightedFeatures;
	}
}
