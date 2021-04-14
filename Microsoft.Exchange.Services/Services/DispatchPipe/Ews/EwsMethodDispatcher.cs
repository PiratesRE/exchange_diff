using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.DispatchPipe.Base;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.DispatchPipe.Ews
{
	internal class EwsMethodDispatcher
	{
		static EwsMethodDispatcher()
		{
			EwsMethodDispatcher.responseHeaderNamespaces.Add("h", "http://schemas.microsoft.com/exchange/services/2006/types");
			EwsMethodDispatcher.responseHeaderNamespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");
			EwsMethodDispatcher.responseHeaderNamespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
		}

		internal EwsMethodDispatcher(EWSService service, ServiceMethodInfo serviceMethodInfo)
		{
			this.service = service;
			this.serviceMethodInfo = serviceMethodInfo;
		}

		internal IAsyncResult InvokeBeginMethod(HttpContext httpContext, AsyncCallback asyncCallback, object state)
		{
			IAsyncResult result;
			try
			{
				Message message = (Message)httpContext.Items["EwsHttpContextMessage"];
				if (message == null)
				{
					result = EwsMethodDispatcher.SetHttpResponseStatusAndCreateCompletedAsyncResult(httpContext.Response, HttpStatusCode.BadRequest, asyncCallback, state);
				}
				else
				{
					this.operationContext = EwsOperationContext.Create(message);
					EwsOperationContextBase.SetCurrent(this.operationContext);
					this.wcfMessageVersion = message.Version;
					this.messageEncoder = EwsServiceHttpHandlerFactory.GetMessageEncoder(this.wcfMessageVersion);
					EwsMethodDispatcher.ThrowOnDelayedException(message);
					result = this.InternalInvokeBeginMethod(httpContext, asyncCallback, state);
				}
			}
			catch (Exception ex)
			{
				this.HandleException(ex, httpContext.Response);
				result = new AsyncResult(asyncCallback, state, true)
				{
					Exception = ex
				};
			}
			finally
			{
				EwsOperationContextBase.SetCurrent(null);
			}
			return result;
		}

		internal void InvokeEndMethod(IAsyncResult result, HttpResponse httpResponse)
		{
			AsyncResult asyncResult = result as AsyncResult;
			try
			{
				if (asyncResult == null || (!asyncResult.CompletedSynchronously && asyncResult.Exception == null))
				{
					EwsOperationContextBase.SetCurrent(this.operationContext);
					CallContext.SetCurrent(this.callcontext);
					this.InternalInvokeEndMethod(result, httpResponse);
				}
			}
			catch (Exception e)
			{
				this.HandleException(e, httpResponse);
			}
			finally
			{
				EwsOperationContextBase.SetCurrent(null);
				CallContext.SetCurrent(null);
			}
		}

		private static void ThrowOnDelayedException(Message message)
		{
			object obj;
			if (message.Properties.TryGetValue("DelayedException", out obj))
			{
				Exception ex = obj as Exception;
				throw ex;
			}
		}

		private IAsyncResult InternalInvokeBeginMethod(HttpContext httpContext, AsyncCallback asyncCallback, object state)
		{
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug(0L, "EwsMethodDispatcher.InvokeBeginMethod");
			try
			{
				Message requestMessage = this.operationContext.RequestMessage;
				if (!EwsMethodDispatcher.EwsAuthorizationManager.CheckAccessCore(EwsOperationContextBase.Current))
				{
					this.BailoutForAuthError();
				}
				EwsMethodDispatcher.messageInspectorManager.AfterReceiveRequest(ref requestMessage, null, null);
				this.callcontext = (CallContext)httpContext.Items["CallContext"];
				using (XmlReader body = this.GetBody(requestMessage))
				{
					XmlRootAttribute xmlRootAttribute = new XmlRootAttribute(this.serviceMethodInfo.Name);
					xmlRootAttribute.Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages";
					SafeXmlSerializer orCreateXmlSerializer = this.serviceMethodInfo.GetOrCreateXmlSerializer(this.serviceMethodInfo.RequestBodyType, xmlRootAttribute);
					BaseRequest obj = (BaseRequest)orCreateXmlSerializer.Deserialize(body);
					return (IAsyncResult)this.serviceMethodInfo.BeginMethod.Invoke(obj, new object[]
					{
						asyncCallback,
						state
					});
				}
			}
			catch (TargetInvocationException ex)
			{
				ExceptionDispatchInfo exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex.InnerException ?? ex);
				exceptionDispatchInfo.Throw();
			}
			return null;
		}

		private void InternalInvokeEndMethod(IAsyncResult result, HttpResponse httpResponse)
		{
			ExTraceGlobals.CommonAlgorithmTracer.TraceDebug((long)this.GetHashCode(), "EwsMethodDispatcher.InvokeEndMethod");
			try
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "ClientConnected", httpResponse.IsClientConnected.ToString());
				BaseSoapResponse baseSoapResponse = (BaseSoapResponse)this.serviceMethodInfo.EndMethod.Invoke(this.service, new object[]
				{
					result
				});
				object value = this.serviceMethodInfo.WrappedResponseBodyField.GetValue(baseSoapResponse);
				this.PrepareAndWriteResponseToWire(value, baseSoapResponse.ServerVersionInfo, httpResponse);
			}
			catch (TargetInvocationException ex)
			{
				ExceptionDispatchInfo exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex.InnerException ?? ex);
				exceptionDispatchInfo.Throw();
			}
		}

		private void HandleException(Exception e, HttpResponse httpResponse)
		{
			ServiceDiagnostics.SendWatsonReportOnUnhandledException(delegate
			{
				Message message = null;
				try
				{
					if (e is BailOutException)
					{
						EWSSettings.ResponseRenderer.Render(null, null, httpResponse);
					}
					else
					{
						Exception ex = e;
						if (e is FaultException || e.InnerException is FaultException)
						{
							ex = ((e is FaultException) ? ((FaultException)e) : ((FaultException)e.InnerException));
							message = Message.CreateMessage(this.wcfMessageVersion, ((FaultException)ex).CreateMessageFault(), null);
						}
						EwsServiceHttpHandlerFactory.ExceptionHandler.ProvideFault(ex, this.wcfMessageVersion, ref message);
						httpResponse.ContentType = this.messageEncoder.ContentType;
						this.WriteMessageToStream(message, httpResponse);
						EwsServiceHttpHandlerFactory.ExceptionHandler.HandleError(ex);
					}
				}
				finally
				{
					if (message != null)
					{
						message.Close();
						message = null;
					}
				}
			});
		}

		private void PrepareAndWriteResponseToWire(object responseBody, ServerVersionInfo serverVersionInfo, HttpResponse httpResponse)
		{
			Message message = null;
			try
			{
				httpResponse.BufferOutput = false;
				httpResponse.Buffer = false;
				message = EwsMethodDispatcher.messageInspectorManager.BeforeSendReply(httpResponse, this.wcfMessageVersion, this.serviceMethodInfo.Name);
				if (message != null)
				{
					httpResponse.AddHeader("x-EwsHandler", this.serviceMethodInfo.Name);
					httpResponse.ContentType = ((EWSSettings.ProxyResponse != null) ? EWSSettings.ProxyResponse.ContentType : "text/xml; charset=utf-8");
					this.WriteMessageToStream(message, httpResponse);
				}
				else if (responseBody == null)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(RequestDetailsLogger.Current, "EndInvokeError", "NullResponseBodyInEndInvoke");
				}
				else
				{
					httpResponse.AddHeader("x-EwsHandler", this.serviceMethodInfo.Name);
					EWSSettings.WritingToWire = true;
					httpResponse.ContentType = "text/xml; charset=utf-8";
					this.WriteResponseToStream(httpResponse.OutputStream, serverVersionInfo, responseBody);
				}
			}
			finally
			{
				if (message != null)
				{
					message.Close();
					message = null;
				}
			}
		}

		private void WriteMessageToStream(Message message, HttpResponse httpResponse)
		{
			bool flag = false;
			EWSSettings.WritingToWire = false;
			SoapWcfResponseRenderer soapWcfResponseRenderer = EWSSettings.ResponseRenderer as SoapWcfResponseRenderer;
			HttpStatusCode? httpStatusCode;
			if (soapWcfResponseRenderer != null && soapWcfResponseRenderer.StatusCode != null)
			{
				httpResponse.StatusCode = (int)soapWcfResponseRenderer.StatusCode.Value;
			}
			else if (EwsMethodDispatcher.TryGetHttpStatusFromHttpResponseMessagePropertyFromOperationContext(out httpStatusCode, out flag))
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<HttpStatusCode, bool>((long)this.GetHashCode(), "return status modified by HttpResponseMessageProperty - status = {0}, suppress = {1}", httpStatusCode.Value, flag);
				httpResponse.StatusCode = (int)httpStatusCode.Value;
				httpResponse.SuppressContent = flag;
			}
			else if (message.IsFault)
			{
				httpResponse.StatusCode = 500;
			}
			if (!flag)
			{
				this.messageEncoder.WriteMessage(message, httpResponse.OutputStream);
			}
		}

		private static IAsyncResult SetHttpResponseStatusAndCreateCompletedAsyncResult(HttpResponse httpResponse, HttpStatusCode statusCode, AsyncCallback asyncCallback, object state)
		{
			httpResponse.StatusCode = (int)statusCode;
			httpResponse.SuppressContent = true;
			return new AsyncResult(asyncCallback, state, true);
		}

		private void BailoutForAuthError()
		{
			object obj;
			EwsOperationContextBase.Current.OutgoingMessageProperties.TryGetValue(HttpResponseMessageProperty.Name, out obj);
			HttpStatusCode? arg;
			bool flag;
			if (!EwsMethodDispatcher.TryGetHttpStatusFromHttpResponseMessagePropertyFromOperationContext(out arg, out flag))
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError((long)this.GetHashCode(), "Access check failed without a message, got {0}", new object[]
				{
					(obj != null) ? obj : "null"
				});
				arg = new HttpStatusCode?(HttpStatusCode.InternalServerError);
				flag = true;
			}
			ExTraceGlobals.CommonAlgorithmTracer.TraceWarning<HttpStatusCode?>((long)this.GetHashCode(), "Access check failed with {0}", arg);
			BailOut.SetHTTPStatusAndClose(arg.Value);
		}

		private static bool TryGetHttpStatusFromHttpResponseMessagePropertyFromOperationContext(out HttpStatusCode? httpStatus, out bool suppressResponseBody)
		{
			httpStatus = null;
			suppressResponseBody = false;
			object obj;
			EwsOperationContextBase.Current.OutgoingMessageProperties.TryGetValue(HttpResponseMessageProperty.Name, out obj);
			HttpResponseMessageProperty httpResponseMessageProperty = obj as HttpResponseMessageProperty;
			if (obj == null || httpResponseMessageProperty == null)
			{
				return false;
			}
			httpStatus = new HttpStatusCode?(httpResponseMessageProperty.StatusCode);
			suppressResponseBody = httpResponseMessageProperty.SuppressEntityBody;
			return true;
		}

		private void WriteResponseToStream(Stream stream, ServerVersionInfo serverVersionInfo, object responseBody)
		{
			using (XmlDictionaryWriter xmlDictionaryWriter = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, true))
			{
				string soapNamespace = this.GetSoapNamespace();
				xmlDictionaryWriter.WriteStartDocument();
				xmlDictionaryWriter.WriteStartElement("s:Envelope");
				xmlDictionaryWriter.WriteXmlnsAttribute("s", soapNamespace);
				this.WriteResponseHeader(xmlDictionaryWriter, soapNamespace, serverVersionInfo);
				this.WriteResponseBody(xmlDictionaryWriter, soapNamespace, responseBody);
				xmlDictionaryWriter.WriteEndElement();
				xmlDictionaryWriter.WriteEndDocument();
				xmlDictionaryWriter.Flush();
			}
		}

		private void WriteResponseHeader(XmlDictionaryWriter writer, string soapNamespace, ServerVersionInfo serverVersionInfo)
		{
			writer.WriteStartElement("Header", soapNamespace);
			XmlRootAttribute xmlRootAttribute = new XmlRootAttribute("ServerVersionInfo");
			xmlRootAttribute.Namespace = "http://schemas.microsoft.com/exchange/services/2006/types";
			SafeXmlSerializer orCreateXmlSerializer = this.serviceMethodInfo.GetOrCreateXmlSerializer(typeof(ServerVersionInfo), xmlRootAttribute);
			orCreateXmlSerializer.Serialize(writer, serverVersionInfo, EwsMethodDispatcher.responseHeaderNamespaces);
			writer.WriteEndElement();
		}

		private void WriteResponseBody(XmlDictionaryWriter writer, string soapNamespace, object responseBody)
		{
			writer.WriteStartElement("Body", soapNamespace);
			XmlRootAttribute xmlRootAttribute = new XmlRootAttribute(this.serviceMethodInfo.ResponseBodyType.Name.Replace("Message", string.Empty));
			xmlRootAttribute.Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages";
			SafeXmlSerializer orCreateXmlSerializer = this.serviceMethodInfo.GetOrCreateXmlSerializer(this.serviceMethodInfo.ResponseBodyType, xmlRootAttribute);
			orCreateXmlSerializer.Serialize(writer, responseBody);
			writer.WriteEndElement();
		}

		private XmlReader GetBody(Message message)
		{
			return message.GetReaderAtBodyContents();
		}

		private string GetSoapNamespace()
		{
			if (this.wcfMessageVersion == MessageVersion.Soap11)
			{
				return "http://schemas.xmlsoap.org/soap/envelope/";
			}
			if (this.wcfMessageVersion == MessageVersion.Soap12)
			{
				return "http://www.w3.org/2003/05/soap-envelope";
			}
			throw new NotImplementedException(string.Format("soap version {0} is not supported", this.wcfMessageVersion));
		}

		private const string SoapNamespacePrefix = "s";

		private const string SoapPrefixedEnvelopeElementName = "s:Envelope";

		private static readonly MessageInspectorManager messageInspectorManager = new MessageInspectorManager();

		private static readonly EWSAuthorizationManager EwsAuthorizationManager = new EWSAuthorizationManager();

		private static readonly XmlSerializerNamespaces responseHeaderNamespaces = new XmlSerializerNamespaces();

		private MessageVersion wcfMessageVersion;

		private CallContext callcontext;

		private EWSService service;

		private ServiceMethodInfo serviceMethodInfo;

		private MessageEncoder messageEncoder;

		private EwsOperationContext operationContext;
	}
}
