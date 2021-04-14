using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;

namespace System.Security
{
	[Serializable]
	internal sealed class PermissionListSet
	{
		internal PermissionListSet()
		{
		}

		private void EnsureTriplesListCreated()
		{
			if (this.m_permSetTriples == null)
			{
				this.m_permSetTriples = new ArrayList();
				if (this.m_firstPermSetTriple != null)
				{
					this.m_permSetTriples.Add(this.m_firstPermSetTriple);
					this.m_firstPermSetTriple = null;
				}
			}
		}

		[SecurityCritical]
		internal void UpdateDomainPLS(PermissionListSet adPLS)
		{
			if (adPLS != null && adPLS.m_firstPermSetTriple != null)
			{
				this.UpdateDomainPLS(adPLS.m_firstPermSetTriple.GrantSet, adPLS.m_firstPermSetTriple.RefusedSet);
			}
		}

		[SecurityCritical]
		internal void UpdateDomainPLS(PermissionSet grantSet, PermissionSet deniedSet)
		{
			if (this.m_firstPermSetTriple == null)
			{
				this.m_firstPermSetTriple = new PermissionSetTriple();
			}
			this.m_firstPermSetTriple.UpdateGrant(grantSet);
			this.m_firstPermSetTriple.UpdateRefused(deniedSet);
		}

		private void Terminate(PermissionSetTriple currentTriple)
		{
			this.UpdateTripleListAndCreateNewTriple(currentTriple, null);
		}

		[SecurityCritical]
		private void Terminate(PermissionSetTriple currentTriple, PermissionListSet pls)
		{
			this.UpdateZoneAndOrigin(pls);
			this.UpdatePermissions(currentTriple, pls);
			this.UpdateTripleListAndCreateNewTriple(currentTriple, null);
		}

		[SecurityCritical]
		private bool Update(PermissionSetTriple currentTriple, PermissionListSet pls)
		{
			this.UpdateZoneAndOrigin(pls);
			return this.UpdatePermissions(currentTriple, pls);
		}

		[SecurityCritical]
		private bool Update(PermissionSetTriple currentTriple, FrameSecurityDescriptor fsd)
		{
			FrameSecurityDescriptorWithResolver frameSecurityDescriptorWithResolver = fsd as FrameSecurityDescriptorWithResolver;
			if (frameSecurityDescriptorWithResolver != null)
			{
				return this.Update2(currentTriple, frameSecurityDescriptorWithResolver);
			}
			bool flag = this.Update2(currentTriple, fsd, false);
			if (!flag)
			{
				flag = this.Update2(currentTriple, fsd, true);
			}
			return flag;
		}

		[SecurityCritical]
		private bool Update2(PermissionSetTriple currentTriple, FrameSecurityDescriptorWithResolver fsdWithResolver)
		{
			DynamicResolver resolver = fsdWithResolver.Resolver;
			CompressedStack securityContext = resolver.GetSecurityContext();
			securityContext.CompleteConstruction(null);
			return this.Update(currentTriple, securityContext.PLS);
		}

		[SecurityCritical]
		private bool Update2(PermissionSetTriple currentTriple, FrameSecurityDescriptor fsd, bool fDeclarative)
		{
			PermissionSet denials = fsd.GetDenials(fDeclarative);
			if (denials != null)
			{
				currentTriple.UpdateRefused(denials);
			}
			PermissionSet permitOnly = fsd.GetPermitOnly(fDeclarative);
			if (permitOnly != null)
			{
				currentTriple.UpdateGrant(permitOnly);
			}
			if (fsd.GetAssertAllPossible())
			{
				if (currentTriple.GrantSet == null)
				{
					currentTriple.GrantSet = PermissionSet.s_fullTrust;
				}
				this.UpdateTripleListAndCreateNewTriple(currentTriple, this.m_permSetTriples);
				currentTriple.GrantSet = PermissionSet.s_fullTrust;
				currentTriple.UpdateAssert(fsd.GetAssertions(fDeclarative));
				return true;
			}
			PermissionSet assertions = fsd.GetAssertions(fDeclarative);
			if (assertions != null)
			{
				if (assertions.IsUnrestricted())
				{
					if (currentTriple.GrantSet == null)
					{
						currentTriple.GrantSet = PermissionSet.s_fullTrust;
					}
					this.UpdateTripleListAndCreateNewTriple(currentTriple, this.m_permSetTriples);
					currentTriple.GrantSet = PermissionSet.s_fullTrust;
					currentTriple.UpdateAssert(assertions);
					return true;
				}
				PermissionSetTriple permissionSetTriple = currentTriple.UpdateAssert(assertions);
				if (permissionSetTriple != null)
				{
					this.EnsureTriplesListCreated();
					this.m_permSetTriples.Add(permissionSetTriple);
				}
			}
			return false;
		}

		[SecurityCritical]
		private void Update(PermissionSetTriple currentTriple, PermissionSet in_g, PermissionSet in_r)
		{
			ZoneIdentityPermission z;
			UrlIdentityPermission u;
			currentTriple.UpdateGrant(in_g, out z, out u);
			currentTriple.UpdateRefused(in_r);
			this.AppendZoneOrigin(z, u);
		}

		[SecurityCritical]
		private void Update(PermissionSet in_g)
		{
			if (this.m_firstPermSetTriple == null)
			{
				this.m_firstPermSetTriple = new PermissionSetTriple();
			}
			this.Update(this.m_firstPermSetTriple, in_g, null);
		}

		private void UpdateZoneAndOrigin(PermissionListSet pls)
		{
			if (pls != null)
			{
				if (this.m_zoneList == null && pls.m_zoneList != null && pls.m_zoneList.Count > 0)
				{
					this.m_zoneList = new ArrayList();
				}
				PermissionListSet.UpdateArrayList(this.m_zoneList, pls.m_zoneList);
				if (this.m_originList == null && pls.m_originList != null && pls.m_originList.Count > 0)
				{
					this.m_originList = new ArrayList();
				}
				PermissionListSet.UpdateArrayList(this.m_originList, pls.m_originList);
			}
		}

		[SecurityCritical]
		private bool UpdatePermissions(PermissionSetTriple currentTriple, PermissionListSet pls)
		{
			if (pls != null)
			{
				if (pls.m_permSetTriples != null)
				{
					this.UpdateTripleListAndCreateNewTriple(currentTriple, pls.m_permSetTriples);
				}
				else
				{
					PermissionSetTriple firstPermSetTriple = pls.m_firstPermSetTriple;
					PermissionSetTriple permissionSetTriple;
					if (currentTriple.Update(firstPermSetTriple, out permissionSetTriple))
					{
						return true;
					}
					if (permissionSetTriple != null)
					{
						this.EnsureTriplesListCreated();
						this.m_permSetTriples.Add(permissionSetTriple);
					}
				}
			}
			else
			{
				this.UpdateTripleListAndCreateNewTriple(currentTriple, null);
			}
			return false;
		}

		private void UpdateTripleListAndCreateNewTriple(PermissionSetTriple currentTriple, ArrayList tripleList)
		{
			if (!currentTriple.IsEmpty())
			{
				if (this.m_firstPermSetTriple == null && this.m_permSetTriples == null)
				{
					this.m_firstPermSetTriple = new PermissionSetTriple(currentTriple);
				}
				else
				{
					this.EnsureTriplesListCreated();
					this.m_permSetTriples.Add(new PermissionSetTriple(currentTriple));
				}
				currentTriple.Reset();
			}
			if (tripleList != null)
			{
				this.EnsureTriplesListCreated();
				this.m_permSetTriples.AddRange(tripleList);
			}
		}

		private static void UpdateArrayList(ArrayList current, ArrayList newList)
		{
			if (newList == null)
			{
				return;
			}
			for (int i = 0; i < newList.Count; i++)
			{
				if (!current.Contains(newList[i]))
				{
					current.Add(newList[i]);
				}
			}
		}

		private void AppendZoneOrigin(ZoneIdentityPermission z, UrlIdentityPermission u)
		{
			if (z != null)
			{
				if (this.m_zoneList == null)
				{
					this.m_zoneList = new ArrayList();
				}
				z.AppendZones(this.m_zoneList);
			}
			if (u != null)
			{
				if (this.m_originList == null)
				{
					this.m_originList = new ArrayList();
				}
				u.AppendOrigin(this.m_originList);
			}
		}

		[SecurityCritical]
		[ComVisible(true)]
		internal static PermissionListSet CreateCompressedState(CompressedStack cs, CompressedStack innerCS)
		{
			bool flag = false;
			if (cs.CompressedStackHandle == null)
			{
				return null;
			}
			PermissionListSet permissionListSet = new PermissionListSet();
			PermissionSetTriple currentTriple = new PermissionSetTriple();
			int dcscount = CompressedStack.GetDCSCount(cs.CompressedStackHandle);
			int num = dcscount - 1;
			while (num >= 0 && !flag)
			{
				DomainCompressedStack domainCompressedStack = CompressedStack.GetDomainCompressedStack(cs.CompressedStackHandle, num);
				if (domainCompressedStack != null)
				{
					if (domainCompressedStack.PLS == null)
					{
						throw new SecurityException(string.Format(CultureInfo.InvariantCulture, Environment.GetResourceString("Security_Generic"), Array.Empty<object>()));
					}
					permissionListSet.UpdateZoneAndOrigin(domainCompressedStack.PLS);
					permissionListSet.Update(currentTriple, domainCompressedStack.PLS);
					flag = domainCompressedStack.ConstructionHalted;
				}
				num--;
			}
			if (!flag)
			{
				PermissionListSet pls = null;
				if (innerCS != null)
				{
					innerCS.CompleteConstruction(null);
					pls = innerCS.PLS;
				}
				permissionListSet.Terminate(currentTriple, pls);
			}
			else
			{
				permissionListSet.Terminate(currentTriple);
			}
			return permissionListSet;
		}

		[SecurityCritical]
		internal static PermissionListSet CreateCompressedState(IntPtr unmanagedDCS, out bool bHaltConstruction)
		{
			PermissionListSet permissionListSet = new PermissionListSet();
			PermissionSetTriple currentTriple = new PermissionSetTriple();
			int descCount = DomainCompressedStack.GetDescCount(unmanagedDCS);
			bHaltConstruction = false;
			int num = 0;
			while (num < descCount && !bHaltConstruction)
			{
				PermissionSet in_g;
				PermissionSet in_r;
				Assembly assembly;
				FrameSecurityDescriptor fsd;
				if (DomainCompressedStack.GetDescriptorInfo(unmanagedDCS, num, out in_g, out in_r, out assembly, out fsd))
				{
					bHaltConstruction = permissionListSet.Update(currentTriple, fsd);
				}
				else
				{
					permissionListSet.Update(currentTriple, in_g, in_r);
				}
				num++;
			}
			if (!bHaltConstruction && !DomainCompressedStack.IgnoreDomain(unmanagedDCS))
			{
				PermissionSet in_g;
				PermissionSet in_r;
				DomainCompressedStack.GetDomainPermissionSets(unmanagedDCS, out in_g, out in_r);
				permissionListSet.Update(currentTriple, in_g, in_r);
			}
			permissionListSet.Terminate(currentTriple);
			return permissionListSet;
		}

		[SecurityCritical]
		internal static PermissionListSet CreateCompressedState_HG()
		{
			PermissionListSet permissionListSet = new PermissionListSet();
			CompressedStack.GetHomogeneousPLS(permissionListSet);
			return permissionListSet;
		}

		[SecurityCritical]
		internal bool CheckDemandNoThrow(CodeAccessPermission demand)
		{
			PermissionToken permToken = null;
			if (demand != null)
			{
				permToken = PermissionToken.GetToken(demand);
			}
			return this.m_firstPermSetTriple.CheckDemandNoThrow(demand, permToken);
		}

		[SecurityCritical]
		internal bool CheckSetDemandNoThrow(PermissionSet pSet)
		{
			return this.m_firstPermSetTriple.CheckSetDemandNoThrow(pSet);
		}

		[SecurityCritical]
		internal bool CheckDemand(CodeAccessPermission demand, PermissionToken permToken, RuntimeMethodHandleInternal rmh)
		{
			bool flag = true;
			if (this.m_permSetTriples != null)
			{
				int num = 0;
				while (num < this.m_permSetTriples.Count && flag)
				{
					PermissionSetTriple permissionSetTriple = (PermissionSetTriple)this.m_permSetTriples[num];
					flag = permissionSetTriple.CheckDemand(demand, permToken, rmh);
					num++;
				}
			}
			else if (this.m_firstPermSetTriple != null)
			{
				flag = this.m_firstPermSetTriple.CheckDemand(demand, permToken, rmh);
			}
			return flag;
		}

		[SecurityCritical]
		internal bool CheckSetDemand(PermissionSet pset, RuntimeMethodHandleInternal rmh)
		{
			PermissionSet permissionSet;
			this.CheckSetDemandWithModification(pset, out permissionSet, rmh);
			return false;
		}

		[SecurityCritical]
		internal bool CheckSetDemandWithModification(PermissionSet pset, out PermissionSet alteredDemandSet, RuntimeMethodHandleInternal rmh)
		{
			bool flag = true;
			PermissionSet demandSet = pset;
			alteredDemandSet = null;
			if (this.m_permSetTriples != null)
			{
				int num = 0;
				while (num < this.m_permSetTriples.Count && flag)
				{
					PermissionSetTriple permissionSetTriple = (PermissionSetTriple)this.m_permSetTriples[num];
					flag = permissionSetTriple.CheckSetDemand(demandSet, out alteredDemandSet, rmh);
					if (alteredDemandSet != null)
					{
						demandSet = alteredDemandSet;
					}
					num++;
				}
			}
			else if (this.m_firstPermSetTriple != null)
			{
				flag = this.m_firstPermSetTriple.CheckSetDemand(demandSet, out alteredDemandSet, rmh);
			}
			return flag;
		}

		[SecurityCritical]
		private bool CheckFlags(int flags)
		{
			bool flag = true;
			if (this.m_permSetTriples != null)
			{
				int num = 0;
				while (num < this.m_permSetTriples.Count && flag)
				{
					if (flags == 0)
					{
						break;
					}
					flag &= ((PermissionSetTriple)this.m_permSetTriples[num]).CheckFlags(ref flags);
					num++;
				}
			}
			else if (this.m_firstPermSetTriple != null)
			{
				flag = this.m_firstPermSetTriple.CheckFlags(ref flags);
			}
			return flag;
		}

		[SecurityCritical]
		internal void DemandFlagsOrGrantSet(int flags, PermissionSet grantSet)
		{
			if (this.CheckFlags(flags))
			{
				return;
			}
			this.CheckSetDemand(grantSet, RuntimeMethodHandleInternal.EmptyHandle);
		}

		internal void GetZoneAndOrigin(ArrayList zoneList, ArrayList originList, PermissionToken zoneToken, PermissionToken originToken)
		{
			if (this.m_zoneList != null)
			{
				zoneList.AddRange(this.m_zoneList);
			}
			if (this.m_originList != null)
			{
				originList.AddRange(this.m_originList);
			}
		}

		private PermissionSetTriple m_firstPermSetTriple;

		private ArrayList m_permSetTriples;

		private ArrayList m_zoneList;

		private ArrayList m_originList;
	}
}
