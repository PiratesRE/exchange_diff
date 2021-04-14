using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Services.OnlineMeetings.ResourceContract;

namespace Microsoft.Exchange.Services.OnlineMeetings.Autodiscover.DataContract
{
	internal class AutodiscoverResponse
	{
		public AccessLocation AccessLocation { get; set; }

		public ICollection<Link> UserLinks
		{
			get
			{
				return new LinksCollection(this.userLinks);
			}
		}

		public ICollection<Link> RootLinks
		{
			get
			{
				return new LinksCollection(this.rootLinks);
			}
		}

		public static AutodiscoverResponse FromDictionary(Dictionary<string, object> dictionary)
		{
			dictionary = new Dictionary<string, object>(dictionary, StringComparer.OrdinalIgnoreCase);
			AutodiscoverResponse autodiscoverResponse = new AutodiscoverResponse();
			if (dictionary.ContainsKey("AccessLocation"))
			{
				AccessLocation accessLocation = AccessLocation.Internal;
				Enum.TryParse<AccessLocation>(dictionary["AccessLocation"].ToString(), out accessLocation);
				autodiscoverResponse.AccessLocation = accessLocation;
			}
			AutodiscoverResponse.ExtractLinks(dictionary, autodiscoverResponse.rootLinks, "root");
			AutodiscoverResponse.ExtractLinks(dictionary, autodiscoverResponse.userLinks, "user");
			return autodiscoverResponse;
		}

		private static void ExtractLinks(Dictionary<string, object> dictionary, List<Link> linksCollection, string keyToUse)
		{
			if (dictionary.ContainsKey(keyToUse))
			{
				IDictionary dictionary2 = dictionary[keyToUse] as IDictionary;
				if (dictionary2 != null)
				{
					IEnumerable enumerable = dictionary2["Links"] as IEnumerable;
					IDictionary dictionary3 = enumerable as IDictionary;
					if (dictionary3 != null)
					{
						enumerable = new IDictionary[]
						{
							dictionary3
						};
					}
					foreach (object obj in enumerable)
					{
						IDictionary dictionary4 = (IDictionary)obj;
						string href = dictionary4["href"] as string;
						string token = dictionary4["token"] as string;
						Link item = new Link(token, href, string.Empty);
						linksCollection.Add(item);
					}
				}
			}
		}

		private const string AccessLocationString = "AccessLocation";

		private readonly List<Link> userLinks = new List<Link>();

		private readonly List<Link> rootLinks = new List<Link>();
	}
}
