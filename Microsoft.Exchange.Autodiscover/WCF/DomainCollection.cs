using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[CollectionDataContract(Name = "Domains", ItemName = "Domain", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class DomainCollection : Collection<string>
	{
		public DomainCollection()
		{
		}

		public DomainCollection(IEnumerable<string> domains)
		{
			foreach (string item in domains)
			{
				base.Add(item);
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(base.Count * 40);
			foreach (string value in this)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(",");
				}
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}
	}
}
