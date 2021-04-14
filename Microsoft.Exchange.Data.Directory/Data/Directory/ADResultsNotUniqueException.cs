using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ADResultsNotUniqueException : ADOperationException
	{
		public ADResultsNotUniqueException(string filter) : base(DirectoryStrings.ErrorResultsAreNonUnique(filter))
		{
			this.filter = filter;
		}

		public ADResultsNotUniqueException(string filter, Exception innerException) : base(DirectoryStrings.ErrorResultsAreNonUnique(filter), innerException)
		{
			this.filter = filter;
		}

		protected ADResultsNotUniqueException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.filter = (string)info.GetValue("filter", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("filter", this.filter);
		}

		public string Filter
		{
			get
			{
				return this.filter;
			}
		}

		private readonly string filter;
	}
}
