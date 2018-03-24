﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XOutput.Input.XInput;

namespace XOutput.Input.Mapper
{
    public abstract class InputMapperBase
    {
        /// <summary>
        /// DPad index to use
        /// </summary>
        public int SelectedDPad { get; set; }

        private const char SPLIT_CHAR = ',';
        protected const string SELECTEDDPAD_KEY = "SelectedDPad";
        protected readonly Dictionary<XInputTypes, MapperData> mappings = new Dictionary<XInputTypes, MapperData>();

        public InputMapperBase()
        {
            SelectedDPad = -1;
        }

        /// <summary>
        /// Sets the mapping for a given XInput.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public void SetMapping(XInputTypes type, MapperData to)
        {
            mappings[type] = to;
        }

        /// <summary>
        /// Gets the mapping for a given XInput. If the mapping does not exist, returns a new mapping.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public MapperData GetMapping(XInputTypes? type)
        {
            if (!type.HasValue)
                return null;
            if (!mappings.ContainsKey(type.Value))
            {
                mappings[type.Value] = new MapperData { InputType = null, MinValue = type.Value.GetDisableValue(), MaxValue = type.Value.GetDisableValue() };
            }
            return mappings[type.Value];
        }

        public virtual Dictionary<string, string> ToDictionary()
        {
            var dict = new Dictionary<string, string>();
            dict.Add(SELECTEDDPAD_KEY, SelectedDPad.ToString());
            foreach (var mapping in mappings)
            {
                dict.Add(mapping.Key.ToString(),
                    string.Join(SPLIT_CHAR.ToString(), new string[] { mapping.Value.InputType?.ToString(), ((int)Math.Round(mapping.Value.MinValue * 100)).ToString(), ((int)Math.Round(mapping.Value.MaxValue * 100)).ToString() }));
            }
            return dict;
        }

        protected static Dictionary<XInputTypes, MapperData> FromDictionary(Dictionary<string, string> data, Type enumType)
        {
            var dict = new Dictionary<XInputTypes, MapperData>();
            foreach (var mapping in data)
            {
                try
                {
                    var key = (XInputTypes)Enum.Parse(typeof(XInputTypes), mapping.Key);
                    var values = mapping.Value.Split(SPLIT_CHAR);
                    if (values.Length != 3)
                    {
                        throw new ArgumentException("Invalid text: " + mapping.Value);
                    }
                    Enum input = null;
                    if (!string.IsNullOrEmpty(values[0]))
                        input = (Enum)Enum.Parse(enumType, values[0]);
                    var min = double.Parse(values[1]) / 100;
                    var max = double.Parse(values[2]) / 100;
                    dict.Add(key, new MapperData { InputType = input, MinValue = min, MaxValue = max });
                }
                catch (Exception)
                {
                    // Ignored
                }
            }
            return dict;
        }
    }
}
