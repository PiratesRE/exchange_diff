using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplyForwardConfiguration
	{
		public ReplyForwardConfiguration(BodyFormat targetFormat) : this(targetFormat, ForwardCreationFlags.None, null)
		{
		}

		internal ReplyForwardConfiguration(CultureInfo culture) : this(BodyFormat.TextPlain, ForwardCreationFlags.None, culture)
		{
		}

		internal ReplyForwardConfiguration(ForwardCreationFlags flags, CultureInfo culture) : this(BodyFormat.TextPlain, flags, culture)
		{
		}

		internal ReplyForwardConfiguration(BodyFormat targetFormat, ForwardCreationFlags flags, CultureInfo culture)
		{
			EnumValidator.ThrowIfInvalid<BodyFormat>(targetFormat, "targetFormat");
			EnumValidator.ThrowIfInvalid<ForwardCreationFlags>(flags, "flags");
			this.targetFormat = targetFormat;
			this.forwardCreationFlags = flags;
			this.culture = culture;
			this.bodyPrefix = null;
			this.conversionCallbacks = null;
			this.subjectPrefix = null;
		}

		public BodyFormat TargetFormat
		{
			get
			{
				return this.targetFormat;
			}
		}

		public HtmlCallbackBase HtmlCallbacks
		{
			get
			{
				return this.conversionCallbacks as HtmlCallbackBase;
			}
			set
			{
				this.conversionCallbacks = value;
			}
		}

		public bool ShouldSkipFilterHtmlOnBodyWrite { get; set; }

		internal RtfCallbackBase RtfCallbacks
		{
			get
			{
				return this.conversionCallbacks as RtfCallbackBase;
			}
			set
			{
				this.conversionCallbacks = value;
			}
		}

		public string BodyPrefix
		{
			get
			{
				return this.bodyPrefix;
			}
		}

		public BodyInjectionFormat BodyPrefixFormat
		{
			get
			{
				return this.bodyPrefixFormat;
			}
		}

		public void AddBodyPrefix(string bodyPrefix)
		{
			this.bodyPrefix = bodyPrefix;
			this.bodyPrefixFormat = ((this.targetFormat == BodyFormat.TextPlain) ? BodyInjectionFormat.Text : BodyInjectionFormat.Html);
		}

		public void AddBodyPrefix(string bodyPrefix, BodyInjectionFormat bodyPrefixFormat)
		{
			EnumValidator.ThrowIfInvalid<BodyInjectionFormat>(bodyPrefixFormat, "bodyPrefixFormat");
			this.bodyPrefix = bodyPrefix;
			this.bodyPrefixFormat = bodyPrefixFormat;
		}

		public CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
			set
			{
				this.culture = value;
			}
		}

		public ForwardCreationFlags ForwardCreationFlags
		{
			get
			{
				return this.forwardCreationFlags;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<ForwardCreationFlags>(value, "value");
				this.forwardCreationFlags = value;
			}
		}

		public string SubjectPrefix
		{
			get
			{
				return this.subjectPrefix;
			}
			set
			{
				this.subjectPrefix = value;
			}
		}

		public string XLoop
		{
			get
			{
				return this.xLoop;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					throw new ArgumentException("Value must not be null or empty", "value");
				}
				this.xLoop = value;
			}
		}

		public InboundConversionOptions ConversionOptionsForSmime
		{
			get
			{
				return this.optionsForSmime;
			}
			set
			{
				this.optionsForSmime = value;
			}
		}

		public ExTimeZone TimeZone
		{
			get
			{
				return this.timeZone;
			}
			set
			{
				this.timeZone = value;
			}
		}

		public bool ShouldSuppressReadReceipt
		{
			get
			{
				return this.shouldSuppressReadReceipt;
			}
			set
			{
				this.shouldSuppressReadReceipt = value;
			}
		}

		private string xLoop;

		private string bodyPrefix;

		private string subjectPrefix;

		private BodyFormat targetFormat;

		private BodyInjectionFormat bodyPrefixFormat;

		private ConversionCallbackBase conversionCallbacks;

		private CultureInfo culture;

		private ForwardCreationFlags forwardCreationFlags;

		private InboundConversionOptions optionsForSmime;

		private ExTimeZone timeZone;

		private bool shouldSuppressReadReceipt;
	}
}
