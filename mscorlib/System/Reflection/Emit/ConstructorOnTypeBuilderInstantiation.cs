using System;
using System.Globalization;

namespace System.Reflection.Emit
{
	internal sealed class ConstructorOnTypeBuilderInstantiation : ConstructorInfo
	{
		internal static ConstructorInfo GetConstructor(ConstructorInfo Constructor, TypeBuilderInstantiation type)
		{
			return new ConstructorOnTypeBuilderInstantiation(Constructor, type);
		}

		internal ConstructorOnTypeBuilderInstantiation(ConstructorInfo constructor, TypeBuilderInstantiation type)
		{
			this.m_ctor = constructor;
			this.m_type = type;
		}

		internal override Type[] GetParameterTypes()
		{
			return this.m_ctor.GetParameterTypes();
		}

		internal override Type GetReturnType()
		{
			return this.DeclaringType;
		}

		public override MemberTypes MemberType
		{
			get
			{
				return this.m_ctor.MemberType;
			}
		}

		public override string Name
		{
			get
			{
				return this.m_ctor.Name;
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
			return this.m_ctor.GetCustomAttributes(inherit);
		}

		public override object[] GetCustomAttributes(Type attributeType, bool inherit)
		{
			return this.m_ctor.GetCustomAttributes(attributeType, inherit);
		}

		public override bool IsDefined(Type attributeType, bool inherit)
		{
			return this.m_ctor.IsDefined(attributeType, inherit);
		}

		internal int MetadataTokenInternal
		{
			get
			{
				ConstructorBuilder constructorBuilder = this.m_ctor as ConstructorBuilder;
				if (constructorBuilder != null)
				{
					return constructorBuilder.MetadataTokenInternal;
				}
				return this.m_ctor.MetadataToken;
			}
		}

		public override Module Module
		{
			get
			{
				return this.m_ctor.Module;
			}
		}

		public new Type GetType()
		{
			return base.GetType();
		}

		public override ParameterInfo[] GetParameters()
		{
			return this.m_ctor.GetParameters();
		}

		public override MethodImplAttributes GetMethodImplementationFlags()
		{
			return this.m_ctor.GetMethodImplementationFlags();
		}

		public override RuntimeMethodHandle MethodHandle
		{
			get
			{
				return this.m_ctor.MethodHandle;
			}
		}

		public override MethodAttributes Attributes
		{
			get
			{
				return this.m_ctor.Attributes;
			}
		}

		public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw new NotSupportedException();
		}

		public override CallingConventions CallingConvention
		{
			get
			{
				return this.m_ctor.CallingConvention;
			}
		}

		public override Type[] GetGenericArguments()
		{
			return this.m_ctor.GetGenericArguments();
		}

		public override bool IsGenericMethodDefinition
		{
			get
			{
				return false;
			}
		}

		public override bool ContainsGenericParameters
		{
			get
			{
				return false;
			}
		}

		public override bool IsGenericMethod
		{
			get
			{
				return false;
			}
		}

		public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
		{
			throw new InvalidOperationException();
		}

		internal ConstructorInfo m_ctor;

		private TypeBuilderInstantiation m_type;
	}
}
