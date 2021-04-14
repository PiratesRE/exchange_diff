using System;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal interface IHashBucket
	{
		string StoreName { get; }

		string ConnectionString { get; }

		object GetPhysicalInstanceId(string hashKey);

		object GetPhysicalInstanceIdByHashValue(int hashValue);

		int GetLogicalHash(string hashKey);
	}
}
