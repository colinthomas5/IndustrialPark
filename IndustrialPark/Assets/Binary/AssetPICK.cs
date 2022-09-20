﻿using HipHopFile;
using System.Collections.Generic;
using System.ComponentModel;

namespace IndustrialPark
{
    public class EntryPICK : GenericAssetDataContainer
    {
        public AssetID PickupHash { get; set; }
        public AssetByte PickupType { get; set; }
        public AssetByte PickupIndex { get; set; }
        [TypeConverter(typeof(HexUShortTypeConverter))]
        public ushort PickupFlags { get; set; }
        public uint Quantity { get; set; }
        [ValidReferenceRequired]
        public AssetID Model { get; set; }
        public AssetID Animation { get; set; }

        public EntryPICK() { }
        public EntryPICK(EndianBinaryReader reader)
        {
            PickupHash = reader.ReadUInt32();
            PickupType = reader.ReadByte();
            PickupIndex = reader.ReadByte();
            PickupFlags = reader.ReadUInt16();
            Quantity = reader.ReadUInt32();
            Model = reader.ReadUInt32();
            Animation = reader.ReadUInt32();
        }

        public byte[] Serialize(Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.Write(PickupHash);
                writer.Write(PickupType);
                writer.Write(PickupIndex);
                writer.Write(PickupFlags);
                writer.Write(Quantity);
                writer.Write(Model);
                writer.Write(Animation);

                return writer.ToArray();
            }
        }

        public override string ToString() =>
            $"[{HexUIntTypeConverter.StringFromAssetID(PickupHash)}] - [{HexUIntTypeConverter.StringFromAssetID(Model)}]";
    }

    public class AssetPICK : Asset
    {
        public override string AssetInfo => $"{Entries.Length} entries";

        public static Dictionary<uint, uint> pickEntries = new Dictionary<uint, uint>();

        private EntryPICK[] _entries;
        [Category("Pickup Table")]
        public EntryPICK[] Entries
        {
            get => _entries;
            set
            {
                _entries = value;
                UpdateDictionary();
            }
        }

        public AssetPICK(Section_AHDR AHDR, Game game, Endianness endianness) : base(AHDR, game, endianness)
        {
            using (var reader = new EndianBinaryReader(AHDR.data, endianness))
            {
                reader.ReadInt32();
                _entries = new EntryPICK[reader.ReadInt32()];
                for (int i = 0; i < _entries.Length; i++)
                    _entries[i] = new EntryPICK(reader);

                UpdateDictionary();
            }
        }

        public override byte[] Serialize(Game game, Endianness endianness)
        {
            using (var writer = new EndianBinaryWriter(endianness))
            {
                writer.WriteMagic("PICK");
                writer.Write(_entries.Length);
                foreach (var l in _entries)
                    writer.Write(l.Serialize(endianness));

                return writer.ToArray();
            }
        }

        private void UpdateDictionary()
        {
            pickEntries.Clear();

            foreach (EntryPICK entry in _entries)
                pickEntries[entry.PickupHash] = entry.Model;
        }

        public void ClearDictionary()
        {
            foreach (EntryPICK entry in _entries)
                pickEntries.Remove(entry.PickupHash);
        }
    }
}