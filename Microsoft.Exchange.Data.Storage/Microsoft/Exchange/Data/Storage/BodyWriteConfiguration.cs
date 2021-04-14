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
	internal class BodyWriteConfiguration
	{
		internal BodyWriteConfiguration(BodyWriteConfiguration configuration)
		{
			this.sourceFormat = configuration.sourceFormat;
			this.sourceCharset = configuration.sourceCharset;
			this.targetFormat = configuration.targetFormat;
			this.targetCharset = configuration.targetCharset;
			this.targetCharsetFlags = configuration.targetCharsetFlags;
			this.injectPrefix = configuration.injectPrefix;
			this.injectSuffix = configuration.injectSuffix;
			this.injectFormat = configuration.injectFormat;
			this.conversionCallbacks = configuration.conversionCallbacks;
			this.htmlFlags = configuration.htmlFlags;
			this.trustHtmlMetaTag = configuration.trustHtmlMetaTag;
		}

		public BodyWriteConfiguration(BodyFormat sourceFormat)
		{
			EnumValidator.ThrowIfInvalid<BodyFormat>(sourceFormat, "sourceFormat");
			this.sourceFormat = sourceFormat;
			this.sourceCharset = ConvertUtils.UnicodeCharset;
			this.targetFormat = sourceFormat;
			this.targetCharset = null;
			this.targetCharsetFlags = BodyCharsetFlags.None;
			this.injectPrefix = null;
			this.injectSuffix = null;
			this.injectFormat = BodyInjectionFormat.Text;
			this.htmlFlags = HtmlStreamingFlags.None;
		}

		public BodyWriteConfiguration(BodyFormat sourceFormat, Charset sourceCharset)
		{
			EnumValidator.ThrowIfInvalid<BodyFormat>(sourceFormat, "sourceFormat");
			this.sourceFormat = sourceFormat;
			this.sourceCharset = sourceCharset;
			this.targetFormat = this.sourceFormat;
			this.targetCharset = this.sourceCharset;
			this.targetCharsetFlags = BodyCharsetFlags.None;
			this.injectPrefix = null;
			this.injectSuffix = null;
			this.injectFormat = BodyInjectionFormat.Text;
			this.htmlFlags = HtmlStreamingFlags.None;
		}

		public BodyWriteConfiguration(BodyFormat sourceFormat, string sourceCharsetName) : this(sourceFormat, string.IsNullOrEmpty(sourceCharsetName) ? null : ConvertUtils.GetCharsetFromCharsetName(sourceCharsetName))
		{
		}

		internal bool IsContentTransformationNeeded(ICoreItem coreItem)
		{
			if (this.sourceFormat != this.targetFormat)
			{
				return true;
			}
			if (!string.IsNullOrEmpty(this.injectPrefix) || !string.IsNullOrEmpty(this.injectSuffix))
			{
				return true;
			}
			switch (this.targetFormat)
			{
			case BodyFormat.TextPlain:
				return this.sourceCharset.CodePage != 1200;
			case BodyFormat.TextHtml:
			{
				Charset charset;
				return this.htmlFlags != HtmlStreamingFlags.None || this.HtmlCallback != null || !this.trustHtmlMetaTag || !coreItem.CharsetDetector.IsItemCharsetKnownWithoutDetection(this.targetCharsetFlags, this.targetCharset, out charset) || charset.CodePage != this.sourceCharset.CodePage;
			}
			case BodyFormat.ApplicationRtf:
				return false;
			default:
				return true;
			}
		}

		public void SetTargetFormat(BodyFormat targetFormat, Charset targetCharset)
		{
			this.SetTargetFormat(targetFormat, targetCharset, BodyCharsetFlags.None);
		}

		public void SetTargetFormat(BodyFormat targetFormat, string targetCharsetName)
		{
			this.SetTargetFormat(targetFormat, targetCharsetName, BodyCharsetFlags.None);
		}

		public void SetTargetFormat(BodyFormat targetFormat, Charset targetCharset, BodyCharsetFlags flags)
		{
			EnumValidator.ThrowIfInvalid<BodyFormat>(targetFormat, "targetFormat");
			if (targetCharset == null && (flags & BodyCharsetFlags.CharsetDetectionMask) == BodyCharsetFlags.DisableCharsetDetection)
			{
				throw new ArgumentNullException("targetCharset");
			}
			this.targetCharset = targetCharset;
			this.targetFormat = targetFormat;
			this.targetCharsetFlags = flags;
		}

		public void SetTargetFormat(BodyFormat targetFormat, string targetCharsetName, BodyCharsetFlags flags)
		{
			EnumValidator.ThrowIfInvalid<BodyFormat>(targetFormat, "targetFormat");
			Charset charset = null;
			if (!string.IsNullOrEmpty(targetCharsetName))
			{
				charset = ConvertUtils.GetCharsetFromCharsetName(targetCharsetName);
			}
			this.SetTargetFormat(targetFormat, charset, flags);
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
			if (this.targetFormat != BodyFormat.TextHtml && this.sourceFormat != BodyFormat.TextHtml)
			{
				throw new InvalidOperationException("BodyReadConfiguration.SetHtmlOptions - neither source not target format is HTML");
			}
			this.htmlFlags = flags;
			this.conversionCallbacks.HtmlCallback = callback;
		}

		public BodyFormat SourceFormat
		{
			get
			{
				return this.sourceFormat;
			}
		}

		public Charset SourceCharset
		{
			get
			{
				return this.sourceCharset;
			}
			internal set
			{
				this.sourceCharset = value;
			}
		}

		internal Encoding SourceEncoding
		{
			get
			{
				if (this.sourceCharset == null)
				{
					return null;
				}
				return this.sourceCharset.GetEncoding();
			}
		}

		internal bool TrustHtmlMetaTag
		{
			get
			{
				return this.trustHtmlMetaTag;
			}
			set
			{
				this.trustHtmlMetaTag = value;
			}
		}

		public BodyFormat TargetFormat
		{
			get
			{
				return this.targetFormat;
			}
		}

		public Charset TargetCharset
		{
			get
			{
				return this.targetCharset;
			}
		}

		internal Encoding TargetEncoding
		{
			get
			{
				if (this.targetCharset == null)
				{
					return null;
				}
				return this.targetCharset.GetEncoding();
			}
		}

		public BodyCharsetFlags TargetCharsetFlags
		{
			get
			{
				return this.targetCharsetFlags;
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
			internal set
			{
				this.htmlFlags = value;
			}
		}

		public bool FilterHtml
		{
			get
			{
				return (this.htmlFlags & HtmlStreamingFlags.FilterHtml) == HtmlStreamingFlags.FilterHtml;
			}
		}

		public HtmlCallbackBase HtmlCallback
		{
			get
			{
				return this.conversionCallbacks.HtmlCallback;
			}
			internal set
			{
				this.conversionCallbacks.HtmlCallback = value;
			}
		}

		internal RtfCallbackBase RtfCallback
		{
			get
			{
				return this.conversionCallbacks.RtfCallback;
			}
			set
			{
				this.conversionCallbacks.RtfCallback = value;
			}
		}

		internal HtmlTagCallback InternalHtmlTagCallback
		{
			get
			{
				if (this.conversionCallbacks.HtmlCallback == null)
				{
					return null;
				}
				return new HtmlTagCallback(this.conversionCallbacks.HtmlCallback.ProcessTag);
			}
		}

		internal ImageRenderingCallback ImageRenderingCallback
		{
			get
			{
				if (this.conversionCallbacks.RtfCallback == null)
				{
					return null;
				}
				return new ImageRenderingCallback(this.conversionCallbacks.RtfCallback.ProcessImage);
			}
		}

		private BodyFormat sourceFormat;

		private Charset sourceCharset;

		private BodyFormat targetFormat;

		private Charset targetCharset;

		private BodyCharsetFlags targetCharsetFlags;

		private string injectPrefix;

		private string injectSuffix;

		private BodyInjectionFormat injectFormat;

		private HtmlStreamingFlags htmlFlags;

		private BodyConversionCallbacks conversionCallbacks = default(BodyConversionCallbacks);

		private bool trustHtmlMetaTag;
	}
}
