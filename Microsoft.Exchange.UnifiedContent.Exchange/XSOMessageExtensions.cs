using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.UnifiedContent.Exchange
{
	internal static class XSOMessageExtensions
	{
		internal static void Serialize(this Item item, IMapiFilteringContext context, UnifiedContentSerializer serializer, bool bypassTextTruncation = true)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			if (item.Body.Size > 0L && context.NeedsClassificationScan())
			{
				serializer.WriteProperty(UnifiedContentSerializer.PropertyId.Subject, item.GetValueOrDefault<string>(InternalSchema.Subject, string.Empty));
				StorePropertyDefinition bodyFormat = XSOMessageExtensions.GetBodyFormat(item.Body.Format);
				if (bodyFormat != null)
				{
					using (Stream stream = item.OpenPropertyStream(bodyFormat, PropertyOpenMode.ReadOnly))
					{
						using (Stream bodyStream = XSOMessageExtensions.GetBodyStream(stream))
						{
							SharedContent sharedContent = XSOMessageExtensions.WriteBody(serializer, bodyStream);
							if (sharedContent != null)
							{
								sharedContent.Properties["Parsing::ParsingKeys::PreferredBody"] = true;
								sharedContent.Properties["Parsing::ParsingKeys::ContentType"] = XSOMessageExtensions.GetBodyContentType(item.Body);
								sharedContent.Properties["Parsing::ParsingKeys::CharSet"] = item.Body.RawCharset.Name;
								if (bypassTextTruncation)
								{
									sharedContent.Properties["Parsing::ConfigKeys::BypassTextTruncation"] = true;
								}
							}
						}
					}
				}
			}
			IList<AttachmentHandle> allHandles = item.AttachmentCollection.GetAllHandles();
			foreach (AttachmentHandle handle in allHandles)
			{
				using (Attachment attachment = item.AttachmentCollection.Open(handle))
				{
					XSOMessageExtensions.Serialize(attachment, context, serializer);
				}
			}
		}

		private static void Serialize(Attachment attachment, IMapiFilteringContext context, UnifiedContentSerializer serializer)
		{
			if (attachment == null)
			{
				throw new ArgumentNullException("attachment");
			}
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			if (attachment.Size > 0L && context.NeedsClassificationScan(attachment))
			{
				using (Stream stream = attachment.PropertyBag.OpenPropertyStream(AttachmentSchema.AttachDataBin, PropertyOpenMode.ReadOnly))
				{
					serializer.AddStream(UnifiedContentSerializer.EntryId.Attachment, stream, string.Format("{0}:{1}", attachment.FileName, attachment.Id.ToBase64String()));
				}
			}
		}

		private static string GetBodyContentType(Body body)
		{
			if (body == null)
			{
				throw new ArgumentNullException("body");
			}
			string result = string.Empty;
			switch (body.Format)
			{
			case BodyFormat.TextPlain:
				result = "text/plaintext";
				break;
			case BodyFormat.TextHtml:
				result = "text/html";
				break;
			case BodyFormat.ApplicationRtf:
				result = "text/rtf";
				break;
			}
			return result;
		}

		private static StorePropertyDefinition GetBodyFormat(BodyFormat bodyFormat)
		{
			StorePropertyDefinition result = null;
			switch (bodyFormat)
			{
			case BodyFormat.TextPlain:
				result = ItemSchema.TextBody;
				break;
			case BodyFormat.TextHtml:
				result = ItemSchema.HtmlBody;
				break;
			case BodyFormat.ApplicationRtf:
				result = ItemSchema.RtfBody;
				break;
			}
			return result;
		}

		private static Stream GetBodyStream(Stream bodyStream)
		{
			if (bodyStream != null)
			{
				try
				{
					long length = bodyStream.Length;
				}
				catch (NotSupportedException)
				{
					Stream stream = new MemoryStream();
					bodyStream.CopyTo(stream);
					bodyStream.Dispose();
					bodyStream = stream;
				}
			}
			return bodyStream;
		}

		private static SharedContent WriteBody(UnifiedContentSerializer serializer, Stream bodyStream)
		{
			if (serializer == null)
			{
				throw new ArgumentNullException("serializer");
			}
			SharedContent sharedContent = null;
			if (bodyStream != null)
			{
				sharedContent = serializer.AddStream(UnifiedContentSerializer.EntryId.Body, bodyStream, "Message Body");
				if (sharedContent != null)
				{
					sharedContent.Properties["Parsing::ParsingKeys::MessageBody"] = true;
				}
			}
			return sharedContent;
		}

		public const string BypassTextTruncationPropertyKey = "Parsing::ConfigKeys::BypassTextTruncation";

		public const string MessageBodyFileName = "Message Body";

		internal enum EntryId
		{
			PpeHeader,
			Property,
			Body,
			Attachment
		}

		internal enum PropertyId
		{
			Subject = 1
		}
	}
}
