using System;
using System.IO;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Html
{
	internal class HtmlNormalizingParser : IHtmlParser, IRestartable, IReusable, IDisposable
	{
		public HtmlNormalizingParser(HtmlParser parser, HtmlInjection injection, bool ensureHead, int maxNesting, bool testBoundaryConditions, Stream traceStream, bool traceShowTokenNum, int traceStopOnTokenNum)
		{
			this.parser = parser;
			this.parser.SetRestartConsumer(this);
			this.injection = injection;
			if (injection != null)
			{
				this.saveState = new HtmlNormalizingParser.DocumentState();
			}
			this.ensureHead = ensureHead;
			int num = testBoundaryConditions ? 1 : 32;
			this.maxElementStack = (testBoundaryConditions ? 30 : maxNesting);
			this.openList = new HtmlTagIndex[8];
			this.closeList = new int[8];
			this.elementStack = new HtmlTagIndex[num];
			this.contextStack = new HtmlNormalizingParser.Context[testBoundaryConditions ? 1 : 4];
			this.elementStack[this.elementStackTop++] = HtmlTagIndex._ROOT;
			this.context.Type = HtmlDtd.ContextType.Root;
			this.context.TextType = HtmlDtd.ContextTextType.Full;
			this.context.Reject = HtmlDtd.SetId.Empty;
			this.queue = new HtmlNormalizingParser.QueueItem[testBoundaryConditions ? 1 : (num / 4)];
			this.tokenBuilder = new HtmlNormalizingParser.SmallTokenBuilder();
		}

		public HtmlToken Token
		{
			get
			{
				return this.token;
			}
		}

		public int CurrentOffset
		{
			get
			{
				return this.parser.CurrentOffset;
			}
		}

		public void SetRestartConsumer(IRestartable restartConsumer)
		{
			this.restartConsumer = restartConsumer;
		}

		public HtmlTokenId Parse()
		{
			while (this.QueueEmpty())
			{
				this.Process(this.parser.Parse());
			}
			return this.GetTokenFromQueue();
		}

		bool IRestartable.CanRestart()
		{
			return this.restartConsumer != null && this.restartConsumer.CanRestart();
		}

		void IRestartable.Restart()
		{
			if (this.restartConsumer != null)
			{
				this.restartConsumer.Restart();
			}
			this.Reinitialize();
		}

		void IRestartable.DisableRestart()
		{
			if (this.restartConsumer != null)
			{
				this.restartConsumer.DisableRestart();
			}
		}

		void IReusable.Initialize(object newSourceOrDestination)
		{
			((IReusable)this.parser).Initialize(newSourceOrDestination);
			this.Reinitialize();
			this.parser.SetRestartConsumer(this);
		}

		public void Initialize(string fragment, bool preformatedText)
		{
			this.parser.Initialize(fragment, preformatedText);
			this.Reinitialize();
		}

		void IDisposable.Dispose()
		{
			if (this.parser != null)
			{
				((IDisposable)this.parser).Dispose();
			}
			this.parser = null;
			this.restartConsumer = null;
			this.contextStack = null;
			this.queue = null;
			this.closeList = null;
			this.openList = null;
			this.token = null;
			this.inputToken = null;
			this.tokenBuilder = null;
			GC.SuppressFinalize(this);
		}

		private static HtmlDtd.TagDefinition GetTagDefinition(HtmlTagIndex tagIndex)
		{
			return HtmlDtd.tags[(int)tagIndex];
		}

		private void Reinitialize()
		{
			this.elementStackTop = 0;
			this.contextStackTop = 0;
			this.ignoreInputTag = false;
			this.elementStack[this.elementStackTop++] = HtmlTagIndex._ROOT;
			this.context.TopElement = 0;
			this.context.Type = HtmlDtd.ContextType.Root;
			this.context.TextType = HtmlDtd.ContextTextType.Full;
			this.context.Accept = HtmlDtd.SetId.Null;
			this.context.Reject = HtmlDtd.SetId.Empty;
			this.context.IgnoreEnd = HtmlDtd.SetId.Null;
			this.context.HasSpace = false;
			this.context.EatSpace = false;
			this.context.OneNL = false;
			this.context.LastCh = '\0';
			this.queueHead = 0;
			this.queueTail = 0;
			this.queueStart = 0;
			this.validRTC = false;
			this.tagIdRTC = HtmlTagIndex._NULL;
			this.token = null;
			if (this.injection != null)
			{
				if (this.injection.Active)
				{
					this.parser = (HtmlParser)this.injection.Pop();
				}
				this.injection.Reset();
			}
		}

		private void Process(HtmlTokenId tokenId)
		{
			if (tokenId == HtmlTokenId.None)
			{
				this.EnqueueHead(HtmlNormalizingParser.QueueItemKind.None);
				return;
			}
			this.inputToken = this.parser.Token;
			switch (tokenId)
			{
			case HtmlTokenId.EndOfFile:
				this.HandleTokenEof();
				return;
			case HtmlTokenId.Text:
				this.HandleTokenText(this.parser.Token);
				return;
			case HtmlTokenId.EncodingChange:
				this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.PassThrough);
				return;
			case HtmlTokenId.Tag:
				if (this.parser.Token.NameIndex < HtmlNameIndex.Unknown)
				{
					this.HandleTokenSpecialTag(this.parser.Token);
					return;
				}
				this.HandleTokenTag(this.parser.Token);
				return;
			case HtmlTokenId.Restart:
				this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.PassThrough);
				return;
			default:
				return;
			}
		}

		private void HandleTokenEof()
		{
			if (this.queueHead != this.queueTail && this.queue[this.queueHead].Kind == HtmlNormalizingParser.QueueItemKind.Suspend)
			{
				HtmlNormalizingParser.QueueItem queueItem = this.DoDequeueFirst();
				this.EnqueueHead(HtmlNormalizingParser.QueueItemKind.EndLastTag, queueItem.TagIndex, 0 != (byte)(queueItem.Flags & HtmlNormalizingParser.QueueItemFlags.AllowWspLeft), 0 != (byte)(queueItem.Flags & HtmlNormalizingParser.QueueItemFlags.AllowWspRight));
				return;
			}
			if (this.injection == null || !this.injection.Active)
			{
				if (this.injection != null)
				{
					int num = this.FindContainer(HtmlTagIndex.Body, HtmlDtd.SetId.Empty);
					if (num == -1)
					{
						this.CloseAllProhibitedContainers(HtmlNormalizingParser.GetTagDefinition(HtmlTagIndex.Body));
						this.OpenContainer(HtmlTagIndex.Body);
						return;
					}
					this.CloseAllContainers(num + 1);
					if (this.queueHead != this.queueTail)
					{
						return;
					}
					if (this.injection.HaveTail && !this.injection.TailDone)
					{
						this.parser = (HtmlParser)this.injection.Push(false, this.parser);
						this.saveState.Save(this, this.elementStackTop);
						this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.InjectionBegin, 0);
						if (this.injection.HeaderFooterFormat == HeaderFooterFormat.Text)
						{
							this.OpenContainer(HtmlTagIndex.TT);
							this.OpenContainer(HtmlTagIndex.Pre);
						}
						return;
					}
				}
				this.CloseAllContainers();
				this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.Eof);
				return;
			}
			this.CloseAllContainers(this.saveState.SavedStackTop);
			if (this.queueHead != this.queueTail)
			{
				return;
			}
			this.saveState.Restore(this);
			this.EnqueueHead(HtmlNormalizingParser.QueueItemKind.InjectionEnd, this.injection.InjectingHead ? 1 : 0);
			this.parser = (HtmlParser)this.injection.Pop();
		}

		private void HandleTokenTag(HtmlToken tag)
		{
			HtmlTagIndex tagIndex = HtmlNameData.Names[(int)tag.NameIndex].TagIndex;
			if (tag.IsTagBegin)
			{
				this.StartTagProcessing(tagIndex, tag);
				return;
			}
			if (!this.ignoreInputTag)
			{
				if (tag.IsTagEnd)
				{
					this.DoDequeueFirst();
				}
				if (this.inputToken.TagIndex != HtmlTagIndex.Unknown)
				{
					bool flag = HtmlNormalizingParser.GetTagDefinition(tagIndex).Scope == HtmlDtd.TagScope.EMPTY;
					this.inputToken.Flags = (flag ? (this.inputToken.Flags | HtmlToken.TagFlags.EmptyScope) : (this.inputToken.Flags & ~HtmlToken.TagFlags.EmptyScope));
				}
				this.EnqueueHead(HtmlNormalizingParser.QueueItemKind.PassThrough);
				return;
			}
			if (tag.IsTagEnd)
			{
				this.ignoreInputTag = false;
			}
		}

		private void HandleTokenSpecialTag(HtmlToken tag)
		{
			tag.Flags |= HtmlToken.TagFlags.EmptyScope;
			HtmlTagIndex tagIndex = HtmlNameData.Names[(int)tag.NameIndex].TagIndex;
			if (tag.IsTagBegin)
			{
				this.StartSpecialTagProcessing(tagIndex, tag);
				return;
			}
			if (!this.ignoreInputTag)
			{
				if (tag.IsTagEnd)
				{
					this.DoDequeueFirst();
				}
				this.EnqueueHead(HtmlNormalizingParser.QueueItemKind.PassThrough);
				return;
			}
			if (tag.IsTagEnd)
			{
				this.ignoreInputTag = false;
			}
		}

		private void HandleTokenText(HtmlToken token)
		{
			HtmlTagIndex htmlTagIndex = this.validRTC ? this.tagIdRTC : this.RequiredTextContainer();
			int num = 0;
			Token.RunEnumerator runs = this.inputToken.Runs;
			if (htmlTagIndex != HtmlTagIndex._NULL)
			{
				while (runs.MoveNext(true))
				{
					TokenRun tokenRun = runs.Current;
					if (tokenRun.TextType > RunTextType.UnusualWhitespace)
					{
						break;
					}
				}
				if (!runs.IsValidPosition)
				{
					return;
				}
				this.CloseAllProhibitedContainers(HtmlNormalizingParser.GetTagDefinition(htmlTagIndex));
				this.OpenContainer(htmlTagIndex);
			}
			else if (this.context.TextType != HtmlDtd.ContextTextType.Literal)
			{
				while (runs.MoveNext(true))
				{
					TokenRun tokenRun2 = runs.Current;
					if (tokenRun2.TextType > RunTextType.UnusualWhitespace)
					{
						break;
					}
					int num2 = num;
					TokenRun tokenRun3 = runs.Current;
					num = num2 + ((tokenRun3.TextType == RunTextType.NewLine) ? 1 : 2);
				}
			}
			if (this.context.TextType == HtmlDtd.ContextTextType.Literal)
			{
				this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.PassThrough);
				return;
			}
			if (this.context.TextType == HtmlDtd.ContextTextType.Full)
			{
				if (num != 0)
				{
					this.AddSpace(num == 1);
				}
				this.currentRun = runs.CurrentIndex;
				this.currentRunOffset = runs.CurrentOffset;
				if (runs.IsValidPosition)
				{
					TokenRun tokenRun4 = runs.Current;
					char firstChar = tokenRun4.FirstChar;
					char lastChar;
					TokenRun tokenRun6;
					do
					{
						TokenRun tokenRun5 = runs.Current;
						lastChar = tokenRun5.LastChar;
						if (!runs.MoveNext(true))
						{
							break;
						}
						tokenRun6 = runs.Current;
					}
					while (tokenRun6.TextType > RunTextType.UnusualWhitespace);
					this.AddNonspace(firstChar, lastChar);
				}
				this.numRuns = runs.CurrentIndex - this.currentRun;
			}
		}

		private void StartTagProcessing(HtmlTagIndex tagIndex, HtmlToken tag)
		{
			if ((this.context.Reject != HtmlDtd.SetId.Null && !HtmlDtd.IsTagInSet(tagIndex, this.context.Reject)) || (this.context.Accept != HtmlDtd.SetId.Null && HtmlDtd.IsTagInSet(tagIndex, this.context.Accept)))
			{
				if (!tag.IsEndTag)
				{
					if (!this.ProcessOpenTag(tagIndex, HtmlNormalizingParser.GetTagDefinition(tagIndex)))
					{
						this.ProcessIgnoredTag(tagIndex, tag);
						return;
					}
				}
				else
				{
					if (this.context.IgnoreEnd != HtmlDtd.SetId.Null && HtmlDtd.IsTagInSet(tagIndex, this.context.IgnoreEnd))
					{
						this.ProcessIgnoredTag(tagIndex, tag);
						return;
					}
					if (!this.ProcessEndTag(tagIndex, HtmlNormalizingParser.GetTagDefinition(tagIndex)))
					{
						this.ProcessIgnoredTag(tagIndex, tag);
						return;
					}
				}
			}
			else if (this.context.Type == HtmlDtd.ContextType.Select && tagIndex == HtmlTagIndex.Select)
			{
				if (!this.ProcessEndTag(tagIndex, HtmlNormalizingParser.GetTagDefinition(tagIndex)))
				{
					this.ProcessIgnoredTag(tagIndex, tag);
					return;
				}
			}
			else
			{
				this.ProcessIgnoredTag(tagIndex, tag);
			}
		}

		private void StartSpecialTagProcessing(HtmlTagIndex tagIndex, HtmlToken tag)
		{
			this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.PassThrough);
			if (!tag.IsTagEnd)
			{
				this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.Suspend, tagIndex, this.allowWspLeft, this.allowWspRight);
			}
		}

		private void ProcessIgnoredTag(HtmlTagIndex tagIndex, HtmlToken tag)
		{
			if (!tag.IsTagEnd)
			{
				this.ignoreInputTag = true;
			}
		}

		private bool ProcessOpenTag(HtmlTagIndex tagIndex, HtmlDtd.TagDefinition tagDef)
		{
			if (!this.PrepareContainer(tagIndex, tagDef))
			{
				return false;
			}
			this.PushElement(tagIndex, true, tagDef);
			return true;
		}

		private bool ProcessEndTag(HtmlTagIndex tagIndex, HtmlDtd.TagDefinition tagDef)
		{
			if (tagIndex == HtmlTagIndex.Unknown)
			{
				this.PushElement(tagIndex, true, tagDef);
				return true;
			}
			bool flag = true;
			bool flag2 = false;
			int num;
			if (tagDef.Match != HtmlDtd.SetId.Null)
			{
				num = this.FindContainer(tagDef.Match, tagDef.EndContainers);
				if (num >= 0 && this.elementStack[num] != tagIndex)
				{
					flag = false;
				}
			}
			else
			{
				num = this.FindContainer(tagIndex, tagDef.EndContainers);
			}
			if (num < 0)
			{
				HtmlTagIndex unmatchedSubstitute = tagDef.UnmatchedSubstitute;
				if (unmatchedSubstitute == HtmlTagIndex._NULL)
				{
					return false;
				}
				if (unmatchedSubstitute == HtmlTagIndex._IMPLICIT_BEGIN)
				{
					if (!this.PrepareContainer(tagIndex, tagDef))
					{
						return false;
					}
					HtmlToken htmlToken = this.inputToken;
					htmlToken.Flags &= ~HtmlToken.TagFlags.EndTag;
					num = this.PushElement(tagIndex, flag, tagDef);
					flag2 = (flag2 || flag);
					flag = false;
				}
				else
				{
					num = this.FindContainer(unmatchedSubstitute, HtmlNormalizingParser.GetTagDefinition(unmatchedSubstitute).EndContainers);
					if (num < 0)
					{
						return false;
					}
					flag = false;
				}
			}
			if (num >= 0 && num < this.elementStackTop)
			{
				flag &= this.inputToken.IsEndTag;
				flag2 = (flag2 || flag);
				this.CloseContainer(num, flag);
			}
			return flag2;
		}

		private bool PrepareContainer(HtmlTagIndex tagIndex, HtmlDtd.TagDefinition tagDef)
		{
			if (tagIndex == HtmlTagIndex.Unknown)
			{
				return true;
			}
			if (tagDef.MaskingContainers != HtmlDtd.SetId.Null)
			{
				int num = this.FindContainer(tagDef.MaskingContainers, tagDef.BeginContainers);
				if (num >= 0)
				{
					return false;
				}
			}
			this.CloseAllProhibitedContainers(tagDef);
			HtmlTagIndex htmlTagIndex = HtmlTagIndex._NULL;
			if (tagDef.TextType == HtmlDtd.TagTextType.ALWAYS || (tagDef.TextType == HtmlDtd.TagTextType.QUERY && this.QueryTextlike(tagIndex)))
			{
				htmlTagIndex = (this.validRTC ? this.tagIdRTC : this.RequiredTextContainer());
				if (htmlTagIndex != HtmlTagIndex._NULL)
				{
					this.CloseAllProhibitedContainers(HtmlNormalizingParser.GetTagDefinition(htmlTagIndex));
					if (-1 == this.OpenContainer(htmlTagIndex))
					{
						return false;
					}
				}
			}
			if (htmlTagIndex == HtmlTagIndex._NULL && tagDef.RequiredContainers != HtmlDtd.SetId.Null)
			{
				int num2 = this.FindContainer(tagDef.RequiredContainers, tagDef.BeginContainers);
				if (num2 < 0)
				{
					this.CloseAllProhibitedContainers(HtmlNormalizingParser.GetTagDefinition(tagDef.DefaultContainer));
					if (-1 == this.OpenContainer(tagDef.DefaultContainer))
					{
						return false;
					}
				}
			}
			return true;
		}

		private int OpenContainer(HtmlTagIndex tagIndex)
		{
			int num = 0;
			while (tagIndex != HtmlTagIndex._NULL)
			{
				this.openList[num++] = tagIndex;
				HtmlDtd.TagDefinition tagDefinition = HtmlNormalizingParser.GetTagDefinition(tagIndex);
				if (tagDefinition.RequiredContainers == HtmlDtd.SetId.Null)
				{
					break;
				}
				int num2 = this.FindContainer(tagDefinition.RequiredContainers, tagDefinition.BeginContainers);
				if (num2 >= 0)
				{
					break;
				}
				tagIndex = tagDefinition.DefaultContainer;
			}
			if (tagIndex == HtmlTagIndex._NULL)
			{
				return -1;
			}
			int result = -1;
			for (int i = num - 1; i >= 0; i--)
			{
				tagIndex = this.openList[i];
				result = this.PushElement(tagIndex, false, HtmlNormalizingParser.GetTagDefinition(tagIndex));
			}
			return result;
		}

		private void CloseContainer(int stackPos, bool useInputTag)
		{
			if (stackPos != this.elementStackTop - 1)
			{
				bool flag = false;
				int num = 0;
				this.closeList[num++] = stackPos;
				if (HtmlNormalizingParser.GetTagDefinition(this.elementStack[stackPos]).Scope == HtmlDtd.TagScope.NESTED)
				{
					flag = true;
				}
				for (int i = stackPos + 1; i < this.elementStackTop; i++)
				{
					HtmlDtd.TagDefinition tagDefinition = HtmlNormalizingParser.GetTagDefinition(this.elementStack[i]);
					if (num == this.closeList.Length)
					{
						int[] destinationArray = new int[this.closeList.Length * 2];
						Array.Copy(this.closeList, 0, destinationArray, 0, num);
						this.closeList = destinationArray;
					}
					if (flag && tagDefinition.Scope == HtmlDtd.TagScope.NESTED)
					{
						this.closeList[num++] = i;
					}
					else
					{
						for (int j = 0; j < num; j++)
						{
							if (HtmlDtd.IsTagInSet(this.elementStack[this.closeList[j]], tagDefinition.EndContainers))
							{
								this.closeList[num++] = i;
								flag = (flag || tagDefinition.Scope == HtmlDtd.TagScope.NESTED);
								break;
							}
						}
					}
				}
				for (int k = num - 1; k > 0; k--)
				{
					this.PopElement(this.closeList[k], false);
				}
			}
			this.PopElement(stackPos, useInputTag);
		}

		private void CloseAllProhibitedContainers(HtmlDtd.TagDefinition tagDef)
		{
			HtmlDtd.SetId prohibitedContainers = tagDef.ProhibitedContainers;
			if (prohibitedContainers != HtmlDtd.SetId.Null)
			{
				for (;;)
				{
					int num = this.FindContainer(prohibitedContainers, tagDef.BeginContainers);
					if (num < 0)
					{
						break;
					}
					this.CloseContainer(num, false);
				}
				return;
			}
		}

		private void CloseAllContainers()
		{
			for (int i = this.elementStackTop - 1; i > 0; i--)
			{
				this.CloseContainer(i, false);
			}
		}

		private void CloseAllContainers(int level)
		{
			for (int i = this.elementStackTop - 1; i >= level; i--)
			{
				this.CloseContainer(i, false);
			}
		}

		private int FindContainer(HtmlDtd.SetId matchSet, HtmlDtd.SetId stopSet)
		{
			int num = this.elementStackTop - 1;
			while (num >= 0 && !HtmlDtd.IsTagInSet(this.elementStack[num], matchSet))
			{
				if (HtmlDtd.IsTagInSet(this.elementStack[num], stopSet))
				{
					return -1;
				}
				num--;
			}
			return num;
		}

		private int FindContainer(HtmlTagIndex match, HtmlDtd.SetId stopSet)
		{
			int num = this.elementStackTop - 1;
			while (num >= 0 && this.elementStack[num] != match)
			{
				if (HtmlDtd.IsTagInSet(this.elementStack[num], stopSet))
				{
					return -1;
				}
				num--;
			}
			return num;
		}

		private HtmlTagIndex RequiredTextContainer()
		{
			this.validRTC = true;
			for (int i = this.elementStackTop - 1; i >= 0; i--)
			{
				HtmlDtd.TagDefinition tagDefinition = HtmlNormalizingParser.GetTagDefinition(this.elementStack[i]);
				if (tagDefinition.TextScope == HtmlDtd.TagTextScope.INCLUDE)
				{
					this.tagIdRTC = HtmlTagIndex._NULL;
					return this.tagIdRTC;
				}
				if (tagDefinition.TextScope == HtmlDtd.TagTextScope.EXCLUDE)
				{
					this.tagIdRTC = tagDefinition.TextSubcontainer;
					return this.tagIdRTC;
				}
			}
			this.tagIdRTC = HtmlTagIndex._NULL;
			return this.tagIdRTC;
		}

		private int PushElement(HtmlTagIndex tagIndex, bool useInputTag, HtmlDtd.TagDefinition tagDef)
		{
			int num;
			if (this.ensureHead)
			{
				if (tagIndex == HtmlTagIndex.Body)
				{
					num = this.PushElement(HtmlTagIndex.Head, false, HtmlDtd.tags[52]);
					this.PopElement(num, false);
				}
				else if (tagIndex == HtmlTagIndex.Head)
				{
					this.ensureHead = false;
				}
			}
			if (tagIndex == HtmlTagIndex.Listing)
			{
				tagIndex = HtmlTagIndex.Pre;
				tagDef = HtmlDtd.tags[84];
				useInputTag = false;
			}
			if (tagDef.TextScope != HtmlDtd.TagTextScope.NEUTRAL)
			{
				this.validRTC = false;
			}
			if (this.elementStackTop == this.elementStack.Length && !this.EnsureElementStackSpace())
			{
				throw new TextConvertersException(TextConvertersStrings.HtmlNestingTooDeep);
			}
			bool flag = tagDef.Scope == HtmlDtd.TagScope.EMPTY;
			if (useInputTag)
			{
				if (this.inputToken.TagIndex != HtmlTagIndex.Unknown)
				{
					this.inputToken.Flags = (flag ? (this.inputToken.Flags | HtmlToken.TagFlags.EmptyScope) : (this.inputToken.Flags & ~HtmlToken.TagFlags.EmptyScope));
				}
				else
				{
					flag = true;
				}
			}
			num = this.elementStackTop++;
			this.elementStack[num] = tagIndex;
			this.LFillTagB(tagDef);
			this.EnqueueTail(useInputTag ? HtmlNormalizingParser.QueueItemKind.PassThrough : HtmlNormalizingParser.QueueItemKind.BeginElement, tagIndex, this.allowWspLeft, this.allowWspRight);
			if (useInputTag && !this.inputToken.IsTagEnd)
			{
				this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.Suspend, tagIndex, this.allowWspLeft, this.allowWspRight);
			}
			this.RFillTagB(tagDef);
			if (!flag)
			{
				if (tagDef.ContextType != HtmlDtd.ContextType.None)
				{
					if (this.contextStackTop == this.contextStack.Length)
					{
						this.EnsureContextStackSpace();
					}
					this.contextStack[this.contextStackTop++] = this.context;
					this.context.TopElement = num;
					this.context.Type = tagDef.ContextType;
					this.context.TextType = tagDef.ContextTextType;
					this.context.Accept = tagDef.Accept;
					this.context.Reject = tagDef.Reject;
					this.context.IgnoreEnd = tagDef.IgnoreEnd;
					this.context.HasSpace = false;
					this.context.EatSpace = false;
					this.context.OneNL = false;
					this.context.LastCh = '\0';
					if (this.context.TextType != HtmlDtd.ContextTextType.Full)
					{
						this.allowWspLeft = false;
						this.allowWspRight = false;
					}
					this.RFillTagB(tagDef);
				}
			}
			else
			{
				this.elementStackTop--;
			}
			return num;
		}

		private void PopElement(int stackPos, bool useInputTag)
		{
			HtmlTagIndex tagIndex = this.elementStack[stackPos];
			HtmlDtd.TagDefinition tagDefinition = HtmlNormalizingParser.GetTagDefinition(tagIndex);
			if (tagDefinition.TextScope != HtmlDtd.TagTextScope.NEUTRAL)
			{
				this.validRTC = false;
			}
			if (stackPos == this.context.TopElement)
			{
				if (this.context.TextType == HtmlDtd.ContextTextType.Full)
				{
					this.LFillTagE(tagDefinition);
				}
				this.context = this.contextStack[--this.contextStackTop];
			}
			this.LFillTagE(tagDefinition);
			if (stackPos != this.elementStackTop - 1)
			{
				this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.OverlappedClose, this.elementStackTop - stackPos - 1);
			}
			this.EnqueueTail(useInputTag ? HtmlNormalizingParser.QueueItemKind.PassThrough : HtmlNormalizingParser.QueueItemKind.EndElement, tagIndex, this.allowWspLeft, this.allowWspRight);
			if (useInputTag && !this.inputToken.IsTagEnd)
			{
				this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.Suspend, tagIndex, this.allowWspLeft, this.allowWspRight);
			}
			this.RFillTagE(tagDefinition);
			if (stackPos != this.elementStackTop - 1)
			{
				this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.OverlappedReopen, this.elementStackTop - stackPos - 1);
				Array.Copy(this.elementStack, stackPos + 1, this.elementStack, stackPos, this.elementStackTop - stackPos - 1);
				if (this.context.TopElement > stackPos)
				{
					this.context.TopElement = this.context.TopElement - 1;
					int num = this.contextStackTop - 1;
					while (num > 0 && this.contextStack[num].TopElement >= stackPos)
					{
						HtmlNormalizingParser.Context[] array = this.contextStack;
						int num2 = num;
						array[num2].TopElement = array[num2].TopElement - 1;
						num--;
					}
				}
			}
			this.elementStackTop--;
		}

		private void AddNonspace(char firstChar, char lastChar)
		{
			if (this.context.HasSpace)
			{
				this.context.HasSpace = false;
				if (this.context.LastCh == '\0' || !this.context.OneNL || !ParseSupport.TwoFarEastNonHanguelChars(this.context.LastCh, firstChar))
				{
					this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.Space);
				}
			}
			this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.Text);
			this.context.EatSpace = false;
			this.context.LastCh = lastChar;
			this.context.OneNL = false;
		}

		private void AddSpace(bool oneNL)
		{
			if (!this.context.EatSpace)
			{
				this.context.HasSpace = true;
			}
			if (this.context.LastCh != '\0')
			{
				if (oneNL && !this.context.OneNL)
				{
					this.context.OneNL = true;
					return;
				}
				this.context.LastCh = '\0';
			}
		}

		private bool QueryTextlike(HtmlTagIndex tagIndex)
		{
			HtmlDtd.ContextType type = this.context.Type;
			int num = this.contextStackTop;
			while (num != 0)
			{
				switch (type)
				{
				case HtmlDtd.ContextType.Head:
					if (tagIndex == HtmlTagIndex.Object)
					{
						return false;
					}
					break;
				case HtmlDtd.ContextType.Body:
					if (tagIndex <= HtmlTagIndex.Div)
					{
						if (tagIndex != HtmlTagIndex.A && tagIndex != HtmlTagIndex.Applet && tagIndex != HtmlTagIndex.Div)
						{
							return false;
						}
					}
					else if (tagIndex != HtmlTagIndex.Input && tagIndex != HtmlTagIndex.Object && tagIndex != HtmlTagIndex.Span)
					{
						return false;
					}
					return true;
				}
				type = this.contextStack[--num].Type;
			}
			return tagIndex == HtmlTagIndex.Object || tagIndex == HtmlTagIndex.Applet;
		}

		private void LFillTagB(HtmlDtd.TagDefinition tagDef)
		{
			if (this.context.TextType == HtmlDtd.ContextTextType.Full)
			{
				this.LFill(this.FillCodeFromTag(tagDef).LB, this.FillCodeFromTag(tagDef).RB);
			}
		}

		private void RFillTagB(HtmlDtd.TagDefinition tagDef)
		{
			if (this.context.TextType == HtmlDtd.ContextTextType.Full)
			{
				this.RFill(this.FillCodeFromTag(tagDef).RB);
			}
		}

		private void LFillTagE(HtmlDtd.TagDefinition tagDef)
		{
			if (this.context.TextType == HtmlDtd.ContextTextType.Full)
			{
				this.LFill(this.FillCodeFromTag(tagDef).LE, this.FillCodeFromTag(tagDef).RE);
			}
		}

		private void RFillTagE(HtmlDtd.TagDefinition tagDef)
		{
			if (this.context.TextType == HtmlDtd.ContextTextType.Full)
			{
				this.RFill(this.FillCodeFromTag(tagDef).RE);
			}
		}

		private void LFill(HtmlDtd.FillCode codeLeft, HtmlDtd.FillCode codeRight)
		{
			this.allowWspLeft = (this.context.HasSpace || codeLeft == HtmlDtd.FillCode.EAT);
			this.context.LastCh = '\0';
			if (this.context.HasSpace)
			{
				if (codeLeft == HtmlDtd.FillCode.PUT)
				{
					this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.Space);
					this.context.EatSpace = true;
				}
				this.context.HasSpace = (codeLeft == HtmlDtd.FillCode.NUL);
			}
			this.allowWspRight = (this.context.HasSpace || codeRight == HtmlDtd.FillCode.EAT);
		}

		private void RFill(HtmlDtd.FillCode code)
		{
			if (code == HtmlDtd.FillCode.EAT)
			{
				this.context.HasSpace = false;
				this.context.EatSpace = true;
				return;
			}
			if (code == HtmlDtd.FillCode.PUT)
			{
				this.context.EatSpace = false;
			}
		}

		private bool QueueEmpty()
		{
			return this.queueHead == this.queueTail || this.queue[this.queueHead].Kind == HtmlNormalizingParser.QueueItemKind.Suspend;
		}

		private HtmlNormalizingParser.QueueItemKind QueueHeadKind()
		{
			if (this.queueHead == this.queueTail)
			{
				return HtmlNormalizingParser.QueueItemKind.Empty;
			}
			return this.queue[this.queueHead].Kind;
		}

		private void EnqueueTail(HtmlNormalizingParser.QueueItemKind kind, HtmlTagIndex tagIndex, bool allowWspLeft, bool allowWspRight)
		{
			if (this.queueTail == this.queue.Length)
			{
				this.ExpandQueue();
			}
			this.queue[this.queueTail].Kind = kind;
			this.queue[this.queueTail].TagIndex = tagIndex;
			this.queue[this.queueTail].Flags = (HtmlNormalizingParser.QueueItemFlags)((allowWspLeft ? 1 : 0) | (allowWspRight ? 2 : 0));
			this.queue[this.queueTail].Argument = 0;
			this.queueTail++;
		}

		private void EnqueueTail(HtmlNormalizingParser.QueueItemKind kind, int argument)
		{
			if (this.queueTail == this.queue.Length)
			{
				this.ExpandQueue();
			}
			this.queue[this.queueTail].Kind = kind;
			this.queue[this.queueTail].TagIndex = HtmlTagIndex._NULL;
			this.queue[this.queueTail].Flags = (HtmlNormalizingParser.QueueItemFlags)0;
			this.queue[this.queueTail].Argument = argument;
			this.queueTail++;
		}

		private void EnqueueTail(HtmlNormalizingParser.QueueItemKind kind)
		{
			if (this.queueTail == this.queue.Length)
			{
				this.ExpandQueue();
			}
			this.queue[this.queueTail].Kind = kind;
			this.queue[this.queueTail].TagIndex = HtmlTagIndex._NULL;
			this.queue[this.queueTail].Flags = (HtmlNormalizingParser.QueueItemFlags)0;
			this.queue[this.queueTail].Argument = 0;
			this.queueTail++;
		}

		private void EnqueueHead(HtmlNormalizingParser.QueueItemKind kind, HtmlTagIndex tagIndex, bool allowWspLeft, bool allowWspRight)
		{
			if (this.queueHead != this.queueStart)
			{
				this.queueHead--;
			}
			else
			{
				this.queueTail++;
			}
			this.queue[this.queueHead].Kind = kind;
			this.queue[this.queueHead].TagIndex = tagIndex;
			this.queue[this.queueHead].Flags = (HtmlNormalizingParser.QueueItemFlags)((allowWspLeft ? 1 : 0) | (allowWspRight ? 2 : 0));
			this.queue[this.queueHead].Argument = 0;
		}

		private void EnqueueHead(HtmlNormalizingParser.QueueItemKind kind)
		{
			this.EnqueueHead(kind, 0);
		}

		private void EnqueueHead(HtmlNormalizingParser.QueueItemKind kind, int argument)
		{
			if (this.queueHead != this.queueStart)
			{
				this.queueHead--;
			}
			else
			{
				this.queueTail++;
			}
			this.queue[this.queueHead].Kind = kind;
			this.queue[this.queueHead].TagIndex = HtmlTagIndex._NULL;
			this.queue[this.queueHead].Flags = (HtmlNormalizingParser.QueueItemFlags)0;
			this.queue[this.queueHead].Argument = argument;
		}

		private HtmlTokenId GetTokenFromQueue()
		{
			switch (this.QueueHeadKind())
			{
			case HtmlNormalizingParser.QueueItemKind.None:
				this.DoDequeueFirst();
				this.token = null;
				return HtmlTokenId.None;
			case HtmlNormalizingParser.QueueItemKind.Eof:
				this.tokenBuilder.BuildEofToken();
				this.token = this.tokenBuilder;
				break;
			case HtmlNormalizingParser.QueueItemKind.BeginElement:
			case HtmlNormalizingParser.QueueItemKind.EndElement:
			{
				HtmlNormalizingParser.QueueItem queueItem = this.DoDequeueFirst();
				this.tokenBuilder.BuildTagToken(queueItem.TagIndex, queueItem.Kind == HtmlNormalizingParser.QueueItemKind.EndElement, (byte)(queueItem.Flags & HtmlNormalizingParser.QueueItemFlags.AllowWspLeft) == 1, (byte)(queueItem.Flags & HtmlNormalizingParser.QueueItemFlags.AllowWspRight) == 2, false);
				this.token = this.tokenBuilder;
				if (queueItem.Kind == HtmlNormalizingParser.QueueItemKind.BeginElement && this.token.OriginalTagId == HtmlTagIndex.Body && this.injection != null && this.injection.HaveHead && !this.injection.HeadDone)
				{
					int num = this.FindContainer(HtmlTagIndex.Body, HtmlDtd.SetId.Empty);
					this.parser = (HtmlParser)this.injection.Push(true, this.parser);
					this.saveState.Save(this, num + 1);
					this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.InjectionBegin, 1);
					if (this.injection.HeaderFooterFormat == HeaderFooterFormat.Text)
					{
						this.OpenContainer(HtmlTagIndex.TT);
						this.OpenContainer(HtmlTagIndex.Pre);
					}
				}
				return this.token.HtmlTokenId;
			}
			case HtmlNormalizingParser.QueueItemKind.OverlappedClose:
			case HtmlNormalizingParser.QueueItemKind.OverlappedReopen:
			{
				HtmlNormalizingParser.QueueItem queueItem = this.DoDequeueFirst();
				this.tokenBuilder.BuildOverlappedToken(queueItem.Kind == HtmlNormalizingParser.QueueItemKind.OverlappedClose, queueItem.Argument);
				this.token = this.tokenBuilder;
				return this.token.HtmlTokenId;
			}
			case HtmlNormalizingParser.QueueItemKind.PassThrough:
			{
				HtmlNormalizingParser.QueueItem queueItem = this.DoDequeueFirst();
				this.token = this.inputToken;
				if (this.token.HtmlTokenId == HtmlTokenId.Tag)
				{
					HtmlToken htmlToken = this.token;
					htmlToken.Flags |= (HtmlToken.TagFlags)((((byte)(queueItem.Flags & HtmlNormalizingParser.QueueItemFlags.AllowWspLeft) == 1) ? 64 : 0) | (((byte)(queueItem.Flags & HtmlNormalizingParser.QueueItemFlags.AllowWspRight) == 2) ? 128 : 0));
					if (this.token.OriginalTagId == HtmlTagIndex.Body && this.token.IsTagEnd && this.injection != null && this.injection.HaveHead && !this.injection.HeadDone)
					{
						int num2 = this.FindContainer(HtmlTagIndex.Body, HtmlDtd.SetId.Empty);
						this.parser = (HtmlParser)this.injection.Push(true, this.parser);
						this.saveState.Save(this, num2 + 1);
						this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.InjectionBegin, 1);
						if (this.injection.HeaderFooterFormat == HeaderFooterFormat.Text)
						{
							this.OpenContainer(HtmlTagIndex.TT);
							this.OpenContainer(HtmlTagIndex.Pre);
						}
					}
				}
				return this.token.HtmlTokenId;
			}
			case HtmlNormalizingParser.QueueItemKind.Space:
			{
				HtmlNormalizingParser.QueueItem queueItem = this.DoDequeueFirst();
				this.tokenBuilder.BuildSpaceToken();
				this.token = this.tokenBuilder;
				return this.token.HtmlTokenId;
			}
			case HtmlNormalizingParser.QueueItemKind.Text:
			{
				bool flag = false;
				int argument = 0;
				HtmlNormalizingParser.QueueItem queueItem = this.DoDequeueFirst();
				if (this.queueHead != this.queueTail)
				{
					flag = true;
					argument = this.queue[this.queueHead].Argument;
					this.DoDequeueFirst();
				}
				this.tokenBuilder.BuildTextSliceToken(this.inputToken, this.currentRun, this.currentRunOffset, this.numRuns);
				this.token = this.tokenBuilder;
				Token.RunEnumerator runs = this.inputToken.Runs;
				if (runs.IsValidPosition)
				{
					int num3 = 0;
					TokenRun tokenRun2;
					do
					{
						int num4 = num3;
						TokenRun tokenRun = runs.Current;
						num3 = num4 + ((tokenRun.TextType == RunTextType.NewLine) ? 1 : 2);
						if (!runs.MoveNext(true))
						{
							break;
						}
						tokenRun2 = runs.Current;
					}
					while (tokenRun2.TextType <= RunTextType.UnusualWhitespace);
					if (num3 != 0)
					{
						this.AddSpace(num3 == 1);
					}
					this.currentRun = runs.CurrentIndex;
					this.currentRunOffset = runs.CurrentOffset;
					if (runs.IsValidPosition)
					{
						TokenRun tokenRun3 = runs.Current;
						char firstChar = tokenRun3.FirstChar;
						char lastChar;
						TokenRun tokenRun5;
						do
						{
							TokenRun tokenRun4 = runs.Current;
							lastChar = tokenRun4.LastChar;
							if (!runs.MoveNext(true))
							{
								break;
							}
							tokenRun5 = runs.Current;
						}
						while (tokenRun5.TextType > RunTextType.UnusualWhitespace);
						this.AddNonspace(firstChar, lastChar);
					}
					this.numRuns = runs.CurrentIndex - this.currentRun;
				}
				else
				{
					this.currentRun = runs.CurrentIndex;
					this.currentRunOffset = runs.CurrentOffset;
					this.numRuns = 0;
				}
				if (flag)
				{
					this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.InjectionEnd, argument);
				}
				return this.token.HtmlTokenId;
			}
			case HtmlNormalizingParser.QueueItemKind.InjectionBegin:
			case HtmlNormalizingParser.QueueItemKind.InjectionEnd:
			{
				HtmlNormalizingParser.QueueItem queueItem = this.DoDequeueFirst();
				this.tokenBuilder.BuildInjectionToken(queueItem.Kind == HtmlNormalizingParser.QueueItemKind.InjectionBegin, queueItem.Argument != 0);
				this.token = this.tokenBuilder;
				break;
			}
			case HtmlNormalizingParser.QueueItemKind.EndLastTag:
			{
				HtmlNormalizingParser.QueueItem queueItem = this.DoDequeueFirst();
				this.tokenBuilder.BuildTagToken(queueItem.TagIndex, false, (byte)(queueItem.Flags & HtmlNormalizingParser.QueueItemFlags.AllowWspLeft) == 1, (byte)(queueItem.Flags & HtmlNormalizingParser.QueueItemFlags.AllowWspRight) == 2, true);
				this.token = this.tokenBuilder;
				if (queueItem.Kind == HtmlNormalizingParser.QueueItemKind.BeginElement && this.token.OriginalTagId == HtmlTagIndex.Body && this.injection != null && this.injection.HaveHead && !this.injection.HeadDone)
				{
					int num5 = this.FindContainer(HtmlTagIndex.Body, HtmlDtd.SetId.Empty);
					this.parser = (HtmlParser)this.injection.Push(true, this.parser);
					this.saveState.Save(this, num5 + 1);
					this.EnqueueTail(HtmlNormalizingParser.QueueItemKind.InjectionBegin, 1);
					if (this.injection.HeaderFooterFormat == HeaderFooterFormat.Text)
					{
						this.OpenContainer(HtmlTagIndex.TT);
						this.OpenContainer(HtmlTagIndex.Pre);
					}
				}
				return this.token.HtmlTokenId;
			}
			}
			return this.token.HtmlTokenId;
		}

		private void ExpandQueue()
		{
			HtmlNormalizingParser.QueueItem[] destinationArray = new HtmlNormalizingParser.QueueItem[this.queue.Length * 2];
			Array.Copy(this.queue, this.queueHead, destinationArray, this.queueHead, this.queueTail - this.queueHead);
			if (this.queueStart != 0)
			{
				Array.Copy(this.queue, 0, destinationArray, 0, this.queueStart);
			}
			this.queue = destinationArray;
		}

		private HtmlNormalizingParser.QueueItem DoDequeueFirst()
		{
			int num = this.queueHead;
			this.queueHead++;
			if (this.queueHead == this.queueTail)
			{
				this.queueHead = (this.queueTail = this.queueStart);
			}
			return this.queue[num];
		}

		private HtmlDtd.TagFill FillCodeFromTag(HtmlDtd.TagDefinition tagDef)
		{
			if (this.context.Type == HtmlDtd.ContextType.Select && tagDef.TagIndex != HtmlTagIndex.Option)
			{
				return HtmlDtd.TagFill.PUT_PUT_PUT_PUT;
			}
			if (this.context.Type == HtmlDtd.ContextType.Title)
			{
				return HtmlDtd.TagFill.NUL_EAT_EAT_NUL;
			}
			return tagDef.Fill;
		}

		private bool EnsureElementStackSpace()
		{
			if (this.elementStackTop == this.elementStack.Length)
			{
				if (this.elementStack.Length >= this.maxElementStack)
				{
					return false;
				}
				int num = (this.maxElementStack / 2 > this.elementStack.Length) ? (this.elementStack.Length * 2) : this.maxElementStack;
				HtmlTagIndex[] destinationArray = new HtmlTagIndex[num];
				Array.Copy(this.elementStack, 0, destinationArray, 0, this.elementStackTop);
				this.elementStack = destinationArray;
			}
			return true;
		}

		private void EnsureContextStackSpace()
		{
			if (this.contextStackTop + 1 > this.contextStack.Length)
			{
				HtmlNormalizingParser.Context[] destinationArray = new HtmlNormalizingParser.Context[this.contextStack.Length * 2];
				Array.Copy(this.contextStack, 0, destinationArray, 0, this.contextStackTop);
				this.contextStack = destinationArray;
			}
		}

		private HtmlParser parser;

		private IRestartable restartConsumer;

		private int maxElementStack;

		private HtmlNormalizingParser.Context context;

		private HtmlNormalizingParser.Context[] contextStack;

		private int contextStackTop;

		private HtmlTagIndex[] elementStack;

		private int elementStackTop;

		private HtmlNormalizingParser.QueueItem[] queue;

		private int queueHead;

		private int queueTail;

		private int queueStart;

		private bool ensureHead = true;

		private int[] closeList;

		private HtmlTagIndex[] openList;

		private bool validRTC;

		private HtmlTagIndex tagIdRTC;

		private HtmlToken token;

		private HtmlToken inputToken;

		private bool ignoreInputTag;

		private int currentRun;

		private int currentRunOffset;

		private int numRuns;

		private bool allowWspLeft;

		private bool allowWspRight;

		private HtmlNormalizingParser.SmallTokenBuilder tokenBuilder;

		private HtmlInjection injection;

		private HtmlNormalizingParser.DocumentState saveState;

		private enum QueueItemKind : byte
		{
			Empty,
			None,
			Eof,
			BeginElement,
			EndElement,
			OverlappedClose,
			OverlappedReopen,
			PassThrough,
			Space,
			Text,
			Suspend,
			InjectionBegin,
			InjectionEnd,
			EndLastTag
		}

		[Flags]
		private enum QueueItemFlags : byte
		{
			AllowWspLeft = 1,
			AllowWspRight = 2
		}

		private struct Context
		{
			public int TopElement;

			public HtmlDtd.ContextType Type;

			public HtmlDtd.ContextTextType TextType;

			public HtmlDtd.SetId Accept;

			public HtmlDtd.SetId Reject;

			public HtmlDtd.SetId IgnoreEnd;

			public char LastCh;

			public bool OneNL;

			public bool HasSpace;

			public bool EatSpace;
		}

		private struct QueueItem
		{
			public HtmlNormalizingParser.QueueItemKind Kind;

			public HtmlTagIndex TagIndex;

			public HtmlNormalizingParser.QueueItemFlags Flags;

			public int Argument;
		}

		private class DocumentState
		{
			public int SavedStackTop
			{
				get
				{
					return this.elementStackTop;
				}
			}

			public void Save(HtmlNormalizingParser document, int stackLevel)
			{
				if (stackLevel != document.elementStackTop)
				{
					Array.Copy(document.elementStack, stackLevel, this.savedElementStackEntries, 0, document.elementStackTop - stackLevel);
					this.savedElementStackEntriesCount = document.elementStackTop - stackLevel;
					document.elementStackTop = stackLevel;
				}
				else
				{
					this.savedElementStackEntriesCount = 0;
				}
				this.elementStackTop = document.elementStackTop;
				this.queueHead = document.queueHead;
				this.queueTail = document.queueTail;
				this.inputToken = document.inputToken;
				this.currentRun = document.currentRun;
				this.currentRunOffset = document.currentRunOffset;
				this.numRuns = document.numRuns;
				this.hasSpace = document.context.HasSpace;
				this.eatSpace = document.context.EatSpace;
				this.validRTC = document.validRTC;
				this.tagIdRTC = document.tagIdRTC;
				document.queueStart = document.queueTail;
				document.queueHead = (document.queueTail = document.queueStart);
			}

			public void Restore(HtmlNormalizingParser document)
			{
				if (this.savedElementStackEntriesCount != 0)
				{
					Array.Copy(this.savedElementStackEntries, 0, document.elementStack, document.elementStackTop, this.savedElementStackEntriesCount);
					document.elementStackTop += this.savedElementStackEntriesCount;
				}
				document.queueStart = 0;
				document.queueHead = this.queueHead;
				document.queueTail = this.queueTail;
				document.inputToken = this.inputToken;
				document.currentRun = this.currentRun;
				document.currentRunOffset = this.currentRunOffset;
				document.numRuns = this.numRuns;
				document.context.HasSpace = this.hasSpace;
				document.context.EatSpace = this.eatSpace;
				document.validRTC = this.validRTC;
				document.tagIdRTC = this.tagIdRTC;
			}

			private int queueHead;

			private int queueTail;

			private HtmlToken inputToken;

			private int elementStackTop;

			private int currentRun;

			private int currentRunOffset;

			private int numRuns;

			private HtmlTagIndex[] savedElementStackEntries = new HtmlTagIndex[5];

			private int savedElementStackEntriesCount;

			private bool hasSpace;

			private bool eatSpace;

			private bool validRTC;

			private HtmlTagIndex tagIdRTC;
		}

		private class SmallTokenBuilder : HtmlToken
		{
			public void BuildTagToken(HtmlTagIndex tagIndex, bool closingTag, bool allowWspLeft, bool allowWspRight, bool endOnly)
			{
				base.TokenId = (TokenId)4;
				base.Argument = 1;
				this.Buffer = this.spareBuffer;
				this.RunList = this.spareRuns;
				this.Whole.Reset();
				this.WholePosition.Rewind(this.Whole);
				this.OriginalTagIndex = tagIndex;
				this.TagIndex = tagIndex;
				this.NameIndex = HtmlDtd.tags[(int)tagIndex].NameIndex;
				if (!endOnly)
				{
					this.PartMajor = HtmlToken.TagPartMajor.Complete;
					this.PartMinor = HtmlToken.TagPartMinor.CompleteName;
				}
				else
				{
					this.PartMajor = HtmlToken.TagPartMajor.End;
					this.PartMinor = HtmlToken.TagPartMinor.Empty;
				}
				base.Flags = (HtmlToken.TagFlags)((byte)((closingTag ? 16 : 0) | (allowWspLeft ? 64 : 0)) | (allowWspRight ? 128 : 0));
			}

			public void BuildOverlappedToken(bool close, int argument)
			{
				base.TokenId = (close ? ((TokenId)6) : ((TokenId)7));
				base.Argument = argument;
				this.Buffer = this.spareBuffer;
				this.RunList = this.spareRuns;
				this.Whole.Reset();
				this.WholePosition.Rewind(this.Whole);
			}

			public void BuildInjectionToken(bool begin, bool head)
			{
				base.TokenId = (begin ? ((TokenId)8) : ((TokenId)9));
				base.Argument = (head ? 1 : 0);
				this.Buffer = this.spareBuffer;
				this.RunList = this.spareRuns;
				this.Whole.Reset();
				this.WholePosition.Rewind(this.Whole);
			}

			public void BuildSpaceToken()
			{
				base.TokenId = TokenId.Text;
				base.Argument = 1;
				this.Buffer = this.spareBuffer;
				this.RunList = this.spareRuns;
				this.Buffer[0] = ' ';
				this.RunList[0].Initialize((RunType)2147483648U, RunTextType.Space, 67108864U, 1, 0);
				this.Whole.Reset();
				this.Whole.Tail = 1;
				this.WholePosition.Rewind(this.Whole);
			}

			public void BuildTextSliceToken(Token source, int startRun, int startRunOffset, int numRuns)
			{
				base.TokenId = TokenId.Text;
				base.Argument = 0;
				this.Buffer = source.Buffer;
				this.RunList = source.RunList;
				this.Whole.Initialize(startRun, startRunOffset);
				this.Whole.Tail = this.Whole.Head + numRuns;
				this.WholePosition.Rewind(this.Whole);
			}

			public void BuildEofToken()
			{
				base.TokenId = TokenId.EndOfFile;
				base.Argument = 0;
				this.Buffer = this.spareBuffer;
				this.RunList = this.spareRuns;
				this.Whole.Reset();
				this.WholePosition.Rewind(this.Whole);
			}

			private char[] spareBuffer = new char[1];

			private Token.RunEntry[] spareRuns = new Token.RunEntry[1];
		}
	}
}
