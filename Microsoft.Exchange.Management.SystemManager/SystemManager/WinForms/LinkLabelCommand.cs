using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class LinkLabelCommand : ExchangeLinkLabel
	{
		public LinkLabelCommand()
		{
			base.Name = "LinkLabelCommand";
			this.UseMnemonic = false;
			this.bindingSource = new BindingSource();
			this.bindingSource.CurrentItemChanged += this.bindingSource_CurrentItemChanged;
		}

		[DefaultValue(false)]
		public bool UseMnemonic
		{
			get
			{
				return base.UseMnemonic;
			}
			set
			{
				base.UseMnemonic = value;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.bindingSource.Dispose();
			}
			base.Dispose(disposing);
		}

		[DefaultValue("")]
		public string Text
		{
			get
			{
				return this.markupText;
			}
			set
			{
				value = (value ?? "");
				if (value != this.Text)
				{
					this.SetTextFromMarkupText(value);
					this.markupText = value;
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
				value = (value ?? this.DefaultListSeparator);
				if (value != this.ListSeparator)
				{
					this.listSeparator = value;
					this.SetTextFromMarkupText(this.Text);
				}
			}
		}

		protected virtual string DefaultListSeparator
		{
			get
			{
				return CultureInfo.CurrentUICulture.TextInfo.ListSeparator;
			}
		}

		private bool ShouldSerializeListSeparator()
		{
			return this.ListSeparator != this.DefaultListSeparator;
		}

		private void ResetListSeparator()
		{
			this.ListSeparator = this.DefaultListSeparator;
		}

		[DefaultValue(null)]
		public object DataSource
		{
			get
			{
				return this.bindingSource.DataSource;
			}
			set
			{
				this.bindingSource.DataSource = value;
			}
		}

		public void SuspendUpdates()
		{
			this.suspendUpdates++;
		}

		public void ResumeUpdates()
		{
			if (this.suspendUpdates == 0)
			{
				throw new InvalidOperationException();
			}
			this.suspendUpdates--;
			if (this.suspendUpdates == 0)
			{
				this.bindingSource.ResetBindings(false);
			}
		}

		private void bindingSource_CurrentItemChanged(object sender, EventArgs e)
		{
			if (this.suspendUpdates == 0)
			{
				this.SetTextFromMarkupText(this.Text);
			}
		}

		private void SetTextFromMarkupText(string markupText)
		{
			MarkupParser markupParser = new MarkupParser();
			markupParser.Markup = markupText;
			markupParser.ReplaceAnchorValues(this.DataSource, this.ListSeparator);
			StringBuilder stringBuilder = new StringBuilder();
			base.Links.Clear();
			foreach (object obj in markupParser.Nodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (XmlNodeType.Element == xmlNode.NodeType && "a" == xmlNode.Name)
				{
					XmlAttribute xmlAttribute = xmlNode.Attributes["id"];
					if (xmlAttribute != null)
					{
						base.Links.Add(new StringInfo(stringBuilder.ToString()).LengthInTextElements, new StringInfo(xmlNode.InnerText).LengthInTextElements, xmlAttribute.Value);
					}
				}
				stringBuilder.Append(xmlNode.InnerText);
			}
			if (base.Text == stringBuilder.ToString() && !string.IsNullOrEmpty(base.Text))
			{
				this.OnTextChanged(EventArgs.Empty);
			}
			base.Text = stringBuilder.ToString();
		}

		protected override void OnClick(EventArgs e)
		{
			base.Select();
			base.OnClick(e);
		}

		public override int PreferredHeight
		{
			get
			{
				if (!string.IsNullOrEmpty(base.Text))
				{
					if (base.IsHandleCreated)
					{
						using (Graphics graphics = base.CreateGraphics())
						{
							graphics.PageUnit = GraphicsUnit.Pixel;
							using (StringFormat stringFormat = new StringFormat(StringFormat.GenericTypographic.FormatFlags | StringFormatFlags.FitBlackBox | StringFormatFlags.NoClip))
							{
								int width = base.Width - (base.Padding.Left + base.Padding.Right);
								int height = Size.Ceiling(graphics.MeasureString(base.Text, this.Font, width, stringFormat)).Height;
								return height + 3 + base.Padding.Top + base.Padding.Bottom;
							}
						}
					}
					return base.Height;
				}
				return 0;
			}
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			this.ApplyPreferredHeight();
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			this.ApplyPreferredHeight();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			this.ApplyPreferredHeight();
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			this.ApplyPreferredHeight();
		}

		private void ApplyPreferredHeight()
		{
			int preferredHeight = this.PreferredHeight;
			if (base.Height != preferredHeight)
			{
				base.Height = preferredHeight;
			}
		}

		private BindingSource bindingSource;

		private string markupText = "";

		private string listSeparator = CultureInfo.CurrentUICulture.TextInfo.ListSeparator;

		private int suspendUpdates;
	}
}
