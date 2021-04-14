using System;
using System.IO;
using Microsoft.Exchange.Common.Sniff;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ADPhotoReader : IADPhotoReader
	{
		public ADPhotoReader(ITracer upstreamTracer)
		{
			if (upstreamTracer == null)
			{
				throw new ArgumentNullException("upstreamTracer");
			}
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
		}

		public PhotoMetadata Read(IRecipientSession session, ADObjectId recipientId, Stream output)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (recipientId == null)
			{
				throw new ArgumentNullException("recipientId");
			}
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			ADRecipient adrecipient = this.FindRecipient(session, recipientId);
			if (adrecipient.ThumbnailPhoto == null || adrecipient.ThumbnailPhoto.Length == 0)
			{
				this.tracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "AD photo reader: user {0} does NOT have a photo in AD.", recipientId);
				throw new ADNoSuchObjectException(Strings.ADUserNoPhoto(recipientId));
			}
			output.Write(adrecipient.ThumbnailPhoto, 0, adrecipient.ThumbnailPhoto.Length);
			string contentType;
			using (MemoryStream memoryStream = new MemoryStream(adrecipient.ThumbnailPhoto))
			{
				contentType = this.sniffer.FindMimeFromData(memoryStream);
			}
			return new PhotoMetadata
			{
				Length = (long)adrecipient.ThumbnailPhoto.Length,
				ContentType = contentType
			};
		}

		private ADRecipient FindRecipient(IRecipientSession session, ADObjectId recipientId)
		{
			ADRecipient adrecipient = session.Read(recipientId);
			if (adrecipient == null)
			{
				this.tracer.TraceError<ADObjectId>((long)this.GetHashCode(), "AD photo reader: user {0} not found in AD.", recipientId);
				throw new ADNoSuchObjectException(DirectoryStrings.ExceptionADOperationFailedNoSuchObject(session.DomainController, recipientId.DistinguishedName));
			}
			return adrecipient;
		}

		private const int SnifferSampleSize = 128;

		private DataSniff sniffer = new DataSniff(128);

		private ITracer tracer = ExTraceGlobals.UserPhotosTracer;
	}
}
