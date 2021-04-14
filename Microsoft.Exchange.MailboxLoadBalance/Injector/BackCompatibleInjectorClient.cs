using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance.Injector
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class BackCompatibleInjectorClient : DisposeTrackableBase, IInjectorService, IVersionedService, IDisposeTrackable, IDisposable
	{
		public BackCompatibleInjectorClient(MailboxLoadBalanceService service, MoveInjector moveInjector)
		{
			if (service == null)
			{
				throw new ArgumentNullException("service");
			}
			this.service = service;
			this.moveInjector = moveInjector;
		}

		public void ExchangeVersionInformation(VersionInformation clientVersion, out VersionInformation serverVersion)
		{
			serverVersion = LoadBalancerVersionInformation.InjectorVersion;
		}

		public void InjectMoves(Guid targetDatabase, string batchName, IEnumerable<LoadEntity> mailboxes)
		{
			this.moveInjector.InjectMovesOnCompatibilityMode(this.service.GetDatabaseData(targetDatabase, false), BatchName.FromString(batchName), mailboxes, false);
		}

		public void InjectSingleMove(Guid targetDatabase, string batchName, LoadEntity mailbox)
		{
			this.InjectMoves(targetDatabase, batchName, new LoadEntity[]
			{
				mailbox
			});
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<BackCompatibleInjectorClient>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		private readonly MailboxLoadBalanceService service;

		private readonly MoveInjector moveInjector;
	}
}
