using System;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.TextProcessing
{
	internal class RegexMatch : IMatch
	{
		internal RegexMatch(string pattern, CaseSensitivityMode caseSensitivityMode, MatchRegexOptions options, TimeSpan matchTimeout)
		{
			this.pattern = pattern;
			this.options = options;
			this.caseSensitivityMode = caseSensitivityMode;
			this.useCache = options.HasFlag(MatchRegexOptions.Cached);
			this.regex = this.CreateRegex(pattern, caseSensitivityMode, options, matchTimeout);
		}

		internal RegexMatch(RegexMatch original)
		{
			this.pattern = original.pattern;
			this.options = original.options;
			this.caseSensitivityMode = original.caseSensitivityMode;
			this.useCache = original.useCache;
			if (this.options.HasFlag(MatchRegexOptions.LazyOptimize))
			{
				this.options &= ~MatchRegexOptions.LazyOptimize;
				this.regex = this.CreateRegex(this.pattern, this.caseSensitivityMode, this.options, original.regex.MatchTimeout);
				return;
			}
			this.regex = original.regex;
		}

		internal long Identifier
		{
			get
			{
				return this.id;
			}
		}

		public bool IsMatch(TextScanContext data)
		{
			if (this.useCache)
			{
				bool result;
				if (!data.TryGetCachedResult(this.id, out result))
				{
					result = this.ExecuteRegex(data);
					data.SetCachedResult(this.id, result);
				}
				return result;
			}
			return this.ExecuteRegex(data);
		}

		private Regex CreateRegex(string pattern, CaseSensitivityMode caseSensitivityMode, MatchRegexOptions options, TimeSpan matchTimeout)
		{
			Regex regex = new Regex(pattern, this.GetRegexOptions(caseSensitivityMode, options), matchTimeout);
			if (options.HasFlag(MatchRegexOptions.Primed) && !options.HasFlag(MatchRegexOptions.LazyOptimize))
			{
				regex.IsMatch("Received: from server.domain.domain.domain.domain (1.2.3.4) by\nserver.domain.domain.domain (1.1.1.1) with Microsoft\nSMTP Server (TLS) id 1.1.1.1 via Mailbox Transport; 1 Jan 2000\n11:08:40 +0000\nX-ExtLoop1: 1\nd=\"pdf'?scan'208,217\";a=\"139187562\"\nWed, 01 Jan 2000 12:26:15 +0000\nFrom: test@microsoft.com\nTo: abc.microsoft.com\nSubject: test subject\nDate: Mon, 1 Jan 2000 01:00:00 +0000\nMessage-ID: <5F15F3000D504c3689DF5FF6E0FFB429@BGSMSX102.domain.domain>\nAccept-Language: en-US\nContent-Language: en-US\nX-MS-Has-Attach: yes\nX-MS-TNEF-Correlator:\nx-originating-ip: [1.2.1.1]\nContent-Type: multipart/mixed;\nboundary=\"BA698262-28A9-4de8-9A78-FA9632EDEEBF\"\nMIME-Version: 1.0\nReturn-Path: test@abc.com\nX-OrganizationHeadersPreserved: DF-H14-01.exchange.corp.microsoft.com\nX-Forefront-Antispam-Report: CIP:1.1.1.2;IPV:NLI;EFV:NLI;SFV:SZE;SFS:;DIR:OUT;LANG:en;\nX-CrossPremisesHeadersPromoted: domain\nX-CrossPremisesHeadersFiltered: domain\nX-MS-Exchange-Organization-Network-Message-Id: 3BDE8C5D-F9E2-4959-9959-13294825ACEC\nX-MS-Exchange-Organization-AVStamp-Service: 1.0\nX-MS-Exchange-Organization-SCL: 0\nX-MS-Exchange-Organization-AuthSource:\nX-MS-Exchange-Organization-AuthAs: Anonymous\nX-OriginatorOrg: domain.domain.domain\nmail from: test@test.com\nrcpt to: test@test.com\nsubject: test message\nReceived: from ABCDEFG.xyz.microsoft.com (1.1.1.1) by\nabcdefghijklmnopqrstuvwxyz\nABCDEFGHIJKLMNOPQRSTUVWXYZ\n1234567890\n-=[];',./_+{}:\"<>?\n\\aaaaaaaaabbbbbbbbbcccccccc000000000011111111122222222---------      ::::::::\n`~!@#$%^&*()_+|\t \n{975FF651-E46D-49d3-8DBD-A65513698885}{F1FFF2FC-EA90-4dc5-BF46-8F796065AF35}\nReceived: from SN2FFOFD003.ffo.gbl (157.55.158.24) by\n BL2SR01CA105.outlook.office365.com (10.255.109.150) with Microsoft SMTP\n Server (TLS) id 15.0.805.1 via Frontend Transport; Fri, 4 Oct 2013 22:46:44\n +0000\nReceived: from hybrid.exchange.microsoft.com (131.107.1.17) by\n SN2FFOFD003.mail.o365filtering.com (10.111.201.40) with Microsoft SMTP Server\n (TLS) id 15.0.795.3 via Frontend Transport; Fri, 4 Oct 2013 22:46:43 +0000\nReceived: from mail121-db9-R.bigfish.com (157.54.51.113) by mail.microsoft.com\n (157.54.80.67) with Microsoft SMTP Server (TLS) id 14.3.136.1; Fri, 4 Oct\n 2013 22:45:31 +0000");
				regex.IsMatch(pattern);
			}
			return regex;
		}

		private bool ExecuteRegex(TextScanContext context)
		{
			return this.regex.IsMatch((CaseSensitivityMode.InsensitiveUsingNormalization == this.caseSensitivityMode) ? context.NormalizedData : context.Data);
		}

		private RegexOptions GetRegexOptions(CaseSensitivityMode caseSensitivityMode, MatchRegexOptions options)
		{
			RegexOptions regexOptions = RegexOptions.None;
			if (CaseSensitivityMode.Insensitive == caseSensitivityMode)
			{
				regexOptions |= RegexOptions.IgnoreCase;
			}
			if (options.HasFlag(MatchRegexOptions.Compiled) && !options.HasFlag(MatchRegexOptions.LazyOptimize))
			{
				regexOptions |= RegexOptions.Compiled;
			}
			if (options.HasFlag(MatchRegexOptions.ExplicitCaptures))
			{
				regexOptions |= RegexOptions.ExplicitCapture;
			}
			if (options.HasFlag(MatchRegexOptions.CultureInvariant))
			{
				regexOptions |= RegexOptions.CultureInvariant;
			}
			return regexOptions;
		}

		private const string RegexPrimingText = "Received: from server.domain.domain.domain.domain (1.2.3.4) by\nserver.domain.domain.domain (1.1.1.1) with Microsoft\nSMTP Server (TLS) id 1.1.1.1 via Mailbox Transport; 1 Jan 2000\n11:08:40 +0000\nX-ExtLoop1: 1\nd=\"pdf'?scan'208,217\";a=\"139187562\"\nWed, 01 Jan 2000 12:26:15 +0000\nFrom: test@microsoft.com\nTo: abc.microsoft.com\nSubject: test subject\nDate: Mon, 1 Jan 2000 01:00:00 +0000\nMessage-ID: <5F15F3000D504c3689DF5FF6E0FFB429@BGSMSX102.domain.domain>\nAccept-Language: en-US\nContent-Language: en-US\nX-MS-Has-Attach: yes\nX-MS-TNEF-Correlator:\nx-originating-ip: [1.2.1.1]\nContent-Type: multipart/mixed;\nboundary=\"BA698262-28A9-4de8-9A78-FA9632EDEEBF\"\nMIME-Version: 1.0\nReturn-Path: test@abc.com\nX-OrganizationHeadersPreserved: DF-H14-01.exchange.corp.microsoft.com\nX-Forefront-Antispam-Report: CIP:1.1.1.2;IPV:NLI;EFV:NLI;SFV:SZE;SFS:;DIR:OUT;LANG:en;\nX-CrossPremisesHeadersPromoted: domain\nX-CrossPremisesHeadersFiltered: domain\nX-MS-Exchange-Organization-Network-Message-Id: 3BDE8C5D-F9E2-4959-9959-13294825ACEC\nX-MS-Exchange-Organization-AVStamp-Service: 1.0\nX-MS-Exchange-Organization-SCL: 0\nX-MS-Exchange-Organization-AuthSource:\nX-MS-Exchange-Organization-AuthAs: Anonymous\nX-OriginatorOrg: domain.domain.domain\nmail from: test@test.com\nrcpt to: test@test.com\nsubject: test message\nReceived: from ABCDEFG.xyz.microsoft.com (1.1.1.1) by\nabcdefghijklmnopqrstuvwxyz\nABCDEFGHIJKLMNOPQRSTUVWXYZ\n1234567890\n-=[];',./_+{}:\"<>?\n\\aaaaaaaaabbbbbbbbbcccccccc000000000011111111122222222---------      ::::::::\n`~!@#$%^&*()_+|\t \n{975FF651-E46D-49d3-8DBD-A65513698885}{F1FFF2FC-EA90-4dc5-BF46-8F796065AF35}\nReceived: from SN2FFOFD003.ffo.gbl (157.55.158.24) by\n BL2SR01CA105.outlook.office365.com (10.255.109.150) with Microsoft SMTP\n Server (TLS) id 15.0.805.1 via Frontend Transport; Fri, 4 Oct 2013 22:46:44\n +0000\nReceived: from hybrid.exchange.microsoft.com (131.107.1.17) by\n SN2FFOFD003.mail.o365filtering.com (10.111.201.40) with Microsoft SMTP Server\n (TLS) id 15.0.795.3 via Frontend Transport; Fri, 4 Oct 2013 22:46:43 +0000\nReceived: from mail121-db9-R.bigfish.com (157.54.51.113) by mail.microsoft.com\n (157.54.80.67) with Microsoft SMTP Server (TLS) id 14.3.136.1; Fri, 4 Oct\n 2013 22:45:31 +0000";

		private readonly long id = IDGenerator.GetNextID();

		private readonly bool useCache;

		private readonly CaseSensitivityMode caseSensitivityMode;

		private readonly string pattern;

		private readonly MatchRegexOptions options;

		private Regex regex;
	}
}
