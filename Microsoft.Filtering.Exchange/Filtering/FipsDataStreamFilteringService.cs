using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics.Components.Filtering;
using Microsoft.Filtering.Results;
using Microsoft.Internal.ManagedWPP;

namespace Microsoft.Filtering
{
	internal class FipsDataStreamFilteringService : IFipsDataStreamFilteringService, IDisposable
	{
		public FipsDataStreamFilteringService() : this(new FilteringService())
		{
			this.teLog.Start();
		}

		internal FipsDataStreamFilteringService(IFilteringService service)
		{
			this.service = service;
		}

		public IAsyncResult BeginScan(FipsDataStreamFilteringRequest fipsDataStreamFilteringRequest, FilteringRequest filteringRequest, AsyncCallback callback, object state)
		{
			filteringRequest.AddRecoveryOptions(fipsDataStreamFilteringRequest.RecoveryOptions);
			IAsyncResult result;
			try
			{
				result = new FipsDataStreamFilteringAsyncResult((AsyncCallback c) => this.service.BeginScan(filteringRequest, c, state), callback, fipsDataStreamFilteringRequest);
			}
			catch (FilteringException e)
			{
				this.AddExceptionData(e);
				throw;
			}
			return result;
		}

		public FilteringResponse EndScan(IAsyncResult ar)
		{
			FipsDataStreamFilteringAsyncResult fipsDataStreamFilteringAsyncResult = (FipsDataStreamFilteringAsyncResult)ar;
			FilteringResponse result;
			try
			{
				FilteringResponse filteringResponse = this.service.EndScan(fipsDataStreamFilteringAsyncResult.InnerAsyncResult);
				if ((filteringResponse.Flags.HasFlag(2) || filteringResponse.Flags.HasFlag(64)) && Tracing.tracer.Level >= 4 && (Tracing.tracer.Flags & 2048) != 0)
				{
					WPP_0aa0dc6458a5fee18236a223d5cb2d97.WPP_isss(6, 10, this.GetHashCode(), TraceProvider.MakeStringArg(fipsDataStreamFilteringAsyncResult.FipsDataStreamFilteringRequest.Id), TraceProvider.MakeStringArg(filteringResponse.Flags), TraceProvider.MakeStringArg(ResultsFormatter.ConsoleFormatter.Format(filteringResponse.Results)));
				}
				this.LogTextExtractionInfo(filteringResponse.Results);
				result = filteringResponse;
			}
			catch (ScanTimeoutException e)
			{
				this.AddExceptionData(e);
				fipsDataStreamFilteringAsyncResult.FipsDataStreamFilteringRequest.RecoveryOptions = RecoveryOptions.Timeout;
				this.LogFailureResults(fipsDataStreamFilteringAsyncResult);
				throw;
			}
			catch (ScannerCrashException e2)
			{
				this.AddExceptionData(e2);
				fipsDataStreamFilteringAsyncResult.FipsDataStreamFilteringRequest.RecoveryOptions = RecoveryOptions.Crash;
				this.LogFailureResults(fipsDataStreamFilteringAsyncResult);
				throw;
			}
			catch (FilteringException e3)
			{
				this.AddExceptionData(e3);
				this.LogFailureResults(fipsDataStreamFilteringAsyncResult);
				throw;
			}
			return result;
		}

		public void Dispose()
		{
			this.teLog.Stop();
			this.service.Dispose();
		}

		private void AddExceptionData(FilteringException e)
		{
			FipsDataStreamFilteringService.ExceptionRetryInfo exceptionRetryInfo = null;
			if (FipsDataStreamFilteringService.exceptionRetryInfoMap.TryGetValue(e.GetType(), out exceptionRetryInfo))
			{
				e.Data.Add("RetryCount", exceptionRetryInfo.RetryCount);
				if (exceptionRetryInfo.MinSecondsBetweenRetries > 0 && exceptionRetryInfo.RetryCount != 0)
				{
					e.Data.Add("MinSecondsBetweenRetries", exceptionRetryInfo.MinSecondsBetweenRetries);
				}
			}
		}

		private void LogFailureResults(FipsDataStreamFilteringAsyncResult eafar)
		{
			FilteringAsyncResult filteringAsyncResult = (FilteringAsyncResult)eafar.InnerAsyncResult;
			if (filteringAsyncResult != null)
			{
				this.LogTextExtractionInfo(filteringAsyncResult.Response.DiagnosticResults);
			}
		}

		private void LogTextExtractionInfo(FilteringResults filteringResults)
		{
			if (filteringResults == null)
			{
				ExTraceGlobals.FilteringServiceApiTracer.TraceDebug((long)this.GetHashCode(), "Diagnostic Results are null. So text extraction information cannot be logged.");
				return;
			}
			string exMessageId = null;
			foreach (StreamIdentity streamIdentity in filteringResults.Streams)
			{
				if (streamIdentity.Id == 0)
				{
					exMessageId = streamIdentity.Name;
				}
				TextExtractionData teData = default(TextExtractionData);
				if (streamIdentity.Types.Length != 0)
				{
					teData.Types = FormatterExtensions.CommaSeparate(from t in streamIdentity.Types
					select t.ToString());
					streamIdentity.Properties.TryGetInt32("ScanningPipeline::TextExtractionKeys::TextExtractionResult", ref teData.TextExtractionResult);
					streamIdentity.Properties.TryGetInt64("Parsing::ParsingKeys::StreamSize", ref teData.StreamSize);
					teData.StreamId = streamIdentity.Id;
					teData.ParentId = streamIdentity.ParentId;
					streamIdentity.Properties.TryGetString("ScanningPipeline::TextExtractionKeys::TextExtractionMethod", ref teData.ModuleUsed);
					streamIdentity.Properties.TryGetString("ScanningPipeline::TextExtractionKeys::TextExtractionSkippedModules", ref teData.SkippedModules);
					streamIdentity.Properties.TryGetString("ScanningPipeline::TextExtractionKeys::TextExtractionFailedModules", ref teData.FailedModules);
					streamIdentity.Properties.TryGetString("ScanningPipeline::TextExtractionKeys::TextExtractionDisabledModules", ref teData.DisabledModules);
					streamIdentity.Properties.TryGetString("ScanningPipeline::TextExtractionKeys::TextExtractionAdditionalInformation", ref teData.AdditionalInformation);
					this.teLog.Trace(exMessageId, teData);
				}
			}
		}

		private static IDictionary<Type, FipsDataStreamFilteringService.ExceptionRetryInfo> exceptionRetryInfoMap = new Dictionary<Type, FipsDataStreamFilteringService.ExceptionRetryInfo>
		{
			{
				typeof(QueueFullException),
				new FipsDataStreamFilteringService.ExceptionRetryInfo
				{
					RetryCount = -1,
					MinSecondsBetweenRetries = (int)TimeSpan.FromMinutes(5.0).TotalSeconds
				}
			},
			{
				typeof(ConfigurationException),
				new FipsDataStreamFilteringService.ExceptionRetryInfo
				{
					RetryCount = 0,
					MinSecondsBetweenRetries = 0
				}
			},
			{
				typeof(ScanQueueTimeoutException),
				new FipsDataStreamFilteringService.ExceptionRetryInfo
				{
					RetryCount = -1,
					MinSecondsBetweenRetries = (int)TimeSpan.FromMinutes(2.0).TotalSeconds
				}
			},
			{
				typeof(ScanTimeoutException),
				new FipsDataStreamFilteringService.ExceptionRetryInfo
				{
					RetryCount = 3,
					MinSecondsBetweenRetries = (int)TimeSpan.FromHours(1.0).TotalSeconds
				}
			},
			{
				typeof(ScanAbortedException),
				new FipsDataStreamFilteringService.ExceptionRetryInfo
				{
					RetryCount = -1,
					MinSecondsBetweenRetries = 60
				}
			},
			{
				typeof(BiasException),
				new FipsDataStreamFilteringService.ExceptionRetryInfo
				{
					RetryCount = 0,
					MinSecondsBetweenRetries = 0
				}
			},
			{
				typeof(ScannerCrashException),
				new FipsDataStreamFilteringService.ExceptionRetryInfo
				{
					RetryCount = -1,
					MinSecondsBetweenRetries = 0
				}
			},
			{
				typeof(ServiceUnavailableException),
				new FipsDataStreamFilteringService.ExceptionRetryInfo
				{
					RetryCount = -1,
					MinSecondsBetweenRetries = 30
				}
			},
			{
				typeof(FilteringException),
				new FipsDataStreamFilteringService.ExceptionRetryInfo
				{
					RetryCount = -1,
					MinSecondsBetweenRetries = 0
				}
			}
		};

		private readonly IFilteringService service;

		private TextExtractionLog teLog = new TextExtractionLog();

		private class ExceptionRetryInfo
		{
			public int RetryCount { get; set; }

			public int MinSecondsBetweenRetries { get; set; }
		}
	}
}
