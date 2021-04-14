using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class IMAPJobSubscriptionSettings : JobSubscriptionSettingsBase
	{
		public string[] ExcludedFolders { get; private set; }

		public override PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return SubscriptionSettingsBase.SubscriptionSettingsBasePropertyDefinitions;
			}
		}

		public override void WriteToBatch(MigrationBatch batch)
		{
			batch.ExcludedFolders = this.ExcludedFolders;
		}

		public override void WriteExtendedProperties(PersistableDictionary dictionary)
		{
			dictionary.SetMultiValuedProperty("ExcludedFolders", this.ExcludedFolders);
		}

		public override bool ReadExtendedProperties(PersistableDictionary dictionary)
		{
			MultiValuedProperty<string> multiValuedStringProperty = dictionary.GetMultiValuedStringProperty("ExcludedFolders");
			if (multiValuedStringProperty != null)
			{
				this.ExcludedFolders = multiValuedStringProperty.ToArray();
				return true;
			}
			return false;
		}

		protected override void InitalizeFromBatch(MigrationBatch batch)
		{
			this.ExcludedFolders = ((batch.ExcludedFolders == null) ? null : batch.ExcludedFolders.ToArray());
		}

		protected override void AddDiagnosticInfoToElement(IMigrationDataProvider dataProvider, XElement parent, MigrationDiagnosticArgument argument)
		{
			parent.Add(new XElement("ExcludedFolders", string.Join(",", this.ExcludedFolders ?? new string[0])));
		}
	}
}
