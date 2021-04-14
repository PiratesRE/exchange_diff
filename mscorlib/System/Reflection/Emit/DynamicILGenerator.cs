using System;
using System.Diagnostics.SymbolStore;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Reflection.Emit
{
	internal class DynamicILGenerator : ILGenerator
	{
		internal DynamicILGenerator(DynamicMethod method, byte[] methodSignature, int size) : base(method, size)
		{
			this.m_scope = new DynamicScope();
			this.m_methodSigToken = this.m_scope.GetTokenFor(methodSignature);
		}

		[SecurityCritical]
		internal void GetCallableMethod(RuntimeModule module, DynamicMethod dm)
		{
			dm.m_methodHandle = ModuleHandle.GetDynamicMethod(dm, module, this.m_methodBuilder.Name, (byte[])this.m_scope[this.m_methodSigToken], new DynamicResolver(this));
		}

		private bool ProfileAPICheck
		{
			get
			{
				return ((DynamicMethod)this.m_methodBuilder).ProfileAPICheck;
			}
		}

		public override LocalBuilder DeclareLocal(Type localType, bool pinned)
		{
			if (localType == null)
			{
				throw new ArgumentNullException("localType");
			}
			RuntimeType runtimeType = localType as RuntimeType;
			if (runtimeType == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"));
			}
			if (this.ProfileAPICheck && (runtimeType.InvocationFlags & INVOCATION_FLAGS.INVOCATION_FLAGS_NON_W8P_FX_API) != INVOCATION_FLAGS.INVOCATION_FLAGS_UNKNOWN)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_APIInvalidForCurrentContext", new object[]
				{
					runtimeType.FullName
				}));
			}
			LocalBuilder result = new LocalBuilder(this.m_localCount, localType, this.m_methodBuilder);
			this.m_localSignature.AddArgument(localType, pinned);
			this.m_localCount++;
			return result;
		}

		[SecuritySafeCritical]
		public override void Emit(OpCode opcode, MethodInfo meth)
		{
			if (meth == null)
			{
				throw new ArgumentNullException("meth");
			}
			int num = 0;
			DynamicMethod dynamicMethod = meth as DynamicMethod;
			int tokenFor;
			if (dynamicMethod == null)
			{
				RuntimeMethodInfo runtimeMethodInfo = meth as RuntimeMethodInfo;
				if (runtimeMethodInfo == null)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeMethodInfo"), "meth");
				}
				RuntimeType runtimeType = runtimeMethodInfo.GetRuntimeType();
				if (runtimeType != null && (runtimeType.IsGenericType || runtimeType.IsArray))
				{
					tokenFor = this.GetTokenFor(runtimeMethodInfo, runtimeType);
				}
				else
				{
					tokenFor = this.GetTokenFor(runtimeMethodInfo);
				}
			}
			else
			{
				if (opcode.Equals(OpCodes.Ldtoken) || opcode.Equals(OpCodes.Ldftn) || opcode.Equals(OpCodes.Ldvirtftn))
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOpCodeOnDynamicMethod"));
				}
				tokenFor = this.GetTokenFor(dynamicMethod);
			}
			base.EnsureCapacity(7);
			base.InternalEmit(opcode);
			if (opcode.StackBehaviourPush == StackBehaviour.Varpush && meth.ReturnType != typeof(void))
			{
				num++;
			}
			if (opcode.StackBehaviourPop == StackBehaviour.Varpop)
			{
				num -= meth.GetParametersNoCopy().Length;
			}
			if (!meth.IsStatic && !opcode.Equals(OpCodes.Newobj) && !opcode.Equals(OpCodes.Ldtoken) && !opcode.Equals(OpCodes.Ldftn))
			{
				num--;
			}
			base.UpdateStackSize(opcode, num);
			base.PutInteger4(tokenFor);
		}

		[ComVisible(true)]
		public override void Emit(OpCode opcode, ConstructorInfo con)
		{
			if (con == null)
			{
				throw new ArgumentNullException("con");
			}
			RuntimeConstructorInfo runtimeConstructorInfo = con as RuntimeConstructorInfo;
			if (runtimeConstructorInfo == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeMethodInfo"), "con");
			}
			RuntimeType runtimeType = runtimeConstructorInfo.GetRuntimeType();
			int tokenFor;
			if (runtimeType != null && (runtimeType.IsGenericType || runtimeType.IsArray))
			{
				tokenFor = this.GetTokenFor(runtimeConstructorInfo, runtimeType);
			}
			else
			{
				tokenFor = this.GetTokenFor(runtimeConstructorInfo);
			}
			base.EnsureCapacity(7);
			base.InternalEmit(opcode);
			base.UpdateStackSize(opcode, 1);
			base.PutInteger4(tokenFor);
		}

		public override void Emit(OpCode opcode, Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			RuntimeType runtimeType = type as RuntimeType;
			if (runtimeType == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"));
			}
			int tokenFor = this.GetTokenFor(runtimeType);
			base.EnsureCapacity(7);
			base.InternalEmit(opcode);
			base.PutInteger4(tokenFor);
		}

		public override void Emit(OpCode opcode, FieldInfo field)
		{
			if (field == null)
			{
				throw new ArgumentNullException("field");
			}
			RuntimeFieldInfo runtimeFieldInfo = field as RuntimeFieldInfo;
			if (runtimeFieldInfo == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeFieldInfo"), "field");
			}
			int tokenFor;
			if (field.DeclaringType == null)
			{
				tokenFor = this.GetTokenFor(runtimeFieldInfo);
			}
			else
			{
				tokenFor = this.GetTokenFor(runtimeFieldInfo, runtimeFieldInfo.GetRuntimeType());
			}
			base.EnsureCapacity(7);
			base.InternalEmit(opcode);
			base.PutInteger4(tokenFor);
		}

		public override void Emit(OpCode opcode, string str)
		{
			if (str == null)
			{
				throw new ArgumentNullException("str");
			}
			int tokenForString = this.GetTokenForString(str);
			base.EnsureCapacity(7);
			base.InternalEmit(opcode);
			base.PutInteger4(tokenForString);
		}

		[SecuritySafeCritical]
		public override void EmitCalli(OpCode opcode, CallingConventions callingConvention, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes)
		{
			int num = 0;
			if (optionalParameterTypes != null && (callingConvention & CallingConventions.VarArgs) == (CallingConventions)0)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NotAVarArgCallingConvention"));
			}
			SignatureHelper memberRefSignature = this.GetMemberRefSignature(callingConvention, returnType, parameterTypes, optionalParameterTypes);
			base.EnsureCapacity(7);
			this.Emit(OpCodes.Calli);
			if (returnType != typeof(void))
			{
				num++;
			}
			if (parameterTypes != null)
			{
				num -= parameterTypes.Length;
			}
			if (optionalParameterTypes != null)
			{
				num -= optionalParameterTypes.Length;
			}
			if ((callingConvention & CallingConventions.HasThis) == CallingConventions.HasThis)
			{
				num--;
			}
			num--;
			base.UpdateStackSize(OpCodes.Calli, num);
			int tokenForSig = this.GetTokenForSig(memberRefSignature.GetSignature(true));
			base.PutInteger4(tokenForSig);
		}

		public override void EmitCalli(OpCode opcode, CallingConvention unmanagedCallConv, Type returnType, Type[] parameterTypes)
		{
			int num = 0;
			int num2 = 0;
			if (parameterTypes != null)
			{
				num2 = parameterTypes.Length;
			}
			SignatureHelper methodSigHelper = SignatureHelper.GetMethodSigHelper(unmanagedCallConv, returnType);
			if (parameterTypes != null)
			{
				for (int i = 0; i < num2; i++)
				{
					methodSigHelper.AddArgument(parameterTypes[i]);
				}
			}
			if (returnType != typeof(void))
			{
				num++;
			}
			if (parameterTypes != null)
			{
				num -= num2;
			}
			num--;
			base.UpdateStackSize(OpCodes.Calli, num);
			base.EnsureCapacity(7);
			this.Emit(OpCodes.Calli);
			int tokenForSig = this.GetTokenForSig(methodSigHelper.GetSignature(true));
			base.PutInteger4(tokenForSig);
		}

		[SecuritySafeCritical]
		public override void EmitCall(OpCode opcode, MethodInfo methodInfo, Type[] optionalParameterTypes)
		{
			if (methodInfo == null)
			{
				throw new ArgumentNullException("methodInfo");
			}
			if (!opcode.Equals(OpCodes.Call) && !opcode.Equals(OpCodes.Callvirt) && !opcode.Equals(OpCodes.Newobj))
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_NotMethodCallOpcode"), "opcode");
			}
			if (methodInfo.ContainsGenericParameters)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_GenericsInvalid"), "methodInfo");
			}
			if (methodInfo.DeclaringType != null && methodInfo.DeclaringType.ContainsGenericParameters)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_GenericsInvalid"), "methodInfo");
			}
			int num = 0;
			int memberRefToken = this.GetMemberRefToken(methodInfo, optionalParameterTypes);
			base.EnsureCapacity(7);
			base.InternalEmit(opcode);
			if (methodInfo.ReturnType != typeof(void))
			{
				num++;
			}
			num -= methodInfo.GetParameterTypes().Length;
			if (!(methodInfo is SymbolMethod) && !methodInfo.IsStatic && !opcode.Equals(OpCodes.Newobj))
			{
				num--;
			}
			if (optionalParameterTypes != null)
			{
				num -= optionalParameterTypes.Length;
			}
			base.UpdateStackSize(opcode, num);
			base.PutInteger4(memberRefToken);
		}

		public override void Emit(OpCode opcode, SignatureHelper signature)
		{
			if (signature == null)
			{
				throw new ArgumentNullException("signature");
			}
			int num = 0;
			base.EnsureCapacity(7);
			base.InternalEmit(opcode);
			if (opcode.StackBehaviourPop == StackBehaviour.Varpop)
			{
				num -= signature.ArgumentCount;
				num--;
				base.UpdateStackSize(opcode, num);
			}
			int tokenForSig = this.GetTokenForSig(signature.GetSignature(true));
			base.PutInteger4(tokenForSig);
		}

		public override Label BeginExceptionBlock()
		{
			return base.BeginExceptionBlock();
		}

		public override void EndExceptionBlock()
		{
			base.EndExceptionBlock();
		}

		public override void BeginExceptFilterBlock()
		{
			throw new NotSupportedException(Environment.GetResourceString("InvalidOperation_NotAllowedInDynamicMethod"));
		}

		public override void BeginCatchBlock(Type exceptionType)
		{
			if (base.CurrExcStackCount == 0)
			{
				throw new NotSupportedException(Environment.GetResourceString("Argument_NotInExceptionBlock"));
			}
			__ExceptionInfo _ExceptionInfo = base.CurrExcStack[base.CurrExcStackCount - 1];
			RuntimeType runtimeType = exceptionType as RuntimeType;
			if (_ExceptionInfo.GetCurrentState() == 1)
			{
				if (exceptionType != null)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_ShouldNotSpecifyExceptionType"));
				}
				this.Emit(OpCodes.Endfilter);
			}
			else
			{
				if (exceptionType == null)
				{
					throw new ArgumentNullException("exceptionType");
				}
				if (runtimeType == null)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"));
				}
				Label endLabel = _ExceptionInfo.GetEndLabel();
				this.Emit(OpCodes.Leave, endLabel);
				base.UpdateStackSize(OpCodes.Nop, 1);
			}
			_ExceptionInfo.MarkCatchAddr(this.ILOffset, exceptionType);
			_ExceptionInfo.m_filterAddr[_ExceptionInfo.m_currentCatch - 1] = this.GetTokenFor(runtimeType);
		}

		public override void BeginFaultBlock()
		{
			throw new NotSupportedException(Environment.GetResourceString("InvalidOperation_NotAllowedInDynamicMethod"));
		}

		public override void BeginFinallyBlock()
		{
			base.BeginFinallyBlock();
		}

		public override void UsingNamespace(string ns)
		{
			throw new NotSupportedException(Environment.GetResourceString("InvalidOperation_NotAllowedInDynamicMethod"));
		}

		public override void MarkSequencePoint(ISymbolDocumentWriter document, int startLine, int startColumn, int endLine, int endColumn)
		{
			throw new NotSupportedException(Environment.GetResourceString("InvalidOperation_NotAllowedInDynamicMethod"));
		}

		public override void BeginScope()
		{
			throw new NotSupportedException(Environment.GetResourceString("InvalidOperation_NotAllowedInDynamicMethod"));
		}

		public override void EndScope()
		{
			throw new NotSupportedException(Environment.GetResourceString("InvalidOperation_NotAllowedInDynamicMethod"));
		}

		[SecurityCritical]
		private int GetMemberRefToken(MethodBase methodInfo, Type[] optionalParameterTypes)
		{
			if (optionalParameterTypes != null && (methodInfo.CallingConvention & CallingConventions.VarArgs) == (CallingConventions)0)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NotAVarArgCallingConvention"));
			}
			RuntimeMethodInfo runtimeMethodInfo = methodInfo as RuntimeMethodInfo;
			DynamicMethod dynamicMethod = methodInfo as DynamicMethod;
			if (runtimeMethodInfo == null && dynamicMethod == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeMethodInfo"), "methodInfo");
			}
			ParameterInfo[] parametersNoCopy = methodInfo.GetParametersNoCopy();
			Type[] array;
			if (parametersNoCopy != null && parametersNoCopy.Length != 0)
			{
				array = new Type[parametersNoCopy.Length];
				for (int i = 0; i < parametersNoCopy.Length; i++)
				{
					array[i] = parametersNoCopy[i].ParameterType;
				}
			}
			else
			{
				array = null;
			}
			SignatureHelper memberRefSignature = this.GetMemberRefSignature(methodInfo.CallingConvention, MethodBuilder.GetMethodBaseReturnType(methodInfo), array, optionalParameterTypes);
			if (runtimeMethodInfo != null)
			{
				return this.GetTokenForVarArgMethod(runtimeMethodInfo, memberRefSignature);
			}
			return this.GetTokenForVarArgMethod(dynamicMethod, memberRefSignature);
		}

		[SecurityCritical]
		internal override SignatureHelper GetMemberRefSignature(CallingConventions call, Type returnType, Type[] parameterTypes, Type[] optionalParameterTypes)
		{
			int num;
			if (parameterTypes == null)
			{
				num = 0;
			}
			else
			{
				num = parameterTypes.Length;
			}
			SignatureHelper methodSigHelper = SignatureHelper.GetMethodSigHelper(call, returnType);
			for (int i = 0; i < num; i++)
			{
				methodSigHelper.AddArgument(parameterTypes[i]);
			}
			if (optionalParameterTypes != null && optionalParameterTypes.Length != 0)
			{
				methodSigHelper.AddSentinel();
				for (int i = 0; i < optionalParameterTypes.Length; i++)
				{
					methodSigHelper.AddArgument(optionalParameterTypes[i]);
				}
			}
			return methodSigHelper;
		}

		internal override void RecordTokenFixup()
		{
		}

		private int GetTokenFor(RuntimeType rtType)
		{
			if (this.ProfileAPICheck && (rtType.InvocationFlags & INVOCATION_FLAGS.INVOCATION_FLAGS_NON_W8P_FX_API) != INVOCATION_FLAGS.INVOCATION_FLAGS_UNKNOWN)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_APIInvalidForCurrentContext", new object[]
				{
					rtType.FullName
				}));
			}
			return this.m_scope.GetTokenFor(rtType.TypeHandle);
		}

		private int GetTokenFor(RuntimeFieldInfo runtimeField)
		{
			if (this.ProfileAPICheck)
			{
				RtFieldInfo rtFieldInfo = runtimeField as RtFieldInfo;
				if (rtFieldInfo != null && (rtFieldInfo.InvocationFlags & INVOCATION_FLAGS.INVOCATION_FLAGS_NON_W8P_FX_API) != INVOCATION_FLAGS.INVOCATION_FLAGS_UNKNOWN)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_APIInvalidForCurrentContext", new object[]
					{
						rtFieldInfo.FullName
					}));
				}
			}
			return this.m_scope.GetTokenFor(runtimeField.FieldHandle);
		}

		private int GetTokenFor(RuntimeFieldInfo runtimeField, RuntimeType rtType)
		{
			if (this.ProfileAPICheck)
			{
				RtFieldInfo rtFieldInfo = runtimeField as RtFieldInfo;
				if (rtFieldInfo != null && (rtFieldInfo.InvocationFlags & INVOCATION_FLAGS.INVOCATION_FLAGS_NON_W8P_FX_API) != INVOCATION_FLAGS.INVOCATION_FLAGS_UNKNOWN)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_APIInvalidForCurrentContext", new object[]
					{
						rtFieldInfo.FullName
					}));
				}
				if ((rtType.InvocationFlags & INVOCATION_FLAGS.INVOCATION_FLAGS_NON_W8P_FX_API) != INVOCATION_FLAGS.INVOCATION_FLAGS_UNKNOWN)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_APIInvalidForCurrentContext", new object[]
					{
						rtType.FullName
					}));
				}
			}
			return this.m_scope.GetTokenFor(runtimeField.FieldHandle, rtType.TypeHandle);
		}

		private int GetTokenFor(RuntimeConstructorInfo rtMeth)
		{
			if (this.ProfileAPICheck && (rtMeth.InvocationFlags & INVOCATION_FLAGS.INVOCATION_FLAGS_NON_W8P_FX_API) != INVOCATION_FLAGS.INVOCATION_FLAGS_UNKNOWN)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_APIInvalidForCurrentContext", new object[]
				{
					rtMeth.FullName
				}));
			}
			return this.m_scope.GetTokenFor(rtMeth.MethodHandle);
		}

		private int GetTokenFor(RuntimeConstructorInfo rtMeth, RuntimeType rtType)
		{
			if (this.ProfileAPICheck)
			{
				if ((rtMeth.InvocationFlags & INVOCATION_FLAGS.INVOCATION_FLAGS_NON_W8P_FX_API) != INVOCATION_FLAGS.INVOCATION_FLAGS_UNKNOWN)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_APIInvalidForCurrentContext", new object[]
					{
						rtMeth.FullName
					}));
				}
				if ((rtType.InvocationFlags & INVOCATION_FLAGS.INVOCATION_FLAGS_NON_W8P_FX_API) != INVOCATION_FLAGS.INVOCATION_FLAGS_UNKNOWN)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_APIInvalidForCurrentContext", new object[]
					{
						rtType.FullName
					}));
				}
			}
			return this.m_scope.GetTokenFor(rtMeth.MethodHandle, rtType.TypeHandle);
		}

		private int GetTokenFor(RuntimeMethodInfo rtMeth)
		{
			if (this.ProfileAPICheck && (rtMeth.InvocationFlags & INVOCATION_FLAGS.INVOCATION_FLAGS_NON_W8P_FX_API) != INVOCATION_FLAGS.INVOCATION_FLAGS_UNKNOWN)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_APIInvalidForCurrentContext", new object[]
				{
					rtMeth.FullName
				}));
			}
			return this.m_scope.GetTokenFor(rtMeth.MethodHandle);
		}

		private int GetTokenFor(RuntimeMethodInfo rtMeth, RuntimeType rtType)
		{
			if (this.ProfileAPICheck)
			{
				if ((rtMeth.InvocationFlags & INVOCATION_FLAGS.INVOCATION_FLAGS_NON_W8P_FX_API) != INVOCATION_FLAGS.INVOCATION_FLAGS_UNKNOWN)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_APIInvalidForCurrentContext", new object[]
					{
						rtMeth.FullName
					}));
				}
				if ((rtType.InvocationFlags & INVOCATION_FLAGS.INVOCATION_FLAGS_NON_W8P_FX_API) != INVOCATION_FLAGS.INVOCATION_FLAGS_UNKNOWN)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_APIInvalidForCurrentContext", new object[]
					{
						rtType.FullName
					}));
				}
			}
			return this.m_scope.GetTokenFor(rtMeth.MethodHandle, rtType.TypeHandle);
		}

		private int GetTokenFor(DynamicMethod dm)
		{
			return this.m_scope.GetTokenFor(dm);
		}

		private int GetTokenForVarArgMethod(RuntimeMethodInfo rtMeth, SignatureHelper sig)
		{
			if (this.ProfileAPICheck && (rtMeth.InvocationFlags & INVOCATION_FLAGS.INVOCATION_FLAGS_NON_W8P_FX_API) != INVOCATION_FLAGS.INVOCATION_FLAGS_UNKNOWN)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_APIInvalidForCurrentContext", new object[]
				{
					rtMeth.FullName
				}));
			}
			VarArgMethod varArgMethod = new VarArgMethod(rtMeth, sig);
			return this.m_scope.GetTokenFor(varArgMethod);
		}

		private int GetTokenForVarArgMethod(DynamicMethod dm, SignatureHelper sig)
		{
			VarArgMethod varArgMethod = new VarArgMethod(dm, sig);
			return this.m_scope.GetTokenFor(varArgMethod);
		}

		private int GetTokenForString(string s)
		{
			return this.m_scope.GetTokenFor(s);
		}

		private int GetTokenForSig(byte[] sig)
		{
			return this.m_scope.GetTokenFor(sig);
		}

		internal DynamicScope m_scope;

		private int m_methodSigToken;
	}
}
