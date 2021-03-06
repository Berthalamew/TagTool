using System;
using System.Collections.Generic;
using System.Text;
using TagTool.Cache;
using TagTool.Cache.Gen3;
using TagTool.Tags;

namespace TagTool.Common
{
    /// <summary>
    /// Represents a magic number which looks like a string.
    /// </summary>
    public struct Tag : IComparable<Tag>, IBlamType
	{
        /// <summary>
        /// The null tag representation.
        /// </summary>
        public static Tag Null { get; } = new Tag("ÿÿÿÿ");

        /// <summary>
        /// Constructs a magic number from an integer.
        /// </summary>
        /// <param name="val">The integer.</param>
        public Tag(int val)
        {
            if (val == -1)
                Value = Null.Value;
            else
                Value = val;
        }

        /// <summary>
        /// Constructs a magic number from a string.
        /// </summary>
        /// <param name="str">The string.</param>
        public Tag(string str)
        {
            var bytes = Encoding.ASCII.GetBytes(str);
            Value = 0;
            foreach (var b in bytes)
            {
                Value <<= 8;
                Value |= b;
            }
        }

        /// <summary>
        /// Constructs a magic number from a character array.
        /// </summary>
        /// <param name="input">The character array.</param>
        public Tag(char[] input)
        {
            var chars = new char[4] { ' ', ' ', ' ', ' ' };

            for (var i = 0; i < input.Length; i++)
            {
                // hackfix to make h3 not crash
                if (input[i] == 0)
                    chars[i] = (char)0x20;
                else
                    chars[i] = input[i];
            }
                

            Value = 0;
            foreach (var c in chars)
            {
                Value <<= 8;
                Value |= c;
            }
        }

        /// <summary>
        /// Gets the value of the magic number as an integer. 
		/// THERE BE DRAGONS HERE: Do not set this manually outside of serialization.
        /// </summary>
        public int Value { get; set; }

        public bool IsNull()
        {
            return Value == -1 || this == Null;
        }


		/// <summary>
		/// Converts the magic number into its string representation.
		/// </summary>
		/// <returns>The string that the magic number looks like.</returns>
		public override string ToString()
        {
            var i = 4;
            var chars = new char[4];
            var val = Value;
            while (val > 0)
            {
                i--;
                chars[i] = (char)(val & 0xFF);
                val >>= 8;
            }
            return (i < 4) ? new string(chars, i, chars.Length - i) : "";
        }

        public static Tag Parse(GameCache cache, string name)
        {
            if (name == "****" || name == "null")
                return Null;

            if (name.Length < 4)
            {
                if (name.Length == 3)
                    name = $"{name} ";
                else if (name.Length == 2)
                    name = $"{name}  ";
            }
            var structureType = cache.TagCache.TagDefinitions.GetTagDefinitionType(name);
            if (structureType != null)
            {
                var attribute = TagStructure.GetTagStructureAttribute(structureType, cache.Version, cache.Platform);
                return new Tag(attribute.Tag);
            }
            
            if(cache.TagCache.TagDefinitions.GetType() == typeof(TagDefinitionsGen3))
            {
                foreach (var pair in cache.TagCache.TagDefinitions.Types)
                    if (name == (pair.Key as TagGroupGen3).Name)
                        return pair.Key.Tag;
            }
            

            return Null;
        }

        public bool TryParse(GameCache cache, List<string> args, out IBlamType result, out string error)
        {
            result = null;
            if (args.Count != 1)
            {
                error = $"{args.Count} arguments supplied; should be 1";
                return false;
            }
            else if (!cache.TagCache.TryParseGroupTag(args[0], out Tag groupTag))
            {
                error = $"Invalid tag group specifier: {args[0]}";
                return false;
            }
            else
            {
                result = groupTag;
                error = null;
                return true;
            }
        }

        public static bool TryParseGroupTag(GameCache cache, string name, out Tag result)
        {
            var structureType = cache.TagCache.TagDefinitions.GetTagDefinitionType(name);
            
            if (structureType != null)
            {
                var attribute = TagStructure.GetTagStructureAttribute(structureType, cache.Version, cache.Platform);
                result = new Tag(attribute.Tag);
                return true;
            }

            if (cache.TagCache.TagDefinitions.GetType() == typeof(TagDefinitionsGen3))
            {
                foreach (var pair in cache.TagCache.TagDefinitions.Types)
                    if (name == (pair.Key as TagGroupGen3).Name)
                    {
                        result = pair.Key.Tag;
                        return true;
                    }
            }

            result = Null;
            return name == "none" || name == "null";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Tag))
                return false;
            var other = (Tag)obj;
            return (Value == other.Value);
        }

        public static bool operator ==(Tag a, Tag b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Tag a, Tag b)
        {
            return !(a == b);
        }

        public static bool operator ==(Tag a, string b)
        {
            return a == new Tag(b);
        }

        public static bool operator !=(Tag a, string b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public int CompareTo(Tag other)
        {
            return Value - other.Value;
        }

        public static implicit operator Tag(string tagString) => new Tag(tagString);
    }
}
