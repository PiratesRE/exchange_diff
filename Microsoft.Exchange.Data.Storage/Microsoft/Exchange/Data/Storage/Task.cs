using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class Task : Item, IToDoItem, IItem, IStoreObject, IStorePropertyBag, IPropertyBag, IReadOnlyPropertyBag, IDisposable
	{
		internal Task(ICoreItem coreItem) : base(coreItem, false)
		{
			ExTraceGlobals.TaskTracer.TraceDebug((long)this.GetHashCode(), "Task::Constructor.");
			this.masterProperties = this.GetTaskMasterProperties();
		}

		public static Task Create(StoreSession session, StoreId parentFolderId)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (parentFolderId == null)
			{
				throw new ArgumentNullException("parentFolderId");
			}
			return Task.CreateInternal(session, parentFolderId);
		}

		public new static Task Bind(StoreSession session, StoreId id)
		{
			return Task.Bind(session, id, null);
		}

		public new static Task Bind(StoreSession session, StoreId id, params PropertyDefinition[] propsToReturn)
		{
			return Task.Bind(session, id, (ICollection<PropertyDefinition>)propsToReturn);
		}

		public new static Task Bind(StoreSession session, StoreId id, ICollection<PropertyDefinition> propsToReturn)
		{
			return Task.Bind(session, id, false, propsToReturn);
		}

		public static Task Bind(StoreSession session, StoreId id, bool suppressCreateOneOff)
		{
			return Task.Bind(session, id, suppressCreateOneOff, null);
		}

		public static Task Bind(StoreSession session, StoreId id, bool suppressCreateOneOff, ICollection<PropertyDefinition> propsToReturn)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			Task task = ItemBuilder.ItemBind<Task>(session, id, TaskSchema.Instance, propsToReturn);
			if (suppressCreateOneOff)
			{
				ExTraceGlobals.TaskTracer.TraceDebug(0L, "Task::Bind. SuppressCreateOneOff.");
				task.SuppressCreateOneOff = true;
			}
			return task;
		}

		public override Schema Schema
		{
			get
			{
				this.CheckDisposed("Schema::get");
				return TaskSchema.Instance;
			}
		}

		public override void SetFlag(string flagRequest, ExDateTime? startDate, ExDateTime? dueDate)
		{
			this.CheckDisposed("SetFlag");
			throw new StoragePermanentException(ServerStrings.InvokingMethodNotSupported("Task", "SetFlag"));
		}

		public override void CompleteFlag(ExDateTime? completeTime)
		{
			this.CheckDisposed("CompleteFlag");
			throw new StoragePermanentException(ServerStrings.InvokingMethodNotSupported("Task", "CompleteFlag"));
		}

		public override void ClearFlag()
		{
			this.CheckDisposed("ClearFlag");
			throw new StoragePermanentException(ServerStrings.InvokingMethodNotSupported("Task", "ClearFlag"));
		}

		public override string Subject
		{
			get
			{
				return base.Subject;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("Subject::set");
				}
				base.Subject = value;
			}
		}

		public ExDateTime? StartDate
		{
			get
			{
				this.CheckDisposed("StartDate::get");
				return base.GetValueAsNullable<ExDateTime>(InternalSchema.StartDate);
			}
			set
			{
				this.CheckDisposed("StartDate::set");
				if (this.StartDate == value)
				{
					return;
				}
				this.SetTaskDateInternal(InternalSchema.StartDate, value);
			}
		}

		public ExDateTime? DueDate
		{
			get
			{
				this.CheckDisposed("DueDate::get");
				return base.GetValueAsNullable<ExDateTime>(InternalSchema.DueDate);
			}
			set
			{
				this.CheckDisposed("DueDate::set");
				if (this.DueDate == value)
				{
					return;
				}
				this.SetTaskDateInternal(InternalSchema.DueDate, value);
			}
		}

		public ExDateTime? DoItTime
		{
			get
			{
				this.CheckDisposed("DoItTime::get");
				return new ExDateTime?(base.GetValueOrDefault<ExDateTime>(InternalSchema.DoItTime));
			}
		}

		public string InternetMessageId
		{
			get
			{
				this.CheckDisposed("InternetMessageId::get");
				return base.GetValueOrDefault<string>(InternalSchema.InternetMessageId, string.Empty);
			}
			set
			{
				this.CheckDisposed("InternetMessageId::set");
				base.CheckSetNull("Task::InternetMessageId", "InternetMessageId", value);
				this[InternalSchema.InternetMessageId] = value;
			}
		}

		public Reminders<ModernReminder> ModernReminders
		{
			get
			{
				this.CheckDisposed("ModernReminders::get");
				if (this.modernReminders == null)
				{
					this.modernReminders = Reminders<ModernReminder>.Get(this, InternalSchema.ModernReminders);
				}
				return this.modernReminders;
			}
			set
			{
				this.CheckDisposed("ModernReminders::set");
				base.Load(new PropertyDefinition[]
				{
					InternalSchema.GlobalObjectId
				});
				if (base.GetValueOrDefault<byte[]>(InternalSchema.GlobalObjectId, null) == null)
				{
					GlobalObjectId globalObjectId = new GlobalObjectId();
					this[InternalSchema.GlobalObjectId] = globalObjectId.Bytes;
				}
				Reminders<ModernReminder>.Set(this, InternalSchema.ModernReminders, value);
				this.modernReminders = value;
			}
		}

		public RemindersState<ModernReminderState> ModernRemindersState
		{
			get
			{
				this.CheckDisposed("ModernRemindersState::get");
				if (this.modernRemindersState == null)
				{
					this.modernRemindersState = RemindersState<ModernReminderState>.Get(this, InternalSchema.ModernRemindersState);
				}
				return this.modernRemindersState;
			}
			set
			{
				this.CheckDisposed("ModernRemindersState::set");
				RemindersState<ModernReminderState>.Set(this, InternalSchema.ModernRemindersState, value);
				this.modernRemindersState = value;
			}
		}

		public Reminders<EventTimeBasedInboxReminder> EventTimeBasedInboxReminders
		{
			get
			{
				this.CheckDisposed("EventTimeBasedInboxReminders::get");
				if (this.eventTimeBasedInboxReminders == null)
				{
					this.eventTimeBasedInboxReminders = Reminders<EventTimeBasedInboxReminder>.Get(this, TaskSchema.EventTimeBasedInboxReminders);
				}
				return this.eventTimeBasedInboxReminders;
			}
			set
			{
				this.CheckDisposed("EventTimeBasedInboxReminders::set");
				Reminders<EventTimeBasedInboxReminder>.Set(this, TaskSchema.EventTimeBasedInboxReminders, value);
				this.eventTimeBasedInboxReminders = value;
			}
		}

		public RemindersState<EventTimeBasedInboxReminderState> EventTimeBasedInboxRemindersState
		{
			get
			{
				this.CheckDisposed("EventTimeBasedInboxRemindersState::get");
				if (this.eventTimeBasedInboxRemindersState == null)
				{
					this.eventTimeBasedInboxRemindersState = RemindersState<EventTimeBasedInboxReminderState>.Get(this, TaskSchema.EventTimeBasedInboxRemindersState);
				}
				return this.eventTimeBasedInboxRemindersState;
			}
			set
			{
				this.CheckDisposed("EventTimeBasedInboxRemindersState::set");
				RemindersState<EventTimeBasedInboxReminderState>.Set(this, TaskSchema.EventTimeBasedInboxRemindersState, value);
				this.eventTimeBasedInboxRemindersState = value;
			}
		}

		public GlobalObjectId GetGlobalObjectId()
		{
			base.Load(new PropertyDefinition[]
			{
				InternalSchema.GlobalObjectId
			});
			byte[] valueOrDefault = base.GetValueOrDefault<byte[]>(InternalSchema.GlobalObjectId, null);
			if (valueOrDefault == null)
			{
				return null;
			}
			return new GlobalObjectId(valueOrDefault);
		}

		public TaskStatus Status
		{
			get
			{
				this.CheckDisposed("Status::get");
				return base.GetValueOrDefault<TaskStatus>(InternalSchema.TaskStatus);
			}
		}

		public LocalizedString StatusDescription
		{
			get
			{
				this.CheckDisposed("StatusDescription::get");
				return this.GenerateStatusDescription();
			}
		}

		public double PercentComplete
		{
			get
			{
				this.CheckDisposed("PercentComplete::get");
				return base.GetValueOrDefault<double>(InternalSchema.PercentComplete);
			}
			set
			{
				this.CheckDisposed("PercentComplete::set");
				if (value.Equals(base.TryGetProperty(InternalSchema.PercentComplete)))
				{
					return;
				}
				if (value > 1.0 || value < 0.0)
				{
					ExTraceGlobals.TaskTracer.TraceError<double>((long)this.GetHashCode(), "Task::PercentComplete. PercentComplete is out of range. PercentComplete = {0}.", value);
					throw new ArgumentOutOfRangeException("PercentComplete");
				}
				if (value == 1.0)
				{
					throw new ArgumentException(ServerStrings.UseMethodInstead("SetStatusCompleted"));
				}
				if (value == 0.0)
				{
					ExTraceGlobals.TaskTracer.TraceDebug((long)this.GetHashCode(), "Task::PercentComplete. PercentComplete = 0.0. Change status to NotStarted.");
					this[InternalSchema.TaskStatus] = TaskStatus.NotStarted;
					base.Delete(InternalSchema.CompleteDate);
				}
				else if (this.Status == TaskStatus.Completed || this.Status == TaskStatus.NotStarted)
				{
					ExTraceGlobals.TaskTracer.TraceDebug<TaskStatus>((long)this.GetHashCode(), "Task::PercentComplete. PercentComplete != 0. Change status to InProgress. Current Status = {0}.", this.Status);
					this[InternalSchema.TaskStatus] = TaskStatus.InProgress;
					base.Delete(InternalSchema.CompleteDate);
				}
				this[InternalSchema.PercentComplete] = value;
			}
		}

		public Recurrence Recurrence
		{
			get
			{
				this.CheckDisposed("Recurrence::get");
				object value = base.TryGetProperty(InternalSchema.TaskRecurrence);
				return Task.ParseRecurrence(this, value);
			}
			set
			{
				this.CheckDisposed("Recurrence::set");
				if (!Task.IsTaskRecurrenceSupported(value))
				{
					ExTraceGlobals.TaskTracer.TraceError<Recurrence>((long)this.GetHashCode(), "Task::Recurrent::set. The recurrence is not supported. Recurrence = {0}.", value);
					throw new NotSupportedException(ServerStrings.TaskRecurrenceNotSupported(value.Pattern.ToString(), value.Range.ToString()));
				}
				if (value == null)
				{
					ExTraceGlobals.TaskTracer.TraceDebug((long)this.GetHashCode(), "Task::Recurrent::set. The recurrence is cleared.");
					base.Delete(InternalSchema.TaskRecurrence);
					this[InternalSchema.IsTaskRecurring] = false;
					this[InternalSchema.IconIndex] = IconIndex.BaseTask;
					return;
				}
				ExTraceGlobals.TaskTracer.TraceDebug<Recurrence>((long)this.GetHashCode(), "Task::Recurrent::set. Set a recurrence. Recurrence = {0}.", value);
				this[InternalSchema.TaskRecurrence] = this.ToRecurrenceBlob(value);
				this[InternalSchema.IsTaskRecurring] = true;
				this.adjustDatesBeforeSave = true;
				this[InternalSchema.IsOneOff] = false;
				this[InternalSchema.IconIndex] = IconIndex.TaskRecur;
			}
		}

		public bool IsRecurring
		{
			get
			{
				this.CheckDisposed("IsRecurring::get");
				return base.GetValueOrDefault<bool>(InternalSchema.IsTaskRecurring);
			}
		}

		public ExDateTime? CompleteDate
		{
			get
			{
				this.CheckDisposed("CompleteDate::get");
				if (this.Status != TaskStatus.Completed)
				{
					return null;
				}
				return base.GetValueAsNullable<ExDateTime>(InternalSchema.CompleteDate);
			}
		}

		public bool IsLastOccurrence
		{
			get
			{
				this.CheckDisposed("IsLastInstance::get");
				if (!this.IsRecurring)
				{
					return true;
				}
				ExDateTime nextTaskInstance = this.GetNextTaskInstance();
				return Task.NoMoreInstance(nextTaskInstance);
			}
		}

		public bool IsComplete
		{
			get
			{
				this.CheckDisposed("IsComplete::get");
				return base.GetValueOrDefault<bool>(InternalSchema.IsComplete);
			}
		}

		public bool SuppressRecurrenceAdjustment
		{
			get
			{
				this.CheckDisposed("SuppressRecurrenceAdjustment::get");
				return this.suppressRecurrenceAdjustment;
			}
			set
			{
				this.CheckDisposed("SuppressRecurrenceAdjustment::set");
				if (this.suppressRecurrenceAdjustment != value)
				{
					this.suppressRecurrenceAdjustment = value;
				}
			}
		}

		public bool SuppressCreateOneOff
		{
			get
			{
				this.CheckDisposed("SuppressCreateOneOff::get");
				return this.isSuppressCreateOneOff;
			}
			set
			{
				this.CheckDisposed("SuppressCreateOneOff::set");
				this.isSuppressCreateOneOff = value;
			}
		}

		private static bool IsTaskRecurrenceSupported(Recurrence recurrence)
		{
			if (recurrence == null)
			{
				return true;
			}
			if (recurrence.Pattern is RegeneratingPattern && recurrence.Range is EndDateRecurrenceRange)
			{
				ExTraceGlobals.TaskTracer.TraceDebug(0L, "Task::IsTaskRecurrenceSupported. We do not support combination of RegeneratingPattern and EndDateRecurrenceRange.");
				return false;
			}
			return true;
		}

		private static void MakeTaskDateIntegrity(Task task, PropertyDefinition property, object value)
		{
			if (!property.Equals(InternalSchema.DueDate) || value != null)
			{
				return;
			}
			ExTraceGlobals.TaskTracer.TraceDebug(0L, "Task::MakeTaskDateIntegrity. We clear StartDate due to DueDate is set to Null.");
			task.Delete(InternalSchema.StartDate);
		}

		private static TaskRecurrence ParseRecurrence(Task task, object value)
		{
			byte[] array = value as byte[];
			if (array == null)
			{
				return null;
			}
			return InternalRecurrence.InternalParseTask(array, task, task.PropertyBag.ExTimeZone, task.Session.ExTimeZone);
		}

		private static bool IsTimeSpanWholeDays(TimeSpan period)
		{
			return period.TotalSeconds % 86400.0 == 0.0;
		}

		private static ExDateTime GetNextTaskInstance(ExDateTime? startDate, ExDateTime? dueDate, ExDateTime? completeDate, TaskRecurrence recurrence, ExTimeZone timezone)
		{
			ExTraceGlobals.TaskTracer.TraceDebug(0L, "Task::GetNextTaskInstance. StartDate = {0}, DueDate = {1}, recurrence = {2}, timezone = {3}.", new object[]
			{
				startDate,
				dueDate,
				recurrence,
				timezone
			});
			if (recurrence == null)
			{
				return ExDateTime.MaxValue;
			}
			ExDateTime exDateTime;
			if (recurrence.Pattern is RegeneratingPattern && completeDate != null)
			{
				exDateTime = completeDate.Value.Date;
			}
			else if (startDate != null)
			{
				exDateTime = startDate.Value;
			}
			else if (dueDate != null)
			{
				exDateTime = dueDate.Value;
			}
			else
			{
				exDateTime = ExDateTime.GetNow(timezone);
				if (exDateTime < recurrence.Range.StartDate)
				{
					exDateTime = recurrence.GetNextOccurrence(exDateTime);
				}
			}
			return recurrence.GetNextOccurrence(exDateTime);
		}

		private static bool NoMoreInstance(ExDateTime instance)
		{
			return instance == ExDateTime.MaxValue;
		}

		private static TimeSpan CalculateTimeDifference(ExDateTime? start, ExDateTime? due)
		{
			if (start == null || due == null)
			{
				return default(TimeSpan);
			}
			return due.Value.Date - start.Value.Date;
		}

		private static Task CreateInternal(StoreSession session, StoreId parentFolderId)
		{
			Task task = null;
			bool flag = false;
			Task result;
			try
			{
				task = ItemBuilder.CreateNewItem<Task>(session, parentFolderId, ItemCreateInfo.TaskInfo);
				task.ClassName = "IPM.Task";
				task.SetStatusNotStarted();
				task[InternalSchema.IsComplete] = false;
				task[InternalSchema.IsTaskRecurring] = false;
				task[InternalSchema.TaskChangeCount] = 1;
				task[InternalSchema.IsDraft] = false;
				task.wasNewBeforeSave = true;
				flag = true;
				result = task;
			}
			finally
			{
				if (!flag && task != null)
				{
					task.Dispose();
					task = null;
				}
			}
			return result;
		}

		public void SetStatusNotStarted()
		{
			this.CheckDisposed("SetStatusNotStarted");
			this.SetStatusInternal(TaskStatus.NotStarted);
		}

		public void SetStatusInProgress()
		{
			this.CheckDisposed("SetStatusInProgress");
			this.SetStatusInternal(TaskStatus.InProgress);
		}

		public void SetStatusWaitingOnOthers()
		{
			this.CheckDisposed("SetStatusWaitingOnOthers");
			this.SetStatusInternal(TaskStatus.WaitingOnOthers);
		}

		public void SetStatusDeferred()
		{
			this.CheckDisposed("SetStatusDeferred");
			this.SetStatusInternal(TaskStatus.Deferred);
		}

		public void SetStatusCompleted(ExDateTime completeTime)
		{
			this.CheckDisposed("SetStatusCompleted");
			ExTimeZone exTimeZone = base.PropertyBag.ExTimeZone;
			if (exTimeZone == null || exTimeZone == ExTimeZone.UtcTimeZone)
			{
				throw new InvalidOperationException(ServerStrings.UseMethodInstead("SetCompleteTimesForUtcSession"));
			}
			this.SetStatusCompletedInternal(completeTime);
		}

		protected override void OnBeforeSave()
		{
			base.OnBeforeSave();
			if (this.AllowCreateOneOff())
			{
				ExTraceGlobals.TaskTracer.TraceDebug((long)this.GetHashCode(), "Task::OnBeforeSave. Creating one-off and updating the master.");
				this.CreateOneOff();
				this.UpdateMaster();
			}
			else
			{
				if (this.adjustDatesBeforeSave && !this.suppressRecurrenceAdjustment)
				{
					this.AdjustStartAndDueDateAccordingToRecurrence();
				}
				this.updatedMasterProperties = this.GetTaskMasterProperties();
			}
			if (this.isCreateOneOff)
			{
				this.RestoreMaster();
			}
			this.OnBeforeSaveUpdateTaskDates();
		}

		public ConflictResolutionResult Save(SaveMode saveMode, out StoreObjectId idOneOff)
		{
			ExTraceGlobals.TaskTracer.TraceDebug((long)this.GetHashCode(), "Task::Save. Save method. Version of idOneOff.");
			this.needOneOffId = true;
			ConflictResolutionResult result;
			try
			{
				ConflictResolutionResult conflictResolutionResult = base.Save(saveMode);
				if (this.oneoffId != null)
				{
					ExTraceGlobals.TaskTracer.TraceDebug<StoreObjectId>((long)this.GetHashCode(), "Task::Save. Save method. Getting idOneOff. idOneOff = {0}.", this.oneoffId);
					idOneOff = this.oneoffId.Clone();
				}
				else
				{
					ExTraceGlobals.TaskTracer.TraceDebug((long)this.GetHashCode(), "Task::Save. Save method. Getting idOneOff. idOneOff = <Null>.");
					idOneOff = null;
				}
				result = conflictResolutionResult;
			}
			finally
			{
				this.oneoffId = null;
				this.needOneOffId = false;
			}
			return result;
		}

		protected override void OnAfterSave(ConflictResolutionResult acrResults)
		{
			base.OnAfterSave(acrResults);
			if (acrResults.SaveStatus != SaveResult.IrresolvableConflict)
			{
				if (this.updatedMasterProperties != null)
				{
					this.masterProperties = this.updatedMasterProperties;
					this.updatedMasterProperties = null;
				}
				if (this.itemOneOff != null)
				{
					this.SaveOneOff();
				}
				this.isCreateOneOff = false;
				this.wasNewBeforeSave = false;
				this.adjustDatesBeforeSave = false;
				return;
			}
			this.updatedMasterProperties = null;
			ExTraceGlobals.TaskTracer.TraceError<SaveResult>((long)this.GetHashCode(), "Task::OnAfterSave. Irresolvable save result. SaveStatus = {0}.", acrResults.SaveStatus);
		}

		public void DeleteCurrentOccurrence()
		{
			this.CheckDisposed("DeleteCurrentOccurrence");
			if (!this.IsRecurring || this.Recurrence.Pattern is RegeneratingPattern)
			{
				ExTraceGlobals.TaskTracer.TraceError<bool, string>((long)this.GetHashCode(), "Task::DeleteCurrentOccurrence. Cannot delete the current occurrence. IsRecurring = {0}, RecurrencePattern = {1}.", this.IsRecurring, (this.Recurrence == null) ? "<null>" : this.Recurrence.Pattern.ToString());
				throw new InvalidOperationException();
			}
			ExDateTime nextTaskInstance = this.GetNextTaskInstance();
			if (Task.NoMoreInstance(nextTaskInstance))
			{
				ExTraceGlobals.TaskTracer.TraceError((long)this.GetHashCode(), "Task::DeleteCurrentOccurrence. Cannot delete the current occurrence. This is the last occurrence.");
				throw new InvalidOperationException(ServerStrings.UseMethodInstead("Delete"));
			}
			TimeSpan timeSpan = this.CalculateTimeDifference();
			if (this.StartDate != null)
			{
				this[InternalSchema.StartDate] = nextTaskInstance;
			}
			this[InternalSchema.DueDate] = nextTaskInstance.AddDays(timeSpan.TotalDays);
			this.SetStatusNotStarted();
		}

		public void SetStartDatesForUtcSession(ExDateTime? localStartDate, ExDateTime? utcStartDate)
		{
			this.CheckDisposed("SetStartDatesForUtcSession");
			if (base.PropertyBag.ExTimeZone != null && base.PropertyBag.ExTimeZone != ExTimeZone.UtcTimeZone)
			{
				throw new InvalidOperationException(ServerStrings.CanUseApiOnlyWhenTimeZoneIsNull("SetStartDatesForUtcSession"));
			}
			this.StartDate = utcStartDate;
			this[InternalSchema.LocalStartDate] = localStartDate;
		}

		public void SetDueDatesForUtcSession(ExDateTime? localDueDate, ExDateTime? utcDueDate)
		{
			this.CheckDisposed("SetDueDatesForUtcSession");
			if (base.PropertyBag.ExTimeZone != null && base.PropertyBag.ExTimeZone != ExTimeZone.UtcTimeZone)
			{
				throw new InvalidOperationException(ServerStrings.CanUseApiOnlyWhenTimeZoneIsNull("SetDueDatesForUtcSession"));
			}
			this.DueDate = utcDueDate;
			this[InternalSchema.LocalDueDate] = localDueDate;
		}

		public void SetCompleteTimesForUtcSession(ExDateTime completeDate, ExDateTime? flagCompleteTime)
		{
			this.CheckDisposed("SetCompleteTimesForUtcSession");
			if (base.PropertyBag.ExTimeZone != null && base.PropertyBag.ExTimeZone != ExTimeZone.UtcTimeZone)
			{
				throw new InvalidOperationException(ServerStrings.CanUseApiOnlyWhenTimeZoneIsNull("SetCompleteTimesForUtcSession"));
			}
			ExDateTime value = TaskDate.PersistentLocalTime(new ExDateTime?(completeDate)).Value;
			ExDateTime? exDateTime = TaskDate.PersistentLocalTime(flagCompleteTime);
			this.SetStatusCompletedInternal(value);
			base.SetOrDeleteProperty(InternalSchema.FlagCompleteTime, exDateTime);
			base.SetOrDeleteProperty(InternalSchema.CompleteDate, value);
		}

		public string GenerateWhen()
		{
			this.CheckDisposed("GenerateWhen");
			if (!this.IsRecurring)
			{
				return string.Empty;
			}
			return this.Recurrence.GenerateWhen(false).ToString(base.Session.InternalPreferedCulture);
		}

		private void OnBeforeSaveUpdateTaskDates()
		{
			if (this.StartDate > this.DueDate)
			{
				ExTraceGlobals.TaskTracer.TraceDebug<ExDateTime?, ExDateTime?>(0L, "Task::ObjectUpdateTaskDates. DueDate is earlier than StartDate so we are changing DueDate. StartDate = {0}, DueDate = {1}.", this.StartDate, this.DueDate);
				TimeSpan timeSpan = Task.CalculateTimeDifference(this.OriginalStartDate, this.OriginalDueDate);
				this.DueDate = new ExDateTime?(this.StartDate.Value.AddDays(timeSpan.TotalDays));
			}
		}

		private void AdjustStartAndDueDateAccordingToRecurrence()
		{
			Recurrence recurrence = this.Recurrence;
			if (recurrence != null)
			{
				ExTraceGlobals.TaskTracer.TraceDebug<ExDateTime?, ExDateTime?>((long)this.GetHashCode(), "Task::AdjustStartAndDueDateAccordingToRecurrence. StartDate is {0}, DueDate is {1}.", this.StartDate, this.DueDate);
				if (this.StartDate != null)
				{
					TimeSpan? timeSpan = null;
					if (this.DueDate != null)
					{
						timeSpan = this.DueDate - this.StartDate;
					}
					this[InternalSchema.StartDate] = recurrence.Range.StartDate;
					if (timeSpan != null)
					{
						this[InternalSchema.DueDate] = (Task.IsTimeSpanWholeDays(timeSpan.Value) ? new ExDateTime?(recurrence.Range.StartDate.AddDays(timeSpan.Value.TotalDays)) : (recurrence.Range.StartDate + timeSpan));
					}
				}
				else
				{
					this[InternalSchema.DueDate] = recurrence.Range.StartDate;
				}
				ExTraceGlobals.TaskTracer.TraceDebug<ExDateTime?, ExDateTime?>((long)this.GetHashCode(), "Task::AdjustStartAndDueDateAccordingToRecurrence. New StartDate is {0}, new DueDate is {1}.", this.StartDate, this.DueDate);
			}
		}

		private void RestoreMaster()
		{
			int num = base.GetValueOrDefault<int>(InternalSchema.TaskChangeCount);
			ExTraceGlobals.TaskTracer.TraceDebug<int>((long)this.GetHashCode(), "Task::OnBeforeSave. Increment count. Count = {0}.", num);
			num++;
			this[InternalSchema.TaskChangeCount] = num;
			this[InternalSchema.IsOneOff] = false;
			this[InternalSchema.IconIndex] = IconIndex.TaskRecur;
			this[InternalSchema.IsTaskRecurring] = true;
			base.Delete(InternalSchema.FlagCompleteTime);
		}

		private LocalizedString GenerateStatusDescription()
		{
			TaskStatus valueOrDefault = base.GetValueOrDefault<TaskStatus>(InternalSchema.TaskStatus, (TaskStatus)(-1));
			if (valueOrDefault < TaskStatus.NotStarted)
			{
				return LocalizedString.Empty;
			}
			switch (valueOrDefault)
			{
			case TaskStatus.NotStarted:
				return ClientStrings.TaskStatusNotStarted;
			case TaskStatus.InProgress:
				return ClientStrings.TaskStatusInProgress;
			case TaskStatus.Completed:
				return ClientStrings.TaskStatusCompleted;
			case TaskStatus.WaitingOnOthers:
				return ClientStrings.TaskStatusWaitOnOthers;
			case TaskStatus.Deferred:
				return ClientStrings.TaskStatusDeferred;
			default:
				return LocalizedString.Empty;
			}
		}

		private PropertyBag GetTaskMasterProperties()
		{
			MemoryPropertyBag memoryPropertyBag = new MemoryPropertyBag();
			IDirectPropertyBag directPropertyBag = memoryPropertyBag;
			foreach (NativeStorePropertyDefinition propertyDefinition in Task.TaskMasterProperties)
			{
				directPropertyBag.SetValue(propertyDefinition, base.TryGetProperty(propertyDefinition));
			}
			return memoryPropertyBag;
		}

		private void SetStatusCompletedAsSingle(ExDateTime completeTime)
		{
			this[InternalSchema.TaskStatus] = TaskStatus.Completed;
			this[InternalSchema.FlagCompleteTime] = completeTime;
			this[InternalSchema.CompleteDate] = completeTime.Date;
			this[InternalSchema.PercentComplete] = 1.0;
			this[InternalSchema.IconIndex] = IconIndex.BaseTask;
			this[InternalSchema.IsTaskRecurring] = false;
			this[InternalSchema.FlagStatus] = FlagStatus.Complete;
			this[InternalSchema.ReminderIsSetInternal] = false;
		}

		private void SetStatusCompletedInternal(ExDateTime completeTime)
		{
			if (completeTime == ExDateTime.MaxValue || completeTime < Util.Date1601)
			{
				ExTraceGlobals.TaskTracer.TraceError<ExDateTime>((long)this.GetHashCode(), "Task::SetStatusCompletedInternal. completeTime is out of range. completeTime = {0}.", completeTime);
				throw new ArgumentOutOfRangeException("completeTime");
			}
			if (!this.IsRecurring)
			{
				this.SetStatusCompletedAsSingle(completeTime);
				return;
			}
			ExDateTime nextTaskInstance = this.GetNextTaskInstance(new ExDateTime?(completeTime));
			if (Task.NoMoreInstance(nextTaskInstance))
			{
				if (!(this.Recurrence.Pattern is RegeneratingPattern))
				{
					ExTraceGlobals.TaskTracer.TraceDebug((long)this.GetHashCode(), "Task::SetStatusCompletedInternal. This is the last occurrence and the pattern is regular recurrence pattern. We are clearing the recurrence blob.");
					this.Recurrence = null;
				}
				this.SetStatusCompletedAsSingle(completeTime);
				this[InternalSchema.IsOneOff] = true;
				return;
			}
			if (this.SuppressCreateOneOff)
			{
				ExTraceGlobals.TaskTracer.TraceError((long)this.GetHashCode(), "Task::SetStatusCompletedInternal. The consumer cannot choose to suppress creating one-off yet calling this API.");
				throw new InvalidOperationException(ServerStrings.ExCannotMarkTaskCompletedWhenSuppressCreateOneOff);
			}
			this.SetCreateOneOff();
			this.SetStatusCompletedAsSingle(completeTime);
		}

		private void SetStatusInternal(TaskStatus status)
		{
			ExTraceGlobals.TaskTracer.TraceDebug<TaskStatus, TaskStatus>((long)this.GetHashCode(), "Task::SetStatusInternal. Update status. Current = {0}, New = {1}.", this.Status, status);
			if (this.Status == TaskStatus.Completed)
			{
				this[InternalSchema.FlagStatus] = FlagStatus.NotFlagged;
			}
			this[InternalSchema.IsComplete] = false;
			this[InternalSchema.TaskStatus] = status;
			base.Delete(InternalSchema.CompleteDate);
			base.Delete(InternalSchema.FlagCompleteTime);
			switch (status)
			{
			case TaskStatus.NotStarted:
				this[InternalSchema.PercentComplete] = 0.0;
				return;
			case TaskStatus.InProgress:
			case TaskStatus.WaitingOnOthers:
			case TaskStatus.Deferred:
				if (this.PercentComplete == 1.0)
				{
					this[InternalSchema.PercentComplete] = 0.0;
					return;
				}
				return;
			}
			throw new InvalidOperationException();
		}

		internal static bool IsTaskRecurrenceSupported(ValidationContext context, IValidatablePropertyBag validatablePropertyBag)
		{
			object obj = validatablePropertyBag.TryGetProperty(InternalSchema.TaskRecurrence);
			byte[] array = obj as byte[];
			if (array != null)
			{
				TaskRecurrence recurrence = InternalRecurrence.InternalParseTask(array, null, null, null);
				return Task.IsTaskRecurrenceSupported(recurrence);
			}
			return true;
		}

		private void SetTaskDateInternal(PropertyDefinition property, ExDateTime? value)
		{
			ExTraceGlobals.TaskTracer.TraceDebug<PropertyDefinition, ExDateTime?>((long)this.GetHashCode(), "Task::SetTaskDateInternal. Change TaskDate. property = {0}, value = {1}.", property, value);
			base.SetOrDeleteProperty(property, value);
			Task.MakeTaskDateIntegrity(this, property, value);
			if (!this.IsRecurring || this.Recurrence.Pattern is RegeneratingPattern || this.SuppressCreateOneOff)
			{
				ExTraceGlobals.TaskTracer.TraceDebug<bool, bool>((long)this.GetHashCode(), "Task::SetTaskDateInternal. Change TaskDate on a single Task. IsRecurring = {0}, SuppressCreateOneOff = {1}.", this.IsRecurring, this.SuppressCreateOneOff);
				return;
			}
			ExDateTime nextTaskInstance = this.GetNextTaskInstance();
			if (Task.NoMoreInstance(nextTaskInstance))
			{
				ExTraceGlobals.TaskTracer.TraceDebug((long)this.GetHashCode(), "Task::SetTaskDateInternal. This is the last occurrence.");
				base.Delete(InternalSchema.TaskRecurrence);
				this[InternalSchema.IsTaskRecurring] = false;
				this[InternalSchema.TaskResetReminder] = false;
				this[InternalSchema.IsOneOff] = true;
				return;
			}
			ExTraceGlobals.TaskTracer.TraceDebug((long)this.GetHashCode(), "Task::SetTaskDateInternal. This is NOT the last occurence. We persist the changes on the master for the time being.");
			this.SetCreateOneOff();
		}

		private byte[] ToRecurrenceBlob(Recurrence recurrence)
		{
			if (recurrence == null)
			{
				return null;
			}
			if (recurrence.Pattern == null)
			{
				throw new CorruptDataException(ServerStrings.ExPatternNotSet);
			}
			if (recurrence.Range == null)
			{
				throw new CorruptDataException(ServerStrings.ExRangeNotSet);
			}
			TimeSpan timeSpan = (this.StartDate == null) ? default(TimeSpan) : this.StartDate.Value.TimeOfDay;
			TimeSpan endOffset = timeSpan;
			InternalRecurrence internalRecurrence = new InternalRecurrence(recurrence.Pattern, recurrence.Range, this, base.PropertyBag.ExTimeZone, base.PropertyBag.ExTimeZone, timeSpan, endOffset, new ExDateTime?(recurrence.EndDate));
			return internalRecurrence.ToByteArray(true);
		}

		private void SetCreateOneOff()
		{
			if (this.OriginalRecurrence != null)
			{
				this.isCreateOneOff = true;
			}
		}

		private bool AllowCreateOneOff()
		{
			bool flag = this.isCreateOneOff && !this.SuppressCreateOneOff && !this.wasNewBeforeSave;
			bool flag2 = this.StartDate != this.OriginalStartDate || this.DueDate != this.OriginalDueDate || (this.Status != this.OriginalStatus && this.Status == TaskStatus.Completed);
			ExTraceGlobals.TaskTracer.TraceDebug((long)this.GetHashCode(), "Task::GetCreateOneOff. The condition of creating one-off. isCreateOneOff = {0}, SuppressCreateOneOff = {1}, wasNewBeforeSave = {2}, statusChanged = {3}.", new object[]
			{
				this.isCreateOneOff,
				this.SuppressCreateOneOff,
				this.wasNewBeforeSave,
				flag2
			});
			return flag && flag2;
		}

		private void CreateOneOff()
		{
			ExTraceGlobals.TaskTracer.TraceDebug((long)this.GetHashCode(), "Task::CreateOneOff. Create one-off.");
			this.itemOneOff = Task.Create(base.Session, base.ParentId);
			IDirectPropertyBag propertyBag = this.itemOneOff.PropertyBag;
			foreach (NativeStorePropertyDefinition propertyDefinition in Task.TaskMasterProperties)
			{
				object propertyValue = base.TryGetProperty(propertyDefinition);
				if (PropertyError.IsPropertyNotFound(propertyValue))
				{
					propertyBag.Delete(propertyDefinition);
				}
				else
				{
					propertyBag.SetValue(propertyDefinition, propertyValue);
				}
			}
			this.itemOneOff[InternalSchema.IsTaskRecurring] = false;
			this.itemOneOff[InternalSchema.IsOneOff] = true;
			this.itemOneOff[InternalSchema.IconIndex] = IconIndex.BaseTask;
			if (this.OriginalRecurrence != null && this.OriginalRecurrence.Pattern is RegeneratingPattern)
			{
				this.itemOneOff.Delete(InternalSchema.TaskRecurrence);
			}
		}

		private void SaveOneOff()
		{
			ExTraceGlobals.TaskTracer.TraceDebug((long)this.GetHashCode(), "Task::SaveOneOff. Saving one-off.");
			Microsoft.Exchange.Data.Storage.CoreObject.MapiCopyTo(base.MapiMessage, this.itemOneOff.MapiMessage, base.Session, this.itemOneOff.Session, CopyPropertiesFlags.None, CopySubObjects.Copy, new NativeStorePropertyDefinition[0]);
			this.itemOneOff.Save(SaveMode.NoConflictResolution);
			if (this.needOneOffId)
			{
				this.itemOneOff.Load(null);
				this.oneoffId = this.itemOneOff.Id.ObjectId;
			}
			this.itemOneOff.Dispose();
			this.itemOneOff = null;
		}

		private void UpdateMaster()
		{
			ExDateTime? completeDate = this.CompleteDate;
			this.SetStatusNotStarted();
			base.Delete(InternalSchema.IsOneOff);
			TimeSpan? timeSpan = null;
			ExDateTime nextTaskInstanceFromOriginal = this.GetNextTaskInstanceFromOriginal(completeDate);
			if (this.StartDate != null)
			{
				timeSpan = new TimeSpan?(nextTaskInstanceFromOriginal - this.StartDate.Value);
			}
			if (this.OriginalStartDate == null)
			{
				base.Delete(InternalSchema.StartDate);
			}
			else
			{
				this[InternalSchema.StartDate] = nextTaskInstanceFromOriginal;
			}
			TimeSpan timeSpan2 = Task.CalculateTimeDifference(this.OriginalStartDate, this.OriginalDueDate);
			this[InternalSchema.DueDate] = nextTaskInstanceFromOriginal.AddDays(timeSpan2.TotalDays);
			Recurrence recurrence = this.Recurrence;
			if (recurrence != null && recurrence.Range is NumberedRecurrenceRange)
			{
				int num = ((NumberedRecurrenceRange)recurrence.Range).NumberOfOccurrences - 1;
				ExTraceGlobals.TaskTracer.TraceDebug<int>((long)this.GetHashCode(), "Task::UpdateMaster. Update NumberedRecurrenceRange. updateNumberedOccurrence = {0}.", num);
				if (num <= 1)
				{
					this.Recurrence = null;
				}
				else
				{
					ExDateTime endDate = recurrence.EndDate;
					recurrence = new Recurrence(recurrence.Pattern, new NumberedRecurrenceRange(recurrence.Range.StartDate, num), new ExDateTime?(endDate));
					this.Recurrence = recurrence;
				}
			}
			this.nextInstanceOnMaster = null;
			if (!(base.TryGetProperty(InternalSchema.ActualWork) is PropertyError))
			{
				this[InternalSchema.ActualWork] = 0;
			}
			if (!(base.TryGetProperty(InternalSchema.TotalWork) is PropertyError))
			{
				this[InternalSchema.TotalWork] = 0;
			}
			bool flag = this.masterProperties.GetValueOrDefault<bool>(InternalSchema.ReminderIsSetInternal) && base.Reminder.DueBy != null;
			bool flag2 = base.GetValueOrDefault<bool>(InternalSchema.TaskResetReminder, false) && base.Reminder.DueBy != null;
			if (flag2)
			{
				this[InternalSchema.TaskResetReminder] = false;
			}
			if ((flag || flag2) && !base.Reminder.IsSet)
			{
				this[InternalSchema.ReminderIsSetInternal] = true;
				if (timeSpan != null)
				{
					base.Reminder.DueBy = new ExDateTime?(base.Reminder.DueBy.Value.AddDays((double)timeSpan.Value.Days));
					if (timeSpan.Value.Hours >= 23)
					{
						base.Reminder.DueBy = new ExDateTime?(base.Reminder.DueBy.Value.AddDays(1.0));
					}
					base.Reminder.Adjust();
				}
			}
		}

		private ExDateTime GetNextTaskInstance()
		{
			return this.GetNextTaskInstance(null);
		}

		private ExDateTime GetNextTaskInstance(ExDateTime? completeDate)
		{
			if (this.nextInstanceOnMaster != null)
			{
				return this.nextInstanceOnMaster.Value;
			}
			this.nextInstanceOnMaster = new ExDateTime?(Task.GetNextTaskInstance(this.StartDate, this.DueDate, completeDate, (TaskRecurrence)this.Recurrence, base.PropertyBag.ExTimeZone));
			return this.nextInstanceOnMaster.Value;
		}

		private ExDateTime GetNextTaskInstanceFromOriginal(ExDateTime? completeDate)
		{
			return Task.GetNextTaskInstance(this.OriginalStartDate, this.OriginalDueDate, completeDate, this.OriginalRecurrence, base.PropertyBag.ExTimeZone);
		}

		private TimeSpan CalculateTimeDifference()
		{
			return Task.CalculateTimeDifference(this.StartDate, this.DueDate);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.itemOneOff != null)
			{
				this.itemOneOff.Dispose();
				this.itemOneOff = null;
			}
			base.InternalDispose(disposing);
		}

		internal static bool IsStartDateDefined(ValidationContext context, IValidatablePropertyBag validatablePropertyBag)
		{
			return !PropertyError.IsPropertyError(validatablePropertyBag.TryGetProperty(InternalSchema.StartDate));
		}

		private ExDateTime? OriginalStartDate
		{
			get
			{
				return this.masterProperties.GetValueAsNullable<ExDateTime>(InternalSchema.StartDate);
			}
		}

		private ExDateTime? OriginalDueDate
		{
			get
			{
				return this.masterProperties.GetValueAsNullable<ExDateTime>(InternalSchema.DueDate);
			}
		}

		private TaskRecurrence OriginalRecurrence
		{
			get
			{
				byte[] valueOrDefault = this.masterProperties.GetValueOrDefault<byte[]>(InternalSchema.TaskRecurrence);
				if (valueOrDefault == null)
				{
					return null;
				}
				return InternalRecurrence.InternalParseTask(valueOrDefault, this, base.PropertyBag.ExTimeZone, base.PropertyBag.ExTimeZone);
			}
		}

		private TaskStatus OriginalStatus
		{
			get
			{
				return this.masterProperties.GetValueOrDefault<TaskStatus>(InternalSchema.TaskStatus);
			}
		}

		internal static void CoreObjectUpdateTaskStatus(CoreItem coreItem)
		{
			TaskStatus valueOrDefault = coreItem.PropertyBag.GetValueOrDefault<TaskStatus>(InternalSchema.TaskStatus);
			bool valueOrDefault2 = coreItem.PropertyBag.GetValueOrDefault<bool>(InternalSchema.IsComplete);
			bool flag = valueOrDefault == TaskStatus.Completed;
			if (flag != valueOrDefault2)
			{
				coreItem.PropertyBag[InternalSchema.IsComplete] = flag;
			}
			if (valueOrDefault == TaskStatus.NotStarted)
			{
				coreItem.PropertyBag[InternalSchema.PercentComplete] = 0.0;
				return;
			}
			if (valueOrDefault == TaskStatus.Completed)
			{
				coreItem.PropertyBag[InternalSchema.PercentComplete] = 1.0;
				return;
			}
			if (PropertyError.IsPropertyError(coreItem.PropertyBag.TryGetProperty(InternalSchema.PercentComplete)))
			{
				coreItem.PropertyBag[InternalSchema.PercentComplete] = 0.0;
				return;
			}
			double num = (double)coreItem.PropertyBag.TryGetProperty(InternalSchema.PercentComplete);
			if (num >= 1.0 || num < 0.0)
			{
				coreItem.PropertyBag[InternalSchema.PercentComplete] = 0.0;
			}
		}

		internal static void CoreObjectUpdateRecurrence(CoreItem coreItem)
		{
			bool flag = coreItem.PropertyBag.GetValueOrDefault<bool>(InternalSchema.IsTaskRecurring);
			byte[] valueOrDefault = coreItem.PropertyBag.GetValueOrDefault<byte[]>(InternalSchema.TaskRecurrence);
			if (valueOrDefault != null)
			{
				ExTimeZone exTimeZone = Microsoft.Exchange.Data.Storage.CoreObject.GetPersistablePropertyBag(coreItem).ExTimeZone;
				try
				{
					TaskRecurrence taskRecurrence = InternalRecurrence.InternalParseTask(valueOrDefault, null, exTimeZone, exTimeZone);
					if (taskRecurrence.Pattern is RegeneratingPattern)
					{
						flag = true;
					}
					goto IL_AB;
				}
				catch (RecurrenceFormatException)
				{
					if (!coreItem.IsMoveUser)
					{
						throw;
					}
					VersionedId valueOrDefault2 = coreItem.PropertyBag.GetValueOrDefault<VersionedId>(InternalSchema.ItemId);
					ExTraceGlobals.TaskTracer.TraceWarning<VersionedId>(0L, "Task::CoreObjectUpdateRecurrence. Removing corrupted recurrence blob from task {0}", valueOrDefault2);
					coreItem.PropertyBag.Delete(InternalSchema.TaskRecurrence);
					coreItem.PropertyBag[InternalSchema.IsTaskRecurring] = false;
					flag = false;
					goto IL_AB;
				}
			}
			flag = false;
			IL_AB:
			coreItem.PropertyBag[InternalSchema.IsRecurring] = flag;
		}

		internal static void CoreObjectUpdateTaskDates(CoreItem coreItem)
		{
			ExDateTime? valueAsNullable = coreItem.PropertyBag.GetValueAsNullable<ExDateTime>(InternalSchema.StartDate);
			ExDateTime? valueAsNullable2 = coreItem.PropertyBag.GetValueAsNullable<ExDateTime>(InternalSchema.DueDate);
			if (valueAsNullable != null && valueAsNullable2 == null)
			{
				ExTraceGlobals.TaskTracer.TraceDebug(0L, "Task::ObjectUpdateTaskDates. DueDate is missing so we are adding DueDate.");
				coreItem.PropertyBag[InternalSchema.DueDate] = valueAsNullable;
			}
		}

		private static readonly NativeStorePropertyDefinition[] TaskMasterProperties = new NativeStorePropertyDefinition[]
		{
			InternalSchema.LocalStartDate,
			InternalSchema.LocalDueDate,
			InternalSchema.UtcStartDate,
			InternalSchema.UtcDueDate,
			InternalSchema.ReminderIsSetInternal,
			InternalSchema.TaskRecurrence,
			InternalSchema.TaskStatus,
			InternalSchema.TaskChangeCount,
			InternalSchema.ActualWork,
			InternalSchema.TotalWork,
			InternalSchema.CompleteDate
		};

		private PropertyBag masterProperties;

		private PropertyBag updatedMasterProperties;

		private Task itemOneOff;

		private StoreObjectId oneoffId;

		private bool needOneOffId;

		private bool isSuppressCreateOneOff;

		private bool isCreateOneOff;

		private ExDateTime? nextInstanceOnMaster = null;

		private bool suppressRecurrenceAdjustment;

		private bool wasNewBeforeSave;

		private bool adjustDatesBeforeSave;

		private Reminders<ModernReminder> modernReminders;

		private RemindersState<ModernReminderState> modernRemindersState;

		private Reminders<EventTimeBasedInboxReminder> eventTimeBasedInboxReminders;

		private RemindersState<EventTimeBasedInboxReminderState> eventTimeBasedInboxRemindersState;
	}
}
