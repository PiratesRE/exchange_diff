using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Exchange.Security.Dkm.Proxy;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("initialize", "DkmDatacenter", SupportsShouldProcess = true)]
	public sealed class InitializeDkmDatacenter : SetupTaskBase
	{
		public static NTAccount DomainAdminsAccount
		{
			get
			{
				return new NTAccount("Domain Admins");
			}
		}

		public static SecurityIdentifier DomainAdminsSid
		{
			get
			{
				return (SecurityIdentifier)InitializeDkmDatacenter.DomainAdminsAccount.Translate(typeof(SecurityIdentifier));
			}
		}

		public static NTAccount ExchangeServersAccount
		{
			get
			{
				return new NTAccount("Exchange Servers");
			}
		}

		public static NTAccount ExchangeTrustedSubsystemAccount
		{
			get
			{
				return new NTAccount("Exchange Trusted Subsystem");
			}
		}

		public static NTAccount EdsServersAccount
		{
			get
			{
				return new NTAccount("EDS Servers");
			}
		}

		public static IEnumerable<Tuple<string, List<SecurityIdentifier>, List<SecurityIdentifier>>> DkmContainersToCreate
		{
			get
			{
				return new List<Tuple<string, List<SecurityIdentifier>, List<SecurityIdentifier>>>
				{
					Tuple.Create<string, List<SecurityIdentifier>, List<SecurityIdentifier>>("Microsoft Exchange DKM", new List<SecurityIdentifier>
					{
						(SecurityIdentifier)InitializeDkmDatacenter.ExchangeTrustedSubsystemAccount.Translate(typeof(SecurityIdentifier)),
						(SecurityIdentifier)InitializeDkmDatacenter.ExchangeServersAccount.Translate(typeof(SecurityIdentifier)),
						new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null)
					}, new List<SecurityIdentifier>
					{
						(SecurityIdentifier)InitializeDkmDatacenter.DomainAdminsAccount.Translate(typeof(SecurityIdentifier)),
						new SecurityIdentifier(WellKnownSidType.EnterpriseControllersSid, null),
						new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null)
					}),
					Tuple.Create<string, List<SecurityIdentifier>, List<SecurityIdentifier>>("Microsoft Exchange Diagnostics DKM", new List<SecurityIdentifier>
					{
						(SecurityIdentifier)InitializeDkmDatacenter.EdsServersAccount.Translate(typeof(SecurityIdentifier))
					}, new List<SecurityIdentifier>
					{
						(SecurityIdentifier)InitializeDkmDatacenter.DomainAdminsAccount.Translate(typeof(SecurityIdentifier)),
						new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null)
					})
				};
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.InternalBeginProcessing();
			string text = this.rootDomain.Id.ToDNString();
			foreach (Tuple<string, List<SecurityIdentifier>, List<SecurityIdentifier>> tuple in InitializeDkmDatacenter.DkmContainersToCreate)
			{
				try
				{
					DkmProxy dkmProxy = null;
					try
					{
						this.CreateDkmContainer(tuple.Item1, string.Format("{0},{1}", "CN=Microsoft,CN=Program Data", text), out dkmProxy);
					}
					catch (ObjectAlreadyExistsException)
					{
						this.WriteWarning(Strings.DkmContainerAlreadyExists(tuple.Item1));
					}
					if (dkmProxy != null)
					{
						this.RemoveUnwantedDkmContainerAccessRules(tuple.Item1, tuple.Item2, tuple.Item3, text);
						InitializeDkmDatacenter.SetDkmContainerAccessRules(dkmProxy, tuple.Item2, tuple.Item3);
					}
				}
				catch (Exception ex)
				{
					this.WriteWarning(Strings.DkmProvisioningException(tuple.Item1, ex));
					ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_DkmProvisioningException, new string[]
					{
						ex.ToString()
					});
					throw;
				}
			}
			ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_DkmProvisioningSuccessful, new string[0]);
			TaskLogger.LogExit();
		}

		private void CreateDkmContainer(string dkmContainerName, string dkmParentContainerDN, out DkmProxy dkmProxy)
		{
			dkmProxy = new DkmProxy(dkmContainerName, null, null)
			{
				PreferredReplicaName = this.rootDomain.OriginatingServer,
				DkmParentContainerDN = dkmParentContainerDN,
				DkmContainerName = "CN=Distributed KeyMan"
			};
			dkmProxy.InitializeDkm();
			dkmProxy.AddGroup();
		}

		private void RemoveUnwantedDkmContainerAccessRules(string dkmContainerName, IEnumerable<SecurityIdentifier> principalsToHaveKeyReadWritePermissionsAdded, IEnumerable<SecurityIdentifier> principalsToHaveFullControlPermissionsAdded, string rootDomainDN)
		{
			DirectoryEntry directoryEntry = new DirectoryEntry(string.Format("LDAP://CN={0},{1},{2},{3}", new object[]
			{
				dkmContainerName,
				"CN=Distributed KeyMan",
				"CN=Microsoft,CN=Program Data",
				rootDomainDN
			}));
			if (!directoryEntry.ObjectSecurity.AreAccessRulesCanonical)
			{
				InitializeDkmDatacenter.CanonicalizeAcl(directoryEntry.ObjectSecurity);
			}
			AuthorizationRuleCollection accessRules = directoryEntry.ObjectSecurity.GetAccessRules(true, true, typeof(SecurityIdentifier));
			foreach (object obj in accessRules)
			{
				AuthorizationRule authorizationRule = (AuthorizationRule)obj;
				if (!(authorizationRule.IdentityReference == InitializeDkmDatacenter.DomainAdminsSid) && !InitializeDkmDatacenter.IsIdentityInCollection(authorizationRule.IdentityReference, principalsToHaveKeyReadWritePermissionsAdded) && !InitializeDkmDatacenter.IsIdentityInCollection(authorizationRule.IdentityReference, principalsToHaveFullControlPermissionsAdded))
				{
					this.WriteWarning(Strings.RemovingAceFromDkmContainerAcl(dkmContainerName, InitializeDkmDatacenter.AccountNameFromSid(authorizationRule.IdentityReference.ToString())));
					directoryEntry.ObjectSecurity.PurgeAccessRules(authorizationRule.IdentityReference);
				}
			}
			directoryEntry.ObjectSecurity.SetSecurityDescriptorBinaryForm(directoryEntry.ObjectSecurity.GetSecurityDescriptorBinaryForm());
			directoryEntry.CommitChanges();
			directoryEntry.Close();
		}

		private static bool IsIdentityInCollection(IdentityReference identity, IEnumerable<IdentityReference> identityCollection)
		{
			IEnumerable<IdentityReference> source = from principal in identityCollection
			where principal.Equals(identity)
			select principal;
			return source.Any<IdentityReference>();
		}

		private static void SetDkmContainerAccessRules(DkmProxy dkmProxy, IEnumerable<SecurityIdentifier> principalsToHaveKeyReadWritePermissionsAdded, IEnumerable<SecurityIdentifier> principalsToHaveFullControlPermissionsAdded)
		{
			try
			{
				foreach (SecurityIdentifier identity in principalsToHaveKeyReadWritePermissionsAdded)
				{
					dkmProxy.AddGroupMemberWithUpdateRights(identity);
				}
				foreach (SecurityIdentifier identity2 in principalsToHaveFullControlPermissionsAdded)
				{
					dkmProxy.AddGroupOwner(identity2);
				}
			}
			catch (COMException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
		}

		private static void CanonicalizeAcl(ActiveDirectorySecurity ads)
		{
			ActiveDirectorySecurity activeDirectorySecurity = new ActiveDirectorySecurity();
			foreach (object obj in ads.GetAccessRules(true, true, typeof(SecurityIdentifier)))
			{
				ActiveDirectoryAccessRule rule = (ActiveDirectoryAccessRule)obj;
				activeDirectorySecurity.AddAccessRule(rule);
			}
			RawSecurityDescriptor rawSecurityDescriptor = new RawSecurityDescriptor(activeDirectorySecurity.GetSecurityDescriptorSddlForm(AccessControlSections.Access));
			RawSecurityDescriptor rawSecurityDescriptor2 = new RawSecurityDescriptor(ads.GetSecurityDescriptorSddlForm(AccessControlSections.Access))
			{
				DiscretionaryAcl = rawSecurityDescriptor.DiscretionaryAcl
			};
			ads.SetSecurityDescriptorSddlForm(rawSecurityDescriptor2.GetSddlForm(AccessControlSections.Access), AccessControlSections.Access);
		}

		private static string AccountNameFromSid(string sid)
		{
			try
			{
				return new SecurityIdentifier(sid).Translate(typeof(NTAccount)).ToString();
			}
			catch (IdentityNotMappedException)
			{
			}
			catch (SystemException)
			{
			}
			return sid;
		}
	}
}
