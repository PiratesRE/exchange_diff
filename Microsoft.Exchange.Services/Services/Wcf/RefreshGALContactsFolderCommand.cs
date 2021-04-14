using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.Performance;
using Microsoft.Exchange.Entities.People.EntitySets.ContactCommands;
using Microsoft.Exchange.Entities.People.EntitySets.ResponseTypes;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RefreshGALContactsFolderCommand : SingleStepServiceCommand<RefreshGALContactsFolderRequest, RefreshGALFolderResponseEntity>
	{
		public RefreshGALContactsFolderCommand(CallContext callContext, RefreshGALContactsFolderRequest request) : base(callContext, request)
		{
			this.InitializeTracers();
			this.perfLogger = new GALContactsRefreshRequestPerformanceLogger(base.CallContext.ProtocolLog, this.requestTracer);
		}

		internal override ServiceResult<RefreshGALFolderResponseEntity> Execute()
		{
			RefreshGalFolder refreshGalFolder = new RefreshGalFolder(base.CallContext.SessionCache.GetMailboxIdentityMailboxSession(), CallContext.Current.ADRecipientSessionContext.GetADRecipientSession(), this.tracer, this.perfLogger, new XSOFactory());
			return new ServiceResult<RefreshGALFolderResponseEntity>(refreshGalFolder.Execute(null));
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new RefreshGALContactsFolderResponse(base.Result);
		}

		protected override void LogTracesForCurrentRequest()
		{
			IOutgoingWebResponseContext outgoingWebResponseContext = base.CallContext.CreateWebResponseContext();
			ServiceCommandBase.TraceLoggerFactory.Create(outgoingWebResponseContext.Headers).LogTraces(this.requestTracer);
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
				tracer = new InMemoryTracer(ExTraceGlobals.RefreshGALContactsFolderTracer.Category, ExTraceGlobals.RefreshGALContactsFolderTracer.TraceTag);
			}
			this.requestTracer = tracer;
			this.tracer = ExTraceGlobals.RefreshGALContactsFolderTracer.Compose(this.requestTracer);
		}

		private ITracer tracer = ExTraceGlobals.RefreshGALContactsFolderTracer;

		private ITracer requestTracer = NullTracer.Instance;

		private readonly IPerformanceDataLogger perfLogger = NullPerformanceDataLogger.Instance;
	}
}
