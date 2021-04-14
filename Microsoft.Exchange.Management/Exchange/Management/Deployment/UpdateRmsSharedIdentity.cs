using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Update", "RmsSharedIdentity", SupportsShouldProcess = false)]
	public sealed class UpdateRmsSharedIdentity : Task
	{
		public UpdateRmsSharedIdentity()
		{
			base.Fields["RemoveLink"] = new SwitchParameter(false);
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter RemoveLink
		{
			get
			{
				return (SwitchParameter)base.Fields["RemoveLink"];
			}
			set
			{
				base.Fields["RemoveLink"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ServerName
		{
			get
			{
				return (string)base.Fields["ServerName"];
			}
			set
			{
				base.Fields["ServerName"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			if (TopologyProvider.IsAdamTopology())
			{
				base.InternalProcessRecord();
				return;
			}
			if (this.RemoveLink.IsPresent)
			{
				this.Unlink();
			}
			else
			{
				this.Link();
			}
			base.InternalProcessRecord();
		}

		private void Link()
		{
			try
			{
				ADUser aduser = this.FindRmsSharedIdentityUser();
				ADComputer adcomputer = this.FindComputer();
				if (!aduser.RMSComputerAccounts.Contains(adcomputer.Id))
				{
					base.WriteVerbose(Strings.RmsSharedIdentityBeingLinkedToComputer(aduser.DistinguishedName, aduser.OriginatingServer, adcomputer.DistinguishedName, adcomputer.OriginatingServer));
					aduser.RMSComputerAccounts.Add(adcomputer.Id);
					this.writeableRecipientSession.LinkResolutionServer = adcomputer.OriginatingServer;
					this.writeableRecipientSession.Save(aduser);
				}
			}
			catch (RmsSharedIdentityTooManyUsersException exception)
			{
				this.WriteError(exception, ErrorCategory.InvalidData, null, true);
			}
			catch (RmsSharedIdentityUserNotFoundException exception2)
			{
				this.WriteError(exception2, ErrorCategory.InvalidData, null, true);
			}
			catch (RmsSharedIdentityLocalComputerNotFoundException exception3)
			{
				this.WriteError(exception3, ErrorCategory.InvalidData, null, true);
			}
			catch (RmsSharedIdentityComputerNotFoundException exception4)
			{
				this.WriteError(exception4, ErrorCategory.InvalidData, null, true);
			}
			catch (ADObjectEntryAlreadyExistsException)
			{
			}
		}

		private void Unlink()
		{
			try
			{
				ADUser aduser = this.FindRmsSharedIdentityUser();
				ADComputer adcomputer = this.FindComputer();
				if (aduser.RMSComputerAccounts.Contains(adcomputer.Id))
				{
					base.WriteVerbose(Strings.RmsSharedIdentityBeingUnlinkedFromComputer(aduser.DistinguishedName, aduser.OriginatingServer, adcomputer.DistinguishedName, adcomputer.OriginatingServer));
					aduser.RMSComputerAccounts.Remove(adcomputer.Id);
					this.writeableRecipientSession.LinkResolutionServer = adcomputer.OriginatingServer;
					this.writeableRecipientSession.Save(aduser);
				}
			}
			catch (RmsSharedIdentityTooManyUsersException ex)
			{
				this.WriteWarning(ex.LocalizedString);
				this.WriteWarning(Strings.RmsSharedIdentityFailedToRemoveLink);
			}
			catch (RmsSharedIdentityUserNotFoundException ex2)
			{
				this.WriteWarning(ex2.LocalizedString);
				this.WriteWarning(Strings.RmsSharedIdentityFailedToRemoveLink);
			}
			catch (RmsSharedIdentityLocalComputerNotFoundException)
			{
				this.WriteWarning(Strings.RmsSharedIdentityLocalComputerNotFound);
				this.WriteWarning(Strings.RmsSharedIdentityFailedToRemoveLink);
			}
			catch (RmsSharedIdentityComputerNotFoundException)
			{
				this.WriteWarning(Strings.RmsSharedIdentityComputerNotFound(this.ServerName));
				this.WriteWarning(Strings.RmsSharedIdentityFailedToRemoveLink);
			}
			catch (DataValidationException ex3)
			{
				this.WriteWarning(Strings.RmsSharedIdentityInconsistentState(ex3.LocalizedString));
				this.WriteWarning(Strings.RmsSharedIdentityFailedToRemoveLink);
			}
		}

		private ADUser FindRmsSharedIdentityUser()
		{
			this.writeableRecipientSession = this.CreateWriteableRecipientSession();
			this.writeableRecipientSession.UseGlobalCatalog = true;
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.CommonName, "FederatedEmail.4c1f4d8b-8179-4148-93bf-00a95fa1e042");
			ADUser[] array = this.writeableRecipientSession.FindADUser(null, QueryScope.SubTree, filter, null, 2);
			if (array != null && array.Length > 1)
			{
				throw new RmsSharedIdentityTooManyUsersException(array[0].DistinguishedName, array[1].DistinguishedName);
			}
			if (array != null && array.Length > 0)
			{
				this.writeableRecipientSession.UseGlobalCatalog = false;
				return array[0];
			}
			base.WriteVerbose(Strings.RmsSharedIdentityUserNotFoundInGC("FederatedEmail.4c1f4d8b-8179-4148-93bf-00a95fa1e042"));
			this.writeableRecipientSession = this.CreateWriteableRecipientSessionForRootDomain();
			array = this.writeableRecipientSession.FindADUser(null, QueryScope.SubTree, filter, null, 2);
			if (array == null || array.Length == 0)
			{
				throw new RmsSharedIdentityUserNotFoundException("FederatedEmail.4c1f4d8b-8179-4148-93bf-00a95fa1e042");
			}
			if (array.Length > 1)
			{
				throw new RmsSharedIdentityTooManyUsersException(array[0].DistinguishedName, array[1].DistinguishedName);
			}
			return array[0];
		}

		private ADComputer FindComputer()
		{
			if (string.IsNullOrEmpty(this.ServerName))
			{
				ITopologyConfigurationSession topologyConfigurationSession = this.CreateReadOnlyLocalConfigurationSession();
				ADComputer adcomputer = topologyConfigurationSession.FindLocalComputer();
				if (adcomputer == null)
				{
					throw new RmsSharedIdentityLocalComputerNotFoundException();
				}
				return adcomputer;
			}
			else
			{
				ITopologyConfigurationSession topologyConfigurationSession2 = this.CreateGlobalCatalogConfigurationSession();
				ADComputer adcomputer2 = topologyConfigurationSession2.FindComputerByHostName(this.ServerName);
				if (adcomputer2 == null)
				{
					throw new RmsSharedIdentityComputerNotFoundException(this.ServerName);
				}
				return adcomputer2;
			}
		}

		private IRecipientSession CreateWriteableRecipientSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 361, "CreateWriteableRecipientSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\UpdateRmsSharedIdentity.cs");
		}

		private IRecipientSession CreateWriteableRecipientSessionForRootDomain()
		{
			ADDomain addomain = ADForest.GetLocalForest().FindRootDomain(true);
			if (addomain == null)
			{
				base.ThrowTerminatingError(new RootDomainNotFoundException(), ErrorCategory.InvalidData, null);
			}
			return DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(addomain.OriginatingServer, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 382, "CreateWriteableRecipientSessionForRootDomain", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\UpdateRmsSharedIdentity.cs");
		}

		private ITopologyConfigurationSession CreateReadOnlyLocalConfigurationSession()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 395, "CreateReadOnlyLocalConfigurationSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\UpdateRmsSharedIdentity.cs");
			topologyConfigurationSession.UseConfigNC = false;
			topologyConfigurationSession.UseGlobalCatalog = false;
			return topologyConfigurationSession;
		}

		private ITopologyConfigurationSession CreateGlobalCatalogConfigurationSession()
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 410, "CreateGlobalCatalogConfigurationSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\UpdateRmsSharedIdentity.cs");
			topologyConfigurationSession.UseConfigNC = false;
			topologyConfigurationSession.UseGlobalCatalog = true;
			return topologyConfigurationSession;
		}

		public const string SharedIdentityCommonName = "FederatedEmail.4c1f4d8b-8179-4148-93bf-00a95fa1e042";

		private const string RemoveLinkParamName = "RemoveLink";

		private const string ServerNameParamName = "ServerName";

		private IRecipientSession writeableRecipientSession;
	}
}
