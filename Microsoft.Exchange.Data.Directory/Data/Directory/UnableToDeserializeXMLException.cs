using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToDeserializeXMLException : LocalizedException
	{
		public UnableToDeserializeXMLException(string errorStr) : base(DirectoryStrings.UnableToDeserializeXMLError(errorStr))
		{
			this.errorStr = errorStr;
		}

		public UnableToDeserializeXMLException(string errorStr, Exception innerException) : base(DirectoryStrings.UnableToDeserializeXMLError(errorStr), innerException)
		{
			this.errorStr = errorStr;
		}

		protected UnableToDeserializeXMLException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorStr = (string)info.GetValue("errorStr", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorStr", this.errorStr);
		}

		public string ErrorStr
		{
			get
			{
				return this.errorStr;
			}
		}

		private readonly string errorStr;
	}
}
