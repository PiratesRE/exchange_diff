using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Microsoft.Exchange.ManagementGUI
{
	[DefaultProperty("Icons")]
	public class IconLibrary : Component
	{
		static IconLibrary()
		{
			TypeDescriptor.AddAttributes(typeof(Icon), new Attribute[]
			{
				new TypeConverterAttribute(typeof(IconLibrary.GlobalIconConverter)),
				new EditorAttribute(typeof(IconLibrary.GlobalIconEditor), typeof(UITypeEditor))
			});
		}

		public IconLibrary()
		{
		}

		public IconLibrary(IContainer container) : this()
		{
			container.Add(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Icons.Clear();
				if (this.smallImageList != null)
				{
					this.smallImageList.Dispose();
				}
				if (this.largeImageList != null)
				{
					this.largeImageList.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public IconLibrary.IconReferenceCollection Icons
		{
			get
			{
				if (this.icons == null)
				{
					this.icons = new IconLibrary.IconReferenceCollection(this);
				}
				return this.icons;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Browsable(false)]
		public ImageList SmallImageList
		{
			get
			{
				if (this.smallImageList == null)
				{
					this.smallImageList = this.CreateImageList(SystemInformation.SmallIconSize);
				}
				return this.smallImageList;
			}
		}

		private bool ShouldSerializeSmallImageList()
		{
			return false;
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Browsable(false)]
		public ImageList LargeImageList
		{
			get
			{
				if (this.largeImageList == null)
				{
					this.largeImageList = this.CreateImageList(SystemInformation.IconSize);
				}
				return this.largeImageList;
			}
		}

		private bool ShouldSerializeLargeImageList()
		{
			return false;
		}

		private ImageList CreateImageList(Size imageSize)
		{
			ImageList imageList = new ImageList();
			imageList.ColorDepth = ColorDepth.Depth32Bit;
			imageList.TransparentColor = Color.Transparent;
			imageList.ImageSize = imageSize;
			foreach (IconLibrary.IconReference iconReference in this.Icons)
			{
				imageList.Images.Add(iconReference.Name, iconReference.ToBitmap(imageSize));
			}
			return imageList;
		}

		public Icon GetIcon(string name, int index)
		{
			Icon result = null;
			if (this.icons != null)
			{
				IconLibrary.IconReference iconReference = null;
				if (string.IsNullOrEmpty(name))
				{
					if (index >= 0 && index < this.icons.Count)
					{
						iconReference = this.icons[index];
					}
				}
				else
				{
					iconReference = this.Icons[name];
				}
				if (iconReference != null)
				{
					result = iconReference.Icon;
				}
			}
			return result;
		}

		public static Bitmap ToSmallBitmap(Icon icon)
		{
			return IconLibrary.ToBitmap(icon, SystemInformation.SmallIconSize);
		}

		public static Bitmap ToBitmap(Icon icon, Size size)
		{
			Bitmap bitmap = null;
			if (icon != null)
			{
				using (Icon icon2 = new Icon(icon, size))
				{
					bitmap = icon2.ToBitmap();
					if (bitmap.Width > size.Width || bitmap.Height > size.Height)
					{
						Bitmap bitmap2 = new Bitmap(bitmap, size);
						bitmap.Dispose();
						bitmap = bitmap2;
					}
				}
			}
			return bitmap;
		}

		private IconLibrary.IconReferenceCollection icons;

		private ImageList smallImageList;

		private ImageList largeImageList;

		[Editor(typeof(IconLibrary.IconReferenceEditor), typeof(UITypeEditor))]
		[TypeConverter(typeof(IconLibrary.IconReferenceConverter))]
		[ImmutableObject(true)]
		public class IconReference
		{
			public IconReference(string name, Icon icon)
			{
				if (string.IsNullOrEmpty(name))
				{
					throw new ArgumentNullException("name");
				}
				if (icon == null)
				{
					throw new ArgumentNullException("icon");
				}
				this.name = (name ?? "");
				this.icon = icon;
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			[TypeConverter(typeof(IconLibrary.GlobalIconConverter))]
			[Editor(typeof(IconLibrary.GlobalIconEditor), typeof(UITypeEditor))]
			public Icon Icon
			{
				get
				{
					return this.icon;
				}
			}

			public Bitmap ToBitmap(Size size)
			{
				return IconLibrary.ToBitmap(this.Icon, size);
			}

			private string name;

			private Icon icon;
		}

		[Editor(typeof(IconLibrary.IconReferenceCollectionEditor), typeof(UITypeEditor))]
		public class IconReferenceCollection : Collection<IconLibrary.IconReference>
		{
			internal IconReferenceCollection(IconLibrary owner)
			{
				this.owner = owner;
			}

			public void AddRange(IconLibrary.IconReference[] iconReferences)
			{
				if (iconReferences == null)
				{
					throw new ArgumentNullException("iconReferences");
				}
				for (int i = 0; i < iconReferences.Length; i++)
				{
					base.Add(iconReferences[i]);
				}
			}

			public void Add(Enum name, Icon icon)
			{
				base.Add(new IconLibrary.IconReference(name.ToString(), icon));
			}

			public void Add(string name, Icon icon)
			{
				base.Add(new IconLibrary.IconReference(name, icon));
			}

			protected override void ClearItems()
			{
				IconLibrary.IconReferenceCollection.ClearImageList(this.owner.smallImageList);
				IconLibrary.IconReferenceCollection.ClearImageList(this.owner.largeImageList);
				base.ClearItems();
			}

			private static void ClearImageList(ImageList imageList)
			{
				if (imageList != null)
				{
					foreach (object obj in imageList.Images)
					{
						Image image = (Image)obj;
						image.Dispose();
					}
					imageList.Images.Clear();
				}
			}

			public IconLibrary.IconReference this[string name]
			{
				get
				{
					int num = this.IndexOf(name);
					if (-1 == num)
					{
						return null;
					}
					return base[num];
				}
			}

			public int IndexOf(string name)
			{
				if (!string.IsNullOrEmpty(name))
				{
					for (int i = 0; i < base.Count; i++)
					{
						if (StringComparer.InvariantCultureIgnoreCase.Compare(base[i].Name, name) == 0)
						{
							return i;
						}
					}
				}
				return -1;
			}

			public IconLibrary.IconReference this[Icon icon]
			{
				get
				{
					int num = this.IndexOf(icon);
					if (-1 == num)
					{
						return null;
					}
					return base[num];
				}
			}

			public int IndexOf(Icon icon)
			{
				if (icon != null)
				{
					for (int i = 0; i < base.Count; i++)
					{
						if (base[i].Icon == icon)
						{
							return i;
						}
					}
				}
				return -1;
			}

			protected override void InsertItem(int index, IconLibrary.IconReference item)
			{
				if (item == null)
				{
					throw new ArgumentNullException("item");
				}
				if (index != base.Count)
				{
					throw new NotSupportedException();
				}
				if (-1 != this.IndexOf(item.Name))
				{
					throw new ArgumentException("Duplicated icon name:" + item.Name);
				}
				IconLibrary.IconReferenceCollection.InsertImage(this.owner.smallImageList, index, item);
				IconLibrary.IconReferenceCollection.InsertImage(this.owner.largeImageList, index, item);
				base.InsertItem(index, item);
			}

			private static void InsertImage(ImageList imageList, int index, IconLibrary.IconReference item)
			{
				if (imageList != null)
				{
					imageList.Images.Add(item.Name, item.ToBitmap(imageList.ImageSize));
				}
			}

			public void Remove(string name)
			{
				int num = this.IndexOf(name);
				if (-1 != num)
				{
					base.RemoveAt(num);
				}
			}

			protected override void RemoveItem(int index)
			{
				IconLibrary.IconReferenceCollection.RemoveImage(this.owner.smallImageList, index);
				IconLibrary.IconReferenceCollection.RemoveImage(this.owner.largeImageList, index);
				base.RemoveItem(index);
			}

			private static void RemoveImage(ImageList imageList, int index)
			{
				if (imageList != null)
				{
					imageList.Images[index].Dispose();
					imageList.Images.RemoveAt(index);
				}
			}

			protected override void SetItem(int index, IconLibrary.IconReference item)
			{
				if (item == null)
				{
					throw new ArgumentNullException("item");
				}
				int num = this.IndexOf(item.Name);
				if (-1 != num && index != num)
				{
					throw new ArgumentException("Duplicated icon name:" + item.Name);
				}
				IconLibrary.IconReferenceCollection.SetImage(this.owner.smallImageList, index, item);
				IconLibrary.IconReferenceCollection.SetImage(this.owner.largeImageList, index, item);
				base.SetItem(index, item);
			}

			private static void SetImage(ImageList imageList, int index, IconLibrary.IconReference item)
			{
				if (imageList != null)
				{
					imageList.Images[index].Dispose();
					imageList.Images[index] = item.ToBitmap(imageList.ImageSize);
					imageList.Images.SetKeyName(index, item.Name);
				}
			}

			private IconLibrary owner;
		}

		public class GlobalIconEditor : IconEditor
		{
			public override void PaintValue(PaintValueEventArgs e)
			{
				Icon icon = e.Value as Icon;
				if (icon != null)
				{
					using (Icon icon2 = new Icon(icon, e.Bounds.Size))
					{
						PaintValueEventArgs e2 = new PaintValueEventArgs(e.Context, icon2, e.Graphics, e.Bounds);
						base.PaintValue(e2);
					}
				}
			}
		}

		public class GlobalIconConverter : IconConverter
		{
			private static bool GetServices(ITypeDescriptorContext context, out ITypeDiscoveryService typeDiscoveryService, out IReferenceService referenceService)
			{
				if (context == null)
				{
					typeDiscoveryService = null;
					referenceService = null;
				}
				else
				{
					typeDiscoveryService = (ITypeDiscoveryService)context.GetService(typeof(ITypeDiscoveryService));
					referenceService = (IReferenceService)context.GetService(typeof(IReferenceService));
				}
				return context != null && typeDiscoveryService != null && null != referenceService;
			}

			public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
			{
				return true;
			}

			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			{
				ITypeDiscoveryService typeDiscoveryService;
				IReferenceService referenceService;
				return IconLibrary.GlobalIconConverter.GetServices(context, out typeDiscoveryService, out referenceService);
			}

			public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
			{
				ITypeDiscoveryService typeDiscoveryService;
				IReferenceService referenceService;
				if (IconLibrary.GlobalIconConverter.GetServices(context, out typeDiscoveryService, out referenceService))
				{
					ArrayList arrayList = new ArrayList();
					foreach (object obj in typeDiscoveryService.GetTypes(typeof(object), true))
					{
						Type type = (Type)obj;
						if (type.IsClass && !type.IsAbstract && !type.IsGenericType && !type.IsGenericTypeDefinition && !type.IsImport && !type.IsNested && type.AssemblyQualifiedName != null && TypeDescriptor.GetAttributes(type)[typeof(GeneratedCodeAttribute)] != null)
						{
							PropertyInfo[] properties = type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
							foreach (PropertyInfo propertyInfo in properties)
							{
								if (propertyInfo.PropertyType == typeof(Icon))
								{
									Icon icon = referenceService.GetReference(type.Name + "." + propertyInfo.Name) as Icon;
									if (icon != null)
									{
										arrayList.Add(icon);
									}
								}
							}
						}
					}
					return new TypeConverter.StandardValuesCollection(arrayList);
				}
				return base.GetStandardValues(context);
			}

			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				if (value is Icon && destinationType == typeof(string) && context != null)
				{
					IReferenceService referenceService = (IReferenceService)context.GetService(typeof(IReferenceService));
					if (referenceService != null)
					{
						string name = referenceService.GetName(value);
						if (!string.IsNullOrEmpty(name))
						{
							return name;
						}
					}
				}
				return base.ConvertTo(context, culture, value, destinationType);
			}

			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				string text = value as string;
				if (!string.IsNullOrEmpty(text) && context != null)
				{
					IReferenceService referenceService = (IReferenceService)context.GetService(typeof(IReferenceService));
					if (referenceService != null)
					{
						Icon icon = referenceService.GetReference(text) as Icon;
						if (icon != null)
						{
							return icon;
						}
					}
				}
				return base.ConvertFrom(context, culture, value);
			}
		}

		public class IconReferenceEditor : UITypeEditor
		{
			[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
			public override bool GetPaintValueSupported(ITypeDescriptorContext context)
			{
				return true;
			}

			[PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust")]
			public override void PaintValue(PaintValueEventArgs e)
			{
				IconLibrary.IconReference iconReference = e.Value as IconLibrary.IconReference;
				if (iconReference != null)
				{
					this.globalIconEditor.PaintValue(new PaintValueEventArgs(e.Context, iconReference.Icon, e.Graphics, e.Bounds));
				}
			}

			private IconLibrary.GlobalIconEditor globalIconEditor = new IconLibrary.GlobalIconEditor();
		}

		public class IconReferenceConverter : TypeConverter
		{
			public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
			{
				return true;
			}

			public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
			{
				if (propertyValues == null)
				{
					throw new ArgumentNullException("propertyValues");
				}
				string name = propertyValues["Name"] as string;
				Icon icon = propertyValues["Icon"] as Icon;
				return new IconLibrary.IconReference(name, icon);
			}

			public override bool GetPropertiesSupported(ITypeDescriptorContext context)
			{
				return true;
			}

			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
			{
				PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(IconLibrary.IconReference), attributes);
				return properties.Sort(new string[]
				{
					"Name",
					"Icon"
				});
			}

			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				return destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
			}

			public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
			{
				IconLibrary.IconReference iconReference = value as IconLibrary.IconReference;
				if (destinationType == typeof(string))
				{
					if (iconReference == null)
					{
						return "(None)";
					}
					string text = (string)this.globalIconConverter.ConvertTo(context, culture, iconReference.Icon, typeof(string));
					if (iconReference.Name != text)
					{
						return iconReference.Name + ":" + text;
					}
					return text;
				}
				else
				{
					if (destinationType == typeof(InstanceDescriptor) && iconReference != null)
					{
						return new InstanceDescriptor(typeof(IconLibrary.IconReference).GetConstructor(new Type[]
						{
							typeof(string),
							typeof(Icon)
						}), new object[]
						{
							iconReference.Name,
							iconReference.Icon
						}, true);
					}
					return base.ConvertTo(context, culture, value, destinationType);
				}
			}

			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
			{
				string text = value as string;
				if (!string.IsNullOrEmpty(text))
				{
					int num = text.LastIndexOf(':');
					string text2 = text.Substring(num + 1);
					string name = (-1 == num) ? text2 : text.Substring(0, num);
					Icon icon = (Icon)this.globalIconConverter.ConvertFrom(context, culture, text2);
					if (icon != null)
					{
						return new IconLibrary.IconReference(name, icon);
					}
				}
				return base.ConvertFrom(context, culture, value);
			}

			public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
			{
				return false;
			}

			public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
			{
				return this.globalIconConverter.GetStandardValuesSupported(context);
			}

			public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
			{
				TypeConverter.StandardValuesCollection standardValues = this.globalIconConverter.GetStandardValues(context);
				IconLibrary.IconReference[] array = new IconLibrary.IconReference[standardValues.Count];
				for (int i = 0; i < standardValues.Count; i++)
				{
					string text = this.globalIconConverter.ConvertToInvariantString(context, standardValues[i]);
					string name = text.Substring(1 + text.IndexOf('.'));
					array[i] = new IconLibrary.IconReference(name, (Icon)standardValues[i]);
				}
				return new TypeConverter.StandardValuesCollection(array);
			}

			private IconLibrary.GlobalIconConverter globalIconConverter = new IconLibrary.GlobalIconConverter();
		}

		public class IconReferenceCollectionEditor : CollectionEditor
		{
			public IconReferenceCollectionEditor() : base(typeof(IconLibrary.IconReference))
			{
			}

			protected override CollectionEditor.CollectionForm CreateCollectionForm()
			{
				return new IconLibrary.IconReferenceCollectionEditor.IconCollectionEditorUI(this);
			}

			protected class IconCollectionEditorUI : CollectionEditor.CollectionForm
			{
				public IconCollectionEditorUI() : this(new IconLibrary.IconReferenceCollectionEditor())
				{
				}

				public IconCollectionEditorUI(IconLibrary.IconReferenceCollectionEditor editor) : base(editor)
				{
					this.InitializeComponent();
				}

				protected override void Dispose(bool disposing)
				{
					if (disposing)
					{
						this.components.Dispose();
					}
					base.Dispose(disposing);
				}

				private void InitializeComponent()
				{
					this.components = new Container();
					this.previewPanel = new FlowLayoutPanel();
					this.iconList = new ListView();
					this.iconLibrary = new IconLibrary(this.components);
					ToolStripSeparator toolStripSeparator = new ToolStripSeparator();
					this.duplicateIconButton = new ToolStripButton();
					this.renameIconButton = new ToolStripButton();
					Button button = new Button();
					Button button2 = new Button();
					PictureBox pictureBox = new PictureBox();
					PictureBox pictureBox2 = new PictureBox();
					ToolStrip toolStrip = new ToolStrip();
					ToolStripLabel toolStripLabel = new ToolStripLabel();
					ColumnHeader columnHeader = new ColumnHeader();
					Label label = new Label();
					TableLayoutPanel tableLayoutPanel = new TableLayoutPanel();
					((ISupportInitialize)pictureBox).BeginInit();
					((ISupportInitialize)pictureBox2).BeginInit();
					toolStrip.SuspendLayout();
					tableLayoutPanel.SuspendLayout();
					this.previewPanel.SuspendLayout();
					base.SuspendLayout();
					button.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
					button.DialogResult = DialogResult.Cancel;
					button.Location = new Point(202, 7);
					button.Name = "cancelButton";
					button.Size = new Size(75, 23);
					button.TabIndex = 2;
					button.Text = "Cancel";
					button.UseVisualStyleBackColor = true;
					button2.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
					button2.DialogResult = DialogResult.OK;
					button2.Location = new Point(121, 7);
					button2.Name = "okButton";
					button2.Size = new Size(75, 23);
					button2.TabIndex = 1;
					button2.Text = "OK";
					button2.UseVisualStyleBackColor = true;
					pictureBox.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
					pictureBox.Location = new Point(57, 11);
					pictureBox.Name = "preview16";
					pictureBox.Size = new Size(16, 16);
					pictureBox.TabIndex = 0;
					pictureBox.TabStop = false;
					pictureBox2.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
					pictureBox2.Location = new Point(79, 3);
					pictureBox2.Name = "preview32";
					pictureBox2.Size = new Size(32, 32);
					pictureBox2.TabIndex = 1;
					pictureBox2.TabStop = false;
					toolStrip.GripStyle = ToolStripGripStyle.Hidden;
					toolStrip.Items.AddRange(new ToolStripItem[]
					{
						toolStripLabel,
						toolStripSeparator,
						this.duplicateIconButton,
						this.renameIconButton
					});
					toolStrip.Location = new Point(0, 0);
					toolStrip.Name = "toolStrip";
					toolStrip.Size = new Size(280, 25);
					toolStrip.TabIndex = 0;
					toolStrip.Text = "toolStrip1";
					toolStripLabel.Name = "toolStripLabel1";
					toolStripLabel.Size = new Size(175, 22);
					toolStripLabel.Text = "Check icons to include in the library";
					columnHeader.Text = "Name";
					columnHeader.Width = 254;
					label.Anchor = (AnchorStyles.Left | AnchorStyles.Right);
					label.AutoSize = true;
					label.Location = new Point(3, 12);
					label.Name = "label1";
					label.Size = new Size(48, 13);
					label.TabIndex = 0;
					label.Text = "Preview:";
					tableLayoutPanel.AutoSize = true;
					tableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
					tableLayoutPanel.ColumnCount = 3;
					tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
					tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
					tableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
					tableLayoutPanel.Controls.Add(button2, 1, 0);
					tableLayoutPanel.Controls.Add(button, 2, 0);
					tableLayoutPanel.Controls.Add(this.previewPanel, 0, 0);
					tableLayoutPanel.Dock = DockStyle.Bottom;
					tableLayoutPanel.Location = new Point(0, 354);
					tableLayoutPanel.Name = "tableLayoutPanel";
					tableLayoutPanel.RowCount = 1;
					tableLayoutPanel.RowStyles.Add(new RowStyle());
					tableLayoutPanel.Size = new Size(280, 38);
					tableLayoutPanel.TabIndex = 2;
					this.previewPanel.AutoSize = true;
					this.previewPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
					this.previewPanel.BackColor = Color.Transparent;
					this.previewPanel.Controls.Add(label);
					this.previewPanel.Controls.Add(pictureBox);
					this.previewPanel.Controls.Add(pictureBox2);
					this.previewPanel.Location = new Point(0, 0);
					this.previewPanel.Margin = new Padding(0);
					this.previewPanel.Name = "previewPanel";
					this.previewPanel.Size = new Size(114, 38);
					this.previewPanel.TabIndex = 0;
					this.previewPanel.WrapContents = false;
					this.iconList.CheckBoxes = true;
					this.iconList.Columns.AddRange(new ColumnHeader[]
					{
						columnHeader
					});
					this.iconList.Dock = DockStyle.Fill;
					this.iconList.HeaderStyle = ColumnHeaderStyle.Nonclickable;
					this.iconList.HideSelection = false;
					this.iconList.LabelEdit = true;
					this.iconList.LargeImageList = this.iconLibrary.LargeImageList;
					this.iconList.Location = new Point(0, 25);
					this.iconList.Name = "iconList";
					this.iconList.Size = new Size(280, 329);
					this.iconList.SmallImageList = this.iconLibrary.SmallImageList;
					this.iconList.Sorting = SortOrder.Ascending;
					this.iconList.TabIndex = 1;
					this.iconList.UseCompatibleStateImageBehavior = false;
					this.iconList.View = View.Details;
					this.iconList.SelectedIndexChanged += this.iconList_SelectedIndexChanged;
					this.iconList.AfterLabelEdit += this.iconList_AfterLabelEdit;
					toolStripSeparator.Name = "toolStripSeparator";
					toolStripSeparator.Size = new Size(6, 25);
					this.duplicateIconButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
					this.duplicateIconButton.Name = "duplicateIconButton";
					this.duplicateIconButton.Size = new Size(79, 22);
					this.duplicateIconButton.Text = "Duplicate";
					this.duplicateIconButton.Click += this.duplicateIconButton_Click;
					this.renameIconButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
					this.renameIconButton.Name = "renameIconButton";
					this.renameIconButton.Size = new Size(79, 22);
					this.renameIconButton.Text = "Rename";
					this.renameIconButton.Click += this.renameIconButton_Click;
					base.AcceptButton = button2;
					base.AutoScaleDimensions = new SizeF(6f, 13f);
					base.AutoScaleMode = AutoScaleMode.Font;
					base.CancelButton = button;
					base.ClientSize = new Size(300, 408);
					base.Controls.Add(this.iconList);
					base.Controls.Add(tableLayoutPanel);
					base.Controls.Add(toolStrip);
					base.MaximizeBox = false;
					base.MinimizeBox = false;
					this.MinimumSize = new Size(306, 204);
					base.Name = "IconCollectionEditorUI";
					base.Padding = new Padding(0, 0, 0, 16);
					base.ShowIcon = false;
					base.ShowInTaskbar = false;
					base.SizeGripStyle = SizeGripStyle.Show;
					base.StartPosition = FormStartPosition.CenterParent;
					this.Text = "IconLibrary Icons Editor";
					((ISupportInitialize)pictureBox).EndInit();
					((ISupportInitialize)pictureBox2).EndInit();
					toolStrip.ResumeLayout(false);
					toolStrip.PerformLayout();
					tableLayoutPanel.ResumeLayout(false);
					tableLayoutPanel.PerformLayout();
					this.previewPanel.ResumeLayout(false);
					this.previewPanel.PerformLayout();
					base.ResumeLayout(false);
					base.PerformLayout();
				}

				protected override void OnEditValueChanged()
				{
					this.iconList.BeginUpdate();
					try
					{
						this.collectionToEdit = (IconLibrary.IconReferenceCollection)base.EditValue;
						if (this.collectionToEdit != null)
						{
							foreach (IconLibrary.IconReference iconReference in this.collectionToEdit)
							{
								ListViewItem listViewItem = this.AddIconReference(iconReference);
								listViewItem.Checked = true;
							}
							this.PopulateIconsFromProjectResources();
							if (this.iconList.Items.Count > 0)
							{
								this.iconList.Items[0].Focused = true;
								this.iconList.Items[0].Selected = true;
							}
							this.iconList.Select();
						}
						else
						{
							this.iconLibrary.icons.Clear();
							this.iconList.Items.Clear();
						}
					}
					finally
					{
						this.iconList.EndUpdate();
					}
				}

				private void PopulateIconsFromProjectResources()
				{
					IconLibrary.IconReferenceConverter iconReferenceConverter = new IconLibrary.IconReferenceConverter();
					foreach (object obj in iconReferenceConverter.GetStandardValues(base.Context))
					{
						IconLibrary.IconReference iconReference = (IconLibrary.IconReference)obj;
						if (-1 == this.collectionToEdit.IndexOf(iconReference.Icon))
						{
							this.AddIconWithUniqueName(iconReference.Name, iconReference.Icon);
						}
					}
				}

				protected override void OnFormClosing(FormClosingEventArgs e)
				{
					if (base.DialogResult == DialogResult.OK)
					{
						bool flag = false;
						StringCollection stringCollection = new StringCollection();
						foreach (object obj in this.iconList.CheckedItems)
						{
							ListViewItem listViewItem = (ListViewItem)obj;
							string value = listViewItem.Text.ToLowerInvariant();
							if (stringCollection.Contains(value))
							{
								flag = true;
								this.DisplayError(new Exception("There are duplicated names included in the library.\r\nEither rename one of the duplicated references or exclude it from the library by unchecking the item."));
								e.Cancel = true;
								listViewItem.BeginEdit();
								break;
							}
							stringCollection.Add(value);
						}
						if (!flag)
						{
							ArrayList arrayList = new ArrayList();
							foreach (object obj2 in this.iconList.CheckedItems)
							{
								ListViewItem listViewItem2 = (ListViewItem)obj2;
								arrayList.Add(IconLibrary.IconReferenceCollectionEditor.IconCollectionEditorUI.GetIconReference(listViewItem2));
							}
							base.Items = arrayList.ToArray();
						}
					}
					base.OnFormClosing(e);
				}

				private ListViewItem AddIconWithUniqueName(string baseName, Icon icon)
				{
					int num = 1;
					string text = baseName;
					for (;;)
					{
						ListViewItem listViewItem = this.iconList.Items[text];
						if (listViewItem == null)
						{
							break;
						}
						num++;
						text = baseName + num.ToString();
					}
					return this.AddIconReference(new IconLibrary.IconReference(text, icon));
				}

				private ListViewItem AddIconReference(IconLibrary.IconReference iconReference)
				{
					this.iconLibrary.Icons.Add(this.iconLibrary.Icons.Count.ToString(), iconReference.Icon);
					ListViewItem listViewItem = this.iconList.Items.Add(iconReference.Name, iconReference.Name, this.iconLibrary.Icons.Count - 1);
					listViewItem.Tag = iconReference;
					return listViewItem;
				}

				private static IconLibrary.IconReference GetIconReference(ListViewItem listViewItem)
				{
					return (IconLibrary.IconReference)listViewItem.Tag;
				}

				private void iconList_AfterLabelEdit(object sender, LabelEditEventArgs e)
				{
					if (string.IsNullOrEmpty(e.Label))
					{
						e.CancelEdit = true;
						return;
					}
					ListViewItem listViewItem = this.iconList.Items[e.Item];
					IconLibrary.IconReference iconReference = IconLibrary.IconReferenceCollectionEditor.IconCollectionEditorUI.GetIconReference(listViewItem);
					listViewItem.Name = e.Label;
					listViewItem.Tag = new IconLibrary.IconReference(e.Label, iconReference.Icon);
				}

				private void iconList_SelectedIndexChanged(object sender, EventArgs e)
				{
					bool enabled = this.iconList.SelectedItems.Count == 1;
					this.duplicateIconButton.Enabled = enabled;
					this.renameIconButton.Enabled = enabled;
					IconLibrary.IconReference iconReference = null;
					if (this.iconList.SelectedItems.Count != 0)
					{
						iconReference = IconLibrary.IconReferenceCollectionEditor.IconCollectionEditorUI.GetIconReference(this.iconList.SelectedItems[0]);
					}
					foreach (object obj in this.previewPanel.Controls)
					{
						Control control = (Control)obj;
						PictureBox pictureBox = control as PictureBox;
						if (pictureBox != null)
						{
							if (pictureBox.Image != null)
							{
								pictureBox.Image.Dispose();
								pictureBox.Image = null;
							}
							if (iconReference != null)
							{
								pictureBox.Image = iconReference.ToBitmap(pictureBox.Size);
							}
						}
					}
				}

				[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
				protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
				{
					bool flag = false;
					if (keyData == Keys.F2)
					{
						if (this.iconList.FocusedItem != null)
						{
							this.iconList.FocusedItem.BeginEdit();
							flag = true;
						}
					}
					else
					{
						flag = false;
					}
					if (!flag)
					{
						flag = base.ProcessCmdKey(ref msg, keyData);
					}
					return flag;
				}

				private void duplicateIconButton_Click(object sender, EventArgs e)
				{
					ListViewItem listViewItem = this.iconList.SelectedItems[0];
					listViewItem.Selected = false;
					IconLibrary.IconReference iconReference = IconLibrary.IconReferenceCollectionEditor.IconCollectionEditorUI.GetIconReference(listViewItem);
					ListViewItem listViewItem2 = this.AddIconWithUniqueName(iconReference.Name.TrimEnd(new char[]
					{
						'0',
						'1',
						'2',
						'3',
						'4',
						'5',
						'6',
						'7',
						'8',
						'9'
					}), iconReference.Icon);
					listViewItem2.Selected = true;
					listViewItem2.Checked = true;
					listViewItem2.BeginEdit();
				}

				private void renameIconButton_Click(object sender, EventArgs e)
				{
					this.iconList.SelectedItems[0].BeginEdit();
				}

				private IContainer components;

				private ListView iconList;

				private IconLibrary iconLibrary;

				private FlowLayoutPanel previewPanel;

				private ToolStripButton duplicateIconButton;

				private ToolStripButton renameIconButton;

				private IconLibrary.IconReferenceCollection collectionToEdit;
			}
		}
	}
}
