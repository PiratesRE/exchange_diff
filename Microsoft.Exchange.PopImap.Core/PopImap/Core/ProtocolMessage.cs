using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Common.IL;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Mime.Internal;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.PopImap.Core
{
	internal abstract class ProtocolMessage : IComparable
	{
		protected ProtocolMessage(int index, int imapId, int docId, int size)
		{
			this.index = index;
			this.id = imapId;
			this.documentId = docId;
			this.Size = size;
		}

		public int Index
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}

		public int Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		public abstract bool IsNotRfc822Renderable { get; set; }

		public bool IsDeleted
		{
			get
			{
				return this.deleted;
			}
			set
			{
				this.deleted = value;
			}
		}

		public abstract StoreObjectId Uid { get; }

		public abstract ResponseFactory Factory { get; }

		internal int DocumentId
		{
			get
			{
				return this.documentId;
			}
			set
			{
				this.documentId = value;
			}
		}

		internal int Size { get; private set; }

		public static Stream CreateMimeStream()
		{
			return new StreamWrapper();
		}

		public static MimeHeaderStream CreateMimeHeaderStream()
		{
			return new MimeHeaderStream();
		}

		public static string Rfc2047Encode(string value, Encoding encoding)
		{
			return MimeInternalHelpers.Rfc2047Encode(value, encoding);
		}

		public override string ToString()
		{
			return string.Format("{0} {1}, user \"{2}\"", base.GetType().Name, this.id, this.Factory.UserName);
		}

		public int CompareTo(object obj)
		{
			ProtocolMessage protocolMessage = obj as ProtocolMessage;
			if (protocolMessage != null)
			{
				return this.Id - protocolMessage.Id;
			}
			throw new ArgumentException("object is not a ProtocolMessage");
		}

		public override bool Equals(object obj)
		{
			return this.CompareTo(obj) == 0;
		}

		public override int GetHashCode()
		{
			return this.id.GetHashCode();
		}

		internal static void HandleMimeConversionException(Item item, ProtocolMessage msg, string subject, string date, Exception exception)
		{
			string arg = string.Format("{0}, subject: \"{1}\", date: \"{2}\"", msg, subject, date);
			if (exception is ObjectNotFoundException)
			{
				ProtocolBaseServices.SessionTracer.TraceDebug<string, Exception>(msg.Factory.Session.SessionId, "Exception in Mime Conversion for message {0}\r\n{1}", arg, exception);
			}
			else
			{
				ProtocolBaseServices.SessionTracer.TraceError<string, Exception>(msg.Factory.Session.SessionId, "Exception in Mime Conversion for message {0}\r\n{1}", arg, exception);
			}
			if (!(exception is StorageTransientException))
			{
				msg.IsNotRfc822Renderable = true;
				if (item != null)
				{
					ProtocolMessage.StampPoisonMessageFlag(item);
				}
			}
		}

		internal static T ProcessMessageWithPoisonMessageHandling<T>(FilterDelegate filter, Func<T> processMessage)
		{
			ProtocolMessage.<>c__DisplayClass1<T> CS$<>8__locals1 = new ProtocolMessage.<>c__DisplayClass1<T>();
			CS$<>8__locals1.processMessage = processMessage;
			CS$<>8__locals1.t = default(T);
			ILUtil.DoTryFilterCatch(new TryDelegate(CS$<>8__locals1, (UIntPtr)ldftn(<ProcessMessageWithPoisonMessageHandling>b__0)), filter, null);
			return CS$<>8__locals1.t;
		}

		internal static void HandlePoisonMessageException(Exception e, Item item)
		{
			if (e is AccessViolationException || e is ArithmeticException || e is ArrayTypeMismatchException || e is ArgumentException || e is KeyNotFoundException || e is FormatException || e is IndexOutOfRangeException || e is InvalidCastException || e is InvalidOperationException || e is InvalidDataException || e is NotImplementedException || e is NotSupportedException || e is NullReferenceException)
			{
				ProtocolMessage.StampPoisonMessageFlag(item);
			}
		}

		internal static int GetDocIdFromInstanceKey(byte[] instanceKey)
		{
			if (instanceKey == null)
			{
				throw new ArgumentNullException("instanceKey");
			}
			if (instanceKey.Length != 20)
			{
				throw new ArgumentException("Invalid instance key:" + instanceKey.Length);
			}
			int num = instanceKey.Length;
			return (int)instanceKey[num - 4] | (int)instanceKey[num - 3] << 8 | (int)instanceKey[num - 2] << 16 | (int)instanceKey[num - 1] << 24;
		}

		private static void StampPoisonMessageFlag(Item item)
		{
			try
			{
				item.OpenAsReadWrite();
				item[MessageItemSchema.MimeConversionFailed] = true;
				item.Save(SaveMode.ResolveConflicts);
			}
			catch (LocalizedException)
			{
			}
		}

		private int index;

		private int id;

		private int documentId;

		private bool deleted;
	}
}
