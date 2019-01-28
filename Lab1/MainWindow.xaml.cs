using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace Lab1
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        LinkedList<float> buforLiczb;
        LinkedList<string> operatory;
        LinkedList<float> liczby;
        private readonly BackgroundWorker worker = new BackgroundWorker();

        static SpeechSynthesizer ss;
        static SpeechRecognitionEngine sre;
        static bool done = false;
        bool flagaWykonania = false;

        float wynik;
        private bool flagaLiczby;

        public MainWindow()
        {
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
            InitializeComponent();

            buforLiczb = new LinkedList<float>();
            liczby = new LinkedList<float>();
            operatory = new LinkedList<string>();
            wynik = 0;
            label.Content = "";
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
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

        private void worker_RunWorkerCompleted(object sender,
                                       RunWorkerCompletedEventArgs e)
        {
            //update ui once worker complete his work
        }

        private void ResetFlagi()
        {
            if (flagaWykonania == true)
            {
                ButtonC.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                flagaWykonania = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ResetFlagi();
            buforLiczb.AddLast(7);
            label.Content += "7";
            flagaLiczby = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ResetFlagi();
            buforLiczb.AddLast(8);
            label.Content += "8";
            flagaLiczby = true;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ResetFlagi();
            buforLiczb.AddLast(9);
            label.Content += "9";
            flagaLiczby = true;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ResetFlagi();
            buforLiczb.AddLast(4);
            label.Content += "4";
            flagaLiczby = true;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            ResetFlagi();
            buforLiczb.AddLast(5);
            label.Content += "5";
            flagaLiczby = true;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            ResetFlagi();
            buforLiczb.AddLast(6);
            label.Content += "6";
            flagaLiczby = true;
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            ResetFlagi();
            buforLiczb.AddLast(1);
            label.Content += "1";
            flagaLiczby = true;
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            ResetFlagi();
            buforLiczb.AddLast(2);
            label.Content += "2";
            flagaLiczby = true;
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            ResetFlagi();
            buforLiczb.AddLast(3);
            label.Content += "3";
            flagaLiczby = true;
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            ResetFlagi();
            buforLiczb.AddLast(0);
            label.Content += "0";
            flagaLiczby = true;
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            if (flagaLiczby) { 
                ResetFlagi();
                Float2float();
                operatory.AddLast("/");
                label.Content += " / ";
                flagaLiczby = false;
            }
            else
            {
                ss.SpeakAsync("Najpierw musi być liczba");
            }
}

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            if (flagaLiczby) { 
                ResetFlagi();
                Float2float();
                operatory.AddLast("*");
                label.Content += " * ";
                flagaLiczby = false;
            }
            else
            {
                ss.SpeakAsync("Najpierw musi być liczba");
            }
}

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            if (flagaLiczby) { 
                ResetFlagi();
                Float2float();
                operatory.AddLast("-");
                label.Content += " - ";
                flagaLiczby = false;
            }
            else
            {
                ss.SpeakAsync("Najpierw musi być liczba");
            }
}

        private void Button_Click_13(object sender, RoutedEventArgs e)
        {
            if (flagaLiczby) {
                ResetFlagi();
                Float2float();
                operatory.AddLast("+");
                label.Content += " + ";
                flagaLiczby = false;
            }
            else
            {
                ss.SpeakAsync("Najpierw musi być liczba");
            }
        }

        private void Button_Click_14(object sender, RoutedEventArgs e)
        {
            if (flagaLiczby)
            {
                flagaLiczby = false;
                Float2float();
                int i = 0;
                wynik = liczby.First();
                foreach (var item in operatory)
                {

                    switch (item)
                    {
                        case "/":
                            wynik = (wynik / liczby.ElementAt(i + 1));
                            break;
                        case "*":
                            wynik = (wynik * liczby.ElementAt(i + 1));
                            break;
                        case "+":
                            wynik = (wynik + liczby.ElementAt(i + 1));
                            break;
                        case "-":
                            wynik = (wynik - liczby.ElementAt(i + 1));
                            break;
                    }
                    i++;
                }

                if (operatory.First().Equals("+"))
                {
                    label.Content += " = " + wynik.ToString("0");
                    if (wynik == 0)
                    {
                        ss.SpeakAsync("Wynik dodawania wynosi zero");
                    }
                    else
                    {
                        ss.SpeakAsync("Wynik dodawania wynosi " + wynik.ToString("0"));
                    }
                }
                else if (operatory.First().Equals("-"))
                {
                    label.Content += " = " + wynik.ToString("0");
                    if (wynik < 0)
                    {
                        ss.SpeakAsync("Wynik odjmowania wynosi minus" + wynik.ToString("0"));

                    }
                    else if (wynik == 0)
                    {
                        ss.SpeakAsync("Wynik odjmowania wynosi zero");
                    }
                    else
                    {
                        ss.SpeakAsync("Wynik odjmowania wynosi " + wynik.ToString("0"));
                    }
                }
                else if (operatory.First().Equals("*"))
                {
                    label.Content += " = " + wynik.ToString("0");
                    if (wynik == 0)
                    {
                        ss.SpeakAsync("Wynik mnożenia wynosi zero");
                    }
                    else
                    {
                        ss.SpeakAsync("Wynik mnożenia wynosi " + wynik.ToString("0"));
                    }
                }
                else if (operatory.First().Equals("/"))
                {
                    if (liczby.ElementAt(1) == 0)
                    {
                        label.Content += " = " + "Nie można dzielić przez 0";
                        ss.SpeakAsync("Nie można dzielić przez zero");
                    }
                    else if (liczby.ElementAt(0) == 0)
                    {
                        label.Content += " = " + wynik.ToString("0.00");
                        ss.SpeakAsync("Wynik dzielenia wynosi zero");
                    }
                    else
                    {
                        label.Content += " = " + wynik.ToString("0.00");
                        String [] dzielenie = wynik.ToString("0.00").Split(',');
                        if (!dzielenie[0].Equals("0"))
                        {
                            ss.SpeakAsync("Wynik wynosi " + dzielenie[0] + " przecinek " + dzielenie[1]);
                        }
                        else{
                            ss.SpeakAsync("Wynik wynosi " + "zero przecinek " + dzielenie[1]);
                        }
                    }
                }

                //ss.SpeakAsync("Wynik wynosi " + wynik.ToString());
                flagaWykonania = true;
            }
        }

        private void Button_Click_15(object sender, RoutedEventArgs e)
        {
            operatory.Clear();
            buforLiczb.Clear();
            liczby.Clear();
            wynik = 0;
            label.Content = "";
        }

        private void Float2float()
        {

                String liczba = "";
                foreach (var item in buforLiczb)
                {
                    liczba += item.ToString("0");

                }
                buforLiczb.Clear();

                liczby.AddLast(Int32.Parse(liczba));

        }


        public void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            float confidence = e.Result.Confidence;

            if (confidence >= 0.4)
            {

                int first = Convert.ToInt32(e.Result.Semantics["first"].Value);

                //int second = Convert.ToInt32(e.Result.Semantics["second"].Value);
                //string operation = e.Result.Semantics["operation"].Value.ToString();
                

                switch (first)
                {
                    case 1:
                        Button1.Dispatcher.Invoke(new Action(() =>
                        {
                            Button1.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
                            Button1.Background = brush;
                            ColorAnimation anima = new ColorAnimation(Colors.Blue, new Duration(TimeSpan.FromSeconds(0.5)));
                            anima.AutoReverse = true;
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, anima);
                        }));
                        break;
                    case 2:
                        Button2.Dispatcher.Invoke(new Action(() =>
                        {
                            Button2.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
                            Button2.Background = brush;
                            ColorAnimation anima = new ColorAnimation(Colors.Blue, new Duration(TimeSpan.FromSeconds(0.5)));
                            anima.AutoReverse = true;
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, anima);
                        }));
                        break;

                    case 3:
                        Button3.Dispatcher.Invoke(new Action(() =>
                        {
                            Button3.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
                            Button3.Background = brush;
                            ColorAnimation anima = new ColorAnimation(Colors.Blue, new Duration(TimeSpan.FromSeconds(0.5)));
                            anima.AutoReverse = true;
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, anima);
                        }));
                        break;

                    case 4:
                        Button4.Dispatcher.Invoke(new Action(() =>
                        {
                            Button4.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
                            Button4.Background = brush;
                            ColorAnimation anima = new ColorAnimation(Colors.Blue, new Duration(TimeSpan.FromSeconds(0.5)));
                            anima.AutoReverse = true;
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, anima);
                        }));
                        break;

                    case 5:
                        Button5.Dispatcher.Invoke(new Action(() =>
                        {
                            Button5.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
                            Button5.Background = brush;
                            ColorAnimation anima = new ColorAnimation(Colors.Blue, new Duration(TimeSpan.FromSeconds(0.5)));
                            anima.AutoReverse = true;
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, anima);
                        }));
                        break;

                    case 6:
                        Button6.Dispatcher.Invoke(new Action(() =>
                        {
                            Button6.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
                            Button6.Background = brush;
                            ColorAnimation anima = new ColorAnimation(Colors.Blue, new Duration(TimeSpan.FromSeconds(0.5)));
                            anima.AutoReverse = true;
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, anima);
                        }));
                        break;

                    case 7:
                        Button7.Dispatcher.Invoke(new Action(() =>
                        {
                            Button7.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
                            Button7.Background = brush;
                            ColorAnimation anima = new ColorAnimation(Colors.Blue, new Duration(TimeSpan.FromSeconds(0.5)));
                            anima.AutoReverse = true;
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, anima);
                        }));
                        break;

                    case 8:
                        Button8.Dispatcher.Invoke(new Action(() =>
                        {
                            Button8.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
                            Button8.Background = brush;
                            ColorAnimation anima = new ColorAnimation(Colors.Blue, new Duration(TimeSpan.FromSeconds(0.5)));
                            anima.AutoReverse = true;
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, anima);
                        }));
                        break;

                    case 9:
                        Button9.Dispatcher.Invoke(new Action(() =>
                        {
                            Button9.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
                            Button9.Background = brush;
                            ColorAnimation anima = new ColorAnimation(Colors.Blue, new Duration(TimeSpan.FromSeconds(0.5)));
                            anima.AutoReverse = true;
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, anima);
                        }));
                        break;

                    case 0:
                        Button0.Dispatcher.Invoke(new Action(() =>
                        {
                            Button0.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
                            Button0.Background = brush;
                            ColorAnimation anima = new ColorAnimation(Colors.Blue, new Duration(TimeSpan.FromSeconds(0.5)));
                            anima.AutoReverse = true;
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, anima);
                        }));
                        break;
                    case 10:
                        ButtonRowne.Dispatcher.Invoke(new Action(() =>
                        {
                            ButtonRowne.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
                            ButtonRowne.Background = brush;
                            ColorAnimation anima = new ColorAnimation(Colors.Blue, new Duration(TimeSpan.FromSeconds(0.5)));
                            anima.AutoReverse = true;
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, anima);
                        }));
                        break;
                    case 11:
                        ButtonPlus.Dispatcher.Invoke(new Action(() =>
                        {
                            ButtonPlus.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
                            ButtonPlus.Background = brush;
                            ColorAnimation anima = new ColorAnimation(Colors.Blue, new Duration(TimeSpan.FromSeconds(0.5)));
                            anima.AutoReverse = true;
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, anima);

                        }));
                        break;
                    case 12:
                        ButtonMinus.Dispatcher.Invoke(new Action(() =>
                        {
                            ButtonMinus.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
                            ButtonMinus.Background = brush;
                            ColorAnimation anima = new ColorAnimation(Colors.Blue, new Duration(TimeSpan.FromSeconds(0.5)));
                            anima.AutoReverse = true;
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, anima);
                        }));
                        break;
                    case 13:
                        ButtonRazy.Dispatcher.Invoke(new Action(() =>
                        {
                            ButtonRazy.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
                            ButtonRazy.Background = brush;
                            ColorAnimation anima = new ColorAnimation(Colors.Blue, new Duration(TimeSpan.FromSeconds(0.5)));
                            anima.AutoReverse = true;
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, anima);
                        }));
                        break;
                    case 14:
                        ButtonPrzez.Dispatcher.Invoke(new Action(() =>
                        {
                            ButtonPrzez.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
                            ButtonPrzez.Background = brush;
                            ColorAnimation anima = new ColorAnimation(Colors.Blue, new Duration(TimeSpan.FromSeconds(0.5)));
                            anima.AutoReverse = true;
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, anima);
                        }));
                        break;
                    case 15:
                        ButtonC.Dispatcher.Invoke(new Action(() =>
                        {
                            ButtonC.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                            SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 221, 221, 221));
                            ButtonC.Background = brush;
                            ColorAnimation anima = new ColorAnimation(Colors.Blue, new Duration(TimeSpan.FromSeconds(0.5)));
                            anima.AutoReverse = true;
                            brush.BeginAnimation(SolidColorBrush.ColorProperty, anima);
                        }));
                        break;
                    default:
                        break;
                }

                

                //if (operation == "plus")
                //{
                //    ButtonPlus.Dispatcher.Invoke(new Action(() =>
                //    {
                //        ButtonPlus.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                //    }));

                //}
                //else if (operation == "minus")
                //{
                //    ButtonMinus.Dispatcher.Invoke(new Action(() =>
                //    {
                //        ButtonMinus.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                //    }));

                //}
                //else if (operation == "razy")
                //{
                //    ButtonRazy.Dispatcher.Invoke(new Action(() =>
                //    {
                //        ButtonRazy.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                //    }));
                //}
                //else if (operation == "przez")
                //{
                //    ButtonPrzez.Dispatcher.Invoke(new Action(() =>
                //    {
                //        ButtonPrzez.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                //    }));


                //}

                //switch (second)
                //{
                //    case 1:
                //        Button1.Dispatcher.Invoke(new Action(() =>
                //        {
                //            Button1.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                //        }));
                //        break;
                //    case 2:
                //        Button2.Dispatcher.Invoke(new Action(() =>
                //        {
                //            Button2.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                //        }));
                //        break;

                //    case 3:
                //        Button3.Dispatcher.Invoke(new Action(() =>
                //        {
                //            Button3.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                //        }));
                //        break;

                //    case 4:
                //        Button4.Dispatcher.Invoke(new Action(() =>
                //        {
                //            Button4.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                //        }));
                //        break;

                //    case 5:
                //        Button5.Dispatcher.Invoke(new Action(() =>
                //        {
                //            Button5.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                //        }));
                //        break;

                //    case 6:
                //        Button6.Dispatcher.Invoke(new Action(() =>
                //        {
                //            Button6.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                //        }));
                //        break;

                //    case 7:
                //        Button7.Dispatcher.Invoke(new Action(() =>
                //        {
                //            Button7.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                //        }));
                //        break;

                //    case 8:
                //        Button8.Dispatcher.Invoke(new Action(() =>
                //        {
                //            Button8.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                //        }));
                //        break;

                //    case 9:
                //        Button9.Dispatcher.Invoke(new Action(() =>
                //        {
                //            Button9.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                //        }));
                //        break;

                //    case 0:
                //        Button0.Dispatcher.Invoke(new Action(() =>
                //        {
                //            Button0.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                //        }));
                //        break;
                //    default:
                //        break;
                //}
                //ButtonRowne.Dispatcher.Invoke(new Action(() =>
                //{
                //    ButtonRowne.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                //}));
            }
            else
            {
                ss.Speak("Proszę powtórzyć");
            }
        }
    }
}
