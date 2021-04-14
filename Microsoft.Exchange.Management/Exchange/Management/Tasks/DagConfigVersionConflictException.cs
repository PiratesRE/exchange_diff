using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagConfigVersionConflictException : LocalizedException
	{
		public DagConfigVersionConflictException(string dagConfigName, int dagconfigversion, int xmlversion) : base(Strings.DagConfigVersionConflictException(dagConfigName, dagconfigversion, xmlversion))
		{
			this.dagConfigName = dagConfigName;
			this.dagconfigversion = dagconfigversion;
			this.xmlversion = xmlversion;
		}

		public DagConfigVersionConflictException(string dagConfigName, int dagconfigversion, int xmlversion, Exception innerException) : base(Strings.DagConfigVersionConflictException(dagConfigName, dagconfigversion, xmlversion), innerException)
		{
			this.dagConfigName = dagConfigName;
			this.dagconfigversion = dagconfigversion;
			this.xmlversion = xmlversion;
		}

		protected DagConfigVersionConflictException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dagConfigName = (string)info.GetValue("dagConfigName", typeof(string));
			this.dagconfigversion = (int)info.GetValue("dagconfigversion", typeof(int));
			this.xmlversion = (int)info.GetValue("xmlversion", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dagConfigName", this.dagConfigName);
			info.AddValue("dagconfigversion", this.dagconfigversion);
			info.AddValue("xmlversion", this.xmlversion);
		}

		public string DagConfigName
		{
			get
			{
				return this.dagConfigName;
			}
		}

		public int Dagconfigversion
		{
			get
			{
				return this.dagconfigversion;
			}
		}

		public int Xmlversion
		{
			get
			{
				return this.xmlversion;
			}
		}

		private readonly string dagConfigName;

		private readonly int dagconfigversion;

		private readonly int xmlversion;
	}
}
