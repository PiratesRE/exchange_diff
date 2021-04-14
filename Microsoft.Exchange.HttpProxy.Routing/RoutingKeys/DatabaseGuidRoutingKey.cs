using System;
using Microsoft.Exchange.HttpProxy.Common;

namespace Microsoft.Exchange.HttpProxy.Routing.RoutingKeys
{
	internal class DatabaseGuidRoutingKey : RoutingKeyBase
	{
		public DatabaseGuidRoutingKey(Guid databaseGuid, string domainName) : this(databaseGuid, domainName, null)
		{
		}

		public DatabaseGuidRoutingKey(Guid databaseGuid, string domainName, string resourceForest)
		{
			if (domainName == null)
			{
				throw new ArgumentNullException("domainName");
			}
			this.databaseGuid = databaseGuid;
			this.domainName = domainName;
			this.resourceForest = resourceForest;
		}

		public override RoutingItemType RoutingItemType
		{
			get
			{
				return RoutingItemType.DatabaseGuid;
			}
		}

		public override string Value
		{
			get
			{
				return string.Concat(new object[]
				{
					this.DatabaseGuid,
					'@',
					this.DomainName,
					'@',
					this.ResourceForest
				});
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		public string DomainName
		{
			get
			{
				return this.domainName;
			}
		}

		public string ResourceForest
		{
			get
			{
				return this.resourceForest;
			}
		}

		public static bool TryParse(string value, out DatabaseGuidRoutingKey key)
		{
			key = null;
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			int num = value.IndexOf('@');
			if (num == -1)
			{
				return false;
			}
			string input = value.Substring(0, num);
			Guid guid;
			if (!Guid.TryParse(input, out guid))
			{
				return false;
			}
			string text;
			string text2;
			Utilities.GetTwoSubstrings(value.Substring(num + 1), '@', out text, out text2);
			key = new DatabaseGuidRoutingKey(guid, text, text2);
			return true;
		}

		private const char Separator = '@';

		private readonly Guid databaseGuid;

		private readonly string domainName;

		private readonly string resourceForest;
	}
}
