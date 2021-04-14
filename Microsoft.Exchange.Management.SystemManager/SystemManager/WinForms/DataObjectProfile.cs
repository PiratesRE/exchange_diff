using System;
using System.ComponentModel;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SnapIn;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[Serializable]
	public class DataObjectProfile : ICloneable
	{
		public DataObjectProfile()
		{
		}

		private DataObjectProfile(string name, Type type, object dataObject, IDataObjectInfoRetriever retriever, IDataObjectValidator validator)
		{
			this.Name = name;
			this.Type = type;
			this.DataObject = dataObject;
			this.Retriever = retriever;
			this.Validator = validator;
		}

		public DataObjectProfile(string name, Type type, IDataObjectInfoRetriever retriever, IDataObjectValidator validator) : this(name, type, null, retriever, validator)
		{
		}

		public string Name { get; set; }

		[TypeConverter(typeof(DDIObjectTypeConverter))]
		public Type Type { get; set; }

		internal object DataObject { get; set; }

		[DefaultValue(null)]
		public IDataObjectCreator DataObjectCreator { get; set; }

		[DefaultValue(null)]
		public IDataObjectInfoRetriever Retriever { get; set; }

		[DefaultValue(null)]
		public IDataObjectValidator Validator { get; set; }

		internal bool HasReportCorrupted { get; set; }

		public object Clone()
		{
			DataObjectProfile dataObjectProfile = new DataObjectProfile(this.Name, this.Type, this.Retriever, this.Validator);
			if (PSConnectionInfoSingleton.GetInstance().Type != OrganizationType.Cloud)
			{
				dataObjectProfile.DataObject = ((this.DataObject is ICloneable) ? ((ICloneable)this.DataObject).Clone() : this.DataObject);
			}
			else if (this.DataObject != null)
			{
				ConfigurableObject configurableObject = this.DataObject as ConfigurableObject;
				if (configurableObject != null)
				{
					ConfigurableObject configurableObject2 = MockObjectInformation.CreateDummyObject(configurableObject.GetType()) as ConfigurableObject;
					configurableObject2.propertyBag = (configurableObject.propertyBag.Clone() as PropertyBag);
					dataObjectProfile.DataObject = configurableObject2;
				}
				else
				{
					dataObjectProfile.DataObject = this.DataObject;
				}
			}
			return dataObjectProfile;
		}

		public void Retrieve(string propertyName, out Type type)
		{
			type = null;
			if (this.Retriever != null)
			{
				this.Retriever.Retrieve(this.Type, propertyName, out type);
			}
		}

		public ValidationError[] Validate()
		{
			return this.Validator.Validate(this.DataObject);
		}
	}
}
