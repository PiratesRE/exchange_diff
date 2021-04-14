using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.Transport;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal class QuarantineHandler : ITransportComponent
	{
		public QuarantineHandler(TimeSpan quarantineCrashCountWindow, TimeSpan quarantineSpan, int quarantineCrashCountThreshold, ICrashRepository crashRepository, IEventNotificationItem eventNotificationItem)
		{
			ArgumentValidator.ThrowIfZeroOrNegative("quarantineCrashCountThreshold", quarantineCrashCountThreshold);
			ArgumentValidator.ThrowIfNull("crashRepository", crashRepository);
			ArgumentValidator.ThrowIfNull("eventNotificationItem", eventNotificationItem);
			this.quarantineCrashCountWindow = quarantineCrashCountWindow;
			this.quarantineSpan = quarantineSpan;
			this.quarantineCrashCountThreshold = quarantineCrashCountThreshold;
			this.crashRepository = crashRepository;
			this.eventNotificationItem = eventNotificationItem;
		}

		public bool Loaded
		{
			get
			{
				return this.loaded;
			}
			protected set
			{
				this.loaded = value;
			}
		}

		public Dictionary<Guid, QuarantineInfoContext> ResourceQuarantineData
		{
			get
			{
				return this.resourceQuarantineData;
			}
			protected set
			{
				this.resourceQuarantineData = value;
			}
		}

		public void Load()
		{
			try
			{
				foreach (Guid guid in this.crashRepository.GetAllResourceIDs())
				{
					QuarantineInfoContext value;
					if (this.crashRepository.GetQuarantineInfoContext(guid, this.quarantineSpan, out value))
					{
						this.resourceQuarantineData.Add(guid, value);
					}
				}
			}
			catch (CrashRepositoryAccessException ex)
			{
				throw new TransportComponentLoadFailedException(ex.ErrorDescription, ex);
			}
			this.loaded = true;
		}

		public void Unload()
		{
			this.resourceProtector.EnterWriteLock();
			try
			{
				if (this.loaded)
				{
					this.resourceQuarantineData.Clear();
					this.loaded = false;
				}
			}
			finally
			{
				this.resourceProtector.ExitWriteLock();
			}
		}

		public string OnUnhandledException(Exception e)
		{
			if (e != null)
			{
				return "Unhandled Exception encountered: " + e.ToString();
			}
			return "Unhandled Exception encountered";
		}

		public void CheckAndQuarantine(Guid resourceGuid, SortedSet<DateTime> crashTimes)
		{
			if (!this.loaded)
			{
				return;
			}
			ArgumentValidator.ThrowIfEmpty("resourceGuid", resourceGuid);
			ArgumentValidator.ThrowIfNull("dateTimes", crashTimes);
			if (this.resourceQuarantineData.ContainsKey(resourceGuid))
			{
				return;
			}
			if (crashTimes.Count == 0)
			{
				return;
			}
			int num = 0;
			foreach (DateTime dateTime in crashTimes)
			{
				if (!StoreDriverUtils.CheckIfDateTimeExceedsThreshold(dateTime, DateTime.UtcNow, this.quarantineCrashCountWindow))
				{
					num++;
				}
			}
			if (num >= this.quarantineCrashCountThreshold)
			{
				QuarantineInfoContext quarantineInfoContext = new QuarantineInfoContext(DateTime.UtcNow);
				if (this.crashRepository.PersistQuarantineInfo(resourceGuid, quarantineInfoContext, false))
				{
					this.AddQuarantineDataToDictionary(resourceGuid, quarantineInfoContext);
					this.LogResourceQuarantineInfoOnCrimsonChannel(resourceGuid);
				}
			}
		}

		public bool IsResourceQuarantined(Guid resourceId, out QuarantineInfoContext quarantineInfoContext, out TimeSpan quarantineRemainingTime)
		{
			quarantineInfoContext = null;
			quarantineRemainingTime = this.quarantineSpan;
			this.resourceProtector.EnterReadLock();
			try
			{
				if (!this.loaded)
				{
					return false;
				}
				ArgumentValidator.ThrowIfEmpty("resourceId", resourceId);
				if (this.resourceQuarantineData.ContainsKey(resourceId))
				{
					quarantineInfoContext = this.resourceQuarantineData[resourceId];
					DateTime utcNow = DateTime.UtcNow;
					if (!StoreDriverUtils.CheckIfDateTimeExceedsThreshold(this.resourceQuarantineData[resourceId].QuarantineStartTime, utcNow, this.quarantineSpan))
					{
						quarantineRemainingTime = this.quarantineSpan - utcNow.Subtract(this.resourceQuarantineData[resourceId].QuarantineStartTime);
						return true;
					}
				}
			}
			finally
			{
				this.resourceProtector.ExitReadLock();
			}
			return false;
		}

		protected void AddQuarantineDataToDictionary(Guid resourceGuid, QuarantineInfoContext quarantineInfoContext)
		{
			if (this.resourceQuarantineData.ContainsKey(resourceGuid))
			{
				this.resourceQuarantineData[resourceGuid] = quarantineInfoContext;
				return;
			}
			this.resourceQuarantineData.Add(resourceGuid, quarantineInfoContext);
		}

		protected void LogResourceQuarantineInfoOnCrimsonChannel(Guid resourceGuid)
		{
			this.eventNotificationItem.Publish(ExchangeComponent.MailboxTransport.Name, "MailboxTransportUserQuarantine", string.Empty, "Resource has been quarantined due to several messages causing crashes. The resource Guid is: " + resourceGuid, ResultSeverityLevel.Warning, false);
		}

		private readonly TimeSpan quarantineCrashCountWindow;

		private readonly TimeSpan quarantineSpan;

		private readonly int quarantineCrashCountThreshold;

		private readonly ICrashRepository crashRepository;

		private readonly ReaderWriterLockSlim resourceProtector = new ReaderWriterLockSlim();

		private readonly IEventNotificationItem eventNotificationItem;

		private Dictionary<Guid, QuarantineInfoContext> resourceQuarantineData = new Dictionary<Guid, QuarantineInfoContext>();

		private bool loaded;
	}
}
