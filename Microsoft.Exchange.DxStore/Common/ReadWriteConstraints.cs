using System;

namespace Microsoft.Exchange.DxStore.Common
{
	public class ReadWriteConstraints
	{
		public ReadWriteConstraints(WriteOptions writeOptions = null, ReadOptions readOptions = null)
		{
			this.WriteOptions = writeOptions;
			this.ReadOptions = readOptions;
		}

		public static ReadWriteConstraints NullConstraints { get; private set; } = new ReadWriteConstraints(null, null);

		public WriteOptions WriteOptions { get; set; }

		public ReadOptions ReadOptions { get; set; }

		public WriteResult WriteResult { get; set; }

		public ReadResult ReadResult { get; set; }

		public static ReadWriteConstraints Copy(ReadWriteConstraints source)
		{
			if (source != null)
			{
				return new ReadWriteConstraints(null, null)
				{
					WriteOptions = source.WriteOptions,
					ReadOptions = source.ReadOptions
				};
			}
			return null;
		}

		public static ReadWriteConstraints NullCheck(ReadWriteConstraints constraints)
		{
			if (constraints != null)
			{
				return constraints;
			}
			return ReadWriteConstraints.NullConstraints;
		}
	}
}
