using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ExternalUserCollection : ICollection<ExternalUser>, IEnumerable<ExternalUser>, IEnumerable, IDisposeTrackable, IDisposable
	{
		internal ExternalUserCollection(MailboxSession session)
		{
			this.data = new List<ExternalUser>();
			byte[] entryId = null;
			StoreSession storeSession = null;
			bool flag = false;
			try
			{
				if (storeSession != null)
				{
					storeSession.BeginMapiCall();
					storeSession.BeginServerHealthCall();
					flag = true;
				}
				if (StorageGlobals.MapiTestHookBeforeCall != null)
				{
					StorageGlobals.MapiTestHookBeforeCall(MethodBase.GetCurrentMethod());
				}
				entryId = session.Mailbox.MapiStore.GetLocalDirectoryEntryId();
			}
			catch (MapiPermanentException ex)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotLookupEntryId, ex, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to lookup Local Directory EntryID", new object[0]),
					ex
				});
			}
			catch (MapiRetryableException ex2)
			{
				throw StorageGlobals.TranslateMapiException(ServerStrings.MapiCannotLookupEntryId, ex2, storeSession, this, "{0}. MapiException = {1}.", new object[]
				{
					string.Format("Unable to lookup Local Directory EntryID", new object[0]),
					ex2
				});
			}
			finally
			{
				try
				{
					if (storeSession != null)
					{
						storeSession.EndMapiCall();
						if (flag)
						{
							storeSession.EndServerHealthCall();
						}
					}
				}
				finally
				{
					if (StorageGlobals.MapiTestHookAfterCall != null)
					{
						StorageGlobals.MapiTestHookAfterCall(MethodBase.GetCurrentMethod());
					}
				}
			}
			StoreObjectId messageId = StoreObjectId.FromProviderSpecificId(entryId, StoreObjectType.Message);
			this.directoryMessage = MessageItem.Bind(session, messageId, new PropertyDefinition[]
			{
				InternalSchema.LocalDirectory
			});
			try
			{
				this.directoryMessage.OpenAsReadWrite();
				try
				{
					using (Stream stream = this.directoryMessage.OpenPropertyStream(InternalSchema.LocalDirectory, PropertyOpenMode.ReadOnly))
					{
						long length = stream.Length;
						using (BinaryReader binaryReader = new BinaryReader(stream))
						{
							while (stream.Position < length)
							{
								ExternalUser item;
								if (ExternalUserCollection.TryReadEntry(binaryReader, out item) && !this.Contains(item))
								{
									this.data.Add(item);
								}
							}
						}
					}
				}
				catch (ObjectNotFoundException)
				{
				}
				catch (EndOfStreamException)
				{
				}
			}
			catch (StoragePermanentException)
			{
				if (this.directoryMessage != null)
				{
					this.directoryMessage.Dispose();
				}
				throw;
			}
			catch (StorageTransientException)
			{
				if (this.directoryMessage != null)
				{
					this.directoryMessage.Dispose();
				}
				throw;
			}
			StorageGlobals.TraceConstructIDisposable(this);
			this.disposeTracker = this.GetDisposeTracker();
		}

		private static bool TryReadEntry(BinaryReader reader, out ExternalUser user)
		{
			if (reader.ReadUInt32() == ExternalUserCollection.ptagLocalDirectoryEntryId)
			{
				uint num = reader.ReadUInt32();
				MemoryPropertyBag memoryPropertyBag = new MemoryPropertyBag();
				int num2 = 0;
				while ((long)num2 < (long)((ulong)num))
				{
					uint num3 = reader.ReadUInt32();
					object obj = ExternalUserCollection.ReadPropValue(reader, ((PropTag)num3).ValueType(), reader.ReadUInt32());
					if (obj != null)
					{
						PropertyTagPropertyDefinition prop = PropertyTagPropertyDefinition.CreateCustom(string.Empty, num3);
						memoryPropertyBag.PreLoadStoreProperty(prop, obj, (int)num);
					}
					num2++;
				}
				memoryPropertyBag.SetAllPropertiesLoaded();
				IDirectPropertyBag directPropertyBag = memoryPropertyBag;
				if (directPropertyBag.IsLoaded(InternalSchema.MemberSIDLocalDirectory) && directPropertyBag.IsLoaded(InternalSchema.MemberExternalIdLocalDirectory) && directPropertyBag.IsLoaded(InternalSchema.MemberEmailLocalDirectory))
				{
					if (!directPropertyBag.IsLoaded(InternalSchema.MemberName))
					{
						memoryPropertyBag[InternalSchema.MemberName] = directPropertyBag.GetValue(InternalSchema.MemberEmailLocalDirectory);
					}
					user = new ExternalUser(memoryPropertyBag);
					return true;
				}
			}
			user = null;
			return false;
		}

		private static object ReadPropValue(BinaryReader reader, PropType type, uint len)
		{
			if (type <= PropType.Binary)
			{
				if (type == PropType.Int)
				{
					return ExternalUserCollection.ReadInt(reader, len);
				}
				if (type == PropType.String)
				{
					return ExternalUserCollection.ReadString(reader, len);
				}
				if (type == PropType.Binary)
				{
					return ExternalUserCollection.ReadBytes(reader, len);
				}
			}
			else
			{
				if (type == PropType.IntArray)
				{
					return ExternalUserCollection.ReadArrayValue<int>(reader, len, new ExternalUserCollection.ReadValue<int>(ExternalUserCollection.ReadInt));
				}
				if (type == PropType.StringArray)
				{
					return ExternalUserCollection.ReadArrayValue<string>(reader, len, new ExternalUserCollection.ReadValue<string>(ExternalUserCollection.ReadString));
				}
				if (type == PropType.BinaryArray)
				{
					return ExternalUserCollection.ReadArrayValue<byte[]>(reader, len, new ExternalUserCollection.ReadValue<byte[]>(ExternalUserCollection.ReadBytes));
				}
			}
			return null;
		}

		private static T[] ReadArrayValue<T>(BinaryReader reader, uint len, ExternalUserCollection.ReadValue<T> readCall)
		{
			T[] array = new T[len];
			int num = 0;
			while ((long)num < (long)((ulong)len))
			{
				uint len2 = reader.ReadUInt32();
				array[num] = readCall(reader, len2);
				num++;
			}
			return array;
		}

		private static string ReadString(BinaryReader reader, uint len)
		{
			string text = new string(Encoding.Unicode.GetChars(ExternalUserCollection.ReadBytes(reader, len)));
			string text2 = text;
			char[] trimChars = new char[1];
			return text2.TrimEnd(trimChars);
		}

		private static byte[] ReadBytes(BinaryReader reader, uint len)
		{
			byte[] result = reader.ReadBytes((int)len);
			if (len % 4U != 0U)
			{
				reader.ReadBytes((int)(len % 4U));
			}
			return result;
		}

		private static int ReadInt(BinaryReader reader, uint len)
		{
			return reader.ReadInt32();
		}

		private static void WriteArrayValue<T>(BinaryWriter writer, T[] value, ExternalUserCollection.WriteValue<T> callback)
		{
			writer.Write((uint)value.Length);
			for (int i = 0; i < value.Length; i++)
			{
				callback(writer, value[i]);
			}
		}

		private static void WriteStringValue(BinaryWriter writer, string value)
		{
			Encoding unicode = Encoding.Unicode;
			char[] trimChars = new char[1];
			byte[] bytes = unicode.GetBytes(value.TrimEnd(trimChars));
			int num = (bytes.Length + 2) % 4 + 2;
			writer.Write((uint)(bytes.Length + num));
			writer.Write(bytes);
			for (int i = 0; i < num; i++)
			{
				writer.Write(0);
			}
		}

		private static void WriteByteValue(BinaryWriter writer, byte[] value)
		{
			writer.Write((uint)value.Length);
			writer.Write(value);
			int num = value.Length % 4;
			for (int i = 0; i < num; i++)
			{
				writer.Write(0);
			}
		}

		private static void WriteIntValue(BinaryWriter writer, int value)
		{
			writer.Write(4U);
			writer.Write(value);
		}

		private static void WritePropValue(BinaryWriter writer, PropertyDefinition prop, MemoryPropertyBag propertyBag)
		{
			PropertyTagPropertyDefinition propertyTagPropertyDefinition = (PropertyTagPropertyDefinition)prop;
			writer.Write(propertyTagPropertyDefinition.PropertyTag);
			object obj = propertyBag.TryGetProperty(propertyTagPropertyDefinition);
			PropType propType = ((PropTag)propertyTagPropertyDefinition.PropertyTag).ValueType();
			PropType propType2 = propType;
			if (propType2 <= PropType.Binary)
			{
				if (propType2 == PropType.Int)
				{
					ExternalUserCollection.WriteIntValue(writer, (int)obj);
					return;
				}
				if (propType2 == PropType.String)
				{
					ExternalUserCollection.WriteStringValue(writer, (string)obj);
					return;
				}
				if (propType2 == PropType.Binary)
				{
					ExternalUserCollection.WriteByteValue(writer, (byte[])obj);
					return;
				}
			}
			else
			{
				if (propType2 == PropType.IntArray)
				{
					ExternalUserCollection.WriteArrayValue<int>(writer, (int[])obj, new ExternalUserCollection.WriteValue<int>(ExternalUserCollection.WriteIntValue));
					return;
				}
				if (propType2 == PropType.StringArray)
				{
					ExternalUserCollection.WriteArrayValue<string>(writer, (string[])obj, new ExternalUserCollection.WriteValue<string>(ExternalUserCollection.WriteStringValue));
					return;
				}
				if (propType2 == PropType.BinaryArray)
				{
					ExternalUserCollection.WriteArrayValue<byte[]>(writer, (byte[][])obj, new ExternalUserCollection.WriteValue<byte[]>(ExternalUserCollection.WriteByteValue));
					return;
				}
			}
			writer.Write(0U);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			this.CheckDisposed("GetEnumerator");
			return this.data.GetEnumerator();
		}

		IEnumerator<ExternalUser> IEnumerable<ExternalUser>.GetEnumerator()
		{
			this.CheckDisposed("GetEnumerator");
			return this.data.GetEnumerator();
		}

		public int Count
		{
			get
			{
				this.CheckDisposed("Count::get");
				return this.data.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				this.CheckDisposed("IsReadOnly::get");
				return false;
			}
		}

		public void Add(ExternalUser item)
		{
			this.CheckDisposed("Add");
			if (item == null)
			{
				throw new ArgumentNullException();
			}
			if (this.Count >= 1000)
			{
				throw new InvalidOperationException("Cannot add any more users to the ExternalUserCollection.");
			}
			if (this.Contains(item))
			{
				throw new ArgumentException("User is already present in the collection.");
			}
			this.data.Add(item);
			this.isDirty = true;
		}

		public void Clear()
		{
			this.CheckDisposed("Clear");
			if (this.data.Count > 0)
			{
				this.data.Clear();
				this.isDirty = true;
			}
		}

		public bool Contains(ExternalUser item)
		{
			this.CheckDisposed("Contains");
			if (item == null)
			{
				throw new ArgumentNullException();
			}
			foreach (ExternalUser externalUser in this.data)
			{
				if (externalUser.SmtpAddress.Equals(item.SmtpAddress) || externalUser.Sid.Equals(item.Sid) || externalUser.ExternalId == item.ExternalId)
				{
					return true;
				}
			}
			return false;
		}

		public void CopyTo(ExternalUser[] array, int arrayIndex)
		{
			this.CheckDisposed("CopyTo");
			if (arrayIndex < 0)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (array == null)
			{
				throw new ArgumentNullException();
			}
			if (array.Length - arrayIndex > this.data.Count)
			{
				throw new ArgumentException();
			}
			foreach (ExternalUser externalUser in this.data)
			{
				array[arrayIndex] = externalUser;
				arrayIndex++;
			}
		}

		public bool Remove(ExternalUser item)
		{
			this.CheckDisposed("Remove");
			if (item == null)
			{
				throw new ArgumentNullException();
			}
			if (this.data.Remove(this.data.Find((ExternalUser obj) => obj.Sid.Equals(item.Sid) || obj.ExternalId == item.ExternalId)))
			{
				this.isDirty = true;
				return true;
			}
			return false;
		}

		private static string CreateExternalIdentity(SmtpAddress smtpAddress)
		{
			byte[] array = new byte[24];
			Util.GetRandomBytes(array);
			string str = Convert.ToBase64String(array);
			return str + "@" + smtpAddress.Domain;
		}

		public ExternalUser AddFederatedUser(SmtpAddress smtpAddress)
		{
			return this.InternalAdd(smtpAddress, false);
		}

		public ExternalUser AddReachUser(SmtpAddress smtpAddress)
		{
			return this.InternalAdd(smtpAddress, true);
		}

		public ExternalUser FindReachUserWithOriginalSmtpAddress(SmtpAddress originalSmtpAddress)
		{
			this.CheckDisposed("FindReachUserWithOriginalSmtpAddress");
			SmtpAddress smtpAddress = ExternalUser.ConvertToReachSmtpAddress(originalSmtpAddress);
			ExternalUser externalUser = this.FindExternalUser(smtpAddress);
			if (externalUser != null && !externalUser.IsReachUser)
			{
				externalUser = null;
			}
			return externalUser;
		}

		public ExternalUser FindFederatedUserWithOriginalSmtpAddress(SmtpAddress originalSmtpAddress)
		{
			this.CheckDisposed("FindFederatedUserWithOriginalSmtpAddress");
			ExternalUser externalUser = this.FindExternalUser(originalSmtpAddress);
			if (externalUser != null && externalUser.IsReachUser)
			{
				externalUser = null;
			}
			return externalUser;
		}

		public ExternalUser FindExternalUser(SmtpAddress smtpAddress)
		{
			this.CheckDisposed("FindExternalUser");
			return this.data.Find((ExternalUser obj) => obj.SmtpAddress == smtpAddress);
		}

		public ExternalUser FindExternalUser(string externalId)
		{
			this.CheckDisposed("FindExternalUser");
			if (externalId == null)
			{
				throw new ArgumentNullException();
			}
			return this.data.Find((ExternalUser obj) => obj.ExternalId == externalId);
		}

		public ExternalUser FindExternalUser(SecurityIdentifier sid)
		{
			this.CheckDisposed("FindExternalUser");
			if (sid == null)
			{
				throw new ArgumentNullException();
			}
			return this.data.Find((ExternalUser obj) => obj.Sid.Equals(sid));
		}

		public SaveResult Save()
		{
			this.CheckDisposed("Save");
			if (!this.isDirty)
			{
				bool flag = false;
				foreach (ExternalUser externalUser in this.data)
				{
					flag |= externalUser.PropertyBag.IsDirty;
				}
				if (!flag)
				{
					return SaveResult.Success;
				}
			}
			using (Stream stream = this.directoryMessage.OpenPropertyStream(InternalSchema.LocalDirectory, PropertyOpenMode.Create))
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(stream))
				{
					foreach (ExternalUser externalUser2 in this.data)
					{
						binaryWriter.Write(ExternalUserCollection.ptagLocalDirectoryEntryId);
						ICollection<PropertyDefinition> allFoundProperties = externalUser2.PropertyBag.AllFoundProperties;
						binaryWriter.Write((uint)allFoundProperties.Count);
						ExternalUserCollection.WritePropValue(binaryWriter, InternalSchema.MemberSIDLocalDirectory, externalUser2.PropertyBag);
						foreach (PropertyDefinition propertyDefinition in allFoundProperties)
						{
							if (propertyDefinition != InternalSchema.MemberSIDLocalDirectory)
							{
								ExternalUserCollection.WritePropValue(binaryWriter, propertyDefinition, externalUser2.PropertyBag);
							}
						}
					}
				}
			}
			ConflictResolutionResult conflictResolutionResult = this.directoryMessage.Save(SaveMode.ResolveConflicts);
			return conflictResolutionResult.SaveStatus;
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ExternalUserCollection>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		private void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				StorageGlobals.TraceFailedCheckDisposed(this, methodName);
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private ExternalUser InternalAdd(SmtpAddress smtpAddress, bool isReachUser)
		{
			this.CheckDisposed("InternalAdd");
			string externalId = ExternalUserCollection.CreateExternalIdentity(smtpAddress);
			ExternalUser externalUser = new ExternalUser(externalId, smtpAddress, isReachUser);
			this.Add(externalUser);
			return externalUser;
		}

		private void Dispose(bool disposing)
		{
			StorageGlobals.TraceDispose(this, this.isDisposed, disposing);
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.InternalDispose(disposing);
			}
		}

		private void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.directoryMessage != null)
				{
					this.directoryMessage.Dispose();
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
				}
			}
		}

		internal const int MaxLimit = 1000;

		private readonly MessageItem directoryMessage;

		private readonly List<ExternalUser> data;

		private bool isDisposed;

		private bool isDirty;

		private readonly DisposeTracker disposeTracker;

		private static uint ptagLocalDirectoryEntryId = 873857282U;

		private delegate T ReadValue<T>(BinaryReader reader, uint len);

		private delegate void WriteValue<T>(BinaryWriter writer, T value);
	}
}
