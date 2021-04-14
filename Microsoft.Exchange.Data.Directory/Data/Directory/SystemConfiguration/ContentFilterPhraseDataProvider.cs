using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ContentFilterPhraseDataProvider : IConfigDataProvider
	{
		public ContentFilterPhraseDataProvider() : this(DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 36, ".ctor", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\MessageHygieneContentFilterPhraseDataProvider.cs"))
		{
		}

		public ContentFilterPhraseDataProvider(IConfigurationSession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			this.session = session;
		}

		public string Source
		{
			get
			{
				if (this.session == null)
				{
					return string.Empty;
				}
				return this.session.Source;
			}
		}

		public IConfigurable Read<T>(ObjectId identity) where T : IConfigurable, new()
		{
			ContentFilterConfig contentFilterConfig = this.session.FindSingletonConfigurationObject<ContentFilterConfig>();
			if (identity != null && contentFilterConfig != null)
			{
				string @string = Encoding.Unicode.GetString(identity.GetBytes());
				foreach (ContentFilterPhrase contentFilterPhrase in contentFilterConfig.GetPhrases())
				{
					if (string.Equals(contentFilterPhrase.Phrase, @string, StringComparison.OrdinalIgnoreCase))
					{
						return contentFilterPhrase;
					}
				}
			}
			return null;
		}

		public IEnumerable<T> FindPaged<T>(QueryFilter filter, ObjectId rootId, bool deepSearch, SortBy sortBy, int pageSize) where T : IConfigurable, new()
		{
			return (IEnumerable<T>)this.Find(filter, rootId, deepSearch, sortBy);
		}

		public IConfigurable[] Find<T>(QueryFilter queryFilter, ObjectId rootId, bool deepSearch, SortBy sortBy) where T : IConfigurable, new()
		{
			return this.Find(queryFilter, rootId, deepSearch, sortBy).ToArray();
		}

		private List<ContentFilterPhrase> Find(QueryFilter queryFilter, ObjectId rootId, bool deepSearch, SortBy sortBy)
		{
			List<ContentFilterPhrase> list = new List<ContentFilterPhrase>();
			ContentFilterPhraseQueryFilter contentFilterPhraseQueryFilter = queryFilter as ContentFilterPhraseQueryFilter;
			ContentFilterConfig contentFilterConfig = this.session.FindSingletonConfigurationObject<ContentFilterConfig>();
			if (contentFilterConfig != null)
			{
				foreach (ContentFilterPhrase contentFilterPhrase in contentFilterConfig.GetPhrases())
				{
					if (contentFilterPhraseQueryFilter != null)
					{
						if (string.Equals(contentFilterPhrase.Phrase, contentFilterPhraseQueryFilter.Phrase, StringComparison.OrdinalIgnoreCase))
						{
							list.Add(contentFilterPhrase);
							break;
						}
					}
					else
					{
						list.Add(contentFilterPhrase);
					}
				}
			}
			return list;
		}

		public void Save(IConfigurable instance)
		{
			ContentFilterPhrase contentFilterPhrase = instance as ContentFilterPhrase;
			ContentFilterConfig contentFilterConfig = this.session.FindSingletonConfigurationObject<ContentFilterConfig>();
			if (contentFilterPhrase != null && contentFilterConfig != null)
			{
				contentFilterConfig.AddPhrase(contentFilterPhrase);
				this.session.Save(contentFilterConfig);
			}
		}

		public void Delete(IConfigurable instance)
		{
			ContentFilterPhrase contentFilterPhrase = instance as ContentFilterPhrase;
			ContentFilterConfig contentFilterConfig = this.session.FindSingletonConfigurationObject<ContentFilterConfig>();
			if (contentFilterPhrase != null && contentFilterConfig != null)
			{
				contentFilterConfig.RemovePhrase(contentFilterPhrase);
				this.session.Save(contentFilterConfig);
			}
		}

		private IConfigurationSession session;
	}
}
