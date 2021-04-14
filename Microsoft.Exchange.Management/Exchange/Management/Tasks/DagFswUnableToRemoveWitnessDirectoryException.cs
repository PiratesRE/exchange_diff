using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagFswUnableToRemoveWitnessDirectoryException : LocalizedException
	{
		public DagFswUnableToRemoveWitnessDirectoryException(string fswMachine, string directory, Exception ex) : base(Strings.DagFswUnableToRemoveWitnessDirectoryException(fswMachine, directory, ex))
		{
			this.fswMachine = fswMachine;
			this.directory = directory;
			this.ex = ex;
		}

		public DagFswUnableToRemoveWitnessDirectoryException(string fswMachine, string directory, Exception ex, Exception innerException) : base(Strings.DagFswUnableToRemoveWitnessDirectoryException(fswMachine, directory, ex), innerException)
		{
			this.fswMachine = fswMachine;
			this.directory = directory;
			this.ex = ex;
		}

		protected DagFswUnableToRemoveWitnessDirectoryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fswMachine = (string)info.GetValue("fswMachine", typeof(string));
			this.directory = (string)info.GetValue("directory", typeof(string));
			this.ex = (Exception)info.GetValue("ex", typeof(Exception));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fswMachine", this.fswMachine);
			info.AddValue("directory", this.directory);
			info.AddValue("ex", this.ex);
		}

		public string FswMachine
		{
			get
			{
				return this.fswMachine;
			}
		}

		public string Directory
		{
			get
			{
				return this.directory;
			}
		}

		public Exception Ex
		{
			get
			{
				return this.ex;
			}
		}

		private readonly string fswMachine;

		private readonly string directory;

		private readonly Exception ex;
	}
}
