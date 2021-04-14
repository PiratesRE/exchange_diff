using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.OOF;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UserOofSettingsType
	{
		public UserOofSettingsType()
		{
		}

		public UserOofSettingsType(UserOofSettings userOofSettings, ExTimeZone timeZone)
		{
			this.ExternalAudience = userOofSettings.ExternalAudience;
			this.ExternalReply = UserOofSettingsType.ConvertHtmlToText(userOofSettings.ExternalReply.Message);
			this.InternalReply = UserOofSettingsType.ConvertHtmlToText(userOofSettings.InternalReply.Message);
			this.IsScheduled = (userOofSettings.OofState == OofState.Scheduled);
			if (userOofSettings.OofState == OofState.Enabled || (userOofSettings.OofState == OofState.Scheduled && DateTime.UtcNow >= userOofSettings.Duration.StartTime && DateTime.UtcNow <= userOofSettings.Duration.EndTime))
			{
				this.IsOofOn = true;
			}
			else
			{
				this.IsOofOn = false;
			}
			ExDateTime exDateTime = new ExDateTime(timeZone, userOofSettings.Duration.EndTime);
			this.EndTime = exDateTime.ToISOString();
		}

		[DataMember(Order = 1)]
		public ExternalAudience ExternalAudience { get; set; }

		[DataMember(Order = 2)]
		public string ExternalReply { get; set; }

		[DataMember(Order = 3)]
		public string InternalReply { get; set; }

		[DataMember(Order = 4)]
		public bool IsOofOn { get; set; }

		[DataMember(Order = 5)]
		public bool IsScheduled { get; set; }

		[DataMember(Order = 6)]
		[DateTimeString]
		public string EndTime { get; set; }

		internal static string ConvertTextToHtml(string text)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(text);
			string @string;
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				memoryStream.Seek(0L, SeekOrigin.Begin);
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					try
					{
						new TextToHtml
						{
							InputEncoding = Encoding.UTF8,
							OutputEncoding = Encoding.UTF8,
							HtmlTagCallback = new HtmlTagCallback(UserOofSettingsType.RemoveLinkCallback),
							Header = null,
							Footer = null,
							OutputHtmlFragment = true
						}.Convert(memoryStream, memoryStream2);
					}
					catch (InvalidCharsetException innerException)
					{
						throw new OwaException("Convert to Html Failed", innerException);
					}
					catch (TextConvertersException innerException2)
					{
						throw new OwaException("Convert to Html Failed", innerException2);
					}
					@string = Encoding.UTF8.GetString(memoryStream2.ToArray());
				}
			}
			return @string;
		}

		private static string ConvertHtmlToText(string html)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(html);
			string @string;
			using (MemoryStream memoryStream = new MemoryStream(bytes))
			{
				memoryStream.Seek(0L, SeekOrigin.Begin);
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					try
					{
						new HtmlToText
						{
							InputEncoding = Encoding.UTF8,
							OutputEncoding = Encoding.UTF8
						}.Convert(memoryStream, memoryStream2);
					}
					catch (InvalidCharsetException innerException)
					{
						throw new OwaException("Convert to Text Failed", innerException);
					}
					catch (TextConvertersException innerException2)
					{
						throw new OwaException("Convert to Text Failed", innerException2);
					}
					@string = Encoding.UTF8.GetString(memoryStream2.ToArray());
				}
			}
			return @string;
		}

		private static void RemoveLinkCallback(HtmlTagContext tagContext, HtmlWriter htmlWriter)
		{
			if (tagContext.TagId == HtmlTagId.A)
			{
				tagContext.DeleteTag();
				return;
			}
			tagContext.WriteTag();
			foreach (HtmlTagContextAttribute htmlTagContextAttribute in tagContext.Attributes)
			{
				htmlTagContextAttribute.Write();
			}
		}
	}
}
