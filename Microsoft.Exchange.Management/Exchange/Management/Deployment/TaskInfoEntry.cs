using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class TaskInfoEntry
	{
		public TaskInfoEntry()
		{
			this.usePrimaryTask = false;
		}

		internal bool UsePrimaryTask
		{
			get
			{
				return this.usePrimaryTask;
			}
			set
			{
				this.usePrimaryTask = value;
			}
		}

		[XmlText]
		public string Task
		{
			get
			{
				if (this.task != null)
				{
					return this.task;
				}
				return string.Empty;
			}
			set
			{
				this.task = value;
			}
		}

		private bool usePrimaryTask;

		private string task;
	}
}
