using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Online.BOX.Shell;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SetUserTheme : ServiceCommand<SetUserThemeResponse>
	{
		public SetUserTheme(CallContext callContext, SetUserThemeRequest request) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(request, "SetUserThemeRequest", "SetUserTheme::SetUserTheme");
			this.request = request;
			ITracer tracer;
			if (!base.IsRequestTracingEnabled)
			{
				ITracer instance = NullTracer.Instance;
				tracer = instance;
			}
			else
			{
				tracer = new InMemoryTracer(ExTraceGlobals.ThemesCallTracer.Category, ExTraceGlobals.ThemesCallTracer.TraceTag);
			}
			this.requestTracer = tracer;
			this.tracer = ExTraceGlobals.ThemesCallTracer.Compose(this.requestTracer);
		}

		protected override void LogTracesForCurrentRequest()
		{
			WcfServiceCommandBase.TraceLoggerFactory.Create(base.CallContext.HttpContext.Response.Headers).LogTraces(this.requestTracer);
		}

		protected override SetUserThemeResponse InternalExecute()
		{
			UserContext userContext = UserContextManager.GetUserContext(CallContext.Current.HttpContext, CallContext.Current.EffectiveCaller, true);
			ConfigurationContext configurationContext = new ConfigurationContext(userContext);
			SetUserThemeResponse setUserThemeResponse = new SetUserThemeResponse
			{
				OwaSuccess = false,
				O365Success = false
			};
			if (!configurationContext.IsFeatureEnabled(Feature.Themes) || string.IsNullOrEmpty(this.request.ThemeId))
			{
				return setUserThemeResponse;
			}
			uint idFromStorageId = ThemeManagerFactory.GetInstance(userContext.CurrentOwaVersion).GetIdFromStorageId(this.request.ThemeId);
			this.tracer.TraceDebug<uint>(1L, "SetUserTheme.InternalExecute::id='{0}'", idFromStorageId);
			if (idFromStorageId == 4294967295U)
			{
				throw new OwaInvalidOperationException("The theme doesn't exist any more on the server");
			}
			string userPrincipalName = userContext.LogonIdentity.GetOWAMiniRecipient().UserPrincipalName;
			setUserThemeResponse.O365Success = this.UpdateO365Theme(this.request.ThemeId, userPrincipalName, userContext);
			if (this.request.ThemeId == userContext.DefaultTheme.StorageId)
			{
				this.request.ThemeId = string.Empty;
			}
			UserConfigurationPropertyDefinition propertyDefinition = UserOptionPropertySchema.Instance.GetPropertyDefinition(UserConfigurationPropertyId.ThemeStorageId);
			new UserOptionsType
			{
				ThemeStorageId = this.request.ThemeId
			}.Commit(base.CallContext, new UserConfigurationPropertyDefinition[]
			{
				propertyDefinition
			});
			setUserThemeResponse.OwaSuccess = true;
			userContext.ClearCachedTheme();
			return setUserThemeResponse;
		}

		private bool UpdateO365Theme(string themeId, string userPrincipalName, UserContext userContext)
		{
			bool flag = userContext.FeaturesManager.ClientServerSettings.O365Header.Enabled || userContext.FeaturesManager.ClientServerSettings.O365G2Header.Enabled;
			this.tracer.TraceDebug<bool, bool>(0L, "UpdateO365Theme::isFeatureSupported='{0}', this.skipO365Call='{1}'", flag, this.request.SkipO365Call);
			if (!flag || this.request.SkipO365Call)
			{
				return false;
			}
			string text = null;
			string text2 = null;
			string text3 = string.Empty;
			bool result;
			try
			{
				using (ShellServiceClient shellServiceClient = new ShellServiceClient("MsOnlineShellService_EndPointConfiguration"))
				{
					string text4 = ConfigurationManager.AppSettings["MsOnlineShellService_CertThumbprint"];
					this.tracer.TraceDebug<string, CommunicationState>(1L, "UpdateO365Theme::certificateThumbprint='{0}',client.State'={1}'", text4, shellServiceClient.State);
					shellServiceClient.ClientCredentials.ClientCertificate.Certificate = TlsCertificateInfo.FindCertByThumbprint(text4);
					EndpointAddress address = shellServiceClient.Endpoint.Address;
					Uri uri = new Uri(address.Uri.AbsoluteUri);
					shellServiceClient.Endpoint.Address = new EndpointAddress(uri, address.Identity, new AddressHeader[0]);
					string text5 = HttpContext.Current.Request.Headers["RPSOrgIdPUID"];
					text = (string.IsNullOrEmpty(text5) ? HttpContext.Current.Request.Headers["RPSPUID"] : text5);
					text2 = shellServiceClient.Endpoint.Address.Uri.AbsoluteUri;
					text3 = Guid.NewGuid().ToString();
					this.tracer.TraceDebug(2L, "UpdateO365Theme::orgIdPuid='{0}', userPuid='{1}', userPrincipalName='{2}',serviceUrl='{3}'", new object[]
					{
						text5,
						text,
						userPrincipalName,
						text2
					});
					SetUserThemeRequest setUserThemeRequest = new SetUserThemeRequest
					{
						ThemeId = themeId,
						TrackingGuid = text3,
						UserPrincipalName = userPrincipalName,
						UserPuid = text,
						WorkloadId = WorkloadAuthenticationId.Exchange
					};
					this.tracer.TraceDebug(3L, "UpdateO365Theme::setUserThemeRequest.ThemeId='{0}', .TrackingGuid='{1}', .UserPrincipalName='{2}', .UserPuid='{3}', .WorkloadId='{4}'", new object[]
					{
						setUserThemeRequest.ThemeId,
						setUserThemeRequest.TrackingGuid,
						setUserThemeRequest.UserPrincipalName,
						setUserThemeRequest.UserPuid,
						setUserThemeRequest.WorkloadId
					});
					shellServiceClient.SetUserTheme(setUserThemeRequest);
					this.tracer.TraceDebug<CommunicationState>(4L, "UpdateO365Theme::setUserThemeRequest.State='{0}'", shellServiceClient.State);
					result = true;
				}
			}
			catch (Exception ex)
			{
				this.tracer.TraceError(5L, "UpdateO365Theme::Exception: themeId='{0}', trackingGuid='{1}', userPrincipalName='{2}', userPuid='{3}', serviceUrl='{4}', exception='{5}'", new object[]
				{
					themeId,
					text3,
					userPrincipalName,
					text,
					text2,
					ex
				});
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_O365SetUserThemeError, userPrincipalName + text2 + themeId, new object[]
				{
					text,
					userPrincipalName,
					text3,
					text2,
					themeId,
					ex
				});
				result = false;
			}
			return result;
		}

		private ITracer tracer = ExTraceGlobals.ThemesCallTracer;

		private ITracer requestTracer = NullTracer.Instance;

		private readonly SetUserThemeRequest request;
	}
}
