using System;
using System.Management.Automation;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Net.DiagnosticsAggregation;

namespace Microsoft.Exchange.Management.QueueDigest
{
	internal abstract class GetQueueDigestAdapter
	{
		public abstract QueueDigestGroupBy GroupBy { get; }

		public abstract DetailsLevel DetailsLevel { get; }

		public abstract string Filter { get; }

		public abstract SwitchParameter IncludeE14Servers { get; }

		public abstract EnhancedTimeSpan Timeout { get; }

		public abstract Unlimited<uint> ResultSize { get; }

		public abstract bool IsVerbose { get; }

		public abstract void WriteDebug(string text);

		public abstract void WriteDebug(LocalizedString text);

		public abstract void WriteVerbose(LocalizedString text);

		public abstract void WriteWarning(LocalizedString text);

		public abstract void WriteObject(object sendToPipeline);

		public abstract IDiagnosticsAggregationService CreateWebServiceClient(Binding binding, EndpointAddress endpoint);

		public abstract void DisposeWebServiceClient(IDiagnosticsAggregationService client);
	}
}
