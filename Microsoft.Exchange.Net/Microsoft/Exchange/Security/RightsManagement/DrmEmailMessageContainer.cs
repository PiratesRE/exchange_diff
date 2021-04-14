using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Security.RightsManagement.StructuredStorage;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal sealed class DrmEmailMessageContainer : EncryptedEmailMessageContainer<DrmEmailMessage>
	{
		public DrmEmailMessageContainer()
		{
			DrmEmailMessageContainer.Tracer.TraceDebug((long)this.GetHashCode(), "Creating DrmEmailMessageContainer to load encrypted message");
		}

		public DrmEmailMessageContainer(string publishLicense, DrmEmailMessage emailMessage) : base(emailMessage)
		{
			DrmEmailMessageContainer.Tracer.TraceDebug((long)this.GetHashCode(), "Creating DrmEmailMessageContainer to save encrypted message");
			if (publishLicense == null)
			{
				throw new ArgumentNullException("publishLicense");
			}
			this.publishLicense = publishLicense;
		}

		public string PublishLicense
		{
			get
			{
				return this.publishLicense;
			}
		}

		protected override string EncryptedEmailMessageStreamName
		{
			get
			{
				return "\tDRMContent";
			}
		}

		protected override void ReadBinding(IStorage rootStorage)
		{
			DrmEmailMessageContainer.Tracer.TraceDebug((long)this.GetHashCode(), "Reading the publish license from RMS message");
			this.publishLicense = DrmDataspaces.Read(rootStorage);
			DrmEmailMessageContainer.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Read the publish license from RMS message: {0}", this.publishLicense);
		}

		protected override void WriteBinding(IStorage rootStorage)
		{
			DrmEmailMessageContainer.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Writing the publish license to RMS message: {0}", this.publishLicense);
			DrmDataspaces.Write(rootStorage, this.publishLicense);
			DrmEmailMessageContainer.Tracer.TraceDebug((long)this.GetHashCode(), "Wrote the publish license to RMS message");
		}

		private static readonly Trace Tracer = ExTraceGlobals.RightsManagementTracer;

		private string publishLicense;
	}
}
