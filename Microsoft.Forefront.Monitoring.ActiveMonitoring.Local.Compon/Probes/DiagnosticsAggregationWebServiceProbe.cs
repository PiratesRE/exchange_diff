using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Net.DiagnosticsAggregation;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.Transport.DiagnosticsAggregationService;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Probes
{
	public class DiagnosticsAggregationWebServiceProbe : ProbeWorkItem
	{
		private static ConfigurationLoader<TransportSettingsConfiguration, TransportSettingsConfiguration.Builder> TransportSettings
		{
			get
			{
				if (DiagnosticsAggregationWebServiceProbe.transportSettings == null)
				{
					lock (DiagnosticsAggregationWebServiceProbe.locker)
					{
						if (DiagnosticsAggregationWebServiceProbe.transportSettings == null)
						{
							ConfigurationLoader<TransportSettingsConfiguration, TransportSettingsConfiguration.Builder> configurationLoader = new ConfigurationLoader<TransportSettingsConfiguration, TransportSettingsConfiguration.Builder>(TimeSpan.Zero);
							configurationLoader.Load();
							DiagnosticsAggregationWebServiceProbe.transportSettings = configurationLoader;
						}
					}
				}
				return DiagnosticsAggregationWebServiceProbe.transportSettings;
			}
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			string uri = string.Format(CultureInfo.InvariantCulture, DiagnosticsAggregationHelper.DiagnosticsAggregationEndpointFormat, new object[]
			{
				"localhost",
				DiagnosticsAggregationWebServiceProbe.TransportSettings.Cache.TransportSettings.DiagnosticsAggregationServicePort
			});
			using (DiagnosticsAggregationServiceClient diagnosticsAggregationServiceClient = new DiagnosticsAggregationServiceClient(this.GetWebServiceBinding(), new EndpointAddress(uri)))
			{
				LocalViewRequest localViewRequest = new LocalViewRequest(RequestType.Queues);
				localViewRequest.QueueLocalViewRequest = new QueueLocalViewRequest();
				try
				{
					LocalViewResponse localView = diagnosticsAggregationServiceClient.GetLocalView(localViewRequest);
					if (localView.QueueLocalViewResponse == null)
					{
						throw new ApplicationException("GetLocalView returned with a null QueueLocalViewResponse");
					}
				}
				catch (Exception)
				{
					diagnosticsAggregationServiceClient.Abort();
					throw;
				}
				ProbeResult result = base.Result;
				result.ExecutionContext += "GetLocalView succeeded. ";
				AggregatedViewRequest aggregatedViewRequest = new AggregatedViewRequest(RequestType.Queues, new List<string>
				{
					Environment.MachineName
				}, 1U);
				aggregatedViewRequest.QueueAggregatedViewRequest = new QueueAggregatedViewRequest(QueueDigestGroupBy.NextHopDomain, DetailsLevel.Normal, null);
				try
				{
					AggregatedViewResponse aggregatedView = diagnosticsAggregationServiceClient.GetAggregatedView(aggregatedViewRequest);
					if (aggregatedView.QueueAggregatedViewResponse == null)
					{
						throw new ApplicationException("GetAggregatedView returned with a null QueueAggregatedViewResponse");
					}
				}
				catch (Exception)
				{
					diagnosticsAggregationServiceClient.Abort();
					throw;
				}
				ProbeResult result2 = base.Result;
				result2.ExecutionContext += "GetAggregatedView succeeded. ";
			}
		}

		private Binding GetWebServiceBinding()
		{
			return new NetTcpBinding
			{
				Security = 
				{
					Transport = 
					{
						ProtectionLevel = ProtectionLevel.EncryptAndSign,
						ClientCredentialType = TcpClientCredentialType.Windows
					},
					Message = 
					{
						ClientCredentialType = MessageCredentialType.Windows
					}
				},
				MaxReceivedMessageSize = (long)((int)ByteQuantifiedSize.FromMB(5UL).ToBytes()),
				OpenTimeout = this.timeout,
				CloseTimeout = this.timeout,
				SendTimeout = this.timeout,
				ReceiveTimeout = this.timeout
			};
		}

		private static object locker = new object();

		private static ConfigurationLoader<TransportSettingsConfiguration, TransportSettingsConfiguration.Builder> transportSettings;

		private readonly EnhancedTimeSpan timeout = EnhancedTimeSpan.FromSeconds(10.0);
	}
}
