using System;
using System.Xml.Linq;

namespace Microsoft.Exchange.DxStore.Common
{
	public interface ILocalDataStore
	{
		int LastInstanceExecuted { get; set; }

		bool CreateKey(int? instanceNumber, string keyName);

		bool DeleteKey(int? instanceNumber, string keyName);

		void SetProperty(int? instanceNumber, string keyName, string propertyName, PropertyValue propertyValue);

		bool DeleteProperty(int? instanceNumber, string keyName, string propertyName);

		void ExecuteBatch(int? instanceNumber, string keyName, DxStoreBatchCommand[] commands);

		bool IsKeyExist(string keyName);

		DataStoreStats GetStoreStats();

		string[] EnumSubkeyNames(string keyName);

		bool IsPropertyExist(string keyName, string propertyName);

		PropertyValue GetProperty(string keyName, string propertyName);

		Tuple<string, PropertyValue>[] GetAllProperties(string keyName);

		PropertyNameInfo[] EnumPropertyNames(string keyName);

		InstanceSnapshotInfo GetSnapshot(string keyName = null, bool isCompress = true);

		XElement GetXElementSnapshot(string keyName, out int lastInstanceExecuted);

		void ApplySnapshot(InstanceSnapshotInfo snapshotInfo, int? instanceNumber = null);

		void ApplySnapshotFromXElement(string keyName, int lastInstanceExecuted, XElement rootElement);
	}
}
