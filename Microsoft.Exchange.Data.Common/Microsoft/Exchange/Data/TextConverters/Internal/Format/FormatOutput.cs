using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal abstract class FormatOutput : IDisposable
	{
		public virtual bool OutputCodePageSameAsInput
		{
			get
			{
				return false;
			}
		}

		public virtual Encoding OutputEncoding
		{
			set
			{
				throw new InvalidOperationException();
			}
		}

		public virtual bool CanAcceptMoreOutput
		{
			get
			{
				return true;
			}
		}

		protected FormatStore FormatStore
		{
			get
			{
				return this.formatStore;
			}
		}

		protected SourceFormat SourceFormat
		{
			get
			{
				return this.sourceFormat;
			}
		}

		protected string Comment
		{
			get
			{
				return this.comment;
			}
		}

		protected FormatNode CurrentNode
		{
			get
			{
				return this.currentOutputLevel.Node;
			}
		}

		protected int CurrentNodeIndex
		{
			get
			{
				return this.currentOutputLevel.Index;
			}
		}

		protected FormatOutput(Stream formatOutputTraceStream)
		{
		}

		public virtual void Initialize(FormatStore store, SourceFormat sourceFormat, string comment)
		{
			this.sourceFormat = sourceFormat;
			this.comment = comment;
			this.formatStore = store;
			this.Restart(this.formatStore.RootNode);
		}

		public void Restart(FormatNode rootNode)
		{
			this.outputStackTop = 0;
			this.currentOutputLevel.Node = rootNode;
			this.currentOutputLevel.State = FormatOutput.OutputState.NotStarted;
			this.rootNode = rootNode;
		}

		protected void Restart()
		{
			this.Restart(this.rootNode);
		}

		public bool HaveSomethingToFlush()
		{
			return this.currentOutputLevel.Node.CanFlush;
		}

		public FlagProperties GetEffectiveFlags()
		{
			return this.propertyState.GetEffectiveFlags();
		}

		public FlagProperties GetDistinctFlags()
		{
			return this.propertyState.GetDistinctFlags();
		}

		public PropertyValue GetEffectiveProperty(PropertyId id)
		{
			return this.propertyState.GetEffectiveProperty(id);
		}

		public PropertyValue GetDistinctProperty(PropertyId id)
		{
			return this.propertyState.GetDistinctProperty(id);
		}

		public void SubtractDefaultContainerPropertiesFromDistinct(FlagProperties flags, Property[] properties)
		{
			this.propertyState.SubtractDefaultFromDistinct(flags, properties);
		}

		public virtual bool Flush()
		{
			while (this.CanAcceptMoreOutput && this.currentOutputLevel.State != FormatOutput.OutputState.Ended)
			{
				if (this.currentOutputLevel.State == FormatOutput.OutputState.NotStarted)
				{
					if (this.StartCurrentLevel())
					{
						this.PushFirstChild();
					}
					else
					{
						this.PopPushNextSibling();
					}
				}
				else if (this.currentOutputLevel.State == FormatOutput.OutputState.Started)
				{
					if (this.ContinueCurrentLevel())
					{
						this.currentOutputLevel.State = FormatOutput.OutputState.EndPending;
					}
				}
				else
				{
					this.EndCurrentLevel();
					this.currentOutputLevel.State = FormatOutput.OutputState.Ended;
					if (this.outputStackTop != 0)
					{
						this.PopPushNextSibling();
					}
				}
			}
			return this.currentOutputLevel.State == FormatOutput.OutputState.Ended;
		}

		public void OutputFragment(FormatNode fragmentNode)
		{
			this.Restart(fragmentNode);
			this.FlushFragment();
		}

		public void OutputFragment(FormatNode beginNode, uint beginTextPosition, FormatNode endNode, uint endTextPosition)
		{
			this.Restart(this.rootNode);
			FormatNode formatNode = beginNode;
			int num = 0;
			while (formatNode != this.rootNode)
			{
				num++;
				formatNode = formatNode.Parent;
			}
			if (this.outputStack == null)
			{
				this.outputStack = new FormatOutput.OutputStackEntry[Math.Max(32, num)];
			}
			else if (this.outputStack.Length < num)
			{
				if (this.outputStackTop >= 4096)
				{
					throw new TextConvertersException(TextConvertersStrings.InputDocumentTooComplex);
				}
				this.outputStack = new FormatOutput.OutputStackEntry[Math.Max(this.outputStack.Length * 2, num)];
			}
			formatNode = beginNode;
			int i = num - 1;
			while (formatNode != this.rootNode)
			{
				this.outputStack[i--].Node = formatNode;
				formatNode = formatNode.Parent;
			}
			for (i = 0; i < num; i++)
			{
				if (!this.StartCurrentLevel())
				{
					this.PopPushNextSibling();
					break;
				}
				this.currentOutputLevel.State = FormatOutput.OutputState.Started;
				this.Push(this.outputStack[i].Node);
			}
			bool flag = false;
			while (this.currentOutputLevel.State != FormatOutput.OutputState.Ended)
			{
				if (this.currentOutputLevel.State == FormatOutput.OutputState.NotStarted)
				{
					if (this.StartCurrentLevel())
					{
						this.PushFirstChild();
					}
					else
					{
						this.PopPushNextSibling();
					}
				}
				else if (this.currentOutputLevel.State == FormatOutput.OutputState.Started)
				{
					uint num2 = (this.currentOutputLevel.Node == beginNode) ? beginTextPosition : this.currentOutputLevel.Node.BeginTextPosition;
					uint num3 = (this.currentOutputLevel.Node == endNode) ? endTextPosition : this.currentOutputLevel.Node.EndTextPosition;
					if (num2 <= num3)
					{
						this.ContinueText(num2, num3);
					}
					this.currentOutputLevel.State = FormatOutput.OutputState.EndPending;
				}
				else
				{
					this.EndCurrentLevel();
					this.currentOutputLevel.State = FormatOutput.OutputState.Ended;
					if (this.outputStackTop != 0)
					{
						if (!flag && this.currentOutputLevel.Node != endNode && (this.currentOutputLevel.Node.NextSibling.IsNull || this.currentOutputLevel.Node.NextSibling != endNode || (this.currentOutputLevel.Node.NextSibling.NodeType == FormatContainerType.Text && this.currentOutputLevel.Node.NextSibling.BeginTextPosition < endTextPosition)))
						{
							this.PopPushNextSibling();
						}
						else
						{
							this.Pop();
							this.currentOutputLevel.State = FormatOutput.OutputState.EndPending;
							flag = true;
						}
					}
				}
			}
		}

		private void FlushFragment()
		{
			while (this.currentOutputLevel.State != FormatOutput.OutputState.Ended)
			{
				if (this.currentOutputLevel.State == FormatOutput.OutputState.NotStarted)
				{
					if (this.StartCurrentLevel())
					{
						this.PushFirstChild();
					}
					else
					{
						this.PopPushNextSibling();
					}
				}
				else if (this.currentOutputLevel.State == FormatOutput.OutputState.Started)
				{
					if (this.ContinueCurrentLevel())
					{
						this.currentOutputLevel.State = FormatOutput.OutputState.EndPending;
					}
				}
				else
				{
					this.EndCurrentLevel();
					this.currentOutputLevel.State = FormatOutput.OutputState.Ended;
					if (this.outputStackTop != 0)
					{
						this.PopPushNextSibling();
					}
				}
			}
		}

		private bool StartCurrentLevel()
		{
			FormatContainerType nodeType = this.currentOutputLevel.Node.NodeType;
			switch (nodeType)
			{
			case FormatContainerType.TableContainer:
				return this.StartTableContainer();
			case FormatContainerType.TableDefinition:
				return this.StartTableDefinition();
			case FormatContainerType.TableColumnGroup:
				return this.StartTableColumnGroup();
			case FormatContainerType.TableColumn:
				this.StartEndTableColumn();
				return false;
			case (FormatContainerType)11:
			case (FormatContainerType)12:
			case (FormatContainerType)13:
			case (FormatContainerType)14:
			case (FormatContainerType)15:
			case (FormatContainerType)16:
			case (FormatContainerType)17:
			case (FormatContainerType)21:
			case (FormatContainerType)23:
			case (FormatContainerType)35:
				break;
			case FormatContainerType.Inline:
				return this.StartInline();
			case FormatContainerType.HyperLink:
				return this.StartHyperLink();
			case FormatContainerType.Bookmark:
				return this.StartBookmark();
			case FormatContainerType.Area:
				this.StartEndArea();
				return false;
			case FormatContainerType.BaseFont:
				this.StartEndBaseFont();
				return false;
			case FormatContainerType.Form:
				return this.StartForm();
			case FormatContainerType.FieldSet:
				return this.StartFieldSet();
			case FormatContainerType.Label:
				return this.StartLabel();
			case FormatContainerType.Input:
				return this.StartInput();
			case FormatContainerType.Button:
				return this.StartButton();
			case FormatContainerType.Legend:
				return this.StartLegend();
			case FormatContainerType.TextArea:
				return this.StartTextArea();
			case FormatContainerType.Select:
				return this.StartSelect();
			case FormatContainerType.OptionGroup:
				return this.StartOptionGroup();
			case FormatContainerType.Option:
				return this.StartOption();
			case FormatContainerType.Text:
				return this.StartText();
			default:
				if (nodeType == FormatContainerType.Image)
				{
					this.StartEndImage();
					return false;
				}
				switch (nodeType)
				{
				case FormatContainerType.Root:
					return this.StartRoot();
				case FormatContainerType.Document:
					return this.StartDocument();
				case FormatContainerType.Fragment:
					return this.StartFragment();
				case FormatContainerType.Block:
					return this.StartBlock();
				case FormatContainerType.BlockQuote:
					return this.StartBlockQuote();
				case FormatContainerType.HorizontalLine:
					this.StartEndHorizontalLine();
					return false;
				case FormatContainerType.TableCaption:
					return this.StartTableCaption();
				case FormatContainerType.TableExtraContent:
					return this.StartTableExtraContent();
				case FormatContainerType.Table:
					return this.StartTable();
				case FormatContainerType.TableRow:
					return this.StartTableRow();
				case FormatContainerType.TableCell:
					return this.StartTableCell();
				case FormatContainerType.List:
					return this.StartList();
				case FormatContainerType.ListItem:
					return this.StartListItem();
				case FormatContainerType.Map:
					return this.StartMap();
				}
				break;
			}
			return true;
		}

		private bool ContinueCurrentLevel()
		{
			return this.ContinueText(this.currentOutputLevel.Node.BeginTextPosition, this.currentOutputLevel.Node.EndTextPosition);
		}

		private void EndCurrentLevel()
		{
			FormatContainerType nodeType = this.currentOutputLevel.Node.NodeType;
			switch (nodeType)
			{
			case FormatContainerType.TableContainer:
				this.EndTableContainer();
				return;
			case FormatContainerType.TableDefinition:
				this.EndTableDefinition();
				return;
			case FormatContainerType.TableColumnGroup:
				this.EndTableColumnGroup();
				return;
			case FormatContainerType.TableColumn:
			case (FormatContainerType)11:
			case (FormatContainerType)12:
			case (FormatContainerType)13:
			case (FormatContainerType)14:
			case (FormatContainerType)15:
			case (FormatContainerType)16:
			case (FormatContainerType)17:
			case (FormatContainerType)21:
			case FormatContainerType.Area:
			case (FormatContainerType)23:
			case FormatContainerType.BaseFont:
			case (FormatContainerType)35:
				break;
			case FormatContainerType.Inline:
				this.EndInline();
				return;
			case FormatContainerType.HyperLink:
				this.EndHyperLink();
				return;
			case FormatContainerType.Bookmark:
				this.EndBookmark();
				return;
			case FormatContainerType.Form:
				this.EndForm();
				return;
			case FormatContainerType.FieldSet:
				this.EndFieldSet();
				return;
			case FormatContainerType.Label:
				this.EndLabel();
				return;
			case FormatContainerType.Input:
				this.EndInput();
				return;
			case FormatContainerType.Button:
				this.EndButton();
				return;
			case FormatContainerType.Legend:
				this.EndLegend();
				return;
			case FormatContainerType.TextArea:
				this.EndTextArea();
				return;
			case FormatContainerType.Select:
				this.EndSelect();
				return;
			case FormatContainerType.OptionGroup:
				this.EndOptionGroup();
				return;
			case FormatContainerType.Option:
				this.EndOption();
				return;
			case FormatContainerType.Text:
				this.EndText();
				break;
			default:
				switch (nodeType)
				{
				case FormatContainerType.Root:
					this.EndRoot();
					return;
				case FormatContainerType.Document:
					this.EndDocument();
					return;
				case FormatContainerType.Fragment:
					this.EndFragment();
					return;
				case FormatContainerType.Block:
					this.EndBlock();
					return;
				case FormatContainerType.BlockQuote:
					this.EndBlockQuote();
					return;
				case FormatContainerType.HorizontalLine:
				case (FormatContainerType)135:
				case (FormatContainerType)136:
				case (FormatContainerType)137:
				case (FormatContainerType)138:
				case (FormatContainerType)146:
				case (FormatContainerType)147:
				case (FormatContainerType)148:
				case (FormatContainerType)149:
				case (FormatContainerType)150:
					break;
				case FormatContainerType.TableCaption:
					this.EndTableCaption();
					return;
				case FormatContainerType.TableExtraContent:
					this.EndTableExtraContent();
					return;
				case FormatContainerType.Table:
					this.EndTable();
					return;
				case FormatContainerType.TableRow:
					this.EndTableRow();
					return;
				case FormatContainerType.TableCell:
					this.EndTableCell();
					return;
				case FormatContainerType.List:
					this.EndList();
					return;
				case FormatContainerType.ListItem:
					this.EndListItem();
					return;
				case FormatContainerType.Map:
					this.EndMap();
					return;
				default:
					return;
				}
				break;
			}
		}

		private void PushFirstChild()
		{
			FormatNode firstChild = this.currentOutputLevel.Node.FirstChild;
			if (!firstChild.IsNull)
			{
				this.currentOutputLevel.State = FormatOutput.OutputState.Started;
				this.Push(firstChild);
				return;
			}
			if (this.currentOutputLevel.Node.IsText)
			{
				this.currentOutputLevel.State = FormatOutput.OutputState.Started;
				return;
			}
			this.currentOutputLevel.State = FormatOutput.OutputState.EndPending;
		}

		private void PopPushNextSibling()
		{
			FormatNode nextSibling = this.currentOutputLevel.Node.NextSibling;
			this.Pop();
			this.currentOutputLevel.ChildIndex = this.currentOutputLevel.ChildIndex + 1;
			if (!nextSibling.IsNull)
			{
				this.Push(nextSibling);
				return;
			}
			this.currentOutputLevel.State = FormatOutput.OutputState.EndPending;
		}

		private void Push(FormatNode node)
		{
			if (this.outputStack == null)
			{
				this.outputStack = new FormatOutput.OutputStackEntry[32];
			}
			else if (this.outputStackTop == this.outputStack.Length)
			{
				if (this.outputStackTop >= 4096)
				{
					throw new TextConvertersException(TextConvertersStrings.InputDocumentTooComplex);
				}
				FormatOutput.OutputStackEntry[] destinationArray = new FormatOutput.OutputStackEntry[this.outputStack.Length * 2];
				Array.Copy(this.outputStack, 0, destinationArray, 0, this.outputStackTop);
				this.outputStack = destinationArray;
			}
			this.outputStack[this.outputStackTop++] = this.currentOutputLevel;
			this.currentOutputLevel.Node = node;
			this.currentOutputLevel.State = FormatOutput.OutputState.NotStarted;
			this.currentOutputLevel.Index = this.currentOutputLevel.ChildIndex;
			this.currentOutputLevel.ChildIndex = 0;
			this.currentOutputLevel.PropertyUndoLevel = this.propertyState.ApplyProperties(node.FlagProperties, node.Properties, FormatStoreData.GlobalInheritanceMasks[node.InheritanceMaskIndex].FlagProperties, FormatStoreData.GlobalInheritanceMasks[node.InheritanceMaskIndex].PropertyMask);
			node.SetOnLeftEdge();
		}

		private void Pop()
		{
			if (this.outputStackTop != 0)
			{
				this.currentOutputLevel.Node.ResetOnLeftEdge();
				this.propertyState.UndoProperties(this.currentOutputLevel.PropertyUndoLevel);
				this.currentOutputLevel = this.outputStack[--this.outputStackTop];
			}
		}

		protected virtual bool StartRoot()
		{
			return this.StartBlockContainer();
		}

		protected virtual void EndRoot()
		{
			this.EndBlockContainer();
		}

		protected virtual bool StartDocument()
		{
			return this.StartBlockContainer();
		}

		protected virtual void EndDocument()
		{
			this.EndBlockContainer();
		}

		protected virtual bool StartFragment()
		{
			return this.StartBlockContainer();
		}

		protected virtual void EndFragment()
		{
			this.EndBlockContainer();
		}

		protected virtual void StartEndBaseFont()
		{
		}

		protected virtual bool StartBlock()
		{
			return this.StartBlockContainer();
		}

		protected virtual void EndBlock()
		{
			this.EndBlockContainer();
		}

		protected virtual bool StartBlockQuote()
		{
			return this.StartBlockContainer();
		}

		protected virtual void EndBlockQuote()
		{
			this.EndBlockContainer();
		}

		protected virtual bool StartTableContainer()
		{
			return true;
		}

		protected virtual void EndTableContainer()
		{
		}

		protected virtual bool StartTableDefinition()
		{
			return true;
		}

		protected virtual void EndTableDefinition()
		{
		}

		protected virtual bool StartTableColumnGroup()
		{
			return true;
		}

		protected virtual void EndTableColumnGroup()
		{
		}

		protected virtual void StartEndTableColumn()
		{
		}

		protected virtual bool StartTableCaption()
		{
			return this.StartBlockContainer();
		}

		protected virtual void EndTableCaption()
		{
			this.EndBlockContainer();
		}

		protected virtual bool StartTableExtraContent()
		{
			return this.StartBlockContainer();
		}

		protected virtual void EndTableExtraContent()
		{
			this.EndBlockContainer();
		}

		protected virtual bool StartTable()
		{
			return this.StartBlockContainer();
		}

		protected virtual void EndTable()
		{
			this.EndBlockContainer();
		}

		protected virtual bool StartTableRow()
		{
			return this.StartBlockContainer();
		}

		protected virtual void EndTableRow()
		{
			this.EndBlockContainer();
		}

		protected virtual bool StartTableCell()
		{
			return this.StartBlockContainer();
		}

		protected virtual void EndTableCell()
		{
			this.EndBlockContainer();
		}

		protected virtual bool StartList()
		{
			return this.StartBlockContainer();
		}

		protected virtual void EndList()
		{
			this.EndBlockContainer();
		}

		protected virtual bool StartListItem()
		{
			return this.StartBlockContainer();
		}

		protected virtual void EndListItem()
		{
			this.EndBlockContainer();
		}

		protected virtual bool StartHyperLink()
		{
			return this.StartInlineContainer();
		}

		protected virtual void EndHyperLink()
		{
			this.EndInlineContainer();
		}

		protected virtual bool StartBookmark()
		{
			return true;
		}

		protected virtual void EndBookmark()
		{
		}

		protected virtual void StartEndImage()
		{
		}

		protected virtual void StartEndHorizontalLine()
		{
		}

		protected virtual bool StartInline()
		{
			return this.StartInlineContainer();
		}

		protected virtual void EndInline()
		{
			this.EndInlineContainer();
		}

		protected virtual bool StartMap()
		{
			return this.StartBlockContainer();
		}

		protected virtual void EndMap()
		{
			this.EndBlockContainer();
		}

		protected virtual void StartEndArea()
		{
		}

		protected virtual bool StartForm()
		{
			return this.StartInlineContainer();
		}

		protected virtual void EndForm()
		{
			this.EndInlineContainer();
		}

		protected virtual bool StartFieldSet()
		{
			return this.StartBlockContainer();
		}

		protected virtual void EndFieldSet()
		{
			this.EndBlockContainer();
		}

		protected virtual bool StartLabel()
		{
			return this.StartInlineContainer();
		}

		protected virtual void EndLabel()
		{
			this.EndInlineContainer();
		}

		protected virtual bool StartInput()
		{
			return this.StartInlineContainer();
		}

		protected virtual void EndInput()
		{
			this.EndInlineContainer();
		}

		protected virtual bool StartButton()
		{
			return this.StartInlineContainer();
		}

		protected virtual void EndButton()
		{
			this.EndInlineContainer();
		}

		protected virtual bool StartLegend()
		{
			return this.StartInlineContainer();
		}

		protected virtual void EndLegend()
		{
			this.EndInlineContainer();
		}

		protected virtual bool StartTextArea()
		{
			return this.StartInlineContainer();
		}

		protected virtual void EndTextArea()
		{
			this.EndInlineContainer();
		}

		protected virtual bool StartSelect()
		{
			return this.StartInlineContainer();
		}

		protected virtual void EndSelect()
		{
			this.EndInlineContainer();
		}

		protected virtual bool StartOptionGroup()
		{
			return true;
		}

		protected virtual void EndOptionGroup()
		{
		}

		protected virtual bool StartOption()
		{
			return true;
		}

		protected virtual void EndOption()
		{
		}

		protected virtual bool StartText()
		{
			return this.StartInlineContainer();
		}

		protected virtual bool ContinueText(uint beginTextPosition, uint endTextPosition)
		{
			return true;
		}

		protected virtual void EndText()
		{
			this.EndInlineContainer();
		}

		private static string Indent(int level)
		{
			return "                                                  ".Substring(0, Math.Min("                                                  ".Length, level * 2));
		}

		protected virtual bool StartBlockContainer()
		{
			return true;
		}

		protected virtual void EndBlockContainer()
		{
		}

		protected virtual bool StartInlineContainer()
		{
			return true;
		}

		protected virtual void EndInlineContainer()
		{
		}

		protected virtual void Dispose(bool disposing)
		{
			this.currentOutputLevel.Node = FormatNode.Null;
			this.outputStack = null;
			this.formatStore = null;
		}

		void IDisposable.Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private SourceFormat sourceFormat;

		private string comment;

		private FormatStore formatStore;

		private FormatNode rootNode;

		private FormatOutput.OutputStackEntry currentOutputLevel;

		private FormatOutput.OutputStackEntry[] outputStack;

		private int outputStackTop;

		protected ScratchBuffer scratchBuffer;

		protected ScratchBuffer scratchValueBuffer;

		private PropertyState propertyState = new PropertyState();

		private enum OutputState : byte
		{
			NotStarted,
			Started,
			EndPending,
			Ended
		}

		private struct OutputStackEntry
		{
			public FormatOutput.OutputState State;

			public FormatNode Node;

			public int Index;

			public int ChildIndex;

			public int PropertyUndoLevel;
		}
	}
}
