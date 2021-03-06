﻿using System.Collections.Generic;
using TagTool.Cache.Codecs;
using TagTool.Tags;

namespace TagTool.Cache.Resources
{
    [TagStructure(Name = "cache_file_resource_layout_table", Tag = "play", Size = 0x3C, MaxVersion = CacheVersion.HaloReach, Platform = CachePlatform.Original)]
    [TagStructure(Name = "cache_file_resource_layout_table", Tag = "play", Size = 0x48, MinVersion = CacheVersion.HaloReach, Platform = CachePlatform.MCC)]
    public class ResourceLayoutTable : TagStructure
    {
        public List<CodecDefinition> CodecDefinitions;
        public List<ResourceSharedFile> SharedFiles;
        public List<ResourcePage> Pages;
        public List<ResourceSubpageTable> SubpageTables;
        public List<ResourceSection> Sections;
    }
}