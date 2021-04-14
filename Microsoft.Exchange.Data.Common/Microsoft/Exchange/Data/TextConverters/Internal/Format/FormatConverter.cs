using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal abstract class FormatConverter : IProgressMonitor
	{
		internal FormatConverter(Stream formatConverterTraceStream)
		{
			this.Store = new FormatStore();
			this.BuildStack = new FormatConverter.BuildStackEntry[16];
			this.ContainerStyleBuildHelper = new StyleBuildHelper(this.Store);
			this.StyleBuildHelper = new StyleBuildHelper(this.Store);
			this.MultiValueBuildHelper = new MultiValueBuildHelper(this.Store);
			this.FontFaceDictionary = new Dictionary<string, PropertyValue>(StringComparer.OrdinalIgnoreCase);
		}

		internal FormatConverter(FormatStore formatStore, Stream formatConverterTraceStream)
		{
			this.Store = formatStore;
			this.BuildStack = new FormatConverter.BuildStackEntry[16];
			this.ContainerStyleBuildHelper = new StyleBuildHelper(this.Store);
			this.StyleBuildHelper = new StyleBuildHelper(this.Store);
			this.MultiValueBuildHelper = new MultiValueBuildHelper(this.Store);
			this.FontFaceDictionary = new Dictionary<string, PropertyValue>(StringComparer.OrdinalIgnoreCase);
		}

		public FormatConverterContainer Root
		{
			get
			{
				return new FormatConverterContainer(this, 0);
			}
		}

		public FormatConverterContainer Last
		{
			get
			{
				return new FormatConverterContainer(this, this.EmptyContainer ? this.BuildStackTop : (this.BuildStackTop - 1));
			}
		}

		public FormatNode LastNode
		{
			get
			{
				return new FormatNode(this.Store, this.LastNodeInternal);
			}
		}

		public FormatConverterContainer LastNonEmpty
		{
			get
			{
				return new FormatConverterContainer(this, this.BuildStackTop - 1);
			}
		}

		public bool EndOfFile
		{
			get
			{
				return this.endOfFile;
			}
		}

		protected bool MustFlush
		{
			get
			{
				return this.mustFlush;
			}
			set
			{
				this.mustFlush = value;
			}
		}

		public abstract void Run();

		public FormatConverterContainer OpenContainer(FormatContainerType nodeType, bool empty)
		{
			if (!this.ContainerFlushed)
			{
				this.FlushContainer(this.EmptyContainer ? this.BuildStackTop : (this.BuildStackTop - 1));
			}
			if (this.EmptyContainer)
			{
				this.PrepareToCloseContainer(this.BuildStackTop);
			}
			int level = this.PushContainer(nodeType, empty, 1);
			return new FormatConverterContainer(this, level);
		}

		public FormatConverterContainer OpenContainer(FormatContainerType nodeType, bool empty, int inheritanceMaskIndex, FormatStyle baseStyle, HtmlNameIndex tagName)
		{
			if (!this.ContainerFlushed)
			{
				this.FlushContainer(this.EmptyContainer ? this.BuildStackTop : (this.BuildStackTop - 1));
			}
			if (this.EmptyContainer)
			{
				this.PrepareToCloseContainer(this.BuildStackTop);
			}
			int num = this.PushContainer(nodeType, empty, inheritanceMaskIndex);
			if (!baseStyle.IsNull)
			{
				baseStyle.AddRef();
				this.ContainerStyleBuildHelper.AddStyle(10, baseStyle.Handle);
			}
			this.BuildStack[num].TagName = tagName;
			return new FormatConverterContainer(this, num);
		}

		public FormatConverterContainer OpenContainer(FormatContainerType nodeType, bool empty, int inheritanceMaskIndex, FormatStyle baseStyle, HtmlTagIndex tagIndex)
		{
			if (!this.ContainerFlushed)
			{
				this.FlushContainer(this.EmptyContainer ? this.BuildStackTop : (this.BuildStackTop - 1));
			}
			if (this.EmptyContainer)
			{
				this.PrepareToCloseContainer(this.BuildStackTop);
			}
			int num = this.PushContainer(nodeType, empty, inheritanceMaskIndex);
			if (!baseStyle.IsNull)
			{
				baseStyle.AddRef();
				this.ContainerStyleBuildHelper.AddStyle(10, baseStyle.Handle);
			}
			this.BuildStack[num].TagIndex = tagIndex;
			return new FormatConverterContainer(this, num);
		}

		public void OpenTextContainer()
		{
			if (!this.ContainerFlushed)
			{
				this.FlushContainer(this.EmptyContainer ? this.BuildStackTop : (this.BuildStackTop - 1));
			}
			if (this.EmptyContainer)
			{
				this.PrepareToCloseContainer(this.BuildStackTop);
			}
			this.PrepareToAddText();
		}

		public void CloseContainer()
		{
			if (!this.ContainerFlushed)
			{
				this.FlushContainer(this.EmptyContainer ? this.BuildStackTop : (this.BuildStackTop - 1));
			}
			if (this.EmptyContainer)
			{
				this.PrepareToCloseContainer(this.BuildStackTop);
			}
			this.PopContainer();
		}

		public void CloseOverlappingContainer(int countLevelsToKeepOpen)
		{
			if (!this.ContainerFlushed)
			{
				this.FlushContainer(this.EmptyContainer ? this.BuildStackTop : (this.BuildStackTop - 1));
			}
			if (this.EmptyContainer)
			{
				this.PrepareToCloseContainer(this.BuildStackTop);
			}
			this.PopContainer(this.BuildStackTop - 1 - countLevelsToKeepOpen);
		}

		public void CloseAllContainersAndSetEOF()
		{
			while (this.BuildStackTop > 1)
			{
				this.CloseContainer();
			}
			this.Store.GetNode(this.BuildStack[0].Node).PrepareToClose(this.Store.CurrentTextPosition);
			this.mustFlush = true;
			this.endOfFile = true;
		}

		public void AddNonSpaceText(char[] buffer, int offset, int count)
		{
			this.PrepareToAddText();
			if (!this.ContainerFlushed)
			{
				this.FlushContainer(this.BuildStackTop);
			}
			this.newLine = false;
			if (this.textQuotingExpected)
			{
				if (buffer[offset] == '>')
				{
					do
					{
						this.Store.AddText(buffer, offset, 1);
						offset++;
						count--;
					}
					while (count != 0 && buffer[offset] == '>');
					if (count == 0)
					{
						return;
					}
				}
				this.Store.SetTextBoundary();
				this.textQuotingExpected = false;
			}
			this.Store.AddText(buffer, offset, count);
		}

		public void AddSpace(int count)
		{
			this.PrepareToAddText();
			if (!this.ContainerFlushed)
			{
				this.FlushContainer(this.BuildStackTop);
			}
			this.Store.AddSpace(count);
			this.newLine = false;
		}

		public void AddLineBreak(int count)
		{
			this.PrepareToAddText();
			if (!this.ContainerFlushed)
			{
				this.FlushContainer(this.BuildStackTop);
			}
			if (!this.newLine)
			{
				this.Store.AddLineBreak(1);
				this.Store.SetTextBoundary();
				if (count > 1)
				{
					this.Store.AddLineBreak(count - 1);
				}
				this.newLine = true;
				this.textQuotingExpected = true;
				return;
			}
			this.Store.AddLineBreak(count);
		}

		public void AddNbsp(int count)
		{
			this.PrepareToAddText();
			if (!this.ContainerFlushed)
			{
				this.FlushContainer(this.BuildStackTop);
			}
			this.Store.AddNbsp(count);
			this.newLine = false;
		}

		public void AddTabulation(int count)
		{
			this.PrepareToAddText();
			if (!this.ContainerFlushed)
			{
				this.FlushContainer(this.BuildStackTop);
			}
			this.Store.AddTabulation(count);
			this.newLine = false;
			this.textQuotingExpected = false;
		}

		public StringValue RegisterStringValue(bool isStatic, string value)
		{
			return this.Store.AllocateStringValue(isStatic, value);
		}

		public StringValue RegisterStringValue(bool isStatic, string str, int offset, int count)
		{
			string value = str;
			if (offset != 0 || count != str.Length)
			{
				value = str.Substring(offset, count);
			}
			return this.Store.AllocateStringValue(isStatic, value);
		}

		public StringValue RegisterStringValue(bool isStatic, BufferString value)
		{
			return this.Store.AllocateStringValue(isStatic, value.ToString());
		}

		public PropertyValue RegisterFaceName(bool isStatic, BufferString value)
		{
			if (value.Length == 0)
			{
				return PropertyValue.Null;
			}
			return this.RegisterFaceName(isStatic, value.ToString());
		}

		public PropertyValue RegisterFaceName(bool isStatic, string faceName)
		{
			if (string.IsNullOrEmpty(faceName))
			{
				return PropertyValue.Null;
			}
			PropertyValue propertyValue;
			if (this.FontFaceDictionary.TryGetValue(faceName, out propertyValue))
			{
				if (propertyValue.IsString)
				{
					this.Store.AddRefValue(propertyValue);
				}
				return propertyValue;
			}
			StringValue stringValue = this.RegisterStringValue(isStatic, faceName);
			propertyValue = stringValue.PropertyValue;
			if (this.FontFaceDictionary.Count < 100)
			{
				stringValue.AddRef();
				this.FontFaceDictionary.Add(faceName, propertyValue);
			}
			return propertyValue;
		}

		public MultiValue RegisterMultiValue(bool isStatic, out MultiValueBuilder builder)
		{
			MultiValue result = this.Store.AllocateMultiValue(isStatic);
			builder = new MultiValueBuilder(this, result.Handle);
			return result;
		}

		public FormatStyle RegisterStyle(bool isStatic, out StyleBuilder builder)
		{
			FormatStyle result = this.Store.AllocateStyle(isStatic);
			builder = new StyleBuilder(this, result.Handle);
			return result;
		}

		public FormatStyle GetStyle(int styleHandle)
		{
			return this.Store.GetStyle(styleHandle);
		}

		public StringValue GetStringValue(PropertyValue pv)
		{
			return this.Store.GetStringValue(pv);
		}

		public MultiValue GetMultiValue(PropertyValue pv)
		{
			return this.Store.GetMultiValue(pv);
		}

		public void ReleasePropertyValue(PropertyValue pv)
		{
			this.Store.ReleaseValue(pv);
		}

		void IProgressMonitor.ReportProgress()
		{
			this.madeProgress = true;
		}

		internal FormatNode InitializeDocument()
		{
			this.Initialize();
			return this.OpenContainer(FormatContainerType.Document, false).Node;
		}

		internal FormatNode InitializeFragment()
		{
			this.Initialize();
			FormatConverterContainer formatConverterContainer = this.OpenContainer(FormatContainerType.Fragment, false);
			this.OpenContainer(FormatContainerType.PropertyContainer, false);
			this.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontFace, this.RegisterFaceName(false, "Times New Roman"));
			this.Last.SetProperty(PropertyPrecedence.InlineStyle, PropertyId.FontSize, new PropertyValue(LengthUnits.Points, 11));
			return formatConverterContainer.Node;
		}

		protected void CloseContainer(FormatContainerType containerType)
		{
			if (!this.ContainerFlushed)
			{
				this.FlushContainer(this.EmptyContainer ? this.BuildStackTop : (this.BuildStackTop - 1));
			}
			if (this.EmptyContainer)
			{
				this.PrepareToCloseContainer(this.BuildStackTop);
			}
			for (int i = this.BuildStackTop - 1; i > 0; i--)
			{
				if (this.BuildStack[i].Type == containerType)
				{
					this.PopContainer(i);
					return;
				}
			}
		}

		protected void CloseContainer(HtmlNameIndex tagName)
		{
			if (!this.ContainerFlushed)
			{
				this.FlushContainer(this.EmptyContainer ? this.BuildStackTop : (this.BuildStackTop - 1));
			}
			if (this.EmptyContainer)
			{
				this.PrepareToCloseContainer(this.BuildStackTop);
			}
			for (int i = this.BuildStackTop - 1; i > 0; i--)
			{
				if (this.BuildStack[i].TagName == tagName)
				{
					this.PopContainer(i);
					return;
				}
			}
		}

		protected FormatNode CreateNode(FormatContainerType type)
		{
			FormatNode result = this.Store.AllocateNode(type);
			result.EndTextPosition = result.BeginTextPosition;
			result.SetOutOfOrder();
			return result;
		}

		protected virtual FormatContainerType FixContainerType(FormatContainerType type, StyleBuildHelper styleBuilderWithContainerProperties)
		{
			return type;
		}

		protected virtual FormatNode GetParentForNewNode(FormatNode node, FormatNode defaultParent, int stackPos, out int propContainerInheritanceStopLevel)
		{
			propContainerInheritanceStopLevel = this.DefaultPropContainerInheritanceStopLevel(stackPos);
			return defaultParent;
		}

		protected int DefaultPropContainerInheritanceStopLevel(int stackPos)
		{
			int num = stackPos - 1;
			while (num >= 0 && this.BuildStack[num].Node == 0)
			{
				num--;
			}
			return num + 1;
		}

		private static string Indent(int level)
		{
			return "                                                  ".Substring(0, Math.Min("                                                  ".Length, level * 2));
		}

		private void Initialize()
		{
			this.BuildStackTop = 0;
			this.ContainerStyleBuildHelper.Clean();
			this.StyleBuildHelper.Clean();
			this.StyleBuildHelperLocked = false;
			this.MultiValueBuildHelper.Cancel();
			this.MultiValueBuildHelperLocked = false;
			this.FontFaceDictionary.Clear();
			this.LastNodeInternal = this.Store.RootNode.Handle;
			this.BuildStack[this.BuildStackTop].Type = FormatContainerType.Root;
			this.BuildStack[this.BuildStackTop].Node = this.Store.RootNode.Handle;
			this.BuildStackTop++;
			this.EmptyContainer = false;
			this.ContainerFlushed = true;
			this.mustFlush = false;
			this.endOfFile = false;
			this.newLine = true;
			this.textQuotingExpected = true;
		}

		public void AddMarkupText(char[] buffer, int offset, int count)
		{
			this.Store.AddMarkupText(buffer, offset, count);
		}

		private void PrepareToAddText()
		{
			if (!this.EmptyContainer || !this.BuildStack[this.BuildStackTop].IsText)
			{
				if (!this.ContainerFlushed)
				{
					this.FlushContainer(this.EmptyContainer ? this.BuildStackTop : (this.BuildStackTop - 1));
				}
				if (this.EmptyContainer)
				{
					this.PrepareToCloseContainer(this.BuildStackTop);
				}
				this.PushContainer(FormatContainerType.Text, true, 5);
			}
		}

		private void FlushContainer(int stackPos)
		{
			FormatContainerType formatContainerType = this.FixContainerType(this.BuildStack[stackPos].Type, this.ContainerStyleBuildHelper);
			if (formatContainerType != this.BuildStack[stackPos].Type)
			{
				this.BuildStack[stackPos].Type = formatContainerType;
			}
			this.ContainerStyleBuildHelper.GetPropertyList(out this.BuildStack[stackPos].Properties, out this.BuildStack[stackPos].FlagProperties, out this.BuildStack[stackPos].PropertyMask);
			if (!this.BuildStack[stackPos].IsPropertyContainerOrNull)
			{
				if (!this.newLine && (byte)(this.BuildStack[stackPos].Type & FormatContainerType.BlockFlag) != 0)
				{
					this.Store.AddBlockBoundary();
					this.newLine = true;
					this.textQuotingExpected = true;
				}
				FormatNode formatNode;
				if (formatContainerType == FormatContainerType.Document)
				{
					formatNode = this.Store.AllocateNode(this.BuildStack[stackPos].Type, 0U);
				}
				else
				{
					formatNode = this.Store.AllocateNode(this.BuildStack[stackPos].Type);
				}
				formatNode.SetOnRightEdge();
				if ((byte)(this.BuildStack[stackPos].Type & FormatContainerType.InlineObjectFlag) != 0)
				{
					this.Store.AddInlineObject();
				}
				FormatNode node = this.Store.GetNode(this.LastNodeInternal);
				int num;
				this.GetParentForNewNode(formatNode, node, stackPos, out num).AppendChild(formatNode);
				this.BuildStack[stackPos].Node = formatNode.Handle;
				this.LastNodeInternal = formatNode.Handle;
				FlagProperties flagProperties2;
				Property[] properties;
				PropertyBitMask propertyMask;
				if (num < stackPos)
				{
					FlagProperties flagProperties = FlagProperties.AllOn;
					PropertyBitMask propertyBitMask = PropertyBitMask.AllOn;
					int num2 = stackPos;
					while (num2 >= num && (!flagProperties.IsClear || !propertyBitMask.IsClear))
					{
						if (num2 == stackPos || this.BuildStack[num2].Type == FormatContainerType.PropertyContainer)
						{
							flagProperties2 = (this.BuildStack[num2].FlagProperties & flagProperties);
							this.ContainerStyleBuildHelper.AddProperties(11, flagProperties2, propertyBitMask, this.BuildStack[num2].Properties);
							flagProperties &= ~this.BuildStack[num2].FlagProperties;
							propertyBitMask &= ~this.BuildStack[num2].PropertyMask;
							flagProperties &= FormatStoreData.GlobalInheritanceMasks[this.BuildStack[num2].InheritanceMaskIndex].FlagProperties;
							propertyBitMask &= FormatStoreData.GlobalInheritanceMasks[this.BuildStack[num2].InheritanceMaskIndex].PropertyMask;
						}
						num2--;
					}
					this.ContainerStyleBuildHelper.GetPropertyList(out properties, out flagProperties2, out propertyMask);
				}
				else
				{
					flagProperties2 = this.BuildStack[stackPos].FlagProperties;
					propertyMask = this.BuildStack[stackPos].PropertyMask;
					properties = this.BuildStack[stackPos].Properties;
					if (properties != null)
					{
						for (int i = 0; i < properties.Length; i++)
						{
							if (properties[i].Value.IsRefCountedHandle)
							{
								this.Store.AddRefValue(properties[i].Value);
							}
						}
					}
				}
				formatNode.SetProps(flagProperties2, propertyMask, properties, this.BuildStack[stackPos].InheritanceMaskIndex);
			}
			this.ContainerStyleBuildHelper.Clean();
			this.ContainerFlushed = true;
		}

		private int PushContainer(FormatContainerType type, bool empty, int inheritanceMaskIndex)
		{
			int buildStackTop = this.BuildStackTop;
			if (buildStackTop == this.BuildStack.Length)
			{
				if (this.BuildStack.Length >= 4096)
				{
					throw new TextConvertersException(TextConvertersStrings.InputDocumentTooComplex);
				}
				int num = (2048 > this.BuildStack.Length) ? (this.BuildStack.Length * 2) : 4096;
				FormatConverter.BuildStackEntry[] array = new FormatConverter.BuildStackEntry[num];
				Array.Copy(this.BuildStack, 0, array, 0, this.BuildStackTop);
				this.BuildStack = array;
			}
			this.Store.SetTextBoundary();
			this.BuildStack[buildStackTop].Type = type;
			this.BuildStack[buildStackTop].TagName = HtmlNameIndex._NOTANAME;
			this.BuildStack[buildStackTop].InheritanceMaskIndex = inheritanceMaskIndex;
			this.BuildStack[buildStackTop].TagIndex = HtmlTagIndex._NULL;
			this.BuildStack[buildStackTop].FlagProperties.ClearAll();
			this.BuildStack[buildStackTop].PropertyMask.ClearAll();
			this.BuildStack[buildStackTop].Properties = null;
			this.BuildStack[buildStackTop].Node = 0;
			if (!empty)
			{
				this.BuildStackTop++;
			}
			this.EmptyContainer = empty;
			this.ContainerFlushed = false;
			return buildStackTop;
		}

		private void PopContainer()
		{
			this.PrepareToCloseContainer(this.BuildStackTop - 1);
			this.BuildStackTop--;
		}

		private void PopContainer(int level)
		{
			this.PrepareToCloseContainer(level);
			Array.Copy(this.BuildStack, level + 1, this.BuildStack, level, this.BuildStackTop - level - 1);
			this.BuildStackTop--;
		}

		private void PrepareToCloseContainer(int stackPosition)
		{
			if (this.BuildStack[stackPosition].Properties != null)
			{
				for (int i = 0; i < this.BuildStack[stackPosition].Properties.Length; i++)
				{
					if (this.BuildStack[stackPosition].Properties[i].Value.IsRefCountedHandle)
					{
						this.Store.ReleaseValue(this.BuildStack[stackPosition].Properties[i].Value);
					}
				}
				this.BuildStack[stackPosition].Properties = null;
			}
			if (this.BuildStack[stackPosition].Node != 0)
			{
				FormatNode node = this.Store.GetNode(this.BuildStack[stackPosition].Node);
				if (!this.newLine && (byte)(node.NodeType & FormatContainerType.BlockFlag) != 0)
				{
					this.Store.AddBlockBoundary();
					this.newLine = true;
					this.textQuotingExpected = true;
				}
				node.PrepareToClose(this.Store.CurrentTextPosition);
				if (!node.Parent.IsNull && node.Parent.NodeType == FormatContainerType.TableContainer)
				{
					node.Parent.PrepareToClose(this.Store.CurrentTextPosition);
				}
				if (this.BuildStack[stackPosition].Node == this.LastNodeInternal)
				{
					for (int j = stackPosition - 1; j >= 0; j--)
					{
						if (this.BuildStack[j].Node != 0)
						{
							this.LastNodeInternal = this.BuildStack[j].Node;
							break;
						}
					}
				}
			}
			this.Store.SetTextBoundary();
			this.EmptyContainer = false;
		}

		public virtual FormatStore ConvertToStore()
		{
			long num = 0L;
			while (!this.endOfFile)
			{
				this.Run();
				if (this.madeProgress)
				{
					this.madeProgress = false;
					num = 0L;
				}
				else
				{
					long num2 = 200000L;
					long num3 = num;
					num = num3 + 1L;
					if (num2 == num3)
					{
						throw new TextConvertersException(TextConvertersStrings.TooManyIterationsToProduceOutput);
					}
				}
			}
			return this.Store;
		}

		internal FormatStore Store;

		internal FormatConverter.BuildStackEntry[] BuildStack;

		internal int BuildStackTop;

		internal int LastNodeInternal;

		internal bool EmptyContainer;

		internal bool ContainerFlushed;

		internal StyleBuildHelper ContainerStyleBuildHelper;

		internal StyleBuildHelper StyleBuildHelper;

		internal bool StyleBuildHelperLocked;

		internal MultiValueBuildHelper MultiValueBuildHelper;

		internal bool MultiValueBuildHelperLocked;

		internal Dictionary<string, PropertyValue> FontFaceDictionary;

		protected bool madeProgress;

		private bool mustFlush;

		private bool endOfFile;

		private bool newLine;

		private bool textQuotingExpected;

		internal struct BuildStackEntry
		{
			public bool IsText
			{
				get
				{
					return this.Type == FormatContainerType.Text;
				}
			}

			public bool IsPropertyContainer
			{
				get
				{
					return this.Type == FormatContainerType.PropertyContainer;
				}
			}

			public bool IsPropertyContainerOrNull
			{
				get
				{
					return this.Type == FormatContainerType.PropertyContainer || this.Type == FormatContainerType.Null;
				}
			}

			public FormatContainerType NodeType
			{
				get
				{
					return this.Type;
				}
				set
				{
					this.Type = value;
				}
			}

			public void Clean()
			{
				this = default(FormatConverter.BuildStackEntry);
			}

			internal FormatContainerType Type;

			internal HtmlNameIndex TagName;

			internal HtmlTagIndex TagIndex;

			internal int Node;

			internal int InheritanceMaskIndex;

			internal Property[] Properties;

			internal FlagProperties FlagProperties;

			internal PropertyBitMask PropertyMask;
		}
	}
}
