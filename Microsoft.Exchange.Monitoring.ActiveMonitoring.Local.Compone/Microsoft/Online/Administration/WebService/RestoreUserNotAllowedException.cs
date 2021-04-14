using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "RestoreUserNotAllowedException", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class RestoreUserNotAllowedException : DataOperationException
	{
		[DataMember]
		public bool SoftDeletedUserIsTooOld
		{
			get
			{
				return this.SoftDeletedUserIsTooOldField;
			}
			set
			{
				this.SoftDeletedUserIsTooOldField = value;
			}
		}

		private bool SoftDeletedUserIsTooOldField;
	}
}
