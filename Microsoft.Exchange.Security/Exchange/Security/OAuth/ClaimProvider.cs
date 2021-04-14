using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Security.OAuth
{
	internal abstract class ClaimProvider
	{
		protected ClaimProvider(OrganizationId organizationId)
		{
			this.organizationId = organizationId;
		}

		public static ClaimProvider Create(ADUser adUser)
		{
			return new ClaimProvider.ADUserClaimProvider(adUser);
		}

		public static ClaimProvider Create(MiniRecipient miniRecipient)
		{
			return new ClaimProvider.MiniRecipientClaimProvider(miniRecipient);
		}

		public bool IsAllowedToIncludeNameId { get; set; }

		public bool IncludeNameIdOnly
		{
			get
			{
				return this.includeNameIdOnly;
			}
			set
			{
				this.includeNameIdOnly = value;
				this.upnClaim = (this.smtpClaim = (this.sipClaim = null));
			}
		}

		public Dictionary<string, string> GetClaims()
		{
			if (AuthCommon.IsWindowsLiveIDEnabled)
			{
				this.FindNameIdClaimForOnline();
			}
			else
			{
				this.FindNameIdClaimForOnPremise();
			}
			if (!this.IncludeNameIdOnly)
			{
				if (AuthCommon.IsWindowsLiveIDEnabled)
				{
					this.FindUPNClaimForOnline();
				}
				else
				{
					this.FindUPNClaimForOnPremise();
				}
				this.FindSmtpClaim();
				this.FindSipClaim();
			}
			else if (string.IsNullOrEmpty(this.nameidClaim))
			{
				throw new OAuthTokenRequestFailedException(OAuthOutboundErrorCodes.EmptyNameIdClaim, null, null);
			}
			return this.GetUserClaimsHelper();
		}

		protected abstract void FindNameIdClaimForOnPremise();

		protected abstract void FindUPNClaimForOnPremise();

		protected abstract void FindNameIdClaimForOnline();

		protected abstract void FindUPNClaimForOnline();

		protected abstract void FindSmtpClaim();

		protected abstract void FindSipClaim();

		private Dictionary<string, string> GetUserClaimsHelper()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>(5);
			if (!string.IsNullOrEmpty(this.nameidClaim) && this.IsAllowedToIncludeNameId)
			{
				dictionary.Add(Constants.ClaimTypes.NameIdentifier, this.nameidClaim);
			}
			if (!string.IsNullOrEmpty(this.upnClaim))
			{
				dictionary.Add(Constants.ClaimTypes.Upn, this.upnClaim);
			}
			if (!string.IsNullOrEmpty(this.smtpClaim))
			{
				dictionary.Add(Constants.ClaimTypes.Smtp, this.smtpClaim);
			}
			if (!string.IsNullOrEmpty(this.sipClaim))
			{
				dictionary.Add(Constants.ClaimTypes.Sip, this.sipClaim);
			}
			if (dictionary.Count == 0)
			{
				throw new OAuthTokenRequestFailedException(OAuthOutboundErrorCodes.EmptyClaimsForUser, null, null);
			}
			if (!string.IsNullOrEmpty(this.nameidClaim) && this.IsAllowedToIncludeNameId)
			{
				dictionary.Add(Constants.ClaimTypes.Nii, this.niiClaim);
			}
			return dictionary;
		}

		protected readonly OrganizationId organizationId;

		protected string niiClaim;

		protected string nameidClaim;

		protected string upnClaim;

		protected string smtpClaim;

		protected string sipClaim;

		private bool includeNameIdOnly;

		private class ADUserClaimProvider : ClaimProvider
		{
			public ADUserClaimProvider(ADUser user) : base(user.OrganizationId)
			{
				this.adUser = user;
			}

			protected override void FindNameIdClaimForOnPremise()
			{
				SecurityIdentifier securityIdentifier = this.adUser.IsLinked ? this.adUser.MasterAccountSid : this.adUser.Sid;
				if (securityIdentifier != null)
				{
					this.niiClaim = Constants.NiiClaimValues.ActiveDirectory;
					this.nameidClaim = securityIdentifier.ToString();
				}
			}

			protected override void FindNameIdClaimForOnline()
			{
				SmtpAddress windowsLiveID = this.adUser.WindowsLiveID;
				if (windowsLiveID == SmtpAddress.Empty)
				{
					return;
				}
				LiveIdInstanceType? liveIdInstanceType = DomainPropertyCache.Singleton.Get(new SmtpDomainWithSubdomains(windowsLiveID.Domain)).LiveIdInstanceType;
				if (liveIdInstanceType != null && liveIdInstanceType.Value == LiveIdInstanceType.Business)
				{
					this.niiClaim = Constants.NiiClaimValues.BusinessLiveId;
					this.nameidClaim = this.adUser.NetID.ToString();
				}
			}

			protected override void FindUPNClaimForOnPremise()
			{
				this.upnClaim = this.adUser.UserPrincipalName;
			}

			protected override void FindUPNClaimForOnline()
			{
				SmtpAddress windowsLiveID = this.adUser.WindowsLiveID;
				this.upnClaim = this.adUser.WindowsLiveID.ToString();
			}

			protected override void FindSmtpClaim()
			{
				SmtpAddress primarySmtpAddress = this.adUser.PrimarySmtpAddress;
				if (primarySmtpAddress != SmtpAddress.Empty)
				{
					this.smtpClaim = primarySmtpAddress.ToString();
				}
			}

			protected override void FindSipClaim()
			{
				ProxyAddressCollection emailAddresses = this.adUser.EmailAddresses;
				if (emailAddresses != null)
				{
					ProxyAddress proxyAddress = emailAddresses.Find((ProxyAddress p) => p.Prefix == ProxyAddressPrefix.SIP);
					if (proxyAddress != null)
					{
						this.sipClaim = proxyAddress.AddressString;
					}
				}
			}

			private readonly ADUser adUser;
		}

		private class MiniRecipientClaimProvider : ClaimProvider
		{
			public MiniRecipientClaimProvider(MiniRecipient miniRecipient) : base(miniRecipient.OrganizationId)
			{
				this.miniRecipient = miniRecipient;
			}

			protected override void FindNameIdClaimForOnPremise()
			{
				if (this.miniRecipient.MasterAccountSid != null)
				{
					this.niiClaim = Constants.NiiClaimValues.ActiveDirectory;
					this.nameidClaim = this.miniRecipient.MasterAccountSid.ToString();
					return;
				}
				if (this.miniRecipient.Sid != null)
				{
					this.niiClaim = Constants.NiiClaimValues.ActiveDirectory;
					this.nameidClaim = this.miniRecipient.Sid.ToString();
				}
			}

			protected override void FindNameIdClaimForOnline()
			{
				SmtpAddress windowsLiveID = this.miniRecipient.WindowsLiveID;
				if (windowsLiveID == SmtpAddress.Empty)
				{
					return;
				}
				LiveIdInstanceType? liveIdInstanceType = DomainPropertyCache.Singleton.Get(new SmtpDomainWithSubdomains(windowsLiveID.Domain)).LiveIdInstanceType;
				if (liveIdInstanceType != null && liveIdInstanceType.Value == LiveIdInstanceType.Business)
				{
					this.niiClaim = Constants.NiiClaimValues.BusinessLiveId;
					this.nameidClaim = this.miniRecipient.NetID.ToString();
				}
			}

			protected override void FindUPNClaimForOnPremise()
			{
				this.upnClaim = this.miniRecipient.UserPrincipalName;
			}

			protected override void FindUPNClaimForOnline()
			{
				SmtpAddress windowsLiveID = this.miniRecipient.WindowsLiveID;
				if (windowsLiveID != SmtpAddress.Empty)
				{
					this.upnClaim = windowsLiveID.ToString();
				}
			}

			protected override void FindSmtpClaim()
			{
				this.smtpClaim = this.miniRecipient.PrimarySmtpAddress.ToString();
			}

			protected override void FindSipClaim()
			{
			}

			private readonly MiniRecipient miniRecipient;
		}
	}
}
