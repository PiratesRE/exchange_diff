using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PersonPropertyAggregationContext : PropertyAggregationContext
	{
		public PersonPropertyAggregationContext(IList<IStorePropertyBag> sources, ContactFolders contactFolders, string clientInfoString) : base(sources)
		{
			this.contactFolders = contactFolders;
			this.clientInfoString = clientInfoString;
		}

		public ContactFolders ContactFolders
		{
			get
			{
				return this.contactFolders;
			}
		}

		public string ClientInfoString
		{
			get
			{
				return this.clientInfoString;
			}
		}

		private readonly ContactFolders contactFolders;

		private readonly string clientInfoString;
	}
}
