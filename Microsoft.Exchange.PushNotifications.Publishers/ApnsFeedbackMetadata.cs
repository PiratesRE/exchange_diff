using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ApnsFeedbackMetadata : ApnsFeedbackFileBase
	{
		internal ApnsFeedbackMetadata(ApnsFeedbackFileId identifier, ApnsFeedbackFileIO fileIO) : this(identifier, fileIO, ExTraceGlobals.PublisherManagerTracer)
		{
		}

		internal ApnsFeedbackMetadata(ApnsFeedbackFileId identifier, ApnsFeedbackFileIO fileIO, ITracer tracer) : base(identifier, fileIO, tracer)
		{
		}

		public override bool IsLoaded
		{
			get
			{
				return this.isLoaded;
			}
		}

		public override void Load()
		{
			base.Tracer.TraceDebug<ApnsFeedbackFileId>((long)this.GetHashCode(), "[Load] Loading APNs Feedback Metadata from '{0}'", base.Identifier);
			this.isLoaded = true;
		}

		private bool isLoaded;
	}
}
