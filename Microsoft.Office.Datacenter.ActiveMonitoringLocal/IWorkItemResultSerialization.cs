using System;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public interface IWorkItemResultSerialization
	{
		string Serialize();

		void Deserialize(string result);
	}
}
