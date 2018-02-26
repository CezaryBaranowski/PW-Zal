using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Projekt_PW
{
    public class Klient
    {
        public int paliwo      { get; set; }             // 1-benzyna,2-on,3-lpg
        public int poz       { get; set; }             // stanowisko na ktorym sie zatrzymal klient
        public int pozk      { get; set; }             // kasa na ktorej sie zatrzymal klient
        private Boolean zatankowany;                        // czy zatankowany

        public Klient()
        {
            paliwo = (new Random().Next(1,4));
            zatankowany = false;
            poz = -1;
            pozk = -1;
        }
        public Klient(int f)
        {
            paliwo = f;
            zatankowany = false;
            poz = -1;
            pozk = -1;
        }
        public Boolean czyZatankowany()
        {
            return this.zatankowany;
        }

        public void tankuj()
        {
            //Console.WriteLine("Przybyl klient z paliwem: {0}", fuel);
            Program.Orlen.infocome(paliwo);

            if (this.paliwo==1)                                            // jesli benzyna
            {
                int p = 4;                                                   // pomocniczy iterator do sprawdzania wolnych stanowisk, 4 bo dla benzyny to pierwsze stanowisko

                Stacja.stanowiskobenz.WaitOne();
                Stacja.stanowiskoon.WaitOne();
                Stacja.dostepstanowisko.WaitOne();
               // Console.WriteLine("Zaraz zatankuje benzyne! {0}", Thread.CurrentThread.Name);

                while (poz==-1)
                {
                    if (Stacja.pozycja[p] == 0) p++;
                    else
                    {
                        Stacja.pozycja[p] = 0;
                        poz = p;
                    }
                }
                Program.Orlen.changetextzajety(poz+1);
                Stacja.dostepstanowisko.Release();
                System.Threading.Thread.Sleep(10000);                // tankuje...
                //Console.WriteLine("Zatankowalem! {0}", Thread.CurrentThread.Name);
                Program.Orlen.infotank(Thread.CurrentThread.Name);
                pay();                                              // place...
                Program.Orlen.infopay(Thread.CurrentThread.Name);
                Stacja.dostepstanowisko.WaitOne();
                Stacja.pozycja[poz] = 1;
                Program.Orlen.changetextwolny(poz+1);
                Stacja.dostepstanowisko.Release();
                Console.WriteLine("Wyjechalem! {0}", Thread.CurrentThread.Name);
                Stacja.stanowiskobenz.Release();
                Stacja.stanowiskoon.Release();
            }
            else
            if (this.paliwo == 2)                                         // jesli diesel
            {
                int p = 0;
                if (Stacja.getliczbaklientowon() + Stacja.getliczbaklientowbenz() > 12)
                {
                    Stacja.stanowiskoon.WaitOne();
                    Stacja.stanowiskolpg.WaitOne();
                    Stacja.dostepstanowisko.WaitOne();
                    Console.WriteLine("Zaraz zatankuje ON! {0}", Thread.CurrentThread.Name);
                    while (poz == -1)
                    {
                        if (Stacja.pozycja[p] == 0) p++;
                        else
                        {
                            Stacja.pozycja[p] = 0;
                            poz = p;
                        }
                    }
                    Program.Orlen.changetextzajety(poz+1);
                    Stacja.dostepstanowisko.Release();
                    System.Threading.Thread.Sleep(10000);                // tankuje...
                    //Console.WriteLine("Zatankowalem! {0}", Thread.CurrentThread.Name);
                    Program.Orlen.infotank(Thread.CurrentThread.Name);
                    pay();                                              // place...
                    Program.Orlen.infopay(Thread.CurrentThread.Name);
                    Stacja.dostepstanowisko.WaitOne();
                    Stacja.pozycja[poz] = 1;
                    Program.Orlen.changetextwolny(poz+1);
                    Stacja.dostepstanowisko.Release();
                    //Console.WriteLine("Wyjechalem! {0}", Thread.CurrentThread.Name);
                    Stacja.stanowiskoon.Release();
                    Stacja.stanowiskolpg.Release();
                }
                else
                {
                    p = 4;
                    Stacja.stanowiskoon.WaitOne();
                    Stacja.stanowiskobenz.WaitOne();
                    Stacja.dostepstanowisko.WaitOne();
                    Console.WriteLine("Zaraz zatankuje ON! {0}", Thread.CurrentThread.Name);
                    while (poz == -1)
                    {
                        if (Stacja.pozycja[p] == 0) p++;
                        else
                        {
                            Stacja.pozycja[p] = 0;
                            poz = p;
                        }
                    }
                    Program.Orlen.changetextzajety(poz+1);
                    Stacja.dostepstanowisko.Release();
                    System.Threading.Thread.Sleep(10000);                // tankuje
                    //Console.WriteLine("Zatankowalem! {0}", Thread.CurrentThread.Name);
                    Program.Orlen.infotank(Thread.CurrentThread.Name);
                    pay();                                              // place...
                    Program.Orlen.infopay(Thread.CurrentThread.Name);
                    Stacja.dostepstanowisko.WaitOne();
                    Stacja.pozycja[poz] = 1;
                    Program.Orlen.changetextwolny(poz+1);
                    Stacja.dostepstanowisko.Release();
                    Console.WriteLine("Wyjechalem! {0}", Thread.CurrentThread.Name);
                    Stacja.stanowiskoon.Release();
                    Stacja.stanowiskobenz.Release();
                }
            }
            else
            if (this.paliwo == 3)                                         // jesli lpg
            {
                int p = 0;
                Stacja.stanowiskolpg.WaitOne();
                Stacja.stanowiskoon.WaitOne();
                Stacja.dostepstanowisko.WaitOne();
                Console.WriteLine("Zaraz zatankuje gaz! {0}", Thread.CurrentThread.Name);
                while (poz == -1)
                {
                    if (Stacja.pozycja[p] == 0) p++;
                    else
                    {
                        Stacja.pozycja[p] = 0;
                        poz = p;
                    }
                }
                Program.Orlen.changetextzajety(poz+1);
                Stacja.dostepstanowisko.Release();
                System.Threading.Thread.Sleep(10000);                // tankuje
                //Console.WriteLine("Zatankowalem! {0}", Thread.CurrentThread.Name);
                Program.Orlen.infotank(Thread.CurrentThread.Name);
                pay();                                              // place...
                Program.Orlen.infopay(Thread.CurrentThread.Name);
                Stacja.dostepstanowisko.WaitOne();
                Stacja.pozycja[poz] = 1;
                Program.Orlen.changetextwolny(poz+1);
                Stacja.dostepstanowisko.Release();
                Console.WriteLine("Wyjechalem! {0}", Thread.CurrentThread.Name);
                Stacja.stanowiskolpg.Release();
                Stacja.stanowiskoon.Release();
            }

            this.zatankowany = true;
            //Console.WriteLine(Thread.CurrentThread.Name + "Ide placic!");
               Console.WriteLine("Usuwam klienta! {0}", Thread.CurrentThread.Name);
               Program.Orlen.removeclient(this);
               Console.WriteLine("teraz usuwam watek! {0}", Thread.CurrentThread.Name);
               Thread.CurrentThread.Abort();
        }

        public void pay()
        {
            int k = 0;
            foreach (int i in Stacja.kasakontrol)
            {
                Console.WriteLine(i);
            }
            Stacja.kasa.WaitOne();
            Stacja.dostepkasa.WaitOne();
            while(pozk == -1)
            {
                if (Stacja.kasakontrol[k] == 0) k++;
                else
                {
                    Stacja.kasakontrol[k] = 0;
                    pozk = k;
                }
            }
  
            Program.Orlen.changetextzajety(pozk + 17);
            Stacja.dostepkasa.Release();
            // Console.WriteLine(Thread.CurrentThread.Name + "Zaraz zaplace!");
            Console.WriteLine("Klient placi {0}", Thread.CurrentThread.Name);
            System.Threading.Thread.Sleep(5000);                                    // Placi...
            Stacja.dostepkasa.WaitOne();
            Stacja.kasakontrol[pozk] = 1;
            Program.Orlen.changetextwolny(pozk+17);
            Stacja.dostepkasa.Release();
            Stacja.kasa.Release();
            Console.WriteLine("Zaplacilem! {0}", Thread.CurrentThread.Name);
        }

    }
}
