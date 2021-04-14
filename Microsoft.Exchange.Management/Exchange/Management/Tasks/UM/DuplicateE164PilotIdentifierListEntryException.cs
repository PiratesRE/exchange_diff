using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DuplicateE164PilotIdentifierListEntryException : LocalizedException
	{
		public DuplicateE164PilotIdentifierListEntryException(string objectName) : base(Strings.DuplicateE164PilotIdentifierListEntry(objectName))
		{
			this.objectName = objectName;
		}

		public DuplicateE164PilotIdentifierListEntryException(string objectName, Exception innerException) : base(Strings.DuplicateE164PilotIdentifierListEntry(objectName), innerException)
		{
			this.objectName = objectName;
		}

		protected DuplicateE164PilotIdentifierListEntryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.objectName = (string)info.GetValue("objectName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("objectName", this.objectName);
		}

		public string ObjectName
		{
			get
			{
				return this.objectName;
			}
		}

		private readonly string objectName;
	}
}
