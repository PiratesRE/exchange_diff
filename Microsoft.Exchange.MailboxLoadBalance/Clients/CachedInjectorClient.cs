using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance.Clients
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CachedInjectorClient : CachedClient, IInjectorService, IVersionedService, IDisposeTrackable, IDisposable
	{
		public CachedInjectorClient(IInjectorService client) : base(client as IWcfClient)
		{
			this.client = client;
		}

		void IVersionedService.ExchangeVersionInformation(VersionInformation clientVersion, out VersionInformation serverVersion)
		{
			this.client.ExchangeVersionInformation(clientVersion, out serverVersion);
		}

		void IInjectorService.InjectMoves(Guid targetDatabase, string batchName, IEnumerable<LoadEntity> mailboxes)
		{
			this.client.InjectMoves(targetDatabase, batchName, mailboxes);
		}

		void IInjectorService.InjectSingleMove(Guid targetDatabase, string batchName, LoadEntity mailbox)
		{
			this.client.InjectSingleMove(targetDatabase, batchName, mailbox);
		}

		internal override void Cleanup()
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CachedInjectorClient>(this);
		}

		private readonly IInjectorService client;
	}
}
