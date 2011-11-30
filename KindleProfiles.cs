// (c) Kyle Sabo 2011

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Kindle.Profiles
{
    public class KindleProfiles
    {
        public static Kindle3 Kindle3 = new Kindle3();
    }

    public abstract class KindleProfile
    {
        public Color[] Palette
        {
            get;
            internal set;
        }
        public int Width
        {
            get;
            internal set;
        }
        public int Height
        {
            get;
            internal set;
        }
    }

    public class Kindle3 : KindleProfile
    {
        public Kindle3()
        {
            Width = 600;
            Height = 800;

            Palette = new Color[15];

            Palette[0] = Color.FromArgb(0x00, 0x00, 0x00);
            Palette[1] = Color.FromArgb(0x11, 0x11, 0x11);
            Palette[2] = Color.FromArgb(0x22, 0x22, 0x22);
            Palette[3] = Color.FromArgb(0x33, 0x33, 0x33);
            Palette[4] = Color.FromArgb(0x44, 0x44, 0x44);
            Palette[5] = Color.FromArgb(0x55, 0x55, 0x55);
            Palette[6] = Color.FromArgb(0x66, 0x66, 0x66);
            Palette[7] = Color.FromArgb(0x77, 0x77, 0x77);
            Palette[8] = Color.FromArgb(0x88, 0x88, 0x88);
            Palette[9] = Color.FromArgb(0x99, 0x99, 0x99);
            Palette[10] = Color.FromArgb(0xAA, 0xAA, 0xAA);
            Palette[11] = Color.FromArgb(0xBB, 0xBB, 0xBB);
            Palette[12] = Color.FromArgb(0xCC, 0xCC, 0xCC);
            Palette[13] = Color.FromArgb(0xDD, 0xDD, 0xDD);
            Palette[14] = Color.FromArgb(0xFF, 0xFF, 0xFF);
        }
    }
}
