using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public abstract class DeserializerOperation : DalProbeOperation
	{
		[XmlAttribute]
		public string Type { get; set; }

		[XmlAttribute]
		public string AdditionalTypes { get; set; }

		[XmlAnyElement]
		public XElement DataObject { get; set; }

		public override void Execute(IDictionary<string, object> variables)
		{
			if (string.IsNullOrEmpty(base.Return))
			{
				throw new ArgumentNullException("Return");
			}
			Type parameterType = DalProbeOperation.ResolveDataType(this.Type);
			Type[] additionalTypes = null;
			if (!string.IsNullOrEmpty(this.AdditionalTypes))
			{
				string[] source = this.AdditionalTypes.Split(new char[0]);
				additionalTypes = source.Select(new Func<string, Type>(DalProbeOperation.ResolveDataType)).ToArray<Type>();
			}
			variables[base.Return] = this.DeserializedValue(parameterType, additionalTypes);
		}

		protected abstract object DeserializedValue(Type parameterType, Type[] additionalTypes);
	}
}
