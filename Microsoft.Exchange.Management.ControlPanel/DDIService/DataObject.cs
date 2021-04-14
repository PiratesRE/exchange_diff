using System;
using System.ComponentModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.DDIService
{
	public class DataObject : ICloneable
	{
		public DataObject()
		{
			this.Retriever = new ExchangeConfigObjectInfoRetriever();
			this.MinSupportedVersion = ExchangeObjectVersion.Exchange2003;
		}

		public DataObject(string name, Type type, IDataObjectInfoRetriever retriever) : this(name, type, null, retriever)
		{
		}

		private DataObject(string name, Type type, object dataObject, IDataObjectInfoRetriever retriever)
		{
			this.Name = name;
			this.Type = type;
			this.Value = dataObject;
			this.Retriever = retriever;
		}

		public object Clone()
		{
			return new DataObject(this.Name, this.Type, this.Retriever)
			{
				DataObjectCreator = this.DataObjectCreator,
				MinSupportedVersion = this.MinSupportedVersion
			};
		}

		[DDIMandatoryValue]
		public string Name { get; set; }

		[TypeConverter(typeof(DDIObjectTypeConverter))]
		[DDIMandatoryValue]
		public Type Type { get; set; }

		[DefaultValue(null)]
		public IDataObjectCreator DataObjectCreator { get; set; }

		[DefaultValue(typeof(ExchangeConfigObjectInfoRetriever))]
		public IDataObjectInfoRetriever Retriever { get; set; }

		[DefaultValue(typeof(ExchangeObjectVersion))]
		public ExchangeObjectVersion MinSupportedVersion { get; set; }

		internal object Value { get; set; }

		public void Retrieve(string propertyName, out Type type, out PropertyDefinition propertyDefinition)
		{
			type = null;
			propertyDefinition = null;
			if (this.Retriever != null)
			{
				this.Retriever.Retrieve(this.Type, propertyName, out type, out propertyDefinition);
			}
		}
	}
}
