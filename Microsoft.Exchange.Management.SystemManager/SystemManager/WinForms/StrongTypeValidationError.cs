using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class StrongTypeValidationError : ValidationError
	{
		public StrongTypeValidationError(LocalizedString description, string propertyName) : base(description, propertyName)
		{
		}
	}
}
