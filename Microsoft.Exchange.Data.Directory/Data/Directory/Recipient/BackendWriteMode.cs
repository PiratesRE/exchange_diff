using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal enum BackendWriteMode
	{
		NoWrites,
		WriteToMServ,
		WriteToMbx
	}
}
