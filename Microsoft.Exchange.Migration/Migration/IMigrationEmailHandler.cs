using System;

namespace Microsoft.Exchange.Migration
{
	internal interface IMigrationEmailHandler
	{
		IMigrationEmailMessageItem CreateEmailMessage();
	}
}
