using System;
using System.Globalization;
using System.Xml;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal abstract class LicenseManager : IDisposeTrackable, IDisposable
	{
		internal LicenseManager(IWSManagerPerfCounters perfcounters, IRmsLatencyTracker latencyTracker)
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.perfcounters = (perfcounters ?? NoopWSManagerPerfCounters.Instance);
			this.rmsLatencyTracker = (latencyTracker ?? NoopRmsLatencyTracker.Instance);
		}

		public virtual void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
				this.disposeTracker = null;
			}
			GC.SuppressFinalize(this);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<LicenseManager>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal IAsyncResult BeginAcquireLicense(XmlNode[] issuanceLicense, LicenseIdentity[] identities, AsyncCallback callback, object state)
		{
			if (identities == null)
			{
				throw new ArgumentNullException("identities");
			}
			if (issuanceLicense == null)
			{
				throw new ArgumentNullException("issuanceLicense");
			}
			LicenseManager.LicenseState worker = new LicenseManager.LicenseState(issuanceLicense, identities);
			LazyAsyncResult lazyAsyncResult = new LazyAsyncResult(worker, state, callback);
			this.IssueNewWebRequest(lazyAsyncResult);
			return lazyAsyncResult;
		}

		internal LicenseResponse[] EndAcquireLicense(IAsyncResult asyncResult)
		{
			ExTraceGlobals.RightsManagementTracer.TraceDebug((long)this.GetHashCode(), "EndAcquireLicense invoked.");
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
			RightsManagementException ex = lazyAsyncResult.Result as RightsManagementException;
			if (ex != null)
			{
				throw ex;
			}
			LicenseManager.LicenseState licenseState = lazyAsyncResult.AsyncObject as LicenseManager.LicenseState;
			if (licenseState == null)
			{
				throw new InvalidOperationException("result.AsyncObject cannot be null here.");
			}
			return licenseState.Responses;
		}

		protected abstract void IssueNewWebRequest(LazyAsyncResult asyncResult);

		protected abstract void AcquireLicenseCallback(IAsyncResult asyncResult);

		private DisposeTracker disposeTracker;

		protected IWSManagerPerfCounters perfcounters;

		protected IRmsLatencyTracker rmsLatencyTracker;

		protected class LicenseState
		{
			internal LicenseState(XmlNode[] issuanceLicense, LicenseIdentity[] identities)
			{
				this.identities = identities;
				this.total = identities.Length;
				this.Responses = new LicenseResponse[this.total];
				this.IssuanceLicense = issuanceLicense;
			}

			internal LicenseResponse[] Responses { get; private set; }

			internal RightsManagementException Exception { get; set; }

			internal XmlNode[] IssuanceLicense { get; private set; }

			internal LicenseIdentity[] GetIdentitiesForNextBatch()
			{
				if (this.Exception != null)
				{
					return null;
				}
				this.SelectIdentitiesForNextBatch();
				if (this.identitiesSubset == null)
				{
					return null;
				}
				return this.identitiesSubset;
			}

			internal void SetFailureForCurrentBatch(RightsManagementException failure)
			{
				int currentBatchLength = this.GetCurrentBatchLength();
				ExTraceGlobals.RightsManagementTracer.TraceError<RightsManagementException, int>((long)this.GetHashCode(), "Setting failure {0} for the current batch. Batch length {1}", failure, currentBatchLength);
				if (currentBatchLength > 0)
				{
					int i = 0;
					int num = this.completed;
					while (i < currentBatchLength)
					{
						this.Responses[num] = new LicenseResponse(failure);
						i++;
						num++;
					}
					return;
				}
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Length ({0}) has to be more than 0.", new object[]
				{
					currentBatchLength
				}));
			}

			internal void SetFailureForCurrentBatchElement(int elementIndex, RightsManagementException failure)
			{
				int currentBatchLength = this.GetCurrentBatchLength();
				int num = this.completed + elementIndex;
				ExTraceGlobals.RightsManagementTracer.TraceError<RightsManagementException, int>((long)this.GetHashCode(), "Setting failure {0} for the batch element {1}", failure, num);
				if (currentBatchLength > 0 && num < this.total)
				{
					this.Responses[num] = new LicenseResponse(failure);
					return;
				}
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Index ({0}) has to be less than total count ({1}) and length ({2}) has to be more than 0.", new object[]
				{
					elementIndex,
					this.total,
					currentBatchLength
				}));
			}

			internal void SetResponseForCurrentBatchElement(int elementIndex, XmlNode[] certChain, XmlNode license)
			{
				int currentBatchLength = this.GetCurrentBatchLength();
				int num = this.completed + elementIndex;
				ExTraceGlobals.RightsManagementTracer.TraceDebug<int>((long)this.GetHashCode(), "Setting successful license for the batch element {0}", num);
				if (currentBatchLength > 0 && num < this.total)
				{
					if (string.IsNullOrEmpty(this.commonCertChain) && certChain != null && certChain.Length > 0)
					{
						if (certChain.Length > 1)
						{
							this.commonCertChain = RMUtil.ConvertXmlNodeArrayToString(certChain);
						}
						else
						{
							this.commonCertChain = certChain[0].OuterXml;
						}
					}
					string text = string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[]
					{
						license.OuterXml,
						this.commonCertChain
					});
					ContentRight? usageRights;
					try
					{
						usageRights = new ContentRight?(DrmClientUtils.GetUsageRightsFromLicense(text));
					}
					catch (RightsManagementException arg)
					{
						ExTraceGlobals.RightsManagementTracer.TraceError<int, RightsManagementException>((long)this.GetHashCode(), "Failed to get usage rights from license for recipient index {0}. Error {1}", num, arg);
						usageRights = null;
					}
					this.Responses[num] = new LicenseResponse(text, usageRights);
					return;
				}
				throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Index ({0}) has to be less than total count ({1}) and length ({2}) has to be more than 0.", new object[]
				{
					elementIndex,
					this.total,
					currentBatchLength
				}));
			}

			internal int GetCurrentBatchLength()
			{
				if (this.identitiesSubset == null)
				{
					return 0;
				}
				return this.identitiesSubset.Length;
			}

			protected void SelectIdentitiesForNextBatch()
			{
				this.completed += this.GetCurrentBatchLength();
				int num = this.total - this.completed;
				if (num < 1)
				{
					this.identitiesSubset = null;
					return;
				}
				int num2 = RmsAppSettings.Instance.AcquireLicenseBatchSize;
				num2 = ((num > num2) ? num2 : num);
				if (this.identitiesSubset == null || this.identitiesSubset.Length != num2)
				{
					this.identitiesSubset = new LicenseIdentity[num2];
				}
				Array.Copy(this.identities, this.completed, this.identitiesSubset, 0, num2);
			}

			private readonly int total;

			private int completed;

			private LicenseIdentity[] identities;

			private LicenseIdentity[] identitiesSubset;

			private string commonCertChain;
		}
	}
}
