using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public abstract class DetailsTemplateControl : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected static void ValidateAttributeName(string attribute, DetailsTemplateControl.AttributeControlTypes controlType, string controlName)
		{
			if (string.IsNullOrEmpty(attribute))
			{
				throw new ArgumentNullException(DirectoryStrings.AttributeNameNull);
			}
		}

		internal static void ValidateRange(int value, int minValue, int maxValue)
		{
			if (value < minValue || value > maxValue)
			{
				throw new ArgumentOutOfRangeException(DirectoryStrings.ValueNotInRange(minValue, maxValue));
			}
		}

		internal static void ValidateText(string text, DetailsTemplateControl.TextLengths maxLength)
		{
			if (text == null)
			{
				throw new ArgumentNullException(DirectoryStrings.ControlTextNull);
			}
			if (text.Length > (int)maxLength)
			{
				throw new ArgumentException(DirectoryStrings.InvalidControlTextLength((int)maxLength));
			}
		}

		internal static void SetBitField(bool setBit, DetailsTemplateControl.ControlFlags currentBit, ref DetailsTemplateControl.ControlFlags bitField)
		{
			if (setBit)
			{
				bitField |= currentBit;
				return;
			}
			bitField &= ~currentBit;
		}

		protected void NotifyPropertyChanged(string info)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		public int X
		{
			get
			{
				return this.x;
			}
			set
			{
				if (value != this.x)
				{
					DetailsTemplateControl.ValidateRange(value, 0, 32767);
					this.x = value;
					this.NotifyPropertyChanged("X");
				}
			}
		}

		public int Y
		{
			get
			{
				return this.y;
			}
			set
			{
				if (value != this.y)
				{
					DetailsTemplateControl.ValidateRange(value, 0, 32767);
					this.y = value;
					this.NotifyPropertyChanged("Y");
				}
			}
		}

		public int Width
		{
			get
			{
				return this.width;
			}
			set
			{
				if (value != this.Width)
				{
					DetailsTemplateControl.ValidateRange(value, 1, 32767);
					this.width = value;
					this.NotifyPropertyChanged("Width");
				}
			}
		}

		public int Height
		{
			get
			{
				return this.height;
			}
			set
			{
				if (value != this.Height)
				{
					DetailsTemplateControl.ValidateRange(value, 1, 32767);
					this.height = value;
					this.NotifyPropertyChanged("Height");
				}
			}
		}

		internal abstract DetailsTemplateControl.ControlTypes GetControlType();

		internal virtual DetailsTemplateControl.ControlFlags GetControlFlags()
		{
			return this.OriginalFlags;
		}

		internal virtual DetailsTemplateControl.AttributeControlTypes GetAttributeControlType()
		{
			return DetailsTemplateControl.AttributeControlTypes.None;
		}

		internal virtual DetailsTemplateControl.MapiPrefix GetMapiPrefix()
		{
			return DetailsTemplateControl.MapiPrefix.None;
		}

		internal DetailsTemplateControl()
		{
		}

		internal virtual bool ValidateAttribute(MAPIPropertiesDictionary propertiesDictionary)
		{
			return true;
		}

		internal bool ValidateAttributeHelper(MAPIPropertiesDictionary propertiesDictionary)
		{
			return !string.IsNullOrEmpty(this.m_AttributeName) && propertiesDictionary != null && propertiesDictionary[this.m_AttributeName] != null && (propertiesDictionary[this.m_AttributeName].ControlType & this.GetAttributeControlType()) != DetailsTemplateControl.AttributeControlTypes.None;
		}

		private const int MinCoordinate = 0;

		private const int MaxCoordinate = 32767;

		private const int MinWidthOrHeigth = 1;

		private const int MaxWidthOrHeigth = 32767;

		internal static string NoTextString = "*";

		internal static int EditMaxLength = 4096;

		internal static int EditDefaultLength = 1024;

		internal string m_AttributeName = string.Empty;

		internal string m_Text = string.Empty;

		internal int m_MaxLength;

		internal DetailsTemplateControl.ControlFlags OriginalFlags;

		private int x;

		private int y;

		private int width;

		private int height;

		internal enum TextLengths
		{
			Label = 127,
			Page = 31,
			Groupbox = 127,
			Checkbox = 127,
			Button = 127
		}

		[Flags]
		protected internal enum AttributeControlTypes
		{
			None = 0,
			Edit = 1,
			MultiValued = 2,
			Checkbox = 4,
			Listbox = 8
		}

		internal enum ControlTypes
		{
			Label,
			Edit,
			Listbox,
			Checkbox = 5,
			Groupbox,
			Button,
			Page,
			MultiValuedListbox = 11,
			MultiValuedDropdown
		}

		internal enum MapiPrefix : uint
		{
			None,
			Checkbox = 11U,
			Edit = 30U,
			Listbox = 13U,
			Button = 13U,
			MultiValued = 4126U
		}

		[Flags]
		internal enum ControlFlags : uint
		{
			Multiline = 1U,
			HorizontalScroll = 1U,
			ReadOnly = 2U,
			VerticalScroll = 2U,
			ConfirmationRequired = 4U,
			UseSystemPasswordChar = 16U,
			AcceptDBCS = 32U
		}
	}
}
