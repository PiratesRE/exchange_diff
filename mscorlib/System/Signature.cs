using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;

namespace System
{
	internal class Signature
	{
		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe extern void GetSignature(void* pCorSig, int cCorSig, RuntimeFieldHandleInternal fieldHandle, IRuntimeMethodInfo methodHandle, RuntimeType declaringType);

		[SecuritySafeCritical]
		public Signature(IRuntimeMethodInfo method, RuntimeType[] arguments, RuntimeType returnType, CallingConventions callingConvention)
		{
			this.m_pMethod = method.Value;
			this.m_arguments = arguments;
			this.m_returnTypeORfieldType = returnType;
			this.m_managedCallingConventionAndArgIteratorFlags = (int)((byte)callingConvention);
			this.GetSignature(null, 0, default(RuntimeFieldHandleInternal), method, null);
		}

		[SecuritySafeCritical]
		public Signature(IRuntimeMethodInfo methodHandle, RuntimeType declaringType)
		{
			this.GetSignature(null, 0, default(RuntimeFieldHandleInternal), methodHandle, declaringType);
		}

		[SecurityCritical]
		public Signature(IRuntimeFieldInfo fieldHandle, RuntimeType declaringType)
		{
			this.GetSignature(null, 0, fieldHandle.Value, null, declaringType);
			GC.KeepAlive(fieldHandle);
		}

		[SecurityCritical]
		public unsafe Signature(void* pCorSig, int cCorSig, RuntimeType declaringType)
		{
			this.GetSignature(pCorSig, cCorSig, default(RuntimeFieldHandleInternal), null, declaringType);
		}

		internal CallingConventions CallingConvention
		{
			get
			{
				return (CallingConventions)((byte)this.m_managedCallingConventionAndArgIteratorFlags);
			}
		}

		internal RuntimeType[] Arguments
		{
			get
			{
				return this.m_arguments;
			}
		}

		internal RuntimeType ReturnType
		{
			get
			{
				return this.m_returnTypeORfieldType;
			}
		}

		internal RuntimeType FieldType
		{
			get
			{
				return this.m_returnTypeORfieldType;
			}
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool CompareSig(Signature sig1, Signature sig2);

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern Type[] GetCustomModifiers(int position, bool required);

		internal RuntimeType[] m_arguments;

		internal RuntimeType m_declaringType;

		internal RuntimeType m_returnTypeORfieldType;

		internal object m_keepalive;

		[SecurityCritical]
		internal unsafe void* m_sig;

		internal int m_managedCallingConventionAndArgIteratorFlags;

		internal int m_nSizeOfArgStack;

		internal int m_csig;

		internal RuntimeMethodHandleInternal m_pMethod;

		internal enum MdSigCallingConvention : byte
		{
			Generics = 16,
			HasThis = 32,
			ExplicitThis = 64,
			CallConvMask = 15,
			Default = 0,
			C,
			StdCall,
			ThisCall,
			FastCall,
			Vararg,
			Field,
			LocalSig,
			Property,
			Unmgd,
			GenericInst,
			Max
		}
	}
}
