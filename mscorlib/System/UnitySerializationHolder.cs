using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[Serializable]
	internal class UnitySerializationHolder : ISerializable, IObjectReference
	{
		internal static void GetUnitySerializationInfo(SerializationInfo info, Missing missing)
		{
			info.SetType(typeof(UnitySerializationHolder));
			info.AddValue("UnityType", 3);
		}

		internal static RuntimeType AddElementTypes(SerializationInfo info, RuntimeType type)
		{
			List<int> list = new List<int>();
			while (type.HasElementType)
			{
				if (type.IsSzArray)
				{
					list.Add(3);
				}
				else if (type.IsArray)
				{
					list.Add(type.GetArrayRank());
					list.Add(2);
				}
				else if (type.IsPointer)
				{
					list.Add(1);
				}
				else if (type.IsByRef)
				{
					list.Add(4);
				}
				type = (RuntimeType)type.GetElementType();
			}
			info.AddValue("ElementTypes", list.ToArray(), typeof(int[]));
			return type;
		}

		internal Type MakeElementTypes(Type type)
		{
			for (int i = this.m_elementTypes.Length - 1; i >= 0; i--)
			{
				if (this.m_elementTypes[i] == 3)
				{
					type = type.MakeArrayType();
				}
				else if (this.m_elementTypes[i] == 2)
				{
					type = type.MakeArrayType(this.m_elementTypes[--i]);
				}
				else if (this.m_elementTypes[i] == 1)
				{
					type = type.MakePointerType();
				}
				else if (this.m_elementTypes[i] == 4)
				{
					type = type.MakeByRefType();
				}
			}
			return type;
		}

		internal static void GetUnitySerializationInfo(SerializationInfo info, RuntimeType type)
		{
			if (type.GetRootElementType().IsGenericParameter)
			{
				type = UnitySerializationHolder.AddElementTypes(info, type);
				info.SetType(typeof(UnitySerializationHolder));
				info.AddValue("UnityType", 7);
				info.AddValue("GenericParameterPosition", type.GenericParameterPosition);
				info.AddValue("DeclaringMethod", type.DeclaringMethod, typeof(MethodBase));
				info.AddValue("DeclaringType", type.DeclaringType, typeof(Type));
				return;
			}
			int unityType = 4;
			if (!type.IsGenericTypeDefinition && type.ContainsGenericParameters)
			{
				unityType = 8;
				type = UnitySerializationHolder.AddElementTypes(info, type);
				info.AddValue("GenericArguments", type.GetGenericArguments(), typeof(Type[]));
				type = (RuntimeType)type.GetGenericTypeDefinition();
			}
			UnitySerializationHolder.GetUnitySerializationInfo(info, unityType, type.FullName, type.GetRuntimeAssembly());
		}

		internal static void GetUnitySerializationInfo(SerializationInfo info, int unityType, string data, RuntimeAssembly assembly)
		{
			info.SetType(typeof(UnitySerializationHolder));
			info.AddValue("Data", data, typeof(string));
			info.AddValue("UnityType", unityType);
			string value;
			if (assembly == null)
			{
				value = string.Empty;
			}
			else
			{
				value = assembly.FullName;
			}
			info.AddValue("AssemblyName", value);
		}

		internal UnitySerializationHolder(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			this.m_unityType = info.GetInt32("UnityType");
			if (this.m_unityType == 3)
			{
				return;
			}
			if (this.m_unityType == 7)
			{
				this.m_declaringMethod = (info.GetValue("DeclaringMethod", typeof(MethodBase)) as MethodBase);
				this.m_declaringType = (info.GetValue("DeclaringType", typeof(Type)) as Type);
				this.m_genericParameterPosition = info.GetInt32("GenericParameterPosition");
				this.m_elementTypes = (info.GetValue("ElementTypes", typeof(int[])) as int[]);
				return;
			}
			if (this.m_unityType == 8)
			{
				this.m_instantiation = (info.GetValue("GenericArguments", typeof(Type[])) as Type[]);
				this.m_elementTypes = (info.GetValue("ElementTypes", typeof(int[])) as int[]);
			}
			this.m_data = info.GetString("Data");
			this.m_assemblyName = info.GetString("AssemblyName");
		}

		private void ThrowInsufficientInformation(string field)
		{
			throw new SerializationException(Environment.GetResourceString("Serialization_InsufficientDeserializationState", new object[]
			{
				field
			}));
		}

		[SecurityCritical]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			throw new NotSupportedException(Environment.GetResourceString("NotSupported_UnitySerHolder"));
		}

		[SecurityCritical]
		public virtual object GetRealObject(StreamingContext context)
		{
			switch (this.m_unityType)
			{
			case 1:
				return Empty.Value;
			case 2:
				return DBNull.Value;
			case 3:
				return Missing.Value;
			case 4:
			{
				if (this.m_data == null || this.m_data.Length == 0)
				{
					this.ThrowInsufficientInformation("Data");
				}
				if (this.m_assemblyName == null)
				{
					this.ThrowInsufficientInformation("AssemblyName");
				}
				if (this.m_assemblyName.Length == 0)
				{
					return Type.GetType(this.m_data, true, false);
				}
				Assembly assembly = Assembly.Load(this.m_assemblyName);
				return assembly.GetType(this.m_data, true, false);
			}
			case 5:
			{
				if (this.m_data == null || this.m_data.Length == 0)
				{
					this.ThrowInsufficientInformation("Data");
				}
				if (this.m_assemblyName == null)
				{
					this.ThrowInsufficientInformation("AssemblyName");
				}
				Assembly assembly = Assembly.Load(this.m_assemblyName);
				Module module = assembly.GetModule(this.m_data);
				if (module == null)
				{
					throw new SerializationException(Environment.GetResourceString("Serialization_UnableToFindModule", new object[]
					{
						this.m_data,
						this.m_assemblyName
					}));
				}
				return module;
			}
			case 6:
				if (this.m_data == null || this.m_data.Length == 0)
				{
					this.ThrowInsufficientInformation("Data");
				}
				if (this.m_assemblyName == null)
				{
					this.ThrowInsufficientInformation("AssemblyName");
				}
				return Assembly.Load(this.m_assemblyName);
			case 7:
				if (this.m_declaringMethod == null && this.m_declaringType == null)
				{
					this.ThrowInsufficientInformation("DeclaringMember");
				}
				if (this.m_declaringMethod != null)
				{
					return this.m_declaringMethod.GetGenericArguments()[this.m_genericParameterPosition];
				}
				return this.MakeElementTypes(this.m_declaringType.GetGenericArguments()[this.m_genericParameterPosition]);
			case 8:
			{
				this.m_unityType = 4;
				Type type = this.GetRealObject(context) as Type;
				this.m_unityType = 8;
				if (this.m_instantiation[0] == null)
				{
					return null;
				}
				return this.MakeElementTypes(type.MakeGenericType(this.m_instantiation));
			}
			default:
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidUnity"));
			}
		}

		internal const int EmptyUnity = 1;

		internal const int NullUnity = 2;

		internal const int MissingUnity = 3;

		internal const int RuntimeTypeUnity = 4;

		internal const int ModuleUnity = 5;

		internal const int AssemblyUnity = 6;

		internal const int GenericParameterTypeUnity = 7;

		internal const int PartialInstantiationTypeUnity = 8;

		internal const int Pointer = 1;

		internal const int Array = 2;

		internal const int SzArray = 3;

		internal const int ByRef = 4;

		private Type[] m_instantiation;

		private int[] m_elementTypes;

		private int m_genericParameterPosition;

		private Type m_declaringType;

		private MethodBase m_declaringMethod;

		private string m_data;

		private string m_assemblyName;

		private int m_unityType;
	}
}
