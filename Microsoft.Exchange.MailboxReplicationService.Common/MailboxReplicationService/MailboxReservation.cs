using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class MailboxReservation : ReservationBase
	{
		public MailboxReservation()
		{
			this.expirationTimestamp = DateTime.UtcNow + ConfigBase<MRSConfigSchema>.GetConfig<TimeSpan>("ReservationExpirationInterval");
		}

		public override bool IsActive
		{
			get
			{
				bool result;
				lock (this.locker)
				{
					result = (this.activeMailboxes.Count != 0 || !(DateTime.UtcNow > this.expirationTimestamp));
				}
				return result;
			}
		}

		public void Activate(Guid mailboxGuid)
		{
			lock (this.locker)
			{
				if (base.IsDisposed)
				{
					throw new ExpiredReservationException();
				}
				this.activeMailboxes.Add(mailboxGuid);
				this.expirationTimestamp = DateTime.MaxValue;
			}
		}

		public void DisconnectOrphanedSession(Guid mailboxGuid)
		{
			lock (this.locker)
			{
				if (this.activeMailboxes.Contains(mailboxGuid))
				{
					Action<Guid> action;
					if (this.disconnectOrphanedSessionActions.TryGetValue(mailboxGuid, out action))
					{
						action(mailboxGuid);
						this.disconnectOrphanedSessionActions.Remove(mailboxGuid);
					}
				}
			}
		}

		public void Deactivate(Guid mailboxGuid)
		{
			lock (this.locker)
			{
				this.activeMailboxes.Remove(mailboxGuid);
				this.disconnectOrphanedSessionActions.Remove(mailboxGuid);
				if (this.activeMailboxes.Count == 0)
				{
					this.expirationTimestamp = DateTime.UtcNow + ConfigBase<MRSConfigSchema>.GetConfig<TimeSpan>("ReservationExpirationInterval");
				}
			}
		}

		public void RegisterDisconnectOrphanedSessionAction(Guid mailboxGuid, Action<Guid> disconnectAction)
		{
			lock (this.locker)
			{
				this.disconnectOrphanedSessionActions[mailboxGuid] = disconnectAction;
			}
		}

		protected override void GetDiagnosticInfoInternal(XElement root)
		{
			base.GetDiagnosticInfoInternal(root);
			if (this.activeMailboxes.Count == 0)
			{
				root.Add(new XAttribute("ExpirationTS", this.expirationTimestamp));
			}
		}

		private object locker = new object();

		private HashSet<Guid> activeMailboxes = new HashSet<Guid>();

		private Dictionary<Guid, Action<Guid>> disconnectOrphanedSessionActions = new Dictionary<Guid, Action<Guid>>();

		private DateTime expirationTimestamp;
	}
}
