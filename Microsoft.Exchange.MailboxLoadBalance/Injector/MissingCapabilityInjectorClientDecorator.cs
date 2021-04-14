using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance.Injector
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MissingCapabilityInjectorClientDecorator : DisposeTrackableBase, IInjectorService, IVersionedService, IDisposeTrackable, IDisposable
	{
		protected MissingCapabilityInjectorClientDecorator(IInjectorService service)
		{
			this.service = service;
		}

		public virtual void ExchangeVersionInformation(VersionInformation clientVersion, out VersionInformation serverVersion)
		{
			this.service.ExchangeVersionInformation(clientVersion, out serverVersion);
		}

		public virtual void InjectMoves(Guid targetDatabase, string batchName, IEnumerable<LoadEntity> mailboxes)
		{
			this.service.InjectMoves(targetDatabase, batchName, mailboxes);
		}

		public virtual void InjectSingleMove(Guid targetDatabase, string batchName, LoadEntity mailbox)
		{
			this.service.InjectSingleMove(targetDatabase, batchName, mailbox);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (this.service != null)
			{
				this.service.Dispose();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MissingCapabilityInjectorClientDecorator>(this);
		}

		private readonly IInjectorService service;
	}
}
