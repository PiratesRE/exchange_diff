using System;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Diagnostics;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal abstract class OptionServiceCommandBase<TRequest, TResponse> : ServiceCommand<TResponse> where TResponse : OptionsResponseBase, new()
	{
		protected OptionServiceCommandBase(CallContext callContext, TRequest request) : base(callContext)
		{
			this.request = request;
			this.instrumentationName = base.GetType().Name;
			OwsLogRegistry.Register(this.instrumentationName, typeof(DefaultOptionsActionMetadata), new Type[0]);
		}

		protected override TResponse InternalExecute()
		{
			this.LogRequestForDebug();
			TResponse tresponse;
			try
			{
				tresponse = this.CreateTaskAndExecute();
			}
			catch (CmdletException ex)
			{
				RequestDetailsLogger.Current.AppendGenericError("ErrCode", ((int)ex.ErrorCode).ToString());
				RequestDetailsLogger.Current.AppendGenericError("ErrMsg", ex.Message);
				tresponse = Activator.CreateInstance<TResponse>();
				tresponse.WasSuccessful = false;
				tresponse.ErrorCode = ex.ErrorCode;
				if (ex.ErrorCode == OptionsActionError.PromptUser)
				{
					tresponse.UserPrompt = ex.Message;
					tresponse.ErrorMessage = CoreResources.GetLocalizedString((CoreResources.IDs)2715027708U);
				}
				else
				{
					tresponse.ErrorMessage = ex.Message;
				}
			}
			this.LogResponseForDebug(tresponse);
			return tresponse;
		}

		protected abstract TResponse CreateTaskAndExecute();

		protected void LogRequestForDebug()
		{
			ExTraceGlobals.OptionsTracer.TraceDebug<string, string, TRequest>((long)this.GetHashCode(), "[{0}] User = {1}, Request: {2}", this.instrumentationName, base.CallContext.AccessingPrincipal.Alias, this.request);
		}

		protected void LogResponseForDebug(TResponse response)
		{
			ExTraceGlobals.OptionsTracer.TraceDebug<string, string, TResponse>((long)this.GetHashCode(), "[{0}] User = {1}, Response: {2}", this.instrumentationName, base.CallContext.AccessingPrincipal.Alias, response);
		}

		public const string IdentityTaskPropertyName = "Identity";

		public const string MailboxTaskPropertyName = "Mailbox";

		protected readonly TRequest request;

		private readonly string instrumentationName;
	}
}
