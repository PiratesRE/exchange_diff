using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RelocationInProgressException : ADOperationException
	{
		public RelocationInProgressException(string tenantName, string permError, string suspened, string autoCompletion, string currentState, string requestedState) : base(DirectoryStrings.RelocationInProgress(tenantName, permError, suspened, autoCompletion, currentState, requestedState))
		{
			this.tenantName = tenantName;
			this.permError = permError;
			this.suspened = suspened;
			this.autoCompletion = autoCompletion;
			this.currentState = currentState;
			this.requestedState = requestedState;
		}

		public RelocationInProgressException(string tenantName, string permError, string suspened, string autoCompletion, string currentState, string requestedState, Exception innerException) : base(DirectoryStrings.RelocationInProgress(tenantName, permError, suspened, autoCompletion, currentState, requestedState), innerException)
		{
			this.tenantName = tenantName;
			this.permError = permError;
			this.suspened = suspened;
			this.autoCompletion = autoCompletion;
			this.currentState = currentState;
			this.requestedState = requestedState;
		}

		protected RelocationInProgressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.tenantName = (string)info.GetValue("tenantName", typeof(string));
			this.permError = (string)info.GetValue("permError", typeof(string));
			this.suspened = (string)info.GetValue("suspened", typeof(string));
			this.autoCompletion = (string)info.GetValue("autoCompletion", typeof(string));
			this.currentState = (string)info.GetValue("currentState", typeof(string));
			this.requestedState = (string)info.GetValue("requestedState", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("tenantName", this.tenantName);
			info.AddValue("permError", this.permError);
			info.AddValue("suspened", this.suspened);
			info.AddValue("autoCompletion", this.autoCompletion);
			info.AddValue("currentState", this.currentState);
			info.AddValue("requestedState", this.requestedState);
		}

		public string TenantName
		{
			get
			{
				return this.tenantName;
			}
		}

		public string PermError
		{
			get
			{
				return this.permError;
			}
		}

		public string Suspened
		{
			get
			{
				return this.suspened;
			}
		}

		public string AutoCompletion
		{
			get
			{
				return this.autoCompletion;
			}
		}

		public string CurrentState
		{
			get
			{
				return this.currentState;
			}
		}

		public string RequestedState
		{
			get
			{
				return this.requestedState;
			}
		}

		private readonly string tenantName;

		private readonly string permError;

		private readonly string suspened;

		private readonly string autoCompletion;

		private readonly string currentState;

		private readonly string requestedState;
	}
}
