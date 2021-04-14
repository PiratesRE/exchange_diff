using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public sealed class DagConfigurationEntry : IConfigurable
	{
		public ObjectId Identity { get; set; }

		public string Name { get; set; }

		public int ServersPerDag { get; set; }

		public int DatabasesPerServer { get; set; }

		public int DatabasesPerVolume { get; set; }

		public int CopiesPerDatabase { get; set; }

		public int MinCopiesPerDatabaseForMonitoring { get; set; }

		public ValidationError[] Validate()
		{
			return new ValidationError[0];
		}

		public void CopyChangesFrom(IConfigurable source)
		{
			throw new NotImplementedException();
		}

		public void ResetChangeTracking()
		{
			throw new NotImplementedException();
		}

		internal bool IsValid
		{
			get
			{
				return true;
			}
		}

		bool IConfigurable.IsValid
		{
			get
			{
				return this.IsValid;
			}
		}

		internal ObjectState ObjectState
		{
			get
			{
				return ObjectState.Unchanged;
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				return this.ObjectState;
			}
		}
	}
}
