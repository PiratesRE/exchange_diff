using System;
using Microsoft.Exchange.Data.ApplicationLogic.PeopleIKnowService;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetPeopleIKnowGraphCommand : ServiceCommand<GetPeopleIKnowGraphResponse>
	{
		public GetPeopleIKnowGraphCommand(CallContext context, GetPeopleIKnowGraphRequest request) : base(context)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			this.InitializeTracers();
		}

		protected override GetPeopleIKnowGraphResponse InternalExecute()
		{
			IPeopleIKnowSerializerFactory serializerFactory = new PeopleIKnowSerializerFactory();
			IPeopleIKnowServiceFactory peopleIKnowServiceFactory = new PeopleIKnowServiceFactory(serializerFactory);
			IPeopleIKnowService peopleIKnowService = peopleIKnowServiceFactory.CreatePeopleIKnowService(this.tracer);
			string serializedPeopleIKnowGraph = peopleIKnowService.GetSerializedPeopleIKnowGraph(base.MailboxIdentityMailboxSession, new XSOFactory());
			return new GetPeopleIKnowGraphResponse
			{
				SerializedPeopleIKnowGraph = serializedPeopleIKnowGraph
			};
		}

		protected override void LogTracesForCurrentRequest()
		{
			IOutgoingWebResponseContext outgoingWebResponseContext = base.CallContext.CreateWebResponseContext();
			WcfServiceCommandBase.TraceLoggerFactory.Create(outgoingWebResponseContext.Headers).LogTraces(this.requestTracer);
		}

		private void InitializeTracers()
		{
			ITracer tracer;
			if (!base.IsRequestTracingEnabled)
			{
				ITracer instance = NullTracer.Instance;
				tracer = instance;
			}
			else
			{
				tracer = new InMemoryTracer(ExTraceGlobals.GetPeopleIKnowGraphCallTracer.Category, ExTraceGlobals.GetPeopleIKnowGraphCallTracer.TraceTag);
			}
			this.requestTracer = tracer;
			this.tracer = ExTraceGlobals.GetPeopleIKnowGraphCallTracer.Compose(this.requestTracer);
		}

		private ITracer tracer = ExTraceGlobals.GetPeopleIKnowGraphCallTracer;

		private ITracer requestTracer = NullTracer.Instance;
	}
}
