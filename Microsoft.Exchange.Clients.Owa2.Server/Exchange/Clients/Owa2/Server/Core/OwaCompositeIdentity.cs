using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public sealed class OwaCompositeIdentity : OwaIdentity
	{
		private OwaCompositeIdentity(OwaIdentity primaryIdentity, IEnumerable<OwaIdentity> secondaryIdentities)
		{
			if (primaryIdentity == null)
			{
				throw new ArgumentNullException("primaryIdentity", "The primary identity must not be null!");
			}
			this.primaryIdentity = primaryIdentity;
			base.UserOrganizationId = primaryIdentity.UserOrganizationId;
			base.OwaMiniRecipient = primaryIdentity.OwaMiniRecipient;
			IReadOnlyList<OwaIdentity> readOnlyList;
			if (secondaryIdentities != null)
			{
				readOnlyList = (from i in secondaryIdentities
				orderby i.UserSid
				select i).ToArray<OwaIdentity>();
			}
			else
			{
				readOnlyList = new OwaIdentity[0];
			}
			this.secondaryIdentities = readOnlyList;
		}

		public static OwaCompositeIdentity CreateFromCompositeIdentity(CompositeIdentity compositeIdentity)
		{
			if (compositeIdentity == null)
			{
				throw new ArgumentNullException("compositeIdentity", "You must specify the source CompositeIdentity.");
			}
			OwaIdentity owaIdentity = OwaIdentity.GetOwaIdentity(compositeIdentity.PrimaryIdentity);
			if (owaIdentity == null)
			{
				ExTraceGlobals.CoreCallTracer.TraceError(0L, "[OwaIdentity::CreateFromCompositeIdentity] - failed to resolve primary identity.");
				throw new OwaIdentityException("Cannot create security context for the specified composite identity. Failed to resolve the primary identity.");
			}
			OwaIdentity[] array = new OwaIdentity[compositeIdentity.SecondaryIdentitiesCount];
			int num = 0;
			foreach (IIdentity identity in compositeIdentity.SecondaryIdentities)
			{
				array[num] = OwaIdentity.GetOwaIdentity(identity);
				if (array[num] == null)
				{
					ExTraceGlobals.CoreCallTracer.TraceError(0L, string.Format("[OwaIdentity::CreateFromCompositeIdentity] - failed to resolve secondary identity {0}.", num));
					throw new OwaIdentityException(string.Format("Cannot create security context for the specified composite identity. Failed to resolve a secondary identity {0}.", num));
				}
				num++;
			}
			return new OwaCompositeIdentity(owaIdentity, array);
		}

		internal static OwaIdentity CreateFromAuthZClientInfo(AuthZClientInfo authZClientInfo)
		{
			if (authZClientInfo == null)
			{
				throw new ArgumentNullException("authZClientInfo", "You must specify the source AuthZClientInfo.");
			}
			OwaIdentity owaIdentity = OwaClientSecurityContextIdentity.CreateFromClientSecurityContext(authZClientInfo.ClientSecurityContext, authZClientInfo.PrimarySmtpAddress, "OverrideClientSecurityContext");
			if (owaIdentity == null)
			{
				ExTraceGlobals.CoreCallTracer.TraceError(0L, "[OwaIdentity::CreateFromAuthZClientInfo] - was unable to create the security context for " + authZClientInfo.PrimarySmtpAddress);
				throw new OwaIdentityException("Cannot create security context for the specified identity. Failed to resolve the identity " + authZClientInfo.PrimarySmtpAddress);
			}
			if (authZClientInfo.SecondaryClientInfoItems.Count > 0)
			{
				OwaIdentity[] array = new OwaIdentity[authZClientInfo.SecondaryClientInfoItems.Count];
				int num = 0;
				foreach (AuthZClientInfo authZClientInfo2 in authZClientInfo.SecondaryClientInfoItems)
				{
					array[num] = OwaClientSecurityContextIdentity.CreateFromClientSecurityContext(authZClientInfo2.ClientSecurityContext, authZClientInfo2.PrimarySmtpAddress, "OverrideClientSecurityContext");
					if (array[num] == null)
					{
						ExTraceGlobals.CoreCallTracer.TraceError(0L, "[OwaIdentity::CreateFromAuthZClientInfo] - was unable to create the security context for composite identity. Failed to resolve secondary identity " + authZClientInfo2.PrimarySmtpAddress);
						throw new OwaIdentityException(string.Format("Cannot create security context for the specified composite identity. Failed to resolve the secondary identity {0}: {1}.", num, authZClientInfo2.PrimarySmtpAddress));
					}
					num++;
				}
				owaIdentity = new OwaCompositeIdentity(owaIdentity, array);
			}
			return owaIdentity;
		}

		public IReadOnlyList<OwaIdentity> SecondaryIdentities
		{
			get
			{
				return this.secondaryIdentities;
			}
		}

		public override WindowsIdentity WindowsIdentity
		{
			get
			{
				return this.primaryIdentity.WindowsIdentity;
			}
		}

		public override SecurityIdentifier UserSid
		{
			get
			{
				return this.primaryIdentity.UserSid;
			}
		}

		public override string AuthenticationType
		{
			get
			{
				return this.primaryIdentity.AuthenticationType;
			}
		}

		public override string UniqueId
		{
			get
			{
				return this.primaryIdentity.UniqueId;
			}
		}

		public override bool IsPartial
		{
			get
			{
				return this.primaryIdentity.IsPartial;
			}
		}

		internal override ClientSecurityContext ClientSecurityContext
		{
			get
			{
				return this.primaryIdentity.ClientSecurityContext;
			}
		}

		public override string GetLogonName()
		{
			return this.primaryIdentity.GetLogonName();
		}

		public override string SafeGetRenderableName()
		{
			return this.primaryIdentity.SafeGetRenderableName();
		}

		internal override ExchangePrincipal InternalCreateExchangePrincipal()
		{
			return this.primaryIdentity.InternalCreateExchangePrincipal();
		}

		internal override MailboxSession CreateMailboxSession(ExchangePrincipal exchangePrincipal, CultureInfo cultureInfo)
		{
			return this.primaryIdentity.CreateMailboxSession(exchangePrincipal, cultureInfo);
		}

		internal override MailboxSession CreateInstantSearchMailboxSession(ExchangePrincipal exchangePrincipal, CultureInfo cultureInfo)
		{
			return this.primaryIdentity.CreateInstantSearchMailboxSession(exchangePrincipal, cultureInfo);
		}

		internal override MailboxSession CreateDelegateMailboxSession(ExchangePrincipal exchangePrincipal, CultureInfo cultureInfo)
		{
			return this.primaryIdentity.CreateDelegateMailboxSession(exchangePrincipal, cultureInfo);
		}

		public override bool IsEqualsTo(OwaIdentity otherIdentity)
		{
			OwaCompositeIdentity owaCompositeIdentity = otherIdentity as OwaCompositeIdentity;
			if (owaCompositeIdentity == null)
			{
				return false;
			}
			bool flag = otherIdentity.UserSid.Equals(this.UserSid);
			if (flag)
			{
				if (this.secondaryIdentities.Count != this.secondaryIdentities.Count)
				{
					return false;
				}
				for (int i = 0; i < this.secondaryIdentities.Count; i++)
				{
					if (!this.secondaryIdentities[i].IsEqualsTo(owaCompositeIdentity.secondaryIdentities[i]))
					{
						return false;
					}
				}
			}
			return flag;
		}

		private readonly OwaIdentity primaryIdentity;

		private readonly IReadOnlyList<OwaIdentity> secondaryIdentities;
	}
}
