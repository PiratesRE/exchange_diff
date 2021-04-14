using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Directory.EventLog;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NonUniqueRecipientException : DataValidationException
	{
		public object AmbiguousData
		{
			get
			{
				return this.ambiguousData;
			}
		}

		public NonUniqueRecipientException(object ambiguousData, ValidationError error) : base(error)
		{
			this.ambiguousData = ambiguousData;
			string text = ambiguousData.ToString();
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_NON_UNIQUE_RECIPIENT, text, new object[]
			{
				text
			});
		}

		protected NonUniqueRecipientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		private object ambiguousData;
	}
}
