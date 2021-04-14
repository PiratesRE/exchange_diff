using System;
using System.Globalization;

namespace System.Reflection.Emit
{
	internal sealed class FieldOnTypeBuilderInstantiation : FieldInfo
	{
		internal static FieldInfo GetField(FieldInfo Field, TypeBuilderInstantiation type)
		{
			FieldInfo fieldInfo;
			if (type.m_hashtable.Contains(Field))
			{
				fieldInfo = (type.m_hashtable[Field] as FieldInfo);
			}
			else
			{
				fieldInfo = new FieldOnTypeBuilderInstantiation(Field, type);
				type.m_hashtable[Field] = fieldInfo;
			}
			return fieldInfo;
		}

		internal FieldOnTypeBuilderInstantiation(FieldInfo field, TypeBuilderInstantiation type)
		{
			this.m_field = field;
			this.m_type = type;
		}

		internal FieldInfo FieldInfo
		{
			get
			{
				return this.m_field;
			}
		}

		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.Field;
			}
		}

		public override string Name
		{
			get
			{
				return this.m_field.Name;
			}
		}

		public override Type DeclaringType
		{
			get
			{
				return this.m_type;
			}
		}

		public override Type ReflectedType
		{
			get
			{
				return this.m_type;
			}
		}

		public override object[] GetCustomAttributes(bool inherit)
		{
			return this.m_field.GetCustomAttributes(inherit);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return this.m_field.GetCustomAttributes(attributeType, inherit);
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return this.m_field.IsDefined(attributeType, inherit);
		}

		internal int MetadataTokenInternal
		{
			get
			{
				FieldBuilder fieldBuilder = this.m_field as FieldBuilder;
				if (fieldBuilder != null)
				{
					return fieldBuilder.MetadataTokenInternal;
				}
				return this.m_field.MetadataToken;
			}
		}

		public override Module Module
		{
			get
			{
				return this.m_field.Module;
			}
		}

		public new Type GetType()
		{
			return base.GetType();
		}

		public override Type[] GetRequiredCustomModifiers()
		{
			return this.m_field.GetRequiredCustomModifiers();
		}

		public override Type[] GetOptionalCustomModifiers()
		{
			return this.m_field.GetOptionalCustomModifiers();
		}

		public override void SetValueDirect(TypedReference obj, object value)
		{
			throw new NotImplementedException();
		}

		public override object GetValueDirect(TypedReference obj)
		{
			throw new NotImplementedException();
		}

		public override RuntimeFieldHandle FieldHandle
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override Type FieldType
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override object GetValue(object obj)
		{
			throw new InvalidOperationException();
		}

		public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
		{
			throw new InvalidOperationException();
		}

		public override FieldAttributes Attributes
		{
			get
			{
				return this.m_field.Attributes;
			}
		}

		private FieldInfo m_field;

		private TypeBuilderInstantiation m_type;
	}
}
