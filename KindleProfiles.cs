// (c) Kyle Sabo 2011

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Kindle.Profiles
{
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

        public string Name
        {
            get;
            internal set;
        }

        internal static Color[] CommonPalette = {
            Color.FromArgb(0x00, 0x00, 0x00),
            Color.FromArgb(0x11, 0x11, 0x11),
            Color.FromArgb(0x22, 0x22, 0x22),
            Color.FromArgb(0x33, 0x33, 0x33),
            Color.FromArgb(0x44, 0x44, 0x44),
            Color.FromArgb(0x55, 0x55, 0x55),
            Color.FromArgb(0x66, 0x66, 0x66),
            Color.FromArgb(0x77, 0x77, 0x77),
            Color.FromArgb(0x88, 0x88, 0x88),
            Color.FromArgb(0x99, 0x99, 0x99),
            Color.FromArgb(0xAA, 0xAA, 0xAA),
            Color.FromArgb(0xBB, 0xBB, 0xBB),
            Color.FromArgb(0xCC, 0xCC, 0xCC),
            Color.FromArgb(0xDD, 0xDD, 0xDD),
            Color.FromArgb(0xEE, 0xEE, 0xEE),
            Color.FromArgb(0xFF, 0xFF, 0xFF)
        };
    }

    public class Kindle3 : KindleProfile
    {
        public Kindle3()
        {
            Width = 600;
            Height = 800;
            Palette = KindleProfile.CommonPalette;
            Name = "Kindle Keyboard";
        }
    }

    public class KindlePaperWhite12 : KindleProfile
    {
        public KindlePaperWhite12()
        {
            Width = 758;
            Height = 1024;
            Palette = KindleProfile.CommonPalette;
            Name = "Kindle PaperWhite 1/2";
        }
    }

    public class KindlePaperWhite3Voyage : KindleProfile
    {
        public KindlePaperWhite3Voyage()
        {
            Width = 1072;
            Height = 1448;
            Palette = KindleProfile.CommonPalette;
            Name = "Kindle PaperWhite 3 / Voyage";
        }
    }
}
