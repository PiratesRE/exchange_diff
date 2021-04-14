using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GroupOperationResult
	{
		public GroupOperationResult(OperationResult operationResult, StoreObjectId[] objectIds, LocalizedException storageException, StoreObjectId[] resultObjectIds, byte[][] resultChangeKeys)
		{
			EnumValidator.ThrowIfInvalid<OperationResult>(operationResult, "operationResult");
			this.OperationResult = operationResult;
			this.ObjectIds = new ReadOnlyCollection<StoreObjectId>(objectIds);
			this.Exception = storageException;
			this.ResultObjectIds = new ReadOnlyCollection<StoreObjectId>(resultObjectIds);
			this.ResultChangeKeys = new ReadOnlyCollection<byte[]>(resultChangeKeys);
		}

		public GroupOperationResult(OperationResult operationResult, IList<StoreObjectId> objectIds, LocalizedException storageException)
		{
			EnumValidator.ThrowIfInvalid<OperationResult>(operationResult, "operationResult");
			this.OperationResult = operationResult;
			this.ObjectIds = new ReadOnlyCollection<StoreObjectId>(objectIds);
			this.Exception = storageException;
			this.ResultObjectIds = null;
			this.ResultChangeKeys = null;
		}

		public IList<StoreObjectId> ObjectIds { get; private set; }

		public IList<StoreObjectId> ResultObjectIds { get; private set; }

		public IList<byte[]> ResultChangeKeys { get; private set; }

		public readonly LocalizedException Exception;

		public readonly OperationResult OperationResult;
	}
}
