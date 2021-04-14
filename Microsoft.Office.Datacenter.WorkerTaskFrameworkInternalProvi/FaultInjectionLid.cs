using System;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public enum FaultInjectionLid
	{
		DataAccess_AsyncExecuteReader,
		DataAccess_AsyncExecuteScalar,
		DataAccess_AsyncExecuteNonQuery,
		DataAccess_AsyncInsert,
		DataAccess_AsyncGetExclusive,
		HttpWebRequestUtility_SendRequest = 100,
		HttpWebRequestUtility_GetHttpResponse
	}
}
