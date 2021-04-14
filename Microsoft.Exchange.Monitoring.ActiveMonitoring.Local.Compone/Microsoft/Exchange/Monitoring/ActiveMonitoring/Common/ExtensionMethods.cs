using System;
using System.Linq.Expressions;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public static class ExtensionMethods
	{
		public static void ApplyCafeAuthentication(this ExchangeServiceBase service, string username, string password)
		{
			service.HttpHeaders["X-IsFromCafe"] = "1";
			CommonAccessToken commonAccessToken;
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.WindowsLiveID.Enabled)
			{
				commonAccessToken = CommonAccessTokenHelper.CreateLiveIdBasic(username);
			}
			else
			{
				commonAccessToken = CommonAccessTokenHelper.CreateWindows(username, password);
			}
			service.HttpHeaders["X-CommonAccessToken"] = commonAccessToken.Serialize();
		}

		public static void SetComponentId(this ExchangeServiceBase service, string componentId)
		{
			service.HttpHeaders[CertificateValidationManager.ComponentIdHeaderName] = componentId;
		}

		public static TReturn EnsureNotNull<TInput, TReturn>(this TInput input, Expression<Func<TInput, TReturn>> accessor) where TReturn : class
		{
			TReturn treturn = accessor.Compile()(input);
			if (treturn == null)
			{
				throw new InvalidOperationException(string.Format("{0} returned a null for {1}, which is unexpected. Exact timestamp: {2}", accessor, typeof(TInput).Name, ExDateTime.UtcNow));
			}
			return treturn;
		}
	}
}
