using System;
using System.Diagnostics;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class IsOffice365Domain : SingleStepServiceCommand<IsOffice365DomainRequest, bool>
	{
		public IsOffice365Domain(CallContext callContext, IsOffice365DomainRequest request) : base(callContext, request)
		{
			OwsLogRegistry.Register("IsOffice365Domain", typeof(IsOffice365DomainMetadata), new Type[0]);
			this.EmailAddress = SmtpAddress.Parse(request.EmailAddress);
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new IsOffice365DomainResponse(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		internal override ServiceResult<bool> Execute()
		{
			ServiceResult<bool> result = null;
			this.ExecuteWithProtocolLogging(IsOffice365DomainMetadata.TotalTime, delegate
			{
				try
				{
					ADSessionSettings.FromTenantAcceptedDomain(this.EmailAddress.Domain);
					result = new ServiceResult<bool>(true);
				}
				catch (CannotResolveTenantNameException arg)
				{
					this.tracer.TraceInformation<string, SmtpAddress, CannotResolveTenantNameException>(this.GetHashCode(), 0L, "Failed to find Office365 tenant with domain name: {0} for email address {1}. Exception: {2}", this.EmailAddress.Domain, this.EmailAddress, arg);
					result = new ServiceResult<bool>(false);
				}
			});
			return result;
		}

		private void ExecuteWithProtocolLogging(Enum logMetadata, Action operation)
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			operation();
			stopwatch.Stop();
			this.requestDetailsLogger_Set.Value(base.CallContext.ProtocolLog, logMetadata, stopwatch.ElapsedMilliseconds);
		}

		private readonly SmtpAddress EmailAddress;

		private readonly Microsoft.Exchange.Diagnostics.Trace tracer = ExTraceGlobals.IsOffice365DomainTracer;

		internal readonly Hookable<Func<RequestDetailsLogger, Enum, object, string>> requestDetailsLogger_Set = Hookable<Func<RequestDetailsLogger, Enum, object, string>>.Create(true, (RequestDetailsLogger protocolLog, Enum property, object value) => protocolLog.Set(property, value));
	}
}
