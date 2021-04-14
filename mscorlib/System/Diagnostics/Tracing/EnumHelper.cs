using System;
using System.Reflection;

namespace System.Diagnostics.Tracing
{
	internal static class EnumHelper<UnderlyingType>
	{
		public static UnderlyingType Cast<ValueType>(ValueType value)
		{
			return EnumHelper<UnderlyingType>.Caster<ValueType>.Instance(value);
		}

		internal static UnderlyingType Identity(UnderlyingType value)
		{
			return value;
		}

		private static readonly MethodInfo IdentityInfo = Statics.GetDeclaredStaticMethod(typeof(EnumHelper<UnderlyingType>), "Identity");

		private delegate UnderlyingType Transformer<ValueType>(ValueType value);

		private static class Caster<ValueType>
		{
			public static readonly EnumHelper<UnderlyingType>.Transformer<ValueType> Instance = (EnumHelper<UnderlyingType>.Transformer<ValueType>)Statics.CreateDelegate(typeof(EnumHelper<UnderlyingType>.Transformer<ValueType>), EnumHelper<UnderlyingType>.IdentityInfo);
		}
	}
}
