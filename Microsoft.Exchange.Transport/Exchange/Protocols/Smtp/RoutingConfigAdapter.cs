using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class RoutingConfigAdapter : IRoutingConfigProvider
	{
		public bool CheckDagSelectorHeader
		{
			get
			{
				return this.appConfig.Routing.CheckDagSelectorHeader;
			}
		}

		public bool LocalLoopDetectionEnabled
		{
			get
			{
				return this.appConfig.Routing.LocalLoopDetectionEnabled;
			}
		}

		public int LocalLoopDetectionSubDomainLeftToRightOffsetForPerfCounter
		{
			get
			{
				return this.appConfig.Routing.LocalLoopDetectionSubDomainLeftToRightOffsetForPerfCounter;
			}
		}

		public List<int> LocalLoopMessageDeferralIntervals
		{
			get
			{
				return this.appConfig.Routing.LocalLoopMessageDeferralIntervals.ToList<int>();
			}
		}

		public int LocalLoopSubdomainDepth
		{
			get
			{
				return this.appConfig.Routing.LocalLoopSubdomainDepth;
			}
		}

		public int LoopDetectionNumberOfTransits
		{
			get
			{
				return this.appConfig.Routing.LoopDetectionNumberOfTransits;
			}
		}

		public static IRoutingConfigProvider Create(ITransportAppConfig appConfig)
		{
			ArgumentValidator.ThrowIfNull("appConfig", appConfig);
			return new RoutingConfigAdapter(appConfig);
		}

		private RoutingConfigAdapter(ITransportAppConfig appConfig)
		{
			this.appConfig = appConfig;
		}

		private readonly ITransportAppConfig appConfig;
	}
}
