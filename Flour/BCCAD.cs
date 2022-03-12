using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Flour
{
    public class BCCAD
    {
        public uint timestamp;
        public ushort textureWidth;
        public ushort textureHeight;
        public List<Sprite> sprites = new List<Sprite>();
        public List<Animation> animations = new List<Animation>();

        public BCCAD()
        {
            FromBCCAD();
        }

        public void FromBCCAD()
        {
            byte[] data = File.ReadAllBytes(@"C:\Users\Braedon\Desktop\RHModding\RomFS\cellanim\arc\batter_00.bccad");
            using (Stream stream = new MemoryStream(data))
            {
                BinaryReader reader = new BinaryReader(stream);
                // var startByte = stream.ReadByte();
                try
                {
                    timestamp = reader.ReadUInt32();
                    textureWidth = reader.ReadUInt16();
                    textureHeight = reader.ReadUInt16();
                    var spriteCount = reader.ReadUInt32();
                    for (int i = 0; i < spriteCount; i++)
                    {
                        Sprite sprite = new Sprite();
                        var partsCount = reader.ReadUInt32();
                        for (int j = 0; j < partsCount; j++)
                        {
                            SpritePart part = new SpritePart();
                            PosInTexture posInTexture = new PosInTexture();
                            posInTexture.x = reader.ReadUInt16();
                            posInTexture.y = reader.ReadUInt16();
                            posInTexture.width = reader.ReadUInt16();
                            posInTexture.height = reader.ReadUInt16();
                            part.texturePos = posInTexture;

                            part.posX = reader.ReadInt16();
                            part.posY = reader.ReadInt16();
                            part.scaleX = reader.ReadSingle();
                            part.scaleY = reader.ReadSingle();
                            part.rotation = reader.ReadSingle();
                            part.flipHorizontal = reader.ReadBoolean();
                            part.flipVertical = reader.ReadBoolean();
                            // part.multiplyColor =

                            reader.ReadByte();
                            reader.ReadByte();
                            reader.ReadByte();
                            
                            reader.ReadByte();
                            reader.ReadByte();
                            reader.ReadByte();
                            
                            part.opacity = reader.ReadByte();

                            for (int k = 0; k < 12; k++)
                            {
                                reader.Read(); // unknown data
                            }
                            
                            reader.ReadByte();
                            reader.ReadInt16(); // unknown
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();
                            reader.ReadSingle();

                            sprite.parts.Add(part);
                            Console.WriteLine(posInTexture.x);
                        }
                        sprites.Add(sprite);
                    }
                }
                finally
                {
                    if (reader != null)
                        reader.Close();
                }
            }
        }
    }

    public class Sprite
    {
        public List<SpritePart> parts = new List<SpritePart>();
    }

    public struct SpritePart
    {
        public PosInTexture texturePos;
        public short posX;
        public short posY;
        public float scaleX;
        public float scaleY;
        public float rotation;
        public bool flipHorizontal;
        public bool flipVertical;
        public Color multiplyColor;
        public Color screenColor;
        public byte opacity;
        public StereoDepth depth;
    }

    public struct PosInTexture
    {
        public uint x;
        public uint y;
        public uint width;
        public uint height;
    }

    public struct StereoDepth
    {
        public float topLeft;
        public float topRight;
        public float bottomLeft;
        public float bottomRight;
    }

    public struct Animation
    {
        public string name;
        public int interpolation;
    }

    public struct AnimationStep
    {
        public ushort sprite;
        public ushort duration;
        public short posX;
        public short posY;
        public float depth;
        public float scaleX;
        public float scaleY;
        public float rotation;
        public Color multiplyColor;
        public ushort opacity;
    }

    public struct Color
    {
        public byte red;
        public byte blue;
        public byte green;
    }
}
