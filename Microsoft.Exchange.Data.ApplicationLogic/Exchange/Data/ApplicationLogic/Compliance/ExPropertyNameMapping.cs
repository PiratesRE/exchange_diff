using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Compliance
{
	internal static class ExPropertyNameMapping
	{
		internal static Dictionary<string, string> Mapping
		{
			get
			{
				if (ExPropertyNameMapping.mapping == null)
				{
					ExPropertyNameMapping.mapping = new Dictionary<string, string>();
					ExPropertyNameMapping.mapping["Item.Extension"] = "Item.Extension";
					ExPropertyNameMapping.mapping["Item.DisplayName"] = "Subject";
					ExPropertyNameMapping.mapping["Item.WhenCreated"] = "Sent";
					ExPropertyNameMapping.mapping["Item.WhenModified"] = "Item.WhenModified";
					ExPropertyNameMapping.mapping["Item.LastModifier"] = "Item.LastModifier";
					ExPropertyNameMapping.mapping["Item.Creator"] = "From";
					ExPropertyNameMapping.mapping["Item.ExpiryTime"] = "Expires";
					ExPropertyNameMapping.mapping["Item.CreationAgeInDays"] = "Received";
					ExPropertyNameMapping.mapping["Item.CreationAgeInMonths"] = "Received";
					ExPropertyNameMapping.mapping["Item.CreationAgeInYears"] = "Received";
					ExPropertyNameMapping.mapping["Item.ModificationAgeInDays"] = "Received";
					ExPropertyNameMapping.mapping["Item.ModificationAgeInMonths"] = "Received";
					ExPropertyNameMapping.mapping["Item.ModificationAgeInYears"] = "Item.Extension";
					ExPropertyNameMapping.mapping["Item.ClassificationDiscovered"] = "Item.ClassificationDiscovered";
					ExPropertyNameMapping.mapping["Item.Extension"] = "Item.Extension";
				}
				return ExPropertyNameMapping.mapping;
			}
		}

		private const string Extension = "Item.Extension";

		private const string DisplayName = "Subject";

		private const string WhenCreated = "Sent";

		private const string WhenModified = "Item.WhenModified";

		private const string Creator = "From";

		private const string LastModifier = "Item.LastModifier";

		private const string ExpiryTime = "Expires";

		private const string CreationAgeInDays = "Received";

		private const string CreationAgeInMonths = "Received";

		private const string CreationAgeInYears = "Received";

		private const string ModificationAgeInDays = "Received";

		private const string ModificationAgeInMonths = "Received";

		private const string ModificationAgeInYears = "Received";

		private const string ClassificationDiscovered = "Item.ClassificationDiscovered";

		private static Dictionary<string, string> mapping;
	}
}
