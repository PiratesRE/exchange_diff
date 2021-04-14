using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.HygieneRules
{
	internal sealed class BifurcationInfo
	{
		public List<string> Recipients
		{
			get
			{
				return this.recipients;
			}
		}

		public List<string> Lists
		{
			get
			{
				return this.lists;
			}
		}

		public List<string> RecipientDomainIs
		{
			get
			{
				return this.recipientDomainIs;
			}
		}

		public bool Exception
		{
			get
			{
				return this.exception;
			}
			set
			{
				this.exception = value;
			}
		}

		public int GetEstimatedSize()
		{
			int result = 18;
			this.AddStringListPropertySize(this.recipients, ref result);
			this.AddStringListPropertySize(this.lists, ref result);
			this.AddStringListPropertySize(this.recipientDomainIs, ref result);
			return result;
		}

		private void AddStringListPropertySize(List<string> property, ref int size)
		{
			if (property != null)
			{
				size += 18;
				foreach (string text in property)
				{
					size += text.Length * 2;
					size += 18;
				}
			}
		}

		private readonly List<string> recipients = new List<string>();

		private readonly List<string> lists = new List<string>();

		private readonly List<string> recipientDomainIs = new List<string>();

		private bool exception;
	}
}
