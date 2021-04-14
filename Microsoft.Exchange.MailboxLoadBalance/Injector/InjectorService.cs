using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Providers;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance.Injector
{
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class InjectorService : VersionedServiceBase, IInjectorService, IVersionedService, IDisposeTrackable, IDisposable
	{
		public InjectorService(IDirectoryProvider directory, ILogger logger, MoveInjector moveInjector) : base(logger)
		{
			this.directory = directory;
			this.moveInjector = moveInjector;
		}

		public static ServiceEndpointAddress EndpointAddress
		{
			get
			{
				return InjectorService.EndpointAddressHook.Value;
			}
		}

		protected override VersionInformation ServiceVersion
		{
			get
			{
				return LoadBalancerVersionInformation.InjectorVersion;
			}
		}

		void IInjectorService.InjectMoves(Guid targetDatabase, string batchName, IEnumerable<LoadEntity> mailboxes)
		{
			base.ForwardExceptions(delegate()
			{
				DirectoryReconnectionVisitor visitor = new DirectoryReconnectionVisitor(this.directory, this.Logger);
				IList<LoadEntity> list = (mailboxes as IList<LoadEntity>) ?? mailboxes.ToList<LoadEntity>();
				foreach (LoadEntity loadEntity in list)
				{
					loadEntity.Accept(visitor);
				}
				this.moveInjector.InjectMoves(targetDatabase, BatchName.FromString(batchName), list, false);
			});
		}

		void IInjectorService.InjectSingleMove(Guid targetDatabase, string batchName, LoadEntity mailbox)
		{
			base.ForwardExceptions(delegate()
			{
				DirectoryReconnectionVisitor visitor = new DirectoryReconnectionVisitor(this.directory, this.Logger);
				mailbox.Accept(visitor);
				this.moveInjector.InjectMoves(targetDatabase, BatchName.FromString(batchName), new LoadEntity[]
				{
					mailbox
				}, false);
			});
		}

		protected internal static IDisposable SetEndpointAddress(ServiceEndpointAddress endpointAddress)
		{
			return InjectorService.EndpointAddressHook.SetTestHook(endpointAddress);
		}

		private const string EndpointSuffix = "Microsoft.Exchange.MailboxLoadBalance.InjectorService";

		private static readonly Hookable<ServiceEndpointAddress> EndpointAddressHook = Hookable<ServiceEndpointAddress>.Create(true, new ServiceEndpointAddress("Microsoft.Exchange.MailboxLoadBalance.InjectorService"));

		private readonly IDirectoryProvider directory;

		private readonly MoveInjector moveInjector;
	}
}
