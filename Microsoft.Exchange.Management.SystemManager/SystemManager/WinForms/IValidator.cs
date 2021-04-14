using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface IValidator
	{
		ValidationError[] Validate();
	}
}
