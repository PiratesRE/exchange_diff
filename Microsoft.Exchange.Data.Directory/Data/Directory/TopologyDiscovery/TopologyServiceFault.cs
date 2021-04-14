using System;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.TopologyDiscovery
{
	[DataContract]
	[KnownType(typeof(TopologyServiceFault))]
	[Serializable]
	internal sealed class TopologyServiceFault : IExtensibleDataObject
	{
		[DataMember]
		public string Message
		{
			get
			{
				return this.message;
			}
			private set
			{
				this.message = value;
			}
		}

		[DataMember]
		public bool CanRetry
		{
			get
			{
				return this.fCanRetry;
			}
			private set
			{
				this.fCanRetry = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public string StackTrace
		{
			get
			{
				return this.stackTrace;
			}
			private set
			{
				this.stackTrace = value;
			}
		}

		[DataMember(EmitDefaultValue = false)]
		public TopologyServiceFault InnerException
		{
			get
			{
				return this.innerException;
			}
			private set
			{
				this.innerException = value;
			}
		}

		public ExtensionDataObject ExtensionData
		{
			get
			{
				return this.extensionData;
			}
			set
			{
				this.extensionData = value;
			}
		}

		public static TopologyServiceFault Create(Exception ex, bool includeExceptionDetails = false)
		{
			if (ex == null)
			{
				return null;
			}
			TopologyServiceFault topologyServiceFault = new TopologyServiceFault();
			if (ex is LocalizedException)
			{
				topologyServiceFault.Message = ((LocalizedException)ex).LocalizedString;
			}
			else
			{
				topologyServiceFault.Message = new LocalizedString(ex.Message);
			}
			topologyServiceFault.CanRetry = (ex is TransientException);
			topologyServiceFault.StackTrace = (includeExceptionDetails ? ex.StackTrace : string.Empty);
			topologyServiceFault.InnerException = (includeExceptionDetails ? TopologyServiceFault.Create(ex.InnerException, includeExceptionDetails) : null);
			return topologyServiceFault;
		}

		public override string ToString()
		{
			if (string.IsNullOrEmpty(this.StackTrace) && this.InnerException == null)
			{
				return this.Message;
			}
			return this.ToStringHelper(false);
		}

		private string ToStringHelper(bool isInner)
		{
			if (string.IsNullOrEmpty(this.Message))
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.Message);
			if (this.InnerException != null)
			{
				stringBuilder.AppendFormat(" ----> {0}", this.InnerException.ToStringHelper(true));
			}
			else
			{
				stringBuilder.Append(Environment.NewLine);
			}
			if (!string.IsNullOrEmpty(this.StackTrace))
			{
				stringBuilder.Append(this.StackTrace);
			}
			if (isInner)
			{
				stringBuilder.AppendFormat("{0}-----------{0}", Environment.NewLine);
			}
			return stringBuilder.ToString();
		}

		private string message;

		private bool fCanRetry;

		private string stackTrace;

		private TopologyServiceFault innerException;

		[NonSerialized]
		private ExtensionDataObject extensionData;
	}
}
