using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Activation;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Metadata;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;

namespace System
{
	[Serializable]
	internal class RuntimeType : System.Reflection.TypeInfo, ISerializable, ICloneable
	{
		internal RemotingTypeCachedData RemotingCache
		{
			get
			{
				RemotingTypeCachedData remotingTypeCachedData = this.m_cachedData;
				if (remotingTypeCachedData == null)
				{
					remotingTypeCachedData = new RemotingTypeCachedData(this);
					RemotingTypeCachedData remotingTypeCachedData2 = Interlocked.CompareExchange<RemotingTypeCachedData>(ref this.m_cachedData, remotingTypeCachedData, null);
					if (remotingTypeCachedData2 != null)
					{
						remotingTypeCachedData = remotingTypeCachedData2;
					}
				}
				return remotingTypeCachedData;
			}
		}

		internal static RuntimeType GetType(string typeName, bool throwOnError, bool ignoreCase, bool reflectionOnly, ref StackCrawlMark stackMark)
		{
			if (typeName == null)
			{
				throw new ArgumentNullException("typeName");
			}
			return RuntimeTypeHandle.GetTypeByName(typeName, throwOnError, ignoreCase, reflectionOnly, ref stackMark, false);
		}

		internal static MethodBase GetMethodBase(RuntimeModule scope, int typeMetadataToken)
		{
			return RuntimeType.GetMethodBase(ModuleHandle.ResolveMethodHandleInternal(scope, typeMetadataToken));
		}

		internal static MethodBase GetMethodBase(IRuntimeMethodInfo methodHandle)
		{
			return RuntimeType.GetMethodBase(null, methodHandle);
		}

		[SecuritySafeCritical]
		internal static MethodBase GetMethodBase(RuntimeType reflectedType, IRuntimeMethodInfo methodHandle)
		{
			MethodBase methodBase = RuntimeType.GetMethodBase(reflectedType, methodHandle.Value);
			GC.KeepAlive(methodHandle);
			return methodBase;
		}

		[SecurityCritical]
		internal static MethodBase GetMethodBase(RuntimeType reflectedType, RuntimeMethodHandleInternal methodHandle)
		{
			if (!RuntimeMethodHandle.IsDynamicMethod(methodHandle))
			{
				RuntimeType runtimeType = RuntimeMethodHandle.GetDeclaringType(methodHandle);
				RuntimeType[] array = null;
				if (reflectedType == null)
				{
					reflectedType = runtimeType;
				}
				if (reflectedType != runtimeType && !reflectedType.IsSubclassOf(runtimeType))
				{
					if (reflectedType.IsArray)
					{
						MethodBase[] array2 = reflectedType.GetMember(RuntimeMethodHandle.GetName(methodHandle), MemberTypes.Constructor | MemberTypes.Method, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) as MethodBase[];
						bool flag = false;
						foreach (IRuntimeMethodInfo runtimeMethodInfo in array2)
						{
							if (runtimeMethodInfo.Value.Value == methodHandle.Value)
							{
								flag = true;
							}
						}
						if (!flag)
						{
							throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveMethodHandle"), reflectedType.ToString(), runtimeType.ToString()));
						}
					}
					else if (runtimeType.IsGenericType)
					{
						RuntimeType right = (RuntimeType)runtimeType.GetGenericTypeDefinition();
						RuntimeType runtimeType2 = reflectedType;
						while (runtimeType2 != null)
						{
							RuntimeType runtimeType3 = runtimeType2;
							if (runtimeType3.IsGenericType && !runtimeType2.IsGenericTypeDefinition)
							{
								runtimeType3 = (RuntimeType)runtimeType3.GetGenericTypeDefinition();
							}
							if (runtimeType3 == right)
							{
								break;
							}
							runtimeType2 = runtimeType2.GetBaseType();
						}
						if (runtimeType2 == null)
						{
							throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveMethodHandle"), reflectedType.ToString(), runtimeType.ToString()));
						}
						runtimeType = runtimeType2;
						if (!RuntimeMethodHandle.IsGenericMethodDefinition(methodHandle))
						{
							array = RuntimeMethodHandle.GetMethodInstantiationInternal(methodHandle);
						}
						methodHandle = RuntimeMethodHandle.GetMethodFromCanonical(methodHandle, runtimeType);
					}
					else if (!runtimeType.IsAssignableFrom(reflectedType))
					{
						throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveMethodHandle"), reflectedType.ToString(), runtimeType.ToString()));
					}
				}
				methodHandle = RuntimeMethodHandle.GetStubIfNeeded(methodHandle, runtimeType, array);
				MethodBase result;
				if (RuntimeMethodHandle.IsConstructor(methodHandle))
				{
					result = reflectedType.Cache.GetConstructor(runtimeType, methodHandle);
				}
				else if (RuntimeMethodHandle.HasMethodInstantiation(methodHandle) && !RuntimeMethodHandle.IsGenericMethodDefinition(methodHandle))
				{
					result = reflectedType.Cache.GetGenericMethodInfo(methodHandle);
				}
				else
				{
					result = reflectedType.Cache.GetMethod(runtimeType, methodHandle);
				}
				GC.KeepAlive(array);
				return result;
			}
			Resolver resolver = RuntimeMethodHandle.GetResolver(methodHandle);
			if (resolver != null)
			{
				return resolver.GetDynamicMethod();
			}
			return null;
		}

		internal object GenericCache
		{
			get
			{
				return this.Cache.GenericCache;
			}
			set
			{
				this.Cache.GenericCache = value;
			}
		}

		internal bool DomainInitialized
		{
			get
			{
				return this.Cache.DomainInitialized;
			}
			set
			{
				this.Cache.DomainInitialized = value;
			}
		}

		[SecuritySafeCritical]
		internal static FieldInfo GetFieldInfo(IRuntimeFieldInfo fieldHandle)
		{
			return RuntimeType.GetFieldInfo(RuntimeFieldHandle.GetApproxDeclaringType(fieldHandle), fieldHandle);
		}

		[SecuritySafeCritical]
		internal static FieldInfo GetFieldInfo(RuntimeType reflectedType, IRuntimeFieldInfo field)
		{
			RuntimeFieldHandleInternal value = field.Value;
			if (reflectedType == null)
			{
				reflectedType = RuntimeFieldHandle.GetApproxDeclaringType(value);
			}
			else
			{
				RuntimeType approxDeclaringType = RuntimeFieldHandle.GetApproxDeclaringType(value);
				if (reflectedType != approxDeclaringType && (!RuntimeFieldHandle.AcquiresContextFromThis(value) || !RuntimeTypeHandle.CompareCanonicalHandles(approxDeclaringType, reflectedType)))
				{
					throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveFieldHandle"), reflectedType.ToString(), approxDeclaringType.ToString()));
				}
			}
			FieldInfo field2 = reflectedType.Cache.GetField(value);
			GC.KeepAlive(field);
			return field2;
		}

		private static PropertyInfo GetPropertyInfo(RuntimeType reflectedType, int tkProperty)
		{
			foreach (RuntimePropertyInfo runtimePropertyInfo in reflectedType.Cache.GetPropertyList(RuntimeType.MemberListType.All, null))
			{
				if (runtimePropertyInfo.MetadataToken == tkProperty)
				{
					return runtimePropertyInfo;
				}
			}
			throw new SystemException();
		}

		private static void ThrowIfTypeNeverValidGenericArgument(RuntimeType type)
		{
			if (type.IsPointer || type.IsByRef || type == typeof(void))
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_NeverValidGenericArgument", new object[]
				{
					type.ToString()
				}));
			}
		}

		internal static void SanityCheckGenericArguments(RuntimeType[] genericArguments, RuntimeType[] genericParamters)
		{
			if (genericArguments == null)
			{
				throw new ArgumentNullException();
			}
			for (int i = 0; i < genericArguments.Length; i++)
			{
				if (genericArguments[i] == null)
				{
					throw new ArgumentNullException();
				}
				RuntimeType.ThrowIfTypeNeverValidGenericArgument(genericArguments[i]);
			}
			if (genericArguments.Length != genericParamters.Length)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_NotEnoughGenArguments", new object[]
				{
					genericArguments.Length,
					genericParamters.Length
				}));
			}
		}

		[SecuritySafeCritical]
		internal static void ValidateGenericArguments(MemberInfo definition, RuntimeType[] genericArguments, Exception e)
		{
			RuntimeType[] typeContext = null;
			RuntimeType[] methodContext = null;
			RuntimeType[] genericArgumentsInternal;
			if (definition is Type)
			{
				RuntimeType runtimeType = (RuntimeType)definition;
				genericArgumentsInternal = runtimeType.GetGenericArgumentsInternal();
				typeContext = genericArguments;
			}
			else
			{
				RuntimeMethodInfo runtimeMethodInfo = (RuntimeMethodInfo)definition;
				genericArgumentsInternal = runtimeMethodInfo.GetGenericArgumentsInternal();
				methodContext = genericArguments;
				RuntimeType runtimeType2 = (RuntimeType)runtimeMethodInfo.DeclaringType;
				if (runtimeType2 != null)
				{
					typeContext = runtimeType2.GetTypeHandleInternal().GetInstantiationInternal();
				}
			}
			for (int i = 0; i < genericArguments.Length; i++)
			{
				Type type = genericArguments[i];
				Type type2 = genericArgumentsInternal[i];
				if (!RuntimeTypeHandle.SatisfiesConstraints(type2.GetTypeHandleInternal().GetTypeChecked(), typeContext, methodContext, type.GetTypeHandleInternal().GetTypeChecked()))
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_GenConstraintViolation", new object[]
					{
						i.ToString(CultureInfo.CurrentCulture),
						type.ToString(),
						definition.ToString(),
						type2.ToString()
					}), e);
				}
			}
		}

		private static void SplitName(string fullname, out string name, out string ns)
		{
			name = null;
			ns = null;
			if (fullname == null)
			{
				return;
			}
			int num = fullname.LastIndexOf(".", StringComparison.Ordinal);
			if (num == -1)
			{
				name = fullname;
				return;
			}
			ns = fullname.Substring(0, num);
			int num2 = fullname.Length - ns.Length - 1;
			if (num2 != 0)
			{
				name = fullname.Substring(num + 1, num2);
				return;
			}
			name = "";
		}

		internal static BindingFlags FilterPreCalculate(bool isPublic, bool isInherited, bool isStatic)
		{
			BindingFlags bindingFlags = isPublic ? BindingFlags.Public : BindingFlags.NonPublic;
			if (isInherited)
			{
				bindingFlags |= BindingFlags.DeclaredOnly;
				if (isStatic)
				{
					bindingFlags |= (BindingFlags.Static | BindingFlags.FlattenHierarchy);
				}
				else
				{
					bindingFlags |= BindingFlags.Instance;
				}
			}
			else if (isStatic)
			{
				bindingFlags |= BindingFlags.Static;
			}
			else
			{
				bindingFlags |= BindingFlags.Instance;
			}
			return bindingFlags;
		}

		private static void FilterHelper(BindingFlags bindingFlags, ref string name, bool allowPrefixLookup, out bool prefixLookup, out bool ignoreCase, out RuntimeType.MemberListType listType)
		{
			prefixLookup = false;
			ignoreCase = false;
			if (name != null)
			{
				if ((bindingFlags & BindingFlags.IgnoreCase) != BindingFlags.Default)
				{
					name = name.ToLower(CultureInfo.InvariantCulture);
					ignoreCase = true;
					listType = RuntimeType.MemberListType.CaseInsensitive;
				}
				else
				{
					listType = RuntimeType.MemberListType.CaseSensitive;
				}
				if (allowPrefixLookup && name.EndsWith("*", StringComparison.Ordinal))
				{
					name = name.Substring(0, name.Length - 1);
					prefixLookup = true;
					listType = RuntimeType.MemberListType.All;
					return;
				}
			}
			else
			{
				listType = RuntimeType.MemberListType.All;
			}
		}

		private static void FilterHelper(BindingFlags bindingFlags, ref string name, out bool ignoreCase, out RuntimeType.MemberListType listType)
		{
			bool flag;
			RuntimeType.FilterHelper(bindingFlags, ref name, false, out flag, out ignoreCase, out listType);
		}

		private static bool FilterApplyPrefixLookup(MemberInfo memberInfo, string name, bool ignoreCase)
		{
			if (ignoreCase)
			{
				if (!memberInfo.Name.StartsWith(name, StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
			}
			else if (!memberInfo.Name.StartsWith(name, StringComparison.Ordinal))
			{
				return false;
			}
			return true;
		}

		private static bool FilterApplyBase(MemberInfo memberInfo, BindingFlags bindingFlags, bool isPublic, bool isNonProtectedInternal, bool isStatic, string name, bool prefixLookup)
		{
			if (isPublic)
			{
				if ((bindingFlags & BindingFlags.Public) == BindingFlags.Default)
				{
					return false;
				}
			}
			else if ((bindingFlags & BindingFlags.NonPublic) == BindingFlags.Default)
			{
				return false;
			}
			bool flag = memberInfo.DeclaringType != memberInfo.ReflectedType;
			if ((bindingFlags & BindingFlags.DeclaredOnly) > BindingFlags.Default && flag)
			{
				return false;
			}
			if (memberInfo.MemberType != MemberTypes.TypeInfo && memberInfo.MemberType != MemberTypes.NestedType)
			{
				if (isStatic)
				{
					if ((bindingFlags & BindingFlags.FlattenHierarchy) == BindingFlags.Default && flag)
					{
						return false;
					}
					if ((bindingFlags & BindingFlags.Static) == BindingFlags.Default)
					{
						return false;
					}
				}
				else if ((bindingFlags & BindingFlags.Instance) == BindingFlags.Default)
				{
					return false;
				}
			}
			if (prefixLookup && !RuntimeType.FilterApplyPrefixLookup(memberInfo, name, (bindingFlags & BindingFlags.IgnoreCase) > BindingFlags.Default))
			{
				return false;
			}
			if ((bindingFlags & BindingFlags.DeclaredOnly) == BindingFlags.Default && flag && isNonProtectedInternal && (bindingFlags & BindingFlags.NonPublic) != BindingFlags.Default && !isStatic && (bindingFlags & BindingFlags.Instance) != BindingFlags.Default)
			{
				MethodInfo methodInfo = memberInfo as MethodInfo;
				if (methodInfo == null)
				{
					return false;
				}
				if (!methodInfo.IsVirtual && !methodInfo.IsAbstract)
				{
					return false;
				}
			}
			return true;
		}

		private static bool FilterApplyType(Type type, BindingFlags bindingFlags, string name, bool prefixLookup, string ns)
		{
			bool isPublic = type.IsNestedPublic || type.IsPublic;
			bool isStatic = false;
			return RuntimeType.FilterApplyBase(type, bindingFlags, isPublic, type.IsNestedAssembly, isStatic, name, prefixLookup) && (ns == null || type.Namespace.Equals(ns));
		}

		private static bool FilterApplyMethodInfo(RuntimeMethodInfo method, BindingFlags bindingFlags, CallingConventions callConv, Type[] argumentTypes)
		{
			return RuntimeType.FilterApplyMethodBase(method, method.BindingFlags, bindingFlags, callConv, argumentTypes);
		}

		private static bool FilterApplyConstructorInfo(RuntimeConstructorInfo constructor, BindingFlags bindingFlags, CallingConventions callConv, Type[] argumentTypes)
		{
			return RuntimeType.FilterApplyMethodBase(constructor, constructor.BindingFlags, bindingFlags, callConv, argumentTypes);
		}

		private static bool FilterApplyMethodBase(MethodBase methodBase, BindingFlags methodFlags, BindingFlags bindingFlags, CallingConventions callConv, Type[] argumentTypes)
		{
			bindingFlags ^= BindingFlags.DeclaredOnly;
			if ((bindingFlags & methodFlags) != methodFlags)
			{
				return false;
			}
			if ((callConv & CallingConventions.Any) == (CallingConventions)0)
			{
				if ((callConv & CallingConventions.VarArgs) != (CallingConventions)0 && (methodBase.CallingConvention & CallingConventions.VarArgs) == (CallingConventions)0)
				{
					return false;
				}
				if ((callConv & CallingConventions.Standard) != (CallingConventions)0 && (methodBase.CallingConvention & CallingConventions.Standard) == (CallingConventions)0)
				{
					return false;
				}
			}
			if (argumentTypes != null)
			{
				ParameterInfo[] parametersNoCopy = methodBase.GetParametersNoCopy();
				if (argumentTypes.Length != parametersNoCopy.Length)
				{
					if ((bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetProperty | BindingFlags.SetProperty)) == BindingFlags.Default)
					{
						return false;
					}
					bool flag = false;
					bool flag2 = argumentTypes.Length > parametersNoCopy.Length;
					if (flag2)
					{
						if ((methodBase.CallingConvention & CallingConventions.VarArgs) == (CallingConventions)0)
						{
							flag = true;
						}
					}
					else if ((bindingFlags & BindingFlags.OptionalParamBinding) == BindingFlags.Default)
					{
						flag = true;
					}
					else if (!parametersNoCopy[argumentTypes.Length].IsOptional)
					{
						flag = true;
					}
					if (flag)
					{
						if (parametersNoCopy.Length == 0)
						{
							return false;
						}
						bool flag3 = argumentTypes.Length < parametersNoCopy.Length - 1;
						if (flag3)
						{
							return false;
						}
						ParameterInfo parameterInfo = parametersNoCopy[parametersNoCopy.Length - 1];
						if (!parameterInfo.ParameterType.IsArray)
						{
							return false;
						}
						if (!parameterInfo.IsDefined(typeof(ParamArrayAttribute), false))
						{
							return false;
						}
					}
				}
				else if ((bindingFlags & BindingFlags.ExactBinding) != BindingFlags.Default && (bindingFlags & BindingFlags.InvokeMethod) == BindingFlags.Default)
				{
					for (int i = 0; i < parametersNoCopy.Length; i++)
					{
						if (argumentTypes[i] != null && parametersNoCopy[i].ParameterType != argumentTypes[i])
						{
							return false;
						}
					}
				}
			}
			return true;
		}

		internal bool IsNonW8PFrameworkAPI()
		{
			if (this.IsGenericParameter)
			{
				return false;
			}
			if (base.HasElementType)
			{
				return ((RuntimeType)this.GetElementType()).IsNonW8PFrameworkAPI();
			}
			if (this.IsSimpleTypeNonW8PFrameworkAPI())
			{
				return true;
			}
			if (this.IsGenericType && !this.IsGenericTypeDefinition)
			{
				foreach (Type type in this.GetGenericArguments())
				{
					if (((RuntimeType)type).IsNonW8PFrameworkAPI())
					{
						return true;
					}
				}
			}
			return false;
		}

		private bool IsSimpleTypeNonW8PFrameworkAPI()
		{
			RuntimeAssembly runtimeAssembly = this.GetRuntimeAssembly();
			if (runtimeAssembly.IsFrameworkAssembly())
			{
				int invocableAttributeCtorToken = runtimeAssembly.InvocableAttributeCtorToken;
				if (System.Reflection.MetadataToken.IsNullToken(invocableAttributeCtorToken) || !CustomAttribute.IsAttributeDefined(this.GetRuntimeModule(), this.MetadataToken, invocableAttributeCtorToken))
				{
					return true;
				}
			}
			return false;
		}

		internal INVOCATION_FLAGS InvocationFlags
		{
			get
			{
				if ((this.m_invocationFlags & INVOCATION_FLAGS.INVOCATION_FLAGS_INITIALIZED) == INVOCATION_FLAGS.INVOCATION_FLAGS_UNKNOWN)
				{
					INVOCATION_FLAGS invocation_FLAGS = INVOCATION_FLAGS.INVOCATION_FLAGS_UNKNOWN;
					if (AppDomain.ProfileAPICheck && this.IsNonW8PFrameworkAPI())
					{
						invocation_FLAGS |= INVOCATION_FLAGS.INVOCATION_FLAGS_NON_W8P_FX_API;
					}
					this.m_invocationFlags = (invocation_FLAGS | INVOCATION_FLAGS.INVOCATION_FLAGS_INITIALIZED);
				}
				return this.m_invocationFlags;
			}
		}

		internal RuntimeType()
		{
			throw new NotSupportedException();
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal override bool CacheEquals(object o)
		{
			RuntimeType runtimeType = o as RuntimeType;
			return !(runtimeType == null) && runtimeType.m_handle.Equals(this.m_handle);
		}

		private RuntimeType.RuntimeTypeCache Cache
		{
			[SecuritySafeCritical]
			get
			{
				if (this.m_cache.IsNull())
				{
					IntPtr gchandle = new RuntimeTypeHandle(this).GetGCHandle(GCHandleType.WeakTrackResurrection);
					if (!Interlocked.CompareExchange(ref this.m_cache, gchandle, (IntPtr)0).IsNull() && !this.IsCollectible())
					{
						GCHandle.InternalFree(gchandle);
					}
				}
				RuntimeType.RuntimeTypeCache runtimeTypeCache = GCHandle.InternalGet(this.m_cache) as RuntimeType.RuntimeTypeCache;
				if (runtimeTypeCache == null)
				{
					runtimeTypeCache = new RuntimeType.RuntimeTypeCache(this);
					RuntimeType.RuntimeTypeCache runtimeTypeCache2 = GCHandle.InternalCompareExchange(this.m_cache, runtimeTypeCache, null, false) as RuntimeType.RuntimeTypeCache;
					if (runtimeTypeCache2 != null)
					{
						runtimeTypeCache = runtimeTypeCache2;
					}
				}
				return runtimeTypeCache;
			}
		}

		internal bool IsSpecialSerializableType()
		{
			RuntimeType runtimeType = this;
			while (!(runtimeType == RuntimeType.DelegateType) && !(runtimeType == RuntimeType.EnumType))
			{
				runtimeType = runtimeType.GetBaseType();
				if (!(runtimeType != null))
				{
					return false;
				}
			}
			return true;
		}

		private string GetDefaultMemberName()
		{
			return this.Cache.GetDefaultMemberName();
		}

		internal RuntimeConstructorInfo GetSerializationCtor()
		{
			return this.Cache.GetSerializationCtor();
		}

		private RuntimeType.ListBuilder<MethodInfo> GetMethodCandidates(string name, BindingFlags bindingAttr, CallingConventions callConv, Type[] types, bool allowPrefixLookup)
		{
			bool flag;
			bool ignoreCase;
			RuntimeType.MemberListType listType;
			RuntimeType.FilterHelper(bindingAttr, ref name, allowPrefixLookup, out flag, out ignoreCase, out listType);
			RuntimeMethodInfo[] methodList = this.Cache.GetMethodList(listType, name);
			RuntimeType.ListBuilder<MethodInfo> result = new RuntimeType.ListBuilder<MethodInfo>(methodList.Length);
			foreach (RuntimeMethodInfo runtimeMethodInfo in methodList)
			{
				if (RuntimeType.FilterApplyMethodInfo(runtimeMethodInfo, bindingAttr, callConv, types) && (!flag || RuntimeType.FilterApplyPrefixLookup(runtimeMethodInfo, name, ignoreCase)))
				{
					result.Add(runtimeMethodInfo);
				}
			}
			return result;
		}

		private RuntimeType.ListBuilder<ConstructorInfo> GetConstructorCandidates(string name, BindingFlags bindingAttr, CallingConventions callConv, Type[] types, bool allowPrefixLookup)
		{
			bool flag;
			bool ignoreCase;
			RuntimeType.MemberListType listType;
			RuntimeType.FilterHelper(bindingAttr, ref name, allowPrefixLookup, out flag, out ignoreCase, out listType);
			RuntimeConstructorInfo[] constructorList = this.Cache.GetConstructorList(listType, name);
			RuntimeType.ListBuilder<ConstructorInfo> result = new RuntimeType.ListBuilder<ConstructorInfo>(constructorList.Length);
			foreach (RuntimeConstructorInfo runtimeConstructorInfo in constructorList)
			{
				if (RuntimeType.FilterApplyConstructorInfo(runtimeConstructorInfo, bindingAttr, callConv, types) && (!flag || RuntimeType.FilterApplyPrefixLookup(runtimeConstructorInfo, name, ignoreCase)))
				{
					result.Add(runtimeConstructorInfo);
				}
			}
			return result;
		}

		private RuntimeType.ListBuilder<PropertyInfo> GetPropertyCandidates(string name, BindingFlags bindingAttr, Type[] types, bool allowPrefixLookup)
		{
			bool flag;
			bool ignoreCase;
			RuntimeType.MemberListType listType;
			RuntimeType.FilterHelper(bindingAttr, ref name, allowPrefixLookup, out flag, out ignoreCase, out listType);
			RuntimePropertyInfo[] propertyList = this.Cache.GetPropertyList(listType, name);
			bindingAttr ^= BindingFlags.DeclaredOnly;
			RuntimeType.ListBuilder<PropertyInfo> result = new RuntimeType.ListBuilder<PropertyInfo>(propertyList.Length);
			foreach (RuntimePropertyInfo runtimePropertyInfo in propertyList)
			{
				if ((bindingAttr & runtimePropertyInfo.BindingFlags) == runtimePropertyInfo.BindingFlags && (!flag || RuntimeType.FilterApplyPrefixLookup(runtimePropertyInfo, name, ignoreCase)) && (types == null || runtimePropertyInfo.GetIndexParameters().Length == types.Length))
				{
					result.Add(runtimePropertyInfo);
				}
			}
			return result;
		}

		private RuntimeType.ListBuilder<EventInfo> GetEventCandidates(string name, BindingFlags bindingAttr, bool allowPrefixLookup)
		{
			bool flag;
			bool ignoreCase;
			RuntimeType.MemberListType listType;
			RuntimeType.FilterHelper(bindingAttr, ref name, allowPrefixLookup, out flag, out ignoreCase, out listType);
			RuntimeEventInfo[] eventList = this.Cache.GetEventList(listType, name);
			bindingAttr ^= BindingFlags.DeclaredOnly;
			RuntimeType.ListBuilder<EventInfo> result = new RuntimeType.ListBuilder<EventInfo>(eventList.Length);
			foreach (RuntimeEventInfo runtimeEventInfo in eventList)
			{
				if ((bindingAttr & runtimeEventInfo.BindingFlags) == runtimeEventInfo.BindingFlags && (!flag || RuntimeType.FilterApplyPrefixLookup(runtimeEventInfo, name, ignoreCase)))
				{
					result.Add(runtimeEventInfo);
				}
			}
			return result;
		}

		private RuntimeType.ListBuilder<FieldInfo> GetFieldCandidates(string name, BindingFlags bindingAttr, bool allowPrefixLookup)
		{
			bool flag;
			bool ignoreCase;
			RuntimeType.MemberListType listType;
			RuntimeType.FilterHelper(bindingAttr, ref name, allowPrefixLookup, out flag, out ignoreCase, out listType);
			RuntimeFieldInfo[] fieldList = this.Cache.GetFieldList(listType, name);
			bindingAttr ^= BindingFlags.DeclaredOnly;
			RuntimeType.ListBuilder<FieldInfo> result = new RuntimeType.ListBuilder<FieldInfo>(fieldList.Length);
			foreach (RuntimeFieldInfo runtimeFieldInfo in fieldList)
			{
				if ((bindingAttr & runtimeFieldInfo.BindingFlags) == runtimeFieldInfo.BindingFlags && (!flag || RuntimeType.FilterApplyPrefixLookup(runtimeFieldInfo, name, ignoreCase)))
				{
					result.Add(runtimeFieldInfo);
				}
			}
			return result;
		}

		private RuntimeType.ListBuilder<Type> GetNestedTypeCandidates(string fullname, BindingFlags bindingAttr, bool allowPrefixLookup)
		{
			bindingAttr &= ~BindingFlags.Static;
			string name;
			string ns;
			RuntimeType.SplitName(fullname, out name, out ns);
			bool prefixLookup;
			bool flag;
			RuntimeType.MemberListType listType;
			RuntimeType.FilterHelper(bindingAttr, ref name, allowPrefixLookup, out prefixLookup, out flag, out listType);
			RuntimeType[] nestedTypeList = this.Cache.GetNestedTypeList(listType, name);
			RuntimeType.ListBuilder<Type> result = new RuntimeType.ListBuilder<Type>(nestedTypeList.Length);
			foreach (RuntimeType runtimeType in nestedTypeList)
			{
				if (RuntimeType.FilterApplyType(runtimeType, bindingAttr, name, prefixLookup, ns))
				{
					result.Add(runtimeType);
				}
			}
			return result;
		}

		public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
		{
			return this.GetMethodCandidates(null, bindingAttr, CallingConventions.Any, null, false).ToArray();
		}

		[ComVisible(true)]
		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			ConstructorInfo[] array = this.GetConstructorCandidates(null, bindingAttr, CallingConventions.Any, null, false).ToArray();
			if (!this.IsDoNotForceOrderOfConstructorsSetImpl() && !this.IsArrayImpl() && this.IsZappedImpl())
			{
				ArraySortHelper<ConstructorInfo>.IntrospectiveSort(array, 0, array.Length, RuntimeType.ConstructorInfoComparer.SortByMetadataToken);
			}
			return array;
		}

		public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
		{
			return this.GetPropertyCandidates(null, bindingAttr, null, false).ToArray();
		}

		public override EventInfo[] GetEvents(BindingFlags bindingAttr)
		{
			return this.GetEventCandidates(null, bindingAttr, false).ToArray();
		}

		public override FieldInfo[] GetFields(BindingFlags bindingAttr)
		{
			return this.GetFieldCandidates(null, bindingAttr, false).ToArray();
		}

		[SecuritySafeCritical]
		public override Type[] GetInterfaces()
		{
			RuntimeType[] interfaceList = this.Cache.GetInterfaceList(RuntimeType.MemberListType.All, null);
			Type[] array = new Type[interfaceList.Length];
			for (int i = 0; i < interfaceList.Length; i++)
			{
				JitHelpers.UnsafeSetArrayElement(array, i, interfaceList[i]);
			}
			return array;
		}

		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			return this.GetNestedTypeCandidates(null, bindingAttr, false).ToArray();
		}

		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			RuntimeType.ListBuilder<MethodInfo> methodCandidates = this.GetMethodCandidates(null, bindingAttr, CallingConventions.Any, null, false);
			RuntimeType.ListBuilder<ConstructorInfo> constructorCandidates = this.GetConstructorCandidates(null, bindingAttr, CallingConventions.Any, null, false);
			RuntimeType.ListBuilder<PropertyInfo> propertyCandidates = this.GetPropertyCandidates(null, bindingAttr, null, false);
			RuntimeType.ListBuilder<EventInfo> eventCandidates = this.GetEventCandidates(null, bindingAttr, false);
			RuntimeType.ListBuilder<FieldInfo> fieldCandidates = this.GetFieldCandidates(null, bindingAttr, false);
			RuntimeType.ListBuilder<Type> nestedTypeCandidates = this.GetNestedTypeCandidates(null, bindingAttr, false);
			MemberInfo[] array = new MemberInfo[methodCandidates.Count + constructorCandidates.Count + propertyCandidates.Count + eventCandidates.Count + fieldCandidates.Count + nestedTypeCandidates.Count];
			int num = 0;
			methodCandidates.CopyTo(array, num);
			num += methodCandidates.Count;
			constructorCandidates.CopyTo(array, num);
			num += constructorCandidates.Count;
			propertyCandidates.CopyTo(array, num);
			num += propertyCandidates.Count;
			eventCandidates.CopyTo(array, num);
			num += eventCandidates.Count;
			fieldCandidates.CopyTo(array, num);
			num += fieldCandidates.Count;
			nestedTypeCandidates.CopyTo(array, num);
			num += nestedTypeCandidates.Count;
			return array;
		}

		[SecuritySafeCritical]
		public override InterfaceMapping GetInterfaceMap(Type ifaceType)
		{
			if (this.IsGenericParameter)
			{
				throw new InvalidOperationException(Environment.GetResourceString("Arg_GenericParameter"));
			}
			if (ifaceType == null)
			{
				throw new ArgumentNullException("ifaceType");
			}
			RuntimeType runtimeType = ifaceType as RuntimeType;
			if (runtimeType == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_MustBeRuntimeType"), "ifaceType");
			}
			RuntimeTypeHandle typeHandleInternal = runtimeType.GetTypeHandleInternal();
			this.GetTypeHandleInternal().VerifyInterfaceIsImplemented(typeHandleInternal);
			if (this.IsSzArray && ifaceType.IsGenericType)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_ArrayGetInterfaceMap"));
			}
			int numVirtuals = RuntimeTypeHandle.GetNumVirtuals(runtimeType);
			InterfaceMapping interfaceMapping;
			interfaceMapping.InterfaceType = ifaceType;
			interfaceMapping.TargetType = this;
			interfaceMapping.InterfaceMethods = new MethodInfo[numVirtuals];
			interfaceMapping.TargetMethods = new MethodInfo[numVirtuals];
			for (int i = 0; i < numVirtuals; i++)
			{
				RuntimeMethodHandleInternal methodAt = RuntimeTypeHandle.GetMethodAt(runtimeType, i);
				MethodBase methodBase = RuntimeType.GetMethodBase(runtimeType, methodAt);
				interfaceMapping.InterfaceMethods[i] = (MethodInfo)methodBase;
				int interfaceMethodImplementationSlot = this.GetTypeHandleInternal().GetInterfaceMethodImplementationSlot(typeHandleInternal, methodAt);
				if (interfaceMethodImplementationSlot != -1)
				{
					RuntimeMethodHandleInternal methodAt2 = RuntimeTypeHandle.GetMethodAt(this, interfaceMethodImplementationSlot);
					MethodBase methodBase2 = RuntimeType.GetMethodBase(this, methodAt2);
					interfaceMapping.TargetMethods[i] = (MethodInfo)methodBase2;
				}
			}
			return interfaceMapping;
		}

		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConv, Type[] types, ParameterModifier[] modifiers)
		{
			RuntimeType.ListBuilder<MethodInfo> methodCandidates = this.GetMethodCandidates(name, bindingAttr, callConv, types, false);
			if (methodCandidates.Count == 0)
			{
				return null;
			}
			if (types == null || types.Length == 0)
			{
				MethodInfo methodInfo = methodCandidates[0];
				if (methodCandidates.Count == 1)
				{
					return methodInfo;
				}
				if (types == null)
				{
					for (int i = 1; i < methodCandidates.Count; i++)
					{
						MethodInfo m = methodCandidates[i];
						if (!System.DefaultBinder.CompareMethodSigAndName(m, methodInfo))
						{
							throw new AmbiguousMatchException(Environment.GetResourceString("Arg_AmbiguousMatchException"));
						}
					}
					return System.DefaultBinder.FindMostDerivedNewSlotMeth(methodCandidates.ToArray(), methodCandidates.Count) as MethodInfo;
				}
			}
			if (binder == null)
			{
				binder = Type.DefaultBinder;
			}
			return binder.SelectMethod(bindingAttr, methodCandidates.ToArray(), types, modifiers) as MethodInfo;
		}

		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			RuntimeType.ListBuilder<ConstructorInfo> constructorCandidates = this.GetConstructorCandidates(null, bindingAttr, CallingConventions.Any, types, false);
			if (constructorCandidates.Count == 0)
			{
				return null;
			}
			if (types.Length == 0 && constructorCandidates.Count == 1)
			{
				ConstructorInfo constructorInfo = constructorCandidates[0];
				ParameterInfo[] parametersNoCopy = constructorInfo.GetParametersNoCopy();
				if (parametersNoCopy == null || parametersNoCopy.Length == 0)
				{
					return constructorInfo;
				}
			}
			if ((bindingAttr & BindingFlags.ExactBinding) != BindingFlags.Default)
			{
				return System.DefaultBinder.ExactBinding(constructorCandidates.ToArray(), types, modifiers) as ConstructorInfo;
			}
			if (binder == null)
			{
				binder = Type.DefaultBinder;
			}
			return binder.SelectMethod(bindingAttr, constructorCandidates.ToArray(), types, modifiers) as ConstructorInfo;
		}

		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			if (name == null)
			{
				throw new ArgumentNullException();
			}
			RuntimeType.ListBuilder<PropertyInfo> propertyCandidates = this.GetPropertyCandidates(name, bindingAttr, types, false);
			if (propertyCandidates.Count == 0)
			{
				return null;
			}
			if (types == null || types.Length == 0)
			{
				if (propertyCandidates.Count == 1)
				{
					PropertyInfo propertyInfo = propertyCandidates[0];
					if (returnType != null && !returnType.IsEquivalentTo(propertyInfo.PropertyType))
					{
						return null;
					}
					return propertyInfo;
				}
				else if (returnType == null)
				{
					throw new AmbiguousMatchException(Environment.GetResourceString("Arg_AmbiguousMatchException"));
				}
			}
			if ((bindingAttr & BindingFlags.ExactBinding) != BindingFlags.Default)
			{
				return System.DefaultBinder.ExactPropertyBinding(propertyCandidates.ToArray(), returnType, types, modifiers);
			}
			if (binder == null)
			{
				binder = Type.DefaultBinder;
			}
			return binder.SelectProperty(bindingAttr, propertyCandidates.ToArray(), returnType, types, modifiers);
		}

		public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
		{
			if (name == null)
			{
				throw new ArgumentNullException();
			}
			bool flag;
			RuntimeType.MemberListType listType;
			RuntimeType.FilterHelper(bindingAttr, ref name, out flag, out listType);
			RuntimeEventInfo[] eventList = this.Cache.GetEventList(listType, name);
			EventInfo eventInfo = null;
			bindingAttr ^= BindingFlags.DeclaredOnly;
			foreach (RuntimeEventInfo runtimeEventInfo in eventList)
			{
				if ((bindingAttr & runtimeEventInfo.BindingFlags) == runtimeEventInfo.BindingFlags)
				{
					if (eventInfo != null)
					{
						throw new AmbiguousMatchException(Environment.GetResourceString("Arg_AmbiguousMatchException"));
					}
					eventInfo = runtimeEventInfo;
				}
			}
			return eventInfo;
		}

		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			if (name == null)
			{
				throw new ArgumentNullException();
			}
			bool flag;
			RuntimeType.MemberListType listType;
			RuntimeType.FilterHelper(bindingAttr, ref name, out flag, out listType);
			RuntimeFieldInfo[] fieldList = this.Cache.GetFieldList(listType, name);
			FieldInfo fieldInfo = null;
			bindingAttr ^= BindingFlags.DeclaredOnly;
			bool flag2 = false;
			foreach (RuntimeFieldInfo runtimeFieldInfo in fieldList)
			{
				if ((bindingAttr & runtimeFieldInfo.BindingFlags) == runtimeFieldInfo.BindingFlags)
				{
					if (fieldInfo != null)
					{
						if (runtimeFieldInfo.DeclaringType == fieldInfo.DeclaringType)
						{
							throw new AmbiguousMatchException(Environment.GetResourceString("Arg_AmbiguousMatchException"));
						}
						if (fieldInfo.DeclaringType.IsInterface && runtimeFieldInfo.DeclaringType.IsInterface)
						{
							flag2 = true;
						}
					}
					if (fieldInfo == null || runtimeFieldInfo.DeclaringType.IsSubclassOf(fieldInfo.DeclaringType) || fieldInfo.DeclaringType.IsInterface)
					{
						fieldInfo = runtimeFieldInfo;
					}
				}
			}
			if (flag2 && fieldInfo.DeclaringType.IsInterface)
			{
				throw new AmbiguousMatchException(Environment.GetResourceString("Arg_AmbiguousMatchException"));
			}
			return fieldInfo;
		}

		public override Type GetInterface(string fullname, bool ignoreCase)
		{
			if (fullname == null)
			{
				throw new ArgumentNullException();
			}
			BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
			bindingFlags &= ~BindingFlags.Static;
			if (ignoreCase)
			{
				bindingFlags |= BindingFlags.IgnoreCase;
			}
			string name;
			string ns;
			RuntimeType.SplitName(fullname, out name, out ns);
			RuntimeType.MemberListType listType;
			RuntimeType.FilterHelper(bindingFlags, ref name, out ignoreCase, out listType);
			RuntimeType[] interfaceList = this.Cache.GetInterfaceList(listType, name);
			RuntimeType runtimeType = null;
			foreach (RuntimeType runtimeType2 in interfaceList)
			{
				if (RuntimeType.FilterApplyType(runtimeType2, bindingFlags, name, false, ns))
				{
					if (runtimeType != null)
					{
						throw new AmbiguousMatchException(Environment.GetResourceString("Arg_AmbiguousMatchException"));
					}
					runtimeType = runtimeType2;
				}
			}
			return runtimeType;
		}

		public override Type GetNestedType(string fullname, BindingFlags bindingAttr)
		{
			if (fullname == null)
			{
				throw new ArgumentNullException();
			}
			bindingAttr &= ~BindingFlags.Static;
			string name;
			string ns;
			RuntimeType.SplitName(fullname, out name, out ns);
			bool flag;
			RuntimeType.MemberListType listType;
			RuntimeType.FilterHelper(bindingAttr, ref name, out flag, out listType);
			RuntimeType[] nestedTypeList = this.Cache.GetNestedTypeList(listType, name);
			RuntimeType runtimeType = null;
			foreach (RuntimeType runtimeType2 in nestedTypeList)
			{
				if (RuntimeType.FilterApplyType(runtimeType2, bindingAttr, name, false, ns))
				{
					if (runtimeType != null)
					{
						throw new AmbiguousMatchException(Environment.GetResourceString("Arg_AmbiguousMatchException"));
					}
					runtimeType = runtimeType2;
				}
			}
			return runtimeType;
		}

		public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
		{
			if (name == null)
			{
				throw new ArgumentNullException();
			}
			RuntimeType.ListBuilder<MethodInfo> listBuilder = default(RuntimeType.ListBuilder<MethodInfo>);
			RuntimeType.ListBuilder<ConstructorInfo> listBuilder2 = default(RuntimeType.ListBuilder<ConstructorInfo>);
			RuntimeType.ListBuilder<PropertyInfo> listBuilder3 = default(RuntimeType.ListBuilder<PropertyInfo>);
			RuntimeType.ListBuilder<EventInfo> listBuilder4 = default(RuntimeType.ListBuilder<EventInfo>);
			RuntimeType.ListBuilder<FieldInfo> listBuilder5 = default(RuntimeType.ListBuilder<FieldInfo>);
			RuntimeType.ListBuilder<Type> listBuilder6 = default(RuntimeType.ListBuilder<Type>);
			int num = 0;
			if ((type & MemberTypes.Method) != (MemberTypes)0)
			{
				listBuilder = this.GetMethodCandidates(name, bindingAttr, CallingConventions.Any, null, true);
				if (type == MemberTypes.Method)
				{
					return listBuilder.ToArray();
				}
				num += listBuilder.Count;
			}
			if ((type & MemberTypes.Constructor) != (MemberTypes)0)
			{
				listBuilder2 = this.GetConstructorCandidates(name, bindingAttr, CallingConventions.Any, null, true);
				if (type == MemberTypes.Constructor)
				{
					return listBuilder2.ToArray();
				}
				num += listBuilder2.Count;
			}
			if ((type & MemberTypes.Property) != (MemberTypes)0)
			{
				listBuilder3 = this.GetPropertyCandidates(name, bindingAttr, null, true);
				if (type == MemberTypes.Property)
				{
					return listBuilder3.ToArray();
				}
				num += listBuilder3.Count;
			}
			if ((type & MemberTypes.Event) != (MemberTypes)0)
			{
				listBuilder4 = this.GetEventCandidates(name, bindingAttr, true);
				if (type == MemberTypes.Event)
				{
					return listBuilder4.ToArray();
				}
				num += listBuilder4.Count;
			}
			if ((type & MemberTypes.Field) != (MemberTypes)0)
			{
				listBuilder5 = this.GetFieldCandidates(name, bindingAttr, true);
				if (type == MemberTypes.Field)
				{
					return listBuilder5.ToArray();
				}
				num += listBuilder5.Count;
			}
			if ((type & (MemberTypes.TypeInfo | MemberTypes.NestedType)) != (MemberTypes)0)
			{
				listBuilder6 = this.GetNestedTypeCandidates(name, bindingAttr, true);
				if (type == MemberTypes.NestedType || type == MemberTypes.TypeInfo)
				{
					return listBuilder6.ToArray();
				}
				num += listBuilder6.Count;
			}
			MemberInfo[] array = (type == (MemberTypes.Constructor | MemberTypes.Method)) ? new MethodBase[num] : new MemberInfo[num];
			int num2 = 0;
			listBuilder.CopyTo(array, num2);
			num2 += listBuilder.Count;
			listBuilder2.CopyTo(array, num2);
			num2 += listBuilder2.Count;
			listBuilder3.CopyTo(array, num2);
			num2 += listBuilder3.Count;
			listBuilder4.CopyTo(array, num2);
			num2 += listBuilder4.Count;
			listBuilder5.CopyTo(array, num2);
			num2 += listBuilder5.Count;
			listBuilder6.CopyTo(array, num2);
			num2 += listBuilder6.Count;
			return array;
		}

		public override Module Module
		{
			get
			{
				return this.GetRuntimeModule();
			}
		}

		internal RuntimeModule GetRuntimeModule()
		{
			return RuntimeTypeHandle.GetModule(this);
		}

		public override Assembly Assembly
		{
			get
			{
				return this.GetRuntimeAssembly();
			}
		}

		internal RuntimeAssembly GetRuntimeAssembly()
		{
			return RuntimeTypeHandle.GetAssembly(this);
		}

		public override RuntimeTypeHandle TypeHandle
		{
			get
			{
				return new RuntimeTypeHandle(this);
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal sealed override RuntimeTypeHandle GetTypeHandleInternal()
		{
			return new RuntimeTypeHandle(this);
		}

		[SecuritySafeCritical]
		internal bool IsCollectible()
		{
			return RuntimeTypeHandle.IsCollectible(this.GetTypeHandleInternal());
		}

		[SecuritySafeCritical]
		protected override TypeCode GetTypeCodeImpl()
		{
			TypeCode typeCode = this.Cache.TypeCode;
			if (typeCode != TypeCode.Empty)
			{
				return typeCode;
			}
			switch (RuntimeTypeHandle.GetCorElementType(this))
			{
			case CorElementType.Boolean:
				typeCode = TypeCode.Boolean;
				goto IL_129;
			case CorElementType.Char:
				typeCode = TypeCode.Char;
				goto IL_129;
			case CorElementType.I1:
				typeCode = TypeCode.SByte;
				goto IL_129;
			case CorElementType.U1:
				typeCode = TypeCode.Byte;
				goto IL_129;
			case CorElementType.I2:
				typeCode = TypeCode.Int16;
				goto IL_129;
			case CorElementType.U2:
				typeCode = TypeCode.UInt16;
				goto IL_129;
			case CorElementType.I4:
				typeCode = TypeCode.Int32;
				goto IL_129;
			case CorElementType.U4:
				typeCode = TypeCode.UInt32;
				goto IL_129;
			case CorElementType.I8:
				typeCode = TypeCode.Int64;
				goto IL_129;
			case CorElementType.U8:
				typeCode = TypeCode.UInt64;
				goto IL_129;
			case CorElementType.R4:
				typeCode = TypeCode.Single;
				goto IL_129;
			case CorElementType.R8:
				typeCode = TypeCode.Double;
				goto IL_129;
			case CorElementType.String:
				typeCode = TypeCode.String;
				goto IL_129;
			case CorElementType.ValueType:
				if (this == Convert.ConvertTypes[15])
				{
					typeCode = TypeCode.Decimal;
					goto IL_129;
				}
				if (this == Convert.ConvertTypes[16])
				{
					typeCode = TypeCode.DateTime;
					goto IL_129;
				}
				if (this.IsEnum)
				{
					typeCode = Type.GetTypeCode(Enum.GetUnderlyingType(this));
					goto IL_129;
				}
				typeCode = TypeCode.Object;
				goto IL_129;
			}
			if (this == Convert.ConvertTypes[2])
			{
				typeCode = TypeCode.DBNull;
			}
			else if (this == Convert.ConvertTypes[18])
			{
				typeCode = TypeCode.String;
			}
			else
			{
				typeCode = TypeCode.Object;
			}
			IL_129:
			this.Cache.TypeCode = typeCode;
			return typeCode;
		}

		public override MethodBase DeclaringMethod
		{
			get
			{
				if (!this.IsGenericParameter)
				{
					throw new InvalidOperationException(Environment.GetResourceString("Arg_NotGenericParameter"));
				}
				IRuntimeMethodInfo declaringMethod = RuntimeTypeHandle.GetDeclaringMethod(this);
				if (declaringMethod == null)
				{
					return null;
				}
				return RuntimeType.GetMethodBase(RuntimeMethodHandle.GetDeclaringType(declaringMethod), declaringMethod);
			}
		}

		[SecuritySafeCritical]
		public override bool IsInstanceOfType(object o)
		{
			return RuntimeTypeHandle.IsInstanceOfType(this, o);
		}

		[ComVisible(true)]
		public override bool IsSubclassOf(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			RuntimeType runtimeType = type as RuntimeType;
			if (runtimeType == null)
			{
				return false;
			}
			RuntimeType baseType = this.GetBaseType();
			while (baseType != null)
			{
				if (baseType == runtimeType)
				{
					return true;
				}
				baseType = baseType.GetBaseType();
			}
			return runtimeType == RuntimeType.ObjectType && runtimeType != this;
		}

		public override bool IsAssignableFrom(System.Reflection.TypeInfo typeInfo)
		{
			return !(typeInfo == null) && this.IsAssignableFrom(typeInfo.AsType());
		}

		public override bool IsAssignableFrom(Type c)
		{
			if (c == null)
			{
				return false;
			}
			if (c == this)
			{
				return true;
			}
			RuntimeType runtimeType = c.UnderlyingSystemType as RuntimeType;
			if (runtimeType != null)
			{
				return RuntimeTypeHandle.CanCastTo(runtimeType, this);
			}
			if (c is TypeBuilder)
			{
				if (c.IsSubclassOf(this))
				{
					return true;
				}
				if (base.IsInterface)
				{
					return c.ImplementInterface(this);
				}
				if (this.IsGenericParameter)
				{
					Type[] genericParameterConstraints = this.GetGenericParameterConstraints();
					for (int i = 0; i < genericParameterConstraints.Length; i++)
					{
						if (!genericParameterConstraints[i].IsAssignableFrom(c))
						{
							return false;
						}
					}
					return true;
				}
			}
			return false;
		}

		public override bool IsEquivalentTo(Type other)
		{
			RuntimeType runtimeType = other as RuntimeType;
			return runtimeType != null && (runtimeType == this || RuntimeTypeHandle.IsEquivalentTo(this, runtimeType));
		}

		public override Type BaseType
		{
			get
			{
				return this.GetBaseType();
			}
		}

		private RuntimeType GetBaseType()
		{
			if (base.IsInterface)
			{
				return null;
			}
			if (RuntimeTypeHandle.IsGenericVariable(this))
			{
				Type[] genericParameterConstraints = this.GetGenericParameterConstraints();
				RuntimeType runtimeType = RuntimeType.ObjectType;
				foreach (RuntimeType runtimeType2 in genericParameterConstraints)
				{
					if (!runtimeType2.IsInterface)
					{
						if (runtimeType2.IsGenericParameter)
						{
							GenericParameterAttributes genericParameterAttributes = runtimeType2.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;
							if ((genericParameterAttributes & GenericParameterAttributes.ReferenceTypeConstraint) == GenericParameterAttributes.None && (genericParameterAttributes & GenericParameterAttributes.NotNullableValueTypeConstraint) == GenericParameterAttributes.None)
							{
								goto IL_55;
							}
						}
						runtimeType = runtimeType2;
					}
					IL_55:;
				}
				if (runtimeType == RuntimeType.ObjectType)
				{
					GenericParameterAttributes genericParameterAttributes2 = this.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask;
					if ((genericParameterAttributes2 & GenericParameterAttributes.NotNullableValueTypeConstraint) != GenericParameterAttributes.None)
					{
						runtimeType = RuntimeType.ValueType;
					}
				}
				return runtimeType;
			}
			return RuntimeTypeHandle.GetBaseType(this);
		}

		public override Type UnderlyingSystemType
		{
			get
			{
				return this;
			}
		}

		public override string FullName
		{
			get
			{
				return this.GetCachedName(TypeNameKind.FullName);
			}
		}

		public override string AssemblyQualifiedName
		{
			get
			{
				string fullName = this.FullName;
				if (fullName == null)
				{
					return null;
				}
				return Assembly.CreateQualifiedName(this.Assembly.FullName, fullName);
			}
		}

		public override string Namespace
		{
			get
			{
				string nameSpace = this.Cache.GetNameSpace();
				if (nameSpace == null || nameSpace.Length == 0)
				{
					return null;
				}
				return nameSpace;
			}
		}

		[SecuritySafeCritical]
		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			return RuntimeTypeHandle.GetAttributes(this);
		}

		public override Guid GUID
		{
			[SecuritySafeCritical]
			get
			{
				Guid result = default(Guid);
				this.GetGUID(ref result);
				return result;
			}
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern void GetGUID(ref Guid result);

		[SecuritySafeCritical]
		protected override bool IsContextfulImpl()
		{
			return RuntimeTypeHandle.IsContextful(this);
		}

		protected override bool IsByRefImpl()
		{
			return RuntimeTypeHandle.IsByRef(this);
		}

		protected override bool IsPrimitiveImpl()
		{
			return RuntimeTypeHandle.IsPrimitive(this);
		}

		protected override bool IsPointerImpl()
		{
			return RuntimeTypeHandle.IsPointer(this);
		}

		[SecuritySafeCritical]
		protected override bool IsCOMObjectImpl()
		{
			return RuntimeTypeHandle.IsComObject(this, false);
		}

		[SecuritySafeCritical]
		private bool IsZappedImpl()
		{
			return RuntimeTypeHandle.IsZapped(this);
		}

		[SecuritySafeCritical]
		private bool IsDoNotForceOrderOfConstructorsSetImpl()
		{
			return RuntimeTypeHandle.IsDoNotForceOrderOfConstructorsSet();
		}

		[SecuritySafeCritical]
		internal override bool IsWindowsRuntimeObjectImpl()
		{
			return RuntimeType.IsWindowsRuntimeObjectType(this);
		}

		[SecuritySafeCritical]
		internal override bool IsExportedToWindowsRuntimeImpl()
		{
			return RuntimeType.IsTypeExportedToWindowsRuntime(this);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsWindowsRuntimeObjectType(RuntimeType type);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsTypeExportedToWindowsRuntime(RuntimeType type);

		[SecuritySafeCritical]
		internal override bool HasProxyAttributeImpl()
		{
			return RuntimeTypeHandle.HasProxyAttribute(this);
		}

		internal bool IsDelegate()
		{
			return this.GetBaseType() == typeof(MulticastDelegate);
		}

		protected override bool IsValueTypeImpl()
		{
			return !(this == typeof(ValueType)) && !(this == typeof(Enum)) && this.IsSubclassOf(typeof(ValueType));
		}

		public override bool IsEnum
		{
			get
			{
				return this.GetBaseType() == RuntimeType.EnumType;
			}
		}

		protected override bool HasElementTypeImpl()
		{
			return RuntimeTypeHandle.HasElementType(this);
		}

		public override GenericParameterAttributes GenericParameterAttributes
		{
			[SecuritySafeCritical]
			get
			{
				if (!this.IsGenericParameter)
				{
					throw new InvalidOperationException(Environment.GetResourceString("Arg_NotGenericParameter"));
				}
				GenericParameterAttributes result;
				RuntimeTypeHandle.GetMetadataImport(this).GetGenericParamProps(this.MetadataToken, out result);
				return result;
			}
		}

		public override bool IsSecurityCritical
		{
			get
			{
				return new RuntimeTypeHandle(this).IsSecurityCritical();
			}
		}

		public override bool IsSecuritySafeCritical
		{
			get
			{
				return new RuntimeTypeHandle(this).IsSecuritySafeCritical();
			}
		}

		public override bool IsSecurityTransparent
		{
			get
			{
				return new RuntimeTypeHandle(this).IsSecurityTransparent();
			}
		}

		internal override bool IsSzArray
		{
			get
			{
				return RuntimeTypeHandle.IsSzArray(this);
			}
		}

		protected override bool IsArrayImpl()
		{
			return RuntimeTypeHandle.IsArray(this);
		}

		[SecuritySafeCritical]
		public override int GetArrayRank()
		{
			if (!this.IsArrayImpl())
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_HasToBeArrayClass"));
			}
			return RuntimeTypeHandle.GetArrayRank(this);
		}

		public override Type GetElementType()
		{
			return RuntimeTypeHandle.GetElementType(this);
		}

		public override string[] GetEnumNames()
		{
			if (!this.IsEnum)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeEnum"), "enumType");
			}
			string[] array = Enum.InternalGetNames(this);
			string[] array2 = new string[array.Length];
			Array.Copy(array, array2, array.Length);
			return array2;
		}

		[SecuritySafeCritical]
		public override Array GetEnumValues()
		{
			if (!this.IsEnum)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeEnum"), "enumType");
			}
			ulong[] array = Enum.InternalGetValues(this);
			Array array2 = Array.UnsafeCreateInstance(this, array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				object value = Enum.ToObject(this, array[i]);
				array2.SetValue(value, i);
			}
			return array2;
		}

		public override Type GetEnumUnderlyingType()
		{
			if (!this.IsEnum)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeEnum"), "enumType");
			}
			return Enum.InternalGetUnderlyingType(this);
		}

		public override bool IsEnumDefined(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			RuntimeType runtimeType = (RuntimeType)value.GetType();
			if (runtimeType.IsEnum)
			{
				if (!runtimeType.IsEquivalentTo(this))
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_EnumAndObjectMustBeSameType", new object[]
					{
						runtimeType.ToString(),
						this.ToString()
					}));
				}
				runtimeType = (RuntimeType)runtimeType.GetEnumUnderlyingType();
			}
			if (runtimeType == RuntimeType.StringType)
			{
				string[] array = Enum.InternalGetNames(this);
				return Array.IndexOf<object>(array, value) >= 0;
			}
			if (Type.IsIntegerType(runtimeType))
			{
				RuntimeType runtimeType2 = Enum.InternalGetUnderlyingType(this);
				if (runtimeType2 != runtimeType)
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_EnumUnderlyingTypeAndObjectMustBeSameType", new object[]
					{
						runtimeType.ToString(),
						runtimeType2.ToString()
					}));
				}
				ulong[] array2 = Enum.InternalGetValues(this);
				ulong value2 = Enum.ToUInt64(value);
				return Array.BinarySearch<ulong>(array2, value2) >= 0;
			}
			else
			{
				if (CompatibilitySwitches.IsAppEarlierThanWindowsPhone8)
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_EnumUnderlyingTypeAndObjectMustBeSameType", new object[]
					{
						runtimeType.ToString(),
						this.GetEnumUnderlyingType()
					}));
				}
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_UnknownEnumType"));
			}
		}

		public override string GetEnumName(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			Type type = value.GetType();
			if (!type.IsEnum && !Type.IsIntegerType(type))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeEnumBaseTypeOrEnum"), "value");
			}
			ulong[] array = Enum.InternalGetValues(this);
			ulong value2 = Enum.ToUInt64(value);
			int num = Array.BinarySearch<ulong>(array, value2);
			if (num >= 0)
			{
				string[] array2 = Enum.InternalGetNames(this);
				return array2[num];
			}
			return null;
		}

		internal RuntimeType[] GetGenericArgumentsInternal()
		{
			return base.GetRootElementType().GetTypeHandleInternal().GetInstantiationInternal();
		}

		public override Type[] GetGenericArguments()
		{
			Type[] array = base.GetRootElementType().GetTypeHandleInternal().GetInstantiationPublic();
			if (array == null)
			{
				array = EmptyArray<Type>.Value;
			}
			return array;
		}

		[SecuritySafeCritical]
		public override Type MakeGenericType(params Type[] instantiation)
		{
			if (instantiation == null)
			{
				throw new ArgumentNullException("instantiation");
			}
			RuntimeType[] array = new RuntimeType[instantiation.Length];
			if (!this.IsGenericTypeDefinition)
			{
				throw new InvalidOperationException(Environment.GetResourceString("Arg_NotGenericTypeDefinition", new object[]
				{
					this
				}));
			}
			if (this.GetGenericArguments().Length != instantiation.Length)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_GenericArgsCount"), "instantiation");
			}
			for (int i = 0; i < instantiation.Length; i++)
			{
				Type type = instantiation[i];
				if (type == null)
				{
					throw new ArgumentNullException();
				}
				RuntimeType runtimeType = type as RuntimeType;
				if (runtimeType == null)
				{
					Type[] array2 = new Type[instantiation.Length];
					for (int j = 0; j < instantiation.Length; j++)
					{
						array2[j] = instantiation[j];
					}
					instantiation = array2;
					return TypeBuilderInstantiation.MakeGenericType(this, instantiation);
				}
				array[i] = runtimeType;
			}
			RuntimeType[] genericArgumentsInternal = this.GetGenericArgumentsInternal();
			RuntimeType.SanityCheckGenericArguments(array, genericArgumentsInternal);
			Type result = null;
			try
			{
				result = new RuntimeTypeHandle(this).Instantiate(array);
			}
			catch (TypeLoadException ex)
			{
				RuntimeType.ValidateGenericArguments(this, array, ex);
				throw ex;
			}
			return result;
		}

		public override bool IsGenericTypeDefinition
		{
			get
			{
				return RuntimeTypeHandle.IsGenericTypeDefinition(this);
			}
		}

		public override bool IsGenericParameter
		{
			get
			{
				return RuntimeTypeHandle.IsGenericVariable(this);
			}
		}

		public override int GenericParameterPosition
		{
			get
			{
				if (!this.IsGenericParameter)
				{
					throw new InvalidOperationException(Environment.GetResourceString("Arg_NotGenericParameter"));
				}
				return new RuntimeTypeHandle(this).GetGenericVariableIndex();
			}
		}

		public override Type GetGenericTypeDefinition()
		{
			if (!this.IsGenericType)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NotGenericType"));
			}
			return RuntimeTypeHandle.GetGenericTypeDefinition(this);
		}

		public override bool IsGenericType
		{
			get
			{
				return RuntimeTypeHandle.HasInstantiation(this);
			}
		}

		public override bool IsConstructedGenericType
		{
			get
			{
				return this.IsGenericType && !this.IsGenericTypeDefinition;
			}
		}

		public override bool ContainsGenericParameters
		{
			get
			{
				return base.GetRootElementType().GetTypeHandleInternal().ContainsGenericVariables();
			}
		}

		public override Type[] GetGenericParameterConstraints()
		{
			if (!this.IsGenericParameter)
			{
				throw new InvalidOperationException(Environment.GetResourceString("Arg_NotGenericParameter"));
			}
			Type[] array = new RuntimeTypeHandle(this).GetConstraints();
			if (array == null)
			{
				array = EmptyArray<Type>.Value;
			}
			return array;
		}

		[SecuritySafeCritical]
		public override Type MakePointerType()
		{
			return new RuntimeTypeHandle(this).MakePointer();
		}

		public override Type MakeByRefType()
		{
			return new RuntimeTypeHandle(this).MakeByRef();
		}

		public override Type MakeArrayType()
		{
			return new RuntimeTypeHandle(this).MakeSZArray();
		}

		public override Type MakeArrayType(int rank)
		{
			if (rank <= 0)
			{
				throw new IndexOutOfRangeException();
			}
			return new RuntimeTypeHandle(this).MakeArray(rank);
		}

		public override StructLayoutAttribute StructLayoutAttribute
		{
			[SecuritySafeCritical]
			get
			{
				return (StructLayoutAttribute)StructLayoutAttribute.GetCustomAttribute(this);
			}
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CanValueSpecialCast(RuntimeType valueType, RuntimeType targetType);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object AllocateValueType(RuntimeType type, object value, bool fForceTypeChange);

		[SecuritySafeCritical]
		internal object CheckValue(object value, Binder binder, CultureInfo culture, BindingFlags invokeAttr)
		{
			if (this.IsInstanceOfType(value))
			{
				RealProxy realProxy = RemotingServices.GetRealProxy(value);
				Type type;
				if (realProxy != null)
				{
					type = realProxy.GetProxiedType();
				}
				else
				{
					type = value.GetType();
				}
				if (type != this && RuntimeTypeHandle.IsValueType(this))
				{
					return RuntimeType.AllocateValueType(this, value, true);
				}
				return value;
			}
			else
			{
				bool isByRef = base.IsByRef;
				if (isByRef)
				{
					RuntimeType elementType = RuntimeTypeHandle.GetElementType(this);
					if (elementType.IsInstanceOfType(value) || value == null)
					{
						return RuntimeType.AllocateValueType(elementType, value, false);
					}
				}
				else
				{
					if (value == null)
					{
						return value;
					}
					if (this == RuntimeType.s_typedRef)
					{
						return value;
					}
				}
				bool flag = base.IsPointer || this.IsEnum || base.IsPrimitive;
				if (flag)
				{
					Pointer pointer = value as Pointer;
					RuntimeType valueType;
					if (pointer != null)
					{
						valueType = pointer.GetPointerType();
					}
					else
					{
						valueType = (RuntimeType)value.GetType();
					}
					if (RuntimeType.CanValueSpecialCast(valueType, this))
					{
						if (pointer != null)
						{
							return pointer.GetPointerValue();
						}
						return value;
					}
				}
				if ((invokeAttr & BindingFlags.ExactBinding) == BindingFlags.ExactBinding)
				{
					throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString("Arg_ObjObjEx"), value.GetType(), this));
				}
				return this.TryChangeType(value, binder, culture, flag);
			}
		}

		[SecurityCritical]
		private object TryChangeType(object value, Binder binder, CultureInfo culture, bool needsSpecialCast)
		{
			if (binder != null && binder != Type.DefaultBinder)
			{
				value = binder.ChangeType(value, this, culture);
				if (this.IsInstanceOfType(value))
				{
					return value;
				}
				if (base.IsByRef)
				{
					RuntimeType elementType = RuntimeTypeHandle.GetElementType(this);
					if (elementType.IsInstanceOfType(value) || value == null)
					{
						return RuntimeType.AllocateValueType(elementType, value, false);
					}
				}
				else if (value == null)
				{
					return value;
				}
				if (needsSpecialCast)
				{
					Pointer pointer = value as Pointer;
					RuntimeType valueType;
					if (pointer != null)
					{
						valueType = pointer.GetPointerType();
					}
					else
					{
						valueType = (RuntimeType)value.GetType();
					}
					if (RuntimeType.CanValueSpecialCast(valueType, this))
					{
						if (pointer != null)
						{
							return pointer.GetPointerValue();
						}
						return value;
					}
				}
			}
			throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString("Arg_ObjObjEx"), value.GetType(), this));
		}

		public override MemberInfo[] GetDefaultMembers()
		{
			MemberInfo[] array = null;
			string defaultMemberName = this.GetDefaultMemberName();
			if (defaultMemberName != null)
			{
				array = base.GetMember(defaultMemberName);
			}
			if (array == null)
			{
				array = EmptyArray<MemberInfo>.Value;
			}
			return array;
		}

		[SecuritySafeCritical]
		[DebuggerStepThrough]
		[DebuggerHidden]
		public override object InvokeMember(string name, BindingFlags bindingFlags, Binder binder, object target, object[] providedArgs, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParams)
		{
			if (this.IsGenericParameter)
			{
				throw new InvalidOperationException(Environment.GetResourceString("Arg_GenericParameter"));
			}
			if ((bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty)) == BindingFlags.Default)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_NoAccessSpec"), "bindingFlags");
			}
			if ((bindingFlags & (BindingFlags)255) == BindingFlags.Default)
			{
				bindingFlags |= (BindingFlags.Instance | BindingFlags.Public);
				if ((bindingFlags & BindingFlags.CreateInstance) == BindingFlags.Default)
				{
					bindingFlags |= BindingFlags.Static;
				}
			}
			if (namedParams != null)
			{
				if (providedArgs != null)
				{
					if (namedParams.Length > providedArgs.Length)
					{
						throw new ArgumentException(Environment.GetResourceString("Arg_NamedParamTooBig"), "namedParams");
					}
				}
				else if (namedParams.Length != 0)
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_NamedParamTooBig"), "namedParams");
				}
			}
			if (target != null && target.GetType().IsCOMObject)
			{
				if ((bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty)) == BindingFlags.Default)
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_COMAccess"), "bindingFlags");
				}
				if ((bindingFlags & BindingFlags.GetProperty) != BindingFlags.Default && (bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty) & ~(BindingFlags.InvokeMethod | BindingFlags.GetProperty)) != BindingFlags.Default)
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_PropSetGet"), "bindingFlags");
				}
				if ((bindingFlags & BindingFlags.InvokeMethod) != BindingFlags.Default && (bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty) & ~(BindingFlags.InvokeMethod | BindingFlags.GetProperty)) != BindingFlags.Default)
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_PropSetInvoke"), "bindingFlags");
				}
				if ((bindingFlags & BindingFlags.SetProperty) != BindingFlags.Default && (bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty) & ~BindingFlags.SetProperty) != BindingFlags.Default)
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_COMPropSetPut"), "bindingFlags");
				}
				if ((bindingFlags & BindingFlags.PutDispProperty) != BindingFlags.Default && (bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty) & ~BindingFlags.PutDispProperty) != BindingFlags.Default)
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_COMPropSetPut"), "bindingFlags");
				}
				if ((bindingFlags & BindingFlags.PutRefDispProperty) != BindingFlags.Default && (bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty) & ~BindingFlags.PutRefDispProperty) != BindingFlags.Default)
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_COMPropSetPut"), "bindingFlags");
				}
				if (RemotingServices.IsTransparentProxy(target))
				{
					return ((MarshalByRefObject)target).InvokeMember(name, bindingFlags, binder, providedArgs, modifiers, culture, namedParams);
				}
				if (name == null)
				{
					throw new ArgumentNullException("name");
				}
				bool[] byrefModifiers = (modifiers == null) ? null : modifiers[0].IsByRefArray;
				int culture2 = (culture == null) ? 1033 : culture.LCID;
				return this.InvokeDispMethod(name, bindingFlags, target, providedArgs, byrefModifiers, culture2, namedParams);
			}
			else
			{
				if (namedParams != null && Array.IndexOf<string>(namedParams, null) != -1)
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_NamedParamNull"), "namedParams");
				}
				int num = (providedArgs != null) ? providedArgs.Length : 0;
				if (binder == null)
				{
					binder = Type.DefaultBinder;
				}
				bool flag = binder == Type.DefaultBinder;
				if ((bindingFlags & BindingFlags.CreateInstance) != BindingFlags.Default)
				{
					if ((bindingFlags & BindingFlags.CreateInstance) != BindingFlags.Default && (bindingFlags & (BindingFlags.InvokeMethod | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty)) != BindingFlags.Default)
					{
						throw new ArgumentException(Environment.GetResourceString("Arg_CreatInstAccess"), "bindingFlags");
					}
					return Activator.CreateInstance(this, bindingFlags, binder, providedArgs, culture);
				}
				else
				{
					if ((bindingFlags & (BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty)) != BindingFlags.Default)
					{
						bindingFlags |= BindingFlags.SetProperty;
					}
					if (name == null)
					{
						throw new ArgumentNullException("name");
					}
					if (name.Length == 0 || name.Equals("[DISPID=0]"))
					{
						name = this.GetDefaultMemberName();
						if (name == null)
						{
							name = "ToString";
						}
					}
					bool flag2 = (bindingFlags & BindingFlags.GetField) > BindingFlags.Default;
					bool flag3 = (bindingFlags & BindingFlags.SetField) > BindingFlags.Default;
					if (flag2 || flag3)
					{
						if (flag2)
						{
							if (flag3)
							{
								throw new ArgumentException(Environment.GetResourceString("Arg_FldSetGet"), "bindingFlags");
							}
							if ((bindingFlags & BindingFlags.SetProperty) != BindingFlags.Default)
							{
								throw new ArgumentException(Environment.GetResourceString("Arg_FldGetPropSet"), "bindingFlags");
							}
						}
						else
						{
							if (providedArgs == null)
							{
								throw new ArgumentNullException("providedArgs");
							}
							if ((bindingFlags & BindingFlags.GetProperty) != BindingFlags.Default)
							{
								throw new ArgumentException(Environment.GetResourceString("Arg_FldSetPropGet"), "bindingFlags");
							}
							if ((bindingFlags & BindingFlags.InvokeMethod) != BindingFlags.Default)
							{
								throw new ArgumentException(Environment.GetResourceString("Arg_FldSetInvoke"), "bindingFlags");
							}
						}
						FieldInfo fieldInfo = null;
						FieldInfo[] array = this.GetMember(name, MemberTypes.Field, bindingFlags) as FieldInfo[];
						if (array.Length == 1)
						{
							fieldInfo = array[0];
						}
						else if (array.Length != 0)
						{
							fieldInfo = binder.BindToField(bindingFlags, array, flag2 ? Empty.Value : providedArgs[0], culture);
						}
						if (fieldInfo != null)
						{
							if (fieldInfo.FieldType.IsArray || fieldInfo.FieldType == typeof(Array))
							{
								int num2;
								if ((bindingFlags & BindingFlags.GetField) != BindingFlags.Default)
								{
									num2 = num;
								}
								else
								{
									num2 = num - 1;
								}
								if (num2 > 0)
								{
									int[] array2 = new int[num2];
									for (int i = 0; i < num2; i++)
									{
										try
										{
											array2[i] = ((IConvertible)providedArgs[i]).ToInt32(null);
										}
										catch (InvalidCastException)
										{
											throw new ArgumentException(Environment.GetResourceString("Arg_IndexMustBeInt"));
										}
									}
									Array array3 = (Array)fieldInfo.GetValue(target);
									if ((bindingFlags & BindingFlags.GetField) != BindingFlags.Default)
									{
										return array3.GetValue(array2);
									}
									array3.SetValue(providedArgs[num2], array2);
									return null;
								}
							}
							if (flag2)
							{
								if (num != 0)
								{
									throw new ArgumentException(Environment.GetResourceString("Arg_FldGetArgErr"), "bindingFlags");
								}
								return fieldInfo.GetValue(target);
							}
							else
							{
								if (num != 1)
								{
									throw new ArgumentException(Environment.GetResourceString("Arg_FldSetArgErr"), "bindingFlags");
								}
								fieldInfo.SetValue(target, providedArgs[0], bindingFlags, binder, culture);
								return null;
							}
						}
						else if ((bindingFlags & (BindingFlags)16773888) == BindingFlags.Default)
						{
							throw new MissingFieldException(this.FullName, name);
						}
					}
					bool flag4 = (bindingFlags & BindingFlags.GetProperty) > BindingFlags.Default;
					bool flag5 = (bindingFlags & BindingFlags.SetProperty) > BindingFlags.Default;
					if (flag4 || flag5)
					{
						if (flag4)
						{
							if (flag5)
							{
								throw new ArgumentException(Environment.GetResourceString("Arg_PropSetGet"), "bindingFlags");
							}
						}
						else if ((bindingFlags & BindingFlags.InvokeMethod) != BindingFlags.Default)
						{
							throw new ArgumentException(Environment.GetResourceString("Arg_PropSetInvoke"), "bindingFlags");
						}
					}
					MethodInfo[] array4 = null;
					MethodInfo methodInfo = null;
					if ((bindingFlags & BindingFlags.InvokeMethod) != BindingFlags.Default)
					{
						MethodInfo[] array5 = this.GetMember(name, MemberTypes.Method, bindingFlags) as MethodInfo[];
						List<MethodInfo> list = null;
						foreach (MethodInfo methodInfo2 in array5)
						{
							if (RuntimeType.FilterApplyMethodInfo((RuntimeMethodInfo)methodInfo2, bindingFlags, CallingConventions.Any, new Type[num]))
							{
								if (methodInfo == null)
								{
									methodInfo = methodInfo2;
								}
								else
								{
									if (list == null)
									{
										list = new List<MethodInfo>(array5.Length);
										list.Add(methodInfo);
									}
									list.Add(methodInfo2);
								}
							}
						}
						if (list != null)
						{
							array4 = new MethodInfo[list.Count];
							list.CopyTo(array4);
						}
					}
					if ((methodInfo == null && flag4) || flag5)
					{
						PropertyInfo[] array6 = this.GetMember(name, MemberTypes.Property, bindingFlags) as PropertyInfo[];
						List<MethodInfo> list2 = null;
						for (int k = 0; k < array6.Length; k++)
						{
							MethodInfo methodInfo3;
							if (flag5)
							{
								methodInfo3 = array6[k].GetSetMethod(true);
							}
							else
							{
								methodInfo3 = array6[k].GetGetMethod(true);
							}
							if (!(methodInfo3 == null) && RuntimeType.FilterApplyMethodInfo((RuntimeMethodInfo)methodInfo3, bindingFlags, CallingConventions.Any, new Type[num]))
							{
								if (methodInfo == null)
								{
									methodInfo = methodInfo3;
								}
								else
								{
									if (list2 == null)
									{
										list2 = new List<MethodInfo>(array6.Length);
										list2.Add(methodInfo);
									}
									list2.Add(methodInfo3);
								}
							}
						}
						if (list2 != null)
						{
							array4 = new MethodInfo[list2.Count];
							list2.CopyTo(array4);
						}
					}
					if (!(methodInfo != null))
					{
						throw new MissingMethodException(this.FullName, name);
					}
					if (array4 == null && num == 0 && methodInfo.GetParametersNoCopy().Length == 0 && (bindingFlags & BindingFlags.OptionalParamBinding) == BindingFlags.Default)
					{
						return methodInfo.Invoke(target, bindingFlags, binder, providedArgs, culture);
					}
					if (array4 == null)
					{
						array4 = new MethodInfo[]
						{
							methodInfo
						};
					}
					if (providedArgs == null)
					{
						providedArgs = EmptyArray<object>.Value;
					}
					object obj = null;
					MethodBase methodBase = null;
					try
					{
						methodBase = binder.BindToMethod(bindingFlags, array4, ref providedArgs, modifiers, culture, namedParams, out obj);
					}
					catch (MissingMethodException)
					{
					}
					if (methodBase == null)
					{
						throw new MissingMethodException(this.FullName, name);
					}
					object result = ((MethodInfo)methodBase).Invoke(target, bindingFlags, binder, providedArgs, culture);
					if (obj != null)
					{
						binder.ReorderArgumentArray(ref providedArgs, obj);
					}
					return result;
				}
			}
		}

		public override bool Equals(object obj)
		{
			return obj == this;
		}

		public override int GetHashCode()
		{
			return RuntimeHelpers.GetHashCode(this);
		}

		public static bool operator ==(RuntimeType left, RuntimeType right)
		{
			return left == right;
		}

		public static bool operator !=(RuntimeType left, RuntimeType right)
		{
			return left != right;
		}

		public override string ToString()
		{
			return this.GetCachedName(TypeNameKind.ToString);
		}

		public object Clone()
		{
			return this;
		}

		[SecurityCritical]
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			UnitySerializationHolder.GetUnitySerializationInfo(info, this);
		}

		[SecuritySafeCritical]
		public override object[] GetCustomAttributes(bool inherit)
		{
			return CustomAttribute.GetCustomAttributes(this, RuntimeType.ObjectType, inherit);
		}

		[SecuritySafeCritical]
		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			if (attributeType == null)
			{
				throw new ArgumentNullException("attributeType");
			}
			RuntimeType runtimeType = attributeType.UnderlyingSystemType as RuntimeType;
			if (runtimeType == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeType"), "attributeType");
			}
			return CustomAttribute.GetCustomAttributes(this, runtimeType, inherit);
		}

		[SecuritySafeCritical]
		public override bool IsDefined(Type attributeType, bool inherit)
		{
			if (attributeType == null)
			{
				throw new ArgumentNullException("attributeType");
			}
			RuntimeType runtimeType = attributeType.UnderlyingSystemType as RuntimeType;
			if (runtimeType == null)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBeType"), "attributeType");
			}
			return CustomAttribute.IsDefined(this, runtimeType, inherit);
		}

		public override IList<CustomAttributeData> GetCustomAttributesData()
		{
			return CustomAttributeData.GetCustomAttributesInternal(this);
		}

		public override string Name
		{
			get
			{
				return this.GetCachedName(TypeNameKind.Name);
			}
		}

		internal override string FormatTypeName(bool serialization)
		{
			if (serialization)
			{
				return this.GetCachedName(TypeNameKind.SerializationName);
			}
			Type rootElementType = base.GetRootElementType();
			if (rootElementType.IsNested)
			{
				return this.Name;
			}
			string text = this.ToString();
			if (rootElementType.IsPrimitive || rootElementType == typeof(void) || rootElementType == typeof(TypedReference))
			{
				text = text.Substring("System.".Length);
			}
			return text;
		}

		private string GetCachedName(TypeNameKind kind)
		{
			return this.Cache.GetName(kind);
		}

		public override MemberTypes MemberType
		{
			get
			{
				if (base.IsPublic || base.IsNotPublic)
				{
					return MemberTypes.TypeInfo;
				}
				return MemberTypes.NestedType;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return this.Cache.GetEnclosingType();
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return this.DeclaringType;
			}
		}

		public override int MetadataToken
		{
			[SecuritySafeCritical]
			get
			{
				return RuntimeTypeHandle.GetToken(this);
			}
		}

		private void CreateInstanceCheckThis()
		{
			if (this is ReflectionOnlyType)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_ReflectionOnlyInvoke"));
			}
			if (this.ContainsGenericParameters)
			{
				throw new ArgumentException(Environment.GetResourceString("Acc_CreateGenericEx", new object[]
				{
					this
				}));
			}
			Type rootElementType = base.GetRootElementType();
			if (rootElementType == typeof(ArgIterator))
			{
				throw new NotSupportedException(Environment.GetResourceString("Acc_CreateArgIterator"));
			}
			if (rootElementType == typeof(void))
			{
				throw new NotSupportedException(Environment.GetResourceString("Acc_CreateVoid"));
			}
		}

		[SecurityCritical]
		internal object CreateInstanceImpl(BindingFlags bindingAttr, Binder binder, object[] args, CultureInfo culture, object[] activationAttributes, ref StackCrawlMark stackMark)
		{
			this.CreateInstanceCheckThis();
			object result = null;
			try
			{
				try
				{
					if (activationAttributes != null)
					{
						ActivationServices.PushActivationAttributes(this, activationAttributes);
					}
					if (args == null)
					{
						args = EmptyArray<object>.Value;
					}
					int num = args.Length;
					if (binder == null)
					{
						binder = Type.DefaultBinder;
					}
					if (num == 0 && (bindingAttr & BindingFlags.Public) != BindingFlags.Default && (bindingAttr & BindingFlags.Instance) != BindingFlags.Default && (this.IsGenericCOMObjectImpl() || base.IsValueType))
					{
						result = this.CreateInstanceDefaultCtor((bindingAttr & BindingFlags.NonPublic) == BindingFlags.Default, false, true, ref stackMark);
					}
					else
					{
						ConstructorInfo[] constructors = this.GetConstructors(bindingAttr);
						List<MethodBase> list = new List<MethodBase>(constructors.Length);
						Type[] array = new Type[num];
						for (int i = 0; i < num; i++)
						{
							if (args[i] != null)
							{
								array[i] = args[i].GetType();
							}
						}
						for (int j = 0; j < constructors.Length; j++)
						{
							if (RuntimeType.FilterApplyConstructorInfo((RuntimeConstructorInfo)constructors[j], bindingAttr, CallingConventions.Any, array))
							{
								list.Add(constructors[j]);
							}
						}
						MethodBase[] array2 = new MethodBase[list.Count];
						list.CopyTo(array2);
						if (array2 != null && array2.Length == 0)
						{
							array2 = null;
						}
						if (array2 == null)
						{
							if (activationAttributes != null)
							{
								ActivationServices.PopActivationAttributes(this);
								activationAttributes = null;
							}
							throw new MissingMethodException(Environment.GetResourceString("MissingConstructor_Name", new object[]
							{
								this.FullName
							}));
						}
						object obj = null;
						MethodBase methodBase;
						try
						{
							methodBase = binder.BindToMethod(bindingAttr, array2, ref args, null, culture, null, out obj);
						}
						catch (MissingMethodException)
						{
							methodBase = null;
						}
						if (methodBase == null)
						{
							if (activationAttributes != null)
							{
								ActivationServices.PopActivationAttributes(this);
								activationAttributes = null;
							}
							throw new MissingMethodException(Environment.GetResourceString("MissingConstructor_Name", new object[]
							{
								this.FullName
							}));
						}
						if (RuntimeType.DelegateType.IsAssignableFrom(methodBase.DeclaringType))
						{
							new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
						}
						if (methodBase.GetParametersNoCopy().Length == 0)
						{
							if (args.Length != 0)
							{
								throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("NotSupported_CallToVarArg"), Array.Empty<object>()));
							}
							result = Activator.CreateInstance(this, true);
						}
						else
						{
							result = ((ConstructorInfo)methodBase).Invoke(bindingAttr, binder, args, culture);
							if (obj != null)
							{
								binder.ReorderArgumentArray(ref args, obj);
							}
						}
					}
				}
				finally
				{
					if (activationAttributes != null)
					{
						ActivationServices.PopActivationAttributes(this);
						activationAttributes = null;
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
			return result;
		}

		[SecuritySafeCritical]
		internal object CreateInstanceSlow(bool publicOnly, bool skipCheckThis, bool fillCache, ref StackCrawlMark stackMark)
		{
			RuntimeMethodHandleInternal rmh = default(RuntimeMethodHandleInternal);
			bool bNeedSecurityCheck = true;
			bool flag = false;
			bool noCheck = false;
			if (!skipCheckThis)
			{
				this.CreateInstanceCheckThis();
			}
			if (!fillCache)
			{
				noCheck = true;
			}
			INVOCATION_FLAGS invocationFlags = this.InvocationFlags;
			if ((invocationFlags & INVOCATION_FLAGS.INVOCATION_FLAGS_NON_W8P_FX_API) != INVOCATION_FLAGS.INVOCATION_FLAGS_UNKNOWN)
			{
				RuntimeAssembly executingAssembly = RuntimeAssembly.GetExecutingAssembly(ref stackMark);
				if (executingAssembly != null && !executingAssembly.IsSafeForReflection())
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_APIInvalidForCurrentContext", new object[]
					{
						this.FullName
					}));
				}
				noCheck = false;
				flag = false;
			}
			object result = RuntimeTypeHandle.CreateInstance(this, publicOnly, noCheck, ref flag, ref rmh, ref bNeedSecurityCheck);
			if (flag && fillCache)
			{
				RuntimeType.ActivatorCache activatorCache = RuntimeType.s_ActivatorCache;
				if (activatorCache == null)
				{
					activatorCache = new RuntimeType.ActivatorCache();
					RuntimeType.s_ActivatorCache = activatorCache;
				}
				RuntimeType.ActivatorCacheEntry entry = new RuntimeType.ActivatorCacheEntry(this, rmh, bNeedSecurityCheck);
				activatorCache.SetEntry(entry);
			}
			return result;
		}

		[SecuritySafeCritical]
		[DebuggerStepThrough]
		[DebuggerHidden]
		internal object CreateInstanceDefaultCtor(bool publicOnly, bool skipCheckThis, bool fillCache, ref StackCrawlMark stackMark)
		{
			if (base.GetType() == typeof(ReflectionOnlyType))
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NotAllowedInReflectionOnly"));
			}
			RuntimeType.ActivatorCache activatorCache = RuntimeType.s_ActivatorCache;
			if (activatorCache != null)
			{
				RuntimeType.ActivatorCacheEntry entry = activatorCache.GetEntry(this);
				if (entry != null)
				{
					if (publicOnly && entry.m_ctor != null && (entry.m_ctorAttributes & MethodAttributes.MemberAccessMask) != MethodAttributes.Public)
					{
						throw new MissingMethodException(Environment.GetResourceString("Arg_NoDefCTor"));
					}
					object obj = RuntimeTypeHandle.Allocate(this);
					if (entry.m_ctor != null)
					{
						if (entry.m_bNeedSecurityCheck)
						{
							RuntimeMethodHandle.PerformSecurityCheck(obj, entry.m_hCtorMethodHandle, this, 268435456U);
						}
						try
						{
							entry.m_ctor(obj);
						}
						catch (Exception inner)
						{
							throw new TargetInvocationException(inner);
						}
					}
					return obj;
				}
			}
			return this.CreateInstanceSlow(publicOnly, skipCheckThis, fillCache, ref stackMark);
		}

		internal void InvalidateCachedNestedType()
		{
			this.Cache.InvalidateCachedNestedType();
		}

		[SecuritySafeCritical]
		internal bool IsGenericCOMObjectImpl()
		{
			return RuntimeTypeHandle.IsComObject(this, true);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object _CreateEnum(RuntimeType enumType, long value);

		[SecuritySafeCritical]
		internal static object CreateEnum(RuntimeType enumType, long value)
		{
			return RuntimeType._CreateEnum(enumType, value);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern object InvokeDispMethod(string name, BindingFlags invokeAttr, object target, object[] args, bool[] byrefModifiers, int culture, string[] namedParameters);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Type GetTypeFromProgIDImpl(string progID, string server, bool throwOnError);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Type GetTypeFromCLSIDImpl(Guid clsid, string server, bool throwOnError);

		[SecuritySafeCritical]
		private object ForwardCallToInvokeMember(string memberName, BindingFlags flags, object target, int[] aWrapperTypes, ref MessageData msgData)
		{
			ParameterModifier[] array = null;
			object obj = null;
			Message message = new Message();
			message.InitFields(msgData);
			MethodInfo methodInfo = (MethodInfo)message.GetMethodBase();
			object[] args = message.Args;
			int num = args.Length;
			ParameterInfo[] parametersNoCopy = methodInfo.GetParametersNoCopy();
			if (num > 0)
			{
				ParameterModifier parameterModifier = new ParameterModifier(num);
				for (int i = 0; i < num; i++)
				{
					if (parametersNoCopy[i].ParameterType.IsByRef)
					{
						parameterModifier[i] = true;
					}
				}
				array = new ParameterModifier[]
				{
					parameterModifier
				};
				if (aWrapperTypes != null)
				{
					this.WrapArgsForInvokeCall(args, aWrapperTypes);
				}
			}
			if (methodInfo.ReturnType == typeof(void))
			{
				flags |= BindingFlags.IgnoreReturn;
			}
			try
			{
				obj = this.InvokeMember(memberName, flags, null, target, args, array, null, null);
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			for (int j = 0; j < num; j++)
			{
				if (array[0][j] && args[j] != null)
				{
					Type elementType = parametersNoCopy[j].ParameterType.GetElementType();
					if (elementType != args[j].GetType())
					{
						args[j] = this.ForwardCallBinder.ChangeType(args[j], elementType, null);
					}
				}
			}
			if (obj != null)
			{
				Type returnType = methodInfo.ReturnType;
				if (returnType != obj.GetType())
				{
					obj = this.ForwardCallBinder.ChangeType(obj, returnType, null);
				}
			}
			RealProxy.PropagateOutParameters(message, args, obj);
			return obj;
		}

		[SecuritySafeCritical]
		private void WrapArgsForInvokeCall(object[] aArgs, int[] aWrapperTypes)
		{
			int num = aArgs.Length;
			for (int i = 0; i < num; i++)
			{
				if (aWrapperTypes[i] != 0)
				{
					if ((aWrapperTypes[i] & 65536) != 0)
					{
						Type type = null;
						bool flag = false;
						RuntimeType.DispatchWrapperType dispatchWrapperType = (RuntimeType.DispatchWrapperType)(aWrapperTypes[i] & -65537);
						if (dispatchWrapperType <= RuntimeType.DispatchWrapperType.Dispatch)
						{
							if (dispatchWrapperType != RuntimeType.DispatchWrapperType.Unknown)
							{
								if (dispatchWrapperType == RuntimeType.DispatchWrapperType.Dispatch)
								{
									type = typeof(DispatchWrapper);
								}
							}
							else
							{
								type = typeof(UnknownWrapper);
							}
						}
						else if (dispatchWrapperType != RuntimeType.DispatchWrapperType.Error)
						{
							if (dispatchWrapperType != RuntimeType.DispatchWrapperType.Currency)
							{
								if (dispatchWrapperType == RuntimeType.DispatchWrapperType.BStr)
								{
									type = typeof(BStrWrapper);
									flag = true;
								}
							}
							else
							{
								type = typeof(CurrencyWrapper);
							}
						}
						else
						{
							type = typeof(ErrorWrapper);
						}
						Array array = (Array)aArgs[i];
						int length = array.Length;
						object[] array2 = (object[])Array.UnsafeCreateInstance(type, length);
						ConstructorInfo constructor;
						if (flag)
						{
							constructor = type.GetConstructor(new Type[]
							{
								typeof(string)
							});
						}
						else
						{
							constructor = type.GetConstructor(new Type[]
							{
								typeof(object)
							});
						}
						for (int j = 0; j < length; j++)
						{
							if (flag)
							{
								array2[j] = constructor.Invoke(new object[]
								{
									(string)array.GetValue(j)
								});
							}
							else
							{
								array2[j] = constructor.Invoke(new object[]
								{
									array.GetValue(j)
								});
							}
						}
						aArgs[i] = array2;
					}
					else
					{
						RuntimeType.DispatchWrapperType dispatchWrapperType = (RuntimeType.DispatchWrapperType)aWrapperTypes[i];
						if (dispatchWrapperType <= RuntimeType.DispatchWrapperType.Dispatch)
						{
							if (dispatchWrapperType != RuntimeType.DispatchWrapperType.Unknown)
							{
								if (dispatchWrapperType == RuntimeType.DispatchWrapperType.Dispatch)
								{
									aArgs[i] = new DispatchWrapper(aArgs[i]);
								}
							}
							else
							{
								aArgs[i] = new UnknownWrapper(aArgs[i]);
							}
						}
						else if (dispatchWrapperType != RuntimeType.DispatchWrapperType.Error)
						{
							if (dispatchWrapperType != RuntimeType.DispatchWrapperType.Currency)
							{
								if (dispatchWrapperType == RuntimeType.DispatchWrapperType.BStr)
								{
									aArgs[i] = new BStrWrapper((string)aArgs[i]);
								}
							}
							else
							{
								aArgs[i] = new CurrencyWrapper(aArgs[i]);
							}
						}
						else
						{
							aArgs[i] = new ErrorWrapper(aArgs[i]);
						}
					}
				}
			}
		}

		private OleAutBinder ForwardCallBinder
		{
			get
			{
				if (RuntimeType.s_ForwardCallBinder == null)
				{
					RuntimeType.s_ForwardCallBinder = new OleAutBinder();
				}
				return RuntimeType.s_ForwardCallBinder;
			}
		}

		private RemotingTypeCachedData m_cachedData;

		private object m_keepalive;

		private IntPtr m_cache;

		internal IntPtr m_handle;

		private INVOCATION_FLAGS m_invocationFlags;

		internal static readonly RuntimeType ValueType = (RuntimeType)typeof(ValueType);

		internal static readonly RuntimeType EnumType = (RuntimeType)typeof(Enum);

		private static readonly RuntimeType ObjectType = (RuntimeType)typeof(object);

		private static readonly RuntimeType StringType = (RuntimeType)typeof(string);

		private static readonly RuntimeType DelegateType = (RuntimeType)typeof(Delegate);

		private static Type[] s_SICtorParamTypes;

		private const BindingFlags MemberBindingMask = (BindingFlags)255;

		private const BindingFlags InvocationMask = BindingFlags.InvokeMethod | BindingFlags.CreateInstance | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty;

		private const BindingFlags BinderNonCreateInstance = BindingFlags.InvokeMethod | BindingFlags.GetField | BindingFlags.SetField | BindingFlags.GetProperty | BindingFlags.SetProperty;

		private const BindingFlags BinderGetSetProperty = BindingFlags.GetProperty | BindingFlags.SetProperty;

		private const BindingFlags BinderSetInvokeProperty = BindingFlags.InvokeMethod | BindingFlags.SetProperty;

		private const BindingFlags BinderGetSetField = BindingFlags.GetField | BindingFlags.SetField;

		private const BindingFlags BinderSetInvokeField = BindingFlags.InvokeMethod | BindingFlags.SetField;

		private const BindingFlags BinderNonFieldGetSet = (BindingFlags)16773888;

		private const BindingFlags ClassicBindingMask = BindingFlags.InvokeMethod | BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.PutDispProperty | BindingFlags.PutRefDispProperty;

		private static RuntimeType s_typedRef = (RuntimeType)typeof(TypedReference);

		private static volatile RuntimeType.ActivatorCache s_ActivatorCache;

		private static volatile OleAutBinder s_ForwardCallBinder;

		internal enum MemberListType
		{
			All,
			CaseSensitive,
			CaseInsensitive,
			HandleToInfo
		}

		private struct ListBuilder<T> where T : class
		{
			public ListBuilder(int capacity)
			{
				this._items = null;
				this._item = default(T);
				this._count = 0;
				this._capacity = capacity;
			}

			public T this[int index]
			{
				get
				{
					if (this._items == null)
					{
						return this._item;
					}
					return this._items[index];
				}
			}

			public T[] ToArray()
			{
				if (this._count == 0)
				{
					return EmptyArray<T>.Value;
				}
				if (this._count == 1)
				{
					return new T[]
					{
						this._item
					};
				}
				Array.Resize<T>(ref this._items, this._count);
				this._capacity = this._count;
				return this._items;
			}

			public void CopyTo(object[] array, int index)
			{
				if (this._count == 0)
				{
					return;
				}
				if (this._count == 1)
				{
					array[index] = this._item;
					return;
				}
				Array.Copy(this._items, 0, array, index, this._count);
			}

			public int Count
			{
				get
				{
					return this._count;
				}
			}

			public void Add(T item)
			{
				if (this._count == 0)
				{
					this._item = item;
				}
				else
				{
					if (this._count == 1)
					{
						if (this._capacity < 2)
						{
							this._capacity = 4;
						}
						this._items = new T[this._capacity];
						this._items[0] = this._item;
					}
					else if (this._capacity == this._count)
					{
						int num = 2 * this._capacity;
						Array.Resize<T>(ref this._items, num);
						this._capacity = num;
					}
					this._items[this._count] = item;
				}
				this._count++;
			}

			private T[] _items;

			private T _item;

			private int _count;

			private int _capacity;
		}

		internal class RuntimeTypeCache
		{
			internal RuntimeTypeCache(RuntimeType runtimeType)
			{
				this.m_typeCode = TypeCode.Empty;
				this.m_runtimeType = runtimeType;
				this.m_isGlobal = (RuntimeTypeHandle.GetModule(runtimeType).RuntimeType == runtimeType);
			}

			private string ConstructName(ref string name, TypeNameFormatFlags formatFlags)
			{
				if (name == null)
				{
					name = new RuntimeTypeHandle(this.m_runtimeType).ConstructName(formatFlags);
				}
				return name;
			}

			private T[] GetMemberList<T>(ref RuntimeType.RuntimeTypeCache.MemberInfoCache<T> m_cache, RuntimeType.MemberListType listType, string name, RuntimeType.RuntimeTypeCache.CacheType cacheType) where T : MemberInfo
			{
				RuntimeType.RuntimeTypeCache.MemberInfoCache<T> memberCache = this.GetMemberCache<T>(ref m_cache);
				return memberCache.GetMemberList(listType, name, cacheType);
			}

			private RuntimeType.RuntimeTypeCache.MemberInfoCache<T> GetMemberCache<T>(ref RuntimeType.RuntimeTypeCache.MemberInfoCache<T> m_cache) where T : MemberInfo
			{
				RuntimeType.RuntimeTypeCache.MemberInfoCache<T> memberInfoCache = m_cache;
				if (memberInfoCache == null)
				{
					RuntimeType.RuntimeTypeCache.MemberInfoCache<T> memberInfoCache2 = new RuntimeType.RuntimeTypeCache.MemberInfoCache<T>(this);
					memberInfoCache = Interlocked.CompareExchange<RuntimeType.RuntimeTypeCache.MemberInfoCache<T>>(ref m_cache, memberInfoCache2, null);
					if (memberInfoCache == null)
					{
						memberInfoCache = memberInfoCache2;
					}
				}
				return memberInfoCache;
			}

			internal object GenericCache
			{
				get
				{
					return this.m_genericCache;
				}
				set
				{
					this.m_genericCache = value;
				}
			}

			internal bool DomainInitialized
			{
				get
				{
					return this.m_bIsDomainInitialized;
				}
				set
				{
					this.m_bIsDomainInitialized = value;
				}
			}

			internal string GetName(TypeNameKind kind)
			{
				switch (kind)
				{
				case TypeNameKind.Name:
					return this.ConstructName(ref this.m_name, TypeNameFormatFlags.FormatBasic);
				case TypeNameKind.ToString:
					return this.ConstructName(ref this.m_toString, TypeNameFormatFlags.FormatNamespace);
				case TypeNameKind.SerializationName:
					return this.ConstructName(ref this.m_serializationname, TypeNameFormatFlags.FormatSerialization);
				case TypeNameKind.FullName:
					if (!this.m_runtimeType.GetRootElementType().IsGenericTypeDefinition && this.m_runtimeType.ContainsGenericParameters)
					{
						return null;
					}
					return this.ConstructName(ref this.m_fullname, (TypeNameFormatFlags)3);
				default:
					throw new InvalidOperationException();
				}
			}

			[SecuritySafeCritical]
			internal string GetNameSpace()
			{
				if (this.m_namespace == null)
				{
					Type type = this.m_runtimeType;
					type = type.GetRootElementType();
					while (type.IsNested)
					{
						type = type.DeclaringType;
					}
					this.m_namespace = RuntimeTypeHandle.GetMetadataImport((RuntimeType)type).GetNamespace(type.MetadataToken).ToString();
				}
				return this.m_namespace;
			}

			internal TypeCode TypeCode
			{
				get
				{
					return this.m_typeCode;
				}
				set
				{
					this.m_typeCode = value;
				}
			}

			[SecuritySafeCritical]
			internal RuntimeType GetEnclosingType()
			{
				if (this.m_enclosingType == null)
				{
					RuntimeType declaringType = RuntimeTypeHandle.GetDeclaringType(this.GetRuntimeType());
					this.m_enclosingType = (declaringType ?? ((RuntimeType)typeof(void)));
				}
				if (!(this.m_enclosingType == typeof(void)))
				{
					return this.m_enclosingType;
				}
				return null;
			}

			internal RuntimeType GetRuntimeType()
			{
				return this.m_runtimeType;
			}

			internal bool IsGlobal
			{
				[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
				get
				{
					return this.m_isGlobal;
				}
			}

			internal void InvalidateCachedNestedType()
			{
				this.m_nestedClassesCache = null;
			}

			internal RuntimeConstructorInfo GetSerializationCtor()
			{
				if (this.m_serializationCtor == null)
				{
					if (RuntimeType.s_SICtorParamTypes == null)
					{
						RuntimeType.s_SICtorParamTypes = new Type[]
						{
							typeof(SerializationInfo),
							typeof(StreamingContext)
						};
					}
					this.m_serializationCtor = (this.m_runtimeType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, CallingConventions.Any, RuntimeType.s_SICtorParamTypes, null) as RuntimeConstructorInfo);
				}
				return this.m_serializationCtor;
			}

			internal string GetDefaultMemberName()
			{
				if (this.m_defaultMemberName == null)
				{
					CustomAttributeData customAttributeData = null;
					Type typeFromHandle = typeof(DefaultMemberAttribute);
					RuntimeType runtimeType = this.m_runtimeType;
					while (runtimeType != null)
					{
						IList<CustomAttributeData> customAttributes = CustomAttributeData.GetCustomAttributes(runtimeType);
						for (int i = 0; i < customAttributes.Count; i++)
						{
							if (customAttributes[i].Constructor.DeclaringType == typeFromHandle)
							{
								customAttributeData = customAttributes[i];
								break;
							}
						}
						if (customAttributeData != null)
						{
							this.m_defaultMemberName = (customAttributeData.ConstructorArguments[0].Value as string);
							break;
						}
						runtimeType = runtimeType.GetBaseType();
					}
				}
				return this.m_defaultMemberName;
			}

			[SecurityCritical]
			internal MethodInfo GetGenericMethodInfo(RuntimeMethodHandleInternal genericMethod)
			{
				LoaderAllocator loaderAllocator = RuntimeMethodHandle.GetLoaderAllocator(genericMethod);
				RuntimeMethodInfo runtimeMethodInfo = new RuntimeMethodInfo(genericMethod, RuntimeMethodHandle.GetDeclaringType(genericMethod), this, RuntimeMethodHandle.GetAttributes(genericMethod), (BindingFlags)(-1), loaderAllocator);
				RuntimeMethodInfo runtimeMethodInfo2;
				if (loaderAllocator != null)
				{
					runtimeMethodInfo2 = loaderAllocator.m_methodInstantiations[runtimeMethodInfo];
				}
				else
				{
					runtimeMethodInfo2 = RuntimeType.RuntimeTypeCache.s_methodInstantiations[runtimeMethodInfo];
				}
				if (runtimeMethodInfo2 != null)
				{
					return runtimeMethodInfo2;
				}
				if (RuntimeType.RuntimeTypeCache.s_methodInstantiationsLock == null)
				{
					Interlocked.CompareExchange(ref RuntimeType.RuntimeTypeCache.s_methodInstantiationsLock, new object(), null);
				}
				bool flag = false;
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
					Monitor.Enter(RuntimeType.RuntimeTypeCache.s_methodInstantiationsLock, ref flag);
					if (loaderAllocator != null)
					{
						runtimeMethodInfo2 = loaderAllocator.m_methodInstantiations[runtimeMethodInfo];
						if (runtimeMethodInfo2 != null)
						{
							return runtimeMethodInfo2;
						}
						loaderAllocator.m_methodInstantiations[runtimeMethodInfo] = runtimeMethodInfo;
					}
					else
					{
						runtimeMethodInfo2 = RuntimeType.RuntimeTypeCache.s_methodInstantiations[runtimeMethodInfo];
						if (runtimeMethodInfo2 != null)
						{
							return runtimeMethodInfo2;
						}
						RuntimeType.RuntimeTypeCache.s_methodInstantiations[runtimeMethodInfo] = runtimeMethodInfo;
					}
				}
				finally
				{
					if (flag)
					{
						Monitor.Exit(RuntimeType.RuntimeTypeCache.s_methodInstantiationsLock);
					}
				}
				return runtimeMethodInfo;
			}

			internal RuntimeMethodInfo[] GetMethodList(RuntimeType.MemberListType listType, string name)
			{
				return this.GetMemberList<RuntimeMethodInfo>(ref this.m_methodInfoCache, listType, name, RuntimeType.RuntimeTypeCache.CacheType.Method);
			}

			internal RuntimeConstructorInfo[] GetConstructorList(RuntimeType.MemberListType listType, string name)
			{
				return this.GetMemberList<RuntimeConstructorInfo>(ref this.m_constructorInfoCache, listType, name, RuntimeType.RuntimeTypeCache.CacheType.Constructor);
			}

			internal RuntimePropertyInfo[] GetPropertyList(RuntimeType.MemberListType listType, string name)
			{
				return this.GetMemberList<RuntimePropertyInfo>(ref this.m_propertyInfoCache, listType, name, RuntimeType.RuntimeTypeCache.CacheType.Property);
			}

			internal RuntimeEventInfo[] GetEventList(RuntimeType.MemberListType listType, string name)
			{
				return this.GetMemberList<RuntimeEventInfo>(ref this.m_eventInfoCache, listType, name, RuntimeType.RuntimeTypeCache.CacheType.Event);
			}

			internal RuntimeFieldInfo[] GetFieldList(RuntimeType.MemberListType listType, string name)
			{
				return this.GetMemberList<RuntimeFieldInfo>(ref this.m_fieldInfoCache, listType, name, RuntimeType.RuntimeTypeCache.CacheType.Field);
			}

			internal RuntimeType[] GetInterfaceList(RuntimeType.MemberListType listType, string name)
			{
				return this.GetMemberList<RuntimeType>(ref this.m_interfaceCache, listType, name, RuntimeType.RuntimeTypeCache.CacheType.Interface);
			}

			internal RuntimeType[] GetNestedTypeList(RuntimeType.MemberListType listType, string name)
			{
				return this.GetMemberList<RuntimeType>(ref this.m_nestedClassesCache, listType, name, RuntimeType.RuntimeTypeCache.CacheType.NestedType);
			}

			internal MethodBase GetMethod(RuntimeType declaringType, RuntimeMethodHandleInternal method)
			{
				this.GetMemberCache<RuntimeMethodInfo>(ref this.m_methodInfoCache);
				return this.m_methodInfoCache.AddMethod(declaringType, method, RuntimeType.RuntimeTypeCache.CacheType.Method);
			}

			internal MethodBase GetConstructor(RuntimeType declaringType, RuntimeMethodHandleInternal constructor)
			{
				this.GetMemberCache<RuntimeConstructorInfo>(ref this.m_constructorInfoCache);
				return this.m_constructorInfoCache.AddMethod(declaringType, constructor, RuntimeType.RuntimeTypeCache.CacheType.Constructor);
			}

			internal FieldInfo GetField(RuntimeFieldHandleInternal field)
			{
				this.GetMemberCache<RuntimeFieldInfo>(ref this.m_fieldInfoCache);
				return this.m_fieldInfoCache.AddField(field);
			}

			private const int MAXNAMELEN = 1024;

			private RuntimeType m_runtimeType;

			private RuntimeType m_enclosingType;

			private TypeCode m_typeCode;

			private string m_name;

			private string m_fullname;

			private string m_toString;

			private string m_namespace;

			private string m_serializationname;

			private bool m_isGlobal;

			private bool m_bIsDomainInitialized;

			private RuntimeType.RuntimeTypeCache.MemberInfoCache<RuntimeMethodInfo> m_methodInfoCache;

			private RuntimeType.RuntimeTypeCache.MemberInfoCache<RuntimeConstructorInfo> m_constructorInfoCache;

			private RuntimeType.RuntimeTypeCache.MemberInfoCache<RuntimeFieldInfo> m_fieldInfoCache;

			private RuntimeType.RuntimeTypeCache.MemberInfoCache<RuntimeType> m_interfaceCache;

			private RuntimeType.RuntimeTypeCache.MemberInfoCache<RuntimeType> m_nestedClassesCache;

			private RuntimeType.RuntimeTypeCache.MemberInfoCache<RuntimePropertyInfo> m_propertyInfoCache;

			private RuntimeType.RuntimeTypeCache.MemberInfoCache<RuntimeEventInfo> m_eventInfoCache;

			private static CerHashtable<RuntimeMethodInfo, RuntimeMethodInfo> s_methodInstantiations;

			private static object s_methodInstantiationsLock;

			private RuntimeConstructorInfo m_serializationCtor;

			private string m_defaultMemberName;

			private object m_genericCache;

			internal enum CacheType
			{
				Method,
				Constructor,
				Field,
				Property,
				Event,
				Interface,
				NestedType
			}

			private struct Filter
			{
				[SecurityCritical]
				public unsafe Filter(byte* pUtf8Name, int cUtf8Name, RuntimeType.MemberListType listType)
				{
					this.m_name = new Utf8String((void*)pUtf8Name, cUtf8Name);
					this.m_listType = listType;
					this.m_nameHash = 0U;
					if (this.RequiresStringComparison())
					{
						this.m_nameHash = this.m_name.HashCaseInsensitive();
					}
				}

				public bool Match(Utf8String name)
				{
					bool result = true;
					if (this.m_listType == RuntimeType.MemberListType.CaseSensitive)
					{
						result = this.m_name.Equals(name);
					}
					else if (this.m_listType == RuntimeType.MemberListType.CaseInsensitive)
					{
						result = this.m_name.EqualsCaseInsensitive(name);
					}
					return result;
				}

				public bool RequiresStringComparison()
				{
					return this.m_listType == RuntimeType.MemberListType.CaseSensitive || this.m_listType == RuntimeType.MemberListType.CaseInsensitive;
				}

				public bool CaseSensitive()
				{
					return this.m_listType == RuntimeType.MemberListType.CaseSensitive;
				}

				public uint GetHashToMatch()
				{
					return this.m_nameHash;
				}

				private Utf8String m_name;

				private RuntimeType.MemberListType m_listType;

				private uint m_nameHash;
			}

			private class MemberInfoCache<T> where T : MemberInfo
			{
				[SecuritySafeCritical]
				internal MemberInfoCache(RuntimeType.RuntimeTypeCache runtimeTypeCache)
				{
					Mda.MemberInfoCacheCreation();
					this.m_runtimeTypeCache = runtimeTypeCache;
				}

				[SecuritySafeCritical]
				internal MethodBase AddMethod(RuntimeType declaringType, RuntimeMethodHandleInternal method, RuntimeType.RuntimeTypeCache.CacheType cacheType)
				{
					T[] array = null;
					MethodAttributes attributes = RuntimeMethodHandle.GetAttributes(method);
					bool isPublic = (attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
					bool isStatic = (attributes & MethodAttributes.Static) > MethodAttributes.PrivateScope;
					bool isInherited = declaringType != this.ReflectedType;
					BindingFlags bindingFlags = RuntimeType.FilterPreCalculate(isPublic, isInherited, isStatic);
					if (cacheType != RuntimeType.RuntimeTypeCache.CacheType.Method)
					{
						if (cacheType == RuntimeType.RuntimeTypeCache.CacheType.Constructor)
						{
							array = (T[])new RuntimeConstructorInfo[]
							{
								new RuntimeConstructorInfo(method, declaringType, this.m_runtimeTypeCache, attributes, bindingFlags)
							};
						}
					}
					else
					{
						array = (T[])new RuntimeMethodInfo[]
						{
							new RuntimeMethodInfo(method, declaringType, this.m_runtimeTypeCache, attributes, bindingFlags, null)
						};
					}
					this.Insert(ref array, null, RuntimeType.MemberListType.HandleToInfo);
					return (MethodBase)((object)array[0]);
				}

				[SecuritySafeCritical]
				internal FieldInfo AddField(RuntimeFieldHandleInternal field)
				{
					FieldAttributes attributes = RuntimeFieldHandle.GetAttributes(field);
					bool isPublic = (attributes & FieldAttributes.FieldAccessMask) == FieldAttributes.Public;
					bool isStatic = (attributes & FieldAttributes.Static) > FieldAttributes.PrivateScope;
					RuntimeType approxDeclaringType = RuntimeFieldHandle.GetApproxDeclaringType(field);
					bool isInherited = RuntimeFieldHandle.AcquiresContextFromThis(field) ? (!RuntimeTypeHandle.CompareCanonicalHandles(approxDeclaringType, this.ReflectedType)) : (approxDeclaringType != this.ReflectedType);
					BindingFlags bindingFlags = RuntimeType.FilterPreCalculate(isPublic, isInherited, isStatic);
					T[] array = (T[])new RuntimeFieldInfo[]
					{
						new RtFieldInfo(field, this.ReflectedType, this.m_runtimeTypeCache, bindingFlags)
					};
					this.Insert(ref array, null, RuntimeType.MemberListType.HandleToInfo);
					return (FieldInfo)((object)array[0]);
				}

				[SecuritySafeCritical]
				private unsafe T[] Populate(string name, RuntimeType.MemberListType listType, RuntimeType.RuntimeTypeCache.CacheType cacheType)
				{
					T[] result = null;
					if (name == null || name.Length == 0 || (cacheType == RuntimeType.RuntimeTypeCache.CacheType.Constructor && name.FirstChar != '.' && name.FirstChar != '*'))
					{
						result = this.GetListByName(null, 0, null, 0, listType, cacheType);
					}
					else
					{
						int length = name.Length;
						fixed (string text = name)
						{
							char* ptr = text;
							if (ptr != null)
							{
								ptr += RuntimeHelpers.OffsetToStringData / 2;
							}
							int byteCount = Encoding.UTF8.GetByteCount(ptr, length);
							if (byteCount > 1024)
							{
								fixed (byte* ptr2 = new byte[byteCount])
								{
									result = this.GetListByName(ptr, length, ptr2, byteCount, listType, cacheType);
								}
							}
							else
							{
								byte* pUtf8Name = stackalloc byte[checked(unchecked((UIntPtr)byteCount) * 1)];
								result = this.GetListByName(ptr, length, pUtf8Name, byteCount, listType, cacheType);
							}
						}
					}
					this.Insert(ref result, name, listType);
					return result;
				}

				[SecurityCritical]
				private unsafe T[] GetListByName(char* pName, int cNameLen, byte* pUtf8Name, int cUtf8Name, RuntimeType.MemberListType listType, RuntimeType.RuntimeTypeCache.CacheType cacheType)
				{
					if (cNameLen != 0)
					{
						Encoding.UTF8.GetBytes(pName, cNameLen, pUtf8Name, cUtf8Name);
					}
					RuntimeType.RuntimeTypeCache.Filter filter = new RuntimeType.RuntimeTypeCache.Filter(pUtf8Name, cUtf8Name, listType);
					object obj = null;
					switch (cacheType)
					{
					case RuntimeType.RuntimeTypeCache.CacheType.Method:
						obj = this.PopulateMethods(filter);
						break;
					case RuntimeType.RuntimeTypeCache.CacheType.Constructor:
						obj = this.PopulateConstructors(filter);
						break;
					case RuntimeType.RuntimeTypeCache.CacheType.Field:
						obj = this.PopulateFields(filter);
						break;
					case RuntimeType.RuntimeTypeCache.CacheType.Property:
						obj = this.PopulateProperties(filter);
						break;
					case RuntimeType.RuntimeTypeCache.CacheType.Event:
						obj = this.PopulateEvents(filter);
						break;
					case RuntimeType.RuntimeTypeCache.CacheType.Interface:
						obj = this.PopulateInterfaces(filter);
						break;
					case RuntimeType.RuntimeTypeCache.CacheType.NestedType:
						obj = this.PopulateNestedClasses(filter);
						break;
					}
					return (T[])obj;
				}

				[SecuritySafeCritical]
				[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
				internal void Insert(ref T[] list, string name, RuntimeType.MemberListType listType)
				{
					bool flag = false;
					RuntimeHelpers.PrepareConstrainedRegions();
					try
					{
						Monitor.Enter(this, ref flag);
						switch (listType)
						{
						case RuntimeType.MemberListType.All:
							if (!this.m_cacheComplete)
							{
								this.MergeWithGlobalList(list);
								int num = this.m_allMembers.Length;
								while (num > 0 && !(this.m_allMembers[num - 1] != null))
								{
									num--;
								}
								Array.Resize<T>(ref this.m_allMembers, num);
								Volatile.Write(ref this.m_cacheComplete, true);
							}
							else
							{
								list = this.m_allMembers;
							}
							break;
						case RuntimeType.MemberListType.CaseSensitive:
						{
							T[] array = this.m_csMemberInfos[name];
							if (array == null)
							{
								this.MergeWithGlobalList(list);
								this.m_csMemberInfos[name] = list;
							}
							else
							{
								list = array;
							}
							break;
						}
						case RuntimeType.MemberListType.CaseInsensitive:
						{
							T[] array2 = this.m_cisMemberInfos[name];
							if (array2 == null)
							{
								this.MergeWithGlobalList(list);
								this.m_cisMemberInfos[name] = list;
							}
							else
							{
								list = array2;
							}
							break;
						}
						default:
							this.MergeWithGlobalList(list);
							break;
						}
					}
					finally
					{
						if (flag)
						{
							Monitor.Exit(this);
						}
					}
				}

				[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
				private void MergeWithGlobalList(T[] list)
				{
					T[] array = this.m_allMembers;
					if (array == null)
					{
						this.m_allMembers = list;
						return;
					}
					int num = array.Length;
					int num2 = 0;
					for (int i = 0; i < list.Length; i++)
					{
						T t = list[i];
						bool flag = false;
						int j;
						for (j = 0; j < num; j++)
						{
							T t2 = array[j];
							if (t2 == null)
							{
								break;
							}
							if (t.CacheEquals(t2))
							{
								list[i] = t2;
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							if (num2 == 0)
							{
								num2 = j;
							}
							if (num2 >= array.Length)
							{
								int newSize;
								if (this.m_cacheComplete)
								{
									newSize = array.Length + 1;
								}
								else
								{
									newSize = Math.Max(Math.Max(4, 2 * array.Length), list.Length);
								}
								T[] array2 = array;
								Array.Resize<T>(ref array2, newSize);
								array = array2;
							}
							array[num2] = t;
							num2++;
						}
					}
					this.m_allMembers = array;
				}

				[SecuritySafeCritical]
				private unsafe RuntimeMethodInfo[] PopulateMethods(RuntimeType.RuntimeTypeCache.Filter filter)
				{
					RuntimeType.ListBuilder<RuntimeMethodInfo> listBuilder = default(RuntimeType.ListBuilder<RuntimeMethodInfo>);
					RuntimeType runtimeType = this.ReflectedType;
					if (RuntimeTypeHandle.IsInterface(runtimeType))
					{
						foreach (RuntimeMethodHandleInternal method in RuntimeTypeHandle.GetIntroducedMethods(runtimeType))
						{
							if (!filter.RequiresStringComparison() || (RuntimeMethodHandle.MatchesNameHash(method, filter.GetHashToMatch()) && filter.Match(RuntimeMethodHandle.GetUtf8Name(method))))
							{
								MethodAttributes attributes = RuntimeMethodHandle.GetAttributes(method);
								bool isPublic = (attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
								bool isStatic = (attributes & MethodAttributes.Static) > MethodAttributes.PrivateScope;
								bool isInherited = false;
								BindingFlags bindingFlags = RuntimeType.FilterPreCalculate(isPublic, isInherited, isStatic);
								if ((attributes & MethodAttributes.RTSpecialName) == MethodAttributes.PrivateScope)
								{
									RuntimeMethodHandleInternal stubIfNeeded = RuntimeMethodHandle.GetStubIfNeeded(method, runtimeType, null);
									RuntimeMethodInfo item = new RuntimeMethodInfo(stubIfNeeded, runtimeType, this.m_runtimeTypeCache, attributes, bindingFlags, null);
									listBuilder.Add(item);
								}
							}
						}
					}
					else
					{
						while (RuntimeTypeHandle.IsGenericVariable(runtimeType))
						{
							runtimeType = runtimeType.GetBaseType();
						}
						bool* ptr = stackalloc bool[checked(unchecked((UIntPtr)RuntimeTypeHandle.GetNumVirtuals(runtimeType)) * 1)];
						bool isValueType = runtimeType.IsValueType;
						do
						{
							int numVirtuals = RuntimeTypeHandle.GetNumVirtuals(runtimeType);
							foreach (RuntimeMethodHandleInternal method2 in RuntimeTypeHandle.GetIntroducedMethods(runtimeType))
							{
								if (!filter.RequiresStringComparison() || (RuntimeMethodHandle.MatchesNameHash(method2, filter.GetHashToMatch()) && filter.Match(RuntimeMethodHandle.GetUtf8Name(method2))))
								{
									MethodAttributes attributes2 = RuntimeMethodHandle.GetAttributes(method2);
									MethodAttributes methodAttributes = attributes2 & MethodAttributes.MemberAccessMask;
									if ((attributes2 & MethodAttributes.RTSpecialName) == MethodAttributes.PrivateScope)
									{
										bool flag = false;
										int num = 0;
										if ((attributes2 & MethodAttributes.Virtual) != MethodAttributes.PrivateScope)
										{
											num = RuntimeMethodHandle.GetSlot(method2);
											flag = (num < numVirtuals);
										}
										bool flag2 = runtimeType != this.ReflectedType;
										if (!CompatibilitySwitches.IsAppEarlierThanWindowsPhone8)
										{
											bool flag3 = methodAttributes == MethodAttributes.Private;
											if (flag2 && flag3 && !flag)
											{
												continue;
											}
										}
										if (flag)
										{
											if (ptr[num])
											{
												continue;
											}
											ptr[num] = true;
										}
										else if (isValueType && (attributes2 & (MethodAttributes.Virtual | MethodAttributes.Abstract)) != MethodAttributes.PrivateScope)
										{
											continue;
										}
										bool isPublic2 = methodAttributes == MethodAttributes.Public;
										bool isStatic2 = (attributes2 & MethodAttributes.Static) > MethodAttributes.PrivateScope;
										BindingFlags bindingFlags2 = RuntimeType.FilterPreCalculate(isPublic2, flag2, isStatic2);
										RuntimeMethodHandleInternal stubIfNeeded2 = RuntimeMethodHandle.GetStubIfNeeded(method2, runtimeType, null);
										RuntimeMethodInfo item2 = new RuntimeMethodInfo(stubIfNeeded2, runtimeType, this.m_runtimeTypeCache, attributes2, bindingFlags2, null);
										listBuilder.Add(item2);
									}
								}
							}
							runtimeType = RuntimeTypeHandle.GetBaseType(runtimeType);
						}
						while (runtimeType != null);
					}
					return listBuilder.ToArray();
				}

				[SecuritySafeCritical]
				private RuntimeConstructorInfo[] PopulateConstructors(RuntimeType.RuntimeTypeCache.Filter filter)
				{
					if (this.ReflectedType.IsGenericParameter)
					{
						return EmptyArray<RuntimeConstructorInfo>.Value;
					}
					RuntimeType.ListBuilder<RuntimeConstructorInfo> listBuilder = default(RuntimeType.ListBuilder<RuntimeConstructorInfo>);
					RuntimeType reflectedType = this.ReflectedType;
					foreach (RuntimeMethodHandleInternal method in RuntimeTypeHandle.GetIntroducedMethods(reflectedType))
					{
						if (!filter.RequiresStringComparison() || (RuntimeMethodHandle.MatchesNameHash(method, filter.GetHashToMatch()) && filter.Match(RuntimeMethodHandle.GetUtf8Name(method))))
						{
							MethodAttributes attributes = RuntimeMethodHandle.GetAttributes(method);
							if ((attributes & MethodAttributes.RTSpecialName) != MethodAttributes.PrivateScope)
							{
								bool isPublic = (attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Public;
								bool isStatic = (attributes & MethodAttributes.Static) > MethodAttributes.PrivateScope;
								bool isInherited = false;
								BindingFlags bindingFlags = RuntimeType.FilterPreCalculate(isPublic, isInherited, isStatic);
								RuntimeMethodHandleInternal stubIfNeeded = RuntimeMethodHandle.GetStubIfNeeded(method, reflectedType, null);
								RuntimeConstructorInfo item = new RuntimeConstructorInfo(stubIfNeeded, this.ReflectedType, this.m_runtimeTypeCache, attributes, bindingFlags);
								listBuilder.Add(item);
							}
						}
					}
					return listBuilder.ToArray();
				}

				[SecuritySafeCritical]
				private RuntimeFieldInfo[] PopulateFields(RuntimeType.RuntimeTypeCache.Filter filter)
				{
					RuntimeType.ListBuilder<RuntimeFieldInfo> listBuilder = default(RuntimeType.ListBuilder<RuntimeFieldInfo>);
					RuntimeType runtimeType = this.ReflectedType;
					while (RuntimeTypeHandle.IsGenericVariable(runtimeType))
					{
						runtimeType = runtimeType.GetBaseType();
					}
					while (runtimeType != null)
					{
						this.PopulateRtFields(filter, runtimeType, ref listBuilder);
						this.PopulateLiteralFields(filter, runtimeType, ref listBuilder);
						runtimeType = RuntimeTypeHandle.GetBaseType(runtimeType);
					}
					if (this.ReflectedType.IsGenericParameter)
					{
						Type[] interfaces = this.ReflectedType.BaseType.GetInterfaces();
						for (int i = 0; i < interfaces.Length; i++)
						{
							this.PopulateLiteralFields(filter, (RuntimeType)interfaces[i], ref listBuilder);
							this.PopulateRtFields(filter, (RuntimeType)interfaces[i], ref listBuilder);
						}
					}
					else
					{
						Type[] interfaces2 = RuntimeTypeHandle.GetInterfaces(this.ReflectedType);
						if (interfaces2 != null)
						{
							for (int j = 0; j < interfaces2.Length; j++)
							{
								this.PopulateLiteralFields(filter, (RuntimeType)interfaces2[j], ref listBuilder);
								this.PopulateRtFields(filter, (RuntimeType)interfaces2[j], ref listBuilder);
							}
						}
					}
					return listBuilder.ToArray();
				}

				[SecuritySafeCritical]
				private unsafe void PopulateRtFields(RuntimeType.RuntimeTypeCache.Filter filter, RuntimeType declaringType, ref RuntimeType.ListBuilder<RuntimeFieldInfo> list)
				{
					IntPtr* ptr = stackalloc IntPtr[checked(unchecked((UIntPtr)64) * (UIntPtr)sizeof(IntPtr))];
					int num = 64;
					if (!RuntimeTypeHandle.GetFields(declaringType, ptr, &num))
					{
						fixed (IntPtr* ptr2 = new IntPtr[num])
						{
							RuntimeTypeHandle.GetFields(declaringType, ptr2, &num);
							this.PopulateRtFields(filter, ptr2, num, declaringType, ref list);
						}
						return;
					}
					if (num > 0)
					{
						this.PopulateRtFields(filter, ptr, num, declaringType, ref list);
					}
				}

				[SecurityCritical]
				private unsafe void PopulateRtFields(RuntimeType.RuntimeTypeCache.Filter filter, IntPtr* ppFieldHandles, int count, RuntimeType declaringType, ref RuntimeType.ListBuilder<RuntimeFieldInfo> list)
				{
					bool flag = RuntimeTypeHandle.HasInstantiation(declaringType) && !RuntimeTypeHandle.ContainsGenericVariables(declaringType);
					bool flag2 = declaringType != this.ReflectedType;
					for (int i = 0; i < count; i++)
					{
						RuntimeFieldHandleInternal staticFieldForGenericType = new RuntimeFieldHandleInternal(ppFieldHandles[i]);
						if (!filter.RequiresStringComparison() || (RuntimeFieldHandle.MatchesNameHash(staticFieldForGenericType, filter.GetHashToMatch()) && filter.Match(RuntimeFieldHandle.GetUtf8Name(staticFieldForGenericType))))
						{
							FieldAttributes attributes = RuntimeFieldHandle.GetAttributes(staticFieldForGenericType);
							FieldAttributes fieldAttributes = attributes & FieldAttributes.FieldAccessMask;
							if (!flag2 || fieldAttributes != FieldAttributes.Private)
							{
								bool isPublic = fieldAttributes == FieldAttributes.Public;
								bool flag3 = (attributes & FieldAttributes.Static) > FieldAttributes.PrivateScope;
								BindingFlags bindingFlags = RuntimeType.FilterPreCalculate(isPublic, flag2, flag3);
								if (flag && flag3)
								{
									staticFieldForGenericType = RuntimeFieldHandle.GetStaticFieldForGenericType(staticFieldForGenericType, declaringType);
								}
								RuntimeFieldInfo item = new RtFieldInfo(staticFieldForGenericType, declaringType, this.m_runtimeTypeCache, bindingFlags);
								list.Add(item);
							}
						}
					}
				}

				[SecuritySafeCritical]
				private void PopulateLiteralFields(RuntimeType.RuntimeTypeCache.Filter filter, RuntimeType declaringType, ref RuntimeType.ListBuilder<RuntimeFieldInfo> list)
				{
					int token = RuntimeTypeHandle.GetToken(declaringType);
					if (System.Reflection.MetadataToken.IsNullToken(token))
					{
						return;
					}
					MetadataImport metadataImport = RuntimeTypeHandle.GetMetadataImport(declaringType);
					MetadataEnumResult metadataEnumResult;
					metadataImport.EnumFields(token, out metadataEnumResult);
					for (int i = 0; i < metadataEnumResult.Length; i++)
					{
						int num = metadataEnumResult[i];
						FieldAttributes fieldAttributes;
						metadataImport.GetFieldDefProps(num, out fieldAttributes);
						FieldAttributes fieldAttributes2 = fieldAttributes & FieldAttributes.FieldAccessMask;
						if ((fieldAttributes & FieldAttributes.Literal) != FieldAttributes.PrivateScope)
						{
							bool flag = declaringType != this.ReflectedType;
							if (!flag || fieldAttributes2 != FieldAttributes.Private)
							{
								if (filter.RequiresStringComparison())
								{
									Utf8String name = metadataImport.GetName(num);
									if (!filter.Match(name))
									{
										goto IL_C5;
									}
								}
								bool isPublic = fieldAttributes2 == FieldAttributes.Public;
								bool isStatic = (fieldAttributes & FieldAttributes.Static) > FieldAttributes.PrivateScope;
								BindingFlags bindingFlags = RuntimeType.FilterPreCalculate(isPublic, flag, isStatic);
								RuntimeFieldInfo item = new MdFieldInfo(num, fieldAttributes, declaringType.GetTypeHandleInternal(), this.m_runtimeTypeCache, bindingFlags);
								list.Add(item);
							}
						}
						IL_C5:;
					}
				}

				private static void AddElementTypes(Type template, IList<Type> types)
				{
					if (!template.HasElementType)
					{
						return;
					}
					RuntimeType.RuntimeTypeCache.MemberInfoCache<T>.AddElementTypes(template.GetElementType(), types);
					for (int i = 0; i < types.Count; i++)
					{
						if (template.IsArray)
						{
							if (template.IsSzArray)
							{
								types[i] = types[i].MakeArrayType();
							}
							else
							{
								types[i] = types[i].MakeArrayType(template.GetArrayRank());
							}
						}
						else if (template.IsPointer)
						{
							types[i] = types[i].MakePointerType();
						}
					}
				}

				private void AddSpecialInterface(ref RuntimeType.ListBuilder<RuntimeType> list, RuntimeType.RuntimeTypeCache.Filter filter, RuntimeType iList, bool addSubInterface)
				{
					if (iList.IsAssignableFrom(this.ReflectedType))
					{
						if (filter.Match(RuntimeTypeHandle.GetUtf8Name(iList)))
						{
							list.Add(iList);
						}
						if (addSubInterface)
						{
							foreach (RuntimeType runtimeType in iList.GetInterfaces())
							{
								if (runtimeType.IsGenericType && filter.Match(RuntimeTypeHandle.GetUtf8Name(runtimeType)))
								{
									list.Add(runtimeType);
								}
							}
						}
					}
				}

				[SecuritySafeCritical]
				private RuntimeType[] PopulateInterfaces(RuntimeType.RuntimeTypeCache.Filter filter)
				{
					RuntimeType.ListBuilder<RuntimeType> listBuilder = default(RuntimeType.ListBuilder<RuntimeType>);
					RuntimeType reflectedType = this.ReflectedType;
					if (!RuntimeTypeHandle.IsGenericVariable(reflectedType))
					{
						Type[] interfaces = RuntimeTypeHandle.GetInterfaces(reflectedType);
						if (interfaces != null)
						{
							foreach (RuntimeType runtimeType in interfaces)
							{
								if (!filter.RequiresStringComparison() || filter.Match(RuntimeTypeHandle.GetUtf8Name(runtimeType)))
								{
									listBuilder.Add(runtimeType);
								}
							}
						}
						if (this.ReflectedType.IsSzArray)
						{
							RuntimeType runtimeType2 = (RuntimeType)this.ReflectedType.GetElementType();
							if (!runtimeType2.IsPointer)
							{
								this.AddSpecialInterface(ref listBuilder, filter, (RuntimeType)typeof(IList<>).MakeGenericType(new Type[]
								{
									runtimeType2
								}), true);
								this.AddSpecialInterface(ref listBuilder, filter, (RuntimeType)typeof(IReadOnlyList<>).MakeGenericType(new Type[]
								{
									runtimeType2
								}), false);
								this.AddSpecialInterface(ref listBuilder, filter, (RuntimeType)typeof(IReadOnlyCollection<>).MakeGenericType(new Type[]
								{
									runtimeType2
								}), false);
							}
						}
					}
					else
					{
						List<RuntimeType> list = new List<RuntimeType>();
						foreach (RuntimeType runtimeType3 in reflectedType.GetGenericParameterConstraints())
						{
							if (runtimeType3.IsInterface)
							{
								list.Add(runtimeType3);
							}
							Type[] interfaces2 = runtimeType3.GetInterfaces();
							for (int k = 0; k < interfaces2.Length; k++)
							{
								list.Add(interfaces2[k] as RuntimeType);
							}
						}
						Dictionary<RuntimeType, RuntimeType> dictionary = new Dictionary<RuntimeType, RuntimeType>();
						for (int l = 0; l < list.Count; l++)
						{
							RuntimeType runtimeType4 = list[l];
							if (!dictionary.ContainsKey(runtimeType4))
							{
								dictionary[runtimeType4] = runtimeType4;
							}
						}
						RuntimeType[] array = new RuntimeType[dictionary.Values.Count];
						dictionary.Values.CopyTo(array, 0);
						for (int m = 0; m < array.Length; m++)
						{
							if (!filter.RequiresStringComparison() || filter.Match(RuntimeTypeHandle.GetUtf8Name(array[m])))
							{
								listBuilder.Add(array[m]);
							}
						}
					}
					return listBuilder.ToArray();
				}

				[SecuritySafeCritical]
				private RuntimeType[] PopulateNestedClasses(RuntimeType.RuntimeTypeCache.Filter filter)
				{
					RuntimeType runtimeType = this.ReflectedType;
					while (RuntimeTypeHandle.IsGenericVariable(runtimeType))
					{
						runtimeType = runtimeType.GetBaseType();
					}
					int token = RuntimeTypeHandle.GetToken(runtimeType);
					if (System.Reflection.MetadataToken.IsNullToken(token))
					{
						return EmptyArray<RuntimeType>.Value;
					}
					RuntimeType.ListBuilder<RuntimeType> listBuilder = default(RuntimeType.ListBuilder<RuntimeType>);
					RuntimeModule module = RuntimeTypeHandle.GetModule(runtimeType);
					MetadataEnumResult metadataEnumResult;
					ModuleHandle.GetMetadataImport(module).EnumNestedTypes(token, out metadataEnumResult);
					int i = 0;
					while (i < metadataEnumResult.Length)
					{
						RuntimeType runtimeType2 = null;
						try
						{
							runtimeType2 = ModuleHandle.ResolveTypeHandleInternal(module, metadataEnumResult[i], null, null);
						}
						catch (TypeLoadException)
						{
							goto IL_90;
						}
						goto IL_6E;
						IL_90:
						i++;
						continue;
						IL_6E:
						if (!filter.RequiresStringComparison() || filter.Match(RuntimeTypeHandle.GetUtf8Name(runtimeType2)))
						{
							listBuilder.Add(runtimeType2);
							goto IL_90;
						}
						goto IL_90;
					}
					return listBuilder.ToArray();
				}

				[SecuritySafeCritical]
				private RuntimeEventInfo[] PopulateEvents(RuntimeType.RuntimeTypeCache.Filter filter)
				{
					Dictionary<string, RuntimeEventInfo> csEventInfos = filter.CaseSensitive() ? null : new Dictionary<string, RuntimeEventInfo>();
					RuntimeType runtimeType = this.ReflectedType;
					RuntimeType.ListBuilder<RuntimeEventInfo> listBuilder = default(RuntimeType.ListBuilder<RuntimeEventInfo>);
					if (!RuntimeTypeHandle.IsInterface(runtimeType))
					{
						while (RuntimeTypeHandle.IsGenericVariable(runtimeType))
						{
							runtimeType = runtimeType.GetBaseType();
						}
						while (runtimeType != null)
						{
							this.PopulateEvents(filter, runtimeType, csEventInfos, ref listBuilder);
							runtimeType = RuntimeTypeHandle.GetBaseType(runtimeType);
						}
					}
					else
					{
						this.PopulateEvents(filter, runtimeType, csEventInfos, ref listBuilder);
					}
					return listBuilder.ToArray();
				}

				[SecuritySafeCritical]
				private void PopulateEvents(RuntimeType.RuntimeTypeCache.Filter filter, RuntimeType declaringType, Dictionary<string, RuntimeEventInfo> csEventInfos, ref RuntimeType.ListBuilder<RuntimeEventInfo> list)
				{
					int token = RuntimeTypeHandle.GetToken(declaringType);
					if (System.Reflection.MetadataToken.IsNullToken(token))
					{
						return;
					}
					MetadataImport metadataImport = RuntimeTypeHandle.GetMetadataImport(declaringType);
					MetadataEnumResult metadataEnumResult;
					metadataImport.EnumEvents(token, out metadataEnumResult);
					int i = 0;
					while (i < metadataEnumResult.Length)
					{
						int num = metadataEnumResult[i];
						if (!filter.RequiresStringComparison())
						{
							goto IL_51;
						}
						Utf8String name = metadataImport.GetName(num);
						if (filter.Match(name))
						{
							goto IL_51;
						}
						IL_B4:
						i++;
						continue;
						IL_51:
						bool flag;
						RuntimeEventInfo runtimeEventInfo = new RuntimeEventInfo(num, declaringType, this.m_runtimeTypeCache, ref flag);
						if (!(declaringType != this.m_runtimeTypeCache.GetRuntimeType()) || !flag)
						{
							if (csEventInfos != null)
							{
								string name2 = runtimeEventInfo.Name;
								if (csEventInfos.GetValueOrDefault(name2) != null)
								{
									goto IL_B4;
								}
								csEventInfos[name2] = runtimeEventInfo;
							}
							else if (list.Count > 0)
							{
								break;
							}
							list.Add(runtimeEventInfo);
							goto IL_B4;
						}
						goto IL_B4;
					}
				}

				[SecuritySafeCritical]
				private RuntimePropertyInfo[] PopulateProperties(RuntimeType.RuntimeTypeCache.Filter filter)
				{
					RuntimeType runtimeType = this.ReflectedType;
					RuntimeType.ListBuilder<RuntimePropertyInfo> listBuilder = default(RuntimeType.ListBuilder<RuntimePropertyInfo>);
					if (!RuntimeTypeHandle.IsInterface(runtimeType))
					{
						while (RuntimeTypeHandle.IsGenericVariable(runtimeType))
						{
							runtimeType = runtimeType.GetBaseType();
						}
						Dictionary<string, List<RuntimePropertyInfo>> csPropertyInfos = filter.CaseSensitive() ? null : new Dictionary<string, List<RuntimePropertyInfo>>();
						bool[] usedSlots = new bool[RuntimeTypeHandle.GetNumVirtuals(runtimeType)];
						do
						{
							this.PopulateProperties(filter, runtimeType, csPropertyInfos, usedSlots, ref listBuilder);
							runtimeType = RuntimeTypeHandle.GetBaseType(runtimeType);
						}
						while (runtimeType != null);
					}
					else
					{
						this.PopulateProperties(filter, runtimeType, null, null, ref listBuilder);
					}
					return listBuilder.ToArray();
				}

				[SecuritySafeCritical]
				private void PopulateProperties(RuntimeType.RuntimeTypeCache.Filter filter, RuntimeType declaringType, Dictionary<string, List<RuntimePropertyInfo>> csPropertyInfos, bool[] usedSlots, ref RuntimeType.ListBuilder<RuntimePropertyInfo> list)
				{
					int token = RuntimeTypeHandle.GetToken(declaringType);
					if (System.Reflection.MetadataToken.IsNullToken(token))
					{
						return;
					}
					MetadataEnumResult metadataEnumResult;
					RuntimeTypeHandle.GetMetadataImport(declaringType).EnumProperties(token, out metadataEnumResult);
					RuntimeModule module = RuntimeTypeHandle.GetModule(declaringType);
					int numVirtuals = RuntimeTypeHandle.GetNumVirtuals(declaringType);
					int i = 0;
					while (i < metadataEnumResult.Length)
					{
						int num = metadataEnumResult[i];
						if (!filter.RequiresStringComparison())
						{
							goto IL_86;
						}
						if (ModuleHandle.ContainsPropertyMatchingHash(module, num, filter.GetHashToMatch()))
						{
							Utf8String name = declaringType.GetRuntimeModule().MetadataImport.GetName(num);
							if (filter.Match(name))
							{
								goto IL_86;
							}
						}
						IL_1A2:
						i++;
						continue;
						IL_86:
						bool flag;
						RuntimePropertyInfo runtimePropertyInfo = new RuntimePropertyInfo(num, declaringType, this.m_runtimeTypeCache, ref flag);
						if (usedSlots != null)
						{
							if (declaringType != this.ReflectedType && flag)
							{
								goto IL_1A2;
							}
							MethodInfo methodInfo = runtimePropertyInfo.GetGetMethod();
							if (methodInfo == null)
							{
								methodInfo = runtimePropertyInfo.GetSetMethod();
							}
							if (methodInfo != null)
							{
								int slot = RuntimeMethodHandle.GetSlot((RuntimeMethodInfo)methodInfo);
								if (slot < numVirtuals)
								{
									if (usedSlots[slot])
									{
										goto IL_1A2;
									}
									usedSlots[slot] = true;
								}
							}
							if (csPropertyInfos != null)
							{
								string name2 = runtimePropertyInfo.Name;
								List<RuntimePropertyInfo> list2 = csPropertyInfos.GetValueOrDefault(name2);
								if (list2 == null)
								{
									list2 = new List<RuntimePropertyInfo>(1);
									csPropertyInfos[name2] = list2;
								}
								for (int j = 0; j < list2.Count; j++)
								{
									if (runtimePropertyInfo.EqualsSig(list2[j]))
									{
										list2 = null;
										break;
									}
								}
								if (list2 == null)
								{
									goto IL_1A2;
								}
								list2.Add(runtimePropertyInfo);
							}
							else
							{
								bool flag2 = false;
								for (int k = 0; k < list.Count; k++)
								{
									if (runtimePropertyInfo.EqualsSig(list[k]))
									{
										flag2 = true;
										break;
									}
								}
								if (flag2)
								{
									goto IL_1A2;
								}
							}
						}
						list.Add(runtimePropertyInfo);
						goto IL_1A2;
					}
				}

				internal T[] GetMemberList(RuntimeType.MemberListType listType, string name, RuntimeType.RuntimeTypeCache.CacheType cacheType)
				{
					if (listType != RuntimeType.MemberListType.CaseSensitive)
					{
						if (listType != RuntimeType.MemberListType.CaseInsensitive)
						{
							if (Volatile.Read(ref this.m_cacheComplete))
							{
								return this.m_allMembers;
							}
							return this.Populate(null, listType, cacheType);
						}
						else
						{
							T[] array = this.m_cisMemberInfos[name];
							if (array != null)
							{
								return array;
							}
							return this.Populate(name, listType, cacheType);
						}
					}
					else
					{
						T[] array = this.m_csMemberInfos[name];
						if (array != null)
						{
							return array;
						}
						return this.Populate(name, listType, cacheType);
					}
				}

				internal RuntimeType ReflectedType
				{
					get
					{
						return this.m_runtimeTypeCache.GetRuntimeType();
					}
				}

				private CerHashtable<string, T[]> m_csMemberInfos;

				private CerHashtable<string, T[]> m_cisMemberInfos;

				private T[] m_allMembers;

				private bool m_cacheComplete;

				private RuntimeType.RuntimeTypeCache m_runtimeTypeCache;
			}
		}

		private class ConstructorInfoComparer : IComparer<ConstructorInfo>
		{
			public int Compare(ConstructorInfo x, ConstructorInfo y)
			{
				return x.MetadataToken.CompareTo(y.MetadataToken);
			}

			internal static readonly RuntimeType.ConstructorInfoComparer SortByMetadataToken = new RuntimeType.ConstructorInfoComparer();
		}

		private class ActivatorCacheEntry
		{
			[SecurityCritical]
			internal ActivatorCacheEntry(RuntimeType t, RuntimeMethodHandleInternal rmh, bool bNeedSecurityCheck)
			{
				this.m_type = t;
				this.m_bNeedSecurityCheck = bNeedSecurityCheck;
				this.m_hCtorMethodHandle = rmh;
				if (!this.m_hCtorMethodHandle.IsNullHandle())
				{
					this.m_ctorAttributes = RuntimeMethodHandle.GetAttributes(this.m_hCtorMethodHandle);
				}
			}

			internal readonly RuntimeType m_type;

			internal volatile CtorDelegate m_ctor;

			internal readonly RuntimeMethodHandleInternal m_hCtorMethodHandle;

			internal readonly MethodAttributes m_ctorAttributes;

			internal readonly bool m_bNeedSecurityCheck;

			internal volatile bool m_bFullyInitialized;
		}

		private class ActivatorCache
		{
			private void InitializeDelegateCreator()
			{
				PermissionSet permissionSet = new PermissionSet(PermissionState.None);
				permissionSet.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));
				permissionSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode));
				this.delegateCreatePermissions = permissionSet;
				ConstructorInfo constructor = typeof(CtorDelegate).GetConstructor(new Type[]
				{
					typeof(object),
					typeof(IntPtr)
				});
				this.delegateCtorInfo = constructor;
			}

			[SecuritySafeCritical]
			private void InitializeCacheEntry(RuntimeType.ActivatorCacheEntry ace)
			{
				if (!ace.m_type.IsValueType)
				{
					if (this.delegateCtorInfo == null)
					{
						this.InitializeDelegateCreator();
					}
					this.delegateCreatePermissions.Assert();
					CtorDelegate ctor = (CtorDelegate)this.delegateCtorInfo.Invoke(new object[]
					{
						null,
						RuntimeMethodHandle.GetFunctionPointer(ace.m_hCtorMethodHandle)
					});
					ace.m_ctor = ctor;
				}
				ace.m_bFullyInitialized = true;
			}

			internal RuntimeType.ActivatorCacheEntry GetEntry(RuntimeType t)
			{
				int num = this.hash_counter;
				for (int i = 0; i < 16; i++)
				{
					RuntimeType.ActivatorCacheEntry activatorCacheEntry = Volatile.Read<RuntimeType.ActivatorCacheEntry>(ref this.cache[num]);
					if (activatorCacheEntry != null && activatorCacheEntry.m_type == t)
					{
						if (!activatorCacheEntry.m_bFullyInitialized)
						{
							this.InitializeCacheEntry(activatorCacheEntry);
						}
						return activatorCacheEntry;
					}
					num = (num + 1 & 15);
				}
				return null;
			}

			internal void SetEntry(RuntimeType.ActivatorCacheEntry ace)
			{
				int num = this.hash_counter - 1 & 15;
				this.hash_counter = num;
				Volatile.Write<RuntimeType.ActivatorCacheEntry>(ref this.cache[num], ace);
			}

			private const int CACHE_SIZE = 16;

			private volatile int hash_counter;

			private readonly RuntimeType.ActivatorCacheEntry[] cache = new RuntimeType.ActivatorCacheEntry[16];

			private volatile ConstructorInfo delegateCtorInfo;

			private volatile PermissionSet delegateCreatePermissions;
		}

		[Flags]
		private enum DispatchWrapperType
		{
			Unknown = 1,
			Dispatch = 2,
			Record = 4,
			Error = 8,
			Currency = 16,
			BStr = 32,
			SafeArray = 65536
		}
	}
}
