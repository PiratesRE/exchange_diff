using System;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ContactDictionaryEntryNestedProperty : ContactDictionaryEntryProperty
	{
		private ContactDictionaryEntryNestedProperty(CommandContext commandContext, string[] xmlNestedLocalNames) : base(commandContext, xmlNestedLocalNames)
		{
		}

		private static ContactDictionaryEntryNestedProperty CreateCommand(CommandContext commandContext, string[] xmlNestedElements)
		{
			return new ContactDictionaryEntryNestedProperty(commandContext, xmlNestedElements);
		}

		public static ContactDictionaryEntryNestedProperty CreateCommandForStreet(CommandContext commandContext)
		{
			return ContactDictionaryEntryNestedProperty.CreateCommand(commandContext, new string[]
			{
				"PhysicalAddresses",
				"Entry",
				"Street"
			});
		}

		public static ContactDictionaryEntryNestedProperty CreateCommandForCity(CommandContext commandContext)
		{
			return ContactDictionaryEntryNestedProperty.CreateCommand(commandContext, new string[]
			{
				"PhysicalAddresses",
				"Entry",
				"City"
			});
		}

		public static ContactDictionaryEntryNestedProperty CreateCommandForState(CommandContext commandContext)
		{
			return ContactDictionaryEntryNestedProperty.CreateCommand(commandContext, new string[]
			{
				"PhysicalAddresses",
				"Entry",
				"State"
			});
		}

		public static ContactDictionaryEntryNestedProperty CreateCommandForCountryOrRegion(CommandContext commandContext)
		{
			return ContactDictionaryEntryNestedProperty.CreateCommand(commandContext, new string[]
			{
				"PhysicalAddresses",
				"Entry",
				"CountryOrRegion"
			});
		}

		public static ContactDictionaryEntryNestedProperty CreateCommandForPostalCode(CommandContext commandContext)
		{
			return ContactDictionaryEntryNestedProperty.CreateCommand(commandContext, new string[]
			{
				"PhysicalAddresses",
				"Entry",
				"PostalCode"
			});
		}
	}
}
