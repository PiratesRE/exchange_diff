using System;
using System.Threading;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Filtering;
using Microsoft.Filtering.Configuration;
using Microsoft.Filtering.Results;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class UnifiedContentServiceInvoker
	{
		public string ErrorDescription { get; protected set; }

		internal static FilteringResults GetUnifiedContentResults(MailItem mailItem)
		{
			TransportFilteringServiceInvokerRequest filteringServiceInvokerRequest = TransportFilteringServiceInvokerRequest.CreateInstance(mailItem, null);
			return UnifiedContentServiceInvoker.GetUnifiedContentResults(filteringServiceInvokerRequest);
		}

		internal static FilteringResults GetUnifiedContentResults(FilteringServiceInvokerRequest filteringServiceInvokerRequest)
		{
			ExTraceGlobals.TransportRulesEngineTracer.TraceDebug(0L, "Executing text extraction filtering service");
			AutoResetEvent scanComplete = new AutoResetEvent(false);
			FilteringResults filteringResults = null;
			FilteringServiceInvoker.ScanResult textExtractionResult = FilteringServiceInvoker.ScanResult.Failure;
			Exception textExtractionException = null;
			UnifiedContentServiceInvoker unifiedContentServiceInvoker = new UnifiedContentServiceInvoker();
			FilteringServiceInvoker.BeginScanResult beginScanResult = unifiedContentServiceInvoker.BeginTextExtraction(filteringServiceInvokerRequest, delegate(FilteringServiceInvoker.ScanResult scanResult, FilteringResults extractedContent, Exception exception)
			{
				UnifiedContentServiceInvoker.TextExtracted(scanComplete, scanResult, exception, extractedContent, out filteringResults, out textExtractionResult, out textExtractionException);
			});
			if (beginScanResult == FilteringServiceInvoker.BeginScanResult.Queued)
			{
				int num = (int)(1.1 * filteringServiceInvokerRequest.ScanTimeout.TotalMilliseconds);
				if (!scanComplete.WaitOne(num))
				{
					string message = string.Format("The attempt to extract text in FIPS timed out after {0}ms", num);
					ExTraceGlobals.TransportRulesEngineTracer.TraceDebug(0L, message);
					throw new FilteringServiceTimeoutException(message);
				}
			}
			if (textExtractionResult == FilteringServiceInvoker.ScanResult.Success)
			{
				return filteringResults;
			}
			string message2 = TransportRulesStrings.ErrorInvokingFilteringService((int)beginScanResult, unifiedContentServiceInvoker.ErrorDescription);
			ExTraceGlobals.TransportRulesEngineTracer.TraceDebug(0L, message2);
			if (textExtractionException != null)
			{
				throw new FilteringServiceFailureException(string.Format("FIPS text extraction failed with error: '{0}'. See inner exception for details", textExtractionException.Message), textExtractionException);
			}
			throw new FilteringServiceFailureException(message2);
		}

		internal static void TextExtracted(AutoResetEvent textExtractionComplete, FilteringServiceInvoker.ScanResult scanResult, Exception exception, FilteringResults extractedContent, out FilteringResults extractedContentResult, out FilteringServiceInvoker.ScanResult textExtractionResult, out Exception textExtractionException)
		{
			textExtractionResult = scanResult;
			textExtractionException = exception;
			extractedContentResult = ((scanResult == FilteringServiceInvoker.ScanResult.Success) ? extractedContent : null);
			textExtractionComplete.Set();
		}

		internal FilteringServiceInvoker.BeginScanResult BeginTextExtraction(FilteringServiceInvokerRequest filteringServiceInvokerRequest, UnifiedContentServiceInvoker.TextExtractionCompleteCallback textExtractionCompleteCallback)
		{
			IFipsDataStreamFilteringService textExtractionService = null;
			FilteringServiceInvoker.BeginScanResult result;
			try
			{
				this.ErrorDescription = string.Empty;
				FilteringServiceFactory.Create(out textExtractionService);
				ScanConfiguration config = FipsFilteringServiceInvoker.CreateTextExtractionConfiguration(filteringServiceInvokerRequest.TextScanLimit, 0);
				FilteringRequest filteringRequest = FipsFilteringServiceInvoker.CreateFipsRequest(config, filteringServiceInvokerRequest);
				textExtractionService.BeginScan(filteringServiceInvokerRequest.FipsDataStreamFilteringRequest, filteringRequest, delegate(IAsyncResult asyncResult)
				{
					this.TextExtractionComplete(textExtractionService, textExtractionCompleteCallback, asyncResult);
				}, this);
				result = FilteringServiceInvoker.BeginScanResult.Queued;
			}
			catch (Exception ex)
			{
				this.LogBeginScanException(ex);
				if (textExtractionService != null)
				{
					textExtractionService.Dispose();
				}
				textExtractionCompleteCallback(FilteringServiceInvoker.ScanResult.Failure, null, ex);
				result = FilteringServiceInvoker.BeginScanResult.Failure;
			}
			return result;
		}

		protected void TextExtractionComplete(IFipsDataStreamFilteringService textExtractionService, UnifiedContentServiceInvoker.TextExtractionCompleteCallback textExtractionCompleteCallback, IAsyncResult asyncResult)
		{
			FilteringResults filteringResults = null;
			Exception exception = null;
			FilteringServiceInvoker.ScanResult result;
			try
			{
				FilteringResponse filteringResponse = textExtractionService.EndScan(asyncResult);
				filteringResults = filteringResponse.Results;
				RuleAgentResultUtils.ValidateResults(filteringResults);
				result = FilteringServiceInvoker.ScanResult.Success;
			}
			catch (Exception ex)
			{
				this.LogEndScanException(ex);
				exception = ex;
				if (ex is ScanQueueTimeoutException)
				{
					result = FilteringServiceInvoker.ScanResult.QueueTimeout;
				}
				else if (ex is ScanTimeoutException)
				{
					result = FilteringServiceInvoker.ScanResult.Timeout;
				}
				else if (ex is ScannerCrashException)
				{
					result = FilteringServiceInvoker.ScanResult.CrashFailure;
				}
				else
				{
					result = FilteringServiceInvoker.ScanResult.Failure;
				}
			}
			finally
			{
				textExtractionService.Dispose();
			}
			textExtractionCompleteCallback(result, filteringResults, exception);
		}

		protected void LogBeginScanException(Exception e)
		{
			this.LogTextExtractionServiceException(e, "BeginScan");
		}

		protected void LogEndScanException(Exception e)
		{
			this.LogTextExtractionServiceException(e, "EndScan");
		}

		protected void LogTextExtractionServiceException(Exception e, string scanPhase)
		{
			this.ErrorDescription = string.Format("Exception in FIPS text extraction (stage = '{0}', exception = '{1}')", scanPhase, e.Message);
			ExTraceGlobals.TransportRulesEngineTracer.TraceError(0L, this.ErrorDescription);
		}

		public delegate void TextExtractionCompleteCallback(FilteringServiceInvoker.ScanResult result, FilteringResults textExtractionResults, Exception exception);
	}
}
