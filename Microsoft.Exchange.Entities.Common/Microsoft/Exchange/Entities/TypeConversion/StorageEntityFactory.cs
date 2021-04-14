using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.TypeConversion.Translators;

namespace Microsoft.Exchange.Entities.TypeConversion
{
	internal static class StorageEntityFactory
	{
		public static IItem CreateFromItem(IItem storageItem)
		{
			foreach (StorageEntityFactory.SupportedTypeInfo supportedTypeInfo in StorageEntityFactory.SupportedTypes)
			{
				if (supportedTypeInfo.StorageType.IsInstanceOfType(storageItem))
				{
					return supportedTypeInfo.Translator.ConvertToEntity(storageItem);
				}
			}
			throw new InvalidOperationException();
		}

		private static readonly List<StorageEntityFactory.SupportedTypeInfo> SupportedTypes = new List<StorageEntityFactory.SupportedTypeInfo>
		{
			new StorageEntityFactory.SupportedTypeInfo(typeof(ICalendarItemBase), "Microsoft.Exchange.Entities.Calendaring", "Microsoft.Exchange.Entities.Calendaring.TypeConversion.Translators.EventTranslator", "Instance"),
			new StorageEntityFactory.SupportedTypeInfo(typeof(IItem), ItemTranslator<IItem, Item<ItemSchema>, ItemSchema>.Instance)
		};

		private class SupportedTypeInfo
		{
			public SupportedTypeInfo(Type storageType, IGenericItemTranslator translator)
			{
				this.StorageType = storageType;
				this.Translator = translator;
			}

			public SupportedTypeInfo(Type storageType, string assemblyName, string typeName, string getterName) : this(storageType, StorageEntityFactory.SupportedTypeInfo.ResolveTranslator(assemblyName, typeName, getterName))
			{
			}

			public IGenericItemTranslator Translator { get; private set; }

			public Type StorageType { get; private set; }

			private static IGenericItemTranslator ResolveTranslator(string assemblyName, string typeName, string getterName)
			{
				AssemblyName name = Assembly.GetExecutingAssembly().GetName();
				string assemblyName2 = name.FullName.Replace(name.Name, assemblyName);
				AssemblyName assemblyRef = new AssemblyName(assemblyName2);
				Assembly assembly = Assembly.Load(assemblyRef);
				Type type = assembly.GetType(typeName);
				PropertyInfo property = type.GetProperty(getterName);
				return (IGenericItemTranslator)property.GetValue(null);
			}
		}
	}
}
