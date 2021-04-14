using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;

namespace Microsoft.Exchange.MessageSecurity.MessageClassifications
{
	internal class ClassificationConfig
	{
		public ClassificationConfig()
		{
			ClassificationConfig.messageClassificationCache = new TenantConfigurationCache<ClassificationConfig.MessageClassificationPerTenantSettings>(ClassificationConfig.cacheSizeInBytes, ClassificationConfig.cacheExpiry, ClassificationConfig.cacheCleanup, null, null);
		}

		public List<ClassificationSummary> GetClassifications(OrganizationId organizationId, CultureInfo locale)
		{
			ClassificationConfig.MessageClassificationPerTenantSettings messageClassificationPerTenantSettings;
			if (organizationId != null && ClassificationConfig.messageClassificationCache.TryGetValue(organizationId, out messageClassificationPerTenantSettings))
			{
				List<ClassificationSummary> list = new List<ClassificationSummary>(messageClassificationPerTenantSettings.Classifications.Count);
				foreach (ClassificationSummary classificationSummary in messageClassificationPerTenantSettings.Classifications)
				{
					if (string.IsNullOrEmpty(classificationSummary.Locale))
					{
						ClassificationSummary classification = this.GetClassification(organizationId, classificationSummary.ClassificationID, locale);
						if (classification != null)
						{
							list.Add(classification);
						}
					}
				}
				return list;
			}
			return ClassificationConfig.EmptyClassificationList;
		}

		public ClassificationSummary GetClassification(OrganizationId organizationId, Guid messageClassificationGuid, CultureInfo locale)
		{
			ClassificationConfig.MessageClassificationPerTenantSettings messageClassificationPerTenantSettings;
			ClassificationSummary result;
			if (organizationId != null && ClassificationConfig.messageClassificationCache.TryGetValue(organizationId, out messageClassificationPerTenantSettings) && messageClassificationPerTenantSettings.TryGetClassification(messageClassificationGuid, locale, out result))
			{
				return result;
			}
			return null;
		}

		public ClassificationSummary Summarize(OrganizationId organizationId, List<string> messageClassificationIds, CultureInfo locale)
		{
			if (messageClassificationIds == null || messageClassificationIds.Count == 0)
			{
				return ClassificationSummary.Empty;
			}
			ClassificationConfig.MessageClassificationPerTenantSettings messageClassificationPerTenantSettings;
			if (organizationId == null || !ClassificationConfig.messageClassificationCache.TryGetValue(organizationId, out messageClassificationPerTenantSettings))
			{
				return ClassificationSummary.Invalid;
			}
			List<ClassificationSummary> list = new List<ClassificationSummary>(messageClassificationIds.Count);
			foreach (string g in messageClassificationIds)
			{
				Guid empty = Guid.Empty;
				ClassificationSummary item;
				if (!GuidHelper.TryParseGuid(g, out empty) || !messageClassificationPerTenantSettings.TryGetClassification(empty, locale, out item))
				{
					return ClassificationSummary.Invalid;
				}
				list.Add(item);
			}
			list.Sort(ClassificationConfig.classificationSummaryComparer);
			string recipientDescription = ClassificationConfig.ConcatDescriptions(list);
			return new ClassificationSummary(list[0])
			{
				RecipientDescription = recipientDescription
			};
		}

		private static string ConcatDescriptions(IEnumerable<ClassificationSummary> sortedClassificationSummaries)
		{
			StringBuilder stringBuilder = new StringBuilder();
			Guid a = Guid.Empty;
			foreach (ClassificationSummary classificationSummary in sortedClassificationSummaries)
			{
				if (a != classificationSummary.ClassificationID && !string.IsNullOrEmpty(classificationSummary.RecipientDescription))
				{
					a = classificationSummary.ClassificationID;
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(" ");
					}
					stringBuilder.Append(classificationSummary.RecipientDescription);
				}
			}
			return stringBuilder.ToString();
		}

		private static readonly List<ClassificationSummary> EmptyClassificationList = new List<ClassificationSummary>(0);

		private static readonly long cacheSizeInBytes = (long)ByteQuantifiedSize.FromMB(1UL).ToBytes();

		private static readonly TimeSpan cacheExpiry = TimeSpan.FromHours(8.0);

		private static readonly TimeSpan cacheCleanup = TimeSpan.FromHours(8.0);

		private static readonly ClassificationConfig.ClassificationSummaryComparer classificationSummaryComparer = new ClassificationConfig.ClassificationSummaryComparer();

		private static TenantConfigurationCache<ClassificationConfig.MessageClassificationPerTenantSettings> messageClassificationCache;

		private class MessageClassificationPerTenantSettings : TenantConfigurationCacheableItem<MessageClassification>
		{
			public override long ItemSize
			{
				get
				{
					return (long)this.itemSize;
				}
			}

			public List<ClassificationSummary> Classifications
			{
				get
				{
					return this.messageClassificationSummaries.Values.ToList<ClassificationSummary>();
				}
			}

			public bool TryGetClassification(Guid messageClassificationId, CultureInfo locale, out ClassificationSummary classification)
			{
				CultureInfo cultureInfo = locale;
				while (cultureInfo != null && !cultureInfo.Equals(CultureInfo.InvariantCulture))
				{
					if (this.messageClassificationSummaries.TryGetValue(cultureInfo.ToString() + messageClassificationId.ToString(), out classification))
					{
						return true;
					}
					cultureInfo = cultureInfo.Parent;
				}
				if (this.messageClassificationSummaries.TryGetValue(messageClassificationId.ToString(), out classification))
				{
					return true;
				}
				classification = null;
				return false;
			}

			public override void ReadData(IConfigurationSession session)
			{
				if (SharedConfiguration.IsDehydratedConfiguration(session))
				{
					session = SharedConfiguration.CreateScopedToSharedConfigADSession(session.SessionSettings.CurrentOrganizationId);
				}
				QueryFilter filter = new NotFilter(new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ClassificationSchema.ClassificationID, ClassificationConfig.MessageClassificationPerTenantSettings.InternetConfidentialGuid),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, "ExInternetConfidential")
				}));
				MessageClassification[] array = session.Find<MessageClassification>(null, QueryScope.SubTree, filter, null, 0);
				if (array == null || array.Length == 0)
				{
					return;
				}
				Dictionary<string, ClassificationSummary> dictionary = new Dictionary<string, ClassificationSummary>(array.Length, StringComparer.OrdinalIgnoreCase);
				CultureInfo[] installedLanguagePackCultures = LanguagePackInfo.GetInstalledLanguagePackCultures(LanguagePackType.Client);
				foreach (MessageClassification classification in array)
				{
					if (installedLanguagePackCultures != null)
					{
						foreach (CultureInfo locale in installedLanguagePackCultures)
						{
							ClassificationSummary classificationSummary = SystemClassificationSummary.GetClassificationSummary(classification, locale);
							if (classificationSummary != null)
							{
								dictionary[classificationSummary.Locale + classificationSummary.ClassificationID.ToString()] = classificationSummary;
							}
						}
					}
				}
				foreach (MessageClassification messageClassification in array)
				{
					ClassificationSummary classificationSummary2 = new ClassificationSummary(messageClassification);
					dictionary[classificationSummary2.Locale + classificationSummary2.ClassificationID.ToString()] = classificationSummary2;
				}
				this.itemSize = 4;
				foreach (KeyValuePair<string, ClassificationSummary> keyValuePair in dictionary)
				{
					this.itemSize += keyValuePair.Key.Length * 2 + keyValuePair.Value.Size;
				}
				Interlocked.Exchange<Dictionary<string, ClassificationSummary>>(ref this.messageClassificationSummaries, dictionary);
			}

			private static readonly Guid InternetConfidentialGuid = new Guid("103a41b0-6d8d-4be5-a866-da3c25d3d679");

			private int itemSize;

			private Dictionary<string, ClassificationSummary> messageClassificationSummaries = new Dictionary<string, ClassificationSummary>();
		}

		private class ClassificationSummaryComparer : IComparer<ClassificationSummary>
		{
			public int Compare(ClassificationSummary x, ClassificationSummary y)
			{
				if (x.DisplayPrecedence < y.DisplayPrecedence)
				{
					return -1;
				}
				if (x.DisplayPrecedence > y.DisplayPrecedence)
				{
					return 1;
				}
				return x.DisplayName.CompareTo(y.DisplayName);
			}
		}
	}
}
