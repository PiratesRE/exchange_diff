using System;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class DetailsTemplateAdapter
	{
		private static uint GetIntFrom4bytes(byte[] ba, int offset)
		{
			uint num = 0U;
			uint num2 = 1U;
			if (ba == null || ba.Length < offset + 4)
			{
				throw new InvalidOperationException(DirectoryStrings.DetailsTemplateCorrupted);
			}
			for (int i = offset; i < offset + 4; i++)
			{
				num += (uint)ba[i] * num2;
				num2 *= 256U;
			}
			return num;
		}

		private static byte[] Get4byteFromInt(uint value)
		{
			byte[] array = new byte[4];
			for (int i = 0; i < 4; i++)
			{
				array[i] = Convert.ToByte(value % 256U);
				value /= 256U;
			}
			return array;
		}

		private static void SetBytesToByteArray(byte[] target, int offset, byte[] bytesToSet)
		{
			if (bytesToSet == null || bytesToSet.Length == 0)
			{
				return;
			}
			int num = bytesToSet.Length;
			if (offset + num > target.Length)
			{
				throw new InvalidOperationException(DirectoryStrings.DetailsTemplateCorrupted);
			}
			for (int i = 0; i < num; i++)
			{
				target[i + offset] = bytesToSet[i];
			}
		}

		private static void SetCaptionToByteArray(byte[] target, ref int captionOffset, string caption)
		{
			byte[] array = new byte[2 * caption.Length + 2];
			DetailsTemplateAdapter.SetBytesToByteArray(array, 0, Encoding.Unicode.GetBytes(caption));
			for (int i = 0; i < 2; i++)
			{
				array[array.Length - 1 - i] = 0;
			}
			DetailsTemplateAdapter.SetBytesToByteArray(target, captionOffset, array);
			captionOffset += array.Length;
		}

		private static int GetNullTerminatedStringLength(byte[] blob, int offset)
		{
			int num = 0;
			if (blob == null || offset < 0)
			{
				throw new InvalidOperationException(DirectoryStrings.DetailsTemplateCorrupted);
			}
			int num2 = blob.Length - 2;
			while (offset < num2 && (blob[offset] != 0 || blob[offset + 1] != 0))
			{
				offset += 2;
				num += 2;
			}
			return num;
		}

		private static uint GetMAPIPropTag(DetailsTemplateControl control, MAPIPropertiesDictionary propertiesDictionary)
		{
			if (control.GetControlType() == DetailsTemplateControl.ControlTypes.Button)
			{
				return ButtonControl.MapiInt;
			}
			string attributeName = control.m_AttributeName;
			if (attributeName == null)
			{
				return 0U;
			}
			AttributeInfo attributeInfo = propertiesDictionary[attributeName];
			if (attributeInfo == null)
			{
				return 0U;
			}
			uint mapiID = (uint)attributeInfo.MapiID;
			return (uint)(control.GetMapiPrefix() | (DetailsTemplateControl.MapiPrefix)(mapiID << 16));
		}

		private static void SetControlToByteArray(byte[] target, ref int controlOffset, ref int captionOffset, DetailsTemplateControl control, MAPIPropertiesDictionary propertiesDictionary)
		{
			uint[] array = new uint[]
			{
				(uint)control.X,
				(uint)control.Width,
				(uint)control.Y,
				(uint)control.Height,
				(uint)control.GetControlType(),
				(uint)control.GetControlFlags(),
				DetailsTemplateAdapter.GetMAPIPropTag(control, propertiesDictionary),
				(uint)control.m_MaxLength
			};
			for (int i = 0; i < array.Length; i++)
			{
				DetailsTemplateAdapter.SetBytesToByteArray(target, controlOffset, DetailsTemplateAdapter.Get4byteFromInt(array[i]));
				controlOffset += 4;
			}
			DetailsTemplateAdapter.SetBytesToByteArray(target, controlOffset, DetailsTemplateAdapter.Get4byteFromInt((uint)captionOffset));
			DetailsTemplateAdapter.SetCaptionToByteArray(target, ref captionOffset, control.m_Text);
			controlOffset += 4;
		}

		private static void AddControl(uint[] data, byte[] blob, MultiValuedProperty<Page> pages, MAPIPropertiesDictionary propertiesDictionary)
		{
			int x = (int)data[0];
			int y = (int)data[2];
			int width = (int)data[1];
			int height = (int)data[3];
			int num = (int)data[8];
			int maxLength = (int)data[7];
			uint num2 = data[6];
			DetailsTemplateControl.ControlFlags controlFlags = (DetailsTemplateControl.ControlFlags)data[5];
			string attributeName = propertiesDictionary[(int)(num2 >> 16 & 65535U)] ?? string.Empty;
			string @string = Encoding.Unicode.GetString(blob, num, DetailsTemplateAdapter.GetNullTerminatedStringLength(blob, num));
			DetailsTemplateControl detailsTemplateControl = null;
			switch (data[4])
			{
			case 0U:
				detailsTemplateControl = new LabelControl();
				break;
			case 1U:
				detailsTemplateControl = new EditControl(controlFlags);
				break;
			case 2U:
				detailsTemplateControl = new ListboxControl(controlFlags);
				break;
			case 5U:
				detailsTemplateControl = new CheckboxControl();
				break;
			case 6U:
				detailsTemplateControl = new GroupboxControl();
				break;
			case 7U:
				detailsTemplateControl = new ButtonControl();
				break;
			case 8U:
				pages.Add(new Page
				{
					Text = @string,
					HelpContext = (int)controlFlags
				});
				break;
			case 11U:
				detailsTemplateControl = new MultiValuedListboxControl();
				break;
			case 12U:
				detailsTemplateControl = new MultiValuedDropdownControl();
				break;
			}
			if (detailsTemplateControl != null)
			{
				if (pages.Count == 0)
				{
					throw new ParsingException(DirectoryStrings.NoPagesSpecified);
				}
				detailsTemplateControl.X = x;
				detailsTemplateControl.Y = y;
				detailsTemplateControl.Width = width;
				detailsTemplateControl.Height = height;
				detailsTemplateControl.m_MaxLength = maxLength;
				detailsTemplateControl.m_AttributeName = attributeName;
				detailsTemplateControl.m_Text = @string;
				detailsTemplateControl.OriginalFlags = controlFlags;
				pages[pages.Count - 1].Controls.Add(detailsTemplateControl);
			}
		}

		public static byte[] PageCollectionToBlob(MultiValuedProperty<Page> pageCollection, MAPIPropertiesDictionary propertiesDictionary)
		{
			byte[] result;
			try
			{
				int num = 8;
				int num2 = 0;
				if (pageCollection == null || pageCollection.Count == 0)
				{
					throw new InvalidOperationException(DirectoryStrings.DetailsTemplateCorrupted);
				}
				foreach (Page page in pageCollection)
				{
					num2++;
					num2 += page.Controls.Count;
					if (page.Text != null)
					{
						num += page.Text.Length * 2;
					}
					foreach (DetailsTemplateControl detailsTemplateControl in page.Controls)
					{
						num += detailsTemplateControl.m_Text.Length * 2;
					}
				}
				num += num2 * 38;
				byte[] array = new byte[num];
				DetailsTemplateAdapter.SetBytesToByteArray(array, 0, DetailsTemplateAdapter.Get4byteFromInt(1U));
				DetailsTemplateAdapter.SetBytesToByteArray(array, 4, DetailsTemplateAdapter.Get4byteFromInt((uint)num2));
				int num3 = 8;
				int value = num2 * 9 * 4 + 8;
				foreach (Page page2 in pageCollection)
				{
					DetailsTemplateAdapter.SetBytesToByteArray(array, num3 + 16, DetailsTemplateAdapter.Get4byteFromInt(8U));
					DetailsTemplateAdapter.SetBytesToByteArray(array, num3 + 20, DetailsTemplateAdapter.Get4byteFromInt((uint)page2.HelpContext));
					DetailsTemplateAdapter.SetBytesToByteArray(array, num3 + 32, DetailsTemplateAdapter.Get4byteFromInt((uint)value));
					DetailsTemplateAdapter.SetCaptionToByteArray(array, ref value, page2.Text);
					num3 += 36;
					foreach (DetailsTemplateControl control in page2.Controls)
					{
						DetailsTemplateAdapter.SetControlToByteArray(array, ref num3, ref value, control, propertiesDictionary);
					}
				}
				result = array;
			}
			catch (ArgumentException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Pages", ex.Message), DetailsTemplateSchema.Pages, pageCollection), ex);
			}
			catch (InvalidOperationException ex2)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Pages", ex2.Message), DetailsTemplateSchema.Pages, pageCollection), ex2);
			}
			catch (ParsingException ex3)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Pages", ex3.Message), DetailsTemplateSchema.Pages, pageCollection), ex3);
			}
			return result;
		}

		public static MultiValuedProperty<Page> BlobToPageCollection(byte[] blob, MAPIPropertiesDictionary propertiesDictionary)
		{
			MultiValuedProperty<Page> multiValuedProperty = new MultiValuedProperty<Page>();
			try
			{
				uint intFrom4bytes = DetailsTemplateAdapter.GetIntFrom4bytes(blob, 4);
				int num = 8;
				int num2 = 0;
				while ((long)num2 < (long)((ulong)intFrom4bytes))
				{
					uint[] array = new uint[9];
					for (int i = 0; i < 9; i++)
					{
						array[i] = DetailsTemplateAdapter.GetIntFrom4bytes(blob, num);
						num += 4;
					}
					DetailsTemplateAdapter.AddControl(array, blob, multiValuedProperty, propertiesDictionary);
					num2++;
				}
			}
			catch (ArgumentException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Pages", ex.Message), DetailsTemplateSchema.Pages, blob), ex);
			}
			catch (InvalidOperationException ex2)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Pages", ex2.Message), DetailsTemplateSchema.Pages, blob), ex2);
			}
			catch (ParsingException ex3)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Pages", ex3.Message), DetailsTemplateSchema.Pages, blob), ex3);
			}
			return multiValuedProperty;
		}

		private const uint DSA_TEMPLATE = 1U;

		private const int DEFAULT_BLOB_OFFSET = 8;

		private const int SIZE_OF_INT = 4;

		private const int NUM_INTS_IN_CONTROL = 9;

		private const int SIZE_OF_CHAR = 2;

		internal enum ControlAttributes
		{
			X,
			Width,
			Y,
			Height,
			Type,
			Flags,
			Mapi,
			Maxlength,
			Caption
		}
	}
}
