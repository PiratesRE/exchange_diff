using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class XmlDeserializationException : LocalizedException
	{
		public XmlDeserializationException(string filename, string error, string addlInfo) : base(Strings.XmlDeserializationException(filename, error, addlInfo))
		{
			this.filename = filename;
			this.error = error;
			this.addlInfo = addlInfo;
		}

		public XmlDeserializationException(string filename, string error, string addlInfo, Exception innerException) : base(Strings.XmlDeserializationException(filename, error, addlInfo), innerException)
		{
			this.filename = filename;
			this.error = error;
			this.addlInfo = addlInfo;
		}

		protected XmlDeserializationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.filename = (string)info.GetValue("filename", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
			this.addlInfo = (string)info.GetValue("addlInfo", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("filename", this.filename);
			info.AddValue("error", this.error);
			info.AddValue("addlInfo", this.addlInfo);
		}

		public string Filename
		{
			get
			{
				return this.filename;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		public string AddlInfo
		{
			get
			{
				return this.addlInfo;
			}
		}

		private readonly string filename;

		private readonly string error;

		private readonly string addlInfo;
	}
}
