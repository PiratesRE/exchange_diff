using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Net.Claim;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Security.X509CertAuth
{
	internal sealed class X509CertUser
	{
		private X509CertUser(X509Identifier identifier)
		{
			this.identifier = identifier;
		}

		public string UserPrincipalName
		{
			get
			{
				return this.userPrincipalName;
			}
		}

		public static bool TryCreateX509CertUser(ReadOnlyCollection<ClaimSet> claimSets, out X509CertUser certUser)
		{
			certUser = null;
			int count = claimSets.Count;
			if (count != 1)
			{
				ExTraceGlobals.X509CertAuthTracer.TraceDebug<int>(0L, "[X509CertUser::TryCreateX509CertUser] the given claim sets has {0} members, expect only 1", count);
				return false;
			}
			ClaimSet claimSet = claimSets[0];
			X500DistinguishedName x500DistinguishedName = null;
			foreach (Claim claim in claimSet)
			{
				if (claim.Match(ClaimTypes.X500DistinguishedName, Rights.PossessProperty))
				{
					claim.HaveProperResource(out x500DistinguishedName);
					break;
				}
			}
			if (x500DistinguishedName == null)
			{
				ExTraceGlobals.X509CertAuthTracer.TraceDebug<int>(0L, "[X509CertUser::TryCreateX509CertUser] unable to find the subject's X500DistinguishedName in the claim set.", count);
				return false;
			}
			X500DistinguishedName x500DistinguishedName2 = null;
			foreach (Claim claim2 in claimSet.Issuer)
			{
				if (claim2.Match(ClaimTypes.X500DistinguishedName, Rights.PossessProperty))
				{
					claim2.HaveProperResource(out x500DistinguishedName2);
					break;
				}
			}
			if (x500DistinguishedName2 == null)
			{
				ExTraceGlobals.X509CertAuthTracer.TraceDebug<int>(0L, "[X509CertUser::TryCreateX509CertUser] unable to find the issuer's X500DistinguishedName in the claim set.", count);
				return false;
			}
			string name = x500DistinguishedName.Name;
			ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(2634427709U, ref name);
			X509Identifier arg = new X509Identifier(x500DistinguishedName2.Name, name);
			ExTraceGlobals.X509CertAuthTracer.TraceDebug<X509Identifier>(0L, "[X509CertUser::TryCreateX509CertUser] the X509Identifier from this claim set is {0}.", arg);
			certUser = new X509CertUser(arg);
			return true;
		}

		public bool TryGetWindowsIdentity(out OrganizationId organizationId, out WindowsIdentity windowsIdentity, out string errorReason)
		{
			organizationId = null;
			windowsIdentity = null;
			errorReason = null;
			string text = null;
			X509CertUserCache.ResolvedX509CertUser resolvedX509CertUser = X509CertUserCache.Singleton.Get(this.identifier);
			if (string.IsNullOrEmpty(resolvedX509CertUser.ErrorString))
			{
				try
				{
					organizationId = resolvedX509CertUser.OrganizationId;
					this.userPrincipalName = resolvedX509CertUser.UserPrincipalName;
					text = resolvedX509CertUser.ImplicitUserPrincipalName;
					windowsIdentity = new WindowsIdentity(text);
					return true;
				}
				catch (SecurityException ex)
				{
					ExTraceGlobals.X509CertAuthTracer.TraceError<X509Identifier, string, string>(0L, "[X509CertUser::TryGetWindowsPrincipal] unable to get WindowsIdentity for X509Identifier '{0}', implicit upn {1}, exception: {2}", this.identifier, text, ex.Message);
					return false;
				}
			}
			errorReason = resolvedX509CertUser.ErrorString;
			ExTraceGlobals.X509CertAuthTracer.TraceDebug<X509Identifier, string>(0L, "[X509CertUser::TryGetWindowsPrincipal] unable to get ADUser for X509Identifier '{0}', reason: {1}", this.identifier, errorReason);
			return false;
		}

		public bool TryGetSidBasedIdentity(out OrganizationId organizationId, out SidBasedIdentity sidBasedIdentity, out string errorReason)
		{
			organizationId = null;
			sidBasedIdentity = null;
			errorReason = null;
			X509CertUserCache.ResolvedX509CertUser resolvedX509CertUser = X509CertUserCache.Singleton.Get(this.identifier);
			if (string.IsNullOrEmpty(resolvedX509CertUser.ErrorString))
			{
				sidBasedIdentity = new SidBasedIdentity(resolvedX509CertUser.UserPrincipalName, resolvedX509CertUser.Sid.ToString(), resolvedX509CertUser.UserPrincipalName);
				this.userPrincipalName = resolvedX509CertUser.UserPrincipalName;
				sidBasedIdentity.UserOrganizationId = resolvedX509CertUser.OrganizationId;
				return true;
			}
			errorReason = resolvedX509CertUser.ErrorString;
			ExTraceGlobals.X509CertAuthTracer.TraceDebug<X509Identifier, string>(0L, "[X509CertUser::TryGetSidBasedIdentity] unable to get ADUser for X509Identifier '{0}', reason: {1}", this.identifier, errorReason);
			return false;
		}

		public override string ToString()
		{
			return string.Format("{0}({1})", this.identifier, this.userPrincipalName);
		}

		private readonly X509Identifier identifier;

		private string userPrincipalName = "<unknown>";
	}
}
