﻿using System.Configuration;
using DecidedlyShared.Constants;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using SObject = StardewValley.Object;

namespace SmartCursor
{
    public class BreakableEntity
    {
        private SmartCursorConfig config;
        public Vector2 Tile { get; }
        public BreakableType Type { get; }

        /// <summary>
        /// A breakable entity in the world.
        /// </summary>
        /// <param name="feature"></param>
        /// <param name="config">The config required to make correct decisions on breakability.</param>
        public BreakableEntity(TerrainFeature feature, SmartCursorConfig config)
        {
            this.config = config;
            this.Type = this.GetBreakableType(feature);
            this.Tile = feature.currentTileLocation;
        }

        /// <summary>
        /// A breakable entity in the world.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="config">The config required to make correct decisions on breakability.</param>
        public BreakableEntity(SObject obj, SmartCursorConfig config)
        {
            this.config = config;
            this.Type = this.GetBreakableType(obj);
            this.Tile = obj.TileLocation;
        }

        /// <summary>
        /// A breakable entity in the world.
        /// </summary>
        /// <param name="clump"></param>
        /// <param name="config">The config required to make correct decisions on breakability.</param>
        public BreakableEntity(ResourceClump clump, SmartCursorConfig config)
        {
            this.config = config;
            this.Type = this.GetBreakableType(clump);
            this.Tile = clump.tile.Value;
        }

        /// <summary>
        /// Returns the BreakableType of the SObject passed in.
        /// </summary>
        /// <returns>The <see cref="BreakableType"/> of the <see cref="SObject"/> passed in.</returns>
        public BreakableType GetBreakableType(SObject obj)
        {
            if (obj.Name.Equals("Stone"))
                return BreakableType.Pickaxe;

            if (obj.Name.Equals("Twig"))
                return BreakableType.Axe;

            if (obj.Name.Equals("Artifact Spot"))
                return BreakableType.Hoe;

            return BreakableType.NotAllowed;
        }

        /// <summary>
        /// Returns the BreakableType of the TerrainFeature passed in.
        /// </summary>
        /// <returns>The <see cref="BreakableType"/> of the <see cref="TerrainFeature"/> passed in.</returns>
        public BreakableType GetBreakableType(TerrainFeature tf)
        {
            if (tf is Tree tree)
            {
                if (tree.growthStage.Value < 5)
                    return this.config.AllowTargetingBabyTrees ? BreakableType.Axe : BreakableType.NotAllowed;
                if (tree.tapped.Value)
                    return this.config.AllowTargetingTappedTrees ? BreakableType.Axe : BreakableType.NotAllowed;
                if (tree.health.Value <= 0)
                    return BreakableType.NotAllowed;

                return BreakableType.Axe;
            }

            if (tf is GiantCrop)
                return this.config.AllowTargetingGiantCrops ? BreakableType.Axe : BreakableType.NotAllowed;

            if (tf is ResourceClump)
                return BreakableType.Pickaxe;

            return BreakableType.NotAllowed;
        }

        // <summary>
        /// Returns the BreakableType of the ResourceClump passed in.
        /// </summary>
        /// <returns>The <see cref="BreakableType"/> of the <see cref="ResourceClump"/> passed in.</returns>
        public BreakableType GetBreakableType(ResourceClump clump)
        {
            if (clump is GiantCrop)
                return this.config.AllowTargetingGiantCrops ? BreakableType.Axe : BreakableType.NotAllowed;

            return BreakableType.Pickaxe;
        }
    }
}
