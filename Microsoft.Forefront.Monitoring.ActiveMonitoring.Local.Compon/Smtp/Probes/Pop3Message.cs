using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Smtp.Probes
{
	public class Pop3Message
	{
		public Pop3Message()
		{
			this.BodyComponents = new List<string>();
			this.bodyParsed = false;
		}

		public Pop3Message(List<string> message)
		{
			List<string> headerComponents;
			List<string> bodyComponents;
			this.SplitMimeHeaderAndBody(message, out headerComponents, out bodyComponents);
			this.HeaderComponents = headerComponents;
			this.BodyComponents = bodyComponents;
			this.bodyParsed = false;
		}

		public long Number
		{
			get
			{
				return this.number;
			}
			set
			{
				this.number = value;
			}
		}

		public long Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		public string Message
		{
			get
			{
				if (string.IsNullOrEmpty(this.message) && this.Components != null)
				{
					this.message = string.Join(string.Empty, this.Components.ToArray());
				}
				return this.message;
			}
			set
			{
				this.message = value;
			}
		}

		public List<string> Components
		{
			get
			{
				return this.components;
			}
			set
			{
				this.components = value;
			}
		}

		public string MessageHeader
		{
			get
			{
				if (string.IsNullOrEmpty(this.messageHeader) && this.HeaderComponents != null)
				{
					this.messageHeader = string.Join(string.Empty, this.HeaderComponents.ToArray());
				}
				return this.messageHeader;
			}
			set
			{
				this.messageHeader = value;
			}
		}

		public List<string> HeaderComponents { get; set; }

		public List<string> BodyComponents { get; set; }

		public string BodyText
		{
			get
			{
				if (!this.bodyParsed && this.BodyComponents != null)
				{
					this.bodyParsed = this.TryParseBody();
				}
				return this.bodyText;
			}
			set
			{
				this.bodyText = value;
			}
		}

		public List<Pop3Attachment> Attachments
		{
			get
			{
				if (!this.bodyParsed && this.BodyComponents != null)
				{
					this.bodyParsed = this.TryParseBody();
				}
				return this.attachments;
			}
			set
			{
				this.attachments = value;
			}
		}

		public int AttachmentCount { get; set; }

		public DateTime ReceivedDate
		{
			get
			{
				if (this.receivedDate == DateTime.MinValue)
				{
					string value = this.GetValue("Received:");
					if (!string.IsNullOrEmpty(value))
					{
						string pattern = "(\\w\\W)*[a-zA-Z]{3},[\\s]+[0-9]{1,2}[\\s]+[a-zA-Z]{3}[\\s]+[0-9]{4}[\\s]+[0-9]{1,2}:[0-9]{1,2}:[0-9]{1,2}[\\s]+[+-]{1}[0-9]{4}(\\w\\W)*";
						Match match = Regex.Match(value, pattern);
						if (match.Success)
						{
							this.receivedDate = Convert.ToDateTime(match.ToString()).ToUniversalTime();
						}
					}
				}
				return this.receivedDate;
			}
		}

		public string From
		{
			get
			{
				if (string.IsNullOrEmpty(this.from))
				{
					this.from = this.GetValue("From:");
				}
				return this.from;
			}
		}

		public string To
		{
			get
			{
				if (string.IsNullOrEmpty(this.to))
				{
					this.to = this.GetValue("To:");
				}
				return this.to;
			}
		}

		public DateTime Date
		{
			get
			{
				if (this.date == DateTime.MinValue)
				{
					this.date = Convert.ToDateTime(this.GetValue("Date:")).ToUniversalTime();
				}
				return this.date;
			}
		}

		public string Subject
		{
			get
			{
				if (string.IsNullOrEmpty(this.subject))
				{
					this.subject = this.GetValue("Subject:");
				}
				return this.subject;
			}
		}

		private string GetValue(string param)
		{
			if (string.IsNullOrEmpty(param) || !Regex.Match(param, "^(Received:|From:|To:|Date:|Subject:|Content-Type:|FFO-ActiveMonitoring:|Message-ID:|MIME-Version:|X-FOPE-CONNECTOR:|X-IPFilteringStamp:|Return-Path:|X-SpamScore:|X-BigFish:|X-Spam-TCS-SCL:|X-Forefront-Antispam-Report:|X-MS-Exchange-Organization-SCL:|X-MS-Exchange-Organization-AVStamp-Mailbox:|X-MS-Exchange-Organization-AuthSource:|X-MS-Exchange-Organization-AuthAs:|Auto-Submitted:|X-MS-Exchange-Message-Is-Ndr:|X-MS-Exchange-Organization-AuthMechanism:|X-MS-Exchange-Organization-Network-Message-Id:|X-MS-Exchange-Organization-AVStamp-Service:|X-Forefront-Antispam-Report:|X-MS-Exchange-Organization-MessageDirectionality:|Thread-Topic:|Thread-Index:|Content-Language:|X-MS-Has-Attach:|X-MS-TNEF-Correlator:|received-spf:|Content-ID:)").Success)
			{
				return null;
			}
			return this.GetHeaderValue(param, this.HeaderComponents);
		}

		private bool TryParseBody()
		{
			bool result;
			try
			{
				List<Pop3BodyText> list = new List<Pop3BodyText>();
				List<Pop3Attachment> list2 = new List<Pop3Attachment>();
				this.ExtractBodyTextsAndAttachments(this.HeaderComponents, this.BodyComponents, list, list2);
				if (list.Count > 0)
				{
					this.bodyText = this.FindBestBodyText(list);
				}
				this.attachments = list2;
				this.AttachmentCount = list2.Count;
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}

		private string FindBestBodyText(List<Pop3BodyText> bodyTexts)
		{
			if (bodyTexts == null || bodyTexts.Count == 0)
			{
				return null;
			}
			if (bodyTexts.Count == 1)
			{
				return bodyTexts[0].Text;
			}
			string[] array = new string[]
			{
				"text/plain",
				"text/html",
				"text/"
			};
			foreach (string value in array)
			{
				foreach (Pop3BodyText pop3BodyText in bodyTexts)
				{
					if (!string.IsNullOrEmpty(pop3BodyText.ContentType) && pop3BodyText.ContentType.StartsWith(value))
					{
						return pop3BodyText.Text;
					}
				}
			}
			foreach (Pop3BodyText pop3BodyText2 in bodyTexts)
			{
				if (!string.IsNullOrWhiteSpace(pop3BodyText2.Text))
				{
					return pop3BodyText2.Text;
				}
			}
			return string.Empty;
		}

		private void ExtractBodyTextsAndAttachments(List<string> headerComponents, List<string> bodyComponents, List<Pop3BodyText> bodyTexts, List<Pop3Attachment> attachments)
		{
			ContentType contentType = this.GetContentType(headerComponents);
			if (contentType.MediaType.StartsWith("multipart/") && !string.IsNullOrEmpty(contentType.Boundary))
			{
				List<List<string>> list = this.SplitMimePartsByBoundary(bodyComponents, contentType.Boundary);
				using (List<List<string>>.Enumerator enumerator = list.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						List<string> mimePart = enumerator.Current;
						this.SplitMimeHeaderAndBody(mimePart, out headerComponents, out bodyComponents);
						this.ExtractBodyTextsAndAttachments(headerComponents, bodyComponents, bodyTexts, attachments);
					}
					return;
				}
			}
			string attachmentName = this.GetAttachmentName(headerComponents);
			string headerValue = this.GetHeaderValue("Content-Transfer-Encoding:", headerComponents);
			if (string.IsNullOrEmpty(attachmentName) && !string.IsNullOrEmpty(contentType.CharSet))
			{
				string text = this.ConvertBodyComponentsToString(bodyComponents, headerValue, contentType);
				bodyTexts.Add(new Pop3BodyText(contentType.MediaType, text));
				return;
			}
			byte[] content = this.ConvertBodyComponentsToAttachmentContent(bodyComponents, headerValue, contentType);
			attachments.Add(new Pop3Attachment(contentType.MediaType, attachmentName, content));
		}

		private string ConvertBodyComponentsToString(List<string> bodyComponents, string contentTransferEncoding, ContentType contentType)
		{
			if (contentTransferEncoding != null)
			{
				if (contentTransferEncoding == "quoted-printable")
				{
					return Encoding.GetEncoding(contentType.CharSet).GetString(this.JoinQuotedPrintable(bodyComponents));
				}
				if (contentTransferEncoding == "base64")
				{
					byte[] bytes = Convert.FromBase64String(string.Join(string.Empty, bodyComponents));
					return Encoding.GetEncoding(contentType.CharSet).GetString(bytes);
				}
				if (contentTransferEncoding == "7bit")
				{
					return string.Join(Environment.NewLine, bodyComponents);
				}
			}
			throw new NotSupportedException("Unsupported Content-Transfer-Encoding: " + contentTransferEncoding);
		}

		private byte[] ConvertBodyComponentsToAttachmentContent(List<string> bodyComponents, string contentTransferEncoding, ContentType contentType)
		{
			if (contentTransferEncoding != null)
			{
				if (contentTransferEncoding == "base64")
				{
					return Convert.FromBase64String(string.Join(string.Empty, bodyComponents));
				}
				if (contentTransferEncoding == "quoted-printable")
				{
					return this.JoinQuotedPrintable(bodyComponents);
				}
				if (contentTransferEncoding == "7bit")
				{
					return Encoding.ASCII.GetBytes(string.Join(Environment.NewLine, bodyComponents));
				}
			}
			throw new NotSupportedException("Unsupported Content-Transfer-Encoding: " + contentTransferEncoding);
		}

		private byte[] JoinQuotedPrintable(List<string> bodyComponents)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string text in bodyComponents)
			{
				string text2 = Regex.Replace(text, "=(?<hex>[0-9a-fA-F]{2})", delegate(Match m)
				{
					string value = m.Groups["hex"].Value;
					return Convert.ToChar(Convert.ToInt32(value, 16)).ToString();
				});
				if (text.EndsWith("="))
				{
					text2 = text2.Remove(text2.Length - 1);
					stringBuilder.Append(text2);
				}
				else
				{
					stringBuilder.Append(text2);
					stringBuilder.AppendLine();
				}
			}
			return Encoding.GetEncoding("iso-8859-1").GetBytes(stringBuilder.ToString());
		}

		private ContentType GetContentType(List<string> headerComponents)
		{
			string headerValue = this.GetHeaderValue("Content-Type:", headerComponents);
			if (string.IsNullOrEmpty(headerValue))
			{
				return new ContentType();
			}
			return new ContentType(headerValue);
		}

		private string GetAttachmentName(List<string> headerComponents)
		{
			string headerValue = this.GetHeaderValue("Content-Disposition:", headerComponents);
			if (!string.IsNullOrEmpty(headerValue))
			{
				ContentDisposition contentDisposition = new ContentDisposition(headerValue);
				if (!string.IsNullOrEmpty(contentDisposition.FileName))
				{
					return contentDisposition.FileName;
				}
			}
			string headerValue2 = this.GetHeaderValue("Content-Description:", headerComponents);
			if (!string.IsNullOrEmpty(headerValue2))
			{
				return headerValue2;
			}
			ContentType contentType = this.GetContentType(headerComponents);
			if (contentType != null && !string.IsNullOrEmpty(contentType.Name))
			{
				return contentType.Name;
			}
			return null;
		}

		private List<List<string>> SplitMimePartsByBoundary(List<string> multipartBodyComponents, string boundary)
		{
			string text = "--" + boundary;
			string b = text + "--";
			List<List<string>> list = new List<List<string>>();
			List<string> list2 = new List<string>();
			bool flag = false;
			foreach (string text2 in multipartBodyComponents)
			{
				if (text2 == text)
				{
					if (flag)
					{
						list.Add(list2);
					}
					list2 = new List<string>();
					flag = true;
				}
				else
				{
					if (text2 == b)
					{
						if (flag)
						{
							list.Add(list2);
						}
						break;
					}
					list2.Add(text2);
				}
			}
			return list;
		}

		private void SplitMimeHeaderAndBody(List<string> mimePart, out List<string> headerComponents, out List<string> bodyComponents)
		{
			headerComponents = new List<string>();
			bodyComponents = new List<string>();
			bool flag = true;
			foreach (string text in mimePart)
			{
				if (flag)
				{
					if (text == string.Empty)
					{
						flag = false;
					}
					else
					{
						headerComponents.Add(text);
					}
				}
				else
				{
					bodyComponents.Add(text);
				}
			}
		}

		private string GetHeaderValue(string headerName, List<string> headerComponents)
		{
			if (string.IsNullOrEmpty(headerName))
			{
				return null;
			}
			if (!headerName.EndsWith(":"))
			{
				headerName += ':';
			}
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (string text in headerComponents)
			{
				if (text.StartsWith(headerName))
				{
					if (flag)
					{
						break;
					}
					stringBuilder.Append(text);
					flag = true;
				}
				else if (flag)
				{
					if (text.Length <= 0 || !char.IsWhiteSpace(text[0]))
					{
						break;
					}
					stringBuilder.Append(text);
				}
			}
			string text2 = stringBuilder.ToString();
			if (!string.IsNullOrEmpty(text2))
			{
				text2 = text2.Substring(headerName.Length).TrimStart(new char[0]);
			}
			return text2;
		}

		private const string HeaderFields = "^(Received:|From:|To:|Date:|Subject:|Content-Type:|FFO-ActiveMonitoring:|Message-ID:|MIME-Version:|X-FOPE-CONNECTOR:|X-IPFilteringStamp:|Return-Path:|X-SpamScore:|X-BigFish:|X-Spam-TCS-SCL:|X-Forefront-Antispam-Report:|X-MS-Exchange-Organization-SCL:|X-MS-Exchange-Organization-AVStamp-Mailbox:|X-MS-Exchange-Organization-AuthSource:|X-MS-Exchange-Organization-AuthAs:|Auto-Submitted:|X-MS-Exchange-Message-Is-Ndr:|X-MS-Exchange-Organization-AuthMechanism:|X-MS-Exchange-Organization-Network-Message-Id:|X-MS-Exchange-Organization-AVStamp-Service:|X-Forefront-Antispam-Report:|X-MS-Exchange-Organization-MessageDirectionality:|Thread-Topic:|Thread-Index:|Content-Language:|X-MS-Has-Attach:|X-MS-TNEF-Correlator:|received-spf:|Content-ID:)";

		private long number;

		private long size;

		private string message;

		private List<string> components;

		private string messageHeader;

		private string from;

		private string to;

		private DateTime date = DateTime.MinValue;

		private DateTime receivedDate = DateTime.MinValue;

		private string subject;

		private string bodyText;

		private List<Pop3Attachment> attachments;

		private bool bodyParsed;
	}
}
