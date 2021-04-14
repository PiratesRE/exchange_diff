using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class EditControl : DetailsTemplateControl
	{
		public string AttributeName
		{
			get
			{
				return this.m_AttributeName;
			}
			set
			{
				DetailsTemplateControl.ValidateAttributeName(value, this.GetAttributeControlType(), base.GetType().Name);
				this.m_AttributeName = value;
			}
		}

		public int MaxLength
		{
			get
			{
				return this.m_MaxLength;
			}
			set
			{
				DetailsTemplateControl.ValidateRange(value, 1, DetailsTemplateControl.EditMaxLength);
				this.m_MaxLength = value;
			}
		}

		public bool UseSystemPasswordChar
		{
			get
			{
				return this.useSystemPasswordChar;
			}
			set
			{
				this.useSystemPasswordChar = value;
			}
		}

		public bool ReadOnly
		{
			get
			{
				return this.readOnly;
			}
			set
			{
				this.readOnly = value;
			}
		}

		public bool Multiline
		{
			get
			{
				return this.multiline;
			}
			set
			{
				if (value != this.multiline)
				{
					this.multiline = value;
					base.NotifyPropertyChanged("Multiline");
				}
			}
		}

		public bool ConfirmationRequired
		{
			get
			{
				return this.confirmationRequired;
			}
			set
			{
				this.confirmationRequired = value;
			}
		}

		internal override DetailsTemplateControl.AttributeControlTypes GetAttributeControlType()
		{
			return DetailsTemplateControl.AttributeControlTypes.Edit;
		}

		internal override DetailsTemplateControl.ControlFlags GetControlFlags()
		{
			DetailsTemplateControl.ControlFlags originalFlags = this.OriginalFlags;
			DetailsTemplateControl.SetBitField(!this.ReadOnly, DetailsTemplateControl.ControlFlags.ReadOnly, ref originalFlags);
			DetailsTemplateControl.SetBitField(this.Multiline, DetailsTemplateControl.ControlFlags.Multiline, ref originalFlags);
			DetailsTemplateControl.SetBitField(this.UseSystemPasswordChar, DetailsTemplateControl.ControlFlags.UseSystemPasswordChar, ref originalFlags);
			DetailsTemplateControl.SetBitField(this.ConfirmationRequired, DetailsTemplateControl.ControlFlags.ConfirmationRequired, ref originalFlags);
			return originalFlags;
		}

		internal EditControl(DetailsTemplateControl.ControlFlags controlFlags)
		{
			this.UseSystemPasswordChar = ((controlFlags & DetailsTemplateControl.ControlFlags.UseSystemPasswordChar) != (DetailsTemplateControl.ControlFlags)0U);
			this.ReadOnly = ((controlFlags & DetailsTemplateControl.ControlFlags.ReadOnly) == (DetailsTemplateControl.ControlFlags)0U);
			this.Multiline = ((controlFlags & DetailsTemplateControl.ControlFlags.Multiline) != (DetailsTemplateControl.ControlFlags)0U);
			this.ConfirmationRequired = ((controlFlags & DetailsTemplateControl.ControlFlags.ConfirmationRequired) != (DetailsTemplateControl.ControlFlags)0U);
		}

		internal override bool ValidateAttribute(MAPIPropertiesDictionary propertiesDictionary)
		{
			return base.ValidateAttributeHelper(propertiesDictionary);
		}

		public EditControl()
		{
			this.OriginalFlags = DetailsTemplateControl.ControlFlags.AcceptDBCS;
			this.m_Text = DetailsTemplateControl.NoTextString;
			this.MaxLength = DetailsTemplateControl.EditDefaultLength;
		}

		internal override DetailsTemplateControl.ControlTypes GetControlType()
		{
			return DetailsTemplateControl.ControlTypes.Edit;
		}

		internal override DetailsTemplateControl.MapiPrefix GetMapiPrefix()
		{
			return DetailsTemplateControl.MapiPrefix.Edit;
		}

		public override string ToString()
		{
			return "Edit";
		}

		private bool useSystemPasswordChar;

		private bool readOnly = true;

		private bool multiline;

		private bool confirmationRequired;
	}
}
