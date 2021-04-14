using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Monitoring;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.Tasks
{
	internal class MailboxAssistantsWatermarks : AssistantTroubleshooterBase
	{
		public MailboxAssistantsWatermarks(PropertyBag fields) : base(fields)
		{
		}

		private TimeSpan WatermarkBehindWarningThrehold
		{
			get
			{
				if (this.watermarkBehindWarningThrehold == null)
				{
					if (this.fields["WatermarkBehindWarningThreholdInMinutes"] != null)
					{
						this.watermarkBehindWarningThrehold = new TimeSpan?(TimeSpan.FromMinutes((uint)this.fields["WatermarkBehindWarningThreholdInMinutes"]));
					}
					else
					{
						this.watermarkBehindWarningThrehold = new TimeSpan?(MailboxAssistantsWatermarks.DefaultWatermarkBehindWarningThrehold);
					}
				}
				return this.watermarkBehindWarningThrehold.Value;
			}
		}

		public override MonitoringData InternalRunCheck()
		{
			MonitoringData monitoringData = new MonitoringData();
			if (base.ExchangeServer.AdminDisplayVersion.Major < MailboxAssistantsWatermarks.minExpectedServerVersion.Major || (base.ExchangeServer.AdminDisplayVersion.Major == MailboxAssistantsWatermarks.minExpectedServerVersion.Major && base.ExchangeServer.AdminDisplayVersion.Minor < MailboxAssistantsWatermarks.minExpectedServerVersion.Minor))
			{
				monitoringData.Events.Add(new MonitoringEvent(AssistantTroubleshooterBase.EventSource, 5101, EventTypeEnumeration.Warning, Strings.TSMinServerVersion(MailboxAssistantsWatermarks.minExpectedServerVersion.ToString())));
				return monitoringData;
			}
			using (ExRpcAdmin exRpcAdmin = ExRpcAdmin.Create("Client=Management", base.ExchangeServer.Name, null, null, null))
			{
				List<MdbStatus> onlineMDBList = base.GetOnlineMDBList(exRpcAdmin);
				foreach (MdbStatus mdbStatus in onlineMDBList)
				{
					Guid empty = Guid.Empty;
					Watermark[] watermarksForMailbox = exRpcAdmin.GetWatermarksForMailbox(mdbStatus.MdbGuid, ref empty, Guid.Empty);
					MapiEventManager mapiEventManager = MapiEventManager.Create(exRpcAdmin, Guid.Empty, mdbStatus.MdbGuid);
					long eventCounter = mapiEventManager.ReadLastEvent().EventCounter;
					bool flag = false;
					foreach (Watermark watermark in watermarksForMailbox)
					{
						if (eventCounter - watermark.EventCounter > 50L)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						MailboxAssistantsWatermarks.WatermarkWithCreateTime[] array2 = MailboxAssistantsWatermarks.BuildWaterMarkWithCreateTimes(mapiEventManager, watermarksForMailbox);
						DateTime eventTime = MailboxAssistantsWatermarks.GetEventTime(mapiEventManager, eventCounter);
						List<MailboxAssistantsWatermarks.WatermarkWithCreateTime> list = new List<MailboxAssistantsWatermarks.WatermarkWithCreateTime>(watermarksForMailbox.Length);
						foreach (MailboxAssistantsWatermarks.WatermarkWithCreateTime watermarkWithCreateTime in array2)
						{
							if (eventTime - watermarkWithCreateTime.CreateTime > this.WatermarkBehindWarningThrehold)
							{
								list.Add(watermarkWithCreateTime);
							}
						}
						if (list.Count > 0)
						{
							monitoringData.Events.Add(new MonitoringEvent(AssistantTroubleshooterBase.EventSource, 5207, EventTypeEnumeration.Error, Strings.AIMDBWatermarksAreTooLow(base.ExchangeServer.Fqdn, mdbStatus.MdbName, ((int)this.WatermarkBehindWarningThrehold.TotalMinutes).ToString(), MailboxAssistantsWatermarks.BuildFormatedEventCounter(eventCounter, eventTime), MailboxAssistantsWatermarks.BuildFormatedWatermarks(list.ToArray()))));
						}
					}
				}
			}
			return monitoringData;
		}

		public override MonitoringData Resolve(MonitoringData monitoringData)
		{
			return monitoringData;
		}

		private static MailboxAssistantsWatermarks.WatermarkWithCreateTime[] BuildWaterMarkWithCreateTimes(MapiEventManager eventManager, Watermark[] watermarks)
		{
			MailboxAssistantsWatermarks.WatermarkWithCreateTime[] array = new MailboxAssistantsWatermarks.WatermarkWithCreateTime[watermarks.Length];
			for (int i = 0; i < watermarks.Length; i++)
			{
				DateTime eventTime = MailboxAssistantsWatermarks.GetEventTime(eventManager, watermarks[i].EventCounter);
				array[i] = new MailboxAssistantsWatermarks.WatermarkWithCreateTime(watermarks[i].ConsumerGuid, watermarks[i].EventCounter, eventTime);
			}
			return array;
		}

		private static DateTime GetEventTime(MapiEventManager eventManager, long eventCounter)
		{
			MapiEvent[] array = eventManager.ReadEvents(eventCounter, 1);
			if (array != null && array.Length != 0)
			{
				return array[0].CreateTime;
			}
			return DateTime.UtcNow;
		}

		private static string BuildFormatedWatermarks(MailboxAssistantsWatermarks.WatermarkWithCreateTime[] watermarks)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < watermarks.Length; i++)
			{
				stringBuilder.Append(watermarks[i].ConsumerGuid);
				stringBuilder.Append(":");
				stringBuilder.Append(MailboxAssistantsWatermarks.BuildFormatedEventCounter(watermarks[i].EventCounter, watermarks[i].CreateTime));
				if (i != watermarks.Length - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			return stringBuilder.ToString();
		}

		private static string BuildFormatedEventCounter(long eventCounter, DateTime eventTime)
		{
			return string.Format("{0}:{1}", eventCounter, eventTime);
		}

		public const string WatermarkBehindWarningThreholdInMinutesPropertyName = "WatermarkBehindWarningThreholdInMinutes";

		private TimeSpan? watermarkBehindWarningThrehold = null;

		private static TimeSpan DefaultWatermarkBehindWarningThrehold = TimeSpan.FromMinutes(60.0);

		private static ServerVersion minExpectedServerVersion = new ServerVersion(14, 1, 0, 0);

		private class WatermarkWithCreateTime
		{
			public Guid ConsumerGuid { get; private set; }

			public long EventCounter { get; private set; }

			public DateTime CreateTime { get; private set; }

			public WatermarkWithCreateTime(Guid consumerGuid, long eventCounter, DateTime createTime)
			{
				this.ConsumerGuid = consumerGuid;
				this.EventCounter = eventCounter;
				this.CreateTime = createTime;
			}
		}
	}
}
