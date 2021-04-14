﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace System.Reflection.Emit
{
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_ConstructorBuilder))]
	[ComVisible(true)]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public sealed class ConstructorBuilder : ConstructorInfo, _ConstructorBuilder
	{
		private ConstructorBuilder()
		{
		}

		[SecurityCritical]
		internal ConstructorBuilder(string name, MethodAttributes attributes, CallingConventions callingConvention, Type[] parameterTypes, Type[][] requiredCustomModifiers, Type[][] optionalCustomModifiers, ModuleBuilder mod, TypeBuilder type)
		{
			this.m_methodBuilder = new MethodBuilder(name, attributes, callingConvention, null, null, null, parameterTypes, requiredCustomModifiers, optionalCustomModifiers, mod, type, false);
			type.m_listMethods.Add(this.m_methodBuilder);
			int num;
			byte[] array = this.m_methodBuilder.GetMethodSignature().InternalGetSignature(out num);
			MethodToken token = this.m_methodBuilder.GetToken();
		}

		[SecurityCritical]
		internal ConstructorBuilder(string name, MethodAttributes attributes, CallingConventions callingConvention, Type[] parameterTypes, ModuleBuilder mod, TypeBuilder type) : this(name, attributes, callingConvention, parameterTypes, null, null, mod, type)
		{
		}

		internal override Type[] GetParameterTypes()
		{
			return this.m_methodBuilder.GetParameterTypes();
		}

		private TypeBuilder GetTypeBuilder()
		{
			return this.m_methodBuilder.GetTypeBuilder();
		}

		internal ModuleBuilder GetModuleBuilder()
		{
			return this.GetTypeBuilder().GetModuleBuilder();
		}

		public override string ToString()
		{
			return this.m_methodBuilder.ToString();
		}

		internal int MetadataTokenInternal
		{
			get
			{
				return this.m_methodBuilder.MetadataTokenInternal;
			}
		}

		public override Module Module
		{
			get
			{
				return this.m_methodBuilder.Module;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return this.m_methodBuilder.ReflectedType;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return this.m_methodBuilder.DeclaringType;
			}
		}

		public override string Name
		{
			get
			{
				return this.m_methodBuilder.Name;
			}
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw new NotSupportedException(Environment.GetResourceString("NotSupported_DynamicModule"));
		}

		public override ParameterInfo[] GetParameters()
		{
			ConstructorInfo constructor = this.GetTypeBuilder().GetConstructor(this.m_methodBuilder.m_parameterTypes);
			return constructor.GetParameters();
		}

		public override MethodAttributes Attributes
		{
			get
			{
				return this.m_methodBuilder.Attributes;
			}
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return this.m_methodBuilder.GetMethodImplementationFlags();
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get
			{
				return this.m_methodBuilder.MethodHandle;
			}
		}

		public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw new NotSupportedException(Environment.GetResourceString("NotSupported_DynamicModule"));
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return this.m_methodBuilder.GetCustomAttributes(inherit);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return this.m_methodBuilder.GetCustomAttributes(attributeType, inherit);
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return this.m_methodBuilder.IsDefined(attributeType, inherit);
		}

		public MethodToken GetToken()
		{
			return this.m_methodBuilder.GetToken();
		}

		public ParameterBuilder DefineParameter(int iSequence, ParameterAttributes attributes, string strParamName)
		{
			attributes &= ~ParameterAttributes.ReservedMask;
			return this.m_methodBuilder.DefineParameter(iSequence, attributes, strParamName);
		}

		public void SetSymCustomAttribute(string name, byte[] data)
		{
			this.m_methodBuilder.SetSymCustomAttribute(name, data);
		}

		public ILGenerator GetILGenerator()
		{
			if (this.m_isDefaultConstructor)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_DefaultConstructorILGen"));
			}
			return this.m_methodBuilder.GetILGenerator();
		}

		public ILGenerator GetILGenerator(int streamSize)
		{
			if (this.m_isDefaultConstructor)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_DefaultConstructorILGen"));
			}
			return this.m_methodBuilder.GetILGenerator(streamSize);
		}

		public void SetMethodBody(byte[] il, int maxStack, byte[] localSignature, IEnumerable<ExceptionHandler> exceptionHandlers, IEnumerable<int> tokenFixups)
		{
			if (this.m_isDefaultConstructor)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_DefaultConstructorDefineBody"));
			}
			this.m_methodBuilder.SetMethodBody(il, maxStack, localSignature, exceptionHandlers, tokenFixups);
		}

		[SecuritySafeCritical]
		public void AddDeclarativeSecurity(SecurityAction action, PermissionSet pset)
		{
			if (pset == null)
			{
				throw new ArgumentNullException("pset");
			}
			if (!Enum.IsDefined(typeof(SecurityAction), action) || action == SecurityAction.RequestMinimum || action == SecurityAction.RequestOptional || action == SecurityAction.RequestRefuse)
			{
				throw new ArgumentOutOfRangeException("action");
			}
			if (this.m_methodBuilder.IsTypeCreated())
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_TypeHasBeenCreated"));
			}
			byte[] array = pset.EncodeXml();
			TypeBuilder.AddDeclarativeSecurity(this.GetModuleBuilder().GetNativeHandle(), this.GetToken().Token, action, array, array.Length);
		}

		public override CallingConventions CallingConvention
		{
			get
			{
				if (this.DeclaringType.IsGenericType)
				{
					return CallingConventions.HasThis;
				}
				return CallingConventions.Standard;
			}
		}

		public Module GetModule()
		{
			return this.m_methodBuilder.GetModule();
		}

		[Obsolete("This property has been deprecated. http://go.microsoft.com/fwlink/?linkid=14202")]
		public Type ReturnType
		{
			get
			{
				return this.GetReturnType();
			}
		}

		internal override Type GetReturnType()
		{
			return this.m_methodBuilder.ReturnType;
		}

		public string Signature
		{
			get
			{
				return this.m_methodBuilder.Signature;
			}
		}

		[ComVisible(true)]
		public void SetCustomAttribute(ConstructorInfo con, byte[] binaryAttribute)
		{
			this.m_methodBuilder.SetCustomAttribute(con, binaryAttribute);
		}

		public void SetCustomAttribute(CustomAttributeBuilder customBuilder)
		{
			this.m_methodBuilder.SetCustomAttribute(customBuilder);
		}

		public void SetImplementationFlags(MethodImplAttributes attributes)
		{
			this.m_methodBuilder.SetImplementationFlags(attributes);
		}

		public bool InitLocals
		{
			get
			{
				return this.m_methodBuilder.InitLocals;
			}
			set
			{
				this.m_methodBuilder.InitLocals = value;
			}
		}

		void _ConstructorBuilder.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _ConstructorBuilder.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _ConstructorBuilder.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _ConstructorBuilder.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		private readonly MethodBuilder m_methodBuilder;

		internal bool m_isDefaultConstructor;
	}
}
