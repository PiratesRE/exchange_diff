using System;
using System.IO;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ADPhotoWriter : IADPhotoWriter
	{
		public ADPhotoWriter(IRecipientSession session, ITracer upstreamTracer)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (upstreamTracer == null)
			{
				throw new ArgumentNullException("upstreamTracer");
			}
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
			this.session = session;
		}

		public void Write(ADObjectId recipientId, Stream photo)
		{
			if (recipientId == null)
			{
				throw new ArgumentNullException("recipientId");
			}
			if (photo == null || photo.Length == 0L)
			{
				this.Clear(recipientId);
				return;
			}
			this.WritePhoto(recipientId, photo);
		}

		private void WritePhoto(ADObjectId recipientId, Stream photo)
		{
			ADRecipient adrecipient = this.FindRecipient(recipientId);
			if (adrecipient == null)
			{
				this.tracer.TraceError<ADObjectId>((long)this.GetHashCode(), "AD photo writer: user {0} not found in AD.", recipientId);
				throw new ADNoSuchObjectException(DirectoryStrings.ExceptionADOperationFailedNoSuchObject(this.session.DomainController, recipientId.DistinguishedName));
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				photo.CopyTo(memoryStream);
				adrecipient.ThumbnailPhoto = memoryStream.ToArray();
				this.tracer.TraceDebug<long, ADRecipient>((long)this.GetHashCode(), "AD photo writer: saving photo with length {0} bytes to AD user {1}", memoryStream.Length, adrecipient);
				this.session.Save(adrecipient);
			}
		}

		private void Clear(ADObjectId recipientId)
		{
			ADRecipient adrecipient = this.FindRecipient(recipientId);
			if (adrecipient == null)
			{
				this.tracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "AD photo writer: request to clear photo of user {0} ignored because user could not be found in AD.", recipientId);
				return;
			}
			adrecipient.ThumbnailPhoto = null;
			this.tracer.TraceDebug<ADRecipient>((long)this.GetHashCode(), "AD photo writer: clearing photo of AD user {0}", adrecipient);
			this.session.Save(adrecipient);
		}

		private ADRecipient FindRecipient(ADObjectId recipientId)
		{
			return this.session.Read(recipientId);
		}

		private ITracer tracer = ExTraceGlobals.UserPhotosTracer;

		private readonly IRecipientSession session;
	}
}
