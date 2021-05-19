using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.IO;

namespace bourse
{
    class Program
    {
        static public int[] MonthDaysbix = new int[] {  31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31  };
        static public int[] MonthDays    = new int[] {  31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31  };
        static public float[] stockA = new float[7];
        static public float[] stockB = new float[7];
        static public int compteurDeLigneFait = 0;
        static public int compteurDeLigneTraduite = 0;
        static public int Date = 0;
        static public int Numligne = 4;
        static public string[] lines = System.IO.File.ReadAllLines(@"P:\Prog\Slam\bourse\Cac40-2.txt");        // lycée
        static public string BinaireFile = @"P:\Prog\Slam\bourse\monfichier.dat";
        //static public string[] lines = System.IO.File.ReadAllLines(@"D:\R.G\Bourse\CAC_40_1990_test.txt");  // chez moi
        //static public string BinaireFile = @"D:\R.G\Bourse\monfichier.dat";


        //static public DonneesBourse[] tabObjetTrier = new DonneesBourse[40];
        //public struct DonneesBourse
        //{
        //    public float DateConvertie { get; set; }
        //    public float Ouverture { get; set; }
        //    public float Eleve { get; set; }
        //    public float Faible { get; set; }
        //    public float Cloture { get; set; }
        //    public float ClotureAjuste { get; set; }
        //    public float Volume { get; set; }
        //}

        static int IsLeapYear(int Year)
        {
            if (((Year % 4) == 0) && (((Year % 100) != 0)) || ((Year % 400) == 0)) // vérifie si année bix
                return 1;
            else
                return 0;
        }
        static bool DoEncodeDate(int Year , int Month, int Day , ref float Date )
        {
            // 1 er méthode non retenue 

            //{
            //    Date = 365 * Year + Day;

            //    int monthSuccessif = 0;
            //    if (Month == 8) monthSuccessif = 1;

            //    Date = Date + ((Month + 1) / 2 + monthSuccessif);

            //    Console.WriteLine("M/8 : " + ((Month + 1) / 2 + monthSuccessif));
            //    return true;
            //}


            int[] tabMonth; // pointeur vers la variable global 

            //  Date = (30 + (Month + 1) / 2 + (Month / 8) - (Convert.ToInt32((Month > 2)) * 2))+Day;

            //   Date = Date + Day;
            //   Console.WriteLine(Date);


            if (IsLeapYear(Year) == 1)
            {
                tabMonth = MonthDaysbix;
            }
            else
            {
                tabMonth = MonthDays;
            }
            if ((Year >= 0) && (Year <= 9999) && (Month >= 1) && (Month <= 12) &&
                    (Day >= 1) && (Day <= tabMonth[Month - 1])) // on teste les cas d'erreur
            {
                for (int i = 1; i < Month; i++) // Pour chaque case du tableau de token 
                {
                    Day = Day + tabMonth[i - 1];
                }
                Year = Year - 1;
                Date = (Year * 365) + (Year % 4) - (Year % 100) + (Year % 400) + Day;
                Console.WriteLine("donnee " + (Year * 365) + " " + (Year % 4) + " " + (Year % 100) + " " + (Year % 400) + " " + Day);
                return true;
            }
            return false;

        }
        static int DecodeDate(int Date)
        {
            // 1 er méthode non retenue 
            //int J001 = 365;
            //int J004 = J001 * 4 + 1;
            //int J100 = J004 * 25 - 1;
            //int J400 = J100 * 4 + 1;

            //int A400 = (Date / J400) * 400;
            //Date = Date % J400;

            //int A100 = (Date / J100) * 100;
            //Date = Date % J100;

            //int A004 = (Date / J004) * 4;
            //Date = Date % J004;

            //int A001 = (Date / J001);
            //Date = Date % J001;

            Console.WriteLine("reste = date :" + Date);
            //         int Nba = A400 + A100 + A004 + A001;

            int Nba = Date / 365;
            Date = Date % 365;

            int[] tabMonth; // pointeur vers la variable global 
            if (IsLeapYear(Nba) == 1)
            {
                tabMonth = MonthDaysbix;
            }
            else
            {
                tabMonth = MonthDays;
            }
            int Nbm = 0;
            for (int i = 1; i < 12; i++) // Pour chaque case du tableau de token 
            {
                if (Date >= tabMonth[i - 1])
                {
                    Date = Date - tabMonth[i - 1];
                    Nbm++;
                }
                else
                {
                    continue;
                }
            }
            Console.WriteLine(Date + " " + (Nbm + 1) + " " + Nba);
            return Nba;
        }

        static bool DoEncodeDate2(int Year, int Month, int Days, ref int Date)
        { // Vas convertir la date qui est en 3 valeurs en une seul valeur 
            int[] tabAnnee;

            if (IsLeapYear(Year) == 1) tabAnnee = MonthDaysbix;
            else tabAnnee = MonthDays;

            if ((Year >= 1) && (Year <= 9999) && (Month >= 1) && (Month <= 12) && (Days >= 1) && (Days <= tabAnnee[Month - 1]))
            {
                int result = Year * 10000 + Month * 100 + Days;

                //Console.WriteLine("date convertie : " + result);                

                Date = result;

                return true;
            }

            Date = -1;

            return false;
        }
        static bool DoDecodeDate2(int convertedDate, ref int Date)
        {// permet de lire la date encoder par DoEncodeDate2
            if (convertedDate != -1)
            {
                int Year = convertedDate / 10000;
                convertedDate -= Year * 10000;
                int Month = convertedDate / 100;
                convertedDate -= Month * 100;
                int Days = convertedDate;

                Console.WriteLine("année décodée : " + Days + " " + Month + " " + Year);

                return true;
            }

            Date = -1;

            return false;
        }
       
        static public void TraduireFichier(ref string[] lines, ref int compteurDeLigneFait, ref int compteurDeLigneTraduite, ref int Date, BinaryWriter bw)
        {
            foreach (string line in lines) // vas lire tout les lignes du fichier txt 
            {
                compteurDeLigneFait++;
                if (line.Length == 0) //on vérifie si la ligne est vide 
                {
                    Console.WriteLine("Ligne vide");
                }
                else
                {
                    // affiche la ligne 
                    Console.WriteLine(line);
                    Console.WriteLine(" ");

                    char[] caracterSep = new char[] { ' ', (char)9 }; // char9 c'est la tabulation , tableau de caractère qui séparer les chiffres 
                    string[] lineSplit = line.Trim(' ').Split(caracterSep, StringSplitOptions.None); // vas enlever les espaces et ranger les tokens non vérifier dans un tableau 
                    string[] lineSplitTries = new string[9]; // création d'un tableau pour les tokens tries 
                    int count = 0;



                    for (int i = 0; i < lineSplit.Length; i++) // Pour chaque case du tableau de token 
                    {
                        if (lineSplit[i] != "") // on ne prend que les cases remplie 
                        {
                            lineSplitTries[count] = lineSplit[i];
                            count++;
                        }
                    }

                    if ((lineSplitTries[3] == "-") && (lineSplitTries[4] == "-") && (lineSplitTries[5] == "-") && (lineSplitTries[6] == "-") &&
                        (lineSplitTries[7] == "-") && (lineSplitTries[8] == "-")) // Vérification si jour férié   Cas Jour férié
                    {
                        continue; // On ne prend pas en compte les jours fériés 
                    }
                    else if (lineSplitTries[8] == "-") // Vérification du volume         Cas Volume vide
                    {
                        lineSplitTries[8] = "-1"; //Volume non indiquer on remplace par une valeur absude 
                    }


                    switch (lineSplitTries[1]) // remplacer les mois en caractère par des chiffres en string 
                    {
                        case "janv.":
                            lineSplitTries[1] = "01";
                            break;
                        case "févr.":
                            lineSplitTries[1] = "02";
                            break;
                        case "mars":
                            lineSplitTries[1] = "03";
                            break;
                        case "avr.":
                            lineSplitTries[1] = "04";
                            break;
                        case "mai":
                            lineSplitTries[1] = "05";
                            break;
                        case "juin":
                            lineSplitTries[1] = "06";
                            break;
                        case "juil.":
                            lineSplitTries[1] = "07";
                            break;
                        case "août":
                            lineSplitTries[1] = "08";
                            break;
                        case "sept.":
                            lineSplitTries[1] = "09";
                            break;
                        case "oct.":
                            lineSplitTries[1] = "10";
                            break;
                        case "nov.":
                            lineSplitTries[1] = "11";
                            break;
                        case "déc.":
                            lineSplitTries[1] = "12";
                            break;

                    }

                    for (int i = 0; i < lineSplitTries.Length; i++) // Pour chaque case du tableau de token 
                    {
                        Console.WriteLine(" " + lineSplitTries[i] + " - " + i); // affichage des ligne du tableau 
                    }



                    float[] result = new float[9]; // Initialisation d'un tableau de float qui vas recevoir les données 

                    for (int i = 0; i < lineSplitTries.Length; i++) // Pour chaque case du tableau de token 
                    {

                        result[i] = Convert.ToSingle(lineSplitTries[i]); // passer d'un tableau de String à un tableau de float 
                        Console.WriteLine(result[i]); 

                    }
                    Console.WriteLine("Encodage : " + DoEncodeDate2((int)result[2], (int)result[1], (int)result[0], ref Date));
                    Console.WriteLine("Date Encodée : " + Date);

                    //DonneesBourse donneCourant = new DonneesBourse
                    //{
                    //    DateConvertie = Date,
                    //    Ouverture = result[3],
                    //    Eleve = result[4],
                    //    Faible = result[5],
                    //    Cloture = result[6],
                    //    ClotureAjuste = result[7],
                    //    Volume = result[8]
                    //}; // création d'un objet bourse 
                    compteurDeLigneTraduite++;
                    // On écrit les données dans le fichier.bat sous forme binaire 

                    bw.Write((float)Date);
                    for (int i = 3; i < 9; i++)
                    {
                        bw.Write((float)result[i]);
                        Console.WriteLine("J'ai ecrit " + result[i]);
                    }

                    Console.WriteLine(sizeof(float) * 7);  // output: 8

                    //Console.WriteLine(tabObjetTrier.Length);
                    //tabObjetTrier.Append(donneCourant);
                    //Console.WriteLine(donneCourant.DateConvertie + " " + donneCourant.Ouverture + " " + donneCourant.Eleve + " " + donneCourant.Faible + " " +
                    //                  donneCourant.Cloture + " " + donneCourant.ClotureAjuste + " " + donneCourant.Volume);

                    Console.WriteLine("Decodage : " + DoDecodeDate2((int)Date, ref Date));
                    Console.WriteLine("Date encodée : " + Date);

                }

                

            }
            bw.Close(); // une seul fois
        }

        static public void LireBinaire(ref string BinaireFile, ref int compteurDeLigne, FileStream fs, BinaryReader br)
        {
            Console.WriteLine("Je suis dans le binaire ");
            int Nbroctect = compteurDeLigne * 28; // Permet de lire une ligne précise 
            fs = File.Open(BinaireFile, FileMode.Open); // Ouvrir le fichier .dat 
            fs.Seek(0, SeekOrigin.Begin); // Fixer l'origine de la lecture 
            br = new BinaryReader(fs);//traduie de binaire en donnée lisible 
          
            fs.Seek(0, SeekOrigin.Begin);
            while (fs.Position < Nbroctect)
            {
             
                Console.Write(br.ReadSingle() + " ");
                if(fs.Position%28 ==0)
                {
                    Console.WriteLine("");
                }
            }
            

            br.Close(); // une seul fois
            fs.Close();
        }
        static public void LireBinaireUneLigne( int Numligne , ref float [] stock)
        { 
            FileStream fs = File.Open(BinaireFile, FileMode.Open);
            fs.Seek(Numligne * 28, SeekOrigin.Begin);
            BinaryReader br = new BinaryReader(fs); //traduit de binaire en données lisibles 

            //Console.WriteLine("Lecture ligne " + numLigne);

            for (int i = 0; i < 7; i++)
            {
                stock[i] = br.ReadSingle();
            }

            fs.Close();
            br.Close();
        }
        static public void EchangerBinaireUneLigne(ref string BinaireFile, BinaryReader br , BinaryWriter bw)
        {
            Console.WriteLine("Je suis dans l'echange ");

            FileStream fs = File.Open(BinaireFile, FileMode.Open);
            long nbLignes = fs.Length / 28;
            fs.Close();
            for (int i1 = 0; i1 <= (nbLignes / 2) - 1; i1++)
            {
                int i2 = (int)nbLignes - i1 - 1;

                //Console.WriteLine("i1 : " + i1 + " <> i2 : " + i2);

                LireBinaireUneLigne(i1, ref stockA); LireBinaireUneLigne(i2, ref stockB); // Récupère les données de deux ligne
                EcrireLigne(i1, stockB); EcrireLigne(i2, stockA);// Les ecrits 
            }



        }
        static void EcrireLigne(int numLigne, float[] tabStock)
        {
            FileStream fs = File.Open(BinaireFile, FileMode.Open);
            fs.Seek(numLigne * 28, SeekOrigin.Begin);
            BinaryWriter bw = new BinaryWriter(fs);

            for (int i = 0; i < 7; i++) // Pour chaque token du tableau qui tien la ligne à écrire 
            {
                bw.Write(tabStock[i]);
            }

            fs.Close();
            bw.Close();
        }

        static void Main(string[] args)
        {
            // initialisation 
            
            BinaryReader br = null; // permet de lire 
            BinaryWriter bw = null; // Permet d'écrire
            FileStream fs = null;   // place le curseur pour le binaire
            

            
            bw = new BinaryWriter(File.Create(BinaireFile)); // vas traduire en binaire et stocker dans la variable visée 

            System.Console.WriteLine("Contenu de Cac40-2.txt = ");

            

            TraduireFichier(ref lines, ref compteurDeLigneFait, ref compteurDeLigneTraduite, ref Date , bw);
            //        compteurDeLigne = compteurDeLigne / 2;


            LireBinaire(ref BinaireFile, ref compteurDeLigneTraduite, fs, br);
            EchangerBinaireUneLigne(ref BinaireFile, br, bw);
            LireBinaire(ref BinaireFile, ref compteurDeLigneTraduite, fs, br);

            Console.WriteLine("\n-----------------");
            
            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit.");
            System.Console.ReadKey();

            /*
             * Partie commentaire
             * 
             * Le code contient des commentaires de solutions explorer mais qui ne sont pas utiliser comme pour les
             * objets qui devait contenir les différentes données de bourse mais un tableau était suffisant et
             * moins complexe à utiliser.
             * 
             * Pour la date j'ai préférer utiliser des multiple de 10 qui me permette de voir directement une valeur
             * et d'en savoir la date. 
             */

        


    }
    }
}
