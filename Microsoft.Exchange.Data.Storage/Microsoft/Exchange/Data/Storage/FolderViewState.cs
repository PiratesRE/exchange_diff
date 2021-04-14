using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Data.Storage
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FolderViewState
	{
		internal FolderViewState()
		{
			this.Init();
		}

		[DataMember(Name = "FolderId")]
		public FolderIdType FolderId { get; set; }

		[DataMember(Name = "View", IsRequired = false)]
		public FolderViewType View { get; set; }

		[DataMember(Name = "SortOrder", IsRequired = false)]
		public SortOrder SortOrder { get; set; }

		[DataMember(Name = "SortColumn", IsRequired = false)]
		public FolderViewColumnId SortColumn { get; set; }

		public static FolderViewState FindViewState(string[] folderViewStates, string folderId)
		{
			if (folderViewStates != null)
			{
				DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(FolderViewState));
				foreach (string s in folderViewStates)
				{
					try
					{
						using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(s)))
						{
							FolderViewState folderViewState = dataContractJsonSerializer.ReadObject(memoryStream) as FolderViewState;
							if (folderViewState != null && folderViewState.FolderId != null && folderViewState.FolderId.Id == folderId)
							{
								return folderViewState;
							}
						}
					}
					catch (IOException)
					{
					}
					catch (InvalidDataContractException)
					{
					}
					catch (SerializationException)
					{
					}
				}
			}
			return FolderViewState.Default;
		}

		public SortBy[] ConvertToSortBy()
		{
			return MailSortOptions.GetSortByForFolderViewState(this) ?? FolderViewState.defaultSortBy;
		}

		[OnDeserializing]
		private void SetValuesOnDeserializing(StreamingContext streamingContext)
		{
			this.Init();
		}

		private void Init()
		{
			this.FolderId = new FolderIdType();
			this.SortColumn = FolderViewColumnId.DateTime;
			this.SortOrder = SortOrder.Descending;
			this.View = FolderViewType.ConversationView;
		}

		public static FolderViewState Default = new FolderViewState();

		private static SortBy[] defaultSortBy = new SortBy[]
		{
			new SortBy(ItemSchema.ReceivedTime, SortOrder.Descending)
		};
	}
}
