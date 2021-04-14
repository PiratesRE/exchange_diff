using System;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.StoreCommonServices;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Server.Storage.Common
{
	internal static class SecurityHelper
	{
		internal static bool CheckTransportPrivilege(ClientSecurityContext callerSecurityContext, SecurityDescriptor securityDescriptor)
		{
			return callerSecurityContext.UserSid.IsWellKnown(WellKnownSidType.LocalSystemSid) || callerSecurityContext.HasExtendedRightOnObject(securityDescriptor, MailboxSecurity.GetObjectRightsGuid(MailboxSecurity.ObjectRights.TransportAccess));
		}

		internal static bool CheckConstrainedDelegationPrivilege(ClientSecurityContext callerSecurityContext, SecurityDescriptor securityDescriptor)
		{
			return callerSecurityContext.UserSid.IsWellKnown(WellKnownSidType.LocalSystemSid) || callerSecurityContext.HasExtendedRightOnObject(securityDescriptor, MailboxSecurity.GetObjectRightsGuid(MailboxSecurity.ObjectRights.ConstrainedDelegation));
		}

		internal static bool CheckAdministrativeRights(ClientSecurityContext callerSecurityContext, SecurityDescriptor securityDescriptor)
		{
			return callerSecurityContext.UserSid.IsWellKnown(WellKnownSidType.LocalSystemSid) || callerSecurityContext.HasExtendedRightOnObject(securityDescriptor, MailboxSecurity.GetObjectRightsGuid(MailboxSecurity.ObjectRights.AdminFullAccess));
		}

		internal static bool CheckAdminFullAccessRights(ClientSecurityContext callerSecurityContext, SecurityDescriptor securityDescriptor)
		{
			return callerSecurityContext.HasExtendedRightOnObject(securityDescriptor, MailboxSecurity.GetObjectRightsGuid(MailboxSecurity.ObjectRights.AdminFullAccess));
		}

		public static void TraceSecurityDescriptor(SecurityDescriptor securityDescriptor, Trace tracer)
		{
			bool flag = tracer != null && tracer.IsTraceEnabled(TraceType.DebugTrace);
			if (flag)
			{
				StringBuilder stringBuilder = new StringBuilder(1024);
				try
				{
					SecurityHelper.AppendToString(stringBuilder, SecurityHelper.CreateRawSecurityDescriptor(securityDescriptor));
				}
				catch (StoreException exception)
				{
					NullExecutionDiagnostics.Instance.OnExceptionCatch(exception);
					stringBuilder.Append("<CORRUPTED>");
				}
				tracer.TraceDebug(0L, stringBuilder.ToString());
			}
		}

		public static void TraceSecurityContext(ClientSecurityContext callerSecurityContext, Trace tracer)
		{
			bool flag = tracer != null && tracer.IsTraceEnabled(TraceType.DebugTrace);
			if (flag)
			{
				SerializedAccessToken serializedAccessToken = new SerializedAccessToken(string.Empty, string.Empty, callerSecurityContext);
				tracer.TraceDebug(0L, serializedAccessToken.ToString());
			}
		}

		private static void AppendToString(StringBuilder sb, RawSecurityDescriptor securityDescriptor)
		{
			if (securityDescriptor != null)
			{
				sb.Append("Security Descriptor\n");
				sb.Append("\t[Owner:");
				SecurityHelper.AppendToString(sb, securityDescriptor.Owner);
				sb.Append("]\n");
				sb.Append("\t[Primary Group:");
				SecurityHelper.AppendToString(sb, securityDescriptor.Group);
				sb.Append("]\n");
				sb.Append("\t[DiscretionaryACL");
				sb.Append("\n");
				if (securityDescriptor.DiscretionaryAcl != null)
				{
					foreach (GenericAce ace in securityDescriptor.DiscretionaryAcl)
					{
						sb.Append("\t\t[");
						SecurityHelper.AppendToString(sb, ace);
						sb.Append("]\n");
					}
					sb.Append("\t]\n");
				}
				sb.Append("\t[System Audit ACL [NOT DISPLAYED]]");
				return;
			}
			sb.Append("Security Descriptor [NULL]");
		}

		private static void AppendToString(StringBuilder sb, GenericAce ace)
		{
			sb.Append("Type:").Append(ace.AceType);
			QualifiedAce qualifiedAce = ace as QualifiedAce;
			if (qualifiedAce != null)
			{
				sb.Append(", Qualifier:").Append(qualifiedAce.AceQualifier);
				sb.Append(", Mask:0x").Append(qualifiedAce.AccessMask.ToString("X08"));
				sb.Append(", SID:");
				SecurityHelper.AppendToString(sb, qualifiedAce.SecurityIdentifier);
				ObjectAce objectAce = qualifiedAce as ObjectAce;
				if (objectAce != null)
				{
					sb.Append(", Object:").Append(objectAce.ObjectAceType.ToString());
					return;
				}
			}
			else
			{
				sb.Append(ace.ToString());
			}
		}

		public static void AppendToString(StringBuilder sb, SecurityIdentifier sid)
		{
			Trace fullAccountNameTracer = ExTraceGlobals.FullAccountNameTracer;
			if (!fullAccountNameTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				sb.Append(sid);
				return;
			}
			string value;
			string value2;
			if (sid != null && NativeMethods.LookupAccountSid(sid, out value, out value2))
			{
				if (!string.IsNullOrEmpty(value))
				{
					sb.Append(value).Append("\\");
				}
				sb.Append(value2).Append(" (").Append(sid).Append(")");
				return;
			}
			sb.Append(sid);
		}

		public static string ToString(RawSecurityDescriptor securityDescriptor)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			SecurityHelper.AppendToString(stringBuilder, securityDescriptor);
			return stringBuilder.ToString();
		}

		public static SecurityDescriptor StripUnknownObjectAces(SecurityDescriptor securityDescriptor)
		{
			if (securityDescriptor == null)
			{
				DiagnosticContext.TraceLocation((LID)49724U);
				return null;
			}
			RawSecurityDescriptor rawSecurityDescriptor = SecurityHelper.CreateRawSecurityDescriptor(securityDescriptor);
			if (rawSecurityDescriptor == null)
			{
				DiagnosticContext.TraceLocation((LID)62312U);
				return null;
			}
			RawAcl discretionaryAcl = rawSecurityDescriptor.DiscretionaryAcl;
			RawAcl rawAcl = new RawAcl(discretionaryAcl.Revision, 5);
			foreach (GenericAce genericAce in discretionaryAcl)
			{
				ObjectAce objectAce = genericAce as ObjectAce;
				if (!(objectAce != null) || MailboxSecurity.IsKnownRights(objectAce.ObjectAceType))
				{
					rawAcl.InsertAce(rawAcl.Count, genericAce);
				}
			}
			if (rawAcl.Count == 0 && discretionaryAcl.Count != 0)
			{
				rawAcl.InsertAce(0, discretionaryAcl[0]);
			}
			rawSecurityDescriptor.DiscretionaryAcl = rawAcl;
			return SecurityDescriptor.FromRawSecurityDescriptor(rawSecurityDescriptor);
		}

		public static bool IsValidSecurityIdentifierBlob(IExecutionContext context, byte[] buffer)
		{
			return SecurityHelper.CreateSecurityIdentifier(context, buffer) != null;
		}

		public static SecurityIdentifier CreateSecurityIdentifier(IExecutionContext context, byte[] buffer)
		{
			SecurityIdentifier result;
			try
			{
				result = new SecurityIdentifier(buffer, 0);
			}
			catch (ArgumentException exception)
			{
				context.Diagnostics.OnExceptionCatch(exception);
				result = null;
			}
			return result;
		}

		public static RawSecurityDescriptor CreateRawSecurityDescriptor(SecurityDescriptor securityDescriptor)
		{
			RawSecurityDescriptor result;
			try
			{
				result = securityDescriptor.ToRawSecurityDescriptorThrow();
			}
			catch (ArgumentException ex)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex);
				throw new StoreException((LID)37836U, ErrorCodeValue.InvalidParameter, "RawSecurityDescriptor parsing failed", ex);
			}
			catch (IndexOutOfRangeException ex2)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(ex2);
				throw new StoreException((LID)54220U, ErrorCodeValue.InvalidParameter, "RawSecurityDescriptor parsing failed", ex2);
			}
			return result;
		}

		public static string GetSDDL(SecurityDescriptor securityDescriptor)
		{
			string result;
			try
			{
				result = SecurityHelper.CreateRawSecurityDescriptor(securityDescriptor).GetSddlForm(AccessControlSections.All);
			}
			catch (StoreException exception)
			{
				NullExecutionDiagnostics.Instance.OnExceptionCatch(exception);
				result = "<CORRUPTED>";
			}
			return result;
		}
	}
}
