using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public sealed class DatabaseMoveResult : IConfigurable
	{
		internal Guid Guid { get; set; }

		public ObjectId Identity { get; internal set; }

		public string ActiveServerAtStart { get; internal set; }

		public string ActiveServerAtEnd { get; internal set; }

		public MoveStatus Status { get; internal set; }

		public string ErrorMessage { get; internal set; }

		public Exception Exception { get; internal set; }

		public MountStatus MountStatusAtMoveStart { get; internal set; }

		public MountStatus MountStatusAtMoveEnd { get; internal set; }

		public long? NumberOfLogsLost { get; internal set; }

		public DateTime? RecoveryPointObjective { get; internal set; }

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
	}
}
