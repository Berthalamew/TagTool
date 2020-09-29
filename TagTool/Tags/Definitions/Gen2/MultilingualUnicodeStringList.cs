using TagTool.Cache;
using TagTool.Common;
using System;
using System.Collections.Generic;
using static TagTool.Tags.TagFieldFlags;

namespace TagTool.Tags.Definitions.Gen2
{
    [TagStructure(Name = "multilingual_unicode_string_list", Tag = "unic", Size = 0x44)]
    public class MultilingualUnicodeStringList : TagStructure
    {
        public List<MultilingualUnicodeStringReference> StringReferences;
        public byte[] StringDataUtf8;
        [TagField(Flags = Padding, Length = 36)]
        public byte[] Padding1;
        
        [TagStructure(Size = 0x28)]
        public class MultilingualUnicodeStringReference : TagStructure
        {
            public StringId StringId;
            public int EnglishOffset;
            public int JapaneseOffset;
            public int GermanOffset;
            public int FrenchOffset;
            public int SpanishOffset;
            public int ItalianOffset;
            public int KoreanOffset;
            public int ChineseOffset;
            public int PortugueseOffset;
        }
    }
}

