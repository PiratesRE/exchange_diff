using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IdentityModel.Claims;
using System.IdentityModel.Policy;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.Net.XropService
{
	internal sealed class Server : IDisposable
	{
		public Server(Uri endpoint, TokenValidator tokenValidator, IAuthorizationManager authorizationManager, IServerSessionProvider sessionProvider, IServerDiagnosticsHandler diagnosticsHandler)
		{
			ExTraceGlobals.XropServiceServerTracer.TraceDebug<Uri>((long)this.GetHashCode(), "Starting service host for endpoint: {0}", endpoint);
			BindingElementCollection listenerBindingElements = Binding.GetListenerBindingElements();
			Binding binding = new Binding(listenerBindingElements);
			this.serviceHost = new ServiceHost(typeof(ServerSession), new Uri[0]);
			Server.SetThrottling(this.serviceHost);
			this.serviceHost.Description.Behaviors.Add(new Server.CustomServiceEndpointBehavior(new Server.InstanceProvider(sessionProvider), diagnosticsHandler));
			foreach (X509Certificate2 item in tokenValidator.TrustedTokenIssuerCertificates)
			{
				this.serviceHost.Credentials.IssuedTokenAuthentication.KnownCertificates.Add(item);
			}
			foreach (X509Certificate2 item2 in tokenValidator.TokenDecryptionCertificates)
			{
				this.serviceHost.Credentials.IssuedTokenAuthentication.KnownCertificates.Add(item2);
			}
			this.serviceHost.Credentials.IssuedTokenAuthentication.CertificateValidationMode = X509CertificateValidationMode.None;
			this.serviceHost.Credentials.IssuedTokenAuthentication.AllowedAudienceUris.Add(tokenValidator.TargetUri.OriginalString);
			this.serviceHost.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.None;
			this.serviceHost.Authorization.ServiceAuthorizationManager = new Server.CustomAuthorizationManager(tokenValidator, authorizationManager);
			this.serviceHost.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new Server.ExchangeUserNamePasswordValidator();
			this.serviceHost.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
			this.serviceHost.AddServiceEndpoint(typeof(IService), binding, endpoint);
			this.serviceHost.Open();
			ExTraceGlobals.XropServiceServerTracer.TraceDebug<Uri>((long)this.GetHashCode(), "Started service host for endpoint: {0}", endpoint);
		}

		public void Dispose()
		{
			this.serviceHost.Close();
			ExTraceGlobals.XropServiceServerTracer.TraceDebug((long)this.GetHashCode(), "Service host disposed");
		}

		public static void InitializeGlobalErrorHandlers(IServerDiagnosticsHandler diagnosticsHandler)
		{
			if (Server.originalTransportExceptionHandler != null || Server.httpListenerExtendedTraceListener != null)
			{
				throw new InvalidOperationException("InitializeGlobalErrorHandlers called without terminate");
			}
			if (Binding.UseWCFTransportExceptionHandler.Value)
			{
				Server.xropWCFTransportExceptionHandler = new Server.WCFTransportExceptionHandler(diagnosticsHandler);
				Server.originalTransportExceptionHandler = ExceptionHandler.TransportExceptionHandler;
				ExceptionHandler.TransportExceptionHandler = Server.xropWCFTransportExceptionHandler;
			}
			if (Binding.UseHttpListenerExtendedErrorLogging.Value)
			{
				Server.httpListenerExtendedTraceListener = new Server.HttpListenerExtendedTraceListener(diagnosticsHandler);
				SystemTraceControl.AddExtendedErrorListener(SystemTraceControl.SourceHttpListener, Server.httpListenerExtendedTraceListener);
			}
		}

		public static void TerminateGlobalErrorHandlers()
		{
			if (ExceptionHandler.TransportExceptionHandler == Server.xropWCFTransportExceptionHandler && Server.originalTransportExceptionHandler != null)
			{
				ExceptionHandler.TransportExceptionHandler = Server.originalTransportExceptionHandler;
				Server.originalTransportExceptionHandler = null;
				Server.xropWCFTransportExceptionHandler = null;
			}
			if (Server.httpListenerExtendedTraceListener != null)
			{
				SystemTraceControl.RemoveExtendedErrorListener(SystemTraceControl.SourceHttpListener, Server.httpListenerExtendedTraceListener);
				Server.httpListenerExtendedTraceListener = null;
			}
		}

		private static void SetThrottling(ServiceHost serviceHost)
		{
			ServiceThrottlingBehavior serviceThrottlingBehavior = new ServiceThrottlingBehavior();
			serviceThrottlingBehavior.MaxConcurrentCalls = 256;
			serviceThrottlingBehavior.MaxConcurrentInstances = int.MaxValue;
			serviceThrottlingBehavior.MaxConcurrentSessions = 65536;
			serviceHost.Description.Behaviors.Add(serviceThrottlingBehavior);
		}

		private const string UserNameClaimSetClass = "UserNameClaimSet";

		private ServiceHost serviceHost;

		private static Server.HttpListenerExtendedTraceListener httpListenerExtendedTraceListener;

		private static ExceptionHandler xropWCFTransportExceptionHandler;

		private static ExceptionHandler originalTransportExceptionHandler;

		private sealed class InstanceProvider : IInstanceProvider
		{
			public InstanceProvider(IServerSessionProvider sessionProvider)
			{
				this.sessionProvider = sessionProvider;
			}

			public object GetInstance(InstanceContext instanceContext)
			{
				object instance = null;
				ExWatson.SendReportOnUnhandledException(delegate()
				{
					instance = this.GetInstanceInternal(instanceContext, null);
				});
				return instance;
			}

			public object GetInstance(InstanceContext instanceContext, Message message)
			{
				object instance = null;
				ExWatson.SendReportOnUnhandledException(delegate()
				{
					instance = this.GetInstanceInternal(instanceContext, message);
				});
				return instance;
			}

			private object GetInstanceInternal(InstanceContext instanceContext, Message message)
			{
				object obj;
				if (!OperationContext.Current.ServiceSecurityContext.AuthorizationContext.Properties.TryGetValue("TokenValidationResults", out obj))
				{
					ExTraceGlobals.XropServiceServerTracer.TraceError((long)this.GetHashCode(), "Principal missing from the authorization context properties. Failing.");
					return null;
				}
				TokenValidationResults tokenValidationResults = obj as TokenValidationResults;
				if (tokenValidationResults == null)
				{
					ExTraceGlobals.XropServiceServerTracer.TraceError((long)this.GetHashCode(), "TokenValidationResults object in authorization context properties not expected type. Failing.");
					return null;
				}
				IServerSession serverSession = this.sessionProvider.Create(tokenValidationResults);
				if (serverSession == null)
				{
					ExTraceGlobals.XropServiceServerTracer.TraceError((long)this.GetHashCode(), "IServerSessionProvider did not provide a IServerSession instance. Failing.");
					return null;
				}
				ExTraceGlobals.XropServiceServerTracer.TraceDebug<int>((long)this.GetHashCode(), "Created new IServerSession: {0}", serverSession.GetHashCode());
				return new ServerSession(serverSession);
			}

			public void ReleaseInstance(InstanceContext instanceContext, object instance)
			{
				IDisposable disposable = instance as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}

			private IServerSessionProvider sessionProvider;
		}

		private sealed class ExchangeUserNamePasswordValidator : UserNamePasswordValidator
		{
			public override void Validate(string userName, string password)
			{
				if (userName == null)
				{
					throw new ArgumentNullException();
				}
				if (userName.Length == 0 || userName.IndexOf("@") < 0)
				{
					ExTraceGlobals.XropServiceServerTracer.TraceError((long)this.GetHashCode(), "Access denied: invalid SMTP email address on supporting UserName token.");
					throw new SecurityTokenException("Invalid SMTP address on UserName token");
				}
				ExTraceGlobals.XropServiceServerTracer.TraceDebug((long)this.GetHashCode(), "UserName token validated.");
			}
		}

		private sealed class CustomServiceEndpointBehavior : IServiceBehavior
		{
			public CustomServiceEndpointBehavior(Server.InstanceProvider instanceProvider, IServerDiagnosticsHandler diagnosticsHandler)
			{
				this.instanceProvider = instanceProvider;
				this.diagnosticsHandler = diagnosticsHandler;
			}

			public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
			{
			}

			public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
			{
			}

			public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
			{
				foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
				{
					ChannelDispatcher channelDispatcher = channelDispatcherBase as ChannelDispatcher;
					if (channelDispatcher != null)
					{
						foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
						{
							endpointDispatcher.DispatchRuntime.InstanceProvider = this.instanceProvider;
							endpointDispatcher.DispatchRuntime.MessageInspectors.Add(new Server.CustomMessageInspector());
						}
						channelDispatcher.ErrorHandlers.Add(new Server.CustomErrorHandler(this.diagnosticsHandler));
					}
				}
			}

			public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
			{
			}

			private Server.InstanceProvider instanceProvider;

			private IServerDiagnosticsHandler diagnosticsHandler;
		}

		private sealed class AuthorizationPolicy : IAuthorizationPolicy, IAuthorizationComponent
		{
			public AuthorizationPolicy(TokenValidationResults tokenValidationResults)
			{
				this.id = Guid.NewGuid().ToString();
				this.tokenValidationResults = tokenValidationResults;
			}

			public string Id
			{
				get
				{
					return this.id;
				}
			}

			public ClaimSet Issuer
			{
				get
				{
					return ClaimSet.System;
				}
			}

			public bool Evaluate(EvaluationContext evaluationContext, ref object state)
			{
				if (evaluationContext.Properties.ContainsKey("TokenValidationResults"))
				{
					return true;
				}
				ExTraceGlobals.XropServiceServerTracer.TraceDebug<EvaluationContextTracer>((long)this.GetHashCode(), "Evaluating: {0}", new EvaluationContextTracer(evaluationContext));
				evaluationContext.Properties["TokenValidationResults"] = this.tokenValidationResults;
				return true;
			}

			public const string TokenValidationResults = "TokenValidationResults";

			private string id;

			private TokenValidationResults tokenValidationResults;
		}

		private sealed class CustomAuthorizationManager : ServiceAuthorizationManager
		{
			public CustomAuthorizationManager(TokenValidator tokenValidator, IAuthorizationManager authorizationManager)
			{
				this.tokenValidator = tokenValidator;
				this.authorizationManager = authorizationManager;
			}

			protected override bool CheckAccessCore(OperationContext operationContext)
			{
				AuthorizationContext authorizationContext = operationContext.ServiceSecurityContext.AuthorizationContext;
				if (!authorizationContext.Properties.ContainsKey("TokenValidationResults"))
				{
					ExTraceGlobals.XropServiceServerTracer.TraceError((long)this.GetHashCode(), "Access denied: application did not authorize the caller.");
					return false;
				}
				ExTraceGlobals.XropServiceServerTracer.TraceDebug((long)this.GetHashCode(), "Access allowed.");
				return true;
			}

			protected override ReadOnlyCollection<IAuthorizationPolicy> GetAuthorizationPolicies(OperationContext operationContext)
			{
				ReadOnlyCollection<IAuthorizationPolicy> readOnlyCollection = base.GetAuthorizationPolicies(operationContext);
				TokenValidationResults tokenValidationResults = this.Validate(operationContext);
				if (tokenValidationResults != null)
				{
					readOnlyCollection = new ReadOnlyCollection<IAuthorizationPolicy>(new List<IAuthorizationPolicy>(readOnlyCollection)
					{
						new Server.AuthorizationPolicy(tokenValidationResults)
					});
				}
				return readOnlyCollection;
			}

			private TokenValidationResults Validate(OperationContext operationContext)
			{
				foreach (SupportingTokenSpecification supportingTokenSpecification in operationContext.SupportingTokens)
				{
					TokenValidationResults tokenValidationResults = this.Validate(supportingTokenSpecification);
					if (tokenValidationResults != null)
					{
						return tokenValidationResults;
					}
				}
				return null;
			}

			private TokenValidationResults Validate(SupportingTokenSpecification supportingTokenSpecification)
			{
				if (supportingTokenSpecification == null)
				{
					return null;
				}
				if (supportingTokenSpecification.SecurityToken == null)
				{
					return null;
				}
				TokenValidationResults tokenValidationResults = this.tokenValidator.ValidateToken(supportingTokenSpecification, Offer.XropLogon);
				if (tokenValidationResults.Result != TokenValidationResult.Valid)
				{
					ExTraceGlobals.XropServiceServerTracer.TraceError<TokenValidationResults>((long)this.GetHashCode(), "SAML token did not pass validation. Validation result: {0}.", tokenValidationResults);
					return null;
				}
				tokenValidationResults = this.UpdateTokenValidationResults(supportingTokenSpecification, tokenValidationResults);
				if (!this.authorizationManager.CheckAccess(tokenValidationResults))
				{
					ExTraceGlobals.XropServiceServerTracer.TraceError<string>((long)this.GetHashCode(), "Application authorization manager did not allow access to {0}.", tokenValidationResults.EmailAddress);
					return null;
				}
				ExTraceGlobals.XropServiceServerTracer.TraceDebug<string>((long)this.GetHashCode(), "Application authorization manager allowed access to {0}.", tokenValidationResults.EmailAddress);
				return tokenValidationResults;
			}

			private TokenValidationResults UpdateTokenValidationResults(SupportingTokenSpecification supportingTokenSpecification, TokenValidationResults results)
			{
				AuthorizationContext authorizationContext = AuthorizationContext.CreateDefaultAuthorizationContext(supportingTokenSpecification.SecurityTokenPolicies);
				foreach (ClaimSet claimSet in authorizationContext.ClaimSets)
				{
					if (claimSet.Issuer == ClaimSet.System && claimSet.GetType().Name == "UserNameClaimSet")
					{
						foreach (Claim claim in claimSet.FindClaims(ClaimTypes.Name, Rights.Identity))
						{
							string text = claim.Resource as string;
							if (text != null)
							{
								return new TokenValidationResults(results.ExternalId, text, results.Offer, results.SecurityToken, results.ProofToken, results.EmailAddresses);
							}
						}
					}
				}
				return results;
			}

			private TokenValidator tokenValidator;

			private IAuthorizationManager authorizationManager;
		}

		private sealed class CustomErrorHandler : IErrorHandler
		{
			internal CustomErrorHandler(IServerDiagnosticsHandler diagnosticsHandler)
			{
				this.diagnosticsHandler = diagnosticsHandler;
			}

			public bool HandleError(Exception error)
			{
				if (!Server.CustomErrorHandler.IsExceptionReported(error))
				{
					this.ReportException(error);
				}
				return true;
			}

			public void ProvideFault(Exception exception, MessageVersion version, ref Message fault)
			{
				Message message = fault;
				this.ReportException(exception);
				Server.CustomErrorHandler.SetErrorAlreadyReported(exception);
				bool flag = false;
				CommunicationException ex = exception as CommunicationException;
				if (ex != null && ex.GetType().FullName.Contains("MustUnderstandSoapException") && fault != null)
				{
					ExTraceGlobals.XropServiceServerTracer.TraceDebug((long)this.GetHashCode(), "[CustomErrorHandler::ProvideFault] Request failed due to the presence of soap:mustUnderstand on a header that XropService did not understand.");
					flag = true;
				}
				FaultException ex2 = exception as FaultException;
				if (ex2 == null)
				{
					Exception exception2 = exception;
					if (!flag)
					{
						this.diagnosticsHandler.AnalyseException(ref exception2);
					}
					ex2 = this.HandleInternalServerError(exception2);
				}
				if (fault == null)
				{
					MessageFault fault2 = ex2.CreateMessageFault();
					fault = Message.CreateMessage(version, fault2, "*");
				}
			}

			private static void SetErrorAlreadyReported(Exception exception)
			{
				if (exception.Data != null)
				{
					exception.Data["ExceptionAlreadyReported"] = null;
				}
			}

			private static bool IsExceptionReported(Exception exception)
			{
				while (exception != null)
				{
					if (exception.Data != null && exception.Data.Contains("ExceptionAlreadyReported"))
					{
						return true;
					}
					exception = exception.InnerException;
				}
				return false;
			}

			private FaultException HandleInternalServerError(Exception exception)
			{
				Exception exception2 = (exception is CommunicationException) ? exception : new Server.CustomErrorHandler.InternalServerErrorException(exception);
				Server.CustomErrorHandler.XropServiceMessageFaultDetail messageFault = new Server.CustomErrorHandler.XropServiceMessageFaultDetail(exception2, Server.CustomErrorHandler.FaultParty.Receiver);
				return this.CreateFaultException(messageFault);
			}

			private void ReportException(Exception exception)
			{
				if (OperationContext.Current != null && OperationContext.Current.RequestContext != null && OperationContext.Current.RequestContext.RequestMessage != null)
				{
					ExTraceGlobals.XropServiceServerTracer.TraceError<Message, string, string>((long)this.GetHashCode(), "Request: {0}.  Exception Class: {1}, Exception Message: {2}", OperationContext.Current.RequestContext.RequestMessage, exception.GetType().FullName, exception.Message);
				}
				else
				{
					ExTraceGlobals.XropServiceServerTracer.TraceError<string, string>((long)this.GetHashCode(), "Exception Class: {0}, Exception Message: {1}", exception.GetType().FullName, exception.Message);
				}
				if (Binding.IncludeErrorDetailsInTrace.Value)
				{
					for (Exception innerException = exception.InnerException; innerException != null; innerException = innerException.InnerException)
					{
						ExTraceGlobals.XropServiceServerTracer.TraceError<string, string>((long)this.GetHashCode(), "Exception Class: {0}, Exception Message: {1}", innerException.GetType().FullName, innerException.Message);
					}
				}
				this.diagnosticsHandler.LogException(exception);
			}

			private FaultException CreateFaultException(MessageFault messageFault)
			{
				return new FaultException(messageFault)
				{
					Source = "XropService"
				};
			}

			private const string GenericServiceName = "XropService";

			private const string ExceptionAlreadyReported = "ExceptionAlreadyReported";

			private const string XropInternalServerError = "InternalServiceFault";

			private IServerDiagnosticsHandler diagnosticsHandler;

			internal enum FaultParty
			{
				Sender,
				Receiver
			}

			internal class XropServiceMessageFaultDetail : MessageFault
			{
				internal XropServiceMessageFaultDetail(Exception exception, Server.CustomErrorHandler.FaultParty faultParty)
				{
					this.exception = exception;
					this.faultCode = ((faultParty == Server.CustomErrorHandler.FaultParty.Sender) ? FaultCode.CreateSenderFaultCode("InternalServiceFault", "http://schemas.microsoft.com/exchange/2010/xrop") : FaultCode.CreateReceiverFaultCode("InternalServiceFault", "http://schemas.microsoft.com/exchange/2010/xrop"));
					this.faultReason = new FaultReason(exception.Message);
				}

				public override FaultCode Code
				{
					get
					{
						return this.faultCode;
					}
				}

				public override bool HasDetail
				{
					get
					{
						return Binding.IncludeDetailsInServiceFaults.Value;
					}
				}

				public override FaultReason Reason
				{
					get
					{
						return this.faultReason;
					}
				}

				protected override void OnWriteDetailContents(XmlDictionaryWriter writer)
				{
					if (!this.HasDetail)
					{
						return;
					}
					string faultDetails = this.GetFaultDetails();
					if (!string.IsNullOrEmpty(faultDetails))
					{
						writer.WriteRaw(faultDetails);
					}
				}

				private string GetFaultDetails()
				{
					SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
					XmlElement xmlElement = safeXmlDocument.CreateElement("DiagnosticInformation", "http://schemas.microsoft.com/exchange/2010/xrop/Error");
					for (Exception innerException = this.exception; innerException != null; innerException = innerException.InnerException)
					{
						XmlElement xmlElement2 = safeXmlDocument.CreateElement("ExceptionMessage", "http://schemas.microsoft.com/exchange/2010/xrop/Error");
						xmlElement.AppendChild(xmlElement2);
						XmlText newChild = safeXmlDocument.CreateTextNode(innerException.Message);
						xmlElement2.AppendChild(newChild);
						if (Binding.IncludeStackInServiceFaults.Value)
						{
							XmlElement xmlElement3 = safeXmlDocument.CreateElement("StackTrace", "http://schemas.microsoft.com/exchange/2010/xrop/Error");
							xmlElement.AppendChild(xmlElement3);
							if (!string.IsNullOrEmpty(innerException.StackTrace))
							{
								XmlText newChild2 = safeXmlDocument.CreateTextNode(innerException.StackTrace);
								xmlElement3.AppendChild(newChild2);
							}
						}
					}
					return xmlElement.OuterXml;
				}

				private FaultCode faultCode;

				private FaultReason faultReason;

				private Exception exception;
			}

			[Serializable]
			internal class InternalServerErrorException : Exception
			{
				public InternalServerErrorException(Exception innerException) : base("InternalServiceFault", innerException)
				{
				}

				public InternalServerErrorException(string message, Exception innerException) : base(message, innerException)
				{
				}
			}
		}

		private class CustomMessageInspector : IDispatchMessageInspector
		{
			internal CustomMessageInspector()
			{
				this.xropHostName = ComputerInformation.DnsPhysicalHostName;
			}

			public void BeforeSendReply(ref Message reply, object correlationState)
			{
				object obj = null;
				HttpResponseMessageProperty httpResponseMessageProperty = null;
				if (reply.Properties.TryGetValue(HttpResponseMessageProperty.Name, out obj))
				{
					httpResponseMessageProperty = (obj as HttpResponseMessageProperty);
				}
				if (httpResponseMessageProperty == null)
				{
					httpResponseMessageProperty = new HttpResponseMessageProperty();
					reply.Properties.Add(HttpResponseMessageProperty.Name, httpResponseMessageProperty);
				}
				httpResponseMessageProperty.Headers.Add("X-DiagInfo", this.xropHostName);
				if (reply.IsFault && Binding.Use200ForSoapFaults.Value)
				{
					httpResponseMessageProperty.StatusCode = HttpStatusCode.OK;
				}
				if (reply.IsFault)
				{
					ExTraceGlobals.XropServiceServerTracer.TraceError<Message>((long)this.GetHashCode(), "Sending Fault: {0}.", reply);
					return;
				}
				ExTraceGlobals.XropServiceServerTracer.TraceDebug<Message>((long)this.GetHashCode(), "Sending Reply: {0}.", reply);
			}

			public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
			{
				ExTraceGlobals.XropServiceServerTracer.TraceDebug<Message>((long)this.GetHashCode(), "Receiving Request: {0}.", request);
				return null;
			}

			private string xropHostName;
		}

		private sealed class HttpListenerExtendedTraceListener : TraceListener
		{
			public HttpListenerExtendedTraceListener(IServerDiagnosticsHandler diagnosticsHandler) : base("XTCHttpListenerExtendedTraceListener")
			{
				this.diagnosticsHandler = diagnosticsHandler;
			}

			public override void Write(string message)
			{
			}

			public override void WriteLine(string message)
			{
			}

			public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
			{
				if (eventType == TraceEventType.Critical || eventType == TraceEventType.Error)
				{
					this.diagnosticsHandler.LogMessage(message);
				}
			}

			private IServerDiagnosticsHandler diagnosticsHandler;
		}

		private sealed class WCFTransportExceptionHandler : ExceptionHandler
		{
			public WCFTransportExceptionHandler(IServerDiagnosticsHandler diagnosticsHandler)
			{
				this.diagnosticsHandler = diagnosticsHandler;
			}

			public override bool HandleException(Exception exception)
			{
				this.diagnosticsHandler.LogException(exception);
				ExceptionHandler originalTransportExceptionHandler = Server.originalTransportExceptionHandler;
				return originalTransportExceptionHandler == null || originalTransportExceptionHandler.HandleException(exception);
			}

			private IServerDiagnosticsHandler diagnosticsHandler;
		}
	}
}
