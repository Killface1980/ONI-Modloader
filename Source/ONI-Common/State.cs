namespace ONI_Common
{
    using JetBrains.Annotations;
    using ONI_Common.Data;
    using ONI_Common.Json;
    using System.Collections.Generic;
    using UnityEngine;
    using Logger = ONI_Common.IO.Logger;

    public static class State
    {
        [NotNull]
        private static readonly JsonFileLoader JsonLoader = new JsonFileLoader(new JsonManager(), Logger);



        // TODO: load from file instead
        [NotNull]
        public static readonly List<string> TileNames = new List<string>
                                                        {
                                                        "Tile",
                                                        "MeshTile",
                                                        "InsulationTile",
                                                        "GasPermeableMembrane",
                                                        "TilePOI",
                                                        "PlasticTile",
                                                        "MetalTile"
                                                        };



        private static Logger _logger;





        [NotNull]
        public static Logger Logger => _logger ?? (_logger = new IO.Logger(Paths.MaterialColorLogFileName));
    }
}