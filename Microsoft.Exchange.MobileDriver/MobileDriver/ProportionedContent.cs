using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.TextMessaging.MobileDriver
{
	internal class ProportionedContent
	{
		public ProportionedContent(IList<ProportionedText> texts)
		{
			if (texts == null)
			{
				throw new ArgumentNullException("texts");
			}
			this.nodes = new List<ProportionedText>(texts);
			this.nodes.RemoveAll((ProportionedText node) => string.IsNullOrEmpty(node.Text));
			List<int> list = new List<int>(this.nodes.Count);
			int num = 0;
			while (this.nodes.Count > num)
			{
				if (ProportionedText.ProportionedTextType.WeightedProportion == this.nodes[num].Type)
				{
					list.Add(num);
					this.TotalWeight += this.nodes[num].Weight;
				}
				num++;
			}
			if (list.Count == 0)
			{
				return;
			}
			foreach (int index in list)
			{
				int num2 = Math.Min(this.nodes[index].LengthKeptAtMost, this.nodes[index].Text.Length) * this.TotalWeight / this.nodes[index].Weight;
				if (this.IdealWeightedTextCapacity < num2)
				{
					this.IdealWeightedTextCapacity = num2;
				}
			}
		}

		public int IdealWeightedTextCapacity { get; set; }

		public int TotalWeight { get; private set; }

		public ProportionedText.PresentationContent GetPresentationContent(int weightedTextCapacity, bool withOptional, bool withGroup)
		{
			List<ProportionedText.PresentationText> list = new List<ProportionedText.PresentationText>(this.nodes.Count);
			int num = 0;
			int num2 = 0;
			while (this.nodes.Count > num2)
			{
				if (withOptional || !this.nodes[num2].IsOptional)
				{
					if (this.nodes[num2].Type == ProportionedText.ProportionedTextType.Delimiter)
					{
						if (!withGroup)
						{
							goto IL_87;
						}
						num++;
					}
					ProportionedText.PresentationText presentationText = this.nodes[num2].ToPresentationText(this.TotalWeight, Math.Min(weightedTextCapacity, this.IdealWeightedTextCapacity), num);
					if (!string.IsNullOrEmpty(presentationText.Text))
					{
						list.Insert(list.Count, presentationText);
					}
				}
				IL_87:
				num2++;
			}
			return new ProportionedText.PresentationContent(list);
		}

		public ProportionedText.PresentationContent GetPresentationContent(int weightedTextCapacity, bool withOptional)
		{
			return this.GetPresentationContent(weightedTextCapacity, withOptional, true);
		}

		private List<ProportionedText> nodes;
	}
}
