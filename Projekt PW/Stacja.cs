using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

/*
•	Różne rodzaje paliw
•	Zamykanie stacji na pewien czas
•	M – liczba stanowisk z dystrybutorami(na stanowisku może być kilka różnych dystrybutorów) 
•	K – liczba kas = 3
•	N – maksymalna liczba samochodów, które mogą przebywać na stacji (jedno stanowisko – jedno auto) =16

 */

namespace Projekt_PW
{

    public partial class Stacja : Form
    {
        private Boolean trybsymulacji;
        private Boolean trybpauzy;
        private static List<Klient> Klienci;
        public static System.Threading.Semaphore stanowiskobenz = new System.Threading.Semaphore(12,12);               // Semafor do stanowisk benzynowych
        public static System.Threading.Semaphore stanowiskoon = new System.Threading.Semaphore(16, 16);               // Semafor do stanowisk on
        public static System.Threading.Semaphore stanowiskolpg = new System.Threading.Semaphore(4, 4);               // Semafor do stanowisk lpg
        public static System.Threading.Semaphore kasa = new System.Threading.Semaphore(3, 3);                       // Semafor dla kas
        public static System.Threading.Semaphore dostepstanowisko = new System.Threading.Semaphore(1, 1);          // Semafor binarny dla dostepu do tablicy stanowisk
        public static System.Threading.Semaphore dostepkasa = new System.Threading.Semaphore(1, 1);               // Semafor binarny dostepu do kas

        public static int[] pozycja = { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };  // 1-oznacza ze dostepny  0-oznacza ze zajety    DYSTRYBUTORY
        public static int[] kasakontrol = {1, 1, 1};  // 1-oznacza ze dostepna  0-oznacza ze zajeta    KASY FISKALNE
        private System.Threading.Timer timer;
        private int l;                              // licznik klientow

        public Stacja()
        {
            InitializeComponent();
            Klienci = new List<Klient>();
            l = 0;
            trybsymulacji = false;
            trybpauzy = false;

            //timer = new System.Threading.Timer(new TimerCallback(addclient), null, 1000, 1000);

        }

        public void addclient(Object st)
        {
            Klient klient;

            if (st == null)
            {
                klient = new Klient();
            }
            else  klient = new Klient((int)st);

            Klienci.Add(klient);
            l++;
            Console.WriteLine("dodano klienta z paliwem typu: {0}",klient.paliwo);
            Thread watek = new Thread(klient.tankuj);
            watek.IsBackground = true;
            watek.Name = l.ToString();
            Console.WriteLine(Klienci.Count());
            watek.Start();
        }
        public void removeclient(Klient kl)
        {
             /*
            Program.Orlen.richTextBox1.Select(0, richTextBox1.GetFirstCharIndexFromLine(1));
            Program.Orlen.richTextBox1.SelectedText = "";
             */
              Klienci.RemoveAt(Klienci.IndexOf(kl));
            Console.WriteLine("usunieto klienta! {0}",Thread.CurrentThread.Name);
            Console.WriteLine(Klienci.Count());
        }

        public void infocome(int fuel)
        {
            if (Program.Orlen.richTextBox1.InvokeRequired)
                Program.Orlen.richTextBox1.Invoke(new Action(delegate ()
                {
                    if (fuel == 1)
                    {
                        Program.Orlen.richTextBox1.AppendText("Przybyl klient z silnikiem benzynowym, nr klienta: ");
                        Program.Orlen.richTextBox1.AppendText(l.ToString());
                        Program.Orlen.richTextBox1.AppendText("\n");
                    }
                    if (fuel == 2)
                    {
                        Program.Orlen.richTextBox1.AppendText("Przybyl klient z silnikiem diesla, nr klienta: ");
                        Program.Orlen.richTextBox1.AppendText(l.ToString());
                        Program.Orlen.richTextBox1.AppendText("\n");
                    }
                    if (fuel == 3)
                    {
                        Program.Orlen.richTextBox1.AppendText("Przybyl klient z silnikiem na LPG, nr klienta: ");
                        Program.Orlen.richTextBox1.AppendText(l.ToString());
                        Program.Orlen.richTextBox1.AppendText("\n");

                    }

                }
                ));
            
            else
                Program.Orlen.richTextBox1.AppendText("Przybylklient");

        }

        public void infotank(string nazwa)
        {
            if (Program.Orlen.richTextBox1.InvokeRequired)
                Program.Orlen.richTextBox1.Invoke(new Action(delegate ()
                {
                    Program.Orlen.richTextBox1.AppendText("Klient nr: ");
                    Program.Orlen.richTextBox1.AppendText(nazwa);
                    Program.Orlen.richTextBox1.AppendText(" Zatankowal ");
                    Program.Orlen.richTextBox1.AppendText("\n");
                }
                ));
            else
                Program.Orlen.richTextBox1.AppendText("Zatankowal");
        }

        public void infopay(string nazwa)
        {
            if (Program.Orlen.richTextBox1.InvokeRequired)
                Program.Orlen.richTextBox1.Invoke(new Action(delegate ()
                {
                    Program.Orlen.richTextBox1.AppendText("Klient nr: ");
                    Program.Orlen.richTextBox1.AppendText(nazwa);
                    Program.Orlen.richTextBox1.AppendText(" Zaplacil ");
                    Program.Orlen.richTextBox1.AppendText("\n");
                }
                ));
            else
                Program.Orlen.richTextBox1.AppendText("Zaplacil");
        }

        public static int getliczbaklientow()
        {
            return Klienci.Count();
        }
        public static int getliczbaklientowon()
        {
            var k =
            from kl in Klienci
            where kl.paliwo == 2
            select kl;

            return k.Count();
        }

        public static int getliczbaklientowlpg()
        {
            var k =
            from kl in Klienci
            where kl.paliwo == 3
            select kl;

            return k.Count();
        }

        public static int getliczbaklientowbenz()
        {
            var k =
            from kl in Klienci
            where kl.paliwo == 1
            select kl;

            return k.Count();
        }
        public void changetextzajety(int p)
        {
            switch(p)
            {
                case 1:
                   if(textBox17.InvokeRequired)
                        textBox17.Invoke(new Action(delegate ()
                        {
                            textBox17.BackColor = System.Drawing.Color.Red;
                            textBox17.Text="ZAJETY";
                        }
                      ));
                    else
                        textBox17.Text = "ZAJETY";
                    break;

                case 2:
                    if (textBox18.InvokeRequired)
                        textBox18.Invoke(new Action(delegate ()
                        {
                            textBox18.BackColor = System.Drawing.Color.Red;
                            textBox18.Text= "ZAJETY";
                        }
                      ));
                    else
                        textBox18.Text = "ZAJETY";
                    break;

                case 3:
                    if (textBox19.InvokeRequired)
                        textBox19.Invoke(new Action(delegate ()
                        {
                            textBox19.BackColor = System.Drawing.Color.Red;
                            textBox19.Text = "ZAJETY";
                        }
                      ));
                    else
                        textBox19.Text = "ZAJETY";
                    break;

                case 4:
                    if (textBox20.InvokeRequired)
                        textBox20.Invoke(new Action(delegate ()
                        {
                            textBox20.BackColor = System.Drawing.Color.Red;
                            textBox20.Text = "ZAJETY";
                        }
                      ));
                    else
                        textBox20.Text = "ZAJETY";
                    break;

                case 5:
                    if (textBox21.InvokeRequired)
                        textBox21.Invoke(new Action(delegate ()
                        {
                            textBox21.BackColor = System.Drawing.Color.Red;
                            textBox21.Text = "ZAJETY";
                        }
                      ));
                    else
                        textBox21.Text = "ZAJETY";
                    break;

                case 6:
                    if (textBox22.InvokeRequired)
                        textBox22.Invoke(new Action(delegate ()
                        {
                            textBox22.BackColor = System.Drawing.Color.Red;
                            textBox22.Text = "ZAJETY";
                        }
                      ));
                    else
                        textBox22.Text = "ZAJETY";
                    break;

                case 7:
                    if (textBox23.InvokeRequired)
                        textBox23.Invoke(new Action(delegate ()
                        {
                            textBox23.BackColor = System.Drawing.Color.Red;
                            textBox23.Text = "ZAJETY";
                        }
                      ));
                    else
                        textBox23.Text = "ZAJETY";
                    break;

                case 8:
                    if (textBox24.InvokeRequired)
                        textBox24.Invoke(new Action(delegate ()
                        {
                            textBox24.BackColor = System.Drawing.Color.Red;
                            textBox24.Text = "ZAJETY";
                        }
                      ));
                    else
                        textBox24.Text = "ZAJETY";
                    break;

                case 9:
                    if (textBox25.InvokeRequired)
                        textBox25.Invoke(new Action(delegate ()
                        {
                            textBox25.BackColor = System.Drawing.Color.Red;
                            textBox25.Text = "ZAJETY";
                        }
                      ));
                    else
                        textBox25.Text = "ZAJETY";
                    break;

                case 10:
                    if (textBox26.InvokeRequired)
                        textBox26.Invoke(new Action(delegate ()
                        {
                            textBox26.BackColor = System.Drawing.Color.Red;
                            textBox26.Text = "ZAJETY";
                        }
                      ));
                    else
                        textBox26.Text = "ZAJETY";
                    break;

                case 11:
                    if (textBox27.InvokeRequired)
                        textBox27.Invoke(new Action(delegate ()
                        {
                            textBox27.BackColor = System.Drawing.Color.Red;
                            textBox27.Text = "ZAJETY";
                        }
                      ));
                    else
                        textBox27.Text = "ZAJETY";
                    break;

                case 12:
                    if (textBox28.InvokeRequired)
                        textBox28.Invoke(new Action(delegate ()
                        {
                            textBox28.BackColor = System.Drawing.Color.Red;
                            textBox28.Text = "ZAJETY";
                        }
                      ));
                    else
                        textBox28.Text = "ZAJETY";
                    break;

                case 13:
                    if (textBox29.InvokeRequired)
                        textBox29.Invoke(new Action(delegate ()
                        {
                            textBox29.BackColor = System.Drawing.Color.Red;
                            textBox29.Text = "ZAJETY";
                        }
                      ));
                    else
                        textBox29.Text = "ZAJETY";
                    break;

                case 14:
                    if (textBox30.InvokeRequired)
                        textBox30.Invoke(new Action(delegate ()
                        {
                            textBox30.BackColor = System.Drawing.Color.Red;
                            textBox30.Text = "ZAJETY";
                        }
                      ));
                    else
                        textBox30.Text = "ZAJETY";
                    break;

                case 15:
                    if (textBox31.InvokeRequired)
                        textBox31.Invoke(new Action(delegate ()
                        {
                            textBox31.BackColor = System.Drawing.Color.Red;
                            textBox31.Text = "ZAJETY";
                        }
                      ));
                    else
                        textBox31.Text = "ZAJETY";
                    break;

                case 16:
                    if (textBox32.InvokeRequired)
                        textBox32.Invoke(new Action(delegate ()
                        {
                            textBox32.BackColor = System.Drawing.Color.Red;
                            textBox32.Text = "ZAJETY";
                        }
                      ));
                    else
                        textBox32.Text = "ZAJETY";
                    break;

                case 17:
                    if (textBox33.InvokeRequired)
                        textBox33.Invoke(new Action(delegate ()
                        {
                            textBox33.BackColor = System.Drawing.Color.Red;
                            textBox33.Text = "ZAJETY";
                        }
                      ));
                    else
                        textBox33.Text = "ZAJETY";
                    break;

                case 18:
                    if (textBox34.InvokeRequired)
                        textBox34.Invoke(new Action(delegate ()
                        {
                            textBox34.BackColor = System.Drawing.Color.Red;
                            textBox34.Text = "ZAJETY";
                        }
                      ));
                    else
                        textBox34.Text = "ZAJETY";
                    break;

                case 19:
                    if (textBox35.InvokeRequired)
                        textBox35.Invoke(new Action(delegate ()
                        {
                            textBox35.BackColor = System.Drawing.Color.Red;
                            textBox35.Text = "ZAJETY";
                        }
                      ));
                    else
                        textBox35.Text = "ZAJETY";
                    break;
            }
        }


        public void changetextwolny(int p)
        {
            switch (p)
            {
                case 1:
                    if (textBox17.InvokeRequired)
                        textBox17.Invoke(new Action(delegate ()
                        {
                            textBox17.BackColor = SystemColors.Window;
                            textBox17.Text = "WOLNY";
                        }
                      ));
                    else
                        textBox17.Text = "WOLNY";
                    break;

                case 2:
                    if (textBox18.InvokeRequired)
                        textBox18.Invoke(new Action(delegate ()
                        {
                            textBox18.BackColor = SystemColors.Window;
                            textBox18.Text = "WOLNY";
                        }
                      ));
                    else
                        textBox18.Text = "WOLNY";
                    break;

                case 3:
                    if (textBox19.InvokeRequired)
                        textBox19.Invoke(new Action(delegate ()
                        {
                            textBox19.BackColor = SystemColors.Window;
                            textBox19.Text = "WOLNY";
                        }
                      ));
                    else
                        textBox19.Text = "WOLNY";
                    break;

                case 4:
                    if (textBox20.InvokeRequired)
                        textBox20.Invoke(new Action(delegate ()
                        {
                            textBox20.BackColor = SystemColors.Window;
                            textBox20.Text = "WOLNY";
                        }
                      ));
                    else
                        textBox20.Text = "WOLNY";
                    break;

                case 5:
                    if (textBox21.InvokeRequired)
                        textBox21.Invoke(new Action(delegate ()
                        {
                            textBox21.BackColor = SystemColors.Window;
                            textBox21.Text = "WOLNY";
                        }
                      ));
                    else
                        textBox21.Text = "WOLNY";
                    break;

                case 6:
                    if (textBox22.InvokeRequired)
                        textBox22.Invoke(new Action(delegate ()
                        {
                            textBox22.BackColor = SystemColors.Window;
                            textBox22.Text = "WOLNY";
                        }
                      ));
                    else
                        textBox22.Text = "WOLNY";
                    break;

                case 7:
                    if (textBox23.InvokeRequired)
                        textBox23.Invoke(new Action(delegate ()
                        {
                            textBox23.BackColor = SystemColors.Window;
                            textBox23.Text = "WOLNY";
                        }
                      ));
                    else
                        textBox23.Text = "WOLNY";
                    break;

                case 8:
                    if (textBox24.InvokeRequired)
                        textBox24.Invoke(new Action(delegate ()
                        {
                            textBox24.BackColor = SystemColors.Window;
                            textBox24.Text = "WOLNY";
                        }
                      ));
                    else
                        textBox24.Text = "WOLNY";
                    break;

                case 9:
                    if (textBox25.InvokeRequired)
                        textBox25.Invoke(new Action(delegate ()
                        {
                            textBox25.BackColor = SystemColors.Window;
                            textBox25.Text = "WOLNY";
                        }
                      ));
                    else
                        textBox25.Text = "WOLNY";
                    break;

                case 10:
                    if (textBox26.InvokeRequired)
                        textBox26.Invoke(new Action(delegate ()
                        {
                            textBox26.BackColor = SystemColors.Window;
                            textBox26.Text = "WOLNY";
                        }
                      ));
                    else
                        textBox26.Text = "WOLNY";
                    break;

                case 11:
                    if (textBox27.InvokeRequired)
                        textBox27.Invoke(new Action(delegate ()
                        {
                            textBox27.BackColor = SystemColors.Window;
                            textBox27.Text = "WOLNY";
                        }
                      ));
                    else
                        textBox27.Text = "WOLNY";
                    break;

                case 12:
                    if (textBox28.InvokeRequired)
                        textBox28.Invoke(new Action(delegate ()
                        {
                            textBox28.BackColor = SystemColors.Window;
                            textBox28.Text = "WOLNY";
                        }
                      ));
                    else
                        textBox28.Text = "WOLNY";
                    break;

                case 13:
                    if (textBox29.InvokeRequired)
                        textBox29.Invoke(new Action(delegate ()
                        {
                            textBox29.BackColor = SystemColors.Window;
                            textBox29.Text = "WOLNY";
                        }
                      ));
                    else
                        textBox29.Text = "WOLNY";
                    break;

                case 14:
                    if (textBox30.InvokeRequired)
                        textBox30.Invoke(new Action(delegate ()
                        {
                            textBox30.BackColor = SystemColors.Window;
                            textBox30.Text = "WOLNY";
                        }
                      ));
                    else
                        textBox30.Text = "WOLNY";
                    break;

                case 15:
                    if (textBox31.InvokeRequired)
                        textBox31.Invoke(new Action(delegate ()
                        {
                            textBox31.BackColor = SystemColors.Window;
                            textBox31.Text = "WOLNY";
                        }
                      ));
                    else
                        textBox31.Text = "WOLNY";
                    break;

                case 16:
                    if (textBox32.InvokeRequired)
                        textBox32.Invoke(new Action(delegate ()
                        {
                            textBox32.BackColor = SystemColors.Window;
                            textBox32.Text = "WOLNY";
                        }
                      ));
                    else
                        textBox32.Text = "WOLNY";
                    break;

                case 17:
                    if (textBox33.InvokeRequired)
                        textBox33.Invoke(new Action(delegate ()
                        {
                            textBox33.BackColor = SystemColors.Window;
                            textBox33.Text = "WOLNY";
                        }
                      ));
                    else
                        textBox33.Text = "WOLNY";
                    break;

                case 18:
                    if (textBox34.InvokeRequired)
                        textBox34.Invoke(new Action(delegate ()
                        {
                            textBox34.BackColor = SystemColors.Window;
                            textBox34.Text = "WOLNY";
                        }
                      ));
                    else
                        textBox34.Text = "WOLNY";
                    break;

                case 19:
                    if (textBox35.InvokeRequired)
                        textBox35.Invoke(new Action(delegate ()
                        {
                            textBox35.BackColor = SystemColors.Window;
                            textBox35.Text = "WOLNY";
                        }
                      ));
                    else
                        textBox35.Text = "WOLNY";
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            addclient(1);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            addclient(2);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            addclient(3);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (trybsymulacji == false)
            {
                trybsymulacji = true;
                this.timer = new System.Threading.Timer(new TimerCallback(addclient), null, 1000, 1000);
            }
            else
            {
                trybsymulacji = false;
                this.timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (trybsymulacji == true)
            {
                trybsymulacji = false;
                this.timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            if(trybpauzy == false)
            {
                button5.Text = "Otworz Stacje";
                trybpauzy = true;
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
            }
            else
            if (trybpauzy == true)
            {
                button5.Text = "Zamknij Stacje";
                trybpauzy = false;
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                button4.Enabled = true;
            }



        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
