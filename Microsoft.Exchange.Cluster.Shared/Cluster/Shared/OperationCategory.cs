using System;

namespace Microsoft.Exchange.Cluster.Shared
{
	public enum OperationCategory
	{
		Unknown,
		GetBaseKey,
		OpenKey,
		OpenOrCreateKey,
		DeleteKey,
		GetSubKeyNames,
		CloseKey,
		SetValue,
		GetValue,
		DeleteValue,
		GetValueNames,
		GetValueInfos,
		ExecuteBatch,
		CreateChangeNotify,
		GetAllValues
	}
}
