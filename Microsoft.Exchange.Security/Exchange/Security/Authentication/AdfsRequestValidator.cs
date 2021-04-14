using System;
using System.Web;
using System.Web.Util;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.IdentityModel.Protocols.WSFederation;
using Microsoft.IdentityModel.Web;

namespace Microsoft.Exchange.Security.Authentication
{
	public class AdfsRequestValidator : RequestValidator
	{
		protected override bool IsValidRequestString(HttpContext context, string value, RequestValidationSource requestValidationSource, string collectionKey, out int validationFailureIndex)
		{
			validationFailureIndex = 0;
			if (AdfsFederationAuthModule.IsAdfsAuthenticationEnabled && requestValidationSource == RequestValidationSource.Form && collectionKey.Equals("wresult", StringComparison.Ordinal) && context.Request.UrlReferrer.ToString().StartsWith(FederatedAuthentication.WSFederationAuthenticationModule.Issuer, StringComparison.OrdinalIgnoreCase))
			{
				SignInResponseMessage signInResponseMessage = WSFederationMessage.CreateFromFormPost(context.Request) as SignInResponseMessage;
				if (signInResponseMessage != null)
				{
					ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsRequestValidator::IsValidRequestString]: Allowing request posted from ADFS.");
					return true;
				}
			}
			ExTraceGlobals.AdfsAuthModuleTracer.TraceDebug(0L, "[AdfsRequestValidator::IsValidRequestString]: Request not posted from ADFS. Falling through to base class handler. IsAdfsAuthenticationEnabled={0}, requestValidationSource={1},  collectionKey={2}, UrlReferrer={3}.", new object[]
			{
				AdfsFederationAuthModule.IsAdfsAuthenticationEnabled,
				requestValidationSource,
				collectionKey,
				context.Request.UrlReferrer
			});
			return base.IsValidRequestString(context, value, requestValidationSource, collectionKey, out validationFailureIndex);
		}
	}
}
