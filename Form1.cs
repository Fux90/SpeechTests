using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestAudio
{
    public partial class Form1 : Form
    {
        #region ERROR_BY_VOICE_CULTURE
        const string BASE_ERROR_MSG = "Attention! No string provided!";
        private static Dictionary<CultureInfo, string> messageByCulture = new Dictionary<CultureInfo, string>()
        {
            {CultureInfo.CreateSpecificCulture("en-US"), BASE_ERROR_MSG },
            {CultureInfo.CreateSpecificCulture("en-GB"), BASE_ERROR_MSG },
            {CultureInfo.CreateSpecificCulture("it-IT"), "Attenzione! Nessuna stringa fornita!" },
        };

        private static string ErrorMessageByCulture(CultureInfo cultureInfo)
        {
            try
            {
                return messageByCulture[cultureInfo];
            }
            catch(KeyNotFoundException keyNotFound)
            {
                return BASE_ERROR_MSG;
            }
        }

        #endregion

        SpeechSynthesizer synth;
        BackgroundWorker readerWorker;

        private InstalledVoice CurrentVoice
        {
            get
            {
                return  cmbVoices.SelectedItem == null 
                        ? null 
                        : (cmbVoices.SelectedItem as VoiceInList).InstalledVoice;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeSynth();
            FillCmbVoices();
        }

        private void InitializeSynth()
        {
            synth = new SpeechSynthesizer();
            synth.SetOutputToDefaultAudioDevice();

            readerWorker = new BackgroundWorker();
            readerWorker.DoWork += ReaderWorker_DoWork;
            readerWorker.RunWorkerCompleted += ReaderWorker_RunWorkerCompleted;

            voiceParametersControl1.SpeechSynthesizer = synth;
        }

        private void ReaderWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btnRead.Enabled = true;
        }

        private void ReaderWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var arguments = (object[])e.Argument;

            var synth = arguments[0] as SpeechSynthesizer;
            var textToBeRead = arguments[1] as string;

            synth.Speak(textToBeRead);
        }

        private void FillCmbVoices()
        {
            cmbVoices.DisplayMember = "Label";
            cmbVoices.ValueMember = "VoiceInfo";

            foreach( InstalledVoice installedVoice in synth.GetInstalledVoices() )
            {
                cmbVoices.Items.Add(new VoiceInList( installedVoice ));
            }

            if ( cmbVoices.Items.Count > 0 )
            {
                cmbVoices.SelectedIndex = 0;
            }
        }

        private void ReadString(string text)
        {
            var notValidText = text == null || text == "";
            if ( notValidText )
            {
                ReadString( ErrorMessageByCulture( CurrentVoice.VoiceInfo.Culture ) );
            }
            else
            {
                if (!readerWorker.IsBusy)
                {
                    readerWorker.RunWorkerAsync(new object[]
                    {
                    synth,
                    text,
                    });
                }
            }
        }

        private void WriteCurrentVoiceInfo(ComboBox cmbVoices)
        {
            var currentVoice = CurrentVoice;

            if (currentVoice != null)
            {
                var info = currentVoice.VoiceInfo;

                var strDescription = new StringBuilder();

                strDescription.AppendLine(" Name:          " + info.Name);
                strDescription.AppendLine(" Culture:       " + info.Culture);
                strDescription.AppendLine(" Age:           " + info.Age);
                strDescription.AppendLine(" Gender:        " + info.Gender);
                strDescription.AppendLine(" Description:   " + info.Description);
                strDescription.AppendLine(" ID:            " + info.Id);

                txtVoiceInfo.Text = strDescription.ToString();
            }
            else
            {
                txtVoiceInfo.Text = string.Empty;
            }
        }

        private void SetSynthVoice(InstalledVoice currentVoice)
        {
            if (currentVoice != null)
            {
                synth.SelectVoice(CurrentVoice.VoiceInfo.Name);
            }
        }

        private void btnRead_Click(object sender, EventArgs e)
        {
            btnRead.Enabled = false;
            ReadString(txtSentenceToBeRead.Text);
        }

        private void cmbVoices_SelectedValueChanged(object sender, EventArgs e)
        {
            WriteCurrentVoiceInfo(cmbVoices);
            SetSynthVoice(CurrentVoice);
        }
    }
}
