﻿using System;
using UnityEngine;

namespace Sabotris.Util
{
    public static class Extensions
    {
        private const float Tolerance = 0.001f;
        
        private static bool IsLessThan(this Vector3Int self, Vector3Int pos) => self.x < pos.x || self.y < pos.y || self.z < pos.z;

        private static bool IsGreaterThan(this Vector3Int self, Vector3Int pos) => self.x > pos.x || self.y > pos.y || self.z > pos.z;

        public static bool IsOutside(this Vector3Int self, Vector3Int bottomLeft, Vector3Int topRight) => self.IsLessThan(bottomLeft) || self.IsGreaterThan(topRight);

        public static Vector3Int Round(this Vector3 self, int round)
        {
            var num = 1.0 / round;
            // return new Vector3Int(
            //     (int) (Math.Round(Math.Abs(self.x) * num) * round * Math.Sign(self.x)),
            //     (int) (Math.Round(Math.Abs(self.y) * num) * round * Math.Sign(self.y)),
            //     (int) (Math.Round(Math.Abs(self.z) * num) * round * Math.Sign(self.z))
            // );
            return new Vector3Int(
                (int) (Math.Round(self.x * num) * round),
                (int) (Math.Round(self.y * num) * round),
                (int) (Math.Round(self.z * num) * round)
            );
        }

        public static bool Same(this float self, float compare) => Math.Abs(self - compare) <= Tolerance;

        public static float GetMinValue(this Vector3 self) => Mathf.Min(Math.Abs(self.x), Math.Abs(self.y), Math.Abs(self.z));

        public static double Lerp(this double self, double target, double time)
        {
            return (target - self) * time;
        }
    }
}