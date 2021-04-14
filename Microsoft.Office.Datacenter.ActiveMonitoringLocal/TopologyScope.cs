using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq.Mapping;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	[Table]
	public class TopologyScope : TableEntity, IWorkData
	{
		public TopologyScope()
		{
			this.CreatedTime = DateTime.UtcNow;
			this.LastDataUpdateTime = new DateTime(1970, 1, 1);
		}

		[Column(DbType = "Int NOT NULL IDENTITY", IsPrimaryKey = true, IsDbGenerated = true)]
		public int Id { get; set; }

		[Range(0, 999, ErrorMessage = "{0} is a mandatory property, and must be between 0..999")]
		[Column]
		[Required(ErrorMessage = "{0} is a mandatory property")]
		public int AggregationLevel { get; set; }

		[Column]
		[Required(AllowEmptyStrings = false, ErrorMessage = "{0} is a mandatory property")]
		[RegularExpression("^[-_a-zA-Z0-9]{1,255}$", ErrorMessage = "Only alpha-numeric characters, dash, and underscore are allowed, and length must be between 1 and 255 characters.")]
		public string Name { get; set; }

		[RegularExpression("^[-_a-zA-Z0-9/]{3,1024}$", ErrorMessage = "Only alpha-numeric and front slash characters are allowed, and length must be between 3 and 255 characters.")]
		[Column]
		public string ParentScope { get; set; }

		[RegularExpression("^[a-zA-Z0-9]{1,255}$", ErrorMessage = "Only alpha-numeric characters are allowed, and length must be between 1 and 255 characters.")]
		[Column]
		[Required(AllowEmptyStrings = false, ErrorMessage = "{0} is a mandatory property")]
		public string ReplicationScopeKey { get; set; }

		[Column]
		public DateTime LastDataUpdateTime { get; set; }

		[Column]
		public DateTime CreatedTime { get; set; }

		public string InternalStorageKey
		{
			get
			{
				return string.Empty;
			}
		}

		public string ExternalStorageKey
		{
			get
			{
				return string.Empty;
			}
		}

		public string SecondaryExternalStorageKey
		{
			get
			{
				return string.Empty;
			}
		}
	}
}
