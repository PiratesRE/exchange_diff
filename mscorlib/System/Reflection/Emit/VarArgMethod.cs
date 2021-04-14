using System;

namespace System.Reflection.Emit
{
	internal sealed class VarArgMethod
	{
		internal VarArgMethod(DynamicMethod dm, SignatureHelper signature)
		{
			this.m_dynamicMethod = dm;
			this.m_signature = signature;
		}

		internal VarArgMethod(RuntimeMethodInfo method, SignatureHelper signature)
		{
			this.m_method = method;
			this.m_signature = signature;
		}

		internal RuntimeMethodInfo m_method;

		internal DynamicMethod m_dynamicMethod;

		internal SignatureHelper m_signature;
	}
}
