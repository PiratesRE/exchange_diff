using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DuplicateExternalDirectoryObjectIdException : LocalizedException
	{
		public DuplicateExternalDirectoryObjectIdException(string objectName, string edoId) : base(Strings.DuplicateExternalDirectoryObjectIdException(objectName, edoId))
		{
			this.objectName = objectName;
			this.edoId = edoId;
		}

		public DuplicateExternalDirectoryObjectIdException(string objectName, string edoId, Exception innerException) : base(Strings.DuplicateExternalDirectoryObjectIdException(objectName, edoId), innerException)
		{
			this.objectName = objectName;
			this.edoId = edoId;
		}

		protected DuplicateExternalDirectoryObjectIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.objectName = (string)info.GetValue("objectName", typeof(string));
			this.edoId = (string)info.GetValue("edoId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("objectName", this.objectName);
			info.AddValue("edoId", this.edoId);
		}

		public string ObjectName
		{
			get
			{
				return this.objectName;
			}
		}

		public string EdoId
		{
			get
			{
				return this.edoId;
			}
		}

		private readonly string objectName;

		private readonly string edoId;
	}
}
