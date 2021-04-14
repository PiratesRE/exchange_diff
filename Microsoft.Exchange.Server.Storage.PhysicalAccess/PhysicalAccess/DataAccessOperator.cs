using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.Common.ExtensionMethods;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public abstract class DataAccessOperator : DisposableBase, IOperationExecutionTrackable
	{
		protected DataAccessOperator(IConnectionProvider connectionProvider, DataAccessOperator.DataAccessOperatorDefinition operatorDefinition)
		{
			this.connectionProvider = connectionProvider;
			this.operatorDefinition = operatorDefinition;
		}

		public CultureInfo Culture
		{
			get
			{
				return this.OperatorDefinitionBase.Culture;
			}
		}

		public CompareInfo CompareInfo
		{
			get
			{
				return this.OperatorDefinitionBase.CompareInfo;
			}
		}

		public IConnectionProvider ConnectionProvider
		{
			get
			{
				return this.connectionProvider;
			}
		}

		public Connection Connection
		{
			get
			{
				return this.connectionProvider.GetConnection();
			}
		}

		protected bool FrequentOperation
		{
			get
			{
				return this.OperatorDefinitionBase.FrequentOperation;
			}
		}

		internal string OperatorName
		{
			get
			{
				return this.OperatorDefinitionBase.OperatorName;
			}
		}

		public virtual bool Interrupted
		{
			get
			{
				return false;
			}
		}

		protected DataAccessOperator.DataAccessOperatorDefinition OperatorDefinitionBase
		{
			get
			{
				return this.operatorDefinition;
			}
		}

		public virtual bool EnableInterrupts(IInterruptControl interruptControl)
		{
			return false;
		}

		public abstract void EnumerateDescendants(Action<DataAccessOperator> operatorAction);

		public void GetDescendants(List<DataAccessOperator> planOperators)
		{
			this.EnumerateDescendants(new Action<DataAccessOperator>(planOperators.Add));
		}

		public virtual void RemoveChildren()
		{
		}

		public abstract object ExecuteScalar();

		internal static IExecutionPlanner GetExecutionPlannerOrNull(SimpleQueryOperator sqo)
		{
			if (sqo != null)
			{
				return sqo.GetExecutionPlanner();
			}
			return null;
		}

		protected void TraceOperation(string operation)
		{
			if (this.summaryTracingEnabled)
			{
				this.DoTraceOperation(operation, null);
			}
		}

		protected void TraceOperation(string operation, string extraData)
		{
			if (this.summaryTracingEnabled)
			{
				this.DoTraceOperation(operation, extraData);
			}
		}

		private void DoTraceOperation(string operation, string extraData)
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			this.AppendOperationInfo(operation, stringBuilder);
			if (DataAccessOperator.MultiLineTracing)
			{
				stringBuilder.Append('\n');
				stringBuilder.Append('\t', 1);
			}
			else
			{
				stringBuilder.Append(" ");
			}
			StringFormatOptions stringFormatOptions = StringFormatOptions.None;
			if (this.detailTracingEnabled)
			{
				stringFormatOptions |= StringFormatOptions.IncludeDetails;
				stringFormatOptions |= StringFormatOptions.IncludeNestedObjectsId;
			}
			if (DataAccessOperator.MultiLineTracing)
			{
				stringFormatOptions |= StringFormatOptions.MultiLine;
			}
			this.OperatorDefinitionBase.AppendToStringBuilder(stringBuilder, stringFormatOptions, 1);
			if (extraData != null)
			{
				if (DataAccessOperator.MultiLineTracing)
				{
					stringBuilder.Append('\n');
					stringBuilder.Append('\t', 1);
				}
				else
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(extraData);
			}
			ExTraceGlobals.DbInteractionSummaryTracer.TraceDebug(0L, stringBuilder.ToString());
		}

		protected void TraceOperationResult(string operation, Column resultColumn, object result)
		{
			if (this.summaryTracingEnabled)
			{
				this.DoTraceOperationResult(operation, resultColumn, result);
			}
		}

		private void DoTraceOperationResult(string operation, Column resultColumn, object result)
		{
			if (this.summaryTracingEnabled)
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				this.AppendOperationInfo(operation, stringBuilder);
				stringBuilder.Append("  result:[");
				if (resultColumn != null)
				{
					stringBuilder.Append(resultColumn.Name);
					stringBuilder.Append("=");
				}
				stringBuilder.AppendAsString(result);
				stringBuilder.Append("]");
				ExTraceGlobals.DbInteractionSummaryTracer.TraceDebug(0L, stringBuilder.ToString());
			}
		}

		protected void TraceCrumb(string operation)
		{
			if (this.detailTracingEnabled)
			{
				this.DoTraceSimpleOperation(operation, null, ExTraceGlobals.DbInteractionDetailTracer);
			}
		}

		protected void TraceCrumb(string operation, object crumb)
		{
			if (this.detailTracingEnabled)
			{
				this.DoTraceSimpleOperation(operation, crumb.ToString(), ExTraceGlobals.DbInteractionDetailTracer);
			}
		}

		protected void TraceMove(string operation, bool rowFound)
		{
			if (this.detailTracingEnabled)
			{
				this.DoTraceSimpleOperation(operation, rowFound ? "row found" : "no more rows", ExTraceGlobals.DbInteractionDetailTracer);
			}
		}

		private void DoTraceSimpleOperation(string operation, string extraData, Microsoft.Exchange.Diagnostics.Trace trace)
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			this.AppendOperationInfo(operation, stringBuilder);
			if (extraData != null)
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(extraData);
			}
			trace.TraceDebug(0L, stringBuilder.ToString());
		}

		protected void AppendOperationInfo(string operation, StringBuilder sb)
		{
			if (this.detailTracingEnabled)
			{
				sb.Append("cn:[");
				sb.Append(this.ConnectionProvider.GetHashCode());
				sb.Append("] ");
			}
			sb.Append(operation);
			if (this.detailTracingEnabled)
			{
				sb.Append(" op:[");
				sb.Append(this.OperatorName);
				sb.Append(" ");
				sb.Append(this.GetHashCode());
				sb.Append("]");
			}
		}

		public override string ToString()
		{
			return this.ToString(StringFormatOptions.IncludeDetails);
		}

		public string ToString(StringFormatOptions formatOptions)
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			this.AppendToStringBuilder(stringBuilder, formatOptions, 0);
			return stringBuilder.ToString();
		}

		internal void AppendToStringBuilder(StringBuilder sb, StringFormatOptions formatOptions, int nestingLevel)
		{
			this.OperatorDefinitionBase.AppendToStringBuilder(sb, formatOptions, nestingLevel);
		}

		[Conditional("DEBUG")]
		protected void ValidateCulture()
		{
			List<DataAccessOperator> list = new List<DataAccessOperator>(10);
			this.GetDescendants(list);
			bool flag = true;
			foreach (DataAccessOperator dataAccessOperator in list)
			{
				if (flag)
				{
					CultureInfo culture = dataAccessOperator.Culture;
					flag = false;
				}
			}
		}

		public IOperationExecutionTrackingKey GetTrackingKey()
		{
			return this.OperatorDefinitionBase;
		}

		public virtual IExecutionPlanner GetExecutionPlanner()
		{
			return this.OperatorDefinitionBase.ExecutionPlanner;
		}

		internal static bool MultiLineTracing;

		protected bool summaryTracingEnabled = ExTraceGlobals.DbInteractionSummaryTracer.IsTraceEnabled(TraceType.DebugTrace);

		protected bool intermediateTracingEnabled = ExTraceGlobals.DbInteractionIntermediateTracer.IsTraceEnabled(TraceType.DebugTrace);

		protected bool detailTracingEnabled = ExTraceGlobals.DbInteractionDetailTracer.IsTraceEnabled(TraceType.DebugTrace);

		private IConnectionProvider connectionProvider;

		private DataAccessOperator.DataAccessOperatorDefinition operatorDefinition;

		public abstract class DataAccessOperatorDefinition : IOperationExecutionTrackingKey
		{
			internal abstract string OperatorName { get; }

			public CultureInfo Culture
			{
				get
				{
					return this.culture;
				}
			}

			public CompareInfo CompareInfo
			{
				get
				{
					return this.compareInfo;
				}
			}

			public bool FrequentOperation
			{
				get
				{
					return this.frequentOperation;
				}
			}

			public IExecutionPlanner ExecutionPlanner
			{
				get
				{
					return this.planner;
				}
			}

			protected DataAccessOperatorDefinition(CultureInfo culture, bool frequentOperation)
			{
				this.culture = culture;
				if (culture != null)
				{
					this.compareInfo = culture.CompareInfo;
				}
				this.frequentOperation = frequentOperation;
			}

			int IOperationExecutionTrackingKey.GetTrackingKeyHashValue()
			{
				if (this.trackingKeyHashValue == 0)
				{
					this.CalculateHashValueForStatisticPurposes(out this.simpleHashValue, out this.trackingKeyHashValue);
				}
				return this.trackingKeyHashValue;
			}

			int IOperationExecutionTrackingKey.GetSimpleHashValue()
			{
				if (this.trackingKeyHashValue == 0)
				{
					this.CalculateHashValueForStatisticPurposes(out this.simpleHashValue, out this.trackingKeyHashValue);
				}
				return this.simpleHashValue;
			}

			bool IOperationExecutionTrackingKey.IsTrackingKeyEqualTo(IOperationExecutionTrackingKey other)
			{
				return this.IsEqualsForStatisticPurposes((DataAccessOperator.DataAccessOperatorDefinition)other);
			}

			string IOperationExecutionTrackingKey.TrackingKeyToString()
			{
				StringBuilder stringBuilder = new StringBuilder(500);
				this.AppendToStringBuilder(stringBuilder, StringFormatOptions.SkipParametersData, 0);
				return stringBuilder.ToString();
			}

			public abstract void EnumerateDescendants(Action<DataAccessOperator.DataAccessOperatorDefinition> operatorDefinitionAction);

			public void AttachPlanner(IExecutionPlanner planner)
			{
				this.planner = planner;
			}

			public override string ToString()
			{
				return this.ToString(StringFormatOptions.IncludeDetails);
			}

			public string ToString(StringFormatOptions formatOptions)
			{
				StringBuilder stringBuilder = new StringBuilder(100);
				this.AppendToStringBuilder(stringBuilder, formatOptions, 0);
				return stringBuilder.ToString();
			}

			internal static void AppendColumnsSummaryToStringBuilder(StringBuilder sb, IList<Column> columns, IList<object> values, StringFormatOptions formatOptions)
			{
				for (int i = 0; i < columns.Count; i++)
				{
					if (i != 0)
					{
						sb.Append(", ");
					}
					columns[i].AppendToString(sb, formatOptions);
					if (values != null && i < values.Count && (formatOptions & StringFormatOptions.SkipParametersData) == StringFormatOptions.None)
					{
						sb.Append("=[");
						if ((formatOptions & StringFormatOptions.IncludeDetails) == StringFormatOptions.IncludeDetails || !(values[i] is byte[]) || ((byte[])values[i]).Length <= 32)
						{
							sb.AppendAsString(values[i]);
						}
						else
						{
							sb.Append("<long blob>");
						}
						sb.Append("]");
					}
				}
			}

			internal static void CheckValueSizes(IList<Column> columns, IList<object> values)
			{
				for (int i = 0; i < columns.Count; i++)
				{
					if (columns[i].MaxLength != 0 && values[i] != null)
					{
						int num = Math.Max(columns[i].MaxLength, columns[i].Size);
						if (columns[i].ExtendedTypeCode == ExtendedTypeCode.Binary)
						{
							byte[] array = values[i] as byte[];
							int num2;
							if (array != null)
							{
								num2 = array.Length;
							}
							else if (values[i] is ArraySegment<byte>)
							{
								num2 = ((ArraySegment<byte>)values[i]).Count;
							}
							else
							{
								num2 = ((FunctionColumn)values[i]).MaxLength;
							}
							if (num < num2)
							{
								DiagnosticContext.TraceDwordAndString((LID)51360U, (uint)num, columns[i].Name);
								DiagnosticContext.TraceDword((LID)34976U, (uint)num2);
								throw new StoreException((LID)52704U, ErrorCodeValue.TooBig, string.Format("Value too big. Column {0}, Size {1}", columns[i].Name, num2));
							}
						}
						else if (columns[i].ExtendedTypeCode == ExtendedTypeCode.String)
						{
							int length = ((string)values[i]).Length;
							if (num < length)
							{
								DiagnosticContext.TraceDwordAndString((LID)59552U, (uint)num, columns[i].Name);
								DiagnosticContext.TraceDword((LID)45216U, (uint)length);
								throw new StoreException((LID)46560U, ErrorCodeValue.TooBig, string.Format("Value too big. Column {0}, Size {1}", columns[i].Name, length));
							}
						}
					}
				}
			}

			internal abstract void AppendToStringBuilder(StringBuilder sb, StringFormatOptions formatOptions, int nestingLevel);

			protected void Indent(StringBuilder sb, bool multiLine, int nestingLevel, bool additionalSpace)
			{
				if (multiLine)
				{
					sb.Append('\n');
					sb.Append('\t', nestingLevel);
					if (additionalSpace)
					{
						sb.Append("  ");
					}
				}
			}

			internal abstract void CalculateHashValueForStatisticPurposes(out int simple, out int detail);

			internal abstract bool IsEqualsForStatisticPurposes(DataAccessOperator.DataAccessOperatorDefinition other);

			private readonly bool frequentOperation;

			private CultureInfo culture;

			private CompareInfo compareInfo;

			private int trackingKeyHashValue;

			private int simpleHashValue;

			private IExecutionPlanner planner;
		}
	}
}
