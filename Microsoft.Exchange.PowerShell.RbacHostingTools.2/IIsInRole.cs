using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.PowerShell.RbacHostingTools
{
	internal interface IIsInRole
	{
		bool IsInRole(string role);

		bool IsInRole(string role, ADRawEntry adRawEntry);
	}
}
