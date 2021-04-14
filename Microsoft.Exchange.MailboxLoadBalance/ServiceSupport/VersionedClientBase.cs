using System;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Clients;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance.ServiceSupport
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class VersionedClientBase<TService> : WcfClientWithFaultHandling<TService, FaultException<LoadBalanceFault>>, IWcfClient where TService : class, IVersionedService
	{
		protected VersionedClientBase(Binding binding, EndpointAddress remoteAddress, ILogger logger) : base(binding, remoteAddress)
		{
			this.logger = logger;
			LoadBalanceUtils.SetDataContractSerializerBehavior(base.ChannelFactory.Endpoint.Contract);
		}

		public static bool UseUpdatedBinding { get; set; }

		public bool IsValid
		{
			get
			{
				switch (base.State)
				{
				case CommunicationState.Created:
				case CommunicationState.Opening:
				case CommunicationState.Opened:
					return true;
				default:
					return false;
				}
			}
		}

		protected ILogger Logger
		{
			get
			{
				return this.logger;
			}
		}

		public void Disconnect()
		{
			this.Dispose();
		}

		public void ExchangeVersionInformation(VersionInformation clientVersion, out VersionInformation serverVersion)
		{
			VersionInformation serverVersionInfo = null;
			this.CallService(delegate()
			{
				TService channel = this.Channel;
				channel.ExchangeVersionInformation(clientVersion, out serverVersionInfo);
			});
			serverVersion = serverVersionInfo;
		}

		protected static TClient CreateClient<TClient>(string serverName, ServiceEndpointAddress serviceEndpoint, Func<Binding, EndpointAddress, ILogger, TClient> constructor, ILogger logger) where TClient : VersionedClientBase<TService>
		{
			logger.Log(MigrationEventType.Verbose, "{0}: Attempting TCP connection to {1}/{2}", new object[]
			{
				typeof(TClient).Name,
				serverName,
				serviceEndpoint
			});
			string address = serviceEndpoint.GetAddress(serverName);
			NetTcpBinding netTcpBinding = new NetTcpBinding(SecurityMode.Transport);
			netTcpBinding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;
			netTcpBinding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
			netTcpBinding.Security.Message.ClientCredentialType = MessageCredentialType.Windows;
			netTcpBinding.MaxReceivedMessageSize = 10485760L;
			netTcpBinding.ReceiveTimeout = TimeSpan.FromMinutes(10.0);
			netTcpBinding.SendTimeout = TimeSpan.FromMinutes(10.0);
			EndpointAddress arg;
			try
			{
				arg = new EndpointAddress(address);
			}
			catch (UriFormatException ex)
			{
				throw new InvalidEndpointAddressException(ex.GetType().Name, address, ex);
			}
			TClient tclient = default(TClient);
			bool flag = false;
			try
			{
				tclient = constructor(netTcpBinding, arg, logger);
				if (VersionedClientBase<TService>.UseUpdatedBinding)
				{
					logger.LogVerbose("Using binging: {0} ({1})", new object[]
					{
						netTcpBinding.Name,
						netTcpBinding.MessageVersion
					});
					LoadBalanceUtils.UpdateAndLogServiceEndpoint(logger, tclient.Endpoint);
				}
				tclient.ExchangeVersionInformation();
				flag = true;
			}
			finally
			{
				if (!flag && tclient != null)
				{
					tclient.Dispose();
				}
			}
			logger.Log(MigrationEventType.Verbose, "{0}: Established connection to {1}, version={2}", new object[]
			{
				typeof(TClient).Name,
				tclient.ServerVersion.ComputerName,
				tclient.ServerVersion.ToString()
			});
			return tclient;
		}

		protected void CallService(Action action)
		{
			this.logger.Log(MigrationEventType.Instrumentation, "BEGIN remote service call: {0}", new object[]
			{
				action.Method
			});
			try
			{
				this.CallService(action, base.Endpoint.Address.ToString());
			}
			finally
			{
				this.logger.Log(MigrationEventType.Instrumentation, "End remote service call: {0}", new object[]
				{
					action.Method
				});
			}
		}

		protected TResult CallService<TResult>(Func<TResult> action)
		{
			TResult result = default(TResult);
			this.logger.Log(MigrationEventType.Instrumentation, "BEGIN remote service call: {0}", new object[]
			{
				action.Method
			});
			try
			{
				this.CallService(delegate
				{
					result = action();
				}, base.Endpoint.Address.ToString());
			}
			finally
			{
				this.logger.Log(MigrationEventType.Instrumentation, "End remote service call: {0}", new object[]
				{
					action.Method
				});
			}
			return result;
		}

		protected void ExchangeVersionInformation()
		{
			VersionInformation serverVersion = null;
			this.CallService(delegate()
			{
				TService channel = this.Channel;
				channel.ExchangeVersionInformation(LoadBalancerVersionInformation.LoadBalancerVersion, out serverVersion);
			});
			base.ServerVersion = serverVersion;
		}

		protected override void HandleFaultException(FaultException<LoadBalanceFault> fault, string context)
		{
			this.logger.LogError(fault, "Error processing service call from server {0}.", new object[]
			{
				context
			});
			fault.Detail.ReconstructAndThrow();
		}

		private readonly ILogger logger;
	}
}
