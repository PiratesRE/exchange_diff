using System;

namespace Microsoft.Exchange.Cluster.ClusApi
{
	internal interface IAmClusterResource : IDisposable
	{
		string Name { get; }

		AmClusterResourceHandle Handle { get; }

		void SetPrivateProperty<MyType>(string key, MyType value);

		void SetPrivatePropertyList(AmClusterPropList propList);

		MyType GetPrivateProperty<MyType>(string key);

		uint OnlineResource();

		uint OfflineResource();

		void DeleteResource();

		string GetTypeName();

		AmResourceState GetState();

		void SetAllPossibleOwnerNodes();
	}
}
