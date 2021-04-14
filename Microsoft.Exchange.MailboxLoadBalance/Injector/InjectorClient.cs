using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Providers;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;

namespace Microsoft.Exchange.MailboxLoadBalance.Injector
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InjectorClient : VersionedClientBase<IInjectorService>, IInjectorService, IVersionedService, IDisposeTrackable, IDisposable
	{
		private InjectorClient(Binding binding, EndpointAddress remoteAddress, ILogger logger) : base(binding, remoteAddress, logger)
		{
		}

		public static InjectorClient Create(string serverName, IDirectoryProvider directory, ILogger logger)
		{
			Func<Binding, EndpointAddress, ILogger, InjectorClient> constructor = (Binding binding, EndpointAddress endpointAddress, ILogger log) => new InjectorClient(binding, endpointAddress, log);
			return VersionedClientBase<IInjectorService>.CreateClient<InjectorClient>(serverName, InjectorService.EndpointAddress, constructor, logger);
		}

		public void InjectMoves(Guid targetDatabase, string batchName, IEnumerable<LoadEntity> mailboxes)
		{
			base.Logger.Log(MigrationEventType.Instrumentation, "Begin injecting '{0}' moves into database '{1}' with batch name '{2}'", new object[]
			{
				mailboxes.Count<LoadEntity>(),
				targetDatabase,
				batchName
			});
			int injectionBatchSize = LoadBalanceADSettings.Instance.Value.InjectionBatchSize;
			foreach (IEnumerable<LoadEntity> enumerable in mailboxes.Batch(injectionBatchSize))
			{
				base.Logger.Log(MigrationEventType.Instrumentation, "Set of {0} moves being injected into database '{1}' with batch name '{2}'", new object[]
				{
					enumerable.Count<LoadEntity>(),
					targetDatabase,
					batchName
				});
				IEnumerable<LoadEntity> batch = enumerable;
				base.CallService(delegate()
				{
					this.Channel.InjectMoves(targetDatabase, batchName, batch);
				});
			}
		}

		public void InjectSingleMove(Guid targetDatabase, string batchName, LoadEntity mailbox)
		{
			base.CallService(delegate()
			{
				this.Channel.InjectSingleMove(targetDatabase, batchName, mailbox);
			});
		}
	}
}
