using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Injector
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ConsumerMetricsInjectorCapabilityDecorator : MissingCapabilityInjectorClientDecorator
	{
		public ConsumerMetricsInjectorCapabilityDecorator(IInjectorService service) : base(service)
		{
		}

		public override void InjectMoves(Guid targetDatabase, string batchName, IEnumerable<LoadEntity> mailboxes)
		{
			base.InjectMoves(targetDatabase, batchName, from m in mailboxes
			select m.ToSerializationFormat(true));
		}

		public override void InjectSingleMove(Guid targetDatabase, string batchName, LoadEntity mailbox)
		{
			base.InjectSingleMove(targetDatabase, batchName, mailbox.ToSerializationFormat(true));
		}
	}
}
