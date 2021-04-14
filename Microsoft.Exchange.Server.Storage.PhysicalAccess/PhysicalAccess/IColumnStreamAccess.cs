using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public interface IColumnStreamAccess
	{
		int GetColumnSize(PhysicalColumn column);

		int ReadStream(PhysicalColumn physicalColumn, long position, byte[] buffer, int offset, int count);

		void WriteStream(PhysicalColumn physicalColumn, long position, byte[] buffer, int offset, int count);
	}
}
