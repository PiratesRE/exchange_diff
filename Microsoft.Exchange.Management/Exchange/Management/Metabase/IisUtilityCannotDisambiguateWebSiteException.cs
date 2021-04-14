using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IisUtilityCannotDisambiguateWebSiteException : LocalizedException
	{
		public IisUtilityCannotDisambiguateWebSiteException(string webSiteName, string path1, string path2) : base(Strings.IisUtilityCannotDisambiguateWebSiteException(webSiteName, path1, path2))
		{
			this.webSiteName = webSiteName;
			this.path1 = path1;
			this.path2 = path2;
		}

		public IisUtilityCannotDisambiguateWebSiteException(string webSiteName, string path1, string path2, Exception innerException) : base(Strings.IisUtilityCannotDisambiguateWebSiteException(webSiteName, path1, path2), innerException)
		{
			this.webSiteName = webSiteName;
			this.path1 = path1;
			this.path2 = path2;
		}

		protected IisUtilityCannotDisambiguateWebSiteException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.webSiteName = (string)info.GetValue("webSiteName", typeof(string));
			this.path1 = (string)info.GetValue("path1", typeof(string));
			this.path2 = (string)info.GetValue("path2", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("webSiteName", this.webSiteName);
			info.AddValue("path1", this.path1);
			info.AddValue("path2", this.path2);
		}

		public string WebSiteName
		{
			get
			{
				return this.webSiteName;
			}
		}

		public string Path1
		{
			get
			{
				return this.path1;
			}
		}

		public string Path2
		{
			get
			{
				return this.path2;
			}
		}

		private readonly string webSiteName;

		private readonly string path1;

		private readonly string path2;
	}
}
