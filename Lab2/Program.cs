using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2
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
            ss.Speak("Witam");
            CultureInfo ci = new CultureInfo("pl-PL"); //ustawienie języka
            sre = new SpeechRecognitionEngine(ci); //powołanie engine rozpoznawania
            sre.SetInputToDefaultAudioDevice(); //ustawienie domyślnego urządzenia wejściowego
            sre.SpeechRecognized += Sre_SpeechRecognized;
            //Grammar gr = new Grammar(CreateGrammar());
            //gr.Enabled = true;

            //sre.LoadGrammarAsync(gr);
            //sre.LoadGrammar(gr);

            Choices liczby = new Choices();
            liczby.Add("1");
            liczby.Add("2");
            liczby.Add("3");
            liczby.Add("4");
            liczby.Add("5");
            liczby.Add("6");
            liczby.Add("7");
            liczby.Add("8");
            liczby.Add("9");
            liczby.Add("0");

            Choices operaotrzy = new Choices();
            operaotrzy.Add("plus");
            operaotrzy.Add("minus");
            operaotrzy.Add("razy");
            operaotrzy.Add("przez");


            GrammarBuilder builder = new GrammarBuilder();
            builder.Append(liczby);
            builder.Append(operaotrzy);
            builder.Append(liczby);

            Grammar grammar = new Grammar(builder);
            grammar.Enabled = true;
            sre.LoadGrammar(grammar);
            sre.RecognizeAsync(RecognizeMode.Multiple);
            while (done == false) {; } //pętla w celu uniknięcia zamknięcia programu
        }

        private static void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            float confidence = e.Result.Confidence;

            if (confidence >= 0.2)
            {
                string[] tab = e.Result.Text.Split(' ');
                int first = Convert.ToInt32(tab[0]);
                int second = Convert.ToInt32(tab[2]);
                string operation = tab[1];

                if (operation == "plus")
                {
                    int sum = first + second;
                    ss.Speak("Wynik dodawania wynosi " + sum.ToString());
                }
                else if (operation == "minus")
                {
                    if(second > first)
                    {
                        int sub = second - first;
                        ss.Speak("Wynik odjmowania wynosi minus" + sub.ToString());
                    }
                    else
                    {
                        int sub = first - second;
                        ss.Speak("Wynik odjmowania wynosi " + sub.ToString());
                    }
                    
                    
                }
                else if (operation == "razy")
                {
                    if (first == 0 || second == 0)
                    {
                        ss.Speak("Wynik mnożenia wynosi zero");
                    }
                    else
                    {
                        int mult = first * second;
                        ss.Speak("Wynik mnożenia wynosi " + mult.ToString());
                    }
                }
                else if (operation == "przez")
                {
                    if (first == 0)
                    {
                        ss.Speak("Wynik dzielenia wynosi zero");
                    }
                    else
                    {


                        if (second != 0)
                        {

                            int mult = first / second;

                            ss.Speak("Wynik dzielenia wynosi " + mult.ToString(CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            ss.Speak("Nie można dzielić przez zero");
                        }
                    }
                }
            }
            else
            {
                ss.Speak("Proszę powtórzyć");
            }

        }
    }

}
