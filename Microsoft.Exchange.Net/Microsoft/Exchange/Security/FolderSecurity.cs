using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security
{
	public static class FolderSecurity
	{
		public static FolderSecurity.ExchangeFolderRights NormalizeFolderRights(FolderSecurity.ExchangeFolderRights folderRights)
		{
			FolderSecurity.ExchangeFolderRights exchangeFolderRights = folderRights & FolderSecurity.ExchangeFolderRights.FreeBusyAll;
			folderRights &= ~(FolderSecurity.ExchangeFolderRights.FreeBusySimple | FolderSecurity.ExchangeFolderRights.FreeBusyDetailed);
			FolderSecurity.ExchangeFolderRights exchangeFolderRights2 = FolderSecurity.FolderRightsFromSecurityDescriptorRights(FolderSecurity.SecurityDescriptorRightsFromFolderRights(folderRights, FolderSecurity.AceTarget.Folder), FolderSecurity.AceTarget.Folder);
			FolderSecurity.ExchangeFolderRights exchangeFolderRights3 = FolderSecurity.FolderRightsFromSecurityDescriptorRights(FolderSecurity.SecurityDescriptorRightsFromFolderRights(folderRights, FolderSecurity.AceTarget.Message), FolderSecurity.AceTarget.Message);
			return exchangeFolderRights2 | exchangeFolderRights3 | exchangeFolderRights;
		}

		public static FolderSecurity.ExchangeFolderRights FolderRightsFromSecurityDescriptorRights(FolderSecurity.ExchangeSecurityDescriptorFolderRights sdRights, FolderSecurity.AceTarget aceTarget)
		{
			FolderSecurity.ExchangeFolderRights exchangeFolderRights = FolderSecurity.ExchangeFolderRights.None;
			if (aceTarget == FolderSecurity.AceTarget.Folder)
			{
				if ((sdRights & FolderSecurity.ExchangeSecurityDescriptorFolderRights.AppendMsg) == FolderSecurity.ExchangeSecurityDescriptorFolderRights.AppendMsg)
				{
					exchangeFolderRights |= FolderSecurity.ExchangeFolderRights.CreateSubfolder;
				}
				if ((sdRights & FolderSecurity.ExchangeSecurityDescriptorFolderRights.Contact) == FolderSecurity.ExchangeSecurityDescriptorFolderRights.Contact)
				{
					exchangeFolderRights |= FolderSecurity.ExchangeFolderRights.Contact;
				}
				if ((sdRights & FolderSecurity.ExchangeSecurityDescriptorFolderRights.FolderOwner) == FolderSecurity.ExchangeSecurityDescriptorFolderRights.FolderOwner)
				{
					exchangeFolderRights |= FolderSecurity.ExchangeFolderRights.Owner;
				}
				if ((sdRights & FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteBody) == FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteBody)
				{
					exchangeFolderRights |= FolderSecurity.ExchangeFolderRights.Create;
				}
				if ((sdRights & FolderSecurity.ExchangeSecurityDescriptorFolderRights.ViewItem) == FolderSecurity.ExchangeSecurityDescriptorFolderRights.ViewItem)
				{
					exchangeFolderRights |= FolderSecurity.ExchangeFolderRights.Visible;
				}
			}
			else if (aceTarget == FolderSecurity.AceTarget.Message)
			{
				if ((sdRights & FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty) == FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty)
				{
					exchangeFolderRights |= FolderSecurity.ExchangeFolderRights.ReadAny;
				}
				if ((sdRights & FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteOwnProperty) == FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteOwnProperty)
				{
					exchangeFolderRights |= FolderSecurity.ExchangeFolderRights.EditOwned;
				}
				if ((sdRights & FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteProperty) == FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteProperty)
				{
					exchangeFolderRights |= FolderSecurity.ExchangeFolderRights.EditAny;
				}
				if ((sdRights & FolderSecurity.ExchangeSecurityDescriptorFolderRights.DeleteOwnItem) == FolderSecurity.ExchangeSecurityDescriptorFolderRights.DeleteOwnItem)
				{
					exchangeFolderRights |= FolderSecurity.ExchangeFolderRights.DeleteOwned;
				}
				if ((sdRights & FolderSecurity.ExchangeSecurityDescriptorFolderRights.Delete) == FolderSecurity.ExchangeSecurityDescriptorFolderRights.Delete)
				{
					exchangeFolderRights |= FolderSecurity.ExchangeFolderRights.DeleteAny;
				}
			}
			else if (aceTarget == FolderSecurity.AceTarget.FreeBusy)
			{
				if ((sdRights & FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadBody) == FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadBody)
				{
					exchangeFolderRights |= FolderSecurity.ExchangeFolderRights.FreeBusySimple;
				}
				if ((sdRights & FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteBody) == FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteBody)
				{
					exchangeFolderRights |= FolderSecurity.ExchangeFolderRights.FreeBusyDetailed;
				}
			}
			return exchangeFolderRights;
		}

		private static FolderSecurity.ExchangeSecurityDescriptorFolderRights SecurityDescriptorRightsFromFolderRights(FolderSecurity.ExchangeFolderRights folderRights, FolderSecurity.AceTarget aceTarget)
		{
			FolderSecurity.ExchangeSecurityDescriptorFolderRights exchangeSecurityDescriptorFolderRights = FolderSecurity.ExchangeSecurityDescriptorFolderRights.None;
			if (aceTarget == FolderSecurity.AceTarget.Folder)
			{
				if ((folderRights & FolderSecurity.ExchangeFolderRights.CreateSubfolder) == FolderSecurity.ExchangeFolderRights.CreateSubfolder)
				{
					exchangeSecurityDescriptorFolderRights |= FolderSecurity.ExchangeSecurityDescriptorFolderRights.AppendMsg;
				}
				if ((folderRights & FolderSecurity.ExchangeFolderRights.Owner) == FolderSecurity.ExchangeFolderRights.Owner)
				{
					exchangeSecurityDescriptorFolderRights |= FolderSecurity.ExchangeSecurityDescriptorFolderRights.FolderOwner;
				}
				if ((folderRights & FolderSecurity.ExchangeFolderRights.Contact) == FolderSecurity.ExchangeFolderRights.Contact)
				{
					exchangeSecurityDescriptorFolderRights |= FolderSecurity.ExchangeSecurityDescriptorFolderRights.Contact;
				}
				if ((folderRights & FolderSecurity.ExchangeFolderRights.Create) == FolderSecurity.ExchangeFolderRights.Create)
				{
					exchangeSecurityDescriptorFolderRights |= FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteBody;
				}
				if ((folderRights & FolderSecurity.ExchangeFolderRights.Visible) == FolderSecurity.ExchangeFolderRights.Visible)
				{
					exchangeSecurityDescriptorFolderRights |= (FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadBody | FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty | FolderSecurity.ExchangeSecurityDescriptorFolderRights.Execute | FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadAttributes | FolderSecurity.ExchangeSecurityDescriptorFolderRights.ViewItem | FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadControl | FolderSecurity.ExchangeSecurityDescriptorFolderRights.Synchronize);
				}
				if ((folderRights & (FolderSecurity.ExchangeFolderRights.ReadAny | FolderSecurity.ExchangeFolderRights.Owner)) != FolderSecurity.ExchangeFolderRights.None)
				{
					exchangeSecurityDescriptorFolderRights |= FolderSecurity.ExchangeSecurityDescriptorFolderRights.ViewItem;
				}
			}
			else if (aceTarget == FolderSecurity.AceTarget.Message)
			{
				if ((folderRights & FolderSecurity.ExchangeFolderRights.EditAny) == FolderSecurity.ExchangeFolderRights.EditAny)
				{
					exchangeSecurityDescriptorFolderRights |= (FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteBody | FolderSecurity.ExchangeSecurityDescriptorFolderRights.AppendMsg | FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteProperty | FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteAttributes | FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadControl | FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteSD | FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteOwner | FolderSecurity.ExchangeSecurityDescriptorFolderRights.Synchronize);
				}
				if ((folderRights & FolderSecurity.ExchangeFolderRights.DeleteAny) == FolderSecurity.ExchangeFolderRights.DeleteAny)
				{
					exchangeSecurityDescriptorFolderRights |= FolderSecurity.ExchangeSecurityDescriptorFolderRights.Delete;
				}
				if ((folderRights & FolderSecurity.ExchangeFolderRights.EditOwned) == FolderSecurity.ExchangeFolderRights.EditOwned)
				{
					exchangeSecurityDescriptorFolderRights |= FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteOwnProperty;
				}
				if ((folderRights & FolderSecurity.ExchangeFolderRights.DeleteOwned) == FolderSecurity.ExchangeFolderRights.DeleteOwned)
				{
					exchangeSecurityDescriptorFolderRights |= FolderSecurity.ExchangeSecurityDescriptorFolderRights.DeleteOwnItem;
				}
				if ((folderRights & FolderSecurity.ExchangeFolderRights.ReadAny) == FolderSecurity.ExchangeFolderRights.ReadAny)
				{
					exchangeSecurityDescriptorFolderRights |= (FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadBody | FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadProperty | FolderSecurity.ExchangeSecurityDescriptorFolderRights.Execute | FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadAttributes | FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadControl | FolderSecurity.ExchangeSecurityDescriptorFolderRights.Synchronize);
				}
				if ((folderRights & FolderSecurity.ExchangeFolderRights.ReadAny) == FolderSecurity.ExchangeFolderRights.ReadAny)
				{
					exchangeSecurityDescriptorFolderRights |= FolderSecurity.ExchangeSecurityDescriptorFolderRights.ViewItem;
				}
			}
			else if (aceTarget == FolderSecurity.AceTarget.FreeBusy)
			{
				if ((folderRights & FolderSecurity.ExchangeFolderRights.FreeBusySimple) == FolderSecurity.ExchangeFolderRights.FreeBusySimple)
				{
					exchangeSecurityDescriptorFolderRights |= FolderSecurity.ExchangeSecurityDescriptorFolderRights.ReadBody;
				}
				if ((folderRights & FolderSecurity.ExchangeFolderRights.FreeBusyDetailed) == FolderSecurity.ExchangeFolderRights.FreeBusyDetailed)
				{
					exchangeSecurityDescriptorFolderRights |= FolderSecurity.ExchangeSecurityDescriptorFolderRights.WriteBody;
				}
			}
			return exchangeSecurityDescriptorFolderRights;
		}

		internal static NativeMethods.GENERIC_MAPPING GenericMapping = new NativeMethods.GENERIC_MAPPING
		{
			GenericRead = 1181833U,
			GenericWrite = 2048278U,
			GenericExecute = 1181856U,
			GenericAll = 2084863U
		};

		[Flags]
		public enum ExchangeSecurityDescriptorFolderRights
		{
			None = 0,
			ReadBody = 1,
			ListContents = 1,
			WriteBody = 2,
			CreateItem = 2,
			AppendMsg = 4,
			CreateContainer = 4,
			ReadProperty = 8,
			WriteProperty = 16,
			Execute = 32,
			Reserved1 = 64,
			ReadAttributes = 128,
			WriteAttributes = 256,
			WriteOwnProperty = 512,
			DeleteOwnItem = 1024,
			ViewItem = 2048,
			Owner = 16384,
			Contact = 32768,
			Delete = 65536,
			ReadControl = 131072,
			WriteSD = 262144,
			WriteOwner = 524288,
			Synchronize = 1048576,
			GenericRead = 1181833,
			GenericWrite = 2048278,
			GenericExecute = 1181856,
			GenericAll = 2084863,
			FolderOwner = 868624,
			AllFolder = 2083327,
			AllMessage = 2035647,
			IgnoreForCanonicalCheck = 32868,
			WriteAnyAccess = 786710,
			MessageGenericRead = 1181833,
			MessageGenericWrite = 2031894,
			MessageGenericExecute = 1181856,
			MessageGenericAll = 2035647,
			FolderGenericRead = 1181833,
			FolderGenericWrite = 2048278,
			FolderGenericExecute = 1181856,
			FolderGenericAll = 2083327,
			FreeBusySimple = 1,
			FreeBusyDetailed = 2,
			FreeBusyAll = 3
		}

		[Flags]
		public enum ExchangeFolderRights
		{
			None = 0,
			ReadAny = 1,
			Create = 2,
			EditOwned = 8,
			DeleteOwned = 16,
			EditAny = 32,
			DeleteAny = 64,
			CreateSubfolder = 128,
			Owner = 256,
			Contact = 512,
			Visible = 1024,
			FreeBusySimple = 2048,
			FreeBusyDetailed = 4096,
			FreeBusyAll = 6144,
			AllFolderRights = 7675,
			Author = 1051,
			ReadOnly = 1,
			ReadWrite = 33
		}

		public enum SecurityIdentifierType
		{
			Unknown,
			User,
			Group
		}

		public enum AceTarget
		{
			Folder,
			Message,
			FreeBusy
		}

		public class AclTableEntry
		{
			public AclTableEntry(long rowId, byte[] entryId, string name, FolderSecurity.ExchangeFolderRights rights)
			{
				this.name = name;
				this.rowId = rowId;
				this.entryId = entryId;
				this.rights = rights;
			}

			public string Name
			{
				get
				{
					return this.name;
				}
				protected set
				{
					this.name = value;
				}
			}

			public long RowId
			{
				get
				{
					return this.rowId;
				}
				protected set
				{
					this.rowId = value;
				}
			}

			public byte[] EntryId
			{
				get
				{
					return this.entryId;
				}
			}

			public FolderSecurity.ExchangeFolderRights Rights
			{
				get
				{
					return this.rights;
				}
				set
				{
					this.rights = value;
				}
			}

			public SecurityIdentifier SecurityIdentifier
			{
				get
				{
					return this.securityIdentifier;
				}
			}

			public bool IsGroup
			{
				get
				{
					return this.isGroup;
				}
			}

			public static List<FolderSecurity.AclTableEntry> ParseTableEntries(BinaryReader deserializer)
			{
				return FolderSecurity.AclTableEntry.ParseTableEntries<FolderSecurity.AclTableEntry>(deserializer, new Func<BinaryReader, FolderSecurity.AclTableEntry>(FolderSecurity.AclTableEntry.Parse));
			}

			public static List<T> ParseTableEntries<T>(BinaryReader deserializer, Func<BinaryReader, T> parser) where T : FolderSecurity.AclTableEntry
			{
				int num = deserializer.ReadInt32();
				if (num != 1)
				{
					return null;
				}
				int num2 = deserializer.ReadInt32();
				if (num2 < 0 || (long)num2 > deserializer.BaseStream.Length - deserializer.BaseStream.Position || num2 > 20000)
				{
					throw new EndOfStreamException("Invalid array length");
				}
				List<T> list = new List<T>(num2);
				for (int i = 0; i < num2; i++)
				{
					T item = parser(deserializer);
					list.Add(item);
				}
				return list;
			}

			public static void SerializeTableEntries<T>(List<T> tableEntries, BinaryWriter serializer) where T : FolderSecurity.AclTableEntry
			{
				serializer.Write(1);
				if (tableEntries == null)
				{
					serializer.Write(0);
					return;
				}
				serializer.Write(tableEntries.Count);
				foreach (T t in tableEntries)
				{
					FolderSecurity.AclTableEntry aclTableEntry = t;
					aclTableEntry.Serialize(serializer);
				}
			}

			public static FolderSecurity.AclTableEntry Parse(BinaryReader deserializer)
			{
				return new FolderSecurity.AclTableEntry(deserializer);
			}

			protected AclTableEntry(BinaryReader deserializer)
			{
				this.name = deserializer.ReadString();
				int num = deserializer.ReadInt32();
				if (num < 0)
				{
					throw new ArgumentException("EntryID length");
				}
				this.entryId = deserializer.ReadBytes(num);
				this.rights = (FolderSecurity.ExchangeFolderRights)deserializer.ReadInt32();
				int num2 = deserializer.ReadInt32();
				if (num2 < 0)
				{
					throw new ArgumentException("SID length");
				}
				this.securityIdentifier = new SecurityIdentifier(deserializer.ReadBytes(num2), 0);
				this.rowId = (long)deserializer.ReadUInt64();
				this.isGroup = (deserializer.ReadInt32() != 0);
			}

			public void Serialize(BinaryWriter serializer)
			{
				serializer.Write(this.name);
				serializer.Write(this.entryId.Length);
				serializer.Write(this.entryId);
				serializer.Write((int)this.rights);
				byte[] array = new byte[this.securityIdentifier.BinaryLength];
				this.securityIdentifier.GetBinaryForm(array, 0);
				serializer.Write(array.Length);
				serializer.Write(array);
				serializer.Write((ulong)this.rowId);
				serializer.Write(this.isGroup ? 1 : 0);
			}

			public void SetSecurityIdentifier(SecurityIdentifier securityIdentifier, bool isGroup)
			{
				this.securityIdentifier = securityIdentifier;
				this.isGroup = isGroup;
			}

			private const int AclTableCurrentVersion = 1;

			private readonly byte[] entryId;

			private FolderSecurity.ExchangeFolderRights rights;

			private SecurityIdentifier securityIdentifier;

			private string name;

			private long rowId;

			private bool isGroup;
		}

		public class AclTableAndSecurityDescriptorProperty
		{
			public AclTableAndSecurityDescriptorProperty(ArraySegment<byte> serializedAclTable, Dictionary<SecurityIdentifier, FolderSecurity.SecurityIdentifierType> securityIdentifierToTypeMap, SecurityDescriptor securityDescriptor, SecurityDescriptor freeBusySecurityDescriptor)
			{
				this.serializedAclTable = serializedAclTable;
				this.securityIdentifierToTypeMap = securityIdentifierToTypeMap;
				this.securityDescriptor = securityDescriptor;
				this.freeBusySecurityDescriptor = freeBusySecurityDescriptor;
			}

			public static byte[] GetEmpty()
			{
				return FolderSecurity.AclTableAndSecurityDescriptorProperty.emptyPropertyBuffer;
			}

			public static bool IsEmpty(byte[] blob)
			{
				return ArrayComparer<byte>.Comparer.Equals(FolderSecurity.AclTableAndSecurityDescriptorProperty.GetEmpty(), blob);
			}

			public static FolderSecurity.AclTableAndSecurityDescriptorProperty Parse(byte[] buffer)
			{
				ArraySegment<byte> arraySegment = new ArraySegment<byte>(Array<byte>.Empty);
				Dictionary<SecurityIdentifier, FolderSecurity.SecurityIdentifierType> dictionary = null;
				SecurityDescriptor securityDescriptor = null;
				SecurityDescriptor securityDescriptor2 = null;
				using (MemoryStream memoryStream = new MemoryStream(buffer, 0, buffer.Length, false, true))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						int num = binaryReader.ReadInt32();
						for (int i = 0; i < num; i++)
						{
							FolderSecurity.AclTableAndSecurityDescriptorProperty.Segment segment = FolderSecurity.AclTableAndSecurityDescriptorProperty.Segment.Parse(binaryReader);
							switch (segment.SegmentType)
							{
							case FolderSecurity.AclTableAndSecurityDescriptorProperty.SegmentType.Table:
								arraySegment = segment.Parse<ArraySegment<byte>>(binaryReader, new Func<BinaryReader, ArraySegment<byte>>(FolderSecurity.AclTableAndSecurityDescriptorProperty.ReadSegment));
								break;
							case FolderSecurity.AclTableAndSecurityDescriptorProperty.SegmentType.SecurityIdentifierMap:
								dictionary = segment.Parse<Dictionary<SecurityIdentifier, FolderSecurity.SecurityIdentifierType>>(binaryReader, new Func<BinaryReader, Dictionary<SecurityIdentifier, FolderSecurity.SecurityIdentifierType>>(FolderSecurity.AclTableAndSecurityDescriptorProperty.ParseSecurityIdentifierToTypeMap));
								break;
							case FolderSecurity.AclTableAndSecurityDescriptorProperty.SegmentType.SecurityDescriptor:
								securityDescriptor = segment.Parse<SecurityDescriptor>(binaryReader, new Func<BinaryReader, SecurityDescriptor>(FolderSecurity.AclTableAndSecurityDescriptorProperty.ParseSecurityDescriptor));
								break;
							case FolderSecurity.AclTableAndSecurityDescriptorProperty.SegmentType.FreeBusySecurityDescriptor:
								securityDescriptor2 = segment.Parse<SecurityDescriptor>(binaryReader, new Func<BinaryReader, SecurityDescriptor>(FolderSecurity.AclTableAndSecurityDescriptorProperty.ParseSecurityDescriptor));
								break;
							default:
								segment.SkipData(binaryReader);
								break;
							}
						}
					}
				}
				return new FolderSecurity.AclTableAndSecurityDescriptorProperty(arraySegment, dictionary, securityDescriptor, securityDescriptor2);
			}

			internal static byte[] GetDefaultBlobForRootFolder()
			{
				return FolderSecurity.AclTableAndSecurityDescriptorProperty.defaultForRootFolder;
			}

			internal static byte[] GetDefaultBlobForGroupMailboxRootFolder(Guid mailboxGuid)
			{
				FolderSecurity.SecurityIdentifierAndFolderRights securityIdentifierAndFolderRights = new FolderSecurity.SecurityIdentifierAndFolderRights(SecurityIdentity.GetGroupSecurityIdentifier(mailboxGuid, SecurityIdentity.GroupMailboxMemberType.Owner), FolderSecurity.ExchangeFolderRights.ReadAny | FolderSecurity.ExchangeFolderRights.Create | FolderSecurity.ExchangeFolderRights.EditOwned | FolderSecurity.ExchangeFolderRights.DeleteOwned | FolderSecurity.ExchangeFolderRights.DeleteAny | FolderSecurity.ExchangeFolderRights.Visible, FolderSecurity.SecurityIdentifierType.User);
				FolderSecurity.SecurityIdentifierAndFolderRights securityIdentifierAndFolderRights2 = new FolderSecurity.SecurityIdentifierAndFolderRights(SecurityIdentity.GetGroupSecurityIdentifier(mailboxGuid, SecurityIdentity.GroupMailboxMemberType.Member), FolderSecurity.ExchangeFolderRights.Author, FolderSecurity.SecurityIdentifierType.User);
				return FolderSecurity.AclTableAndSecurityDescriptorProperty.GetBlobForSecurityDescriptor(new FolderSecurity.SecurityIdentifierAndFolderRights[]
				{
					securityIdentifierAndFolderRights,
					securityIdentifierAndFolderRights2,
					FolderSecurity.AclTableAndSecurityDescriptorProperty.anonymousNoRights,
					FolderSecurity.AclTableAndSecurityDescriptorProperty.everyoneNoRights
				});
			}

			internal static byte[] GetDefaultBlobForPublicFolders()
			{
				return FolderSecurity.AclTableAndSecurityDescriptorProperty.defaultForPublicFolders;
			}

			internal static byte[] GetDefaultBlobForInternalSubmissionPublicFolder()
			{
				return FolderSecurity.AclTableAndSecurityDescriptorProperty.defaultForInternalSubmissionPublicFolder;
			}

			internal static byte[] CreateForChildFolder(byte[] parentdAclTableAndSD)
			{
				return FolderSecurity.AclTableAndSecurityDescriptorProperty.CreateForChildFolder(parentdAclTableAndSD, null, null, null, FolderSecurity.ExchangeFolderRights.None);
			}

			internal static byte[] CreateForChildFolder(byte[] parentdAclTableAndSD, SecurityIdentifier delegateUser, byte[] delegateEntryId, string delegateDisplayName, FolderSecurity.ExchangeFolderRights delegateRights)
			{
				if (delegateUser == null)
				{
					return parentdAclTableAndSD;
				}
				FolderSecurity.AclTableAndSecurityDescriptorProperty aclTableAndSecurityDescriptorProperty = FolderSecurity.AclTableAndSecurityDescriptorProperty.Parse(parentdAclTableAndSD);
				RawSecurityDescriptor rawSecurityDescriptor = aclTableAndSecurityDescriptorProperty.SecurityDescriptor.ToRawSecurityDescriptorThrow();
				RawSecurityDescriptor rawSecurityDescriptor2 = (aclTableAndSecurityDescriptorProperty.FreeBusySecurityDescriptor != null) ? aclTableAndSecurityDescriptorProperty.FreeBusySecurityDescriptor.ToRawSecurityDescriptorThrow() : null;
				Dictionary<SecurityIdentifier, FolderSecurity.SecurityIdentifierType> dictionary = aclTableAndSecurityDescriptorProperty.SecurityIdentifierToTypeMap;
				if (rawSecurityDescriptor.DiscretionaryAcl != null)
				{
					FolderSecurity.AnnotatedAceList.RemoveAcesFromAcl(rawSecurityDescriptor.DiscretionaryAcl, delegateUser);
				}
				else
				{
					rawSecurityDescriptor.DiscretionaryAcl = new RawAcl(2, 4);
				}
				FolderSecurity.AnnotatedAceList.InsertUserAceToFolderAcl(rawSecurityDescriptor.DiscretionaryAcl, 0, new FolderSecurity.SecurityIdentifierAndFolderRights(delegateUser, delegateRights, FolderSecurity.SecurityIdentifierType.User), true);
				byte[] array = Array<byte>.Empty;
				if (aclTableAndSecurityDescriptorProperty.SerializedAclTable.Count != 0)
				{
					ArraySegment<byte> arraySegment = aclTableAndSecurityDescriptorProperty.SerializedAclTable;
					List<FolderSecurity.AclTableEntry> list;
					using (MemoryStream memoryStream = new MemoryStream(arraySegment.Array, arraySegment.Offset, arraySegment.Count))
					{
						using (BinaryReader binaryReader = new BinaryReader(memoryStream))
						{
							list = FolderSecurity.AclTableEntry.ParseTableEntries(binaryReader);
						}
					}
					if (list != null && list.Count != 0)
					{
						list.RemoveAll((FolderSecurity.AclTableEntry tableEntry) => tableEntry.SecurityIdentifier == delegateUser);
						long rowId;
						if (list.Count != 0)
						{
							rowId = (from entry in list
							select entry.RowId).Max() + 1L;
						}
						else
						{
							rowId = 1L;
						}
						FolderSecurity.AclTableEntry aclTableEntry = new FolderSecurity.AclTableEntry(rowId, delegateEntryId, delegateDisplayName, delegateRights);
						aclTableEntry.SetSecurityIdentifier(delegateUser, false);
						list.Add(aclTableEntry);
						using (MemoryStream memoryStream2 = new MemoryStream(200))
						{
							using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream2))
							{
								FolderSecurity.AclTableEntry.SerializeTableEntries<FolderSecurity.AclTableEntry>(list, binaryWriter);
								array = memoryStream2.ToArray();
							}
						}
					}
				}
				if (dictionary == null)
				{
					dictionary = new Dictionary<SecurityIdentifier, FolderSecurity.SecurityIdentifierType>(1);
				}
				dictionary[delegateUser] = FolderSecurity.SecurityIdentifierType.User;
				FolderSecurity.AclTableAndSecurityDescriptorProperty aclTableAndSecurityDescriptorProperty2 = new FolderSecurity.AclTableAndSecurityDescriptorProperty(new ArraySegment<byte>(array), dictionary, SecurityDescriptor.FromRawSecurityDescriptor(rawSecurityDescriptor), SecurityDescriptor.FromRawSecurityDescriptor(rawSecurityDescriptor2));
				return aclTableAndSecurityDescriptorProperty2.Serialize();
			}

			internal static SecurityDescriptor CreateFolderSecurityDescriptor(RawAcl dacl)
			{
				RawSecurityDescriptor rawSecurityDescriptor = new RawSecurityDescriptor(ControlFlags.DiscretionaryAclPresent, FolderSecurity.AclTableAndSecurityDescriptorProperty.localSystemSID, FolderSecurity.AclTableAndSecurityDescriptorProperty.localSystemSID, null, dacl);
				return SecurityDescriptor.FromRawSecurityDescriptor(rawSecurityDescriptor);
			}

			public byte[] Serialize()
			{
				byte[] result;
				using (MemoryStream memoryStream = new MemoryStream(512))
				{
					int num = 0;
					if (this.serializedAclTable.Count != 0)
					{
						num++;
					}
					if (this.securityIdentifierToTypeMap != null)
					{
						num++;
					}
					if (this.securityDescriptor != null)
					{
						num++;
					}
					if (this.freeBusySecurityDescriptor != null)
					{
						num++;
					}
					using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
					{
						binaryWriter.Write(num);
						if (this.serializedAclTable.Count != 0)
						{
							FolderSecurity.AclTableAndSecurityDescriptorProperty.Segment.Serialize<ArraySegment<byte>>(binaryWriter, FolderSecurity.AclTableAndSecurityDescriptorProperty.SegmentType.Table, this.serializedAclTable, new FolderSecurity.AclTableAndSecurityDescriptorProperty.Segment.SerializerDelegate<ArraySegment<byte>>(FolderSecurity.AclTableAndSecurityDescriptorProperty.WriteSegment));
						}
						if (this.securityIdentifierToTypeMap != null)
						{
							FolderSecurity.AclTableAndSecurityDescriptorProperty.Segment.Serialize<Dictionary<SecurityIdentifier, FolderSecurity.SecurityIdentifierType>>(binaryWriter, FolderSecurity.AclTableAndSecurityDescriptorProperty.SegmentType.SecurityIdentifierMap, this.securityIdentifierToTypeMap, new FolderSecurity.AclTableAndSecurityDescriptorProperty.Segment.SerializerDelegate<Dictionary<SecurityIdentifier, FolderSecurity.SecurityIdentifierType>>(FolderSecurity.AclTableAndSecurityDescriptorProperty.SerializeSecurityIdentifierToTypeMap));
						}
						if (this.securityDescriptor != null)
						{
							FolderSecurity.AclTableAndSecurityDescriptorProperty.Segment.Serialize<SecurityDescriptor>(binaryWriter, FolderSecurity.AclTableAndSecurityDescriptorProperty.SegmentType.SecurityDescriptor, this.securityDescriptor, new FolderSecurity.AclTableAndSecurityDescriptorProperty.Segment.SerializerDelegate<SecurityDescriptor>(FolderSecurity.AclTableAndSecurityDescriptorProperty.SerializeSecurityDescriptor));
						}
						if (this.freeBusySecurityDescriptor != null)
						{
							FolderSecurity.AclTableAndSecurityDescriptorProperty.Segment.Serialize<SecurityDescriptor>(binaryWriter, FolderSecurity.AclTableAndSecurityDescriptorProperty.SegmentType.FreeBusySecurityDescriptor, this.freeBusySecurityDescriptor, new FolderSecurity.AclTableAndSecurityDescriptorProperty.Segment.SerializerDelegate<SecurityDescriptor>(FolderSecurity.AclTableAndSecurityDescriptorProperty.SerializeSecurityDescriptor));
						}
					}
					result = memoryStream.ToArray();
				}
				return result;
			}

			public SecurityDescriptor ComputeFreeBusySdFromFolderSd()
			{
				if (this.SecurityDescriptor == null)
				{
					return null;
				}
				FolderSecurity.AnnotatedAceList annotatedAceList = new FolderSecurity.AnnotatedAceList(this.SecurityDescriptor.ToRawSecurityDescriptorThrow(), null, (SecurityIdentifier sid) => FolderSecurity.SecurityIdentifierType.Unknown);
				IList<FolderSecurity.SecurityIdentifierAndFolderRights> securityIdentifierAndRightsList = annotatedAceList.GetSecurityIdentifierAndRightsList();
				bool flag = false;
				foreach (FolderSecurity.SecurityIdentifierAndFolderRights securityIdentifierAndFolderRights in securityIdentifierAndRightsList)
				{
					if (securityIdentifierAndFolderRights.SecurityIdentifier.IsWellKnown(WellKnownSidType.AnonymousSid))
					{
						securityIdentifierAndFolderRights.AllowRights = FolderSecurity.ExchangeFolderRights.None;
						securityIdentifierAndFolderRights.DenyRights = FolderSecurity.ExchangeFolderRights.FreeBusyAll;
					}
					else
					{
						if (securityIdentifierAndFolderRights.SecurityIdentifier.IsWellKnown(WellKnownSidType.WorldSid))
						{
							flag = true;
						}
						if ((securityIdentifierAndFolderRights.AllowRights & FolderSecurity.ExchangeFolderRights.ReadAny) == FolderSecurity.ExchangeFolderRights.ReadAny)
						{
							securityIdentifierAndFolderRights.AllowRights = FolderSecurity.ExchangeFolderRights.FreeBusyAll;
							securityIdentifierAndFolderRights.DenyRights = FolderSecurity.ExchangeFolderRights.None;
						}
						else
						{
							securityIdentifierAndFolderRights.AllowRights = FolderSecurity.ExchangeFolderRights.FreeBusySimple;
							securityIdentifierAndFolderRights.DenyRights = FolderSecurity.ExchangeFolderRights.FreeBusyDetailed;
						}
					}
				}
				if (!flag)
				{
					securityIdentifierAndRightsList.Add(new FolderSecurity.SecurityIdentifierAndFolderRights(FolderSecurity.AclTableAndSecurityDescriptorProperty.everyoneSID, FolderSecurity.ExchangeFolderRights.FreeBusySimple, FolderSecurity.ExchangeFolderRights.FreeBusyDetailed, FolderSecurity.SecurityIdentifierType.User));
				}
				return FolderSecurity.AclTableAndSecurityDescriptorProperty.CreateFolderSecurityDescriptor(FolderSecurity.AnnotatedAceList.BuildFreeBusyCanonicalAceList(securityIdentifierAndRightsList));
			}

			public ArraySegment<byte> SerializedAclTable
			{
				get
				{
					return this.serializedAclTable;
				}
			}

			public Dictionary<SecurityIdentifier, FolderSecurity.SecurityIdentifierType> SecurityIdentifierToTypeMap
			{
				get
				{
					return this.securityIdentifierToTypeMap;
				}
			}

			public SecurityDescriptor SecurityDescriptor
			{
				get
				{
					return this.securityDescriptor;
				}
			}

			public SecurityDescriptor FreeBusySecurityDescriptor
			{
				get
				{
					return this.freeBusySecurityDescriptor;
				}
			}

			private static byte[] GetBlobForSecurityDescriptor(params FolderSecurity.SecurityIdentifierAndFolderRights[] rights)
			{
				RawAcl dacl = FolderSecurity.AnnotatedAceList.BuildFolderCanonicalAceList(rights);
				SecurityDescriptor securityDescriptor = FolderSecurity.AclTableAndSecurityDescriptorProperty.CreateFolderSecurityDescriptor(dacl);
				FolderSecurity.AclTableAndSecurityDescriptorProperty aclTableAndSecurityDescriptorProperty = new FolderSecurity.AclTableAndSecurityDescriptorProperty(new ArraySegment<byte>(Array<byte>.Empty), null, securityDescriptor, null);
				return aclTableAndSecurityDescriptorProperty.Serialize();
			}

			private static ArraySegment<byte> AllocateSegment(BinaryWriter writer, int segmentSize)
			{
				MemoryStream memoryStream = writer.BaseStream as MemoryStream;
				writer.Write(segmentSize);
				memoryStream.SetLength(memoryStream.Position + (long)segmentSize);
				ArraySegment<byte> result = new ArraySegment<byte>(memoryStream.GetBuffer(), (int)memoryStream.Position, segmentSize);
				memoryStream.Seek((long)segmentSize, SeekOrigin.Current);
				return result;
			}

			private static void WriteSegment(BinaryWriter writer, ArraySegment<byte> segment)
			{
				writer.Write(segment.Count);
				writer.Write(segment.Array, segment.Offset, segment.Count);
			}

			private static ArraySegment<byte> ReadSegment(BinaryReader reader)
			{
				MemoryStream memoryStream = reader.BaseStream as MemoryStream;
				int num = reader.ReadInt32();
				if (num < 0)
				{
					throw new ArgumentException("Segment length cannot be negative");
				}
				ArraySegment<byte> result = new ArraySegment<byte>(memoryStream.GetBuffer(), (int)memoryStream.Position, num);
				memoryStream.Seek((long)num, SeekOrigin.Current);
				return result;
			}

			private static void SerializeSecurityDescriptor(BinaryWriter writer, SecurityDescriptor securityDescriptor)
			{
				ArraySegment<byte> arraySegment = FolderSecurity.AclTableAndSecurityDescriptorProperty.AllocateSegment(writer, securityDescriptor.BinaryForm.Length);
				Array.Copy(securityDescriptor.BinaryForm, 0, arraySegment.Array, arraySegment.Offset, securityDescriptor.BinaryForm.Length);
			}

			private static SecurityDescriptor ParseSecurityDescriptor(BinaryReader reader)
			{
				ArraySegment<byte> arraySegment = FolderSecurity.AclTableAndSecurityDescriptorProperty.ReadSegment(reader);
				byte[] array = new byte[arraySegment.Count];
				Array.Copy(arraySegment.Array, arraySegment.Offset, array, 0, arraySegment.Count);
				return new SecurityDescriptor(array);
			}

			private static void SerializeSecurityIdentifier(BinaryWriter writer, SecurityIdentifier securityIdentifier)
			{
				ArraySegment<byte> arraySegment = FolderSecurity.AclTableAndSecurityDescriptorProperty.AllocateSegment(writer, securityIdentifier.BinaryLength);
				securityIdentifier.GetBinaryForm(arraySegment.Array, arraySegment.Offset);
			}

			private static SecurityIdentifier ParseSecurityIdentifier(BinaryReader reader)
			{
				ArraySegment<byte> arraySegment = FolderSecurity.AclTableAndSecurityDescriptorProperty.ReadSegment(reader);
				return new SecurityIdentifier(arraySegment.Array, arraySegment.Offset);
			}

			private static void SerializeSecurityIdentifierToTypeMap(BinaryWriter writer, Dictionary<SecurityIdentifier, FolderSecurity.SecurityIdentifierType> securityIdentifierToTypeMap)
			{
				writer.Write(securityIdentifierToTypeMap.Count);
				foreach (SecurityIdentifier securityIdentifier in securityIdentifierToTypeMap.Keys)
				{
					FolderSecurity.AclTableAndSecurityDescriptorProperty.SerializeSecurityIdentifier(writer, securityIdentifier);
					writer.Write((int)securityIdentifierToTypeMap[securityIdentifier]);
				}
			}

			private static Dictionary<SecurityIdentifier, FolderSecurity.SecurityIdentifierType> ParseSecurityIdentifierToTypeMap(BinaryReader reader)
			{
				int num = reader.ReadInt32();
				if (num < 0)
				{
					throw new ArgumentException("Map size cannot be negative");
				}
				Dictionary<SecurityIdentifier, FolderSecurity.SecurityIdentifierType> dictionary = new Dictionary<SecurityIdentifier, FolderSecurity.SecurityIdentifierType>(Math.Min(num, 50));
				for (int i = 0; i < num; i++)
				{
					SecurityIdentifier key = FolderSecurity.AclTableAndSecurityDescriptorProperty.ParseSecurityIdentifier(reader);
					FolderSecurity.SecurityIdentifierType value = (FolderSecurity.SecurityIdentifierType)reader.ReadInt32();
					dictionary[key] = value;
				}
				return dictionary;
			}

			private static readonly byte[] emptyPropertyBuffer = new FolderSecurity.AclTableAndSecurityDescriptorProperty(new ArraySegment<byte>(Array<byte>.Empty), null, null, null).Serialize();

			private static SecurityIdentifier localSystemSID = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);

			private static SecurityIdentifier everyoneSID = new SecurityIdentifier(WellKnownSidType.WorldSid, null);

			private static SecurityIdentifier anonymousSID = new SecurityIdentifier(WellKnownSidType.AnonymousSid, null);

			private static readonly FolderSecurity.SecurityIdentifierAndFolderRights anonymousNoRights = new FolderSecurity.SecurityIdentifierAndFolderRights(FolderSecurity.AclTableAndSecurityDescriptorProperty.anonymousSID, FolderSecurity.ExchangeFolderRights.None, FolderSecurity.SecurityIdentifierType.User);

			private static readonly FolderSecurity.SecurityIdentifierAndFolderRights everyoneNoRights = new FolderSecurity.SecurityIdentifierAndFolderRights(FolderSecurity.AclTableAndSecurityDescriptorProperty.everyoneSID, FolderSecurity.ExchangeFolderRights.None, FolderSecurity.SecurityIdentifierType.User);

			private static byte[] defaultForRootFolder = FolderSecurity.AclTableAndSecurityDescriptorProperty.GetBlobForSecurityDescriptor(new FolderSecurity.SecurityIdentifierAndFolderRights[]
			{
				FolderSecurity.AclTableAndSecurityDescriptorProperty.anonymousNoRights,
				FolderSecurity.AclTableAndSecurityDescriptorProperty.everyoneNoRights
			});

			private static byte[] defaultForPublicFolders = FolderSecurity.AclTableAndSecurityDescriptorProperty.GetBlobForSecurityDescriptor(new FolderSecurity.SecurityIdentifierAndFolderRights[]
			{
				FolderSecurity.AclTableAndSecurityDescriptorProperty.anonymousNoRights,
				new FolderSecurity.SecurityIdentifierAndFolderRights(FolderSecurity.AclTableAndSecurityDescriptorProperty.everyoneSID, FolderSecurity.ExchangeFolderRights.Author, FolderSecurity.SecurityIdentifierType.User)
			});

			private static byte[] defaultForInternalSubmissionPublicFolder = FolderSecurity.AclTableAndSecurityDescriptorProperty.GetBlobForSecurityDescriptor(new FolderSecurity.SecurityIdentifierAndFolderRights[]
			{
				FolderSecurity.AclTableAndSecurityDescriptorProperty.anonymousNoRights,
				new FolderSecurity.SecurityIdentifierAndFolderRights(FolderSecurity.AclTableAndSecurityDescriptorProperty.everyoneSID, FolderSecurity.ExchangeFolderRights.Create | FolderSecurity.ExchangeFolderRights.EditOwned | FolderSecurity.ExchangeFolderRights.DeleteOwned | FolderSecurity.ExchangeFolderRights.Visible, FolderSecurity.SecurityIdentifierType.User)
			});

			private SecurityDescriptor securityDescriptor;

			private SecurityDescriptor freeBusySecurityDescriptor;

			private ArraySegment<byte> serializedAclTable;

			private Dictionary<SecurityIdentifier, FolderSecurity.SecurityIdentifierType> securityIdentifierToTypeMap;

			private struct Segment
			{
				private Segment(FolderSecurity.AclTableAndSecurityDescriptorProperty.SegmentType type, int length)
				{
					this.type = type;
					this.length = length;
				}

				public static FolderSecurity.AclTableAndSecurityDescriptorProperty.Segment Parse(BinaryReader reader)
				{
					FolderSecurity.AclTableAndSecurityDescriptorProperty.SegmentType segmentType = (FolderSecurity.AclTableAndSecurityDescriptorProperty.SegmentType)reader.ReadInt32();
					int num = reader.ReadInt32();
					if (num < 0)
					{
						throw new ArgumentException("Segment length cannot be negative");
					}
					return new FolderSecurity.AclTableAndSecurityDescriptorProperty.Segment(segmentType, num);
				}

				public T Parse<T>(BinaryReader reader, Func<BinaryReader, T> parser)
				{
					return parser(reader);
				}

				public static void Serialize<T>(BinaryWriter writer, FolderSecurity.AclTableAndSecurityDescriptorProperty.SegmentType segmentType, T contentObject, FolderSecurity.AclTableAndSecurityDescriptorProperty.Segment.SerializerDelegate<T> serializer)
				{
					writer.Write((int)segmentType);
					long position = writer.BaseStream.Position;
					writer.Write(0);
					long position2 = writer.BaseStream.Position;
					serializer(writer, contentObject);
					long position3 = writer.BaseStream.Position;
					writer.BaseStream.Position = position;
					writer.Write((int)(position3 - position2));
					writer.BaseStream.Position = position3;
				}

				public void SkipData(BinaryReader reader)
				{
					reader.BaseStream.Position += (long)this.length;
				}

				public FolderSecurity.AclTableAndSecurityDescriptorProperty.SegmentType SegmentType
				{
					get
					{
						return this.type;
					}
				}

				private readonly FolderSecurity.AclTableAndSecurityDescriptorProperty.SegmentType type;

				private readonly int length;

				public delegate void SerializerDelegate<T>(BinaryWriter writer, T contentObject);
			}

			private enum SegmentType
			{
				Table,
				SecurityIdentifierMap,
				SecurityDescriptor,
				FreeBusySecurityDescriptor
			}
		}

		public class SecurityIdentifierAndFolderRights
		{
			public SecurityIdentifierAndFolderRights(SecurityIdentifier securityIdentifier, FolderSecurity.ExchangeFolderRights allowRights, FolderSecurity.ExchangeFolderRights denyRights, FolderSecurity.SecurityIdentifierType securityIdentifierType)
			{
				this.securityIdentifier = securityIdentifier;
				this.allowRights = allowRights;
				this.denyRights = denyRights;
				this.securityIdentifierType = securityIdentifierType;
			}

			public SecurityIdentifierAndFolderRights(SecurityIdentifier securityIdentifier, FolderSecurity.ExchangeFolderRights allowRights, FolderSecurity.SecurityIdentifierType securityIdentifierType) : this(securityIdentifier, allowRights, FolderSecurity.ExchangeFolderRights.AllFolderRights & ~allowRights, securityIdentifierType)
			{
			}

			public SecurityIdentifier SecurityIdentifier
			{
				get
				{
					return this.securityIdentifier;
				}
			}

			public FolderSecurity.ExchangeFolderRights AllowRights
			{
				get
				{
					return this.allowRights;
				}
				set
				{
					this.allowRights = value;
				}
			}

			public FolderSecurity.ExchangeFolderRights DenyRights
			{
				get
				{
					return this.denyRights;
				}
				set
				{
					this.denyRights = value;
				}
			}

			public FolderSecurity.SecurityIdentifierType SecurityIdentifierType
			{
				get
				{
					return this.securityIdentifierType;
				}
			}

			private readonly SecurityIdentifier securityIdentifier;

			private FolderSecurity.ExchangeFolderRights allowRights;

			private FolderSecurity.ExchangeFolderRights denyRights;

			private FolderSecurity.SecurityIdentifierType securityIdentifierType;
		}

		public class AnnotatedAceList
		{
			public AnnotatedAceList(RawSecurityDescriptor securityDescriptor, RawSecurityDescriptor freeBusySecurityDescriptor, Func<SecurityIdentifier, FolderSecurity.SecurityIdentifierType> securityIdentifierTypeResolver)
			{
				this.securityIdentifierTypeResolver = securityIdentifierTypeResolver;
				if (securityDescriptor.DiscretionaryAcl != null)
				{
					this.aceEntries = new List<FolderSecurity.AnnotatedAceList.AnnotatedAce>(securityDescriptor.DiscretionaryAcl.Count);
					foreach (GenericAce genericAce in securityDescriptor.DiscretionaryAcl)
					{
						if (genericAce.AceType == AceType.AccessAllowedObject || genericAce.AceType == AceType.AccessDeniedObject)
						{
							this.objectAceFound = true;
						}
						else
						{
							KnownAce knownAce = genericAce as KnownAce;
							if (knownAce == null)
							{
								this.nonCanonicalAceFound = true;
							}
							else if (!knownAce.SecurityIdentifier.IsWellKnown(WellKnownSidType.SelfSid))
							{
								FolderSecurity.AceTarget aceTarget;
								FolderSecurity.SecurityIdentifierType securityIdentifierType;
								FolderSecurity.AnnotatedAceList.AceCanonicalType aceCanonicalType = this.GetAceCanonicalType(knownAce, out aceTarget, out securityIdentifierType);
								if (aceCanonicalType == FolderSecurity.AnnotatedAceList.AceCanonicalType.Invalid)
								{
									this.nonCanonicalAceFound = true;
								}
								else
								{
									this.aceEntries.Add(new FolderSecurity.AnnotatedAceList.AnnotatedAce(knownAce, aceCanonicalType, aceTarget, securityIdentifierType));
								}
							}
						}
					}
				}
				else
				{
					this.aceEntries = new List<FolderSecurity.AnnotatedAceList.AnnotatedAce>();
				}
				if (freeBusySecurityDescriptor != null && freeBusySecurityDescriptor.DiscretionaryAcl != null)
				{
					foreach (GenericAce genericAce2 in freeBusySecurityDescriptor.DiscretionaryAcl)
					{
						KnownAce knownAce2 = genericAce2 as KnownAce;
						if (!(knownAce2 == null) && !knownAce2.SecurityIdentifier.IsWellKnown(WellKnownSidType.SelfSid))
						{
							FolderSecurity.SecurityIdentifierType securityIdentifierType2;
							FolderSecurity.AnnotatedAceList.AceCanonicalType aceCanonicalTypeForTarget = this.GetAceCanonicalTypeForTarget(knownAce2, FolderSecurity.AceTarget.FreeBusy, out securityIdentifierType2);
							if (aceCanonicalTypeForTarget != FolderSecurity.AnnotatedAceList.AceCanonicalType.Invalid)
							{
								this.aceEntries.Add(new FolderSecurity.AnnotatedAceList.AnnotatedAce(knownAce2, aceCanonicalTypeForTarget, FolderSecurity.AceTarget.FreeBusy, securityIdentifierType2));
							}
						}
					}
				}
			}

			public IList<FolderSecurity.SecurityIdentifierAndFolderRights> GetSecurityIdentifierAndRightsList()
			{
				List<FolderSecurity.SecurityIdentifierAndFolderRights> list = new List<FolderSecurity.SecurityIdentifierAndFolderRights>(this.aceEntries.Count);
				using (IEnumerator<FolderSecurity.AnnotatedAceList.AnnotatedAce> enumerator = this.aceEntries.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						FolderSecurity.AnnotatedAceList.AnnotatedAce ace = enumerator.Current;
						FolderSecurity.AnnotatedAceList.AnnotatedAce ace9 = ace;
						FolderSecurity.ExchangeSecurityDescriptorFolderRights accessMask = (FolderSecurity.ExchangeSecurityDescriptorFolderRights)ace9.Ace.AccessMask;
						FolderSecurity.ExchangeSecurityDescriptorFolderRights sdRights = accessMask;
						FolderSecurity.AnnotatedAceList.AnnotatedAce ace2 = ace;
						FolderSecurity.ExchangeFolderRights exchangeFolderRights = FolderSecurity.FolderRightsFromSecurityDescriptorRights(sdRights, ace2.Target);
						int num = list.FindIndex(delegate(FolderSecurity.SecurityIdentifierAndFolderRights entry)
						{
							SecurityIdentifier securityIdentifier2 = entry.SecurityIdentifier;
							FolderSecurity.AnnotatedAceList.AnnotatedAce ace8 = ace;
							return securityIdentifier2 == ace8.Ace.SecurityIdentifier;
						});
						if (num != -1)
						{
							FolderSecurity.AnnotatedAceList.AnnotatedAce ace3 = ace;
							if (ace3.IsAllowAce)
							{
								list[num].AllowRights |= (exchangeFolderRights & ~list[num].DenyRights);
							}
							else
							{
								list[num].DenyRights |= (exchangeFolderRights & ~list[num].AllowRights);
							}
						}
						else
						{
							List<FolderSecurity.SecurityIdentifierAndFolderRights> list2 = list;
							FolderSecurity.AnnotatedAceList.AnnotatedAce ace4 = ace;
							SecurityIdentifier securityIdentifier = ace4.Ace.SecurityIdentifier;
							FolderSecurity.AnnotatedAceList.AnnotatedAce ace5 = ace;
							FolderSecurity.ExchangeFolderRights allowRights = ace5.IsAllowAce ? exchangeFolderRights : FolderSecurity.ExchangeFolderRights.None;
							FolderSecurity.AnnotatedAceList.AnnotatedAce ace6 = ace;
							FolderSecurity.ExchangeFolderRights denyRights = ace6.IsAllowAce ? FolderSecurity.ExchangeFolderRights.None : exchangeFolderRights;
							FolderSecurity.AnnotatedAceList.AnnotatedAce ace7 = ace;
							list2.Add(new FolderSecurity.SecurityIdentifierAndFolderRights(securityIdentifier, allowRights, denyRights, ace7.SecurityIdentifierType));
						}
					}
				}
				return list;
			}

			public bool IsCanonical(out string errorInformation)
			{
				if (this.objectAceFound || this.nonCanonicalAceFound)
				{
					errorInformation = this.CreateErrorInformation((LID)52352U, new int[0]);
					return false;
				}
				Dictionary<SecurityIdentifier, int> dictionary = new Dictionary<SecurityIdentifier, int>(this.aceEntries.Count);
				Dictionary<SecurityIdentifier, int> dictionary2 = new Dictionary<SecurityIdentifier, int>(this.aceEntries.Count);
				if (!this.IsCanonicalForTarget(FolderSecurity.AceTarget.Folder, dictionary, dictionary2, out errorInformation))
				{
					return false;
				}
				if (!this.IsCanonicalForTarget(FolderSecurity.AceTarget.Message, dictionary, dictionary2, out errorInformation))
				{
					return false;
				}
				if (!dictionary2.Values.Any((int rights) => !FolderSecurity.AnnotatedAceList.HasFullRights(rights, FolderSecurity.AceTarget.Folder)))
				{
					if (!dictionary.Values.Any((int rights) => !FolderSecurity.AnnotatedAceList.HasFullRights(rights, FolderSecurity.AceTarget.Message)))
					{
						return true;
					}
				}
				errorInformation = this.CreateErrorInformation((LID)42112U, new int[0]);
				return false;
			}

			private bool IsCanonicalForTarget(FolderSecurity.AceTarget target, Dictionary<SecurityIdentifier, int> accumulativeMessageSidRights, Dictionary<SecurityIdentifier, int> accumulativeFolderSidRights, out string errorInformation)
			{
				FolderSecurity.AnnotatedAceList.AceCanonicalType aceCanonicalType = FolderSecurity.AnnotatedAceList.AceCanonicalType.Invalid;
				bool flag = false;
				FolderSecurity.AnnotatedAceList.AnnotatedAce? annotatedAce = null;
				for (int i = 0; i < this.aceEntries.Count; i++)
				{
					FolderSecurity.AnnotatedAceList.AnnotatedAce value = this.aceEntries[i];
					if (value.Target == target && !value.Ace.SecurityIdentifier.IsWellKnown(WellKnownSidType.AnonymousSid))
					{
						FolderSecurity.AnnotatedAceList.AceCanonicalType canonicalType = value.CanonicalType;
						bool[][] array;
						int num;
						if (!flag)
						{
							if (canonicalType == FolderSecurity.AnnotatedAceList.AceCanonicalType.GroupAllow || canonicalType == FolderSecurity.AnnotatedAceList.AceCanonicalType.GroupDeny || (i < this.aceEntries.Count - 1 && canonicalType == FolderSecurity.AnnotatedAceList.AceCanonicalType.UnknownAllowPartial && (this.aceEntries[i + 1].CanonicalType == FolderSecurity.AnnotatedAceList.AceCanonicalType.UnknownAllowFull || this.aceEntries[i + 1].CanonicalType == FolderSecurity.AnnotatedAceList.AceCanonicalType.UnknownAllowPartial)))
							{
								flag = true;
								array = FolderSecurity.AnnotatedAceList.userToGroupTransition;
								num = 0;
							}
							else
							{
								array = FolderSecurity.AnnotatedAceList.userSectionTransitions;
								num = 1;
							}
						}
						else
						{
							array = FolderSecurity.AnnotatedAceList.groupSectionTransitions;
							num = 2;
						}
						if (!array[(int)aceCanonicalType][(int)canonicalType])
						{
							errorInformation = this.CreateErrorInformation((LID)46208U, new int[]
							{
								i,
								num,
								(int)aceCanonicalType,
								(int)canonicalType
							});
							return false;
						}
						aceCanonicalType = canonicalType;
						switch (canonicalType)
						{
						case FolderSecurity.AnnotatedAceList.AceCanonicalType.UserAllowFull:
						case FolderSecurity.AnnotatedAceList.AceCanonicalType.UserAllowPartial:
							annotatedAce = new FolderSecurity.AnnotatedAceList.AnnotatedAce?(value);
							break;
						case FolderSecurity.AnnotatedAceList.AceCanonicalType.UserDenyFull:
							goto IL_1C9;
						case FolderSecurity.AnnotatedAceList.AceCanonicalType.UserDenyPartial:
							if (annotatedAce == null || annotatedAce.Value.Ace.SecurityIdentifier != value.Ace.SecurityIdentifier || !FolderSecurity.AnnotatedAceList.HasFullRights(annotatedAce.Value.Ace.AccessMask | value.Ace.AccessMask, value.Target))
							{
								errorInformation = this.CreateErrorInformation((LID)54400U, new int[]
								{
									i
								});
								return false;
							}
							annotatedAce = null;
							break;
						default:
							goto IL_1C9;
						}
						IL_1D1:
						if (!value.Ace.SecurityIdentifier.IsWellKnown(WellKnownSidType.WorldSid))
						{
							if (!accumulativeFolderSidRights.ContainsKey(value.Ace.SecurityIdentifier))
							{
								accumulativeFolderSidRights[value.Ace.SecurityIdentifier] = 0;
							}
							if (!accumulativeMessageSidRights.ContainsKey(value.Ace.SecurityIdentifier))
							{
								accumulativeMessageSidRights[value.Ace.SecurityIdentifier] = 0;
							}
							Dictionary<SecurityIdentifier, int> dictionary = (target == FolderSecurity.AceTarget.Folder) ? accumulativeFolderSidRights : accumulativeMessageSidRights;
							dictionary[value.Ace.SecurityIdentifier] = (dictionary[value.Ace.SecurityIdentifier] | value.Ace.AccessMask);
							goto IL_272;
						}
						goto IL_272;
						IL_1C9:
						annotatedAce = null;
						goto IL_1D1;
					}
					IL_272:;
				}
				errorInformation = string.Empty;
				return true;
			}

			public static RawAcl BuildFolderCanonicalAceList(IList<FolderSecurity.SecurityIdentifierAndFolderRights> sidAndRightsList)
			{
				return FolderSecurity.AnnotatedAceList.BuildCanonicalAceList(sidAndRightsList, new FolderSecurity.AnnotatedAceList.AppendUserAceToAclDelegate(FolderSecurity.AnnotatedAceList.AppendUserAceToFolderAcl), new FolderSecurity.AnnotatedAceList.AppendGroupAceToAclDelegate(FolderSecurity.AnnotatedAceList.AppendGroupAceToFolderAcl));
			}

			public static RawAcl BuildFreeBusyCanonicalAceList(IList<FolderSecurity.SecurityIdentifierAndFolderRights> sidAndRightsList)
			{
				return FolderSecurity.AnnotatedAceList.BuildCanonicalAceList(sidAndRightsList, new FolderSecurity.AnnotatedAceList.AppendUserAceToAclDelegate(FolderSecurity.AnnotatedAceList.AppendUserAceToFreeBusyAcl), new FolderSecurity.AnnotatedAceList.AppendGroupAceToAclDelegate(FolderSecurity.AnnotatedAceList.AppendGroupAceToFreeBusyAcl));
			}

			private static RawAcl BuildCanonicalAceList(IList<FolderSecurity.SecurityIdentifierAndFolderRights> sidAndRightsList, FolderSecurity.AnnotatedAceList.AppendUserAceToAclDelegate appendUserAceToAclDelegate, FolderSecurity.AnnotatedAceList.AppendGroupAceToAclDelegate appendGroupAceToAclDelegate)
			{
				RawAcl rawAcl = new RawAcl(2, sidAndRightsList.Count * 4);
				List<int> list = new List<int>(sidAndRightsList.Count);
				FolderSecurity.SecurityIdentifierAndFolderRights securityIdentifierAndFolderRights = null;
				FolderSecurity.SecurityIdentifierAndFolderRights securityIdentifierAndFolderRights2 = null;
				for (int i = 0; i < sidAndRightsList.Count; i++)
				{
					FolderSecurity.SecurityIdentifierAndFolderRights securityIdentifierAndFolderRights3 = sidAndRightsList[i];
					switch (securityIdentifierAndFolderRights3.SecurityIdentifierType)
					{
					case FolderSecurity.SecurityIdentifierType.Unknown:
					case FolderSecurity.SecurityIdentifierType.User:
						if (securityIdentifierAndFolderRights3.SecurityIdentifier.IsWellKnown(WellKnownSidType.WorldSid))
						{
							securityIdentifierAndFolderRights = securityIdentifierAndFolderRights3;
						}
						else if (securityIdentifierAndFolderRights3.SecurityIdentifier.IsWellKnown(WellKnownSidType.AnonymousSid))
						{
							securityIdentifierAndFolderRights2 = securityIdentifierAndFolderRights3;
						}
						else
						{
							appendUserAceToAclDelegate(rawAcl, securityIdentifierAndFolderRights3, true);
						}
						break;
					case FolderSecurity.SecurityIdentifierType.Group:
						list.Add(i);
						break;
					}
				}
				foreach (int index in list)
				{
					appendGroupAceToAclDelegate(rawAcl, true, sidAndRightsList[index]);
				}
				foreach (int index2 in list)
				{
					appendGroupAceToAclDelegate(rawAcl, false, sidAndRightsList[index2]);
				}
				if (securityIdentifierAndFolderRights2 != null)
				{
					appendUserAceToAclDelegate(rawAcl, securityIdentifierAndFolderRights2, true);
				}
				if (securityIdentifierAndFolderRights != null)
				{
					appendUserAceToAclDelegate(rawAcl, securityIdentifierAndFolderRights, false);
				}
				return rawAcl;
			}

			private static void AppendGroupAceToFolderAcl(RawAcl acl, bool allowAccess, FolderSecurity.SecurityIdentifierAndFolderRights sidAndRights)
			{
				FolderSecurity.AnnotatedAceList.AppendAceToAcl(acl, allowAccess, FolderSecurity.AceTarget.Folder, sidAndRights);
				FolderSecurity.AnnotatedAceList.AppendAceToAcl(acl, allowAccess, FolderSecurity.AceTarget.Message, sidAndRights);
			}

			private static void AppendUserAceToFolderAcl(RawAcl acl, FolderSecurity.SecurityIdentifierAndFolderRights sidAndRights, bool addDenyAce)
			{
				FolderSecurity.AnnotatedAceList.InsertUserAceToFolderAcl(acl, acl.Count, sidAndRights, addDenyAce);
			}

			internal static void InsertUserAceToFolderAcl(RawAcl acl, int insertIndex, FolderSecurity.SecurityIdentifierAndFolderRights sidAndRights, bool addDenyAce)
			{
				int num = insertIndex;
				if (FolderSecurity.AnnotatedAceList.InsertAceToAcl(acl, num, true, FolderSecurity.AceTarget.Folder, sidAndRights))
				{
					num++;
				}
				if (addDenyAce && FolderSecurity.AnnotatedAceList.InsertAceToAcl(acl, num, false, FolderSecurity.AceTarget.Folder, sidAndRights))
				{
					num++;
				}
				if (FolderSecurity.AnnotatedAceList.InsertAceToAcl(acl, num, true, FolderSecurity.AceTarget.Message, sidAndRights))
				{
					num++;
				}
				if (addDenyAce && FolderSecurity.AnnotatedAceList.InsertAceToAcl(acl, num, false, FolderSecurity.AceTarget.Message, sidAndRights))
				{
					num++;
				}
			}

			internal static void RemoveAcesFromAcl(RawAcl acl, SecurityIdentifier sid)
			{
				for (int i = acl.Count - 1; i >= 0; i--)
				{
					KnownAce knownAce = acl[i] as KnownAce;
					if (knownAce != null && knownAce.SecurityIdentifier == sid)
					{
						acl.RemoveAce(i);
					}
				}
			}

			private static void AppendGroupAceToFreeBusyAcl(RawAcl acl, bool allowAccess, FolderSecurity.SecurityIdentifierAndFolderRights sidAndRights)
			{
				FolderSecurity.AnnotatedAceList.AppendAceToAcl(acl, allowAccess, FolderSecurity.AceTarget.FreeBusy, sidAndRights);
			}

			private static void AppendUserAceToFreeBusyAcl(RawAcl acl, FolderSecurity.SecurityIdentifierAndFolderRights sidAndRights, bool addDenyAce)
			{
				FolderSecurity.AnnotatedAceList.AppendAceToAcl(acl, true, FolderSecurity.AceTarget.FreeBusy, sidAndRights);
				if (addDenyAce)
				{
					FolderSecurity.AnnotatedAceList.AppendAceToAcl(acl, false, FolderSecurity.AceTarget.FreeBusy, sidAndRights);
				}
			}

			private static void AppendAceToAcl(RawAcl acl, bool allowAccess, FolderSecurity.AceTarget aceTarget, FolderSecurity.SecurityIdentifierAndFolderRights sidAndRights)
			{
				FolderSecurity.AnnotatedAceList.InsertAceToAcl(acl, acl.Count, allowAccess, aceTarget, sidAndRights);
			}

			private static bool InsertAceToAcl(RawAcl acl, int insertIndex, bool allowAccess, FolderSecurity.AceTarget aceTarget, FolderSecurity.SecurityIdentifierAndFolderRights sidAndRights)
			{
				FolderSecurity.ExchangeSecurityDescriptorFolderRights exchangeSecurityDescriptorFolderRights = FolderSecurity.SecurityDescriptorRightsFromFolderRights(sidAndRights.AllowRights, aceTarget);
				FolderSecurity.ExchangeSecurityDescriptorFolderRights exchangeSecurityDescriptorFolderRights2;
				if (allowAccess)
				{
					exchangeSecurityDescriptorFolderRights2 = exchangeSecurityDescriptorFolderRights;
				}
				else
				{
					FolderSecurity.ExchangeSecurityDescriptorFolderRights exchangeSecurityDescriptorFolderRights3 = FolderSecurity.SecurityDescriptorRightsFromFolderRights(sidAndRights.DenyRights, aceTarget);
					FolderSecurity.ExchangeSecurityDescriptorFolderRights exchangeSecurityDescriptorFolderRights4 = exchangeSecurityDescriptorFolderRights & exchangeSecurityDescriptorFolderRights3;
					exchangeSecurityDescriptorFolderRights3 &= ~exchangeSecurityDescriptorFolderRights4;
					exchangeSecurityDescriptorFolderRights2 = exchangeSecurityDescriptorFolderRights3;
				}
				if (exchangeSecurityDescriptorFolderRights2 == FolderSecurity.ExchangeSecurityDescriptorFolderRights.None)
				{
					return false;
				}
				acl.InsertAce(insertIndex, new CommonAce(FolderSecurity.AnnotatedAceList.GetAceFlagsForTarget(aceTarget), allowAccess ? AceQualifier.AccessAllowed : AceQualifier.AccessDenied, (int)exchangeSecurityDescriptorFolderRights2, sidAndRights.SecurityIdentifier, false, null));
				return true;
			}

			private static bool IsFolderAce(GenericAce ace)
			{
				return (ace.AceFlags & FolderSecurity.AnnotatedAceList.aceFlagsMask) == FolderSecurity.AnnotatedAceList.GetAceFlagsForTarget(FolderSecurity.AceTarget.Folder);
			}

			private static bool IsMessageAce(GenericAce ace)
			{
				return (ace.AceFlags & FolderSecurity.AnnotatedAceList.aceFlagsMask) == FolderSecurity.AnnotatedAceList.GetAceFlagsForTarget(FolderSecurity.AceTarget.Message);
			}

			private static AceFlags GetAceFlagsForTarget(FolderSecurity.AceTarget aceTarget)
			{
				if (aceTarget != FolderSecurity.AceTarget.Folder && aceTarget != FolderSecurity.AceTarget.FreeBusy)
				{
					return AceFlags.ObjectInherit | AceFlags.InheritOnly;
				}
				return AceFlags.ContainerInherit;
			}

			private static bool HasFullRights(int accessMask, FolderSecurity.AceTarget aceTarget)
			{
				switch (aceTarget)
				{
				case FolderSecurity.AceTarget.Folder:
					return (accessMask & -32869) == 2050459;
				case FolderSecurity.AceTarget.Message:
					return (accessMask & -32869) == 2035611;
				case FolderSecurity.AceTarget.FreeBusy:
					return accessMask == 3;
				default:
					return false;
				}
			}

			internal string CreateErrorInformation(LID errorLocation, params int[] errorParameters)
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (FolderSecurity.AnnotatedAceList.AnnotatedAce annotatedAce in this.aceEntries)
				{
					stringBuilder.AppendFormat("({0};{1};{2};{3:X};{4};{5})", new object[]
					{
						annotatedAce.IsAllowAce ? 'A' : 'D',
						annotatedAce.Ace.SecurityIdentifier,
						annotatedAce.Target,
						annotatedAce.Ace.AccessMask,
						annotatedAce.CanonicalType,
						annotatedAce.SecurityIdentifierType
					});
				}
				stringBuilder.Append(";EL:");
				stringBuilder.Append(errorLocation.Value);
				foreach (int value in errorParameters)
				{
					stringBuilder.Append(';');
					stringBuilder.Append(value);
				}
				return stringBuilder.ToString();
			}

			private FolderSecurity.AnnotatedAceList.AceCanonicalType GetAceCanonicalType(KnownAce ace, out FolderSecurity.AceTarget aceTarget, out FolderSecurity.SecurityIdentifierType securityIdentifierType)
			{
				aceTarget = FolderSecurity.AceTarget.Folder;
				securityIdentifierType = FolderSecurity.SecurityIdentifierType.User;
				if (FolderSecurity.AnnotatedAceList.IsFolderAce(ace))
				{
					aceTarget = FolderSecurity.AceTarget.Folder;
				}
				else
				{
					if (!FolderSecurity.AnnotatedAceList.IsMessageAce(ace))
					{
						return FolderSecurity.AnnotatedAceList.AceCanonicalType.Invalid;
					}
					aceTarget = FolderSecurity.AceTarget.Message;
				}
				return this.GetAceCanonicalTypeForTarget(ace, aceTarget, out securityIdentifierType);
			}

			private FolderSecurity.AnnotatedAceList.AceCanonicalType GetAceCanonicalTypeForTarget(KnownAce ace, FolderSecurity.AceTarget aceTarget, out FolderSecurity.SecurityIdentifierType securityIdentifierType)
			{
				securityIdentifierType = FolderSecurity.SecurityIdentifierType.User;
				bool flag;
				if (ace.AceType == AceType.AccessAllowed)
				{
					flag = true;
				}
				else
				{
					if (ace.AceType != AceType.AccessDenied)
					{
						return FolderSecurity.AnnotatedAceList.AceCanonicalType.Invalid;
					}
					flag = false;
				}
				if (ace.SecurityIdentifier.IsWellKnown(WellKnownSidType.WorldSid))
				{
					if (!flag)
					{
						return FolderSecurity.AnnotatedAceList.AceCanonicalType.Invalid;
					}
					return FolderSecurity.AnnotatedAceList.AceCanonicalType.Everyone;
				}
				else if (ace.SecurityIdentifier.IsWellKnown(WellKnownSidType.AnonymousSid))
				{
					if (!flag)
					{
						if (!FolderSecurity.AnnotatedAceList.HasFullRights(ace.AccessMask, aceTarget))
						{
							return FolderSecurity.AnnotatedAceList.AceCanonicalType.UserDenyPartial;
						}
						return FolderSecurity.AnnotatedAceList.AceCanonicalType.UserDenyFull;
					}
					else
					{
						if (!FolderSecurity.AnnotatedAceList.HasFullRights(ace.AccessMask, aceTarget))
						{
							return FolderSecurity.AnnotatedAceList.AceCanonicalType.UserAllowPartial;
						}
						return FolderSecurity.AnnotatedAceList.AceCanonicalType.UserAllowFull;
					}
				}
				else
				{
					securityIdentifierType = this.securityIdentifierTypeResolver(ace.SecurityIdentifier);
					switch (securityIdentifierType)
					{
					case FolderSecurity.SecurityIdentifierType.Unknown:
						if (!flag)
						{
							if (!FolderSecurity.AnnotatedAceList.HasFullRights(ace.AccessMask, aceTarget))
							{
								return FolderSecurity.AnnotatedAceList.AceCanonicalType.UnknownDenyPartial;
							}
							return FolderSecurity.AnnotatedAceList.AceCanonicalType.UnknownDenyFull;
						}
						else
						{
							if (!FolderSecurity.AnnotatedAceList.HasFullRights(ace.AccessMask, aceTarget))
							{
								return FolderSecurity.AnnotatedAceList.AceCanonicalType.UnknownAllowPartial;
							}
							return FolderSecurity.AnnotatedAceList.AceCanonicalType.UnknownAllowFull;
						}
						break;
					case FolderSecurity.SecurityIdentifierType.User:
						if (!flag)
						{
							if (!FolderSecurity.AnnotatedAceList.HasFullRights(ace.AccessMask, aceTarget))
							{
								return FolderSecurity.AnnotatedAceList.AceCanonicalType.UserDenyPartial;
							}
							return FolderSecurity.AnnotatedAceList.AceCanonicalType.UserDenyFull;
						}
						else
						{
							if (!FolderSecurity.AnnotatedAceList.HasFullRights(ace.AccessMask, aceTarget))
							{
								return FolderSecurity.AnnotatedAceList.AceCanonicalType.UserAllowPartial;
							}
							return FolderSecurity.AnnotatedAceList.AceCanonicalType.UserAllowFull;
						}
						break;
					case FolderSecurity.SecurityIdentifierType.Group:
						if (!flag)
						{
							return FolderSecurity.AnnotatedAceList.AceCanonicalType.GroupDeny;
						}
						return FolderSecurity.AnnotatedAceList.AceCanonicalType.GroupAllow;
					default:
						return FolderSecurity.AnnotatedAceList.AceCanonicalType.Invalid;
					}
				}
			}

			// Note: this type is marked as 'beforefieldinit'.
			static AnnotatedAceList()
			{
				bool[][] array = new bool[8][];
				array[0] = new bool[]
				{
					true,
					true,
					true,
					true,
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					false
				};
				bool[][] array2 = array;
				int num = 1;
				bool[] array3 = new bool[12];
				array3[2] = true;
				array3[3] = true;
				array2[num] = array3;
				array[2] = new bool[]
				{
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					false
				};
				array[3] = new bool[]
				{
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					false
				};
				array[4] = new bool[]
				{
					false,
					false,
					false,
					false,
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					true
				};
				array[5] = new bool[]
				{
					false,
					false,
					false,
					false,
					false,
					true,
					true,
					false,
					false,
					false,
					true,
					true
				};
				bool[][] array4 = array;
				int num2 = 6;
				bool[] array5 = new bool[12];
				array5[6] = true;
				array4[num2] = array5;
				array[7] = new bool[]
				{
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					false,
					true,
					true,
					true,
					false
				};
				FolderSecurity.AnnotatedAceList.knowAceTransitions = array;
				FolderSecurity.AnnotatedAceList.userSectionUnknownAceTransition = new bool[][]
				{
					new bool[]
					{
						true,
						true,
						true,
						true,
						false,
						false,
						true,
						false,
						true,
						true,
						true,
						true
					},
					new bool[]
					{
						false,
						false,
						true,
						true,
						false,
						false,
						false,
						false,
						false,
						false,
						true,
						true
					},
					new bool[]
					{
						true,
						true,
						true,
						false,
						false,
						false,
						true,
						false,
						true,
						true,
						true,
						false
					},
					new bool[]
					{
						true,
						true,
						true,
						false,
						false,
						false,
						true,
						false,
						true,
						true,
						true,
						false
					}
				};
				FolderSecurity.AnnotatedAceList.groupSectionUnknownAceTransition = new bool[][]
				{
					new bool[]
					{
						false,
						false,
						false,
						false,
						true,
						true,
						true,
						false,
						true,
						true,
						true,
						true
					},
					new bool[]
					{
						false,
						false,
						false,
						false,
						true,
						true,
						false,
						false,
						true,
						true,
						true,
						true
					},
					new bool[]
					{
						false,
						false,
						false,
						false,
						false,
						true,
						true,
						false,
						false,
						false,
						true,
						true
					},
					new bool[]
					{
						false,
						false,
						false,
						false,
						false,
						true,
						true,
						false,
						false,
						false,
						true,
						true
					}
				};
				FolderSecurity.AnnotatedAceList.userToGroupUnknownAceTransition = new bool[][]
				{
					new bool[]
					{
						false,
						false,
						false,
						false,
						true,
						true,
						true,
						false,
						true,
						true,
						true,
						true
					},
					new bool[]
					{
						false,
						false,
						false,
						false,
						true,
						true,
						true,
						false,
						true,
						true,
						true,
						true
					},
					new bool[]
					{
						false,
						false,
						false,
						false,
						true,
						true,
						true,
						false,
						true,
						true,
						true,
						true
					},
					new bool[]
					{
						false,
						false,
						false,
						false,
						true,
						true,
						true,
						false,
						true,
						true,
						true,
						true
					}
				};
				FolderSecurity.AnnotatedAceList.userSectionTransitions = new List<bool[]>(FolderSecurity.AnnotatedAceList.knowAceTransitions).Concat(FolderSecurity.AnnotatedAceList.userSectionUnknownAceTransition).ToArray<bool[]>();
				FolderSecurity.AnnotatedAceList.userToGroupTransition = new List<bool[]>(FolderSecurity.AnnotatedAceList.knowAceTransitions).Concat(FolderSecurity.AnnotatedAceList.userToGroupUnknownAceTransition).ToArray<bool[]>();
				FolderSecurity.AnnotatedAceList.groupSectionTransitions = new List<bool[]>(FolderSecurity.AnnotatedAceList.knowAceTransitions).Concat(FolderSecurity.AnnotatedAceList.groupSectionUnknownAceTransition).ToArray<bool[]>();
			}

			private static readonly AceFlags aceFlagsMask = AceFlags.ObjectInherit | AceFlags.ContainerInherit | AceFlags.InheritOnly;

			private readonly bool objectAceFound;

			private readonly bool nonCanonicalAceFound;

			private readonly IList<FolderSecurity.AnnotatedAceList.AnnotatedAce> aceEntries;

			private readonly Func<SecurityIdentifier, FolderSecurity.SecurityIdentifierType> securityIdentifierTypeResolver;

			private static bool[][] knowAceTransitions;

			private static bool[][] userSectionUnknownAceTransition;

			private static bool[][] groupSectionUnknownAceTransition;

			private static bool[][] userToGroupUnknownAceTransition;

			private static bool[][] userSectionTransitions;

			private static bool[][] userToGroupTransition;

			private static bool[][] groupSectionTransitions;

			private delegate void AppendUserAceToAclDelegate(RawAcl acl, FolderSecurity.SecurityIdentifierAndFolderRights sidAndRights, bool addDenyAce);

			private delegate void AppendGroupAceToAclDelegate(RawAcl acl, bool allowAccess, FolderSecurity.SecurityIdentifierAndFolderRights sidAndRights);

			private struct AnnotatedAce
			{
				public AnnotatedAce(KnownAce ace, FolderSecurity.AnnotatedAceList.AceCanonicalType aceCanonicalType, FolderSecurity.AceTarget aceTarget, FolderSecurity.SecurityIdentifierType securityIdentifierType)
				{
					this.ace = ace;
					this.aceCanonicalType = aceCanonicalType;
					this.aceTarget = aceTarget;
					this.securityIdentifierType = securityIdentifierType;
				}

				public KnownAce Ace
				{
					get
					{
						return this.ace;
					}
				}

				public FolderSecurity.AnnotatedAceList.AceCanonicalType CanonicalType
				{
					get
					{
						return this.aceCanonicalType;
					}
				}

				public bool IsAllowAce
				{
					get
					{
						return this.ace.AceType == AceType.AccessAllowed;
					}
				}

				public FolderSecurity.AceTarget Target
				{
					get
					{
						return this.aceTarget;
					}
				}

				public FolderSecurity.SecurityIdentifierType SecurityIdentifierType
				{
					get
					{
						return this.securityIdentifierType;
					}
				}

				private readonly KnownAce ace;

				private readonly FolderSecurity.AnnotatedAceList.AceCanonicalType aceCanonicalType;

				private readonly FolderSecurity.AceTarget aceTarget;

				private readonly FolderSecurity.SecurityIdentifierType securityIdentifierType;
			}

			private enum AceCanonicalType
			{
				UserAllowFull,
				UserAllowPartial,
				UserDenyFull,
				UserDenyPartial,
				GroupAllow,
				GroupDeny,
				Everyone,
				LastCanonicalValue = 6,
				Invalid,
				UnknownAllowFull,
				UnknownAllowPartial,
				UnknownDenyFull,
				UnknownDenyPartial
			}
		}
	}
}
