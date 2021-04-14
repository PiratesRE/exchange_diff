using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.TextMatching;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal sealed class MessageBodies : IContent
	{
		public MessageBodies(EmailMessage message, int level)
		{
			this.message = message;
			this.level = level;
			this.Reset();
		}

		public void Reset()
		{
			this.firstBody = true;
		}

		private static bool CtsMatcher(MultiMatcher matcher, RulesEvaluationContext context, MessageBodies.MessageBody body, int bodyLevel, int bodyPartNumber)
		{
			return matcher.IsMatch(body.Reader, string.Concat(new object[]
			{
				"tagMessageBody",
				bodyLevel,
				'.',
				bodyPartNumber
			}), 0, context);
		}

		private MessageBodies.MessageBody GetNextBody()
		{
			if (this.firstBody)
			{
				this.firstBody = false;
				return MessageBodies.MessageBody.Create(this.message.Body);
			}
			return null;
		}

		bool IContent.Matches(MultiMatcher matcher, RulesEvaluationContext context)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)context;
			return this.Matches((MessageBodies.MessageBody body, int bodyLevel, int bodyPartNumber) => MessageBodies.CtsMatcher(matcher, context, body, bodyLevel, bodyPartNumber));
		}

		private bool Matches(MessageBodies.BodyMatchingDelegate matcher)
		{
			this.Reset();
			int num = 0;
			bool result;
			for (;;)
			{
				MessageBodies.MessageBody nextBody;
				MessageBodies.MessageBody messageBody = nextBody = this.GetNextBody();
				try
				{
					if (messageBody == null)
					{
						result = false;
						break;
					}
					if (matcher(messageBody, this.level, num))
					{
						result = true;
						break;
					}
				}
				finally
				{
					if (nextBody != null)
					{
						((IDisposable)nextBody).Dispose();
					}
				}
				num++;
			}
			return result;
		}

		private const string TagMessageBody = "tagMessageBody";

		private readonly int level;

		private readonly EmailMessage message;

		private bool firstBody = true;

		private delegate bool BodyMatchingDelegate(MessageBodies.MessageBody body, int level, int bodyPartNumber);

		internal class MessageBody : ITextInputBuffer, IDisposable
		{
			public TextReader Reader
			{
				get
				{
					return this.reader;
				}
			}

			private MessageBody(BodyFormat bodyFormat, Stream stream, int codePage)
			{
				TextConverter converter;
				switch (bodyFormat)
				{
				case BodyFormat.Text:
				{
					TextToText textToText = new TextToText
					{
						InputEncoding = Charset.GetEncoding(codePage)
					};
					converter = textToText;
					break;
				}
				case BodyFormat.Rtf:
				{
					RtfToText rtfToText = new RtfToText();
					converter = rtfToText;
					break;
				}
				case BodyFormat.Html:
				{
					HtmlToText htmlToText = new HtmlToText
					{
						InputEncoding = Charset.GetEncoding(codePage)
					};
					converter = htmlToText;
					break;
				}
				default:
					throw new ArgumentOutOfRangeException(string.Format("Parameter bodyFormat is out of range: '{0}'", bodyFormat));
				}
				this.reader = new ConverterReader(stream, converter);
			}

			internal static MessageBodies.MessageBody Create(BodyFormat bodyFormat, Stream stream, int codePage)
			{
				return new MessageBodies.MessageBody(bodyFormat, stream, codePage);
			}

			internal static MessageBodies.MessageBody Create(Body body)
			{
				if (body == null)
				{
					return null;
				}
				BodyFormat bodyFormat = body.BodyFormat;
				if (bodyFormat != BodyFormat.Text && bodyFormat != BodyFormat.Html && bodyFormat != BodyFormat.Rtf)
				{
					return null;
				}
				Stream rawContentReadStream;
				if (!body.TryGetContentReadStream(out rawContentReadStream))
				{
					if (body.MimePart == null)
					{
						return null;
					}
					rawContentReadStream = body.MimePart.GetRawContentReadStream();
				}
				string charsetName = body.CharsetName;
				Encoding ascii;
				if (charsetName == null || !Charset.TryGetEncoding(charsetName, out ascii))
				{
					ascii = Encoding.ASCII;
				}
				return new MessageBodies.MessageBody(bodyFormat, rawContentReadStream, ascii.CodePage);
			}

			public void Dispose()
			{
				this.reader.Close();
			}

			public int NextChar
			{
				get
				{
					if (this.firstRead)
					{
						this.firstRead = false;
						return 1;
					}
					int num = this.reader.Read();
					if (-1 == num)
					{
						return -1;
					}
					return (int)char.ToLowerInvariant((char)num);
				}
			}

			private readonly TextReader reader;

			private bool firstRead = true;
		}
	}
}
