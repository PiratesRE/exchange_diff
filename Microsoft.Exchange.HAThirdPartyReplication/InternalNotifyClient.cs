using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	[DebuggerStepThrough]
	[GeneratedCode("System.ServiceModel", "3.0.0.0")]
	public class InternalNotifyClient : ClientBase<IInternalNotify>, IInternalNotify
	{
		public InternalNotifyClient()
		{
		}

		public InternalNotifyClient(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}

		public InternalNotifyClient(string endpointConfigurationName, string remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public InternalNotifyClient(string endpointConfigurationName, EndpointAddress remoteAddress) : base(endpointConfigurationName, remoteAddress)
		{
		}

		public InternalNotifyClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public void BecomePame()
		{
			base.Channel.BecomePame();
		}

		public void RevokePame()
		{
			base.Channel.RevokePame();
		}

		public NotificationResponse DatabaseMoveNeeded(Guid databaseId, string currentActiveFqdn, bool mountDesired)
		{
			return base.Channel.DatabaseMoveNeeded(databaseId, currentActiveFqdn, mountDesired);
		}

		public int GetTimeouts(out TimeSpan retryDelay, out TimeSpan openTimeout, out TimeSpan sendTimeout, out TimeSpan receiveTimeout)
		{
			return base.Channel.GetTimeouts(out retryDelay, out openTimeout, out sendTimeout, out receiveTimeout);
		}
	}
}
