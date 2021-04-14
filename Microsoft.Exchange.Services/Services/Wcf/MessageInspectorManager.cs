using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.DispatchPipe.Ews;

namespace Microsoft.Exchange.Services.Wcf
{
	internal class MessageInspectorManager : IDispatchMessageInspector
	{
		internal MessageInspectorManager()
		{
			this.inboundInspectors = new List<IInboundInspector>();
			this.outboundInspectors = new List<IOutboundInspector>();
			this.AddRequestResponseInspectors();
		}

		public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
		{
			return this.InternalAfterReceiveRequest(ref request, channel, instanceContext, null);
		}

		protected virtual object InternalAfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext, MessageBuffer buffer)
		{
			this.TraceCorrelationHeader(EWSSettings.RequestCorrelation);
			DispatchByBodyElementOperationSelector.CheckWcfDelayedException(request);
			this.CheckForDuplicateHeaders(request);
			CallContext callContext = null;
			try
			{
				FaultInjection.GenerateFault((FaultInjection.LIDs)3286641981U);
				MessageHeaderProcessor messageHeaderProcessor = (MessageHeaderProcessor)request.Properties["MessageHeaderProcessor"];
				messageHeaderProcessor.MarkMessageHeaderAsUnderstoodIfExists(request, "RequestServerVersion", "http://schemas.microsoft.com/exchange/services/2006/types");
				Message requestRef = request;
				RequestDetailsLogger.Current.TrackLatency(ServiceLatencyMetadata.CallContextInitLatency, delegate()
				{
					callContext = CallContext.CreateFromRequest(messageHeaderProcessor, requestRef);
					HttpContext.Current.Items["CallContext"] = callContext;
					if (Global.ChargePreExecuteToBudgetEnabled)
					{
						IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
						TimeSpan timeSpan = TimeSpan.Zero;
						if (currentActivityScope != null && currentActivityScope.TotalMilliseconds >= 0.0)
						{
							timeSpan = TimeSpan.FromMilliseconds(Math.Max(currentActivityScope.TotalMilliseconds - EWSSettings.WcfDispatchLatency, 0.0));
							ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<TimeSpan, double, double>((long)this.GetHashCode(), "[MessageInspectorManager::AfterReceiveRequest] preExecuteCharge = {0}; preExecuteLatency = {1}, WcfDispatchlatency = {2}", timeSpan, currentActivityScope.TotalMilliseconds, EWSSettings.WcfDispatchLatency);
						}
						callContext.Budget.StartLocal("MessageInspectorManager.AfterReceiveRequest[" + callContext.MethodName + "]", timeSpan);
					}
				});
			}
			catch (LocalizedException ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<string, string>((long)this.GetHashCode(), "[MessageInspectorManager::AfterReceiveRequest] Caught localized exception trying to create callcontext.  Class: {0}, Message: {1}", ex.GetType().FullName, ex.Message);
				throw FaultExceptionUtilities.CreateFault(ex, FaultParty.Receiver);
			}
			catch (AuthzException innerException)
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug(0L, "[MessageInspectorManager::AfterReceiveRequest] Trying to create a CallContext failed with AuthZException.");
				AuthZFailureException exception = new AuthZFailureException(innerException);
				throw FaultExceptionUtilities.CreateFault(exception, FaultParty.Sender);
			}
			bool flag = false;
			try
			{
				if (callContext.AccessingPrincipal != null && ExUserTracingAdaptor.Instance.IsTracingEnabledUser(callContext.AccessingPrincipal.LegacyDn))
				{
					flag = true;
					BaseTrace.CurrentThreadSettings.EnableTracing();
				}
				if (!callContext.CallerHasAccess())
				{
					EWSAuthorizationManager.Return403ForbiddenResponse(EwsOperationContextBase.Current, "User not allowed to access EWS");
					throw FaultExceptionUtilities.CreateFault(new ServiceAccessDeniedException(), FaultParty.Receiver);
				}
				if (buffer == null)
				{
					buffer = this.CreateMessageBuffer(request);
					if (Global.UseGcCollect && buffer.BufferSize > Global.CreateItemRequestSizeThreshold)
					{
						using (Process currentProcess = Process.GetCurrentProcess())
						{
							if (currentProcess.PrivateMemorySize64 > (long)Global.PrivateWorkingSetThreshold)
							{
								this.CheckCollectIntervalAndCollect();
							}
						}
					}
				}
				EWSSettings.MessageCopyForProxyOnly = buffer.CreateMessage();
				foreach (IInboundInspector inboundInspector in this.inboundInspectors)
				{
					Message request2 = buffer.CreateMessage();
					inboundInspector.ProcessInbound(ExchangeVersion.Current, request2);
				}
				request = buffer.CreateMessage();
			}
			finally
			{
				if (flag)
				{
					BaseTrace.CurrentThreadSettings.DisableTracing();
				}
			}
			return null;
		}

		protected MessageBuffer CreateMessageBuffer(Message request)
		{
			MessageBuffer result = null;
			try
			{
				result = request.CreateBufferedCopy(int.MaxValue);
			}
			catch (ArgumentException ex)
			{
				throw FaultExceptionUtilities.CreateFault(new SchemaValidationException(ex, 0, 0, ex.Message), FaultParty.Sender);
			}
			catch (SchemaValidationException exception)
			{
				throw FaultExceptionUtilities.DealWithSchemaViolation(exception, request);
			}
			catch (XmlException ex2)
			{
				throw FaultExceptionUtilities.CreateFault(new SchemaValidationException(ex2, ex2.LineNumber, ex2.LinePosition, ex2.Message), FaultParty.Sender);
			}
			return result;
		}

		private void CheckCollectIntervalAndCollect()
		{
			lock (this.collectTimeLock)
			{
				if (DateTime.UtcNow > this.lastCollectTime.AddMilliseconds((double)Global.CollectIntervalInMilliseconds))
				{
					GC.Collect();
					this.lastCollectTime = DateTime.UtcNow;
				}
			}
		}

		private static bool IsOperationIn(string[] operationsToCheckAgainst, string methodName)
		{
			if (methodName == null)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError(0L, "[MessageInspectorManager::IsOperationIn] message did not have Action header.");
				return false;
			}
			foreach (string value in operationsToCheckAgainst)
			{
				if (methodName.Contains(value))
				{
					return true;
				}
			}
			return false;
		}

		private void TraceCorrelationHeader(Guid correlationGuid)
		{
			if (ExTraceGlobals.AllRequestsTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				string traceCorrelationHeader = MessageEncoderWithXmlDeclaration.GetTraceCorrelationHeader(correlationGuid);
				ExTraceGlobals.AllRequestsTracer.TraceDebug((long)this.GetHashCode(), traceCorrelationHeader);
			}
		}

		private void CreateReplyFromHTTPProxyResponse(MessageVersion messageVersion, string action, out Message reply)
		{
			try
			{
				FaultInjection.GenerateFault((FaultInjection.LIDs)4259720509U);
				reply = ProxyResponseMessage.Create();
			}
			catch (WebException ex)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceError<string, string>((long)this.GetHashCode(), "[MessageInspectorManager::CreateReplyFromHTTPProxyResponse] Caught WebException trying to create XmlReader from proxy response stream.  WebException status: {0}, Message: {1}", ex.Status.ToString(), ex.Message);
				if (ex.Status != WebExceptionStatus.RequestCanceled)
				{
					throw;
				}
				FaultException ex2 = FaultExceptionUtilities.CreateFault(new TransientException(CoreResources.GetLocalizedString((CoreResources.IDs)3995283118U), ex), FaultParty.Receiver);
				MessageFault fault = ex2.CreateMessageFault();
				reply = Message.CreateMessage(messageVersion, fault, action);
			}
		}

		public void BeforeSendReply(ref Message reply, object correlationState)
		{
			this.InternalBeforeSendReply(ref reply, correlationState);
		}

		public Message BeforeSendReply(HttpResponse httpResponse, MessageVersion messageVersion, string action)
		{
			Message result = null;
			this.InternalBeforeSendReply(httpResponse, ref result, messageVersion, action, null);
			return result;
		}

		protected virtual void InternalBeforeSendReply(ref Message reply, object correlationState)
		{
			this.InternalBeforeSendReply((HttpContext.Current != null && HttpContext.Current.Response != null) ? HttpContext.Current.Response : null, ref reply, (reply != null) ? reply.Version : null, (reply != null) ? reply.Headers.Action : null, correlationState);
		}

		private void InternalBeforeSendReply(HttpResponse httpResponse, ref Message reply, MessageVersion messageVersion, string action, object correlationState)
		{
			if (EWSSettings.ProxyResponse != null)
			{
				this.CreateReplyFromHTTPProxyResponse(messageVersion, action, out reply);
				Dictionary<string, string> proxyHopHeaders = EWSSettings.ProxyHopHeaders;
				if (!Global.WriteProxyHopHeaders || proxyHopHeaders == null || httpResponse == null)
				{
					return;
				}
				using (Dictionary<string, string>.Enumerator enumerator = proxyHopHeaders.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, string> keyValuePair = enumerator.Current;
						httpResponse.AppendHeader(keyValuePair.Key, keyValuePair.Value);
					}
					return;
				}
			}
			ExchangeVersion requestVersion = ExchangeVersion.Current;
			if (httpResponse != null)
			{
				if (Global.WriteFailoverTypeHeader && EWSSettings.FailoverType != null)
				{
					httpResponse.AppendHeader("FailoverType", EWSSettings.FailoverType);
				}
				if (EWSSettings.ExceptionType != null)
				{
					httpResponse.AppendHeader("X-BEServerException", EWSSettings.ExceptionType);
				}
			}
			if (reply != null)
			{
				MessageBuffer messageBuffer = (this.outboundInspectors.Count > 0) ? reply.CreateBufferedCopy(int.MaxValue) : null;
				try
				{
					foreach (IOutboundInspector outboundInspector in this.outboundInspectors)
					{
						Message reply2 = messageBuffer.CreateMessage();
						outboundInspector.ProcessOutbound(requestVersion, reply2);
					}
					if (messageBuffer != null)
					{
						reply = messageBuffer.CreateMessage();
					}
				}
				catch (FaultException ex)
				{
					MessageFault fault = ex.CreateMessageFault();
					reply = Message.CreateMessage(messageVersion, fault, action);
				}
			}
		}

		private void CheckForDuplicateHeaders(Message request)
		{
			bool flag = false;
			bool flag2 = false;
			foreach (MessageHeaderInfo messageHeaderInfo in request.Headers)
			{
				MessageHeader header = (MessageHeader)messageHeaderInfo;
				this.ValidateS2SHeaderDups(header, ref flag);
				this.ValidateProxyHeaderDups(header, ref flag2);
			}
		}

		private void ValidateS2SHeaderDups(MessageHeader header, ref bool seenS2SHeader)
		{
			if ((header.Name == "ExchangeImpersonation" || header.Name == "SerializedSecurityContext") && header.Namespace == "http://schemas.microsoft.com/exchange/services/2006/types")
			{
				if (seenS2SHeader)
				{
					throw FaultExceptionUtilities.CreateFault(new MoreThanOneAccessModeSpecifiedException(), FaultParty.Sender);
				}
				seenS2SHeader = true;
			}
		}

		private void ValidateProxyHeaderDups(MessageHeader header, ref bool seenProxyHeader)
		{
			if ((header.Name == "ProxySecurityContext" || header.Name == "ProxySuggesterSid" || header.Name == "ProxyPartnerToken") && header.Namespace == "http://schemas.microsoft.com/exchange/services/2006/types")
			{
				if (seenProxyHeader)
				{
					throw FaultExceptionUtilities.CreateFault(new InvalidProxySecurityContextException(), FaultParty.Sender);
				}
				seenProxyHeader = true;
			}
		}

		protected virtual void AddRequestResponseInspectors()
		{
		}

		internal static bool IsEWSRequest(string methodName)
		{
			return !MessageInspectorManager.IsAvailabilityRequest(methodName);
		}

		internal static bool IsAvailabilityRequest(string methodName)
		{
			return MessageInspectorManager.IsOperationIn(MessageInspectorManager.ASOperations, methodName);
		}

		internal const string ResponseHasBegunKey = "ResponseHasBegun";

		private static readonly string[] ASOperations = new string[]
		{
			"GetUserAvailability",
			"GetUserOofSettings",
			"SetUserOofSettings"
		};

		private static readonly string[] DelayExecutedOperations = new string[]
		{
			"GetItem",
			"GetAttachment",
			"ExportItems"
		};

		protected List<IInboundInspector> inboundInspectors;

		protected List<IOutboundInspector> outboundInspectors;

		private object collectTimeLock = new object();

		private DateTime lastCollectTime = DateTime.UtcNow;
	}
}
