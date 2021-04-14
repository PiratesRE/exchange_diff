using System;
using Microsoft.Exchange.Data.TextConverters.Internal.Html;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Css
{
	internal class CssTokenBuilder : TokenBuilder
	{
		public CssTokenBuilder(char[] buffer, int maxProperties, int maxSelectors, int maxRuns, bool testBoundaryConditions) : base(new CssToken(), buffer, maxRuns, testBoundaryConditions)
		{
			this.cssToken = (CssToken)base.Token;
			int num = 16;
			int num2 = 16;
			if (!testBoundaryConditions)
			{
				this.maxProperties = maxProperties;
				this.maxSelectors = maxSelectors;
			}
			else
			{
				num = 1;
				num2 = 1;
				this.maxProperties = 5;
				this.maxSelectors = 5;
			}
			this.cssToken.PropertyList = new CssToken.PropertyEntry[num];
			this.cssToken.SelectorList = new CssToken.SelectorEntry[num2];
		}

		public new CssToken Token
		{
			get
			{
				return this.cssToken;
			}
		}

		public bool Incomplete
		{
			get
			{
				return this.state >= 10 && this.state != 10;
			}
		}

		public override void Reset()
		{
			if (this.state >= 6)
			{
				this.cssToken.Reset();
			}
			base.Reset();
		}

		public void StartRuleSet(int baseOffset, CssTokenId id)
		{
			this.state = 23;
			this.cssToken.TokenId = (TokenId)id;
			this.cssToken.Whole.HeadOffset = baseOffset;
			this.tailOffset = baseOffset;
		}

		public void EndRuleSet()
		{
			if (this.state >= 43)
			{
				this.EndDeclarations();
			}
			this.tokenValid = true;
			this.state = 6;
			this.token.WholePosition.Rewind(this.token.Whole);
		}

		public void BuildUniversalSelector()
		{
			this.StartSelectorName();
			this.EndSelectorName(0);
		}

		public bool CanAddSelector()
		{
			return this.cssToken.SelectorTail - this.cssToken.SelectorHead < this.maxSelectors;
		}

		public void StartSelectorName()
		{
			if (this.cssToken.SelectorTail == this.cssToken.SelectorList.Length)
			{
				int num;
				if (this.maxSelectors / 2 > this.cssToken.SelectorList.Length)
				{
					num = this.cssToken.SelectorList.Length * 2;
				}
				else
				{
					num = this.maxSelectors;
				}
				CssToken.SelectorEntry[] array = new CssToken.SelectorEntry[num];
				Array.Copy(this.cssToken.SelectorList, 0, array, 0, this.cssToken.SelectorTail);
				this.cssToken.SelectorList = array;
			}
			this.cssToken.SelectorList[this.cssToken.SelectorTail].NameId = HtmlNameIndex.Unknown;
			this.cssToken.SelectorList[this.cssToken.SelectorTail].Name.Initialize(this.cssToken.Whole.Tail, this.tailOffset);
			this.cssToken.SelectorList[this.cssToken.SelectorTail].ClassName.Reset();
			this.state = 24;
		}

		public void EndSelectorName(int nameLength)
		{
			this.cssToken.SelectorList[this.cssToken.SelectorTail].Name.Tail = this.cssToken.Whole.Tail;
			this.cssToken.SelectorList[this.cssToken.SelectorTail].NameId = this.LookupTagName(nameLength, this.cssToken.SelectorList[this.cssToken.SelectorTail].Name);
			this.state = 25;
		}

		public void StartSelectorClass(CssSelectorClassType classType)
		{
			this.cssToken.SelectorList[this.cssToken.SelectorTail].ClassName.Initialize(this.cssToken.Whole.Tail, this.tailOffset);
			this.cssToken.SelectorList[this.cssToken.SelectorTail].ClassType = classType;
			this.state = 26;
		}

		public void EndSelectorClass()
		{
			this.cssToken.SelectorList[this.cssToken.SelectorTail].ClassName.Tail = this.cssToken.Whole.Tail;
			this.state = 27;
		}

		public void SetSelectorCombinator(CssSelectorCombinator combinator, bool previous)
		{
			int num = this.cssToken.SelectorTail;
			if (previous)
			{
				num--;
			}
			this.cssToken.SelectorList[num].Combinator = combinator;
		}

		public void EndSimpleSelector()
		{
			this.cssToken.SelectorTail++;
		}

		public void StartDeclarations(int baseOffset)
		{
			this.state = 43;
			if (this.cssToken.TokenId == TokenId.None)
			{
				this.cssToken.TokenId = (TokenId)5;
			}
			this.cssToken.PartMajor = CssToken.PropertyListPartMajor.Begin;
			this.cssToken.PartMinor = CssToken.PropertyListPartMinor.Empty;
			this.cssToken.Whole.HeadOffset = baseOffset;
			this.tailOffset = baseOffset;
		}

		public bool CanAddProperty()
		{
			return this.cssToken.PropertyTail - this.cssToken.PropertyHead < this.maxProperties;
		}

		public void StartPropertyName()
		{
			if (this.cssToken.PropertyTail == this.cssToken.PropertyList.Length)
			{
				int num;
				if (this.maxProperties / 2 > this.cssToken.PropertyList.Length)
				{
					num = this.cssToken.PropertyList.Length * 2;
				}
				else
				{
					num = this.maxProperties;
				}
				CssToken.PropertyEntry[] array = new CssToken.PropertyEntry[num];
				Array.Copy(this.cssToken.PropertyList, 0, array, 0, this.cssToken.PropertyTail);
				this.cssToken.PropertyList = array;
			}
			if (this.cssToken.PartMinor == CssToken.PropertyListPartMinor.Empty)
			{
				this.cssToken.PartMinor = CssToken.PropertyListPartMinor.BeginProperty;
			}
			this.cssToken.PropertyList[this.cssToken.PropertyTail].NameId = CssNameIndex.Unknown;
			this.cssToken.PropertyList[this.cssToken.PropertyTail].PartMajor = CssToken.PropertyPartMajor.Begin;
			this.cssToken.PropertyList[this.cssToken.PropertyTail].PartMinor = CssToken.PropertyPartMinor.BeginName;
			this.cssToken.PropertyList[this.cssToken.PropertyTail].QuoteChar = 0;
			this.cssToken.PropertyList[this.cssToken.PropertyTail].Name.Initialize(this.cssToken.Whole.Tail, this.tailOffset);
			this.cssToken.PropertyList[this.cssToken.PropertyTail].Value.Reset();
			this.state = 44;
		}

		public void EndPropertyName(int nameLength)
		{
			this.cssToken.PropertyList[this.cssToken.PropertyTail].Name.Tail = this.cssToken.Whole.Tail;
			CssToken.PropertyEntry[] propertyList = this.cssToken.PropertyList;
			int propertyTail = this.cssToken.PropertyTail;
			propertyList[propertyTail].PartMinor = (propertyList[propertyTail].PartMinor | CssToken.PropertyPartMinor.EndName);
			if (this.cssToken.PropertyList[this.cssToken.PropertyTail].IsPropertyBegin)
			{
				this.cssToken.PropertyList[this.cssToken.PropertyTail].NameId = this.LookupName(nameLength, this.cssToken.PropertyList[this.cssToken.PropertyTail].Name);
			}
			this.state = 45;
		}

		public void StartPropertyValue()
		{
			this.cssToken.PropertyList[this.cssToken.PropertyTail].Value.Initialize(this.cssToken.Whole.Tail, this.tailOffset);
			CssToken.PropertyEntry[] propertyList = this.cssToken.PropertyList;
			int propertyTail = this.cssToken.PropertyTail;
			propertyList[propertyTail].PartMinor = (propertyList[propertyTail].PartMinor | CssToken.PropertyPartMinor.BeginValue);
			this.state = 46;
		}

		public void SetPropertyValueQuote(char ch)
		{
			this.cssToken.PropertyList[this.cssToken.PropertyTail].IsPropertyValueQuoted = true;
			this.cssToken.PropertyList[this.cssToken.PropertyTail].QuoteChar = (byte)ch;
		}

		public void EndPropertyValue()
		{
			this.cssToken.PropertyList[this.cssToken.PropertyTail].Value.Tail = this.cssToken.Whole.Tail;
			CssToken.PropertyEntry[] propertyList = this.cssToken.PropertyList;
			int propertyTail = this.cssToken.PropertyTail;
			propertyList[propertyTail].PartMinor = (propertyList[propertyTail].PartMinor | CssToken.PropertyPartMinor.EndValue);
			this.state = 47;
		}

		public void EndProperty()
		{
			CssToken.PropertyEntry[] propertyList = this.cssToken.PropertyList;
			int propertyTail = this.cssToken.PropertyTail;
			propertyList[propertyTail].PartMajor = (propertyList[propertyTail].PartMajor | CssToken.PropertyPartMajor.End);
			this.cssToken.PropertyTail++;
			if (this.cssToken.PropertyTail < this.cssToken.PropertyList.Length)
			{
				this.cssToken.PropertyList[this.cssToken.PropertyTail].PartMajor = CssToken.PropertyPartMajor.None;
				this.cssToken.PropertyList[this.cssToken.PropertyTail].PartMinor = CssToken.PropertyPartMinor.Empty;
			}
			if (this.cssToken.PartMinor == CssToken.PropertyListPartMinor.BeginProperty)
			{
				this.cssToken.PartMinor = CssToken.PropertyListPartMinor.Properties;
			}
			else if (this.cssToken.PartMinor == CssToken.PropertyListPartMinor.ContinueProperty)
			{
				this.cssToken.PartMinor = CssToken.PropertyListPartMinor.EndProperty;
			}
			else
			{
				CssToken cssToken = this.cssToken;
				cssToken.PartMinor |= CssToken.PropertyListPartMinor.Properties;
			}
			this.state = 43;
		}

		public void EndDeclarations()
		{
			if (this.state != 20)
			{
				if (this.state == 44)
				{
					this.cssToken.PropertyList[this.cssToken.PropertyTail].Name.Tail = this.cssToken.Whole.Tail;
				}
				else if (this.state == 46)
				{
					this.cssToken.PropertyList[this.cssToken.PropertyTail].Value.Tail = this.cssToken.Whole.Tail;
				}
			}
			if (this.state == 44)
			{
				this.EndPropertyName(0);
			}
			else if (this.state == 46)
			{
				this.EndPropertyValue();
			}
			if (this.state == 45 || this.state == 47)
			{
				this.EndProperty();
			}
			this.state = 43;
			CssToken cssToken = this.cssToken;
			cssToken.PartMajor |= CssToken.PropertyListPartMajor.End;
			this.tokenValid = true;
		}

		public bool PrepareAndAddRun(CssRunKind cssRunKind, int start, int end)
		{
			if (end != start)
			{
				if (!base.PrepareToAddMoreRuns(1))
				{
					return false;
				}
				base.AddRun((cssRunKind == CssRunKind.Invalid) ? RunType.Invalid : ((RunType)2147483648U), (cssRunKind == CssRunKind.Space) ? RunTextType.Space : RunTextType.NonSpace, (uint)cssRunKind, start, end, 0);
			}
			return true;
		}

		public bool PrepareAndAddInvalidRun(CssRunKind cssRunKind, int end)
		{
			if (!base.PrepareToAddMoreRuns(1))
			{
				return false;
			}
			base.AddInvalidRun(end, (uint)cssRunKind);
			return true;
		}

		public bool PrepareAndAddLiteralRun(CssRunKind cssRunKind, int start, int end, int value)
		{
			if (end != start)
			{
				if (!base.PrepareToAddMoreRuns(1))
				{
					return false;
				}
				base.AddRun((RunType)3221225472U, RunTextType.NonSpace, (uint)cssRunKind, start, end, value);
			}
			return true;
		}

		public void InvalidateLastValidRun(CssRunKind kind)
		{
			int num = this.token.Whole.Tail;
			Token.RunEntry runEntry;
			for (;;)
			{
				num--;
				runEntry = this.token.RunList[num];
				if (runEntry.Type != RunType.Invalid)
				{
					break;
				}
				if (num <= 0)
				{
					return;
				}
			}
			if (kind == (CssRunKind)runEntry.Kind)
			{
				this.token.RunList[num].Initialize(RunType.Invalid, runEntry.TextType, runEntry.Kind, runEntry.Length, runEntry.Value);
				return;
			}
		}

		public void MarkPropertyAsDeleted()
		{
			this.cssToken.PropertyList[this.cssToken.PropertyTail].IsPropertyDeleted = true;
		}

		public CssTokenId MakeEmptyToken(CssTokenId tokenId)
		{
			return (CssTokenId)base.MakeEmptyToken((TokenId)tokenId);
		}

		public CssNameIndex LookupName(int nameLength, Token.Fragment fragment)
		{
			if (nameLength > 26)
			{
				return CssNameIndex.Unknown;
			}
			short num = (short)((ulong)(this.token.CalculateHashLowerCase(fragment) ^ 2) % 329UL);
			int num2 = (int)CssData.nameHashTable[(int)num];
			if (num2 > 0)
			{
				for (;;)
				{
					string name = CssData.names[num2].Name;
					if (name.Length == nameLength)
					{
						if (fragment.Tail == fragment.Head + 1)
						{
							if (name[0] == ParseSupport.ToLowerCase(this.token.Buffer[fragment.HeadOffset]) && (nameLength == 1 || this.token.CaseInsensitiveCompareRunEqual(fragment.HeadOffset + 1, name, 1)))
							{
								break;
							}
						}
						else if (this.token.CaseInsensitiveCompareEqual(ref fragment, name))
						{
							goto Block_6;
						}
					}
					num2++;
					if (CssData.names[num2].Hash != num)
					{
						return CssNameIndex.Unknown;
					}
				}
				return (CssNameIndex)num2;
				Block_6:
				return (CssNameIndex)num2;
			}
			return CssNameIndex.Unknown;
		}

		public HtmlNameIndex LookupTagName(int nameLength, Token.Fragment fragment)
		{
			if (nameLength > 14)
			{
				return HtmlNameIndex.Unknown;
			}
			short num = (short)((ulong)(this.token.CalculateHashLowerCase(fragment) ^ 221) % 601UL);
			int num2 = (int)HtmlNameData.nameHashTable[(int)num];
			if (num2 > 0)
			{
				for (;;)
				{
					string name = HtmlNameData.Names[num2].Name;
					if (name.Length == nameLength)
					{
						if (fragment.Tail == fragment.Head + 1)
						{
							if (name[0] == ParseSupport.ToLowerCase(this.token.Buffer[fragment.HeadOffset]) && (nameLength == 1 || this.token.CaseInsensitiveCompareRunEqual(fragment.HeadOffset + 1, name, 1)))
							{
								break;
							}
						}
						else if (this.token.CaseInsensitiveCompareEqual(ref fragment, name))
						{
							goto Block_6;
						}
					}
					num2++;
					if (HtmlNameData.Names[num2].Hash != num)
					{
						return HtmlNameIndex.Unknown;
					}
				}
				return (HtmlNameIndex)num2;
				Block_6:
				return (HtmlNameIndex)num2;
			}
			return HtmlNameIndex.Unknown;
		}

		protected const byte BuildStateEndedCss = 6;

		protected const byte BuildStatePropertyListStarted = 20;

		protected const byte BuildStateBeforeSelector = 23;

		protected const byte BuildStateSelectorName = 24;

		protected const byte BuildStateEndSelectorName = 25;

		protected const byte BuildStateSelectorClass = 26;

		protected const byte BuildStateEndSelectorClass = 27;

		protected const byte BuildStateBeforeProperty = 43;

		protected const byte BuildStatePropertyName = 44;

		protected const byte BuildStateEndPropertyName = 45;

		protected const byte BuildStatePropertyValue = 46;

		protected const byte BuildStateEndPropertyValue = 47;

		protected CssToken cssToken;

		protected int maxProperties;

		protected int maxSelectors;
	}
}
