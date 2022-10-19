using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SpcReader
{
	public class SPCHeader
	{
		public FtflgValues Ftflag;
		public byte Fversn;
		public byte Fexper;
		public byte Fexp;
		public int Fnpts;
		public double Ffirst;
		public double Flast;
		public int Fnsub;
		public byte Fxtype;
		public byte Fytype;
		public byte Fztype;
		public byte Fpost;
		public DateTime Fdate;
		public string Fres;
		public string Fsource;
		public short Fpeakpt;
		public float[] Fspare = new float[4];
		public string Fcmnt;
		public string Fcatxt;
		public int Flogoff;
		public int Fmods;
		public byte Fprocs;
		public byte Flevel;
		public short Fsampin;
		public float Ffactor;
		public string Fmethod;
		public float Fzinc;
		public int Fwplanes;
		public float Fwinc;
		public byte Fwtype;
		public string Freserv;

		public static SPCHeader ReadHeader(byte[] input)
		{
			var returnValue = new SPCHeader();

			using (BinaryReader br = new BinaryReader(new MemoryStream(input), System.Text.Encoding.UTF8))
			{
				returnValue.Ftflag = (FtflgValues)br.ReadByte();
				returnValue.Fversn = br.ReadByte();

				if (returnValue.Fversn != 0x4b)
				{
					throw new FormatException("The input SPC file is an unknown format");
				}

				returnValue.Fexper = br.ReadByte();
				returnValue.Fexp = br.ReadByte();
				returnValue.Fnpts = br.ReadInt32();
				returnValue.Ffirst = br.ReadDouble();
				returnValue.Flast = br.ReadDouble();
				returnValue.Fnsub = br.ReadInt32();
				returnValue.Fxtype = br.ReadByte();
				returnValue.Fytype = br.ReadByte();
				returnValue.Fztype = br.ReadByte();
				returnValue.Fpost = br.ReadByte();

				var dateInt = br.ReadInt32();
				returnValue.Fdate = new DateTime
				(
					(int)(dateInt & 0xFFF0_0000) >> 20,
					(dateInt & 0x000F_0000) >> 16,
					(dateInt & 0x0000_F800) >> 11,
					(dateInt & 0x0000_07C0) >> 6,
					(dateInt & 0x0000_003F),
					0
				);

				returnValue.Fres = Encoding.UTF8.GetString(br.ReadBytes(9));
				returnValue.Fsource = Encoding.UTF8.GetString(br.ReadBytes(9));
				returnValue.Fpeakpt = br.ReadInt16();

				var spare = br.ReadBytes(32);

				for (int i = 0; i < 4; i++)
				{
					returnValue.Fspare[i] = BitConverter.ToSingle(spare, i * sizeof(Single));
				}

				returnValue.Fcmnt = Encoding.UTF8.GetString(br.ReadBytes(130));
				returnValue.Fcatxt = Encoding.UTF8.GetString(br.ReadBytes(30));
				returnValue.Flogoff = br.ReadInt32();
				returnValue.Fmods = br.ReadInt32();
				returnValue.Fprocs = br.ReadByte();
				returnValue.Flevel = br.ReadByte();
				returnValue.Fsampin = br.ReadInt16();
				returnValue.Ffactor = br.ReadSingle();
				returnValue.Fmethod = Encoding.UTF8.GetString(br.ReadBytes(48));
				returnValue.Fzinc = br.ReadSingle();
				returnValue.Fwplanes = br.ReadInt32();
				returnValue.Fwinc = br.ReadSingle();
				returnValue.Fwtype = br.ReadByte();
				returnValue.Freserv = Encoding.UTF8.GetString(br.ReadBytes(187));
			}

			return returnValue;
		}
	}

	[Flags]
	public enum FtflgValues
	{
		TSPREC = 0x01,
		TCGRAM = 0x02,
		TMULTI = 0x04,
		TRANDM = 0x08,
		TORDRD = 0x10,
		TALABS = 0x20,
		TXYXYS = 0x40,
		TXVALS = 0x80,
	}
}
