using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OutboundConversionOptions : ICloneable
	{
		public OutboundConversionOptions(string imceaDomain)
		{
			this.imceaEncapsulationDomain = InboundConversionOptions.CheckImceaDomain(imceaDomain);
			this.encodeAttachmentsAsBinhex = false;
			this.suppressDisplayName = false;
			this.internetMessageFormat = InternetMessageFormat.Mime;
			this.internetTextFormat = InternetTextFormat.HtmlAndTextAlternative;
			this.preserveReportBody = true;
			this.byteEncoderTypeFor7BitCharsets = ByteEncoderTypeFor7BitCharsets.UseQP;
			this.clearCategories = true;
			this.owaServer = null;
			this.userADSession = null;
			this.recipientCache = null;
			this.useRFC2231Encoding = false;
			this.allowUTF8Headers = false;
			this.limits = new ConversionLimits(false);
			this.dsnWriter = null;
			this.logDirectoryPath = null;
			this.isSenderTrusted = true;
			this.useSimpleDisplayName = false;
			this.allowPartialStnefContent = false;
			this.generateMimeSkeleton = false;
			this.demoteBcc = false;
			this.resolveRecipientsInAttachedMessages = true;
			this.quoteDisplayNameBeforeRfc2047Encoding = false;
			this.allowDlpHeadersToPenetrateFirewall = false;
			this.EnableCalendarHeaderGeneration = true;
			this.EwsOutboundMimeConversion = false;
			this.blockPlainTextConversion = true;
			this.useSkeleton = true;
		}

		public OutboundConversionOptions(IRecipientSession scopedRecipientSession, string imceaDomain) : this(imceaDomain)
		{
			this.UserADSession = scopedRecipientSession;
		}

		public OutboundConversionOptions(IADRecipientCache scopedRecipientCache, string imceaDomain) : this(imceaDomain)
		{
			this.RecipientCache = scopedRecipientCache;
		}

		public OutboundConversionOptions(OutboundConversionOptions source)
		{
			this.encodeAttachmentsAsBinhex = source.encodeAttachmentsAsBinhex;
			this.suppressDisplayName = source.suppressDisplayName;
			this.internetMessageFormat = source.internetMessageFormat;
			this.internetTextFormat = source.internetTextFormat;
			this.imceaEncapsulationDomain = source.imceaEncapsulationDomain;
			this.preserveReportBody = source.preserveReportBody;
			this.byteEncoderTypeFor7BitCharsets = source.byteEncoderTypeFor7BitCharsets;
			this.clearCategories = source.clearCategories;
			this.owaServer = source.owaServer;
			this.userADSession = source.userADSession;
			this.recipientCache = source.recipientCache;
			this.useRFC2231Encoding = source.useRFC2231Encoding;
			this.allowUTF8Headers = source.allowUTF8Headers;
			this.limits = (ConversionLimits)source.limits.Clone();
			this.dsnWriter = source.dsnWriter;
			this.logDirectoryPath = source.LogDirectoryPath;
			this.isSenderTrusted = source.IsSenderTrusted;
			this.useSimpleDisplayName = source.useSimpleDisplayName;
			this.allowPartialStnefContent = source.allowPartialStnefContent;
			this.generateMimeSkeleton = source.generateMimeSkeleton;
			this.demoteBcc = source.demoteBcc;
			this.detectionOptions = source.detectionOptions;
			this.filterAttachment = source.filterAttachment;
			this.filterBody = source.filterBody;
			this.resolveRecipientsInAttachedMessages = source.resolveRecipientsInAttachedMessages;
			this.quoteDisplayNameBeforeRfc2047Encoding = source.quoteDisplayNameBeforeRfc2047Encoding;
			this.allowDlpHeadersToPenetrateFirewall = source.allowDlpHeadersToPenetrateFirewall;
			this.EnableCalendarHeaderGeneration = source.EnableCalendarHeaderGeneration;
			this.EwsOutboundMimeConversion = source.EwsOutboundMimeConversion;
			this.blockPlainTextConversion = source.BlockPlainTextConversion;
			this.useSkeleton = source.UseSkeleton;
		}

		public CharsetDetectionOptions DetectionOptions
		{
			get
			{
				return this.detectionOptions;
			}
			set
			{
				if (value != null)
				{
					this.detectionOptions = new CharsetDetectionOptions(value);
					return;
				}
				throw new ArgumentNullException();
			}
		}

		public bool DemoteBcc
		{
			get
			{
				return this.demoteBcc;
			}
			set
			{
				this.demoteBcc = value;
			}
		}

		public bool GenerateMimeSkeleton
		{
			get
			{
				return this.generateMimeSkeleton;
			}
			set
			{
				this.generateMimeSkeleton = value;
			}
		}

		public bool EncodeAttachmentsAsBinhex
		{
			get
			{
				return this.encodeAttachmentsAsBinhex;
			}
			set
			{
				this.encodeAttachmentsAsBinhex = value;
			}
		}

		public bool SuppressDisplayName
		{
			get
			{
				return this.suppressDisplayName;
			}
			set
			{
				this.suppressDisplayName = value;
			}
		}

		public DsnMdnOptions DsnMdnOptions
		{
			get
			{
				return this.dsnMdnOptions;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<DsnMdnOptions>(value, "value");
				this.dsnMdnOptions = value;
			}
		}

		public InternetMessageFormat InternetMessageFormat
		{
			get
			{
				return this.internetMessageFormat;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<InternetMessageFormat>(value, "value");
				this.internetMessageFormat = value;
			}
		}

		public InternetTextFormat InternetTextFormat
		{
			get
			{
				return this.internetTextFormat;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<InternetTextFormat>(value, "value");
				this.internetTextFormat = value;
			}
		}

		public bool PreserveReportBody
		{
			get
			{
				return this.preserveReportBody;
			}
			set
			{
				this.preserveReportBody = value;
			}
		}

		public ConversionLimits Limits
		{
			get
			{
				return this.limits;
			}
			set
			{
				this.limits = value;
			}
		}

		public string ImceaEncapsulationDomain
		{
			get
			{
				return this.imceaEncapsulationDomain;
			}
			set
			{
				this.imceaEncapsulationDomain = InboundConversionOptions.CheckImceaDomain(value);
			}
		}

		public ByteEncoderTypeFor7BitCharsets ByteEncoderTypeFor7BitCharsets
		{
			get
			{
				return this.byteEncoderTypeFor7BitCharsets;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<ByteEncoderTypeFor7BitCharsets>(value, "ByteEncoderTypeFor7BitCharsets");
				this.byteEncoderTypeFor7BitCharsets = value;
			}
		}

		public bool ClearCategories
		{
			get
			{
				return this.clearCategories;
			}
			set
			{
				this.clearCategories = value;
			}
		}

		public string LogDirectoryPath
		{
			get
			{
				return this.logDirectoryPath;
			}
			set
			{
				this.logDirectoryPath = value;
			}
		}

		public bool IsSenderTrusted
		{
			get
			{
				return this.isSenderTrusted;
			}
			set
			{
				this.isSenderTrusted = value;
			}
		}

		public string OwaServer
		{
			get
			{
				return this.owaServer;
			}
			set
			{
				Uri uri;
				if (!Uri.TryCreate(value, UriKind.Absolute, out uri))
				{
					throw new ArgumentException("value");
				}
				this.owaServer = uri.ToString();
			}
		}

		public IRecipientSession UserADSession
		{
			get
			{
				return this.userADSession;
			}
			set
			{
				this.userADSession = value;
			}
		}

		public IADRecipientCache RecipientCache
		{
			get
			{
				return this.recipientCache;
			}
			set
			{
				this.recipientCache = value;
			}
		}

		public bool UseRFC2231Encoding
		{
			get
			{
				return this.useRFC2231Encoding;
			}
			set
			{
				this.useRFC2231Encoding = value;
			}
		}

		public bool AllowUTF8Headers
		{
			get
			{
				return this.allowUTF8Headers;
			}
			set
			{
				this.allowUTF8Headers = value;
			}
		}

		public DsnHumanReadableWriter DsnHumanReadableWriter
		{
			get
			{
				return this.dsnWriter;
			}
			set
			{
				this.dsnWriter = value;
			}
		}

		public bool UseSimpleDisplayName
		{
			get
			{
				return this.useSimpleDisplayName;
			}
			set
			{
				this.useSimpleDisplayName = value;
			}
		}

		public bool AllowPartialStnefConversion
		{
			get
			{
				return this.allowPartialStnefContent;
			}
			set
			{
				this.allowPartialStnefContent = value;
			}
		}

		public bool QuoteDisplayNameBeforeRfc2047Encoding
		{
			get
			{
				return this.quoteDisplayNameBeforeRfc2047Encoding;
			}
			set
			{
				this.quoteDisplayNameBeforeRfc2047Encoding = value;
			}
		}

		public bool ResolveRecipientsInAttachedMessages
		{
			get
			{
				return this.resolveRecipientsInAttachedMessages;
			}
			set
			{
				this.resolveRecipientsInAttachedMessages = value;
			}
		}

		public OutboundConversionOptions.FilterAttachment FilterAttachmentHandler
		{
			get
			{
				return this.filterAttachment;
			}
			set
			{
				this.filterAttachment = value;
			}
		}

		public OutboundConversionOptions.FilterBody FilterBodyHandler
		{
			get
			{
				return this.filterBody;
			}
			set
			{
				this.filterBody = value;
			}
		}

		public bool AllowDlpHeadersToPenetrateFirewall
		{
			get
			{
				return this.allowDlpHeadersToPenetrateFirewall;
			}
			set
			{
				this.allowDlpHeadersToPenetrateFirewall = value;
			}
		}

		public bool BlockPlainTextConversion
		{
			get
			{
				return this.blockPlainTextConversion;
			}
			set
			{
				this.blockPlainTextConversion = value;
			}
		}

		public bool UseSkeleton
		{
			get
			{
				return this.useSkeleton;
			}
			set
			{
				this.useSkeleton = value;
			}
		}

		public bool EnableCalendarHeaderGeneration { get; set; }

		public bool EwsOutboundMimeConversion { get; set; }

		public object Clone()
		{
			return new OutboundConversionOptions(this);
		}

		public override string ToString()
		{
			return string.Format("OutboundConversionOptions:\r\n- detectionOptions: {0}\r\n- encodeAttachmentsAsBinhex: {1}\r\n- suppressDisplayName: {2}\r\n- internetMessageFormat: {3}\r\n- internetTextFormat: {4}\r\n- imceaEncapsulationDomain: {5}\r\n- preserveReportBody: {6}\r\n- byteEncoderTypeFor7BitCharsets: {7}\r\n- clearCategories: {8}\r\n- owaServer: {9}\r\n- logDirectoryPath: {10}\r\n- isSenderTrusted: {11}\r\n- dsnWriter: {12}\r\n- userADSession: {13}\r\n- useRFC2231Encoding: {14}\r\n- recipientCache: {15}\r\n- demoteBcc: {16}\r\n- useSimpleDisplayName: {17}\r\n- partialStnefConversion: {18}\r\n- resolveRecipientsInAttachedMessages: {19}\r\n- quoteDisplayNameBeforeRfc2047Encoding: {20}\r\n- allowDlpHeadersToPenetrateFirewall: {21}\r\n- enableCalendarHeaderGeneration: {22}\r\n- ewsOutboundMimeConversion: {23}\r\n{24}\r\n", new object[]
			{
				this.detectionOptions.ToString(),
				this.encodeAttachmentsAsBinhex,
				this.suppressDisplayName,
				this.internetMessageFormat,
				this.internetTextFormat,
				this.imceaEncapsulationDomain,
				this.preserveReportBody,
				this.byteEncoderTypeFor7BitCharsets,
				this.clearCategories,
				this.owaServer,
				this.logDirectoryPath,
				this.isSenderTrusted,
				this.dsnWriter,
				this.userADSession,
				this.useRFC2231Encoding,
				this.recipientCache,
				this.demoteBcc,
				this.useSimpleDisplayName,
				this.allowPartialStnefContent,
				this.resolveRecipientsInAttachedMessages,
				this.quoteDisplayNameBeforeRfc2047Encoding,
				this.allowDlpHeadersToPenetrateFirewall,
				this.EnableCalendarHeaderGeneration,
				this.EwsOutboundMimeConversion,
				this.limits.ToString()
			});
		}

		public void LoadPerOrganizationCharsetDetectionOptions(OrganizationId organizationId)
		{
			OrganizationContentConversionProperties organizationContentConversionProperties;
			if (OutboundConversionOptions.directoryAccessor.TryGetOrganizationContentConversionProperties(organizationId, out organizationContentConversionProperties))
			{
				this.detectionOptions.PreferredInternetCodePageForShiftJis = organizationContentConversionProperties.PreferredInternetCodePageForShiftJis;
				this.detectionOptions.RequiredCoverage = organizationContentConversionProperties.RequiredCharsetCoverage;
				if (Enum.IsDefined(typeof(ByteEncoderTypeFor7BitCharsets), organizationContentConversionProperties.ByteEncoderTypeFor7BitCharsets))
				{
					this.byteEncoderTypeFor7BitCharsets = (ByteEncoderTypeFor7BitCharsets)organizationContentConversionProperties.ByteEncoderTypeFor7BitCharsets;
				}
			}
		}

		internal IADRecipientCache InternalGetRecipientCache(int count)
		{
			IADRecipientCache iadrecipientCache = this.RecipientCache;
			if (iadrecipientCache == null)
			{
				IRecipientSession tenantOrRootOrgRecipientSession = this.UserADSession;
				if (tenantOrRootOrgRecipientSession == null)
				{
					try
					{
						tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 2002, "InternalGetRecipientCache", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\ContentConversion\\ConversionOptions.cs");
					}
					catch (ExAssertException)
					{
						throw new ArgumentException(InboundConversionOptions.NoScopedTenantInfoNotice);
					}
				}
				iadrecipientCache = new ADRecipientCache<ADRawEntry>(tenantOrRootOrgRecipientSession, Util.CollectionToArray<ADPropertyDefinition>(ParticipantSchema.SupportedADProperties), count);
			}
			return iadrecipientCache;
		}

		private OutboundConversionOptions.FilterAttachment filterAttachment;

		private OutboundConversionOptions.FilterBody filterBody;

		private bool encodeAttachmentsAsBinhex;

		private bool suppressDisplayName;

		private DsnMdnOptions dsnMdnOptions;

		private InternetMessageFormat internetMessageFormat;

		private InternetTextFormat internetTextFormat;

		private string imceaEncapsulationDomain;

		private bool preserveReportBody;

		private ByteEncoderTypeFor7BitCharsets byteEncoderTypeFor7BitCharsets;

		private bool clearCategories;

		private string owaServer;

		private IRecipientSession userADSession;

		private IADRecipientCache recipientCache;

		private string logDirectoryPath;

		private bool useRFC2231Encoding;

		private bool allowUTF8Headers;

		private bool isSenderTrusted;

		private bool useSimpleDisplayName;

		private bool allowPartialStnefContent;

		private bool quoteDisplayNameBeforeRfc2047Encoding;

		private bool generateMimeSkeleton;

		private bool demoteBcc;

		private CharsetDetectionOptions detectionOptions = new CharsetDetectionOptions();

		private ConversionLimits limits;

		private DsnHumanReadableWriter dsnWriter;

		private bool resolveRecipientsInAttachedMessages;

		private bool allowDlpHeadersToPenetrateFirewall;

		private bool blockPlainTextConversion;

		private bool useSkeleton;

		private static readonly IDirectoryAccessor directoryAccessor = new DirectoryAccessor();

		public delegate bool FilterAttachment(Item item, Attachment attachment);

		public delegate bool FilterBody(Item item);
	}
}
