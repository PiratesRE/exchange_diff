using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.SystemManager.WinForms;
using Microsoft.Exchange.ManagementGUI.Resources;

namespace Microsoft.Exchange.Management.SystemManager
{
	internal class FilterNode : Component, ICustomTypeDescriptor, IPropertyConstraintProvider
	{
		internal string ValueParsingError
		{
			get
			{
				return this.valueParsingError;
			}
			set
			{
				this.valueParsingError = value;
			}
		}

		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string DisplayName
		{
			get
			{
				if (this.FilterablePropertyDescription != null)
				{
					return this.FilterablePropertyDescription.DisplayName;
				}
				return string.Empty;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(null)]
		public ProviderPropertyDefinition PropertyDefinition
		{
			get
			{
				if (this.FilterablePropertyDescription != null)
				{
					return this.FilterablePropertyDescription.PropertyDefinition;
				}
				return null;
			}
		}

		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FilterablePropertyDescription FilterablePropertyDescription
		{
			get
			{
				return this.propDesc;
			}
			set
			{
				if (this.propDesc != value)
				{
					this.propDesc = value;
					this.OnFilterablePropertyDescriptionChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnFilterablePropertyDescriptionChanged(EventArgs e)
		{
			this.ResetValue();
			EventHandler eventHandler = (EventHandler)base.Events[FilterNode.EventFilterablePropertyDescriptionChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler FilterablePropertyDescriptionChanged
		{
			add
			{
				base.Events.AddHandler(FilterNode.EventFilterablePropertyDescriptionChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(FilterNode.EventFilterablePropertyDescriptionChanged, value);
			}
		}

		[DefaultValue(PropertyFilterOperator.Equal)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PropertyFilterOperator Operator
		{
			get
			{
				return this.op;
			}
			set
			{
				if (this.op != value)
				{
					this.op = value;
					this.OnOperatorChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnOperatorChanged(EventArgs e)
		{
			this.ResetValue();
			EventHandler eventHandler = (EventHandler)base.Events[FilterNode.EventOperatorChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler OperatorChanged
		{
			add
			{
				base.Events.AddHandler(FilterNode.EventOperatorChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(FilterNode.EventOperatorChanged, value);
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(null)]
		public object Value
		{
			get
			{
				return this.value;
			}
			set
			{
				if (!object.Equals(this.value, value))
				{
					this.value = value;
					this.OnValueChanged(EventArgs.Empty);
				}
			}
		}

		protected virtual void OnValueChanged(EventArgs e)
		{
			EventHandler eventHandler = (EventHandler)base.Events[FilterNode.EventValueChanged];
			if (eventHandler != null)
			{
				eventHandler(this, e);
			}
		}

		public event EventHandler ValueChanged
		{
			add
			{
				base.Events.AddHandler(FilterNode.EventValueChanged, value);
			}
			remove
			{
				base.Events.RemoveHandler(FilterNode.EventValueChanged, value);
			}
		}

		private void ResetValue()
		{
			if (this.PropertyDefinition != null && (typeof(DateTime).IsAssignableFrom(this.PropertyDefinition.Type) || typeof(DateTime?).IsAssignableFrom(this.PropertyDefinition.Type)))
			{
				this.Value = (DateTime)ExDateTime.Now;
				return;
			}
			this.Value = null;
		}

		public bool IsComplete
		{
			get
			{
				bool result;
				if (this.Operator == PropertyFilterOperator.Present || this.Operator == PropertyFilterOperator.NotPresent)
				{
					result = (null != this.PropertyDefinition);
				}
				else
				{
					string text = this.Value as string;
					if (text == null)
					{
						result = (this.PropertyDefinition != null && null != this.Value);
					}
					else
					{
						result = (this.PropertyDefinition != null && !string.IsNullOrEmpty(text));
					}
				}
				return result;
			}
		}

		public string Validate()
		{
			if (this.ValueParsingError != null)
			{
				return string.Format("{0}: '{1}'", this.DisplayName, this.ValueParsingError);
			}
			string text = null;
			if (this.op == PropertyFilterOperator.Equal || this.op == PropertyFilterOperator.NotEqual || this.op == PropertyFilterOperator.GreaterThan || this.op == PropertyFilterOperator.LessThan || this.op == PropertyFilterOperator.LessThanOrEqual || this.op == PropertyFilterOperator.GreaterThanOrEqual)
			{
				Type type = this.Value.GetType();
				if (type != this.PropertyDefinition.Type)
				{
					try
					{
						string valueToConvert;
						if (this.Value is DateTime)
						{
							valueToConvert = ((DateTime)this.Value).ToString("s");
						}
						else
						{
							valueToConvert = this.Value.ToString();
						}
						MonadFilter.ConvertValueFromString(valueToConvert, this.PropertyDefinition.Type);
					}
					catch (PSInvalidCastException ex)
					{
						Exception ex2 = ex;
						while (ex2.InnerException != null && !(ex2.InnerException is FormatException))
						{
							ex2 = ex2.InnerException;
						}
						text = string.Format("{0}: '{1}'", this.DisplayName, (ex2.InnerException != null) ? ex2.InnerException.Message : ex.Message);
						if (this.PropertyDefinition.Type == typeof(Version))
						{
							text = string.Format("{0}. {1}", text, Strings.ValidVersionExample);
						}
					}
				}
			}
			return text;
		}

		public QueryFilter QueryFilter
		{
			get
			{
				QueryFilter queryFilter = null;
				if (typeof(Enum).IsAssignableFrom(this.PropertyDefinition.Type))
				{
					queryFilter = this.GetQueryFilterForEnum();
				}
				else if (typeof(bool).IsAssignableFrom(this.PropertyDefinition.Type))
				{
					queryFilter = new ComparisonFilter((ComparisonOperator)this.Operator, this.PropertyDefinition, true);
					if (this.Value.Equals(false))
					{
						queryFilter = new NotFilter(queryFilter);
					}
				}
				else
				{
					switch (this.Operator)
					{
					case PropertyFilterOperator.Equal:
					case PropertyFilterOperator.NotEqual:
					case PropertyFilterOperator.LessThan:
					case PropertyFilterOperator.LessThanOrEqual:
					case PropertyFilterOperator.GreaterThan:
					case PropertyFilterOperator.GreaterThanOrEqual:
						if (this.Value is ADObjectId)
						{
							queryFilter = new ComparisonFilter((ComparisonOperator)this.op, this.PropertyDefinition, ((ADObjectId)this.Value).ToDNString());
						}
						else if (this.Value is DateTime)
						{
							queryFilter = new ComparisonFilter((ComparisonOperator)this.op, this.PropertyDefinition, ((DateTime)this.Value).ToString("s"));
						}
						else
						{
							queryFilter = new ComparisonFilter((ComparisonOperator)this.op, this.PropertyDefinition, this.Value);
						}
						break;
					case PropertyFilterOperator.StartsWith:
						queryFilter = new TextFilter(this.PropertyDefinition, this.Value.ToString(), MatchOptions.Prefix, MatchFlags.IgnoreCase);
						break;
					case PropertyFilterOperator.EndsWith:
						queryFilter = new TextFilter(this.PropertyDefinition, this.Value.ToString(), MatchOptions.Suffix, MatchFlags.IgnoreCase);
						break;
					case PropertyFilterOperator.Contains:
					case PropertyFilterOperator.NotContains:
						queryFilter = this.GetQueryFilterForLike(this.Operator);
						break;
					case PropertyFilterOperator.Present:
						queryFilter = new ExistsFilter(this.PropertyDefinition);
						break;
					case PropertyFilterOperator.NotPresent:
						queryFilter = new NotFilter(new ExistsFilter(this.PropertyDefinition));
						break;
					}
				}
				return queryFilter;
			}
		}

		private QueryFilter GetQueryFilterForEnum()
		{
			QueryFilter result = null;
			if (this.PropertyDefinition.Type.GetCustomAttributes(typeof(FlagsAttribute), false).Length == 0)
			{
				Type underlyingType = Enum.GetUnderlyingType(this.PropertyDefinition.Type);
				result = new ComparisonFilter((ComparisonOperator)this.op, this.PropertyDefinition, Convert.ChangeType(this.Value, underlyingType));
			}
			else
			{
				switch (this.Operator)
				{
				case PropertyFilterOperator.Equal:
					result = this.GetQueryFilterForLike(PropertyFilterOperator.Contains);
					break;
				case PropertyFilterOperator.NotEqual:
					result = this.GetQueryFilterForLike(PropertyFilterOperator.NotContains);
					break;
				}
			}
			return result;
		}

		private QueryFilter GetQueryFilterForLike(PropertyFilterOperator op)
		{
			QueryFilter result = null;
			switch (op)
			{
			case PropertyFilterOperator.Contains:
				result = new TextFilter(this.PropertyDefinition, this.Value.ToString(), MatchOptions.SubString, MatchFlags.IgnoreCase);
				break;
			case PropertyFilterOperator.NotContains:
				result = new NotFilter(new TextFilter(this.PropertyDefinition, this.Value.ToString(), MatchOptions.SubString, MatchFlags.IgnoreCase));
				break;
			}
			return result;
		}

		public static List<FilterNode> GetNodesFromSerializedQueryFilter(byte[] serializedQueryFilter, IList<FilterablePropertyDescription> filterableProperties, ObjectSchema schema)
		{
			List<FilterNode> list = new List<FilterNode>();
			if (serializedQueryFilter != null)
			{
				Hashtable hashtable = new Hashtable(filterableProperties.Count);
				foreach (FilterablePropertyDescription filterablePropertyDescription in filterableProperties)
				{
					hashtable.Add(filterablePropertyDescription.PropertyDefinition.Name, filterablePropertyDescription);
				}
				FilterNode.GetNodesFromExpressionTree((QueryFilter)WinformsHelper.DeSerialize(serializedQueryFilter), hashtable, list);
			}
			return list;
		}

		internal static List<FilterNode> GetNodesFromExpressionString(string expression, IList<FilterablePropertyDescription> filterableProperties, ObjectSchema schema)
		{
			return FilterNode.GetNodesFromSerializedQueryFilter(FilterNode.ConvertExpressionStringToByteArray(expression, schema), filterableProperties, schema);
		}

		internal static byte[] ConvertExpressionStringToByteArray(string expression, ObjectSchema schema)
		{
			QueryParser.ConvertValueFromStringDelegate convertDelegate = new QueryParser.ConvertValueFromStringDelegate(MonadFilter.ConvertValueFromString);
			QueryParser.EvaluateVariableDelegate evalDelegate = new QueryParser.EvaluateVariableDelegate(FilterNode.VarConverter);
			QueryParser queryParser = new QueryParser(expression, schema, QueryParser.Capabilities.All, evalDelegate, convertDelegate);
			return WinformsHelper.Serialize(queryParser.ParseTree);
		}

		private static object VarConverter(string varName)
		{
			return null;
		}

		private static void GetNodesFromExpressionTree(QueryFilter queryFilter, Hashtable allowedProperties, List<FilterNode> filterNodes)
		{
			if (queryFilter != null)
			{
				CompositeFilter compositeFilter = queryFilter as CompositeFilter;
				TextFilter textFilter = queryFilter as TextFilter;
				ComparisonFilter comparisonFilter = queryFilter as ComparisonFilter;
				ExistsFilter existsFilter = queryFilter as ExistsFilter;
				NotFilter notFilter = queryFilter as NotFilter;
				if (compositeFilter != null)
				{
					using (ReadOnlyCollection<QueryFilter>.Enumerator enumerator = compositeFilter.Filters.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							QueryFilter queryFilter2 = enumerator.Current;
							FilterNode.GetNodesFromExpressionTree(queryFilter2, allowedProperties, filterNodes);
						}
						return;
					}
				}
				if (notFilter != null)
				{
					textFilter = (notFilter.Filter as TextFilter);
					existsFilter = (notFilter.Filter as ExistsFilter);
					comparisonFilter = (notFilter.Filter as ComparisonFilter);
					if ((textFilter == null && existsFilter == null && comparisonFilter == null) || (comparisonFilter != null && !typeof(bool).IsAssignableFrom(comparisonFilter.Property.Type)))
					{
						throw new InvalidOperationException(Strings.InvalidNotFilter.ToString());
					}
				}
				if (textFilter != null)
				{
					FilterNode.GetNodeFromTextFilter(textFilter, allowedProperties, filterNodes, notFilter != null);
					return;
				}
				if (comparisonFilter != null)
				{
					FilterNode.GetNodeFromComparisonFilter(comparisonFilter, allowedProperties, filterNodes, notFilter != null);
					return;
				}
				if (existsFilter != null)
				{
					FilterNode.GetNodeFromExistsFilter(existsFilter, allowedProperties, filterNodes, null != notFilter);
					return;
				}
				throw new InvalidOperationException(Strings.UnsuportedFilterType(queryFilter.GetType()).ToString());
			}
		}

		private static void GetNodeFromExistsFilter(ExistsFilter existsFilter, Hashtable allowedProperties, List<FilterNode> filterNodes, bool isNotFilter)
		{
			if (allowedProperties.ContainsKey(existsFilter.Property.Name))
			{
				FilterNode filterNode = new FilterNode();
				filterNode.FilterablePropertyDescription = (FilterablePropertyDescription)allowedProperties[existsFilter.Property.Name];
				if (isNotFilter)
				{
					filterNode.Operator = PropertyFilterOperator.NotPresent;
				}
				else
				{
					filterNode.Operator = PropertyFilterOperator.Present;
				}
				filterNodes.Add(filterNode);
				return;
			}
			throw new InvalidOperationException(Strings.UnknownFilterableProperty(existsFilter.Property.Name).ToString());
		}

		private static void GetNodeFromComparisonFilter(ComparisonFilter comparisonFilter, Hashtable allowedProperties, List<FilterNode> filterNodes, bool isNotFilter)
		{
			if (allowedProperties.ContainsKey(comparisonFilter.Property.Name))
			{
				FilterNode filterNode = new FilterNode();
				filterNode.FilterablePropertyDescription = (FilterablePropertyDescription)allowedProperties[comparisonFilter.Property.Name];
				filterNode.Operator = (PropertyFilterOperator)comparisonFilter.ComparisonOperator;
				if (typeof(Enum).IsAssignableFrom(comparisonFilter.Property.Type))
				{
					filterNode.Value = Enum.Parse(comparisonFilter.Property.Type, comparisonFilter.PropertyValue.ToString(), true);
				}
				else if (typeof(bool).IsAssignableFrom(comparisonFilter.Property.Type))
				{
					filterNode.Value = !isNotFilter;
				}
				else if (comparisonFilter.Property.Type == typeof(MultiValuedProperty<string>))
				{
					filterNode.Value = comparisonFilter.PropertyValue.ToUserFriendText(CultureInfo.CurrentUICulture.TextInfo.ListSeparator, (object input) => false);
				}
				else
				{
					filterNode.Value = MonadFilter.ConvertValueFromString(comparisonFilter.PropertyValue, comparisonFilter.Property.Type);
				}
				filterNodes.Add(filterNode);
				return;
			}
			throw new InvalidOperationException(Strings.UnknownFilterableProperty(comparisonFilter.Property.Name).ToString());
		}

		private static void GetNodeFromTextFilter(TextFilter textFilter, Hashtable allowedProperties, List<FilterNode> filterNodes, bool isNotFilter)
		{
			if (allowedProperties.ContainsKey(textFilter.Property.Name))
			{
				FilterNode filterNode = new FilterNode();
				filterNode.FilterablePropertyDescription = (FilterablePropertyDescription)allowedProperties[textFilter.Property.Name];
				switch (textFilter.MatchOptions)
				{
				case MatchOptions.SubString:
					if (isNotFilter)
					{
						filterNode.Operator = PropertyFilterOperator.NotContains;
					}
					else
					{
						filterNode.Operator = PropertyFilterOperator.Contains;
					}
					break;
				case MatchOptions.Prefix:
					filterNode.Operator = PropertyFilterOperator.StartsWith;
					break;
				case MatchOptions.Suffix:
					filterNode.Operator = PropertyFilterOperator.EndsWith;
					break;
				default:
					throw new InvalidOperationException(Strings.UnsupportedTextFilter(textFilter.Property.Name, textFilter.MatchOptions.ToString(), textFilter.Text).ToString());
				}
				if (typeof(Enum).IsAssignableFrom(filterNode.PropertyDefinition.Type) && filterNode.PropertyDefinition.Type.GetCustomAttributes(typeof(FlagsAttribute), false).Length != 0)
				{
					if (isNotFilter)
					{
						filterNode.Operator = PropertyFilterOperator.NotEqual;
					}
					else
					{
						filterNode.Operator = PropertyFilterOperator.Equal;
					}
					filterNode.Value = Enum.Parse(filterNode.PropertyDefinition.Type, textFilter.Text, true);
				}
				else
				{
					filterNode.Value = textFilter.Text;
				}
				if (isNotFilter)
				{
					if (!typeof(Enum).IsAssignableFrom(filterNode.PropertyDefinition.Type) && filterNode.Operator != PropertyFilterOperator.NotContains)
					{
						throw new InvalidOperationException(Strings.InvalidTextFilterForNonEnums(filterNode.Operator.ToString()).ToString());
					}
					if (typeof(Enum).IsAssignableFrom(filterNode.PropertyDefinition.Type) && filterNode.Operator != PropertyFilterOperator.NotEqual)
					{
						throw new InvalidOperationException(Strings.InvalidTextFilterForEnums(filterNode.Operator.ToString()).ToString());
					}
				}
				filterNodes.Add(filterNode);
				return;
			}
			throw new InvalidOperationException(Strings.UnknownFilterableProperty(textFilter.Property.Name).ToString());
		}

		AttributeCollection ICustomTypeDescriptor.GetAttributes()
		{
			return TypeDescriptor.GetAttributes(this, true);
		}

		string ICustomTypeDescriptor.GetClassName()
		{
			return TypeDescriptor.GetClassName(this, true);
		}

		string ICustomTypeDescriptor.GetComponentName()
		{
			return TypeDescriptor.GetComponentName(this, true);
		}

		TypeConverter ICustomTypeDescriptor.GetConverter()
		{
			return TypeDescriptor.GetConverter(this, true);
		}

		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(this, true);
		}

		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(this, true);
		}

		object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(this, editorBaseType, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
		{
			return TypeDescriptor.GetEvents(this, true);
		}

		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(this, attributes, true);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
		{
			return ((ICustomTypeDescriptor)this).GetProperties(null);
		}

		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
		{
			PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
			foreach (object obj in TypeDescriptor.GetProperties(this, attributes, true))
			{
				PropertyDescriptor propertyDescriptor = (PropertyDescriptor)obj;
				if (propertyDescriptor.Name == "Value")
				{
					propertyDescriptorCollection.Add(new FilterValuePropertyDescriptor(this, propertyDescriptor));
				}
				else
				{
					propertyDescriptorCollection.Add(propertyDescriptor);
				}
			}
			return propertyDescriptorCollection;
		}

		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
		{
			return this;
		}

		public PropertyDefinitionConstraint[] GetPropertyDefinitionConstraints(string propertyName)
		{
			PropertyDefinitionConstraint[] array = new PropertyDefinitionConstraint[0];
			if (propertyName == "Value" && this.PropertyDefinition != null)
			{
				array = new PropertyDefinitionConstraint[this.PropertyDefinition.AllConstraints.Count];
				this.PropertyDefinition.AllConstraints.CopyTo(array, 0);
			}
			return array;
		}

		private FilterablePropertyDescription propDesc;

		private PropertyFilterOperator op;

		private object value;

		private string valueParsingError;

		private static readonly object EventFilterablePropertyDescriptionChanged = new object();

		private static readonly object EventOperatorChanged = new object();

		private static readonly object EventValueChanged = new object();
	}
}
