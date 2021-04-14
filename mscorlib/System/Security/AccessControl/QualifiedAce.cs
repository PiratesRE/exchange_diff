using System;
using System.Globalization;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public abstract class QualifiedAce : KnownAce
	{
		private AceQualifier QualifierFromType(AceType type, out bool isCallback)
		{
			switch (type)
			{
			case AceType.AccessAllowed:
				isCallback = false;
				return AceQualifier.AccessAllowed;
			case AceType.AccessDenied:
				isCallback = false;
				return AceQualifier.AccessDenied;
			case AceType.SystemAudit:
				isCallback = false;
				return AceQualifier.SystemAudit;
			case AceType.SystemAlarm:
				isCallback = false;
				return AceQualifier.SystemAlarm;
			case AceType.AccessAllowedObject:
				isCallback = false;
				return AceQualifier.AccessAllowed;
			case AceType.AccessDeniedObject:
				isCallback = false;
				return AceQualifier.AccessDenied;
			case AceType.SystemAuditObject:
				isCallback = false;
				return AceQualifier.SystemAudit;
			case AceType.SystemAlarmObject:
				isCallback = false;
				return AceQualifier.SystemAlarm;
			case AceType.AccessAllowedCallback:
				isCallback = true;
				return AceQualifier.AccessAllowed;
			case AceType.AccessDeniedCallback:
				isCallback = true;
				return AceQualifier.AccessDenied;
			case AceType.AccessAllowedCallbackObject:
				isCallback = true;
				return AceQualifier.AccessAllowed;
			case AceType.AccessDeniedCallbackObject:
				isCallback = true;
				return AceQualifier.AccessDenied;
			case AceType.SystemAuditCallback:
				isCallback = true;
				return AceQualifier.SystemAudit;
			case AceType.SystemAlarmCallback:
				isCallback = true;
				return AceQualifier.SystemAlarm;
			case AceType.SystemAuditCallbackObject:
				isCallback = true;
				return AceQualifier.SystemAudit;
			case AceType.SystemAlarmCallbackObject:
				isCallback = true;
				return AceQualifier.SystemAlarm;
			}
			throw new SystemException();
		}

		internal QualifiedAce(AceType type, AceFlags flags, int accessMask, SecurityIdentifier sid, byte[] opaque) : base(type, flags, accessMask, sid)
		{
			this._qualifier = this.QualifierFromType(type, out this._isCallback);
			this.SetOpaque(opaque);
		}

		public AceQualifier AceQualifier
		{
			get
			{
				return this._qualifier;
			}
		}

		public bool IsCallback
		{
			get
			{
				return this._isCallback;
			}
		}

		internal abstract int MaxOpaqueLengthInternal { get; }

		public int OpaqueLength
		{
			get
			{
				if (this._opaque != null)
				{
					return this._opaque.Length;
				}
				return 0;
			}
		}

		public byte[] GetOpaque()
		{
			return this._opaque;
		}

		public void SetOpaque(byte[] opaque)
		{
			if (opaque != null)
			{
				if (opaque.Length > this.MaxOpaqueLengthInternal)
				{
					throw new ArgumentOutOfRangeException("opaque", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_ArrayLength"), 0, this.MaxOpaqueLengthInternal));
				}
				if (opaque.Length % 4 != 0)
				{
					throw new ArgumentOutOfRangeException("opaque", string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_ArrayLengthMultiple"), 4));
				}
			}
			this._opaque = opaque;
		}

		private readonly bool _isCallback;

		private readonly AceQualifier _qualifier;

		private byte[] _opaque;
	}
}
