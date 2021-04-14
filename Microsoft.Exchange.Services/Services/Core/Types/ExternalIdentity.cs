using System;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.InfoWorker.Common.Sharing;
using Microsoft.Exchange.Net.WSSecurity;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ExternalIdentity : WindowsIdentity
	{
		public new string AuthenticationType
		{
			get
			{
				return "External";
			}
		}

		public override bool IsAuthenticated
		{
			get
			{
				return true;
			}
		}

		public override string Name
		{
			get
			{
				return this.emailAddress.ToString();
			}
		}

		public SmtpAddress EmailAddress
		{
			get
			{
				return this.emailAddress;
			}
		}

		public SmtpAddress ExternalId
		{
			get
			{
				return this.externalId;
			}
		}

		public Offer Offer
		{
			get
			{
				return this.offer;
			}
		}

		public WSSecurityHeader WSSecurityHeader
		{
			get
			{
				return this.wsSecurityHeader;
			}
		}

		public SharingSecurityHeader SharingSecurityHeader
		{
			get
			{
				return this.sharingSecurityHeader;
			}
		}

		internal ExternalIdentity(SmtpAddress emailAddress, SmtpAddress externalId, WSSecurityHeader wsSecurityHeader, SharingSecurityHeader sharingSecurityHeader, Offer offer, IntPtr token) : base(token)
		{
			this.emailAddress = emailAddress;
			this.externalId = externalId;
			this.wsSecurityHeader = wsSecurityHeader;
			this.sharingSecurityHeader = sharingSecurityHeader;
			this.offer = offer;
		}

		private const string ExternalAuthenticationType = "External";

		private SmtpAddress emailAddress;

		private SmtpAddress externalId;

		private Offer offer;

		private WSSecurityHeader wsSecurityHeader;

		private SharingSecurityHeader sharingSecurityHeader;
	}
}
