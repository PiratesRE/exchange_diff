using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public interface IDataObjectValidator
	{
		ValidationError[] Validate(object dataObject);
	}
}
