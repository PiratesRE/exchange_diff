using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Entities;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Entities.TypeConversion.Converters
{
	internal static class BodyConverter
	{
		internal static ItemBody GetEntityBody(this IItem input, char[] buffer)
		{
			Body body;
			BodyType bodyType;
			try
			{
				body = IrmUtils.GetBody(input);
				BodyFormat format = body.Format;
				bodyType = format.ToEntityType();
			}
			catch (PropertyErrorException ex)
			{
				ExTraceGlobals.CommonTracer.TraceDebug<string, string>(0L, "[BodyConverter::GetEntityBody] Encountered exception - Class: {0}; Message: {1}", ex.GetType().FullName, ex.Message);
				throw new CorruptDataException(Strings.ErrorItemCorrupt, ex);
			}
			catch (StoragePermanentException ex2)
			{
				if (ex2.InnerException is MapiExceptionNoSupport)
				{
					throw new CorruptDataException(Strings.ErrorItemCorrupt, ex2);
				}
				ExTraceGlobals.CommonTracer.TraceDebug(0L, "[BodyConverter::GetEntityBody] Encountered exception - Class: {0}, Message: {1} Inner exception was not MapiExceptionNoSupport but rather Class: {2}; Message: {3}", new object[]
				{
					ex2.GetType().FullName,
					ex2.Message,
					(ex2.InnerException == null) ? "<NULL>" : ex2.InnerException.GetType().FullName,
					(ex2.InnerException == null) ? "<NULL>" : ex2.InnerException.Message
				});
				throw;
			}
			ItemBody itemBody = new ItemBody
			{
				ContentType = bodyType
			};
			using (TextWriter textWriter = new StringWriter())
			{
				if (bodyType == BodyType.Html)
				{
					BodyConverter.WriteHtmlContent(textWriter, input, buffer);
				}
				else
				{
					BodyConverter.WriteTextContent(textWriter, body, buffer);
				}
				itemBody.Content = textWriter.ToString();
			}
			return itemBody;
		}

		internal static void SetOnStorageItem(this ItemBody entityBody, IItem storageItem, bool update)
		{
			string value = entityBody.Content ?? string.Empty;
			HtmlUpdateBodyCallback htmlUpdateBodyCallback;
			using (TextWriter textWriter = BodyConverter.CreateTextWriter(storageItem, entityBody.ContentType, update, out htmlUpdateBodyCallback))
			{
				textWriter.Write(value);
			}
			if (htmlUpdateBodyCallback != null)
			{
				htmlUpdateBodyCallback.SaveChanges();
			}
		}

		internal static BodyType ToEntityType(this BodyFormat input)
		{
			switch (input)
			{
			case BodyFormat.TextPlain:
				return BodyType.Text;
			case BodyFormat.TextHtml:
			case BodyFormat.ApplicationRtf:
				return BodyType.Html;
			default:
				throw new ArgumentOutOfRangeException("input");
			}
		}

		internal static BodyFormat ToStorageType(this BodyType input)
		{
			switch (input)
			{
			case BodyType.Text:
				return BodyFormat.TextPlain;
			case BodyType.Html:
				return BodyFormat.TextHtml;
			default:
				throw new ArgumentOutOfRangeException("input");
			}
		}

		private static void CopyContent(TextReader reader, TextWriter writer, char[] buffer)
		{
			bool flag = false;
			int index = 0;
			int num = buffer.Length;
			int num2;
			while ((num2 = reader.Read(buffer, index, num)) > 0)
			{
				bool flag2 = false;
				if (num2 == num)
				{
					flag2 = char.IsHighSurrogate(buffer[buffer.Length - 1]);
				}
				if (flag)
				{
					num2++;
				}
				if (flag2)
				{
					BodyConverter.WriteChars(writer, buffer, num2 - 1);
					flag = true;
					buffer[0] = buffer[buffer.Length - 1];
					index = 1;
					num = buffer.Length - 1;
				}
				else
				{
					BodyConverter.WriteChars(writer, buffer, num2);
					flag = false;
					index = 0;
					num = buffer.Length;
				}
			}
		}

		private static TextWriter CreateTextWriter(IItem item, BodyType type, bool update, out HtmlUpdateBodyCallback htmlUpdateBodyCallback)
		{
			Body body = IrmUtils.GetBody(item);
			BodyWriteConfiguration bodyWriteConfiguration = new BodyWriteConfiguration(type.ToStorageType());
			if (update && type == BodyType.Html)
			{
				htmlUpdateBodyCallback = new HtmlUpdateBodyCallback(item);
				bodyWriteConfiguration.SetHtmlOptions(HtmlStreamingFlags.None, htmlUpdateBodyCallback);
				return body.OpenTextWriter(bodyWriteConfiguration);
			}
			htmlUpdateBodyCallback = null;
			return body.OpenTextWriter(bodyWriteConfiguration);
		}

		private static void WriteChars(TextWriter writer, char[] copyBuffer, int bytesRead)
		{
			try
			{
				writer.Write(copyBuffer, 0, bytesRead);
			}
			catch (ArgumentException ex)
			{
				ex.Data["NeverGenerateWatson"] = null;
				throw;
			}
		}

		private static void WriteHtmlContent(TextWriter writer, IItem item, char[] charBuffer)
		{
			BodyReadConfiguration bodyReadConfiguration = new BodyReadConfiguration(BodyFormat.TextHtml, "utf-8");
			bodyReadConfiguration.HtmlFlags &= ~HtmlStreamingFlags.FilterHtml;
			item.Load(StoreObjectSchema.ContentConversionProperties);
			bodyReadConfiguration.ConversionCallback = new DefaultHtmlCallbacks(item, true);
			Body body = IrmUtils.GetBody(item);
			using (Stream stream = body.OpenReadStream(bodyReadConfiguration))
			{
				using (StreamReader streamReader = new StreamReader(stream, Encoding.UTF8))
				{
					BodyConverter.CopyContent(streamReader, writer, charBuffer);
				}
			}
		}

		private static void WriteTextContent(TextWriter writer, Body body, char[] charBuffer)
		{
			using (TextReader textReader = body.OpenTextReader(BodyFormat.TextPlain))
			{
				BodyConverter.CopyContent(textReader, writer, charBuffer);
			}
		}

		private const string DefaultCharacterSet = "utf-8";
	}
}
