using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal static class ReservationManager
	{
		static ReservationManager()
		{
			ReservationManager.wlmResourceStatsLastLoggingTimeUtc = DateTime.UtcNow;
		}

		private static bool WLMResourceStatsLogEnabled
		{
			get
			{
				return ConfigBase<MRSConfigSchema>.GetConfig<bool>("WLMResourceStatsLogEnabled");
			}
		}

		private static TimeSpan WLMResourceStatsLoggingPeriod
		{
			get
			{
				return ConfigBase<MRSConfigSchema>.GetConfig<TimeSpan>("WLMResourceStatsLoggingPeriod");
			}
		}

		public static ReservationBase CreateReservation(Guid mailboxGuid, TenantPartitionHint partitionHint, Guid resourceId, ReservationFlags flags, string clientName)
		{
			ReservationBase result;
			lock (ReservationManager.Locker)
			{
				ReservationBase reservationBase = ReservationBase.CreateReservation(mailboxGuid, partitionHint, resourceId, flags, clientName);
				ReservationManager.reservations.TryInsertSliding(reservationBase.ReservationId, reservationBase, ConfigBase<MRSConfigSchema>.GetConfig<TimeSpan>("ReservationExpirationInterval"));
				reservationBase.AddReleaseAction(new Action<ReservationBase>(ReservationManager.ReleaseReservation));
				result = reservationBase;
			}
			return result;
		}

		public static ReservationBase FindReservation(Guid reservationId)
		{
			if (reservationId == Guid.Empty)
			{
				return null;
			}
			ReservationBase result;
			lock (ReservationManager.Locker)
			{
				ReservationBase reservationBase;
				if (!ReservationManager.reservations.TryGetValue(reservationId, out reservationBase))
				{
					throw new ExpiredReservationException();
				}
				result = reservationBase;
			}
			return result;
		}

		public static void UpdateHealthState()
		{
			foreach (WorkloadType workloadType in ReservationManager.interestingWorkloadTypes)
			{
				MRSResource.Cache.GetInstance(MRSResource.Id.ObjectGuid, workloadType);
				LocalServerReadResource.Cache.GetInstance(LocalServerResource.ResourceId, workloadType);
				LocalServerWriteResource.Cache.GetInstance(LocalServerResource.ResourceId, workloadType);
			}
			foreach (Guid resourceID in MapiUtils.GetDatabasesOnThisServer())
			{
				foreach (WorkloadType workloadType2 in ReservationManager.interestingWorkloadTypes)
				{
					DatabaseReadResource.Cache.GetInstance(resourceID, workloadType2);
					DatabaseWriteResource.Cache.GetInstance(resourceID, workloadType2);
				}
			}
			bool logHealthState = false;
			TimeSpan t = DateTime.UtcNow - ReservationManager.wlmResourceStatsLastLoggingTimeUtc;
			if (ReservationManager.WLMResourceStatsLogEnabled && t >= ReservationManager.WLMResourceStatsLoggingPeriod)
			{
				logHealthState = true;
				ReservationManager.wlmResourceStatsLastLoggingTimeUtc = DateTime.UtcNow;
			}
			MRSResource.Cache.ForEach(delegate(MRSResource m)
			{
				m.UpdateHealthState(logHealthState);
			});
			LocalServerReadResource.Cache.ForEach(delegate(LocalServerReadResource m)
			{
				m.UpdateHealthState(logHealthState);
			});
			LocalServerWriteResource.Cache.ForEach(delegate(LocalServerWriteResource m)
			{
				m.UpdateHealthState(logHealthState);
			});
			DatabaseReadResource.Cache.ForEach(delegate(DatabaseReadResource m)
			{
				m.UpdateHealthState(logHealthState);
			});
			DatabaseWriteResource.Cache.ForEach(delegate(DatabaseWriteResource m)
			{
				m.UpdateHealthState(logHealthState);
			});
		}

		public static XElement GetReservationsDiagnosticInfo(MRSDiagnosticArgument arguments)
		{
			XElement xelement = new XElement("Reservations");
			lock (ReservationManager.Locker)
			{
				using (List<ReservationBase>.Enumerator enumerator = ReservationManager.reservations.Values.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ReservationBase reservation = enumerator.Current;
						xelement.Add(arguments.RunDiagnosticOperation(() => reservation.GetDiagnosticInfo(arguments)));
					}
				}
			}
			return xelement;
		}

		public static XElement GetResourcesDiagnosticInfo(MRSDiagnosticArgument arguments)
		{
			XElement root = new XElement("Resources");
			lock (ReservationManager.Locker)
			{
				MRSResource.Cache.ForEach(delegate(MRSResource m)
				{
					root.Add(arguments.RunDiagnosticOperation(() => m.GetDiagnosticInfo(arguments)));
				});
				LocalServerReadResource.Cache.ForEach(delegate(LocalServerReadResource m)
				{
					root.Add(arguments.RunDiagnosticOperation(() => m.GetDiagnosticInfo(arguments)));
				});
				LocalServerWriteResource.Cache.ForEach(delegate(LocalServerWriteResource m)
				{
					root.Add(arguments.RunDiagnosticOperation(() => m.GetDiagnosticInfo(arguments)));
				});
				DatabaseReadResource.Cache.ForEach(delegate(DatabaseReadResource m)
				{
					root.Add(arguments.RunDiagnosticOperation(() => m.GetDiagnosticInfo(arguments)));
				});
				DatabaseWriteResource.Cache.ForEach(delegate(DatabaseWriteResource m)
				{
					root.Add(arguments.RunDiagnosticOperation(() => m.GetDiagnosticInfo(arguments)));
				});
				MailboxMoveSourceResource.Cache.ForEach(delegate(MailboxMoveSourceResource m)
				{
					root.Add(arguments.RunDiagnosticOperation(() => m.GetDiagnosticInfo(arguments)));
				});
				MailboxMoveTargetResource.Cache.ForEach(delegate(MailboxMoveTargetResource m)
				{
					root.Add(arguments.RunDiagnosticOperation(() => m.GetDiagnosticInfo(arguments)));
				});
				MailboxMergeSourceResource.Cache.ForEach(delegate(MailboxMergeSourceResource m)
				{
					root.Add(arguments.RunDiagnosticOperation(() => m.GetDiagnosticInfo(arguments)));
				});
				MailboxMergeTargetResource.Cache.ForEach(delegate(MailboxMergeTargetResource m)
				{
					root.Add(arguments.RunDiagnosticOperation(() => m.GetDiagnosticInfo(arguments)));
				});
			}
			return root;
		}

		private static void ReleaseReservation(ReservationBase reservation)
		{
			lock (ReservationManager.Locker)
			{
				ReservationManager.reservations.Remove(reservation.ReservationId);
			}
		}

		private static bool ShouldRemoveReservation(Guid reservationID, ReservationBase reservation)
		{
			return !reservation.IsActive;
		}

		private static void RemoveReservationCallback(Guid reservationID, ReservationBase reservation, RemoveReason reason)
		{
			lock (ReservationManager.Locker)
			{
				if (!reservation.IsDisposed)
				{
					reservation.Dispose();
				}
			}
		}

		internal static readonly object Locker = new object();

		private static DateTime wlmResourceStatsLastLoggingTimeUtc;

		private static readonly ExactTimeoutCache<Guid, ReservationBase> reservations = new ExactTimeoutCache<Guid, ReservationBase>(new RemoveItemDelegate<Guid, ReservationBase>(ReservationManager.RemoveReservationCallback), new ShouldRemoveDelegate<Guid, ReservationBase>(ReservationManager.ShouldRemoveReservation), null, 10000, true);

		private static readonly WorkloadType[] interestingWorkloadTypes = new WorkloadType[]
		{
			WorkloadType.MailboxReplicationService,
			WorkloadType.MailboxReplicationServiceHighPriority
		};
	}
}
