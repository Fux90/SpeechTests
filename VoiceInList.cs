using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace TestAudio
{
    public class VoiceInList
    {
        public InstalledVoice InstalledVoice { get; private set; }
        public string Label
        {
            get
            {
                return InstalledVoice.VoiceInfo.Name;
            }
        }

        public VoiceInList(InstalledVoice installedVoice)
        {
            InstalledVoice = installedVoice;
        }
    }
}
