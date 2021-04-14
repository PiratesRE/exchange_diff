using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class ADInconsistencyCheck
	{
		public static void CleanADOrphanAndInconsistency()
		{
			PartitionId[] allAccountPartitionIds = ADAccountPartitionLocator.GetAllAccountPartitionIds();
			for (int i = 0; i < allAccountPartitionIds.Length; i++)
			{
				PartitionId partitionId = allAccountPartitionIds[i];
				CommonUtils.CheckForServiceStopping();
				ADSessionSettings adsessionSettings = ADSessionSettings.FromAllTenantsPartitionId(partitionId);
				adsessionSettings.IncludeSoftDeletedObjects = true;
				adsessionSettings.IncludeInactiveMailbox = true;
				IRecipientSession recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, false, ConsistencyMode.PartiallyConsistent, null, adsessionSettings, 45, "CleanADOrphanAndInconsistency", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\Service\\ADInconsistencyCheck.cs");
				IConfigurationSession configSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(null, false, ConsistencyMode.PartiallyConsistent, null, adsessionSettings, 52, "CleanADOrphanAndInconsistency", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\Service\\ADInconsistencyCheck.cs");
				using (List<Guid>.Enumerator enumerator = MapiUtils.GetDatabasesOnThisServer().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Guid dbGuid = enumerator.Current;
						CommonUtils.CheckForServiceStopping();
						DatabaseInformation databaseInformation = MapiUtils.FindServerForMdb(dbGuid, null, null, FindServerFlags.AllowMissing);
						if (databaseInformation.IsMissing)
						{
							MrsTracer.Service.Debug("CleanADOrphanAndInconsistency Database {0} in Forest {1} is missing, skip it", new object[]
							{
								dbGuid,
								databaseInformation.ForestFqdn
							});
						}
						else
						{
							CommonUtils.CatchKnownExceptions(delegate
							{
								ADInconsistencyCheck.CleanADOrphanAndInconsistencyForRequests(recipientSession, configSession, dbGuid);
							}, delegate(Exception f)
							{
								MrsTracer.Service.Error("CleanADOrphanAndInconsistency() failed for DB {0}. Error: {1}", new object[]
								{
									dbGuid,
									CommonUtils.FullExceptionMessage(f, true)
								});
								MRSService.LogEvent(MRSEventLogConstants.Tuple_ScanADInconsistencyRequestFailEvent, new object[]
								{
									dbGuid,
									string.Empty,
									string.Empty,
									CommonUtils.FullExceptionMessage(f, true)
								});
							});
						}
					}
				}
			}
		}

		private static void CleanADOrphanAndInconsistencyForMoves(IRecipientSession recipientSession, RequestJobProvider rjProvider, Guid dbGuid)
		{
			ADObjectId propertyValue = new ADObjectId(dbGuid);
			QueryFilter queryFilter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.NotEqual, ADUserSchema.MailboxMoveStatus, RequestStatus.None),
				new ExistsFilter(ADUserSchema.MailboxMoveStatus)
			});
			QueryFilter queryFilter2 = QueryFilter.OrTogether(new QueryFilter[]
			{
				QueryFilter.AndTogether(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.MailboxMoveSourceMDB, propertyValue),
					new BitMaskAndFilter(ADUserSchema.MailboxMoveFlags, 4UL)
				}),
				QueryFilter.AndTogether(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.MailboxMoveTargetMDB, propertyValue),
					new BitMaskAndFilter(ADUserSchema.MailboxMoveFlags, 8UL)
				})
			});
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				queryFilter,
				queryFilter2
			});
			ADRecipient[] array = recipientSession.Find(null, QueryScope.SubTree, filter, null, 1000);
			if (array == null || array.Length == 0)
			{
				MrsTracer.Service.Debug("CleanADOrphanAndInconsistencyForMoves(): No move requests found for DB {0}", new object[]
				{
					dbGuid
				});
				return;
			}
			bool needUpdateAD = false;
			ADRecipient[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				ADRecipient request = array2[i];
				CommonUtils.CatchKnownExceptions(delegate
				{
					needUpdateAD = false;
					ADUser aduser = request as ADUser;
					MoveRequestStatistics moveRequestStatistics = (MoveRequestStatistics)rjProvider.Read<MoveRequestStatistics>(new RequestJobObjectId(aduser.ExchangeGuid, dbGuid, null));
					if (moveRequestStatistics == null || (!moveRequestStatistics.IsFake && moveRequestStatistics.Status == RequestStatus.None))
					{
						MrsTracer.Service.Debug("CleanADOrphanAndInconsistencyForMoves():AD orphan {0} found for DB {1}", new object[]
						{
							aduser.ExchangeGuid,
							dbGuid
						});
						CommonUtils.CleanupMoveRequestInAD(recipientSession, null, aduser);
						return;
					}
					if (moveRequestStatistics.IsFake)
					{
						MrsTracer.Service.Debug("CleanADOrphanAndInconsistencyForMoves():request {0} is uplevel/Fake, ignore it", new object[]
						{
							aduser.ExchangeGuid
						});
						return;
					}
					if (moveRequestStatistics.User == null)
					{
						MrsTracer.Service.Debug("CleanADOrphanAndInconsistencyForMoves():request {0} has been deleted from AD, ignore it", new object[]
						{
							aduser.ExchangeGuid
						});
						return;
					}
					if (moveRequestStatistics.TimeTracker.GetCurrentDurationChunk() > ADInconsistencyCheck.updateLatencyThreshhold)
					{
						RequestStatus versionAppropriateStatus = RequestJobBase.GetVersionAppropriateStatus(moveRequestStatistics.Status, aduser.ExchangeVersion);
						if (versionAppropriateStatus != moveRequestStatistics.User.MailboxMoveStatus)
						{
							MrsTracer.Service.Debug("CleanADOrphanAndInconsistencyForMoves():AD inconsistency {0} found for DB {1}, AD status is {2} but store status is {3} ({4})", new object[]
							{
								aduser.ExchangeGuid,
								dbGuid,
								aduser.MailboxMoveStatus,
								moveRequestStatistics.Status,
								versionAppropriateStatus
							});
							aduser.MailboxMoveStatus = versionAppropriateStatus;
							needUpdateAD = true;
						}
						RequestFlags versionAppropriateFlags = RequestJobBase.GetVersionAppropriateFlags(moveRequestStatistics.Flags, aduser.ExchangeVersion);
						if (versionAppropriateFlags != moveRequestStatistics.User.MailboxMoveFlags)
						{
							MrsTracer.Service.Debug("CleanADOrphanAndInconsistencyForMoves():AD inconsistency {0} found for DB {1}, AD flags is {2} but store flags is {3} ({4})", new object[]
							{
								aduser.ExchangeGuid,
								dbGuid,
								aduser.MailboxMoveFlags,
								moveRequestStatistics.Flags,
								versionAppropriateFlags
							});
							aduser.MailboxMoveFlags = versionAppropriateFlags;
							needUpdateAD = true;
						}
						if (needUpdateAD)
						{
							recipientSession.Save(request);
						}
					}
				}, delegate(Exception f)
				{
					MrsTracer.Service.Error("CleanADOrphanAndInconsistencyForMoves() failed for DB {0}. move request {1} Error: {2}", new object[]
					{
						dbGuid,
						((ADUser)request).ExchangeGuid,
						CommonUtils.FullExceptionMessage(f, true)
					});
					MRSService.LogEvent(MRSEventLogConstants.Tuple_ScanADInconsistencyRequestFailEvent, new object[]
					{
						dbGuid,
						MRSRequestType.Move.ToString(),
						((ADUser)request).ExchangeGuid,
						CommonUtils.FullExceptionMessage(f, true)
					});
				});
			}
		}

		private static void CleanADOrphanAndInconsistencyForNonMoves(IConfigurationSession configSession, MRSRequestType requestType, RequestJobProvider rjProvider, Guid dbGuid)
		{
			ADObjectId requestQueueId = new ADObjectId(dbGuid);
			RequestIndexEntryQueryFilter requestIndexEntryQueryFilter = new RequestIndexEntryQueryFilter();
			requestIndexEntryQueryFilter.IndexId = new RequestIndexId(RequestIndexLocation.AD);
			requestIndexEntryQueryFilter.RequestType = requestType;
			requestIndexEntryQueryFilter.RequestQueueId = requestQueueId;
			MRSRequestWrapper[] array = ADHandler.Find(configSession, requestIndexEntryQueryFilter, null, true, null);
			if (array != null && array.Length > 0)
			{
				bool needUpdateAD = false;
				MRSRequestWrapper[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					MRSRequestWrapper request = array2[i];
					CommonUtils.CatchKnownExceptions(delegate
					{
						needUpdateAD = false;
						if (request.Status != RequestStatus.None)
						{
							RequestStatisticsBase requestStatisticsBase = (RequestStatisticsBase)rjProvider.Read<RequestStatisticsBase>(new RequestJobObjectId(request.RequestGuid, dbGuid, null));
							if (requestStatisticsBase == null || (!requestStatisticsBase.IsFake && requestStatisticsBase.Status == RequestStatus.None))
							{
								MrsTracer.Service.Debug("CleanADOrphanAndInconsistencyForRequests():AD orphan {0} found for DB {1}", new object[]
								{
									request.RequestGuid,
									dbGuid
								});
								ADHandler.Delete(configSession, request);
								return;
							}
							if (requestStatisticsBase.IsFake)
							{
								MrsTracer.Service.Debug("CleanADOrphanAndInconsistencyForRequests():request {0} is uplevel/Fake, ignoring it", new object[]
								{
									request.RequestGuid
								});
								return;
							}
							if (requestStatisticsBase.IndexEntries == null || requestStatisticsBase.IndexEntries.Count == 0)
							{
								MrsTracer.Service.Debug("CleanADOrphanAndInconsistencyForRequests():request {0} has been removed from AD, ignoring it", new object[]
								{
									request.RequestGuid
								});
								return;
							}
							if (requestStatisticsBase.TimeTracker.GetCurrentDurationChunk() > ADInconsistencyCheck.updateLatencyThreshhold)
							{
								if (requestStatisticsBase.Status != requestStatisticsBase.IndexEntries[0].Status)
								{
									MrsTracer.Service.Debug("CleanADOrphanAndInconsistencyForRequests():AD inconsistency {0} found for DB {1}, AD status is {2} but store status is {3}", new object[]
									{
										request.RequestGuid,
										dbGuid,
										request.Status,
										requestStatisticsBase.Status
									});
									request.Status = requestStatisticsBase.Status;
									needUpdateAD = true;
								}
								if (requestStatisticsBase.Flags != requestStatisticsBase.IndexEntries[0].Flags)
								{
									MrsTracer.Service.Debug("CleanADOrphanAndInconsistencyForRequests():AD inconsistency {0} found for DB {1}, AD flags is {2} but store flags is {3}", new object[]
									{
										request.RequestGuid,
										dbGuid,
										request.Flags,
										requestStatisticsBase.Flags
									});
									request.Flags = requestStatisticsBase.Flags;
									needUpdateAD = true;
								}
								if (needUpdateAD)
								{
									ADHandler.Save(configSession, request);
									return;
								}
							}
						}
						else
						{
							MrsTracer.Service.Debug("CleanADOrphanAndInconsistencyForRequests():request {0} for {1} has been removed from AD, store orphan will be handled by MRS pick up job logic", new object[]
							{
								request.RequestGuid,
								dbGuid
							});
						}
					}, delegate(Exception f)
					{
						MrsTracer.Service.Error("CleanADOrphanAndInconsistencyForRequests() failed for DB {0}. Request type {1} Request guid {2} Error: {3}", new object[]
						{
							dbGuid,
							requestType.ToString(),
							request.RequestGuid,
							CommonUtils.FullExceptionMessage(f, true)
						});
						MRSService.LogEvent(MRSEventLogConstants.Tuple_ScanADInconsistencyRequestFailEvent, new object[]
						{
							dbGuid,
							requestType.ToString(),
							request.RequestGuid,
							CommonUtils.FullExceptionMessage(f, true)
						});
					});
				}
				return;
			}
			MrsTracer.Service.Debug("CleanADOrphanAndInconsistencyForRequests(): No {0} requests found for DB {1}", new object[]
			{
				requestType,
				dbGuid
			});
		}

		private static void CleanADOrphanAndInconsistencyForRequests(IRecipientSession recipientSession, IConfigurationSession configSession, Guid dbGuid)
		{
			MrsTracer.Service.Debug("Try to CleanADOrphanAndInconsistencyForRequests for Database {0} name is {1}", new object[]
			{
				dbGuid,
				dbGuid
			});
			using (RequestJobProvider rjProvider = new RequestJobProvider(dbGuid))
			{
				rjProvider.LoadReport = false;
				rjProvider.AllowInvalid = true;
				foreach (object obj in Enum.GetValues(typeof(MRSRequestType)))
				{
					MRSRequestType requestType = (MRSRequestType)obj;
					MrsTracer.Service.Debug("Try to CleanADOrphanAndInconsistencyForRequests for request type {0}", new object[]
					{
						requestType
					});
					CommonUtils.CatchKnownExceptions(delegate
					{
						if (requestType == MRSRequestType.Move)
						{
							ADInconsistencyCheck.CleanADOrphanAndInconsistencyForMoves(recipientSession, rjProvider, dbGuid);
							return;
						}
						ADInconsistencyCheck.CleanADOrphanAndInconsistencyForNonMoves(configSession, requestType, rjProvider, dbGuid);
					}, delegate(Exception f)
					{
						MrsTracer.Service.Error("CleanADOrphanAndInconsistencyForRequests() failed for DB {0}. Request type {1} Error: {2}", new object[]
						{
							dbGuid,
							requestType.ToString(),
							CommonUtils.FullExceptionMessage(f, true)
						});
						MRSService.LogEvent(MRSEventLogConstants.Tuple_ScanADInconsistencyRequestFailEvent, new object[]
						{
							dbGuid,
							requestType.ToString(),
							string.Empty,
							CommonUtils.FullExceptionMessage(f, true)
						});
					});
				}
			}
		}

		private static TimeSpan updateLatencyThreshhold = TimeSpan.FromMinutes(30.0);
	}
}
