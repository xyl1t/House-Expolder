using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OLC_CodeJam
{
    interface IRotateAble
    {
        float Angle { get; set; }
        Size BoundingBox { get; set; }
    }
}
