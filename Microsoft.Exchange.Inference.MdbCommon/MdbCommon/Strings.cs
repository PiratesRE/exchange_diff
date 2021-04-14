using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Inference.MdbCommon
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(3777091915U, "NestedDocumentCountZero");
			Strings.stringIDs.Add(823938420U, "MissingReceivedTime");
			Strings.stringIDs.Add(2612598579U, "InvalidDocumentInTrainingSet");
			Strings.stringIDs.Add(266918677U, "FailedToOpenActivityLog");
			Strings.stringIDs.Add(254348434U, "AbortOnProcessingRequested");
			Strings.stringIDs.Add(662878388U, "MissingConversationId");
			Strings.stringIDs.Add(2549017857U, "AdRecipientNotFound");
			Strings.stringIDs.Add(1492146775U, "InvalidAdRecipient");
			Strings.stringIDs.Add(1292225851U, "SaveWithNoItemError");
			Strings.stringIDs.Add(1800498867U, "MissingSender");
			Strings.stringIDs.Add(3003303052U, "MissingMailboxOwnerProperty");
			Strings.stringIDs.Add(752028432U, "NullDocumentProcessingContext");
		}

		public static LocalizedString MissingDumpsterIdInFolderList(string mbx)
		{
			return new LocalizedString("MissingDumpsterIdInFolderList", Strings.ResourceManager, new object[]
			{
				mbx
			});
		}

		public static LocalizedString NestedDocumentCountZero
		{
			get
			{
				return new LocalizedString("NestedDocumentCountZero", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingReceivedTime
		{
			get
			{
				return new LocalizedString("MissingReceivedTime", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDocumentInTrainingSet
		{
			get
			{
				return new LocalizedString("InvalidDocumentInTrainingSet", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingFolderId(string mbx)
		{
			return new LocalizedString("MissingFolderId", Strings.ResourceManager, new object[]
			{
				mbx
			});
		}

		public static LocalizedString FailedToOpenActivityLog
		{
			get
			{
				return new LocalizedString("FailedToOpenActivityLog", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConnectionToMailboxFailed(Guid mbxGuid)
		{
			return new LocalizedString("ConnectionToMailboxFailed", Strings.ResourceManager, new object[]
			{
				mbxGuid
			});
		}

		public static LocalizedString SetPropertyFailed(string property)
		{
			return new LocalizedString("SetPropertyFailed", Strings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString AbortOnProcessingRequested
		{
			get
			{
				return new LocalizedString("AbortOnProcessingRequested", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingDeletedId(string mbx)
		{
			return new LocalizedString("MissingDeletedId", Strings.ResourceManager, new object[]
			{
				mbx
			});
		}

		public static LocalizedString MissingConversationId
		{
			get
			{
				return new LocalizedString("MissingConversationId", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PropertyMappingFailed(string property)
		{
			return new LocalizedString("PropertyMappingFailed", Strings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString AdRecipientNotFound
		{
			get
			{
				return new LocalizedString("AdRecipientNotFound", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingDeletedIdInFolderList(string mbx)
		{
			return new LocalizedString("MissingDeletedIdInFolderList", Strings.ResourceManager, new object[]
			{
				mbx
			});
		}

		public static LocalizedString GetPropertyAsStreamFailed(string property)
		{
			return new LocalizedString("GetPropertyAsStreamFailed", Strings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString InvalidAdRecipient
		{
			get
			{
				return new LocalizedString("InvalidAdRecipient", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingInboxId(string mbx)
		{
			return new LocalizedString("MissingInboxId", Strings.ResourceManager, new object[]
			{
				mbx
			});
		}

		public static LocalizedString MissingInboxIdInFolderList(string mbx)
		{
			return new LocalizedString("MissingInboxIdInFolderList", Strings.ResourceManager, new object[]
			{
				mbx
			});
		}

		public static LocalizedString SaveWithNoItemError
		{
			get
			{
				return new LocalizedString("SaveWithNoItemError", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingSender
		{
			get
			{
				return new LocalizedString("MissingSender", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingMailboxOwnerProperty
		{
			get
			{
				return new LocalizedString("MissingMailboxOwnerProperty", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NullDocumentProcessingContext
		{
			get
			{
				return new LocalizedString("NullDocumentProcessingContext", Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(12);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Inference.MdbCommon.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			NestedDocumentCountZero = 3777091915U,
			MissingReceivedTime = 823938420U,
			InvalidDocumentInTrainingSet = 2612598579U,
			FailedToOpenActivityLog = 266918677U,
			AbortOnProcessingRequested = 254348434U,
			MissingConversationId = 662878388U,
			AdRecipientNotFound = 2549017857U,
			InvalidAdRecipient = 1492146775U,
			SaveWithNoItemError = 1292225851U,
			MissingSender = 1800498867U,
			MissingMailboxOwnerProperty = 3003303052U,
			NullDocumentProcessingContext = 752028432U
		}

		private enum ParamIDs
		{
			MissingDumpsterIdInFolderList,
			MissingFolderId,
			ConnectionToMailboxFailed,
			SetPropertyFailed,
			MissingDeletedId,
			PropertyMappingFailed,
			MissingDeletedIdInFolderList,
			GetPropertyAsStreamFailed,
			MissingInboxId,
			MissingInboxIdInFolderList
		}
	}
}
