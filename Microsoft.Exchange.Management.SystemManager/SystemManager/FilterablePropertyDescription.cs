using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager
{
	internal class FilterablePropertyDescription : IComparable<FilterablePropertyDescription>
	{
		internal FilterablePropertyDescription(ProviderPropertyDefinition propdef, string displayName, PropertyFilterOperator[] operators)
		{
			this.propertyDefinition = propdef;
			this.displayName = displayName;
			this.supportedOperators = new EnumListSource<PropertyFilterOperator>(operators);
			this.SurfaceFilterablePropertyDescription = this;
			this.SurfaceFilterNodeSynchronizer = FilterablePropertyDescription.defaultFilterNodeSynchronizer;
			this.UnderlyingFilterablePropertyDescription = this;
			this.UnderlyingFilterNodeSynchronizer = FilterablePropertyDescription.defaultFilterNodeSynchronizer;
		}

		public FilterablePropertyDescription(ProviderPropertyDefinition propdef, string displayName, PropertyFilterOperator[] operators, string pickerProfile, string objectPickerValueMember) : this(propdef, displayName, operators)
		{
			this.PickerProfileName = pickerProfile;
			this.ObjectPickerValueMember = objectPickerValueMember;
		}

		public string PickerProfileName { get; internal set; }

		public ObjectPicker ObjectPicker
		{
			get
			{
				if (this.objectPicker == null && !string.IsNullOrEmpty(this.PickerProfileName))
				{
					this.objectPicker = new AutomatedObjectPicker(this.PickerProfileName);
				}
				return this.objectPicker;
			}
			internal set
			{
				this.objectPicker = value;
			}
		}

		public ProviderPropertyDefinition PropertyDefinition
		{
			get
			{
				return this.propertyDefinition;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
		}

		public EnumListSource<PropertyFilterOperator> SupportedOperators
		{
			get
			{
				return this.supportedOperators;
			}
		}

		public Type ColumnType
		{
			get
			{
				return this.columnType;
			}
			set
			{
				this.columnType = value;
			}
		}

		internal Type ValueType
		{
			get
			{
				return this.ColumnType ?? this.PropertyDefinition.Type;
			}
		}

		public ObjectListSource FilterableListSource
		{
			get
			{
				ObjectListSource objectListSource = this.filterableListSource;
				if (objectListSource == null)
				{
					Type valueType = this.ValueType;
					if (typeof(Enum).IsAssignableFrom(valueType))
					{
						objectListSource = new EnumListSource(valueType);
					}
					else if (typeof(bool).IsAssignableFrom(valueType))
					{
						objectListSource = new BoolListSource();
					}
				}
				return objectListSource;
			}
			set
			{
				this.filterableListSource = value;
			}
		}

		public DisplayFormatMode FormatMode
		{
			get
			{
				return this.formatMode;
			}
			set
			{
				this.formatMode = value;
			}
		}

		public string ObjectPickerValueMember
		{
			get
			{
				return this.objectPickerValueMember;
			}
			set
			{
				this.objectPickerValueMember = value;
			}
		}

		public ADPropertyDefinition ObjectPickerValueMemberPropertyDefinition
		{
			get
			{
				return this.objectPickerValueMemberPropertyDefinition;
			}
			set
			{
				this.objectPickerValueMemberPropertyDefinition = value;
			}
		}

		public string ObjectPickerDisplayMember
		{
			get
			{
				return this.objectPickerDisplayMember;
			}
			set
			{
				this.objectPickerDisplayMember = value;
			}
		}

		public FilterablePropertyDescription SurfaceFilterablePropertyDescription
		{
			get
			{
				return this.surfaceFilterablePropertyDescription;
			}
			set
			{
				if (this.SurfaceFilterablePropertyDescription != value)
				{
					this.surfaceFilterablePropertyDescription = (value ?? this);
					this.SurfaceFilterablePropertyDescription.UnderlyingFilterablePropertyDescription = this;
				}
			}
		}

		public FilterablePropertyDescription UnderlyingFilterablePropertyDescription
		{
			get
			{
				return this.underlyingFilterablePropertyDescription;
			}
			set
			{
				if (this.UnderlyingFilterablePropertyDescription != value)
				{
					this.underlyingFilterablePropertyDescription = (value ?? this);
					this.UnderlyingFilterablePropertyDescription.SurfaceFilterablePropertyDescription = this;
				}
			}
		}

		public FilterNodeSynchronizer SurfaceFilterNodeSynchronizer
		{
			get
			{
				return this.surfaceFilterNodeSynchronizer;
			}
			set
			{
				if (this.SurfaceFilterNodeSynchronizer != value)
				{
					this.surfaceFilterNodeSynchronizer = (value ?? FilterablePropertyDescription.defaultFilterNodeSynchronizer);
				}
			}
		}

		public FilterNodeSynchronizer UnderlyingFilterNodeSynchronizer
		{
			get
			{
				return this.underlyingFilterNodeSynchronizer;
			}
			set
			{
				if (this.UnderlyingFilterNodeSynchronizer != value)
				{
					this.underlyingFilterNodeSynchronizer = (value ?? FilterablePropertyDescription.defaultFilterNodeSynchronizer);
				}
			}
		}

		public void SetFilterablePropertyValueEditor(PropertyFilterOperator filterOperator, FilterablePropertyValueEditor editorType)
		{
			if (this.editorTypePerOperator.ContainsKey(filterOperator))
			{
				this.editorTypePerOperator[filterOperator] = editorType;
				return;
			}
			this.editorTypePerOperator.Add(filterOperator, editorType);
		}

		internal FilterablePropertyValueEditor GetPropertyFilterEditor(PropertyFilterOperator filterOperator)
		{
			FilterablePropertyValueEditor result = FilterablePropertyValueEditor.TextBox;
			if (this.editorTypePerOperator.ContainsKey(filterOperator))
			{
				result = this.editorTypePerOperator[filterOperator];
			}
			else
			{
				Type valueType = this.ValueType;
				if (filterOperator == PropertyFilterOperator.NotPresent || filterOperator == PropertyFilterOperator.Present)
				{
					result = FilterablePropertyValueEditor.DisabledTextBox;
				}
				else if (valueType == typeof(DateTime) || valueType == typeof(DateTime?))
				{
					result = FilterablePropertyValueEditor.DateTimePicker;
				}
				else if (filterOperator == PropertyFilterOperator.Equal || filterOperator == PropertyFilterOperator.NotEqual)
				{
					if (this.FilterableListSource != null)
					{
						result = FilterablePropertyValueEditor.ComboBox;
					}
					else if (!string.IsNullOrEmpty(this.PickerProfileName) || this.objectPicker != null)
					{
						result = FilterablePropertyValueEditor.PickerLauncherTextBox;
					}
				}
				else if ((filterOperator == PropertyFilterOperator.Contains || filterOperator == PropertyFilterOperator.NotContains) && this.PropertyDefinition.IsMultivalued && (!string.IsNullOrEmpty(this.PickerProfileName) || this.objectPicker != null))
				{
					result = FilterablePropertyValueEditor.PickerLauncherTextBox;
				}
			}
			return result;
		}

		public int CompareTo(FilterablePropertyDescription other)
		{
			return this.DisplayName.CompareTo(other.DisplayName);
		}

		private static FilterablePropertyDescription.DefaultFilterNodeSynchronizer defaultFilterNodeSynchronizer = new FilterablePropertyDescription.DefaultFilterNodeSynchronizer();

		private ObjectPicker objectPicker;

		private ProviderPropertyDefinition propertyDefinition;

		private string displayName;

		private EnumListSource<PropertyFilterOperator> supportedOperators;

		private Type columnType;

		private ObjectListSource filterableListSource;

		private DisplayFormatMode formatMode;

		private string objectPickerValueMember;

		private ADPropertyDefinition objectPickerValueMemberPropertyDefinition;

		private string objectPickerDisplayMember;

		private FilterablePropertyDescription surfaceFilterablePropertyDescription;

		public FilterablePropertyDescription underlyingFilterablePropertyDescription;

		private FilterNodeSynchronizer surfaceFilterNodeSynchronizer;

		private FilterNodeSynchronizer underlyingFilterNodeSynchronizer;

		private Dictionary<PropertyFilterOperator, FilterablePropertyValueEditor> editorTypePerOperator = new Dictionary<PropertyFilterOperator, FilterablePropertyValueEditor>();

		private class DefaultFilterNodeSynchronizer : FilterNodeSynchronizer
		{
			public override void Synchronize(FilterNode sourceNode, FilterNode targetNode)
			{
				if (sourceNode != targetNode)
				{
					targetNode.FilterablePropertyDescription = sourceNode.FilterablePropertyDescription;
					targetNode.Operator = sourceNode.Operator;
					targetNode.Value = sourceNode.Value;
				}
			}
		}
	}
}
