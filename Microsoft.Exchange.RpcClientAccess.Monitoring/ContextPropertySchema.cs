using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ContextPropertySchema
	{
		static ContextPropertySchema()
		{
			ContextPropertySchema.AllProperties = (from property in (from field in typeof(ContextPropertySchema).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
			where typeof(ContextProperty).IsAssignableFrom(field.FieldType)
			select field).Select(delegate(FieldInfo field)
			{
				ContextProperty contextProperty = (ContextProperty)field.GetValue(null);
				if (string.IsNullOrEmpty(contextProperty.Name))
				{
					contextProperty.Name = field.Name;
				}
				if (contextProperty.AllowedAccessMode != ContextProperty.AccessMode.None)
				{
					throw new InvalidOperationException("AccessMode can only be set on derived properties specific to tasks");
				}
				if (!field.IsPublic)
				{
					return null;
				}
				return contextProperty;
			})
			where property != null
			select property).ToList<ContextProperty>().AsReadOnly();
		}

		public static readonly ICollection<ContextProperty> AllProperties;

		public static readonly ContextProperty<ICredentials> Credentials = ContextProperty<ICredentials>.Declare(null);

		public static readonly ContextProperty<bool> IgnoreInvalidServerCertificateSubject = ContextProperty<bool>.Declare(true);

		public static readonly ContextProperty<TimeSpan> Timeout = ContextProperty<TimeSpan>.Declare(Constants.DefaultRpcTimeout);

		public static readonly ContextProperty<TimeSpan> Latency = ContextProperty<TimeSpan>.Declare();

		public static readonly ContextProperty<Exception> Exception = ContextProperty<System.Exception>.Declare();

		public static readonly ContextProperty<string> ErrorDetails = ContextProperty<string>.Declare(string.Empty);

		public static readonly ContextProperty<ExDateTime> TaskFinished = ContextProperty<ExDateTime>.Declare();

		public static readonly ContextProperty<ExDateTime> TaskStarted = ContextProperty<ExDateTime>.Declare();

		public static readonly ContextProperty<string> RpcServer = ContextProperty<string>.Declare();

		public static readonly ContextProperty<AuthenticationService> RpcAuthenticationType = ContextProperty<AuthenticationService>.Declare(AuthenticationService.Negotiate);

		public static readonly ContextProperty<string> WebProxyServer = ContextProperty<string>.Declare(null);

		public static readonly ContextProperty<string> ActualBinding = ContextProperty<string>.Declare();

		public static readonly ContextProperty<string> RpcProxyServer = ContextProperty<string>.Declare();

		public static readonly ContextProperty<RpcProxyPort> RpcProxyPort = ContextProperty<Microsoft.Exchange.Rpc.RpcProxyPort>.Declare(Microsoft.Exchange.Rpc.RpcProxyPort.Default);

		public static readonly ContextProperty<HttpAuthenticationScheme> RpcProxyAuthenticationType = ContextProperty<HttpAuthenticationScheme>.Declare();

		public static readonly ContextProperty<string> OutlookSessionCookieValue = ContextProperty<string>.Declare(() => Guid.NewGuid().ToString("B").ToUpperInvariant());

		public static readonly ContextProperty<OutlookEndpointSelection> RpcEndpoint = ContextProperty<OutlookEndpointSelection>.Declare(OutlookEndpointSelection.Consolidated);

		public static readonly ContextProperty<WebHeaderCollection> AdditionalHttpHeaders = ContextProperty<WebHeaderCollection>.Declare(new WebHeaderCollection());

		public static readonly ContextProperty<string> MapiHttpServer = ContextProperty<string>.Declare();

		public static readonly ContextProperty<string> MapiHttpPersonalizedServerName = ContextProperty<string>.Declare();

		public static readonly ContextProperty<string[]> RequestedRpcProxyAuthenticationTypes = ContextProperty<string[]>.Declare();

		public static readonly ContextProperty<CertificateValidationError> CertificateValidationErrors = ContextProperty<CertificateValidationError>.Declare();

		public static readonly ContextProperty<string> RespondingWebProxyServer = ContextProperty<string>.Declare();

		public static readonly ContextProperty<string> RespondingHttpServer = ContextProperty<string>.Declare();

		public static readonly ContextProperty<string> RespondingRpcProxyServer = ContextProperty<string>.Declare();

		public static readonly ContextProperty<HttpStatusCode> ResponseStatusCode = ContextProperty<HttpStatusCode>.Declare();

		public static readonly ContextProperty<string> ResponseStatusCodeDescription = ContextProperty<string>.Declare();

		public static readonly ContextProperty<string> RequestUrl = ContextProperty<string>.Declare();

		public static readonly ContextProperty<WebHeaderCollection> ResponseHeaderCollection = ContextProperty<WebHeaderCollection>.Declare();

		public static readonly ContextProperty<WebHeaderCollection> RequestHeaderCollection = ContextProperty<WebHeaderCollection>.Declare();

		public static readonly ContextProperty<string> DirectoryServer = ContextProperty<string>.Declare();

		public static readonly ContextProperty<string> RpcServerLegacyDN = ContextProperty<string>.Declare();

		public static readonly ContextProperty<string> HomeMdbLegacyDN = ContextProperty<string>.Declare();

		public static readonly ContextProperty<string> PrimarySmtpAddress = ContextProperty<string>.Declare();

		public static readonly ContextProperty<int[]> NspiMinimalIds = ContextProperty<int[]>.Declare();

		public static readonly ContextProperty<MapiVersion> RespondingRpcClientAccessServerVersion = ContextProperty<MapiVersion>.Declare();

		public static readonly ContextProperty<bool> UseMonitoringContext = ContextProperty<bool>.Declare(true);

		public static readonly ContextProperty<string> ActivityContext = ContextProperty<string>.Declare();

		public static readonly ContextProperty<string> UserLegacyDN = ContextProperty<string>.Declare();

		public static readonly ContextProperty<string> MailboxLegacyDN = ContextProperty<string>.Declare();

		public static readonly ContextProperty<int> RetryCount = ContextProperty<int>.Declare();

		public static readonly ContextProperty<TimeSpan> RetryInterval = ContextProperty<TimeSpan>.Declare();

		public static readonly ContextProperty<TimeSpan> InitialLatency = ContextProperty<TimeSpan>.Declare();

		public static readonly ContextProperty<Exception> InitialException = ContextProperty<System.Exception>.Declare();
	}
}
