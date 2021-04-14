using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Exchange.VariantConfiguration
{
	internal class VariantConfigurationTypeResolver
	{
		internal VariantConfigurationTypeResolver(IDictionary<Type, VariantConfigurationTypeInformation> typeData)
		{
			if (typeData == null)
			{
				throw new ArgumentNullException("typeData");
			}
			this.typeData = typeData;
			this.types = new Dictionary<string, Type>();
			foreach (Type type in this.typeData.Keys)
			{
				this.types.Add(type.Namespace + "." + type.Name, type);
			}
		}

		public static VariantConfigurationTypeResolver Create(Assembly assembly)
		{
			Assembly[] assemblies = new Assembly[]
			{
				assembly
			};
			return new VariantConfigurationTypeResolver(VariantConfigurationTypeResolver.GetTypeData(assemblies));
		}

		public Type ResolveType(string typeName)
		{
			if (string.IsNullOrEmpty(typeName))
			{
				throw new ArgumentNullException("type");
			}
			if (!this.types.ContainsKey(typeName))
			{
				return null;
			}
			return this.types[typeName];
		}

		public VariantConfigurationTypeInformation GetTypeInformation(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (!this.typeData.ContainsKey(type))
			{
				return null;
			}
			return this.typeData[type];
		}

		private static IDictionary<Type, VariantConfigurationTypeInformation> GetTypeData(Assembly[] assemblies)
		{
			Dictionary<Type, VariantConfigurationTypeInformation> dictionary = new Dictionary<Type, VariantConfigurationTypeInformation>();
			foreach (Assembly assembly in assemblies)
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (type.IsInterface && type.Name.StartsWith("I"))
					{
						dictionary.Add(type, VariantConfigurationTypeInformation.Create(type));
					}
				}
			}
			return dictionary;
		}

		private IDictionary<Type, VariantConfigurationTypeInformation> typeData;

		private IDictionary<string, Type> types;
	}
}
