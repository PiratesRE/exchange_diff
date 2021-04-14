using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MRSIssueCache : ServiceIssueCache
	{
		public override bool ScanningIsEnabled
		{
			get
			{
				return ConfigBase<MRSConfigSchema>.GetConfig<bool>("IssueCacheIsEnabled");
			}
		}

		protected override TimeSpan FullScanFrequency
		{
			get
			{
				return ConfigBase<MRSConfigSchema>.GetConfig<TimeSpan>("IssueCacheScanFrequency");
			}
		}

		protected override int IssueLimit
		{
			get
			{
				return ConfigBase<MRSConfigSchema>.GetConfig<int>("IssueCacheItemLimit");
			}
		}

		protected override ICollection<ServiceIssue> RunFullIssueScan()
		{
			ICollection<ServiceIssue> collection = new List<ServiceIssue>();
			foreach (Guid mdbGuid in MapiUtils.GetDatabasesOnThisServer())
			{
				using (new DatabaseSettingsContext(mdbGuid, null).Activate())
				{
					try
					{
						DatabaseInformation databaseInformation = MapiUtils.FindServerForMdb(mdbGuid, null, null, FindServerFlags.None);
						string databaseName = databaseInformation.DatabaseName;
						if (!databaseInformation.IsOnThisServer)
						{
							return null;
						}
						using (MapiStore systemMailbox = MapiUtils.GetSystemMailbox(mdbGuid, false))
						{
							using (MapiFolder requestJobsFolder = RequestJobXML.GetRequestJobsFolder(systemMailbox))
							{
								using (MapiTable contentsTable = requestJobsFolder.GetContentsTable(ContentsTableFlags.DeferredErrors))
								{
									if (contentsTable.GetRowCount() > 0)
									{
										RequestJobNamedPropertySet requestJobNamedPropertySet = RequestJobNamedPropertySet.Get(systemMailbox);
										contentsTable.SetColumns(requestJobNamedPropertySet.PropTags);
										Restriction restriction = Restriction.GT(requestJobNamedPropertySet.PropTags[23], ConfigBase<MRSConfigSchema>.GetConfig<int>("PoisonLimit"));
										List<MoveJob> allMoveJobs = SystemMailboxJobs.GetAllMoveJobs(restriction, null, contentsTable, mdbGuid, null);
										if (allMoveJobs != null)
										{
											foreach (MoveJob job in allMoveJobs)
											{
												collection.Add(new MRSPoisonedJobIssue(job));
											}
										}
									}
								}
							}
						}
					}
					catch (LocalizedException lastScanError)
					{
						base.LastScanError = lastScanError;
					}
				}
			}
			return collection;
		}
	}
}
