using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.Protocols.DeltaSync.HMFolder;
using Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail;
using Microsoft.Exchange.Net.Protocols.DeltaSync.ItemOperationsRequest;
using Microsoft.Exchange.Net.Protocols.DeltaSync.SendRequest;
using Microsoft.Exchange.Net.Protocols.DeltaSync.SettingsRequest;
using Microsoft.Exchange.Net.Protocols.DeltaSync.StatelessRequest;
using Microsoft.Exchange.Net.Protocols.DeltaSync.SyncRequest;

namespace Microsoft.Exchange.Net.Protocols.DeltaSync
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DeltaSyncRequestGenerator
	{
		private XmlSerializer SyncSerializer
		{
			get
			{
				if (this.syncSerializer == null)
				{
					this.syncSerializer = new XmlSerializer(typeof(Sync));
				}
				return this.syncSerializer;
			}
		}

		private XmlSerializer ItemOperationsSerializer
		{
			get
			{
				if (this.itemOperationsSerializer == null)
				{
					this.itemOperationsSerializer = new XmlSerializer(typeof(ItemOperations));
				}
				return this.itemOperationsSerializer;
			}
		}

		private XmlSerializer SettingsSerializer
		{
			get
			{
				if (this.settingsSerializer == null)
				{
					this.settingsSerializer = new XmlSerializer(typeof(Settings));
				}
				return this.settingsSerializer;
			}
		}

		private XmlSerializer SendSerializer
		{
			get
			{
				if (this.sendSerializer == null)
				{
					this.sendSerializer = new XmlSerializer(typeof(Send));
				}
				return this.sendSerializer;
			}
		}

		private XmlSerializer StatelessSerializer
		{
			get
			{
				if (this.statelessSerializer == null)
				{
					this.statelessSerializer = new XmlSerializer(typeof(Stateless));
				}
				return this.statelessSerializer;
			}
		}

		internal void SetupGetChangesRequest(string folderSyncKey, string emailSyncKey, int windowSize, Stream requestStream)
		{
			Sync sync = new Sync();
			Collection collection = sync.Collections.CollectionCollection.Add();
			collection.Class = DeltaSyncCommon.FolderCollectionName;
			collection.GetChanges = DeltaSyncRequestGenerator.GetChanges;
			collection.SyncKey = (folderSyncKey ?? DeltaSyncCommon.DefaultSyncKey);
			Collection collection2 = sync.Collections.CollectionCollection.Add();
			collection2.Class = DeltaSyncCommon.EmailCollectionName;
			collection2.GetChanges = DeltaSyncRequestGenerator.GetChanges;
			collection2.SyncKey = (emailSyncKey ?? DeltaSyncCommon.DefaultSyncKey);
			collection2.WindowSize = windowSize;
			this.SyncSerializer.Serialize(requestStream, sync);
		}

		internal void SetupSendMessageRequest(DeltaSyncMail deltaSyncEmail, bool saveInSentItems, DeltaSyncRecipients recipients, Stream requestStream)
		{
			Send o = DeltaSyncRequestGenerator.SetupXmlSendMessageRequest(deltaSyncEmail, saveInSentItems, recipients);
			using (Stream stream = TemporaryStorage.Create())
			{
				this.SendSerializer.Serialize(stream, o);
				stream.Position = 0L;
				DeltaSyncRequestGenerator.SetupMtomRequestWithXmlPartAndMessagePart(stream, deltaSyncEmail.EmailMessage, deltaSyncEmail.MessageIncludeContentId, requestStream);
			}
		}

		internal void SetupApplyChangesRequest(List<DeltaSyncOperation> operations, ConflictResolution conflictResolution, string folderSyncKey, string emailSyncKey, Stream requestStream)
		{
			List<DeltaSyncMail> list = new List<DeltaSyncMail>(operations.Count / 2);
			Sync sync = new Sync();
			sync.Options.Conflict = (byte)conflictResolution;
			Collection collection = sync.Collections.CollectionCollection.Add();
			collection.Class = DeltaSyncCommon.FolderCollectionName;
			collection.SyncKey = (folderSyncKey ?? DeltaSyncCommon.DefaultSyncKey);
			Collection collection2 = sync.Collections.CollectionCollection.Add();
			collection2.Class = DeltaSyncCommon.EmailCollectionName;
			collection2.SyncKey = (emailSyncKey ?? DeltaSyncCommon.DefaultSyncKey);
			foreach (DeltaSyncOperation deltaSyncOperation in operations)
			{
				if (!(deltaSyncOperation.DeltaSyncObject is DeltaSyncFolder) && !(deltaSyncOperation.DeltaSyncObject is DeltaSyncMail))
				{
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Unknown Object Type : {0}", new object[]
					{
						deltaSyncOperation.DeltaSyncObject.GetType()
					}));
				}
				switch (deltaSyncOperation.OperationType)
				{
				case DeltaSyncOperation.Type.Add:
					if (deltaSyncOperation.DeltaSyncObject is DeltaSyncFolder)
					{
						DeltaSyncFolder deltaSyncFolder = deltaSyncOperation.DeltaSyncObject as DeltaSyncFolder;
						Add add = collection.Commands.AddCollection.Add();
						add.ClientId = deltaSyncFolder.ClientId;
						add.ApplicationData.DisplayName.Value = deltaSyncFolder.DisplayName;
						add.ApplicationData.DisplayName.charset = DeltaSyncCommon.DefaultStringCharset;
						add.ApplicationData.Version2 = DeltaSyncCommon.DefaultEncodingVersion;
						add.ApplicationData.ParentId.isClientId = (deltaSyncFolder.Parent.IsClientObject ? Microsoft.Exchange.Net.Protocols.DeltaSync.HMFolder.bitType.one : Microsoft.Exchange.Net.Protocols.DeltaSync.HMFolder.bitType.zero);
						add.ApplicationData.ParentId.Value = deltaSyncFolder.Parent.Id;
					}
					else
					{
						DeltaSyncMail deltaSyncMail = deltaSyncOperation.DeltaSyncObject as DeltaSyncMail;
						DeltaSyncRequestGenerator.AddApplicationDataToEmailCollection(collection2.Commands.AddCollection, deltaSyncMail);
						list.Add(deltaSyncMail);
					}
					break;
				case DeltaSyncOperation.Type.Change:
					if (deltaSyncOperation.DeltaSyncObject is DeltaSyncFolder)
					{
						DeltaSyncFolder deltaSyncFolder = deltaSyncOperation.DeltaSyncObject as DeltaSyncFolder;
						Change change = collection.Commands.ChangeCollection.Add();
						change.ServerId = deltaSyncFolder.ServerId.ToString();
						change.ApplicationData.DisplayName.Value = deltaSyncFolder.DisplayName;
						change.ApplicationData.DisplayName.charset = DeltaSyncCommon.DefaultStringCharset;
						change.ApplicationData.Version2 = DeltaSyncCommon.DefaultEncodingVersion;
					}
					else
					{
						DeltaSyncMail deltaSyncMail = deltaSyncOperation.DeltaSyncObject as DeltaSyncMail;
						Change change2 = collection2.Commands.ChangeCollection.Add();
						change2.ServerId = deltaSyncMail.ServerId.ToString();
						change2.ApplicationData.Read = (deltaSyncMail.Read ? 1 : 0);
						if (deltaSyncMail.ReplyToOrForward != null)
						{
							change2.ApplicationData.ReplyToOrForwardState = (byte)deltaSyncMail.ReplyToOrForward.Value;
						}
					}
					break;
				case DeltaSyncOperation.Type.Delete:
				{
					Delete delete;
					if (deltaSyncOperation.DeltaSyncObject is DeltaSyncFolder)
					{
						delete = collection.Commands.DeleteCollection.Add();
					}
					else
					{
						delete = collection2.Commands.DeleteCollection.Add();
					}
					delete.ServerId = deltaSyncOperation.DeltaSyncObject.ServerId.ToString();
					break;
				}
				default:
					throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Change Type Not supported : {0}", new object[]
					{
						deltaSyncOperation.OperationType
					}));
				}
			}
			using (Stream stream = TemporaryStorage.Create())
			{
				this.SyncSerializer.Serialize(stream, sync);
				stream.Position = 0L;
				string value = Guid.NewGuid().ToString();
				MimeWriter mimeWriter = new MimeWriter(requestStream);
				mimeWriter.StartPart();
				mimeWriter.WriteHeader(HeaderId.MimeVersion, DeltaSyncCommon.VersionOneDotZero);
				mimeWriter.StartHeader(HeaderId.ContentType);
				mimeWriter.WriteHeaderValue(DeltaSyncCommon.MultipartRelated);
				mimeWriter.WriteParameter(DeltaSyncCommon.Boundary, Guid.NewGuid().ToString());
				mimeWriter.WriteParameter(DeltaSyncCommon.Type, DeltaSyncCommon.ApplicationXopXmlContentType);
				mimeWriter.WriteParameter(DeltaSyncCommon.Start, value);
				mimeWriter.WriteParameter(DeltaSyncCommon.StartInfo, DeltaSyncCommon.ApplicationXopXmlContentType);
				mimeWriter.StartPart();
				mimeWriter.StartHeader(HeaderId.ContentType);
				mimeWriter.WriteHeaderValue(DeltaSyncCommon.ApplicationXopXmlContentType);
				mimeWriter.WriteParameter(DeltaSyncCommon.Charset, Encoding.UTF8.WebName);
				mimeWriter.WriteParameter(DeltaSyncCommon.Type, DeltaSyncCommon.ApplicationXopXmlContentType);
				mimeWriter.WriteHeader(HeaderId.ContentTransferEncoding, DeltaSyncCommon.SevenBit);
				mimeWriter.WriteHeader(HeaderId.ContentId, value);
				mimeWriter.WriteContent(stream);
				mimeWriter.EndPart();
				foreach (DeltaSyncMail deltaSyncMail2 in list)
				{
					deltaSyncMail2.EmailMessage.Position = 0L;
					mimeWriter.StartPart();
					mimeWriter.WriteHeader(HeaderId.ContentType, DeltaSyncCommon.ApplicationRFC822);
					mimeWriter.WriteHeader(HeaderId.ContentTransferEncoding, DeltaSyncCommon.Binary);
					mimeWriter.WriteHeader(HeaderId.ContentId, deltaSyncMail2.MessageIncludeContentId);
					mimeWriter.WriteContent(deltaSyncMail2.EmailMessage);
					mimeWriter.EndPart();
				}
				mimeWriter.EndPart();
				mimeWriter.Flush();
			}
		}

		internal void SetupFetchMessageRequest(Guid serverId, Stream requestStream)
		{
			ItemOperations itemOperations = new ItemOperations();
			itemOperations.Fetch = new FetchType();
			itemOperations.Fetch.Class = DeltaSyncCommon.EmailCollectionName;
			itemOperations.Fetch.ServerId = serverId.ToString();
			itemOperations.Fetch.Compression = "hm-compression";
			itemOperations.Fetch.ResponseContentType = Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.ResponseContentTypeType.mtom;
			this.ItemOperationsSerializer.Serialize(requestStream, itemOperations);
		}

		internal void SetupGetSettingsRequest(Stream requestStream)
		{
			Settings settings = new Settings();
			settings.ServiceSettings = new ServiceSettingsType();
			settings.ServiceSettings.Properties = new Properties();
			settings.ServiceSettings.Properties.Get = new PropertiesGet();
			settings.AccountSettings = new AccountSettingsType();
			settings.AccountSettings.Get = new AccountSettingsTypeGet();
			settings.AccountSettings.Get.Properties = new AccountSettingsTypeGetProperties();
			this.SettingsSerializer.Serialize(requestStream, settings);
		}

		internal void SetupGetStatisticsRequest(Stream requestStream)
		{
			Stateless stateless = new Stateless();
			stateless.Collections = new StatelessCollection[]
			{
				new StatelessCollection
				{
					Class = DeltaSyncCommon.FolderCollectionName,
					Get = new StatelessCollectionGet()
				}
			};
			this.StatelessSerializer.Serialize(requestStream, stateless);
		}

		private static void SetupMtomRequestWithXmlPartAndMessagePart(Stream xmlPartSourceStream, Stream messagePartSourceStream, string messagePartContentId, Stream requestStream)
		{
			string value = Guid.NewGuid().ToString();
			MimeWriter mimeWriter = new MimeWriter(requestStream);
			mimeWriter.StartPart();
			mimeWriter.WriteHeader(HeaderId.MimeVersion, DeltaSyncCommon.VersionOneDotZero);
			mimeWriter.StartHeader(HeaderId.ContentType);
			mimeWriter.WriteHeaderValue(DeltaSyncCommon.MultipartRelated);
			mimeWriter.WriteParameter(DeltaSyncCommon.Boundary, Guid.NewGuid().ToString());
			mimeWriter.WriteParameter(DeltaSyncCommon.Type, DeltaSyncCommon.ApplicationXopXmlContentType);
			mimeWriter.WriteParameter(DeltaSyncCommon.Start, value);
			mimeWriter.WriteParameter(DeltaSyncCommon.StartInfo, DeltaSyncCommon.ApplicationXopXmlContentType);
			mimeWriter.StartPart();
			mimeWriter.StartHeader(HeaderId.ContentType);
			mimeWriter.WriteHeaderValue(DeltaSyncCommon.ApplicationXopXmlContentType);
			mimeWriter.WriteParameter(DeltaSyncCommon.Charset, Encoding.UTF8.WebName);
			mimeWriter.WriteParameter(DeltaSyncCommon.Type, DeltaSyncCommon.ApplicationXopXmlContentType);
			mimeWriter.WriteHeader(HeaderId.ContentTransferEncoding, DeltaSyncCommon.SevenBit);
			mimeWriter.WriteHeader(HeaderId.ContentId, value);
			mimeWriter.WriteContent(xmlPartSourceStream);
			mimeWriter.EndPart();
			mimeWriter.StartPart();
			mimeWriter.WriteHeader(HeaderId.ContentType, DeltaSyncCommon.ApplicationRFC822);
			mimeWriter.WriteHeader(HeaderId.ContentTransferEncoding, DeltaSyncCommon.Binary);
			mimeWriter.WriteHeader(HeaderId.ContentId, messagePartContentId);
			mimeWriter.WriteContent(messagePartSourceStream);
			mimeWriter.EndPart();
			mimeWriter.EndPart();
			mimeWriter.Flush();
		}

		private static void AddApplicationDataToEmailCollection(AddCollection addCollection, DeltaSyncMail deltaSyncEmail)
		{
			Add add = addCollection.Add();
			add.ClientId = deltaSyncEmail.ClientId;
			add.ApplicationData.From.Value = deltaSyncEmail.From;
			add.ApplicationData.Subject.Value = deltaSyncEmail.Subject;
			add.ApplicationData.DateReceived = deltaSyncEmail.DateReceivedUniversalTimeString;
			add.ApplicationData.HasAttachments = (deltaSyncEmail.HasAttachments ? Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.bitType.one : Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.bitType.zero);
			add.ApplicationData.FolderId.isClientId = (deltaSyncEmail.Parent.IsClientObject ? Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.bitType.one : Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.bitType.zero);
			add.ApplicationData.FolderId.Value = deltaSyncEmail.Parent.Id;
			add.ApplicationData.MessageClass = deltaSyncEmail.MessageClass;
			add.ApplicationData.Importance = (byte)deltaSyncEmail.Importance;
			add.ApplicationData.ConversationTopic.Value = deltaSyncEmail.ConversationTopic;
			add.ApplicationData.ConversationIndex.Value = deltaSyncEmail.ConversationIndex;
			add.ApplicationData.Sensitivity = (byte)deltaSyncEmail.Sensitivity;
			add.ApplicationData.Read = (deltaSyncEmail.Read ? 1 : 0);
			add.ApplicationData.Size = deltaSyncEmail.Size;
			add.ApplicationData.TrustedSource = Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.bitType.one;
			add.ApplicationData.IsFromSomeoneAddressBook = Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.bitType.zero;
			add.ApplicationData.IsToAllowList = Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.bitType.zero;
			add.ApplicationData.Version = DeltaSyncCommon.DefaultEncodingVersion;
			add.ApplicationData.ReplyToOrForwardState = (byte)((deltaSyncEmail.ReplyToOrForward != null) ? deltaSyncEmail.ReplyToOrForward.Value : DeltaSyncMail.ReplyToOrForwardState.None);
			add.ApplicationData.Categories = DeltaSyncRequestGenerator.Categories;
			add.ApplicationData.ConfirmedJunk = Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.bitType.zero;
			add.ApplicationData.Flag = DeltaSyncRequestGenerator.Flag;
			add.ApplicationData.Message.Include.href = deltaSyncEmail.MessageIncludeContentId;
		}

		private static Send SetupXmlSendMessageRequest(DeltaSyncMail deltaSyncEmail, bool saveInSentItems, DeltaSyncRecipients recipients)
		{
			Send send = new Send();
			send.SendItem.Class = DeltaSyncCommon.EmailCollectionName;
			send.SendItem.Recipients = new Recipients();
			foreach (string obj in recipients.To)
			{
				send.SendItem.Recipients.To.Add(obj);
			}
			foreach (string obj2 in recipients.Cc)
			{
				send.SendItem.Recipients.Cc.Add(obj2);
			}
			foreach (string obj3 in recipients.Bcc)
			{
				send.SendItem.Recipients.Bcc.Add(obj3);
			}
			send.SendItem.Item.Include.href = deltaSyncEmail.MessageIncludeContentId;
			if (saveInSentItems)
			{
				send.SaveItem = DeltaSyncRequestGenerator.SaveItem;
			}
			return send;
		}

		private const string HMCompression = "hm-compression";

		private static readonly SaveItem SaveItem = new SaveItem();

		private static readonly GetChanges GetChanges = new GetChanges();

		private static readonly Categories Categories = new Categories();

		private static readonly Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.Flag Flag = new Microsoft.Exchange.Net.Protocols.DeltaSync.HMMail.Flag();

		private XmlSerializer syncSerializer;

		private XmlSerializer itemOperationsSerializer;

		private XmlSerializer settingsSerializer;

		private XmlSerializer sendSerializer;

		private XmlSerializer statelessSerializer;
	}
}
