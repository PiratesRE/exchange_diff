using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.PowerShell.RbacHostingTools;
using Microsoft.PowerShell.HostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	public abstract class AdObjectResolver
	{
		internal ADSessionSettings TenantSessionSetting
		{
			get
			{
				return ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), RbacPrincipal.Current.RbacConfiguration.OrganizationId, null, false);
			}
		}

		internal ADSessionSettings TenantSharedConfigurationSessionSetting
		{
			get
			{
				LocalSession localSession = LocalSession.Current;
				if (localSession.IsDehydrated)
				{
					SharedConfiguration sharedConfiguration = SharedConfiguration.GetSharedConfiguration(localSession.RbacConfiguration.OrganizationId);
					if (sharedConfiguration != null)
					{
						return sharedConfiguration.GetSharedConfigurationSessionSettings();
					}
				}
				return null;
			}
		}

		internal abstract IDirectorySession CreateAdSession();

		protected IEnumerable<T> ResolveObjects<T>(IEnumerable<ADObjectId> identities, IEnumerable<PropertyDefinition> propertiesToRead, Func<ADRawEntry, T> factory)
		{
			if (identities == null)
			{
				throw new FaultException(new ArgumentNullException("identities").Message);
			}
			return from entry in this.FindObjects(identities.ToArray<ADObjectId>(), propertiesToRead)
			select factory(entry);
		}

		private IEnumerable<ADRawEntry> FindObjects(ADObjectId[] identities, IEnumerable<PropertyDefinition> propertiesToRead)
		{
			if (ADSessionSettings.GetThreadServerSettings() == null)
			{
				EcpRunspaceFactory ecpRunspaceFactory = new EcpRunspaceFactory(new InitialSessionStateSectionFactory());
				ADSessionSettings.SetThreadADContext(new ADDriverContext(ecpRunspaceFactory.CreateRunspaceServerSettings(), ContextMode.Cmdlet));
			}
			IDirectorySession session = this.CreateAdSession();
			List<QueryFilter> filters = new List<QueryFilter>(this.properMaxCustomFilterTreeSize);
			int filterLenRemain = 31197;
			int i = 0;
			while (i < identities.Length)
			{
				QueryFilter idFilter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, identities[i]);
				filters.Add(idFilter);
				string ldapIdFilter = LdapFilterBuilder.LdapFilterFromQueryFilter(idFilter);
				if (ldapIdFilter.Length > filterLenRemain || filters.Count == this.properMaxCustomFilterTreeSize || i == identities.Length - 1)
				{
					ADPagedReader<ADRawEntry> entries = null;
					try
					{
						entries = session.FindPagedADRawEntry(null, QueryScope.SubTree, new OrFilter(filters.ToArray()), null, this.properMaxCustomFilterTreeSize, propertiesToRead);
					}
					catch (ADFilterException)
					{
						if (this.isFirstError)
						{
							i -= filters.Count;
							this.properMaxCustomFilterTreeSize /= 2;
							filters.Clear();
							filterLenRemain = 31197;
							this.isFirstError = false;
							goto IL_22E;
						}
						throw;
					}
					foreach (ADRawEntry entry in entries)
					{
						yield return entry;
					}
					filters.Clear();
					filterLenRemain = 31197;
					goto IL_216;
				}
				goto IL_216;
				IL_22E:
				i++;
				continue;
				IL_216:
				filterLenRemain -= ldapIdFilter.Length;
				goto IL_22E;
			}
			yield break;
		}

		private const int MaxLdapFilterSize = 31197;

		private int properMaxCustomFilterTreeSize = Util.IsDataCenter ? (LdapFilterBuilder.MaxCustomFilterTreeSize / 2) : LdapFilterBuilder.MaxCustomFilterTreeSize;

		private bool isFirstError = true;
	}
}
