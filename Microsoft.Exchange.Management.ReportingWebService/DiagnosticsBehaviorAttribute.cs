using System;
using System.Collections.ObjectModel;
using System.Data.Services;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Web;

namespace Microsoft.Exchange.Management.ReportingWebService
{
	[AttributeUsage(AttributeTargets.Class)]
	public class DiagnosticsBehaviorAttribute : Attribute, IServiceBehavior, IErrorHandler
	{
		public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
		{
		}

		public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
			foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
			{
				ChannelDispatcher channelDispatcher = (ChannelDispatcher)channelDispatcherBase;
				foreach (EndpointDispatcher endpointDispatcher in channelDispatcher.Endpoints)
				{
					endpointDispatcher.ChannelDispatcher.ErrorHandlers.Add(this);
				}
			}
		}

		public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
		{
		}

		bool IErrorHandler.HandleError(Exception error)
		{
			return false;
		}

		void IErrorHandler.ProvideFault(Exception error, MessageVersion version, ref Message fault)
		{
			ServiceDiagnostics.ReportUnhandledException(error, HttpContext.Current);
			string text = HttpContext.Current.Request.QueryString["$format"];
			string text2 = HttpContext.Current.Request.Headers["Accept"];
			XmlObjectSerializer serializer;
			WebBodyFormatMessageProperty property;
			if ((text != null && text.Equals("json", StringComparison.InvariantCultureIgnoreCase)) || (text2 != null && text2.Equals("application/json", StringComparison.InvariantCultureIgnoreCase)))
			{
				serializer = new DataContractJsonSerializer(typeof(ServiceFault));
				property = new WebBodyFormatMessageProperty(WebContentFormat.Json);
			}
			else
			{
				serializer = new DataContractSerializer(typeof(ServiceFault));
				property = new WebBodyFormatMessageProperty(WebContentFormat.Xml);
			}
			fault = Message.CreateMessage(version, string.Empty, new ServiceFault(string.Empty, error), serializer);
			fault.Properties.Add("WebBodyFormatMessageProperty", property);
			HttpResponseMessageProperty httpResponseMessageProperty = new HttpResponseMessageProperty();
			DataServiceException ex = error as DataServiceException;
			if (ex != null)
			{
				httpResponseMessageProperty.StatusCode = (HttpStatusCode)ex.StatusCode;
			}
			else
			{
				httpResponseMessageProperty.StatusCode = HttpStatusCode.InternalServerError;
			}
			fault.Properties.Add(HttpResponseMessageProperty.Name, httpResponseMessageProperty);
		}
	}
}
