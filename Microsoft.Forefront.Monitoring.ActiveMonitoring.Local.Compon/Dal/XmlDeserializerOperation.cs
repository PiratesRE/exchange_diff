using System;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public class XmlDeserializerOperation : DeserializerOperation
	{
		protected override object DeserializedValue(Type parameterType, Type[] additionalTypes)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(parameterType, additionalTypes);
			return xmlSerializer.Deserialize(base.DataObject.CreateReader());
		}
	}
}
