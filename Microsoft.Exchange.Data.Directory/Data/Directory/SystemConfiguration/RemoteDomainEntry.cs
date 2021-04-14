using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class RemoteDomainEntry : RemoteDomain, DomainMatchMap<RemoteDomainEntry>.IDomainEntry
	{
		public RemoteDomainEntry(DomainContentConfig domain)
		{
			this.domain = domain.DomainName;
			this.ByteEncoderTypeFor7BitCharsets = domain.ByteEncoderTypeFor7BitCharsets;
			this.CharacterSet = domain.CharacterSet;
			this.NonMimeCharacterSet = domain.NonMimeCharacterSet;
			this.AllowedOOFType = domain.AllowedOOFType;
			this.ContentType = domain.ContentType;
			this.DisplaySenderName = domain.DisplaySenderName;
			this.PreferredInternetCodePageForShiftJis = domain.PreferredInternetCodePageForShiftJis;
			this.RequiredCharsetCoverage = domain.RequiredCharsetCoverage;
			this.TNEFEnabled = domain.TNEFEnabled;
			this.LineWrapSize = domain.LineWrapSize;
			this.OofSettings = domain.GetOOFSettings();
			this.Flags = (int)domain[EdgeDomainContentConfigSchema.Flags];
			this.UseSimpleDisplayName = domain.UseSimpleDisplayName;
			this.MessageCountThreshold = domain.MessageCountThreshold;
		}

		public SmtpDomainWithSubdomains DomainName
		{
			get
			{
				return this.domain;
			}
		}

		public bool TrustedMailOutboundEnabled
		{
			get
			{
				return (this.Flags & 1) != 0;
			}
		}

		public bool TrustedMailInboundEnabled
		{
			get
			{
				return (this.Flags & 2) != 0;
			}
		}

		public bool AutoReplyEnabled
		{
			get
			{
				return (this.OofSettings & AcceptMessageType.AutoReply) != AcceptMessageType.Default;
			}
		}

		public bool AutoForwardEnabled
		{
			get
			{
				return (this.OofSettings & AcceptMessageType.AutoForward) != AcceptMessageType.Default;
			}
		}

		public bool DeliveryReportEnabled
		{
			get
			{
				return (this.OofSettings & AcceptMessageType.DR) != AcceptMessageType.Default;
			}
		}

		public bool NDREnabled
		{
			get
			{
				return (this.OofSettings & AcceptMessageType.NDR) != AcceptMessageType.Default;
			}
		}

		public bool MFNEnabled
		{
			get
			{
				return (this.OofSettings & AcceptMessageType.MFN) != AcceptMessageType.Default;
			}
		}

		public bool NDRDiagnosticInfoEnabled
		{
			get
			{
				return (this.OofSettings & AcceptMessageType.NDRDiagnosticInfoDisabled) == AcceptMessageType.Default;
			}
		}

		public override string NameSpecification
		{
			get
			{
				return this.DomainName.ToString();
			}
		}

		public override string NonMimeCharset
		{
			get
			{
				return this.NonMimeCharacterSet;
			}
		}

		public override bool IsInternal
		{
			get
			{
				return (this.OofSettings & AcceptMessageType.InternalDomain) != AcceptMessageType.Default;
			}
		}

		public static int GetLenghAfterNullCheck(string s)
		{
			if (s == null)
			{
				return 0;
			}
			return s.Length;
		}

		public override string ToString()
		{
			return this.NameSpecification;
		}

		public int EstimateSize
		{
			get
			{
				if (this.DomainName == null)
				{
					return RemoteDomainEntry.GetLenghAfterNullCheck(this.NameSpecification) * 2 + RemoteDomainEntry.GetLenghAfterNullCheck(this.NonMimeCharacterSet) * 2 + 2 + 20;
				}
				return RemoteDomainEntry.GetLenghAfterNullCheck(this.DomainName.Domain) * 2 + 1;
			}
		}

		public readonly string CharacterSet;

		public readonly string NonMimeCharacterSet;

		public readonly ByteEncoderTypeFor7BitCharsetsEnum ByteEncoderTypeFor7BitCharsets;

		public readonly AllowedOOFType AllowedOOFType;

		public readonly ContentType ContentType;

		public readonly bool DisplaySenderName;

		public readonly PreferredInternetCodePageForShiftJisEnum PreferredInternetCodePageForShiftJis;

		public readonly int? RequiredCharsetCoverage;

		public readonly bool? TNEFEnabled;

		public readonly Unlimited<int> LineWrapSize;

		public readonly AcceptMessageType OofSettings;

		private readonly int Flags;

		public readonly bool UseSimpleDisplayName;

		public readonly int MessageCountThreshold;

		private readonly SmtpDomainWithSubdomains domain;
	}
}
