using System;

namespace Microsoft.Exchange.Transport.Storage
{
	internal interface IDataExternalComponent : IDataObjectComponent
	{
		void MarkToDelete();

		void SaveToExternalRow(Transaction transaction);

		void ParentPrimaryKeyChanged();
	}
}
