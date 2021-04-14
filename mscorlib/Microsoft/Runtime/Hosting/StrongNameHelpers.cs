using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Runtime.Hosting
{
	internal static class StrongNameHelpers
	{
		private static IClrStrongName StrongName
		{
			[SecurityCritical]
			get
			{
				if (StrongNameHelpers.s_StrongName == null)
				{
					StrongNameHelpers.s_StrongName = (IClrStrongName)RuntimeEnvironment.GetRuntimeInterfaceAsObject(new Guid("B79B0ACD-F5CD-409b-B5A5-A16244610B92"), new Guid("9FD93CCF-3280-4391-B3A9-96E1CDE77C8D"));
				}
				return StrongNameHelpers.s_StrongName;
			}
		}

		private static IClrStrongNameUsingIntPtr StrongNameUsingIntPtr
		{
			[SecurityCritical]
			get
			{
				return (IClrStrongNameUsingIntPtr)StrongNameHelpers.StrongName;
			}
		}

		[SecurityCritical]
		public static int StrongNameErrorInfo()
		{
			return StrongNameHelpers.ts_LastStrongNameHR;
		}

		[SecurityCritical]
		public static void StrongNameFreeBuffer(IntPtr pbMemory)
		{
			StrongNameHelpers.StrongNameUsingIntPtr.StrongNameFreeBuffer(pbMemory);
		}

		[SecurityCritical]
		public static bool StrongNameGetPublicKey(string pwzKeyContainer, IntPtr pbKeyBlob, int cbKeyBlob, out IntPtr ppbPublicKeyBlob, out int pcbPublicKeyBlob)
		{
			int num = StrongNameHelpers.StrongNameUsingIntPtr.StrongNameGetPublicKey(pwzKeyContainer, pbKeyBlob, cbKeyBlob, out ppbPublicKeyBlob, out pcbPublicKeyBlob);
			if (num < 0)
			{
				StrongNameHelpers.ts_LastStrongNameHR = num;
				ppbPublicKeyBlob = IntPtr.Zero;
				pcbPublicKeyBlob = 0;
				return false;
			}
			return true;
		}

		[SecurityCritical]
		public static bool StrongNameKeyDelete(string pwzKeyContainer)
		{
			int num = StrongNameHelpers.StrongName.StrongNameKeyDelete(pwzKeyContainer);
			if (num < 0)
			{
				StrongNameHelpers.ts_LastStrongNameHR = num;
				return false;
			}
			return true;
		}

		[SecurityCritical]
		public static bool StrongNameKeyGen(string pwzKeyContainer, int dwFlags, out IntPtr ppbKeyBlob, out int pcbKeyBlob)
		{
			int num = StrongNameHelpers.StrongName.StrongNameKeyGen(pwzKeyContainer, dwFlags, out ppbKeyBlob, out pcbKeyBlob);
			if (num < 0)
			{
				StrongNameHelpers.ts_LastStrongNameHR = num;
				ppbKeyBlob = IntPtr.Zero;
				pcbKeyBlob = 0;
				return false;
			}
			return true;
		}

		[SecurityCritical]
		public static bool StrongNameKeyInstall(string pwzKeyContainer, IntPtr pbKeyBlob, int cbKeyBlob)
		{
			int num = StrongNameHelpers.StrongNameUsingIntPtr.StrongNameKeyInstall(pwzKeyContainer, pbKeyBlob, cbKeyBlob);
			if (num < 0)
			{
				StrongNameHelpers.ts_LastStrongNameHR = num;
				return false;
			}
			return true;
		}

		[SecurityCritical]
		public static bool StrongNameSignatureGeneration(string pwzFilePath, string pwzKeyContainer, IntPtr pbKeyBlob, int cbKeyBlob)
		{
			IntPtr zero = IntPtr.Zero;
			int num = 0;
			return StrongNameHelpers.StrongNameSignatureGeneration(pwzFilePath, pwzKeyContainer, pbKeyBlob, cbKeyBlob, ref zero, out num);
		}

		[SecurityCritical]
		public static bool StrongNameSignatureGeneration(string pwzFilePath, string pwzKeyContainer, IntPtr pbKeyBlob, int cbKeyBlob, ref IntPtr ppbSignatureBlob, out int pcbSignatureBlob)
		{
			int num = StrongNameHelpers.StrongNameUsingIntPtr.StrongNameSignatureGeneration(pwzFilePath, pwzKeyContainer, pbKeyBlob, cbKeyBlob, ppbSignatureBlob, out pcbSignatureBlob);
			if (num < 0)
			{
				StrongNameHelpers.ts_LastStrongNameHR = num;
				pcbSignatureBlob = 0;
				return false;
			}
			return true;
		}

		[SecurityCritical]
		public static bool StrongNameSignatureSize(IntPtr pbPublicKeyBlob, int cbPublicKeyBlob, out int pcbSize)
		{
			int num = StrongNameHelpers.StrongNameUsingIntPtr.StrongNameSignatureSize(pbPublicKeyBlob, cbPublicKeyBlob, out pcbSize);
			if (num < 0)
			{
				StrongNameHelpers.ts_LastStrongNameHR = num;
				pcbSize = 0;
				return false;
			}
			return true;
		}

		[SecurityCritical]
		public static bool StrongNameSignatureVerification(string pwzFilePath, int dwInFlags, out int pdwOutFlags)
		{
			int num = StrongNameHelpers.StrongName.StrongNameSignatureVerification(pwzFilePath, dwInFlags, out pdwOutFlags);
			if (num < 0)
			{
				StrongNameHelpers.ts_LastStrongNameHR = num;
				pdwOutFlags = 0;
				return false;
			}
			return true;
		}

		[SecurityCritical]
		public static bool StrongNameSignatureVerificationEx(string pwzFilePath, bool fForceVerification, out bool pfWasVerified)
		{
			int num = StrongNameHelpers.StrongName.StrongNameSignatureVerificationEx(pwzFilePath, fForceVerification, out pfWasVerified);
			if (num < 0)
			{
				StrongNameHelpers.ts_LastStrongNameHR = num;
				pfWasVerified = false;
				return false;
			}
			return true;
		}

		[SecurityCritical]
		public static bool StrongNameTokenFromPublicKey(IntPtr pbPublicKeyBlob, int cbPublicKeyBlob, out IntPtr ppbStrongNameToken, out int pcbStrongNameToken)
		{
			int num = StrongNameHelpers.StrongNameUsingIntPtr.StrongNameTokenFromPublicKey(pbPublicKeyBlob, cbPublicKeyBlob, out ppbStrongNameToken, out pcbStrongNameToken);
			if (num < 0)
			{
				StrongNameHelpers.ts_LastStrongNameHR = num;
				ppbStrongNameToken = IntPtr.Zero;
				pcbStrongNameToken = 0;
				return false;
			}
			return true;
		}

		[SecurityCritical]
		public static bool StrongNameSignatureSize(byte[] bPublicKeyBlob, int cbPublicKeyBlob, out int pcbSize)
		{
			int num = StrongNameHelpers.StrongName.StrongNameSignatureSize(bPublicKeyBlob, cbPublicKeyBlob, out pcbSize);
			if (num < 0)
			{
				StrongNameHelpers.ts_LastStrongNameHR = num;
				pcbSize = 0;
				return false;
			}
			return true;
		}

		[SecurityCritical]
		public static bool StrongNameTokenFromPublicKey(byte[] bPublicKeyBlob, int cbPublicKeyBlob, out IntPtr ppbStrongNameToken, out int pcbStrongNameToken)
		{
			int num = StrongNameHelpers.StrongName.StrongNameTokenFromPublicKey(bPublicKeyBlob, cbPublicKeyBlob, out ppbStrongNameToken, out pcbStrongNameToken);
			if (num < 0)
			{
				StrongNameHelpers.ts_LastStrongNameHR = num;
				ppbStrongNameToken = IntPtr.Zero;
				pcbStrongNameToken = 0;
				return false;
			}
			return true;
		}

		[SecurityCritical]
		public static bool StrongNameGetPublicKey(string pwzKeyContainer, byte[] bKeyBlob, int cbKeyBlob, out IntPtr ppbPublicKeyBlob, out int pcbPublicKeyBlob)
		{
			int num = StrongNameHelpers.StrongName.StrongNameGetPublicKey(pwzKeyContainer, bKeyBlob, cbKeyBlob, out ppbPublicKeyBlob, out pcbPublicKeyBlob);
			if (num < 0)
			{
				StrongNameHelpers.ts_LastStrongNameHR = num;
				ppbPublicKeyBlob = IntPtr.Zero;
				pcbPublicKeyBlob = 0;
				return false;
			}
			return true;
		}

		[SecurityCritical]
		public static bool StrongNameKeyInstall(string pwzKeyContainer, byte[] bKeyBlob, int cbKeyBlob)
		{
			int num = StrongNameHelpers.StrongName.StrongNameKeyInstall(pwzKeyContainer, bKeyBlob, cbKeyBlob);
			if (num < 0)
			{
				StrongNameHelpers.ts_LastStrongNameHR = num;
				return false;
			}
			return true;
		}

		[SecurityCritical]
		public static bool StrongNameSignatureGeneration(string pwzFilePath, string pwzKeyContainer, byte[] bKeyBlob, int cbKeyBlob)
		{
			IntPtr zero = IntPtr.Zero;
			int num = 0;
			return StrongNameHelpers.StrongNameSignatureGeneration(pwzFilePath, pwzKeyContainer, bKeyBlob, cbKeyBlob, ref zero, out num);
		}

		[SecurityCritical]
		public static bool StrongNameSignatureGeneration(string pwzFilePath, string pwzKeyContainer, byte[] bKeyBlob, int cbKeyBlob, ref IntPtr ppbSignatureBlob, out int pcbSignatureBlob)
		{
			int num = StrongNameHelpers.StrongName.StrongNameSignatureGeneration(pwzFilePath, pwzKeyContainer, bKeyBlob, cbKeyBlob, ppbSignatureBlob, out pcbSignatureBlob);
			if (num < 0)
			{
				StrongNameHelpers.ts_LastStrongNameHR = num;
				pcbSignatureBlob = 0;
				return false;
			}
			return true;
		}

		[ThreadStatic]
		private static int ts_LastStrongNameHR;

		[SecurityCritical]
		[ThreadStatic]
		private static IClrStrongName s_StrongName;
	}
}
