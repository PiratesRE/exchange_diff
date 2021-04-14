using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class CommonAce : QualifiedAce
	{
		public CommonAce(AceFlags flags, AceQualifier qualifier, int accessMask, SecurityIdentifier sid, bool isCallback, byte[] opaque) : base(CommonAce.TypeFromQualifier(isCallback, qualifier), flags, accessMask, sid, opaque)
		{
		}

		private static AceType TypeFromQualifier(bool isCallback, AceQualifier qualifier)
		{
			switch (qualifier)
			{
			case AceQualifier.AccessAllowed:
				if (!isCallback)
				{
					return AceType.AccessAllowed;
				}
				return AceType.AccessAllowedCallback;
			case AceQualifier.AccessDenied:
				if (!isCallback)
				{
					return AceType.AccessDenied;
				}
				return AceType.AccessDeniedCallback;
			case AceQualifier.SystemAudit:
				if (!isCallback)
				{
					return AceType.SystemAudit;
				}
				return AceType.SystemAuditCallback;
			case AceQualifier.SystemAlarm:
				if (!isCallback)
				{
					return AceType.SystemAlarm;
				}
				return AceType.SystemAlarmCallback;
			default:
				throw new ArgumentOutOfRangeException("qualifier", Environment.GetResourceString("ArgumentOutOfRange_Enum"));
			}
		}

		internal static bool ParseBinaryForm(byte[] binaryForm, int offset, out AceQualifier qualifier, out int accessMask, out SecurityIdentifier sid, out bool isCallback, out byte[] opaque)
		{
			GenericAce.VerifyHeader(binaryForm, offset);
			if (binaryForm.Length - offset >= 8 + SecurityIdentifier.MinBinaryLength)
			{
				AceType aceType = (AceType)binaryForm[offset];
				if (aceType == AceType.AccessAllowed || aceType == AceType.AccessDenied || aceType == AceType.SystemAudit || aceType == AceType.SystemAlarm)
				{
					isCallback = false;
				}
				else
				{
					if (aceType != AceType.AccessAllowedCallback && aceType != AceType.AccessDeniedCallback && aceType != AceType.SystemAuditCallback && aceType != AceType.SystemAlarmCallback)
					{
						goto IL_114;
					}
					isCallback = true;
				}
				if (aceType == AceType.AccessAllowed || aceType == AceType.AccessAllowedCallback)
				{
					qualifier = AceQualifier.AccessAllowed;
				}
				else if (aceType == AceType.AccessDenied || aceType == AceType.AccessDeniedCallback)
				{
					qualifier = AceQualifier.AccessDenied;
				}
				else if (aceType == AceType.SystemAudit || aceType == AceType.SystemAuditCallback)
				{
					qualifier = AceQualifier.SystemAudit;
				}
				else
				{
					if (aceType != AceType.SystemAlarm && aceType != AceType.SystemAlarmCallback)
					{
						goto IL_114;
					}
					qualifier = AceQualifier.SystemAlarm;
				}
				int num = offset + 4;
				int num2 = 0;
				accessMask = (int)binaryForm[num] + ((int)binaryForm[num + 1] << 8) + ((int)binaryForm[num + 2] << 16) + ((int)binaryForm[num + 3] << 24);
				num2 += 4;
				sid = new SecurityIdentifier(binaryForm, num + num2);
				opaque = null;
				int num3 = ((int)binaryForm[offset + 3] << 8) + (int)binaryForm[offset + 2];
				if (num3 % 4 == 0)
				{
					int num4 = num3 - 4 - 4 - (int)((byte)sid.BinaryLength);
					if (num4 > 0)
					{
						opaque = new byte[num4];
						for (int i = 0; i < num4; i++)
						{
							opaque[i] = binaryForm[offset + num3 - num4 + i];
						}
					}
					return true;
				}
			}
			IL_114:
			qualifier = AceQualifier.AccessAllowed;
			accessMask = 0;
			sid = null;
			isCallback = false;
			opaque = null;
			return false;
		}

		public override int BinaryLength
		{
			get
			{
				return 8 + base.SecurityIdentifier.BinaryLength + base.OpaqueLength;
			}
		}

		public static int MaxOpaqueLength(bool isCallback)
		{
			return 65527 - SecurityIdentifier.MaxBinaryLength;
		}

		internal override int MaxOpaqueLengthInternal
		{
			get
			{
				return CommonAce.MaxOpaqueLength(base.IsCallback);
			}
		}

		public override void GetBinaryForm(byte[] binaryForm, int offset)
		{
			base.MarshalHeader(binaryForm, offset);
			int num = offset + 4;
			int num2 = 0;
			binaryForm[num] = (byte)base.AccessMask;
			binaryForm[num + 1] = (byte)(base.AccessMask >> 8);
			binaryForm[num + 2] = (byte)(base.AccessMask >> 16);
			binaryForm[num + 3] = (byte)(base.AccessMask >> 24);
			num2 += 4;
			base.SecurityIdentifier.GetBinaryForm(binaryForm, num + num2);
			num2 += base.SecurityIdentifier.BinaryLength;
			if (base.GetOpaque() != null)
			{
				if (base.OpaqueLength > this.MaxOpaqueLengthInternal)
				{
					throw new SystemException();
				}
				base.GetOpaque().CopyTo(binaryForm, num + num2);
			}
		}
	}
}
