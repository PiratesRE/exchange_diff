using System;
using System.IO;
using System.Net;
using System.Net.Security;
using Microsoft.Exchange.Data.ApplicationLogic.Performance;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.Performance;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetPeopleICommunicateWith : SingleStepServiceCommand<GetPeopleICommunicateWithRequest, Stream>
	{
		static GetPeopleICommunicateWith()
		{
			CertificateValidationManager.RegisterCallback("GetPeopleICommunicateWith", new RemoteCertificateValidationCallback(CommonCertificateValidationCallbacks.InternalServerToServer));
		}

		public GetPeopleICommunicateWith(CallContext callContext, GetPeopleICommunicateWithRequest request) : base(callContext, request)
		{
			this.requestId = this.ComputeRequestId();
			this.clientRequestId = this.ComputeClientRequestId();
			this.requestTracer = new InMemoryTracer(ExTraceGlobals.PeopleICommunicateWithTracer.Category, ExTraceGlobals.PeopleICommunicateWithTracer.TraceTag);
			this.tracer = ExTraceGlobals.PeopleICommunicateWithTracer.Compose(this.requestTracer);
			OwsLogRegistry.Register("GetPeopleICommunicateWith", typeof(GetPeopleICommunicateWithMetadata), new Type[0]);
			this.OutgoingResponse = request.OutgoingResponse;
		}

		private IOutgoingWebResponseContext OutgoingResponse { get; set; }

		internal override IExchangeWebMethodResponse GetResponse()
		{
			GetPeopleICommunicateWithResponseMessage message = new GetPeopleICommunicateWithResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
			GetPeopleICommunicateWithResponse getPeopleICommunicateWithResponse = new GetPeopleICommunicateWithResponse();
			getPeopleICommunicateWithResponse.AddResponse(message);
			return getPeopleICommunicateWithResponse;
		}

		internal override ServiceResult<Stream> Execute()
		{
			ServiceResult<Stream> result;
			try
			{
				this.tracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "GetPeopleICommunicateWith.Execute: executing on host {0}.  Request-Id: {1}.  Client-Request-Id: {2}", this.GetIncomingRequestHost(), this.requestId, this.clientRequestId);
				using (new StopwatchPerformanceTracker("GetPeopleICommunicateWithTotal", this.perfLogger))
				{
					using (new ADPerformanceTracker("GetPeopleICommunicateWithTotal", this.perfLogger))
					{
						using (new StorePerformanceTracker("GetPeopleICommunicateWithTotal", this.perfLogger))
						{
							result = this.ExecuteGetPeopleICommunicateWith();
						}
					}
				}
			}
			catch (Exception caughtException)
			{
				result = this.TraceExceptionAndReturnInternalServerError(this.OutgoingResponse, caughtException);
			}
			return result;
		}

		private string ComputeRequestId()
		{
			if (!string.IsNullOrEmpty(this.requestId))
			{
				return this.requestId;
			}
			if (base.CallContext.ProtocolLog == null)
			{
				return Guid.NewGuid().ToString();
			}
			if (!Guid.Empty.Equals(base.CallContext.ProtocolLog.ActivityId))
			{
				return base.CallContext.ProtocolLog.ActivityId.ToString();
			}
			return Guid.NewGuid().ToString();
		}

		private string ComputeClientRequestId()
		{
			if (!string.IsNullOrEmpty(this.clientRequestId))
			{
				return this.clientRequestId;
			}
			if (base.CallContext.ProtocolLog == null || base.CallContext.ProtocolLog.ActivityScope == null)
			{
				return this.ComputeRequestId();
			}
			string text = base.CallContext.ProtocolLog.ActivityScope.ClientRequestId;
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return this.ComputeRequestId();
		}

		private ServiceResult<Stream> ExecuteGetPeopleICommunicateWith()
		{
			if (base.CallContext.AccessingPrincipal == null || base.CallContext.AccessingPrincipal.MailboxInfo == null)
			{
				this.tracer.TraceError((long)this.GetHashCode(), "CallContext.AccessingPrincipal=null, InboxRulesCommandBaseRequest.MailboxSmtpAddress not specified. NonExistentMailbox error returned.");
				throw new NonExistentMailboxException((CoreResources.IDs)2489326695U, (base.CallContext.EffectiveCaller == null) ? string.Empty : base.CallContext.EffectiveCaller.PrimarySmtpAddress);
			}
			base.CallContext.ProtocolLog.Set(GetPeopleICommunicateWithMetadata.TargetEmailAddress, base.CallContext.AccessingPrincipal.MailboxInfo.PrimarySmtpAddress);
			MemoryStream memoryStream = new MemoryStream();
			ServiceResult<Stream> result;
			using (DisposeGuard disposeGuard = memoryStream.Guard())
			{
				byte[] array = new byte[]
				{
					1
				};
				memoryStream.Write(array, 0, array.Length);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				string text = "application/octet-stream";
				this.OutgoingResponse.ContentType = text;
				base.CallContext.ProtocolLog.Set(GetPeopleICommunicateWithMetadata.ResponseContentType, text);
				this.OutgoingResponse.StatusCode = HttpStatusCode.OK;
				this.tracer.TraceDebug<HttpStatusCode, string, int>((long)this.GetHashCode(), "GetPeopleICommunicateWith: request completed.  Status: {0};  Content-Type: {1};  Content-Length: {2}", this.OutgoingResponse.StatusCode, text, array.Length);
				disposeGuard.Success();
				result = new ServiceResult<Stream>(memoryStream);
			}
			return result;
		}

		private string GetIncomingRequestHost()
		{
			if (base.CallContext == null || base.CallContext.HttpContext == null || base.CallContext.HttpContext.Request == null || base.CallContext.HttpContext.Request.Headers == null)
			{
				return string.Empty;
			}
			return base.CallContext.HttpContext.Request.Headers["Host"];
		}

		private ServiceResult<Stream> TraceExceptionAndReturnInternalServerError(IOutgoingWebResponseContext webContext, Exception caughtException)
		{
			this.tracer.TraceError<Exception>((long)this.GetHashCode(), "Request failed with exception: {0}", caughtException);
			webContext.StatusCode = HttpStatusCode.InternalServerError;
			base.CallContext.ProtocolLog.Set(GetPeopleICommunicateWithMetadata.GetPeopleICommunicateWithFailed, true);
			return new ServiceResult<Stream>(new MemoryStream(Array<byte>.Empty));
		}

		private const string CertificateValidationComponentId = "GetPeopleICommunicateWith";

		private const string HttpGetContentType = "application/octet-stream";

		private const string ActionName = "GetPeopleICommunicateWith";

		private const string GetPeopleICommunicateWithTotalPerformanceMarker = "GetPeopleICommunicateWithTotal";

		private const string HttpHostHeader = "Host";

		private readonly IPerformanceDataLogger perfLogger = NullPerformanceDataLogger.Instance;

		private readonly string requestId;

		private readonly string clientRequestId;

		private readonly ITracer tracer = ExTraceGlobals.PeopleICommunicateWithTracer;

		private readonly ITracer requestTracer = NullTracer.Instance;
	}
}
