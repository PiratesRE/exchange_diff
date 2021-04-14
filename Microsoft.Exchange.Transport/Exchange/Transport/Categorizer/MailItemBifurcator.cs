using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class MailItemBifurcator<T> where T : IEquatable<T>, IComparable<T>, new()
	{
		public MailItemBifurcator(TransportMailItem mailItem, IMailBifurcationHelper<T> bifurcationHelper)
		{
			this.originalMailItem = mailItem;
			this.bifurcatedMailItems = new List<TransportMailItem>();
			this.recipientsToBifurcate = new List<RecipientWithBifurcationInfo<T>>();
			this.bifurcationHelper = bifurcationHelper;
		}

		public List<TransportMailItem> GetBifurcatedMailItems()
		{
			if (!this.bifurcationHelper.NeedsBifurcation())
			{
				return this.bifurcatedMailItems;
			}
			this.GetRecipientsToBifurcate();
			if (!this.HasRecipientsToBifurcate())
			{
				return this.bifurcatedMailItems;
			}
			this.SortRecipients();
			this.CreateMailItems();
			return this.bifurcatedMailItems;
		}

		private void CreateMailItems()
		{
			int startRecipientIndex = 0;
			T other = this.recipientsToBifurcate[0].BifurcationInfo;
			for (int i = 1; i < this.recipientsToBifurcate.Count; i++)
			{
				T bifurcationInfo = this.recipientsToBifurcate[i].BifurcationInfo;
				if (!bifurcationInfo.Equals(other))
				{
					int endRecipientIndex = i - 1;
					this.CreateItemForRecipients(startRecipientIndex, endRecipientIndex);
					startRecipientIndex = i;
					other = bifurcationInfo;
				}
			}
			this.CreateItemForRecipients(startRecipientIndex, this.recipientsToBifurcate.Count - 1);
		}

		private void CreateItemForRecipients(int startRecipientIndex, int endRecipientIndex)
		{
			List<MailRecipient> list = new List<MailRecipient>(endRecipientIndex - startRecipientIndex + 1);
			for (int i = startRecipientIndex; i <= endRecipientIndex; i++)
			{
				list.Add(this.recipientsToBifurcate[i].Recipient);
			}
			TransportMailItem item = this.bifurcationHelper.GenerateNewMailItem(list, this.recipientsToBifurcate[startRecipientIndex].BifurcationInfo);
			this.bifurcatedMailItems.Add(item);
		}

		private void GetRecipientsToBifurcate()
		{
			foreach (MailRecipient recipient in this.originalMailItem.Recipients)
			{
				T bifurcationInfo;
				if (this.bifurcationHelper.GetBifurcationInfo(recipient, out bifurcationInfo))
				{
					this.recipientsToBifurcate.Add(new RecipientWithBifurcationInfo<T>(recipient, bifurcationInfo));
				}
			}
		}

		private bool HasRecipientsToBifurcate()
		{
			return this.recipientsToBifurcate.Count != 0;
		}

		private void SortRecipients()
		{
			this.recipientsToBifurcate.Sort(new MailItemBifurcator<T>.RecipientWithBifurcationInfoComparer());
		}

		private IMailBifurcationHelper<T> bifurcationHelper;

		private TransportMailItem originalMailItem;

		private List<TransportMailItem> bifurcatedMailItems;

		private List<RecipientWithBifurcationInfo<T>> recipientsToBifurcate;

		private class RecipientWithBifurcationInfoComparer : IComparer<RecipientWithBifurcationInfo<T>>
		{
			public int Compare(RecipientWithBifurcationInfo<T> recip1, RecipientWithBifurcationInfo<T> recip2)
			{
				T bifurcationInfo = recip1.BifurcationInfo;
				return bifurcationInfo.CompareTo(recip2.BifurcationInfo);
			}
		}
	}
}
