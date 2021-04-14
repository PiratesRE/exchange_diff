using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public abstract class StrongTypeEditor<T> : BindableUserControl
	{
		public StrongTypeEditor()
		{
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public T StrongType
		{
			get
			{
				return this.strongType;
			}
			set
			{
				if (!object.Equals(this.strongType, value))
				{
					this.strongType = value;
					((StrongTypeEditorDataHandler<T>)base.Validator).StrongType = value;
					if (this.StrongTypeChanged != null)
					{
						this.StrongTypeChanged(this, EventArgs.Empty);
					}
				}
			}
		}

		public event EventHandler StrongTypeChanged;

		public void LoadValidator(string schema)
		{
			base.Validator = new StrongTypeEditorDataHandler<T>(this, schema);
		}

		protected override string ExposedPropertyName
		{
			get
			{
				return "StrongType";
			}
		}

		private T strongType;
	}
}
