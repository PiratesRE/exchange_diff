using System;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public class DalWorkDefinition
	{
		[XmlArrayItem("DeserializeDataContract", typeof(DataContractDeserializerOperation))]
		[XmlArrayItem("DalSave", typeof(SaveOperation))]
		[XmlArrayItem("DeserializeADObject", typeof(ADObjectDeserializerOperation))]
		[XmlArrayItem("New", typeof(ReflectionCreateOperation))]
		[XmlArrayItem("Assert", typeof(AssertOperation))]
		[XmlArrayItem("Deserialize", typeof(XmlDeserializerOperation))]
		[XmlArrayItem("Invoke", typeof(ReflectionInvokeOperation))]
		[XmlArrayItem("DalFind", typeof(FindOperation))]
		[XmlArrayItem("DalDelete", typeof(DeleteOperation))]
		[XmlArrayItem("DalTest", typeof(TestReadWriteOperation))]
		public DalProbeOperation[] Operations { get; set; }
	}
}
