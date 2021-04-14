﻿using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	internal sealed class TypeBuilderInstantiation : TypeInfo
	{
		public override bool IsAssignableFrom(TypeInfo typeInfo)
		{
			return !(typeInfo == null) && this.IsAssignableFrom(typeInfo.AsType());
		}

		internal static Type MakeGenericType(Type type, Type[] typeArguments)
		{
			if (!type.IsGenericTypeDefinition)
			{
				throw new InvalidOperationException();
			}
			if (typeArguments == null)
			{
				throw new ArgumentNullException("typeArguments");
			}
			foreach (Type left in typeArguments)
			{
				if (left == null)
				{
					throw new ArgumentNullException("typeArguments");
				}
			}
			return new TypeBuilderInstantiation(type, typeArguments);
		}

		private TypeBuilderInstantiation(Type type, Type[] inst)
		{
			this.m_type = type;
			this.m_inst = inst;
			this.m_hashtable = new Hashtable();
		}

		public override string ToString()
		{
			return TypeNameBuilder.ToString(this, TypeNameBuilder.Format.ToString);
		}

		public override Type DeclaringType
		{
			get
			{
				return this.m_type.DeclaringType;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return this.m_type.ReflectedType;
			}
		}

		public override string Name
		{
			get
			{
				return this.m_type.Name;
			}
		}

		public override Module Module
		{
			get
			{
				return this.m_type.Module;
			}
		}

		public override Type MakePointerType()
		{
			return SymbolType.FormCompoundType("*".ToCharArray(), this, 0);
		}

		public override Type MakeByRefType()
		{
			return SymbolType.FormCompoundType("&".ToCharArray(), this, 0);
		}

		public override Type MakeArrayType()
		{
			return SymbolType.FormCompoundType("[]".ToCharArray(), this, 0);
		}

		public override Type MakeArrayType(int rank)
		{
			if (rank <= 0)
			{
				throw new IndexOutOfRangeException();
			}
			string text = "";
			for (int i = 1; i < rank; i++)
			{
				text += ",";
			}
			string text2 = string.Format(CultureInfo.InvariantCulture, "[{0}]", text);
			return SymbolType.FormCompoundType(text2.ToCharArray(), this, 0);
		}

		public override Guid GUID
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
		{
			throw new NotSupportedException();
		}

		public override Assembly Assembly
		{
			get
			{
				return this.m_type.Assembly;
			}
		}

		public override RuntimeTypeHandle TypeHandle
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		public override string FullName
		{
			get
			{
				if (this.m_strFullQualName == null)
				{
					this.m_strFullQualName = TypeNameBuilder.ToString(this, TypeNameBuilder.Format.FullName);
				}
				return this.m_strFullQualName;
			}
		}

		public override string Namespace
		{
			get
			{
				return this.m_type.Namespace;
			}
		}

		public override string AssemblyQualifiedName
		{
			get
			{
				return TypeNameBuilder.ToString(this, TypeNameBuilder.Format.AssemblyQualifiedName);
			}
		}

		private Type Substitute(Type[] substitutes)
		{
			Type[] genericArguments = this.GetGenericArguments();
			Type[] array = new Type[genericArguments.Length];
			for (int i = 0; i < array.Length; i++)
			{
				Type type = genericArguments[i];
				if (type is TypeBuilderInstantiation)
				{
					array[i] = (type as TypeBuilderInstantiation).Substitute(substitutes);
				}
				else if (type is GenericTypeParameterBuilder)
				{
					array[i] = substitutes[type.GenericParameterPosition];
				}
				else
				{
					array[i] = type;
				}
			}
			return this.GetGenericTypeDefinition().MakeGenericType(array);
		}

		public override Type BaseType
		{
			get
			{
				Type baseType = this.m_type.BaseType;
				if (baseType == null)
				{
					return null;
				}
				TypeBuilderInstantiation typeBuilderInstantiation = baseType as TypeBuilderInstantiation;
				if (typeBuilderInstantiation == null)
				{
					return baseType;
				}
				return typeBuilderInstantiation.Substitute(this.GetGenericArguments());
			}
		}

		protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			throw new NotSupportedException();
		}

		[ComVisible(true)]
		public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}

		protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			throw new NotSupportedException();
		}

		public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}

		public override FieldInfo GetField(string name, BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}

		public override FieldInfo[] GetFields(BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}

		public override Type GetInterface(string name, bool ignoreCase)
		{
			throw new NotSupportedException();
		}

		public override Type[] GetInterfaces()
		{
			throw new NotSupportedException();
		}

		public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}

		public override EventInfo[] GetEvents()
		{
			throw new NotSupportedException();
		}

		protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			throw new NotSupportedException();
		}

		public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}

		public override Type[] GetNestedTypes(BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}

		public override Type GetNestedType(string name, BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}

		public override MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}

		[ComVisible(true)]
		public override InterfaceMapping GetInterfaceMap(Type interfaceType)
		{
			throw new NotSupportedException();
		}

		public override EventInfo[] GetEvents(BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}

		public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
		{
			throw new NotSupportedException();
		}

		protected override TypeAttributes GetAttributeFlagsImpl()
		{
			return this.m_type.Attributes;
		}

		protected override bool IsArrayImpl()
		{
			return false;
		}

		protected override bool IsByRefImpl()
		{
			return false;
		}

		protected override bool IsPointerImpl()
		{
			return false;
		}

		protected override bool IsPrimitiveImpl()
		{
			return false;
		}

		protected override bool IsCOMObjectImpl()
		{
			return false;
		}

		public override Type GetElementType()
		{
			throw new NotSupportedException();
		}

		protected override bool HasElementTypeImpl()
		{
			return false;
		}

		public override Type UnderlyingSystemType
		{
			get
			{
				return this;
			}
		}

		public override Type[] GetGenericArguments()
		{
			return this.m_inst;
		}

		public override bool IsGenericTypeDefinition
		{
			get
			{
				return false;
			}
		}

		public override bool IsGenericType
		{
			get
			{
				return true;
			}
		}

		public override bool IsConstructedGenericType
		{
			get
			{
				return true;
			}
		}

		public override bool IsGenericParameter
		{
			get
			{
				return false;
			}
		}

		public override int GenericParameterPosition
		{
			get
			{
				throw new InvalidOperationException();
			}
		}

		protected override bool IsValueTypeImpl()
		{
			return this.m_type.IsValueType;
		}

		public override bool ContainsGenericParameters
		{
			get
			{
				for (int i = 0; i < this.m_inst.Length; i++)
				{
					if (this.m_inst[i].ContainsGenericParameters)
					{
						return true;
					}
				}
				return false;
			}
		}

		public override MethodBase DeclaringMethod
		{
			get
			{
				return null;
			}
		}

		public override Type GetGenericTypeDefinition()
		{
			return this.m_type;
		}

		public override Type MakeGenericType(params Type[] inst)
		{
			throw new InvalidOperationException(Environment.GetResourceString("Arg_NotGenericTypeDefinition"));
		}

		public override bool IsAssignableFrom(Type c)
		{
			throw new NotSupportedException();
		}

		[ComVisible(true)]
		public override bool IsSubclassOf(Type c)
		{
			throw new NotSupportedException();
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			throw new NotSupportedException();
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			throw new NotSupportedException();
		}

		private Type m_type;

		private Type[] m_inst;

		private string m_strFullQualName;

		internal Hashtable m_hashtable = new Hashtable();
	}
}
