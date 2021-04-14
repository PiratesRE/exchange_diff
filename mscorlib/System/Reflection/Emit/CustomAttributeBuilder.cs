using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace System.Reflection.Emit
{
	[ClassInterface(ClassInterfaceType.None)]
	[ComDefaultInterface(typeof(_CustomAttributeBuilder))]
	[ComVisible(true)]
	[HostProtection(SecurityAction.LinkDemand, MayLeakOnAbort = true)]
	public class CustomAttributeBuilder : _CustomAttributeBuilder
	{
		public CustomAttributeBuilder(ConstructorInfo con, object[] constructorArgs)
		{
			this.InitCustomAttributeBuilder(con, constructorArgs, new PropertyInfo[0], new object[0], new FieldInfo[0], new object[0]);
		}

		public CustomAttributeBuilder(ConstructorInfo con, object[] constructorArgs, PropertyInfo[] namedProperties, object[] propertyValues)
		{
			this.InitCustomAttributeBuilder(con, constructorArgs, namedProperties, propertyValues, new FieldInfo[0], new object[0]);
		}

		public CustomAttributeBuilder(ConstructorInfo con, object[] constructorArgs, FieldInfo[] namedFields, object[] fieldValues)
		{
			this.InitCustomAttributeBuilder(con, constructorArgs, new PropertyInfo[0], new object[0], namedFields, fieldValues);
		}

		public CustomAttributeBuilder(ConstructorInfo con, object[] constructorArgs, PropertyInfo[] namedProperties, object[] propertyValues, FieldInfo[] namedFields, object[] fieldValues)
		{
			this.InitCustomAttributeBuilder(con, constructorArgs, namedProperties, propertyValues, namedFields, fieldValues);
		}

		private bool ValidateType(Type t)
		{
			if (t.IsPrimitive || t == typeof(string) || t == typeof(Type))
			{
				return true;
			}
			if (t.IsEnum)
			{
				TypeCode typeCode = Type.GetTypeCode(Enum.GetUnderlyingType(t));
				return typeCode - TypeCode.SByte <= 7;
			}
			if (t.IsArray)
			{
				return t.GetArrayRank() == 1 && this.ValidateType(t.GetElementType());
			}
			return t == typeof(object);
		}

		internal void InitCustomAttributeBuilder(ConstructorInfo con, object[] constructorArgs, PropertyInfo[] namedProperties, object[] propertyValues, FieldInfo[] namedFields, object[] fieldValues)
		{
			if (con == null)
			{
				throw new ArgumentNullException("con");
			}
			if (constructorArgs == null)
			{
				throw new ArgumentNullException("constructorArgs");
			}
			if (namedProperties == null)
			{
				throw new ArgumentNullException("namedProperties");
			}
			if (propertyValues == null)
			{
				throw new ArgumentNullException("propertyValues");
			}
			if (namedFields == null)
			{
				throw new ArgumentNullException("namedFields");
			}
			if (fieldValues == null)
			{
				throw new ArgumentNullException("fieldValues");
			}
			if (namedProperties.Length != propertyValues.Length)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_ArrayLengthsDiffer"), "namedProperties, propertyValues");
			}
			if (namedFields.Length != fieldValues.Length)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_ArrayLengthsDiffer"), "namedFields, fieldValues");
			}
			if ((con.Attributes & MethodAttributes.Static) == MethodAttributes.Static || (con.Attributes & MethodAttributes.MemberAccessMask) == MethodAttributes.Private)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_BadConstructor"));
			}
			if ((con.CallingConvention & CallingConventions.Standard) != CallingConventions.Standard)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_BadConstructorCallConv"));
			}
			this.m_con = con;
			this.m_constructorArgs = new object[constructorArgs.Length];
			Array.Copy(constructorArgs, this.m_constructorArgs, constructorArgs.Length);
			Type[] parameterTypes = con.GetParameterTypes();
			if (parameterTypes.Length != constructorArgs.Length)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_BadParameterCountsForConstructor"));
			}
			for (int i = 0; i < parameterTypes.Length; i++)
			{
				if (!this.ValidateType(parameterTypes[i]))
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_BadTypeInCustomAttribute"));
				}
			}
			for (int i = 0; i < parameterTypes.Length; i++)
			{
				if (constructorArgs[i] != null)
				{
					TypeCode typeCode = Type.GetTypeCode(parameterTypes[i]);
					if (typeCode != Type.GetTypeCode(constructorArgs[i].GetType()) && (typeCode != TypeCode.Object || !this.ValidateType(constructorArgs[i].GetType())))
					{
						throw new ArgumentException(Environment.GetResourceString("Argument_BadParameterTypeForConstructor", new object[]
						{
							i
						}));
					}
				}
			}
			MemoryStream output = new MemoryStream();
			BinaryWriter binaryWriter = new BinaryWriter(output);
			binaryWriter.Write(1);
			for (int i = 0; i < constructorArgs.Length; i++)
			{
				this.EmitValue(binaryWriter, parameterTypes[i], constructorArgs[i]);
			}
			binaryWriter.Write((ushort)(namedProperties.Length + namedFields.Length));
			for (int i = 0; i < namedProperties.Length; i++)
			{
				if (namedProperties[i] == null)
				{
					throw new ArgumentNullException("namedProperties[" + i + "]");
				}
				Type propertyType = namedProperties[i].PropertyType;
				if (propertyValues[i] == null && propertyType.IsPrimitive)
				{
					throw new ArgumentNullException("propertyValues[" + i + "]");
				}
				if (!this.ValidateType(propertyType))
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_BadTypeInCustomAttribute"));
				}
				if (!namedProperties[i].CanWrite)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_NotAWritableProperty"));
				}
				if (namedProperties[i].DeclaringType != con.DeclaringType && !(con.DeclaringType is TypeBuilderInstantiation) && !con.DeclaringType.IsSubclassOf(namedProperties[i].DeclaringType) && !TypeBuilder.IsTypeEqual(namedProperties[i].DeclaringType, con.DeclaringType) && (!(namedProperties[i].DeclaringType is TypeBuilder) || !con.DeclaringType.IsSubclassOf(((TypeBuilder)namedProperties[i].DeclaringType).BakedRuntimeType)))
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_BadPropertyForConstructorBuilder"));
				}
				if (propertyValues[i] != null && propertyType != typeof(object) && Type.GetTypeCode(propertyValues[i].GetType()) != Type.GetTypeCode(propertyType))
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_ConstantDoesntMatch"));
				}
				binaryWriter.Write(84);
				this.EmitType(binaryWriter, propertyType);
				this.EmitString(binaryWriter, namedProperties[i].Name);
				this.EmitValue(binaryWriter, propertyType, propertyValues[i]);
			}
			for (int i = 0; i < namedFields.Length; i++)
			{
				if (namedFields[i] == null)
				{
					throw new ArgumentNullException("namedFields[" + i + "]");
				}
				Type fieldType = namedFields[i].FieldType;
				if (fieldValues[i] == null && fieldType.IsPrimitive)
				{
					throw new ArgumentNullException("fieldValues[" + i + "]");
				}
				if (!this.ValidateType(fieldType))
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_BadTypeInCustomAttribute"));
				}
				if (namedFields[i].DeclaringType != con.DeclaringType && !(con.DeclaringType is TypeBuilderInstantiation) && !con.DeclaringType.IsSubclassOf(namedFields[i].DeclaringType) && !TypeBuilder.IsTypeEqual(namedFields[i].DeclaringType, con.DeclaringType) && (!(namedFields[i].DeclaringType is TypeBuilder) || !con.DeclaringType.IsSubclassOf(((TypeBuilder)namedFields[i].DeclaringType).BakedRuntimeType)))
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_BadFieldForConstructorBuilder"));
				}
				if (fieldValues[i] != null && fieldType != typeof(object) && Type.GetTypeCode(fieldValues[i].GetType()) != Type.GetTypeCode(fieldType))
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_ConstantDoesntMatch"));
				}
				binaryWriter.Write(83);
				this.EmitType(binaryWriter, fieldType);
				this.EmitString(binaryWriter, namedFields[i].Name);
				this.EmitValue(binaryWriter, fieldType, fieldValues[i]);
			}
			this.m_blob = ((MemoryStream)binaryWriter.BaseStream).ToArray();
		}

		private void EmitType(BinaryWriter writer, Type type)
		{
			if (type.IsPrimitive)
			{
				switch (Type.GetTypeCode(type))
				{
				case TypeCode.Boolean:
					writer.Write(2);
					return;
				case TypeCode.Char:
					writer.Write(3);
					return;
				case TypeCode.SByte:
					writer.Write(4);
					return;
				case TypeCode.Byte:
					writer.Write(5);
					return;
				case TypeCode.Int16:
					writer.Write(6);
					return;
				case TypeCode.UInt16:
					writer.Write(7);
					return;
				case TypeCode.Int32:
					writer.Write(8);
					return;
				case TypeCode.UInt32:
					writer.Write(9);
					return;
				case TypeCode.Int64:
					writer.Write(10);
					return;
				case TypeCode.UInt64:
					writer.Write(11);
					return;
				case TypeCode.Single:
					writer.Write(12);
					return;
				case TypeCode.Double:
					writer.Write(13);
					return;
				default:
					return;
				}
			}
			else
			{
				if (type.IsEnum)
				{
					writer.Write(85);
					this.EmitString(writer, type.AssemblyQualifiedName);
					return;
				}
				if (type == typeof(string))
				{
					writer.Write(14);
					return;
				}
				if (type == typeof(Type))
				{
					writer.Write(80);
					return;
				}
				if (type.IsArray)
				{
					writer.Write(29);
					this.EmitType(writer, type.GetElementType());
					return;
				}
				writer.Write(81);
				return;
			}
		}

		private void EmitString(BinaryWriter writer, string str)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			uint num = (uint)bytes.Length;
			if (num <= 127U)
			{
				writer.Write((byte)num);
			}
			else if (num <= 16383U)
			{
				writer.Write((byte)(num >> 8 | 128U));
				writer.Write((byte)(num & 255U));
			}
			else
			{
				writer.Write((byte)(num >> 24 | 192U));
				writer.Write((byte)(num >> 16 & 255U));
				writer.Write((byte)(num >> 8 & 255U));
				writer.Write((byte)(num & 255U));
			}
			writer.Write(bytes);
		}

		private void EmitValue(BinaryWriter writer, Type type, object value)
		{
			if (type.IsEnum)
			{
				switch (Type.GetTypeCode(Enum.GetUnderlyingType(type)))
				{
				case TypeCode.SByte:
					writer.Write((sbyte)value);
					return;
				case TypeCode.Byte:
					writer.Write((byte)value);
					return;
				case TypeCode.Int16:
					writer.Write((short)value);
					return;
				case TypeCode.UInt16:
					writer.Write((ushort)value);
					return;
				case TypeCode.Int32:
					writer.Write((int)value);
					return;
				case TypeCode.UInt32:
					writer.Write((uint)value);
					return;
				case TypeCode.Int64:
					writer.Write((long)value);
					return;
				case TypeCode.UInt64:
					writer.Write((ulong)value);
					return;
				default:
					return;
				}
			}
			else if (type == typeof(string))
			{
				if (value == null)
				{
					writer.Write(byte.MaxValue);
					return;
				}
				this.EmitString(writer, (string)value);
				return;
			}
			else if (type == typeof(Type))
			{
				if (value == null)
				{
					writer.Write(byte.MaxValue);
					return;
				}
				string text = TypeNameBuilder.ToString((Type)value, TypeNameBuilder.Format.AssemblyQualifiedName);
				if (text == null)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_InvalidTypeForCA", new object[]
					{
						value.GetType()
					}));
				}
				this.EmitString(writer, text);
				return;
			}
			else if (type.IsArray)
			{
				if (value == null)
				{
					writer.Write(uint.MaxValue);
					return;
				}
				Array array = (Array)value;
				Type elementType = type.GetElementType();
				writer.Write(array.Length);
				for (int i = 0; i < array.Length; i++)
				{
					this.EmitValue(writer, elementType, array.GetValue(i));
				}
				return;
			}
			else if (type.IsPrimitive)
			{
				switch (Type.GetTypeCode(type))
				{
				case TypeCode.Boolean:
					writer.Write(((bool)value) ? 1 : 0);
					return;
				case TypeCode.Char:
					writer.Write(Convert.ToUInt16((char)value));
					return;
				case TypeCode.SByte:
					writer.Write((sbyte)value);
					return;
				case TypeCode.Byte:
					writer.Write((byte)value);
					return;
				case TypeCode.Int16:
					writer.Write((short)value);
					return;
				case TypeCode.UInt16:
					writer.Write((ushort)value);
					return;
				case TypeCode.Int32:
					writer.Write((int)value);
					return;
				case TypeCode.UInt32:
					writer.Write((uint)value);
					return;
				case TypeCode.Int64:
					writer.Write((long)value);
					return;
				case TypeCode.UInt64:
					writer.Write((ulong)value);
					return;
				case TypeCode.Single:
					writer.Write((float)value);
					return;
				case TypeCode.Double:
					writer.Write((double)value);
					return;
				default:
					return;
				}
			}
			else
			{
				if (!(type == typeof(object)))
				{
					string text2 = "null";
					if (value != null)
					{
						text2 = value.GetType().ToString();
					}
					throw new ArgumentException(Environment.GetResourceString("Argument_BadParameterTypeForCAB", new object[]
					{
						text2
					}));
				}
				Type type2 = (value == null) ? typeof(string) : ((value is Type) ? typeof(Type) : value.GetType());
				if (type2 == typeof(object))
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_BadParameterTypeForCAB", new object[]
					{
						type2.ToString()
					}));
				}
				this.EmitType(writer, type2);
				this.EmitValue(writer, type2, value);
				return;
			}
		}

		[SecurityCritical]
		internal void CreateCustomAttribute(ModuleBuilder mod, int tkOwner)
		{
			this.CreateCustomAttribute(mod, tkOwner, mod.GetConstructorToken(this.m_con).Token, false);
		}

		[SecurityCritical]
		internal int PrepareCreateCustomAttributeToDisk(ModuleBuilder mod)
		{
			return mod.InternalGetConstructorToken(this.m_con, true).Token;
		}

		[SecurityCritical]
		internal void CreateCustomAttribute(ModuleBuilder mod, int tkOwner, int tkAttrib, bool toDisk)
		{
			TypeBuilder.DefineCustomAttribute(mod, tkOwner, tkAttrib, this.m_blob, toDisk, typeof(DebuggableAttribute) == this.m_con.DeclaringType);
		}

		void _CustomAttributeBuilder.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		void _CustomAttributeBuilder.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		void _CustomAttributeBuilder.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		void _CustomAttributeBuilder.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		internal ConstructorInfo m_con;

		internal object[] m_constructorArgs;

		internal byte[] m_blob;
	}
}
