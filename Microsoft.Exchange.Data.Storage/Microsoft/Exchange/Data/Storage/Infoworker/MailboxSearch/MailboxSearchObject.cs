using System;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[Serializable]
	public class MailboxSearchObject : IConfigurable
	{
		public string Identity
		{
			get
			{
				if (this.discoverySearch != null)
				{
					return this.discoverySearch.Identity.ToString();
				}
				if (this.searchObject != null && this.searchObject.Identity != null)
				{
					return this.searchObject.Identity.ToString();
				}
				return string.Empty;
			}
		}

		public string Name
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchObject.Name;
				}
				return this.discoverySearch.Name;
			}
		}

		public string CreatedBy
		{
			get
			{
				if (this.discoverySearch != null)
				{
					return this.discoverySearch.CreatedBy;
				}
				if (!string.IsNullOrEmpty(this.searchObject.CreatedByEx))
				{
					return this.searchObject.CreatedByEx;
				}
				if (this.searchObject.CreatedBy != null)
				{
					return this.searchObject.CreatedBy.ToString();
				}
				return string.Empty;
			}
		}

		public MultiValuedProperty<ADObjectId> SourceMailboxes
		{
			get
			{
				if (this.discoverySearch != null)
				{
					return this.sourceMailboxes;
				}
				return this.searchObject.SourceMailboxes;
			}
		}

		public MultiValuedProperty<string> Sources
		{
			get
			{
				if (this.discoverySearch != null)
				{
					return this.discoverySearch.Sources;
				}
				return null;
			}
		}

		public bool AllSourceMailboxes
		{
			get
			{
				return this.discoverySearch != null && this.discoverySearch.AllSourceMailboxes;
			}
		}

		public MultiValuedProperty<string> PublicFolderSources
		{
			get
			{
				if (this.discoverySearch != null)
				{
					return this.discoverySearch.PublicFolderSources;
				}
				return null;
			}
		}

		public bool AllPublicFolderSources
		{
			get
			{
				return this.discoverySearch != null && this.discoverySearch.AllPublicFolderSources;
			}
		}

		public MultiValuedProperty<DiscoverySearchStats> SearchStatistics
		{
			get
			{
				if (this.discoverySearch != null)
				{
					return this.discoverySearch.SearchStatistics;
				}
				return null;
			}
		}

		public SearchObjectVersion Version
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return SearchObjectVersion.Original;
				}
				return this.discoverySearch.Version;
			}
		}

		public ADObjectId TargetMailbox
		{
			get
			{
				if (this.discoverySearch != null)
				{
					return this.targetMailbox;
				}
				return this.searchObject.TargetMailbox;
			}
		}

		public string Target
		{
			get
			{
				if (this.discoverySearch != null)
				{
					return this.discoverySearch.Target;
				}
				return null;
			}
		}

		public string SearchQuery
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchObject.SearchQuery;
				}
				return this.discoverySearch.Query;
			}
		}

		public CultureInfo Language
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchObject.Language;
				}
				return new CultureInfo(this.discoverySearch.Language);
			}
		}

		public MultiValuedProperty<string> Senders
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchObject.Senders;
				}
				return this.discoverySearch.Senders;
			}
		}

		public MultiValuedProperty<string> Recipients
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchObject.Recipients;
				}
				return this.discoverySearch.Recipients;
			}
		}

		public ExDateTime? StartDate
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchObject.StartDate;
				}
				return this.discoverySearch.StartDate;
			}
		}

		public ExDateTime? EndDate
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchObject.EndDate;
				}
				return this.discoverySearch.EndDate;
			}
		}

		public MultiValuedProperty<KindKeyword> MessageTypes
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchObject.MessageTypes;
				}
				return this.discoverySearch.MessageTypes;
			}
		}

		public bool IncludeUnsearchableItems
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchObject.IncludeUnsearchableItems;
				}
				return this.discoverySearch.IncludeUnsearchableItems;
			}
		}

		public bool EstimateOnly
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchObject.EstimateOnly;
				}
				return this.discoverySearch.StatisticsOnly;
			}
		}

		public bool ExcludeDuplicateMessages
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchObject.ExcludeDuplicateMessages;
				}
				return this.discoverySearch.ExcludeDuplicateMessages;
			}
		}

		public bool Resume
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchObject.Resume;
				}
				return this.discoverySearch.Resume;
			}
		}

		public bool IncludeKeywordStatistics
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchObject.IncludeKeywordStatistics;
				}
				return this.discoverySearch.IncludeKeywordStatistics;
			}
		}

		public bool KeywordStatisticsDisabled
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchObject.KeywordStatisticsDisabled;
				}
				return this.discoverySearch.KeywordStatisticsDisabled;
			}
		}

		public bool PreviewDisabled
		{
			get
			{
				return this.discoverySearch != null && this.discoverySearch.PreviewDisabled;
			}
		}

		public MultiValuedProperty<string> Information
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchObject.Information;
				}
				return this.discoverySearch.Information;
			}
		}

		public int StatisticsStartIndex
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return 1;
				}
				return this.discoverySearch.StatisticsStartIndex;
			}
		}

		public int TotalKeywords
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return 0;
				}
				return this.discoverySearch.TotalKeywords;
			}
		}

		public LoggingLevel LogLevel
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchObject.LogLevel;
				}
				return this.discoverySearch.LogLevel;
			}
		}

		public MultiValuedProperty<ADObjectId> StatusMailRecipients
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchObject.StatusMailRecipients;
				}
				return this.discoverySearch.StatusMailRecipients;
			}
		}

		public SearchState Status
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchStatus.Status;
				}
				return this.discoverySearch.Status;
			}
		}

		public string LastRunBy
		{
			get
			{
				if (this.discoverySearch != null)
				{
					return this.discoverySearch.LastModifiedBy;
				}
				if (!string.IsNullOrEmpty(this.searchStatus.LastRunByEx))
				{
					return this.searchStatus.LastRunByEx;
				}
				if (this.searchStatus.LastRunBy != null)
				{
					return this.searchStatus.LastRunBy.ToString();
				}
				return string.Empty;
			}
		}

		public ExDateTime? LastStartTime
		{
			get
			{
				ExDateTime? result = (this.searchStatus != null) ? this.searchStatus.LastStartTime : null;
				if (this.discoverySearch != null && this.discoverySearch.LastStartTime != default(DateTime))
				{
					result = new ExDateTime?((ExDateTime)this.discoverySearch.LastStartTime);
				}
				return result;
			}
		}

		public ExDateTime? LastEndTime
		{
			get
			{
				ExDateTime? result = (this.searchStatus != null) ? this.searchStatus.LastEndTime : null;
				if (this.discoverySearch != null && this.discoverySearch.LastEndTime != default(DateTime))
				{
					result = new ExDateTime?((ExDateTime)this.discoverySearch.LastEndTime);
				}
				return result;
			}
		}

		public int NumberMailboxesToSearch
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchStatus.NumberMailboxesToSearch;
				}
				return this.discoverySearch.NumberOfMailboxes;
			}
		}

		public int PercentComplete
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchStatus.PercentComplete;
				}
				return this.discoverySearch.PercentComplete;
			}
		}

		public long ResultNumber
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchStatus.ResultNumber;
				}
				return this.discoverySearch.ResultItemCountCopied;
			}
		}

		public long ResultNumberEstimate
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchStatus.ResultNumberEstimate;
				}
				return this.discoverySearch.ResultItemCountEstimate;
			}
		}

		public ByteQuantifiedSize ResultSize
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchStatus.ResultSize;
				}
				return new ByteQuantifiedSize((ulong)this.discoverySearch.ResultSizeCopied);
			}
		}

		public ByteQuantifiedSize ResultSizeEstimate
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchStatus.ResultSizeEstimate;
				}
				return new ByteQuantifiedSize((ulong)this.discoverySearch.ResultSizeEstimate);
			}
		}

		public ByteQuantifiedSize ResultSizeCopied
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchStatus.ResultSizeCopied;
				}
				return new ByteQuantifiedSize((ulong)this.discoverySearch.ResultSizeCopied);
			}
		}

		public string ResultsLink
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchStatus.ResultsLink;
				}
				return this.discoverySearch.ResultsLink;
			}
		}

		public string PreviewResultsLink
		{
			get
			{
				if (this.discoverySearch != null && this.Status != SearchState.NotStarted && this.Status != SearchState.EstimateFailed && this.Status != SearchState.Failed)
				{
					return this.discoverySearch.PreviewResultsLink;
				}
				return null;
			}
		}

		public MultiValuedProperty<string> Errors
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchStatus.Errors;
				}
				return this.discoverySearch.Errors;
			}
		}

		public bool InPlaceHoldEnabled
		{
			get
			{
				return this.discoverySearch != null && this.discoverySearch.InPlaceHoldEnabled;
			}
		}

		public Unlimited<EnhancedTimeSpan> ItemHoldPeriod
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return Unlimited<EnhancedTimeSpan>.UnlimitedValue;
				}
				return this.discoverySearch.ItemHoldPeriod;
			}
		}

		public string InPlaceHoldIdentity
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return string.Empty;
				}
				return this.discoverySearch.InPlaceHoldIdentity;
			}
		}

		public string ManagedByOrganization
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return string.Empty;
				}
				if (!(this.discoverySearch.ManagedByOrganization == "b5d6efcd-1aee-42b9-b168-6fef285fe613"))
				{
					return this.discoverySearch.ManagedByOrganization;
				}
				return ServerStrings.ManagedByRemoteExchangeOrganization;
			}
		}

		public MultiValuedProperty<string> FailedToHoldMailboxes
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return null;
				}
				return this.discoverySearch.FailedToHoldMailboxes;
			}
		}

		public MultiValuedProperty<string> InPlaceHoldErrors
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return null;
				}
				return this.discoverySearch.InPlaceHoldErrors;
			}
		}

		public string Description
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return ServerStrings.LegacyMailboxSearchDescription;
				}
				return this.discoverySearch.Description;
			}
		}

		public ExDateTime? LastModifiedTime
		{
			get
			{
				if (this.discoverySearch != null)
				{
					DateTime lastModifiedTime = this.discoverySearch.LastModifiedTime;
					return new ExDateTime?((ExDateTime)this.discoverySearch.LastModifiedTime.ToUniversalTime());
				}
				if (this.searchStatus != null && this.searchStatus.LastStartTime != null)
				{
					return this.searchStatus.LastStartTime;
				}
				return null;
			}
		}

		public MultiValuedProperty<KeywordHit> KeywordHits
		{
			get
			{
				if (this.discoverySearch != null)
				{
					return new MultiValuedProperty<KeywordHit>((from hit in this.discoverySearch.KeywordHits
					where hit.Phrase != "652beee2-75f7-4ca0-8a02-0698a3919cb9"
					select hit).ToArray<KeywordHit>());
				}
				return new MultiValuedProperty<KeywordHit>((from hit in this.searchStatus.KeywordHits
				where hit.Phrase != "652beee2-75f7-4ca0-8a02-0698a3919cb9"
				select hit).ToArray<KeywordHit>());
			}
		}

		internal MultiValuedProperty<KeywordHit> AllKeywordHits
		{
			get
			{
				if (this.discoverySearch != null)
				{
					return new MultiValuedProperty<KeywordHit>(this.discoverySearch.KeywordHits);
				}
				return this.searchStatus.KeywordHits;
			}
		}

		public MailboxSearchObject() : this(new SearchObject(), new SearchStatus())
		{
		}

		internal MailboxSearchObject(SearchObject searchObject) : this(searchObject, new SearchStatus())
		{
		}

		internal MailboxSearchObject(SearchObject searchObject, SearchStatus searchStatus)
		{
			if (searchObject == null)
			{
				throw new ArgumentNullException("searchObject");
			}
			if (searchStatus == null)
			{
				throw new ArgumentException("searchStatus");
			}
			this.searchObject = searchObject;
			this.searchStatus = searchStatus;
		}

		internal MailboxSearchObject(MailboxDiscoverySearch discoverySearch, OrganizationId orgId)
		{
			if (discoverySearch == null)
			{
				throw new ArgumentNullException("discoverySearch");
			}
			this.discoverySearch = discoverySearch;
			if (orgId != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), orgId, null, false);
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.PartiallyConsistent, sessionSettings, 791, ".ctor", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Search\\MailboxSearch\\MailboxSearchObject.cs");
				if (this.discoverySearch.Sources != null && this.discoverySearch.Sources.Count > 0)
				{
					if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
					{
						tenantOrRootOrgRecipientSession.SessionSettings.IncludeInactiveMailbox = true;
					}
					Result<ADRawEntry>[] array = tenantOrRootOrgRecipientSession.FindByLegacyExchangeDNs(this.discoverySearch.Sources.ToArray(), null);
					if (array != null && array.Length > 0)
					{
						this.sourceMailboxes = new MultiValuedProperty<ADObjectId>();
						for (int i = 0; i < array.Length; i++)
						{
							if (array[i].Data != null)
							{
								this.sourceMailboxes.Add(array[i].Data.Id);
							}
						}
					}
				}
				if (!string.IsNullOrEmpty(this.discoverySearch.Target))
				{
					tenantOrRootOrgRecipientSession.SessionSettings.IncludeInactiveMailbox = false;
					ADRawEntry adrawEntry = tenantOrRootOrgRecipientSession.FindByLegacyExchangeDN(this.discoverySearch.Target);
					if (adrawEntry != null)
					{
						this.targetMailbox = adrawEntry.Id;
					}
				}
			}
		}

		ObjectId IConfigurable.Identity
		{
			get
			{
				if (this.discoverySearch == null)
				{
					return this.searchObject.Identity;
				}
				return this.discoverySearch.Identity;
			}
		}

		ValidationError[] IConfigurable.Validate()
		{
			return Array<ValidationError>.Empty;
		}

		bool IConfigurable.IsValid
		{
			get
			{
				return true;
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				return ObjectState.Unchanged;
			}
		}

		void IConfigurable.ResetChangeTracking()
		{
			throw new NotImplementedException();
		}

		void IConfigurable.CopyChangesFrom(IConfigurable changedObject)
		{
			throw new NotSupportedException();
		}

		private SearchObject searchObject;

		private SearchStatus searchStatus;

		private MailboxDiscoverySearch discoverySearch;

		private MultiValuedProperty<ADObjectId> sourceMailboxes;

		private ADObjectId targetMailbox;
	}
}
