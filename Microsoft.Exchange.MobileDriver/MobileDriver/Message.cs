using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class Message
	{
		public Message(string text) : this(new ProportionedText[]
		{
			new ProportionedText(text)
		})
		{
		}

		public Message(IList<ProportionedText> texts) : this(texts, DateTime.MaxValue.ToUniversalTime(), DateTime.UtcNow)
		{
		}

		public Message(string text, DateTime expiryTimeUtc, DateTime deferralTimeUtc) : this(new ProportionedText[]
		{
			new ProportionedText(text)
		}, expiryTimeUtc, deferralTimeUtc)
		{
		}

		public Message(IList<ProportionedText> texts, DateTime expiryTimeUtc, DateTime deferralTimeUtc)
		{
			if (texts == null)
			{
				throw new ArgumentNullException("texts");
			}
			if (!texts.IsReadOnly)
			{
				texts = new ReadOnlyCollection<ProportionedText>(texts);
			}
			this.ProportionedTexts = texts;
			this.ExpiryTimeUtc = expiryTimeUtc;
			this.DeferralTimeUtc = deferralTimeUtc;
		}

		public IList<ProportionedText> ProportionedTexts { get; private set; }

		public DateTime ExpiryTimeUtc { get; private set; }

		public DateTime DeferralTimeUtc { get; private set; }

		public string OriginalText
		{
			get
			{
				if (this.originalText == null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					foreach (ProportionedText value in this.ProportionedTexts)
					{
						stringBuilder.Append(value);
					}
					this.originalText = stringBuilder.ToString();
				}
				return this.originalText;
			}
		}

		private string originalText;
	}
}
