using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagFswAndAlternateFswOnSameWitnessServerButPointToDifferentDirectoriesException : LocalizedException
	{
		public DagFswAndAlternateFswOnSameWitnessServerButPointToDifferentDirectoriesException(string witnessserver, string witnessdirectory, string alternatewitnessdirectory) : base(Strings.DagFswAndAlternateFswOnSameWitnessServerButPointToDifferentDirectoriesException(witnessserver, witnessdirectory, alternatewitnessdirectory))
		{
			this.witnessserver = witnessserver;
			this.witnessdirectory = witnessdirectory;
			this.alternatewitnessdirectory = alternatewitnessdirectory;
		}

		public DagFswAndAlternateFswOnSameWitnessServerButPointToDifferentDirectoriesException(string witnessserver, string witnessdirectory, string alternatewitnessdirectory, Exception innerException) : base(Strings.DagFswAndAlternateFswOnSameWitnessServerButPointToDifferentDirectoriesException(witnessserver, witnessdirectory, alternatewitnessdirectory), innerException)
		{
			this.witnessserver = witnessserver;
			this.witnessdirectory = witnessdirectory;
			this.alternatewitnessdirectory = alternatewitnessdirectory;
		}

		protected DagFswAndAlternateFswOnSameWitnessServerButPointToDifferentDirectoriesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.witnessserver = (string)info.GetValue("witnessserver", typeof(string));
			this.witnessdirectory = (string)info.GetValue("witnessdirectory", typeof(string));
			this.alternatewitnessdirectory = (string)info.GetValue("alternatewitnessdirectory", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("witnessserver", this.witnessserver);
			info.AddValue("witnessdirectory", this.witnessdirectory);
			info.AddValue("alternatewitnessdirectory", this.alternatewitnessdirectory);
		}

		public string Witnessserver
		{
			get
			{
				return this.witnessserver;
			}
		}

		public string Witnessdirectory
		{
			get
			{
				return this.witnessdirectory;
			}
		}

		public string Alternatewitnessdirectory
		{
			get
			{
				return this.alternatewitnessdirectory;
			}
		}

		private readonly string witnessserver;

		private readonly string witnessdirectory;

		private readonly string alternatewitnessdirectory;
	}
}
