using System;
using System.Management.Automation;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.DiagnosticsAggregation;

namespace Microsoft.Exchange.Management.QueueDigest
{
	internal class GetQueueDigestCmdletAdapter : GetQueueDigestAdapter
	{
		public GetQueueDigestCmdletAdapter(GetQueueDigest cmdlet)
		{
			this.cmdlet = cmdlet;
		}

		public override QueueDigestGroupBy GroupBy
		{
			get
			{
				return this.cmdlet.GroupBy;
			}
		}

		public override DetailsLevel DetailsLevel
		{
			get
			{
				return this.cmdlet.DetailsLevel;
			}
		}

		public override string Filter
		{
			get
			{
				return this.cmdlet.Filter;
			}
		}

		public override SwitchParameter IncludeE14Servers
		{
			get
			{
				return this.cmdlet.IncludeE14Servers;
			}
		}

		public override EnhancedTimeSpan Timeout
		{
			get
			{
				return this.cmdlet.Timeout;
			}
		}

		public override Unlimited<uint> ResultSize
		{
			get
			{
				return this.cmdlet.ResultSize;
			}
		}

		public override bool IsVerbose
		{
			get
			{
				return this.cmdlet.IsVerbose;
			}
		}

		public override void WriteDebug(string text)
		{
			this.cmdlet.WriteDebug(text);
		}

		public override void WriteDebug(LocalizedString text)
		{
			this.cmdlet.WriteDebug(text);
		}

		public override void WriteVerbose(LocalizedString text)
		{
			this.cmdlet.WriteVerbose(text);
		}

		public override void WriteWarning(LocalizedString text)
		{
			this.cmdlet.WriteWarning(text);
		}

		public override void WriteObject(object sendToPipeline)
		{
			this.cmdlet.WriteObject(sendToPipeline);
		}

		public override IDiagnosticsAggregationService CreateWebServiceClient(Binding binding, EndpointAddress endpoint)
		{
			return new DiagnosticsAggregationServiceClient(binding, endpoint);
		}

		public override void DisposeWebServiceClient(IDiagnosticsAggregationService client)
		{
			if (client != null)
			{
				WcfUtils.DisposeWcfClientGracefully((DiagnosticsAggregationServiceClient)client, false);
			}
		}

		private GetQueueDigest cmdlet;
	}
}
