using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Synthesis;

namespace TestAudio
{
    public delegate void ModificationOnSynth(SpeechSynthesizer synth);

    public partial class VoiceParametersControl : UserControl
    {
        public event EventHandler<SynthEventArgs> SynthChanged;

        private SpeechSynthesizer speechSynthesizer;
        public SpeechSynthesizer SpeechSynthesizer
        {
            get
            {
                return speechSynthesizer;
            }

            set
            {
                speechSynthesizer = value;
                OnSynthChanged(new SynthEventArgs( SpeechSynthesizer ));
            }
        }

        private void OnSynthChanged(SynthEventArgs e)
        {
            var tmp = SynthChanged;
            if (tmp != null)
            {
                tmp(this, e);
            }

            var synth = e.SpeechSynthesizer;

            if (synth != null)
            {
                trackBarRate.Value = synth.Rate;
                trackBarVolume.Value = synth.Volume;
            }
        }

        public VoiceParametersControl()
        {
            InitializeComponent();

            trackBarRate.Value = trackBarRate.Minimum;
            trackBarRate.Tag = new ControlTag()
            {
                Label = labelRateValue,
                ModificationOnSynth = (s) => { s.Rate = trackBarRate.Value; },
            };

            trackBarVolume.Value = trackBarVolume.Minimum;
            trackBarVolume.Tag = new ControlTag()
            {
                Label = labelVolumeValue,
                ModificationOnSynth = (s) => { s.Volume = trackBarVolume.Value; },
            };
    }

        private void trackBar_ValueChanged(object sender, EventArgs e)
        {
            var tBar = sender as TrackBar;
            var tag = tBar.Tag;

            if (tag != null && tag is ControlTag)
            {
                var controlTag = tag as ControlTag;

                controlTag.Label.Text = tBar.Value.ToString();
                controlTag?.ModificationOnSynth(SpeechSynthesizer);
            }
        }
    }

    class ControlTag
    {
        public Label Label { get; set; }
        public ModificationOnSynth ModificationOnSynth { get; set; }
    }

    public class SynthEventArgs : EventArgs
    {
        public SpeechSynthesizer SpeechSynthesizer { get; private set; }

        public SynthEventArgs(SpeechSynthesizer speechSynthesizer)
        {
            SpeechSynthesizer = speechSynthesizer;
        }
    }
}
