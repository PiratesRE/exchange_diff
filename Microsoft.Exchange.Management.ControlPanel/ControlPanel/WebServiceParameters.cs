using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.PowerShell.RbacHostingTools;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class WebServiceParameters : IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		protected WebServiceParameters()
		{
			this.InitializeDictionary();
		}

		public abstract string AssociatedCmdlet { get; }

		public abstract string RbacScope { get; }

		public bool IgnoreNullOrEmpty { get; set; }

		[DataMember]
		public virtual bool ShouldContinue
		{
			get
			{
				return this["Force"] != null;
			}
			set
			{
				if (value)
				{
					this.values["Force"] = new SwitchParameter(true);
				}
			}
		}

		[DataMember]
		public virtual bool SuppressConfirm { get; set; }

		public virtual string SuppressConfirmParameterName { get; set; }

		internal bool AllowExceuteThruHttpGetRequest { get; set; }

		internal bool CanSuppressConfirm
		{
			get
			{
				return !string.IsNullOrEmpty(this.SuppressConfirmParameterName) && this.CanSetParameter(this.SuppressConfirmParameterName);
			}
		}

		protected object this[PropertyDefinition cmdletParameterDefinition]
		{
			get
			{
				return this[cmdletParameterDefinition.Name];
			}
			set
			{
				this[cmdletParameterDefinition.Name] = value;
			}
		}

		protected object this[string cmdletParameterName]
		{
			get
			{
				object result = null;
				this.values.TryGetValue(cmdletParameterName, out result);
				return result;
			}
			set
			{
				StringBuilder stringBuilder = new StringBuilder(this.AssociatedCmdlet, 80);
				stringBuilder.Append("?");
				stringBuilder.Append(cmdletParameterName);
				stringBuilder.Append(this.RbacScope);
				string role = stringBuilder.ToString();
				new PrincipalPermission(null, role).Demand();
				this.values[cmdletParameterName] = value;
			}
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			if (this.IgnoreNullOrEmpty)
			{
				return (from entry in this.values
				where entry.Value != null && string.Empty != entry.Value as string
				select entry).GetEnumerator();
			}
			return this.values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		protected bool ParameterIsSpecified(PropertyDefinition cmdletParameterDefinition)
		{
			return this.ParameterIsSpecified(cmdletParameterDefinition.Name);
		}

		protected bool ParameterIsSpecified(string cmdletParameterName)
		{
			return this.values.ContainsKey(cmdletParameterName);
		}

		protected bool CanSetParameter(string cmdletParameterName)
		{
			string role = this.AssociatedCmdlet + "?" + cmdletParameterName;
			return RbacPrincipal.Current.IsInRole(role);
		}

		internal string[] GetRestrictedParameters(string[] parameters)
		{
			return (from x in parameters
			where !this.CanSetParameter(x)
			select x).ToArray<string>();
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			this.InitializeDictionary();
		}

		private void InitializeDictionary()
		{
			this.IgnoreNullOrEmpty = true;
			this.values = new Dictionary<string, object>();
		}

		private Dictionary<string, object> values;
	}
}
