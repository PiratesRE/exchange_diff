using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	public interface IClusterDBWriteBatch : IDisposable
	{
		void CreateOrOpenKey(string keyName);

		void DeleteKey(string keyName);

		void SetValue(string valueName, string value);

		void SetValue(string valueName, IEnumerable<string> value);

		void SetValue(string valueName, int value);

		void SetValue(string valueName, long value);

		void DeleteValue(string valueName);

		void Execute();
	}
}
