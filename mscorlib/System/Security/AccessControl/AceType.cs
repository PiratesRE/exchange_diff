using System;

namespace System.Security.AccessControl
{
	public enum AceType : byte
	{
		AccessAllowed,
		AccessDenied,
		SystemAudit,
		SystemAlarm,
		AccessAllowedCompound,
		AccessAllowedObject,
		AccessDeniedObject,
		SystemAuditObject,
		SystemAlarmObject,
		AccessAllowedCallback,
		AccessDeniedCallback,
		AccessAllowedCallbackObject,
		AccessDeniedCallbackObject,
		SystemAuditCallback,
		SystemAlarmCallback,
		SystemAuditCallbackObject,
		SystemAlarmCallbackObject,
		MaxDefinedAceType = 16
	}
}
