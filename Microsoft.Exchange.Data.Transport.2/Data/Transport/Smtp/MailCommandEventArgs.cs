using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Transport.Smtp
{
	public class MailCommandEventArgs : ReceiveCommandEventArgs
	{
		internal MailCommandEventArgs()
		{
		}

		internal MailCommandEventArgs(SmtpSession smtpSession) : base(smtpSession)
		{
		}

		public string Auth
		{
			get
			{
				return this.auth;
			}
			set
			{
				this.auth = value;
			}
		}

		public BodyType BodyType
		{
			get
			{
				return this.body;
			}
			set
			{
				this.body = value;
			}
		}

		public string EnvelopeId
		{
			get
			{
				return this.envId;
			}
			set
			{
				this.envId = value;
			}
		}

		public RoutingAddress FromAddress
		{
			get
			{
				return this.fromAddress;
			}
			set
			{
				if (!value.IsValid)
				{
					throw new ArgumentException(string.Format("The specified address is an invalid SMTP address - {0}", value));
				}
				this.fromAddress = value;
			}
		}

		public IDictionary<string, object> MailItemProperties
		{
			get
			{
				return this.properties;
			}
		}

		public DsnFormatRequested DsnFormatRequested
		{
			get
			{
				return this.ret;
			}
			set
			{
				this.ret = value;
			}
		}

		public long Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.size = value;
			}
		}

		public bool SmtpUtf8
		{
			get
			{
				return this.smtpUtf8;
			}
			set
			{
				this.smtpUtf8 = value;
			}
		}

		public string Oorg
		{
			get
			{
				return this.oorg;
			}
			set
			{
				if (!string.IsNullOrEmpty(value) && !RoutingAddress.IsValidDomain(value))
				{
					throw new ArgumentException("Invalid originator organization value '{0}'. Originator organizations should be valid SMTP domains, like 'contoso.com'", value);
				}
				this.oorg = value;
			}
		}

		internal Guid SystemProbeId
		{
			get
			{
				return this.systemProbeId;
			}
			set
			{
				this.systemProbeId = value;
			}
		}

		internal RoutingAddress OriginalFromAddress
		{
			get
			{
				return this.originalFromAddress;
			}
			set
			{
				this.originalFromAddress = value;
			}
		}

		internal Dictionary<string, string> ConsumerMailOptionalArguments { get; set; }

		private string auth;

		private BodyType body;

		private string envId;

		private RoutingAddress fromAddress;

		private readonly IDictionary<string, object> properties = new Dictionary<string, object>();

		private DsnFormatRequested ret;

		private long size;

		private string oorg;

		private Guid systemProbeId;

		private bool smtpUtf8;

		private RoutingAddress originalFromAddress;
	}
}
