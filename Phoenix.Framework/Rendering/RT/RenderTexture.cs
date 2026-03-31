using Silk.NET.OpenGL;
using Phoenix.Framework.Rendering.Textures;
using System.Numerics;

namespace Phoenix.Framework.Rendering.RT
{
    public class RenderTexture
    {
        public bool FollowsWindowSize { get; private set; }

        public Vector2 SizeMultiplier { get; private set; }
        public Vector2 Size
        {
            get;
            internal set;
        }
        
        internal GLTexture Texture = default!;
        

        public RenderTexture(GLTexture tex, RenderTextureInfo ti)
        {
            Texture = tex;
            FollowsWindowSize = ti.FollowsWindowSize;
            SizeMultiplier = ti.SizeMultiplier;
            Size = ti.Size;
        }
        
        public RenderTexture(GL gl)
        {
            var ti = new RenderTextureInfo();
            var tex = new GLTexture(gl, ti);

            Texture = tex;
            FollowsWindowSize = ti.FollowsWindowSize;
            SizeMultiplier = ti.SizeMultiplier;
            Size = ti.Size;
        }

        public static implicit operator uint(RenderTexture target)
        {
            return target.Texture.Handle;
        }
    }
}
