using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.MailTips;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.MailTips
{
	internal sealed class GetMailTipsQuery : Query<IEnumerable<MailTips>>
	{
		public GetMailTipsQuery(int traceId, ClientContext clientContext, ProxyAddress sendingAs, CachedOrganizationConfiguration configuration, ProxyAddress[] proxyAddresses, MailTipTypes tipsRequested, int lcid, IBudget callerBudget, HttpResponse httpResponse) : base(clientContext, httpResponse, CasTraceEventType.MailTips, MailTipsApplication.MailTipsIOCompletion, MailTipsPerfCounters.MailTipsCurrentRequests)
		{
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction((long)traceId, "GetMailTipsQuery constructor: enter");
			this.traceId = traceId;
			this.sendingAs = sendingAs;
			this.configuration = configuration;
			this.proxyAddresses = proxyAddresses;
			this.tipsRequested = tipsRequested;
			this.lcid = lcid;
			this.callerBudget = callerBudget;
			this.StartTimeUtc = DateTime.UtcNow;
			base.RequestLogger.AppendToLog<string>(",MailTipsStart", this.StartTimeUtc.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffZ"));
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction((long)traceId, "GetMailTipsQuery constructor: exit");
		}

		public DateTime StartTimeUtc { get; private set; }

		public override string ToString()
		{
			string summary = this.GetSummary();
			if (this.results == null)
			{
				return summary;
			}
			int num = 1000 * this.results.Length;
			StringBuilder stringBuilder = new StringBuilder(summary.Length + num);
			stringBuilder.Append(summary);
			for (int i = 0; i < this.results.Length; i++)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "MailTips{0}:{1},{2}", new object[]
				{
					i,
					this.results[i],
					Environment.NewLine
				});
			}
			return stringBuilder.ToString();
		}

		internal bool IsGalSegmented(MailTips mailTips)
		{
			if (base.ClientContext.QueryBaseDN == null)
			{
				return false;
			}
			if (base.ClientContext is InternalClientContext && OrganizationId.ForestWideOrgId == mailTips.Configuration.OrganizationId)
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0} / {1}: Segmented GAL detected, skip setting invalid MailTip.", TraceContext.Get(), mailTips.EmailAddress);
				return true;
			}
			return false;
		}

		internal void EvaluateNonResolved(MailTips mailTips)
		{
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction<object, EmailAddress>((long)this.traceId, "{0} / {1}: GetMailTipsQuery.EvaluateNonResolved: enter", TraceContext.Get(), mailTips.EmailAddress);
			GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, Exception>((long)this.traceId, "{0} / {1}: RecipientData.Exception {2}", TraceContext.Get(), mailTips.EmailAddress, mailTips.RecipientData.Exception);
			if (mailTips.RecipientData.Exception is InvalidSmtpAddressException)
			{
				this.SetInvalidRecipient(mailTips, true);
			}
			else
			{
				string routingType = mailTips.EmailAddress.RoutingType;
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, string>((long)this.traceId, "{0} / {1}: RoutingType {2}", TraceContext.Get(), mailTips.EmailAddress, routingType);
				if (ProxyAddressPrefix.LegacyDN.PrimaryPrefix.Equals(routingType, StringComparison.OrdinalIgnoreCase))
				{
					GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0} / {1}: EX address did not resolve in AD", TraceContext.Get(), mailTips.EmailAddress);
					if (!this.IsGalSegmented(mailTips))
					{
						this.SetInvalidRecipient(mailTips, true);
					}
				}
				else if (ProxyAddressPrefix.Smtp.PrimaryPrefix.Equals(routingType, StringComparison.OrdinalIgnoreCase))
				{
					string domain = mailTips.EmailAddress.Domain;
					OrganizationDomains domains = mailTips.Configuration.Domains;
					DomainType acceptedDomainType = domains.GetAcceptedDomainType(domain);
					GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, DomainType>((long)this.traceId, "{0} / {1}: DomainType {2}", TraceContext.Get(), mailTips.EmailAddress, acceptedDomainType);
					if (DomainType.Authoritative == acceptedDomainType)
					{
						if (!this.IsGalSegmented(mailTips))
						{
							this.SetInvalidRecipient(mailTips, true);
						}
						if (mailTips.RecipientData.Exception == null)
						{
							mailTips.RecipientData.Exception = new InvalidSmtpAddressException(Strings.descInvalidSmtpAddress(mailTips.EmailAddress.ToString()));
						}
					}
					else
					{
						mailTips.NeedMerge = true;
					}
				}
				else if ("MAPIPDL".Equals(routingType, StringComparison.OrdinalIgnoreCase))
				{
					this.SetInvalidRecipient(mailTips, false);
					this.SetExternalMemberCount(mailTips, 0);
					this.SetModeration(mailTips, false);
					this.SetRestriction(mailTips, false);
				}
				else
				{
					this.SetInvalidRecipient(mailTips, false);
					this.SetExternalMemberCount(mailTips, 1);
					this.SetModeration(mailTips, false);
					this.SetRestriction(mailTips, false);
				}
			}
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction<object, EmailAddress>((long)this.traceId, "{0} / {1}: GetMailTipsQuery.EvaluateNonResolved: exit", TraceContext.Get(), mailTips.EmailAddress);
		}

		internal static string FormatRecipientListEntry(MailTips mailTips)
		{
			PerRecipientMailTipsUsage perRecipientUsage = GetMailTipsQuery.GetPerRecipientUsage(mailTips);
			string text = null;
			if (perRecipientUsage != PerRecipientMailTipsUsage.None)
			{
				IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
				string format = "~Usage:{0}";
				object[] array = new object[1];
				object[] array2 = array;
				int num = 0;
				int num2 = (int)perRecipientUsage;
				array2[num] = num2.ToString("X");
				text = string.Format(invariantCulture, format, array);
			}
			string text2 = null;
			string text3 = null;
			if (1 < mailTips.TotalMemberCount)
			{
				text2 = string.Format(CultureInfo.InvariantCulture, "~GroupSize:{0}", new object[]
				{
					mailTips.TotalMemberCount
				});
				if (0 < mailTips.ExternalMemberCount)
				{
					text3 = string.Format(CultureInfo.InvariantCulture, "~GroupExternal:{0}", new object[]
					{
						mailTips.ExternalMemberCount
					});
				}
			}
			string text4 = null;
			if (mailTips.RecipientData != null && mailTips.RecipientData.Exception != null)
			{
				string text5;
				if (mailTips.RecipientData.Exception is QueryGenerationNotRequiredException)
				{
					text5 = "NG";
				}
				else
				{
					text5 = mailTips.RecipientData.Exception.GetType().Name;
				}
				text4 = string.Format(CultureInfo.InvariantCulture, "~ADException:{0}", new object[]
				{
					text5
				});
			}
			string text6 = null;
			if (mailTips.Exception != null)
			{
				string text7;
				if (mailTips.Exception is QueryGenerationNotRequiredException)
				{
					text7 = "NG";
				}
				else
				{
					text7 = mailTips.Exception.GetType().Name;
				}
				text6 = string.Format(CultureInfo.InvariantCulture, "~GeneralException:{0}", new object[]
				{
					text7
				});
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}{2}{3}{4}{5}{6}", new object[]
			{
				mailTips.EmailAddress.RoutingType,
				mailTips.EmailAddress.Address,
				text,
				text2,
				text3,
				text4,
				text6
			});
		}

		protected override void ValidateSpecificInputData()
		{
			if (this.configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			if (this.proxyAddresses == null)
			{
				throw new ArgumentNullException("proxyAddresses");
			}
			if (this.proxyAddresses.Length == 0)
			{
				throw new ArgumentException("proxyAddresses is empty");
			}
			if (this.proxyAddresses.Length > 50)
			{
				throw new ArgumentOutOfRangeException("proxyAddresses.Length");
			}
			foreach (ProxyAddress proxyAddress in this.proxyAddresses)
			{
				if (proxyAddress == null)
				{
					throw new ArgumentException("proxyAddresses contains a null element");
				}
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<ProxyAddress>((long)this.traceId, "ProxyAddress {0}", proxyAddress);
			}
		}

		protected override IEnumerable<MailTips> ExecuteInternal()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			bool flag = true;
			DateTime utcNow = DateTime.UtcNow;
			this.lastStageEndTime = utcNow;
			TimeSpan dueTime = this.requestProcessingDeadline - utcNow + GetMailTipsQuery.overdueReportThreshold;
			using (new Timer(new TimerCallback(this.SendOverdueReport), null, dueTime, TimeSpan.FromMilliseconds(-1.0)))
			{
				try
				{
					this.currentStage = "RecipCounter";
					PerfCounterHelper.UpdateMailTipsRecipientNumberPerformanceCounter((long)this.proxyAddresses.Length);
					this.RecordStageDuration();
					Stopwatch stopwatch2 = new Stopwatch();
					stopwatch2.Start();
					this.currentStage = "OrgConfig";
					Organization organization = this.configuration.OrganizationConfiguration.Configuration;
					this.RecordStageDuration();
					if (!organization.MailTipsAllTipsEnabled)
					{
						GetMailTipsQuery.GetMailTipsTracer.TraceDebug((long)this.traceId, "{0}: GetMailTipsQuery.Execute: MailTips disabled via organization configuration, returning empty results.", new object[]
						{
							TraceContext.Get()
						});
						return this.CreateEmptyResultObjects();
					}
					GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, MailTipTypes>((long)this.traceId, "{0}: Caller requested {1}", TraceContext.Get(), this.tipsRequested);
					this.currentStage = "AdQuerySetup";
					RecipientQueryResults recipientQueryResults = this.QueryDirectoryForMailTips();
					this.RecordStageDuration();
					this.results = new MailTips[recipientQueryResults.Count - 1];
					ProxyAddress proxyAddress = this.EvaluateDirectorySourcedMailTips(recipientQueryResults);
					stopwatch2.Stop();
					this.currentStage = "AdTimeCounter";
					PerfCounterHelper.UpdateMailTipsAverageActiveDirectoryResponseTimePerformanceCounter(stopwatch2.ElapsedMilliseconds);
					this.RecordStageDuration();
					if (!CachedOrganizationConfiguration.GetInstance(OrganizationId.ForestWideOrgId, CachedOrganizationConfiguration.ConfigurationTypes.All).OrganizationConfiguration.Configuration.MailTipsGroupMetricsEnabled)
					{
						GetMailTipsQuery.GetMailTipsTracer.TraceDebug((long)this.traceId, "{0}: GetMailTipsQuery.Execute: Group metrics disabled in first organization configuration.", new object[]
						{
							TraceContext.Get()
						});
					}
					else if (!organization.MailTipsGroupMetricsEnabled)
					{
						GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, OrganizationId>((long)this.traceId, "{0}: GetMailTipsQuery.Execute: Group metrics MailTips disabled in configuration for organization {1}", TraceContext.Get(), organization.OrganizationId);
					}
					else
					{
						this.currentStage = "GetGM";
						this.EvaluateGroupMetrics();
						this.RecordStageDuration();
					}
					MailTipsApplication mailTipsApplication = new MailTipsApplication(this.traceId, proxyAddress, this.tipsRequested, this.callerBudget);
					try
					{
						using (RequestDispatcher requestDispatcher = new RequestDispatcher(base.RequestLogger))
						{
							QueryGenerator queryGenerator = new QueryGenerator(mailTipsApplication, base.ClientContext, base.RequestLogger, requestDispatcher, this.queryPrepareDeadline, this.requestProcessingDeadline, recipientQueryResults);
							try
							{
								this.currentStage = "QueryGen";
								BaseQuery[] queries = queryGenerator.GetQueries();
								this.RecordStageDuration();
								this.SetMailTipsQueryPermissions(queries);
								bool flag2 = false;
								try
								{
									if (base.ClientContext.Budget != null)
									{
										base.ClientContext.Budget.EndLocal();
										flag2 = true;
									}
									this.currentStage = "QueryExe";
									requestDispatcher.Execute(this.requestProcessingDeadline, base.HttpResponse);
									this.RecordStageDuration();
								}
								finally
								{
									if (flag2)
									{
										base.ClientContext.Budget.StartLocal("GetMailTipsQuery.ExecuteInternal", default(TimeSpan));
									}
								}
								ExTraceGlobals.FaultInjectionTracer.TraceTest(3097898301U);
								if (requestDispatcher.CrossForestQueryCount > 0 || requestDispatcher.FederatedCrossForestQueryCount > 0)
								{
									flag = false;
								}
								this.individualMailboxesProcessed = queryGenerator.UniqueQueriesCount;
								this.currentStage = "MergeResults";
								this.MergeQueryResults(queries);
								this.RecordStageDuration();
							}
							finally
							{
								requestDispatcher.LogStatistics(base.RequestLogger);
							}
							this.currentStage = "DisposeRD";
						}
						this.RecordStageDuration();
					}
					finally
					{
						mailTipsApplication.LogThreadsUsage(base.RequestLogger);
					}
				}
				finally
				{
					if (stopwatch != null)
					{
						stopwatch.Stop();
						this.currentStage = "MailTipsTotal";
						this.lastStageEndTime = utcNow;
						this.RecordStageDuration();
						if (flag)
						{
							GetMailTipsQuery.inForestProcessingTime.AddValue(stopwatch.ElapsedMilliseconds);
							MailTipsPerfCounters.MailTipsInForest99thPercentileResponseTime.RawValue = GetMailTipsQuery.inForestProcessingTime.PercentileQuery(99.0);
							MailTipsPerfCounters.MailTipsInForest95thPercentileResponseTime.RawValue = GetMailTipsQuery.inForestProcessingTime.PercentileQuery(95.0);
							MailTipsPerfCounters.MailTipsInForest90thPercentileResponseTime.RawValue = GetMailTipsQuery.inForestProcessingTime.PercentileQuery(90.0);
							MailTipsPerfCounters.MailTipsInForest80thPercentileResponseTime.RawValue = GetMailTipsQuery.inForestProcessingTime.PercentileQuery(80.0);
							MailTipsPerfCounters.MailTipsInForest50thPercentileResponseTime.RawValue = GetMailTipsQuery.inForestProcessingTime.PercentileQuery(50.0);
						}
						else
						{
							GetMailTipsQuery.crossForestProcessingTime.AddValue(stopwatch.ElapsedMilliseconds);
							MailTipsPerfCounters.MailTipsCrossForest99thPercentileResponseTime.RawValue = GetMailTipsQuery.crossForestProcessingTime.PercentileQuery(99.0);
							MailTipsPerfCounters.MailTipsCrossForest95thPercentileResponseTime.RawValue = GetMailTipsQuery.crossForestProcessingTime.PercentileQuery(95.0);
							MailTipsPerfCounters.MailTipsCrossForest90thPercentileResponseTime.RawValue = GetMailTipsQuery.crossForestProcessingTime.PercentileQuery(90.0);
							MailTipsPerfCounters.MailTipsCrossForest80thPercentileResponseTime.RawValue = GetMailTipsQuery.crossForestProcessingTime.PercentileQuery(80.0);
							MailTipsPerfCounters.MailTipsCrossForest50thPercentileResponseTime.RawValue = GetMailTipsQuery.crossForestProcessingTime.PercentileQuery(50.0);
						}
					}
					int num = base.LogFailures(this.results, GetMailTipsQuery.GetExceptionStatistics(this.results));
					if (0 < num)
					{
						MailTipsPerfCounters.MailTipsAccumulatedExceptionRecipients.IncrementBy((long)num);
					}
					this.LogQueryResult();
				}
			}
			return this.results;
		}

		protected override void UpdateCountersAtExecuteEnd(Stopwatch responseTimer)
		{
			MailTipsPerfCounters.MailTipsQueriesAnsweredWithinOneSecond_Base.Increment();
			if (responseTimer.ElapsedMilliseconds < 1000L)
			{
				MailTipsPerfCounters.MailTipsQueriesAnsweredWithinOneSecond.Increment();
				MailTipsPerfCounters.MailTipsQueriesAnsweredWithinThreeSeconds.Increment();
				MailTipsPerfCounters.MailTipsQueriesAnsweredWithinTenSeconds.Increment();
			}
			else if (responseTimer.ElapsedMilliseconds < 3000L)
			{
				MailTipsPerfCounters.MailTipsQueriesAnsweredWithinThreeSeconds.Increment();
				MailTipsPerfCounters.MailTipsQueriesAnsweredWithinTenSeconds.Increment();
			}
			else if (responseTimer.ElapsedMilliseconds < 10000L)
			{
				MailTipsPerfCounters.MailTipsQueriesAnsweredWithinTenSeconds.Increment();
			}
			PerfCounterHelper.UpdateMailTipsResponseTimePerformanceCounter(responseTimer.ElapsedMilliseconds);
			MailTipsPerfCounters.MailTipsAccumulatedRequests.Increment();
			MailTipsPerfCounters.MailTipsAccumulatedRecipients.IncrementBy((long)this.proxyAddresses.Length);
			if (0 < this.individualMailboxesProcessed)
			{
				MailTipsPerfCounters.MailTipsAccumulatedMailboxSourcedRecipients.IncrementBy((long)this.individualMailboxesProcessed);
			}
		}

		protected override void AppendSpecificSpExecuteOperationData(StringBuilder spOperationData)
		{
			spOperationData.AppendFormat("Recipients Processed: {0}", this.proxyAddresses.Length);
		}

		protected override void LogExpensiveRequests(RequestStatistics threadStatistics, RequestStatistics mapiStatistics, RequestStatistics adStatistics)
		{
			if (50 < adStatistics.RequestCount)
			{
				base.RequestLogger.AppendToLog<int>("Expensive", 1);
			}
		}

		private static IDictionary<string, int> GetExceptionStatistics(IEnumerable<MailTips> results)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			if (results != null)
			{
				foreach (MailTips mailTips in results)
				{
					if (mailTips != null && mailTips.Exception != null && !(mailTips.Exception is QueryGenerationNotRequiredException))
					{
						string name = mailTips.Exception.GetType().Name;
						if (!dictionary.ContainsKey(name))
						{
							dictionary.Add(name, 1);
						}
						else
						{
							Dictionary<string, int> dictionary2;
							string key;
							(dictionary2 = dictionary)[key = name] = dictionary2[key] + 1;
						}
					}
				}
			}
			return dictionary;
		}

		private static PerRecipientMailTipsUsage GetPerRecipientUsage(MailTips mailTips)
		{
			PerRecipientMailTipsUsage perRecipientMailTipsUsage = PerRecipientMailTipsUsage.None;
			if (mailTips.IsAvailable(MailTipTypes.OutOfOfficeMessage) && !string.IsNullOrEmpty(mailTips.OutOfOfficeMessage))
			{
				perRecipientMailTipsUsage |= PerRecipientMailTipsUsage.AutoReply;
			}
			if (mailTips.IsAvailable(MailTipTypes.MailboxFullStatus) && mailTips.MailboxFull)
			{
				perRecipientMailTipsUsage |= PerRecipientMailTipsUsage.MailboxFull;
			}
			if (mailTips.IsAvailable(MailTipTypes.CustomMailTip) && !string.IsNullOrEmpty(mailTips.CustomMailTip))
			{
				perRecipientMailTipsUsage |= PerRecipientMailTipsUsage.CustomMailTip;
			}
			if (mailTips.IsAvailable(MailTipTypes.ExternalMemberCount) && 0 < mailTips.ExternalMemberCount)
			{
				perRecipientMailTipsUsage |= PerRecipientMailTipsUsage.External;
			}
			if (mailTips.IsAvailable(MailTipTypes.DeliveryRestriction) && mailTips.DeliveryRestricted)
			{
				perRecipientMailTipsUsage |= PerRecipientMailTipsUsage.Restricted;
			}
			if (mailTips.IsAvailable(MailTipTypes.ModerationStatus) && mailTips.IsModerated)
			{
				perRecipientMailTipsUsage |= PerRecipientMailTipsUsage.Moderated;
			}
			if (mailTips.IsAvailable(MailTipTypes.InvalidRecipient) && mailTips.InvalidRecipient)
			{
				perRecipientMailTipsUsage |= PerRecipientMailTipsUsage.Invalid;
			}
			if (mailTips.IsAvailable(MailTipTypes.Scope) && mailTips.Scope != ScopeTypes.None)
			{
				perRecipientMailTipsUsage |= PerRecipientMailTipsUsage.Scope;
			}
			return perRecipientMailTipsUsage;
		}

		private string GetSummary()
		{
			int capacity = 100 * (1 + this.proxyAddresses.Length);
			StringBuilder stringBuilder = new StringBuilder(capacity);
			stringBuilder.Append("SendingAs=");
			stringBuilder.AppendLine((null == this.sendingAs) ? "null" : this.sendingAs.ToString());
			for (int i = 0; i < this.proxyAddresses.Length; i++)
			{
				stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "Recipient{0}={1}{2}", new object[]
				{
					i,
					this.proxyAddresses[i],
					Environment.NewLine
				});
			}
			return stringBuilder.ToString();
		}

		private IEnumerable<MailTips> CreateEmptyResultObjects()
		{
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction((long)this.traceId, "{0}: GetMailTipsQuery.CreateEmptyResultObjects: enter", new object[]
			{
				TraceContext.Get()
			});
			for (int addressIndex = 0; addressIndex < this.proxyAddresses.Length; addressIndex++)
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<ProxyAddress>((long)this.traceId, "{0}: Creating empty MailTips object", this.proxyAddresses[addressIndex]);
				yield return new MailTips(new EmailAddress(string.Empty, this.proxyAddresses[addressIndex].AddressString, this.proxyAddresses[addressIndex].PrefixString));
			}
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction((long)this.traceId, "{0}: GetMailTipsQuery.CreateEmptyResultObjects: exit", new object[]
			{
				TraceContext.Get()
			});
			yield break;
		}

		private RecipientQueryResults QueryDirectoryForMailTips()
		{
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction((long)this.traceId, "{0}: GetMailTipsQuery.QueryDirectoryForMailTips: enter", new object[]
			{
				TraceContext.Get()
			});
			MailTipsRecipientQuery mailTipsRecipientQuery = new MailTipsRecipientQuery(base.ClientContext, base.ClientContext.QueryBaseDN, this.configuration.OrganizationConfiguration.Configuration.OrganizationId, this.queryPrepareDeadline);
			RecipientQueryResults result = mailTipsRecipientQuery.Query(this.proxyAddresses, this.sendingAs);
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction((long)this.traceId, "{0}: GetMailTipsQuery.QueryDirectoryForMailTips: exit", new object[]
			{
				TraceContext.Get()
			});
			return result;
		}

		private ProxyAddress EvaluateDirectorySourcedMailTips(RecipientQueryResults recipientQueryResults)
		{
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction((long)this.traceId, "{0}: GetMailTipsQuery.EvaluateDirectorySourcedMailTips", new object[]
			{
				TraceContext.Get()
			});
			this.currentStage = "AdBatch1";
			RecipientData recipientData = recipientQueryResults[0];
			this.RecordStageDuration();
			ProxyAddress result = this.ParseSendingAs(recipientData);
			MailTipsPerRequesterPermissionMap mailTipsPerRequesterPermissionMap = new MailTipsPerRequesterPermissionMap(base.ClientContext, recipientQueryResults.Count - 1, GetMailTipsQuery.GetMailTipsTracer, this.traceId);
			for (int i = 1; i < recipientQueryResults.Count; i++)
			{
				MailTips mailTips = new MailTips(recipientQueryResults[i]);
				this.results[i - 1] = mailTips;
				if (mailTips.Configuration == null)
				{
					mailTips.Configuration = this.configuration;
					GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, OrganizationId>((long)this.traceId, "{0}: {1} did not resolve in AD, using the effective config for org {2} used.", TraceContext.Get(), mailTips.EmailAddress, this.configuration.OrganizationId);
				}
				else
				{
					GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, OrganizationId, EmailAddress>((long)this.traceId, "{0}: found org {1} config for resolved recipient {2}.", TraceContext.Get(), mailTips.Configuration.OrganizationId, mailTips.EmailAddress);
					if (mailTips.Configuration.OrganizationConfiguration.Configuration.MailTipsAllTipsEnabled)
					{
						this.currentStage = "PermLookup";
						mailTips.Permission = mailTipsPerRequesterPermissionMap.Lookup(mailTips.RecipientData);
						this.RecordStageDuration();
					}
				}
				if (mailTips.Configuration.OrganizationConfiguration.Configuration.MailTipsAllTipsEnabled && mailTips.Permission.CanAccessAMailTip())
				{
					this.EvaluateRecipient(recipientData, recipientQueryResults, mailTips);
				}
				else
				{
					mailTips.RecipientData.Exception = new QueryGenerationNotRequiredException();
					if (mailTips.Configuration.OrganizationConfiguration.Configuration.MailTipsAllTipsEnabled)
					{
						GetMailTipsQuery.GetMailTipsTracer.TraceDebug((long)this.traceId, "{0}: {1} org config MailTipsAllTipsEnabled=true, MailTipsAccessEnabled={2}, MailTipsAccessLevel={3}, MailTipsInAccessScope={4}.", new object[]
						{
							TraceContext.Get(),
							mailTips.EmailAddress,
							mailTips.Permission.AccessEnabled,
							mailTips.Permission.AccessLevel,
							mailTips.Permission.InAccessScope
						});
					}
					else
					{
						GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0}: {1} org config MailTipsAllTipsEnabled=false.", TraceContext.Get(), mailTips.EmailAddress);
					}
				}
			}
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction((long)this.traceId, "{0}: GetMailTipsQuery.EvaluateDirectorySourcedMailTips: exit", new object[]
			{
				TraceContext.Get()
			});
			return result;
		}

		private ProxyAddress ParseSendingAs(RecipientData sendingAsData)
		{
			if (sendingAsData.Exception == null)
			{
				sendingAsData.Exception = new QueryGenerationNotRequiredException();
			}
			if (sendingAsData.IsEmpty)
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, ProxyAddress>((long)this.traceId, "{0}: Sending as {1} did not resolve in AD.", TraceContext.Get(), this.sendingAs);
			}
			else
			{
				if (sendingAsData.PrimarySmtpAddress.IsValidAddress)
				{
					return new SmtpProxyAddress(sendingAsData.PrimarySmtpAddress.ToString(), true);
				}
				GetMailTipsQuery.GetMailTipsTracer.TraceError<object, ProxyAddress, SmtpAddress>((long)this.traceId, "{0}: Sending as {1} resolved in AD with invalid primary SMTP address {2}", TraceContext.Get(), this.sendingAs, sendingAsData.PrimarySmtpAddress);
			}
			return this.sendingAs;
		}

		private void EvaluateRecipient(RecipientData sendingAsRecipientData, RecipientQueryResults recipientQueryResults, MailTips mailTips)
		{
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction<object, EmailAddress>((long)this.traceId, "{0} / {1}: GetMailTipsQuery.EvaluateRecipient: enter", TraceContext.Get(), mailTips.EmailAddress);
			this.SetTotalMemberCount(mailTips, 1);
			this.EvaluateScope(mailTips);
			if (mailTips.RecipientData.IsEmpty)
			{
				this.EvaluateNonResolved(mailTips);
			}
			else
			{
				this.EvaluateResolved(sendingAsRecipientData, recipientQueryResults, mailTips);
			}
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction<object, EmailAddress>((long)this.traceId, "{0} / {1}: GetMailTipsQuery.EvaluateRecipient: exit, tips retrieved", TraceContext.Get(), mailTips.EmailAddress);
		}

		private void EvaluateScope(MailTips mailTips)
		{
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction<object, EmailAddress>((long)this.traceId, "{0} / {1}: GetMailTipsQuery.EvaluateScope: enter", TraceContext.Get(), mailTips.EmailAddress);
			mailTips.Scope = ScopeTypes.None;
			if (this.CallerRequested(MailTipTypes.Scope))
			{
				RoutingAddress routingAddress = (RoutingAddress)mailTips.EmailAddress.Address;
				bool flag = !string.IsNullOrEmpty(routingAddress.DomainPart) && this.configuration.Domains.IsInternal(routingAddress.DomainPart);
				bool flag2 = !flag;
				bool flag3 = !string.IsNullOrEmpty(routingAddress.DomainPart) && this.configuration.TransportSettings.Configuration.IsTLSSendSecureDomain(routingAddress.DomainPart);
				bool flag4 = flag2 && !flag3;
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug((long)this.traceId, "{0} / {1}: Internal = {2}, External = {3}, ExternalPartner = {4}, ExternalNonPartner = {5}", new object[]
				{
					TraceContext.Get(),
					mailTips.EmailAddress,
					flag,
					flag2,
					flag3,
					flag4
				});
				mailTips.Scope = ((flag ? ScopeTypes.Internal : ScopeTypes.None) | (flag2 ? ScopeTypes.External : ScopeTypes.None) | (flag3 ? ScopeTypes.ExternalPartner : ScopeTypes.None) | (flag4 ? ScopeTypes.ExternalNonPartner : ScopeTypes.None));
			}
			else
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0} / {1}: Caller did not request scope MailTip", TraceContext.Get(), mailTips.EmailAddress);
			}
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction<object, EmailAddress>((long)this.traceId, "{0} / {1}: GetMailTipsQuery.EvaluateScope: exit", TraceContext.Get(), mailTips.EmailAddress);
		}

		private void EvaluateResolved(RecipientData sendingAsRecipientData, RecipientQueryResults recipientQueryResults, MailTips mailTips)
		{
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction<object, EmailAddress>((long)this.traceId, "{0} / {1}: GetMailTipsQuery.EvaluateResolved: enter", TraceContext.Get(), mailTips.EmailAddress);
			Microsoft.Exchange.Data.Directory.Recipient.RecipientType recipientType = mailTips.RecipientData.RecipientType;
			GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, Microsoft.Exchange.Data.Directory.Recipient.RecipientType>((long)this.traceId, "{0} / {1}: RecipientType {2}", TraceContext.Get(), mailTips.EmailAddress, recipientType);
			if (recipientType == Microsoft.Exchange.Data.Directory.Recipient.RecipientType.Invalid)
			{
				this.SetInvalidRecipient(mailTips, true);
				if (mailTips.RecipientData.Exception == null)
				{
					mailTips.RecipientData.Exception = new QueryGenerationNotRequiredException();
				}
			}
			else
			{
				if (Microsoft.Exchange.Data.Directory.Recipient.RecipientType.UserMailbox == recipientType)
				{
					this.SetInvalidRecipient(mailTips, false);
					this.SetExternalMemberCount(mailTips, 0);
					if (this.RequestingMailboxSourcedMailTips(mailTips))
					{
						mailTips.NeedMerge = true;
					}
					else if (mailTips.RecipientData.Exception == null)
					{
						mailTips.RecipientData.Exception = new QueryGenerationNotRequiredException();
					}
				}
				else if (mailTips.RecipientData.IsDistributionGroup)
				{
					this.SetInvalidRecipient(mailTips, false);
					this.SetExternalMemberCount(mailTips, 0);
					if (mailTips.RecipientData.Exception == null)
					{
						mailTips.RecipientData.Exception = new QueryGenerationNotRequiredException();
					}
				}
				else if (Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailContact == recipientType || Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUser == recipientType)
				{
					mailTips.NeedMerge = true;
				}
				else
				{
					this.SetInvalidRecipient(mailTips, false);
					this.SetExternalMemberCount(mailTips, 0);
				}
				this.EvaluateModerationAndRestriction(sendingAsRecipientData, recipientQueryResults, mailTips);
				if (!mailTips.DeliveryRestricted)
				{
					this.EvaluateCustom(mailTips);
					this.EvaluateMaxMessageSize(mailTips);
				}
			}
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction<object, EmailAddress>((long)this.traceId, "{0} / {1}: GetMailTipsQuery.EvaluateResolved: exit", TraceContext.Get(), mailTips.EmailAddress);
		}

		private void EvaluateModerationAndRestriction(RecipientData sendingAsRecipientData, RecipientQueryResults recipientQueryResults, MailTips mailTips)
		{
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction<object, EmailAddress>((long)this.traceId, "{0} / {1}: GetMailTipsQuery.EvaluateModerationAndRestriction: enter", TraceContext.Get(), mailTips.EmailAddress);
			if (this.CallerRequested(MailTipTypes.DeliveryRestriction | MailTipTypes.ModerationStatus))
			{
				MailTipsRecipientQuery mailTipsRecipientQuery = (MailTipsRecipientQuery)recipientQueryResults.RecipientQuery;
				RestrictionCheckResult restrictionCheckResult = mailTipsRecipientQuery.CheckDeliveryRestriction(sendingAsRecipientData, mailTips.RecipientData);
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, string>((long)this.traceId, "{0} / {1}: Permission restriction check returned {2}", TraceContext.Get(), mailTips.EmailAddress, restrictionCheckResult.ToString("X"));
				this.SetRestriction(mailTips, (RestrictionCheckResult)0U != ((RestrictionCheckResult)2147483648U & restrictionCheckResult));
				this.SetModeration(mailTips, (RestrictionCheckResult)0U != (RestrictionCheckResult.Moderated & restrictionCheckResult));
				if (mailTips.DeliveryRestricted && Microsoft.Exchange.Data.Directory.Recipient.RecipientType.UserMailbox == mailTips.RecipientData.RecipientType && mailTips.RecipientData.Exception == null)
				{
					mailTips.RecipientData.Exception = new QueryGenerationNotRequiredException();
				}
			}
			else
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0} / {1}: Permission restriction check was not requested.", TraceContext.Get(), mailTips.EmailAddress);
			}
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction((long)this.traceId, "{0} / {1}: GetMailTipsQuery.EvaluateModerationAndRestriction: exit, restricted {2}, moderated {3}", new object[]
			{
				TraceContext.Get(),
				mailTips.EmailAddress,
				mailTips.DeliveryRestricted,
				mailTips.IsModerated
			});
		}

		private void EvaluateCustom(MailTips mailTips)
		{
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction<object, EmailAddress>((long)this.traceId, "{0} / {1}: GetMailTipsQuery.EvaluateCustom: enter", TraceContext.Get(), mailTips.EmailAddress);
			if (this.CallerRequested(MailTipTypes.CustomMailTip))
			{
				if (MailTipsAccessLevel.All == mailTips.Permission.AccessLevel)
				{
					string text = MailTipsUtility.GetCustomMailTip(mailTips.RecipientData, this.traceId, this.lcid);
					if (!string.IsNullOrEmpty(text))
					{
						text = MailTipsUtility.MakeSafeHtml(this.traceId, text);
					}
					mailTips.CustomMailTip = text;
				}
				else
				{
					GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, MailTipsAccessLevel>((long)this.traceId, "{0} / {1}: MailTipsAccessLevel={2} is insufficient to request custom MailTip.", TraceContext.Get(), mailTips.EmailAddress, mailTips.Permission.AccessLevel);
				}
			}
			else
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0} / {1}: Caller did not request custom MailTip.", TraceContext.Get(), mailTips.EmailAddress);
			}
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction<object, EmailAddress>((long)this.traceId, "{0} / {1}: GetMailTipsQuery.EvaluateCustom: exit", TraceContext.Get(), mailTips.EmailAddress);
		}

		private void EvaluateMaxMessageSize(MailTips mailTips)
		{
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction<object, EmailAddress>((long)this.traceId, "{0} / {1}: GetMailTipsQuery.EvaluateMaxMessageSize: enter", TraceContext.Get(), mailTips.EmailAddress);
			if (!this.CallerRequested(MailTipTypes.MaxMessageSize))
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0} / {1}: Skipping MaxMessageSize, it was not requested", TraceContext.Get(), mailTips.EmailAddress);
				return;
			}
			TransportConfigContainer transportConfigContainer = mailTips.Configuration.TransportSettings.Configuration;
			if (transportConfigContainer.MaxReceiveSize.IsUnlimited)
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0} / {1}: organization's max receive size is unlimited", TraceContext.Get(), mailTips.EmailAddress);
				mailTips.MaxMessageSize = int.MaxValue;
			}
			else
			{
				int num = (int)transportConfigContainer.MaxReceiveSize.Value.ToBytes();
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, int>((long)this.traceId, "{0} / {1}: organization's max receive size is {2}", TraceContext.Get(), mailTips.EmailAddress, num);
				mailTips.MaxMessageSize = num;
			}
			Unlimited<ByteQuantifiedSize> maxReceiveSize = mailTips.RecipientData.MaxReceiveSize;
			if (!maxReceiveSize.IsUnlimited)
			{
				int num2 = (int)maxReceiveSize.Value.ToBytes();
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, int>((long)this.traceId, "{0} / {1}: recipient's max receive size is {2}", TraceContext.Get(), mailTips.EmailAddress, num2);
				mailTips.MaxMessageSize = num2;
			}
			if (!OrganizationId.ForestWideOrgId.Equals(mailTips.Configuration.OrganizationId))
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, OrganizationId>((long)this.traceId, "{0} / {1}: the recipient's organization {2} is different to the forestwide organization, retrieving forestwide organization settings.", TraceContext.Get(), mailTips.EmailAddress, mailTips.Configuration.OrganizationId);
				CachedOrganizationConfiguration instance = CachedOrganizationConfiguration.GetInstance(OrganizationId.ForestWideOrgId, CachedOrganizationConfiguration.ConfigurationTypes.All);
				Unlimited<ByteQuantifiedSize> maxReceiveSize2 = instance.TransportSettings.Configuration.MaxReceiveSize;
				if (instance.TransportSettings.Configuration.MaxReceiveSize.IsUnlimited)
				{
					GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0} / {1}: forestwide max receive size is unlimited, no need to override max message size.", TraceContext.Get(), mailTips.EmailAddress);
				}
				else
				{
					int num3 = (int)instance.TransportSettings.Configuration.MaxReceiveSize.Value.ToBytes();
					if (num3 < mailTips.MaxMessageSize)
					{
						mailTips.MaxMessageSize = num3;
						GetMailTipsQuery.GetMailTipsTracer.TraceDebug((long)this.traceId, "{0} / {1}: forestwide max receive size {2} bytes is less than existing max receive size {3}, overriden.", new object[]
						{
							TraceContext.Get(),
							mailTips.EmailAddress,
							num3,
							mailTips.MaxMessageSize
						});
					}
					else
					{
						GetMailTipsQuery.GetMailTipsTracer.TraceDebug((long)this.traceId, "{0} / {1}: forestwide max receive size {2} bytes is not less than existing max receive size {3}, no need to override max message size.", new object[]
						{
							TraceContext.Get(),
							mailTips.EmailAddress,
							num3,
							mailTips.MaxMessageSize
						});
					}
				}
			}
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction<object, EmailAddress, int>((long)this.traceId, "{0} / {1}: GetMailTipsQuery.EvaluateMaxMessageSize: exit: {2}", TraceContext.Get(), mailTips.EmailAddress, mailTips.MaxMessageSize);
		}

		private void EvaluateGroupMetrics()
		{
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction((long)this.traceId, "{0}: GetMailTipsQuery.EvaluateGroupMetrics: enter", new object[]
			{
				TraceContext.Get()
			});
			if (!this.CallerRequested(MailTipTypes.ExternalMemberCount | MailTipTypes.TotalMemberCount))
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug((long)this.traceId, "{0}: Skipping group metrics, they were not requested", new object[]
				{
					TraceContext.Get()
				});
				return;
			}
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			int num = 0;
			try
			{
				foreach (MailTips mailTips in this.results)
				{
					RecipientData recipientData = mailTips.RecipientData;
					if (!mailTips.Configuration.OrganizationConfiguration.Configuration.MailTipsAllTipsEnabled)
					{
						GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, OrganizationId>((long)this.traceId, "{0} / {1}: Organization {2} Config MailTipsAllTipsEnabled=False.", TraceContext.Get(), mailTips.EmailAddress, mailTips.Configuration.OrganizationConfiguration.Configuration.OrganizationId);
					}
					else if (!mailTips.Permission.CanAccessAMailTip())
					{
						GetMailTipsQuery.GetMailTipsTracer.TraceDebug((long)this.traceId, "{0} / {1}: Insufficient MailTipsPermission to access group metrics data: AccessEnabled={2}, AccessLevel={3}, InAccessScope={4}.", new object[]
						{
							TraceContext.Get(),
							mailTips.EmailAddress,
							mailTips.Permission.AccessEnabled,
							mailTips.Permission.AccessLevel,
							mailTips.Permission.InAccessScope
						});
					}
					else if (MailTipsAccessLevel.All != mailTips.Permission.AccessLevel)
					{
						GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, MailTipsAccessLevel>((long)this.traceId, "{0} / {1}: Insufficient MailTipsAccessLevel to access group metrics data: AccessLevel={2}.", TraceContext.Get(), mailTips.EmailAddress, mailTips.Permission.AccessLevel);
					}
					else if (recipientData.IsEmpty)
					{
						GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0} / {1}: Recipient did not resolve in AD.", TraceContext.Get(), mailTips.EmailAddress);
					}
					else if (!recipientData.IsDistributionGroup)
					{
						GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, Microsoft.Exchange.Data.Directory.Recipient.RecipientType>((long)this.traceId, "{0} / {1}: Recipient is not a distribution group, it is a {2}", TraceContext.Get(), mailTips.EmailAddress, mailTips.RecipientData.RecipientType);
					}
					else if (GroupMetricsDataStore.RenderingDisabled)
					{
						GetMailTipsQuery.GetMailTipsTracer.TraceDebug((long)this.traceId, "{0}: Group Metrics rendering has been disabled.", new object[]
						{
							TraceContext.Get()
						});
					}
					else
					{
						num++;
						GetMailTipsQuery.GetMailTipsTracer.TraceDebug((long)this.traceId, "{0} / {1}: Found group metrics record, {2} total, {3} external", new object[]
						{
							TraceContext.Get(),
							mailTips.EmailAddress,
							recipientData.GroupMemberCount,
							recipientData.GroupExternalMemberCount
						});
						this.SetTotalMemberCount(mailTips, recipientData.GroupMemberCount);
						this.SetExternalMemberCount(mailTips, recipientData.GroupExternalMemberCount);
					}
				}
			}
			finally
			{
				stopwatch.Stop();
				base.RequestLogger.AppendToLog<long>("GM.TimeTaken", stopwatch.ElapsedMilliseconds);
				base.RequestLogger.AppendToLog<int>("GM.RequestCount", num);
				GetMailTipsQuery.GetMailTipsTracer.TraceFunction((long)this.traceId, "{0}: GetMailTipsQuery.EvaluateGroupMetrics: exit", new object[]
				{
					TraceContext.Get()
				});
			}
		}

		private void MergeQueryResults(BaseQuery[] baseQueries)
		{
			for (int i = 1; i < baseQueries.Length; i++)
			{
				MailTips mailTips = this.results[i - 1];
				if (mailTips.NeedMerge)
				{
					MailTipsQuery mailTipsQuery = (MailTipsQuery)baseQueries[i];
					this.MergeSingleQueryResult(mailTips, mailTipsQuery.Result);
				}
			}
		}

		private void MergeSingleQueryResult(MailTips mailTips, MailTipsQueryResult queryResult)
		{
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction((long)this.traceId, "{0}: GetMailTipsQuery.MergeSingleQueryResult: enter", new object[]
			{
				TraceContext.Get()
			});
			if (queryResult == null)
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceError<object, EmailAddress>((long)this.traceId, "{0} / {1}: MailTipsQueryResult was null, this indicates an unexpected exception was thrown during local query.", TraceContext.Get(), mailTips.EmailAddress);
			}
			else if (queryResult.MailTips == null)
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceError<object, EmailAddress, LocalizedException>((long)this.traceId, "{0} / {1}: MailTipsQueryResult contained exception {2}.", TraceContext.Get(), mailTips.EmailAddress, queryResult.ExceptionInfo);
				if (mailTips.Exception == null)
				{
					mailTips.Exception = queryResult.ExceptionInfo;
				}
				if (mailTips.RecipientData.IsEmpty || Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailContact == mailTips.RecipientData.RecipientType || Microsoft.Exchange.Data.Directory.Recipient.RecipientType.MailUser == mailTips.RecipientData.RecipientType)
				{
					this.SetInvalidRecipient(mailTips, false);
					this.EvaluateLocalExternalityForRemoteRecipient(mailTips, null);
				}
			}
			else
			{
				this.MergeMailTipsForSingleRecipient(mailTips, queryResult.MailTips);
			}
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction((long)this.traceId, "{0}: GetMailTipsQuery.MergeQueryResults: exit", new object[]
			{
				TraceContext.Get()
			});
		}

		private void MergeMailTipsForSingleRecipient(MailTips mailTips, MailTips queryResultMailTips)
		{
			if (queryResultMailTips.IsAvailable(MailTipTypes.TotalMemberCount))
			{
				this.SetTotalMemberCount(mailTips, queryResultMailTips.TotalMemberCount);
			}
			if (queryResultMailTips.IsAvailable(MailTipTypes.InvalidRecipient))
			{
				this.SetInvalidRecipient(mailTips, queryResultMailTips.InvalidRecipient);
				if (mailTips.InvalidRecipient)
				{
					GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0}: Recipient {1} is invalid according to remote AD.", TraceContext.Get(), mailTips.EmailAddress);
					return;
				}
			}
			if (queryResultMailTips.IsAvailable(MailTipTypes.DeliveryRestriction))
			{
				if (mailTips.RecipientData.IsEmpty)
				{
					this.SetRestriction(mailTips, queryResultMailTips.DeliveryRestricted);
				}
				else if (!mailTips.DeliveryRestricted && queryResultMailTips.DeliveryRestricted)
				{
					this.SetRestriction(mailTips, true);
				}
				if (mailTips.DeliveryRestricted)
				{
					GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0}: Recipient {1} is restricted according to remote AD.", TraceContext.Get(), mailTips.EmailAddress);
					return;
				}
			}
			if (queryResultMailTips.IsAvailable(MailTipTypes.OutOfOfficeMessage))
			{
				this.SetAutoReply(mailTips, queryResultMailTips.OutOfOfficeMessage, queryResultMailTips.OutOfOfficeMessageLanguage, queryResultMailTips.OutOfOfficeDuration);
			}
			if (queryResultMailTips.IsAvailable(MailTipTypes.MailboxFullStatus))
			{
				this.SetMailboxFull(mailTips, queryResultMailTips.MailboxFull);
			}
			if (!mailTips.RecipientData.IsEmpty && Microsoft.Exchange.Data.Directory.Recipient.RecipientType.UserMailbox == mailTips.RecipientData.RecipientType)
			{
				return;
			}
			this.EvaluateLocalExternalityForRemoteRecipient(mailTips, queryResultMailTips);
			if (queryResultMailTips.IsAvailable(MailTipTypes.ModerationStatus))
			{
				if (mailTips.RecipientData.IsEmpty)
				{
					this.SetModeration(mailTips, queryResultMailTips.IsModerated);
				}
				else if (!mailTips.IsModerated && queryResultMailTips.IsModerated)
				{
					this.SetModeration(mailTips, true);
				}
			}
			if (queryResultMailTips.IsAvailable(MailTipTypes.CustomMailTip))
			{
				if (mailTips.RecipientData.IsEmpty)
				{
					this.SetCustom(mailTips, queryResultMailTips.CustomMailTip);
				}
				else if (string.IsNullOrEmpty(mailTips.CustomMailTip))
				{
					this.SetCustom(mailTips, queryResultMailTips.CustomMailTip);
				}
			}
			if (queryResultMailTips.IsAvailable(MailTipTypes.MaxMessageSize))
			{
				if (mailTips.RecipientData.IsEmpty)
				{
					this.SetMaxMessageSize(mailTips, queryResultMailTips.MaxMessageSize);
					return;
				}
				if (mailTips.MaxMessageSize > queryResultMailTips.MaxMessageSize)
				{
					this.SetMaxMessageSize(mailTips, queryResultMailTips.MaxMessageSize);
				}
			}
		}

		private void EvaluateLocalExternalityForRemoteRecipient(MailTips mailTips, MailTips remoteMailTips)
		{
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction<object, EmailAddress>((long)this.traceId, "{0} / {1}: GetMailTipsQuery.EvaluateLocalExternalityForRemoteRecipient: enter", TraceContext.Get(), mailTips.EmailAddress);
			OrganizationDomains domains = mailTips.Configuration.Domains;
			string text;
			if (mailTips.RecipientData.IsEmpty)
			{
				text = mailTips.EmailAddress.Domain;
			}
			else
			{
				text = MailTipsUtility.GetTargetAddressDomain(mailTips.RecipientData.ExternalEmailAddress);
			}
			bool flag = domains.IsInternal(text);
			GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, bool>((long)this.traceId, "{0} / {1}: IsInternal {2}", TraceContext.Get(), mailTips.EmailAddress, flag);
			if (flag)
			{
				if (remoteMailTips == null)
				{
					GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0} / {1}: Setting external count to 0 since this recipient did not go out of the org.", TraceContext.Get(), mailTips.EmailAddress);
					this.SetExternalMemberCount(mailTips, 0);
				}
				else if (remoteMailTips.IsAvailable(MailTipTypes.ExternalMemberCount))
				{
					GetMailTipsQuery.GetMailTipsTracer.TraceDebug((long)this.traceId, "{0} / {1}: Setting external count to the remote external count {2} since the remote domain {3} is considered internal.", new object[]
					{
						TraceContext.Get(),
						mailTips.EmailAddress,
						remoteMailTips.ExternalMemberCount,
						text
					});
					this.SetExternalMemberCount(mailTips, remoteMailTips.ExternalMemberCount);
				}
				else
				{
					GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0} / {1}: Leaving the external count unset since the remote org did not set external count.", TraceContext.Get(), mailTips.EmailAddress);
				}
			}
			else
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, string>((long)this.traceId, "{0} / {1}: Setting the external count to the total member count since domain {2} is not considered internal", TraceContext.Get(), mailTips.EmailAddress, text);
				this.SetExternalMemberCount(mailTips, mailTips.TotalMemberCount);
			}
			GetMailTipsQuery.GetMailTipsTracer.TraceFunction<object, EmailAddress>((long)this.traceId, "{0} / {1}: GetMailTipsQuery.EvaluateLocalExternalityForRemoteRecipient: exit", TraceContext.Get(), mailTips.EmailAddress);
		}

		private bool CallerRequested(MailTipTypes types)
		{
			return (this.tipsRequested & types) != MailTipTypes.None;
		}

		private void SetTotalMemberCount(MailTips mailTips, int count)
		{
			if (this.CallerRequested(MailTipTypes.TotalMemberCount))
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, int>((long)this.traceId, "{0} / {1}: TotalMemberCount = {2}", TraceContext.Get(), mailTips.EmailAddress, count);
				mailTips.TotalMemberCount = count;
				return;
			}
			GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0} / {1}: Caller did not request total member count MailTip, defaulting to 1", TraceContext.Get(), mailTips.EmailAddress);
			mailTips.TotalMemberCount = 1;
		}

		private void SetExternalMemberCount(MailTips mailTips, int count)
		{
			if (!this.CallerRequested(MailTipTypes.ExternalMemberCount))
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0} / {1}: Caller did not request external MailTip", TraceContext.Get(), mailTips.EmailAddress);
				return;
			}
			if (!mailTips.Configuration.OrganizationConfiguration.Configuration.MailTipsExternalRecipientsTipsEnabled)
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, OrganizationId>((long)this.traceId, "{0} / {1}: Organization {2} config has MailTipsExternalRecipientsTipsEnabled=False", TraceContext.Get(), mailTips.EmailAddress, mailTips.Configuration.OrganizationConfiguration.Configuration.OrganizationId);
				return;
			}
			if (MailTipsAccessLevel.All == mailTips.Permission.AccessLevel)
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, int>((long)this.traceId, "{0} / {1}: ExternalMemberCount = {2} for MailTipsAccessLevel All.", TraceContext.Get(), mailTips.EmailAddress, count);
				mailTips.ExternalMemberCount = count;
				return;
			}
			GetMailTipsQuery.GetMailTipsTracer.TraceDebug((long)this.traceId, "{0} / {1}: ExternalMemberCount = {2} for MailTipsAccessLevel {3}.", new object[]
			{
				TraceContext.Get(),
				mailTips.EmailAddress,
				mailTips.TotalMemberCount,
				mailTips.Permission.AccessLevel
			});
			mailTips.ExternalMemberCount = mailTips.TotalMemberCount;
		}

		private void SetInvalidRecipient(MailTips mailTips, bool invalid)
		{
			if (this.CallerRequested(MailTipTypes.InvalidRecipient))
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, bool>((long)this.traceId, "{0} / {1}: InvalidRecipient = {2}", TraceContext.Get(), mailTips.EmailAddress, invalid);
				mailTips.InvalidRecipient = invalid;
			}
			else
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0} / {1}: Caller did not request invalid MailTip", TraceContext.Get(), mailTips.EmailAddress);
			}
			if (invalid)
			{
				this.SetExternalMemberCount(mailTips, 0);
				this.SetModeration(mailTips, false);
				this.SetRestriction(mailTips, false);
			}
		}

		private void SetModeration(MailTips mailTips, bool moderated)
		{
			if (!this.CallerRequested(MailTipTypes.ModerationStatus))
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0} / {1}: Caller did not request moderation MailTip", TraceContext.Get(), mailTips.EmailAddress);
				return;
			}
			if (MailTipsAccessLevel.All == mailTips.Permission.AccessLevel)
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, bool>((long)this.traceId, "{0} / {1}: Moderation = {2}", TraceContext.Get(), mailTips.EmailAddress, moderated);
				mailTips.IsModerated = moderated;
				return;
			}
			GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, MailTipsAccessLevel>((long)this.traceId, "{0} / {1}: MailTipsAccessLevel was {2} which is not enough to retrieve moderation MailTip", TraceContext.Get(), mailTips.EmailAddress, mailTips.Permission.AccessLevel);
		}

		private void SetRestriction(MailTips mailTips, bool restricted)
		{
			if (this.CallerRequested(MailTipTypes.DeliveryRestriction))
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, bool>((long)this.traceId, "{0} / {1}: Restriction = {2}", TraceContext.Get(), mailTips.EmailAddress, restricted);
				mailTips.DeliveryRestricted = restricted;
				return;
			}
			GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0} / {1}: Caller did not request restricted MailTip", TraceContext.Get(), mailTips.EmailAddress);
		}

		private void SetAutoReply(MailTips mailTips, string message, string language, Duration duration)
		{
			if (!this.CallerRequested(MailTipTypes.OutOfOfficeMessage))
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0} / {1}: Caller did not request autoreply MailTip", TraceContext.Get(), mailTips.EmailAddress);
				return;
			}
			if (mailTips.Configuration.OrganizationConfiguration.Configuration.MailTipsMailboxSourcedTipsEnabled)
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug((long)this.traceId, "{0} / {1} : Auto reply hasMessage={2}, language={3}, duration={4}", new object[]
				{
					TraceContext.Get(),
					mailTips.EmailAddress,
					!string.IsNullOrEmpty(message),
					language,
					duration
				});
				mailTips.OutOfOfficeMessage = message;
				mailTips.OutOfOfficeMessageLanguage = language;
				mailTips.OutOfOfficeDuration = duration;
				return;
			}
			GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, OrganizationId>((long)this.traceId, "{0} / {1}: Organization {2} config has MailTipsMailboxSourcedTipsEnabled=False", TraceContext.Get(), mailTips.EmailAddress, mailTips.Configuration.OrganizationConfiguration.Configuration.OrganizationId);
		}

		private void SetMailboxFull(MailTips mailTips, bool full)
		{
			if (!this.CallerRequested(MailTipTypes.MailboxFullStatus))
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0} / {1}: Caller did not request mailbox full MailTip", TraceContext.Get(), mailTips.EmailAddress);
				return;
			}
			if (mailTips.Configuration.OrganizationConfiguration.Configuration.MailTipsMailboxSourcedTipsEnabled)
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, bool>((long)this.traceId, "{0} / {1}: Mailbox full = {2}", TraceContext.Get(), mailTips.EmailAddress, full);
				mailTips.MailboxFull = full;
				return;
			}
			GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, OrganizationId>((long)this.traceId, "{0} / {1}: Organization {2} config has MailTipsMailboxSourcedTipsEnabled=False", TraceContext.Get(), mailTips.EmailAddress, mailTips.Configuration.OrganizationConfiguration.Configuration.OrganizationId);
		}

		private void SetCustom(MailTips mailTips, string customTip)
		{
			if (this.CallerRequested(MailTipTypes.CustomMailTip))
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, bool>((long)this.traceId, "{0} / {1}: hasCustom = {2}", TraceContext.Get(), mailTips.EmailAddress, !string.IsNullOrEmpty(customTip));
				mailTips.CustomMailTip = customTip;
				return;
			}
			GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0} / {1}: Caller did not request custom MailTip", TraceContext.Get(), mailTips.EmailAddress);
		}

		private void SetMaxMessageSize(MailTips mailTips, int maxMessageSize)
		{
			if (this.CallerRequested(MailTipTypes.MaxMessageSize))
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress, int>((long)this.traceId, "{0} / {1}: Max message size = {2}", TraceContext.Get(), mailTips.EmailAddress, maxMessageSize);
				mailTips.MaxMessageSize = maxMessageSize;
				return;
			}
			GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, EmailAddress>((long)this.traceId, "{0} / {1}: Caller did not request message size MailTip", TraceContext.Get(), mailTips.EmailAddress);
		}

		private bool RequestingMailboxSourcedMailTips(MailTips mailTips)
		{
			if (!this.CallerRequested(MailTipTypes.OutOfOfficeMessage | MailTipTypes.MailboxFullStatus))
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug((long)this.traceId, "{0}: Caller did not request mailbox based MailTips.", new object[]
				{
					TraceContext.Get()
				});
				return false;
			}
			if (!mailTips.Configuration.OrganizationConfiguration.Configuration.MailTipsMailboxSourcedTipsEnabled)
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, OrganizationId>((long)this.traceId, "{0}: Mailbox based MailTips are disabled in configuration for organization {1}.", TraceContext.Get(), mailTips.Configuration.OrganizationId);
				return false;
			}
			if (!CachedOrganizationConfiguration.GetInstance(OrganizationId.ForestWideOrgId, CachedOrganizationConfiguration.ConfigurationTypes.All).OrganizationConfiguration.Configuration.MailTipsAllTipsEnabled)
			{
				GetMailTipsQuery.GetMailTipsTracer.TraceDebug((long)this.traceId, "{0}: Mailbox based MailTips are disabled in first organization configuration.", new object[]
				{
					TraceContext.Get()
				});
				return false;
			}
			GetMailTipsQuery.GetMailTipsTracer.TraceDebug<object, OrganizationId>((long)this.traceId, "{0}: Mailbox based MailTips were requested, and are enabled  for both first organization and current organization {1}", TraceContext.Get(), mailTips.Configuration.OrganizationId);
			return true;
		}

		private void LogQueryResult()
		{
			base.RequestLogger.AppendToLog<ProxyAddress>("SendAs", this.sendingAs);
			base.RequestLogger.AppendToLog<int>("Recipients", this.proxyAddresses.Length);
			if (this.results != null)
			{
				string value = string.Join("|", (from result in this.results
				select GetMailTipsQuery.FormatRecipientListEntry(result)).ToArray<string>());
				base.RequestLogger.AppendToLog<string>("RecipientsList", value);
				if (this.CallerRequested(MailTipTypes.TotalMemberCount))
				{
					int num = 0;
					foreach (MailTips mailTips in this.results)
					{
						num += mailTips.TotalMemberCount;
						if ((ulong)this.configuration.OrganizationConfiguration.Configuration.MailTipsLargeAudienceThreshold < (ulong)((long)num))
						{
							base.RequestLogger.AppendToLog<int>("LargeAudience", 1);
							return;
						}
					}
					return;
				}
			}
			else
			{
				string value = string.Join("|", (from proxyAddress in this.proxyAddresses
				select proxyAddress.ProxyAddressString).ToArray<string>());
				base.RequestLogger.AppendToLog<string>("RecipientsList", value);
			}
		}

		private void SetMailTipsQueryPermissions(BaseQuery[] baseQueryArray)
		{
			for (int i = 1; i < baseQueryArray.Length; i++)
			{
				MailTipsQuery mailTipsQuery = (MailTipsQuery)baseQueryArray[i];
				mailTipsQuery.Permission = this.results[i - 1].Permission;
			}
		}

		private void RecordStageDuration()
		{
			DateTime utcNow = DateTime.UtcNow;
			TimeSpan timeSpan = utcNow - this.lastStageEndTime;
			base.RequestLogger.AppendToLog<ulong>(this.currentStage, Convert.ToUInt64(timeSpan.TotalMilliseconds));
			this.lastStageEndTime = utcNow;
		}

		private void SendOverdueReport(object stateNotUsed)
		{
			string value = string.Format(CultureInfo.InvariantCulture, "Stage:{0},OverdueThreshold:{1}", new object[]
			{
				this.currentStage,
				GetMailTipsQuery.overdueReportThreshold
			});
			base.RequestLogger.AppendToLog<string>("Overdue", value);
		}

		public const string DateTimeUtcFormatSpecifier = "yyyy-MM-ddTHH\\:mm\\:ss.fffZ";

		private const string PdlRoutingType = "MAPIPDL";

		private const int AdRequestThreshold = 50;

		private static readonly Microsoft.Exchange.Diagnostics.Trace GetMailTipsTracer = ExTraceGlobals.GetMailTipsTracer;

		private static readonly PercentileCounter inForestProcessingTime = new PercentileCounter(TimeSpan.FromHours(2.0), TimeSpan.FromMinutes(1.0), 100L, 10000L);

		private static readonly PercentileCounter crossForestProcessingTime = new PercentileCounter(TimeSpan.FromHours(2.0), TimeSpan.FromMinutes(1.0), 100L, 10000L);

		private static readonly TimeSpan overdueReportThreshold = TimeSpan.FromMinutes(15.0);

		private ProxyAddress sendingAs;

		private ProxyAddress[] proxyAddresses;

		private MailTipTypes tipsRequested;

		private int lcid;

		private int traceId;

		private CachedOrganizationConfiguration configuration;

		private MailTips[] results;

		private IBudget callerBudget;

		private string currentStage;

		private DateTime lastStageEndTime;
	}
}
