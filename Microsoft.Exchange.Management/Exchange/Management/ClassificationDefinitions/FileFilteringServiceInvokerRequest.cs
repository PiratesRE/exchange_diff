using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.UnifiedContent;
using Microsoft.Filtering;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal sealed class FileFilteringServiceInvokerRequest : FilteringServiceInvokerRequest, IDisposeTrackable, IDisposable
	{
		private FileFilteringServiceInvokerRequest(string organizationId, TimeSpan scanTimeout, int textScanLimit, FileFipsDataStreamFilteringRequest fileFipsDataStreamFilteringRequest) : base(organizationId, scanTimeout, textScanLimit, fileFipsDataStreamFilteringRequest)
		{
			this.contentManager = fileFipsDataStreamFilteringRequest.ContentManager;
			this.disposeTracker = this.GetDisposeTracker();
		}

		public static FileFilteringServiceInvokerRequest CreateInstance(string fileName, Stream fileStream, int textScanLimit, string organizationId)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("fileName", fileName);
			ArgumentValidator.ThrowIfInvalidValue<Stream>("fileStream", fileStream, (Stream stream) => stream != null || stream.Length > 0L);
			ArgumentValidator.ThrowIfNullOrEmpty("organizationId", organizationId);
			TimeSpan scanTimeout = FileFilteringServiceInvokerRequest.GetScanTimeout(fileStream);
			ContentManager contentManager = new ContentManager(Path.GetTempPath());
			FileFipsDataStreamFilteringRequest fileFipsDataStreamFilteringRequest = FileFipsDataStreamFilteringRequest.CreateInstance(fileName, fileStream, contentManager);
			return new FileFilteringServiceInvokerRequest(organizationId, scanTimeout, textScanLimit, fileFipsDataStreamFilteringRequest);
		}

		internal static TimeSpan GetScanTimeout(Stream fileStream)
		{
			return FileFilteringServiceInvokerRequest.rulesScanTimeout.GetTimeout(fileStream, null);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<FileFilteringServiceInvokerRequest>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
				this.disposeTracker = null;
			}
			if (this.contentManager != null)
			{
				this.contentManager.Dispose();
				this.contentManager = null;
			}
			GC.SuppressFinalize(this);
		}

		private DisposeTracker disposeTracker;

		private ContentManager contentManager;

		private static readonly RulesScanTimeout rulesScanTimeout = new RulesScanTimeout(Components.TransportAppConfig.TransportRuleConfig.ScanVelocities, Components.TransportAppConfig.TransportRuleConfig.TransportRuleMinFipsTimeoutInMilliseconds);
	}
}
