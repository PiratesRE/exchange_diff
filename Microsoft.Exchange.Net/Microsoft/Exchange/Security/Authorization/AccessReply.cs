using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security.Authorization
{
	internal class AccessReply
	{
		private AccessReply(AccessReply.AuthzAccessReply accessReply, AccessMask accessMaskToCheck)
		{
			this.results = new bool[accessReply.ResultListLength];
			this.grantedAccessMasks = new int[accessReply.ResultListLength];
			this.errors = new int[accessReply.ResultListLength];
			Marshal.Copy(accessReply.GrantedAccessMask, this.grantedAccessMasks, 0, (int)accessReply.ResultListLength);
			Marshal.Copy(accessReply.Error, this.errors, 0, (int)accessReply.ResultListLength);
			int num = 0;
			while ((long)num < (long)((ulong)accessReply.ResultListLength))
			{
				if (this.errors[num] == 0 && this.grantedAccessMasks[num] == (int)accessMaskToCheck)
				{
					this.results[num] = true;
				}
				else
				{
					this.results[num] = false;
				}
				num++;
			}
		}

		public int[] GrantedAccessMasks
		{
			get
			{
				return this.grantedAccessMasks;
			}
		}

		public int[] Errors
		{
			get
			{
				return this.errors;
			}
		}

		public bool[] Results
		{
			get
			{
				return this.results;
			}
		}

		public static SafeHGlobalHandle AllocateNativeStruct(int replyCount)
		{
			int num = AccessReply.AuthzAccessReply.MarshalSize;
			num += AccessReply.UInt32MarshalSize * replyCount;
			num += AccessReply.UInt32MarshalSize * replyCount;
			SafeHGlobalHandle safeHGlobalHandle = NativeMethods.AllocHGlobal(num);
			AccessReply.AuthzAccessReply authzAccessReply;
			authzAccessReply.ResultListLength = (uint)replyCount;
			authzAccessReply.GrantedAccessMask = (IntPtr)((long)safeHGlobalHandle.DangerousGetHandle() + (long)AccessReply.AuthzAccessReply.MarshalSize);
			authzAccessReply.SaclEvaluationResults = IntPtr.Zero;
			authzAccessReply.Error = (IntPtr)((long)authzAccessReply.GrantedAccessMask + (long)(AccessReply.UInt32MarshalSize * replyCount));
			Marshal.StructureToPtr(authzAccessReply, safeHGlobalHandle.DangerousGetHandle(), false);
			return safeHGlobalHandle;
		}

		public static AccessReply Create(SafeHGlobalHandle safeHandle)
		{
			return AccessReply.Create(safeHandle, AccessMask.ControlAccess);
		}

		public static AccessReply Create(SafeHGlobalHandle safeHandle, AccessMask accessMaskToCheck)
		{
			return new AccessReply((AccessReply.AuthzAccessReply)Marshal.PtrToStructure(safeHandle.DangerousGetHandle(), typeof(AccessReply.AuthzAccessReply)), accessMaskToCheck);
		}

		private static readonly int UInt32MarshalSize = Marshal.SizeOf(typeof(uint));

		private int[] grantedAccessMasks;

		private int[] errors;

		private bool[] results;

		private struct AuthzAccessReply
		{
			public static readonly int MarshalSize = Marshal.SizeOf(typeof(AccessReply.AuthzAccessReply));

			public uint ResultListLength;

			public IntPtr GrantedAccessMask;

			public IntPtr SaclEvaluationResults;

			public IntPtr Error;
		}
	}
}
