using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class ConvertId : MultiStepServiceCommand<ConvertIdRequest, AlternateIdBase>
	{
		public ConvertId(CallContext callContext, ConvertIdRequest request) : base(callContext, request)
		{
			this.destinationFormat = request.DestinationFormat;
			this.sourceIds = request.SourceIds;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			ConvertIdResponse convertIdResponse = new ConvertIdResponse();
			convertIdResponse.AddResponses(base.Results);
			return convertIdResponse;
		}

		internal override ServiceResult<AlternateIdBase> Execute()
		{
			int num = 0;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<int>(2647010621U, ref num);
			if (num != 0)
			{
				Thread.Sleep(num);
			}
			AlternateIdBase alternateIdBase = this.sourceIds[base.CurrentStep];
			AlternateIdBase value = alternateIdBase.ConvertId(this.destinationFormat);
			return new ServiceResult<AlternateIdBase>(value);
		}

		internal override int StepCount
		{
			get
			{
				return this.sourceIds.Length;
			}
		}

		private IdFormat destinationFormat;

		private AlternateIdBase[] sourceIds;
	}
}
