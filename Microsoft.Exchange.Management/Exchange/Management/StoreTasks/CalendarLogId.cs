using System;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Serializable]
	public class CalendarLogId : ObjectId
	{
		public CalendarLogId()
		{
			this.Uri = new UriHandler();
		}

		public CalendarLogId(string path)
		{
			this.Uri = new UriHandler(path);
		}

		internal CalendarLogId(Item item, string user)
		{
			if (item == null)
			{
				throw new ArgumentNullException("item");
			}
			byte[] property = item.GetProperty(CalendarItemBaseSchema.CleanGlobalObjectId);
			if (property != null && property.Length > 0)
			{
				this.CleanGlobalObjectId = property.To64BitString();
			}
			if (item.StoreObjectId != null)
			{
				this.StoreObjectId = item.StoreObjectId.ToBase64String();
			}
			if (item.Session != null)
			{
				this.Uri = new UriHandler(user, string.IsNullOrEmpty(this.StoreObjectId) ? this.CleanGlobalObjectId : this.StoreObjectId);
				return;
			}
			this.Uri = new UriHandler(user, string.IsNullOrEmpty(this.StoreObjectId) ? this.CleanGlobalObjectId : this.StoreObjectId);
		}

		public Uri Uri { get; private set; }

		public string User
		{
			get
			{
				return this.UriHandler.UserName;
			}
		}

		public string StoreObjectId { get; private set; }

		public string CleanGlobalObjectId { get; private set; }

		public override byte[] GetBytes()
		{
			return Encoding.Unicode.GetBytes(this.Uri.ToString());
		}

		private UriHandler UriHandler
		{
			get
			{
				return new UriHandler(this.Uri);
			}
		}

		public override string ToString()
		{
			return this.Uri.ToString();
		}
	}
}
