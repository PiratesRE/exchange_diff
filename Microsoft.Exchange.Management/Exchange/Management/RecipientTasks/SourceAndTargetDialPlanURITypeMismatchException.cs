using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SourceAndTargetDialPlanURITypeMismatchException : LocalizedException
	{
		public SourceAndTargetDialPlanURITypeMismatchException(string sourceUriType, string targetUriType) : base(Strings.SourceAndTargetDialPlanURITypeMismatch(sourceUriType, targetUriType))
		{
			this.sourceUriType = sourceUriType;
			this.targetUriType = targetUriType;
		}

		public SourceAndTargetDialPlanURITypeMismatchException(string sourceUriType, string targetUriType, Exception innerException) : base(Strings.SourceAndTargetDialPlanURITypeMismatch(sourceUriType, targetUriType), innerException)
		{
			this.sourceUriType = sourceUriType;
			this.targetUriType = targetUriType;
		}

		protected SourceAndTargetDialPlanURITypeMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.sourceUriType = (string)info.GetValue("sourceUriType", typeof(string));
			this.targetUriType = (string)info.GetValue("targetUriType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("sourceUriType", this.sourceUriType);
			info.AddValue("targetUriType", this.targetUriType);
		}

		public string SourceUriType
		{
			get
			{
				return this.sourceUriType;
			}
		}

		public string TargetUriType
		{
			get
			{
				return this.targetUriType;
			}
		}

		private readonly string sourceUriType;

		private readonly string targetUriType;
	}
}
