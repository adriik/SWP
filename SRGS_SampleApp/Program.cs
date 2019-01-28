using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System.Globalization;
using System.IO;

namespace SRGS_SampleApp
{
    class Program
    {
        static SpeechSynthesizer ss;
        static SpeechRecognitionEngine sre;
        static bool done = false;
        static void Main(string[] args)
        {
            ss = new SpeechSynthesizer();
            ss.SetOutputToDefaultAudioDevice();
            ss.Speak("Witam w kalkulatorze");
            CultureInfo ci = new CultureInfo("pl-PL"); //ustawienie języka
            sre = new SpeechRecognitionEngine(ci); //powołanie engine rozpoznawania
            sre.SetInputToDefaultAudioDevice(); //ustawienie domyślnego urządzenia wejściowego
            sre.SpeechRecognized += Sre_SpeechRecognized;
            Grammar grammar = new Grammar("Grammars\\SimpleGrammar.xml", "rootRule");
            grammar.Enabled = true;
            sre.LoadGrammar(grammar);
            sre.RecognizeAsync(RecognizeMode.Multiple);
            while (done == false) {; } //pętla w celu uniknięcia zamknięcia programu
        }

        private static void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            float confidence = e.Result.Confidence;
            if(confidence>=0.7)
            {
                int first = Convert.ToInt32(e.Result.Semantics["first"].Value);
                int second = Convert.ToInt32(e.Result.Semantics["second"].Value);
                string operation = e.Result.Semantics["operation"].Value.ToString();
                if(operation=="suma")
                {
                    int sum = first + second;
                    ss.Speak("Wynik dodawania wynosi " + sum.ToString());
                }
                else if(operation=="roznica")
                {
                    int sub = first - second;
                    ss.Speak("Wynik odjmowania wynosi " + sub.ToString());
                }
                else if(operation=="iloczyn")
                {
                    int mult = first * second;
                    ss.Speak("Wynik mnożenia wynosi " + mult.ToString());
                }
            }
            else
            {
                ss.Speak("Proszę powtórzyć");
            }

        }
    }
}
