﻿using System;
using System.Collections.Generic;
using System.Text;
using WinApi;

namespace Gothic.Types
{
    public class zVec3 : zClass, IDisposable
    {
        public const int ByteSize = 12;

        public class FuncAddresses
        {
        }

        public zVec3(int address)
            : base(address)
        {
            
        }

        public zVec3()
        {
        }

        public static zVec3 Create()
        {
            return zVec3.Create(0, 0, 0);
        }

        public static zVec3 Create(float x, float y, float z)
        {
            int ptr = Process.Alloc(zVec3.ByteSize).ToInt32();
            zVec3 rV = new zVec3(ptr);
            rV.X = x; rV.Y = y; rV.Z = z;
            return rV;
        }
        
        public void Dispose()
        {
            Process.Free(new IntPtr(Address), zVec3.ByteSize);
        }

        public float this[int index]
        {
            get
            {
                if (index >= 0 && index < 3)
                {
                    return Process.ReadFloat(Address + 4 * index);
                }
                throw new ArgumentException("zVec3 get index is outside of range: " + index);
            }
            set
            {
                if (index >= 0 && index < 3)
                {
                    Process.Write(Address + 4 * index, value);
                }
                throw new ArgumentException("zVec3 set index is outside of range: " + index);
            }
        }

        public float X 
        {
            get { return Process.ReadFloat(Address); }
            set { Process.Write(Address, value); }
        }

        public float Y
        {
            get { return Process.ReadFloat(Address + 4); }
            set { Process.Write(Address + 4, value); }
        }

        public float Z
        {
            get { return Process.ReadFloat(Address + 8); }
            set { Process.Write(Address + 8, value); }
        }

        public override string ToString()
        {
            return string.Format("zVec3({0} {1} {2})", X, Y, Z);
        }
    }
}
