using System;
using System.ComponentModel;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	[DDIPropertyExistInDataObject]
	public class ColumnProfile
	{
		public ColumnProfile()
		{
		}

		public ColumnProfile(string name, string dataObjectName, string mappingProperty)
		{
			this.name = name;
			this.dataObjectName = dataObjectName;
			this.mappingProperty = (string.IsNullOrEmpty(mappingProperty) ? name : mappingProperty);
		}

		[DDIMandatoryValue]
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		[DefaultValue(true)]
		public bool SupportBulkEdit
		{
			get
			{
				return this.supportBulkEdit;
			}
			set
			{
				this.supportBulkEdit = value;
			}
		}

		[DefaultValue(false)]
		public bool IgnoreChangeTracking { get; set; }

		[DefaultValue(null)]
		public ICustomTextConverter TextConverter
		{
			get
			{
				return this.converter;
			}
			set
			{
				this.converter = value;
			}
		}

		[DefaultValue(null)]
		public IPropertySetter PropertySetter
		{
			get
			{
				return this.propertySetter;
			}
			set
			{
				this.propertySetter = value;
			}
		}

		[DefaultValue(false)]
		public bool PersistWholeObject { get; set; }

		[TypeConverter(typeof(DDIObjectTypeConverter))]
		[DefaultValue(null)]
		public Type Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
				this.isTypeSpecified = true;
			}
		}

		[DDIValidLambdaExpression]
		[DefaultValue(null)]
		public string LambdaExpression { get; set; }

		[DDIValidLambdaExpression]
		[DefaultValue(null)]
		public string OnceLambdaExpression { get; set; }

		[DefaultValue(null)]
		public string DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
			set
			{
				this.defaultValue = value;
				this.isDefaultValueSpecified = true;
			}
		}

		public string MappingProperty
		{
			get
			{
				if (!string.IsNullOrEmpty(this.mappingProperty))
				{
					return this.mappingProperty;
				}
				return this.Name;
			}
			set
			{
				this.mappingProperty = value;
			}
		}

		[DefaultValue(null)]
		[DDIDataObjectNameExist]
		public string DataObjectName
		{
			get
			{
				return this.dataObjectName;
			}
			set
			{
				this.dataObjectName = value;
			}
		}

		public void Retrieve(ref Type type, ref object defaultValue)
		{
			if (this.isTypeSpecified)
			{
				type = this.type;
			}
			if (this.isDefaultValueSpecified)
			{
				if (this.TextConverter != null)
				{
					defaultValue = this.TextConverter.Parse(type, this.defaultValue, null);
					return;
				}
				if (type != null && type.IsEnum)
				{
					defaultValue = Enum.Parse(type, this.defaultValue);
					return;
				}
				defaultValue = this.defaultValue;
			}
		}

		private string name;

		private Type type;

		private string defaultValue;

		private string dataObjectName;

		private string mappingProperty;

		private bool isTypeSpecified;

		private bool isDefaultValueSpecified;

		private ICustomTextConverter converter;

		private IPropertySetter propertySetter;

		private bool supportBulkEdit = true;
	}
}
