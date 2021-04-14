using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Tasks;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal sealed class MailboxAuditLogSearchWorker
	{
		public MailboxAuditLogSearchWorker(int searchTimeoutSeconds, MailboxAuditLogSearch searchCriteria, Unlimited<int> resultSize, AuditLogOpticsLogData searchStatistics)
		{
			if (searchTimeoutSeconds <= 0)
			{
				throw new ArgumentOutOfRangeException("searchTimeoutSeconds");
			}
			if (searchCriteria == null)
			{
				throw new ArgumentNullException("searchCriteria");
			}
			this.searchCriteria = searchCriteria;
			this.searchStatistics = searchStatistics;
			if (MailboxAuditLogSearchWorker.UseFASTQuery(this.searchCriteria))
			{
				this.queryString = this.GenerateFASTSearchQueryString();
			}
			else
			{
				this.queryFilter = this.GenerateSearchQueryFilter();
			}
			this.searchTimeoutSeconds = searchTimeoutSeconds;
			this.resultSize = resultSize;
			this.recipientSessionInternal = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(searchCriteria.OrganizationId), 163, ".ctor", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxAuditLog\\MailboxAuditLogSearchWorker.cs");
		}

		public IEnumerable<MailboxAuditLogRecord> SearchMailboxAudits(ADUser mailbox)
		{
			if (mailbox == null)
			{
				throw new ArgumentNullException("mailbox");
			}
			this.mailboxUser = mailbox;
			this.outputCount = 0;
			AuditLogSearchId auditLogSearchId = (this.searchCriteria.Identity != null) ? (this.searchCriteria.Identity as AuditLogSearchId) : null;
			if (this.searchStatistics != null)
			{
				this.searchStatistics.CorrelationID = ((auditLogSearchId != null) ? auditLogSearchId.Guid.ToString() : string.Empty);
			}
			if (this.searchCriteria.ShowDetails)
			{
				return this.DeepSearch();
			}
			return this.ShallowSearch();
		}

		private IEnumerable<MailboxAuditLogRecord> ShallowSearch()
		{
			List<MailboxAuditLogRecord> list = new List<MailboxAuditLogRecord>();
			List<string> list2 = (this.searchCriteria.LogonTypes == null) ? new List<string>() : new List<string>(this.searchCriteria.LogonTypes);
			if (this.searchCriteria.ExternalAccess == null)
			{
				if (list2.Count != 1 || !list2[0].Equals("Delegate"))
				{
					list2.Add("External");
				}
				MailboxAuditLogRecord mailboxAuditLogRecord = this.CompareLastAccessDatesAndPrepareUserRecord(list2);
				if (mailboxAuditLogRecord != null)
				{
					list.Add(mailboxAuditLogRecord);
				}
			}
			else if (this.searchCriteria.ExternalAccess.Value)
			{
				MailboxAuditLogRecord latestAccessDateAndPrepareUserRecord = MailboxAuditLogSearchWorker.GetLatestAccessDateAndPrepareUserRecord(this.mailboxUser, "External");
				if (latestAccessDateAndPrepareUserRecord != null)
				{
					list.Add(latestAccessDateAndPrepareUserRecord);
				}
			}
			else
			{
				MailboxAuditLogRecord mailboxAuditLogRecord2 = this.CompareLastAccessDatesAndPrepareUserRecord(list2);
				if (mailboxAuditLogRecord2 != null)
				{
					list.Add(mailboxAuditLogRecord2);
				}
			}
			if (this.searchStatistics != null)
			{
				this.searchStatistics.ResultsReturned += (long)this.resolvedUserNameList.Count;
			}
			return list;
		}

		private IEnumerable<MailboxAuditLogRecord> DeepSearch()
		{
			ExchangePrincipal principal = ExchangePrincipal.FromADUser(this.mailboxUser.OrganizationId.ToADSessionSettings(), this.mailboxUser, RemotingOptions.AllowCrossSite);
			TaskLogger.Trace("Opening EWS connection for '{0}'", new object[]
			{
				this.mailboxUser
			});
			IEnumerable<MailboxAuditLogRecord> primaryResults = this.SearchMailboxStorage(new EwsAuditClient(new EwsConnectionManager(principal, OpenAsAdminOrSystemServiceBudgetTypeType.Default, MailboxAuditLogSearchWorker.Tracer), TimeSpan.FromSeconds((double)this.searchTimeoutSeconds), MailboxAuditLogSearchWorker.Tracer), false);
			foreach (MailboxAuditLogRecord record in primaryResults)
			{
				if (!this.IsLessThanResultSize(this.outputCount))
				{
					yield break;
				}
				this.outputCount++;
				if (this.searchStatistics != null)
				{
					this.searchStatistics.ResultsReturned += 1L;
				}
				yield return record;
			}
			if (this.IsLessThanResultSize(this.outputCount) && !principal.MailboxInfo.IsArchive && this.mailboxUser.ArchiveGuid != Guid.Empty && (this.mailboxUser.ArchiveState == ArchiveState.HostedProvisioned || this.mailboxUser.ArchiveState == ArchiveState.Local))
			{
				ExchangePrincipal archivePrincipal = principal.GetArchiveExchangePrincipal(RemotingOptions.AllowCrossSite | RemotingOptions.AllowCrossPremise);
				IEnumerable<MailboxAuditLogRecord> archiveResults = this.SearchMailboxStorage(new EwsAuditClient(new EwsConnectionManager(archivePrincipal, OpenAsAdminOrSystemServiceBudgetTypeType.Default, MailboxAuditLogSearchWorker.Tracer), TimeSpan.FromSeconds((double)this.searchTimeoutSeconds), MailboxAuditLogSearchWorker.Tracer), true);
				foreach (MailboxAuditLogRecord record2 in archiveResults)
				{
					if (!this.IsLessThanResultSize(this.outputCount))
					{
						yield break;
					}
					this.outputCount++;
					if (this.searchStatistics != null)
					{
						this.searchStatistics.ResultsReturned += 1L;
					}
					yield return record2;
				}
			}
			yield break;
		}

		private IEnumerable<MailboxAuditLogRecord> SearchMailboxStorage(EwsAuditClient ewsClient, bool isArchive)
		{
			FolderIdType auditRootId;
			bool auditExists = ewsClient.FindFolder("Audits", isArchive ? MailboxAuditLogSearchWorker.ArchiveRecoverableItemsRootId : MailboxAuditLogSearchWorker.RecoverableItemsRootId, out auditRootId);
			if (auditExists)
			{
				TaskLogger.Trace("Search query filter is defined. Searching audit log messages in audit folder", new object[0]);
				int maxCount = this.resultSize.IsUnlimited ? int.MaxValue : ((this.resultSize.Value > this.outputCount) ? (this.resultSize.Value - this.outputCount) : 0);
				EwsAuditLogCollection logCollection = new EwsAuditLogCollection(ewsClient, auditRootId);
				IEnumerable<MailboxAuditLogRecord> records;
				if (this.queryString != null)
				{
					records = AuditLogSearchQuery.SearchAuditLogs<MailboxAuditLogRecord, QueryStringType>(logCollection, this.queryString, maxCount, TimeSpan.FromSeconds((double)this.searchTimeoutSeconds), new MailboxAuditLogSearchWorker.QueryStrategy(this), MailboxAuditLogSearchWorker.Tracer);
				}
				else
				{
					records = AuditLogSearchQuery.SearchAuditLogs<MailboxAuditLogRecord, RestrictionType>(logCollection, this.queryFilter, maxCount, TimeSpan.FromSeconds((double)this.searchTimeoutSeconds), new MailboxAuditLogSearchWorker.QueryStrategy(this), MailboxAuditLogSearchWorker.Tracer);
				}
				foreach (MailboxAuditLogRecord record in records)
				{
					yield return record;
				}
			}
			yield break;
		}

		private MailboxAuditLogRecord ReadAuditLogRecord(ObjectId itemId, ExDateTime creationTime, string content)
		{
			MailboxAuditLogRecordId identity = new MailboxAuditLogRecordId(itemId);
			string mailboxResolvedName = MailboxAuditLogSearchWorker.ResolveMailboxOwnerName(this.mailboxUser);
			MailboxAuditLogEvent mailboxAuditLogEvent = AuditLogParseSerialize.ParseMailboxAuditRecord(identity, content, mailboxResolvedName, this.mailboxUser.Guid.ToString(), new DateTime?(creationTime.UniversalTime.ToLocalTime()));
			this.ResolveDisplayNames(mailboxAuditLogEvent);
			return mailboxAuditLogEvent;
		}

		private static bool UseFASTQuery(MailboxAuditLogSearch searchCriteria)
		{
			return (searchCriteria.LogonTypes != null && searchCriteria.LogonTypes.Count > 0) || searchCriteria.ExternalAccess != null;
		}

		private QueryStringType GenerateFASTSearchQueryString()
		{
			TaskLogger.LogEnter();
			QueryStringType result;
			try
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				int num = 0;
				AqsQueryBuilder.AppendDateClause(stringBuilder, PropertyKeyword.Received, DateRangeQueryOperation.GreaterThanOrEqual, this.searchCriteria.StartDateUtc.Value.ToLocalTime());
				num++;
				AqsQueryBuilder.AppendDateClause(stringBuilder, PropertyKeyword.Received, DateRangeQueryOperation.LessThan, this.searchCriteria.EndDateUtc.Value.ToLocalTime());
				num++;
				if (this.searchCriteria.ExternalAccess != null)
				{
					string text = string.Format(CultureInfo.InvariantCulture, "{0}: {1}", new object[]
					{
						"ExternalAccess",
						this.searchCriteria.ExternalAccess.Value
					});
					AqsQueryBuilder.AppendKeywordOrClause(stringBuilder, PropertyKeyword.Body, new string[]
					{
						text
					});
					num++;
				}
				if (this.searchCriteria.LogonTypes != null && this.searchCriteria.LogonTypes.Count > 0)
				{
					string[] array = new string[this.searchCriteria.LogonTypes.Count];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = string.Format(CultureInfo.InvariantCulture, "{0}: {1}", new object[]
						{
							"LogonType",
							this.searchCriteria.LogonTypes[i]
						});
						num++;
					}
					AqsQueryBuilder.AppendKeywordOrClause(stringBuilder, PropertyKeyword.Body, array);
				}
				if (this.searchCriteria.Operations != null && this.searchCriteria.Operations.Count > 0)
				{
					string[] array2 = new string[this.searchCriteria.Operations.Count];
					for (int j = 0; j < array2.Length; j++)
					{
						array2[j] = string.Format(CultureInfo.InvariantCulture, "{0}: {1}", new object[]
						{
							"Operation",
							this.searchCriteria.Operations[j]
						});
						num++;
					}
					AqsQueryBuilder.AppendKeywordOrClause(stringBuilder, PropertyKeyword.Body, array2);
				}
				this.searchCriteria.QueryComplexity = num;
				if (this.searchStatistics != null)
				{
					this.searchStatistics.QueryComplexity = num;
				}
				if (stringBuilder.Length > 0)
				{
					result = new QueryStringType
					{
						Value = stringBuilder.ToString()
					};
				}
				else
				{
					result = null;
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
			return result;
		}

		private RestrictionType GenerateSearchQueryFilter()
		{
			TaskLogger.LogEnter();
			RestrictionType result;
			try
			{
				List<SearchExpressionType> list = new List<SearchExpressionType>();
				int num = 0;
				ExDateTime exDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, this.searchCriteria.StartDateUtc.Value);
				list.Add(new IsGreaterThanOrEqualToType
				{
					Item = new PathToUnindexedFieldType
					{
						FieldURI = UnindexedFieldURIType.itemDateTimeReceived
					},
					FieldURIOrConstant = new FieldURIOrConstantType
					{
						Item = new ConstantValueType
						{
							Value = exDateTime.ToISOString()
						}
					}
				});
				num++;
				exDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, this.searchCriteria.EndDateUtc.Value);
				list.Add(new IsLessThanType
				{
					Item = new PathToUnindexedFieldType
					{
						FieldURI = UnindexedFieldURIType.itemDateTimeReceived
					},
					FieldURIOrConstant = new FieldURIOrConstantType
					{
						Item = new ConstantValueType
						{
							Value = exDateTime.ToISOString()
						}
					}
				});
				num++;
				this.searchCriteria.QueryComplexity = num;
				if (this.searchStatistics != null)
				{
					this.searchStatistics.QueryComplexity = num;
				}
				if (list.Count > 0)
				{
					result = new RestrictionType
					{
						Item = new AndType
						{
							Items = list.ToArray()
						}
					};
				}
				else
				{
					result = null;
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
			return result;
		}

		private MailboxAuditLogRecord CompareLastAccessDatesAndPrepareUserRecord(List<string> logonTypes)
		{
			new List<MailboxAuditLogRecord>();
			ExDateTime endDate = new ExDateTime(ExTimeZone.UtcTimeZone, this.searchCriteria.EndDateUtc.Value);
			ExDateTime startDate = new ExDateTime(ExTimeZone.UtcTimeZone, this.searchCriteria.StartDateUtc.Value);
			DateTime? lastAccessed = MailboxAuditLogSearchWorker.CompareLastAccessWithSearchCriteria(this.mailboxUser, startDate, endDate, logonTypes);
			if (lastAccessed != null)
			{
				MailboxAuditLogRecordId identity = new MailboxAuditLogRecordId(this.mailboxUser.Identity);
				string mailboxResolvedName = MailboxAuditLogSearchWorker.ResolveMailboxOwnerName(this.mailboxUser);
				return new MailboxAuditLogRecord(identity, mailboxResolvedName, this.mailboxUser.Guid.ToString(), lastAccessed);
			}
			return null;
		}

		private void ResolveDisplayNames(MailboxAuditLogEvent auditRecord)
		{
			if (!string.IsNullOrEmpty(auditRecord.LogonUserSid) && string.IsNullOrEmpty(auditRecord.LogonUserDisplayName))
			{
				auditRecord.LogonUserDisplayName = this.GetDisplayNameFromSid(auditRecord.LogonUserSid);
			}
		}

		private string GetDisplayNameFromSid(string userSid)
		{
			if (this.resolvedUserNameList.ContainsKey(userSid))
			{
				return this.resolvedUserNameList[userSid];
			}
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(userSid);
			ADRecipient adrecipient = this.recipientSessionInternal.FindBySid(securityIdentifier);
			string text;
			if (adrecipient != null)
			{
				text = MailboxAuditLogSearchWorker.ResolveMailboxOwnerName(adrecipient);
			}
			else
			{
				text = SecurityPrincipalIdParameter.GetFriendlyUserName(securityIdentifier, null);
			}
			this.resolvedUserNameList[userSid] = text;
			return text;
		}

		private bool IsLessThanResultSize(int value)
		{
			return this.resultSize.IsUnlimited || value < this.resultSize.Value;
		}

		private static MailboxAuditLogRecord GetLatestAccessDateAndPrepareUserRecord(ADUser user, string logonType)
		{
			DateTime? lastAccessed = null;
			if (logonType.Equals("External", StringComparison.OrdinalIgnoreCase))
			{
				DateTime? dateTime = (DateTime?)user[ADRecipientSchema.AuditLastExternalAccess];
				if (dateTime != null)
				{
					lastAccessed = dateTime;
				}
			}
			else if (logonType.Equals("Admin", StringComparison.OrdinalIgnoreCase))
			{
				DateTime? dateTime2 = (DateTime?)user[ADRecipientSchema.AuditLastAdminAccess];
				if (dateTime2 != null)
				{
					lastAccessed = dateTime2;
				}
			}
			else if (logonType.Equals("Delegate", StringComparison.OrdinalIgnoreCase))
			{
				DateTime? dateTime3 = (DateTime?)user[ADRecipientSchema.AuditLastDelegateAccess];
				if (dateTime3 != null)
				{
					lastAccessed = dateTime3;
				}
			}
			if (lastAccessed != null)
			{
				MailboxAuditLogRecordId identity = new MailboxAuditLogRecordId(user.Identity);
				string mailboxResolvedName = MailboxAuditLogSearchWorker.ResolveMailboxOwnerName(user);
				return new MailboxAuditLogRecord(identity, mailboxResolvedName, user.Guid.ToString(), lastAccessed);
			}
			return null;
		}

		private static DateTime? CompareLastAccessWithSearchCriteria(ADUser user, ExDateTime startDate, ExDateTime endDate, List<string> logonTypes)
		{
			DateTime? dateTime = null;
			if (logonTypes.Contains("External"))
			{
				DateTime? dateTime2 = (DateTime?)user[ADRecipientSchema.AuditLastExternalAccess];
				if (dateTime2 != null)
				{
					ExDateTime t = new ExDateTime(ExTimeZone.UtcTimeZone, dateTime2.Value.ToUniversalTime());
					if (t >= startDate && t <= endDate)
					{
						dateTime = dateTime2;
					}
				}
			}
			if (logonTypes.Contains("Admin"))
			{
				DateTime? dateTime3 = (DateTime?)user[ADRecipientSchema.AuditLastAdminAccess];
				if (dateTime3 != null)
				{
					ExDateTime t2 = new ExDateTime(ExTimeZone.UtcTimeZone, dateTime3.Value.ToUniversalTime());
					if (t2 >= startDate && t2 <= endDate && (dateTime == null || dateTime < dateTime3))
					{
						dateTime = dateTime3;
					}
				}
			}
			if (logonTypes.Contains("Delegate"))
			{
				DateTime? dateTime4 = (DateTime?)user[ADRecipientSchema.AuditLastDelegateAccess];
				if (dateTime4 != null)
				{
					ExDateTime t3 = new ExDateTime(ExTimeZone.UtcTimeZone, dateTime4.Value.ToUniversalTime());
					if (t3 >= startDate && t3 <= endDate && (dateTime == null || dateTime < dateTime4))
					{
						dateTime = dateTime4;
					}
				}
			}
			return dateTime;
		}

		private static string ResolveMailboxOwnerName(ADRecipient owner)
		{
			string result;
			if (!string.IsNullOrEmpty(owner.DisplayName))
			{
				result = owner.DisplayName;
			}
			else if (!string.IsNullOrEmpty(owner.Alias))
			{
				result = owner.Alias;
			}
			else
			{
				result = owner.Identity.ToString();
			}
			return result;
		}

		private static readonly DistinguishedFolderIdType RecoverableItemsRootId = new DistinguishedFolderIdType
		{
			Id = DistinguishedFolderIdNameType.recoverableitemsroot
		};

		private static readonly DistinguishedFolderIdType ArchiveRecoverableItemsRootId = new DistinguishedFolderIdType
		{
			Id = DistinguishedFolderIdNameType.archiverecoverableitemsroot
		};

		private static readonly Trace Tracer = ExTraceGlobals.TraceTracer;

		private MailboxAuditLogSearch searchCriteria;

		private RestrictionType queryFilter;

		private QueryStringType queryString;

		private readonly int searchTimeoutSeconds;

		private readonly Unlimited<int> resultSize;

		private int outputCount;

		private IRecipientSession recipientSessionInternal;

		private ADUser mailboxUser;

		private Dictionary<string, string> resolvedUserNameList = new Dictionary<string, string>();

		private AuditLogOpticsLogData searchStatistics;

		private class QueryStrategy : IAuditQueryStrategy<MailboxAuditLogRecord>
		{
			public QueryStrategy(MailboxAuditLogSearchWorker search)
			{
				this.search = search;
			}

			public bool RecordFilter(IReadOnlyPropertyBag propertyBag, out bool stopNow)
			{
				stopNow = false;
				return true;
			}

			public MailboxAuditLogRecord Convert(IReadOnlyPropertyBag propertyBag)
			{
				ObjectId itemId = propertyBag[ItemSchema.Id] as ObjectId;
				ExDateTime creationTime = (ExDateTime)propertyBag[StoreObjectSchema.CreationTime];
				return this.search.ReadAuditLogRecord(itemId, creationTime, propertyBag[ItemSchema.TextBody] as string);
			}

			public Exception GetTimeoutException(TimeSpan timeout)
			{
				return new MailboxAuditLogSearchException(Strings.ErrorMailboxAuditLogSearchTimeout(timeout.TotalSeconds.ToString(), this.search.searchCriteria.ToString(), this.search.mailboxUser.Identity.ToString()), null);
			}

			public Exception GetQueryFailedException()
			{
				return new MailboxAuditLogSearchException(Strings.MailboxAuditLogSearchFailed(this.search.searchCriteria.ToString(), this.search.mailboxUser.Identity.ToString()), null);
			}

			private MailboxAuditLogSearchWorker search;
		}
	}
}
