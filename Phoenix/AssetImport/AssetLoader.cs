using Phoenix.AssetImport.Model;
using Phoenix.AssetImport.Texture;
using Phoenix.Rendering.Shaders;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Text;

namespace Phoenix.AssetImport
{
    public static class AssetLoader
    {
        public const string ManifestDefaultPath = "Content/asset-manifest.json";
        public static string ContentBinBaseDirectory = "";

        private static readonly Dictionary<string, BinaryModel> _loadedModels = new();
        private static readonly Dictionary<string, BinaryTexture> _loadedTextures = new();
        private static readonly Dictionary<string, GLShader> _loadedShaders = new();

        private static AssetManifest _assetManifest = default!;
        internal static GL GL = default!;
        public static void Init(GL gl, string path = ManifestDefaultPath)
        {
            GL=gl;
            var manifestPath = Path.Combine(AppContext.BaseDirectory, path);
            _assetManifest = AssetManifestIO.Load(manifestPath);
            ContentBinBaseDirectory = "Content/ContentBin/";
        }
        
        public static BinaryModel LoadModel(string name)
        {
            var absolutePath = AssetAbsolutePath(name);
            if (!_loadedModels.TryGetValue(absolutePath, out var model))
            {
                model = BinaryModelReader.Load(absolutePath);
                _loadedModels[absolutePath] = model;
            }

            return model;
        }
        public static BinaryTexture LoadTextureAbs(string absolutePath)
        {
            if (!_loadedTextures.TryGetValue(absolutePath, out var tex))
            {
                tex = BinaryTextureReader.Load(absolutePath);
                _loadedTextures[absolutePath] = tex;
            }

            return tex;
        }

        public static BinaryTexture LoadTexture(string name)
        {
            var absolutePath = AssetAbsolutePath(name);
            return LoadTextureAbs(absolutePath);
        }


        public static GLShader LoadShader(string name)
        {
            var path = ShaderAbsolutePath(name);
            return LoadShaderAbs(path.absVert, path.absFrag);
        }
        public static GLShader LoadShaderAbs(string vert, string frag)
        {
            var name = Path.GetFileNameWithoutExtension(vert);

            if(!_loadedShaders.TryGetValue(name, out var shader))
            {
                shader = new GLShader(GL, vert, frag);
            }

            return shader;
        }

        internal static string AssetAbsolutePath(string name)
        {
            var noExt = Path.ChangeExtension(name, null).Replace('\\', '/');

            var relativeBin = Path.ChangeExtension(name, ".bin");

            var asset = _assetManifest.Assets
                .Find(a =>
                    Path.ChangeExtension(a.RelativePath, null)
                    .Replace('\\', '/')
                    .Equals(noExt, StringComparison.OrdinalIgnoreCase));

            if (asset == null)
                throw new Exception($"Asset '{name}' not found in manifest");

            var absolutePath = Path.Combine(
                AppContext.BaseDirectory,
                ContentBinBaseDirectory,
                relativeBin
            ).Replace('\\', '/');

            return absolutePath;
        }

        internal static (string absVert, string absFrag) ShaderAbsolutePath(string name)
        {
            var fileName = Path.GetFileNameWithoutExtension(name);

            var assets = _assetManifest.Assets
                .FindAll(a =>
                    Path.GetFileNameWithoutExtension(a.RelativePath)
                    .Equals(fileName, StringComparison.OrdinalIgnoreCase)).ToList();

            if (assets.Count != 2)
                throw new Exception($"Expected 2 files for {name}, found {assets.Count}");


            var pathA = assets[0].RelativePath;
            var pathB = assets[1].RelativePath;


            var absolutePathA = Path.Combine(
               AppContext.BaseDirectory,
               ContentBinBaseDirectory,
               pathA
            ).Replace('\\', '/');
            var absolutePathB = Path.Combine(
               AppContext.BaseDirectory,
               ContentBinBaseDirectory,
               pathB
            ).Replace('\\', '/');


            return Path.GetExtension(pathA) == ".vert" ?
                (absolutePathA, absolutePathB) :
                (absolutePathB, absolutePathA);
        }

    }

}
