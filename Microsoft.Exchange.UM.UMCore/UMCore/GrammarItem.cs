using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class GrammarItem : GrammarItemBase
	{
		internal GrammarItem(string item, CultureInfo transcriptionLanguage) : this(item, string.Empty, transcriptionLanguage)
		{
		}

		internal GrammarItem(string item, string tag, CultureInfo transcriptionLanguage) : this(item, tag, transcriptionLanguage, 1f)
		{
		}

		internal GrammarItem(string item, string tag, CultureInfo transcriptionLanguage, float weight) : base(weight)
		{
			this.item = SpeechUtils.SrgsEncode(item);
			this.tag = SpeechUtils.SrgsEncode(tag);
			if (string.IsNullOrEmpty(Utils.TrimSpaces(this.item)))
			{
				this.item = string.Empty;
			}
			if (string.IsNullOrEmpty(Utils.TrimSpaces(this.tag)))
			{
				this.tag = string.Empty;
			}
			this.transcriptionLanguage = transcriptionLanguage;
		}

		public override bool IsEmpty
		{
			get
			{
				return string.IsNullOrEmpty(this.item);
			}
		}

		public override bool Equals(GrammarItemBase otherItemBase)
		{
			GrammarItem grammarItem = otherItemBase as GrammarItem;
			return grammarItem != null && string.Compare(grammarItem.item, this.item, true, this.transcriptionLanguage) == 0 && string.Equals(grammarItem.tag, this.tag, StringComparison.InvariantCultureIgnoreCase);
		}

		protected override string GetInnerItem()
		{
			ExAssert.RetailAssert(!this.IsEmpty, "We should never be trying to get the XML for an empty grammar item");
			string str;
			if (string.IsNullOrEmpty(this.tag))
			{
				str = string.Format(CultureInfo.InvariantCulture, "\r\n                <item>{0}</item>", new object[]
				{
					this.item
				});
			}
			else
			{
				str = string.Format(CultureInfo.InvariantCulture, "\r\n                <item>{0}</item>\r\n                <tag>{1}</tag>", new object[]
				{
					this.item,
					this.tag
				});
			}
			return str + GrammarItem.customGrammarTrueTag;
		}

		protected override int InternalGetHashCode()
		{
			return this.item.GetHashCode() ^ this.tag.GetHashCode();
		}

		private static readonly string customGrammarTrueTag = string.Format(CultureInfo.InvariantCulture, "\r\n                <tag>out.{0} = {1};</tag>", new object[]
		{
			"customGrammarWords",
			"true"
		});

		private string item;

		private string tag;

		private CultureInfo transcriptionLanguage;
	}
}
