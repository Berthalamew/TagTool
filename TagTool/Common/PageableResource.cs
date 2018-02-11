using TagTool.Cache;
using TagTool.Serialization;
using System;

namespace TagTool.Common
{
    /// <summary>
    /// A reference to a resource used by a tag.
    /// This is treated by the serialization system as a special type of tag element.
    /// </summary>
    [TagStructure(Size = 0x40, MaxVersion = CacheVersion.Halo3ODST)]
    [TagStructure(Size = 0x6C, MaxVersion = CacheVersion.HaloOnline106708)]
    [TagStructure(Size = 0x70, MinVersion = CacheVersion.HaloOnline235640)]
    public class PageableResource
    {
        /// <summary>
        /// The <see cref="RawPage"/> of the <see cref="PageableResource"/>.
        /// </summary>
        public RawPage Page;

        /// <summary>
        /// The <see cref="TagResource"/> of the <see cref="PageableResource"/>.
        /// </summary>
        public TagResource Resource;

        /// <summary>
        /// Gets the location of the resource by checking its location flags.
        /// </summary>
        /// <returns>The resource's location.</returns>
        /// <exception cref="InvalidOperationException">The resource does not have a location flag set</exception>
        public ResourceLocation GetLocation()
        {
            if (Page.OldFlags != 0)
            {
                if ((Page.OldFlags & OldRawPageFlags.InResources) != 0)
                    return ResourceLocation.Resources;
                if ((Page.OldFlags & OldRawPageFlags.InTextures) != 0)
                    return ResourceLocation.Textures;
                if ((Page.OldFlags & OldRawPageFlags.InTexturesB) != 0)
                    return ResourceLocation.TexturesB;
                if ((Page.OldFlags & OldRawPageFlags.InAudio) != 0)
                    return ResourceLocation.Audio;
                if ((Page.OldFlags & OldRawPageFlags.InResourcesB) != 0)
                    return ResourceLocation.ResourcesB;
            }
            else if (Page.NewFlags != 0)
            {
                // FIXME: haxhaxhax, maybe we should just have separate types for the old and new reference layouts?
                if ((Page.NewFlags & NewRawPageFlags.InResources) != 0)
                    return ResourceLocation.Resources;
                if ((Page.NewFlags & NewRawPageFlags.InTextures) != 0)
                    return ResourceLocation.Textures;
                if ((Page.NewFlags & NewRawPageFlags.InTexturesB) != 0)
                    return ResourceLocation.TexturesB;
                if ((Page.NewFlags & NewRawPageFlags.InAudio) != 0)
                    return ResourceLocation.Audio;
                if ((Page.NewFlags & NewRawPageFlags.InResourcesB) != 0)
                    return ResourceLocation.ResourcesB;
                if ((Page.NewFlags & NewRawPageFlags.InRenderModels) != 0)
                    return ResourceLocation.RenderModels;
                if ((Page.NewFlags & NewRawPageFlags.InLightmaps) != 0)
                    return ResourceLocation.Lightmaps;
            }
            throw new InvalidOperationException("The resource does not have a location flag set");
        }

        /// <summary>
        /// Changes the location of the resource by changing its location flags.
        /// </summary>
        /// <param name="newLocation">The new location.</param>
        /// <exception cref="System.ArgumentException">Unsupported resource location</exception>
        public void ChangeLocation(ResourceLocation newLocation)
        {
            Page.OldFlags &= ~OldRawPageFlags.LocationMask;
            Page.NewFlags &= ~NewRawPageFlags.LocationMask;
            switch (newLocation)
            {
                case ResourceLocation.Resources:
                    Page.OldFlags |= OldRawPageFlags.InResources;
                    Page.NewFlags |= NewRawPageFlags.InResources;
                    break;
                case ResourceLocation.Textures:
                    Page.OldFlags |= OldRawPageFlags.InTextures;
                    Page.NewFlags |= NewRawPageFlags.InTextures;
                    break;
                case ResourceLocation.TexturesB:
                    Page.OldFlags |= OldRawPageFlags.InTexturesB;
                    Page.NewFlags |= NewRawPageFlags.InTexturesB;
                    break;
                case ResourceLocation.Audio:
                    Page.OldFlags |= OldRawPageFlags.InAudio;
                    Page.NewFlags |= NewRawPageFlags.InAudio;
                    break;
                case ResourceLocation.ResourcesB:
                    Page.OldFlags |= OldRawPageFlags.InResourcesB;
                    Page.NewFlags |= NewRawPageFlags.InResourcesB;
                    break;
                case ResourceLocation.RenderModels:
                    Page.NewFlags |= NewRawPageFlags.InRenderModels;
                    break;
                case ResourceLocation.Lightmaps:
                    Page.NewFlags |= NewRawPageFlags.InLightmaps;
                    break;
                default:
                    throw new ArgumentException("Unsupported resource location");
            }
        }

        /// <summary>
        /// Disables the resource's checksum by changing its location flags.
        /// </summary>
        public void DisableChecksum()
        {
            Page.OldFlags &= ~(OldRawPageFlags.UseChecksum | OldRawPageFlags.UseChecksum2);
            Page.NewFlags &= ~NewRawPageFlags.UseChecksum;
        }
    }
    
    /// <summary>
    /// Resource location constants used by <see cref="PageableResource.GetLocation"/>.
    /// </summary>
    public enum ResourceLocation
    {
        /// <summary>
        /// The resource is in resources.dat.
        /// </summary>
        Resources,

        /// <summary>
        /// The resource is in textures.dat.
        /// </summary>
        Textures,

        /// <summary>
        /// The resource is in textures_b.dat.
        /// </summary>
        TexturesB,

        /// <summary>
        /// The resource is in audio.dat.
        /// </summary>
        Audio,

        /// <summary>
        /// The resource is in resources_b.dat.
        /// </summary>
        ResourcesB,

        /// <summary>
        /// The resource is in render_models.dat.
        /// </summary>
        RenderModels,

        /// <summary>
        /// The resource is in lightmaps.dat.
        /// </summary>
        Lightmaps,
    }
}
