using System;
using System.Reflection;

namespace Microsoft.Exchange.Diagnostics.Internal
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
	internal sealed class TaggedFieldAttribute : Attribute
	{
		public TaggedFieldAttribute(int id)
		{
			this.id = id;
		}

		public int Id
		{
			get
			{
				return this.id;
			}
		}

		public static FieldInfo FindTaggedField(Type type, BindingFlags bindingFlags, int fieldId)
		{
			foreach (FieldInfo fieldInfo in type.GetFields(bindingFlags))
			{
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(TaggedFieldAttribute), false);
				if (customAttributes.Length > 0)
				{
					TaggedFieldAttribute taggedFieldAttribute = (TaggedFieldAttribute)customAttributes[0];
					if (taggedFieldAttribute.Id == fieldId)
					{
						return fieldInfo;
					}
				}
			}
			return null;
		}

		private readonly int id;
	}
}
