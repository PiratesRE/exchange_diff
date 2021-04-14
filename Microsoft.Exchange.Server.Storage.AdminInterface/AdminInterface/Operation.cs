using System;

namespace Microsoft.Exchange.Server.Storage.AdminInterface
{
	public enum Operation : uint
	{
		CreateJob = 1U,
		QueryJob,
		RemoveJob,
		PauseJob,
		ResumeJob,
		ExecuteJob,
		Count
	}
}
