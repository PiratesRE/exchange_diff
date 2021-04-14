using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.MessagingPolicies;
using Microsoft.Exchange.MessagingPolicies.Journaling;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.Transport.Logging.MessageTracking;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Win32;

namespace Microsoft.Exchange.Transport.Agent.JournalFilter
{
	internal sealed class JournalFilterAgent : SmtpReceiveAgent
	{
		internal JournalFilterAgent(JournalPerfCountersWrapper perfCountersWrapper)
		{
			base.OnEndOfHeaders += this.EndOfHeaderHandler;
			this.perfCountersWrapper = perfCountersWrapper;
		}

		private static bool IsJournalFilterEnabled()
		{
			try
			{
				object value = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Exchange_Test\\v15\\BCM", "DisableJournalFilterSmtpAgent", 0);
				if (value is int && (int)value != 0)
				{
					return false;
				}
			}
			catch (SecurityException)
			{
			}
			catch (IOException)
			{
			}
			return true;
		}

		private void EndOfHeaderHandler(ReceiveMessageEventSource receiveMessageEventSource, EndOfHeadersEventArgs endOfHeadersEventArgs)
		{
			if (JournalFilterAgent.DisableJournalReportFiltering)
			{
				return;
			}
			if (receiveMessageEventSource == null)
			{
				throw new ArgumentNullException("receiveMessageEventSource");
			}
			if (endOfHeadersEventArgs == null)
			{
				throw new ArgumentNullException("endOfHeadersEventArgs");
			}
			if (endOfHeadersEventArgs.MailItem.Recipients.Count > 0 && endOfHeadersEventArgs.MailItem.Recipients[0].RecipientCategory == RecipientCategory.Incoming)
			{
				MessagingPoliciesUtils.JournalVersion journalVersion = MessagingPoliciesUtils.CheckJournalReportVersion(endOfHeadersEventArgs.Headers);
				bool flag = journalVersion != MessagingPoliciesUtils.JournalVersion.None;
				bool flag2 = endOfHeadersEventArgs.Headers.FindFirst("X-MS-Gcc-Journal-Report") != null;
				bool flag3 = endOfHeadersEventArgs.Headers.FindFirst("X-MS-EHA-MessageDate") != null;
				if (flag && !flag2)
				{
					MailItem mailItem = endOfHeadersEventArgs.MailItem;
					ITransportMailItemWrapperFacade transportMailItemWrapperFacade = (ITransportMailItemWrapperFacade)mailItem;
					ITransportMailItemFacade transportMailItem = transportMailItemWrapperFacade.TransportMailItem;
					OrganizationId organizationId = (OrganizationId)transportMailItem.OrganizationIdAsObject;
					PerTenantTransportSettings perTenantTransportSettings = null;
					string text = string.Empty;
					Header header = endOfHeadersEventArgs.Headers.FindFirst(HeaderId.MessageId);
					if (header != null)
					{
						text = (header.Value ?? string.Empty);
					}
					using (JournalLogContext journalLogContext = new JournalLogContext("JF", "OnEndOfHeaders", text, mailItem))
					{
						SmtpResponse response;
						if (!Components.Configuration.TryGetTransportSettings(organizationId, out perTenantTransportSettings))
						{
							ExTraceGlobals.JournalingTracer.TraceError<OrganizationId>(0L, "Journal Filter Agent: Unable to determine LegacyArchiveJournalling setting because Transport Settings could not be loaded for tenant: {0}. All journal reports are blocked in DC unless LegacyArchiveJournaling is enabled on the tenant. This report will be dropped.", organizationId);
							JournalFilterAgent.logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_JournalFilterTransportSettingLoadError, DateTime.UtcNow.Hour.ToString(), new object[]
							{
								text,
								organizationId
							});
							journalLogContext.AddParameter("Cfg", new object[0]);
							response = SmtpResponse.DataTransactionFailed;
						}
						else
						{
							bool flag4 = false;
							bool flag5 = false;
							bool flag6 = false;
							if (perTenantTransportSettings != null && (organizationId != OrganizationId.ForestWideOrgId || VariantConfiguration.InvariantNoFlightingSnapshot.Ipaed.ProcessForestWideOrgJournal.Enabled))
							{
								flag4 = perTenantTransportSettings.LegacyArchiveJournalingEnabled;
								flag5 = perTenantTransportSettings.LegacyArchiveLiveJournalingEnabled;
								flag6 = perTenantTransportSettings.JournalArchivingEnabled;
							}
							if (flag3 && flag4)
							{
								ExTraceGlobals.JournalingTracer.TraceError<OrganizationId>(0L, "Journal Filter Agent: isLegacyArchiveJournalEhaMigration && isLegacyArchiveJournalEnabled is true for tenant {0} hence we let this journal enter.", organizationId);
								journalLogContext.LogAsSkipped("Eha", new object[0]);
								return;
							}
							if (flag && !flag3 && (flag5 || flag6))
							{
								ExTraceGlobals.JournalingTracer.TraceError<OrganizationId, string>(0L, "Journal Filter Agent: isAnyJournalReport && !isLegacyArchiveEhaMigrationJournal && (isLegacyArchiveLiveJournalEnabled || isJournalArchiveEnabled) is true for tenant {0} hence we let this journal enter. Journal Version is {1}", organizationId, journalVersion.ToString());
								journalLogContext.LogAsSkipped("LeArch", new object[0]);
								return;
							}
							response = SmtpResponse.QueuedMailForDelivery(text);
						}
						receiveMessageEventSource.RejectMessage(response);
						this.TrackAgentInfo(transportMailItem, "incoming");
						this.JournalArchiveCustomerCheck(mailItem);
						this.perfCountersWrapper.Increment(PerfCounters.IncomingJournalReportsDropped, 1L);
					}
				}
			}
		}

		private void TrackAgentInfo(ITransportMailItemFacade tmi, string reasonCode)
		{
			TransportMailItem transportMailItem = tmi as TransportMailItem;
			if (transportMailItem != null && !transportMailItem.IsRowDeleted)
			{
				transportMailItem.AddAgentInfo("JA", "EF", new List<KeyValuePair<string, string>>
				{
					new KeyValuePair<string, string>("reason", reasonCode)
				});
				MessageTrackingLog.TrackAgentInfo(transportMailItem);
			}
		}

		private void JournalArchiveCustomerCheck(MailItem mailItem)
		{
			TransportMailItem transportMailItem = ((ITransportMailItemWrapperFacade)mailItem).TransportMailItem as TransportMailItem;
			if (transportMailItem != null)
			{
				string text = transportMailItem.ExternalOrganizationId.ToString();
				if (this.journalArchiveCustomers.Contains(text.ToLower()))
				{
					this.PublishMonitoringResults(text);
				}
			}
		}

		private void PublishMonitoringResults(string tenantId)
		{
			new EventNotificationItem(ExchangeComponent.Compliance.Name, "JournalFilterAgentComponent", string.Empty, ResultSeverityLevel.Error)
			{
				StateAttribute1 = (string.IsNullOrEmpty(tenantId) ? string.Empty : tenantId)
			}.Publish(false);
		}

		private static readonly bool DisableJournalReportFiltering = !JournalFilterAgent.IsJournalFilterEnabled();

		private static ExEventLog logger = new ExEventLog(new Guid("7D2A0005-2C75-42ac-B495-8FE62F3B4FCF"), "MSExchange Messaging Policies");

		private readonly JournalPerfCountersWrapper perfCountersWrapper;

		private readonly List<string> journalArchiveCustomers = new List<string>
		{
			"b2ff72b3-04ad-460b-a7fc-7d332b2a83e6",
			"23ba22b4-c2f6-4cb5-8fc9-054c56f20749",
			"412b58da-bb0a-4b8a-b0eb-b80ba2a4b2d6",
			"7bcb91e3-8b67-457d-a7c8-041ad559d7e0",
			"71093690-14bf-4d92-b8e2-f5f0689e6cbf",
			"c05a06ce-b498-486f-bc68-4f3f80327515",
			"c7478c99-e339-4bd2-8b19-776654981f88",
			"fb54e24c-5f45-44ef-9612-d43fe675a303",
			"933d39bb-fcc8-40de-b7b2-eb205028a021",
			"3a4ed0b8-c25a-4e40-a0d5-2d7ebb47729a",
			"4130bece-b7f7-40c6-8a3d-f347f0214b97",
			"8dcd68a6-fd26-4d3b-af68-fbc260b64c32",
			"3c9415d8-1cb7-4349-a3b2-7947b93e50d8",
			"191415a6-d71b-49d8-8b87-881cfa7d2386",
			"0dd43548-510b-4027-b11e-572b4e725aa0",
			"011eddd5-2df1-4168-be9e-5704ce6af90a",
			"c58dc4ed-3528-41da-988d-db3fd3a41f41",
			"620c697c-f921-459f-b563-402b1e0e0c9c",
			"8883a173-3891-4fcf-abc7-8d721f488ae0",
			"3e30016d-9339-41fe-9b6b-3f7525e58f61",
			"0f6a0506-5804-4333-bb09-d385a9d53da0",
			"7e622088-a407-4dd6-a1d9-342b87bc4263",
			"48957010-d0de-4e00-b53b-71cfe601e391",
			"8cf54f01-faf1-4306-bd09-40ac9ac7e930",
			"5444a274-db1c-49fa-b44f-8c43968dccf7",
			"a6f6c76e-3ac2-44f4-be90-f49150fcc667",
			"931bf31a-9bb4-48c7-b9fc-6908aac53ab3",
			"1f7dbb83-4bf7-4c0e-92a5-847e18da3841",
			"82e8eefb-7c66-4486-8ee3-0fb13c32b776",
			"7c91ed10-8406-4e04-ae57-1713ab6489d0",
			"a7cdf46f-c323-41c8-be79-59c78eb1ca00",
			"9c7ee59f-7266-4931-88bb-ee2331df7cb4",
			"85874664-2ceb-48b2-935c-d6137d8ddfa5",
			"8aed231d-3ae9-4da5-8f97-1108f77fbef3",
			"fcab7a24-3935-4814-a178-713a23d424a0",
			"57666bb3-535e-4a68-be6c-48daa0393a15",
			"3a8fa8df-607b-44c5-9b49-cf94a79e505a",
			"fcb54e92-1ce4-4687-894c-227802b96325",
			"1b7685fb-7e35-48c9-b992-8de00c7c1855",
			"8b4f7acb-af0f-4726-8c15-1d2f2d56b95a",
			"ea91f81c-300f-4de3-bf29-6b2546f5534e",
			"9054bcb0-8fdd-4db6-80bc-19b11470b4f0",
			"67bba897-cfd2-4190-b2d6-46cdc6aa8742",
			"d1875e9c-6ef8-4640-8dfc-40526d1112ed",
			"07547cc2-1d68-4d0f-8da3-28e006e2b202",
			"bf0ac3a3-10f8-4570-9570-5eed302dba7c",
			"583bb070-77a2-41be-8c65-d1ae5668a6a1",
			"d860f2d3-ef44-4a65-955e-acab0e674f71",
			"c7555a66-2f59-4a3e-a6a3-f5d1aeab52a1",
			"b7b43cd5-70ae-46d3-b11d-62e77f3b43cc",
			"214268a3-e121-4486-acd4-545c9faf2252",
			"56ddf89f-f56a-4580-a618-b3ec65cf64c4",
			"fd542e5a-8f48-4f76-bf40-d012be158f52",
			"464a71a6-4a3d-482a-98d9-197e72d8b766",
			"8c16abf9-02ad-4dc0-a6b1-fca318c7ddf6",
			"3a707b16-081d-4cb9-be7f-6039c780495b",
			"01039f9c-7e16-4cf0-b227-49a6d0ea3fc9",
			"b91a9c85-cc42-44ba-ac82-ba44de8306e8",
			"96acb2f0-4295-4059-b0b9-6becc51eb38c",
			"8de9cf77-347d-4209-b268-e723cae1f46b",
			"9b2b3b1f-28a0-4158-97b2-66016140feb1",
			"63ea9758-94c4-4afb-839e-873d27585e1e",
			"17f63f33-c4f0-4818-b6d0-618898d30ae3",
			"1d824559-42b3-4114-8151-cb2450e2b362",
			"8a5fc2b0-bcf0-4e35-b77d-8a24bbd6225d",
			"e49bcdf5-2d95-469a-9d1f-3861e368be4d",
			"3e0960eb-39cf-49c7-b836-06b8e510a589",
			"2813f3e9-5272-4a92-9be0-93862107727f",
			"7b43b295-247e-43a2-aaa7-4cd49f70bb5d",
			"8c309a24-01fb-4353-a7a8-1476ca437567",
			"c07e3958-dd29-4024-8e81-fbbb9218f9e6",
			"20959a94-893f-49ca-96e7-42a53e1d1aa5",
			"8727a554-a874-4216-9892-598987dc41bf",
			"e55663a0-4d83-44fe-908e-cdc121d490ea",
			"c446d971-f5ae-4d9c-a6c6-11fae1db69e1",
			"a2d2fcce-c9b0-4e93-8c14-7295264a8541",
			"3bc15d78-1f43-416d-bd03-abf771c84009",
			"88c926a0-e07a-4e5e-8370-fee437cac17c",
			"56e74457-aba2-47f6-b13b-e2353f728eb0",
			"352cfbc7-408a-4476-bf8b-ebd273e4fd15",
			"738aaa31-8d7d-4614-a4c1-7029a9a39b8d",
			"3090abf7-2ab4-4e6d-8f27-1aa7ea67aa00",
			"e48d77d1-182b-459c-ae39-79d17aa4771f",
			"882ae029-3f22-4e2f-8134-bf1f1c9c096c",
			"05f4d373-eb40-465f-87ad-e3af4f5dd7ed",
			"5e8824b2-df51-48fa-8715-ba8095c1cf47",
			"71589528-97db-4d44-b7ec-7bc235c5f637",
			"1253a058-5675-4e4c-b71a-bd539850486c",
			"dff4ad88-2e66-415d-836c-d2b7e0d919a2",
			"8cb42433-8de6-44b6-9fb7-668aa91e91ab",
			"f3844388-6554-42f5-a390-93613c62dcff",
			"fe510255-a21c-4494-a01a-f4655440dd63",
			"85502a98-58f6-4523-8487-0eb3bbc369fd",
			"25883a64-3524-4b4c-8407-dab11d02694c",
			"f0b2639c-842b-48ec-9baf-5594e9e2eac7",
			"6c6599e2-77cf-4c13-8a10-1ad9b357b4b1",
			"1fdbe99d-2298-44ed-8561-d90c0630ab10",
			"ada073e1-4d52-4aba-b781-642bdcd2e075",
			"587e87fd-557d-47ab-90e7-65571bf80e82",
			"5be88093-0ce8-46be-b4f9-ac45968f4ced",
			"d9cb2dc2-cdab-46e5-8259-b1c402ab8add",
			"d9f72c99-81ca-4ac9-b619-25d6d875ce12",
			"894c72af-6607-4c24-bd9a-c0dd7ba7e185",
			"3f12c30c-f665-4b47-915c-1665f0a27308",
			"8d6aa279-6a92-4127-b125-04ea9426d4fd",
			"75713b2f-3ff6-40b0-89c0-6fb5748bcebb",
			"4e435301-c93d-488b-823d-db671a6bf3fe",
			"be8361f5-fe7d-4010-b742-00172b393ec4",
			"47460281-a589-428d-9552-be375b0a9d04",
			"5471ca0f-471b-4df4-8175-4fa0b6ace9d8",
			"ca6f59ab-2d21-4296-9153-7734b93a3e9a",
			"63604307-487a-43c8-86f0-26057a46bf09",
			"2fb8827f-af0c-4c7d-9f11-14ff89be86d8",
			"2062a668-6143-4509-be6e-3b8bb06cea24",
			"59f0d279-9af4-457f-bab3-dea1e30a6a9a",
			"5166cfc0-0ae9-4b27-9812-e652474f2695",
			"3a8eaaa7-b1bb-4a44-8683-7edf066194fa",
			"b5c9d923-77b2-459d-957a-5429441b30e8",
			"fefb3326-a304-428a-aaf3-b52e531784be",
			"a00e249f-ba16-4aac-981f-92a810611cc0",
			"018fd840-2a9b-4c47-8589-bbd675253644",
			"ab487e3d-d23a-4e8b-8f05-843cd598add4",
			"b4ee9dae-4aa3-4d35-b8c8-750dd8841260",
			"94ec3c22-ecf1-48bb-a745-0e76b2cb9ac3",
			"0931708d-12d0-4227-bc39-fe46c255553e",
			"cf5c2986-4c15-4925-970e-01e322fa9e71",
			"7f242490-1f88-4dc2-9094-35fa007ad45c",
			"4c4f4586-10be-430e-8cf7-cc08be86eeba",
			"9dc44320-431c-4ae6-bdb8-a79ad739f72c",
			"876a2a66-7dd2-4311-9ff2-23647e1d8675",
			"fb104ec2-8f30-4b19-a3a6-fe4c7970d536",
			"26fd9b82-ee87-46ce-88e9-d07e09c32f29",
			"d712dfd7-1f6b-448c-aece-66e66b5e9806",
			"4ec4cd74-b4c6-4290-bd34-8969ba84ad6b",
			"6c4dd5b0-e43d-403a-b6e8-1fb60fa3e723",
			"f0f26d4f-2e92-481c-be5c-4bb9a8e23bef",
			"78c8d07d-35d9-4a88-a6e6-5d321a42b160",
			"b1d40111-1bde-46a4-b15d-9df3226e2e0f",
			"72250acf-8850-4afd-9653-49930425c257",
			"d7c7bafc-fb53-4f80-b73c-80237256ea32",
			"a501811c-cb9f-4927-af12-cacdbc225542",
			"c8f13377-c5b9-47c7-8fb3-6986094682ee",
			"c856ecef-76f0-4b0f-9bb7-477441fba3f4",
			"28707726-0fd8-482e-bd1d-f7160a2b7bb1",
			"1c25d73c-6c7e-47d8-91ab-24b86399c01a",
			"01b31129-60ac-4ab3-a58b-babaf515a310",
			"f8d301d1-c655-4a66-89a6-aeb312d2912d",
			"f5d3a94a-aa17-4633-a38f-cfea258b7541",
			"a9527549-cda8-4974-a680-357cd85f8faf",
			"7d3a0d3f-4d47-4805-86a3-96d43a6507c3",
			"2c00e1ef-8fa3-497d-853c-89295c64f600",
			"4a54bd4e-56b0-4885-bc43-2b9e28cd77af",
			"6b73a9d0-0de7-4fab-8e33-eab95ec697d0",
			"513dfeec-21d8-4839-af5d-7bbd01eeb5c4",
			"a7a8bda8-0f3d-4deb-98bc-d86ca7568f55",
			"22bdc63c-3744-4c46-b50d-7437110db0d5",
			"3d4f74b4-4c53-4744-bc33-db1a00cab2a9",
			"4134ed48-294a-47a5-96c7-c98e914c464b",
			"97b5e70c-04b0-40de-8abd-bf60929610c7",
			"ec1b67c2-6711-40e5-9c0b-c858b3d5e4af",
			"5e9e5811-8ddf-4aab-8ea0-bf9641b36273",
			"58fd3b44-bdc3-4884-a213-8710e9e0cf35",
			"d07299d9-af59-466a-af92-72ad4e30f8b2",
			"221c582e-582a-4c9d-bd55-cc20a840d2a2",
			"a8457c94-17b7-4c5e-9c22-dd7fe03ef36f",
			"84c83325-280a-40cb-a8df-559ad007a34e",
			"0e6341ab-3662-4fe2-b551-9640d66cb3cf",
			"c9d4a41b-6207-4c6d-b0e0-30fd783c0ea9",
			"4a507564-3047-4f5a-980e-a218e46b2391",
			"ed03aaf3-bfd2-4a91-8c6e-5fad0dca158b",
			"033b356a-f31e-4895-8581-211e495498d9",
			"e0f3d9fc-efc0-4e32-821f-281899c9ebb9",
			"c73203b2-8fd1-4faa-92b3-feb7837b4d1d",
			"bbcb5b31-a77b-4cdb-a6e7-2c7054a216db",
			"67892ae9-6a68-435b-90fb-49846920d93b",
			"78be554c-608a-4cfc-be5c-9ab210401ce9",
			"bcc51ab2-2daf-4a56-8494-8fea42548f00",
			"4baf964b-f24f-4e04-9b0e-892d15aef8f0",
			"5d8d719f-2cb0-48e6-b4c3-edc51605c268",
			"3ad0e8c6-19a7-4642-83f5-f0e70114eb24",
			"a7a4bd0e-cad2-4606-9c13-d7ffa07f2944",
			"f58e2485-2175-46d9-b2f5-a08c93a73bac",
			"f84b054d-8962-498b-9b9b-ab044563eebc",
			"2ce547c5-e80a-4062-8a56-f25adceefb52",
			"13d4609e-0f04-489d-b154-16b3a0504576",
			"1533e5d1-38ea-4d0b-925e-a72836dd015d",
			"ae12d7de-709e-471a-a5a0-ec339fb59b24",
			"7a6298a2-1d78-4488-8808-081f30247382",
			"555c5f0e-96a4-4ff9-a638-069d8fc44f48",
			"edd71ef1-f252-4f25-90b0-c55690e65a76",
			"c0216af7-c63e-47c8-b884-5b39aa3287c2",
			"1a57001f-4251-497b-8e26-67ab3f0e29b6",
			"6f3c5c61-f8b2-4b42-ac0f-c2607ef93d50",
			"30dfce86-905c-44f1-8e8f-78067f32f618",
			"11c25270-4910-453c-bc3c-0a6fc727750d",
			"1c237568-d6a3-4fc3-ab71-9a305f379909",
			"5ba72c9d-6651-495f-8af5-c1cb0737175b",
			"7802cb40-ea94-4860-a806-fbb4e3f35e8c",
			"96c7cefd-8178-435c-a5dc-c6e66619d5aa",
			"30146291-1d1e-4b91-b7c9-eebacf0efee3",
			"d43bb55c-d0f6-4b33-a340-2980b4d18548",
			"3fc0d085-de39-4d28-9774-6280141dee89",
			"f8f3ee66-b08e-4504-83bc-9dd64b3de980",
			"a5eea1d1-c737-43dd-b4c3-ac4a2d5afeb8",
			"41f4570a-b1e3-4769-95a2-318eab478bad",
			"701865f9-fbf0-44fc-9511-c561903af50e",
			"df54fdef-aa3c-4d09-b635-a7553f002db1",
			"034574e4-1113-4d07-9f33-b5e5923c0db4",
			"9622b82f-f762-455a-9902-4e185f4e1225",
			"52d44c8b-afbf-420c-a0a9-c267528de561",
			"c9daae23-70d6-4d81-9d22-033cbfe232d6",
			"1aff4bca-2cd2-480a-a7e1-b6c33e892749",
			"39933028-35c7-443d-a59b-c0eb1d49b483",
			"2184c73e-a578-4885-b815-b3758ec354a7",
			"c4bee1d1-2e9b-426f-9b51-d2eae5551fe0",
			"0e437243-004e-4c7c-960c-83d67e6bcaf1",
			"25434a66-4478-4711-b3c1-6c3a31d3b91a",
			"2ac27dda-ca60-4769-98a5-68f5bdf44677",
			"93e9be78-f39c-45bf-b770-ea6534d55c1a",
			"08295dca-2abd-42f9-9a77-65314b36ed00",
			"b129eb9f-c815-40a7-b1c8-19542f833fa5",
			"b8d604a6-e6f0-401d-91d1-c4a6da917804",
			"0526d333-e9e0-4e9d-a664-0708baa41c77",
			"d906a4fe-8e74-4770-b126-5369f592855f",
			"fecf927f-8f9e-4797-b3e7-c890325c4b90",
			"80a1c9fb-8260-4df1-9c3c-2de257be97e7",
			"94a3b0a7-66d6-4f1d-a507-956d0d20deea",
			"2fd10e7d-fbd4-4a96-a752-0d1a305b768e",
			"d5f16ebc-22c2-454b-add1-1fb4fb69f539",
			"89bb3d63-a3d1-46d6-9185-6551b4f06e95",
			"e6ec949a-6ae8-4c55-8192-62ef5d49b84b",
			"850225f8-6be8-4a98-b754-545caa106cf6",
			"246d7d9b-193a-4594-80bf-04ead03447c8",
			"3ebea242-c78c-4ae0-9db9-0737f69ddff9",
			"fddf44eb-b802-4a95-9695-311ae3de23f2",
			"ecf7148d-00e3-4e16-95d5-4d3d136c2769",
			"a41462f2-63d3-42e2-990b-a546da2a192f",
			"858574f1-ad32-4d5d-8e46-effbb652c905",
			"2d834c80-d5c8-4fdc-9cda-f354135b7968",
			"510d3466-af82-4dd3-a4fe-ce404c065f52",
			"d67a9c32-719b-46b7-8a4b-9db92065c4c5",
			"e6b7eb95-58de-4568-93ac-9d378f315429",
			"b6c2c8e7-7818-4e23-9f7e-7b86f16e166c",
			"2a92b825-082d-4e62-9ee2-a32a34346382",
			"912e587c-c2de-4fe4-af80-4486e0292c9d",
			"969b2c01-8090-4859-846a-1042225692d1",
			"82df44ef-bc9d-412f-b21d-3be5d5f19633",
			"7bc21b5c-83b0-4482-be9e-2825d33c1759",
			"4e72ad44-10e9-415b-a678-66a6cf35331e",
			"8860d2f3-6316-4061-aadd-ecfa6948f36a",
			"f9c5ceab-1863-4c52-b82c-329f037d6144",
			"5ea0560d-086a-4d43-a323-2dd82034a32c",
			"48dddee4-ee83-4b74-a15e-494afb4f0439",
			"7d9af0c2-5cfe-4ea5-9ff1-f089dfee14b2",
			"ebe00ade-c04b-4c1e-bd55-e00552109e1a",
			"f245e2c6-cebe-4847-856c-e6a261963a9e",
			"56ab580f-93c3-424d-8310-683a4a8937d1",
			"b38db23a-00c9-4ea1-b0e5-3ce19e32ee4f",
			"4b89efe9-1838-407b-9056-8c0f7493a172",
			"5a0c2241-eb84-4fc4-9a3f-76026e9a9554",
			"4b0dbc1f-b1a3-4f15-8330-b02d00f4be78",
			"a3117fa6-63b3-43a4-bb26-4600e9787e3b",
			"85a7da7a-f72c-4f79-8dbf-7b93f2961e56",
			"a15ecdfd-5c52-49d1-a234-aab29d5d6f8b",
			"c2bae459-4bfa-4ebe-ae27-c65a25cc482e",
			"3526a4bc-243b-450b-b17b-b95ebdb61702",
			"fae5f54c-5a1a-4bd7-a994-215c33eefb97",
			"4f361f15-e756-48e2-b89c-c8069bcacab9",
			"2129e8df-927d-4b1f-a761-d556b735c8c0",
			"bd6bf8c0-b595-4559-b809-dd37a84b9db7",
			"312d91bd-c0af-4e1e-9f36-6e16add8b81e",
			"a25ad2ac-f0e5-4105-b552-720d641a89fe",
			"df821e45-170f-4108-820f-566a5a7ce428",
			"e82c1508-3fc8-4849-8ed5-f47cc48d4983",
			"7281e1cb-695c-43d5-a024-0913cc7f8521",
			"3fffe5fc-862b-438e-98fa-db947bf7c01c",
			"66638d0d-f29a-449c-965c-911fd6db64d8",
			"b95c9d3f-9787-457b-b01b-6d5afdcebd83",
			"62c46b57-d41e-4887-8217-5fe02c9682d7",
			"3511dc15-af98-411f-b93e-9a4172b3c024",
			"b5955f81-ec5f-48a7-88c4-1b6e335c3d3f",
			"8e0ce7d2-f370-49b9-9e24-63a90f58991c",
			"5cd19d26-65df-486a-96a1-94b8b4a01737",
			"5f8fbab8-3c99-47be-bf96-5fd43a239591",
			"43d27c74-cd5b-4c93-8c0f-d78041298474",
			"259debd0-9311-4d75-bb42-17f182d60227",
			"fe2b9aa2-06dc-4786-913b-a3917e3c9126",
			"217aeade-04ed-4719-b9ad-dddd0c5e727f",
			"984340ad-dc7a-43fa-83ce-d6ed5996e4f6",
			"2f16b4c6-5ad3-451f-9088-1d4ab66f7083",
			"d27752c0-c8c8-461d-822f-2359a1d58ed3",
			"23ae9c6e-0a58-4f95-a1c8-a0b23db4158f",
			"51f7815a-e747-43de-84c3-e9135aa63574",
			"e57a338a-106f-458b-ab22-728d5817db6f",
			"14a55fce-3b49-42e8-9f56-cf48d3bb2d19",
			"779b673e-e2e0-4377-afdd-8f7a44cd54a9",
			"52e9da0f-4f02-45ef-ad35-12e3e3dea602",
			"ff226ed5-bb32-47fc-af2e-540e65eb822a",
			"bac226ec-c783-4b03-b742-8d46ae9b8af8",
			"b3de0f67-87d4-4c09-9659-4f4ed47e63cb",
			"fcc54c2c-25d9-4083-9781-0c8b95a25544",
			"9ab92e47-ae46-4dfa-9990-519c50006e40",
			"959d5a63-27f6-4c8b-a0f5-f4cba2248fe2",
			"4cac5972-432f-4ede-9f27-be72926a1995",
			"431493be-2cc3-4c44-82dc-c4fc310d7f2a",
			"bfa96cfc-9746-4d77-96b6-1856a1b328e8",
			"a44bb37d-ad00-4cae-a1e2-50a5e7de493e",
			"b1d7b074-f3c8-4766-b641-34a34cfa6a4e",
			"d8284b34-ba2d-426e-b805-085127011d80",
			"9c9b2d08-c8bb-4af7-acd5-c9f72e5f92d4",
			"db4697d0-f981-4b9e-9777-5da30a493570",
			"6fbe8c6b-1134-490c-a42b-37e49a28ecf4",
			"00a76f7d-e84c-4133-bc4b-2fe09d67ffe2",
			"95489697-058a-47cc-b5ee-6f6609a813f1",
			"73a4c997-ac5a-4bcd-81f3-fd25589a48b7",
			"66c029b6-a0d1-4eae-8be8-b7363a1bb98f",
			"096880fc-9479-4929-a086-2ab26b3beb5d",
			"469a4435-c4cc-4f3b-8bd6-417837989b69",
			"1c35e6ac-92cc-4e56-a628-924dc40e7c4e",
			"2fae85c4-480a-4ad7-9ea5-bb4cb7242cef",
			"05821385-3c56-4048-9996-0cb4cd34a0a6",
			"9d27c8c6-b086-482c-bfd2-da76942487e4",
			"8e889101-808e-48bf-b4d2-2b1188e95a79",
			"834d6185-b0d7-4be7-bf43-de560f66192c",
			"b93ab0fc-e90d-43fd-a8f3-972cd6f27d1b",
			"5a1bbcfc-ce49-4df1-88e0-c04ac8bed4f1",
			"33e15217-3c38-48c6-b1a8-00d78c0b44dc",
			"899fe014-9d74-4b10-a2ba-292afafbb8f9",
			"5ede2486-b3f0-4350-9d14-a000db19c73d",
			"dd241ea4-8df7-4a54-ac4c-8deb6934e6b6",
			"2f370bb2-2194-4ef8-ae4a-320a9694f480",
			"6257f707-3765-43b6-9311-0ab7e7f0ed4d",
			"599e5649-83a3-421c-906e-6f12d870eade",
			"829ba696-0526-4137-a7e2-5a6850733bf5",
			"a362a3b6-fc4a-4c66-9e12-c11fc1c1fc1c",
			"21a8474b-0978-419e-982c-e52da9953628",
			"f8fee76d-cc9e-4ad2-aa69-19be395999cb",
			"e5f718b8-bd36-4b6f-b6c4-5c68f67ff2b3",
			"ba38d6a7-a749-4f30-8611-5533f84a9a0c",
			"9b2ab93d-4c23-4b5f-b2bb-c66d0e067458",
			"1110ea71-4d61-45cf-b785-fdba7cf4bda4",
			"bcad90ef-d099-4b92-b050-91ad79c06a85",
			"ac6b7dcc-333e-4f82-803a-c1bf9dc5d6f4",
			"a2a9e65b-45c9-4fb4-8ecf-5d2112f84b89",
			"44896b31-3c35-4501-bc50-28364042a0eb",
			"62c72f2b-bc39-4436-9350-a873e8cb0763",
			"9b498101-3ddb-4234-b2a2-3c48ec26e6b8",
			"6453fbc4-66c7-41de-9ed9-bfc91dd99b62",
			"45b42250-4f28-44ae-a560-7529764dd91f",
			"b2258b76-e5b5-4b5a-b994-78b866381336",
			"6aef6bdf-3e04-4f64-9b62-356a1a16ea48",
			"6f92eaf5-4a32-4aef-826a-2c1d65288695",
			"9f225e17-e690-4592-a72e-5e50971909d6",
			"a31d0088-62db-4448-97dd-48290d59d0c6",
			"47504cc5-5077-445f-8f09-e44221773a7e",
			"db644bee-3f34-4deb-a8e8-a2ca78569148",
			"2e89d042-d40d-449b-bd11-2288647235e9",
			"8941919a-be45-477b-a668-9be78e551cee",
			"6ff1a927-0691-49fd-a609-b32d82506fd8",
			"1d113da0-d064-461d-91fc-07a0467d3f80",
			"a88b47a1-7725-4b9d-a566-ae2fad7ff415",
			"873abdfd-871d-424a-a1cf-facfc3e43237",
			"7bdf47f8-1e3d-4604-9654-2dc9b0396c77",
			"6b508851-1366-49e5-872c-028c4d990f45",
			"0fade6f7-5085-40e2-a38d-ca740561a1f6",
			"5ee33a9e-0a31-4bed-86d1-d3cc11e62280",
			"deb1e400-cff4-4e68-9397-67e04870e488",
			"f2b8d92b-0d46-43de-b499-05e5e7935349",
			"e061cd62-def0-4f35-a5a3-b46d033835ab",
			"fe093d4b-7337-442a-abb5-6fd9c113a11e",
			"4b0dbc1f-b1a3-4f15-8330-b02d00f4be78",
			"246d7d9b-193a-4594-80bf-04ead03447c8",
			"85353c6c-8a7d-4b54-80ac-8918ef9df1a0",
			"5874c6ee-77f5-4b93-a3cf-cf3edf8351df",
			"8bf9f1f6-d99d-4b54-8936-e12eacadbe21",
			"ff15dbe1-e8a2-4d36-ad9e-6f62a48b1c72",
			"a31e0cdc-c031-4c13-aaab-46b22cc2b411",
			"82c1a7ef-5f09-4efe-92a6-4ac8ee67ad71",
			"834d6185-b0d7-4be7-bf43-de560f66192c",
			"f019eda9-427b-4b5e-953a-123828e645d1",
			"adbd1dc9-f936-4818-85cf-948a8087cfd7",
			"79ff3dd0-51bd-4a4d-8c72-bc39f249b74d",
			"8688c7c4-1f2a-4115-a918-361023dde469",
			"5d53c843-3af1-4445-935f-96f9f537e8b5",
			"7fcdb9de-5b95-4327-b578-1b276f4d0ca2",
			"06d390e3-6460-428a-b710-0f3e3039503a",
			"846f1883-ad5e-4528-9c5c-1b109d1076aa",
			"b8e8d71a-947d-41dd-81dd-8401dcc51007",
			"22d5c2cf-ce3e-443d-9a7f-dfcc0231f73f",
			"25ee51e5-673e-4af8-a84d-23a8186e08a7",
			"568884bc-b6e8-4240-9818-ca501d78fd33",
			"1ed9b049-57b5-4ada-82a8-19ebeec47ca2",
			"408ce90c-fe5d-4fe4-bec8-bcdb44bc6e82",
			"f46cb8ea-7900-4d10-8ceb-80e8c1c81ee7",
			"6257f707-3765-43b6-9311-0ab7e7f0ed4d",
			"096880fc-9479-4929-a086-2ab26b3beb5d",
			"23ae9c6e-0a58-4f95-a1c8-a0b23db4158f",
			"959d5a63-27f6-4c8b-a0f5-f4cba2248fe2",
			"014e8fe6-b871-434c-b297-6d111cb835b7",
			"3c012268-10a4-44c0-94e1-ffa0570e0331",
			"2f16b4c6-5ad3-451f-9088-1d4ab66f7083",
			"14a55fce-3b49-42e8-9f56-cf48d3bb2d19",
			"5f5c8279-ccd7-4c47-8d15-ac9cc1973501",
			"0f9dd353-eaab-48cc-9804-72502e48c5af",
			"b6fb5409-ebb7-4c20-82df-d0ae1cdda7fc",
			"578ecb1a-4448-4c2d-b6eb-9fe9df3b72fa",
			"d2456032-f373-4f8d-908c-3b899f0f6097",
			"d3e0dbb1-74d3-40dc-a3b0-20506fa2b4f9"
		};
	}
}
