using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RequestJobProvider : DisposeTrackableBase, IConfigDataProvider
	{
		public RequestJobProvider(Guid mdbGuid) : this(mdbGuid, null)
		{
		}

		public RequestJobProvider(Guid mdbGuid, MapiStore mdbSession)
		{
			this.MdbGuid = mdbGuid;
			this.store = mdbSession;
			this.ownsStore = (mdbSession == null);
			this.IndexProvider = new RequestIndexEntryProvider();
		}

		public RequestJobProvider(IRecipientSession recipSession, IConfigurationSession configSession)
		{
			this.MdbGuid = Guid.Empty;
			this.store = null;
			this.ownsStore = true;
			this.IndexProvider = new RequestIndexEntryProvider(recipSession, configSession);
		}

		public bool LoadReport { get; set; }

		public bool AllowInvalid { get; set; }

		public Guid MdbGuid { get; private set; }

		public string Source
		{
			get
			{
				return ((IConfigDataProvider)this.IndexProvider).Source;
			}
		}

		public RequestIndexEntryProvider IndexProvider { get; private set; }

		internal MapiStore SystemMailbox
		{
			get
			{
				this.EnsureStoreConnectionExists(this.MdbGuid);
				return this.store;
			}
		}

		public void AttachToMDB(Guid mdbGuid)
		{
			this.EnsureStoreConnectionExists(mdbGuid);
		}

		public LocalizedString ComputePositionInQueue(Guid requestGuid)
		{
			int crows = 1000;
			int num = 0;
			int num2 = 0;
			if (requestGuid.Equals(Guid.Empty))
			{
				return MrsStrings.ErrorEmptyMailboxGuid;
			}
			using (MapiFolder requestJobsFolder = RequestJobXML.GetRequestJobsFolder(this.SystemMailbox))
			{
				using (MapiTable contentsTable = requestJobsFolder.GetContentsTable(ContentsTableFlags.DeferredErrors))
				{
					RequestJobNamedPropertySet requestJobNamedPropertySet = RequestJobNamedPropertySet.Get(this.SystemMailbox);
					contentsTable.SetColumns(requestJobNamedPropertySet.PropTags);
					SortOrder sortOrder = new SortOrder(requestJobNamedPropertySet.PropTags[7], SortFlags.Ascend);
					contentsTable.SortTable(sortOrder, SortTableFlags.None);
					Restriction restriction = Restriction.EQ(requestJobNamedPropertySet.PropTags[0], RequestStatus.Queued);
					contentsTable.Restrict(restriction);
					contentsTable.SeekRow(BookMark.Beginning, 0);
					PropValue[][] array = contentsTable.QueryRows(crows);
					bool flag = false;
					while (array != null && array.Length > 0)
					{
						int num3 = 0;
						if (!flag)
						{
							foreach (PropValue[] array3 in array)
							{
								num3++;
								if (num2 + num3 >= 10000)
								{
									break;
								}
								Guid value = MapiUtils.GetValue<Guid>(array3[26], Guid.Empty);
								if (requestGuid.Equals(value))
								{
									flag = true;
									num = num2 + num3;
									break;
								}
							}
						}
						num2 += array.Length;
						if (num2 >= 10000)
						{
							break;
						}
						array = contentsTable.QueryRows(crows);
					}
				}
			}
			LocalizedString value2 = LocalizedString.Empty;
			LocalizedString value3 = LocalizedString.Empty;
			if (num == 0)
			{
				if (num2 < 10000)
				{
					value2 = MrsStrings.MoveRequestNotFoundInQueue;
					value3 = MrsStrings.PositionInteger(num2);
				}
				else
				{
					value2 = MrsStrings.PositionIntegerPlus(10000);
					value3 = MrsStrings.PositionIntegerPlus(10000);
				}
			}
			else
			{
				value2 = MrsStrings.PositionInteger(num);
				if (num2 < 10000)
				{
					value3 = MrsStrings.PositionInteger(num2);
				}
				else
				{
					value3 = MrsStrings.PositionIntegerPlus(num2);
				}
			}
			return MrsStrings.PositionOfMoveRequestInSystemMailboxQueue(value2, value3);
		}

		public int GetQueueLength()
		{
			MrsTracer.Common.Function("RequestJobProvider.GetQueueLength", new object[0]);
			RequestJobNamedPropertySet requestJobNamedPropertySet = RequestJobNamedPropertySet.Get(this.SystemMailbox);
			Restriction restriction = Restriction.EQ(requestJobNamedPropertySet.PropTags[4], true);
			Restriction restriction2 = Restriction.BitMaskNonZero(requestJobNamedPropertySet.PropTags[10], 256);
			Restriction restriction3 = Restriction.EQ(requestJobNamedPropertySet.PropTags[0], RequestStatus.Completed);
			Restriction restriction4 = Restriction.EQ(requestJobNamedPropertySet.PropTags[0], RequestStatus.CompletedWithWarning);
			Restriction restriction5 = Restriction.Or(new Restriction[]
			{
				restriction,
				restriction2,
				restriction3,
				restriction4
			});
			Restriction restriction6 = Restriction.Not(restriction5);
			int result;
			using (MapiFolder requestJobsFolder = RequestJobXML.GetRequestJobsFolder(this.SystemMailbox))
			{
				using (MapiTable contentsTable = requestJobsFolder.GetContentsTable(ContentsTableFlags.DeferredErrors))
				{
					contentsTable.Restrict(restriction6);
					int rowCount = contentsTable.GetRowCount();
					MrsTracer.Common.Debug("Queue length is {0}", new object[]
					{
						rowCount
					});
					result = rowCount;
				}
			}
			return result;
		}

		public IConfigurable[] Find<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			List<IConfigurable> list = new List<IConfigurable>();
			foreach (T t in this.InternalFind<T>(filter, rootId, deepSearch, sortBy, -1))
			{
				list.Add(t);
			}
			return list.ToArray();
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			return this.InternalFind<T>(filter, rootId, deepSearch, sortBy, pageSize);
		}

		public IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			return this.Read<T>(identity, ReadJobFlags.None);
		}

		public IConfigurable Read<T>(ObjectId identity, ReadJobFlags readJobFlags) where T : IConfigurable, new()
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity", "The identity of the object to be read must be specified.");
			}
			if (!(identity is RequestJobObjectId))
			{
				throw new ArgumentException("RequestJobProvider can only identify RequestJobs based on RequestJobObjectIds.", "identity");
			}
			if (!typeof(T).Equals(typeof(TransactionalRequestJob)) && !RequestJobProvider.IsRequestStatistics(typeof(T), true))
			{
				throw new ArgumentException("RequestJobProvider can only Read *RequestStatistics or TransactionalRequestJob objects.");
			}
			bool flag = false;
			if (typeof(T).Equals(typeof(TransactionalRequestJob)))
			{
				flag = true;
			}
			RequestJobObjectId requestJobObjectId = (RequestJobObjectId)identity;
			Guid requestGuid = requestJobObjectId.RequestGuid;
			Guid mdbGuid = requestJobObjectId.MdbGuid;
			byte[] messageId = requestJobObjectId.MessageId;
			ADUser aduser = requestJobObjectId.User;
			ADUser aduser2 = requestJobObjectId.SourceUser;
			ADUser aduser3 = requestJobObjectId.TargetUser;
			IRequestIndexEntry indexEntry = requestJobObjectId.IndexEntry;
			List<IRequestIndexEntry> list = new List<IRequestIndexEntry>();
			if (requestGuid == Guid.Empty || mdbGuid == Guid.Empty)
			{
				throw new NotEnoughInformationToFindMoveRequestPermanentException();
			}
			this.EnsureStoreConnectionExists(mdbGuid);
			MoveObjectInfo<RequestJobXML> moveObjectInfo = null;
			IConfigurable result;
			try
			{
				moveObjectInfo = new MoveObjectInfo<RequestJobXML>(mdbGuid, this.store, messageId, RequestJobXML.RequestJobsFolderName, RequestJobXML.RequestJobsMessageClass, RequestJobXML.CreateMessageSubject(requestGuid), RequestJobXML.CreateMessageSearchKey(requestGuid));
				RequestJobXML requestJobXML = null;
				if (moveObjectInfo.OpenMessage())
				{
					if (moveObjectInfo.CheckObjectType(new MoveObjectInfo<RequestJobXML>.IsSupportedObjectTypeDelegate(RequestJobXML.IsMessageTypeSupported)))
					{
						requestJobXML = moveObjectInfo.ReadObject(ReadObjectFlags.DontThrowOnCorruptData);
					}
					else
					{
						MrsTracer.Common.Warning("Found unexpected JobType for move job {0}", new object[]
						{
							requestJobObjectId.ToString()
						});
					}
					if (requestJobXML == null)
					{
						if (!this.AllowInvalid)
						{
							return null;
						}
						requestJobXML = RequestJobBase.CreateDummyObject<RequestJobXML>();
						requestJobXML.RequestGuid = requestGuid;
						requestJobXML.ExchangeGuid = requestGuid;
					}
					requestJobXML.OriginatingMDBGuid = mdbGuid;
					if (requestJobXML.Identity == null)
					{
						requestJobXML.Identity = requestJobObjectId;
					}
					requestJobXML.Identity.MessageId = moveObjectInfo.MessageId;
					RequestJobProvider.FixTenantInfo(requestJobXML);
					if (!requestJobXML.IsFake)
					{
						using (this.IndexProvider.RescopeTo(requestJobXML.DomainControllerToUpdate, requestJobXML.OrganizationId))
						{
							if (aduser == null && requestJobXML.UserId != null)
							{
								aduser = this.IndexProvider.ReadADUser(requestJobXML.UserId, requestJobXML.ExchangeGuid);
							}
							if (aduser2 == null && requestJobXML.SourceUserId != null)
							{
								aduser2 = this.IndexProvider.ReadADUser(requestJobXML.SourceUserId, requestJobXML.SourceExchangeGuid);
							}
							if (aduser3 == null && requestJobXML.TargetUserId != null)
							{
								aduser3 = this.IndexProvider.ReadADUser(requestJobXML.TargetUserId, requestJobXML.TargetExchangeGuid);
							}
							if (!typeof(T).Equals(typeof(MoveRequestStatistics)) && requestJobXML.RequestType != MRSRequestType.Move && requestJobXML.IndexIds != null && requestJobXML.IndexIds.Count > 0)
							{
								int capacity = requestJobXML.IndexIds.Count - 1;
								bool flag2 = false;
								List<RequestIndexEntryObjectId> list2 = new List<RequestIndexEntryObjectId>(capacity);
								foreach (RequestIndexId requestIndexId in requestJobXML.IndexIds)
								{
									if (indexEntry != null && requestIndexId.Equals(indexEntry.RequestIndexId))
									{
										if (!flag2)
										{
											list.Add(indexEntry);
										}
										flag2 = true;
									}
									else if (readJobFlags.HasFlag(ReadJobFlags.SkipReadingMailboxRequestIndexEntries) && requestIndexId.Location == RequestIndexLocation.Mailbox)
									{
										MrsTracer.Common.Debug("Skipping loading of an IRequestIndexEntry found in a mailbox.", new object[0]);
									}
									else
									{
										list2.Add(new RequestIndexEntryObjectId(requestJobXML.RequestGuid, requestJobXML.TargetExchangeGuid, requestJobXML.RequestType, requestJobXML.OrganizationId, requestIndexId, null));
									}
								}
								foreach (RequestIndexEntryObjectId objectId in list2)
								{
									IRequestIndexEntry requestIndexEntry = this.IndexProvider.Read(objectId);
									if (requestIndexEntry != null)
									{
										list.Add(requestIndexEntry);
									}
								}
							}
							if (this.IndexProvider.DomainController == null && !string.IsNullOrEmpty(requestJobXML.DomainControllerToUpdate))
							{
								requestJobXML.DomainControllerToUpdate = null;
							}
						}
					}
					requestJobXML.User = aduser;
					requestJobXML.SourceUser = aduser2;
					requestJobXML.TargetUser = aduser3;
					requestJobXML.IndexEntries = list;
					if (!readJobFlags.HasFlag(ReadJobFlags.SkipValidation))
					{
						requestJobXML.ValidateRequestJob();
					}
					if (this.AllowInvalid)
					{
						ValidationError[] array = requestJobXML.Validate();
						if (array != null && array.Length > 0)
						{
							requestJobXML.IsFake = true;
						}
					}
					if (flag)
					{
						TransactionalRequestJob transactionalRequestJob = new TransactionalRequestJob(requestJobXML);
						requestJobXML.Retire();
						transactionalRequestJob.Provider = this;
						transactionalRequestJob.MoveObject = moveObjectInfo;
						moveObjectInfo = null;
						result = transactionalRequestJob;
					}
					else
					{
						RequestStatisticsBase requestStatisticsBase = RequestJobProvider.CreateRequestStatistics(typeof(T), requestJobXML, true);
						if (requestStatisticsBase == null)
						{
							requestStatisticsBase = new MoveRequestStatistics(requestJobXML);
							requestJobXML.Retire();
						}
						if (this.LoadReport)
						{
							ReportData reportData = new ReportData(requestStatisticsBase.IdentifyingGuid, requestStatisticsBase.ReportVersion);
							reportData.Load(this.SystemMailbox);
							requestStatisticsBase.Report = reportData.ToReport();
						}
						result = requestStatisticsBase;
					}
				}
				else
				{
					result = null;
				}
			}
			finally
			{
				if (moveObjectInfo != null)
				{
					moveObjectInfo.Dispose();
					moveObjectInfo = null;
				}
			}
			return result;
		}

		public void Save(IConfigurable instance)
		{
			if (!(instance is TransactionalRequestJob))
			{
				throw new ArgumentException("RequestJobProvider can only Save TransactionalRequestJob objects.", "instance");
			}
			this.SaveOrDelete((TransactionalRequestJob)instance, true);
		}

		public void Delete(IConfigurable instance)
		{
			if (!(instance is TransactionalRequestJob))
			{
				throw new ArgumentException("RequestJobProvider can only Delete TransactionalRequestJob objects.", "instance");
			}
			this.SaveOrDelete((TransactionalRequestJob)instance, false);
		}

		public void DeleteIndexEntries(TransactionalRequestJob requestJob)
		{
			if (!requestJob.IsValid)
			{
				throw new ArgumentException("Should not attempt to delete AD data off an invalid request");
			}
			using (this.IndexProvider.RescopeTo(requestJob.DomainControllerToUpdate, requestJob.OrganizationId))
			{
				if (requestJob.RequestType == MRSRequestType.Move)
				{
					if (requestJob.User != null && !requestJob.CancelRequest)
					{
						this.IndexProvider.UpdateADData(requestJob.User, requestJob, false);
						this.IndexProvider.RecipientSession.Save(requestJob.User);
					}
				}
				else if (requestJob.IndexEntries != null)
				{
					foreach (IRequestIndexEntry instance in requestJob.IndexEntries)
					{
						this.IndexProvider.Delete(instance);
					}
				}
			}
		}

		internal static RequestStatisticsBase CreateRequestStatistics(TransactionalRequestJob rj)
		{
			RequestStatisticsBase result = null;
			switch (rj.RequestType)
			{
			case MRSRequestType.Move:
				result = new MoveRequestStatistics(rj);
				break;
			case MRSRequestType.Merge:
				result = new MergeRequestStatistics(rj);
				break;
			case MRSRequestType.MailboxImport:
				result = new MailboxImportRequestStatistics(rj);
				break;
			case MRSRequestType.MailboxExport:
				result = new MailboxExportRequestStatistics(rj);
				break;
			case MRSRequestType.MailboxRestore:
				result = new MailboxRestoreRequestStatistics(rj);
				break;
			case MRSRequestType.PublicFolderMove:
				result = new PublicFolderMoveRequestStatistics(rj);
				break;
			case MRSRequestType.PublicFolderMigration:
				result = new PublicFolderMigrationRequestStatistics(rj);
				break;
			case MRSRequestType.Sync:
				result = new SyncRequestStatistics(rj);
				break;
			case MRSRequestType.MailboxRelocation:
				result = new MailboxRelocationRequestStatistics(rj);
				break;
			case MRSRequestType.FolderMove:
				result = new FolderMoveRequestStatistics(rj);
				break;
			case MRSRequestType.PublicFolderMailboxMigration:
				result = new PublicFolderMailboxMigrationRequestStatistics(rj);
				break;
			}
			return result;
		}

		internal static RequestStatisticsBase CreateRequestStatistics(RequestJobXML rj)
		{
			RequestStatisticsBase requestStatisticsBase = null;
			switch (rj.RequestType)
			{
			case MRSRequestType.Move:
				requestStatisticsBase = new MoveRequestStatistics(rj);
				break;
			case MRSRequestType.Merge:
				requestStatisticsBase = new MergeRequestStatistics(rj);
				break;
			case MRSRequestType.MailboxImport:
				requestStatisticsBase = new MailboxImportRequestStatistics(rj);
				break;
			case MRSRequestType.MailboxExport:
				requestStatisticsBase = new MailboxExportRequestStatistics(rj);
				break;
			case MRSRequestType.MailboxRestore:
				requestStatisticsBase = new MailboxRestoreRequestStatistics(rj);
				break;
			case MRSRequestType.PublicFolderMove:
				requestStatisticsBase = new PublicFolderMoveRequestStatistics(rj);
				break;
			case MRSRequestType.PublicFolderMigration:
				requestStatisticsBase = new PublicFolderMigrationRequestStatistics(rj);
				break;
			case MRSRequestType.Sync:
				requestStatisticsBase = new SyncRequestStatistics(rj);
				break;
			case MRSRequestType.MailboxRelocation:
				requestStatisticsBase = new MailboxRelocationRequestStatistics(rj);
				break;
			case MRSRequestType.FolderMove:
				requestStatisticsBase = new FolderMoveRequestStatistics(rj);
				break;
			case MRSRequestType.PublicFolderMailboxMigration:
				requestStatisticsBase = new PublicFolderMailboxMigrationRequestStatistics(rj);
				break;
			}
			if (requestStatisticsBase != null)
			{
				rj.Retire();
			}
			return requestStatisticsBase;
		}

		internal static RequestStatisticsBase CreateRequestStatistics(Type t, TransactionalRequestJob rj, bool acceptBase)
		{
			RequestStatisticsBase result = null;
			if (t.Equals(typeof(MoveRequestStatistics)))
			{
				result = new MoveRequestStatistics(rj);
			}
			else if (t.Equals(typeof(MergeRequestStatistics)))
			{
				result = new MergeRequestStatistics(rj);
			}
			else if (t.Equals(typeof(MailboxImportRequestStatistics)))
			{
				result = new MailboxImportRequestStatistics(rj);
			}
			else if (t.Equals(typeof(MailboxExportRequestStatistics)))
			{
				result = new MailboxExportRequestStatistics(rj);
			}
			else if (t.Equals(typeof(MailboxRelocationRequestStatistics)))
			{
				result = new MailboxRelocationRequestStatistics(rj);
			}
			else if (t.Equals(typeof(MailboxRestoreRequestStatistics)))
			{
				result = new MailboxRestoreRequestStatistics(rj);
			}
			else if (t.Equals(typeof(PublicFolderMoveRequestStatistics)))
			{
				result = new PublicFolderMoveRequestStatistics(rj);
			}
			else if (t.Equals(typeof(PublicFolderMigrationRequestStatistics)))
			{
				result = new PublicFolderMigrationRequestStatistics(rj);
			}
			else if (t.Equals(typeof(PublicFolderMailboxMigrationRequestStatistics)))
			{
				result = new PublicFolderMailboxMigrationRequestStatistics(rj);
			}
			else if (t.Equals(typeof(SyncRequestStatistics)))
			{
				result = new SyncRequestStatistics(rj);
			}
			else if (t.Equals(typeof(FolderMoveRequestStatistics)))
			{
				result = new FolderMoveRequestStatistics(rj);
			}
			else if (acceptBase && t.Equals(typeof(RequestStatisticsBase)))
			{
				return RequestJobProvider.CreateRequestStatistics(rj);
			}
			return result;
		}

		internal static RequestStatisticsBase CreateRequestStatistics(Type t, RequestJobXML rj, bool acceptBase)
		{
			RequestStatisticsBase requestStatisticsBase = null;
			if (t.Equals(typeof(MoveRequestStatistics)))
			{
				requestStatisticsBase = new MoveRequestStatistics(rj);
			}
			else if (t.Equals(typeof(MergeRequestStatistics)))
			{
				requestStatisticsBase = new MergeRequestStatistics(rj);
			}
			else if (t.Equals(typeof(MailboxImportRequestStatistics)))
			{
				requestStatisticsBase = new MailboxImportRequestStatistics(rj);
			}
			else if (t.Equals(typeof(MailboxExportRequestStatistics)))
			{
				requestStatisticsBase = new MailboxExportRequestStatistics(rj);
			}
			else if (t.Equals(typeof(MailboxRelocationRequestStatistics)))
			{
				requestStatisticsBase = new MailboxRelocationRequestStatistics(rj);
			}
			else if (t.Equals(typeof(MailboxRestoreRequestStatistics)))
			{
				requestStatisticsBase = new MailboxRestoreRequestStatistics(rj);
			}
			else if (t.Equals(typeof(PublicFolderMoveRequestStatistics)))
			{
				requestStatisticsBase = new PublicFolderMoveRequestStatistics(rj);
			}
			else if (t.Equals(typeof(PublicFolderMigrationRequestStatistics)))
			{
				requestStatisticsBase = new PublicFolderMigrationRequestStatistics(rj);
			}
			else if (t.Equals(typeof(PublicFolderMailboxMigrationRequestStatistics)))
			{
				requestStatisticsBase = new PublicFolderMailboxMigrationRequestStatistics(rj);
			}
			else if (t.Equals(typeof(SyncRequestStatistics)))
			{
				requestStatisticsBase = new SyncRequestStatistics(rj);
			}
			else if (t.Equals(typeof(FolderMoveRequestStatistics)))
			{
				requestStatisticsBase = new FolderMoveRequestStatistics(rj);
			}
			else if (acceptBase && t.Equals(typeof(RequestStatisticsBase)))
			{
				return RequestJobProvider.CreateRequestStatistics(rj);
			}
			if (requestStatisticsBase != null)
			{
				rj.Retire();
			}
			return requestStatisticsBase;
		}

		internal static bool IsRequestStatistics(Type t, bool acceptBase)
		{
			return (acceptBase && t.Equals(typeof(RequestStatisticsBase))) || t.Equals(typeof(MoveRequestStatistics)) || t.Equals(typeof(MailboxImportRequestStatistics)) || t.Equals(typeof(MailboxExportRequestStatistics)) || t.Equals(typeof(MailboxRelocationRequestStatistics)) || t.Equals(typeof(MailboxRestoreRequestStatistics)) || t.Equals(typeof(MergeRequestStatistics)) || t.Equals(typeof(PublicFolderMoveRequestStatistics)) || t.Equals(typeof(PublicFolderMigrationRequestStatistics)) || t.Equals(typeof(PublicFolderMailboxMigrationRequestStatistics)) || t.Equals(typeof(FolderMoveRequestStatistics)) || t.Equals(typeof(SyncRequestStatistics));
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<RequestJobProvider>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.store != null && this.ownsStore)
			{
				this.store.Dispose();
				this.store = null;
				this.MdbGuid = Guid.Empty;
			}
		}

		private static void UpdateIndexEntryData(IRequestIndexEntry indexEntry, TransactionalRequestJob requestJob)
		{
			if (indexEntry != null && requestJob != null)
			{
				indexEntry.Status = requestJob.Status;
				indexEntry.Flags = requestJob.Flags;
				indexEntry.SourceMDB = requestJob.SourceDatabase;
				indexEntry.StorageMDB = requestJob.WorkItemQueueMdb;
				indexEntry.TargetMDB = requestJob.TargetDatabase;
				indexEntry.SourceUserId = requestJob.SourceUserId;
				indexEntry.TargetUserId = requestJob.TargetUserId;
				indexEntry.FilePath = requestJob.FilePath;
				indexEntry.RemoteHostName = requestJob.RemoteHostName;
				indexEntry.BatchName = requestJob.BatchName;
				indexEntry.RequestGuid = requestJob.Identity.RequestGuid;
				AggregatedAccountConfigurationWrapper aggregatedAccountConfigurationWrapper = indexEntry as AggregatedAccountConfigurationWrapper;
				if (aggregatedAccountConfigurationWrapper != null)
				{
					aggregatedAccountConfigurationWrapper.UpdateData(requestJob);
				}
			}
		}

		private static void FixTenantInfo(RequestJobXML requestJobXML)
		{
			if (!CommonUtils.IsMultiTenantEnabled())
			{
				requestJobXML.OrganizationId = OrganizationId.ForestWideOrgId;
				return;
			}
			if (requestJobXML.OrganizationId == OrganizationId.ForestWideOrgId && requestJobXML.ExternalDirectoryOrganizationId == Guid.Empty && requestJobXML.PartitionHint == null)
			{
				return;
			}
			if (requestJobXML.OrganizationId != OrganizationId.ForestWideOrgId && requestJobXML.PartitionHint == null)
			{
				if (requestJobXML.ExternalDirectoryOrganizationId != Guid.Empty)
				{
					requestJobXML.OrganizationId = OrganizationId.FromExternalDirectoryOrganizationId(requestJobXML.ExternalDirectoryOrganizationId);
				}
				requestJobXML.PartitionHint = TenantPartitionHint.FromOrganizationId(requestJobXML.OrganizationId);
				requestJobXML.ExternalDirectoryOrganizationId = requestJobXML.PartitionHint.GetExternalDirectoryOrganizationId();
				return;
			}
			if (requestJobXML.PartitionHint != null)
			{
				if (requestJobXML.ExternalDirectoryOrganizationId == Guid.Empty)
				{
					requestJobXML.ExternalDirectoryOrganizationId = requestJobXML.PartitionHint.GetExternalDirectoryOrganizationId();
				}
				requestJobXML.OrganizationId = OrganizationId.FromExternalDirectoryOrganizationId(requestJobXML.ExternalDirectoryOrganizationId);
			}
		}

		private void SaveOrDelete(TransactionalRequestJob requestJob, bool save)
		{
			if (!save && (requestJob.MoveObject == null || requestJob.Provider == null || requestJob.Provider != this))
			{
				throw new ArgumentException("RequestJobProvider can only Delete TransactionalRequestJob objects created by this RequestJobProvider.", "instance");
			}
			if (requestJob.Provider != null && requestJob.Provider != this)
			{
				throw new ArgumentException("RequestJobProvider can only work on TransactionalRequestJob objects created by this RequestJobProvider.", "instance");
			}
			Guid guid = Guid.Empty;
			Guid guid2 = Guid.Empty;
			byte[] messageId = null;
			ADUser aduser = null;
			List<IRequestIndexEntry> list = null;
			RequestJobObjectId requestJobObjectId = requestJob.Identity;
			if (requestJobObjectId != null)
			{
				messageId = requestJobObjectId.MessageId;
				guid = requestJobObjectId.RequestGuid;
				guid2 = requestJobObjectId.MdbGuid;
			}
			else
			{
				if (requestJob.RequestType == MRSRequestType.Move)
				{
					guid = requestJob.ExchangeGuid;
				}
				else
				{
					guid = requestJob.RequestGuid;
				}
				guid2 = requestJob.WorkItemQueueMdb.ObjectGuid;
			}
			if (requestJob.RequestQueue == null)
			{
				requestJob.RequestQueue = requestJob.WorkItemQueueMdb;
			}
			if (!(guid == Guid.Empty) && !(guid2 == Guid.Empty))
			{
				if (requestJobObjectId == null)
				{
					requestJobObjectId = new RequestJobObjectId(guid, guid2, messageId);
					requestJob.Identity = requestJobObjectId;
				}
				requestJob.ValidateRequestJob();
				if (requestJob.IsValid)
				{
					aduser = requestJob.User;
					list = requestJob.IndexEntries;
				}
				bool flag = false;
				MoveObjectInfo<RequestJobXML> moveObjectInfo = null;
				try
				{
					MoveObjectInfo<RequestJobXML> moveObjectInfo2;
					if (requestJob.MoveObject != null)
					{
						moveObjectInfo2 = requestJob.MoveObject;
					}
					else
					{
						this.EnsureStoreConnectionExists(guid2);
						moveObjectInfo = new MoveObjectInfo<RequestJobXML>(guid2, this.store, messageId, RequestJobXML.RequestJobsFolderName, RequestJobXML.RequestJobsMessageClass, RequestJobXML.CreateMessageSubject(guid), RequestJobXML.CreateMessageSearchKey(guid));
						moveObjectInfo2 = moveObjectInfo;
					}
					flag = (moveObjectInfo2.MessageFound || moveObjectInfo2.OpenMessage());
					if (save)
					{
						RequestJobXML requestJobXML = new RequestJobXML(requestJob);
						requestJobXML.TimeTracker.SetTimestamp(RequestJobTimestamp.LastUpdate, new DateTime?(DateTime.UtcNow));
						if (!flag)
						{
							moveObjectInfo2.DeleteOldMessages();
						}
						moveObjectInfo2.SaveObject(requestJobXML, new MoveObjectInfo<RequestJobXML>.GetAdditionalProperties(requestJobXML.GetPropertiesWrittenOnRequestJob));
						requestJobXML.Retire();
					}
					else if (flag)
					{
						moveObjectInfo2.DeleteMessage();
						requestJob.TimeTracker.CurrentState = RequestState.Removed;
					}
					else
					{
						MrsTracer.Common.Warning("Request job message could not be removed for Request '{0}'.  AD data will still be removed", new object[]
						{
							requestJobObjectId
						});
					}
				}
				finally
				{
					if (moveObjectInfo != null)
					{
						moveObjectInfo.Dispose();
						moveObjectInfo = null;
					}
				}
				if ((aduser != null && requestJob.RequestType == MRSRequestType.Move) || (list != null && requestJob.RequestType != MRSRequestType.Move))
				{
					if (save && !requestJob.CancelRequest)
					{
						using (this.IndexProvider.RescopeTo(requestJob.DomainControllerToUpdate, requestJob.OrganizationId))
						{
							MrsTracer.Common.Debug("Updating Index/ADUser data for RequestJob '{0}'.", new object[]
							{
								requestJobObjectId
							});
							bool flag2 = false;
							for (;;)
							{
								try
								{
									if (requestJob.RequestType == MRSRequestType.Move)
									{
										this.IndexProvider.UpdateADData(aduser, requestJob, save);
										this.IndexProvider.RecipientSession.Save(aduser);
									}
									else
									{
										foreach (IRequestIndexEntry requestIndexEntry in list)
										{
											RequestJobProvider.UpdateIndexEntryData(requestIndexEntry, requestJob);
											this.IndexProvider.Save(requestIndexEntry);
										}
									}
								}
								catch (ADTransientException ex)
								{
									if (!flag2 && this.IndexProvider.OwnsSessions)
									{
										this.IndexProvider.DomainController = null;
										flag2 = true;
										continue;
									}
									CommonUtils.LogEvent(MRSEventLogConstants.Tuple_ADWriteFailed, new object[]
									{
										requestJob.Identity,
										requestJob.RequestType,
										requestJob.RequestGuid.ToString(),
										(requestJob.RequestQueue != null) ? requestJob.RequestQueue.ToString() : "null",
										requestJob.Status,
										requestJob.StatusDetail,
										requestJob.Flags,
										CommonUtils.FullExceptionMessage(ex)
									});
									throw;
								}
								break;
							}
							return;
						}
					}
					MrsTracer.Common.Warning("Not updating Index/ADUser data for RequestJob '{0}'.", new object[]
					{
						requestJobObjectId
					});
					return;
				}
				MrsTracer.Common.Warning("Not updating Index/ADUser data for orphaned RequestJob '{0}'.", new object[]
				{
					requestJobObjectId
				});
				if (!flag && !save)
				{
					throw new UnableToDeleteMoveRequestMessagePermanentException();
				}
				return;
			}
			if (save)
			{
				throw new MoveRequestMissingInfoSavePermanentException();
			}
			throw new MoveRequestMissingInfoDeletePermanentException();
		}

		private void EnsureStoreConnectionExists(Guid mdbGuid)
		{
			if (!this.MdbGuid.Equals(mdbGuid) && this.store != null)
			{
				MrsTracer.Common.Error("This RequestJobProvider is opened against Mailbox Database '{0}'; it cannot be used for items in Mailbox Database '{1}'.", new object[]
				{
					this.MdbGuid,
					mdbGuid
				});
				throw new ArgumentException(MrsStrings.ProviderAlreadySpecificToDatabase(this.MdbGuid, mdbGuid));
			}
			if (this.store == null && mdbGuid != Guid.Empty)
			{
				this.MdbGuid = mdbGuid;
				this.store = MapiUtils.GetSystemMailbox(this.MdbGuid);
				this.ownsStore = true;
			}
		}

		private RequestJobXML CreateDummyFromSearchKey(byte[] key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			RequestJobXML requestJobXML = RequestJobBase.CreateDummyObject<RequestJobXML>();
			requestJobXML.Identity = new RequestJobObjectId(new Guid(key), this.MdbGuid, null);
			requestJobXML.RequestGuid = new Guid(key);
			requestJobXML.ExchangeGuid = new Guid(key);
			return requestJobXML;
		}

		private List<T> InternalFind<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			if (!RequestJobProvider.IsRequestStatistics(typeof(T), false))
			{
				throw new ArgumentException("RequestJobProvider can only find *RequestStatistics objects.");
			}
			List<T> list = new List<T>();
			RequestJobQueryFilter requestJobQueryFilter = filter as RequestJobQueryFilter;
			if (requestJobQueryFilter != null)
			{
				MRSRequestType? requestType = requestJobQueryFilter.RequestType;
				Guid mdbGuid = requestJobQueryFilter.MdbGuid;
				Guid requestGuid = requestJobQueryFilter.RequestGuid;
				this.EnsureStoreConnectionExists(mdbGuid);
				Restriction restriction = null;
				if (requestType != null)
				{
					RequestJobNamedPropertySet requestJobNamedPropertySet = RequestJobNamedPropertySet.Get(this.SystemMailbox);
					restriction = Restriction.EQ(requestJobNamedPropertySet.PropTags[14], requestType.Value);
					if (requestType.Value == MRSRequestType.Move)
					{
						restriction = Restriction.Or(new Restriction[]
						{
							Restriction.Not(Restriction.Exist(requestJobNamedPropertySet.PropTags[14])),
							restriction
						});
					}
				}
				byte[] searchKey = null;
				if (requestGuid != Guid.Empty)
				{
					searchKey = RequestJobXML.CreateMessageSearchKey(requestGuid);
				}
				List<RequestJobXML> list2 = MoveObjectInfo<RequestJobXML>.LoadAll(searchKey, restriction, mdbGuid, this.store, RequestJobXML.RequestJobsFolderName, new MoveObjectInfo<RequestJobXML>.IsSupportedObjectTypeDelegate(RequestJobXML.IsMessageTypeSupported), new MoveObjectInfo<RequestJobXML>.EmptyTDelegate(this.CreateDummyFromSearchKey));
				if (list2 == null || list2.Count == 0)
				{
					MrsTracer.Common.Debug("No RequestJob messages found.", new object[0]);
				}
				else
				{
					foreach (RequestJobXML requestJobXML in list2)
					{
						RequestJobProvider.FixTenantInfo(requestJobXML);
						RequestStatisticsBase requestStatisticsBase = RequestJobProvider.CreateRequestStatistics(typeof(T), requestJobXML, true);
						requestStatisticsBase.OriginatingMDBGuid = mdbGuid;
						if (requestStatisticsBase.Identity == null)
						{
							requestStatisticsBase.Identity = new RequestJobObjectId(requestStatisticsBase.IdentifyingGuid, mdbGuid, null);
						}
						if (!requestStatisticsBase.IsFake)
						{
							using (this.IndexProvider.RescopeTo(requestStatisticsBase.DomainControllerToUpdate, requestStatisticsBase.OrganizationId))
							{
								if (requestStatisticsBase.UserId != null)
								{
									requestStatisticsBase.User = this.IndexProvider.ReadADUser(requestStatisticsBase.UserId, requestStatisticsBase.ExchangeGuid);
								}
								if (requestStatisticsBase.SourceUserId != null)
								{
									requestStatisticsBase.SourceUser = this.IndexProvider.ReadADUser(requestStatisticsBase.SourceUserId, requestStatisticsBase.SourceExchangeGuid);
								}
								if (requestStatisticsBase.TargetUserId != null)
								{
									requestStatisticsBase.TargetUser = this.IndexProvider.ReadADUser(requestStatisticsBase.TargetUserId, requestStatisticsBase.TargetExchangeGuid);
								}
								if (!typeof(T).Equals(typeof(MoveRequestStatistics)) && requestStatisticsBase.RequestType != MRSRequestType.Move)
								{
									List<IRequestIndexEntry> list3 = new List<IRequestIndexEntry>();
									if (requestStatisticsBase.IndexIds != null && requestStatisticsBase.IndexIds.Count > 0)
									{
										foreach (RequestIndexId indexId in requestStatisticsBase.IndexIds)
										{
											IRequestIndexEntry requestIndexEntry;
											try
											{
												requestIndexEntry = this.IndexProvider.Read(new RequestIndexEntryObjectId(requestStatisticsBase.RequestGuid, requestStatisticsBase.TargetExchangeGuid, requestStatisticsBase.RequestType, requestStatisticsBase.OrganizationId, indexId, null));
											}
											catch (TenantOrgContainerNotFoundException)
											{
												requestIndexEntry = null;
											}
											if (requestIndexEntry != null)
											{
												list3.Add(requestIndexEntry);
											}
										}
									}
									requestStatisticsBase.IndexEntries = list3;
								}
								if (this.IndexProvider.DomainController == null && !string.IsNullOrEmpty(requestStatisticsBase.DomainControllerToUpdate))
								{
									requestStatisticsBase.DomainControllerToUpdate = null;
								}
							}
						}
						requestStatisticsBase.ValidateRequestJob();
						if (this.LoadReport)
						{
							ReportData reportData = new ReportData(requestStatisticsBase.IdentifyingGuid, requestStatisticsBase.ReportVersion);
							reportData.Load(this.SystemMailbox);
							requestStatisticsBase.Report = reportData.ToReport();
						}
						if (requestStatisticsBase != null && requestStatisticsBase.GetType().Equals(typeof(T)))
						{
							list.Add((T)((object)requestStatisticsBase));
						}
					}
				}
			}
			return list;
		}

		private MapiStore store;

		private bool ownsStore;
	}
}
