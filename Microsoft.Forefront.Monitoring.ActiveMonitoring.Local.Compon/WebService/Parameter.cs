using System;
using System.Xml;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.WebService
{
	public class Parameter
	{
		internal string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		internal XmlNode Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
			}
		}

		internal bool IsNull
		{
			get
			{
				return this.isNull;
			}
			set
			{
				this.isNull = value;
			}
		}

		internal string UseResultFromOperationId
		{
			get
			{
				return this.useResultFromOperationId;
			}
			set
			{
				this.useResultFromOperationId = value;
			}
		}

		internal string PropertyName
		{
			get
			{
				return this.propertyName;
			}
			set
			{
				this.propertyName = value;
			}
		}

		internal string Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		internal int Index
		{
			get
			{
				return this.index;
			}
			set
			{
				this.index = value;
			}
		}

		internal bool UseFile
		{
			get
			{
				return this.useFile;
			}
			set
			{
				this.useFile = value;
			}
		}

		public override string ToString()
		{
			return string.Format("Name={0}, UseResultFromOperationId={1}, PropertyName={2}, Type={3}, Index={4}, IsNull={5}, Value={6}, UseFile={7}", new object[]
			{
				this.Name,
				this.UseResultFromOperationId,
				this.PropertyName,
				this.Type,
				this.Index,
				this.IsNull,
				(this.Value == null) ? string.Empty : this.Value.InnerXml,
				this.UseFile
			});
		}

		private string name;

		private XmlNode value;

		private bool isNull;

		private string useResultFromOperationId;

		private string propertyName;

		private string type;

		private int index;

		private bool useFile;
	}
}
