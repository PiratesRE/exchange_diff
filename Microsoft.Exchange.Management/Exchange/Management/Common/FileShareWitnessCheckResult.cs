using System;

namespace Microsoft.Exchange.Management.Common
{
	internal enum FileShareWitnessCheckResult
	{
		FswOK,
		FswDoesNotExist,
		FswWrongDirectory,
		FswWrongPermissions
	}
}
