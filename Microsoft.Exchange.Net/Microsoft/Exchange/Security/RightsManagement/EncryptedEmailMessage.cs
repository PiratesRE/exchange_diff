using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Security.RightsManagement.StructuredStorage;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal abstract class EncryptedEmailMessage
	{
		public EncryptedEmailMessage()
		{
			EncryptedEmailMessage.Tracer.TraceDebug((long)this.GetHashCode(), "Creating EncryptedEmailMessage for load");
		}

		public EncryptedEmailMessage(Stream bodyStream)
		{
			if (bodyStream == null)
			{
				throw new ArgumentNullException("bodyStream");
			}
			EncryptedEmailMessage.Tracer.TraceDebug((long)this.GetHashCode(), "Creating EncryptedEmailMessage to save");
			this.bodyStream = bodyStream;
		}

		public Stream BodyStream
		{
			get
			{
				return this.bodyStream;
			}
			protected set
			{
				if (this.bodyStream != null)
				{
					this.bodyStream.Close();
				}
				this.bodyStream = value;
			}
		}

		public abstract void Load(IStorage rootStorage, CreateStreamCallbackDelegate createBodyStreamCallback, CreateStreamCallbackDelegate createAttachmentStreamCallback);

		public abstract void Save(IStorage rootStorage, EncryptedEmailMessageBinding messageBinding);

		public virtual void Close()
		{
			if (this.bodyStream != null)
			{
				this.bodyStream.Close();
				this.bodyStream = null;
			}
			EncryptedEmailMessage.Tracer.TraceDebug((long)this.GetHashCode(), "Disposed EncryptedEmailMessage");
		}

		private static readonly Trace Tracer = ExTraceGlobals.RightsManagementTracer;

		private Stream bodyStream;
	}
}
