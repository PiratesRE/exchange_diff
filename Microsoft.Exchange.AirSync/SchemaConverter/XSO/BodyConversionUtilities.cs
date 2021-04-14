using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.AirSync.SchemaConverter.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics.Components.SchemaConverter;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.AirSync.SchemaConverter.XSO
{
	internal static class BodyConversionUtilities
	{
		public static Stream ConvertToBodyStream(Item item, long truncationSize, out long totalDataSize, out IList<AttachmentLink> attachmentLinks)
		{
			Microsoft.Exchange.Data.Storage.BodyFormat format = item.Body.Format;
			switch (format)
			{
			case Microsoft.Exchange.Data.Storage.BodyFormat.TextPlain:
				return BodyConversionUtilities.ConvertToPlainTextStream(item, truncationSize, out totalDataSize, out attachmentLinks);
			case Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml:
				return BodyConversionUtilities.ConvertHtmlStream(item, truncationSize, out totalDataSize, out attachmentLinks);
			case Microsoft.Exchange.Data.Storage.BodyFormat.ApplicationRtf:
				return BodyConversionUtilities.ConvertToRtfStream(item, truncationSize, out totalDataSize, out attachmentLinks);
			default:
				throw new ConversionException("Unsupported bodyFormat for this function: " + format);
			}
		}

		public static Stream ConvertToPlainTextStream(Item item, long truncationSizeByChars, out long totalDataSize, out IList<AttachmentLink> attachmentLinks)
		{
			BodyReadConfiguration configuration = new BodyReadConfiguration(Microsoft.Exchange.Data.Storage.BodyFormat.TextPlain, "utf-8");
			AirSyncStream airSyncStream = new AirSyncStream();
			int num = 0;
			Body body = null;
			if (BodyConversionUtilities.IsMessageRestrictedAndDecoded(item))
			{
				body = ((RightsManagedMessageItem)item).ProtectedBody;
			}
			else
			{
				body = item.Body;
			}
			uint streamHash;
			using (Stream stream = body.OpenReadStream(configuration))
			{
				num = StreamHelper.CopyStream(stream, airSyncStream, Encoding.UTF8, (int)truncationSizeByChars, true, out streamHash);
			}
			airSyncStream.StreamHash = (int)streamHash;
			totalDataSize = ((truncationSizeByChars < 0L || (long)num < truncationSizeByChars) ? ((long)num) : body.Size);
			attachmentLinks = null;
			return airSyncStream;
		}

		public static Stream ConvertHtmlStream(Item item, long truncationSizeByChars, out long totalDataSize, out IList<AttachmentLink> attachmentLinks)
		{
			SafeHtmlCallback safeHtmlCallback = new SafeHtmlCallback(item);
			bool flag = truncationSizeByChars == -1L;
			BodyReadConfiguration bodyReadConfiguration;
			if (flag)
			{
				bodyReadConfiguration = new BodyReadConfiguration(Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml, "utf-8");
				bodyReadConfiguration.SetHtmlOptions(HtmlStreamingFlags.FilterHtml, safeHtmlCallback);
			}
			else
			{
				bodyReadConfiguration = new BodyReadConfiguration(Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml, "utf-8");
				bodyReadConfiguration.SetHtmlOptions(HtmlStreamingFlags.FilterHtml, safeHtmlCallback, new int?(1024));
			}
			AirSyncStream airSyncStream = new AirSyncStream();
			Body body = null;
			if (BodyConversionUtilities.IsMessageRestrictedAndDecoded(item))
			{
				body = ((RightsManagedMessageItem)item).ProtectedBody;
			}
			else
			{
				body = item.Body;
			}
			uint streamHash;
			using (Stream stream = body.OpenReadStream(bodyReadConfiguration))
			{
				StreamHelper.CopyStream(stream, airSyncStream, Encoding.UTF8, (int)truncationSizeByChars, true, out streamHash);
			}
			airSyncStream.StreamHash = (int)streamHash;
			totalDataSize = ((truncationSizeByChars < 0L || airSyncStream.Length < truncationSizeByChars) ? airSyncStream.Length : body.Size);
			attachmentLinks = safeHtmlCallback.AttachmentLinks;
			return airSyncStream;
		}

		public static Stream ConvertToRtfStream(Item item, long truncationSizeByBytes, out long totalDataSize, out IList<AttachmentLink> attachmentLinks)
		{
			BodyReadConfiguration configuration = new BodyReadConfiguration(Microsoft.Exchange.Data.Storage.BodyFormat.ApplicationRtf, "utf-8");
			AirSyncStream airSyncStream = new AirSyncStream();
			Body body = null;
			if (BodyConversionUtilities.IsMessageRestrictedAndDecoded(item))
			{
				body = ((RightsManagedMessageItem)item).ProtectedBody;
			}
			else
			{
				body = item.Body;
			}
			uint streamHash;
			using (Stream stream = body.OpenReadStream(configuration))
			{
				airSyncStream.DoBase64Conversion = true;
				StreamHelper.CopyStream(stream, airSyncStream, (int)truncationSizeByBytes, out streamHash);
			}
			airSyncStream.StreamHash = (int)streamHash;
			totalDataSize = ((truncationSizeByBytes < 0L || airSyncStream.Length < truncationSizeByBytes) ? airSyncStream.Length : body.Size);
			attachmentLinks = null;
			return airSyncStream;
		}

		internal static void CopyBody(Item sourceItem, Item targetItem)
		{
			using (Stream stream = sourceItem.Body.OpenReadStream(new BodyReadConfiguration(sourceItem.Body.Format, sourceItem.Body.Charset)))
			{
				using (Stream stream2 = targetItem.Body.OpenWriteStream(new BodyWriteConfiguration(sourceItem.Body.Format, sourceItem.Body.Charset)))
				{
					StreamHelper.CopyStream(stream, stream2);
				}
			}
		}

		internal static void OnBeforeItemLoadInConversation(object sender, LoadItemEventArgs e)
		{
			e.HtmlStreamOptionCallback = new HtmlStreamOptionCallback(BodyConversionUtilities.GetSafeHtmlCallbacks);
		}

		internal static void OnBeforeItemLoadInConversationForceOpen(object sender, LoadItemEventArgs e)
		{
			e.HtmlStreamOptionCallback = new HtmlStreamOptionCallback(BodyConversionUtilities.GetSafeHtmlCallbacks);
			e.MessagePropertyDefinitions = new PropertyDefinition[]
			{
				ItemSchema.Importance
			};
		}

		private static KeyValuePair<HtmlStreamingFlags, HtmlCallbackBase> GetSafeHtmlCallbacks(Item item)
		{
			SafeHtmlCallback value = new SafeHtmlCallback(item);
			return new KeyValuePair<HtmlStreamingFlags, HtmlCallbackBase>(HtmlStreamingFlags.FilterHtml, value);
		}

		internal static bool IsMessageRestrictedAndDecoded(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			RightsManagedMessageItem rightsManagedMessageItem = item as RightsManagedMessageItem;
			if (rightsManagedMessageItem != null && rightsManagedMessageItem.IsRestricted && rightsManagedMessageItem.IsDecoded)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.XsoTracer, item, "The message is restricted and decoded");
				return true;
			}
			return false;
		}

		internal static bool IsIRMFailedToDecode(Item item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			RightsManagedMessageItem rightsManagedMessageItem = item as RightsManagedMessageItem;
			if (rightsManagedMessageItem != null && rightsManagedMessageItem.DecryptionStatus.Failed && rightsManagedMessageItem.DecryptionStatus.FailureCode != RightsManagementFailureCode.InternalLicensingDisabled && rightsManagedMessageItem.DecryptionStatus.FailureCode != RightsManagementFailureCode.ExternalLicensingDisabled)
			{
				AirSyncDiagnostics.TraceDebug(ExTraceGlobals.XsoTracer, item, "The message is failed to decode");
				return true;
			}
			return false;
		}

		public const string OutputDisplayCharset = "utf-8";

		private const int CSSTruncationLimit = 1024;
	}
}
