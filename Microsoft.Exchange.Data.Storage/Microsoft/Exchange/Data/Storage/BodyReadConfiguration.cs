using System;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.TextConverters;
using Microsoft.Exchange.Data.TextConverters.Internal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	internal class BodyReadConfiguration
	{
		internal BodyReadConfiguration(BodyReadConfiguration configuration)
		{
			this.format = configuration.format;
			this.charset = configuration.charset;
			this.injectPrefix = configuration.injectPrefix;
			this.injectSuffix = configuration.injectSuffix;
			this.injectFormat = configuration.injectFormat;
			this.conversionCallback = configuration.conversionCallback;
			this.htmlFlags = configuration.htmlFlags;
			this.styleSheetLimit = configuration.styleSheetLimit;
			this.shouldCalculateLength = configuration.shouldCalculateLength;
			this.ShouldUseNarrowGapForPTagHtmlToTextConversion = configuration.ShouldUseNarrowGapForPTagHtmlToTextConversion;
			this.ShouldOutputAnchorLinks = configuration.ShouldOutputAnchorLinks;
			this.ShouldOutputImageLinks = configuration.ShouldOutputImageLinks;
		}

		public BodyReadConfiguration(BodyFormat targetFormat)
		{
			EnumValidator.ThrowIfInvalid<BodyFormat>(targetFormat, "targetFormat");
			this.format = targetFormat;
			this.charset = null;
			this.injectPrefix = null;
			this.injectSuffix = null;
			this.injectFormat = BodyInjectionFormat.Text;
			this.conversionCallback = null;
			this.htmlFlags = HtmlStreamingFlags.None;
			this.styleSheetLimit = null;
		}

		public BodyReadConfiguration(BodyFormat targetFormat, string charsetName) : this(targetFormat, BodyReadConfiguration.GetCharsetFromName(charsetName), false)
		{
		}

		public BodyReadConfiguration(BodyFormat targetFormat, string charsetName, bool shouldCalculateLength) : this(targetFormat, BodyReadConfiguration.GetCharsetFromName(charsetName), shouldCalculateLength)
		{
		}

		public BodyReadConfiguration(BodyFormat targetFormat, Charset charset) : this(targetFormat, charset, false)
		{
		}

		public BodyReadConfiguration(BodyFormat targetFormat, Charset charset, bool shouldCalculateLength)
		{
			EnumValidator.ThrowIfInvalid<BodyFormat>(targetFormat, "targetFormat");
			this.format = targetFormat;
			this.charset = charset;
			this.injectPrefix = null;
			this.injectSuffix = null;
			this.injectFormat = BodyInjectionFormat.Text;
			this.conversionCallback = null;
			this.htmlFlags = HtmlStreamingFlags.None;
			this.styleSheetLimit = null;
			this.shouldCalculateLength = shouldCalculateLength;
		}

		internal bool IsBodyTransformationNeeded(Body body)
		{
			if (body.RawFormat != this.format)
			{
				return true;
			}
			if (!string.IsNullOrEmpty(this.injectPrefix) || !string.IsNullOrEmpty(this.injectSuffix))
			{
				return true;
			}
			switch (this.format)
			{
			case BodyFormat.TextPlain:
				return this.charset.CodePage != 1200;
			case BodyFormat.TextHtml:
				return this.charset.CodePage != body.RawCharset.CodePage || this.htmlFlags != HtmlStreamingFlags.None || this.conversionCallback != null || this.styleSheetLimit != null;
			case BodyFormat.ApplicationRtf:
				return false;
			default:
				return true;
			}
		}

		public void AddInjectedText(string prefix, string suffix, BodyInjectionFormat format)
		{
			if (prefix == null && suffix == null)
			{
				throw new ArgumentException("Either prefix or suffix should be non-null");
			}
			EnumValidator.ThrowIfInvalid<BodyInjectionFormat>(format, "format");
			this.injectPrefix = prefix;
			this.injectSuffix = suffix;
			this.injectFormat = format;
		}

		public void SetHtmlOptions(HtmlStreamingFlags flags, HtmlCallbackBase callback)
		{
			EnumValidator.ThrowIfInvalid<HtmlStreamingFlags>(flags, "flags");
			this.SetHtmlOptions(flags, callback, null);
		}

		public void SetHtmlOptions(HtmlStreamingFlags flags, HtmlCallbackBase callback, int? styleSheetLimit)
		{
			EnumValidator.ThrowIfInvalid<HtmlStreamingFlags>(flags, "flags");
			if (this.Format != BodyFormat.TextHtml)
			{
				throw new InvalidOperationException("BodyReadConfiguration.SetHtmlOptions - target format is not HTML");
			}
			this.htmlFlags = flags;
			this.conversionCallback = callback;
			this.styleSheetLimit = styleSheetLimit;
		}

		public BodyFormat Format
		{
			get
			{
				return this.format;
			}
		}

		public Charset Charset
		{
			get
			{
				return this.charset;
			}
			internal set
			{
				this.charset = value;
			}
		}

		public Encoding Encoding
		{
			get
			{
				if (this.charset == null)
				{
					return null;
				}
				return this.charset.GetEncoding();
			}
		}

		public string InjectPrefix
		{
			get
			{
				return this.injectPrefix;
			}
		}

		public string InjectSuffix
		{
			get
			{
				return this.injectSuffix;
			}
		}

		public BodyInjectionFormat InjectionFormat
		{
			get
			{
				return this.injectFormat;
			}
		}

		internal HeaderFooterFormat InjectionHeaderFooterFormat
		{
			get
			{
				if (this.InjectionFormat != BodyInjectionFormat.Html)
				{
					return HeaderFooterFormat.Text;
				}
				return HeaderFooterFormat.Html;
			}
		}

		public HtmlStreamingFlags HtmlFlags
		{
			get
			{
				return this.htmlFlags;
			}
			set
			{
				this.htmlFlags = value;
			}
		}

		public bool IsHtmlFragment
		{
			get
			{
				return (this.htmlFlags & HtmlStreamingFlags.Fragment) == HtmlStreamingFlags.Fragment;
			}
		}

		public bool FilterHtml
		{
			get
			{
				return (this.htmlFlags & HtmlStreamingFlags.FilterHtml) == HtmlStreamingFlags.FilterHtml;
			}
		}

		public int? StyleSheetLimit
		{
			get
			{
				return this.styleSheetLimit;
			}
		}

		public bool ShouldCalculateLength
		{
			get
			{
				return this.shouldCalculateLength;
			}
		}

		public HtmlCallbackBase HtmlCallback
		{
			get
			{
				return this.conversionCallback as HtmlCallbackBase;
			}
			internal set
			{
				this.conversionCallback = value;
			}
		}

		public ConversionCallbackBase ConversionCallback
		{
			get
			{
				return this.conversionCallback;
			}
			set
			{
				this.conversionCallback = value;
			}
		}

		public bool ShouldUseNarrowGapForPTagHtmlToTextConversion { get; set; }

		public bool ShouldOutputAnchorLinks
		{
			get
			{
				return this.shouldOutputAnchorLinks;
			}
			set
			{
				this.shouldOutputAnchorLinks = value;
			}
		}

		public bool ShouldOutputImageLinks
		{
			get
			{
				return this.shouldOutputImageLinks;
			}
			set
			{
				this.shouldOutputImageLinks = value;
			}
		}

		internal HtmlTagCallback InternalHtmlTagCallback
		{
			get
			{
				HtmlCallbackBase htmlCallback = this.HtmlCallback;
				if (htmlCallback == null)
				{
					return null;
				}
				return new HtmlTagCallback(htmlCallback.ProcessTag);
			}
		}

		internal ImageRenderingCallback ImageRenderingCallback
		{
			get
			{
				RtfCallbackBase rtfCallbackBase = this.conversionCallback as RtfCallbackBase;
				if (rtfCallbackBase == null)
				{
					return null;
				}
				return new ImageRenderingCallback(rtfCallbackBase.ProcessImage);
			}
		}

		private static Charset GetCharsetFromName(string charsetName)
		{
			if (string.IsNullOrEmpty(charsetName))
			{
				throw new ArgumentNullException("charsetName");
			}
			return ConvertUtils.GetCharsetFromCharsetName(charsetName);
		}

		private BodyFormat format;

		private Charset charset;

		private string injectPrefix;

		private string injectSuffix;

		private BodyInjectionFormat injectFormat;

		private HtmlStreamingFlags htmlFlags;

		private ConversionCallbackBase conversionCallback;

		private int? styleSheetLimit;

		private bool shouldCalculateLength;

		private bool shouldOutputAnchorLinks = true;

		private bool shouldOutputImageLinks = true;
	}
}
