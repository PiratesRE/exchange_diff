using System;
using System.Globalization;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal sealed class QuotaInformation
	{
		public QuotaInformation(Charset messageCharset, CultureInfo culture, string subject, string topText, string details, string finalText, string topTextFont, string finalTextFont, string bodyTextFont, string currentSizeTitle, string currentSizeText, string maxSizeTitle, string maxSizeText, bool hasMaxSize, bool isWarning, float percentFull)
		{
			this.culture = culture;
			this.CurrentSizeText = currentSizeText;
			this.TopText = topText;
			this.Details = details;
			this.FinalText = finalText;
			this.TopTextFont = topTextFont;
			this.FinalTextFont = finalTextFont;
			this.BodyTextFont = bodyTextFont;
			this.HasMaxSize = hasMaxSize;
			this.CurrentSizeTitle = currentSizeTitle;
			this.MaxSizeTitle = maxSizeTitle;
			this.MaxSizeText = maxSizeText;
			this.IsWarning = isWarning;
			this.messageCharset = messageCharset;
			this.PercentFull = percentFull;
			this.Subject = subject;
		}

		public CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
		}

		public Charset MessageCharset
		{
			get
			{
				return this.messageCharset;
			}
		}

		public readonly bool HasMaxSize;

		public readonly string CurrentSizeText;

		public readonly string Subject;

		public readonly string TopText;

		public readonly string FinalText;

		public readonly string Details;

		public readonly string TopTextFont;

		public readonly string FinalTextFont;

		public readonly string BodyTextFont;

		public readonly string CurrentSizeTitle;

		public readonly string MaxSizeTitle;

		public readonly string MaxSizeText;

		public readonly bool IsWarning;

		public readonly float PercentFull;

		private Charset messageCharset;

		private CultureInfo culture;
	}
}
