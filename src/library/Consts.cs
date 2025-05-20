using Newtonsoft.Json;
using System;

namespace HoloSimpID
{
    public static class Consts
    {
        public static Random global = new();

        public const int defaultCartPlan = 7; // In days
        public const char cMultiply = '×';
        public const char cPlusMinus = '±';
    }
}