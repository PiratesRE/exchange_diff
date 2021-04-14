using System;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal interface IPostSavePropertyCommand
	{
		void ExecutePostSaveOperation(StoreObject item);
	}
}
