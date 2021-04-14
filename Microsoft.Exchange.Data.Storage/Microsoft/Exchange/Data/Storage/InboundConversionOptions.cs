using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InboundConversionOptions : ICloneable
	{
		private InboundConversionOptions()
		{
		}

		public InboundConversionOptions(string imceaDomain)
		{
			this.imceaResolvableDomain = InboundConversionOptions.CheckImceaDomain(imceaDomain);
		}

		public InboundConversionOptions(IRecipientSession scopedRecipientSession) : this(scopedRecipientSession, null)
		{
		}

		public InboundConversionOptions(IRecipientSession scopedRecipientSession, string imceaDomain)
		{
			if (string.IsNullOrEmpty(imceaDomain))
			{
				this.ignoreImceaDomain = true;
			}
			else
			{
				this.imceaResolvableDomain = InboundConversionOptions.CheckImceaDomain(imceaDomain);
			}
			this.UserADSession = scopedRecipientSession;
		}

		public InboundConversionOptions(IADRecipientCache scopedRecipientCache) : this(scopedRecipientCache, null)
		{
		}

		public InboundConversionOptions(IADRecipientCache scopedRecipientCache, string imceaDomain)
		{
			if (string.IsNullOrEmpty(imceaDomain))
			{
				this.ignoreImceaDomain = true;
			}
			else
			{
				this.imceaResolvableDomain = InboundConversionOptions.CheckImceaDomain(imceaDomain);
			}
			this.RecipientCache = scopedRecipientCache;
		}

		public InboundConversionOptions(InboundConversionOptions source)
		{
			this.defaultCharset = source.defaultCharset;
			this.trustAsciiCharsets = source.trustAsciiCharsets;
			this.isSenderTrusted = source.isSenderTrusted;
			this.imceaResolvableDomain = source.imceaResolvableDomain;
			this.preserveReportBody = source.preserveReportBody;
			this.clearCategories = source.clearCategories;
			this.userADSession = source.userADSession;
			this.recipientCache = source.recipientCache;
			this.limits = (ConversionLimits)source.limits.Clone();
			this.dsnWriter = source.dsnWriter;
			this.clientSubmittedSecurely = source.clientSubmittedSecurely;
			this.serverSubmittedSecurely = source.serverSubmittedSecurely;
			this.logDirectoryPath = source.logDirectoryPath;
			this.treatInlineDispositionAsAttachment = source.treatInlineDispositionAsAttachment;
			this.headerPromotionMode = source.headerPromotionMode;
			this.convertReportToMessage = source.convertReportToMessage;
			this.detectionOptions = new CharsetDetectionOptions(source.detectionOptions);
			this.applyTrustToAttachedMessages = source.applyTrustToAttachedMessages;
			this.resolveRecipientsInAttachedMessages = source.resolveRecipientsInAttachedMessages;
			this.applyHeaderFirewall = source.applyHeaderFirewall;
			this.ignoreImceaDomain = source.ignoreImceaDomain;
		}

		internal static InboundConversionOptions CreateWithNoDomain()
		{
			return new InboundConversionOptions
			{
				ignoreImceaDomain = true
			};
		}

		internal static InboundConversionOptions FromOutboundOptions(OutboundConversionOptions outboundOptions)
		{
			InboundConversionOptions inboundConversionOptions = new InboundConversionOptions(outboundOptions.ImceaEncapsulationDomain);
			if (outboundOptions.DetectionOptions.PreferredCharset != null)
			{
				inboundConversionOptions.defaultCharset = outboundOptions.DetectionOptions.PreferredCharset;
			}
			inboundConversionOptions.isSenderTrusted = outboundOptions.IsSenderTrusted;
			inboundConversionOptions.userADSession = outboundOptions.UserADSession;
			inboundConversionOptions.recipientCache = outboundOptions.RecipientCache;
			inboundConversionOptions.clearCategories = outboundOptions.ClearCategories;
			inboundConversionOptions.preserveReportBody = outboundOptions.PreserveReportBody;
			inboundConversionOptions.logDirectoryPath = outboundOptions.LogDirectoryPath;
			inboundConversionOptions.detectionOptions = outboundOptions.DetectionOptions;
			inboundConversionOptions.resolveRecipientsInAttachedMessages = outboundOptions.ResolveRecipientsInAttachedMessages;
			if (outboundOptions.Limits != null)
			{
				inboundConversionOptions.limits = outboundOptions.Limits;
			}
			return inboundConversionOptions;
		}

		internal static string CheckImceaDomain(string imceaDomain)
		{
			if (string.IsNullOrEmpty(imceaDomain))
			{
				return null;
			}
			if (!SmtpAddress.IsValidDomain(imceaDomain))
			{
				throw new ArgumentException("imceaDomain must be a valid domain name. Domain value: " + imceaDomain);
			}
			return imceaDomain;
		}

		public override string ToString()
		{
			return string.Format("InboundConversionOptions:\r\n- defaultCharset: {0}\r\n- trustAsciiCharsets: {1}\r\n- isSenderTrusted: {2}\r\n- imceaResolveableDomain: {3}\r\n- preserveReportBody: {4}\r\n- clearCategories: {5}\r\n- userADSession: {6}\r\n- recipientCache: {7}\r\n- clientSubmittedSecurely: {8}\r\n- serverSubmittedSecurely: {9}\r\n- charsetDetectionOptions: {10}\r\n- convertReportToMessage:  {11}\r\n- headerPromotionMode: {12}\r\n- treatInlineDispositionAsAttachment:  {13}\r\n- applyTrustToAttachedMessages: {14}\r\n- resolveRecipientsInAttachedMessages: {15}\r\n- applyHeaderFirewall: {16}\r\n- ignoreImceaDomain: {17}\r\n{18}\r\n", new object[]
			{
				this.defaultCharset,
				this.trustAsciiCharsets,
				this.isSenderTrusted,
				this.imceaResolvableDomain,
				this.preserveReportBody,
				this.clearCategories,
				this.userADSession,
				this.recipientCache,
				this.clientSubmittedSecurely,
				this.serverSubmittedSecurely,
				this.detectionOptions.ToString(),
				this.convertReportToMessage,
				this.headerPromotionMode,
				this.treatInlineDispositionAsAttachment,
				this.applyTrustToAttachedMessages,
				this.resolveRecipientsInAttachedMessages,
				this.applyHeaderFirewall,
				this.ignoreImceaDomain,
				this.limits.ToString()
			});
		}

		internal bool IgnoreImceaDomain
		{
			get
			{
				return this.ignoreImceaDomain;
			}
		}

		public Charset DefaultCharset
		{
			get
			{
				return this.defaultCharset;
			}
			set
			{
				this.defaultCharset = (value ?? Culture.Default.MimeCharset);
			}
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

		public bool TrustAsciiCharsets
		{
			get
			{
				return this.trustAsciiCharsets;
			}
			set
			{
				this.trustAsciiCharsets = value;
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

		public string ImceaEncapsulationDomain
		{
			get
			{
				return this.imceaResolvableDomain;
			}
			set
			{
				this.imceaResolvableDomain = InboundConversionOptions.CheckImceaDomain(value);
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

		public HeaderPromotionMode HeaderPromotion
		{
			get
			{
				return this.headerPromotionMode;
			}
			set
			{
				EnumValidator.ThrowIfInvalid<HeaderPromotionMode>(value);
				this.headerPromotionMode = value;
			}
		}

		internal PropertyBagSaveFlags GetSaveFlags(bool isTopLevelMessage)
		{
			PropertyBagSaveFlags propertyBagSaveFlags = PropertyBagSaveFlags.Default;
			HeaderPromotionMode headerPromotionMode = isTopLevelMessage ? this.HeaderPromotion : HeaderPromotionMode.NoCreate;
			if (headerPromotionMode == HeaderPromotionMode.MayCreate)
			{
				propertyBagSaveFlags |= PropertyBagSaveFlags.IgnoreUnresolvedHeaders;
			}
			else if (headerPromotionMode == HeaderPromotionMode.NoCreate)
			{
				propertyBagSaveFlags |= (PropertyBagSaveFlags.IgnoreUnresolvedHeaders | PropertyBagSaveFlags.DisableNewXHeaderMapping);
			}
			return propertyBagSaveFlags;
		}

		public bool TreatInlineDispositionAsAttachment
		{
			get
			{
				return this.treatInlineDispositionAsAttachment;
			}
			set
			{
				this.treatInlineDispositionAsAttachment = value;
			}
		}

		public bool ApplyTrustToAttachedMessages
		{
			get
			{
				return this.applyTrustToAttachedMessages;
			}
			set
			{
				this.applyTrustToAttachedMessages = value;
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

		public bool ConvertReportToMessage
		{
			get
			{
				return this.convertReportToMessage;
			}
			set
			{
				this.convertReportToMessage = value;
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
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				this.limits = value;
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

		public bool ClientSubmittedSecurely
		{
			get
			{
				return this.clientSubmittedSecurely;
			}
			set
			{
				this.clientSubmittedSecurely = value;
			}
		}

		public bool ServerSubmittedSecurely
		{
			get
			{
				return this.serverSubmittedSecurely;
			}
			set
			{
				this.serverSubmittedSecurely = value;
			}
		}

		public bool ApplyHeaderFirewall
		{
			get
			{
				return this.applyHeaderFirewall;
			}
			set
			{
				this.applyHeaderFirewall = value;
			}
		}

		public object Clone()
		{
			return new InboundConversionOptions(this);
		}

		public void LoadPerOrganizationCharsetDetectionOptions(OrganizationId organizationId)
		{
			OrganizationContentConversionProperties organizationContentConversionProperties;
			if (InboundConversionOptions.directoryAccessor.TryGetOrganizationContentConversionProperties(organizationId, out organizationContentConversionProperties))
			{
				this.detectionOptions.PreferredInternetCodePageForShiftJis = organizationContentConversionProperties.PreferredInternetCodePageForShiftJis;
				this.detectionOptions.RequiredCoverage = organizationContentConversionProperties.RequiredCharsetCoverage;
			}
		}

		public const int MaxParticipantDisplayNameLength = 512;

		internal static readonly string NoScopedTenantInfoNotice = "No IRecipientSession or IADRecipientCache has been supplied into the [Inbound/Outbound]ConversionOptions object you used. This is now required to properly scope recipient lookups.";

		private string logDirectoryPath;

		private Charset defaultCharset = Culture.Default.MimeCharset;

		private bool trustAsciiCharsets = true;

		private bool isSenderTrusted;

		private string imceaResolvableDomain;

		private bool preserveReportBody;

		private bool clearCategories = true;

		private IRecipientSession userADSession;

		private IADRecipientCache recipientCache;

		private ConversionLimits limits = new ConversionLimits(true);

		private bool clientSubmittedSecurely;

		private bool serverSubmittedSecurely;

		private DsnHumanReadableWriter dsnWriter;

		private HeaderPromotionMode headerPromotionMode;

		private CharsetDetectionOptions detectionOptions = new CharsetDetectionOptions();

		private bool convertReportToMessage;

		private bool treatInlineDispositionAsAttachment;

		private bool applyTrustToAttachedMessages = true;

		private bool resolveRecipientsInAttachedMessages = true;

		private bool applyHeaderFirewall;

		private bool ignoreImceaDomain;

		private static readonly IDirectoryAccessor directoryAccessor = new DirectoryAccessor();
	}
}
