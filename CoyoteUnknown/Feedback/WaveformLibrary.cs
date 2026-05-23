using System;
using System.Collections.Generic;

namespace CoyoteUnknown.Feedback
{
    public static class WaveformLibrary
    {
        
        

        
        public static readonly string[] HeavyShock = new string[]
        {
            "282828285A5A5A5A", 
            "2828282846464646", 
            "2828282832323232"  
        };

        
        public static readonly string[] Sting = new string[]
        {
            "5050505032323232", 
            "505050501E1E1E1E", 
            "505050500A0A0A0A"  
        };

        
        public static readonly string[] Heartbeat = new string[]
        {
            "0C0C0C0C14141414", 
            "0C0C0C0C28282828", 
            "0C0C0C0C14141414", 
            "0C0C0C0C00000000"  
        };

        
        public static readonly string[] SoftBuzz = new string[]
        {
            "1E1E1E1E0A0A0A0A", 
            "1E1E1E1E0A0A0A0A"  
        };

        
        private static readonly Dictionary<string, string[]> Library = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            { "HeavyShock", HeavyShock },
            { "Sting", Sting },
            { "Heartbeat", Heartbeat },
            { "SoftBuzz", SoftBuzz }
        };

        
        
        
        public static string[] Get(string name)
        {
            if (string.IsNullOrEmpty(name)) return SoftBuzz;
            if (Library.TryGetValue(name, out var pattern))
            {
                return pattern;
            }
            return SoftBuzz;
        }

        
        
        
        public static List<string> GetAvailableNames()
        {
            return new List<string>(Library.Keys);
        }
    }
}