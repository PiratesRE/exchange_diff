using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.EDiscovery.Export;
using Microsoft.Exchange.EDiscovery.Export.EwsProxy;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch
{
	internal class MailboxSearchTask : IMailboxSearchTask, IDisposable
	{
		public MailboxSearchTask(IEwsClient ewsClient, string keywordStatisticsQuery, MultiValuedProperty<string> userKeywords, IRecipientSession recipientSession, IExportContext exportContext, string executingUserPrimarySmtpAddress, int previewMaxMailboxes, OrganizationId orgId) : this(ewsClient, keywordStatisticsQuery, userKeywords, recipientSession, exportContext, executingUserPrimarySmtpAddress, previewMaxMailboxes, false, null, orgId)
		{
		}

		public MailboxSearchTask(IEwsClient ewsClient, string keywordStatisticsQuery, MultiValuedProperty<string> userKeywords, IRecipientSession recipientSession, IExportContext exportContext, string executingUserPrimarySmtpAddress, int previewMaxMailboxes, bool isPFSearchFlightingEnabled, MailboxDiscoverySearch searchObject, OrganizationId orgId)
		{
			Util.ThrowIfNull(ewsClient, "ewsClient");
			Util.ThrowIfNull(exportContext, "exportContext");
			this.isStatisticsOnlySearch = true;
			this.keywordStatisticsQuery = keywordStatisticsQuery;
			this.CurrentState = SearchState.NotStarted;
			this.Errors = new List<string>(1);
			this.executingUserPrimarySmtpAddress = executingUserPrimarySmtpAddress;
			this.ewsClient = ewsClient;
			this.ExportContext = exportContext;
			this.previewMaxMailboxes = previewMaxMailboxes;
			this.InitializeUserKeywordsMapping(keywordStatisticsQuery, userKeywords, recipientSession);
			this.isPFSearchFlightingEnabled = isPFSearchFlightingEnabled;
			this.searchObject = searchObject;
			this.orgId = orgId;
		}

		public MailboxSearchTask(ITargetMailbox targetMailbox, ServerToServerCallingContextFactory callingContextFactory, string executingUserPrimarySmtpAddress, OrganizationId orgId) : this(targetMailbox, callingContextFactory, executingUserPrimarySmtpAddress, orgId, false)
		{
		}

		public MailboxSearchTask(ITargetMailbox targetMailbox, ServerToServerCallingContextFactory callingContextFactory, string executingUserPrimarySmtpAddress, OrganizationId orgId, bool isDocIdHintFlightingEnabled) : this(targetMailbox, ExportHandlerFactory.CreateExportHandler(new MailboxSearchTask.MailboxSearchTracer(), targetMailbox, new ExchangeServiceClientFactory(callingContextFactory ?? new ServerToServerCallingContextFactory(null))), executingUserPrimarySmtpAddress, orgId, isDocIdHintFlightingEnabled)
		{
		}

		public MailboxSearchTask(ITargetMailbox targetMailbox, IExportHandler exportHandler, string executingUserPrimarySmtpAddress, OrganizationId orgId) : this(targetMailbox, exportHandler, executingUserPrimarySmtpAddress, orgId, false)
		{
		}

		public MailboxSearchTask(ITargetMailbox targetMailbox, IExportHandler exportHandler, string executingUserPrimarySmtpAddress, OrganizationId orgId, bool isDocIdHintFlightingEnabled)
		{
			Util.ThrowIfNull(targetMailbox, "targetMailbox");
			Util.ThrowIfNull(exportHandler, "exportHandler");
			this.isStatisticsOnlySearch = false;
			this.CurrentState = SearchState.NotStarted;
			this.Errors = new List<string>(1);
			this.TargetMailbox = targetMailbox;
			this.executingUserPrimarySmtpAddress = executingUserPrimarySmtpAddress;
			exportHandler.IsDocIdHintFlightingEnabled = isDocIdHintFlightingEnabled;
			this.exportHandler = exportHandler;
			this.ExportContext = this.exportHandler.ExportContext;
			this.exportHandler.OnReportStatistics += this.ReportStatistics;
			this.previewMaxMailboxes = 0;
			this.orgId = orgId;
		}

		public EventHandler<ExportStatusEventArgs> OnReportStatistics { get; set; }

		public Action<int, long, long, long, List<KeywordHit>> OnEstimateCompleted { get; set; }

		public Action<ISearchResults> OnPrepareCompleted { get; set; }

		public Action OnExportCompleted { get; set; }

		public IExportContext ExportContext { get; private set; }

		public ISearchResults SearchResults { get; private set; }

		public SearchState CurrentState { get; private set; }

		public IList<string> Errors { get; private set; }

		public ITargetMailbox TargetMailbox { get; private set; }

		private string DiscoverySearchTaskErrorHint
		{
			get
			{
				string result = string.Empty;
				if (this.orgId != null)
				{
					result = this.orgId.ToString();
				}
				else if (this.ExportContext.Sources != null && this.ExportContext.Sources.Count > 0)
				{
					result = this.ExportContext.Sources[0].Id;
				}
				return result;
			}
		}

		public void Abort()
		{
			this.isTaskAborted = true;
			if (this.exportHandler != null)
			{
				this.exportHandler.Stop();
			}
		}

		public void Start()
		{
			ExWatson.SendReportOnUnhandledException(delegate()
			{
				int mailboxesSearchedCount = 0;
				long itemCount = 0L;
				long totalSize = 0L;
				long unsearchableItemCount = 0L;
				long unsearchableTotalSize = 0L;
				List<ErrorRecord> unsearchableFailedMailboxes = null;
				List<ErrorRecord> failedMailboxes = null;
				List<KeywordHit> keywordHits = null;
				SearchState searchState = SearchState.NotStarted;
				Exception exception = null;
				try
				{
					GrayException.MapAndReportGrayExceptions(delegate()
					{
						try
						{
							if (this.searchObject != null)
							{
								this.searchObject.ScenarioId = ScenarioData.Current["SID"];
							}
							if (this.isStatisticsOnlySearch)
							{
								if (string.IsNullOrEmpty(this.keywordStatisticsQuery))
								{
									ScenarioData.Current["S"] = "ES";
									if (this.searchObject != null && !string.IsNullOrEmpty(this.searchObject.CalculatedQuery))
									{
										ScenarioData.Current["QL"] = this.searchObject.CalculatedQuery.Length.ToString();
									}
								}
								else
								{
									ScenarioData.Current["S"] = "KS";
									ScenarioData.Current["QL"] = this.keywordStatisticsQuery.Length.ToString();
								}
								string text = (this.searchObject == null) ? string.Empty : this.searchObject.CalculatedQuery;
								List<string> list = null;
								if (this.ExportContext.Sources != null && this.ExportContext.Sources.Count > 0)
								{
									ScenarioData.Current["M"] = this.ExportContext.Sources.Count.ToString();
									ISource source = this.ExportContext.Sources[0];
									if (string.IsNullOrEmpty(text))
									{
										text = source.SourceFilter;
									}
									list = new List<string>(this.ExportContext.Sources.Count);
									foreach (ISource source2 in this.ExportContext.Sources)
									{
										list.Add(source2.LegacyExchangeDN);
									}
								}
								if (list != null || this.isPFSearchFlightingEnabled)
								{
									bool flag = false;
									this.GetSearchResultEstimation(text, list, out mailboxesSearchedCount, false, ref flag, out itemCount, out totalSize, out failedMailboxes);
								}
								if (!string.IsNullOrEmpty(this.keywordStatisticsQuery) && !this.isTaskAborted)
								{
									List<ErrorRecord> list2 = null;
									List<KeywordStatisticsSearchResultType> keywordStatistics = this.ewsClient.GetKeywordStatistics(this.executingUserPrimarySmtpAddress, this.keywordStatisticsQuery, this.ExportContext.ExportMetadata.Language, (list != null) ? list : new List<string>(1), out list2, (this.searchObject == null || !this.isPFSearchFlightingEnabled) ? null : this.searchObject.Name);
									keywordHits = Util.ConvertToKeywordHitList(keywordStatistics, this.userKeywordsMap);
									if (list2 != null)
									{
										if (failedMailboxes == null)
										{
											failedMailboxes = list2;
										}
										else
										{
											failedMailboxes.AddRange(list2);
										}
									}
								}
								if (Util.IncludeUnsearchableItems(this.ExportContext) && !this.isTaskAborted && (list != null || (this.searchObject != null && this.searchObject.IsFeatureFlighted("PublicFolderSearchFlighted"))))
								{
									KeywordHit keywordHit = null;
									if (keywordHits != null)
									{
										keywordHit = new KeywordHit
										{
											Phrase = "652beee2-75f7-4ca0-8a02-0698a3919cb9"
										};
										keywordHits.Add(keywordHit);
									}
									bool flag2 = false;
									if (this.searchObject != null && this.searchObject.IsFeatureFlighted("PublicFolderSearchFlighted"))
									{
										this.GetSearchResultEstimation(text, list, out mailboxesSearchedCount, true, ref flag2, out unsearchableItemCount, out unsearchableTotalSize, out unsearchableFailedMailboxes);
										if (flag2 && keywordHit != null)
										{
											keywordHit.Count += (int)unsearchableItemCount;
										}
									}
									else
									{
										foreach (string mailboxId in list)
										{
											if (this.isTaskAborted)
											{
												break;
											}
											long unsearchableItemStatistics = this.ewsClient.GetUnsearchableItemStatistics(this.executingUserPrimarySmtpAddress, mailboxId);
											unsearchableItemCount += unsearchableItemStatistics;
											if (keywordHit != null)
											{
												keywordHit.Count += (int)unsearchableItemStatistics;
												keywordHit.MailboxCount += ((unsearchableItemStatistics == 0L) ? 0 : 1);
											}
										}
									}
								}
								searchState = (this.isTaskAborted ? SearchState.EstimateStopped : SearchState.EstimateSucceeded);
							}
							else
							{
								ScenarioData.Current["S"] = "CS";
								this.exportHandler.Prepare();
								this.exportHandler.Export();
								this.PrepareCompleted(this.exportHandler.SearchResults);
								searchState = (this.isTaskAborted ? SearchState.Stopped : SearchState.Succeeded);
							}
						}
						catch (ExportException exception)
						{
							ExportException exception = exception;
						}
						catch (StorageTransientException exception2)
						{
							ExportException exception = exception2;
						}
						catch (StoragePermanentException exception3)
						{
							ExportException exception = exception3;
						}
						catch (DataSourceOperationException exception4)
						{
							ExportException exception = exception4;
						}
					});
				}
				catch (GrayException ex)
				{
					exception = ex;
					ExTraceGlobals.SearchTracer.TraceError<GrayException>((long)this.GetHashCode(), "GrayException {0} is thrown", ex);
				}
				finally
				{
					if (exception != null)
					{
						string message = exception.Message;
						if (string.IsNullOrEmpty(message) && exception.InnerException != null)
						{
							message = exception.InnerException.Message;
						}
						SearchEventLogger.Instance.LogDiscoverySearchTaskErrorEvent(this.ExportContext.ExportMetadata.ExportName, this.DiscoverySearchTaskErrorHint, exception);
						this.Errors.Add(message);
						mailboxesSearchedCount = 0;
						itemCount = 0L;
						totalSize = 0L;
						unsearchableItemCount = 0L;
						keywordHits = null;
						searchState = (this.isStatisticsOnlySearch ? SearchState.EstimateFailed : SearchState.Failed);
					}
					if (failedMailboxes != null && failedMailboxes.Count > 0)
					{
						foreach (ErrorRecord errorRecord in failedMailboxes)
						{
							this.Errors.Add(Util.GenerateErrorMessageFromErrorRecord(errorRecord));
						}
						this.Errors.Insert(0, "Number of failed mailboxes: " + failedMailboxes.Count);
					}
					try
					{
						if (this.isStatisticsOnlySearch)
						{
							this.EstimateCompleted(mailboxesSearchedCount, itemCount, totalSize, unsearchableItemCount, keywordHits, searchState);
						}
						else
						{
							this.ExportCompleted(searchState);
						}
					}
					catch (ExportException ex2)
					{
						SearchEventLogger.Instance.LogDiscoverySearchTaskErrorEvent(this.ExportContext.ExportMetadata.ExportName, this.DiscoverySearchTaskErrorHint, ex2);
						this.Errors.Add(ex2.Message);
						searchState = (this.isStatisticsOnlySearch ? SearchState.EstimateFailed : SearchState.Failed);
					}
				}
			}, delegate(object exception)
			{
				ExTraceGlobals.SearchTracer.TraceError((long)this.GetHashCode(), "InternalStart: Unhandled exception {0}", new object[]
				{
					exception
				});
				SearchEventLogger.Instance.LogDiscoverySearchTaskErrorEvent(this.ExportContext.ExportMetadata.ExportName, this.DiscoverySearchTaskErrorHint, exception.ToString());
				return !(exception is GrayException);
			});
		}

		public void Dispose()
		{
			if (this.TargetMailbox != null)
			{
				this.TargetMailbox.Dispose();
				this.TargetMailbox = null;
			}
			if (this.exportHandler != null)
			{
				this.exportHandler.OnReportStatistics -= this.ReportStatistics;
				this.exportHandler.Dispose();
				this.exportHandler = null;
			}
		}

		private void ReportStatistics(object sender, ExportStatusEventArgs e)
		{
			if (this.OnReportStatistics != null)
			{
				this.OnReportStatistics(sender, e);
			}
		}

		private void EstimateCompleted(int mailboxesSearchedCount, long itemCount, long totalSize, long unsearchableItemCount, List<KeywordHit> keywordHits, SearchState status)
		{
			this.CurrentState = status;
			if (this.OnEstimateCompleted != null)
			{
				this.OnEstimateCompleted(mailboxesSearchedCount, itemCount, totalSize, unsearchableItemCount, keywordHits);
			}
		}

		private void PrepareCompleted(ISearchResults searchResults)
		{
			if (!this.isTaskAborted && this.OnPrepareCompleted != null)
			{
				this.OnPrepareCompleted(searchResults);
			}
		}

		private void ExportCompleted(SearchState status)
		{
			this.CurrentState = status;
			if (this.OnExportCompleted != null)
			{
				this.OnExportCompleted();
			}
		}

		private void GetSearchResultEstimation(string query, List<string> mailboxIds, out int mailboxesSearchedCount, bool isUnsearchable, ref bool newSchemaSearchSucceeded, out long totalItemCount, out long totalSize, out List<ErrorRecord> failedMailboxes)
		{
			mailboxesSearchedCount = 0;
			totalItemCount = 0L;
			totalSize = 0L;
			failedMailboxes = null;
			int num = 0;
			while ((mailboxIds != null && num < mailboxIds.Count) || this.isPFSearchFlightingEnabled)
			{
				if (this.isTaskAborted)
				{
					return;
				}
				int num2 = 0;
				long num3 = 0L;
				long num4 = 0L;
				List<ErrorRecord> list = null;
				List<string> list2 = new List<string>(this.previewMaxMailboxes);
				int num5 = 0;
				if (mailboxIds != null)
				{
					num5 = ((num + this.previewMaxMailboxes >= mailboxIds.Count) ? (mailboxIds.Count - num) : this.previewMaxMailboxes);
					list2.AddRange(mailboxIds.GetRange(num, num5));
				}
				newSchemaSearchSucceeded = false;
				this.ewsClient.GetSearchResultEstimation(this.executingUserPrimarySmtpAddress, query, this.ExportContext.ExportMetadata.Language, list2, out num2, isUnsearchable, out num3, out num4, out list, out newSchemaSearchSucceeded, (this.searchObject == null || !this.isPFSearchFlightingEnabled) ? null : this.searchObject.Name);
				mailboxesSearchedCount += num2;
				totalItemCount += num3;
				totalSize += num4;
				if (list != null)
				{
					if (failedMailboxes == null)
					{
						failedMailboxes = list;
					}
					else
					{
						failedMailboxes.AddRange(list);
					}
				}
				num += num5;
				if (this.isPFSearchFlightingEnabled)
				{
					return;
				}
			}
		}

		private void InitializeUserKeywordsMapping(string keywordStatisticsQuery, MultiValuedProperty<string> userKeywords, IRecipientSession recipientSession)
		{
			if (string.IsNullOrEmpty(keywordStatisticsQuery))
			{
				return;
			}
			Util.ThrowIfNull(recipientSession, "recipientSession");
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, recipientSession.SessionSettings, 788, "InitializeUserKeywordsMapping", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\MailboxSearch\\Common\\MailboxSearchTask.cs");
			if (userKeywords != null && userKeywords.Count > 0)
			{
				CultureInfo culture = CultureInfo.InvariantCulture;
				if (this.ExportContext != null && this.ExportContext.ExportMetadata != null && !string.IsNullOrEmpty(this.ExportContext.ExportMetadata.Language))
				{
					try
					{
						culture = new CultureInfo(this.ExportContext.ExportMetadata.Language);
					}
					catch (CultureNotFoundException)
					{
						ExTraceGlobals.SearchTracer.TraceError<string>((long)this.GetHashCode(), "Culture info: \"{0}\" returns CultureNotFoundException", this.ExportContext.ExportMetadata.Language);
					}
				}
				SearchCriteria searchCriteria = new SearchCriteria(keywordStatisticsQuery, null, culture, SearchType.Statistics, recipientSession, tenantOrTopologyConfigurationSession, Guid.NewGuid(), new List<DefaultFolderType>());
				this.userKeywordsMap = new Dictionary<string, string>(userKeywords.Count);
				if (searchCriteria.SubFilters != null && searchCriteria.SubFilters.Count > 0)
				{
					if (userKeywords.Count != searchCriteria.SubFilters.Count)
					{
						return;
					}
					int num = 0;
					using (MultiValuedProperty<string>.Enumerator enumerator = userKeywords.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							string key = enumerator.Current;
							string value = searchCriteria.SubFilters.Keys.ElementAt(num++);
							this.userKeywordsMap.Add(key, value);
						}
						return;
					}
				}
				if (userKeywords.Count == 1)
				{
					this.userKeywordsMap.Add(userKeywords[0], keywordStatisticsQuery);
				}
			}
		}

		private readonly bool isStatisticsOnlySearch;

		private readonly string keywordStatisticsQuery;

		private readonly int previewMaxMailboxes;

		private readonly IEwsClient ewsClient;

		private readonly string executingUserPrimarySmtpAddress;

		private readonly bool isPFSearchFlightingEnabled;

		private Dictionary<string, string> userKeywordsMap;

		private IExportHandler exportHandler;

		private bool isTaskAborted;

		private MailboxDiscoverySearch searchObject;

		private OrganizationId orgId;

		internal class MailboxSearchTracer : Microsoft.Exchange.EDiscovery.Export.ITracer
		{
			public void TraceError(string format, params object[] args)
			{
				ExTraceGlobals.SearchTracer.TraceError((long)this.GetHashCode(), format, args);
			}

			public void TraceWarning(string format, params object[] args)
			{
				ExTraceGlobals.SearchTracer.TraceWarning((long)this.GetHashCode(), format, args);
			}

			public void TraceInformation(string format, params object[] args)
			{
				ExTraceGlobals.SearchTracer.TraceDebug((long)this.GetHashCode(), format, args);
			}
		}
	}
}
