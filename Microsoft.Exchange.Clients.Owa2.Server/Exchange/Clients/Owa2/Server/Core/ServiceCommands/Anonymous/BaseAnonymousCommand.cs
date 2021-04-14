using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.ServiceCommands.Anonymous
{
	internal abstract class BaseAnonymousCommand<TRequest, TResponse> where TRequest : BaseJsonRequest where TResponse : BaseJsonResponse
	{
		protected BaseAnonymousCommand(TRequest request)
		{
			this.TraceDebug("BaseAnonymousCommand():Invoking request with PublishingUrl={0}, Request={1}", new object[]
			{
				this.Context.PublishingUrl,
				this.Request
			});
			this.Request = request;
			this.ValidateHeader();
			this.SetCallContext();
			ExchangeVersion.Current = ExchangeVersion.Latest;
			TRequest request2 = this.Request;
			EWSSettings.RequestTimeZone = request2.Header.TimeZoneContext.TimeZoneDefinition.ExTimeZone;
		}

		protected AnonymousUserContext Context
		{
			get
			{
				return AnonymousUserContext.Current;
			}
		}

		private protected TRequest Request { protected get; private set; }

		internal TResponse Execute()
		{
			this.ValidateRequestBody();
			TResponse result;
			try
			{
				this.TraceDebug("Creating the published folder instance", new object[0]);
				using (PublishedCalendar publishedCalendar = (PublishedCalendar)PublishedFolder.Create(this.Context.PublishingUrl))
				{
					publishedCalendar.TimeZone = EWSSettings.RequestTimeZone;
					this.UpdateRequestBody(publishedCalendar);
					this.TraceDebug("Invoking the command", new object[0]);
					result = this.InternalExecute(publishedCalendar);
				}
			}
			catch (OverBudgetException exception)
			{
				result = this.CreateErrorResponse(exception, ResponseCodeType.ErrorServerBusy);
			}
			catch (PublishedFolderAccessDeniedException exception2)
			{
				result = this.CreateErrorResponse(exception2, ResponseCodeType.ErrorAccessDenied);
			}
			catch (FolderNotPublishedException exception3)
			{
				result = this.CreateErrorResponse(exception3, ResponseCodeType.ErrorAccessDenied);
			}
			return result;
		}

		protected string GetExceptionMessage(Exception exception)
		{
			string message = exception.Message;
			while (string.IsNullOrEmpty(message) && exception != null)
			{
				message = exception.Message;
				exception = exception.InnerException;
			}
			return message;
		}

		protected abstract TResponse CreateErrorResponse(Exception exception, ResponseCodeType codeType);

		protected abstract void ValidateRequestBody();

		protected abstract void UpdateRequestBody(PublishedCalendar publishedFolder);

		protected abstract TResponse InternalExecute(PublishedCalendar publishedFolder);

		protected void TraceDebug(string message, params object[] args)
		{
			ExTraceGlobals.AnonymousServiceCommandTracer.TraceDebug(0L, message, args);
		}

		protected void TraceError(string message, params object[] args)
		{
			ExTraceGlobals.AnonymousServiceCommandTracer.TraceError(0L, message, args);
		}

		private void ValidateHeader()
		{
			TRequest request = this.Request;
			if (request.Header != null)
			{
				TRequest request2 = this.Request;
				if (request2.Header.TimeZoneContext != null)
				{
					TRequest request3 = this.Request;
					if (request3.Header.TimeZoneContext.TimeZoneDefinition != null)
					{
						return;
					}
				}
			}
			this.TraceError("Missing timezone header", new object[0]);
			throw new FaultException(new FaultReason("Missing timezone header"));
		}

		private void SetCallContext()
		{
			CallContext callContext = (CallContext)FormatterServices.GetUninitializedObject(typeof(CallContext));
			callContext.EncodeStringProperties = Global.EncodeStringProperties;
			HttpContext.Current.Items["CallContext"] = callContext;
		}
	}
}
