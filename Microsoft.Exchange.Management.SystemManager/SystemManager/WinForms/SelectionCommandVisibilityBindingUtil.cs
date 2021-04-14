using System;
using System.Collections;
using System.ComponentModel;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class SelectionCommandVisibilityBindingUtil : SelectionCommandUpdatingUtil
	{
		public DataListViewResultPane DataListViewResultPane
		{
			get
			{
				return base.ResultPane as DataListViewResultPane;
			}
		}

		public string PropertyName { get; set; }

		public object TrueValue { get; set; }

		public bool AllowMixedValues { get; set; }

		protected override void UpdateCommand()
		{
			if (!string.IsNullOrEmpty(this.PropertyName))
			{
				SelectionCommandVisibilityBindingUtil.PropertyStatus propertyStatus = this.GetPropertyStatus(this.AllowMixedValues);
				base.Command.Visible = ((propertyStatus & SelectionCommandVisibilityBindingUtil.PropertyStatus.True) == SelectionCommandVisibilityBindingUtil.PropertyStatus.True);
			}
		}

		private SelectionCommandVisibilityBindingUtil.PropertyStatus GetPropertyStatus()
		{
			SelectionCommandVisibilityBindingUtil.PropertyStatus result = SelectionCommandVisibilityBindingUtil.PropertyStatus.None;
			IEnumerator enumerator = this.DataListViewResultPane.SelectedObjects.GetEnumerator();
			if (enumerator.MoveNext())
			{
				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(enumerator.Current)[this.PropertyName];
				if (propertyDescriptor != null)
				{
					object propertyValue = WinformsHelper.GetPropertyValue(enumerator.Current, propertyDescriptor);
					result = (object.Equals(propertyValue, this.TrueValue) ? SelectionCommandVisibilityBindingUtil.PropertyStatus.True : SelectionCommandVisibilityBindingUtil.PropertyStatus.False);
					while (enumerator.MoveNext())
					{
						object component = enumerator.Current;
						object propertyValue2 = WinformsHelper.GetPropertyValue(component, propertyDescriptor);
						if (!object.Equals(propertyValue, propertyValue2))
						{
							result = SelectionCommandVisibilityBindingUtil.PropertyStatus.None;
							break;
						}
					}
				}
			}
			return result;
		}

		private SelectionCommandVisibilityBindingUtil.PropertyStatus GetPropertyStatus(bool allowMixedValues)
		{
			if (!allowMixedValues)
			{
				return this.GetPropertyStatus();
			}
			SelectionCommandVisibilityBindingUtil.PropertyStatus propertyStatus = SelectionCommandVisibilityBindingUtil.PropertyStatus.None;
			foreach (object component in this.DataListViewResultPane.SelectedObjects)
			{
				PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(component)[this.PropertyName];
				if (propertyDescriptor != null)
				{
					object propertyValue = WinformsHelper.GetPropertyValue(component, propertyDescriptor);
					propertyStatus |= (object.Equals(propertyValue, this.TrueValue) ? SelectionCommandVisibilityBindingUtil.PropertyStatus.True : SelectionCommandVisibilityBindingUtil.PropertyStatus.False);
					if (propertyStatus == SelectionCommandVisibilityBindingUtil.PropertyStatus.TrueAndFalse)
					{
						break;
					}
				}
			}
			return propertyStatus;
		}

		[Flags]
		private enum PropertyStatus
		{
			None = 0,
			True = 1,
			False = 2,
			TrueAndFalse = 3
		}
	}
}
