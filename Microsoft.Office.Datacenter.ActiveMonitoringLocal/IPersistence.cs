using System;
using System.Collections.Generic;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public interface IPersistence
	{
		LocalDataAccessMetaData LocalDataAccessMetaData { get; }

		void Initialize(Dictionary<string, string> propertyBag, LocalDataAccessMetaData metaData);

		void SetProperties(Dictionary<string, string> propertyBag);

		void Write(Action<IPersistence> preWriteHandler = null);
	}
}
