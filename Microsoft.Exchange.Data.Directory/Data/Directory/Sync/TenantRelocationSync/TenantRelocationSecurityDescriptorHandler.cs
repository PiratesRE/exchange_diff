using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.Sync.TenantRelocationSync
{
	internal class TenantRelocationSecurityDescriptorHandler
	{
		public TenantRelocationSecurityDescriptorHandler(TenantRelocationSyncData syncConfigData, ITopologyConfigurationSession sourceSession, ITopologyConfigurationSession targetSession)
		{
			this.syncConfigData = syncConfigData;
			this.sourceSession = sourceSession;
			this.targetSession = targetSession;
			this.sourceConfigNC = this.sourceSession.GetConfigurationNamingContext();
			this.targetConfigNC = this.targetSession.GetConfigurationNamingContext();
			this.Initialize();
		}

		public void ProcessSecurityDescriptor(ADObjectId sourceId, ADObjectId targetId, bool forceResetTargetSD)
		{
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "ProcessSecurityDescriptor: process object {0}.", sourceId.DistinguishedName);
			RawSecurityDescriptor rsd = TenantRelocationSecurityDescriptorHandler.ReadSecurityDescriptorWrapper(this.sourceSession, sourceId, sourceId.IsDescendantOf(this.sourceConfigNC));
			List<GenericAce> customizedAces = this.GetCustomizedAces(rsd);
			if (!forceResetTargetSD && customizedAces.Count == 0)
			{
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "ProcessSecurityDescriptor: no customized ACEs found on source found {0}, skip update.", sourceId.DistinguishedName);
				return;
			}
			bool useConfigNC = targetId.IsDescendantOf(this.targetConfigNC);
			RawSecurityDescriptor targetSd = TenantRelocationSecurityDescriptorHandler.ReadSecurityDescriptorWrapper(this.targetSession, targetId, useConfigNC);
			RawSecurityDescriptor sd = this.ApplyAcesToTargetSecurityDescriptor(targetSd, customizedAces);
			bool useConfigNC2 = this.targetSession.UseConfigNC;
			this.targetSession.UseConfigNC = useConfigNC;
			try
			{
				this.targetSession.SaveSecurityDescriptor(targetId, sd);
			}
			finally
			{
				this.targetSession.UseConfigNC = useConfigNC2;
			}
			ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "ProcessSecurityDescriptor: process done object {0}.", sourceId.DistinguishedName);
		}

		private static int GetRidFromSecurityIdentifier(SecurityIdentifier sid)
		{
			string text = sid.ToString();
			int num = text.LastIndexOf('-');
			return int.Parse(text.Substring(num + 1));
		}

		private static SecurityIdentifier GetSidFromAce(GenericAce ace)
		{
			KnownAce knownAce = ace as KnownAce;
			if (knownAce != null)
			{
				return knownAce.SecurityIdentifier;
			}
			return null;
		}

		private static RawSecurityDescriptor ReadSecurityDescriptorWrapper(ITopologyConfigurationSession session, ADObjectId id, bool useConfigNC)
		{
			bool useConfigNC2 = session.UseConfigNC;
			session.UseConfigNC = useConfigNC;
			RawSecurityDescriptor result;
			try
			{
				result = session.ReadSecurityDescriptor(id);
			}
			finally
			{
				session.UseConfigNC = useConfigNC2;
			}
			return result;
		}

		private List<GenericAce> GetCustomizedAces(RawSecurityDescriptor rsd)
		{
			List<GenericAce> list = new List<GenericAce>();
			foreach (GenericAce genericAce in rsd.DiscretionaryAcl)
			{
				if (!genericAce.IsInherited)
				{
					SecurityIdentifier sidFromAce = TenantRelocationSecurityDescriptorHandler.GetSidFromAce(genericAce);
					if (!(sidFromAce == null))
					{
						if (this.IsKnownGlobalPrincipal(sidFromAce))
						{
							ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "GetCustomizedAces: wellknown SID skipped {0}.", sidFromAce.ToString());
						}
						else
						{
							ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "GetCustomizedAces: customized SID found {0}.", sidFromAce.ToString());
							list.Add(genericAce);
						}
					}
				}
			}
			return list;
		}

		private void Initialize()
		{
			ADDomain addomain = this.targetSession.Read<ADDomain>(this.syncConfigData.Target.PartitionRoot);
			this.targetDomainSid = addomain.Sid;
			this.wellKnownExchangeSecurityPrincipals = new HashSet<SecurityIdentifier>();
			ADObjectId childId = this.syncConfigData.Source.PartitionRoot.GetChildId("OU", "Microsoft Exchange Security Groups");
			ADRawEntry[] array = this.sourceSession.Find(childId, QueryScope.OneLevel, this.groupFilter, null, 0, this.groupProperties);
			foreach (ADRawEntry adrawEntry in array)
			{
				this.wellKnownExchangeSecurityPrincipals.Add((SecurityIdentifier)adrawEntry[ADMailboxRecipientSchema.Sid]);
			}
		}

		private RawSecurityDescriptor ApplyAcesToTargetSecurityDescriptor(RawSecurityDescriptor targetSd, List<GenericAce> sourceAces)
		{
			List<GenericAce> list = new List<GenericAce>();
			foreach (GenericAce genericAce in targetSd.DiscretionaryAcl)
			{
				SecurityIdentifier sidFromAce = TenantRelocationSecurityDescriptorHandler.GetSidFromAce(genericAce);
				if (!(sidFromAce == null))
				{
					SecurityIdentifier accountDomainSid = sidFromAce.AccountDomainSid;
					if (sidFromAce.IsAccountSid() && !accountDomainSid.Equals(this.targetDomainSid))
					{
						ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>((long)this.GetHashCode(), "ApplyAcesToTargetSecurityDescriptor: customized SID found {0} on target object, removed.", sidFromAce.ToString());
					}
					else
					{
						list.Add(genericAce);
					}
				}
			}
			RawAcl rawAcl = new RawAcl(targetSd.DiscretionaryAcl.Revision, list.Count + sourceAces.Count);
			int num = 0;
			foreach (GenericAce ace in list)
			{
				rawAcl.InsertAce(num++, ace);
			}
			foreach (GenericAce ace2 in sourceAces)
			{
				rawAcl.InsertAce(num++, ace2);
			}
			targetSd.DiscretionaryAcl = rawAcl;
			return targetSd;
		}

		private bool IsKnownGlobalPrincipal(SecurityIdentifier sid)
		{
			int ridFromSecurityIdentifier = TenantRelocationSecurityDescriptorHandler.GetRidFromSecurityIdentifier(sid);
			return ridFromSecurityIdentifier <= 1000 || this.wellKnownExchangeSecurityPrincipals.Contains(sid);
		}

		private const int MaxRidOfAdDefaultPrincipals = 1000;

		private const string ExchangeGlobalGroupContainer = "Microsoft Exchange Security Groups";

		private SecurityIdentifier targetDomainSid;

		private ITopologyConfigurationSession sourceSession;

		private ITopologyConfigurationSession targetSession;

		private ADObjectId sourceConfigNC;

		private ADObjectId targetConfigNC;

		private HashSet<SecurityIdentifier> wellKnownExchangeSecurityPrincipals;

		private TenantRelocationSyncData syncConfigData;

		private readonly QueryFilter groupFilter = ADObject.ObjectClassFilter(ADGroup.MostDerivedClass);

		private PropertyDefinition[] groupProperties = new PropertyDefinition[]
		{
			ADMailboxRecipientSchema.Sid
		};
	}
}
