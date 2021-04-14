using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class VotingInfo
	{
		internal VotingInfo(MessageItem messageItem)
		{
			this.messageItem = messageItem;
			byte[] largeBinaryProperty = this.messageItem.PropertyBag.GetLargeBinaryProperty(InternalSchema.OutlookUserPropsVerbStream);
			if (largeBinaryProperty != null)
			{
				using (MemoryStream memoryStream = new MemoryStream(largeBinaryProperty))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						this.Read(binaryReader);
					}
				}
			}
		}

		private void InitializeDefaults()
		{
			if (this.defaultOptions.Count == 0)
			{
				this.defaultOptions.Add(VotingInfo.VotingOption.ReplyOption);
				this.defaultOptions.Add(VotingInfo.VotingOption.ReplyAllOption);
				this.defaultOptions.Add(VotingInfo.VotingOption.ForwardOption);
				this.defaultOptions.Add(VotingInfo.VotingOption.ReplyToFolderOption);
			}
		}

		public IList<string> GetOptionsList()
		{
			string[] array = new string[this.userOptions.Count];
			for (int i = 0; i < this.userOptions.Count; i++)
			{
				array[i] = this.userOptions[i].DisplayName;
			}
			return array;
		}

		public IList<VotingInfo.OptionData> GetOptionsDataList()
		{
			VotingInfo.OptionData[] array = new VotingInfo.OptionData[this.userOptions.Count];
			for (int i = 0; i < this.userOptions.Count; i++)
			{
				array[i] = this.userOptions[i].OptionData;
			}
			return array;
		}

		public void AddOption(string optionDisplayName)
		{
			VotingInfo.OptionData data;
			data.DisplayName = optionDisplayName;
			data.SendPrompt = VotingInfo.SendPrompt.VotingOption;
			this.AddOption(data);
		}

		public void AddOption(VotingInfo.OptionData data)
		{
			this.InitializeDefaults();
			if (this.userOptions.Count == 31)
			{
				throw new ArgumentException("Can't add more options", data.DisplayName);
			}
			this.userOptions.Add(new VotingInfo.VotingOption(data, this.userOptions.Count + 1));
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					this.Write(binaryWriter);
				}
				this.messageItem[InternalSchema.OutlookUserPropsVerbStream] = memoryStream.ToArray();
				this.messageItem[InternalSchema.IsReplyRequested] = true;
				this.messageItem[InternalSchema.IsResponseRequested] = true;
			}
		}

		public VersionedId GetCorrelatedItem(StoreId folderId)
		{
			Util.ThrowOnNullArgument(folderId, "folderId");
			VersionedId correlatedItem;
			using (Folder folder = Folder.Bind(this.messageItem.Session, folderId))
			{
				correlatedItem = this.GetCorrelatedItem(folder);
			}
			return correlatedItem;
		}

		public VersionedId GetCorrelatedItem(Folder folder)
		{
			Util.ThrowOnNullArgument(folder, "folder");
			try
			{
				byte[] messageCorrelationBlob = this.MessageCorrelationBlob;
				if (messageCorrelationBlob != null)
				{
					using (QueryResult queryResult = folder.ItemQuery(ItemQueryType.None, null, new SortBy[]
					{
						new SortBy(InternalSchema.ReportTag, SortOrder.Ascending)
					}, new PropertyDefinition[]
					{
						InternalSchema.ItemId,
						InternalSchema.ReportTag
					}))
					{
						queryResult.SeekToCondition(SeekReference.OriginBeginning, new ComparisonFilter(ComparisonOperator.Equal, InternalSchema.ReportTag, messageCorrelationBlob));
						object[][] rows = queryResult.GetRows(1);
						if (rows.Length == 1)
						{
							byte[] array = rows[0][1] as byte[];
							if (array != null && ArrayComparer<byte>.Comparer.Equals(messageCorrelationBlob, array))
							{
								return rows[0][0] as VersionedId;
							}
						}
					}
				}
			}
			catch (CorruptDataException)
			{
			}
			return null;
		}

		public byte[] MessageCorrelationBlob
		{
			get
			{
				object obj = this.messageItem.TryGetProperty(InternalSchema.ReportTag);
				PropertyError propertyError = obj as PropertyError;
				if (propertyError != null && propertyError.PropertyErrorCode == PropertyErrorCode.RequireStreamed)
				{
					throw new CorruptDataException(ServerStrings.ExCorruptMessageCorrelationBlob);
				}
				byte[] array = obj as byte[];
				if (array != null && array.Length > 250)
				{
					throw new CorruptDataException(ServerStrings.ExCorruptMessageCorrelationBlob);
				}
				return array;
			}
			set
			{
				Util.ThrowOnNullArgument(value, "MessageCorrelationBlob");
				if (value.Length == 0 || value.Length > 250)
				{
					throw new ArgumentException("MessageCorrelationBlob.Length > 0 && < 250");
				}
				this.messageItem.SafeSetProperty(InternalSchema.ReportTag, value);
			}
		}

		public string Response
		{
			get
			{
				return this.messageItem.PropertyBag.GetValueOrDefault<string>(InternalSchema.VotingResponse);
			}
		}

		private void Read(BinaryReader reader)
		{
			try
			{
				List<VotingInfo.VotingOption> list = new List<VotingInfo.VotingOption>();
				VotingInfo.VotingBlobVersion votingBlobVersion = (VotingInfo.VotingBlobVersion)reader.ReadUInt16();
				if (votingBlobVersion != VotingInfo.VotingBlobVersion.AsciiBlob)
				{
					throw new CorruptDataException(ServerStrings.VotingDataCorrupt);
				}
				for (int i = reader.ReadInt32(); i > 0; i--)
				{
					VotingInfo.VotingOption item = VotingInfo.VotingOption.ReadVersion102(reader);
					list.Add(item);
				}
				if (reader.BaseStream.Position < reader.BaseStream.Length)
				{
					votingBlobVersion = (VotingInfo.VotingBlobVersion)reader.ReadUInt16();
					if (votingBlobVersion >= VotingInfo.VotingBlobVersion.UnicodeBlob)
					{
						for (int j = 0; j < list.Count; j++)
						{
							list[j].ReadVersion104(reader);
							if (list[j].IsDefaultOption)
							{
								this.defaultOptions.Add(list[j]);
							}
							else
							{
								this.userOptions.Add(list[j]);
							}
						}
					}
				}
			}
			catch (EndOfStreamException innerException)
			{
				throw new CorruptDataException(ServerStrings.ExVotingBlobCorrupt, innerException);
			}
		}

		private void Write(BinaryWriter writer)
		{
			writer.Write(258);
			writer.Write(this.defaultOptions.Count + this.userOptions.Count);
			for (int i = 0; i < this.defaultOptions.Count; i++)
			{
				this.defaultOptions[i].WriteVersion102(writer);
			}
			for (int j = 0; j < this.userOptions.Count; j++)
			{
				this.userOptions[j].WriteVersion102(writer);
			}
			writer.Write(260);
			for (int k = 0; k < this.defaultOptions.Count; k++)
			{
				this.defaultOptions[k].WriteVersion104(writer);
			}
			for (int l = 0; l < this.userOptions.Count; l++)
			{
				this.userOptions[l].WriteVersion104(writer);
			}
		}

		private List<VotingInfo.VotingOption> defaultOptions = new List<VotingInfo.VotingOption>();

		private List<VotingInfo.VotingOption> userOptions = new List<VotingInfo.VotingOption>();

		private MessageItem messageItem;

		[Flags]
		private enum ReplyStyle
		{
			Omit = 0,
			Embed = 1,
			Include = 2,
			Indent = 4
		}

		private enum VotingBlobVersion : ushort
		{
			AsciiBlob = 258,
			UnicodeBlob = 260
		}

		private enum UIVerb
		{
			SendPrompt = 2
		}

		private enum ShowVerb
		{
			MenuAndToolbar = 2
		}

		private enum PropertyCopyLike
		{
			Reply,
			ReplyAll,
			Forward,
			ReplyToFolder,
			VotingButton
		}

		public enum SendPrompt
		{
			None,
			Send,
			VotingOption
		}

		public struct OptionData
		{
			public string DisplayName;

			public VotingInfo.SendPrompt SendPrompt;
		}

		private class VotingOption
		{
			internal VotingOption(VotingInfo.OptionData data, int verbId) : this(data.DisplayName, verbId)
			{
				this.sendPrompt = data.SendPrompt;
			}

			internal VotingOption(string votingOption, int verbId)
			{
				Util.ThrowOnNullArgument(votingOption, "votingOption");
				if (votingOption.Length == 0)
				{
					throw new ArgumentException("invalid argument", "votingOption");
				}
				this.displayName = votingOption;
				this.actionPrefix = this.displayName;
				this.actionMessageClass = "IPM.Note";
				this.actionFormName = string.Empty;
				this.actionTemplateName = string.Empty;
				this.propertyCopyLike = VotingInfo.PropertyCopyLike.VotingButton;
				this.replyStyle = VotingInfo.ReplyStyle.Omit;
				this.showVerb = VotingInfo.ShowVerb.MenuAndToolbar;
				this.enabled = true;
				this.sendPrompt = VotingInfo.SendPrompt.VotingOption;
				this.verbId = (LastAction)verbId;
				this.verbPosition = -1;
			}

			private VotingOption()
			{
			}

			private static string ReadString(BinaryReader reader, bool unicode)
			{
				ushort num = (ushort)reader.ReadByte();
				if (num == 255)
				{
					num = reader.ReadUInt16();
				}
				if (unicode)
				{
					byte[] array = reader.ReadBytes((int)(num * 2));
					return VotingInfo.VotingOption.unicodeEncoder.GetString(array, 0, array.Length);
				}
				byte[] array2 = reader.ReadBytes((int)num);
				return VotingInfo.VotingOption.asciiEncoder.GetString(array2, 0, array2.Length);
			}

			private static void WriteString(BinaryWriter writer, string str, bool unicode)
			{
				if (str.Length >= 255)
				{
					writer.Write(byte.MaxValue);
					writer.Write((ushort)str.Length);
				}
				else
				{
					writer.Write((byte)str.Length);
				}
				if (unicode)
				{
					writer.Write(VotingInfo.VotingOption.unicodeEncoder.GetBytes(str));
					return;
				}
				writer.Write(VotingInfo.VotingOption.asciiEncoder.GetBytes(str));
			}

			internal static VotingInfo.VotingOption ReadVersion102(BinaryReader reader)
			{
				return new VotingInfo.VotingOption
				{
					propertyCopyLike = (VotingInfo.PropertyCopyLike)reader.ReadUInt32(),
					displayName = VotingInfo.VotingOption.ReadString(reader, false),
					actionMessageClass = VotingInfo.VotingOption.ReadString(reader, false),
					actionFormName = VotingInfo.VotingOption.ReadString(reader, false),
					actionPrefix = VotingInfo.VotingOption.ReadString(reader, false),
					replyStyle = (VotingInfo.ReplyStyle)reader.ReadUInt32(),
					actionTemplateName = VotingInfo.VotingOption.ReadString(reader, false),
					usePrefixHeader = (reader.ReadUInt32() != 0U),
					enabled = (reader.ReadUInt32() != 0U),
					sendPrompt = (VotingInfo.SendPrompt)reader.ReadUInt32(),
					showVerb = (VotingInfo.ShowVerb)reader.ReadInt32(),
					verbId = (LastAction)reader.ReadUInt32(),
					verbPosition = reader.ReadInt32()
				};
			}

			internal void ReadVersion104(BinaryReader reader)
			{
				this.displayName = VotingInfo.VotingOption.ReadString(reader, true);
				this.actionPrefix = VotingInfo.VotingOption.ReadString(reader, true);
			}

			internal void WriteVersion102(BinaryWriter writer)
			{
				writer.Write((int)this.propertyCopyLike);
				VotingInfo.VotingOption.WriteString(writer, this.displayName, false);
				VotingInfo.VotingOption.WriteString(writer, this.actionMessageClass, false);
				VotingInfo.VotingOption.WriteString(writer, this.actionFormName, false);
				VotingInfo.VotingOption.WriteString(writer, this.actionPrefix, false);
				writer.Write((int)this.replyStyle);
				VotingInfo.VotingOption.WriteString(writer, this.actionTemplateName, false);
				writer.Write(this.usePrefixHeader ? 1U : 0U);
				writer.Write(this.enabled ? 1U : 0U);
				writer.Write((int)this.sendPrompt);
				writer.Write((int)this.showVerb);
				writer.Write((int)this.verbId);
				writer.Write(this.verbPosition);
			}

			internal void WriteVersion104(BinaryWriter writer)
			{
				VotingInfo.VotingOption.WriteString(writer, this.displayName, true);
				VotingInfo.VotingOption.WriteString(writer, this.actionPrefix, true);
			}

			public string DisplayName
			{
				get
				{
					return this.displayName;
				}
			}

			public VotingInfo.OptionData OptionData
			{
				get
				{
					VotingInfo.OptionData result;
					result.SendPrompt = this.sendPrompt;
					result.DisplayName = this.displayName;
					return result;
				}
			}

			internal bool IsDefaultOption
			{
				get
				{
					return this.verbId == LastAction.ReplyToSender || this.verbId == LastAction.ReplyToAll || this.verbId == LastAction.Forward || this.verbId == LastAction.ReplyToFolder;
				}
			}

			private static VotingInfo.VotingOption GetReplyOption()
			{
				return new VotingInfo.VotingOption("Reply", 0)
				{
					propertyCopyLike = VotingInfo.PropertyCopyLike.Reply,
					actionPrefix = "RE",
					actionMessageClass = "IPM.Note",
					actionFormName = "Message",
					replyStyle = (VotingInfo.ReplyStyle.Embed | VotingInfo.ReplyStyle.Indent),
					usePrefixHeader = false,
					enabled = true,
					sendPrompt = VotingInfo.SendPrompt.None,
					showVerb = VotingInfo.ShowVerb.MenuAndToolbar,
					verbId = LastAction.ReplyToSender,
					verbPosition = 2
				};
			}

			private static VotingInfo.VotingOption GetReplyAllOption()
			{
				return new VotingInfo.VotingOption("Reply to All", 0)
				{
					propertyCopyLike = VotingInfo.PropertyCopyLike.ReplyAll,
					actionPrefix = "RE",
					actionMessageClass = "IPM.Note",
					actionFormName = "Message",
					replyStyle = (VotingInfo.ReplyStyle.Embed | VotingInfo.ReplyStyle.Indent),
					usePrefixHeader = false,
					enabled = true,
					sendPrompt = VotingInfo.SendPrompt.None,
					showVerb = VotingInfo.ShowVerb.MenuAndToolbar,
					verbId = LastAction.ReplyToAll,
					verbPosition = 3
				};
			}

			private static VotingInfo.VotingOption GetForwardOption()
			{
				return new VotingInfo.VotingOption("Forward", 0)
				{
					propertyCopyLike = VotingInfo.PropertyCopyLike.Forward,
					actionPrefix = "FW",
					actionMessageClass = "IPM.Note",
					actionFormName = "Message",
					replyStyle = (VotingInfo.ReplyStyle.Embed | VotingInfo.ReplyStyle.Indent),
					usePrefixHeader = false,
					enabled = true,
					sendPrompt = VotingInfo.SendPrompt.None,
					showVerb = VotingInfo.ShowVerb.MenuAndToolbar,
					verbId = LastAction.Forward,
					verbPosition = 4
				};
			}

			private static VotingInfo.VotingOption GetReplyToFolderOption()
			{
				return new VotingInfo.VotingOption("Reply to Folder", 0)
				{
					propertyCopyLike = VotingInfo.PropertyCopyLike.ReplyToFolder,
					actionPrefix = string.Empty,
					actionMessageClass = "IPM.Post",
					actionFormName = "Post",
					replyStyle = (VotingInfo.ReplyStyle.Embed | VotingInfo.ReplyStyle.Indent),
					usePrefixHeader = false,
					enabled = true,
					sendPrompt = VotingInfo.SendPrompt.None,
					showVerb = VotingInfo.ShowVerb.MenuAndToolbar,
					verbId = LastAction.ReplyToFolder,
					verbPosition = 8
				};
			}

			private static readonly UnicodeEncoding unicodeEncoder = new UnicodeEncoding();

			private static readonly Encoding asciiEncoder = CTSGlobals.AsciiEncoding;

			private string displayName;

			private string actionPrefix;

			private string actionMessageClass;

			private string actionFormName;

			private string actionTemplateName;

			private VotingInfo.ReplyStyle replyStyle;

			private VotingInfo.ShowVerb showVerb;

			private VotingInfo.PropertyCopyLike propertyCopyLike;

			private bool usePrefixHeader;

			private bool enabled;

			private VotingInfo.SendPrompt sendPrompt;

			private LastAction verbId;

			private int verbPosition;

			internal static readonly VotingInfo.VotingOption ReplyOption = VotingInfo.VotingOption.GetReplyOption();

			internal static readonly VotingInfo.VotingOption ReplyAllOption = VotingInfo.VotingOption.GetReplyAllOption();

			internal static readonly VotingInfo.VotingOption ForwardOption = VotingInfo.VotingOption.GetForwardOption();

			internal static readonly VotingInfo.VotingOption ReplyToFolderOption = VotingInfo.VotingOption.GetReplyToFolderOption();
		}
	}
}
