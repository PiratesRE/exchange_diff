using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.RightsManagementServices.Online;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Serializable]
	internal class RmsOnlineImportTpdException : ImportTpdException
	{
		public ServerErrorCode ServerErrorCode { get; set; }

		public RmsOnlineImportTpdException()
		{
		}

		protected RmsOnlineImportTpdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ServerErrorCode = (ServerErrorCode)info.GetInt32("ServerErrorCode");
		}

		public RmsOnlineImportTpdException(string message, ServerErrorCode serverErrorCode) : base(message, null)
		{
			this.ServerErrorCode = serverErrorCode;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("RMS Online error code: ");
			stringBuilder.Append(this.ServerErrorCode);
			stringBuilder.AppendLine();
			stringBuilder.Append(base.ToString());
			return stringBuilder.ToString();
		}

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			ArgumentValidator.ThrowIfNull("info", info);
			info.AddValue("ServerErrorCode", (int)this.ServerErrorCode);
			base.GetObjectData(info, context);
		}
	}
}
