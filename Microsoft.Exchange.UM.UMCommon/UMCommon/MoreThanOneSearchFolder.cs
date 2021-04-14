using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MoreThanOneSearchFolder : LocalizedException
	{
		public MoreThanOneSearchFolder(int searchFolderCount, string searchFolderName) : base(Strings.MoreThanOneSearchFolder(searchFolderCount, searchFolderName))
		{
			this.searchFolderCount = searchFolderCount;
			this.searchFolderName = searchFolderName;
		}

		public MoreThanOneSearchFolder(int searchFolderCount, string searchFolderName, Exception innerException) : base(Strings.MoreThanOneSearchFolder(searchFolderCount, searchFolderName), innerException)
		{
			this.searchFolderCount = searchFolderCount;
			this.searchFolderName = searchFolderName;
		}

		protected MoreThanOneSearchFolder(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.searchFolderCount = (int)info.GetValue("searchFolderCount", typeof(int));
			this.searchFolderName = (string)info.GetValue("searchFolderName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("searchFolderCount", this.searchFolderCount);
			info.AddValue("searchFolderName", this.searchFolderName);
		}

		public int SearchFolderCount
		{
			get
			{
				return this.searchFolderCount;
			}
		}

		public string SearchFolderName
		{
			get
			{
				return this.searchFolderName;
			}
		}

		private readonly int searchFolderCount;

		private readonly string searchFolderName;
	}
}
