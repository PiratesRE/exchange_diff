using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public sealed class NullValidator : IValidator
	{
		public ValidationError[] Validate()
		{
			return new ValidationError[0];
		}
	}
}
