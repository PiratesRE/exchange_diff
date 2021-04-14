using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.RightsManagement.SOAP.Template;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal sealed class TemplateWSManager : IDisposeTrackable, IDisposable
	{
		public TemplateWSManager(Uri url, IWSManagerPerfCounters perfcounters, IRmsLatencyTracker latencyTracker, WebProxy proxy, TimeSpan timeout)
		{
			if (url == null)
			{
				throw new ArgumentNullException("url");
			}
			this.wsProxy = new TemplateWS();
			this.wsProxy.Url = RMUtil.ConvertUriToTemplateDistributionUrl(url);
			if (this.wsProxy.Url == null)
			{
				throw new InvalidRmsUrlException(url.OriginalString);
			}
			if (proxy != null)
			{
				this.wsProxy.Proxy = proxy;
			}
			this.wsProxy.Timeout = (int)timeout.TotalMilliseconds;
			this.wsProxy.Credentials = CredentialCache.DefaultCredentials;
			this.wsProxy.VersionDataValue = new VersionData();
			this.wsProxy.VersionDataValue.MaximumVersion = "1.1.0.0";
			this.wsProxy.VersionDataValue.MinimumVersion = "1.1.0.0";
			this.disposeTracker = this.GetDisposeTracker();
			this.perfcounters = (perfcounters ?? NoopWSManagerPerfCounters.Instance);
			this.rmsLatencyTracker = (latencyTracker ?? NoopRmsLatencyTracker.Instance);
		}

		public static bool IsValidTemplateInformation(TemplateInformation templateInformation)
		{
			if (templateInformation == null)
			{
				return false;
			}
			if (templateInformation.GuidHash == null)
			{
				if (templateInformation.GuidHashCount != 0)
				{
					ExTraceGlobals.RightsManagementTracer.TraceError(0L, "Validating template information failed, the GuidHash is null but GuidHashCount is not 0");
					return false;
				}
				ExTraceGlobals.RightsManagementTracer.TraceDebug(0L, "Validating template information passed, the GuidHash is null");
				return true;
			}
			else
			{
				if (templateInformation.GuidHash.Length != templateInformation.GuidHashCount || templateInformation.GuidHash.Length > 200)
				{
					ExTraceGlobals.RightsManagementTracer.TraceError(0L, "Validating template information failed, the GuidHash length is invalid");
					return false;
				}
				foreach (GuidHash guidHash2 in templateInformation.GuidHash)
				{
					Guid guid;
					if (guidHash2 == null || !DrmClientUtils.TryParseGuid(guidHash2.Guid, out guid))
					{
						ExTraceGlobals.RightsManagementTracer.TraceError(0L, "Validating template information failed, one of the GuidHash is not valid");
						return false;
					}
				}
				return true;
			}
		}

		public static bool IsValidTemplatesResponse(GuidTemplate[] guidTemplates, string[] requestedTemplateGuids)
		{
			if (guidTemplates == null || guidTemplates.Length != requestedTemplateGuids.Length)
			{
				ExTraceGlobals.RightsManagementTracer.TraceError(0L, "Validating Templates response failed, the returned guid template array is not valid");
				return false;
			}
			foreach (GuidTemplate guidTemplate in guidTemplates)
			{
				Guid guid;
				if (guidTemplate == null || !DrmClientUtils.TryParseGuid(guidTemplate.Guid, out guid) || string.IsNullOrEmpty(guidTemplate.Template))
				{
					ExTraceGlobals.RightsManagementTracer.TraceError(0L, "Validating templates response failed, the returned guid template is not valid");
					return false;
				}
			}
			return true;
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
				this.disposeTracker = null;
			}
			if (this.wsProxy != null)
			{
				this.wsProxy.Dispose();
				this.wsProxy = null;
			}
			GC.SuppressFinalize(this);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<TemplateWSManager>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal RmsTemplate[] AcquireAllTemplates()
		{
			IAsyncResult asyncResult = this.BeginAcquireAllTemplates(null, null);
			return this.EndAcquireAllTemplates(asyncResult);
		}

		internal IAsyncResult BeginAcquireAllTemplates(AsyncCallback callback, object state)
		{
			LazyAsyncResult lazyAsyncResult = new LazyAsyncResult(new LinkedList<RmsTemplate>(), state, callback);
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string>((long)this.GetHashCode(), "BeginAcquireAllTemplates: Getting TemplateInformation async from {0}", this.wsProxy.Url);
			this.rmsLatencyTracker.BeginTrackRmsLatency(RmsOperationType.AcquireTemplateInfo);
			this.wsProxy.BeginAcquireTemplateInformation(WsAsyncProxyWrapper.WrapCallbackWithUnhandledExceptionHandlerAndCrash(new AsyncCallback(this.AcquireTemplateInformationCallback)), lazyAsyncResult);
			return lazyAsyncResult;
		}

		internal RmsTemplate[] EndAcquireAllTemplates(IAsyncResult asyncResult)
		{
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string>((long)this.GetHashCode(), "EndAcquireAllTemplates invoked. Uri {0}", this.wsProxy.Url);
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			LazyAsyncResult lazyAsyncResult = asyncResult as LazyAsyncResult;
			if (lazyAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult has to be type of LazyAsyncResult.");
			}
			if (!lazyAsyncResult.IsCompleted)
			{
				lazyAsyncResult.InternalWaitForCompletion();
			}
			Exception ex = lazyAsyncResult.Result as Exception;
			if (ex != null)
			{
				throw ex;
			}
			LinkedList<RmsTemplate> linkedList = lazyAsyncResult.AsyncObject as LinkedList<RmsTemplate>;
			if (linkedList == null)
			{
				throw new InvalidOperationException("asyncResult.AsyncObject cannot be null here.");
			}
			return linkedList.ToArray<RmsTemplate>();
		}

		private void AcquireTemplatesCallback(IAsyncResult asyncResult)
		{
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string>((long)this.GetHashCode(), "AcquireTemplatesCallback invoked. Uri {0}", this.wsProxy.Url);
			if (asyncResult == null)
			{
				throw new InvalidOperationException("asyncResult cannot be null here.");
			}
			LazyAsyncResult lazyAsyncResult = asyncResult.AsyncState as LazyAsyncResult;
			if (lazyAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult.AsyncState cannot be null and has to be of type LazyAsyncResult.");
			}
			Exception ex = null;
			GuidTemplate[] array = null;
			try
			{
				array = this.wsProxy.EndAcquireTemplates(asyncResult);
			}
			catch (WebException ex2)
			{
				ex = ex2;
			}
			catch (SoapException ex3)
			{
				ex = ex3;
			}
			catch (InvalidOperationException ex4)
			{
				ex = ex4;
			}
			catch (IOException ex5)
			{
				ex = ex5;
			}
			catch (UnauthorizedAccessException ex6)
			{
				ex = ex6;
			}
			this.rmsLatencyTracker.EndTrackRmsLatency(RmsOperationType.AcquireTemplates);
			if (ex != null)
			{
				this.perfcounters.AcquireTemplatesFailed();
				ExTraceGlobals.RightsManagementTracer.TraceError<string, Exception>((long)this.GetHashCode(), "Failed to acquire templates from {0}. Error: {1}", this.wsProxy.Url, ex);
				ex = new RightsManagementException(RightsManagementFailureCode.TemplateAcquisitionFailed, DrmStrings.FailedToAcquireTemplates(this.wsProxy.Url), ex, this.wsProxy.Url);
				lazyAsyncResult.InvokeCallback(ex);
				return;
			}
			if (!TemplateWSManager.IsValidTemplatesResponse(array, this.lastRequestedTemplateGuids))
			{
				this.perfcounters.AcquireTemplatesFailed();
				ExTraceGlobals.RightsManagementTracer.TraceError<string>((long)this.GetHashCode(), "The template response returned from {0} is invalid.", this.wsProxy.Url);
				ex = new RightsManagementException(RightsManagementFailureCode.TemplateAcquisitionFailed, DrmStrings.FailedToAcquireTemplates(this.wsProxy.Url));
				lazyAsyncResult.InvokeCallback(ex);
				return;
			}
			LinkedList<RmsTemplate> linkedList = lazyAsyncResult.AsyncObject as LinkedList<RmsTemplate>;
			if (linkedList == null)
			{
				throw new InvalidOperationException("asyncResult.AsyncObject cannot be null here.");
			}
			this.perfcounters.AcquireTemplatesSuccessful(this.wsProxy.GetElapsedMilliseconds());
			foreach (GuidTemplate guidTemplate in array)
			{
				linkedList.AddLast(RmsTemplate.Create(new Guid(guidTemplate.Guid), guidTemplate.Template));
			}
			ExTraceGlobals.RightsManagementTracer.TraceDebug<int, string, int>((long)this.GetHashCode(), "Acquired {0} templates from {1}. Total {2}", linkedList.Count, this.wsProxy.Url, this.templateInformation.GuidHashCount);
			if (this.templateInformation.GuidHashCount == linkedList.Count)
			{
				lazyAsyncResult.InvokeCallback(null);
				return;
			}
			this.SendNextRequest(lazyAsyncResult);
		}

		private void AcquireTemplateInformationCallback(IAsyncResult asyncResult)
		{
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string>((long)this.GetHashCode(), "AcquireTemplateInformationCallback invoked. Uri {0}", this.wsProxy.Url);
			if (asyncResult == null || asyncResult.AsyncState == null)
			{
				throw new InvalidOperationException("asyncResult or state cannot be null here.");
			}
			LazyAsyncResult lazyAsyncResult = asyncResult.AsyncState as LazyAsyncResult;
			if (lazyAsyncResult == null)
			{
				throw new InvalidOperationException("asyncResult.AsyncState has to be type of LazyAsyncResult.");
			}
			Exception ex = null;
			try
			{
				this.templateInformation = this.wsProxy.EndAcquireTemplateInformation(asyncResult);
			}
			catch (WebException ex2)
			{
				ex = ex2;
			}
			catch (SoapException ex3)
			{
				ex = ex3;
			}
			catch (InvalidOperationException ex4)
			{
				ex = ex4;
			}
			catch (IOException ex5)
			{
				ex = ex5;
			}
			catch (UnauthorizedAccessException ex6)
			{
				ex = ex6;
			}
			this.rmsLatencyTracker.EndTrackRmsLatency(RmsOperationType.AcquireTemplateInfo);
			if (ex != null)
			{
				this.perfcounters.AcquireTemplatesFailed();
				ExTraceGlobals.RightsManagementTracer.TraceError<string, Exception>((long)this.GetHashCode(), "Failed to acquire templates from {0}. Error: {1}", this.wsProxy.Url, ex);
				ex = new RightsManagementException(RightsManagementFailureCode.TemplateAcquisitionFailed, DrmStrings.FailedToAcquireTemplateInformation(this.wsProxy.Url), ex, this.wsProxy.Url);
				lazyAsyncResult.InvokeCallback(ex);
				return;
			}
			if (!TemplateWSManager.IsValidTemplateInformation(this.templateInformation))
			{
				this.perfcounters.AcquireTemplatesFailed();
				ExTraceGlobals.RightsManagementTracer.TraceDebug<string>((long)this.GetHashCode(), "Invalid template information retrieved from url: {0}.", this.wsProxy.Url);
				ex = new RightsManagementException(RightsManagementFailureCode.TemplateAcquisitionFailed, DrmStrings.InvalidResponseToTemplateInformationRequest(this.wsProxy.Url), this.wsProxy.Url);
				lazyAsyncResult.InvokeCallback(ex);
				return;
			}
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string>((long)this.GetHashCode(), "Successfully acquired the templates from {0}", this.wsProxy.Url);
			this.perfcounters.AcquireTemplatesSuccessful(this.wsProxy.GetElapsedMilliseconds());
			if (this.templateInformation.GuidHash == null)
			{
				ExTraceGlobals.RightsManagementTracer.TraceDebug<string>((long)this.GetHashCode(), "There are no server side templates configured for {0}. Invoking caller callback", this.wsProxy.Url);
				lazyAsyncResult.InvokeCallback();
				return;
			}
			this.processedTemplateIndex = 0;
			this.SendNextRequest(lazyAsyncResult);
		}

		private void SendNextRequest(LazyAsyncResult asyncResult)
		{
			int num = this.templateInformation.GuidHashCount - this.processedTemplateIndex;
			ExTraceGlobals.RightsManagementTracer.TraceDebug<string, int>((long)this.GetHashCode(), "Sending next set of request to fetch templates from {0}. Remaining: {1}", this.wsProxy.Url, num);
			string[] array;
			if (num < 25)
			{
				array = new string[num];
				int i = 0;
				while (i < num)
				{
					array[i] = this.templateInformation.GuidHash[this.processedTemplateIndex].Guid;
					i++;
					this.processedTemplateIndex++;
				}
			}
			else
			{
				array = new string[25];
				int j = 0;
				while (j < 25)
				{
					array[j] = this.templateInformation.GuidHash[this.processedTemplateIndex].Guid;
					j++;
					this.processedTemplateIndex++;
				}
			}
			this.lastRequestedTemplateGuids = array;
			this.rmsLatencyTracker.BeginTrackRmsLatency(RmsOperationType.AcquireTemplates);
			this.wsProxy.BeginAcquireTemplates(array, WsAsyncProxyWrapper.WrapCallbackWithUnhandledExceptionHandlerAndCrash(new AsyncCallback(this.AcquireTemplatesCallback)), asyncResult);
		}

		private TemplateWS wsProxy;

		private TemplateInformation templateInformation;

		private int processedTemplateIndex;

		private string[] lastRequestedTemplateGuids;

		private DisposeTracker disposeTracker;

		private IWSManagerPerfCounters perfcounters;

		private IRmsLatencyTracker rmsLatencyTracker;
	}
}
