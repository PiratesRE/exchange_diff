using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	internal static class SecurityHelper
	{
		internal static bool CheckTransportPrivilege(ClientSecurityContext callerSecurityContext, SecurityDescriptor securityDescriptor)
		{
			Trace securityServiceAccessTracer = ExTraceGlobals.SecurityServiceAccessTracer;
			bool flag = SecurityHelper.CheckTransportPrivilege(callerSecurityContext, securityDescriptor);
			if (!flag && securityServiceAccessTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				securityServiceAccessTracer.TraceError(0L, "Transport privilege check has failed; enable tracing of security descriptors and security contexts for more info");
				SecurityHelper.TraceSecurityContext(callerSecurityContext, ExTraceGlobals.SecurityContextTracer);
				SecurityHelper.TraceSecurityDescriptor(securityDescriptor, ExTraceGlobals.SecurityDescriptorTracer);
			}
			return flag;
		}

		internal static bool CheckConstrainedDelegationPrivilege(ClientSecurityContext callerSecurityContext, SecurityDescriptor securityDescriptor)
		{
			Trace securityServiceAccessTracer = ExTraceGlobals.SecurityServiceAccessTracer;
			bool flag = SecurityHelper.CheckConstrainedDelegationPrivilege(callerSecurityContext, securityDescriptor);
			if (!flag && securityServiceAccessTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				securityServiceAccessTracer.TraceError(0L, "Constrained delegation privilege check has failed; enable tracing of security descriptors and security contexts for more info");
				SecurityHelper.TraceSecurityContext(callerSecurityContext, ExTraceGlobals.SecurityContextTracer);
				SecurityHelper.TraceSecurityDescriptor(securityDescriptor, ExTraceGlobals.SecurityDescriptorTracer);
			}
			return flag;
		}

		internal static bool CheckAdministrativeRights(ClientSecurityContext callerSecurityContext, SecurityDescriptor securityDescriptor)
		{
			Trace securityAdminAccessTracer = ExTraceGlobals.SecurityAdminAccessTracer;
			bool flag = SecurityHelper.CheckAdministrativeRights(callerSecurityContext, securityDescriptor);
			if (!flag && securityAdminAccessTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				securityAdminAccessTracer.TraceError(0L, "Administrative privilege check has failed; enable tracing of security descriptors and security contexts for more info");
				SecurityHelper.TraceSecurityContext(callerSecurityContext, ExTraceGlobals.SecurityContextTracer);
				SecurityHelper.TraceSecurityDescriptor(securityDescriptor, ExTraceGlobals.SecurityDescriptorTracer);
			}
			return flag;
		}

		internal static bool CheckMailboxOwnerRights(ClientSecurityContext callerSecurityContext, AddressInfo addressDirectoryInfo)
		{
			Trace securityMailboxOwnerAccessTracer = ExTraceGlobals.SecurityMailboxOwnerAccessTracer;
			SecurityIdentifier securityIdentifier;
			bool flag2;
			bool flag = SecurityHelper.CheckIfContextMatchObjectSids(callerSecurityContext, addressDirectoryInfo.UserSid, addressDirectoryInfo.MasterAccountSid, addressDirectoryInfo.UserSidHistory, addressDirectoryInfo.IsDistributionList, out securityIdentifier, out flag2, securityMailboxOwnerAccessTracer);
			if (!flag)
			{
				DiagnosticContext.TraceLocation((LID)47920U);
			}
			if (!flag && securityMailboxOwnerAccessTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				SecurityHelper.TraceAccessCheckInfo("Owner check has failed for the AddressInfo passed in; for more info enable StoreCommonServices.SecurityContext and StoreCommonServices.SecurityDescriptor", callerSecurityContext, addressDirectoryInfo, securityMailboxOwnerAccessTracer);
			}
			return flag;
		}

		internal static bool CheckMailboxOwnerRights(ClientSecurityContext callerSecurityContext, MailboxInfo mailboxDirectoryInfo, DatabaseInfo databaseDirectoryInfo)
		{
			Trace securityMailboxOwnerAccessTracer = ExTraceGlobals.SecurityMailboxOwnerAccessTracer;
			SecurityDescriptor securityDescriptor = null;
			SecurityIdentifier securityIdentifier;
			bool flag2;
			bool flag = SecurityHelper.CheckIfContextMatchObjectSids(callerSecurityContext, mailboxDirectoryInfo.UserSid, mailboxDirectoryInfo.MasterAccountSid, mailboxDirectoryInfo.UserSidHistory, false, out securityIdentifier, out flag2, securityMailboxOwnerAccessTracer);
			if (!flag)
			{
				DiagnosticContext.TraceLocation((LID)59184U);
				if (securityIdentifier == null)
				{
					DiagnosticContext.TraceLocation((LID)43824U);
				}
				else
				{
					securityDescriptor = new SecurityDescriptor(Mailbox.CreateMailboxSecurityDescriptorBlob(databaseDirectoryInfo.NTSecurityDescriptor, mailboxDirectoryInfo.ExchangeSecurityDescriptor));
					if (securityDescriptor == null)
					{
						if (securityMailboxOwnerAccessTracer.IsTraceEnabled(TraceType.ErrorTrace))
						{
							securityMailboxOwnerAccessTracer.TraceError(0L, "Security descriptor for the mailbox is NULL");
						}
						DiagnosticContext.TraceLocation((LID)39935U);
					}
					else
					{
						flag = SecurityHelper.CheckExtendedRightsOnObject(callerSecurityContext, securityDescriptor, MailboxSecurity.GetObjectRightsGuid(MailboxSecurity.ObjectRights.User), AccessMask.CreateChild, securityIdentifier, flag2 ? mailboxDirectoryInfo.UserSidHistory : Array<SecurityIdentifier>.Empty);
						if (!flag)
						{
							DiagnosticContext.TraceLocation((LID)56319U);
						}
					}
				}
			}
			if (!flag && securityMailboxOwnerAccessTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				SecurityHelper.TraceAccessCheckInfo("Mailbox owner check has failed; for more info enable StoreCommonServices.SecurityContext and StoreCommonServices.SecurityDescriptor", callerSecurityContext, mailboxDirectoryInfo, securityDescriptor, securityMailboxOwnerAccessTracer);
			}
			return flag;
		}

		internal static bool CheckSendAsRights(ClientSecurityContext callerSecurityContext, AddressInfo addressInfo)
		{
			Trace securitySendAsAccessTracer = ExTraceGlobals.SecuritySendAsAccessTracer;
			SecurityIdentifier principalSelfSid;
			bool flag2;
			bool flag = SecurityHelper.CheckIfContextMatchObjectSids(callerSecurityContext, addressInfo.UserSid, addressInfo.MasterAccountSid, addressInfo.UserSidHistory, addressInfo.IsDistributionList, out principalSelfSid, out flag2, securitySendAsAccessTracer);
			if (!flag)
			{
				DiagnosticContext.TraceLocation((LID)34608U);
				flag = SecurityHelper.CheckExtendedRightsOnObject(callerSecurityContext, addressInfo.OSSecurityDescriptor, MailboxSecurity.GetObjectRightsGuid(MailboxSecurity.ObjectRights.SendAs), AccessMask.ControlAccess, principalSelfSid, flag2 ? addressInfo.UserSidHistory : Array<SecurityIdentifier>.Empty);
				if (!flag)
				{
					DiagnosticContext.TraceLocation((LID)56112U);
				}
			}
			if (!flag && securitySendAsAccessTracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				SecurityHelper.TraceAccessCheckInfo("SendAs check has failed; for more info enable StoreCommonServices.SecurityContext and StoreCommonServices.SecurityDescriptor", callerSecurityContext, addressInfo, securitySendAsAccessTracer);
			}
			return flag;
		}

		public static uint MapGenericMask(MailboxSecurity.MailboxAccessMask accessMask, NativeMethods.GENERIC_MAPPING mapping)
		{
			uint result = (uint)accessMask;
			NativeMethods.MapGenericMask(ref result, ref mapping);
			return result;
		}

		public static SecurityIdentifier ComputeObjectSID(SecurityIdentifier objectSid, SecurityIdentifier masterAccountSid)
		{
			bool flag;
			return SecurityHelper.ComputeObjectSID(objectSid, masterAccountSid, out flag);
		}

		private static SecurityIdentifier ComputeObjectSID(SecurityIdentifier objectSid, SecurityIdentifier masterAccountSid, out bool sidHistoryIsApplicable)
		{
			sidHistoryIsApplicable = false;
			if (objectSid == null && masterAccountSid == null)
			{
				return null;
			}
			SecurityIdentifier securityIdentifier;
			if (masterAccountSid != null)
			{
				securityIdentifier = masterAccountSid;
			}
			else
			{
				securityIdentifier = objectSid;
				sidHistoryIsApplicable = true;
			}
			if (securityIdentifier.IsWellKnown(WellKnownSidType.SelfSid))
			{
				securityIdentifier = objectSid;
				sidHistoryIsApplicable = true;
			}
			return securityIdentifier;
		}

		private static bool CheckIfContextMatchObjectSids(ClientSecurityContext callerSecurityContext, SecurityIdentifier objectSid, SecurityIdentifier objectMasterAccountSid, SecurityIdentifier[] objectSidHistory, bool objectIsDistributionList, out SecurityIdentifier computedObjectSid, out bool sidHistoryIsApplicable, Trace trace)
		{
			computedObjectSid = SecurityHelper.ComputeObjectSID(objectSid, objectMasterAccountSid, out sidHistoryIsApplicable);
			if (computedObjectSid == null)
			{
				DiagnosticContext.TraceLocation((LID)34375U);
				if (trace.IsTraceEnabled(TraceType.ErrorTrace))
				{
					trace.TraceError(0L, "Both objectSid and objectMasterAccountSid are null");
				}
				return false;
			}
			if (objectIsDistributionList)
			{
				DiagnosticContext.TraceLocation((LID)40348U);
				return false;
			}
			if (callerSecurityContext.UserSid == computedObjectSid)
			{
				return true;
			}
			DiagnosticContext.TraceLocation((LID)41232U);
			if (sidHistoryIsApplicable)
			{
				foreach (SecurityIdentifier right in objectSidHistory)
				{
					if (callerSecurityContext.UserSid == right)
					{
						return true;
					}
				}
			}
			DiagnosticContext.TraceLocation((LID)60208U);
			IdentityReferenceCollection groups;
			try
			{
				groups = callerSecurityContext.GetGroups();
			}
			catch (AuthzException ex)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
				throw new StoreException((LID)39728U, ErrorCodeValue.CallFailed, "Failed to get token groups", ex);
			}
			if (groups != null)
			{
				foreach (IdentityReference identityReference in groups)
				{
					SecurityIdentifier left = identityReference as SecurityIdentifier;
					if (left != null && left == computedObjectSid)
					{
						return true;
					}
				}
			}
			DiagnosticContext.TraceLocation((LID)37136U);
			return false;
		}

		private static bool CheckExtendedRightsOnObject(ClientSecurityContext callerSecurityContext, SecurityDescriptor securityDescriptor, Guid extendedRightGuid, AccessMask accessMask, SecurityIdentifier principalSelfSid, SecurityIdentifier[] principalSelfSidHistory)
		{
			bool flag = callerSecurityContext.HasExtendedRightOnObject(securityDescriptor, extendedRightGuid, accessMask, principalSelfSid);
			if (flag)
			{
				return flag;
			}
			DiagnosticContext.TraceLocation((LID)55056U);
			foreach (SecurityIdentifier principalSelfSid2 in principalSelfSidHistory)
			{
				flag = callerSecurityContext.HasExtendedRightOnObject(securityDescriptor, extendedRightGuid, accessMask, principalSelfSid2);
				if (flag)
				{
					return flag;
				}
			}
			DiagnosticContext.TraceLocation((LID)42768U);
			return false;
		}

		public static void TraceAccessCheckInfo(string description, ClientSecurityContext callerSecurityContext, AddressInfo addressInfo, Trace tracer)
		{
			SecurityHelper.TraceAccessCheckInfo(description, addressInfo.LegacyExchangeDN, callerSecurityContext, addressInfo.UserSid, addressInfo.MasterAccountSid, addressInfo.UserSidHistory, addressInfo.OSSecurityDescriptor, tracer);
		}

		public static void TraceAccessCheckInfo(string description, ClientSecurityContext callerSecurityContext, MailboxInfo mailboxInfo, SecurityDescriptor mailboxComputedSecurityDescriptor, Trace tracer)
		{
			SecurityHelper.TraceAccessCheckInfo(description, mailboxInfo.OwnerLegacyDN, callerSecurityContext, mailboxInfo.UserSid, mailboxInfo.MasterAccountSid, mailboxInfo.UserSidHistory, mailboxInfo.ExchangeSecurityDescriptor, tracer);
			if (mailboxComputedSecurityDescriptor != null)
			{
				SecurityHelper.TraceSecurityDescriptor(mailboxComputedSecurityDescriptor, tracer);
			}
		}

		private static void TraceAccessCheckInfo(string description, string legacyDN, ClientSecurityContext callerSecurityContext, SecurityIdentifier objectSid, SecurityIdentifier objectMasterAccountSid, SecurityIdentifier[] objectSidHistory, SecurityDescriptor objectSecurityDescriptor, Trace tracer)
		{
			tracer.TraceError(0L, description);
			tracer.TraceError(0L, "LegacyDN: " + legacyDN);
			tracer.TraceError(0L, "Sid: " + objectSid);
			tracer.TraceError(0L, "MasterAccountSID: " + objectMasterAccountSid);
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("SidHistory: ");
			foreach (SecurityIdentifier securityIdentifier in objectSidHistory)
			{
				stringBuilder.Append(securityIdentifier.ToString());
				stringBuilder.Append("; ");
			}
			tracer.TraceError(0L, stringBuilder.ToString());
			SecurityHelper.TraceSecurityContext(callerSecurityContext, ExTraceGlobals.SecurityContextTracer);
			SecurityHelper.TraceSecurityDescriptor(objectSecurityDescriptor, tracer);
		}
	}
}
