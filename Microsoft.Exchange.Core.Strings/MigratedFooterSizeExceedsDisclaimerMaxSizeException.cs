using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MigratedFooterSizeExceedsDisclaimerMaxSizeException : LocalizedException
	{
		public MigratedFooterSizeExceedsDisclaimerMaxSizeException(string domain, string disclaimer, int actualSize, int maxSize) : base(CoreStrings.MigratedFooterSizeExceedsDisclaimerMaxSize(domain, disclaimer, actualSize, maxSize))
		{
			this.domain = domain;
			this.disclaimer = disclaimer;
			this.actualSize = actualSize;
			this.maxSize = maxSize;
		}

		public MigratedFooterSizeExceedsDisclaimerMaxSizeException(string domain, string disclaimer, int actualSize, int maxSize, Exception innerException) : base(CoreStrings.MigratedFooterSizeExceedsDisclaimerMaxSize(domain, disclaimer, actualSize, maxSize), innerException)
		{
			this.domain = domain;
			this.disclaimer = disclaimer;
			this.actualSize = actualSize;
			this.maxSize = maxSize;
		}

		protected MigratedFooterSizeExceedsDisclaimerMaxSizeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.domain = (string)info.GetValue("domain", typeof(string));
			this.disclaimer = (string)info.GetValue("disclaimer", typeof(string));
			this.actualSize = (int)info.GetValue("actualSize", typeof(int));
			this.maxSize = (int)info.GetValue("maxSize", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("domain", this.domain);
			info.AddValue("disclaimer", this.disclaimer);
			info.AddValue("actualSize", this.actualSize);
			info.AddValue("maxSize", this.maxSize);
		}

		public string Domain
		{
			get
			{
				return this.domain;
			}
		}

		public string Disclaimer
		{
			get
			{
				return this.disclaimer;
			}
		}

		public int ActualSize
		{
			get
			{
				return this.actualSize;
			}
		}

		public int MaxSize
		{
			get
			{
				return this.maxSize;
			}
		}

		private readonly string domain;

		private readonly string disclaimer;

		private readonly int actualSize;

		private readonly int maxSize;
	}
}
