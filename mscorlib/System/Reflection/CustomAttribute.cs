using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Reflection
{
	internal static class CustomAttribute
	{
		[SecurityCritical]
		internal static bool IsDefined(RuntimeType type, RuntimeType caType, bool inherit)
		{
			if (type.GetElementType() != null)
			{
				return false;
			}
			if (PseudoCustomAttribute.IsDefined(type, caType))
			{
				return true;
			}
			if (CustomAttribute.IsCustomAttributeDefined(type.GetRuntimeModule(), type.MetadataToken, caType))
			{
				return true;
			}
			if (!inherit)
			{
				return false;
			}
			type = (type.BaseType as RuntimeType);
			while (type != null)
			{
				if (CustomAttribute.IsCustomAttributeDefined(type.GetRuntimeModule(), type.MetadataToken, caType, 0, inherit))
				{
					return true;
				}
				type = (type.BaseType as RuntimeType);
			}
			return false;
		}

		[SecuritySafeCritical]
		internal static bool IsDefined(RuntimeMethodInfo method, RuntimeType caType, bool inherit)
		{
			if (PseudoCustomAttribute.IsDefined(method, caType))
			{
				return true;
			}
			if (CustomAttribute.IsCustomAttributeDefined(method.GetRuntimeModule(), method.MetadataToken, caType))
			{
				return true;
			}
			if (!inherit)
			{
				return false;
			}
			method = method.GetParentDefinition();
			while (method != null)
			{
				if (CustomAttribute.IsCustomAttributeDefined(method.GetRuntimeModule(), method.MetadataToken, caType, 0, inherit))
				{
					return true;
				}
				method = method.GetParentDefinition();
			}
			return false;
		}

		[SecurityCritical]
		internal static bool IsDefined(RuntimeConstructorInfo ctor, RuntimeType caType)
		{
			return PseudoCustomAttribute.IsDefined(ctor, caType) || CustomAttribute.IsCustomAttributeDefined(ctor.GetRuntimeModule(), ctor.MetadataToken, caType);
		}

		[SecurityCritical]
		internal static bool IsDefined(RuntimePropertyInfo property, RuntimeType caType)
		{
			return PseudoCustomAttribute.IsDefined(property, caType) || CustomAttribute.IsCustomAttributeDefined(property.GetRuntimeModule(), property.MetadataToken, caType);
		}

		[SecurityCritical]
		internal static bool IsDefined(RuntimeEventInfo e, RuntimeType caType)
		{
			return PseudoCustomAttribute.IsDefined(e, caType) || CustomAttribute.IsCustomAttributeDefined(e.GetRuntimeModule(), e.MetadataToken, caType);
		}

		[SecurityCritical]
		internal static bool IsDefined(RuntimeFieldInfo field, RuntimeType caType)
		{
			return PseudoCustomAttribute.IsDefined(field, caType) || CustomAttribute.IsCustomAttributeDefined(field.GetRuntimeModule(), field.MetadataToken, caType);
		}

		[SecurityCritical]
		internal static bool IsDefined(RuntimeParameterInfo parameter, RuntimeType caType)
		{
			return PseudoCustomAttribute.IsDefined(parameter, caType) || CustomAttribute.IsCustomAttributeDefined(parameter.GetRuntimeModule(), parameter.MetadataToken, caType);
		}

		[SecuritySafeCritical]
		internal static bool IsDefined(RuntimeAssembly assembly, RuntimeType caType)
		{
			return PseudoCustomAttribute.IsDefined(assembly, caType) || CustomAttribute.IsCustomAttributeDefined(assembly.ManifestModule as RuntimeModule, RuntimeAssembly.GetToken(assembly.GetNativeHandle()), caType);
		}

		[SecurityCritical]
		internal static bool IsDefined(RuntimeModule module, RuntimeType caType)
		{
			return PseudoCustomAttribute.IsDefined(module, caType) || CustomAttribute.IsCustomAttributeDefined(module, module.MetadataToken, caType);
		}

		[SecurityCritical]
		internal static object[] GetCustomAttributes(RuntimeType type, RuntimeType caType, bool inherit)
		{
			if (type.GetElementType() != null)
			{
				if (!caType.IsValueType)
				{
					return CustomAttribute.CreateAttributeArrayHelper(caType, 0);
				}
				return EmptyArray<object>.Value;
			}
			else
			{
				if (type.IsGenericType && !type.IsGenericTypeDefinition)
				{
					type = (type.GetGenericTypeDefinition() as RuntimeType);
				}
				int i = 0;
				Attribute[] customAttributes = PseudoCustomAttribute.GetCustomAttributes(type, caType, true, out i);
				if (!inherit || (caType.IsSealed && !CustomAttribute.GetAttributeUsage(caType).Inherited))
				{
					object[] customAttributes2 = CustomAttribute.GetCustomAttributes(type.GetRuntimeModule(), type.MetadataToken, i, caType, !CustomAttribute.AllowCriticalCustomAttributes(type));
					if (i > 0)
					{
						Array.Copy(customAttributes, 0, customAttributes2, customAttributes2.Length - i, i);
					}
					return customAttributes2;
				}
				List<object> list = new List<object>();
				bool mustBeInheritable = false;
				Type elementType = (caType == null || caType.IsValueType || caType.ContainsGenericParameters) ? typeof(object) : caType;
				while (i > 0)
				{
					list.Add(customAttributes[--i]);
				}
				while (type != (RuntimeType)typeof(object) && type != null)
				{
					object[] customAttributes3 = CustomAttribute.GetCustomAttributes(type.GetRuntimeModule(), type.MetadataToken, 0, caType, mustBeInheritable, list, !CustomAttribute.AllowCriticalCustomAttributes(type));
					mustBeInheritable = true;
					for (int j = 0; j < customAttributes3.Length; j++)
					{
						list.Add(customAttributes3[j]);
					}
					type = (type.BaseType as RuntimeType);
				}
				object[] array = CustomAttribute.CreateAttributeArrayHelper(elementType, list.Count);
				Array.Copy(list.ToArray(), 0, array, 0, list.Count);
				return array;
			}
		}

		private static bool AllowCriticalCustomAttributes(RuntimeType type)
		{
			if (type.IsGenericParameter)
			{
				MethodBase declaringMethod = type.DeclaringMethod;
				if (declaringMethod != null)
				{
					return CustomAttribute.AllowCriticalCustomAttributes(declaringMethod);
				}
				type = (type.DeclaringType as RuntimeType);
			}
			return !type.IsSecurityTransparent || CustomAttribute.SpecialAllowCriticalAttributes(type);
		}

		private static bool SpecialAllowCriticalAttributes(RuntimeType type)
		{
			return type != null && type.Assembly.IsFullyTrusted && RuntimeTypeHandle.IsEquivalentType(type);
		}

		private static bool AllowCriticalCustomAttributes(MethodBase method)
		{
			return !method.IsSecurityTransparent || CustomAttribute.SpecialAllowCriticalAttributes((RuntimeType)method.DeclaringType);
		}

		private static bool AllowCriticalCustomAttributes(RuntimeFieldInfo field)
		{
			return !field.IsSecurityTransparent || CustomAttribute.SpecialAllowCriticalAttributes((RuntimeType)field.DeclaringType);
		}

		private static bool AllowCriticalCustomAttributes(RuntimeParameterInfo parameter)
		{
			return CustomAttribute.AllowCriticalCustomAttributes(parameter.DefiningMethod);
		}

		[SecurityCritical]
		internal static object[] GetCustomAttributes(RuntimeMethodInfo method, RuntimeType caType, bool inherit)
		{
			if (method.IsGenericMethod && !method.IsGenericMethodDefinition)
			{
				method = (method.GetGenericMethodDefinition() as RuntimeMethodInfo);
			}
			int i = 0;
			Attribute[] customAttributes = PseudoCustomAttribute.GetCustomAttributes(method, caType, true, out i);
			if (!inherit || (caType.IsSealed && !CustomAttribute.GetAttributeUsage(caType).Inherited))
			{
				object[] customAttributes2 = CustomAttribute.GetCustomAttributes(method.GetRuntimeModule(), method.MetadataToken, i, caType, !CustomAttribute.AllowCriticalCustomAttributes(method));
				if (i > 0)
				{
					Array.Copy(customAttributes, 0, customAttributes2, customAttributes2.Length - i, i);
				}
				return customAttributes2;
			}
			List<object> list = new List<object>();
			bool mustBeInheritable = false;
			Type elementType = (caType == null || caType.IsValueType || caType.ContainsGenericParameters) ? typeof(object) : caType;
			while (i > 0)
			{
				list.Add(customAttributes[--i]);
			}
			while (method != null)
			{
				object[] customAttributes3 = CustomAttribute.GetCustomAttributes(method.GetRuntimeModule(), method.MetadataToken, 0, caType, mustBeInheritable, list, !CustomAttribute.AllowCriticalCustomAttributes(method));
				mustBeInheritable = true;
				for (int j = 0; j < customAttributes3.Length; j++)
				{
					list.Add(customAttributes3[j]);
				}
				method = method.GetParentDefinition();
			}
			object[] array = CustomAttribute.CreateAttributeArrayHelper(elementType, list.Count);
			Array.Copy(list.ToArray(), 0, array, 0, list.Count);
			return array;
		}

		[SecuritySafeCritical]
		internal static object[] GetCustomAttributes(RuntimeConstructorInfo ctor, RuntimeType caType)
		{
			int num = 0;
			Attribute[] customAttributes = PseudoCustomAttribute.GetCustomAttributes(ctor, caType, true, out num);
			object[] customAttributes2 = CustomAttribute.GetCustomAttributes(ctor.GetRuntimeModule(), ctor.MetadataToken, num, caType, !CustomAttribute.AllowCriticalCustomAttributes(ctor));
			if (num > 0)
			{
				Array.Copy(customAttributes, 0, customAttributes2, customAttributes2.Length - num, num);
			}
			return customAttributes2;
		}

		[SecuritySafeCritical]
		internal static object[] GetCustomAttributes(RuntimePropertyInfo property, RuntimeType caType)
		{
			int num = 0;
			Attribute[] customAttributes = PseudoCustomAttribute.GetCustomAttributes(property, caType, out num);
			bool isDecoratedTargetSecurityTransparent = property.GetRuntimeModule().GetRuntimeAssembly().IsAllSecurityTransparent();
			object[] customAttributes2 = CustomAttribute.GetCustomAttributes(property.GetRuntimeModule(), property.MetadataToken, num, caType, isDecoratedTargetSecurityTransparent);
			if (num > 0)
			{
				Array.Copy(customAttributes, 0, customAttributes2, customAttributes2.Length - num, num);
			}
			return customAttributes2;
		}

		[SecuritySafeCritical]
		internal static object[] GetCustomAttributes(RuntimeEventInfo e, RuntimeType caType)
		{
			int num = 0;
			Attribute[] customAttributes = PseudoCustomAttribute.GetCustomAttributes(e, caType, out num);
			bool isDecoratedTargetSecurityTransparent = e.GetRuntimeModule().GetRuntimeAssembly().IsAllSecurityTransparent();
			object[] customAttributes2 = CustomAttribute.GetCustomAttributes(e.GetRuntimeModule(), e.MetadataToken, num, caType, isDecoratedTargetSecurityTransparent);
			if (num > 0)
			{
				Array.Copy(customAttributes, 0, customAttributes2, customAttributes2.Length - num, num);
			}
			return customAttributes2;
		}

		[SecuritySafeCritical]
		internal static object[] GetCustomAttributes(RuntimeFieldInfo field, RuntimeType caType)
		{
			int num = 0;
			Attribute[] customAttributes = PseudoCustomAttribute.GetCustomAttributes(field, caType, out num);
			object[] customAttributes2 = CustomAttribute.GetCustomAttributes(field.GetRuntimeModule(), field.MetadataToken, num, caType, !CustomAttribute.AllowCriticalCustomAttributes(field));
			if (num > 0)
			{
				Array.Copy(customAttributes, 0, customAttributes2, customAttributes2.Length - num, num);
			}
			return customAttributes2;
		}

		[SecuritySafeCritical]
		internal static object[] GetCustomAttributes(RuntimeParameterInfo parameter, RuntimeType caType)
		{
			int num = 0;
			Attribute[] customAttributes = PseudoCustomAttribute.GetCustomAttributes(parameter, caType, out num);
			object[] customAttributes2 = CustomAttribute.GetCustomAttributes(parameter.GetRuntimeModule(), parameter.MetadataToken, num, caType, !CustomAttribute.AllowCriticalCustomAttributes(parameter));
			if (num > 0)
			{
				Array.Copy(customAttributes, 0, customAttributes2, customAttributes2.Length - num, num);
			}
			return customAttributes2;
		}

		[SecuritySafeCritical]
		internal static object[] GetCustomAttributes(RuntimeAssembly assembly, RuntimeType caType)
		{
			int num = 0;
			Attribute[] customAttributes = PseudoCustomAttribute.GetCustomAttributes(assembly, caType, true, out num);
			int token = RuntimeAssembly.GetToken(assembly.GetNativeHandle());
			bool isDecoratedTargetSecurityTransparent = assembly.IsAllSecurityTransparent();
			object[] customAttributes2 = CustomAttribute.GetCustomAttributes(assembly.ManifestModule as RuntimeModule, token, num, caType, isDecoratedTargetSecurityTransparent);
			if (num > 0)
			{
				Array.Copy(customAttributes, 0, customAttributes2, customAttributes2.Length - num, num);
			}
			return customAttributes2;
		}

		[SecuritySafeCritical]
		internal static object[] GetCustomAttributes(RuntimeModule module, RuntimeType caType)
		{
			int num = 0;
			Attribute[] customAttributes = PseudoCustomAttribute.GetCustomAttributes(module, caType, out num);
			bool isDecoratedTargetSecurityTransparent = module.GetRuntimeAssembly().IsAllSecurityTransparent();
			object[] customAttributes2 = CustomAttribute.GetCustomAttributes(module, module.MetadataToken, num, caType, isDecoratedTargetSecurityTransparent);
			if (num > 0)
			{
				Array.Copy(customAttributes, 0, customAttributes2, customAttributes2.Length - num, num);
			}
			return customAttributes2;
		}

		[SecuritySafeCritical]
		internal static bool IsAttributeDefined(RuntimeModule decoratedModule, int decoratedMetadataToken, int attributeCtorToken)
		{
			return CustomAttribute.IsCustomAttributeDefined(decoratedModule, decoratedMetadataToken, null, attributeCtorToken, false);
		}

		[SecurityCritical]
		private static bool IsCustomAttributeDefined(RuntimeModule decoratedModule, int decoratedMetadataToken, RuntimeType attributeFilterType)
		{
			return CustomAttribute.IsCustomAttributeDefined(decoratedModule, decoratedMetadataToken, attributeFilterType, 0, false);
		}

		[SecurityCritical]
		private static bool IsCustomAttributeDefined(RuntimeModule decoratedModule, int decoratedMetadataToken, RuntimeType attributeFilterType, int attributeCtorToken, bool mustBeInheritable)
		{
			if (decoratedModule.Assembly.ReflectionOnly)
			{
				throw new InvalidOperationException(Environment.GetResourceString("Arg_ReflectionOnlyCA"));
			}
			CustomAttributeRecord[] customAttributeRecords = CustomAttributeData.GetCustomAttributeRecords(decoratedModule, decoratedMetadataToken);
			if (attributeFilterType != null)
			{
				MetadataImport metadataImport = decoratedModule.MetadataImport;
				Assembly assembly = null;
				foreach (CustomAttributeRecord caRecord in customAttributeRecords)
				{
					RuntimeType runtimeType;
					IRuntimeMethodInfo runtimeMethodInfo;
					bool flag;
					bool flag2;
					if (CustomAttribute.FilterCustomAttributeRecord(caRecord, metadataImport, ref assembly, decoratedModule, decoratedMetadataToken, attributeFilterType, mustBeInheritable, null, null, out runtimeType, out runtimeMethodInfo, out flag, out flag2))
					{
						return true;
					}
				}
			}
			else
			{
				foreach (CustomAttributeRecord customAttributeRecord in customAttributeRecords)
				{
					if (customAttributeRecord.tkCtor == attributeCtorToken)
					{
						return true;
					}
				}
			}
			return false;
		}

		[SecurityCritical]
		private static object[] GetCustomAttributes(RuntimeModule decoratedModule, int decoratedMetadataToken, int pcaCount, RuntimeType attributeFilterType, bool isDecoratedTargetSecurityTransparent)
		{
			return CustomAttribute.GetCustomAttributes(decoratedModule, decoratedMetadataToken, pcaCount, attributeFilterType, false, null, isDecoratedTargetSecurityTransparent);
		}

		[SecurityCritical]
		private unsafe static object[] GetCustomAttributes(RuntimeModule decoratedModule, int decoratedMetadataToken, int pcaCount, RuntimeType attributeFilterType, bool mustBeInheritable, IList derivedAttributes, bool isDecoratedTargetSecurityTransparent)
		{
			if (decoratedModule.Assembly.ReflectionOnly)
			{
				throw new InvalidOperationException(Environment.GetResourceString("Arg_ReflectionOnlyCA"));
			}
			MetadataImport metadataImport = decoratedModule.MetadataImport;
			CustomAttributeRecord[] customAttributeRecords = CustomAttributeData.GetCustomAttributeRecords(decoratedModule, decoratedMetadataToken);
			Type elementType = (attributeFilterType == null || attributeFilterType.IsValueType || attributeFilterType.ContainsGenericParameters) ? typeof(object) : attributeFilterType;
			if (attributeFilterType == null && customAttributeRecords.Length == 0)
			{
				return CustomAttribute.CreateAttributeArrayHelper(elementType, 0);
			}
			object[] array = CustomAttribute.CreateAttributeArrayHelper(elementType, customAttributeRecords.Length);
			int num = 0;
			SecurityContextFrame securityContextFrame = default(SecurityContextFrame);
			securityContextFrame.Push(decoratedModule.GetRuntimeAssembly());
			Assembly assembly = null;
			for (int i = 0; i < customAttributeRecords.Length; i++)
			{
				object obj = null;
				CustomAttributeRecord caRecord = customAttributeRecords[i];
				IRuntimeMethodInfo runtimeMethodInfo = null;
				RuntimeType runtimeType = null;
				int num2 = 0;
				IntPtr intPtr = caRecord.blob.Signature;
				IntPtr intPtr2 = (IntPtr)((void*)((byte*)((void*)intPtr) + caRecord.blob.Length));
				int num3 = (int)((long)((byte*)((void*)intPtr2) - (byte*)((void*)intPtr)));
				bool flag;
				bool isVarArg;
				if (CustomAttribute.FilterCustomAttributeRecord(caRecord, metadataImport, ref assembly, decoratedModule, decoratedMetadataToken, attributeFilterType, mustBeInheritable, array, derivedAttributes, out runtimeType, out runtimeMethodInfo, out flag, out isVarArg))
				{
					if (runtimeMethodInfo != null)
					{
						RuntimeMethodHandle.CheckLinktimeDemands(runtimeMethodInfo, decoratedModule, isDecoratedTargetSecurityTransparent);
					}
					RuntimeConstructorInfo.CheckCanCreateInstance(runtimeType, isVarArg);
					if (flag)
					{
						obj = CustomAttribute.CreateCaObject(decoratedModule, runtimeMethodInfo, ref intPtr, intPtr2, out num2);
					}
					else
					{
						obj = RuntimeTypeHandle.CreateCaInstance(runtimeType, runtimeMethodInfo);
						if (num3 == 0)
						{
							num2 = 0;
						}
						else
						{
							if (Marshal.ReadInt16(intPtr) != 1)
							{
								throw new CustomAttributeFormatException();
							}
							intPtr = (IntPtr)((void*)((byte*)((void*)intPtr) + 2));
							num2 = (int)Marshal.ReadInt16(intPtr);
							intPtr = (IntPtr)((void*)((byte*)((void*)intPtr) + 2));
						}
					}
					for (int j = 0; j < num2; j++)
					{
						IntPtr signature = caRecord.blob.Signature;
						string text;
						bool flag2;
						RuntimeType runtimeType2;
						object obj2;
						CustomAttribute.GetPropertyOrFieldData(decoratedModule, ref intPtr, intPtr2, out text, out flag2, out runtimeType2, out obj2);
						try
						{
							if (flag2)
							{
								if (runtimeType2 == null && obj2 != null)
								{
									runtimeType2 = (RuntimeType)obj2.GetType();
									if (runtimeType2 == CustomAttribute.Type_RuntimeType)
									{
										runtimeType2 = CustomAttribute.Type_Type;
									}
								}
								RuntimePropertyInfo runtimePropertyInfo;
								if (runtimeType2 == null)
								{
									runtimePropertyInfo = (runtimeType.GetProperty(text) as RuntimePropertyInfo);
								}
								else
								{
									runtimePropertyInfo = (runtimeType.GetProperty(text, runtimeType2, Type.EmptyTypes) as RuntimePropertyInfo);
								}
								if (runtimePropertyInfo == null)
								{
									throw new CustomAttributeFormatException(string.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString(flag2 ? "RFLCT.InvalidPropFail" : "RFLCT.InvalidFieldFail"), text));
								}
								RuntimeMethodInfo runtimeMethodInfo2 = runtimePropertyInfo.GetSetMethod(true) as RuntimeMethodInfo;
								if (runtimeMethodInfo2.IsPublic)
								{
									RuntimeMethodHandle.CheckLinktimeDemands(runtimeMethodInfo2, decoratedModule, isDecoratedTargetSecurityTransparent);
									runtimeMethodInfo2.UnsafeInvoke(obj, BindingFlags.Default, null, new object[]
									{
										obj2
									}, null);
								}
							}
							else
							{
								RtFieldInfo rtFieldInfo = runtimeType.GetField(text) as RtFieldInfo;
								if (isDecoratedTargetSecurityTransparent)
								{
									RuntimeFieldHandle.CheckAttributeAccess(rtFieldInfo.FieldHandle, decoratedModule.GetNativeHandle());
								}
								rtFieldInfo.CheckConsistency(obj);
								rtFieldInfo.UnsafeSetValue(obj, obj2, BindingFlags.Default, Type.DefaultBinder, null);
							}
						}
						catch (Exception inner)
						{
							throw new CustomAttributeFormatException(string.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString(flag2 ? "RFLCT.InvalidPropFail" : "RFLCT.InvalidFieldFail"), text), inner);
						}
					}
					if (!intPtr.Equals(intPtr2))
					{
						throw new CustomAttributeFormatException();
					}
					array[num++] = obj;
				}
			}
			securityContextFrame.Pop();
			if (num == customAttributeRecords.Length && pcaCount == 0)
			{
				return array;
			}
			object[] array2 = CustomAttribute.CreateAttributeArrayHelper(elementType, num + pcaCount);
			Array.Copy(array, 0, array2, 0, num);
			return array2;
		}

		[SecurityCritical]
		private unsafe static bool FilterCustomAttributeRecord(CustomAttributeRecord caRecord, MetadataImport scope, ref Assembly lastAptcaOkAssembly, RuntimeModule decoratedModule, MetadataToken decoratedToken, RuntimeType attributeFilterType, bool mustBeInheritable, object[] attributes, IList derivedAttributes, out RuntimeType attributeType, out IRuntimeMethodInfo ctor, out bool ctorHasParameters, out bool isVarArg)
		{
			ctor = null;
			attributeType = null;
			ctorHasParameters = false;
			isVarArg = false;
			IntPtr signature = caRecord.blob.Signature;
			IntPtr intPtr = (IntPtr)((void*)((byte*)((void*)signature) + caRecord.blob.Length));
			attributeType = (decoratedModule.ResolveType(scope.GetParentToken(caRecord.tkCtor), null, null) as RuntimeType);
			if (!attributeFilterType.IsAssignableFrom(attributeType))
			{
				return false;
			}
			if (!CustomAttribute.AttributeUsageCheck(attributeType, mustBeInheritable, attributes, derivedAttributes))
			{
				return false;
			}
			if ((attributeType.Attributes & TypeAttributes.WindowsRuntime) == TypeAttributes.WindowsRuntime)
			{
				return false;
			}
			RuntimeAssembly runtimeAssembly = (RuntimeAssembly)attributeType.Assembly;
			RuntimeAssembly runtimeAssembly2 = (RuntimeAssembly)decoratedModule.Assembly;
			if (runtimeAssembly != lastAptcaOkAssembly && !RuntimeAssembly.AptcaCheck(runtimeAssembly, runtimeAssembly2))
			{
				return false;
			}
			lastAptcaOkAssembly = runtimeAssembly2;
			ConstArray methodSignature = scope.GetMethodSignature(caRecord.tkCtor);
			isVarArg = ((methodSignature[0] & 5) > 0);
			ctorHasParameters = (methodSignature[1] > 0);
			if (ctorHasParameters)
			{
				ctor = ModuleHandle.ResolveMethodHandleInternal(decoratedModule.GetNativeHandle(), caRecord.tkCtor);
			}
			else
			{
				ctor = attributeType.GetTypeHandleInternal().GetDefaultConstructor();
				if (ctor == null && !attributeType.IsValueType)
				{
					throw new MissingMethodException(".ctor");
				}
			}
			MetadataToken token = default(MetadataToken);
			if (decoratedToken.IsParamDef)
			{
				token = new MetadataToken(scope.GetParentToken(decoratedToken));
				token = new MetadataToken(scope.GetParentToken(token));
			}
			else if (decoratedToken.IsMethodDef || decoratedToken.IsProperty || decoratedToken.IsEvent || decoratedToken.IsFieldDef)
			{
				token = new MetadataToken(scope.GetParentToken(decoratedToken));
			}
			else if (decoratedToken.IsTypeDef)
			{
				token = decoratedToken;
			}
			else if (decoratedToken.IsGenericPar)
			{
				token = new MetadataToken(scope.GetParentToken(decoratedToken));
				if (token.IsMethodDef)
				{
					token = new MetadataToken(scope.GetParentToken(token));
				}
			}
			RuntimeTypeHandle sourceTypeHandle = token.IsTypeDef ? decoratedModule.ModuleHandle.ResolveTypeHandle(token) : default(RuntimeTypeHandle);
			return RuntimeMethodHandle.IsCAVisibleFromDecoratedType(attributeType.TypeHandle, ctor, sourceTypeHandle, decoratedModule);
		}

		[SecurityCritical]
		private static bool AttributeUsageCheck(RuntimeType attributeType, bool mustBeInheritable, object[] attributes, IList derivedAttributes)
		{
			AttributeUsageAttribute attributeUsageAttribute = null;
			if (mustBeInheritable)
			{
				attributeUsageAttribute = CustomAttribute.GetAttributeUsage(attributeType);
				if (!attributeUsageAttribute.Inherited)
				{
					return false;
				}
			}
			if (derivedAttributes == null)
			{
				return true;
			}
			for (int i = 0; i < derivedAttributes.Count; i++)
			{
				if (derivedAttributes[i].GetType() == attributeType)
				{
					if (attributeUsageAttribute == null)
					{
						attributeUsageAttribute = CustomAttribute.GetAttributeUsage(attributeType);
					}
					return attributeUsageAttribute.AllowMultiple;
				}
			}
			return true;
		}

		[SecurityCritical]
		internal static AttributeUsageAttribute GetAttributeUsage(RuntimeType decoratedAttribute)
		{
			RuntimeModule runtimeModule = decoratedAttribute.GetRuntimeModule();
			MetadataImport metadataImport = runtimeModule.MetadataImport;
			CustomAttributeRecord[] customAttributeRecords = CustomAttributeData.GetCustomAttributeRecords(runtimeModule, decoratedAttribute.MetadataToken);
			AttributeUsageAttribute attributeUsageAttribute = null;
			foreach (CustomAttributeRecord customAttributeRecord in customAttributeRecords)
			{
				RuntimeType runtimeType = runtimeModule.ResolveType(metadataImport.GetParentToken(customAttributeRecord.tkCtor), null, null) as RuntimeType;
				if (!(runtimeType != (RuntimeType)typeof(AttributeUsageAttribute)))
				{
					if (attributeUsageAttribute != null)
					{
						throw new FormatException(string.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString("Format_AttributeUsage"), runtimeType));
					}
					AttributeTargets validOn;
					bool inherited;
					bool allowMultiple;
					CustomAttribute.ParseAttributeUsageAttribute(customAttributeRecord.blob, out validOn, out inherited, out allowMultiple);
					attributeUsageAttribute = new AttributeUsageAttribute(validOn, allowMultiple, inherited);
				}
			}
			if (attributeUsageAttribute == null)
			{
				return AttributeUsageAttribute.Default;
			}
			return attributeUsageAttribute;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void _ParseAttributeUsageAttribute(IntPtr pCa, int cCa, out int targets, out bool inherited, out bool allowMultiple);

		[SecurityCritical]
		private static void ParseAttributeUsageAttribute(ConstArray ca, out AttributeTargets targets, out bool inherited, out bool allowMultiple)
		{
			int num;
			CustomAttribute._ParseAttributeUsageAttribute(ca.Signature, ca.Length, out num, out inherited, out allowMultiple);
			targets = (AttributeTargets)num;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern object _CreateCaObject(RuntimeModule pModule, IRuntimeMethodInfo pCtor, byte** ppBlob, byte* pEndBlob, int* pcNamedArgs);

		[SecurityCritical]
		private unsafe static object CreateCaObject(RuntimeModule module, IRuntimeMethodInfo ctor, ref IntPtr blob, IntPtr blobEnd, out int namedArgs)
		{
			byte* value = (byte*)((void*)blob);
			byte* pEndBlob = (byte*)((void*)blobEnd);
			int num;
			object result = CustomAttribute._CreateCaObject(module, ctor, &value, pEndBlob, &num);
			blob = (IntPtr)((void*)value);
			namedArgs = num;
			return result;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private unsafe static extern void _GetPropertyOrFieldData(RuntimeModule pModule, byte** ppBlobStart, byte* pBlobEnd, out string name, out bool bIsProperty, out RuntimeType type, out object value);

		[SecurityCritical]
		private unsafe static void GetPropertyOrFieldData(RuntimeModule module, ref IntPtr blobStart, IntPtr blobEnd, out string name, out bool isProperty, out RuntimeType type, out object value)
		{
			byte* value2 = (byte*)((void*)blobStart);
			CustomAttribute._GetPropertyOrFieldData(module.GetNativeHandle(), &value2, (byte*)((void*)blobEnd), out name, out isProperty, out type, out value);
			blobStart = (IntPtr)((void*)value2);
		}

		[SecuritySafeCritical]
		private static object[] CreateAttributeArrayHelper(Type elementType, int elementCount)
		{
			return (object[])Array.UnsafeCreateInstance(elementType, elementCount);
		}

		private static RuntimeType Type_RuntimeType = (RuntimeType)typeof(RuntimeType);

		private static RuntimeType Type_Type = (RuntimeType)typeof(Type);
	}
}
