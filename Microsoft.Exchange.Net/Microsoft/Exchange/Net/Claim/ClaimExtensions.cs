using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Claims;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net.Claim
{
	internal static class ClaimExtensions
	{
		public static bool PossessClaimType(this ReadOnlyCollection<ClaimSet> claimSets, string desiredClaimType)
		{
			foreach (ClaimSet claimSet in claimSets)
			{
				foreach (Claim claim in claimSet)
				{
					if (claim.Match(desiredClaimType, Rights.PossessProperty))
					{
						return true;
					}
				}
			}
			return false;
		}

		public static bool Match(this Claim claim, string claimTypeToTest, string rightToTest)
		{
			return claim.ClaimType == claimTypeToTest && claim.Right == rightToTest;
		}

		public static bool HaveProperResource<T>(this Claim claim, out T resource) where T : class
		{
			resource = default(T);
			if (claim.Resource == null)
			{
				ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string>(0L, "[ClaimHelper::HaveProperResource] {0}/{1} claim had a null Resource", claim.ClaimType, claim.Right);
				return false;
			}
			resource = (claim.Resource as T);
			if (resource == null)
			{
				ExTraceGlobals.ClaimTracer.TraceDebug(0L, "[ClaimHelper::HaveProperResource] {0}/{1} claim had a claim of type {2}, not of type {3}", new object[]
				{
					claim.ClaimType,
					claim.Right,
					claim.Resource.GetType().FullName,
					typeof(T).FullName
				});
				return false;
			}
			return true;
		}

		public static void TraceClaimSets(this ReadOnlyCollection<ClaimSet> claimSets)
		{
			if (!ExTraceGlobals.ClaimTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				return;
			}
			ExTraceGlobals.ClaimTracer.TraceDebug<string>(0L, "ClaimSets: {0}", claimSets.GetTraceString());
		}
	}
}
