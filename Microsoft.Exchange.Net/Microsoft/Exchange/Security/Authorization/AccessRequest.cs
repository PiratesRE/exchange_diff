using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security.Authorization
{
	internal class AccessRequest
	{
		private AccessRequest()
		{
		}

		public static SafeHGlobalHandle AllocateNativeStruct(AccessMask requestAccess, Guid[] extendedRightGuids, SecurityIdentifier principalSelfSid)
		{
			int num = (extendedRightGuids == null) ? 0 : extendedRightGuids.GetLength(0);
			int num2 = AccessRequest.AuthzAccessRequest.MarshalSize;
			num2 += AccessRequest.AuthzObjectTypeList.MarshalSize * num;
			num2 += AccessRequest.GuidMarshalSize * num;
			if (principalSelfSid != null)
			{
				num2 += principalSelfSid.BinaryLength;
			}
			SafeHGlobalHandle safeHGlobalHandle = NativeMethods.AllocHGlobal(num2);
			AccessRequest.AuthzAccessRequest authzAccessRequest;
			authzAccessRequest.ObjectTypeListLength = (uint)num;
			IntPtr intPtr = (IntPtr)((long)safeHGlobalHandle.DangerousGetHandle() + (long)AccessRequest.AuthzAccessRequest.MarshalSize);
			if (num != 0)
			{
				authzAccessRequest.ObjectTypeList = intPtr;
				intPtr = (IntPtr)((long)intPtr + (long)(AccessRequest.AuthzObjectTypeList.MarshalSize * num) + (long)(AccessRequest.GuidMarshalSize * num));
			}
			else
			{
				authzAccessRequest.ObjectTypeList = IntPtr.Zero;
			}
			authzAccessRequest.DesiredAccess = requestAccess;
			authzAccessRequest.OptionalArguments = IntPtr.Zero;
			if (principalSelfSid != null)
			{
				authzAccessRequest.PrincipalSelfSid = intPtr;
			}
			else
			{
				authzAccessRequest.PrincipalSelfSid = IntPtr.Zero;
			}
			Marshal.StructureToPtr(authzAccessRequest, safeHGlobalHandle.DangerousGetHandle(), false);
			IntPtr intPtr2 = authzAccessRequest.ObjectTypeList;
			IntPtr intPtr3 = (IntPtr)((long)authzAccessRequest.ObjectTypeList + (long)(AccessRequest.AuthzObjectTypeList.MarshalSize * num));
			for (int i = 0; i < num; i++)
			{
				AccessRequest.AuthzObjectTypeList authzObjectTypeList;
				authzObjectTypeList.Sbz = 0;
				if (i > 0)
				{
					authzObjectTypeList.Level = ObjectLevel.AccessPropertySetGuid;
				}
				else
				{
					authzObjectTypeList.Level = ObjectLevel.AccessObjectGuid;
				}
				authzObjectTypeList.ObjectType = intPtr3;
				Marshal.StructureToPtr(authzObjectTypeList, intPtr2, false);
				Marshal.StructureToPtr(extendedRightGuids[i], intPtr3, false);
				intPtr2 = (IntPtr)((long)intPtr2 + (long)AccessRequest.AuthzObjectTypeList.MarshalSize);
				intPtr3 = (IntPtr)((long)intPtr3 + (long)AccessRequest.GuidMarshalSize);
			}
			if (principalSelfSid != null)
			{
				byte[] array = new byte[principalSelfSid.BinaryLength];
				principalSelfSid.GetBinaryForm(array, 0);
				Marshal.Copy(array, 0, authzAccessRequest.PrincipalSelfSid, principalSelfSid.BinaryLength);
			}
			return safeHGlobalHandle;
		}

		private static readonly int GuidMarshalSize = Marshal.SizeOf(typeof(Guid));

		private struct AuthzAccessRequest
		{
			public static readonly int MarshalSize = Marshal.SizeOf(typeof(AccessRequest.AuthzAccessRequest));

			public AccessMask DesiredAccess;

			public IntPtr PrincipalSelfSid;

			public IntPtr ObjectTypeList;

			public uint ObjectTypeListLength;

			public IntPtr OptionalArguments;
		}

		private struct AuthzObjectTypeList
		{
			public static readonly int MarshalSize = Marshal.SizeOf(typeof(AccessRequest.AuthzObjectTypeList));

			public ObjectLevel Level;

			public ushort Sbz;

			public IntPtr ObjectType;
		}
	}
}
