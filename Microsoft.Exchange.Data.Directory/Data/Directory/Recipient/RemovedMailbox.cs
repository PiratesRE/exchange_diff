using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class RemovedMailbox : DeletedRecipient
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return RemovedMailbox.schema;
			}
		}

		public RemovedMailbox()
		{
		}

		internal RemovedMailbox(IRecipientSession session, PropertyBag propertyBag) : base(session, propertyBag)
		{
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new AndFilter(new QueryFilter[]
				{
					base.ImplicitFilter,
					new ExistsFilter(RemovedMailboxSchema.ExchangeGuid)
				});
			}
		}

		internal static object IsPasswordResetRequiredGetter(IPropertyBag propertyBag)
		{
			return null != propertyBag[RemovedMailboxSchema.NetID];
		}

		internal new static object NameGetter(IPropertyBag propertyBag)
		{
			string text = (string)propertyBag[ADObjectSchema.RawName];
			string[] array = text.Split(new char[]
			{
				'\n'
			});
			if (array.Length > 0)
			{
				return array[0];
			}
			return null;
		}

		internal static object NetIDGetter(IPropertyBag propertyBag)
		{
			return RemovedMailbox.GetNetIDWithPrefix("DLTDNETID", propertyBag);
		}

		internal static object ConsumerNetIDGetter(IPropertyBag propertyBag)
		{
			return RemovedMailbox.GetNetIDWithPrefix("DLTDCSID", propertyBag);
		}

		private static NetID GetNetIDWithPrefix(string netIdPrefix, IPropertyBag propertyBag)
		{
			ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)propertyBag[ADRecipientSchema.EmailAddresses];
			if (proxyAddressCollection != null)
			{
				ProxyAddress proxyAddress = proxyAddressCollection.FindPrimary(ProxyAddressPrefix.GetPrefix(netIdPrefix));
				if (null != proxyAddress)
				{
					return NetID.Parse(proxyAddress.ValueString);
				}
			}
			return null;
		}

		internal static SinglePropertyFilter NameFilterBuilderDelegate(SinglePropertyFilter filter)
		{
			if (filter is ExistsFilter)
			{
				return new ExistsFilter(ADObjectSchema.Name);
			}
			if (filter is ComparisonFilter)
			{
				string arg = (string)((ComparisonFilter)filter).PropertyValue;
				string text = string.Format("{0}{1}{2}", arg, '\n', "DEL");
				return new TextFilter(ADObjectSchema.Name, text, MatchOptions.Prefix, MatchFlags.IgnoreCase);
			}
			if (!(filter is TextFilter))
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForPropertyMultiple(filter.Property.Name, filter.GetType(), typeof(ComparisonFilter), typeof(TextFilter)));
			}
			TextFilter textFilter = (TextFilter)filter;
			if (textFilter.MatchOptions == MatchOptions.FullString)
			{
				string text2 = textFilter.Text;
				string text3 = string.Format("{0}{1}{2}", text2, '\n', "DEL");
				return new TextFilter(ADObjectSchema.Name, text3, MatchOptions.Prefix, textFilter.MatchFlags);
			}
			if (textFilter.MatchOptions == MatchOptions.Prefix)
			{
				return new TextFilter(ADObjectSchema.Name, textFilter.Text, MatchOptions.Prefix, textFilter.MatchFlags);
			}
			if (textFilter.MatchOptions == MatchOptions.Suffix)
			{
				string text4 = textFilter.Text;
				string text5 = string.Format("{0}{1}{2}", text4, '\n', "DEL");
				return new TextFilter(ADObjectSchema.Name, text5, MatchOptions.SubString, textFilter.MatchFlags);
			}
			if (textFilter.MatchOptions == MatchOptions.SubString)
			{
				return new TextFilter(ADObjectSchema.Name, textFilter.Text, MatchOptions.SubString, textFilter.MatchFlags);
			}
			throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedMatchOptionsForProperty(filter.Property.Name, textFilter.MatchOptions.ToString()));
		}

		public new string Name
		{
			get
			{
				return (string)this[RemovedMailboxSchema.Name];
			}
		}

		public string RawName
		{
			get
			{
				return (string)this[ADObjectSchema.RawName];
			}
		}

		public ADObjectId PreviousDatabase
		{
			get
			{
				return (ADObjectId)this[RemovedMailboxSchema.PreviousDatabase];
			}
		}

		public ProxyAddressCollection EmailAddresses
		{
			get
			{
				ProxyAddressCollection proxyAddressCollection = (ProxyAddressCollection)this.propertyBag[ADRecipientSchema.EmailAddresses];
				if (proxyAddressCollection != null)
				{
					ProxyAddressCollection proxyAddressCollection2 = new ProxyAddressCollection();
					foreach (ProxyAddress proxyAddress in proxyAddressCollection)
					{
						if (!proxyAddress.PrefixString.Equals("DLTDNETID") && !proxyAddress.PrefixString.Equals("DLTDCSID"))
						{
							proxyAddressCollection2.Add(proxyAddress);
						}
					}
					return proxyAddressCollection2;
				}
				return null;
			}
		}

		public Guid ExchangeGuid
		{
			get
			{
				return (Guid)this[RemovedMailboxSchema.ExchangeGuid];
			}
		}

		public string LegacyExchangeDN
		{
			get
			{
				return (string)this[RemovedMailboxSchema.LegacyExchangeDN];
			}
		}

		public string SamAccountName
		{
			get
			{
				return (string)this[RemovedMailboxSchema.SamAccountName];
			}
		}

		public SmtpAddress WindowsLiveID
		{
			get
			{
				return (SmtpAddress)this[RemovedMailboxSchema.WindowsLiveID];
			}
		}

		public SmtpAddress MicrosoftOnlineServicesID
		{
			get
			{
				return this.WindowsLiveID;
			}
		}

		internal NetID NetID
		{
			get
			{
				return (NetID)this[RemovedMailboxSchema.NetID];
			}
		}

		internal NetID ConsumerNetID
		{
			get
			{
				return (NetID)this[RemovedMailboxSchema.ConsumerNetID];
			}
		}

		public bool IsPasswordResetRequired
		{
			get
			{
				return (bool)this[RemovedMailboxSchema.IsPasswordResetRequired];
			}
		}

		internal bool StoreMailboxExists { get; set; }

		internal const string DELETED_NETID_PREFIX = "DLTDNETID";

		internal const string DELETED_CONSUMERNETID_PREFIX = "DLTDCSID";

		private static readonly RemovedMailboxSchema schema = ObjectSchema.GetInstance<RemovedMailboxSchema>();
	}
}
