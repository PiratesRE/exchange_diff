using System;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using Microsoft.Exchange.Autodiscover;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.AutoDiscoverV2
{
	[ExcludeFromCodeCoverage]
	internal class AutoDiscoverV2HandlerBase
	{
		public AutoDiscoverV2HandlerBase()
		{
		}

		internal AutoDiscoverV2HandlerBase(RequestDetailsLogger logger)
		{
			this.Logger = logger;
		}

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		protected RequestDetailsLogger Logger { get; set; }

		public void ProcessRequest(HttpContext context)
		{
			this.Logger = RequestDetailsLoggerBase<RequestDetailsLogger>.Current;
			this.ProcessRequest(new HttpContextWrapper(context));
		}

		public virtual string GetEmailAddressFromUrl(HttpContextBase context)
		{
			return null;
		}

		public virtual bool Validate(HttpContextBase context)
		{
			return true;
		}

		internal void ProcessRequest(HttpContextBase context)
		{
			try
			{
				Common.SendWatsonReportOnUnhandledException(delegate
				{
					this.Logger.Set(ActivityStandardMetadata.Action, "AutoDiscoverV2");
					FlightSettingRepository flightSettings = new FlightSettingRepository();
					try
					{
						if (this.Validate(context))
						{
							AutoDiscoverV2 autoDiscoverV = new AutoDiscoverV2(this.Logger, flightSettings, new TenantRepository(this.Logger));
							string emailAddressFromUrl = this.GetEmailAddressFromUrl(context);
							AutoDiscoverV2Request request = autoDiscoverV.CreateRequestFromContext(context, emailAddressFromUrl);
							AutoDiscoverV2Response autoDiscoverV2Response = autoDiscoverV.ProcessRequest(request, flightSettings);
							if (autoDiscoverV2Response != null)
							{
								this.EmitResponse(context, autoDiscoverV2Response);
							}
							else
							{
								this.LogAndEmitErrorResponse(context, AutoDiscoverResponseException.NotFound());
							}
						}
					}
					catch (AutoDiscoverResponseException exception)
					{
						this.LogAndEmitErrorResponse(context, exception);
					}
				});
			}
			catch (Exception innerException)
			{
				this.EmitErrorResponse(context, AutoDiscoverResponseException.InternalServerError("InternalServerError", innerException));
			}
		}

		internal void EmitResponse(HttpContextBase context, AutoDiscoverV2Response response)
		{
			context.Response.ContentType = "application/json";
			if (response.RedirectUrl != null)
			{
				context.Response.Redirect(response.RedirectUrl, false);
				this.Logger.AppendGenericInfo("EmitResponseRedirect", response.RedirectUrl);
				return;
			}
			if (response.Url != null)
			{
				context.Response.StatusCode = 200;
				JsonSuccessResponse jsonSuccessResponse = new JsonSuccessResponse
				{
					Protocol = response.ProtocolName,
					Url = response.Url
				};
				string text = jsonSuccessResponse.SerializeToJson(jsonSuccessResponse);
				this.Logger.AppendGenericInfo("EmitResponseJsonString", text);
				context.Response.Write(text);
			}
		}

		internal void EmitErrorResponse(HttpContextBase context, AutoDiscoverResponseException exception)
		{
			context.Response.ContentType = "application/json";
			context.Response.Headers["jsonerror"] = "true";
			context.Response.StatusCode = exception.HttpStatusCodeValue;
			JsonErrorResponse jsonErrorResponse = new JsonErrorResponse
			{
				ErrorMessage = exception.Message,
				ErrorCode = exception.ErrorCode
			};
			string text = jsonErrorResponse.SerializeToJson(jsonErrorResponse);
			this.Logger.AppendGenericInfo("EmitErrorResponseJsonString", text);
			context.Response.Write(text);
		}

		private void LogAndEmitErrorResponse(HttpContextBase context, AutoDiscoverResponseException exception)
		{
			this.Logger.AppendGenericError("AutoDiscoverResponseException", exception.ToString());
			this.EmitErrorResponse(context, exception);
		}
	}
}
