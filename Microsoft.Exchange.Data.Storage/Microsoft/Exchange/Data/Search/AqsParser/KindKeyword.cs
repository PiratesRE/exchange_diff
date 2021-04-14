using System;

namespace Microsoft.Exchange.Data.Search.AqsParser
{
	public enum KindKeyword
	{
		[LocDescription(DataStrings.IDs.KindKeywordEmail)]
		email,
		[LocDescription(DataStrings.IDs.KindKeywordMeetings)]
		meetings,
		[LocDescription(DataStrings.IDs.KindKeywordTasks)]
		tasks,
		[LocDescription(DataStrings.IDs.KindKeywordNotes)]
		notes,
		[LocDescription(DataStrings.IDs.KindKeywordDocs)]
		docs,
		[LocDescription(DataStrings.IDs.KindKeywordJournals)]
		journals,
		[LocDescription(DataStrings.IDs.KindKeywordContacts)]
		contacts,
		[LocDescription(DataStrings.IDs.KindKeywordIm)]
		im,
		[LocDescription(DataStrings.IDs.KindKeywordVoiceMail)]
		voicemail,
		[LocDescription(DataStrings.IDs.KindKeywordFaxes)]
		faxes,
		[LocDescription(DataStrings.IDs.KindKeywordPosts)]
		posts,
		[LocDescription(DataStrings.IDs.KindKeywordRssFeeds)]
		rssfeeds
	}
}
