using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mapi;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Exchange.Management.MapiTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.OAB;
using Microsoft.Exchange.Provisioning.LoadBalancing;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Mapi;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Common
{
	internal static class MailboxTaskHelper
	{
		internal static SecurityIdentifier GetAccountSidFromAnotherForest(IIdentityParameter id, string userForestDomainController, NetworkCredential userForestCredential, ITopologyConfigurationSession resourceForestSession, MailboxTaskHelper.GetUniqueObject getUniqueObject, Task.ErrorLoggerDelegate errorHandler)
		{
			return MailboxTaskHelper.GetSidFromAnotherForest<ADUser>(id, userForestDomainController, userForestCredential, resourceForestSession, getUniqueObject, errorHandler, new MailboxTaskHelper.OneStringErrorDelegate(Strings.ErrorLinkedAccountInTheCurrentForest), new MailboxTaskHelper.TwoStringErrorDelegate(Strings.ErrorUserNotFoundOnGlobalCatalog), new MailboxTaskHelper.TwoStringErrorDelegate(Strings.ErrorUserNotFoundOnDomainController), new MailboxTaskHelper.TwoStringErrorDelegate(Strings.ErrorUserNotUniqueOnGlobalCatalog), new MailboxTaskHelper.TwoStringErrorDelegate(Strings.ErrorUserNotUniqueOnDomainController), new MailboxTaskHelper.OneStringErrorDelegate(Strings.ErrorVerifyLinkedForest));
		}

		internal static SecurityIdentifier GetSidFromAnotherForest<TObject>(IIdentityParameter id, string userForestDomainController, NetworkCredential userForestCredential, ITopologyConfigurationSession resourceForestSession, MailboxTaskHelper.GetUniqueObject getUniqueObject, Task.ErrorLoggerDelegate errorHandler, MailboxTaskHelper.OneStringErrorDelegate linkedObjectInCurrentForest, MailboxTaskHelper.TwoStringErrorDelegate linkedObjectNotFoundOnGC, MailboxTaskHelper.TwoStringErrorDelegate linkedObjectNotFoundOnDC, MailboxTaskHelper.TwoStringErrorDelegate linkedObjectNotUniqueOnGC, MailboxTaskHelper.TwoStringErrorDelegate linkedObjectNotUniqueOnDC, MailboxTaskHelper.OneStringErrorDelegate linkedObjectVerifyForest) where TObject : IADSecurityPrincipal
		{
			try
			{
				SecurityIdentifier securityIdentifier = new SecurityIdentifier(id.RawIdentity);
				if (securityIdentifier != null)
				{
					if (!MailboxTaskHelper.IsMasterAccountAlreadyExist(resourceForestSession, securityIdentifier, id, errorHandler))
					{
						return securityIdentifier;
					}
					return null;
				}
			}
			catch (ArgumentException)
			{
			}
			try
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(userForestDomainController, true, ConsistencyMode.PartiallyConsistent, userForestCredential, ADSessionSettings.FromRootOrgScopeSet(), 448, "GetSidFromAnotherForest", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\recipient\\MailboxTaskHelper.cs");
				string rootDomainNamingContextFromCurrentReadConnection = resourceForestSession.GetRootDomainNamingContextFromCurrentReadConnection();
				string rootDomainNamingContextFromCurrentReadConnection2 = topologyConfigurationSession.GetRootDomainNamingContextFromCurrentReadConnection();
				if (string.Equals(rootDomainNamingContextFromCurrentReadConnection, rootDomainNamingContextFromCurrentReadConnection2, StringComparison.OrdinalIgnoreCase))
				{
					errorHandler(new TaskInvalidOperationException(linkedObjectInCurrentForest(NativeHelpers.CanonicalNameFromDistinguishedName(rootDomainNamingContextFromCurrentReadConnection))), ExchangeErrorCategory.Client, null);
				}
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(userForestDomainController, true, ConsistencyMode.PartiallyConsistent, userForestCredential, ADSessionSettings.FromRootOrgScopeSet(), 466, "GetSidFromAnotherForest", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\recipient\\MailboxTaskHelper.cs");
				tenantOrRootOrgRecipientSession.UseGlobalCatalog = true;
				if (!tenantOrRootOrgRecipientSession.IsReadConnectionAvailable())
				{
					tenantOrRootOrgRecipientSession.UseGlobalCatalog = false;
				}
				TObject tobject = (TObject)((object)getUniqueObject(id, tenantOrRootOrgRecipientSession, null, new LocalizedString?(tenantOrRootOrgRecipientSession.UseGlobalCatalog ? linkedObjectNotFoundOnGC(id.ToString(), userForestDomainController) : linkedObjectNotFoundOnDC(id.ToString(), userForestDomainController)), new LocalizedString?(tenantOrRootOrgRecipientSession.UseGlobalCatalog ? linkedObjectNotUniqueOnGC(id.ToString(), userForestDomainController) : linkedObjectNotUniqueOnDC(id.ToString(), userForestDomainController)), ExchangeErrorCategory.Client));
				if (tobject != null)
				{
					SecurityIdentifier sid = tobject.Sid;
					if (!(tobject is ADUser))
					{
						return sid;
					}
					if (!MailboxTaskHelper.IsMasterAccountAlreadyExist(resourceForestSession, sid, id, errorHandler))
					{
						return sid;
					}
				}
			}
			catch (DataSourceTransientException ex)
			{
				errorHandler(new TaskInvalidOperationException(linkedObjectVerifyForest(ex.Message), ex), ExchangeErrorCategory.ServerTransient, null);
			}
			catch (DataSourceOperationException ex2)
			{
				errorHandler(new TaskInvalidOperationException(linkedObjectVerifyForest(ex2.Message), ex2), ExchangeErrorCategory.ServerOperation, null);
			}
			return null;
		}

		internal static bool IsMasterAccountAlreadyExist(ITopologyConfigurationSession resourceForestSession, SecurityIdentifier sid, IIdentityParameter id, Task.ErrorLoggerDelegate errorHandler)
		{
			IRecipientSession recipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(resourceForestSession.DomainController, null, CultureInfo.CurrentCulture.LCID, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromRootOrgScopeSet(), 526, "IsMasterAccountAlreadyExist", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\recipient\\MailboxTaskHelper.cs");
			if (!recipientSession.IsReadConnectionAvailable())
			{
				recipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 538, "IsMasterAccountAlreadyExist", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\recipient\\MailboxTaskHelper.cs");
			}
			recipientSession.UseGlobalCatalog = true;
			IConfigurable[] array = recipientSession.Find(null, QueryScope.SubTree, new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.MasterAccountSid, sid), null, 1);
			if (array.Length == 0)
			{
				return false;
			}
			errorHandler(new TaskInvalidOperationException(Strings.ErrorLinkedUserAccountIsAlreadyUsed(id.ToString(), array[0].Identity.ToString())), ExchangeErrorCategory.Client, null);
			return true;
		}

		internal static void GrantPermissionToLinkedUserAccount(ADUser user, IConfigurationSession adConfigSession, Task.ErrorLoggerDelegate errorHandler, Task.TaskVerboseLoggingDelegate logHandler)
		{
			RawSecurityDescriptor exchangeSecurityDescriptor = null;
			if (Guid.Empty != user.ExchangeGuid && user.Database != null)
			{
				ActiveDirectorySecurity activeDirectorySecurity = PermissionTaskHelper.ReadMailboxSecurityDescriptor(user, adConfigSession, logHandler, errorHandler);
				exchangeSecurityDescriptor = new RawSecurityDescriptor(activeDirectorySecurity.GetSecurityDescriptorBinaryForm(), 0);
			}
			logHandler(Strings.VerboseReadADSecurityDescriptor(user.Id.ToString()));
			RawSecurityDescriptor rawSecurityDescriptor = user.ReadSecurityDescriptor();
			MailboxTaskHelper.GrantPermissionToLinkedUserAccount(user.MasterAccountSid, ref exchangeSecurityDescriptor, ref rawSecurityDescriptor);
			user.ExchangeSecurityDescriptor = exchangeSecurityDescriptor;
			user.propertyBag.SetField(ADObjectSchema.NTSecurityDescriptor, SecurityDescriptor.FromRawSecurityDescriptor(rawSecurityDescriptor));
		}

		internal static void GrantPermissionToLinkedUserAccount(ADUser user, Task.TaskVerboseLoggingDelegate logHandler)
		{
			MailboxTaskHelper.GrantPermissionToLinkedUserAccounts(user, null, logHandler);
		}

		internal static void GrantPermissionToLinkedUserAccounts(ADUser user, SecurityIdentifier[] altUserSids, Task.TaskVerboseLoggingDelegate logHandler)
		{
			RawSecurityDescriptor exchangeSecurityDescriptor = user.ExchangeSecurityDescriptor;
			logHandler(Strings.VerboseReadADSecurityDescriptor(user.Id.ToString()));
			RawSecurityDescriptor rawSecurityDescriptor = user.ReadSecurityDescriptor();
			MailboxTaskHelper.GrantPermissionToLinkedUserAccounts(user.MasterAccountSid, altUserSids, ref exchangeSecurityDescriptor, ref rawSecurityDescriptor);
			user.ExchangeSecurityDescriptor = exchangeSecurityDescriptor;
			user.propertyBag.SetField(ADObjectSchema.NTSecurityDescriptor, SecurityDescriptor.FromRawSecurityDescriptor(rawSecurityDescriptor));
		}

		internal static void ClearExternalAssociatedAccountPermission(ADUser user, IConfigurationSession adConfigSession, Task.ErrorLoggerDelegate errorHandler, Task.TaskVerboseLoggingDelegate logHandler)
		{
			ActiveDirectorySecurity activeDirectorySecurity = PermissionTaskHelper.ReadMailboxSecurityDescriptor(user, adConfigSession, logHandler, errorHandler);
			RawSecurityDescriptor rawSecurityDescriptor = new RawSecurityDescriptor(activeDirectorySecurity.GetSecurityDescriptorBinaryForm(), 0);
			RawAcl discretionaryAcl = rawSecurityDescriptor.DiscretionaryAcl;
			MailboxTaskHelper.ClearExternalAssociatedAccountPermission(discretionaryAcl);
			rawSecurityDescriptor.DiscretionaryAcl = discretionaryAcl;
			user.ExchangeSecurityDescriptor = rawSecurityDescriptor;
		}

		private static void ClearExternalAssociatedAccountPermission(RawAcl dacl)
		{
			if (dacl != null)
			{
				for (int i = 0; i < dacl.Count; i++)
				{
					if (dacl[i] is CommonAce)
					{
						CommonAce commonAce = (CommonAce)dacl[i];
						if ((4 & commonAce.AccessMask) != 0)
						{
							commonAce.AccessMask &= -5;
							dacl[i] = commonAce;
						}
					}
				}
			}
		}

		internal static void GrantPermissionToLinkedUserAccount(SecurityIdentifier linkedUserSid, ref RawSecurityDescriptor exchangeSecurityDescriptor, ref RawSecurityDescriptor ntSecurityDescriptor)
		{
			MailboxTaskHelper.GrantPermissionToLinkedUserAccounts(linkedUserSid, null, ref exchangeSecurityDescriptor, ref ntSecurityDescriptor);
		}

		internal static void GrantPermissionToLinkedUserAccounts(SecurityIdentifier linkedUserSid, SecurityIdentifier[] altUserSids, ref RawSecurityDescriptor exchangeSecurityDescriptor, ref RawSecurityDescriptor ntSecurityDescriptor)
		{
			DiscretionaryAcl discretionaryAcl2;
			if (exchangeSecurityDescriptor != null)
			{
				byte[] binaryForm = new byte[exchangeSecurityDescriptor.BinaryLength];
				exchangeSecurityDescriptor.GetBinaryForm(binaryForm, 0);
				exchangeSecurityDescriptor = new RawSecurityDescriptor(binaryForm, 0);
				RawAcl discretionaryAcl = exchangeSecurityDescriptor.DiscretionaryAcl;
				MailboxTaskHelper.ClearExternalAssociatedAccountPermission(discretionaryAcl);
				discretionaryAcl2 = new DiscretionaryAcl(true, true, discretionaryAcl);
			}
			else
			{
				using (WindowsIdentity current = WindowsIdentity.GetCurrent())
				{
					SecurityIdentifier user = current.User;
					exchangeSecurityDescriptor = new RawSecurityDescriptor(ControlFlags.DiscretionaryAclDefaulted | ControlFlags.SystemAclDefaulted | ControlFlags.SelfRelative, user, user, null, null);
				}
				discretionaryAcl2 = new DiscretionaryAcl(true, true, 1);
			}
			DiscretionaryAcl discretionaryAcl3;
			if (ntSecurityDescriptor != null)
			{
				discretionaryAcl3 = new DiscretionaryAcl(true, true, ntSecurityDescriptor.DiscretionaryAcl);
			}
			else
			{
				using (WindowsIdentity current2 = WindowsIdentity.GetCurrent())
				{
					SecurityIdentifier user2 = current2.User;
					ntSecurityDescriptor = new RawSecurityDescriptor(ControlFlags.DiscretionaryAclDefaulted | ControlFlags.SystemAclDefaulted | ControlFlags.SelfRelative, user2, user2, null, null);
					discretionaryAcl3 = new DiscretionaryAcl(true, true, 2);
				}
			}
			discretionaryAcl2.AddAccess(AccessControlType.Allow, linkedUserSid, 5, InheritanceFlags.ContainerInherit, PropagationFlags.None);
			discretionaryAcl3.AddAccess(AccessControlType.Allow, linkedUserSid, 256, InheritanceFlags.ContainerInherit, PropagationFlags.None, ObjectAceFlags.ObjectAceTypePresent, WellKnownGuid.SendAsExtendedRightGuid, Guid.Empty);
			if (altUserSids != null)
			{
				foreach (SecurityIdentifier sid in altUserSids)
				{
					discretionaryAcl3.AddAccess(AccessControlType.Allow, sid, 256, InheritanceFlags.ContainerInherit, PropagationFlags.None, ObjectAceFlags.ObjectAceTypePresent, WellKnownGuid.SendAsExtendedRightGuid, Guid.Empty);
				}
			}
			discretionaryAcl3.AddAccess(AccessControlType.Allow, linkedUserSid, 48, InheritanceFlags.ContainerInherit, PropagationFlags.None, ObjectAceFlags.ObjectAceTypePresent, WellKnownGuid.PersonalInfoPropSetGuid, Guid.Empty);
			byte[] binaryForm2 = new byte[discretionaryAcl2.BinaryLength];
			byte[] binaryForm3 = new byte[discretionaryAcl3.BinaryLength];
			discretionaryAcl2.GetBinaryForm(binaryForm2, 0);
			discretionaryAcl3.GetBinaryForm(binaryForm3, 0);
			exchangeSecurityDescriptor.DiscretionaryAcl = new RawAcl(binaryForm2, 0);
			ntSecurityDescriptor.DiscretionaryAcl = new RawAcl(binaryForm3, 0);
		}

		internal static ADRecipient FindConnectedMailbox(IRecipientSession globalCatalogSession, Guid mailboxGuid, Task.TaskVerboseLoggingDelegate logHandler)
		{
			OrFilter filter = new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.ExchangeGuid, mailboxGuid),
				new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.ArchiveGuid, mailboxGuid)
			});
			logHandler(TaskVerboseStringHelper.GetFindDataObjectsVerboseString(globalCatalogSession, typeof(ADRecipient), filter, null, true));
			ADUser[] array = null;
			try
			{
				array = globalCatalogSession.FindADUser(null, QueryScope.SubTree, filter, null, 0);
			}
			finally
			{
				logHandler(TaskVerboseStringHelper.GetSourceVerboseString(globalCatalogSession));
			}
			if (array != null && array.Length > 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if ((array[i].ExchangeGuid == mailboxGuid && array[i].Database != null) || (array[i].ArchiveGuid == mailboxGuid && array[i].ArchiveState == ArchiveState.Local))
					{
						return array[i];
					}
					TaskLogger.Trace("User {0} has ExchangeGuid/ArchiveGuid pointing to mailbox {1} but is not really connected to it.", new object[]
					{
						array[i].Identity,
						mailboxGuid
					});
				}
			}
			return null;
		}

		internal static void ValidateMailboxIsDisconnected(IRecipientSession globalCatalogSession, Guid mailboxGuid, Task.TaskVerboseLoggingDelegate logHandler, Task.ErrorLoggerDelegate errorHandler)
		{
			ADRecipient adrecipient = MailboxTaskHelper.FindConnectedMailbox(globalCatalogSession, mailboxGuid, logHandler);
			if (adrecipient != null)
			{
				errorHandler(new TaskInvalidOperationException(Strings.ErrorMailboxIsConnected(adrecipient.DisplayName, mailboxGuid.ToString())), ExchangeErrorCategory.Client, null);
			}
		}

		internal static void StampMailboxRecipientTypes(ADRecipient recipient, string parameterSetName)
		{
			if (parameterSetName == "Linked" || parameterSetName == "LinkedWithSyncMailbox")
			{
				recipient.RecipientTypeDetails = RecipientTypeDetails.LinkedMailbox;
			}
			else if (parameterSetName == "Shared")
			{
				recipient.RecipientTypeDetails = RecipientTypeDetails.SharedMailbox;
			}
			else if (parameterSetName == "TeamMailboxIW" || parameterSetName == "TeamMailboxITPro")
			{
				recipient.RecipientTypeDetails = RecipientTypeDetails.TeamMailbox;
			}
			else if (parameterSetName == "Room" || parameterSetName == "EnableRoomMailboxAccount")
			{
				recipient.RecipientTypeDetails = RecipientTypeDetails.RoomMailbox;
			}
			else if (parameterSetName == "LinkedRoomMailbox")
			{
				recipient.RecipientTypeDetails = RecipientTypeDetails.LinkedRoomMailbox;
			}
			else if (parameterSetName == "Equipment")
			{
				recipient.RecipientTypeDetails = RecipientTypeDetails.EquipmentMailbox;
			}
			else if (parameterSetName == "Arbitration")
			{
				recipient.RecipientTypeDetails = RecipientTypeDetails.ArbitrationMailbox;
			}
			else if (parameterSetName == "Discovery")
			{
				recipient.RecipientTypeDetails = RecipientTypeDetails.DiscoveryMailbox;
			}
			else if (parameterSetName == "MailboxPlan")
			{
				recipient.RecipientTypeDetails = RecipientTypeDetails.MailboxPlan;
			}
			else if (parameterSetName == "PublicFolder")
			{
				recipient.RecipientTypeDetails = RecipientTypeDetails.PublicFolderMailbox;
			}
			else if (parameterSetName == "GroupMailbox")
			{
				recipient.RecipientTypeDetails = RecipientTypeDetails.GroupMailbox;
			}
			else if (parameterSetName == "AuditLog")
			{
				recipient.RecipientTypeDetails = RecipientTypeDetails.AuditLogMailbox;
			}
			else if (parameterSetName == "Monitoring")
			{
				recipient.RecipientTypeDetails = RecipientTypeDetails.MonitoringMailbox;
			}
			else
			{
				recipient.RecipientTypeDetails = RecipientTypeDetails.UserMailbox;
			}
			MailboxTaskHelper.StampMailboxRecipientDisplayType(recipient);
		}

		internal static void StampMailboxRecipientDisplayType(ADRecipient recipient)
		{
			if (RecipientTypeDetails.LinkedMailbox == recipient.RecipientTypeDetails)
			{
				recipient.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.ACLableMailboxUser);
				return;
			}
			if (RecipientTypeDetails.SharedMailbox == recipient.RecipientTypeDetails)
			{
				recipient.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.MailboxUser);
				return;
			}
			if (RecipientTypeDetails.TeamMailbox == recipient.RecipientTypeDetails)
			{
				recipient.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.TeamMailboxUser);
				return;
			}
			if (RecipientTypeDetails.RoomMailbox == recipient.RecipientTypeDetails || RecipientTypeDetails.LinkedRoomMailbox == recipient.RecipientTypeDetails)
			{
				recipient.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.ConferenceRoomMailbox);
				return;
			}
			if (RecipientTypeDetails.EquipmentMailbox == recipient.RecipientTypeDetails)
			{
				recipient.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.EquipmentMailbox);
				return;
			}
			if (RecipientTypeDetails.UserMailbox == recipient.RecipientTypeDetails)
			{
				recipient.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.ACLableMailboxUser);
				return;
			}
			if (RecipientTypeDetails.LegacyMailbox == recipient.RecipientTypeDetails)
			{
				recipient.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.ACLableMailboxUser);
				return;
			}
			if (RecipientTypeDetails.ArbitrationMailbox == recipient.RecipientTypeDetails)
			{
				recipient.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.ArbitrationMailbox);
				return;
			}
			if (RecipientTypeDetails.MailboxPlan == recipient.RecipientTypeDetails)
			{
				recipient.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.MailboxPlan);
				return;
			}
			if (RecipientTypeDetails.LinkedUser == recipient.RecipientTypeDetails)
			{
				recipient.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.LinkedUser);
				return;
			}
			if (RecipientTypeDetails.RoomList == recipient.RecipientTypeDetails)
			{
				recipient.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.DistributionGroup);
				return;
			}
			if (RecipientTypeDetails.GroupMailbox == recipient.RecipientTypeDetails)
			{
				recipient.RecipientDisplayType = new RecipientDisplayType?(RecipientDisplayType.GroupMailboxUser);
				return;
			}
			if (RecipientTypeDetails.DiscoveryMailbox == recipient.RecipientTypeDetails)
			{
				recipient.RecipientDisplayType = null;
				return;
			}
			if (RecipientTypeDetails.AuditLogMailbox == recipient.RecipientTypeDetails)
			{
				recipient.RecipientDisplayType = null;
			}
		}

		internal static void IsLiveIdExists(IRecipientSession recipientSession, SmtpAddress windowsLiveID, NetID netId, Task.ErrorLoggerDelegate errorLogger)
		{
			ADUser aduser = MailboxTaskHelper.FindADUserByNetId(netId, recipientSession);
			if (aduser == null)
			{
				ADUser aduser2 = MailboxTaskHelper.FindADUserByWindowsLiveId(windowsLiveID, recipientSession);
				if (aduser2 != null)
				{
					errorLogger(new UserWithMatchingWindowsLiveIdAndDifferentNetIdExistsException(Strings.ErrorUserWithMatchingWindowsLiveIdAndDifferentNetIdExists(windowsLiveID.ToString(), aduser2.Identity.ToString())), ExchangeErrorCategory.Client, null);
				}
				return;
			}
			if (aduser.WindowsLiveID.Equals(windowsLiveID))
			{
				errorLogger(new WindowsLiveIdAlreadyUsedException(Strings.ErrorWindowsLiveIdAssociatedWithAnotherRecipient(windowsLiveID.ToString(), aduser.Identity.ToString())), ExchangeErrorCategory.WindowsLiveIdAlreadyUsed, null);
				return;
			}
			errorLogger(new UserWithMatchingNetIdAndDifferentWindowsLiveIdExistsException(Strings.ErrorUserWithMatchingNetIdAndDifferentWindowsLiveIdExists(windowsLiveID.ToString(), aduser.Identity.ToString())), ExchangeErrorCategory.Client, null);
		}

		internal static void IsMemberExists(IRecipientSession recipientSession, SmtpAddress windowsLiveID, Task.ErrorLoggerDelegate errorLogger)
		{
			ADUser aduser = MailboxTaskHelper.FindADUserByWindowsLiveId(windowsLiveID, recipientSession);
			if (aduser != null)
			{
				errorLogger(new UserWithMatchingWindowsLiveIdExistsException(Strings.ErrorUserWithMatchingWindowsLiveIdExists(windowsLiveID.ToString(), aduser.Identity.ToString())), ExchangeErrorCategory.Client, null);
			}
		}

		private static ADUser FindADUserByNetId(NetID netId, IRecipientSession recipientSession)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, IADSecurityPrincipalSchema.NetID, netId);
			ADUser[] array = recipientSession.FindADUser(null, QueryScope.SubTree, filter, null, 1);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			return array[0];
		}

		private static ADUser FindADUserByWindowsLiveId(SmtpAddress windowsLiveId, IRecipientSession recipientSession)
		{
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.WindowsLiveID, windowsLiveId);
			ADUser[] array = recipientSession.FindADUser(null, QueryScope.SubTree, filter, null, 1);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			return array[0];
		}

		public static void CheckNameAvailability(IRecipientSession recipientSession, string name, ADObjectId parentContainer, Task.ErrorLoggerDelegate errorLogger)
		{
			ADRecipient adrecipient = recipientSession.Read(parentContainer.GetChildId(name));
			if (adrecipient != null)
			{
				errorLogger(new NameNotAvailableException(Strings.ErrorNameNotAvailable(name)), ExchangeErrorCategory.Client, null);
			}
		}

		internal static string AppendRandomNameSuffix(string name)
		{
			return name.Substring(0, Math.Min(name.Length, 53)) + "_" + Guid.NewGuid().ToString("N").Substring(0, 10);
		}

		internal static void SetMailboxPassword(IRecipientSession recipientSession, ADUser user, string staticPassword, Task.ErrorLoggerDelegate errorLogger)
		{
			int num = 0;
			for (;;)
			{
				string password;
				if (staticPassword == null)
				{
					password = PasswordHelper.GetRandomPassword(user.DisplayName, user.SamAccountName, 128);
				}
				else
				{
					password = staticPassword;
				}
				try
				{
					using (SecureString secureString = password.ConvertToSecureString())
					{
						recipientSession.SetPassword(user, secureString);
					}
				}
				catch (ADInvalidPasswordException ex)
				{
					if (num != 3 && staticPassword == null)
					{
						num++;
						continue;
					}
					errorLogger(new InvalidADObjectOperationException(Strings.ErrorFailedToGenerateRandomPassword(password, user.DisplayName, user.SamAccountName, ex.Message)), ExchangeErrorCategory.ServerOperation, user.Identity);
				}
				break;
			}
		}

		internal static bool ExcludeArbitrationMailbox(ADRecipient mbx, bool showArbitration)
		{
			return mbx != null && ((!showArbitration && RecipientTypeDetails.ArbitrationMailbox == mbx.RecipientTypeDetails) || (showArbitration && RecipientTypeDetails.ArbitrationMailbox != mbx.RecipientTypeDetails));
		}

		internal static bool ExcludePublicFolderMailbox(ADRecipient mbx, bool showPublicFolder)
		{
			return mbx != null && ((!showPublicFolder && RecipientTypeDetails.PublicFolderMailbox == mbx.RecipientTypeDetails) || (showPublicFolder && RecipientTypeDetails.PublicFolderMailbox != mbx.RecipientTypeDetails));
		}

		internal static bool ExcludeMailboxPlan(ADRecipient mbx, bool showMailboxPlan)
		{
			return mbx != null && ((!showMailboxPlan && RecipientTypeDetails.MailboxPlan == mbx.RecipientTypeDetails) || (showMailboxPlan && RecipientTypeDetails.MailboxPlan != mbx.RecipientTypeDetails));
		}

		internal static bool ExcludeGroupMailbox(ADRecipient mbx, bool showGroupMailbox)
		{
			return mbx != null && ((!showGroupMailbox && RecipientTypeDetails.GroupMailbox == mbx.RecipientTypeDetails) || (showGroupMailbox && RecipientTypeDetails.GroupMailbox != mbx.RecipientTypeDetails));
		}

		internal static bool ExcludeTeamMailbox(ADRecipient mbx, bool showTeamMailbox)
		{
			return mbx != null && ((!showTeamMailbox && RecipientTypeDetails.TeamMailbox == mbx.RecipientTypeDetails) || (showTeamMailbox && RecipientTypeDetails.TeamMailbox != mbx.RecipientTypeDetails));
		}

		internal static bool ExcludeAuditLogMailbox(ADRecipient mbx, bool showAuditLog)
		{
			return mbx != null && showAuditLog != (RecipientTypeDetails.AuditLogMailbox == mbx.RecipientTypeDetails);
		}

		internal static void UpdateHostedExchangeSecurityGroupForMailbox(IRecipientSession recipientSession, OrganizationId orgId, Guid wellKnownGroup, ADUser mailbox, bool addMember, Task.TaskVerboseLoggingDelegate writeVerbose, Task.TaskWarningLoggingDelegate writeWarning)
		{
			if (null == orgId)
			{
				throw new ArgumentNullException("orgId");
			}
			if (OrganizationId.ForestWideOrgId.Equals(orgId))
			{
				return;
			}
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			if (recipientSession.UseGlobalCatalog)
			{
				throw new ArgumentOutOfRangeException("recipientSession.UseGlobalCatalog");
			}
			ADGroup adgroup = MailboxTaskHelper.ResolveWellknownExchangeSecurityGroup(recipientSession, orgId, wellKnownGroup, writeVerbose, writeWarning);
			if (adgroup != null)
			{
				MailboxTaskHelper.UpdateExchangeSecurityGroupForMailbox(recipientSession, adgroup, mailbox, addMember, writeVerbose, writeWarning);
			}
		}

		internal static void UpdateExchangeSecurityGroupForMailbox(IRecipientSession recipientSession, ADGroup group, ADUser mailbox, bool addMember, Task.TaskVerboseLoggingDelegate writeVerbose, Task.TaskWarningLoggingDelegate writeWarning)
		{
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			if (recipientSession.UseGlobalCatalog)
			{
				throw new ArgumentOutOfRangeException("recipientSession.UseGlobalCatalog");
			}
			LocalizedException ex = null;
			if ((addMember && group.Members.Contains(mailbox.Id)) || (group.Members.IsCompletelyRead && !addMember && !group.Members.Contains(mailbox.Id)))
			{
				return;
			}
			try
			{
				group.Members.Clear();
				group.ResetChangeTracking();
				group.Members.Add(mailbox.Id);
				if (!addMember)
				{
					group.ResetChangeTracking();
					group.Members.Remove(mailbox.Id);
				}
				if (group.m_Session != recipientSession)
				{
					recipientSession = (IRecipientSession)group.m_Session;
				}
				recipientSession.Save(group);
			}
			catch (ADTransientException ex2)
			{
				ex = ex2;
			}
			catch (ADOperationException ex3)
			{
				if (addMember && !(ex3 is ADObjectEntryAlreadyExistsException))
				{
					ex = ex3;
				}
			}
			catch (DataValidationException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				if (addMember)
				{
					writeWarning(Strings.ErrorCanNotAddMailboxToWellKnownHostedMailboxSG(mailbox.Id.ToString(), group.Name, (group.OrganizationId != null) ? group.OrganizationId.OrganizationalUnit.Name : "/", ex.Message));
					ExEventLog.EventTuple tuple_FailedToAddToUSG = ManagementEventLogConstants.Tuple_FailedToAddToUSG;
					ExManagementApplicationLogger.LogEvent(tuple_FailedToAddToUSG, new string[]
					{
						mailbox.Id.ToString(),
						group.Name,
						(group.OrganizationId != null) ? group.OrganizationId.OrganizationalUnit.Name : "/",
						ex.Message
					});
					return;
				}
				writeWarning(Strings.ErrorCanNotRemoveMailboxToWellKnownHostedMailboxSG(mailbox.Id.ToString(), group.Name, (group.OrganizationId != null) ? group.OrganizationId.OrganizationalUnit.Name : "/", ex.Message));
				ExEventLog.EventTuple tuple_FailedToRemoveFromUSG = ManagementEventLogConstants.Tuple_FailedToRemoveFromUSG;
				ExManagementApplicationLogger.LogEvent(tuple_FailedToRemoveFromUSG, new string[]
				{
					mailbox.Id.ToString(),
					group.Name,
					(group.OrganizationId != null) ? group.OrganizationId.OrganizationalUnit.Name : "/",
					ex.Message
				});
			}
		}

		public static ADGroup ResolveWellknownExchangeSecurityGroup(IRecipientSession recipientSession, OrganizationId orgId, Guid wellKnownGroup, Task.TaskVerboseLoggingDelegate writeVerbose, Task.TaskWarningLoggingDelegate writeWarning)
		{
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			if (recipientSession.UseGlobalCatalog)
			{
				throw new ArgumentOutOfRangeException("recipientSession.UseGlobalCatalog");
			}
			if (wellKnownGroup.Equals(WellKnownGuid.EopsWkGuid))
			{
				string exchangePasswordSettingsSG = InitializeTenantUniversalGroups.ExchangePasswordSettingsSG;
				string domainController = recipientSession.DomainController;
				bool useConfigNC = recipientSession.UseConfigNC;
				bool useGlobalCatalog = recipientSession.UseGlobalCatalog;
				ADGroup adgroup = null;
				try
				{
					recipientSession.UseConfigNC = false;
					recipientSession.UseGlobalCatalog = true;
					bool skipRangedAttributes = recipientSession.SkipRangedAttributes;
					try
					{
						recipientSession.SkipRangedAttributes = true;
						adgroup = recipientSession.ResolveWellKnownGuid<ADGroup>(wellKnownGroup, orgId.ConfigurationUnit);
					}
					finally
					{
						recipientSession.SkipRangedAttributes = skipRangedAttributes;
					}
				}
				finally
				{
					recipientSession.UseConfigNC = useConfigNC;
					recipientSession.UseGlobalCatalog = useGlobalCatalog;
				}
				if (adgroup == null)
				{
					writeWarning(Strings.ErrorWellKnownHostedMailboxSGNotFound(exchangePasswordSettingsSG, (orgId != null) ? orgId.OrganizationalUnit.Name : "/"));
					ExEventLog.EventTuple tuple_FailedToFindUSG = ManagementEventLogConstants.Tuple_FailedToFindUSG;
					ExManagementApplicationLogger.LogEvent(tuple_FailedToFindUSG, new string[]
					{
						exchangePasswordSettingsSG,
						(orgId != null) ? orgId.OrganizationalUnit.Name : "/"
					});
				}
				return adgroup;
			}
			throw new ArgumentOutOfRangeException("wellKnownGroup");
		}

		private static ADGroup ResolveForestWideUSGGuid(IRecipientSession recipientSession, Guid wkg)
		{
			ADObjectId configurationNamingContext = ADSession.GetConfigurationNamingContext(recipientSession.SessionSettings.GetAccountOrResourceForestFqdn());
			ADGroup adgroup = null;
			bool useConfigNC = recipientSession.UseConfigNC;
			bool useGlobalCatalog = recipientSession.UseGlobalCatalog;
			recipientSession.UseConfigNC = false;
			recipientSession.UseGlobalCatalog = true;
			ADGroup result;
			try
			{
				try
				{
					adgroup = recipientSession.ResolveWellKnownGuid<ADGroup>(wkg, configurationNamingContext);
				}
				catch (ADReferralException)
				{
				}
				result = adgroup;
			}
			finally
			{
				recipientSession.UseConfigNC = useConfigNC;
				recipientSession.UseGlobalCatalog = useGlobalCatalog;
			}
			return result;
		}

		internal static string GetNameOfAcceptableLengthForMultiTenantMode(string name, out LocalizedString warning)
		{
			string text = name;
			warning = LocalizedString.Empty;
			if (text.Length > 64 && VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.LimitNameMaxlength.Enabled)
			{
				string text2 = Guid.NewGuid().ToString("N");
				text = text.Substring(0, 64 - text2.Length - 1) + text2;
				warning = Strings.WarningChangingUserSuppliedName(name, text);
			}
			return text;
		}

		internal static ADObject LookupManager(UserContactIdParameter managerId, DataAccessHelper.CategorizedGetDataObjectDelegate getDataObjectDelegate, ExchangeErrorCategory errorCategory, IRecipientSession tenantGlobalCatalogSession)
		{
			if (managerId != null)
			{
				return (ADRecipient)getDataObjectDelegate(managerId, tenantGlobalCatalogSession, null, null, new LocalizedString?(Strings.ErrorUserOrContactNotFound(managerId.ToString())), new LocalizedString?(Strings.ErrorUserOrContactNotUnique(managerId.ToString())), errorCategory);
			}
			return null;
		}

		internal static ADObjectId GetArbitrationMailbox(IRecipientSession adRecipientSession, ADObjectId orgAdObjectId)
		{
			return MailboxTaskHelper.GetArbitrationMailbox(adRecipientSession, orgAdObjectId, null);
		}

		internal static ADObjectId GetArbitrationMailbox(IRecipientSession adRecipientSession, ADObjectId orgAdObjectId, ADObjectId excludedArbitrationMailboxId)
		{
			if (adRecipientSession == null)
			{
				throw new ArgumentNullException("adRecipientSession");
			}
			if (orgAdObjectId == null)
			{
				throw new ArgumentNullException("orgAdObjectId");
			}
			ADObjectId descendantId = orgAdObjectId.GetDescendantId(ApprovalApplication.ParentPathInternal);
			ADObjectId childId = descendantId.GetChildId("ModeratedRecipients");
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.ArbitrationMailbox);
			QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.ApprovalApplications, childId);
			QueryFilter filter;
			if (excludedArbitrationMailboxId == null)
			{
				filter = new AndFilter(new QueryFilter[]
				{
					queryFilter,
					queryFilter2
				});
			}
			else
			{
				filter = new AndFilter(new QueryFilter[]
				{
					queryFilter,
					queryFilter2,
					new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Id, excludedArbitrationMailboxId)
				});
			}
			ADPagedReader<ADRecipient> adpagedReader = adRecipientSession.FindPaged(null, QueryScope.SubTree, filter, null, 1);
			using (IEnumerator<ADRecipient> enumerator = adpagedReader.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					ADRecipient adrecipient = enumerator.Current;
					return adrecipient.Id;
				}
			}
			return null;
		}

		internal static void ApplyMailboxPlansDelta(ADUser oldPlan, ADUser newPlan, ADUser target, ApplyMailboxPlanFlags flags)
		{
			ADPresentationObject[] array;
			if (oldPlan != null)
			{
				array = new ADPresentationObject[]
				{
					new Mailbox(oldPlan),
					new User(oldPlan),
					new CASMailbox(oldPlan),
					new UMMailbox(oldPlan)
				};
			}
			else
			{
				ADPresentationObject[] array2 = new ADPresentationObject[4];
				array = array2;
			}
			ADPresentationObject[] array3 = new ADPresentationObject[]
			{
				new Mailbox(newPlan),
				new User(newPlan),
				new CASMailbox(newPlan),
				new UMMailbox(newPlan)
			};
			ADPresentationObject[] array4 = new ADPresentationObject[]
			{
				new Mailbox(target),
				new User(target),
				new CASMailbox(target),
				new UMMailbox(target)
			};
			for (int i = 0; i < array4.Length; i++)
			{
				ADPresentationObject.ApplyPresentationObjectDelta(array[i], array3[i], array4[i], flags);
			}
		}

		internal static void ApplyDefaultArchivePolicy(ADUser user, IConfigurationSession configSession)
		{
			if (user.RetentionPolicy != null)
			{
				return;
			}
			ADObjectId childId;
			if (user.OrganizationId.ConfigurationUnit == null)
			{
				childId = configSession.GetOrgContainerId().GetChildId("Retention Policies Container").GetChildId(RecipientConstants.DefaultArchiveAndRetentionPolicyName);
			}
			else
			{
				childId = user.OrganizationId.ConfigurationUnit.GetChildId("Retention Policies Container").GetChildId(RecipientConstants.DefaultArchiveAndRetentionPolicyName);
			}
			if (configSession.Read<RetentionPolicy>(childId) != null)
			{
				user.RetentionPolicy = childId;
			}
		}

		internal static IRecipientSession GetSessionForDeletedObjects(Fqdn domainController, OrganizationId currentOrganizationId, OrganizationId executingUserOrganizationId)
		{
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), currentOrganizationId, executingUserOrganizationId, true);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(domainController, true, ConsistencyMode.IgnoreInvalid, sessionSettings, 1884, "GetSessionForDeletedObjects", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\recipient\\MailboxTaskHelper.cs");
			tenantOrRootOrgRecipientSession.UseGlobalCatalog = true;
			return tenantOrRootOrgRecipientSession;
		}

		internal static RemovedMailbox GetRemovedMailbox(Fqdn domainController, OrganizationId organizationId, OrganizationId executingUserOrganizationId, RemovedMailboxIdParameter identity, Task.ErrorLoggerDelegate errorLogger)
		{
			RemovedMailbox result = null;
			IRecipientSession sessionForDeletedObjects = MailboxTaskHelper.GetSessionForDeletedObjects(domainController, organizationId, executingUserOrganizationId);
			IEnumerable<RemovedMailbox> enumerable = identity.GetObjects<RemovedMailbox>(organizationId.OrganizationalUnit, sessionForDeletedObjects) ?? new List<RemovedMailbox>();
			using (IEnumerator<RemovedMailbox> enumerator = enumerable.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					errorLogger(new RecipientTaskException(Strings.ErrorRemovedMailboxNotFound(identity.ToString())), ExchangeErrorCategory.Client, null);
				}
				result = enumerator.Current;
				if (enumerator.MoveNext())
				{
					errorLogger(new RecipientTaskException(Strings.ErrorRemovedMailboxNotUnique(identity.ToString())), ExchangeErrorCategory.Client, null);
				}
			}
			return result;
		}

		internal static MailboxStatistics GetDeletedStoreMailbox(IConfigDataProvider dataSession, StoreMailboxIdParameter identity, ObjectId rootId, DatabaseIdParameter databaseId, Task.ErrorLoggerDelegate errorHandler)
		{
			MailboxStatistics[] storeMailboxesFromId = MapiTaskHelper.GetStoreMailboxesFromId(dataSession, identity, rootId);
			if (storeMailboxesFromId == null || storeMailboxesFromId.Length == 0)
			{
				errorHandler(new MdbAdminTaskException(Strings.ErrorStoreMailboxNotFound(identity.ToString(), databaseId.ToString())), ExchangeErrorCategory.Client, identity);
			}
			if (storeMailboxesFromId.Length > 1)
			{
				errorHandler(new MdbAdminTaskException(Strings.ErrorStoreMailboxNotUnique(identity.ToString(), databaseId.ToString())), ExchangeErrorCategory.Client, identity);
			}
			ObjectClass objectClass = storeMailboxesFromId[0].ObjectClass;
			if ((ObjectClass.ExOleDbSystemMailbox & objectClass) != ObjectClass.Unknown)
			{
				errorHandler(new MdbAdminTaskException(Strings.ErrorConnectSystemMailbox(identity.ToString(), databaseId.ToString())), ExchangeErrorCategory.Client, identity);
			}
			if ((ObjectClass.SystemAttendantMailbox & objectClass) != ObjectClass.Unknown)
			{
				errorHandler(new MdbAdminTaskException(Strings.ErrorConnectSystemAttendantMailbox(identity.ToString(), databaseId.ToString())), ExchangeErrorCategory.Client, identity);
			}
			return storeMailboxesFromId[0];
		}

		internal static bool HasPublicFolderDatabases(DataAccessHelper.CategorizedGetDataObjectDelegate getDataObjectDelegate, ITopologyConfigurationSession globalConfigSession)
		{
			ServerIdParameter serverIdParameter = ServerIdParameter.Parse(Environment.MachineName);
			Server server = (Server)getDataObjectDelegate(serverIdParameter, globalConfigSession, null, null, new LocalizedString?(Strings.ErrorServerNotFound(serverIdParameter.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(serverIdParameter.ToString())), ExchangeErrorCategory.Client);
			PublicFolderDatabase[] publicFolderDatabases = server.GetPublicFolderDatabases();
			PublicFolderDatabase publicFolderDatabase = null;
			if (publicFolderDatabases.Length == 0)
			{
				ADObjectId adobjectId = PublicFolderDatabase.FindClosestPublicFolderDatabase(globalConfigSession, server.Id);
				if (adobjectId != null)
				{
					publicFolderDatabase = globalConfigSession.Read<PublicFolderDatabase>(adobjectId);
				}
			}
			else
			{
				publicFolderDatabase = publicFolderDatabases[0];
			}
			return publicFolderDatabase != null;
		}

		internal static bool IsReservedLiveId(SmtpAddress windowsLiveId)
		{
			return windowsLiveId.Local.Equals("c4e67852e761400490f0750a898dc64e") || windowsLiveId.Local.StartsWith("ExRemoved-");
		}

		internal static void BlockRemoveOrDisableIfLitigationHoldEnabled(ADUser dataObject, Task.ErrorLoggerDelegate writeError, bool isDisableOperation, bool ignoreLitigationHold)
		{
			if (!ignoreLitigationHold && dataObject.LitigationHoldEnabled)
			{
				string mbxId = dataObject.Identity.ToString();
				LocalizedString message;
				if (isDisableOperation)
				{
					if (dataObject.RecipientType == RecipientType.MailUser)
					{
						message = Strings.ErrorDisableMailuserWithLitigationHold(mbxId);
					}
					else
					{
						message = Strings.ErrorDisableMailboxWithLitigationHold(mbxId);
					}
				}
				else if (dataObject.RecipientType == RecipientType.MailUser)
				{
					message = Strings.ErrorRemoveMailuserWithLitigationHold(mbxId);
				}
				else
				{
					message = Strings.ErrorRemoveMailboxWithLitigationHold(mbxId);
				}
				writeError(new RecipientTaskException(message), ExchangeErrorCategory.Client, dataObject.Identity);
			}
		}

		internal static void BlockRemoveOrDisableIfDiscoveryHoldEnabled(ADUser dataObject, Task.ErrorLoggerDelegate writeError, bool isDisableOperation, bool ignoreLitigationHold)
		{
			if (!ignoreLitigationHold && dataObject.InPlaceHolds != null && dataObject.InPlaceHolds.Count > 0 && dataObject.RecipientType != RecipientType.MailUser)
			{
				string mbxId = dataObject.Identity.ToString();
				LocalizedString message;
				if (isDisableOperation)
				{
					message = Strings.ErrorDisableMailboxWithDiscoveryHold(mbxId);
				}
				else
				{
					message = Strings.ErrorRemoveMailboxWithDiscoveryHold(mbxId);
				}
				writeError(new RecipientTaskException(message), ExchangeErrorCategory.Client, dataObject.Identity);
			}
		}

		internal static void BlockRemoveOrDisableIfJournalNDRMailbox(ADUser dataObject, IConfigurationSession configSession, Task.ErrorLoggerDelegate writeError, bool isDisableOperation = false)
		{
			if (dataObject.OrganizationId != OrganizationId.ForestWideOrgId)
			{
				TransportConfigContainer[] array = configSession.Find<TransportConfigContainer>(dataObject.OrganizationId.ConfigurationUnit, QueryScope.SubTree, null, null, 1);
				if (array != null && array.Length == 1)
				{
					dataObject.Identity.ToString();
					SmtpAddress journalingReportNdrTo = array[0].JournalingReportNdrTo;
					if (journalingReportNdrTo != SmtpAddress.NullReversePath && journalingReportNdrTo == dataObject.PrimarySmtpAddress)
					{
						writeError(new RecipientTaskException(isDisableOperation ? Strings.ErrorDisableMailboxIsJournalReportNdrTo(dataObject.Identity.ToString()) : Strings.ErrorRemoveMailboxIsJournalReportNdrTo(dataObject.Identity.ToString())), ExchangeErrorCategory.Client, dataObject.Identity);
					}
				}
			}
		}

		internal static void BlockRemoveOrDisableMailUserIfJournalArchiveEnabled(IRecipientSession recipientSession, IConfigurationSession configurationSession, ADUser dataObject, Task.ErrorLoggerDelegate writeError, bool isDisableOperation, bool isSyncOperation)
		{
			if (dataObject.IsSoftDeleted)
			{
				TaskLogger.Trace("Mail user is soft deleted, skip blocking.", new object[0]);
				return;
			}
			if (dataObject.JournalArchiveAddress == SmtpAddress.Empty)
			{
				TaskLogger.Trace("Mail user has no journal archive address, skip blocking.", new object[0]);
				return;
			}
			bool flag = false;
			SmtpProxyAddress proxyAddress = new SmtpProxyAddress(dataObject.JournalArchiveAddress.ToString(), true);
			try
			{
				ADRecipient adrecipient = recipientSession.FindByProxyAddress(proxyAddress);
				if (adrecipient != null && adrecipient.RecipientTypeDetails == RecipientTypeDetails.UserMailbox)
				{
					flag = true;
				}
			}
			catch (NonUniqueRecipientException)
			{
				writeError(new RecipientTaskException(Strings.ErrorMailuserWithMultipleJournalArchive(dataObject.JournalArchiveAddress.ToString())), ExchangeErrorCategory.Client, dataObject.Identity);
				return;
			}
			bool flag2 = false;
			if (isSyncOperation)
			{
				flag2 = (dataObject.IsDirSyncEnabled && MailboxTaskHelper.IsOrgDirSyncEnabled(configurationSession, dataObject.OrganizationId));
			}
			if (flag && !flag2)
			{
				LocalizedString message;
				if (isDisableOperation)
				{
					message = Strings.ErrorDisableMailuserWithJournalArchive;
				}
				else
				{
					message = Strings.ErrorRemoveMailuserWithJournalArchive;
				}
				writeError(new RecipientTaskException(message), ExchangeErrorCategory.Client, dataObject.Identity);
				return;
			}
			TaskLogger.Trace("Block was skipped because journalMailboxExists = {0} and ignoreBlock = {1}.", new object[]
			{
				flag,
				flag2
			});
		}

		internal static void BlockRemoveOrDisableMailboxIfJournalArchiveEnabled(IRecipientSession recipientSession, IConfigurationSession configurationSession, ADUser dataObject, Task.ErrorLoggerDelegate writeError, bool isDisableOperation)
		{
			if (dataObject.IsSoftDeleted)
			{
				TaskLogger.Trace("Mailbox user is soft deleted, skip blocking.", new object[0]);
				return;
			}
			if (dataObject.PrimarySmtpAddress == SmtpAddress.Empty)
			{
				TaskLogger.Trace("Mailbox user has no primary smtp address, skip blocking.", new object[0]);
				return;
			}
			ADRecipient journalArchiveMailUser = MailboxTaskHelper.GetJournalArchiveMailUser(recipientSession, dataObject);
			if (journalArchiveMailUser != null)
			{
				bool flag = journalArchiveMailUser.IsDirSyncEnabled && MailboxTaskHelper.IsOrgDirSyncEnabled(configurationSession, journalArchiveMailUser.OrganizationId);
				LocalizedString message;
				if (isDisableOperation)
				{
					if (flag)
					{
						message = Strings.ErrorDisableMailboxWithJournalArchiveWithDirSync;
					}
					else
					{
						message = Strings.ErrorDisableMailboxWithJournalArchive;
					}
				}
				else if (flag)
				{
					message = Strings.ErrorRemoveMailboxWithJournalArchiveWithDirSync;
				}
				else
				{
					message = Strings.ErrorRemoveMailboxWithJournalArchive;
				}
				writeError(new RecipientTaskException(message), ExchangeErrorCategory.Client, dataObject.Identity);
				return;
			}
			TaskLogger.Trace("No user is using this mailbox for journal archiving, skip blocking.", new object[0]);
		}

		internal static ADRecipient GetJournalArchiveMailUser(IRecipientSession recipientSession, ADUser dataObject)
		{
			ADRecipient result = null;
			CustomProxyAddress customProxyAddress = new CustomProxyAddress((CustomProxyAddressPrefix)ProxyAddressPrefix.JRNL, dataObject.PrimarySmtpAddress.ToString(), false);
			ComparisonFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.EmailAddresses, customProxyAddress.ToString());
			ADRecipient[] array = recipientSession.Find<ADRecipient>(null, QueryScope.SubTree, filter, null, 2);
			foreach (ADRecipient adrecipient in array)
			{
				if (!adrecipient.Identity.Equals(dataObject.Identity))
				{
					result = adrecipient;
					break;
				}
			}
			return result;
		}

		internal static bool IsOrgDirSyncEnabled(IConfigurationSession configurationSession, OrganizationId organizationId)
		{
			if (organizationId == null)
			{
				TaskLogger.Trace("Unable to determine organization dirSync status because organizationId is null.", new object[0]);
				return false;
			}
			ExchangeConfigurationUnit exchangeConfigUnit = RecipientTaskHelper.GetExchangeConfigUnit(configurationSession, organizationId);
			if (exchangeConfigUnit != null)
			{
				return exchangeConfigUnit.IsDirSyncEnabled;
			}
			TaskLogger.Trace("Unable to determine organization dirSync status because Exchange configuration unit was not found for {0}.", new object[]
			{
				organizationId.ToString()
			});
			return false;
		}

		internal static void ValidateNotBuiltInArbitrationMailbox(ADUser dataObject, Task.ErrorLoggerDelegate writeError, LocalizedString cantRemoveError)
		{
			string name = dataObject.Name;
			if (name.Equals("FederatedEmail.4c1f4d8b-8179-4148-93bf-00a95fa1e042", StringComparison.OrdinalIgnoreCase) || name.Equals("SystemMailbox{e0dc1c29-89c3-4034-b678-e6c29d823ed9}", StringComparison.OrdinalIgnoreCase))
			{
				writeError(new RecipientTaskException(cantRemoveError), ExchangeErrorCategory.Client, dataObject.Identity);
			}
		}

		internal static void ValidateMaximumDiscoveryMailboxQuota(ADUser dataObject, IConfigurationSession configurationSession, OrganizationId currentOrganizationId, Task.ErrorLoggerDelegate errorLogger)
		{
			if (configurationSession == null || currentOrganizationId == null || currentOrganizationId.ConfigurationUnit == null)
			{
				MailboxTaskHelper.ValidateMaximumDiscoveryMailboxQuota(dataObject, errorLogger);
				return;
			}
			ExchangeConfigurationUnit exchangeConfigUnit = RecipientTaskHelper.GetExchangeConfigUnit(configurationSession, currentOrganizationId);
			MailboxTaskHelper.ValidateMaximumDiscoveryMailboxQuota(dataObject, exchangeConfigUnit, errorLogger);
		}

		private static void ValidateMaximumDiscoveryMailboxQuota(ADUser dataObject, Task.ErrorLoggerDelegate errorLogger)
		{
			MailboxTaskHelper.ValidateMaximumDiscoveryMailboxQuota(dataObject, VariantConfiguration.InvariantNoFlightingSnapshot, errorLogger);
		}

		private static void ValidateMaximumDiscoveryMailboxQuota(ADUser dataObject, ExchangeConfigurationUnit configurationUnit, Task.ErrorLoggerDelegate errorLogger)
		{
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(dataObject.GetContext(configurationUnit), null, null);
			MailboxTaskHelper.ValidateMaximumDiscoveryMailboxQuota(dataObject, snapshot, errorLogger);
		}

		private static void ValidateMaximumDiscoveryMailboxQuota(ADUser dataObject, VariantConfigurationSnapshot configurationSnapshot, Task.ErrorLoggerDelegate errorLogger)
		{
			if (dataObject.IsModified(MailboxSchema.ProhibitSendReceiveQuota))
			{
				Unlimited<ByteQuantifiedSize> other = Unlimited<ByteQuantifiedSize>.Parse(configurationSnapshot.Discovery.DiscoveryMailboxMaxProhibitSendReceiveQuota.Value);
				if (((Unlimited<ByteQuantifiedSize>)dataObject[MailboxSchema.ProhibitSendReceiveQuota]).CompareTo(other) > 0)
				{
					errorLogger(new RecipientTaskException(Strings.DiscoveryMailboxQuotaLimitExceeded("ProhibitSendReceiveQuota", other.ToString())), ExchangeErrorCategory.Client, dataObject);
				}
			}
			if (dataObject.IsModified(MailboxSchema.ProhibitSendQuota))
			{
				Unlimited<ByteQuantifiedSize> other2 = Unlimited<ByteQuantifiedSize>.Parse(configurationSnapshot.Discovery.DiscoveryMailboxMaxProhibitSendQuota.Value);
				if (((Unlimited<ByteQuantifiedSize>)dataObject[MailboxSchema.ProhibitSendQuota]).CompareTo(other2) > 0)
				{
					errorLogger(new RecipientTaskException(Strings.DiscoveryMailboxQuotaLimitExceeded("ProhibitSendQuota", other2.ToString())), ExchangeErrorCategory.Client, dataObject);
				}
			}
		}

		internal static void RemoveOrDisablePublicFolderMailbox(ADUser dataObject, Guid mailboxGuid, IConfigurationSession tenantConfigurationSession, Task.ErrorLoggerDelegate writeError, bool isDisableOperation, bool ignoreContentExistenceCheck)
		{
			if (dataObject.ExchangeGuid != Guid.Empty)
			{
				mailboxGuid = dataObject.ExchangeGuid;
			}
			Organization orgContainer = tenantConfigurationSession.GetOrgContainer();
			TenantPublicFolderConfiguration value = TenantPublicFolderConfigurationCache.Instance.GetValue(orgContainer.OrganizationId);
			PublicFolderInformation hierarchyMailboxInformation = value.GetHierarchyMailboxInformation();
			bool flag = hierarchyMailboxInformation.Type == PublicFolderInformation.HierarchyType.InTransitMailboxGuid;
			if (hierarchyMailboxInformation.HierarchyMailboxGuid == mailboxGuid)
			{
				if (value.GetContentMailboxGuids().Length > 0)
				{
					LocalizedString message;
					if (isDisableOperation)
					{
						message = Strings.ErrorCannotDisablePrimaryPublicFolderMailbox(dataObject.Identity.ToString());
					}
					else
					{
						message = Strings.ErrorCannotRemovePrimaryPublicFolderMailbox(dataObject.Identity.ToString());
					}
					writeError(new TaskInvalidOperationException(message), ExchangeErrorCategory.Client, dataObject);
					return;
				}
				if (!flag && !ignoreContentExistenceCheck && MailboxTaskHelper.DoesPublicFolderMailboxContainFoldersWithContents(orgContainer.OrganizationId, Guid.Empty))
				{
					LocalizedString message2;
					if (isDisableOperation)
					{
						message2 = Strings.ErrorCannotDisablePublicFolderMailboxWithFolders;
					}
					else
					{
						message2 = Strings.ErrorCannotRemovePublicFolderMailboxWithFolders;
					}
					writeError(new TaskInvalidOperationException(message2), ExchangeErrorCategory.Client, dataObject);
				}
				if (!hierarchyMailboxInformation.CanUpdate)
				{
					writeError(new RecipientTaskException(Strings.ErrorCannotUpdatePublicFolderHierarchyInformation), ExchangeErrorCategory.Client, dataObject);
				}
				orgContainer.DefaultPublicFolderMailbox = orgContainer.DefaultPublicFolderMailbox.Clone();
				orgContainer.DefaultPublicFolderMailbox.SetHierarchyMailbox(Guid.Empty, PublicFolderInformation.HierarchyType.MailboxGuid);
				tenantConfigurationSession.Save(orgContainer);
				return;
			}
			else
			{
				if (flag)
				{
					return;
				}
				if (!ignoreContentExistenceCheck && MailboxTaskHelper.DoesPublicFolderMailboxContainFoldersWithContents(orgContainer.OrganizationId, mailboxGuid))
				{
					LocalizedString message3;
					if (isDisableOperation)
					{
						message3 = Strings.ErrorCannotDisablePublicFolderMailboxWithFolders;
					}
					else
					{
						message3 = Strings.ErrorCannotRemovePublicFolderMailboxWithFolders;
					}
					writeError(new TaskInvalidOperationException(message3), ExchangeErrorCategory.Client, dataObject);
				}
				return;
			}
		}

		internal static void PrepopulateCacheForMailbox(Database database, string owningServerFqdn, OrganizationId organizationId, string mailboxLegacyDN, Guid mailboxGuid, string domainController, Task.TaskWarningLoggingDelegate warningLogger, Task.TaskVerboseLoggingDelegate verboseLogger)
		{
			MailboxTaskHelper.PrepopulateCacheForMailbox(database.Guid, database.Name, owningServerFqdn, organizationId, mailboxLegacyDN, mailboxGuid, domainController, warningLogger, verboseLogger);
		}

		internal static void PrepopulateCacheForMailbox(Guid databaseGuid, string databaseName, string owningServerFqdn, OrganizationId organizationId, string mailboxLegacyDN, Guid mailboxGuid, string domainController, Task.TaskWarningLoggingDelegate warningLogger, Task.TaskVerboseLoggingDelegate verboseLogger)
		{
			ExRpcAdmin exRpcAdmin = null;
			try
			{
				exRpcAdmin = ExRpcAdmin.Create("Client=Management", owningServerFqdn, null, null, null);
				verboseLogger(Strings.VerboseRPCConnectionCreated(owningServerFqdn));
				exRpcAdmin.PrePopulateCache(databaseGuid, mailboxLegacyDN, mailboxGuid, TenantPartitionHint.Serialize(TenantPartitionHint.FromOrganizationId(organizationId ?? OrganizationId.ForestWideOrgId)), domainController);
				verboseLogger(Strings.VerboseSucceedToPrepopulateCache);
			}
			catch (MapiPermanentException ex)
			{
				warningLogger(Strings.ErrorFailedToPrepopulateCache(databaseName, owningServerFqdn, mailboxLegacyDN, ex.LowLevelError.ToString("X")));
				verboseLogger(Strings.VerboseFailedToPrepopulateCache(ex.Message));
			}
			catch (MapiRetryableException ex2)
			{
				warningLogger(Strings.ErrorFailedToPrepopulateCache(databaseName, owningServerFqdn, mailboxLegacyDN, ex2.LowLevelError.ToString("X")));
				verboseLogger(Strings.VerboseFailedToPrepopulateCache(ex2.Message));
			}
			finally
			{
				if (exRpcAdmin != null)
				{
					exRpcAdmin.Dispose();
				}
			}
		}

		internal static string GetMonitoringTenantName(string nameSuffix = "E15")
		{
			string text = NativeHelpers.GetForestName();
			if (text.Equals("prod.exchangelabs.com", StringComparison.OrdinalIgnoreCase))
			{
				text = "namprd01";
			}
			else
			{
				int num = text.IndexOf('.');
				if (num != -1)
				{
					text = text.Substring(0, num);
				}
			}
			return string.Format("{0}{1}.O365.ExchangeMon.net", text, nameSuffix);
		}

		internal static ADUser CreateMonitoringMailbox(Guid guid, MailboxDatabase database, ADObjectId[] forcedReplicationSites, Task.ErrorLoggerDelegate errorLogger, Task.TaskWarningLoggingDelegate warningLogger, Task.TaskVerboseLoggingDelegate verboseLogger, Action<ADUser, string> liveIdFiller = null, string displayName = null, string monitoringTenantName = null, string password = null)
		{
			string monitoringMailboxName = ADUser.GetMonitoringMailboxName(guid);
			verboseLogger(Strings.VerboseCreatingMonitoringMailbox(monitoringMailboxName));
			ADUser aduser = new ADUser();
			aduser.StampPersistableDefaultValues();
			aduser.Name = monitoringMailboxName;
			aduser.DisplayName = ((displayName == null) ? monitoringMailboxName : displayName);
			aduser.Alias = monitoringMailboxName;
			aduser.SamAccountName = "SM_" + Guid.NewGuid().ToString("N").Substring(0, 17);
			aduser.HiddenFromAddressListsEnabled = true;
			aduser.RecipientTypeDetails = RecipientTypeDetails.MonitoringMailbox;
			aduser.Database = database.Id;
			aduser.ExchangeGuid = Guid.NewGuid();
			aduser.ArchiveDatabase = aduser.Database;
			aduser.ArchiveGuid = Guid.NewGuid();
			aduser.ArchiveName = new MultiValuedProperty<string>(Strings.ArchiveNamePrefix + monitoringMailboxName);
			aduser.ArchiveStatus = ArchiveStatusFlags.Active;
			Server server = database.GetServer();
			aduser.ServerLegacyDN = server.ExchangeLegacyDN;
			if (VariantConfiguration.InvariantNoFlightingSnapshot.ActiveMonitoring.PinMonitoringMailboxesToDatabases.Enabled)
			{
				aduser.MailboxProvisioningConstraint = new MailboxProvisioningConstraint(string.Format("{{DatabaseName -eq '{0}'}}", database.Name));
			}
			aduser.UserAccountControl = (UserAccountControlFlags.PasswordNotRequired | UserAccountControlFlags.NormalAccount | UserAccountControlFlags.DoNotExpirePassword);
			ADSessionSettings sessionSettings;
			IConfigurationSession configurationSession;
			IRecipientSession recipientSession;
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
			{
				if (string.IsNullOrWhiteSpace(monitoringTenantName))
				{
					monitoringTenantName = MailboxTaskHelper.GetMonitoringTenantName("E15");
				}
				try
				{
					sessionSettings = ADSessionSettings.FromTenantCUName(monitoringTenantName);
					configurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 2805, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\recipient\\MailboxTaskHelper.cs");
					recipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(false, ConsistencyMode.IgnoreInvalid, sessionSettings, 2806, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\recipient\\MailboxTaskHelper.cs");
				}
				catch (CannotResolveTenantNameException)
				{
					warningLogger(Strings.WarningNoMonitoringTenant(monitoringTenantName));
					return null;
				}
				ExchangeConfigurationUnit[] array = configurationSession.Find<ExchangeConfigurationUnit>(null, QueryScope.SubTree, null, null, 0);
				if (array == null || array.Length == 0)
				{
					warningLogger(Strings.WarningNoMonitoringTenant(monitoringTenantName));
					return null;
				}
				ExchangeConfigurationUnit exchangeConfigurationUnit = array[0];
				if (exchangeConfigurationUnit.OrganizationStatus != OrganizationStatus.Active)
				{
					warningLogger(Strings.WarningMonitoringTenantNotActive(monitoringTenantName, exchangeConfigurationUnit.OrganizationStatus.ToString()));
					return null;
				}
				goto IL_258;
			}
			sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
			configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 2836, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\recipient\\MailboxTaskHelper.cs");
			recipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(false, ConsistencyMode.IgnoreInvalid, sessionSettings, 2837, "CreateMonitoringMailbox", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\recipient\\MailboxTaskHelper.cs");
			IL_258:
			AcceptedDomain defaultAcceptedDomain = configurationSession.GetDefaultAcceptedDomain();
			if (defaultAcceptedDomain == null || defaultAcceptedDomain.DomainName == null || defaultAcceptedDomain.DomainName.Domain == null)
			{
				throw new ManagementObjectNotFoundException(Strings.ErrorNoDefaultAcceptedDomainFound(database.Identity.ToString()));
			}
			aduser.EmailAddresses.Add(ProxyAddress.Parse("SMTP:" + aduser.Alias + "@" + defaultAcceptedDomain.DomainName.Domain.ToString()));
			aduser.EmailAddresses.Add(ProxyAddress.Parse("SIP:" + aduser.Alias + "@" + defaultAcceptedDomain.DomainName.Domain.ToString()));
			aduser.WindowsEmailAddress = aduser.PrimarySmtpAddress;
			aduser.UserPrincipalName = aduser.Alias + "@" + defaultAcceptedDomain.DomainName.Domain.ToString();
			aduser.ResetPasswordOnNextLogon = false;
			aduser.SendModerationNotifications = TransportModerationNotificationFlags.Never;
			ADObjectId orgContainerId = configurationSession.GetOrgContainerId();
			ADOrganizationConfig adorganizationConfig = configurationSession.Read<ADOrganizationConfig>(orgContainerId);
			aduser.OrganizationId = adorganizationConfig.OrganizationId;
			ADObjectId adobjectId;
			if (adorganizationConfig.OrganizationId != OrganizationId.ForestWideOrgId)
			{
				adobjectId = adorganizationConfig.OrganizationId.OrganizationalUnit;
			}
			else
			{
				bool useConfigNC = configurationSession.UseConfigNC;
				bool useGlobalCatalog = configurationSession.UseGlobalCatalog;
				ADComputer adcomputer;
				try
				{
					configurationSession.UseConfigNC = false;
					configurationSession.UseGlobalCatalog = true;
					adcomputer = ((ITopologyConfigurationSession)configurationSession).FindComputerByHostName(server.Name);
				}
				finally
				{
					configurationSession.UseConfigNC = useConfigNC;
					configurationSession.UseGlobalCatalog = useGlobalCatalog;
				}
				if (adcomputer == null)
				{
					throw new ManagementObjectNotFoundException(Strings.ErrorDBOwningServerNotFound(database.Identity.ToString()));
				}
				ADObjectId adobjectId2 = adcomputer.Id.DomainId;
				adobjectId2 = adobjectId2.GetChildId("Microsoft Exchange System Objects");
				adobjectId = adobjectId2.GetChildId("Monitoring Mailboxes");
			}
			aduser.SetId(adobjectId.GetChildId(aduser.Name));
			string parentLegacyDN = string.Format(CultureInfo.InvariantCulture, "{0}/ou={1}/cn=Recipients", new object[]
			{
				adorganizationConfig.LegacyExchangeDN,
				adobjectId.Name
			});
			aduser.LegacyExchangeDN = LegacyDN.GenerateLegacyDN(parentLegacyDN, aduser);
			RoleAssignmentPolicy roleAssignmentPolicy = RecipientTaskHelper.FindDefaultRoleAssignmentPolicy(configurationSession, errorLogger, Strings.ErrorDefaultRoleAssignmentPolicyNotUnique, Strings.ErrorDefaultRoleAssignmentPolicyNotFound);
			if (roleAssignmentPolicy != null)
			{
				aduser.RoleAssignmentPolicy = roleAssignmentPolicy.Id;
			}
			recipientSession.LinkResolutionServer = database.OriginatingServer;
			if (liveIdFiller != null)
			{
				liveIdFiller(aduser, password);
			}
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, monitoringMailboxName);
			ADUser[] array2 = recipientSession.FindADUser(adobjectId, QueryScope.SubTree, filter, null, 1);
			if (array2 != null && array2.Length > 0)
			{
				return array2[0];
			}
			recipientSession.Save(aduser);
			MailboxTaskHelper.SetMailboxPassword(recipientSession, aduser, password, errorLogger);
			if (database.Mounted != null && database.Mounted.Value)
			{
				MailboxTaskHelper.PrepopulateCacheForMailbox(database, database.GetServer().Fqdn, aduser.OrganizationId, aduser.LegacyExchangeDN, aduser.ExchangeGuid, aduser.OriginatingServer, warningLogger, verboseLogger);
			}
			if (forcedReplicationSites != null)
			{
				DagTaskHelper.ForceReplication(recipientSession, aduser, forcedReplicationSites, database.Name, warningLogger, verboseLogger);
			}
			array2 = recipientSession.FindADUser(adobjectId, QueryScope.SubTree, filter, null, 1);
			if (array2 != null && array2.Length > 0)
			{
				aduser = array2[0];
				aduser.UserAccountControl &= ~UserAccountControlFlags.PasswordNotRequired;
				recipientSession.Save(aduser);
				return aduser;
			}
			return null;
		}

		internal static void RemoveMonitoringMailboxes(MailboxDatabase database, Task.TaskWarningLoggingDelegate warningLogger, Task.TaskVerboseLoggingDelegate verboseLogger)
		{
			verboseLogger(Strings.VerboseDeleteMonitoringMailbox(database.Id.ToString()));
			IRecipientSession recipientSession;
			IRecipientSession recipientSession2;
			if (Datacenter.IsMicrosoftHostedOnly(true))
			{
				string monitoringTenantName = MailboxTaskHelper.GetMonitoringTenantName("E15");
				try
				{
					ADSessionSettings sessionSettings = ADSessionSettings.FromTenantCUName(monitoringTenantName);
					recipientSession = DirectorySessionFactory.Default.CreateTenantRecipientSession(false, ConsistencyMode.IgnoreInvalid, sessionSettings, 3032, "RemoveMonitoringMailboxes", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\recipient\\MailboxTaskHelper.cs");
					recipientSession2 = DirectorySessionFactory.Default.CreateTenantRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 3033, "RemoveMonitoringMailboxes", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\recipient\\MailboxTaskHelper.cs");
					goto IL_C1;
				}
				catch (CannotResolveTenantNameException)
				{
					warningLogger(Strings.WarningNoMonitoringTenant(monitoringTenantName));
					return;
				}
			}
			ADSessionSettings sessionSettings2 = ADSessionSettings.FromRootOrgScopeSet();
			recipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(false, ConsistencyMode.IgnoreInvalid, sessionSettings2, 3046, "RemoveMonitoringMailboxes", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\recipient\\MailboxTaskHelper.cs");
			recipientSession2 = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings2, 3047, "RemoveMonitoringMailboxes", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\recipient\\MailboxTaskHelper.cs");
			IL_C1:
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADMailboxRecipientSchema.Database, database.Id),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetails, RecipientTypeDetails.MonitoringMailbox)
			});
			ADRecipient[] array = recipientSession2.Find(null, QueryScope.SubTree, filter, null, 0);
			if (array != null && array.Length != 0)
			{
				foreach (ADRecipient instanceToDelete in array)
				{
					recipientSession.Delete(instanceToDelete);
				}
				return;
			}
		}

		private static bool DoesPublicFolderMailboxContainFoldersWithContents(OrganizationId organizationId, Guid contentMailboxGuid)
		{
			new List<StoreId>();
			bool flag = contentMailboxGuid == Guid.Empty;
			using (PublicFolderSession publicFolderSession = PublicFolderSession.OpenAsAdmin(organizationId, null, Guid.Empty, null, CultureInfo.InvariantCulture, "Client=Management;Action=RemoveOrDisablePublicFolderMailbox", null))
			{
				using (Folder folder = Folder.Bind(publicFolderSession, publicFolderSession.GetIpmSubtreeFolderId()))
				{
					using (QueryResult queryResult = folder.FolderQuery(flag ? FolderQueryFlags.None : FolderQueryFlags.DeepTraversal, null, null, new PropertyDefinition[]
					{
						FolderSchema.Id,
						FolderSchema.ReplicaList,
						FolderSchema.ReplicaListBinary
					}))
					{
						for (;;)
						{
							object[][] rows = queryResult.GetRows(flag ? 1 : 10000);
							if (rows.Length <= 0)
							{
								goto IL_F3;
							}
							if (flag)
							{
								break;
							}
							foreach (object[] array2 in rows)
							{
								string[] array3 = array2[1] as string[];
								if (array3 != null && array3.Length > 0)
								{
									PublicFolderContentMailboxInfo publicFolderContentMailboxInfo = new PublicFolderContentMailboxInfo(array3[0]);
									if (publicFolderContentMailboxInfo.IsValid && publicFolderContentMailboxInfo.MailboxGuid == contentMailboxGuid)
									{
										goto Block_13;
									}
								}
							}
						}
						return true;
						Block_13:
						return true;
						IL_F3:;
					}
				}
			}
			return false;
		}

		internal static void ValidatePublicFolderInformationWritable(IConfigurationSession tenantLocalConfigSession, bool holdForMigration, Task.ErrorLoggerDelegate writeError, bool force)
		{
			Organization orgContainer = tenantLocalConfigSession.GetOrgContainer();
			PublicFolderInformation defaultPublicFolderMailbox = orgContainer.DefaultPublicFolderMailbox;
			if (!defaultPublicFolderMailbox.CanUpdate)
			{
				writeError(new RecipientTaskException(Strings.ErrorCannotUpdatePublicFolderHierarchyInformation), ExchangeErrorCategory.Client, null);
			}
			if (holdForMigration && defaultPublicFolderMailbox.Type != PublicFolderInformation.HierarchyType.InTransitMailboxGuid && defaultPublicFolderMailbox.HierarchyMailboxGuid != Guid.Empty)
			{
				writeError(new RecipientTaskException(Strings.ErrorPublicFolderHierarchyAlreadyProvisioned), ExchangeErrorCategory.Client, null);
			}
		}

		internal static void ValidateArbitrationMailboxHasNoGroups(ADUser dataObject, IRecipientSession tenantGCSession, Task.ErrorLoggerDelegate writeError, LocalizedString validationFailedError)
		{
			QueryFilter queryFilter = (OrganizationId.ForestWideOrgId == dataObject.OrganizationId) ? new NotFilter(new ExistsFilter(ADObjectSchema.ConfigurationUnit)) : new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ConfigurationUnit, dataObject.ConfigurationUnit);
			ADRecipient[] array = tenantGCSession.Find(null, QueryScope.SubTree, new AndFilter(new QueryFilter[]
			{
				queryFilter,
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.ArbitrationMailbox, dataObject.Id),
				new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADGroupSchema.MemberJoinRestriction, MemberUpdateType.ApprovalRequired),
					new ComparisonFilter(ComparisonOperator.Equal, ADGroupSchema.MemberDepartRestriction, MemberUpdateType.ApprovalRequired),
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.ModerationEnabled, true)
				})
			}), null, 1);
			if (array != null && array.Length != 0)
			{
				writeError(new RecipientTaskException(validationFailedError), ExchangeErrorCategory.Client, dataObject.Identity);
			}
		}

		internal static void ValidateNotLastArbitrationMailbox(ADUser dataObject, IRecipientSession tenantGCSession, ADObjectId rootOrgContainerId, bool allowRemoveOrDisableLast, Task.ErrorLoggerDelegate writeError, LocalizedString validationFailedError)
		{
			bool flag = null == MailboxTaskHelper.GetArbitrationMailbox(tenantGCSession, dataObject.ConfigurationUnit ?? rootOrgContainerId, dataObject.Id);
			if (flag && !allowRemoveOrDisableLast)
			{
				writeError(new RecipientTaskException(validationFailedError), ExchangeErrorCategory.Client, dataObject.Identity);
			}
		}

		internal static void ValidateNoOABsAssignedToArbitrationMailbox(ADUser dataObject, bool overrideCheck, Task.ErrorLoggerDelegate writeError, LocalizedString validationFailedError)
		{
			if (overrideCheck)
			{
				return;
			}
			if (OABVariantConfigurationSettings.IsLinkedOABGenMailboxesEnabled && dataObject.RecipientTypeDetails == RecipientTypeDetails.ArbitrationMailbox && dataObject.PersistedCapabilities != null && dataObject.PersistedCapabilities.Contains(Capability.OrganizationCapabilityOABGen) && dataObject.GeneratedOfflineAddressBooks != null && dataObject.GeneratedOfflineAddressBooks.Count > 0)
			{
				writeError(new RecipientTaskException(validationFailedError), ExchangeErrorCategory.Client, dataObject.Identity);
			}
		}

		internal static void ValidateMailboxPlanRelease(ADUser mailboxPlan, Task.ErrorLoggerDelegate writeError)
		{
			if (MailboxPlanRelease.NonCurrentRelease == (MailboxPlanRelease)mailboxPlan[ADRecipientSchema.MailboxPlanRelease])
			{
				writeError(new RecipientTaskException(Strings.ErrorMailboxPlanInvalidInThisRelease), ExchangeErrorCategory.Client, mailboxPlan);
			}
		}

		internal static void ValidateRoomMailboxPasswordParameterCanOnlyBeUsedWithEnableRoomMailboxPassword(bool userHasSpecifiedRoomMailboxPasswordInCommandLine, bool userHasSpecifiedEnableRoomMailboxAccountInCommandLine, Task.ErrorLoggerDelegate errorLogger)
		{
			if (userHasSpecifiedRoomMailboxPasswordInCommandLine && !userHasSpecifiedEnableRoomMailboxAccountInCommandLine)
			{
				errorLogger(new TaskArgumentException(Strings.ErrorRoomMailboxPasswordCanOnlyBeUsedWithEnableRoomMailboxAccount), ExchangeErrorCategory.Client, null);
			}
		}

		internal static void EnsureUserSpecifiedDatabaseMatchesMailboxProvisioningConstraint(Database mailboxDatabase, Database archiveDatabase, PropertyBag fields, MailboxProvisioningConstraint mailboxProvisioningConstraint, Task.ErrorLoggerDelegate errorLogger, object databaseObject)
		{
			if (mailboxProvisioningConstraint != null)
			{
				if (fields.IsModified(databaseObject) && mailboxDatabase.MailboxProvisioningAttributes != null && !mailboxProvisioningConstraint.IsMatch(mailboxDatabase.MailboxProvisioningAttributes))
				{
					errorLogger(new RecipientTaskException(Strings.Error_DatabaseAttributesMismatch(mailboxDatabase.Name, mailboxProvisioningConstraint.Value)), ExchangeErrorCategory.Client, null);
				}
				if (fields.IsModified(ADUserSchema.ArchiveDatabase) && archiveDatabase.MailboxProvisioningAttributes != null && !mailboxProvisioningConstraint.IsMatch(archiveDatabase.MailboxProvisioningAttributes))
				{
					errorLogger(new RecipientTaskException(Strings.Error_DatabaseAttributesMismatch(archiveDatabase.Name, mailboxProvisioningConstraint.Value)), ExchangeErrorCategory.Client, null);
				}
			}
		}

		internal static void ValidateMailboxProvisioningConstraintEntries(IEnumerable<MailboxProvisioningConstraint> mailboxProvisioningConstraints, string domainController, LogMessageDelegate verboseLogger, Task.ErrorLoggerDelegate errorLogger)
		{
			List<MailboxDatabase> allCachedDatabasesForProvisioning = PhysicalResourceLoadBalancing.GetAllCachedDatabasesForProvisioning(domainController, verboseLogger);
			foreach (MailboxProvisioningConstraint mailboxProvisioningConstraint in mailboxProvisioningConstraints)
			{
				bool flag = false;
				foreach (MailboxDatabase mailboxDatabase in allCachedDatabasesForProvisioning)
				{
					if (mailboxDatabase.MailboxProvisioningAttributes != null && mailboxProvisioningConstraint.IsMatch(mailboxDatabase.MailboxProvisioningAttributes))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					errorLogger(new RecipientTaskException(Strings.Error_NoDatabaseAttributesMatchingMailboxProvisioningConstraint(mailboxProvisioningConstraint.Value)), ExchangeErrorCategory.Client, null);
				}
			}
		}

		internal static AuthenticationType? GetNamespaceAuthenticationType(OrganizationId organizationId, string domain)
		{
			return ProvisioningCache.Instance.TryAddAndGetOrganizationDictionaryValue<AuthenticationType?, string>(CannedProvisioningCacheKeys.NamespaceAuthenticationTypeCacheKey, organizationId, domain, delegate()
			{
				OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(organizationId);
				return organizationIdCacheValue.GetNamespaceAuthenticationType(domain);
			});
		}

		internal static ADUser FindMailboxPlanWithSKUCapability(Capability skuCapability, IRecipientSession session, out LocalizedString errorString, bool checkCurrentReleasePlanFirst)
		{
			errorString = LocalizedString.Empty;
			QueryFilter queryFilter = checkCurrentReleasePlanFirst ? MailboxTaskHelper.currentReleaseMailboxPlanFilter : MailboxTaskHelper.mailboxPlanFilter;
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				queryFilter,
				new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.PersistedCapabilities, skuCapability)
			});
			bool includeSoftDeletedObjects = session.SessionSettings.IncludeSoftDeletedObjects;
			ADUser[] array = null;
			try
			{
				session.SessionSettings.IncludeSoftDeletedObjects = false;
				array = session.FindADUser(null, QueryScope.OneLevel, filter, null, 2);
				if (checkCurrentReleasePlanFirst && array.Length != 1)
				{
					filter = new AndFilter(new QueryFilter[]
					{
						MailboxTaskHelper.mailboxPlanFilter,
						new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.PersistedCapabilities, skuCapability)
					});
					array = session.FindADUser(null, QueryScope.OneLevel, filter, null, 2);
				}
			}
			finally
			{
				session.SessionSettings.IncludeSoftDeletedObjects = includeSoftDeletedObjects;
			}
			if (array.Length == 1)
			{
				return array[0];
			}
			if (array.Length < 1)
			{
				errorString = Strings.ErrorNoMailboxPlanWithSKUCapability(skuCapability.ToString());
			}
			else
			{
				errorString = Strings.ErrorMoreThanOneMailboxPlanWithSKUCapability(skuCapability.ToString());
			}
			return null;
		}

		internal static void VerifyDatabaseIsWithinScopeForRecipientCmdlets(ADSessionSettings sessionSettings, Database database, Task.ErrorLoggerDelegate errorHandler)
		{
			if (!VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
			{
				MapiTaskHelper.VerifyDatabaseIsWithinScope(sessionSettings, database, errorHandler);
			}
		}

		internal static void UpdateAuditSettings(ADUser user)
		{
			string[] array = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Exchange_Test", "AuditConfig", null) as string[];
			if (array != null && array.Length > 0 && bool.TrueString.Equals(array[0], StringComparison.OrdinalIgnoreCase))
			{
				user.MailboxAuditEnabled = true;
				bool flag = Array.Exists<string>(array, (string str) => string.Compare(str, "admin", true) == 0);
				bool flag2 = Array.Exists<string>(array, (string str) => string.Compare(str, "delegate", true) == 0);
				bool flag3 = Array.Exists<string>(array, (string str) => string.Compare(str, "owner", true) == 0);
				if (!flag)
				{
					user.AuditAdminOperations = MailboxAuditOperations.None;
					user.AuditDelegateAdminOperations = MailboxAuditOperations.None;
				}
				if (!flag2)
				{
					user.AuditDelegateOperations = MailboxAuditOperations.None;
				}
				if (!flag3)
				{
					user.AuditOwnerOperations = MailboxAuditOperations.None;
				}
			}
		}

		internal static void WriteWarningWhenMailboxIsUnlicensed(ADUser user, Task.TaskWarningLoggingDelegate writeWarning)
		{
			if (user.RecipientTypeDetails == RecipientTypeDetails.UserMailbox && (user.SKUAssigned == null || !user.SKUAssigned.Value) && CapabilityHelper.GetIsLicensingEnforcedInOrg(user.OrganizationId))
			{
				writeWarning(Strings.WarningUnlicensedMailbox);
			}
		}

		internal static bool IsArchiveRecoverable(ADUser user, IConfigurationSession configurationSession, IRecipientSession globalCatalogSession)
		{
			if (configurationSession == null)
			{
				throw new ArgumentNullException("configurationSession");
			}
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			ADObjectId deletedObjectsContainer = configurationSession.DeletedObjectsContainer;
			if (user.DisabledArchiveDatabase == null)
			{
				return false;
			}
			if (user.DisabledArchiveDatabase.Parent != null && !user.DisabledArchiveDatabase.Parent.Equals(deletedObjectsContainer))
			{
				using (MapiAdministrationSession adminSession = MapiTaskHelper.GetAdminSession(RecipientTaskHelper.GetActiveManagerInstance(), user.DisabledArchiveDatabase.ObjectGuid))
				{
					string mailboxLegacyDN = MapiTaskHelper.GetMailboxLegacyDN(adminSession, user.DisabledArchiveDatabase, user.DisabledArchiveGuid);
					if (mailboxLegacyDN != null)
					{
						ADRecipient adrecipient = ConnectMailbox.FindMailboxByLegacyDN(mailboxLegacyDN, globalCatalogSession);
						if (adrecipient == null || adrecipient.LegacyExchangeDN == user.LegacyExchangeDN)
						{
							return true;
						}
						TaskLogger.Trace("The previous archive '{1}' of user '{0}' is in use by the following user in Active Directory: '{2}'. The recovery of archive failed.", new object[]
						{
							user.DisplayName,
							user.DisabledArchiveGuid,
							adrecipient.DisplayName
						});
					}
					else
					{
						TaskLogger.Trace("The previous archive '{1}' of user '{0}' cannot be found in store. The recovery of archive failed. This can occur if the archive was disabled and the mailbox retention period has passed. It can also occur if the archive was enabled but later disabled without the user ever logging on to it.", new object[]
						{
							user.DisplayName,
							user.DisabledArchiveGuid
						});
					}
				}
				return false;
			}
			TaskLogger.Trace("The previous archive database '{1}' of user '{0}' is no longer available. The recovery of archive failed.", new object[]
			{
				user.DisplayName,
				user.DisabledArchiveDatabase
			});
			return false;
		}

		internal static void BlockLowerMajorVersionArchive(int archiveServerVersion, string primaryDatabaseDN, string archiveDatabaseDN, string archiveDatabaseName, ADObjectId primaryDatabaseId, DataAccessHelper.CategorizedGetDataObjectDelegate getDataObjectDelegate, ITopologyConfigurationSession globalConfigSession, ActiveManager activeManager, Task.ErrorLoggerDelegate errorLogger)
		{
			if (archiveDatabaseDN != primaryDatabaseDN)
			{
				Database database = (MailboxDatabase)getDataObjectDelegate(new DatabaseIdParameter(primaryDatabaseId), globalConfigSession, null, null, new LocalizedString?(Strings.ErrorDatabaseNotFound(primaryDatabaseId.ToString())), new LocalizedString?(Strings.ErrorDatabaseNotUnique(primaryDatabaseId.ToString())), ExchangeErrorCategory.Client);
				DatabaseLocationInfo databaseLocationInfo = MailboxTaskHelper.GetDatabaseLocationInfo(database, activeManager, errorLogger);
				if (databaseLocationInfo == null)
				{
					errorLogger(new RecipientTaskException(Strings.ErrorPrimaryDatabaseLocationNotFound(database.ToString())), ExchangeErrorCategory.Client, null);
					return;
				}
				if (archiveServerVersion < databaseLocationInfo.ServerVersion)
				{
					ServerVersion serverVersion = new ServerVersion(databaseLocationInfo.ServerVersion);
					ServerVersion serverVersion2 = new ServerVersion(archiveServerVersion);
					if (serverVersion2.Major < serverVersion.Major)
					{
						errorLogger(new RecipientTaskException(Strings.ErrorArchiveCanNotBeDownVersion(archiveDatabaseName, database.ToString())), ExchangeErrorCategory.Client, null);
					}
				}
			}
		}

		internal static DatabaseLocationInfo GetDatabaseLocationInfo(Database database, ActiveManager activeManager, Task.ErrorLoggerDelegate errorLogger)
		{
			try
			{
				return activeManager.GetServerForDatabase(database.Guid);
			}
			catch (ObjectNotFoundException exception)
			{
				errorLogger(exception, ExchangeErrorCategory.Client, null);
			}
			catch (ServerForDatabaseNotFoundException exception2)
			{
				errorLogger(exception2, ExchangeErrorCategory.Client, null);
			}
			return null;
		}

		internal static bool SupportsMailboxReleaseVersioning(ADUser adUser)
		{
			return adUser.IsFromDatacenter;
		}

		internal static MailboxRelease ComputeRequiredMailboxRelease(IConfigurationSession configSession, ADUser adUser, ExchangeConfigurationUnit configurationUnit, Task.ErrorLoggerDelegate errorLogger)
		{
			if (configurationUnit == null)
			{
				configurationUnit = configSession.Read<ExchangeConfigurationUnit>(adUser.OrganizationId.ConfigurationUnit);
				if (configurationUnit == null)
				{
					errorLogger(new ManagementObjectNotFoundException(Strings.ErrorOrganizationNotFound(adUser.OrganizationId.ToString())), ExchangeErrorCategory.Client, null);
				}
			}
			if (adUser.UpgradeRequest != UpgradeRequestTypes.PilotUpgrade)
			{
				return configurationUnit.MailboxRelease;
			}
			return configurationUnit.PilotMailboxRelease;
		}

		internal static void ValidateMailboxRelease(MailboxRelease targetServerMailboxRelease, MailboxRelease requiredMailboxRelease, string userIdentity, string databaseIdentity, Task.ErrorLoggerDelegate errorLogger)
		{
			if (requiredMailboxRelease != targetServerMailboxRelease)
			{
				errorLogger(new MismatchedMailboxReleaseException(userIdentity, databaseIdentity, targetServerMailboxRelease.ToString(), requiredMailboxRelease.ToString()), ExchangeErrorCategory.Client, null);
			}
		}

		internal static void ApplyMbxPlanSettingsInTargetForest(ADUser user, Func<ADObjectId, ADUser> getMbxPlanObject, ApplyMailboxPlanFlags flags)
		{
			ADObjectId adobjectId = null;
			if (user.IntendedMailboxPlan != null)
			{
				adobjectId = user.IntendedMailboxPlan;
			}
			else if (user.MailboxPlan != null)
			{
				adobjectId = user.MailboxPlan;
			}
			if (adobjectId != null)
			{
				ADUser newPlan = getMbxPlanObject(adobjectId);
				MailboxTaskHelper.ApplyMailboxPlansDelta(null, newPlan, user, flags);
				user.MailboxPlan = adobjectId;
				user.IntendedMailboxPlan = null;
			}
		}

		internal static void ApplyMbxPlanDeltaOnMbxMove(ADUser user, Func<ADObjectId, ADUser> getMbxPlanObject, IConfigurationSession configurationSession, IRecipientSession recipientSession, Task.ErrorLoggerDelegate writeError)
		{
			if (user.MailboxPlan != null && user.SKUCapability != null)
			{
				bool checkCurrentReleasePlanFirst = RecipientTaskHelper.IsOrganizationInPilot(configurationSession, user.OrganizationId);
				LocalizedString message;
				ADUser aduser = MailboxTaskHelper.FindMailboxPlanWithSKUCapability(user.SKUCapability.Value, recipientSession, out message, checkCurrentReleasePlanFirst);
				if (aduser == null)
				{
					writeError(new RecipientTaskException(message), ExchangeErrorCategory.ServerOperation, user.Id);
				}
				MailboxTaskHelper.UpdateMailboxPlan(user, aduser, getMbxPlanObject);
			}
		}

		internal static void UpdateMailboxPlan(ADUser userObject, ADUser newMbxPlan, Func<ADObjectId, ADUser> getMbxPlanObject)
		{
			ADObjectId mailboxPlan = userObject.MailboxPlan;
			if (!newMbxPlan.Id.Equals(mailboxPlan))
			{
				ADUser oldPlan = null;
				if (mailboxPlan != null)
				{
					oldPlan = getMbxPlanObject(mailboxPlan);
				}
				userObject.MailboxPlan = newMbxPlan.Id;
				MailboxTaskHelper.ApplyMailboxPlansDelta(oldPlan, newMbxPlan, userObject, ApplyMailboxPlanFlags.None);
				RecipientTaskHelper.UpgradeArchiveQuotaOnArchiveAddOnSKU(userObject, userObject.PersistedCapabilities);
			}
		}

		internal static MailboxIdParameter ResolveMailboxIdentity(ADObjectId executingUserId, Task.ErrorLoggerDelegate errorLogger)
		{
			if (executingUserId != null)
			{
				return new MailboxIdParameter(executingUserId);
			}
			errorLogger(new RecipientTaskException(Strings.ErrorParameterRequired("Mailbox")), ExchangeErrorCategory.Client, null);
			return null;
		}

		internal static void RemovePersistentProperties(List<PropertyDefinition> propertiesToClear)
		{
			propertiesToClear.Remove(ADRecipientSchema.EmailAddresses);
			propertiesToClear.Remove(ADRecipientSchema.WindowsEmailAddress);
			propertiesToClear.RemoveAll(delegate(PropertyDefinition definition)
			{
				string ldapDisplayName = ((ADPropertyDefinition)definition).LdapDisplayName;
				return !string.IsNullOrEmpty(ldapDisplayName) && ldapDisplayName.StartsWith("extensionAttribute", StringComparison.InvariantCultureIgnoreCase);
			});
		}

		internal static void ClearExchangeProperties(ADRecipient recipient, IEnumerable<PropertyDefinition> propertiesToReset)
		{
			if (recipient.MaximumSupportedExchangeObjectVersion.IsOlderThan(recipient.ExchangeVersion))
			{
				throw new DataValidationException(new PropertyValidationError(Strings.ErrorCannotSaveBecauseTooNew(recipient.ExchangeVersion.ToString(), recipient.MaximumSupportedExchangeObjectVersion.ToString()), ADObjectSchema.ExchangeVersion, recipient.ExchangeVersion));
			}
			foreach (PropertyDefinition propertyDefinition in propertiesToReset)
			{
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)propertyDefinition;
				if (!recipient.ExchangeVersion.IsOlderThan(adpropertyDefinition.VersionAdded))
				{
					recipient[adpropertyDefinition] = null;
				}
			}
			if (recipient.RecipientTypeDetails == RecipientTypeDetails.PublicFolderMailbox && !recipient.ExchangeVersion.IsOlderThan(ADRecipientSchema.RecipientTypeDetails.VersionAdded))
			{
				recipient[ADRecipientSchema.RecipientTypeDetails] = null;
			}
		}

		public static void ValidateExternalEmailAddress(ADRecipient recipient, IConfigurationSession configurationSession, Task.ErrorLoggerDelegate writeError, ProvisioningCache provisioningCache)
		{
			if (VariantConfiguration.InvariantNoFlightingSnapshot.CmdletInfra.ValidateExternalEmailAddressInAcceptedDomain.Enabled)
			{
				SmtpProxyAddress smtpProxyAddress = recipient.ExternalEmailAddress as SmtpProxyAddress;
				if (smtpProxyAddress == null)
				{
					writeError(new RecipientTaskException(Strings.ErrorExternalEmailAddressNotSmtpAddress((recipient.ExternalEmailAddress == null) ? "$null" : recipient.ExternalEmailAddress.ToString())), ExchangeErrorCategory.Client, recipient.Identity);
					return;
				}
				if (RecipientTaskHelper.SMTPAddressCheckWithAcceptedDomain(configurationSession, recipient.OrganizationId, writeError, provisioningCache))
				{
					RecipientTaskHelper.ValidateInAcceptedDomain(configurationSession, recipient.OrganizationId, new SmtpAddress(smtpProxyAddress.SmtpAddress).Domain, writeError, provisioningCache);
				}
				recipient.EmailAddressPolicyEnabled = false;
				if (recipient.PrimarySmtpAddress == SmtpAddress.Empty)
				{
					recipient.PrimarySmtpAddress = new SmtpAddress(smtpProxyAddress.SmtpAddress);
				}
			}
		}

		internal static void CheckAndResolveManagedBy<TObject>(NewGeneralRecipientObjectTask<TObject> task, DataAccessHelper.CategorizedGetDataObjectDelegate getDataObject, ExchangeErrorCategory errorCategory, RecipientIdParameter[] managedByParameter, out MultiValuedProperty<ADRecipient> managedByRecipients) where TObject : ADRecipient, new()
		{
			managedByRecipients = null;
			if (task.Fields.IsModified(ADGroupSchema.ManagedBy))
			{
				if (managedByParameter == null || managedByParameter.Length == 0)
				{
					task.WriteError(new RecipientTaskException(Strings.AutoGroupManagedByCannotBeEmpty), ErrorCategory.InvalidArgument, null);
				}
				managedByRecipients = new MultiValuedProperty<ADRecipient>();
				foreach (RecipientIdParameter recipientIdParameter in managedByParameter)
				{
					ADRecipient item = (ADRecipient)getDataObject(recipientIdParameter, task.TenantGlobalCatalogSession, null, null, new LocalizedString?(Strings.ErrorRecipientNotFound(recipientIdParameter.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(recipientIdParameter.ToString())), errorCategory);
					managedByRecipients.Add(item);
				}
			}
		}

		internal static void StampOnManagedBy(ADGroup group, MultiValuedProperty<ADRecipient> managedByRecipients, Task.ErrorLoggerDelegate writeError)
		{
			if (managedByRecipients == null || managedByRecipients.Count == 0)
			{
				group.ManagedBy = null;
				return;
			}
			MultiValuedProperty<ADObjectId> multiValuedProperty = new MultiValuedProperty<ADObjectId>();
			foreach (ADRecipient adrecipient in managedByRecipients)
			{
				if (multiValuedProperty.Contains(adrecipient.Id))
				{
					writeError(new TaskInvalidOperationException(Strings.ErrorManagedByAlreadyExisted(group.Identity.ToString(), adrecipient.Id.ToString())), ExchangeErrorCategory.Client, group.Identity);
				}
				else
				{
					multiValuedProperty.Add(adrecipient.Id);
				}
			}
			group.ManagedBy = multiValuedProperty;
		}

		internal static void ValidateAndAddMember(IConfigDataProvider session, ADGroup group, RecipientIdParameter member, bool isSelfValidation, Task.ErrorLoggerDelegate writeError, DataAccessHelper.CategorizedGetDataObjectDelegate getDataObject)
		{
			ADRecipient adrecipient = (ADRecipient)getDataObject(member, session, group.OrganizationId.OrganizationalUnit, null, new LocalizedString?(Strings.ErrorRecipientNotFound((string)member)), new LocalizedString?(Strings.ErrorRecipientNotUnique((string)member)), ExchangeErrorCategory.Client);
			if (MailboxTaskHelper.GroupContainsMember(group, adrecipient.Id, session))
			{
				MailboxTaskHelper.WriteMemberAlreadyExistsError(group, member, isSelfValidation, writeError);
			}
			MailboxTaskHelper.ValidateAndAddMember(group, member, adrecipient, writeError);
		}

		internal static void ValidateAndAddMember(ADGroup group, RecipientIdParameter memberId, ADRecipient memberRecipient, Task.ErrorLoggerDelegate writeError)
		{
			MailboxTaskHelper.ValidateGroupMember(group, memberRecipient, memberId, writeError);
			group.Members.Add(memberRecipient.Id);
		}

		internal static void ValidateGroupMember(ADGroup group, ADRecipient memberRecipient, Task.ErrorLoggerDelegate writeError)
		{
			MailboxTaskHelper.ValidateGroupMember(group, memberRecipient, null, writeError);
		}

		internal static void ValidateGroupMember(ADGroup group, ADRecipient memberRecipient, RecipientIdParameter memberId, Task.ErrorLoggerDelegate writeError)
		{
			if (group.RecipientTypeDetails == RecipientTypeDetails.RoomList && memberRecipient.RecipientTypeDetails != RecipientTypeDetails.RoomMailbox && memberRecipient.RecipientTypeDetails != RecipientTypeDetails.RoomList && memberRecipient.RecipientDisplayType != RecipientDisplayType.SyncedConferenceRoomMailbox)
			{
				writeError(new NonRoomMailboxAddToRoomListException(group.Id.ToString()), ExchangeErrorCategory.Client, memberId);
			}
			MailboxTaskHelper.ValidateMemberInGroup(memberRecipient, group, writeError);
			MailboxTaskHelper.CheckGroupVersion(group);
		}

		internal static void ValidateAddedMembers(IConfigDataProvider session, ADGroup dataObject, Task.ErrorLoggerDelegate writeError, DataAccessHelper.CategorizedGetDataObjectDelegate getDataObject)
		{
			if ((dataObject.GroupType & GroupTypeFlags.Universal) != GroupTypeFlags.Universal)
			{
				writeError(new RecipientTaskException(Strings.ErrorOnlyAllowChangeMembersOnUniversalGroup(dataObject.Name)), ExchangeErrorCategory.Client, dataObject.Identity);
			}
			if (dataObject.Members != null && dataObject.Members.Added.Length > 0)
			{
				foreach (ADObjectId adobjectId in dataObject.Members.Added)
				{
					ADRecipient adrecipient = (ADRecipient)getDataObject(new GeneralRecipientIdParameter(adobjectId), session, null, null, new LocalizedString?(Strings.ErrorRecipientNotFound(adobjectId.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(adobjectId.ToString())), ExchangeErrorCategory.Client);
					MailboxTaskHelper.ValidateMemberInGroup(adrecipient, dataObject, writeError);
					if (adrecipient.Id.Parent.DistinguishedName.StartsWith("CN=ForeignSecurityPrincipals,DC=", StringComparison.InvariantCultureIgnoreCase))
					{
						writeError(new RecipientTaskException(Strings.ErrorUniversalGroupCannotHaveForeignSP(dataObject.Name, adobjectId.ToString())), ExchangeErrorCategory.Client, dataObject.Identity);
					}
				}
			}
		}

		internal static bool ValidateMemberInGroup(ADRecipient memberRecipient, ADGroup containerGroup, Task.ErrorLoggerDelegate writeError)
		{
			if (containerGroup.Guid == memberRecipient.Guid)
			{
				writeError(new RecipientTaskException(Strings.ErrorGroupMembersCannotContainItself(memberRecipient.Id.Name)), ExchangeErrorCategory.Client, memberRecipient.Identity);
				return false;
			}
			OrganizationId organizationId = containerGroup.OrganizationId;
			OrganizationId organizationId2 = memberRecipient.OrganizationId;
			if (!organizationId.Equals(organizationId2))
			{
				writeError(new RecipientTaskException(Strings.ErrorAddGroupMemberCrossTenant), ExchangeErrorCategory.Client, memberRecipient.Identity);
				return false;
			}
			ADGroup adgroup = memberRecipient as ADGroup;
			if (adgroup != null && (containerGroup.GroupType & GroupTypeFlags.Universal) == GroupTypeFlags.Universal && (adgroup.GroupType & GroupTypeFlags.DomainLocal) == GroupTypeFlags.DomainLocal)
			{
				writeError(new RecipientTaskException(Strings.ErrorUniversalGroupCannotHaveLocalGroup(containerGroup.Id.Name, adgroup.Id.Name)), ExchangeErrorCategory.Client, containerGroup.Identity);
				return false;
			}
			return true;
		}

		internal static void CheckGroupVersion(ADGroup group)
		{
			if (group.MaximumSupportedExchangeObjectVersion.IsOlderThan(group.ExchangeVersion))
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.ErrorCannotSaveBecauseTooNew(group.ExchangeVersion, group.MaximumSupportedExchangeObjectVersion), ADObjectSchema.ExchangeVersion, group.ExchangeVersion));
			}
		}

		internal static void WriteMemberAlreadyExistsError(ADGroup group, RecipientIdParameter member, bool isSelfValidation, Task.ErrorLoggerDelegate writeError)
		{
			if (isSelfValidation)
			{
				writeError(new SelfMemberAlreadyExistsException(group.Id.ToString()), ExchangeErrorCategory.Client, member);
				return;
			}
			writeError(new MemberAlreadyExistsException(member.ToString(), group.Id.ToString()), ExchangeErrorCategory.Client, member);
		}

		internal static void ValidateGroupManagedBy(IRecipientSession recipientSession, ADGroup group, MultiValuedProperty<ADRecipient> recipients, DataAccessHelper.CategorizedGetDataObjectDelegate getDataObject, Task.ErrorLoggerDelegate writeError)
		{
			MailboxTaskHelper.ValidateGroupManagedBy(recipientSession, group, recipients, RecipientConstants.DistributionGroup_OwnerRecipientTypeDetails, false, getDataObject, writeError);
		}

		internal static void ValidateGroupManagedBy(IRecipientSession recipientSession, ADGroup group, MultiValuedProperty<ADRecipient> recipients, RecipientTypeDetails[] allowedRecipientTypeDetails, bool useSecurityPrincipalIdParameter, DataAccessHelper.CategorizedGetDataObjectDelegate getDataObject, Task.ErrorLoggerDelegate writeError)
		{
			recipients = MailboxTaskHelper.GetGroupManagedbyRecipients(recipientSession, group, useSecurityPrincipalIdParameter, recipients, getDataObject);
			foreach (ADRecipient adrecipient in recipients)
			{
				if (!group.OrganizationId.Equals(adrecipient.OrganizationId))
				{
					writeError(new RecipientTaskException(Strings.ErrorManagedByCrossTenant(adrecipient.Id.ToString())), ExchangeErrorCategory.Client, group.Identity);
				}
				MailboxTaskHelper.ValidateGroupManagedBy(group, adrecipient, allowedRecipientTypeDetails, writeError);
			}
		}

		internal static void ValidateGroupManagedBy(ADGroup group, ADRecipient recipient, Task.ErrorLoggerDelegate writeError)
		{
			MailboxTaskHelper.ValidateGroupManagedBy(group, recipient, RecipientConstants.DistributionGroup_OwnerRecipientTypeDetails, writeError);
		}

		private static void ValidateGroupManagedBy(ADGroup group, ADRecipient recipient, RecipientTypeDetails[] allowedRecipientTypeDetails, Task.ErrorLoggerDelegate writeError)
		{
			if (!group.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010))
			{
				if (!Array.Exists<RecipientTypeDetails>(allowedRecipientTypeDetails, (RecipientTypeDetails item) => item == recipient.RecipientTypeDetails))
				{
					writeError(new RecipientTaskException(Strings.ErrorManagedByWrongRecipientTypeDetails(group.Id.ToString(), recipient.Id.ToString(), string.Join(",", (from detail in allowedRecipientTypeDetails
					select detail.ToString()).ToArray<string>()))), ExchangeErrorCategory.Client, group.Identity);
				}
			}
		}

		internal static void ValidateGroupManagedByRecipientRestriction(IRecipientSession recipientSession, ADGroup group, MultiValuedProperty<ADRecipient> recipients, DataAccessHelper.CategorizedGetDataObjectDelegate getDataObject, Task.ErrorLoggerDelegate writeError, Task.TaskWarningLoggingDelegate writeWarning)
		{
			recipients = MailboxTaskHelper.GetGroupManagedbyRecipients(recipientSession, group, false, recipients, getDataObject);
			bool flag = false;
			bool flag2 = true;
			foreach (ADRecipient adrecipient in recipients)
			{
				if (adrecipient.RecipientTypeDetails != RecipientTypeDetails.MailUniversalSecurityGroup && adrecipient.RecipientTypeDetails != RecipientTypeDetails.UniversalSecurityGroup)
				{
					flag2 = false;
				}
				else if (adrecipient.RecipientTypeDetails == RecipientTypeDetails.MailUniversalSecurityGroup || adrecipient.RecipientTypeDetails == RecipientTypeDetails.UniversalSecurityGroup)
				{
					flag = true;
				}
			}
			if (recipients.Count > 0 && writeError != null && writeWarning != null)
			{
				if (group.MemberJoinRestriction == MemberUpdateType.ApprovalRequired || group.MemberDepartRestriction == MemberUpdateType.ApprovalRequired)
				{
					if (flag2)
					{
						writeError(new RecipientTaskException(Strings.ErrorRestrictionWithWrongGroupType), ExchangeErrorCategory.Client, group.Identity);
					}
					else if (flag)
					{
						writeWarning(Strings.WarningRestrictionWithWrongGroupType);
					}
				}
				if (group.ModerationEnabled && group.ModeratedBy.Count == 0)
				{
					if (flag2)
					{
						writeError(new RecipientTaskException(Strings.ErrorModerationWithWrongGroupType), ExchangeErrorCategory.Client, group.Identity);
						return;
					}
					if (flag)
					{
						writeWarning(Strings.WarningModerationWithWrongGroupType);
					}
				}
			}
		}

		private static MultiValuedProperty<ADRecipient> GetGroupManagedbyRecipients(IRecipientSession recipientSession, ADGroup group, bool useSecurityPrincipalIdParameter, MultiValuedProperty<ADRecipient> recipients, DataAccessHelper.CategorizedGetDataObjectDelegate getDataObject)
		{
			if (recipients == null)
			{
				recipients = new MultiValuedProperty<ADRecipient>();
				foreach (ADObjectId adobjectId in group.ManagedBy)
				{
					ADRecipient item = null;
					try
					{
						item = (ADRecipient)getDataObject(useSecurityPrincipalIdParameter ? new SecurityPrincipalIdParameter(adobjectId) : new RecipientIdParameter(adobjectId), recipientSession, null, null, new LocalizedString?(Strings.ErrorRecipientNotFound(adobjectId.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(adobjectId.ToString())), ExchangeErrorCategory.Client);
					}
					catch (ManagementObjectNotFoundException)
					{
						RoleGroupIdParameter roleGroupIdParameter = new RoleGroupIdParameter(adobjectId);
						if (roleGroupIdParameter.InternalADObjectId == null || !(roleGroupIdParameter.InternalADObjectId.Name == "Organization Management"))
						{
							throw;
						}
						item = (ADRecipient)getDataObject(useSecurityPrincipalIdParameter ? new SecurityPrincipalIdParameter(adobjectId) : roleGroupIdParameter, recipientSession, null, null, new LocalizedString?(Strings.ErrorRecipientNotFound(adobjectId.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(adobjectId.ToString())), ExchangeErrorCategory.Client);
					}
					recipients.Add(item);
				}
			}
			return recipients;
		}

		internal static void ValidateAndRemoveMember(IConfigDataProvider session, ADGroup group, RecipientIdParameter member, string groupRawIdentity, bool isSelfValidation, Task.TaskErrorLoggingDelegate writeError, DataAccessHelper.GetDataObjectDelegate getDataObject)
		{
			ADRecipient adrecipient = (ADRecipient)getDataObject(member, session, group.OrganizationId.OrganizationalUnit, null, new LocalizedString?(Strings.ErrorRecipientNotFound((string)member)), new LocalizedString?(Strings.ErrorRecipientNotUnique((string)member)));
			if (!MailboxTaskHelper.GroupContainsMember(group, adrecipient.Id, session))
			{
				MailboxTaskHelper.WriteMemberNotFoundError(group, member, groupRawIdentity, isSelfValidation, writeError);
			}
			MailboxTaskHelper.ValidateAndRemoveMember(group, adrecipient);
		}

		internal static void ValidateAndRemoveMember(ADGroup group, ADRecipient memberRecipient)
		{
			MailboxTaskHelper.CheckGroupVersion(group);
			MailboxTaskHelper.RemoveItem(group.Members, memberRecipient.Id);
		}

		internal static bool GroupContainsMember(ADGroup group, ADObjectId memberid, IConfigDataProvider session)
		{
			bool result = false;
			if (group.Members.IsCompletelyRead)
			{
				result = group.Members.Contains(memberid);
			}
			else
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADGroupSchema.Members, memberid);
				IDirectorySession directorySession = (IDirectorySession)session;
				bool skipRangedAttributes = directorySession.SkipRangedAttributes;
				ADRawEntry[] array;
				try
				{
					directorySession.SkipRangedAttributes = true;
					array = directorySession.Find(group.Id, QueryScope.Base, filter, null, 1, new ADPropertyDefinition[]
					{
						ADObjectSchema.Id
					});
				}
				finally
				{
					directorySession.SkipRangedAttributes = skipRangedAttributes;
				}
				if (array != null && array.Length > 0)
				{
					result = true;
				}
			}
			return result;
		}

		internal static void RemoveItem(MultiValuedProperty<ADObjectId> collection, ADObjectId itemToRemove)
		{
			if (!collection.Contains(itemToRemove))
			{
				object[] added = collection.Added;
				object[] removed = collection.Removed;
				foreach (object item in added)
				{
					collection.Remove(item);
				}
				foreach (object item2 in removed)
				{
					collection.Add(item2);
				}
				collection.Add(itemToRemove);
				collection.ResetChangeTracking();
				foreach (object item3 in added)
				{
					collection.Add(item3);
				}
				foreach (object item4 in removed)
				{
					collection.Remove(item4);
				}
			}
			collection.Remove(itemToRemove);
		}

		internal static void WriteMemberNotFoundError(ADGroup group, RecipientIdParameter member, string groupRawIdentity, bool isSelfValidation, Task.TaskErrorLoggingDelegate writeError)
		{
			if (isSelfValidation)
			{
				writeError(new SelfMemberNotFoundException(string.IsNullOrEmpty(groupRawIdentity) ? group.Id.ToString() : groupRawIdentity), ErrorCategory.InvalidData, member);
				return;
			}
			writeError(new MemberNotFoundException(member.ToString(), string.IsNullOrEmpty(groupRawIdentity) ? group.Id.ToString() : groupRawIdentity), ErrorCategory.InvalidData, member);
		}

		internal static MultiValuedProperty<string> ValidateAndSanitizeTranslations(IList<string> mailTipsTranslations, HashSet<string> mailTipTranslationCultures, bool isDefaultMailTipChanged, bool isDeletingDefaultMailTip, Task.ErrorLoggerDelegate writeError)
		{
			List<string> list = new List<string>(mailTipsTranslations);
			for (int i = 0; i < mailTipsTranslations.Count; i++)
			{
				string text;
				string unsafeHtml;
				if (!ADRecipient.TryGetMailTipParts(mailTipsTranslations[i], out text, out unsafeHtml))
				{
					writeError(new RecipientTaskException(DirectoryStrings.ErrorMailTipTranslationFormatIncorrect), ExchangeErrorCategory.Client, null);
				}
				bool flag = "default".Equals(text, StringComparison.OrdinalIgnoreCase);
				if (flag && isDefaultMailTipChanged)
				{
					writeError(new RecipientTaskException(Strings.ErrorMoreThanOneDefaultMailTipTranslationSpecified), ExchangeErrorCategory.Client, null);
				}
				else if (mailTipTranslationCultures.Contains(text))
				{
					if (flag)
					{
						writeError(new RecipientTaskException(Strings.ErrorMoreThanOneDefaultMailTipTranslationSpecified), ExchangeErrorCategory.Client, null);
					}
					else
					{
						writeError(new RecipientTaskException(Strings.ErrorMoreThanOneMailTipTranslationForThisCulture(text)), ExchangeErrorCategory.Client, null);
					}
				}
				if (isDeletingDefaultMailTip && !flag)
				{
					writeError(new RecipientTaskException(Strings.ErrorMailTipSetTranslationsWithoutDefault), ExchangeErrorCategory.Client, null);
				}
				mailTipTranslationCultures.Add(text);
				string str = TextConverterHelper.SanitizeHtml(unsafeHtml);
				list[i] = text + ":" + str;
			}
			if (!isDefaultMailTipChanged && !mailTipTranslationCultures.Contains("default"))
			{
				writeError(new RecipientTaskException(Strings.ErrorMailTipSetTranslationsWithoutDefault), ExchangeErrorCategory.Client, null);
			}
			return new MultiValuedProperty<string>(list);
		}

		internal static MultiValuedProperty<string> ValidateAndSanitizeTranslations(IList<string> translations, Task.ErrorLoggerDelegate writeError)
		{
			HashSet<string> mailTipTranslationCultures = new HashSet<string>(translations.Count<string>(), StringComparer.OrdinalIgnoreCase);
			return MailboxTaskHelper.ValidateAndSanitizeTranslations(translations, mailTipTranslationCultures, false, false, writeError);
		}

		public static void ProcessRecord(Action action, MailboxTaskHelper.ThrowTerminatingErrorDelegate handleError, object identity)
		{
			try
			{
				action();
			}
			catch (InboxRuleOperationException ex)
			{
				handleError(new InvalidOperationException(ex.Message), ErrorCategory.InvalidOperation, identity);
			}
			catch (RulesTooBigException)
			{
				handleError(new InvalidOperationException(Strings.ErrorInboxRuleTooBig), ErrorCategory.InvalidOperation, identity);
			}
		}

		internal static ADMicrosoftExchangeRecipient FindMicrosoftExchangeRecipient(IRecipientSession recipientSession, IConfigurationSession configurationSession)
		{
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			if (configurationSession == null)
			{
				throw new ArgumentNullException("configurationSession");
			}
			bool useConfigNC = recipientSession.UseConfigNC;
			bool useGlobalCatalog = recipientSession.UseGlobalCatalog;
			recipientSession.UseConfigNC = true;
			recipientSession.UseGlobalCatalog = false;
			ADMicrosoftExchangeRecipient result = (ADMicrosoftExchangeRecipient)recipientSession.Read(ADMicrosoftExchangeRecipient.GetDefaultId(configurationSession));
			recipientSession.UseConfigNC = useConfigNC;
			recipientSession.UseGlobalCatalog = useGlobalCatalog;
			return result;
		}

		internal const string LiveIdForBrandInfoPrefix = "c4e67852e761400490f0750a898dc64e";

		internal const string ExchangeRemovedMailboxId = "ExRemoved-";

		public const string Password = "Password";

		public const string RoomMailboxPassword = "RoomMailboxPassword";

		public const string ParameterSkipMailboxProvisioningConstraintValidation = "SkipMailboxProvisioningConstraintValidation";

		private const string ForeignSecurityPrincipalsDNPrefix = "CN=ForeignSecurityPrincipals,DC=";

		internal const int MaxNameLength = 64;

		internal static readonly QueryFilter mailboxPlanFilter = new AndFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.MailboxPlan),
			new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.MailboxPlanRelease, MailboxPlanRelease.CurrentRelease),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.MailboxPlanRelease, MailboxPlanRelease.AllReleases)
			})
		});

		internal static readonly QueryFilter currentReleaseMailboxPlanFilter = new AndFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.MailboxPlan),
			new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.MailboxPlanRelease, MailboxPlanRelease.CurrentRelease)
		});

		internal static readonly QueryFilter defaultMailboxPlanFilter = QueryFilter.AndTogether(new QueryFilter[]
		{
			MailboxTaskHelper.mailboxPlanFilter,
			new ComparisonFilter(ComparisonOperator.Equal, MailboxPlanSchema.IsDefault, true)
		});

		internal delegate IConfigurable GetUniqueObject(IIdentityParameter id, IConfigDataProvider session, ObjectId rootID, LocalizedString? notFoundError, LocalizedString? multipleFoundError, ExchangeErrorCategory errorCategory);

		internal delegate LocalizedString TwoStringErrorDelegate(string str1, string str2);

		internal delegate LocalizedString OneStringErrorDelegate(string str1);

		public delegate void ThrowTerminatingErrorDelegate(Exception exception, ErrorCategory category, object target);
	}
}
