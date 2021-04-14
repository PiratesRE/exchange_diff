using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "UserMailboxType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class UserMailbox
	{
		public UserMailbox()
		{
		}

		internal UserMailbox(string id, bool isArchive)
		{
			this.id = id;
			this.isArchive = isArchive;
		}

		public override bool Equals(object obj)
		{
			UserMailbox userMailbox = obj as UserMailbox;
			return userMailbox != null && string.Compare(this.id, userMailbox.Id, StringComparison.OrdinalIgnoreCase) == 0 && this.isArchive == userMailbox.IsArchive;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		[XmlAttribute("Id")]
		public string Id
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

		[XmlAttribute("IsArchive")]
		public bool IsArchive
		{
			get
			{
				return this.isArchive;
			}
			set
			{
				this.isArchive = value;
			}
		}

		private string id;

		private bool isArchive;
	}
}
