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
 }

 class Tablet : Uređaj
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

 class Mobitel : Uređaj
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
 }

 class Program
 {
  static void Main(string[] args)
  {
   Console.WriteLine("unos tableta: ");
   Console.Write("Unesite marku: ");
   string tMarka = Console.ReadLine();
   Console.Write("Unesite model: ");
   string tModel = Console.ReadLine();
   Console.Write("Unesite cijenu (EUR): ");
   float tCijena = float.Parse(Console.ReadLine());
   Console.Write("Unesite godinu izdanja: ");
   int tGodina = int.Parse(Console.ReadLine());
   Console.Write("Unesite boju: ");
   string tBoja = Console.ReadLine();
   Console.Write("Unesite dijagonalu ekrana (inči): ");
   float tDijagonala = float.Parse(Console.ReadLine());
   Console.Write("Ima li olovku (true/false): ");
   bool tOlovka = bool.Parse(Console.ReadLine());
   Console.Write("Unesite kapacitet baterije (mAh): ");
   int tBaterija = int.Parse(Console.ReadLine());


   Tablet mojTablet = new Tablet(tMarka, tModel, tCijena, tGodina, tBoja, tDijagonala, tOlovka, tBaterija);
   mojTablet.ispis();



  }
 }
}
