using System;
using ONI_Common.Json;

namespace ONI_Common
{
    public class ModSettings
    {
        public string modPath;

        private JsonManager _jsonManager = new JsonManager();

        public virtual void Write()
        {
            try
            {
                this._jsonManager.Serialize(this, this.modPath);
            }
            catch (Exception e)
            {
                State.Logger.Log(this.modPath + " state save failed.");
                State.Logger.Log(e);
            }
        }

        public ColorMode ColorMode { get; set; } = ColorMode.Json;

        public bool Enabled { get; set; } = true;



        public bool LegacyTileColorHandling { get; set; } = false;

        // gas overlay

        public bool ShowBuildingsAsWhite { get; set; }

        public bool ShowMissingElementColorInfos { get; set; }

        public bool ShowMissingTypeColorOffsets { get; set; }

    }
}
