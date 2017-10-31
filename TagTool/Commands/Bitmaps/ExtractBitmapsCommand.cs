﻿using BlamCore.Bitmaps;
using BlamCore.Cache;
using BlamCore.Serialization;
using BlamCore.TagDefinitions;
using System;
using System.Collections.Generic;
using System.IO;

namespace TagTool.Commands.Bitmaps
{
    class ExtractBitmapsCommand : Command
    {
        private GameCacheContext CacheContext { get; }

        public ExtractBitmapsCommand(GameCacheContext cacheContext) :
            base(CommandFlags.Inherit,
                
                "ExtractBitmaps",
                "Extract all bitmaps to a folder",
                
                "ExtractBitmaps <Folder>",
                
                "Extract all bitmap tags and any subimages to the given folder.\n" +
                "If the folder does not exist, it will be created.")
        {
            CacheContext = cacheContext;
        }

        public override object Execute(List<string> args)
        {
            if (args.Count != 1)
                return false;

            var outDir = args[0];
            Directory.CreateDirectory(outDir);

            Console.WriteLine("Loading resource caches...");

            var count = 0;

            using (var tagsStream = CacheContext.OpenTagCacheRead())
            {
                var extractor = new BitmapDdsExtractor(CacheContext);

                foreach (var tag in CacheContext.TagCache.Index.FindAllInGroup("bitm"))
                {
                    Console.Write("Extracting ");
                    TagPrinter.PrintTagShort(tag);

                #if !DEBUG
                    try
                    {
                #endif
                        var tagContext = new TagSerializationContext(tagsStream, CacheContext, tag);
                        var bitmap = CacheContext.Deserializer.Deserialize<Bitmap>(tagContext);
                        var ddsOutDir = outDir;

                        if (bitmap.Images.Count > 1)
                        {
                            ddsOutDir = Path.Combine(outDir, tag.Index.ToString("X8"));
                            Directory.CreateDirectory(ddsOutDir);
                        }

                        for (var i = 0; i < bitmap.Images.Count; i++)
                        {
                            var outPath = Path.Combine(ddsOutDir, ((bitmap.Images.Count > 1) ? i.ToString() : tag.Index.ToString("X8")) + ".dds");

                            using (var outStream = File.Open(outPath, FileMode.Create, FileAccess.Write))
                            {
                                extractor.ExtractDds(CacheContext.Deserializer, bitmap, i, outStream);
                            }
                        }
                        count++;
                #if !DEBUG
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: Failed to extract bitmap: " + ex.Message);
                    }
                #endif
                }
            }

            Console.WriteLine("Extracted {0} bitmaps.", count);

            return true;
        }
    }
}