using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.InfoWorker.Common;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.DispatchPipe.Ews;
using Microsoft.Exchange.Services.EventLogs;

namespace Microsoft.Exchange.Services.Wcf
{
	internal abstract class ExceptionHandlerBase : IErrorHandler
	{
		public static FaultCode InternalServerErrorFaultCode { get; protected set; }

		internal static FaultException HandleInternalServerError(object responsibleObject, Exception exception)
		{
			ExceptionHandlerBase.ReportException(responsibleObject, exception);
			return ExceptionHandlerBase.CreateInternalServerErrorException(exception);
		}

		internal static void ReportException(object responsibleObject, Exception exception)
		{
			if (EwsOperationContextBase.Current != null && EwsOperationContextBase.Current.RequestMessage != null)
			{
				ExTraceGlobals.ExceptionTracer.TraceError<Message, string, string>(0L, "[ExceptionHandlerInspector::HandleInternalServerError] Request that caused exception: {0}.  Exception Class: {1}, Exception Message: {2}", EwsOperationContextBase.Current.RequestMessage, exception.GetType().FullName, exception.Message);
			}
			bool flag = true;
			if (exception is FaultException)
			{
				FaultException ex = exception as FaultException;
				if (ex.Code.IsSenderFault)
				{
					flag = false;
				}
			}
			if (flag)
			{
				ServiceDiagnostics.ReportException(exception, ServicesEventLogConstants.Tuple_InternalServerError, responsibleObject, "Encountered unhandled exception: {0}");
			}
		}

		internal static FaultException CreateInternalServerErrorException(Exception exception)
		{
			return FaultExceptionUtilities.CreateFault(new InternalServerErrorException(exception), FaultParty.Receiver);
		}

		public bool HandleError(Exception error)
		{
			ExceptionHandlerBase.ReportException(this, error);
			return true;
		}

		public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
		{
			Message message = fault;
			if (this.TryProvideCommunicationExceptionFault(error, version, ref fault))
			{
				ExWatson.SetWatsonReportAlreadySent(error);
				return;
			}
			FaultException ex = error as FaultException;
			if (ex == null)
			{
				ex = ExceptionHandlerBase.HandleInternalServerError(this, error);
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(RequestDetailsLogger.Current, error, "ExceptionHandlerBase_ProvideFault_Error");
			}
			else if (!EWSSettings.IsFromEwsAssemblies(ex.Source))
			{
				ExceptionHandlerBase.ReportException(this, error);
			}
			else
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeLogRequestException(RequestDetailsLogger.Current, ex, "ExceptionHandlerBase_ProvideFault_FaultException");
			}
			if (fault == null)
			{
				MessageFault fault2 = ex.CreateMessageFault();
				fault = Message.CreateMessage(version, fault2, "*");
			}
		}

		private bool TryProvideCommunicationExceptionFault(Exception error, MessageVersion version, ref Message fault)
		{
			bool result = false;
			CommunicationException ex = error as CommunicationException;
			if (ex != null)
			{
				if (ex.GetType().FullName.Contains("MustUnderstandSoapException") && fault != null)
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceDebug((long)this.GetHashCode(), "[ExceptionHandlerInspector::ProvideFault] Request failed due to the presence of soap:mustUnderstand on a header that EWS did not understand.");
					result = true;
				}
				else if (ex.InnerException != null)
				{
					LocalizedException ex2 = null;
					if (ex.InnerException is InvalidOperationException && ex.InnerException.InnerException != null && (ex.InnerException.InnerException is XmlException || ex.InnerException.InnerException is InvalidParameterException))
					{
						ex2 = new InvalidRequestException(ex);
					}
					else if (ex.InnerException is QuotaExceededException)
					{
						ex2 = new InvalidRequestException(CoreResources.IDs.ErrorInvalidRequestQuotaExceeded, ex);
					}
					if (ex2 != null)
					{
						ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<CommunicationException>((long)this.GetHashCode(), "[ExceptionHandlerInspector::ProvideFault] encountered a communication exception. Likely was unable to deserialize the request due to the following error: {0}", ex);
						FaultException ex3 = FaultExceptionUtilities.CreateFault(ex2, FaultParty.Sender);
						MessageFault fault2 = ex3.CreateMessageFault();
						fault = Message.CreateMessage(version, fault2, "*");
						result = true;
					}
				}
			}
			return result;
		}
	}
}
