using System;
using System.Security.Principal;

namespace System.Security.AccessControl
{
	public sealed class ObjectAce : QualifiedAce
	{
		public ObjectAce(AceFlags aceFlags, AceQualifier qualifier, int accessMask, SecurityIdentifier sid, ObjectAceFlags flags, Guid type, Guid inheritedType, bool isCallback, byte[] opaque) : base(ObjectAce.TypeFromQualifier(isCallback, qualifier), aceFlags, accessMask, sid, opaque)
		{
			this._objectFlags = flags;
			this._objectAceType = type;
			this._inheritedObjectAceType = inheritedType;
		}

		private static AceType TypeFromQualifier(bool isCallback, AceQualifier qualifier)
		{
			switch (qualifier)
			{
			case AceQualifier.AccessAllowed:
				if (!isCallback)
				{
					return AceType.AccessAllowedObject;
				}
				return AceType.AccessAllowedCallbackObject;
			case AceQualifier.AccessDenied:
				if (!isCallback)
				{
					return AceType.AccessDeniedObject;
				}
				return AceType.AccessDeniedCallbackObject;
			case AceQualifier.SystemAudit:
				if (!isCallback)
				{
					return AceType.SystemAuditObject;
				}
				return AceType.SystemAuditCallbackObject;
			case AceQualifier.SystemAlarm:
				if (!isCallback)
				{
					return AceType.SystemAlarmObject;
				}
				return AceType.SystemAlarmCallbackObject;
			default:
				throw new ArgumentOutOfRangeException("qualifier", Environment.GetResourceString("ArgumentOutOfRange_Enum"));
			}
		}

		internal bool ObjectTypesMatch(ObjectAceFlags objectFlags, Guid objectType)
		{
			return (this.ObjectAceFlags & ObjectAceFlags.ObjectAceTypePresent) == (objectFlags & ObjectAceFlags.ObjectAceTypePresent) && ((this.ObjectAceFlags & ObjectAceFlags.ObjectAceTypePresent) == ObjectAceFlags.None || this.ObjectAceType.Equals(objectType));
		}

		internal bool InheritedObjectTypesMatch(ObjectAceFlags objectFlags, Guid inheritedObjectType)
		{
			return (this.ObjectAceFlags & ObjectAceFlags.InheritedObjectAceTypePresent) == (objectFlags & ObjectAceFlags.InheritedObjectAceTypePresent) && ((this.ObjectAceFlags & ObjectAceFlags.InheritedObjectAceTypePresent) == ObjectAceFlags.None || this.InheritedObjectAceType.Equals(inheritedObjectType));
		}

		internal static bool ParseBinaryForm(byte[] binaryForm, int offset, out AceQualifier qualifier, out int accessMask, out SecurityIdentifier sid, out ObjectAceFlags objectFlags, out Guid objectAceType, out Guid inheritedObjectAceType, out bool isCallback, out byte[] opaque)
		{
			byte[] array = new byte[16];
			GenericAce.VerifyHeader(binaryForm, offset);
			if (binaryForm.Length - offset >= 12 + SecurityIdentifier.MinBinaryLength)
			{
				AceType aceType = (AceType)binaryForm[offset];
				if (aceType == AceType.AccessAllowedObject || aceType == AceType.AccessDeniedObject || aceType == AceType.SystemAuditObject || aceType == AceType.SystemAlarmObject)
				{
					isCallback = false;
				}
				else
				{
					if (aceType != AceType.AccessAllowedCallbackObject && aceType != AceType.AccessDeniedCallbackObject && aceType != AceType.SystemAuditCallbackObject && aceType != AceType.SystemAlarmCallbackObject)
					{
						goto IL_209;
					}
					isCallback = true;
				}
				if (aceType == AceType.AccessAllowedObject || aceType == AceType.AccessAllowedCallbackObject)
				{
					qualifier = AceQualifier.AccessAllowed;
				}
				else if (aceType == AceType.AccessDeniedObject || aceType == AceType.AccessDeniedCallbackObject)
				{
					qualifier = AceQualifier.AccessDenied;
				}
				else if (aceType == AceType.SystemAuditObject || aceType == AceType.SystemAuditCallbackObject)
				{
					qualifier = AceQualifier.SystemAudit;
				}
				else
				{
					if (aceType != AceType.SystemAlarmObject && aceType != AceType.SystemAlarmCallbackObject)
					{
						goto IL_209;
					}
					qualifier = AceQualifier.SystemAlarm;
				}
				int num = offset + 4;
				int num2 = 0;
				accessMask = (int)binaryForm[num] + ((int)binaryForm[num + 1] << 8) + ((int)binaryForm[num + 2] << 16) + ((int)binaryForm[num + 3] << 24);
				num2 += 4;
				objectFlags = (ObjectAceFlags)((int)binaryForm[num + num2] + ((int)binaryForm[num + num2 + 1] << 8) + ((int)binaryForm[num + num2 + 2] << 16) + ((int)binaryForm[num + num2 + 3] << 24));
				num2 += 4;
				if ((objectFlags & ObjectAceFlags.ObjectAceTypePresent) != ObjectAceFlags.None)
				{
					for (int i = 0; i < 16; i++)
					{
						array[i] = binaryForm[num + num2 + i];
					}
					num2 += 16;
				}
				else
				{
					for (int j = 0; j < 16; j++)
					{
						array[j] = 0;
					}
				}
				objectAceType = new Guid(array);
				if ((objectFlags & ObjectAceFlags.InheritedObjectAceTypePresent) != ObjectAceFlags.None)
				{
					for (int k = 0; k < 16; k++)
					{
						array[k] = binaryForm[num + num2 + k];
					}
					num2 += 16;
				}
				else
				{
					for (int l = 0; l < 16; l++)
					{
						array[l] = 0;
					}
				}
				inheritedObjectAceType = new Guid(array);
				sid = new SecurityIdentifier(binaryForm, num + num2);
				opaque = null;
				int num3 = ((int)binaryForm[offset + 3] << 8) + (int)binaryForm[offset + 2];
				if (num3 % 4 == 0)
				{
					int num4 = num3 - 4 - 4 - 4 - (int)((byte)sid.BinaryLength);
					if ((objectFlags & ObjectAceFlags.ObjectAceTypePresent) != ObjectAceFlags.None)
					{
						num4 -= 16;
					}
					if ((objectFlags & ObjectAceFlags.InheritedObjectAceTypePresent) != ObjectAceFlags.None)
					{
						num4 -= 16;
					}
					if (num4 > 0)
					{
						opaque = new byte[num4];
						for (int m = 0; m < num4; m++)
						{
							opaque[m] = binaryForm[offset + num3 - num4 + m];
						}
					}
					return true;
				}
			}
			IL_209:
			qualifier = AceQualifier.AccessAllowed;
			accessMask = 0;
			sid = null;
			objectFlags = ObjectAceFlags.None;
			objectAceType = Guid.NewGuid();
			inheritedObjectAceType = Guid.NewGuid();
			isCallback = false;
			opaque = null;
			return false;
		}

		public ObjectAceFlags ObjectAceFlags
		{
			get
			{
				return this._objectFlags;
			}
			set
			{
				this._objectFlags = value;
			}
		}

		public Guid ObjectAceType
		{
			get
			{
				return this._objectAceType;
			}
			set
			{
				this._objectAceType = value;
			}
		}

		public Guid InheritedObjectAceType
		{
			get
			{
				return this._inheritedObjectAceType;
			}
			set
			{
				this._inheritedObjectAceType = value;
			}
		}

		public override int BinaryLength
		{
			get
			{
				int num = (((this._objectFlags & ObjectAceFlags.ObjectAceTypePresent) != ObjectAceFlags.None) ? 16 : 0) + (((this._objectFlags & ObjectAceFlags.InheritedObjectAceTypePresent) != ObjectAceFlags.None) ? 16 : 0);
				return 12 + num + base.SecurityIdentifier.BinaryLength + base.OpaqueLength;
			}
		}

		public static int MaxOpaqueLength(bool isCallback)
		{
			return 65491 - SecurityIdentifier.MaxBinaryLength;
		}

		internal override int MaxOpaqueLengthInternal
		{
			get
			{
				return ObjectAce.MaxOpaqueLength(base.IsCallback);
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
			binaryForm[num + num2] = (byte)this.ObjectAceFlags;
			binaryForm[num + num2 + 1] = (byte)(this.ObjectAceFlags >> 8);
			binaryForm[num + num2 + 2] = (byte)(this.ObjectAceFlags >> 16);
			binaryForm[num + num2 + 3] = (byte)(this.ObjectAceFlags >> 24);
			num2 += 4;
			if ((this.ObjectAceFlags & ObjectAceFlags.ObjectAceTypePresent) != ObjectAceFlags.None)
			{
				this.ObjectAceType.ToByteArray().CopyTo(binaryForm, num + num2);
				num2 += 16;
			}
			if ((this.ObjectAceFlags & ObjectAceFlags.InheritedObjectAceTypePresent) != ObjectAceFlags.None)
			{
				this.InheritedObjectAceType.ToByteArray().CopyTo(binaryForm, num + num2);
				num2 += 16;
			}
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

		private ObjectAceFlags _objectFlags;

		private Guid _objectAceType;

		private Guid _inheritedObjectAceType;

		private const int ObjectFlagsLength = 4;

		private const int GuidLength = 16;

		internal static readonly int AccessMaskWithObjectType = 315;
	}
}
