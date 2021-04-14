using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class PhraseDescriptor : INotifyPropertyChanged
	{
		public PhraseDescriptor(int index, string markupText)
		{
			if (string.IsNullOrEmpty(markupText))
			{
				throw new ArgumentNullException("markupText");
			}
			this.index = index;
			this.markupText = markupText;
			this.EditingProperties = new MarkupParser
			{
				Markup = markupText
			}.GetEditingProperties();
		}

		public Dictionary<string, bool> EditingProperties { get; private set; }

		public int Index
		{
			get
			{
				return this.index;
			}
		}

		public string MarkupText
		{
			get
			{
				return this.markupText;
			}
		}

		public TypeMapping Type2UIEditor
		{
			get
			{
				return this.type2UIEditor;
			}
			set
			{
				this.type2UIEditor = value;
			}
		}

		[DefaultValue(null)]
		public object DataSource
		{
			get
			{
				return this.dataSource;
			}
			set
			{
				if (value != this.DataSource)
				{
					this.dataSource = value;
					this.used = this.HasInitializedValuesOfEditingProperties();
				}
			}
		}

		[DefaultValue(false)]
		public bool Used
		{
			get
			{
				return this.used;
			}
			set
			{
				if (this.used != value)
				{
					this.used = value;
					if (value)
					{
						this.InitializeValuesOfEditingProperties();
					}
					else
					{
						this.CleanValuesOfEditingProperties();
					}
					this.NotifyPropertyChanged("Used");
				}
			}
		}

		public string ListSeparator
		{
			get
			{
				return this.listSeparator;
			}
			set
			{
				this.listSeparator = value;
			}
		}

		[DefaultValue("")]
		public string PhrasePresentationPrefix { get; set; }

		[DefaultValue(0)]
		public int GroupID { get; set; }

		public bool IsValuesOfEditingPropertiesValid
		{
			get
			{
				ValidationError[] array = this.Validate();
				return array == null || array.Length == 0;
			}
		}

		public virtual UITypeEditor CreateEditor(PropertyDescriptor property)
		{
			UITypeEditor result = null;
			if (this.Type2UIEditor != null && property != null)
			{
				Type nullableTypeArgument = WinformsHelper.GetNullableTypeArgument(property.PropertyType);
				Type type = this.Type2UIEditor[nullableTypeArgument] as Type;
				if (typeof(UITypeEditor).IsAssignableFrom(type))
				{
					result = (Activator.CreateInstance(type) as UITypeEditor);
				}
			}
			return result;
		}

		public virtual ValidationError[] Validate()
		{
			List<ValidationError> list = new List<ValidationError>();
			if (this.Used)
			{
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this.DataSource);
				foreach (string text in this.EditingProperties.Keys)
				{
					PropertyDescriptor propertyDescriptor = properties[text];
					object propertyValue = (propertyDescriptor != null) ? propertyDescriptor.GetValue(this.DataSource) : null;
					if (WinformsHelper.IsEmptyValue(propertyValue))
					{
						list.Add(new StrongTypeValidationError(Strings.InvalidPhraseValues(text), text));
					}
				}
			}
			return list.ToArray();
		}

		protected virtual void CleanValuesOfEditingProperties()
		{
			if (this.DataSource != null)
			{
				foreach (string text in this.EditingProperties.Keys)
				{
					object value = null;
					if (!this.EditingProperties[text])
					{
						value = false;
					}
					this.SetDataSourceProperty(text, value);
				}
			}
		}

		protected virtual void InitializeValuesOfEditingProperties()
		{
			if (this.DataSource != null)
			{
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this.DataSource);
				foreach (string text in this.EditingProperties.Keys)
				{
					if (!this.EditingProperties[text])
					{
						this.SetDataSourceProperty(text, true);
					}
					else
					{
						PropertyDescriptor propertyDescriptor = properties[text];
						Type nullableTypeArgument = WinformsHelper.GetNullableTypeArgument(propertyDescriptor.PropertyType);
						if (null != nullableTypeArgument && nullableTypeArgument.IsValueType)
						{
							this.SetDataSourceProperty(text, Activator.CreateInstance(nullableTypeArgument));
						}
					}
				}
			}
		}

		protected virtual bool HasInitializedValuesOfEditingProperties()
		{
			bool result = false;
			if (this.DataSource != null)
			{
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this.DataSource);
				foreach (string text in this.EditingProperties.Keys)
				{
					PropertyDescriptor propertyDescriptor = properties[text];
					object value = propertyDescriptor.GetValue(this.DataSource);
					if ((!this.EditingProperties[text] && true.Equals(value)) || (this.EditingProperties[text] && !WinformsHelper.IsEmptyValue(value)))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		public void SetDataSourceProperty(string propertyName, object value)
		{
			if (this.DataSource != null)
			{
				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(this.DataSource)[propertyName];
				propertyDescriptor.SetValue(this.DataSource, value);
				this.OnPhraseEditingPropertyUpdated();
			}
		}

		public event PropertyChangedEventHandler PhraseEditingPropertyUpdated;

		private void OnPhraseEditingPropertyUpdated()
		{
			if (this.PhraseEditingPropertyUpdated != null)
			{
				foreach (string propertyName in this.EditingProperties.Keys)
				{
					this.PhraseEditingPropertyUpdated(this, new PropertyChangedEventArgs(propertyName));
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		private int index;

		private string markupText;

		private TypeMapping type2UIEditor;

		private object dataSource;

		private bool used;

		private string listSeparator = " " + Strings.ListSeparatorForCollection + " ";
	}
}
