using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SetupValidationError : ValidationError
	{
		public SetupValidationError(LocalizedString description) : base(description)
		{
		}
	}
}
