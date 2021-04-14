using System;

namespace Microsoft.Exchange.Data.Globalization
{
	internal static class FEData
	{
		// Note: this type is marked as 'beforefieldinit'.
		static FEData()
		{
			FEData.ST[,] array = new FEData.ST[2, 20];
			array[0, 3] = FEData.ST.ERR;
			array[0, 4] = FEData.ST.ST1;
			array[0, 5] = FEData.ST.ST1;
			array[0, 6] = FEData.ST.ST1;
			array[0, 7] = FEData.ST.ST1;
			array[0, 8] = FEData.ST.ERR;
			array[0, 12] = FEData.ST.ST1;
			array[0, 13] = FEData.ST.ST1;
			array[0, 14] = FEData.ST.ST1;
			array[0, 15] = FEData.ST.ST1;
			array[0, 16] = FEData.ST.ST1;
			array[0, 17] = FEData.ST.ERR;
			array[0, 18] = FEData.ST.ERR;
			array[1, 0] = FEData.ST.ERR;
			array[1, 1] = FEData.ST.ERR;
			array[1, 17] = FEData.ST.ERR;
			array[1, 18] = FEData.ST.ERR;
			array[1, 19] = FEData.ST.ERR;
			FEData.SJisNextState = array;
			FEData.ST[,] array2 = new FEData.ST[4, 20];
			array2[0, 3] = FEData.ST.ERR;
			array2[0, 4] = FEData.ST.ERR;
			array2[0, 5] = FEData.ST.ST2;
			array2[0, 6] = FEData.ST.ST3;
			array2[0, 7] = FEData.ST.ERR;
			array2[0, 8] = FEData.ST.ERR;
			array2[0, 9] = FEData.ST.ST1;
			array2[0, 10] = FEData.ST.ST1;
			array2[0, 11] = FEData.ST.ST1;
			array2[0, 12] = FEData.ST.ST1;
			array2[0, 13] = FEData.ST.ST1;
			array2[0, 14] = FEData.ST.ST1;
			array2[0, 15] = FEData.ST.ST1;
			array2[0, 16] = FEData.ST.ST1;
			array2[0, 17] = FEData.ST.ST1;
			array2[0, 18] = FEData.ST.ERR;
			array2[1, 0] = FEData.ST.ERR;
			array2[1, 1] = FEData.ST.ERR;
			array2[1, 2] = FEData.ST.ERR;
			array2[1, 3] = FEData.ST.ERR;
			array2[1, 4] = FEData.ST.ERR;
			array2[1, 5] = FEData.ST.ERR;
			array2[1, 6] = FEData.ST.ERR;
			array2[1, 7] = FEData.ST.ERR;
			array2[1, 8] = FEData.ST.ERR;
			array2[1, 18] = FEData.ST.ERR;
			array2[1, 19] = FEData.ST.ERR;
			array2[2, 0] = FEData.ST.ERR;
			array2[2, 1] = FEData.ST.ERR;
			array2[2, 2] = FEData.ST.ERR;
			array2[2, 3] = FEData.ST.ERR;
			array2[2, 4] = FEData.ST.ERR;
			array2[2, 5] = FEData.ST.ERR;
			array2[2, 6] = FEData.ST.ERR;
			array2[2, 7] = FEData.ST.ERR;
			array2[2, 8] = FEData.ST.ERR;
			array2[2, 12] = FEData.ST.ERR;
			array2[2, 13] = FEData.ST.ERR;
			array2[2, 14] = FEData.ST.ERR;
			array2[2, 15] = FEData.ST.ERR;
			array2[2, 16] = FEData.ST.ERR;
			array2[2, 17] = FEData.ST.ERR;
			array2[2, 18] = FEData.ST.ERR;
			array2[2, 19] = FEData.ST.ERR;
			array2[3, 0] = FEData.ST.ERR;
			array2[3, 1] = FEData.ST.ERR;
			array2[3, 2] = FEData.ST.ERR;
			array2[3, 3] = FEData.ST.ERR;
			array2[3, 4] = FEData.ST.ERR;
			array2[3, 5] = FEData.ST.ERR;
			array2[3, 6] = FEData.ST.ERR;
			array2[3, 7] = FEData.ST.ERR;
			array2[3, 8] = FEData.ST.ERR;
			array2[3, 9] = FEData.ST.ST1;
			array2[3, 10] = FEData.ST.ST1;
			array2[3, 11] = FEData.ST.ST1;
			array2[3, 12] = FEData.ST.ST1;
			array2[3, 13] = FEData.ST.ST1;
			array2[3, 14] = FEData.ST.ST1;
			array2[3, 15] = FEData.ST.ST1;
			array2[3, 16] = FEData.ST.ST1;
			array2[3, 17] = FEData.ST.ST1;
			array2[3, 18] = FEData.ST.ERR;
			array2[3, 19] = FEData.ST.ERR;
			FEData.EucJpNextState = array2;
			FEData.ST[,] array3 = new FEData.ST[2, 20];
			array3[0, 3] = FEData.ST.ERR;
			array3[0, 4] = FEData.ST.ST1;
			array3[0, 5] = FEData.ST.ST1;
			array3[0, 6] = FEData.ST.ST1;
			array3[0, 7] = FEData.ST.ST1;
			array3[0, 8] = FEData.ST.ST1;
			array3[0, 9] = FEData.ST.ST1;
			array3[0, 10] = FEData.ST.ST1;
			array3[0, 11] = FEData.ST.ST1;
			array3[0, 12] = FEData.ST.ST1;
			array3[0, 13] = FEData.ST.ST1;
			array3[0, 14] = FEData.ST.ST1;
			array3[0, 15] = FEData.ST.ST1;
			array3[0, 16] = FEData.ST.ST1;
			array3[0, 17] = FEData.ST.ST1;
			array3[0, 18] = FEData.ST.ERR;
			array3[1, 0] = FEData.ST.ERR;
			array3[1, 1] = FEData.ST.ERR;
			array3[1, 18] = FEData.ST.ERR;
			array3[1, 19] = FEData.ST.ERR;
			FEData.GbkWanNextState = array3;
			FEData.ST[,] array4 = new FEData.ST[2, 20];
			array4[0, 3] = FEData.ST.ERR;
			array4[0, 4] = FEData.ST.ERR;
			array4[0, 5] = FEData.ST.ERR;
			array4[0, 6] = FEData.ST.ERR;
			array4[0, 7] = FEData.ST.ERR;
			array4[0, 8] = FEData.ST.ERR;
			array4[0, 9] = FEData.ST.ST1;
			array4[0, 10] = FEData.ST.ST1;
			array4[0, 11] = FEData.ST.ST1;
			array4[0, 12] = FEData.ST.ST1;
			array4[0, 13] = FEData.ST.ST1;
			array4[0, 14] = FEData.ST.ST1;
			array4[0, 15] = FEData.ST.ST1;
			array4[0, 16] = FEData.ST.ST1;
			array4[0, 17] = FEData.ST.ST1;
			array4[0, 18] = FEData.ST.ERR;
			array4[1, 0] = FEData.ST.ERR;
			array4[1, 1] = FEData.ST.ERR;
			array4[1, 2] = FEData.ST.ERR;
			array4[1, 3] = FEData.ST.ERR;
			array4[1, 4] = FEData.ST.ERR;
			array4[1, 5] = FEData.ST.ERR;
			array4[1, 6] = FEData.ST.ERR;
			array4[1, 7] = FEData.ST.ERR;
			array4[1, 8] = FEData.ST.ERR;
			array4[1, 18] = FEData.ST.ERR;
			array4[1, 19] = FEData.ST.ERR;
			FEData.EucKrCnNextState = array4;
			FEData.ST[,] array5 = new FEData.ST[2, 20];
			array5[0, 3] = FEData.ST.ERR;
			array5[0, 4] = FEData.ST.ST1;
			array5[0, 5] = FEData.ST.ST1;
			array5[0, 6] = FEData.ST.ST1;
			array5[0, 7] = FEData.ST.ST1;
			array5[0, 8] = FEData.ST.ST1;
			array5[0, 9] = FEData.ST.ST1;
			array5[0, 10] = FEData.ST.ST1;
			array5[0, 11] = FEData.ST.ST1;
			array5[0, 12] = FEData.ST.ST1;
			array5[0, 13] = FEData.ST.ST1;
			array5[0, 14] = FEData.ST.ST1;
			array5[0, 15] = FEData.ST.ST1;
			array5[0, 16] = FEData.ST.ST1;
			array5[0, 17] = FEData.ST.ST1;
			array5[0, 18] = FEData.ST.ERR;
			array5[1, 0] = FEData.ST.ERR;
			array5[1, 1] = FEData.ST.ERR;
			array5[1, 3] = FEData.ST.ERR;
			array5[1, 4] = FEData.ST.ERR;
			array5[1, 5] = FEData.ST.ERR;
			array5[1, 6] = FEData.ST.ERR;
			array5[1, 7] = FEData.ST.ERR;
			array5[1, 8] = FEData.ST.ERR;
			array5[1, 18] = FEData.ST.ERR;
			array5[1, 19] = FEData.ST.ERR;
			FEData.Big5NextState = array5;
			FEData.ST[,] array6 = new FEData.ST[6, 20];
			array6[0, 3] = FEData.ST.ERR;
			array6[0, 4] = FEData.ST.ERR;
			array6[0, 5] = FEData.ST.ERR;
			array6[0, 6] = FEData.ST.ERR;
			array6[0, 7] = FEData.ST.ERR;
			array6[0, 8] = FEData.ST.ERR;
			array6[0, 9] = FEData.ST.ERR;
			array6[0, 10] = FEData.ST.ERR;
			array6[0, 11] = FEData.ST.ST1;
			array6[0, 12] = FEData.ST.ST4;
			array6[0, 13] = FEData.ST.ST2;
			array6[0, 14] = FEData.ST.ST5;
			array6[0, 15] = FEData.ST.ST3;
			array6[0, 16] = FEData.ST.ERR;
			array6[0, 17] = FEData.ST.ERR;
			array6[0, 18] = FEData.ST.ERR;
			array6[1, 0] = FEData.ST.ERR;
			array6[1, 1] = FEData.ST.ERR;
			array6[1, 2] = FEData.ST.ERR;
			array6[1, 10] = FEData.ST.ERR;
			array6[1, 11] = FEData.ST.ERR;
			array6[1, 12] = FEData.ST.ERR;
			array6[1, 13] = FEData.ST.ERR;
			array6[1, 14] = FEData.ST.ERR;
			array6[1, 15] = FEData.ST.ERR;
			array6[1, 16] = FEData.ST.ERR;
			array6[1, 17] = FEData.ST.ERR;
			array6[1, 18] = FEData.ST.ERR;
			array6[1, 19] = FEData.ST.ERR;
			array6[2, 0] = FEData.ST.ERR;
			array6[2, 1] = FEData.ST.ERR;
			array6[2, 2] = FEData.ST.ERR;
			array6[2, 3] = FEData.ST.ST1;
			array6[2, 4] = FEData.ST.ST1;
			array6[2, 5] = FEData.ST.ST1;
			array6[2, 6] = FEData.ST.ST1;
			array6[2, 7] = FEData.ST.ST1;
			array6[2, 8] = FEData.ST.ST1;
			array6[2, 9] = FEData.ST.ST1;
			array6[2, 10] = FEData.ST.ERR;
			array6[2, 11] = FEData.ST.ERR;
			array6[2, 12] = FEData.ST.ERR;
			array6[2, 13] = FEData.ST.ERR;
			array6[2, 14] = FEData.ST.ERR;
			array6[2, 15] = FEData.ST.ERR;
			array6[2, 16] = FEData.ST.ERR;
			array6[2, 17] = FEData.ST.ERR;
			array6[2, 18] = FEData.ST.ERR;
			array6[2, 19] = FEData.ST.ERR;
			array6[3, 0] = FEData.ST.ERR;
			array6[3, 1] = FEData.ST.ERR;
			array6[3, 2] = FEData.ST.ERR;
			array6[3, 3] = FEData.ST.ST2;
			array6[3, 4] = FEData.ST.ST2;
			array6[3, 5] = FEData.ST.ST2;
			array6[3, 6] = FEData.ST.ST2;
			array6[3, 7] = FEData.ST.ST2;
			array6[3, 8] = FEData.ST.ST2;
			array6[3, 9] = FEData.ST.ST2;
			array6[3, 10] = FEData.ST.ERR;
			array6[3, 11] = FEData.ST.ERR;
			array6[3, 12] = FEData.ST.ERR;
			array6[3, 13] = FEData.ST.ERR;
			array6[3, 14] = FEData.ST.ERR;
			array6[3, 15] = FEData.ST.ERR;
			array6[3, 16] = FEData.ST.ERR;
			array6[3, 17] = FEData.ST.ERR;
			array6[3, 18] = FEData.ST.ERR;
			array6[3, 19] = FEData.ST.ERR;
			array6[4, 0] = FEData.ST.ERR;
			array6[4, 1] = FEData.ST.ERR;
			array6[4, 2] = FEData.ST.ERR;
			array6[4, 3] = FEData.ST.ERR;
			array6[4, 4] = FEData.ST.ERR;
			array6[4, 5] = FEData.ST.ERR;
			array6[4, 6] = FEData.ST.ERR;
			array6[4, 7] = FEData.ST.ERR;
			array6[4, 8] = FEData.ST.ST1;
			array6[4, 9] = FEData.ST.ST1;
			array6[4, 10] = FEData.ST.ERR;
			array6[4, 11] = FEData.ST.ERR;
			array6[4, 12] = FEData.ST.ERR;
			array6[4, 13] = FEData.ST.ERR;
			array6[4, 14] = FEData.ST.ERR;
			array6[4, 15] = FEData.ST.ERR;
			array6[4, 16] = FEData.ST.ERR;
			array6[4, 17] = FEData.ST.ERR;
			array6[4, 18] = FEData.ST.ERR;
			array6[4, 19] = FEData.ST.ERR;
			array6[5, 0] = FEData.ST.ERR;
			array6[5, 1] = FEData.ST.ERR;
			array6[5, 2] = FEData.ST.ERR;
			array6[5, 3] = FEData.ST.ERR;
			array6[5, 4] = FEData.ST.ERR;
			array6[5, 5] = FEData.ST.ERR;
			array6[5, 6] = FEData.ST.ERR;
			array6[5, 7] = FEData.ST.ST2;
			array6[5, 8] = FEData.ST.ST2;
			array6[5, 9] = FEData.ST.ST2;
			array6[5, 10] = FEData.ST.ERR;
			array6[5, 11] = FEData.ST.ERR;
			array6[5, 12] = FEData.ST.ERR;
			array6[5, 13] = FEData.ST.ERR;
			array6[5, 14] = FEData.ST.ERR;
			array6[5, 15] = FEData.ST.ERR;
			array6[5, 16] = FEData.ST.ERR;
			array6[5, 17] = FEData.ST.ERR;
			array6[5, 18] = FEData.ST.ERR;
			array6[5, 19] = FEData.ST.ERR;
			FEData.Utf8NextState = array6;
			FEData.JC[] array7 = new FEData.JC[128];
			array7[14] = FEData.JC.so;
			array7[15] = FEData.JC.si;
			array7[27] = FEData.JC.esc;
			array7[36] = FEData.JC.dlr;
			array7[38] = FEData.JC.amp;
			array7[40] = FEData.JC.opr;
			array7[41] = FEData.JC.cpr;
			array7[64] = FEData.JC.at;
			array7[66] = FEData.JC.tkB;
			array7[67] = FEData.JC.tkC;
			array7[68] = FEData.JC.tkD;
			array7[72] = FEData.JC.tkH;
			array7[73] = FEData.JC.tkI;
			array7[74] = FEData.JC.tkJ;
			FEData.JisCharClass = array7;
			FEData.JS[,] array8 = new FEData.JS[7, 15];
			array8[0, 1] = FEData.JS.CNTA;
			array8[0, 3] = FEData.JS.S1;
			array8[1, 4] = FEData.JS.S2;
			array8[1, 5] = FEData.JS.S6;
			array8[1, 6] = FEData.JS.S5;
			array8[2, 6] = FEData.JS.S3;
			array8[2, 7] = FEData.JS.S4;
			array8[2, 8] = FEData.JS.CNTJ;
			array8[2, 9] = FEData.JS.CNTJ;
			array8[3, 11] = FEData.JS.CNTJ;
			array8[4, 10] = FEData.JS.CNTK;
			array8[5, 9] = FEData.JS.CNTJ;
			array8[5, 12] = FEData.JS.CNTJ;
			array8[5, 13] = FEData.JS.CNTJ;
			array8[5, 14] = FEData.JS.CNTJ;
			array8[6, 8] = FEData.JS.CNTJ;
			FEData.JisEscNextState = array8;
		}

		internal static FEData.CC[] CharClass = new FEData.CC[]
		{
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.ctlws,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x213f,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.x407e,
			FEData.CC.ctlws,
			FEData.CC.x0080,
			FEData.CC.x818d,
			FEData.CC.x818d,
			FEData.CC.x818d,
			FEData.CC.x818d,
			FEData.CC.x818d,
			FEData.CC.x818d,
			FEData.CC.x818d,
			FEData.CC.x818d,
			FEData.CC.x818d,
			FEData.CC.x818d,
			FEData.CC.x818d,
			FEData.CC.x818d,
			FEData.CC.x818d,
			FEData.CC.x008e,
			FEData.CC.x008f,
			FEData.CC.x909f,
			FEData.CC.x909f,
			FEData.CC.x909f,
			FEData.CC.x909f,
			FEData.CC.x909f,
			FEData.CC.x909f,
			FEData.CC.x909f,
			FEData.CC.x909f,
			FEData.CC.x909f,
			FEData.CC.x909f,
			FEData.CC.x909f,
			FEData.CC.x909f,
			FEData.CC.x909f,
			FEData.CC.x909f,
			FEData.CC.x909f,
			FEData.CC.x909f,
			FEData.CC.x00a0,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xa1bf,
			FEData.CC.xc0c1,
			FEData.CC.xc0c1,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.xc2df,
			FEData.CC.x00e0,
			FEData.CC.xe1ef,
			FEData.CC.xe1ef,
			FEData.CC.xe1ef,
			FEData.CC.xe1ef,
			FEData.CC.xe1ef,
			FEData.CC.xe1ef,
			FEData.CC.xe1ef,
			FEData.CC.xe1ef,
			FEData.CC.xe1ef,
			FEData.CC.xe1ef,
			FEData.CC.xe1ef,
			FEData.CC.xe1ef,
			FEData.CC.xe1ef,
			FEData.CC.xe1ef,
			FEData.CC.xe1ef,
			FEData.CC.x00f0,
			FEData.CC.xf1f7,
			FEData.CC.xf1f7,
			FEData.CC.xf1f7,
			FEData.CC.xf1f7,
			FEData.CC.xf1f7,
			FEData.CC.xf1f7,
			FEData.CC.xf1f7,
			FEData.CC.xf8fc,
			FEData.CC.xf8fc,
			FEData.CC.xf8fc,
			FEData.CC.xf8fc,
			FEData.CC.xf8fc,
			FEData.CC.xfdfe,
			FEData.CC.xfdfe,
			FEData.CC.x00ff
		};

		internal static FEData.ST[,] SJisNextState;

		internal static FEData.ST[,] EucJpNextState;

		internal static FEData.ST[,] GbkWanNextState;

		internal static FEData.ST[,] EucKrCnNextState;

		internal static FEData.ST[,] Big5NextState;

		internal static FEData.ST[,] Utf8NextState;

		internal static FEData.JC[] JisCharClass;

		internal static FEData.JS[,] JisEscNextState;

		internal enum CC : byte
		{
			ctlws,
			x213f,
			x407e,
			x0080,
			x818d,
			x008e,
			x008f,
			x909f,
			x00a0,
			xa1bf,
			xc0c1,
			xc2df,
			x00e0,
			xe1ef,
			x00f0,
			xf1f7,
			xf8fc,
			xfdfe,
			x00ff,
			eof
		}

		internal enum ST : byte
		{
			ST0,
			ST1,
			ST2,
			ST3,
			ST4,
			ST5,
			ACC = 0,
			ERR = 127
		}

		internal enum JC : byte
		{
			ni,
			so,
			si,
			esc,
			dlr,
			amp,
			opr,
			cpr,
			at,
			tkB,
			tkC,
			tkD,
			tkH,
			tkI,
			tkJ
		}

		internal enum JS : byte
		{
			S0,
			S1,
			S2,
			S3,
			S4,
			S5,
			S6,
			CNTA = 128,
			CNTJ,
			CNTK
		}
	}
}
