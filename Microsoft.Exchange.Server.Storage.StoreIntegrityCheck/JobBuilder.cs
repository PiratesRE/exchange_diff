using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public sealed class JobBuilder
	{
		private JobBuilder(Context context)
		{
			this.context = context;
		}

		public static JobBuilder Create(Context context)
		{
			return new JobBuilder(context);
		}

		public static Guid BuildAndSchedule(Context context, Guid mailboxGuid, IntegrityCheckRequestFlags flags, TaskId[] taskIds, StorePropTag[] propTags, ref Properties[] propertiesRows)
		{
			Guid result = Guid.Empty;
			bool flag = (flags & IntegrityCheckRequestFlags.DetectOnly) != IntegrityCheckRequestFlags.None;
			JobSource source = JobSource.OnDemand;
			JobPriority jobPriority = JobPriority.Normal;
			if ((flags & IntegrityCheckRequestFlags.Maintenance) != IntegrityCheckRequestFlags.None)
			{
				source = JobSource.Maintenance;
				jobPriority = JobPriority.Low;
			}
			else if ((flags & IntegrityCheckRequestFlags.Force) != IntegrityCheckRequestFlags.None)
			{
				jobPriority = JobPriority.High;
			}
			if (jobPriority == JobPriority.Low && InMemoryJobStorage.Instance(context.Database).IsFull)
			{
				return Guid.Empty;
			}
			IList<IntegrityCheckJob> list = JobBuilder.Create(context).ScopeToMailbox(mailboxGuid).CheckCorruptions(taskIds).FromSource(source).WithPriority(jobPriority).DetectOnly(flag).Build();
			if (list != null && list.Count > 0)
			{
				if (propTags != null)
				{
					propertiesRows = new Properties[list.Count];
				}
				for (int i = 0; i < list.Count; i++)
				{
					InMemoryJobStorage.Instance(context.Database).AddJob(list[i]);
					JobScheduler.Instance(context.Database).ScheduleJob(list[i]);
					if (propTags != null)
					{
						propertiesRows[i] = list[i].GetProperties(propTags);
					}
				}
				result = list[0].RequestGuid;
			}
			return result;
		}

		public JobBuilder ScopeToMailbox(Guid mailboxGuid)
		{
			this.mailboxGuid = mailboxGuid;
			return this;
		}

		public JobBuilder CheckCorruptions(IList<TaskId> taskIds)
		{
			this.taskIds = taskIds;
			return this;
		}

		public JobBuilder FromSource(JobSource source)
		{
			if (!Enum.IsDefined(typeof(JobSource), source))
			{
				throw new StoreException((LID)54364U, ErrorCodeValue.InvalidParameter, "Invalid job source");
			}
			this.jobSource = source;
			return this;
		}

		public JobBuilder WithPriority(JobPriority priority)
		{
			if (!Enum.IsDefined(typeof(JobPriority), priority))
			{
				throw new StoreException((LID)42076U, ErrorCodeValue.InvalidParameter, "Invalid job priority");
			}
			this.jobPriority = priority;
			return this;
		}

		public JobBuilder DetectOnly(bool detectOnly)
		{
			this.detectOnly = detectOnly;
			return this;
		}

		public IList<IntegrityCheckJob> Build()
		{
			List<KeyValuePair<int, Guid>> mailboxes = null;
			MailboxTable mailboxTable = DatabaseSchema.MailboxTable(this.context.Database);
			MailboxPropValueGetter mailboxPropValueGetter = new MailboxPropValueGetter(this.context);
			ErrorCode errorCode = mailboxPropValueGetter.Execute(this.mailboxGuid, new Column[]
			{
				mailboxTable.MailboxNumber,
				mailboxTable.MailboxGuid
			}, delegate(Reader reader)
			{
				int @int = reader.GetInt32(mailboxTable.MailboxNumber);
				Guid? nullableGuid = reader.GetNullableGuid(mailboxTable.MailboxGuid);
				if (nullableGuid != null)
				{
					if (mailboxes == null)
					{
						mailboxes = new List<KeyValuePair<int, Guid>>();
					}
					mailboxes.Add(new KeyValuePair<int, Guid>(@int, nullableGuid.Value));
				}
				return ErrorCode.NoError;
			}, () => true);
			if (ErrorCode.NoError != errorCode)
			{
				throw new StoreException((LID)33884U, errorCode, "Request expansion failed");
			}
			if (mailboxes == null || mailboxes.Count == 0)
			{
				throw new StoreException((LID)58460U, ErrorCodeValue.UnknownUser, "No mailbox found");
			}
			Guid requestGuid = Guid.NewGuid();
			DateTime utcNow = DateTime.UtcNow;
			List<IntegrityCheckJob> list = new List<IntegrityCheckJob>(mailboxes.Count);
			foreach (KeyValuePair<int, Guid> keyValuePair in mailboxes)
			{
				foreach (TaskId taskId in this.taskIds)
				{
					list.Add(new IntegrityCheckJob(Guid.NewGuid(), requestGuid, keyValuePair.Key, keyValuePair.Value, this.detectOnly, taskId, utcNow, this.jobSource, this.jobPriority));
				}
			}
			return list;
		}

		private readonly Context context;

		private Guid mailboxGuid;

		private bool detectOnly;

		private IList<TaskId> taskIds;

		private JobSource jobSource;

		private JobPriority jobPriority;
	}
}
