using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class ReservationBase : DisposeTrackableBase, IReservation, IDisposable
	{
		public ReservationBase()
		{
			this.ReservationId = Guid.NewGuid();
			this.ReservedResources = new List<ResourceBase>();
			this.releaseActions = new List<Action<ReservationBase>>();
		}

		public Guid ReservationId { get; private set; }

		public Guid MailboxGuid { get; private set; }

		public TenantPartitionHint PartitionHint { get; private set; }

		public Guid ResourceId { get; private set; }

		public ReservationFlags Flags { get; private set; }

		public string ClientName { get; private set; }

		public List<ResourceBase> ReservedResources { get; private set; }

		public abstract bool IsActive { get; }

		public WorkloadType WorkloadType
		{
			get
			{
				if (this.Flags.HasFlag(ReservationFlags.HighPriority))
				{
					return WorkloadType.MailboxReplicationServiceHighPriority;
				}
				if (this.Flags.HasFlag(ReservationFlags.Interactive))
				{
					return WorkloadType.MailboxReplicationServiceInteractive;
				}
				if (this.Flags.HasFlag(ReservationFlags.InternalMaintenance))
				{
					return WorkloadType.MailboxReplicationServiceInternalMaintenance;
				}
				return WorkloadType.MailboxReplicationService;
			}
		}

		Guid IReservation.Id
		{
			get
			{
				return this.ReservationId;
			}
		}

		ReservationFlags IReservation.Flags
		{
			get
			{
				return this.Flags;
			}
		}

		Guid IReservation.ResourceId
		{
			get
			{
				return this.ResourceId;
			}
		}

		public static ReservationBase CreateReservation(Guid mailboxGuid, TenantPartitionHint partitionHint, Guid resourceId, ReservationFlags flags, string clientName)
		{
			ReservationBase result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				SettingsContextBase settingsContextBase = new MailboxSettingsContext(mailboxGuid, null);
				if (partitionHint != null)
				{
					settingsContextBase = new OrganizationSettingsContext(OrganizationId.FromExternalDirectoryOrganizationId(partitionHint.GetExternalDirectoryOrganizationId()), settingsContextBase);
				}
				ReservationBase reservationBase;
				if (resourceId == MRSResource.Id.ObjectGuid)
				{
					reservationBase = new MRSReservation();
				}
				else
				{
					if (flags.HasFlag(ReservationFlags.Read))
					{
						reservationBase = new ReadReservation();
					}
					else
					{
						reservationBase = new WriteReservation();
					}
					settingsContextBase = new DatabaseSettingsContext(resourceId, settingsContextBase);
				}
				disposeGuard.Add<ReservationBase>(reservationBase);
				settingsContextBase = new GenericSettingsContext("WorkloadType", reservationBase.WorkloadType.ToString(), settingsContextBase);
				reservationBase.MailboxGuid = mailboxGuid;
				reservationBase.PartitionHint = partitionHint;
				reservationBase.ResourceId = resourceId;
				reservationBase.Flags = flags;
				reservationBase.ClientName = clientName;
				using (settingsContextBase.Activate())
				{
					reservationBase.ReserveResources();
				}
				disposeGuard.Success();
				result = reservationBase;
			}
			return result;
		}

		public void AddReleaseAction(Action<ReservationBase> releaseAction)
		{
			lock (this.locker)
			{
				this.releaseActions.Add(releaseAction);
			}
		}

		public XElement GetDiagnosticInfo(MRSDiagnosticArgument arguments)
		{
			XElement xelement = new XElement("Reservation");
			this.GetDiagnosticInfoInternal(xelement);
			return xelement;
		}

		protected abstract IEnumerable<ResourceBase> GetDependentResources();

		protected virtual void GetDiagnosticInfoInternal(XElement root)
		{
			root.Add(new XAttribute("Id", this.ReservationId));
			root.Add(new XAttribute("Flags", this.Flags.ToString()));
			root.Add(new XAttribute("MbxGuid", this.MailboxGuid));
			if (this.PartitionHint != null)
			{
				root.Add(new XAttribute("Partition", this.PartitionHint.ToString()));
			}
			foreach (ResourceBase resourceBase in this.ReservedResources)
			{
				root.Add(new XElement("Resource", new object[]
				{
					new XAttribute("Type", resourceBase.ResourceType),
					new XAttribute("Name", resourceBase.ResourceName)
				}));
			}
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose)
			{
				lock (this.locker)
				{
					using (List<Action<ReservationBase>>.Enumerator enumerator = this.releaseActions.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							Action<ReservationBase> releaseAction = enumerator.Current;
							CommonUtils.CatchKnownExceptions(delegate
							{
								releaseAction(this);
							}, null);
						}
					}
					this.releaseActions.Clear();
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ReservationBase>(this);
		}

		private void ReserveResources()
		{
			foreach (ResourceBase resourceBase in this.GetDependentResources())
			{
				resourceBase.Reserve(this);
				this.ReservedResources.Add(resourceBase);
			}
			MrsTracer.Throttling.Debug("Successful reservation: Id {0}, mbxGuid {1}, resourceID {2}, flags {3}", new object[]
			{
				this.ReservationId,
				this.MailboxGuid,
				this.ResourceId,
				this.Flags
			});
		}

		private List<Action<ReservationBase>> releaseActions;

		private object locker = new object();
	}
}
