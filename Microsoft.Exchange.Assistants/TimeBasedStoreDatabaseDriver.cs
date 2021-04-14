using System;
using System.Collections.Generic;
using Microsoft.Exchange.Assistants.Logging;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Assistants;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Assistants
{
	internal class TimeBasedStoreDatabaseDriver : TimeBasedDatabaseDriver
	{
		internal TimeBasedStoreDatabaseDriver(ThrottleGovernor parentGovernor, DatabaseInfo databaseInfo, ITimeBasedAssistantType timeBasedAssistantType, PoisonMailboxControl poisonControl, PerformanceCountersPerDatabaseInstance databaseCounters) : base(parentGovernor, databaseInfo, timeBasedAssistantType, poisonControl, databaseCounters)
		{
		}

		public override void RunNow(Guid mailboxGuid, string parameters)
		{
			StoreMailboxData mailboxData;
			using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=TBA", null, null, null, null))
			{
				try
				{
					PropValue[][] mailboxTableInfo = exRpcAdmin.GetMailboxTableInfo(base.DatabaseInfo.Guid, mailboxGuid, new PropTag[]
					{
						PropTag.MailboxType,
						PropTag.PersistableTenantPartitionHint,
						PropTag.LastLogonTime
					});
					if (mailboxTableInfo.Length != 1 || mailboxTableInfo[0].Length < 1)
					{
						throw new InvalidOperationException("Failed to get the mailbox property");
					}
					PropValue mailboxProperty = MailboxTableQuery.GetMailboxProperty(mailboxTableInfo[0], PropTag.PersistableTenantPartitionHint);
					TenantPartitionHint tenantPartitionHint = null;
					if (mailboxProperty.PropTag == PropTag.PersistableTenantPartitionHint)
					{
						byte[] bytes = mailboxProperty.GetBytes();
						if (bytes != null && bytes.Length != 0)
						{
							tenantPartitionHint = TenantPartitionHint.FromPersistablePartitionHint(bytes);
						}
					}
					if (string.IsNullOrEmpty(parameters))
					{
						MailboxInformation mailboxInformation = MailboxInformation.Create(mailboxGuid.ToByteArray(), base.DatabaseInfo.Guid, mailboxGuid.ToString(), ControlData.Empty, mailboxTableInfo[0], MailboxInformation.GetLastLogonTime(mailboxTableInfo[0]), tenantPartitionHint);
						mailboxData = mailboxInformation.MailboxData;
					}
					else
					{
						mailboxData = new MailboxDataForDemandJob(mailboxGuid, base.DatabaseInfo.Guid, null, parameters, tenantPartitionHint);
					}
				}
				catch (MapiExceptionNotFound arg)
				{
					ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedStoreDatabaseDriver, MapiExceptionNotFound>((long)this.GetHashCode(), "{0}: Mailbox does not exist on the store: {1}", this, arg);
					if (string.IsNullOrEmpty(parameters))
					{
						mailboxData = new StoreMailboxData(mailboxGuid, base.DatabaseInfo.Guid, mailboxGuid.ToString(), null);
					}
					else
					{
						mailboxData = new MailboxDataForDemandJob(mailboxGuid, base.DatabaseInfo.Guid, null, parameters);
					}
				}
			}
			ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedStoreDatabaseDriver, Guid>((long)this.GetHashCode(), "{0}: RunNow: about to start processing mailbox {1} on this database.", this, mailboxGuid);
			base.RunNow(mailboxData);
		}

		protected override List<MailboxData> GetMailboxesForCurrentWindow(out int totalMailboxOnDatabaseCount, out int notInterestingMailboxCount, out int filteredMailboxCount, out int failedFilteringCount)
		{
			Guid activityId = Guid.NewGuid();
			PropTag[] mailboxProperties = this.GetMailboxProperties();
			PropValue[][] mailboxes = MailboxTableQuery.GetMailboxes("Client=TBA", base.DatabaseInfo, mailboxProperties);
			totalMailboxOnDatabaseCount = ((mailboxes == null) ? 0 : mailboxes.Length);
			notInterestingMailboxCount = 0;
			filteredMailboxCount = 0;
			failedFilteringCount = 0;
			AssistantsLog.LogGetMailboxesQueryEvent(activityId, base.Assistant.NonLocalizedName, totalMailboxOnDatabaseCount, base.Assistant as AssistantBase);
			if (mailboxes == null)
			{
				return new List<MailboxData>();
			}
			List<MailboxInformation> list3 = new List<MailboxInformation>(mailboxes.Length);
			Guid mailboxGuid = Guid.Empty;
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(DatabaseSettingsContext.Get(base.DatabaseInfo.Guid), null, null);
			TimeSpan notInterestingLogInterval = snapshot.MailboxAssistants.GetObject<IMailboxAssistantSettings>(base.AssistantType.Identifier, new object[0]).MailboxNotInterestingLogInterval;
			int filteredMailboxCountLocal = 0;
			int notInterestingMailboxCountLocal = 0;
			string traceMessage;
			foreach (PropValue[] propertiesForDelegateClosure2 in mailboxes)
			{
				try
				{
					PropValue[] propertiesForDelegateClosure = propertiesForDelegateClosure2;
					List<MailboxInformation> list = list3;
					base.CatchMeIfYouCan(delegate
					{
						PropValue mailboxProperty = MailboxTableQuery.GetMailboxProperty(propertiesForDelegateClosure, PropTag.UserGuid);
						PropValue mailboxProperty2 = MailboxTableQuery.GetMailboxProperty(propertiesForDelegateClosure, PropTag.DisplayName);
						string mailboxDisplayNameTracingOnlyUsage = (mailboxProperty2.IsNull() || mailboxProperty2.Value == null) ? string.Empty : mailboxProperty2.Value.ToString();
						if (mailboxProperty.PropTag != PropTag.UserGuid || mailboxProperty2.PropTag != PropTag.DisplayName)
						{
							this.LogStoreDriverMailboxFilterEvent(notInterestingLogInterval, activityId, mailboxGuid, mailboxDisplayNameTracingOnlyUsage, Strings.MailboxNoGuidFilter, MailboxSlaFilterReasonType.NoGuid, new object[]
							{
								mailboxProperty.Value
							});
							filteredMailboxCountLocal++;
							return;
						}
						byte[] bytes = mailboxProperty.GetBytes();
						if (bytes != null && bytes.Length == 16)
						{
							mailboxGuid = new Guid(bytes);
						}
						if (MailboxTableQuery.GetMailboxProperty(propertiesForDelegateClosure, PropTag.DateDiscoveredAbsentInDS).PropTag == PropTag.DateDiscoveredAbsentInDS)
						{
							this.LogStoreDriverMailboxFilterEvent(notInterestingLogInterval, activityId, mailboxGuid, mailboxDisplayNameTracingOnlyUsage, Strings.MailboxNotInDirectoryFilter, MailboxSlaFilterReasonType.NotInDirectory, new object[0]);
							filteredMailboxCountLocal++;
							return;
						}
						PropValue mailboxProperty3 = MailboxTableQuery.GetMailboxProperty(propertiesForDelegateClosure, PropTag.MailboxMiscFlags);
						MailboxMiscFlags @int = (MailboxMiscFlags)mailboxProperty3.GetInt(0);
						if ((@int & MailboxMiscFlags.CreatedByMove) == MailboxMiscFlags.CreatedByMove)
						{
							this.LogStoreDriverMailboxFilterEvent(notInterestingLogInterval, activityId, mailboxGuid, mailboxDisplayNameTracingOnlyUsage, Strings.MailboxMoveDestinationFilter, MailboxSlaFilterReasonType.MoveDestination, new object[0]);
							filteredMailboxCountLocal++;
							return;
						}
						MailboxMiscFlags mailboxMiscFlags = @int & (MailboxMiscFlags.DisabledMailbox | MailboxMiscFlags.SoftDeletedMailbox | MailboxMiscFlags.MRSSoftDeletedMailbox);
						if (mailboxMiscFlags != MailboxMiscFlags.None)
						{
							this.LogStoreDriverMailboxFilterEvent(notInterestingLogInterval, activityId, mailboxGuid, mailboxDisplayNameTracingOnlyUsage, Strings.MailboxInaccessibleFilter, MailboxSlaFilterReasonType.Inaccessible, new object[]
							{
								mailboxMiscFlags
							});
							filteredMailboxCountLocal++;
							return;
						}
						IMailboxFilter mailboxFilter = this.AssistantType as IMailboxFilter;
						if (mailboxFilter != null && mailboxProperty3.PropTag == PropTag.MailboxMiscFlags)
						{
							MailboxMiscFlags int2 = (MailboxMiscFlags)mailboxProperty3.GetInt();
							bool flag = (int2 & MailboxMiscFlags.ArchiveMailbox) == MailboxMiscFlags.ArchiveMailbox;
							if (flag && !mailboxFilter.MailboxType.Contains(MailboxType.Archive))
							{
								this.LogStoreDriverMailboxFilterEvent(notInterestingLogInterval, activityId, mailboxGuid, mailboxDisplayNameTracingOnlyUsage, Strings.MailboxArchiveFilter, MailboxSlaFilterReasonType.Archive, new object[0]);
								filteredMailboxCountLocal++;
								return;
							}
						}
						ControlData controlData = ControlData.Empty;
						if (this.AssistantType.ControlDataPropertyDefinition != null)
						{
							PropValue mailboxProperty4 = MailboxTableQuery.GetMailboxProperty(propertiesForDelegateClosure, (PropTag)this.AssistantType.ControlDataPropertyDefinition.PropertyTag);
							if (mailboxProperty4.PropTag == (PropTag)this.AssistantType.ControlDataPropertyDefinition.PropertyTag)
							{
								controlData = ControlData.CreateFromByteArray(mailboxProperty4.GetBytes());
							}
						}
						TenantPartitionHint tenantPartitionHint = null;
						PropValue mailboxProperty5 = MailboxTableQuery.GetMailboxProperty(propertiesForDelegateClosure, PropTag.PersistableTenantPartitionHint);
						if (mailboxProperty5.PropTag == PropTag.PersistableTenantPartitionHint)
						{
							byte[] bytes2 = mailboxProperty5.GetBytes();
							if (bytes2 != null && bytes2.Length != 0)
							{
								tenantPartitionHint = TenantPartitionHint.FromPersistablePartitionHint(bytes2);
							}
						}
						MailboxInformation mailboxInformation2 = MailboxInformation.Create(mailboxProperty.GetBytes(), this.DatabaseInfo.Guid, mailboxProperty2.GetString(), controlData, propertiesForDelegateClosure, MailboxInformation.GetLastLogonTime(propertiesForDelegateClosure), tenantPartitionHint);
						if (!this.DatabaseInfo.IsUserMailbox(mailboxInformation2.MailboxGuid))
						{
							this.LogStoreDriverMailboxFilterEvent(notInterestingLogInterval, activityId, mailboxGuid, mailboxDisplayNameTracingOnlyUsage, Strings.MailboxNotUserFilter, MailboxSlaFilterReasonType.NotUser, new object[0]);
							filteredMailboxCountLocal++;
							return;
						}
						if (mailboxInformation2.MailboxData.IsPublicFolderMailbox && (mailboxFilter == null || !mailboxFilter.MailboxType.Contains(MailboxType.PublicFolder)))
						{
							this.LogStoreDriverMailboxFilterEvent(notInterestingLogInterval, activityId, mailboxGuid, mailboxDisplayNameTracingOnlyUsage, Strings.MailboxPublicFolderFilter, MailboxSlaFilterReasonType.PublicFolder, new object[0]);
							filteredMailboxCountLocal++;
							return;
						}
						if (this.IsMailboxInDemandJob(mailboxInformation2.MailboxData))
						{
							this.LogStoreDriverMailboxFilterEvent(notInterestingLogInterval, activityId, mailboxGuid, mailboxDisplayNameTracingOnlyUsage, Strings.MailboxInDemandJobFilter, MailboxSlaFilterReasonType.InDemandJob, new object[0]);
							filteredMailboxCountLocal++;
							return;
						}
						if (this.AssistantType.MailboxExtendedProperties != null)
						{
							if (!this.AssistantType.IsMailboxInteresting(mailboxInformation2))
							{
								traceMessage = string.Format("{0}: {1} is not interesting for the assistant {2}.", this, mailboxGuid, this.Assistant.NonLocalizedName);
								ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug((long)this.GetHashCode(), traceMessage);
								DateTime utcNow = DateTime.UtcNow;
								if (!this.notInterestingEventsLastLogDateTimes.ContainsKey(this.Assistant.NonLocalizedName) || utcNow - this.notInterestingEventsLastLogDateTimes[this.Assistant.NonLocalizedName] >= notInterestingLogInterval)
								{
									AssistantsLog.LogMailboxNotInterestingEvent(activityId, this.Assistant.NonLocalizedName, this.Assistant as AssistantBase, mailboxGuid, mailboxDisplayNameTracingOnlyUsage);
									this.notInterestingEventsLastLogDateTimes[this.Assistant.NonLocalizedName] = utcNow;
								}
								notInterestingMailboxCountLocal++;
								return;
							}
						}
						else
						{
							traceMessage = string.Format("{0}: {1} mailbox properties are null, IsMailboxInteresting was not called.", this, mailboxGuid);
							ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug((long)this.GetHashCode(), traceMessage);
						}
						traceMessage = string.Format("{0}: {1} is interesting for the assistant {2}.", this, mailboxGuid, this.Assistant.NonLocalizedName);
						ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug((long)this.GetHashCode(), traceMessage);
						AssistantsLog.LogMailboxInterestingEvent(activityId, this.Assistant.NonLocalizedName, this.Assistant as AssistantBase, null, mailboxGuid, mailboxDisplayNameTracingOnlyUsage);
						list.Add(mailboxInformation2);
					}, base.AssistantType.NonLocalizedName);
				}
				catch (AIException exception)
				{
					failedFilteringCount++;
					AssistantsLog.LogErrorEnumeratingMailboxes(base.Assistant, mailboxGuid, exception, true);
				}
				catch (Exception exception2)
				{
					AssistantsLog.LogErrorEnumeratingMailboxes(base.Assistant, mailboxGuid, exception2, false);
					throw;
				}
			}
			traceMessage = string.Format("{0}: {1} mailboxes after filtering", this, list3.Count);
			ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug((long)this.GetHashCode(), traceMessage);
			if (this.firstIteration || base.AssistantType.WorkCycleCheckpoint < base.AssistantType.WorkCycle)
			{
				int count = list3.Count;
				list3 = list3.FindAll((MailboxInformation mailbox) => DateTime.UtcNow - mailbox.LastProcessedDate > base.AssistantType.WorkCycle - base.AssistantType.WorkCycleCheckpoint);
				this.firstIteration = false;
				filteredMailboxCountLocal += count - list3.Count;
			}
			list3.Sort((MailboxInformation m1, MailboxInformation m2) => m1.LastProcessedDate.CompareTo(m2.LastProcessedDate));
			List<MailboxData> list2 = new List<MailboxData>();
			foreach (MailboxInformation mailboxInformation in list3)
			{
				list2.Add(mailboxInformation.MailboxData);
			}
			AssistantsLog.LogEndGetMailboxesEvent(activityId, base.Assistant.NonLocalizedName, list2.Count, base.Assistant as AssistantBase);
			filteredMailboxCount = filteredMailboxCountLocal;
			notInterestingMailboxCount = notInterestingMailboxCountLocal;
			return list2;
		}

		private PropTag[] GetMailboxProperties()
		{
			int capacity = MailboxTableQuery.RequiredMailboxTableProperties.Length + 1 + ((base.AssistantType.MailboxExtendedProperties == null) ? 0 : base.AssistantType.MailboxExtendedProperties.Length);
			List<PropTag> list = new List<PropTag>(capacity);
			list.AddRange(MailboxTableQuery.RequiredMailboxTableProperties);
			if (base.AssistantType.ControlDataPropertyDefinition != null)
			{
				list.Add((PropTag)base.AssistantType.ControlDataPropertyDefinition.PropertyTag);
			}
			if (base.AssistantType.MailboxExtendedProperties != null)
			{
				ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedStoreDatabaseDriver, int>((long)this.GetHashCode(), "{0}: Assistant requested {1} extended properties.", this, base.AssistantType.MailboxExtendedProperties.Length);
				for (int i = 0; i < base.AssistantType.MailboxExtendedProperties.Length; i++)
				{
					list.Add((PropTag)base.AssistantType.MailboxExtendedProperties[i].PropertyTag);
				}
			}
			return list.ToArray();
		}

		private void LogStoreDriverMailboxFilterEvent(TimeSpan filterLogInterval, Guid activityId, Guid mailboxGuid, string mailboxDisplayNameTracingOnlyUsage, string message, MailboxSlaFilterReasonType filterReason, params object[] args)
		{
			DateTime utcNow = DateTime.UtcNow;
			Tuple<string, MailboxSlaFilterReasonType> key = Tuple.Create<string, MailboxSlaFilterReasonType>(base.Assistant.NonLocalizedName, filterReason);
			if (this.filterEventsLastLogDateTimes.ContainsKey(key) && utcNow - this.filterEventsLastLogDateTimes[key] < filterLogInterval)
			{
				return;
			}
			string arg = string.Format("{0}: ", this);
			string text;
			if (!string.IsNullOrEmpty(message))
			{
				if (args != null)
				{
					text = string.Format("{0}{1}", arg, string.Format(message, args));
				}
				else
				{
					text = string.Format("{0}{1}", arg, message);
				}
			}
			else
			{
				text = string.Empty;
			}
			ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug((long)this.GetHashCode(), text);
			AssistantsLog.LogMailboxFilteredEvent(activityId, base.Assistant.NonLocalizedName, base.Assistant as AssistantBase, text, mailboxGuid, mailboxDisplayNameTracingOnlyUsage, filterReason);
			this.filterEventsLastLogDateTimes[key] = utcNow;
		}

		private bool firstIteration = true;

		private readonly Dictionary<string, DateTime> notInterestingEventsLastLogDateTimes = new Dictionary<string, DateTime>();

		private readonly Dictionary<Tuple<string, MailboxSlaFilterReasonType>, DateTime> filterEventsLastLogDateTimes = new Dictionary<Tuple<string, MailboxSlaFilterReasonType>, DateTime>();
	}
}
