using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IncompleteSettingsException : LocalizedException
	{
		public IncompleteSettingsException() : base(Strings.ErrorIncompleteSettings)
		{
		}

		public IncompleteSettingsException(Exception innerException) : base(Strings.ErrorIncompleteSettings, innerException)
		{
		}

		protected IncompleteSettingsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
