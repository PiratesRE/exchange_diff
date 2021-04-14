using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Management.ControlPanel;
using Microsoft.Exchange.Management.DDIService;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public class DiagnosticsBehavior : IEndpointBehavior, IErrorHandler, IDispatchMessageInspector
	{
		public static string GetErrorCause(Exception exception)
		{
			string result;
			if (!DiagnosticsBehavior.KnownDDIExceptions.TryGetValue(exception.GetType(), out result))
			{
				return null;
			}
			return result;
		}

		public static void CheckSystemProbeCookie(HttpContext context)
		{
			HttpCookie httpCookie = context.Request.Cookies["xsysprobeid"];
			if (httpCookie != null)
			{
				Guid guid;
				SystemProbe.ActivityId = (Guid.TryParse(httpCookie.Value, out guid) ? guid : Guid.Empty);
			}
		}

		void IEndpointBehavior.AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
		{
		}

		void IEndpointBehavior.ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
		{
		}

		void IEndpointBehavior.ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
		{
			EcpEventLogConstants.Tuple_EcpWebServiceStarted.LogEvent(new object[]
			{
				EcpEventLogExtensions.GetUserNameToLog(),
				endpoint.Address.Uri
			});
			endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(this);
			endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
			foreach (DispatchOperation dispatchOperation in endpointDispatcher.DispatchRuntime.Operations)
			{
				if (dispatchOperation.Formatter != null)
				{
					dispatchOperation.Formatter = new DiagnosticsBehavior.SerializationPerformanceTracker(dispatchOperation.Formatter);
				}
			}
		}

		void IEndpointBehavior.Validate(ServiceEndpoint endpoint)
		{
		}

		bool IErrorHandler.HandleError(Exception error)
		{
			ErrorHandlingUtil.SendReportForCriticalException(HttpContext.Current, error);
			return false;
		}

		void IErrorHandler.ProvideFault(Exception error, MessageVersion version, ref Message fault)
		{
			string value = DiagnosticsBehavior.GetErrorCause(error) ?? error.GetType().FullName;
			EcpPerfCounters.WebServiceErrors.Increment();
			EcpEventLogConstants.Tuple_WebServiceFailed.LogPeriodicFailure(EcpEventLogExtensions.GetUserNameToLog(), HttpContext.Current.GetRequestUrlForLog(), error, EcpEventLogExtensions.GetFlightInfoForLog());
			ExTraceGlobals.EventLogTracer.TraceError<string, EcpTraceFormatter<Exception>>(0, 0L, "{0}'s webservice request failed with exception: {1}", EcpEventLogExtensions.GetUserNameToLog(), error.GetTraceFormatter());
			HttpContext.Current.Response.AddHeader("X-ECP-ERROR", value);
			DDIHelper.Trace("Webservice request failed with exception: {0}", new object[]
			{
				error.GetTraceFormatter()
			});
			if (fault != null && version == MessageVersion.None)
			{
				MessageProperties properties = fault.Properties;
				fault = Message.CreateMessage(version, string.Empty, new JsonFaultDetail(error), new DataContractJsonSerializer(typeof(JsonFaultDetail)));
				fault.Properties.CopyProperties(properties);
			}
		}

		object IDispatchMessageInspector.AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			DiagnosticsBehavior.CheckSystemProbeCookie(HttpContext.Current);
			PerfRecord.Current.WebServiceCallStarted();
			return null;
		}

		void IDispatchMessageInspector.BeforeSendReply(ref Message reply, object correlationState)
		{
			PerfRecord.Current.WebServiceCallCompleted();
			if (reply != null)
			{
				HttpResponseMessageProperty httpResponseMessageProperty;
				if (reply.Properties.ContainsKey(HttpResponseMessageProperty.Name))
				{
					httpResponseMessageProperty = (HttpResponseMessageProperty)reply.Properties[HttpResponseMessageProperty.Name];
				}
				else
				{
					httpResponseMessageProperty = new HttpResponseMessageProperty();
					reply.Properties.Add(HttpResponseMessageProperty.Name, httpResponseMessageProperty);
				}
				httpResponseMessageProperty.Headers.Set("Cache-Control", "no-cache, no-store");
			}
			RbacPrincipal.Current.RbacConfiguration.TroubleshootingContext.TraceOperationCompletedAndUpdateContext();
			ActivityContextManager.CleanupActivityContext(HttpContext.Current);
		}

		public const string NotAvailableForPartner = "notavailableforpartner";

		public const string ServiceNotExist = "servicenotexist";

		private static readonly Dictionary<Type, string> KnownDDIExceptions = new Dictionary<Type, string>
		{
			{
				typeof(SchemaNotExistException),
				"servicenotexist"
			},
			{
				typeof(WorkflowNotExistException),
				"servicenotexist"
			}
		};

		private class SerializationPerformanceTracker : IDispatchMessageFormatter
		{
			public SerializationPerformanceTracker(IDispatchMessageFormatter formatter)
			{
				this.formatter = formatter;
			}

			public void DeserializeRequest(Message message, object[] parameters)
			{
				using (EcpPerformanceData.WcfSerialization.StartRequestTimer())
				{
					IPrincipal currentPrincipal = Thread.CurrentPrincipal;
					try
					{
						Thread.CurrentPrincipal = RbacPrincipal.Current;
						this.formatter.DeserializeRequest(message, parameters);
					}
					finally
					{
						Thread.CurrentPrincipal = currentPrincipal;
					}
				}
			}

			public Message SerializeReply(MessageVersion messageVersion, object[] parameters, object result)
			{
				Message result2;
				using (EcpPerformanceData.WcfSerialization.StartRequestTimer())
				{
					result2 = this.formatter.SerializeReply(messageVersion, parameters, result);
				}
				return result2;
			}

			private IDispatchMessageFormatter formatter;
		}
	}
}
