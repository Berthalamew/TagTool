using TagTool.Cache;
using System;
using System.Collections.Generic;
using TagTool.Serialization;
using TagTool.Tags.Definitions;

namespace TagTool.Commands.Porting
{
    public class ListBitmapsCommand : Command
    {
        private GameCacheContext CacheContext { get; }
        private CacheFile BlamCache { get; }

        public ListBitmapsCommand(GameCacheContext cacheContext, CacheFile blamCache)
            : base(CommandFlags.None,

                  "ListBitmaps",
                  "",

                  "ListBitmaps <Blam Tag>",
                  "")
        {
            CacheContext = cacheContext;
            BlamCache = blamCache;
        }

        public override object Execute(List<string> args)
        {
            if (args.Count != 1)
                return false;

            CacheFile.IndexItem item = null;

            Console.WriteLine("Verifying blam shader tag...");

            var shaderName = args[0];

            foreach (var tag in BlamCache.IndexItems)
            {
                if ((tag.ParentClass == "rm") && tag.Filename == shaderName)
                {
                    item = tag;
                    break;
                }
            }

            if (item == null)
            {
                Console.WriteLine("Blam shader tag does not exist: " + shaderName);
                return false;
            }

            TagDeserializer deserializer = new TagDeserializer(BlamCache.Version);

            var blamContext = new CacheSerializationContext(CacheContext, BlamCache, item);
            var blamShader = deserializer.Deserialize<RenderMethod>(blamContext);

            

            var templateItem = BlamCache.IndexItems.Find(i =>
                i.ID == blamShader.ShaderProperties[0].Template.Index);

            blamContext = new CacheSerializationContext(CacheContext, BlamCache, templateItem);
            var template = deserializer.Deserialize<RenderMethodTemplate>(blamContext);

            for (var i = 0; i < template.ShaderMaps.Count; i++)
            {
                var entry = template.ShaderMaps[i].Name;

                var bitmItem = BlamCache.IndexItems.Find(j =>
                j.ID == blamShader.ShaderProperties[0].ShaderMaps[i].Bitmap.Index);
                Console.WriteLine(string.Format("{0:D2} {2}\t {1}", i, bitmItem, BlamCache.Strings.GetString(entry)));
            }

            return true;
        }
    }
}