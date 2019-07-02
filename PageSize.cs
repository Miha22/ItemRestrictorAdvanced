using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemRestrictorAdvanced
{
    class PageSize
    {
        public byte Width { get; set; }
        public byte Height { get; set; }

        public PageSize(byte width, byte height)
        {
            Width = width;
            Height = height;
        }
    }
}
