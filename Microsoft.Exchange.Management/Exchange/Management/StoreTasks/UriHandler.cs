using System;
using System.Web;

namespace Microsoft.Exchange.Management.StoreTasks
{
	internal class UriHandler : UriBuilder, IComparable, IEquatable<UriHandler>
	{
		public UriHandler() : this(string.Empty)
		{
		}

		public UriHandler(string uri) : this(new Uri(uri))
		{
		}

		public UriHandler(Uri uri) : base(uri)
		{
		}

		public UriHandler(Uri uri, string id) : base(uri)
		{
			this.Id = HttpUtility.UrlEncode(id);
		}

		public UriHandler(string user, string messageId)
		{
			base.Scheme = "excallog";
			base.Host = user;
			this.Id = messageId;
		}

		public bool IsValidLink
		{
			get
			{
				return base.Scheme == "excallog" || base.Scheme == "file";
			}
		}

		public bool IsMailboxLink
		{
			get
			{
				return base.Scheme == "excallog";
			}
		}

		public bool IsFileLink
		{
			get
			{
				return base.Scheme == "file";
			}
		}

		public string Id
		{
			get
			{
				if (!string.IsNullOrEmpty(base.Query))
				{
					return base.Query.Substring(string.Format("{0}id=", '?').Length);
				}
				return string.Empty;
			}
			set
			{
				base.Query = string.Format("id={0}", value);
			}
		}

		public static implicit operator Uri(UriHandler uh)
		{
			if (uh == null)
			{
				return null;
			}
			return uh.Uri;
		}

		protected virtual bool CheckEquality(UriHandler other)
		{
			return other != null && !(this.Id != other.Id) && !(base.Scheme != other.Scheme) && !(base.Host != other.Host) && !(base.UserName != other.UserName) && !(base.Path != other.Path);
		}

		public bool Equals(UriHandler other)
		{
			return this.CheckEquality(other);
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj", string.Empty);
			}
			return string.Compare(base.Uri.AbsoluteUri, ((UriHandler)obj).Uri.AbsoluteUri, StringComparison.Ordinal);
		}

		private const int ItemIdSegment = 1;

		public const char QueryDesignator = '?';

		internal abstract class Schemes
		{
			public const string Mailbox = "excallog";

			public const string File = "file";
		}
	}
}
