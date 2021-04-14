using System;
using System.ComponentModel;
using System.Web.UI.WebControls;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.DDIService
{
	[DDIAllVariableHasRoles]
	[DDIPropertyExistInDataObject]
	public class Variable : ICloneable
	{
		public Variable()
		{
		}

		public Variable(string name, string dataObjectName, string mappingProperty)
		{
			this.name = name;
			this.dataObjectName = dataObjectName;
			this.mappingProperty = (string.IsNullOrEmpty(mappingProperty) ? name : mappingProperty);
		}

		public object Clone()
		{
			return new Variable(this.Name, this.DataObjectName, this.MappingProperty)
			{
				PropertySetter = this.PropertySetter,
				PersistWholeObject = this.PersistWholeObject,
				IgnoreChangeTracking = this.IgnoreChangeTracking,
				UnicodeString = this.UnicodeString,
				Type = this.Type,
				Value = this.Value,
				OutputConverter = this.OutputConverter,
				InputConverter = this.InputConverter,
				SetRoles = this.SetRoles,
				NewRoles = this.NewRoles,
				RbacDataObjectName = this.RbacDataObjectName,
				RbacDependenciesForNew = this.RbacDependenciesForNew,
				RbacDependenciesForSet = this.RbacDependenciesForSet
			};
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

		[DefaultValue(false)]
		public bool IgnoreChangeTracking { get; set; }

		[DefaultValue(false)]
		public bool UnicodeString { get; set; }

		[DefaultValue(null)]
		[TypeConverter(typeof(DDIObjectTypeConverter))]
		public Type Type { get; set; }

		[DefaultValue(null)]
		[DDIValidLambdaExpression]
		public object Value { get; set; }

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

		[DDIDataObjectNameExist]
		public string RbacDataObjectName { get; set; }

		[DefaultValue(null)]
		public IInputConverter InputConverter { get; set; }

		[DefaultValue(null)]
		public IOutputConverter OutputConverter { get; set; }

		[DDIValidRole]
		public string SetRoles { get; set; }

		[DDIValidRole]
		public string NewRoles { get; set; }

		[TypeConverter(typeof(StringArrayConverter))]
		public string[] RbacDependenciesForNew { get; set; }

		[TypeConverter(typeof(StringArrayConverter))]
		public string[] RbacDependenciesForSet { get; set; }

		public Variable ShallowClone()
		{
			return base.MemberwiseClone() as Variable;
		}

		public const string ReadOnlyVariableName = "IsReadOnly";

		internal static readonly string[] MandatoryVariablesForGetObject = new string[]
		{
			"IsReadOnly"
		};

		private string name;

		private string dataObjectName;

		private string mappingProperty;

		private IPropertySetter propertySetter;
	}
}
