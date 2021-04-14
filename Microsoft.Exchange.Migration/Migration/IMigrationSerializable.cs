using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMigrationSerializable
	{
		PropertyDefinition[] PropertyDefinitions { get; }

		bool ReadFromMessageItem(IMigrationStoreObject message);

		void WriteToMessageItem(IMigrationStoreObject message, bool loaded);

		XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument);
	}
}
