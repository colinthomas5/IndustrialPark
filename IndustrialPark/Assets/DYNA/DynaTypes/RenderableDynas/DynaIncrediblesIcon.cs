﻿using HipHopFile;
using IndustrialPark.Models;
using SharpDX;
using System.Collections.Generic;
using System.ComponentModel;
using static IndustrialPark.ArchiveEditorFunctions;

namespace IndustrialPark
{
    public class DynaIncrediblesIcon : RenderableRotatableDynaBase
    {
        private const string dynaCategoryName = "Incredibles:Icon";

        protected override short constVersion => 1;
        
        [Category(dynaCategoryName)]
        public AssetSingle UnknownFloat_18 { get; set; }
        [Category(dynaCategoryName)]
        public int UnknownInt_1C { get; set; }

        public DynaIncrediblesIcon(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, DynaType.Incredibles__Icon, game, endianness)
        {
            var reader = new EndianBinaryReader(AHDR.data, endianness);
            reader.BaseStream.Position = dynaDataStartPosition;

            _position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            _yaw = reader.ReadSingle();
            _pitch = reader.ReadSingle();
            _roll = reader.ReadSingle();
            UnknownFloat_18 = reader.ReadSingle();
            UnknownInt_1C = reader.ReadInt32();

            CreateTransformMatrix();
            AddToRenderableAssets(this);
        }

        protected override byte[] SerializeDyna(Game game, Endianness endianness)
        {
            var writer = new EndianBinaryWriter(endianness);

            writer.Write(_position.X);
            writer.Write(_position.Y);
            writer.Write(_position.Z);
            writer.Write(_yaw);
            writer.Write(_pitch);
            writer.Write(_roll);
            writer.Write(UnknownFloat_18);
            writer.Write(UnknownInt_1C);

            return writer.ToArray();
        }

        protected override List<Vector3> vertexSource => SharpRenderer.cubeVertices;

        protected override List<Triangle> triangleSource => SharpRenderer.cubeTriangles;

        public override void Draw(SharpRenderer renderer)
        {
            renderer.DrawCube(world, isSelected);
        }
    }
}