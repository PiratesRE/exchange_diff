using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Common.DiskManagement
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidCallWMIMethodArgumentsException : BitlockerUtilException
	{
		public InvalidCallWMIMethodArgumentsException(string[] inParamNameList, object inParamValueList, int inParamNameListLenght, int inParamValueListLenght) : base(DiskManagementStrings.InvalidCallWMIMethodArgumentsError(inParamNameList, inParamValueList, inParamNameListLenght, inParamValueListLenght))
		{
			this.inParamNameList = inParamNameList;
			this.inParamValueList = inParamValueList;
			this.inParamNameListLenght = inParamNameListLenght;
			this.inParamValueListLenght = inParamValueListLenght;
		}

		public InvalidCallWMIMethodArgumentsException(string[] inParamNameList, object inParamValueList, int inParamNameListLenght, int inParamValueListLenght, Exception innerException) : base(DiskManagementStrings.InvalidCallWMIMethodArgumentsError(inParamNameList, inParamValueList, inParamNameListLenght, inParamValueListLenght), innerException)
		{
			this.inParamNameList = inParamNameList;
			this.inParamValueList = inParamValueList;
			this.inParamNameListLenght = inParamNameListLenght;
			this.inParamValueListLenght = inParamValueListLenght;
		}

		protected InvalidCallWMIMethodArgumentsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.inParamNameList = (string[])info.GetValue("inParamNameList", typeof(string[]));
			this.inParamValueList = info.GetValue("inParamValueList", typeof(object));
			this.inParamNameListLenght = (int)info.GetValue("inParamNameListLenght", typeof(int));
			this.inParamValueListLenght = (int)info.GetValue("inParamValueListLenght", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("inParamNameList", this.inParamNameList);
			info.AddValue("inParamValueList", this.inParamValueList);
			info.AddValue("inParamNameListLenght", this.inParamNameListLenght);
			info.AddValue("inParamValueListLenght", this.inParamValueListLenght);
		}

		public string[] InParamNameList
		{
			get
			{
				return this.inParamNameList;
			}
		}

		public object InParamValueList
		{
			get
			{
				return this.inParamValueList;
			}
		}

		public int InParamNameListLenght
		{
			get
			{
				return this.inParamNameListLenght;
			}
		}

		public int InParamValueListLenght
		{
			get
			{
				return this.inParamValueListLenght;
			}
		}

		private readonly string[] inParamNameList;

		private readonly object inParamValueList;

		private readonly int inParamNameListLenght;

		private readonly int inParamValueListLenght;
	}
}
