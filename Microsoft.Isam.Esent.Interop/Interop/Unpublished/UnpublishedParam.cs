using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public static class UnpublishedParam
	{
		public const JET_param AssertAction = (JET_param)36;

		public const JET_param AccessDeniedRetryPeriod = (JET_param)53;

		public const JET_param ExceptionAction = JET_param.ExceptionAction;

		public const JET_param MaxCoalesceReadSize = (JET_param)164;

		public const JET_param MaxCoalesceWriteSize = (JET_param)165;

		public const JET_param MaxCoalesceReadGapSize = (JET_param)166;

		public const JET_param MaxCoalesceWriteGapSize = (JET_param)167;

		public const JET_param WaypointLatency = (JET_param)153;

		public const JET_param CheckpointTooDeep = (JET_param)154;

		public const JET_param AggressiveLogRollover = (JET_param)157;

		public const JET_param EnableHaPublish = (JET_param)168;

		public const JET_param EmitLogDataCallback = (JET_param)173;

		public const JET_param EmitLogDataCallbackCtx = (JET_param)174;

		public const JET_param EnableExternalAutoHealing = (JET_param)175;

		public const JET_param PatchRequestTimeout = (JET_param)176;

		public const JET_param AutomaticShrinkDatabaseFreeSpaceThreshold = (JET_param)185;

		public const JET_param ConfigStoreSpec = (JET_param)189;

		public const JET_param StageFlighting = (JET_param)190;

		public const JET_param ZeroDatabaseUnusedSpace = (JET_param)191;
	}
}
