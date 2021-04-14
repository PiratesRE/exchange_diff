using System;
using System.Runtime.Serialization;
using System.Security;

namespace System.Diagnostics.Contracts
{
	[Serializable]
	internal sealed class ContractException : Exception
	{
		public ContractFailureKind Kind
		{
			get
			{
				return this._Kind;
			}
		}

		public string Failure
		{
			get
			{
				return this.Message;
			}
		}

		public string UserMessage
		{
			get
			{
				return this._UserMessage;
			}
		}

		public string Condition
		{
			get
			{
				return this._Condition;
			}
		}

		private ContractException()
		{
			base.HResult = -2146233022;
		}

		public ContractException(ContractFailureKind kind, string failure, string userMessage, string condition, Exception innerException) : base(failure, innerException)
		{
			base.HResult = -2146233022;
			this._Kind = kind;
			this._UserMessage = userMessage;
			this._Condition = condition;
		}

		private ContractException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this._Kind = (ContractFailureKind)info.GetInt32("Kind");
			this._UserMessage = info.GetString("UserMessage");
			this._Condition = info.GetString("Condition");
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Kind", this._Kind);
			info.AddValue("UserMessage", this._UserMessage);
			info.AddValue("Condition", this._Condition);
		}

		private readonly ContractFailureKind _Kind;

		private readonly string _UserMessage;

		private readonly string _Condition;
	}
}
