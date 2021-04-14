using System;
using System.Runtime.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public class DataContractDeserializerOperation : DeserializerOperation
	{
		protected override object DeserializedValue(Type parameterType, Type[] additionalTypes)
		{
			DataContractSerializer dataContractSerializer = new DataContractSerializer(parameterType, additionalTypes);
			return dataContractSerializer.ReadObject(base.DataObject.CreateReader());
		}
	}
}
