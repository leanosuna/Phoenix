using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Phoenix.Framework.Rendering.Shaders
{
    public interface IResizeAffected
    {
        public void HandleResize(Vector2 size);
    }
}
