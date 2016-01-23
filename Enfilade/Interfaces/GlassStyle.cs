using System;

namespace Enfilade.Interfaces
{
    [Flags]
    public enum GlassStyle
    {
        BottomLeftCorner = 1 << 0,
        BottomRightCorner = 1 << 1,
        TopLeftCorner = 1 << 2,
        TopRightCorner = 1 << 3
    }
}