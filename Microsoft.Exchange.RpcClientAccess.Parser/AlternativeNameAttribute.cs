using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	internal sealed class AlternativeNameAttribute : Attribute
	{
		public AlternativeNameAttribute(AlternativeNaming scheme, string name)
		{
			this.Scheme = scheme;
			this.Name = name;
		}

		public AlternativeNaming Scheme { get; private set; }

		public string Name { get; private set; }

		internal static ILookup<TEnum, string> GetMappingForEnum<TEnum>(AlternativeNaming scheme)
		{
			if (!typeof(TEnum).IsEnum)
			{
				throw new ArgumentException("Must be of an enumerated type", "TEnum");
			}
			return (from field in typeof(TEnum).GetFields(BindingFlags.Static | BindingFlags.Public)
			from attr in 
				from AlternativeNameAttribute attr in Attribute.GetCustomAttributes(field, typeof(AlternativeNameAttribute))
				where attr.Scheme == scheme
				select attr
			select new KeyValuePair<TEnum, string>((TEnum)((object)field.GetRawConstantValue()), attr.Name)).ToLookup((KeyValuePair<TEnum, string> pair) => pair.Key, (KeyValuePair<TEnum, string> pair) => pair.Value);
		}
	}
}
