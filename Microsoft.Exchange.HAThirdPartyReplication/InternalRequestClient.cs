using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	[DebuggerStepThrough]
	[GeneratedCode("System.ServiceModel", "3.0.0.0")]
	public class InternalRequestClient : ClientBase<IInternalRequest>, IInternalRequest
	{
		public InternalRequestClient()
		{
		}

		public InternalRequestClient(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}

		public InternalRequestClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public InternalRequestClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public InternalRequestClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public string GetPrimaryActiveManager(out byte[] ex)
		{
			return base.Channel.GetPrimaryActiveManager(out ex);
		}

		public byte[] ChangeActiveServer(Guid databaseId, string newActiveServerName)
		{
			return base.Channel.ChangeActiveServer(databaseId, newActiveServerName);
		}

		public byte[] ImmediateDismountMailboxDatabase(Guid databaseId)
		{
			return base.Channel.ImmediateDismountMailboxDatabase(databaseId);
		}

		public void AmeIsStarting(TimeSpan retryDelay, TimeSpan openTimeout, TimeSpan sendTimeout, TimeSpan receiveTimeout)
		{
			base.Channel.AmeIsStarting(retryDelay, openTimeout, sendTimeout, receiveTimeout);
		}

		public void AmeIsStopping()
		{
			base.Channel.AmeIsStopping();
		}
	}
}
