using System;
using System.Web;
using Microsoft.Exchange.Autodiscover.ConfigurationSettings;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	internal sealed class MServeQueryResult : ResultBase
	{
		internal MServeQueryResult(UserResultMapping userResultMapping) : base(userResultMapping)
		{
		}

		internal override UserResponse CreateResponse(IBudget budget)
		{
			UserResponse result;
			if (!string.IsNullOrEmpty(this.RedirectServer))
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Creating response using redirect url {0} for {1}.", this.RedirectServer, this.userResultMapping.Mailbox);
				result = this.CreateRedirectResponse();
			}
			else
			{
				ExTraceGlobals.FrameworkTracer.TraceDebug<string>((long)this.GetHashCode(), "MServer didn't find {0}.  Creating invalid user response.", this.userResultMapping.Mailbox);
				result = base.CreateInvalidUserResponse();
			}
			return result;
		}

		internal UserResponse CreateRedirectResponse()
		{
			UriBuilder uriBuilder = new UriBuilder(HttpContext.Current.Request.Headers[WellKnownHeader.MsExchProxyUri]);
			uriBuilder.Host = this.RedirectServer;
			return ResultBase.GenerateUserResponseError(new UserConfigurationSettings
			{
				ErrorCode = UserConfigurationSettingsErrorCode.RedirectUrl,
				ErrorMessage = string.Format(Strings.RedirectUrlForUser, this.userResultMapping.Mailbox),
				RedirectTarget = ((!string.IsNullOrEmpty(base.UserResultMapping.Mailbox)) ? Common.AddUserHintToUrl(uriBuilder.Uri, base.UserResultMapping.Mailbox) : uriBuilder.ToString())
			}, this.userResultMapping.CallContext.SettingErrors);
		}

		internal string RedirectServer { get; set; }
	}
}
