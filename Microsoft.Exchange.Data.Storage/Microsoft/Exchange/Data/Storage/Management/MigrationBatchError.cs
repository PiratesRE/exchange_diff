using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public sealed class MigrationBatchError : MigrationError
	{
		public int RowIndex { get; internal set; }

		public override string ToString()
		{
			return ServerStrings.MigrationBatchErrorString(this.RowIndex, base.EmailAddress, base.LocalizedErrorMessage);
		}
	}
}
