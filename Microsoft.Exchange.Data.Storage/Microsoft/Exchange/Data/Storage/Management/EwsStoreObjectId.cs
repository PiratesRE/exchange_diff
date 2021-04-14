using System;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public class EwsStoreObjectId : ObjectId, ISerializable
	{
		public EwsStoreObjectId(ItemId ewsObjectId)
		{
			this.ewsObjectId = ewsObjectId;
		}

		public EwsStoreObjectId(string uniqueId) : this(new ItemId(uniqueId))
		{
		}

		public EwsStoreObjectId(byte[] bytes) : this(Encoding.UTF8.GetString(bytes))
		{
		}

		private EwsStoreObjectId(SerializationInfo info, StreamingContext context) : this(info.GetString("id"))
		{
		}

		public static bool TryParse(string id, out EwsStoreObjectId ewsStoreObjectId)
		{
			ewsStoreObjectId = null;
			try
			{
				StoreId.EwsIdToStoreObjectId(id);
			}
			catch (InvalidIdMalformedException)
			{
				return false;
			}
			catch (InvalidIdNotAnItemAttachmentIdException)
			{
				return false;
			}
			ewsStoreObjectId = new EwsStoreObjectId(id);
			return true;
		}

		public override bool Equals(object obj)
		{
			return obj != null && obj is EwsStoreObjectId && this.EwsObjectId.Equals(((EwsStoreObjectId)obj).EwsObjectId);
		}

		public override int GetHashCode()
		{
			return this.EwsObjectId.GetHashCode();
		}

		public override string ToString()
		{
			return this.EwsObjectId.ToString();
		}

		public override byte[] GetBytes()
		{
			return Encoding.UTF8.GetBytes(this.ToString());
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("id", this.ToString());
		}

		internal ItemId EwsObjectId
		{
			get
			{
				return this.ewsObjectId;
			}
		}

		[NonSerialized]
		private ItemId ewsObjectId;
	}
}
