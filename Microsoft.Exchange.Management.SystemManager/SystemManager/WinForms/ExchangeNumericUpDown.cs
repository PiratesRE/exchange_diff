using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.Exchange.Data;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ExchangeNumericUpDown : NumericUpDown, IBulkEditor, IBulkEditSupport
	{
		public ExchangeNumericUpDown()
		{
			base.Name = "ExchangeNumericUpDown";
			base.DataBindings.CollectionChanged += this.DataBindings_CollectionChanged;
			base.GotFocus += delegate(object param0, EventArgs param1)
			{
				base.Select(0, this.Text.Length);
			};
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		[Browsable(false)]
		public event EventHandler<PropertyChangedEventArgs> UserModified;

		protected void OnUserModified(EventArgs e)
		{
			if (this.UserModified != null)
			{
				this.UserModified(this, new PropertyChangedEventArgs("Value"));
			}
		}

		BulkEditorAdapter IBulkEditor.BulkEditorAdapter
		{
			get
			{
				if (this.bulkEditorAdapter == null)
				{
					this.bulkEditorAdapter = new NumericUpDownBulkEditorAdapter(this);
				}
				return this.bulkEditorAdapter;
			}
		}

		private void DataBindings_CollectionChanged(object sender, CollectionChangeEventArgs e)
		{
			Binding binding = e.Element as Binding;
			if (e.Action == CollectionChangeAction.Add && (binding.PropertyName == "Text" || binding.PropertyName == "Value"))
			{
				object constraintProvider = (binding.DataSource is BindingSource) ? ((BindingSource)binding.DataSource).DataSource : binding.DataSource;
				PropertyDefinitionConstraint[] propertyDefinitionConstraints = PropertyConstraintProvider.GetPropertyDefinitionConstraints(constraintProvider, binding.BindingMemberInfo.BindingField);
				for (int i = 0; i < propertyDefinitionConstraints.Length; i++)
				{
					if (propertyDefinitionConstraints[i].GetType() == typeof(RangedValueConstraint<int>))
					{
						RangedValueConstraint<int> rangedValueConstraint = (RangedValueConstraint<int>)propertyDefinitionConstraints[i];
						base.Maximum = rangedValueConstraint.MaximumValue;
						base.Minimum = rangedValueConstraint.MinimumValue;
						return;
					}
				}
			}
		}

		private BulkEditorAdapter bulkEditorAdapter;
	}
}
