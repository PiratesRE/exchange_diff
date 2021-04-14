using System;
using System.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public class RecipientTypeFilter<T> : IEnumerableFilter<T> where T : IConfigurable, new()
	{
		private RecipientTypeFilter(RecipientType[] recipientTypes)
		{
			if (recipientTypes == null)
			{
				throw new ArgumentNullException("recipientTypes");
			}
			if (recipientTypes.Length == 0)
			{
				throw new ArgumentException("RecipientTypes parameter should never be empty", "recipientTypes");
			}
			this.recipientTypes = new RecipientType[recipientTypes.Length];
			recipientTypes.CopyTo(this.recipientTypes, 0);
			Array.Sort<RecipientType>(this.recipientTypes);
		}

		public static RecipientTypeFilter<T> GetRecipientTypeFilter(RecipientType[] recipientTypes)
		{
			return new RecipientTypeFilter<T>(recipientTypes);
		}

		public bool AcceptElement(T element)
		{
			if (element == null)
			{
				return false;
			}
			IList list = this.recipientTypes;
			ADRecipient adrecipient = element as ADRecipient;
			if (adrecipient != null)
			{
				return list.Contains(adrecipient.RecipientType);
			}
			ReducedRecipient reducedRecipient = element as ReducedRecipient;
			return reducedRecipient != null && list.Contains(reducedRecipient.RecipientType);
		}

		public override bool Equals(object obj)
		{
			RecipientTypeFilter<T> recipientTypeFilter = obj as RecipientTypeFilter<T>;
			if (recipientTypeFilter == null)
			{
				return false;
			}
			if (this.recipientTypes.Length != recipientTypeFilter.recipientTypes.Length)
			{
				return false;
			}
			for (int i = 0; i < this.recipientTypes.Length; i++)
			{
				if (this.recipientTypes[i] != recipientTypeFilter.recipientTypes[i])
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			if (this.hashCode == 0)
			{
				this.hashCode = typeof(RecipientTypeFilter<T>).GetHashCode();
				foreach (RecipientType recipientType in this.recipientTypes)
				{
					this.hashCode = (int)(this.hashCode + recipientType);
				}
			}
			return this.hashCode;
		}

		private int hashCode;

		private RecipientType[] recipientTypes;
	}
}
