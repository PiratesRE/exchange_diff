using System;
using Microsoft.Exchange.Data.DocumentLibrary;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class DocumentLibrary : IComparable
	{
		public string DisplayName
		{
			get
			{
				return this.displayName;
			}
			set
			{
				this.displayName = value;
			}
		}

		public string SiteName
		{
			get
			{
				return this.siteName;
			}
			set
			{
				this.siteName = value;
			}
		}

		public string Uri
		{
			get
			{
				return this.uri;
			}
			set
			{
				this.uri = value;
			}
		}

		public UriFlags Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		public int CompareTo(object value)
		{
			DocumentLibrary documentLibrary = value as DocumentLibrary;
			if (documentLibrary == null)
			{
				throw new ArgumentException("object is not a DocumentLibrary");
			}
			if (this.displayName != null && documentLibrary.DisplayName != null)
			{
				return this.displayName.CompareTo(documentLibrary.DisplayName);
			}
			if (this.displayName == null && documentLibrary.DisplayName != null)
			{
				return -1;
			}
			if (this.displayName != null && documentLibrary.DisplayName == null)
			{
				return 1;
			}
			return 0;
		}

		private string displayName;

		private string siteName;

		private string uri;

		private UriFlags type = UriFlags.DocumentLibrary;
	}
}
