using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal class FileMappingNotFoundException : Exception
	{
		public FileMappingNotFoundException(string message) : base(message)
		{
		}
	}
}
