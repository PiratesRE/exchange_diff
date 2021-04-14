using System;

namespace Microsoft.Exchange.Data.Directory
{
	public interface IUsnChanged
	{
		long UsnChanged { get; }
	}
}
