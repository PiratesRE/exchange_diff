using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Search.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LocalizedKeywordMapping
	{
		private LocalizedKeywordMapping(CultureInfo culture)
		{
			this.propertyKeywordMapping = LocalizedKeywordMapping.InitializeKeywords<PropertyKeyword>(culture, LocalizedKeywordMapping.propertyKeywordToLocStringMapping, LocalizedKeywordMapping.propertyKeywordToCanonicalStringMapping);
			this.kindKeywordMapping = LocalizedKeywordMapping.InitializeKeywords<KindKeyword>(culture, LocalizedKeywordMapping.kindKeywordToLocStringMapping, LocalizedKeywordMapping.kindKeywordToCanonicalStringMapping);
			this.importanceKeywordMapping = LocalizedKeywordMapping.InitializeKeywords<Importance>(culture, LocalizedKeywordMapping.importanceToLocStringMapping, LocalizedKeywordMapping.importanceToCanonicalStringMapping);
		}

		public bool TryGetPropertyKeyword(string name, out PropertyKeyword propertyKeyword)
		{
			if (this.propertyKeywordMapping.TryGetValue(name, out propertyKeyword))
			{
				return true;
			}
			propertyKeyword = PropertyKeyword.All;
			return false;
		}

		public bool TryGetKindKeyword(string value, out KindKeyword kindKeyword)
		{
			if (this.kindKeywordMapping.TryGetValue(value, out kindKeyword))
			{
				return true;
			}
			kindKeyword = KindKeyword.email;
			return false;
		}

		public bool TryGetImportance(string value, out Importance importance)
		{
			int num;
			if (int.TryParse(value, out num) && num >= 0 && num <= 2)
			{
				importance = (Importance)num;
				return true;
			}
			return this.importanceKeywordMapping.TryGetValue(value, out importance);
		}

		private static Dictionary<string, TKeyword> InitializeKeywords<TKeyword>(CultureInfo culture, List<KeyValuePair<TKeyword, LocalizedString>> keywordToLocstringMap, List<KeyValuePair<TKeyword, string>> keywordToCanonicalStringMap)
		{
			Dictionary<string, TKeyword> dictionary = new Dictionary<string, TKeyword>(keywordToLocstringMap.Count + keywordToCanonicalStringMap.Count, StringComparer.Create(culture, true));
			foreach (KeyValuePair<TKeyword, LocalizedString> keyValuePair in keywordToLocstringMap)
			{
				string value = keyValuePair.Value.ToString(culture);
				if (!string.IsNullOrEmpty(value))
				{
					LocalizedKeywordMapping.AddKeywordToDictionary<TKeyword>(culture, dictionary, keyValuePair.Key, value);
				}
				else if (!culture.TwoLetterISOLanguageName.Equals("en", StringComparison.OrdinalIgnoreCase) && !culture.TwoLetterISOLanguageName.Equals("iv", StringComparison.OrdinalIgnoreCase))
				{
					throw new ArgumentNullException(string.Format("LocalizedString for keyword '{0}' for resource '{1}' in culture '{2}' is null.", keyValuePair.Key, keyValuePair.Value.FullId, culture.Name));
				}
			}
			foreach (KeyValuePair<TKeyword, string> keyValuePair2 in keywordToCanonicalStringMap)
			{
				LocalizedKeywordMapping.AddKeywordToDictionary<TKeyword>(culture, dictionary, keyValuePair2.Key, keyValuePair2.Value);
			}
			return dictionary;
		}

		private static void AddKeywordToDictionary<TKeyword>(CultureInfo culture, Dictionary<string, TKeyword> target, TKeyword keyword, string value)
		{
			TKeyword tkeyword;
			if (target.TryGetValue(value, out tkeyword))
			{
				ExAssert.RetailAssert(tkeyword.Equals(keyword), "Keyword name conflict for {0}, keyword: {1}, existing mapping: {2}, conflicting mapping: {3}", new object[]
				{
					culture.Name,
					value,
					tkeyword,
					keyword
				});
				return;
			}
			target.Add(value, keyword);
		}

		public static LocalizedKeywordMapping GetMapping(CultureInfo culture)
		{
			LocalizedKeywordMapping localizedKeywordMapping;
			lock (LocalizedKeywordMapping.keywordCacheLock)
			{
				if (LocalizedKeywordMapping.mapsByCultures.TryGetValue(culture, out localizedKeywordMapping))
				{
					return localizedKeywordMapping;
				}
			}
			localizedKeywordMapping = new LocalizedKeywordMapping(culture);
			lock (LocalizedKeywordMapping.keywordCacheLock)
			{
				LocalizedKeywordMapping result;
				if (LocalizedKeywordMapping.mapsByCultures.TryGetValue(culture, out result))
				{
					return result;
				}
				LocalizedKeywordMapping.mapsByCultures.Add(culture, localizedKeywordMapping);
			}
			return localizedKeywordMapping;
		}

		private static readonly object keywordCacheLock = new object();

		private static readonly Dictionary<CultureInfo, LocalizedKeywordMapping> mapsByCultures = new Dictionary<CultureInfo, LocalizedKeywordMapping>();

		private static readonly List<KeyValuePair<PropertyKeyword, LocalizedString>> propertyKeywordToLocStringMapping = new List<KeyValuePair<PropertyKeyword, LocalizedString>>
		{
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.All, ClientStrings.AllKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.Attachment, ClientStrings.AttachmentKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.AttachmentNames, ClientStrings.AttachmentNamesKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.Bcc, ClientStrings.BccKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.Body, ClientStrings.BodyKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.Category, ClientStrings.CategoryKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.Cc, ClientStrings.CcKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.Expires, ClientStrings.ExpiresKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.From, ClientStrings.FromKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.HasAttachment, ClientStrings.HasAttachmentKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.Importance, ClientStrings.ImportanceKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.IsFlagged, ClientStrings.IsFlaggedKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.IsRead, ClientStrings.IsReadKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.Kind, ClientStrings.KindKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.Participants, ClientStrings.ParticipantsKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.PolicyTag, ClientStrings.PolicyTagKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.Received, ClientStrings.ReceivedKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.Recipients, ClientStrings.RecipientsKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.Sent, ClientStrings.SentKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.Size, ClientStrings.SizeKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.To, ClientStrings.ToKqlKeyword),
			new KeyValuePair<PropertyKeyword, LocalizedString>(PropertyKeyword.Subject, ClientStrings.SubjectKqlKeyword)
		};

		private static readonly List<KeyValuePair<KindKeyword, LocalizedString>> kindKeywordToLocStringMapping = new List<KeyValuePair<KindKeyword, LocalizedString>>
		{
			new KeyValuePair<KindKeyword, LocalizedString>(KindKeyword.contacts, ClientStrings.ContactsKqlKeyword),
			new KeyValuePair<KindKeyword, LocalizedString>(KindKeyword.docs, ClientStrings.DocsKqlKeyword),
			new KeyValuePair<KindKeyword, LocalizedString>(KindKeyword.email, ClientStrings.EmailKqlKeyword),
			new KeyValuePair<KindKeyword, LocalizedString>(KindKeyword.faxes, ClientStrings.FaxesKqlKeyword),
			new KeyValuePair<KindKeyword, LocalizedString>(KindKeyword.im, ClientStrings.ImKqlKeyword),
			new KeyValuePair<KindKeyword, LocalizedString>(KindKeyword.journals, ClientStrings.JournalsKqlKeyword),
			new KeyValuePair<KindKeyword, LocalizedString>(KindKeyword.meetings, ClientStrings.MeetingsKqlKeyword),
			new KeyValuePair<KindKeyword, LocalizedString>(KindKeyword.notes, ClientStrings.NotesKqlKeyword),
			new KeyValuePair<KindKeyword, LocalizedString>(KindKeyword.posts, ClientStrings.PostsKqlKeyword),
			new KeyValuePair<KindKeyword, LocalizedString>(KindKeyword.rssfeeds, ClientStrings.RssFeedsKqlKeyword),
			new KeyValuePair<KindKeyword, LocalizedString>(KindKeyword.tasks, ClientStrings.TasksKqlKeyword),
			new KeyValuePair<KindKeyword, LocalizedString>(KindKeyword.voicemail, ClientStrings.VoicemailKqlKeyword)
		};

		private static readonly List<KeyValuePair<Importance, LocalizedString>> importanceToLocStringMapping = new List<KeyValuePair<Importance, LocalizedString>>
		{
			new KeyValuePair<Importance, LocalizedString>(Importance.High, ClientStrings.HighKqlKeyword),
			new KeyValuePair<Importance, LocalizedString>(Importance.Low, ClientStrings.LowKqlKeyword),
			new KeyValuePair<Importance, LocalizedString>(Importance.Normal, ClientStrings.NormalKqlKeyword)
		};

		private static readonly List<KeyValuePair<PropertyKeyword, string>> propertyKeywordToCanonicalStringMapping = new List<KeyValuePair<PropertyKeyword, string>>
		{
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.All, "All"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.Attachment, "Attachment"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.AttachmentNames, "AttachmentNames"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.Bcc, "Bcc"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.Body, "Body"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.Category, "Category"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.Cc, "Cc"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.Expires, "Expires"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.From, "From"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.HasAttachment, "HasAttachment"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.Importance, "Importance"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.IsFlagged, "IsFlagged"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.IsRead, "IsRead"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.Kind, "Kind"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.Participants, "Participants"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.PolicyTag, "PolicyTag"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.Received, "Received"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.Recipients, "Recipients"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.Sent, "Sent"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.Size, "Size"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.To, "To"),
			new KeyValuePair<PropertyKeyword, string>(PropertyKeyword.Subject, "Subject")
		};

		private static readonly List<KeyValuePair<KindKeyword, string>> kindKeywordToCanonicalStringMapping = new List<KeyValuePair<KindKeyword, string>>
		{
			new KeyValuePair<KindKeyword, string>(KindKeyword.contacts, "Contacts"),
			new KeyValuePair<KindKeyword, string>(KindKeyword.docs, "Docs"),
			new KeyValuePair<KindKeyword, string>(KindKeyword.email, "Email"),
			new KeyValuePair<KindKeyword, string>(KindKeyword.faxes, "Faxes"),
			new KeyValuePair<KindKeyword, string>(KindKeyword.im, "Im"),
			new KeyValuePair<KindKeyword, string>(KindKeyword.journals, "Journals"),
			new KeyValuePair<KindKeyword, string>(KindKeyword.meetings, "Meetings"),
			new KeyValuePair<KindKeyword, string>(KindKeyword.notes, "Notes"),
			new KeyValuePair<KindKeyword, string>(KindKeyword.posts, "Posts"),
			new KeyValuePair<KindKeyword, string>(KindKeyword.rssfeeds, "RssFeeds"),
			new KeyValuePair<KindKeyword, string>(KindKeyword.tasks, "Tasks"),
			new KeyValuePair<KindKeyword, string>(KindKeyword.voicemail, "Voicemail")
		};

		private static readonly List<KeyValuePair<Importance, string>> importanceToCanonicalStringMapping = new List<KeyValuePair<Importance, string>>
		{
			new KeyValuePair<Importance, string>(Importance.High, "High"),
			new KeyValuePair<Importance, string>(Importance.Low, "Low"),
			new KeyValuePair<Importance, string>(Importance.Normal, "Normal")
		};

		private readonly Dictionary<string, PropertyKeyword> propertyKeywordMapping;

		private readonly Dictionary<string, KindKeyword> kindKeywordMapping;

		private readonly Dictionary<string, Importance> importanceKeywordMapping;
	}
}
