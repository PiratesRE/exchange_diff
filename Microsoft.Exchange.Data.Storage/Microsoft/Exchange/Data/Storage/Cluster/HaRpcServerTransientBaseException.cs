using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage.Cluster
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public abstract class HaRpcServerTransientBaseException : TransientException, IHaRpcServerBaseException, IHaRpcServerBaseExceptionInternal
	{
		public HaRpcServerTransientBaseException(LocalizedString message) : base(message)
		{
			this.Initialize();
		}

		public HaRpcServerTransientBaseException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
			this.Initialize();
		}

		protected HaRpcServerTransientBaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.m_exceptionInfo = (HaRpcExceptionInfo)info.GetValue("exceptionInfo", typeof(HaRpcExceptionInfo));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("exceptionInfo", this.m_exceptionInfo, typeof(HaRpcExceptionInfo));
		}

		public abstract string ErrorMessage { get; }

		public string OriginatingServer
		{
			get
			{
				return this.m_exceptionInfo.OriginatingServer;
			}
		}

		public string OriginatingStackTrace
		{
			get
			{
				return this.m_exceptionInfo.OriginatingStackTrace;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.m_exceptionInfo.DatabaseName;
			}
		}

		string IHaRpcServerBaseExceptionInternal.MessageInternal
		{
			get
			{
				return base.Message;
			}
		}

		string IHaRpcServerBaseExceptionInternal.OriginatingServer
		{
			set
			{
				this.m_exceptionInfo.OriginatingServer = value;
			}
		}

		string IHaRpcServerBaseExceptionInternal.OriginatingStackTrace
		{
			set
			{
				this.m_exceptionInfo.OriginatingStackTrace = value;
			}
		}

		string IHaRpcServerBaseExceptionInternal.DatabaseName
		{
			set
			{
				this.m_exceptionInfo.DatabaseName = value;
			}
		}

		public override string Message
		{
			get
			{
				this.UpdateMessage();
				return this.m_message;
			}
		}

		public override string ToString()
		{
			this.UpdateFullString();
			return this.m_fullString;
		}

		private void UpdateFullString()
		{
			if (string.IsNullOrEmpty(this.OriginatingStackTrace) && string.IsNullOrEmpty(this.OriginatingServer))
			{
				this.m_fullString = base.ToString();
				return;
			}
			this.m_fullString = HaRpcExceptionHelper.GetFullString(this, this);
		}

		private void UpdateMessage()
		{
			this.m_message = string.Format("{0}{1}", base.Message, HaRpcExceptionHelper.GetOriginatingServerString(this.OriginatingServer, this.DatabaseName));
		}

		private void Initialize()
		{
			this.m_exceptionInfo = new HaRpcExceptionInfo();
		}

		private string m_message;

		private string m_fullString;

		protected HaRpcExceptionInfo m_exceptionInfo;
	}
}
