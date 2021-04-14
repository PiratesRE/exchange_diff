using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.Common
{
	public static class MailboxMoveHelper
	{
		public static void UpdateRecipientTypeProperties(ADUser source, UserHoster sourceHoster, int sourceServerVersion, ADUser target, UserHoster targetHoster, int targetServerVersion, MailboxMoveType moveType, MailboxMoveTransition transition)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (transition == MailboxMoveTransition.IntraOrg)
			{
				if (MailboxMoveHelper.IsMovingPrimary(moveType))
				{
					MailboxMoveHelper.UpdateRecipientTypePropertiesForIntraOrg(target, targetServerVersion);
				}
				return;
			}
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!source.propertyBag.Contains(ADUserSchema.UserAccountControl))
			{
				throw new ArgumentException("Source ADUser missed property: UserAccountControl", "source");
			}
			if (!target.propertyBag.Contains(ADUserSchema.UserAccountControl))
			{
				throw new ArgumentException("Target ADUser missed property: UserAccountControl", "target");
			}
			if (transition == MailboxMoveTransition.UpdateSourceUser && sourceServerVersion == 0)
			{
				throw new ArgumentException("Version of source mailbox server is required", "sourceServerVersion");
			}
			if (transition == MailboxMoveTransition.UpdateTargetUser && targetServerVersion == 0)
			{
				throw new ArgumentException("Version of target mailbox server is required", "targetServerVersion");
			}
			MailboxMoveHelper.Scenario scenario = MailboxMoveHelper.GetScenario(sourceHoster, targetHoster);
			if (scenario == MailboxMoveHelper.Scenario.CrossPremiseOnboarding && sourceServerVersion < Server.E14SP1MinVersion)
			{
				sourceServerVersion = Server.E14SP1MinVersion;
			}
			MailboxMoveHelper.UpdateRecipientTypeProperties(source, sourceServerVersion, target, targetServerVersion, scenario, moveType, transition);
		}

		private static void UpdateRecipientTypePropertiesForIntraOrg(ADUser target, int targetServerVersion)
		{
			if (MailboxMoveHelper.IsRTDRDTDefined(targetServerVersion))
			{
				RecipientTypeDetails recipientTypeDetails = target.RecipientTypeDetails;
				RecipientTypeDetails recipientTypeDetails2 = (recipientTypeDetails == RecipientTypeDetails.None || recipientTypeDetails == RecipientTypeDetails.LegacyMailbox) ? MailboxMoveHelper.CalculateLegacyMailboxRTD(target) : recipientTypeDetails;
				target.RecipientTypeDetails = recipientTypeDetails2;
				if (target.RecipientDisplayType == null || recipientTypeDetails != recipientTypeDetails2)
				{
					target.RecipientDisplayType = new RecipientDisplayType?(MailboxMoveHelper.CalculateTargetRDT(target.RecipientTypeDetails, MailboxMoveHelper.IsSecurityPrincipal(target)));
					return;
				}
			}
			else
			{
				target.RecipientTypeDetails = RecipientTypeDetails.None;
				target.RecipientDisplayType = null;
			}
		}

		private static void UpdateRecipientTypeProperties(ADUser source, int sourceServerVersion, ADUser target, int targetServerVersion, MailboxMoveHelper.Scenario scenario, MailboxMoveType moveType, MailboxMoveTransition transition)
		{
			if (!MailboxMoveHelper.IsMovingPrimary(moveType))
			{
				return;
			}
			if (scenario == MailboxMoveHelper.Scenario.CrossPremiseOnboarding && transition == MailboxMoveTransition.UpdateSourceUser && MailboxMoveHelper.IsRemoteMailboxTypeDefined(sourceServerVersion))
			{
				source.RemoteRecipientType = MailboxMoveHelper.CalculateSourceRRT(source.RecipientTypeDetails);
			}
			else if (scenario == MailboxMoveHelper.Scenario.CrossPremiseOffboarding && transition == MailboxMoveTransition.UpdateTargetUser && MailboxMoveHelper.IsRemoteMailboxTypeDefined(targetServerVersion))
			{
				target.RemoteRecipientType = RemoteRecipientType.Migrated;
			}
			if (transition == MailboxMoveTransition.UpdateTargetUser)
			{
				RecipientTypeDetails recipientTypeDetails = (source.RecipientTypeDetails != RecipientTypeDetails.None) ? source.RecipientTypeDetails : MailboxMoveHelper.CalculateLegacyMailboxRTD(source);
				if (target.MasterAccountSid == null && MailboxMoveHelper.IsResourceOrShared(recipientTypeDetails))
				{
					target.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
				}
				if (MailboxMoveHelper.IsRTDRDTDefined(targetServerVersion))
				{
					target.RecipientTypeDetails = MailboxMoveHelper.CalculateTargetRTD(recipientTypeDetails, MailboxMoveHelper.IsLinkedAccount(target));
					target.RecipientDisplayType = new RecipientDisplayType?(MailboxMoveHelper.CalculateTargetRDT(target.RecipientTypeDetails, MailboxMoveHelper.IsSecurityPrincipal(target)));
				}
				else
				{
					target.RecipientTypeDetails = RecipientTypeDetails.None;
					target.RecipientDisplayType = null;
				}
			}
			if (transition == MailboxMoveTransition.UpdateSourceUser)
			{
				if (MailboxMoveHelper.IsRTDRDTDefined(sourceServerVersion))
				{
					source.RecipientDisplayType = new RecipientDisplayType?(MailboxMoveHelper.CalculateSourceRDT(source.RecipientTypeDetails, MailboxMoveHelper.IsACLableSyncedObjectEnabled(source, scenario) && MailboxMoveHelper.IsSecurityPrincipal(source)));
					source.RecipientTypeDetails = MailboxMoveHelper.CalculateSourceRTD(source, sourceServerVersion, scenario);
					return;
				}
				source.RecipientTypeDetails = RecipientTypeDetails.None;
				source.RecipientDisplayType = null;
			}
		}

		private static RemoteRecipientType CalculateSourceRRT(RecipientTypeDetails recipientTypeDetails)
		{
			RemoteRecipientType remoteRecipientType = RemoteRecipientType.Migrated;
			if (recipientTypeDetails <= RecipientTypeDetails.RoomMailbox)
			{
				if (recipientTypeDetails != RecipientTypeDetails.SharedMailbox)
				{
					if (recipientTypeDetails == RecipientTypeDetails.RoomMailbox)
					{
						remoteRecipientType |= RemoteRecipientType.RoomMailbox;
					}
				}
				else
				{
					remoteRecipientType |= RemoteRecipientType.SharedMailbox;
				}
			}
			else if (recipientTypeDetails != RecipientTypeDetails.EquipmentMailbox)
			{
				if (recipientTypeDetails == RecipientTypeDetails.TeamMailbox)
				{
					remoteRecipientType |= RemoteRecipientType.TeamMailbox;
				}
			}
			else
			{
				remoteRecipientType |= RemoteRecipientType.EquipmentMailbox;
			}
			return remoteRecipientType;
		}

		private static RecipientTypeDetails CalculateTargetRTD(RecipientTypeDetails sourceRTD, bool isLinkedAccount)
		{
			RecipientTypeDetails recipientTypeDetails;
			if (sourceRTD <= RecipientTypeDetails.RoomMailbox)
			{
				if (sourceRTD <= RecipientTypeDetails.SharedMailbox)
				{
					if (sourceRTD < RecipientTypeDetails.UserMailbox)
					{
						goto IL_52;
					}
					switch ((int)(sourceRTD - RecipientTypeDetails.UserMailbox))
					{
					case 0:
					case 3:
						goto IL_49;
					case 1:
						recipientTypeDetails = RecipientTypeDetails.UserMailbox;
						goto IL_55;
					case 2:
						goto IL_52;
					}
				}
				if (sourceRTD != RecipientTypeDetails.RoomMailbox)
				{
					goto IL_52;
				}
			}
			else if (sourceRTD != RecipientTypeDetails.EquipmentMailbox && sourceRTD != RecipientTypeDetails.TeamMailbox)
			{
				goto IL_52;
			}
			IL_49:
			recipientTypeDetails = sourceRTD;
			goto IL_55;
			IL_52:
			recipientTypeDetails = RecipientTypeDetails.UserMailbox;
			IL_55:
			if (recipientTypeDetails == RecipientTypeDetails.UserMailbox && isLinkedAccount)
			{
				recipientTypeDetails = RecipientTypeDetails.LinkedMailbox;
			}
			return recipientTypeDetails;
		}

		private static RecipientTypeDetails CalculateSourceRTD(ADUser source, int sourceServerVersion, MailboxMoveHelper.Scenario scenario)
		{
			RecipientTypeDetails result;
			if (scenario == MailboxMoveHelper.Scenario.CrossPremiseOnboarding && MailboxMoveHelper.IsRemoteMailboxTypeDefined(sourceServerVersion))
			{
				RecipientTypeDetails recipientTypeDetails = source.RecipientTypeDetails;
				if (recipientTypeDetails <= RecipientTypeDetails.RoomMailbox)
				{
					if (recipientTypeDetails <= RecipientTypeDetails.SharedMailbox)
					{
						if (recipientTypeDetails < RecipientTypeDetails.UserMailbox)
						{
							goto IL_91;
						}
						switch ((int)(recipientTypeDetails - RecipientTypeDetails.UserMailbox))
						{
						case 0:
						case 1:
						case 2:
							goto IL_91;
						case 3:
							return RecipientTypeDetails.RemoteSharedMailbox;
						}
					}
					if (recipientTypeDetails == RecipientTypeDetails.RoomMailbox)
					{
						return RecipientTypeDetails.RemoteRoomMailbox;
					}
				}
				else
				{
					if (recipientTypeDetails == RecipientTypeDetails.EquipmentMailbox)
					{
						return RecipientTypeDetails.RemoteEquipmentMailbox;
					}
					if (recipientTypeDetails == RecipientTypeDetails.TeamMailbox)
					{
						return RecipientTypeDetails.RemoteTeamMailbox;
					}
				}
				IL_91:
				result = (RecipientTypeDetails)((ulong)int.MinValue);
			}
			else
			{
				result = RecipientTypeDetails.MailUser;
			}
			return result;
		}

		private static RecipientDisplayType CalculateTargetRDT(RecipientTypeDetails sourceRTD, bool isSecurityPrincipal)
		{
			if (sourceRTD <= RecipientTypeDetails.RoomMailbox)
			{
				if (sourceRTD <= RecipientTypeDetails.SharedMailbox)
				{
					if (sourceRTD < RecipientTypeDetails.UserMailbox)
					{
						goto IL_4C;
					}
					switch ((int)(sourceRTD - RecipientTypeDetails.UserMailbox))
					{
					case 0:
					case 1:
					case 2:
					case 3:
						goto IL_4C;
					}
				}
				if (sourceRTD == RecipientTypeDetails.RoomMailbox)
				{
					return RecipientDisplayType.ConferenceRoomMailbox;
				}
			}
			else
			{
				if (sourceRTD == RecipientTypeDetails.EquipmentMailbox)
				{
					return RecipientDisplayType.EquipmentMailbox;
				}
				if (sourceRTD != RecipientTypeDetails.TeamMailbox)
				{
				}
			}
			IL_4C:
			if (!isSecurityPrincipal)
			{
				return RecipientDisplayType.MailboxUser;
			}
			return RecipientDisplayType.ACLableMailboxUser;
		}

		private static RecipientDisplayType CalculateSourceRDT(RecipientTypeDetails originalRTD, bool isSecurityPrincipal)
		{
			if (originalRTD <= RecipientTypeDetails.RoomMailbox)
			{
				if (originalRTD <= RecipientTypeDetails.SharedMailbox)
				{
					if (originalRTD < RecipientTypeDetails.UserMailbox)
					{
						goto IL_54;
					}
					switch ((int)(originalRTD - RecipientTypeDetails.UserMailbox))
					{
					case 0:
					case 1:
					case 2:
					case 3:
						goto IL_54;
					}
				}
				if (originalRTD == RecipientTypeDetails.RoomMailbox)
				{
					return RecipientDisplayType.SyncedConferenceRoomMailbox;
				}
			}
			else
			{
				if (originalRTD == RecipientTypeDetails.EquipmentMailbox)
				{
					return RecipientDisplayType.SyncedEquipmentMailbox;
				}
				if (originalRTD != RecipientTypeDetails.TeamMailbox)
				{
				}
			}
			IL_54:
			if (!isSecurityPrincipal)
			{
				return RecipientDisplayType.SyncedMailboxUser;
			}
			return RecipientDisplayType.ACLableSyncedMailboxUser;
		}

		private static RecipientTypeDetails CalculateLegacyMailboxRTD(ADUser user)
		{
			if (MailboxMoveHelper.IsLinkedAccount(user))
			{
				return RecipientTypeDetails.LinkedMailbox;
			}
			if (MailboxMoveHelper.IsSelfSid(user.MasterAccountSid))
			{
				return RecipientTypeDetails.SharedMailbox;
			}
			return RecipientTypeDetails.UserMailbox;
		}

		private static MailboxMoveHelper.Scenario GetScenario(UserHoster sourceHoster, UserHoster targetHoster)
		{
			if (sourceHoster == UserHoster.None)
			{
				throw new ArgumentException("The hoster of source ADUser should not be None.", "sourceHoster");
			}
			if (targetHoster == UserHoster.None)
			{
				throw new ArgumentException("The hoster of target ADUser should not be None.", "targetHoster");
			}
			if (sourceHoster == UserHoster.OnPremise)
			{
				if (targetHoster != UserHoster.Datacenter)
				{
					return MailboxMoveHelper.Scenario.CrossForest;
				}
				return MailboxMoveHelper.Scenario.CrossPremiseOnboarding;
			}
			else
			{
				if (targetHoster != UserHoster.Datacenter)
				{
					return MailboxMoveHelper.Scenario.CrossPremiseOffboarding;
				}
				return MailboxMoveHelper.Scenario.CrossDatacenter;
			}
		}

		private static bool IsSecurityPrincipal(ADUser user)
		{
			return !MailboxMoveHelper.IsSelfSid(user.MasterAccountSid) && ((user.UserAccountControl & UserAccountControlFlags.AccountDisabled) == UserAccountControlFlags.None || MailboxMoveHelper.IsLinkedAccount(user));
		}

		private static bool IsSelfSid(SecurityIdentifier accountSid)
		{
			return accountSid != null && accountSid.IsWellKnown(WellKnownSidType.SelfSid);
		}

		private static bool IsLinkedAccount(ADUser user)
		{
			return user.MasterAccountSid != null && user.MasterAccountSid.IsAccountSid();
		}

		private static bool IsResourceOrShared(RecipientTypeDetails rtd)
		{
			return rtd == RecipientTypeDetails.RoomMailbox || rtd == RecipientTypeDetails.EquipmentMailbox || rtd == RecipientTypeDetails.SharedMailbox || rtd == RecipientTypeDetails.TeamMailbox || rtd == RecipientTypeDetails.RemoteRoomMailbox || rtd == RecipientTypeDetails.RemoteEquipmentMailbox || rtd == RecipientTypeDetails.RemoteTeamMailbox || rtd == RecipientTypeDetails.RemoteSharedMailbox;
		}

		private static bool IsRTDRDTDefined(int serverVersion)
		{
			return serverVersion >= Server.E2007MinVersion;
		}

		private static bool IsMovingPrimary(MailboxMoveType moveType)
		{
			return (moveType & MailboxMoveType.IsPrimaryMoving) == MailboxMoveType.IsPrimaryMoving;
		}

		private static bool IsRemoteMailboxTypeDefined(int serverVersion)
		{
			return serverVersion >= Server.E14SP1MinVersion;
		}

		private static bool IsCrossPremiseScenario(MailboxMoveHelper.Scenario scenario)
		{
			return scenario == MailboxMoveHelper.Scenario.CrossPremiseOnboarding || scenario == MailboxMoveHelper.Scenario.CrossPremiseOffboarding;
		}

		private static bool IsACLableSyncedObjectEnabled(ADUser source, MailboxMoveHelper.Scenario scenario)
		{
			bool result = false;
			Trace exceptionTracer = ExTraceGlobals.ExceptionTracer;
			if (!MailboxMoveHelper.IsCrossPremiseScenario(scenario))
			{
				result = true;
			}
			else if (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled)
			{
				try
				{
					IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(source.OrganizationId), 605, "IsACLableSyncedObjectEnabled", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\recipient\\MailboxMoveHelper.cs");
					ADObjectId orgContainerId = tenantOrTopologyConfigurationSession.GetOrgContainerId();
					ADOrganizationConfig adorganizationConfig = tenantOrTopologyConfigurationSession.Read<ADOrganizationConfig>(orgContainerId);
					if (adorganizationConfig != null)
					{
						result = adorganizationConfig.ACLableSyncedObjectEnabled;
					}
				}
				catch (LocalizedException ex)
				{
					exceptionTracer.TraceWarning(100L, ex.Message);
				}
				catch (Exception ex2)
				{
					exceptionTracer.TraceWarning(200L, ex2.Message);
				}
			}
			return result;
		}

		internal static readonly ADPropertyDefinition[] PropertiesForCalculateRecipientTypeInMailboxMove = new ADPropertyDefinition[]
		{
			ADRecipientSchema.RecipientDisplayType,
			ADRecipientSchema.RecipientTypeDetails,
			ADUserSchema.RemoteRecipientType,
			ADRecipientSchema.MasterAccountSid,
			ADUserSchema.UserAccountControl,
			ADObjectSchema.ExchangeVersion
		};

		private enum Scenario
		{
			CrossPremiseOnboarding,
			CrossPremiseOffboarding,
			CrossForest,
			CrossDatacenter
		}
	}
}
