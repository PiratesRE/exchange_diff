using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract(Name = "{0}Results")]
	[KnownType(typeof(ValidatorInfo[]))]
	public class PowerShellResults<O> : PowerShellResults, IEnumerable<O>, IEnumerable
	{
		public bool SucceededWithValue
		{
			get
			{
				return base.Succeeded && this.HasValue;
			}
		}

		public bool HasValue
		{
			get
			{
				return this.Output != null && this.Output.Length == 1;
			}
		}

		public O Value
		{
			get
			{
				if (this.HasValue)
				{
					return this.Output[0];
				}
				throw new InvalidOperationException();
			}
		}

		public AsyncGetListContext AsyncGetListContext { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string[] SortColumnNames { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int[][] SortData { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string[] SortDataRawId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int StartIndex { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public int EndIndex { get; set; }

		[DataMember]
		public O[] Output { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string[] ReadOnlyProperties { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string[] NoAccessProperties { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public JsonDictionary<ValidatorInfo[]> Validators { get; set; }

		public PowerShellResults()
		{
			this.Output = Array<O>.Empty;
		}

		public PowerShellResults<O> MergeAll(PowerShellResults<O> results)
		{
			this.Output = ((this.Output == null) ? results.Output : this.Output.Concat(results.Output).ToArray<O>());
			this.MergeProgressData<O>(results);
			return base.MergeErrors<O>(results);
		}

		public PowerShellResults<O> MergeProgressData<T>(PowerShellResults<T> results)
		{
			if (results != null)
			{
				this.StartIndex = results.StartIndex;
				this.EndIndex = results.EndIndex;
				this.AsyncGetListContext = results.AsyncGetListContext;
				this.SortColumnNames = results.SortColumnNames;
				this.SortData = results.SortData;
			}
			return this;
		}

		public PowerShellResults<O> MergeOutput(O[] output)
		{
			this.Output = ((this.Output == null) ? output : this.Output.Concat(output).ToArray<O>());
			return this;
		}

		public override void UseAsRbacScopeInCurrentHttpContext()
		{
			if (this.HasValue)
			{
				BaseRow baseRow = this.Value as BaseRow;
				IConfigurable legacyTargetObject = (baseRow != null) ? baseRow.ConfigurationObject : null;
				RbacQuery.LegacyTargetObject = legacyTargetObject;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public IEnumerator<O> GetEnumerator()
		{
			return ((IEnumerable<!0>)(this.Output ?? Array<O>.Empty)).GetEnumerator();
		}
	}
}
