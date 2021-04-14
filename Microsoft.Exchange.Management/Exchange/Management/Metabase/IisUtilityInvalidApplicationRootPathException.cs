using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IisUtilityInvalidApplicationRootPathException : LocalizedException
	{
		public IisUtilityInvalidApplicationRootPathException(string applicationRootPath) : base(Strings.IisUtilityInvalidApplicationRootPathException(applicationRootPath))
		{
			this.applicationRootPath = applicationRootPath;
		}

		public IisUtilityInvalidApplicationRootPathException(string applicationRootPath, Exception innerException) : base(Strings.IisUtilityInvalidApplicationRootPathException(applicationRootPath), innerException)
		{
			this.applicationRootPath = applicationRootPath;
		}

		protected IisUtilityInvalidApplicationRootPathException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.applicationRootPath = (string)info.GetValue("applicationRootPath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("applicationRootPath", this.applicationRootPath);
		}

		public string ApplicationRootPath
		{
			get
			{
				return this.applicationRootPath;
			}
		}

		private readonly string applicationRootPath;
	}
}
