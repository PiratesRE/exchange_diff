using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xml.Schema;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal static class FaultExceptionUtilities
	{
		public static bool GetIsTransient(Exception exception)
		{
			bool result = false;
			if (exception != null)
			{
				if (exception is TransientException)
				{
					result = true;
				}
				else if (exception is OverBudgetException)
				{
					result = true;
				}
				else
				{
					object obj = exception.Data["IsTransient"];
					if (obj is bool)
					{
						result = (bool)obj;
					}
				}
			}
			return result;
		}

		internal static FaultException CreateFault(LocalizedException exception, FaultParty faultParty)
		{
			EwsMessageFaultDetail messageFault = new EwsMessageFaultDetail(exception, faultParty, ExchangeVersion.UnsafeGetCurrent());
			return FaultExceptionUtilities.CreateFaultException(messageFault, exception);
		}

		internal static FaultException CreateFault(LocalizedException exception, FaultParty faultParty, ExchangeVersion currentExchangeVersion)
		{
			EwsMessageFaultDetail messageFault = new EwsMessageFaultDetail(exception, faultParty, currentExchangeVersion);
			return FaultExceptionUtilities.CreateFaultException(messageFault, exception);
		}

		internal static FaultException CreateAvailabilityFault(LocalizedException exception, FaultParty faultParty)
		{
			AvailabilityMessageFaultDetail messageFault = new AvailabilityMessageFaultDetail(exception, faultParty);
			return FaultExceptionUtilities.CreateFaultException(messageFault, exception);
		}

		internal static FaultException CreateUMFault(LocalizedException exception, FaultParty faultParty)
		{
			UmMessageFaultDetail messageFault = new UmMessageFaultDetail(exception, faultParty);
			return FaultExceptionUtilities.CreateFaultException(messageFault, exception);
		}

		private static FaultException CreateFaultException(MessageFault messageFault, Exception exception)
		{
			FaultException ex = new FaultException(messageFault, string.Empty);
			ex.Source = EWSSettings.SimpleAssemblyName;
			ex.Data["IsTransient"] = FaultExceptionUtilities.GetIsTransient(exception);
			if (exception != null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericError(RequestDetailsLogger.Current, "FaultInnerException", exception.ToString());
			}
			if (ex.Code != null && ex.Code.SubCode != null)
			{
				RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(RequestDetailsLogger.Current, ServiceCommonMetadata.ErrorCode, ex.Code.SubCode.Name);
			}
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(RequestDetailsLogger.Current, ServiceCommonMetadata.HttpStatus, "500");
			return ex;
		}

		internal static FaultException DealWithSchemaViolation(SchemaValidationException exception, Message request)
		{
			ExchangeVersion exchangeVersion = (ExchangeVersion)request.Properties["WS_ServerVersionKey"];
			if (exchangeVersion.Equals(ExchangeVersion.Latest))
			{
				return FaultExceptionUtilities.CreateFault(exception, FaultParty.Sender);
			}
			Stream stream = (Stream)request.Properties["MessageStream"];
			string methodName = (string)request.Properties["MethodName"];
			FaultException result;
			try
			{
				try
				{
					stream.Position = 0L;
				}
				catch (InvalidOperationException)
				{
					return FaultExceptionUtilities.CreateFault(exception, FaultParty.Sender);
				}
				catch (ArgumentOutOfRangeException)
				{
					return FaultExceptionUtilities.CreateFault(exception, FaultParty.Sender);
				}
				SchemaValidator schemaValidator = new SchemaValidator(delegate(XmlSchemaException xmlException, SoapSavvyReader.SoapSection soapSection)
				{
					throw new SchemaValidationException(xmlException, xmlException.LineNumber, xmlException.LinePosition, xmlException.Message);
				});
				try
				{
					schemaValidator.ValidateMessage(stream, ExchangeVersion.Latest, MessageInspectorManager.IsEWSRequest(methodName), true);
				}
				catch (SchemaValidationException)
				{
					return FaultExceptionUtilities.CreateFault(exception, FaultParty.Sender);
				}
				result = FaultExceptionUtilities.CreateFault(new IncorrectSchemaVersionException(), FaultParty.Sender);
			}
			finally
			{
				BufferedRegionStream bufferedRegionStream = stream as BufferedRegionStream;
				if (bufferedRegionStream != null)
				{
					bufferedRegionStream.Dispose();
					request.Properties["MessageStream"] = null;
				}
			}
			return result;
		}

		private const string IsTransientKey = "IsTransient";
	}
}
