public interface IUređaj
{
 void ispis();
}

public interface ITablet
{
 void ispis();
}

public interface IMobitel
{
 void ispis();
}

class Uređaj
{
 public string marka;
 public string model;
 public float cijena;
 public int godizd;
 public string boja;

 public Uređaj(string m, string mm, float c, int g, string b)
 {
  marka = m;
  model = mm;
  cijena = c;
  godizd = g;
  boja = b;
 }

 public virtual void ispis()
 {
  Console.WriteLine($"ovo je uređaj marke {marka} i cijena mu je {cijena} EUR");
  Console.WriteLine($"uređaj je boje {boja}, a godina izdanja mu je {godizd}");
  Console.WriteLine($"model se zove: {model}");
 }
}

class Tablet : Uređaj,ITablet
 {
  public float dijag;
  public bool olovka;
  public int kapacitetbat;

  public Tablet(string m, string mm, float c, int g, string b, float d, bool o, int kb) : base(m, mm, c, g, b)
  {
   dijag = d;
   olovka = o;
   kapacitetbat = kb;
  }

  public override void ispis()
  {
   base.ispis();
   if (olovka == true)
   {
    Console.WriteLine("Ima olovku.");
   }
   else
   {
    Console.WriteLine("Nema Olovku");
   }
  } 
 }
 class Mobitel : Uređaj, IMobitel
 {
  private string brojtel;
  public int brojkamera;
  public bool dualsim;

  public Mobitel(string m, string mm, float c, int g, string b, string bt, int bk, bool d) : base(m, mm, c, g, b)
  {
   brojtel = bt;
   brojkamera = bk;
   dualsim = d;
  }

  public string Broj
  {
   get { return brojtel; }
   set { brojtel = value; }
  }

  public override void ispis()
  {
   base.ispis();
   if (dualsim == true)
   {
    Console.WriteLine("ima dualsim");
   }
   else
   {
    Console.WriteLine("nema dualsim");
   }
   Console.WriteLine("brojtel: " + brojtel);
   Console.WriteLine("brojkamera: " + brojkamera);
  }
  
 }

//nevezane klase za drugi primjer kao 
 class Oblici
 {
  virtual public void povrsina()
  {
   Console.WriteLine("Povrsina: ");
  }
 }

 class Pravokutnik : Oblici
 {
  public float a, b;
  public Pravokutnik(float a, float b)
  {
   this.a = a;
   this.b = b;
  }

  public override void povrsina()
  {
   float p = a * b;
   Console.WriteLine($"Povrsina:{p} ");
  }
 }
 class Program
 {
  static void Main(string[] args)
  {
   
   //drugi dioooo
   Pravokutnik p = new Pravokutnik(3, 4);
   p.povrsina(); 
   
   Console.WriteLine("unos tableta: ");
   Console.Write("marka: ");
   string tMarka = Console.ReadLine();
   Console.Write("model: ");
   string tModel = Console.ReadLine();
   Console.Write("cijena (EUR): ");
   float tCijena = float.Parse(Console.ReadLine());
   Console.Write("godina izdanja: ");
   int tGodina = int.Parse(Console.ReadLine());
   Console.Write("boja: ");
   string tBoja = Console.ReadLine();
   Console.Write("dijagonala ekrana (inči): ");
   float tDijagonala = float.Parse(Console.ReadLine());
   Console.Write("ima li olovku (true/false): ");
   bool tOlovka = bool.Parse(Console.ReadLine());
   Console.Write("kapacitet baterije (mAh): ");
   int tBaterija = int.Parse(Console.ReadLine());


   Tablet mojTablet = new Tablet(tMarka, tModel, tCijena, tGodina, tBoja, tDijagonala, tOlovka, tBaterija);
   mojTablet.ispis();

   mojTablet.boja = "crvena";
   mojTablet.ispis();
   
   Console.WriteLine("unos mobitela: ");
   Console.Write("marka: ");
   string mMarka = Console.ReadLine();
   Console.Write("model: ");
   string mModel = Console.ReadLine();
   Console.Write("cijena (EUR): ");
   float mCijena = float.Parse(Console.ReadLine());
   Console.Write("godina izdanja: ");
   int mGodina = int.Parse(Console.ReadLine());
   Console.Write("boja: ");
   string mBoja = Console.ReadLine();
   Console.Write("dualsim (true/false): ");
   bool mDualsim = bool.Parse(Console.ReadLine());
   Console.Write("brojtel: ");
   string mBrojtel = Console.ReadLine();
   Console.Write("broj kamera: ");
   int mBrojka = int.Parse(Console.ReadLine());
   Mobitel mojMobitel = new Mobitel( 
    mMarka,
    mModel,
    mCijena,
    mGodina,
    mBoja,
    mBrojtel,
    mBrojka,
    mDualsim
   );
   mojMobitel.ispis();
  }
 }

