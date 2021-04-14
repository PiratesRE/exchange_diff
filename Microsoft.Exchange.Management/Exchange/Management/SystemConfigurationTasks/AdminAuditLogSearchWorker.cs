using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
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
	internal sealed class AdminAuditLogSearchWorker
	{
		public AdminAuditLogSearchWorker(int searchTimeoutSeconds, AdminAuditLogSearch searchObject, AuditLogOpticsLogData searchStatistics)
		{
			if (searchTimeoutSeconds <= 0)
			{
				throw new ArgumentOutOfRangeException("searchTimeoutSeconds");
			}
			this.searchTimeoutSeconds = searchTimeoutSeconds;
			this.searchCriteria = searchObject;
			this.searchStatistics = searchStatistics;
		}

		public AdminAuditLogEvent[] Search()
		{
			TaskLogger.LogEnter();
			TaskLogger.Trace("Search criteria:\\r\\n{0}", new object[]
			{
				this.searchCriteria.ToString()
			});
			if (DatacenterRegistry.IsForefrontForOffice())
			{
				return this.SearchInFFO();
			}
			ADUser tenantArbitrationMailbox;
			try
			{
				tenantArbitrationMailbox = AdminAuditLogHelper.GetTenantArbitrationMailbox(this.searchCriteria.OrganizationId);
			}
			catch (ObjectNotFoundException innerException)
			{
				TaskLogger.Trace("ObjectNotFoundException occurred when getting Exchange principal from the discovery mailbox user.", new object[0]);
				throw new AdminAuditLogSearchException(Strings.AdminAuditLogsLocationNotFound(this.searchCriteria.OrganizationId.ToString()), innerException);
			}
			catch (NonUniqueRecipientException innerException2)
			{
				TaskLogger.Trace("More than one tenant arbitration mailbox found for the current organization.", new object[0]);
				throw new AdminAuditLogSearchException(Strings.AdminAuditLogsLocationNotFound(this.searchCriteria.OrganizationId.ToString()), innerException2);
			}
			Exception ex = null;
			ExchangePrincipal principal = ExchangePrincipal.FromADUser(this.searchCriteria.OrganizationId.ToADSessionSettings(), tenantArbitrationMailbox, RemotingOptions.AllowCrossSite);
			AdminAuditLogEvent[] result;
			try
			{
				TaskLogger.Trace("Opening EWS connection for the tenant arbitration mailbox", new object[0]);
				EwsAuditClient ewsAuditClient = new EwsAuditClient(new EwsConnectionManager(principal, OpenAsAdminOrSystemServiceBudgetTypeType.Default, AdminAuditLogSearchWorker.Tracer), TimeSpan.FromSeconds((double)this.searchTimeoutSeconds), AdminAuditLogSearchWorker.Tracer);
				FolderIdType folderIdType = null;
				ewsAuditClient.CheckAndCreateWellKnownFolder(DistinguishedFolderIdNameType.root, DistinguishedFolderIdNameType.recoverableitemsroot, out folderIdType);
				ewsAuditClient.CheckAndCreateWellKnownFolder(DistinguishedFolderIdNameType.recoverableitemsroot, DistinguishedFolderIdNameType.adminauditlogs, out folderIdType);
				if (folderIdType == null)
				{
					result = Array<AdminAuditLogEvent>.Empty;
				}
				else
				{
					EwsAuditLogCollection logCollection = new EwsAuditLogCollection(ewsAuditClient, folderIdType);
					AuditLogSearchId auditLogSearchId = this.searchCriteria.Identity as AuditLogSearchId;
					IEnumerable<AdminAuditLogEvent> source;
					if (this.UseFASTQuery())
					{
						QueryStringType queryFilter = this.GenerateFASTSearchQueryString();
						source = AuditLogSearchQuery.SearchAuditLogs<AdminAuditLogEvent, QueryStringType>(logCollection, queryFilter, this.searchCriteria.ResultSize + this.searchCriteria.StartIndex, TimeSpan.FromSeconds((double)this.searchTimeoutSeconds), new AdminAuditLogSearchWorker.QueryStrategy(this.searchCriteria), AdminAuditLogSearchWorker.Tracer);
					}
					else
					{
						RestrictionType queryFilter2 = this.GenerateSearchQueryFilterForEWS();
						source = AuditLogSearchQuery.SearchAuditLogs<AdminAuditLogEvent, RestrictionType>(logCollection, queryFilter2, this.searchCriteria.ResultSize + this.searchCriteria.StartIndex, TimeSpan.FromSeconds((double)this.searchTimeoutSeconds), new AdminAuditLogSearchWorker.QueryStrategy(this.searchCriteria), AdminAuditLogSearchWorker.Tracer);
					}
					if (this.searchStatistics != null)
					{
						this.searchStatistics.QueryComplexity = this.searchCriteria.QueryComplexity;
						this.searchStatistics.CorrelationID = ((auditLogSearchId != null) ? auditLogSearchId.Guid.ToString() : string.Empty);
					}
					AdminAuditLogEvent[] array = source.Take(this.searchCriteria.ResultSize).ToArray<AdminAuditLogEvent>();
					this.RedactCallerField(array);
					if (this.searchStatistics != null)
					{
						this.searchStatistics.ResultsReturned += array.LongLength;
						this.searchStatistics.CallResult = true;
					}
					result = array;
				}
			}
			catch (StorageTransientException ex2)
			{
				ex = ex2;
				TaskLogger.Trace("Search admin audit log failed with transient storage exception. {0}", new object[]
				{
					ex2
				});
				throw new AdminAuditLogSearchException(Strings.AdminAuditLogSearchFailed, ex2, this.searchCriteria);
			}
			catch (StoragePermanentException ex3)
			{
				ex = ex3;
				TaskLogger.Trace("Search admin audit log failed with permanent storage exception. {0}", new object[]
				{
					ex3
				});
				throw new AdminAuditLogSearchException(Strings.AdminAuditLogSearchFailed, ex3, this.searchCriteria);
			}
			catch (AuditLogException ex4)
			{
				ex = ex4;
				TaskLogger.Trace("Search admin audit log failed with storage exception. {0}", new object[]
				{
					ex4
				});
				throw new AdminAuditLogSearchException(Strings.AdminAuditLogSearchFailed, ex4, this.searchCriteria);
			}
			finally
			{
				if (this.searchStatistics != null && ex != null)
				{
					this.searchStatistics.ErrorType = ex;
					this.searchStatistics.ErrorCount++;
				}
				TaskLogger.LogExit();
			}
			return result;
		}

		private AdminAuditLogEvent[] SearchInFFO()
		{
			IConfigurationSession configurationSession = AdminAuditLogHelper.CreateSession(this.searchCriteria.OrganizationId, null);
			QueryFilter filter = this.GenerateSearchQueryFilterForFFO();
			if (this.searchStatistics != null)
			{
				this.searchStatistics.QueryComplexity = this.searchCriteria.QueryComplexity;
			}
			AdminAuditLogEvent[] array = configurationSession.Find<AdminAuditLogEventFacade>(filter, null, false, null).Cast<AdminAuditLogEventFacade>().Select(delegate(AdminAuditLogEventFacade e)
			{
				AdminAuditLogEvent adminAuditLogEvent = new AdminAuditLogEvent
				{
					ObjectModified = e.ObjectModified,
					ModifiedObjectResolvedName = e.ModifiedObjectResolvedName,
					CmdletName = e.CmdletName,
					CmdletParameters = e.CmdletParameters,
					ModifiedProperties = e.ModifiedProperties,
					Caller = e.Caller,
					Succeeded = e.Succeeded,
					Error = e.Error,
					RunDate = e.RunDate,
					OriginatingServer = e.OriginatingServer
				};
				adminAuditLogEvent[SimpleProviderObjectSchema.Identity] = new AdminAuditLogEventId(e.Identity);
				return adminAuditLogEvent;
			}).ToArray<AdminAuditLogEvent>();
			this.RedactCallerField(array);
			if (array != null && this.searchStatistics != null)
			{
				this.searchStatistics.ResultsReturned += array.LongLength;
				this.searchStatistics.CallResult = true;
			}
			return array;
		}

		private void RedactCallerField(AdminAuditLogEvent[] logEvents)
		{
			if (this.searchCriteria.RedactDatacenterAdmins && logEvents != null)
			{
				for (long num = 0L; num < logEvents.LongLength; num += 1L)
				{
					checked
					{
						if (logEvents[(int)((IntPtr)num)].ExternalAccess ?? false)
						{
							logEvents[(int)((IntPtr)num)].Caller = "********";
						}
					}
				}
			}
		}

		private QueryFilter GenerateSearchQueryFilterForFFO()
		{
			TaskLogger.LogEnter();
			List<QueryFilter> list = new List<QueryFilter>();
			if (this.searchCriteria.Cmdlets != null && this.searchCriteria.Cmdlets.Count > 0)
			{
				list.Add(new OrFilter((from c in this.searchCriteria.Cmdlets
				select new ComparisonFilter(ComparisonOperator.Equal, AdminAuditLogSchema.CmdletName, c)).ToArray<ComparisonFilter>()));
			}
			if (this.searchCriteria.Parameters != null && this.searchCriteria.Parameters.Count > 0)
			{
				list.Add(new OrFilter((from p in this.searchCriteria.Parameters
				select new ComparisonFilter(ComparisonOperator.Equal, AdminAuditLogSchema.CmdletParameters, p)).ToArray<ComparisonFilter>()));
			}
			if (this.searchCriteria.StartDateUtc != null)
			{
				ExDateTime exDateTime = new ExDateTime(ExTimeZone.UtcTimeZone, this.searchCriteria.StartDateUtc.Value);
				list.Add(new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ItemSchema.ReceivedTime, exDateTime));
			}
			if (this.searchCriteria.EndDateUtc != null)
			{
				ExDateTime exDateTime2 = new ExDateTime(ExTimeZone.UtcTimeZone, this.searchCriteria.EndDateUtc.Value);
				list.Add(new ComparisonFilter(ComparisonOperator.LessThan, ItemSchema.ReceivedTime, exDateTime2));
			}
			if (this.searchCriteria.Succeeded != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, AdminAuditLogSchema.Succeeded, this.searchCriteria.Succeeded.Value));
			}
			if (this.searchCriteria.ExternalAccess != null)
			{
				list.Add(new TextFilter(ItemSchema.TextBody, string.Format(CultureInfo.InvariantCulture, "{0}: {1}", new object[]
				{
					"ExternalAccess",
					this.searchCriteria.ExternalAccess.Value
				}), MatchOptions.SubString, MatchFlags.Loose));
			}
			if (this.searchCriteria.ObjectIds != null && this.searchCriteria.ObjectIds.Count > 0)
			{
				list.Add(new OrFilter((from o in this.searchCriteria.ObjectIds
				select new ComparisonFilter(ComparisonOperator.Equal, AdminAuditLogSchema.ObjectModified, o)).ToArray<ComparisonFilter>()));
			}
			if (this.searchCriteria.ResolvedUsers != null && this.searchCriteria.ResolvedUsers.Count > 0)
			{
				list.Add(new OrFilter((from u in this.searchCriteria.ResolvedUsers
				select new ComparisonFilter(ComparisonOperator.Equal, AdminAuditLogSchema.Caller, u)).ToArray<ComparisonFilter>()));
			}
			this.searchCriteria.QueryComplexity = list.Count;
			TaskLogger.LogExit();
			if (list.Count > 0)
			{
				return new AndFilter(list.ToArray());
			}
			return null;
		}

		private bool UseFASTQuery()
		{
			return (this.searchCriteria.Cmdlets != null && this.searchCriteria.Cmdlets.Count > 0) || (this.searchCriteria.Parameters != null && this.searchCriteria.Parameters.Count > 0) || this.searchCriteria.Succeeded != null || this.searchCriteria.ExternalAccess != null || (this.searchCriteria.ObjectIds != null && this.searchCriteria.ObjectIds.Count > 0) || (this.searchCriteria.ResolvedUsers != null && this.searchCriteria.ResolvedUsers.Count > 0);
		}

		private RestrictionType GenerateSearchQueryFilterForEWS()
		{
			TaskLogger.LogEnter();
			RestrictionType result;
			try
			{
				List<SearchExpressionType> list = new List<SearchExpressionType>();
				int num = 0;
				if (this.searchCriteria.StartDateUtc != null)
				{
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
				}
				if (this.searchCriteria.EndDateUtc != null)
				{
					ExDateTime exDateTime2 = new ExDateTime(ExTimeZone.UtcTimeZone, this.searchCriteria.EndDateUtc.Value);
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
								Value = exDateTime2.ToISOString()
							}
						}
					});
					num++;
				}
				this.searchCriteria.QueryComplexity = num;
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

		private QueryStringType GenerateFASTSearchQueryString()
		{
			TaskLogger.LogEnter();
			QueryStringType result;
			try
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				int num = 0;
				if (this.searchCriteria.StartDateUtc != null)
				{
					DateTime date = this.searchCriteria.StartDateUtc.Value.ToLocalTime();
					AqsQueryBuilder.AppendDateClause(stringBuilder, PropertyKeyword.Received, DateRangeQueryOperation.GreaterThanOrEqual, date);
					num++;
				}
				if (this.searchCriteria.EndDateUtc != null)
				{
					DateTime date2 = this.searchCriteria.EndDateUtc.Value.ToLocalTime();
					AqsQueryBuilder.AppendDateClause(stringBuilder, PropertyKeyword.Received, DateRangeQueryOperation.LessThan, date2);
					num++;
				}
				if (this.searchCriteria.Succeeded != null)
				{
					string text = string.Format(CultureInfo.InvariantCulture, "{0}: {1}", new object[]
					{
						"Succeeded",
						this.searchCriteria.Succeeded.Value
					});
					AqsQueryBuilder.AppendKeywordOrClause(stringBuilder, PropertyKeyword.Body, new string[]
					{
						text
					});
					num++;
				}
				if (this.searchCriteria.ExternalAccess != null)
				{
					string text2 = string.Format(CultureInfo.InvariantCulture, "{0}: {1}", new object[]
					{
						"ExternalAccess",
						this.searchCriteria.ExternalAccess.Value
					});
					AqsQueryBuilder.AppendKeywordOrClause(stringBuilder, PropertyKeyword.Body, new string[]
					{
						text2
					});
					num++;
				}
				if (this.searchCriteria.Cmdlets != null && this.searchCriteria.Cmdlets.Count > 0)
				{
					string[] array = new string[this.searchCriteria.Cmdlets.Count];
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = string.Format(CultureInfo.InvariantCulture, "{0}: {1}", new object[]
						{
							"Cmdlet Name",
							this.searchCriteria.Cmdlets[i]
						});
						num++;
					}
					AqsQueryBuilder.AppendKeywordOrClause(stringBuilder, PropertyKeyword.Body, array);
				}
				if (this.searchCriteria.Parameters != null && this.searchCriteria.Parameters.Count > 0)
				{
					string[] array2 = new string[this.searchCriteria.Parameters.Count];
					for (int j = 0; j < array2.Length; j++)
					{
						array2[j] = string.Format(CultureInfo.InvariantCulture, "{0}: {1}", new object[]
						{
							"Parameter",
							this.searchCriteria.Parameters[j]
						});
						num++;
					}
					AqsQueryBuilder.AppendKeywordOrClause(stringBuilder, PropertyKeyword.Body, array2);
				}
				if (this.searchCriteria.ObjectIds != null && this.searchCriteria.ObjectIds.Count > 0)
				{
					string[] array3 = new string[this.searchCriteria.ObjectIds.Count];
					for (int k = 0; k < this.searchCriteria.ObjectIds.Count; k++)
					{
						array3[k] = this.searchCriteria.ObjectIds[k] + AdminAuditLogSearchWorker.LogID;
						num++;
					}
					AqsQueryBuilder.AppendKeywordOrClause(stringBuilder, PropertyKeyword.To, array3);
				}
				if (this.searchCriteria.ResolvedUsers != null && this.searchCriteria.ResolvedUsers.Count > 0)
				{
					string[] array4 = new string[this.searchCriteria.ResolvedUsers.Count];
					for (int l = 0; l < this.searchCriteria.ResolvedUsers.Count; l++)
					{
						array4[l] = this.searchCriteria.ResolvedUsers[l] + AdminAuditLogSearchWorker.LogID;
						num++;
					}
					AqsQueryBuilder.AppendKeywordOrClause(stringBuilder, PropertyKeyword.From, array4);
				}
				this.searchCriteria.QueryComplexity = num;
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

		private static readonly Trace Tracer = ExTraceGlobals.TraceTracer;

		private readonly int searchTimeoutSeconds;

		private readonly AdminAuditLogSearch searchCriteria;

		private static readonly string LogID = "audit";

		private AuditLogOpticsLogData searchStatistics;

		private class QueryStrategy : IAuditQueryStrategy<AdminAuditLogEvent>
		{
			public QueryStrategy(AdminAuditLogSearch searchCriteria)
			{
				this.searchCriteria = searchCriteria;
				this.currentIndex = 0;
			}

			public bool RecordFilter(IReadOnlyPropertyBag propertyBag, out bool stopNow)
			{
				stopNow = false;
				ObjectId objectId = propertyBag[ItemSchema.Id] as ObjectId;
				string value = propertyBag[ItemSchema.TextBody] as string;
				bool result = objectId != null && !string.IsNullOrEmpty(value) && this.currentIndex >= this.searchCriteria.StartIndex;
				this.currentIndex++;
				return result;
			}

			public AdminAuditLogEvent Convert(IReadOnlyPropertyBag propertyBag)
			{
				ObjectId storeId = propertyBag[ItemSchema.Id] as ObjectId;
				AdminAuditLogEventId identity = new AdminAuditLogEventId(storeId);
				return new AdminAuditLogEvent(identity, propertyBag[ItemSchema.TextBody] as string);
			}

			public Exception GetTimeoutException(TimeSpan timeout)
			{
				return new AdminAuditLogSearchException(Strings.AdminAuditLogSearchTimeout(timeout.TotalSeconds.ToString()), this.searchCriteria);
			}

			public Exception GetQueryFailedException()
			{
				TaskLogger.Trace("AdminAuditLog CI search failed because CI is not running, search query was not served by CI", new object[0]);
				return new AdminAuditLogSearchException(Strings.AdminAuditLogSearchFailed, this.searchCriteria);
			}

			private AdminAuditLogSearch searchCriteria;

			private int currentIndex;
		}
	}
}
