using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MigrationDataProviderEmailHandler : IMigrationEmailHandler
	{
		public MigrationDataProviderEmailHandler(IMigrationDataProvider dataProvider)
		{
			this.dataProvider = dataProvider;
		}

		public IMigrationEmailMessageItem CreateEmailMessage()
		{
			return this.dataProvider.CreateEmailMessage();
		}

		private readonly IMigrationDataProvider dataProvider;
	}
}
