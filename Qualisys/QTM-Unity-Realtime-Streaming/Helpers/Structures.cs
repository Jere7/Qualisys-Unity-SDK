﻿using UnityEngine;
using System.Collections.Generic;
using System;

namespace QualisysRealTime.Unity
{
    internal interface ITakeFrom<T>
    {
        void TakeFrom(T source);
    }

    public class SixDOFBody : ITakeFrom<SixDOFBody>
    {
        public SixDOFBody() { }
        public string Name = "";
        public Vector3 Position = Vector3.zero;
        public Quaternion Rotation = Quaternion.identity;
        public Color Color = Color.yellow;
        public void TakeFrom(SixDOFBody source)
        {
            this.Name = source.Name;
            this.Position = source.Position;
            this.Rotation = source.Rotation;
            this.Color = source.Color;
        }
    }

    public class LabeledMarker : ITakeFrom<LabeledMarker>
    {
        public LabeledMarker() { }
        public string Name = "";
        public Vector3 Position  = Vector3.zero;
        public Color Color = Color.yellow;
        public float Residual = 0f;

        public void TakeFrom(LabeledMarker source)
        {
            this.Name = source.Name;
            this.Position = source.Position;
            this.Residual = source.Residual;
            this.Color = source.Color;
        }
    }

    public class UnlabeledMarker : ITakeFrom<UnlabeledMarker>
    {
        public uint Id = 0;
        public Vector3 Position = Vector3.zero;
        public float Residual = 0f;
        public void TakeFrom(UnlabeledMarker source)
        {
            this.Id = source.Id;
            this.Position = source.Position;
            this.Residual = source.Residual;
        }
    }

    public class Bone : ITakeFrom<Bone>
    {
        public Bone() { }
        public string From = "";
        public LabeledMarker FromMarker = new LabeledMarker();
        public string To = "";
        public LabeledMarker ToMarker = new LabeledMarker();
        public Color Color = Color.yellow;
        public void TakeFrom(Bone source)
        {
            From = source.From;
            FromMarker.TakeFrom(source.FromMarker);
            To = source.To;
            ToMarker.TakeFrom(source.ToMarker);
            Color = source.Color;
        }
    }

    public class GazeVector : ITakeFrom<GazeVector>
    {
        public GazeVector() { }
        public string Name = "" ;
        public Vector3 Position = Vector3.zero;
        public Vector3 Direction = Vector3.forward;
        public void TakeFrom(GazeVector source)
        {
            Name = source.Name;
            Position = source.Position;
            Direction = source.Direction;
        }
    }

    public class AnalogChannel : ITakeFrom<AnalogChannel>
    {
        public string Name = "";
        public float[] Values = new float[0];
        public AnalogChannel() { }
        public AnalogChannel(AnalogChannel analogChannel)
        {
            Name = analogChannel.Name;
            Values = new float[analogChannel.Values.Length];
            Array.Copy(analogChannel.Values, Values, analogChannel.Values.Length);
        }
        public void TakeFrom(AnalogChannel source)
        {
            Name = source.Name;
            if (Values.Length != source.Values.Length)
            {
                Array.Resize(ref Values, source.Values.Length);
            }
            Array.Copy(source.Values, Values, source.Values.Length);
        }

    }
    public class Segment
    {
        public string Name = "";
        public uint Id = 0;
        public uint ParentId = 0;
        public Vector3 Position = Vector3.zero;
        public Quaternion Rotation = Quaternion.identity;
        public Vector3 TPosition = Vector3.zero;
        public Quaternion TRotation = Quaternion.identity;
    }

    public class Skeleton : ITakeFrom<Skeleton>
    {
        public string Name = "";
        public Dictionary<uint, Segment> Segments = new Dictionary<uint, Segment>();
        public Skeleton() { }
        public Skeleton(Skeleton skeleton) 
        {
            Name = skeleton.Name;
            foreach (var kv in skeleton.Segments) {
                var segment = kv.Value;
                var key = kv.Key;
                Segments.Add(key, new Segment() {  
                    Id = segment.Id, 
                    Name = segment.Name, 
                    ParentId = segment.ParentId, 
                    Position = segment.Position, 
                    Rotation = segment.Rotation, 
                    TPosition = segment.TPosition, 
                    TRotation = segment.TRotation
                });
            }
        }

        public void TakeFrom(Skeleton source) 
        {
            Name = source.Name;
            foreach (var kv in source.Segments)
            {
                var segment = kv.Value;
                var key = kv.Key;
                Segment s = null;
                if (Segments.TryGetValue(key, out s))
                {
                    s.Id = segment.Id;
                    s.Name = segment.Name;
                    s.ParentId = segment.ParentId;
                    s.Position = segment.Position;
                    s.Rotation = segment.Rotation;
                    s.TPosition = segment.TPosition;
                    s.TRotation = segment.TRotation;
                }
                else
                {
                    Segments.Add(key, new Segment()
                    {
                        Id = segment.Id,
                        Name = segment.Name,
                        ParentId = segment.ParentId,
                        Position = segment.Position,
                        Rotation = segment.Rotation,
                        TPosition = segment.TPosition,
                        TRotation = segment.TRotation
                    });
                }
            }
        } 
    }
}
