using System;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public interface IWorkData
	{
		string InternalStorageKey { get; }

		string ExternalStorageKey { get; }

		string SecondaryExternalStorageKey { get; }
	}
}
