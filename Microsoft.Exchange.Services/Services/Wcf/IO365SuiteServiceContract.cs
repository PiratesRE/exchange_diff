using System;

namespace Microsoft.Exchange.Services.Wcf
{
	public interface IO365SuiteServiceContract
	{
		[JsonRequestFormat(Format = JsonRequestFormat.Custom)]
		[OfflineClient(Queued = false)]
		SuiteStorageJsonResponse ProcessO365SuiteStorage(SuiteStorageJsonRequest request);
	}
}
