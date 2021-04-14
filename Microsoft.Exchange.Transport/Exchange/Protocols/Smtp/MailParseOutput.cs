using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class MailParseOutput
	{
		public MailParseOutput(RoutingAddress fromAddress)
		{
			this.FromAddress = fromAddress;
			this.ConsumerMailOptionalArguments = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
		}

		public string Auth { get; set; }

		public MailDirectionality Directionality { get; set; }

		public DsnFormat DsnFormat { get; set; }

		public string EnvelopeId { get; set; }

		public RoutingAddress FromAddress { get; private set; }

		public string InternetMessageId { get; set; }

		public BodyType MailBodyType { get; set; }

		public MailCommandMessageContextParameters MessageContextParameters { get; set; }

		public string Oorg { get; set; }

		public RoutingAddress OriginalFromAddress { get; set; }

		public Guid ShadowMessageId { get; set; }

		public long Size { get; set; }

		public bool SmtpUtf8 { get; set; }

		public Guid SystemProbeId { get; set; }

		public MailParseOutput.XAttrOrgIdData XAttrOrgId { get; set; }

		public string XShadow { get; set; }

		public Dictionary<string, string> ConsumerMailOptionalArguments { get; private set; }

		public class XAttrOrgIdData
		{
			public string ExoAccountForest { get; set; }

			public string ExoTenantContainer { get; set; }

			public Guid ExternalOrgId { get; set; }

			public OrganizationId InternalOrgId { get; set; }
		}
	}
}
