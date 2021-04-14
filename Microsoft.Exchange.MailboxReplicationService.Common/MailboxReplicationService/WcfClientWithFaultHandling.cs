using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal abstract class WcfClientWithFaultHandling<TChannel, TFault> : WcfClientBase<TChannel> where TChannel : class where TFault : FaultException
	{
		public WcfClientWithFaultHandling(string endpointConfigurationName) : base(endpointConfigurationName)
		{
		}

		public WcfClientWithFaultHandling(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress)
		{
		}

		public VersionInformation ServerVersion { get; protected set; }

		protected abstract void HandleFaultException(TFault fault, string context);

		protected override void CallService(Action serviceCall, string context)
		{
			string serverNameVersion = context;
			if (this.ServerVersion != null)
			{
				serverNameVersion = string.Format("{0} {1} ({2})", context, this.ServerVersion.ComputerName, this.ServerVersion.ToString());
			}
			base.CallService(delegate
			{
				try
				{
					serviceCall();
				}
				catch (TFault tfault)
				{
					TFault fault = (TFault)((object)tfault);
					this.HandleFaultException(fault, serverNameVersion);
				}
			}, serverNameVersion);
		}
	}
}
