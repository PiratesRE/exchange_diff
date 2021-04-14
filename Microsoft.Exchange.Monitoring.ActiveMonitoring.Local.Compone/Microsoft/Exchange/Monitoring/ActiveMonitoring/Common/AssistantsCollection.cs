using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public static class AssistantsCollection
	{
		static AssistantsCollection()
		{
			AssistantsCollection.getAssistantsCollection.Add(new Guid("DF4B5565-53E9-4776-A824-185F22FB3CA6"), "CalendarAssistant");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("6695D35B-60DA-4fb4-8FD9-4D1E869018EB"), "ApprovalAssistant");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("024841B8-84CA-473d-88E4-512BB35615CA"), "MwiAssistant");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("55462BFF-3E63-4d49-9B4C-5FC2575672EB"), "CalendarNotificationAssistant");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("D22F2F1F-5AF4-4606-A0BE-D255770A1BF9"), "ConversationsAssistant");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("96315D66-3451-40ce-8031-779CB62DB522"), "OofAssistant");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("ED158DFD-E42A-441d-A4F5-5BC3917015BF"), "ElcEventBasedAssistant");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("B4262426-A19C-463f-896F-D84059EE1881"), "ProvisioningAssistant");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("11FF643B-6BC5-4497-8326-3AB14A51BEAB"), "ResourceBookingAssistant");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("CD16F035-1FE9-45b6-8B5E-6DAA48DE02AB"), "FreeBusyPublishingAssistant");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("CA9B1CCE-956F-4665-89C4-E3902F075C18"), "JunkEmailOptionsAssistant");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("B9FDDC1F-A933-488b-A687-9D73C1E047F6"), "UMPartnerMessageAssistant");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("2ADFFCAD-8506-41ad-8735-937A19CD8491"), "SharingFolderAssistant");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("C43217CB-946B-485a-A0A7-701633020096"), "SubscriptionEventAssistant");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("A77BE922-83FD-4EB1-9E88-6CAADBC7340E"), "MailSubmissionService");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("CC64DD2D-2428-4f12-BBA2-79D6D34C4D27"), "ContentIndexingService");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("4AA722B7-26E7-40FC-9B91-A3023D1FF117"), "SearchServiceNotificationsFeederAssistant");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("1693EBA9-A9EF-46DF-9625-F29D7BD95418"), "PushNotificationAssistant");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("9409FDD0-E7E0-4D6B-AF02-F49A36C10FD4"), "MailboxTransportSubmissionAssistant");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("955983AE-0C4E-4D0C-AA36-6F48350C7902"), "DiscoverySearchEventBasedAssistant");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("7625F2B9-981F-4F2A-AB71-5902EB1626FC"), "RemindersAssistant");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("3CA30B76-BF55-4EBA-9749-58FAAEA8647F"), "RecipientDLExpansionAssistant");
			AssistantsCollection.getAssistantsCollection.Add(new Guid("CC883983-BEC2-479E-82EC-599B257D99C7"), "CalendarInteropAssistant");
		}

		internal static string GetAssistantName(Guid assistantGuid)
		{
			if (AssistantsCollection.getAssistantsCollection.ContainsKey(assistantGuid))
			{
				return AssistantsCollection.getAssistantsCollection[assistantGuid];
			}
			return assistantGuid.ToString();
		}

		internal static bool Contains(Guid assistantGuid)
		{
			return AssistantsCollection.getAssistantsCollection.ContainsKey(assistantGuid);
		}

		internal static Guid? GetAssistantConsumerGuidFromName(string assistantName)
		{
			if (AssistantsCollection.getAssistantsCollection.ContainsValue(assistantName))
			{
				return new Guid?(AssistantsCollection.getAssistantsCollection.First((KeyValuePair<Guid, string> x) => x.Value == assistantName).Key);
			}
			if (string.Equals(assistantName, "MultipleAssistants", StringComparison.OrdinalIgnoreCase))
			{
				return new Guid?(Guid.Empty);
			}
			return null;
		}

		internal const string MultipleAssistantsString = "MultipleAssistants";

		internal const string MailboxTransportSubmissionAssistantString = "MailboxTransportSubmissionAssistant";

		internal const string DiscoverySearchEventBasedAssistantString = "DiscoverySearchEventBasedAssistant";

		internal const string ElcEventBasedAssistantString = "ElcEventBasedAssistant";

		internal static readonly ReadOnlyCollection<string> complianceAssistants = new ReadOnlyCollection<string>(new string[]
		{
			"DiscoverySearchEventBasedAssistant",
			"ElcEventBasedAssistant"
		});

		private static readonly Dictionary<Guid, string> getAssistantsCollection = new Dictionary<Guid, string>();
	}
}
