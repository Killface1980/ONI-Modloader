using ONI_Common;

namespace ModLoader
{

    public abstract class Mod
    {
        public string ModName;

        public Mod(string configName)
        {
            this.ModName = configName;

            this.TryLoadConfigFromFile(configName);
        }

        private void TryLoadConfigFromFile(string s)
        {
            //  var json =
        }

        public ModSettings modSettings;

        public virtual void WriteSettings()
        {
            if (this.modSettings != null)
            {
                this.modSettings.Write();
            }
        }
    }

}