using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

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

                            part.multiplyColor = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                            part.screenColor = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());

                            part.opacity = reader.ReadByte();

                            for (int k = 0; k < 12; k++)
                            {
                                reader.ReadByte(); // unknown data
                            }

                            part.designationID = reader.ReadByte();
                            reader.ReadInt16(); // unknown
                            StereoDepth stereoDepth = new StereoDepth();
                            stereoDepth.topLeft = reader.ReadSingle();
                            stereoDepth.bottomLeft = reader.ReadSingle();
                            stereoDepth.topRight = reader.ReadSingle();
                            stereoDepth.bottomRight = reader.ReadSingle();
                            part.depth = stereoDepth;

                            sprite.parts.Add(part);
                        }
                        sprites.Add(sprite);
                    }

                    var animCount = reader.ReadInt32();
                    for (int i = 0; i < animCount; i++)
                    {
                        List<byte> bytes = new List<byte>();
                        var size = reader.ReadByte();
                        for (int k = 0; k < size; k++)
                        {
                            bytes.Add(reader.ReadByte());
                        }
                        var paddingSize = 4 - ((size + 1) % 4);
                        for (int k = 0; k < paddingSize; k++)
                        {
                            reader.ReadByte();
                        }
                        string final = Encoding.UTF8.GetString(bytes.ToArray());
                        Console.WriteLine(final);

                        Animation animation = new Animation();
                        animation.name = final;
                        animation.interpolation = reader.ReadInt32();
                        var stepCount = reader.ReadUInt32();
                        for (int j = 0; j < stepCount; j++)
                        {
                            AnimationStep step = new AnimationStep();
                            step.sprite = reader.ReadUInt16();
                            step.duration = reader.ReadUInt16();
                            step.posX = reader.ReadInt16();
                            step.posY = reader.ReadInt16();
                            step.depth = reader.ReadSingle();
                            step.scaleX = reader.ReadSingle();
                            step.scaleY = reader.ReadSingle();
                            step.rotation = reader.ReadSingle();
                            step.multiplyColor = new Color(reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                            reader.ReadByte();
                            reader.ReadByte();
                            reader.ReadByte();
                            step.opacity = reader.ReadUInt16();
                            animation.steps.Add(step);
                        }
                        animations.Add(animation);
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
        public byte designationID;
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

    public class Animation
    {
        public string? name;
        public int interpolation;
        public List<AnimationStep> steps = new List<AnimationStep>();
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
        public byte unknown1;
        public byte unknown2;
        public ushort opacity;
    }

    public struct Color
    {
        public byte red;
        public byte green;
        public byte blue;

        public Color(byte red, byte green, byte blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }
    }
}
